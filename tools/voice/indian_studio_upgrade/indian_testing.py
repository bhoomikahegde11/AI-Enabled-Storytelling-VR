"""Audited Indian-character A/B testing inside the existing Parler studio.

No Unity writes, Qwen calls, model downloads or automatic production approval.
"""
from __future__ import annotations
import hashlib, importlib.util, json, math, re, threading, time
from datetime import datetime, timezone
from pathlib import Path
from uuid import uuid4
import numpy as np
import soundfile as sf
from tts_text import integer_words, NORMALIZATION_VERSION
from audio_quality import measure_wav

ROOT=Path(__file__).resolve().parent
DATA=ROOT/'testing/indian_auditions'
OUTPUT=ROOT/'samples/09_Indian_Dialogue_AB'
VERSION='indian-ab-v1'
ASSIGNMENTS={'lakshmi_amma':('Lakshmamma','Swapna','Lakshmi Amma'),
 'chinnamma_naik':('Chinnamma Naik','Priya','Chinnamma Naik'),
 'saraswati_chetti':('Saraswati Chetti','Sneha','Saraswati Chetti'),
 'abdul_rahman':('Abdul Rahman','Kabir','Abdul Rahman')}
JOIN_PRESETS={
 'Original edges':dict(trim=False,leading_ms=0,trailing_ms=0,gap_ms=20,crossfade_ms=0,fade_ms=0,normalize=False),
 'Tight':dict(trim=True,leading_ms=8,trailing_ms=12,gap_ms=6,crossfade_ms=0,fade_ms=.5,normalize=True),
 'Near contiguous':dict(trim=True,leading_ms=3,trailing_ms=5,gap_ms=2,crossfade_ms=0,fade_ms=.3,normalize=True),
 'Speech-aware 60 ms':dict(trim=True,leading_ms=25,trailing_ms=35,gap_ms=60,crossfade_ms=0,fade_ms=1,normalize=True,edge_policy='hysteresis-v1')}
ZERO_NUMBER_SETTINGS=dict(JOIN_PRESETS['Speech-aware 60 ms'],zero_number_boundaries=True)
LOCK=threading.RLock()

def sha(path):return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def fingerprint(value):return hashlib.sha256(json.dumps(value,sort_keys=True,ensure_ascii=False).encode()).hexdigest()
def now():return datetime.now(timezone.utc).isoformat()
def load(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))
def create_json(path,value):
    path=Path(path);path.parent.mkdir(parents=True,exist_ok=True)
    with path.open('x',encoding='utf-8') as f:json.dump(value,f,indent=2,ensure_ascii=False)
def words(text):return re.findall(r"[\w']+",text.replace('-',' ').lower())
def int32(value,minimum=0):
    if isinstance(value,bool) or not re.fullmatch(r'\d+',str(value)):raise ValueError('Use an exact integer, not a decimal or scientific notation.')
    n=int(value)
    if not minimum<=n<=2147483647:raise ValueError(f'Integer must be {minimum}–2,147,483,647.')
    return n
def number_text(value):
    n=int32(value)
    if n<1000000000:return integer_words(n)
    head,tail=divmod(n,1000000000)
    return integer_words(head)+' billion'+((' and ' if tail<100 else ' ')+integer_words(tail) if tail else '')
def quantity_text(count,unit):
    count=int32(count,1)
    unit=unit.rstrip('s') if unit in {'Palams','Seers','Manangus','Bahars'} else unit
    if unit not in {'Palam','Seer','Veesai','Manangu','Bahar'}:raise ValueError('Unsupported historical unit.')
    grams=count*{'Palam':35,'Seer':280,'Veesai':1400,'Manangu':11200,'Bahar':448000}[unit]
    if grams>2147483647:raise ValueError('Quantity exceeds the game integer-gram representation.')
    # Match MarketManager's actual output-unit thresholds; do not audition labels
    # such as eight Palams when Unity would render that amount as one Seer.
    family=next((name for threshold,name in [(403200,'Bahar'),(10080,'Manangu'),(1260,'Veesai'),(224,'Seer')] if grams>=threshold),'Palam')
    if family!=unit:raise ValueError(f'Unity formats this amount using {family}; select that canonical unit/count instead.')
    return number_text(count)+' '+(unit+'s' if count!=1 and unit!='Veesai' else unit)
def pronunciation(text,rules):
    for key,value in rules.items():
        if not re.fullmatch(r"[^\W\d_]+",key) or not isinstance(value,str) or not re.fullmatch(r"[\w '-]+",value) or re.search(r'\d|_',value):
            raise ValueError('Pronunciation rules must map one word to spelled-out speech words; no digits/IDs/placeholders.')
        text=re.sub(r'\b'+re.escape(key)+r'\b',lambda m:value,text,flags=re.I)
    return text

def make_plan(row,price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper',strategy='Phrase numbers',singular=False,rules=None):
    if strategy not in {'Phrase numbers','Compact grammar'}:raise ValueError('Unknown segment strategy.')
    fields=set(row['placeholders'])
    if fields&{'spiceName','spiceDescriptor'} and spice not in {'pepper','cinnamon','cardamom'}:raise ValueError('Unsupported physical trade spice.')
    price=int32(price,1) if fields&{'currentBuyerOffer','counterPrice','finalPrice'} else 234
    seller=int32(seller_price,1) if 'offeredPrice' in fields else 300
    quantity=int32(quantity,1) if 'quantityLabel' in fields else 2
    unit=unit if 'quantityLabel' in fields else 'Seer'
    spice=spice if fields&{'spiceName','spiceDescriptor'} else 'pepper'
    rules=rules or {};qty=quantity_text(quantity,unit)
    # Use only actual audited fields; no dialogue selection or economic decisions happen here.
    values={'currentBuyerOffer':number_text(price),'counterPrice':number_text(price),'finalPrice':number_text(price),
        'offeredPrice':number_text(seller),'quantityLabel':qty,'spiceName':spice,'currency':'varahas',
        'spiceDescriptor':{'pepper':'black pepper','cinnamon':'cinnamon bark','cardamom':'green cardamom'}[spice],
        'ruleReply':'The market is busy and your words were unclear. Say it again plainly.'}
    numeric={'currentBuyerOffer':price,'counterPrice':price,'finalPrice':price,'offeredPrice':seller}
    template=row['exact_template'];ops=[];warnings=[]
    last_price=None
    for part in re.split(r'(\{[^{}]+\})',template):
        if not part:continue
        if part.startswith('{'):
            field=part[1:-1]
            if field not in values:raise ValueError('Unmapped actual source placeholder '+field)
            text=values[field];kind='phrase'
            if field in numeric:
                last_price=numeric[field];kind='number'
                if strategy=='Compact grammar':
                    ops.extend(dict(text=w,kind='number',field=field) for w in number_text(numeric[field]).replace('-',' ').split());continue
            if field=='currency':
                if last_price==1:
                    if singular:text='varaha'
                    else:warnings.append('Exact Unity wording is one varahas; no silent grammar correction applied.')
                kind='currency'
            if field=='quantityLabel' and strategy=='Compact grammar':
                ops.extend(dict(text=w,kind='number',field=field) for w in number_text(quantity).replace('-',' ').split())
                ops.append(dict(text=qty[len(number_text(quantity)):].strip(),kind='unit',field=field));continue
            if field in {'spiceName','spiceDescriptor'} and ops and ops[-1]['kind']=='literal' and re.fullmatch(r'\s*(?:of|for(?: the)?|the)\s*',ops[-1]['text']):
                previous=ops.pop();text=previous['text']+text;kind='item_context'
            ops.append(dict(text=text,kind=kind,field=field))
        else:ops.append(dict(text=part,kind='literal',field=None))
    # Currency and surrounding connective words form meaningful phrases rather than a lone suffix.
    merged=[]
    for op in ops:
        if op['kind']=='currency' and merged and merged[-1]['kind']=='literal':
            prev=merged.pop();op=dict(op,text=prev['text']+op['text'])
        if merged and merged[-1]['kind']=='currency' and op['kind']=='literal':merged[-1]['text']+=op['text'];continue
        merged.append(op)
    segments=[];punctuation=''
    for op in merged:
        if not words(op['text']):
            if segments:segments[-1]['text']+=op['text']
            else:punctuation+=op['text']
            continue
        op=dict(op,text=(punctuation+op['text']).strip());punctuation='';segments.append(op)
    full=re.sub(r'\{([^{}]+)\}',lambda m:values[m[1]],template)
    if singular:
        # Explicit review policy. Price-specific occurrences only; no global replacement by the last amount.
        parts=re.split(r'(\{[^{}]+\})',template);out=[];last=None
        for p in parts:
            if p.startswith('{'):
                f=p[1:-1]
                if f in numeric:last=numeric[f]
                out.append('varaha' if f=='currency' and last==1 else values[f])
            else:out.append(p)
        full=''.join(out)
    if words(' '.join(s['text'] for s in segments))!=words(full):raise ValueError('Segment plan changes the spoken words.')
    full_tts=pronunciation(full,rules)
    # Punctuation is retained in the ordered source plan and boundary pauses, rather
    # than asking TTS to begin an independent phrase by reading a sentence terminator.
    for s in segments:s['tts_input']=pronunciation(s['text'].lstrip(' .!?;:,'),rules)
    if words(full_tts)!=words(' '.join(s['tts_input'] for s in segments)):raise ValueError('Pronunciation mapping crosses a segment boundary.')
    raw_values={'currentBuyerOffer':str(price),'counterPrice':str(price),'finalPrice':str(price),'offeredPrice':str(seller),
        'quantityLabel':str(quantity)+' '+qty[len(number_text(quantity)):].strip(),'spiceName':spice,'spiceDescriptor':values['spiceDescriptor'],'currency':'varahas','ruleReply':values['ruleReply']}
    if row['scenario']=='BuyerCounterFirst' and row['character_id']=='saraswati_chetti':warnings.append('Audited wording issue: For this two Seers. Original text preserved.')
    if row['scenario'] in {'PlayerAcceptedDeal','SellerPriceAccepted'}:warnings.append('Agreement audition assumes these frozen terms; it does not resolve a real trade.')
    if row['scenario']=='TransactionSuccess':warnings.append('Receipt line is only valid after fulfillment/settlement in Unity.')
    if row['restrictions'].get('patience_bucket')=='Low':warnings.append('Low-patience wording requires actual patience <=2.')
    if row['scenario']=='OfferReason' and row['character_id']=='lakshmi_amma':warnings.append('Conditional Lakshmamma provider route; normal pressure query often uses budget response.')
    return dict(version=VERSION,character_id=row['character_id'],dialogue_id=row['inventory_id'],scenario=row['scenario'],
        original_template=template,restrictions=row['restrictions'],source_file=row['source_file'],source_line=row['source_line'],
        unity_resolved_text=re.sub(r'\{([^{}]+)\}',lambda m:raw_values[m[1]],template),spoken_text=full,tts_input=full_tts,segments=segments,
        strategy=strategy,values=dict(price=price,seller_price=seller,quantity=quantity,unit=unit,spice=spice),
        singular_currency_review_policy=singular,pronunciation_rules=rules,warnings=warnings,
        content_verified=False,review_status='NEEDS REVIEW')

_dsp=None
def dsp():
    global _dsp
    if _dsp is None:
        p=ROOT/'production/batches/portuguese_ready_20261027/verification_studio_v3/process_voice_v3.py'
        spec=importlib.util.spec_from_file_location('existing_edge_processing',p);_dsp=importlib.util.module_from_spec(spec);spec.loader.exec_module(_dsp)
    return _dsp

def speech_activity_edges(x,rate):
    """Find sustained speech, then extend into quieter attacks/decays above the noise bed.

    Only outer edges are selected; no internal speech or pauses are removed.
    This is an energy heuristic, not a phoneme recognizer. Padding remains mandatory.
    """
    win=max(1,round(rate*.01));hop=max(1,round(rate*.002))
    frames=np.lib.stride_tricks.sliding_window_view(np.pad(x,(0,max(0,win-len(x)))),win)[::hop]
    db=20*np.log10(np.maximum(np.sqrt(np.mean(frames*frames,axis=1)),1e-12))
    floor=float(np.percentile(db,20));peak=float(db.max())
    if peak<-90:raise ValueError('No usable speech activity in source PCM')
    gate=min(peak-8,max(peak-30,floor+14));support=min(gate-4,floor+6)
    active=db>gate
    # Reject isolated clicks; keep even very short spoken-word cores.
    changes=np.diff(np.r_[False,active,False].astype(int));spans=[(a,b) for a,b in zip(np.flatnonzero(changes==1),np.flatnonzero(changes==-1)) if b-a>=max(1,round(.012*rate/hop))]
    if not spans:raise ValueError('No sustained speech found; leave source unchanged for review')
    first,last=spans[0][0],spans[-1][1]-1
    # Bridge up to 20ms of quiet inside a soft consonant/decay, not long noise tails.
    limit=max(1,round(.02*rate/hop))
    for direction in [-1,1]:
        i=first if direction==-1 else last;edge=i;quiet=0
        while 0<=i+direction<len(db):
            i+=direction
            if db[i]>support:edge=i;quiet=0
            else:
                quiet+=1
                if quiet>=limit:break
        if direction==-1:first=edge
        else:last=edge
    start=int(first*hop);end=min(len(x),int(last*hop+win))
    return dict(first=start,last=end,gate_dbfs=gate,support_gate_dbfs=support,noise_floor_dbfs=floor,window_ms=10,hop_ms=2,
        source_edge_risk=start<round(rate*.002) or len(x)-end<round(rate*.002),policy='hysteresis-v1')

def numeric_segment(segment):
    return segment.get('kind')=='number' or segment.get('field')=='quantityLabel'

def process_piece(x,rate,settings,target=None,number=False):
    x=np.asarray(x,dtype=np.float64)
    if x.ndim!=1 or x.size==0 or not np.isfinite(x).all():raise ValueError('Invalid source PCM')
    edge=speech_activity_edges(x,rate) if settings.get('edge_policy')=='hysteresis-v1' else dsp().speech_edges(x,rate)
    warnings=[];start=0;end=len(x)
    if edge['source_edge_risk']:warnings.append('Speech approaches original edge; completeness needs listening.')
    if settings['trim']:
        # Hysteresis edges retain additional guard for low-level consonants/breath tails.
        guard=15 if settings.get('edge_policy')=='hysteresis-v1' else 0
        lead=max(float(settings['leading_ms']),guard,6 if edge['source_edge_risk'] else 0)
        trail=max(float(settings['trailing_ms']),guard,8 if edge['source_edge_risk'] else 0)
        if number and settings.get('zero_number_boundaries'):
            lead=trail=0
        start=max(0,edge['first']-round(rate*lead/1000));end=min(len(x),edge['last']+round(rate*trail/1000))
        if number and settings.get('zero_number_boundaries'):
            # Outward zero crossings retain source samples, never overlap or cut into speech.
            reach=max(1,round(rate*.001))
            low=max(0,start-reach);start=low+int(np.argmin(abs(x[low:start+1])))
            high=min(len(x),end+reach);end=end-1+int(np.argmin(abs(x[end-1:high])))+1
    y=x[start:end].copy()
    gain=0
    if settings['normalize'] and target is not None:
        metrics=dsp().old.measure(y,rate)
        gain=min(4,max(-9,float(target)-metrics['speech_active_weighted_dbfs']))
        gain=min(gain,-2-metrics['true_peak_estimate_dbtp'])
        y*=10**(gain/20)
    fi=min(round(rate*float(settings['fade_ms'])/1000),max(0,edge['first']-start));fo=min(round(rate*float(settings['fade_ms'])/1000),max(0,end-edge['last']))
    if fi:y[:fi]*=np.linspace(0,1,fi)
    if fo:y[-fo:]*=np.linspace(1,0,fo)
    return y,dict(trim_start_sample=start,trim_end_sample=end,fade_in_samples=fi,fade_out_samples=fo,gain_db=gain,
        zero_number_boundary_processing=bool(number and settings.get('zero_number_boundaries')),added_number_padding_ms=0 if number and settings.get('zero_number_boundaries') else None,
        leading_safe_samples=max(0,edge['first']-start),trailing_safe_samples=max(0,end-edge['last']),speech_edges=edge,warnings=warnings)

def assemble(pieces,rate,segments,settings):
    if not pieces or len(pieces)!=len(segments):raise ValueError('Incomplete assembly')
    out=pieces[0][0].copy();joins=[]
    for i,(x,meta) in enumerate(pieces[1:],1):
        gap=float(settings['gap_ms']);prev=segments[i-1]['text']
        if segments[i-1]['kind']=='number' and segments[i]['kind']=='number':gap=min(gap,40 if settings.get('edge_policy')=='hysteresis-v1' else 3)
        if re.search(r'[.!?]\s*$',prev) or re.match(r'\s*[.!?]',segments[i]['text']):gap=max(gap,55)
        elif re.search(r'[,;:]\s*$',prev) or re.match(r'\s*[,;:]',segments[i]['text']):gap=max(gap,25)
        zero_number=settings.get('zero_number_boundaries') and (numeric_segment(segments[i-1]) or numeric_segment(segments[i]))
        if zero_number:gap=0
        # Never crossfade audible phonemes; only protected padding may overlap, and only at zero gap.
        overlap=0
        if gap==0 and not zero_number:
            overlap=min(round(rate*float(settings['crossfade_ms'])/1000),pieces[i-1][1]['trailing_safe_samples'],meta['leading_safe_samples'],len(x),len(out))
        if overlap:
            ramp=np.linspace(0,1,overlap);out=np.concatenate((out[:-overlap],out[-overlap:]*(1-ramp)+x[:overlap]*ramp,x[overlap:]))
        else:out=np.concatenate((out,np.zeros(round(rate*gap/1000)),x))
        joins.append(dict(after_segment=i-1,gap_ms=gap,crossfade_samples=overlap))
    # Attenuate only if joining somehow exceeds the peak ceiling. No compressor or clipping.
    peak=float(np.max(np.abs(out)));attenuation=min(0,-2-20*math.log10(max(peak,1e-12)))
    if attenuation<0:out*=10**(attenuation/20)
    return out,dict(joins=joins,final_attenuation_db=attenuation,pitch_or_speed_change=False)

class TestingStudio:
    def __init__(self,root=ROOT,engine=None):
        self.root=Path(root);self.data=self.root/'testing/indian_auditions';self.output=self.root/'samples/09_Indian_Dialogue_AB';self.engine=engine
        self.library=load(self.data/'library.json');self.new_generations=0;self.maximum_new=120
        self.baseline_generation=load(ROOT/'cache/huggingface/models--ai4bharat--indic-parler-tts/snapshots/660bc53c64ae83150613264111708a0465211237/generation_config.json')
    def rows(self,cid):return self.library['characters'][cid]['selections']
    def row(self,cid,id):return next(r for r in self.rows(cid) if r['inventory_id']==id)
    def default_profile(self,cid):
        name,speaker,key=ASSIGNMENTS[cid];p=load(self.root/'character_presets.json')[key];v=p.get('delivery_presets',{}).get('Natural Conversational',dict(description=p['description'],mood='neutral',pace='moderate',expression='moderate'))
        assert p['speaker']==speaker
        return dict(character_id=cid,speaker=speaker,description=v['description'],mood=v['mood'],pace=v['pace'],expression=v['expression'],
            seed=1522,generation_overrides={},pronunciation_rules={},strategy='Phrase numbers',singular=False,processing=dict(JOIN_PRESETS['Tight']),
            normalization=NORMALIZATION_VERSION+' + explicit Int32 phrase expansion',approved=False)
    def profile(self,cid):
        pointer=self.data/'settings'/cid/'CURRENT.json'
        return load(pointer) if pointer.exists() else self.default_profile(cid)
    def validate_profile(self,cid,p):
        if p.get('character_id')!=cid or p.get('speaker')!=ASSIGNMENTS[cid][1]:raise ValueError('Character/voice identity mismatch')
        if not str(p.get('description','')).strip():raise ValueError('Description is required')
        if p['mood'] not in {'neutral','friendly','stern','impatient','happy','persuasive','authoritative'} or p['pace'] not in {'slow','moderate','brisk'} or p['expression'] not in {'restrained','moderate','high'}:raise ValueError('Unsupported existing delivery setting')
        if not 0<=int32(p['seed'])<=2147483647:raise ValueError('Invalid seed')
        if p.get('seed_policy') not in {None,'character-text-v1'}:raise ValueError('Unsupported seed policy')
        g=p['generation_overrides'];limits={'temperature':(.1,2),'top_p':(.05,1),'top_k':(1,200),'max_new_tokens':(128,2610),'repetition_penalty':(1,2)}
        if not set(g)<=set(limits):raise ValueError('Unsupported generation parameter')
        for key,value in g.items():
            if not limits[key][0]<=value<=limits[key][1]:raise ValueError('Generation parameter out of range '+key)
            if key in {'top_k','max_new_tokens'} and (isinstance(value,bool) or int(value)!=value):raise ValueError('Integer model parameter required')
        s=p['processing']
        if s.get('edge_policy','legacy') not in {'legacy','hysteresis-v1'}:raise ValueError('Unknown speech-edge policy')
        for key in ['leading_ms','trailing_ms','gap_ms','crossfade_ms','fade_ms']:
            if not math.isfinite(float(s[key])) or not 0<=float(s[key])<=({'crossfade_ms':10,'fade_ms':3}.get(key,100)):raise ValueError('Invalid join parameter '+key)
        pronunciation('sample',p['pronunciation_rules'])
        return p
    def save_profile(self,cid,p):
        self.validate_profile(cid,p);p=dict(p,approved=False,saved_utc=now());folder=self.data/'settings'/cid
        create_json(folder/(datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%SZ')+'_'+uuid4().hex[:8]+'.json'),p)
        folder.mkdir(parents=True,exist_ok=True);tmp=folder/('CURRENT_'+uuid4().hex+'.tmp');tmp.write_text(json.dumps(p,indent=2),encoding='utf-8');tmp.replace(folder/'CURRENT.json')
        return str(folder/'CURRENT.json')
    def raw_key(self,cid,text,p):
        key=dict(version=VERSION,character_id=cid,speaker=p['speaker'],description=p['description'],mood=p['mood'],pace=p['pace'],expression=p['expression'],
            seed=p['seed'],generation_overrides=p['generation_overrides'],model='ai4bharat/indic-parler-tts',model_defaults=self.baseline_generation,
            normalized_input=text,normalization=p['normalization'],pronunciation_rules=p['pronunciation_rules'])
        if p.get('seed_policy'):key['seed_policy']=p['seed_policy']
        return fingerprint(key)
    def raw(self,cid,text,p,dialogue_id,generate):
        key=self.raw_key(cid,text,p);index=self.data/'cache'/cid/(key+'.json')
        if index.exists():
            try:
                item=load(index);wav=Path(item['path'])
                if item['character_id']==cid and wav.is_file() and sha(wav)==item['sha256']:
                    x,sr=sf.read(wav,dtype='float64')
                    if x.ndim==1 and len(x)>0 and np.isfinite(x).all():return item
            except (OSError,ValueError,KeyError):pass
        if not generate:raise FileNotFoundError('Missing tested raw recording: '+text)
        if self.engine is None:raise RuntimeError('Parler engine unavailable')
        if self.new_generations>=self.maximum_new:raise RuntimeError('Small audition synthesis budget reached; no bulk generation')
        self.new_generations+=1
        print(f'[INDIC AUDITION] {cid} new={self.new_generations}/{self.maximum_new} text={text}',flush=True)
        effective_seed=p['seed']
        if p.get('seed_policy')=='character-text-v1':effective_seed=int(fingerprint(dict(cid=cid,speaker=p['speaker'],text=text,seed=p['seed']))[:8],16)&0x7fffffff
        native,meta=self.engine.generate(text,p['speaker'],p['description'],p['mood'],p['pace'],p['expression'],preset_name='Audited Indian A/B test',
            seed=effective_seed,dialogue_id=dialogue_id,sample_label='AB_'+key[:18],sample_stage='improved',
            output_directory=self.output/'raw'/cid,generation_overrides=p['generation_overrides'])
        info=measure_wav(native,text)
        if info['sample_rate']!=meta['model_output_sample_rate'] or info['peak']>=.9999:raise ValueError('Invalid/clipping raw take preserved for review: '+native)
        item=dict(character_id=cid,speaker=p['speaker'],path=native,sha256=sha(native),metadata_path=str(Path(native).with_suffix('.json')),fingerprint=key,metrics=info,review_status='NEEDS REVIEW')
        if index.exists():index.rename(index.with_name(key+'_invalid_'+uuid4().hex[:8]+'.json'))
        create_json(index,item)
        return item
    def run(self,cid,id,values,p,generate=True):
        with LOCK:
            self.validate_profile(cid,p);plan=make_plan(self.row(cid,id),**values,strategy=p['strategy'],singular=p['singular'],rules=p['pronunciation_rules'])
            full=self.raw(cid,plan['tts_input'],p,id,generate);raws=[]
            if not self.row(cid,id)['placeholders']:raws=[full];plan['segments']=[dict(text=plan['spoken_text'],tts_input=plan['tts_input'],kind='full',field=None)]
            else:
                for part in plan['segments']:raws.append(self.raw(cid,part['tts_input'],p,id,generate))
            key=fingerprint(dict(plan=plan,profile=p,raw_hashes=[full['sha256']]+[r['sha256'] for r in raws]))
            dest=self.output/'comparisons'/cid/key[:20];record=dest/'comparison.json'
            if record.exists():
                old=load(record)
                if all(Path(old[k]).is_file() and sha(old[k])==old[k+'_sha256'] for k in ['full_wav','assembled_wav']):return old
                dest=dest.with_name(dest.name+'_'+uuid4().hex[:8]);record=dest/'comparison.json'
            dest.mkdir(parents=True,exist_ok=True);x,rate=sf.read(full['path'],dtype='float64')
            policy=p['processing'].get('loudness_policy');target=None if policy else dsp().old.measure(x,rate)['speech_active_weighted_dbfs']
            whole,full_processing=process_piece(x,rate,dict(p['processing'],normalize=False));pieces=[];segment_data=[]
            if policy:
                from speech_loudness import normalize,measure as loudness_measure
                whole,lm=normalize(whole,rate,policy,plan['tts_input'],quality_reference=loudness_measure(x,rate));full_processing['loudness']=lm
            for i,(part,r) in enumerate(zip(plan['segments'],raws)):
                audio,sr=sf.read(r['path'],dtype='float64')
                if sr!=rate:raise ValueError('Native sample rates differ; no implicit resampling')
                y,pm=process_piece(audio,rate,dict(p['processing'],normalize=False) if policy else p['processing'],target,number=numeric_segment(part))
                if policy:
                    y,lm=normalize(y,rate,policy,part['tts_input'],quality_reference=loudness_measure(audio,rate));pm['loudness']=lm;pm['gain_db']=lm['gain_db']
                pieces.append((y,pm));wav=dest/f'segment_{i+1:02d}.wav'
                with wav.open('xb') as f:sf.write(f,y,rate,format='WAV',subtype='PCM_24')
                segment_data.append(dict(index=i,**part,raw=r,processed_path=str(wav),processed_sha256=sha(wav),processing=pm))
            joined,joins=assemble(pieces,rate,plan['segments'],p['processing'])
            if policy:
                joined,lm=normalize(joined,rate,policy,plan['tts_input'],stage='assembly',trusted_sources=True);joins['loudness']=lm
                defective=[s['tts_input'] for s in segment_data if s['processing']['loudness']['status']=='FLAGGED_DEFECTIVE']
                if defective:
                    lm['status']='FLAGGED_DEFECTIVE_SOURCE';lm['warnings'].append('Assembly contains defective speech component(s): '+repr(defective))
            paths={}
            for name,audio in [('full',whole),('assembled',joined)]:
                wav=dest/(id+'_'+p['strategy'].replace(' ','')+'_'+name+'.wav')
                with wav.open('xb') as f:sf.write(f,audio,rate,format='WAV',subtype='PCM_24')
                paths[name+'_wav']=str(wav);paths[name+'_wav_sha256']=sha(wav)
            metrics={name:measure_wav(paths[name+'_wav'],plan['tts_input']) for name in ['full','assembled']}
            warnings=list(plan['warnings'])+['Speech content/number pronunciation/identity need human listening; no offline ASR installed.']
            if policy:
                warnings += ['Full: '+w for w in full_processing['loudness']['warnings']]
                warnings += ['Assembly: '+w for w in joins['loudness']['warnings']]
                warnings += [part['tts_input']+': '+w for part in segment_data for w in part['processing']['loudness']['warnings']]
            for name,m in metrics.items():
                if m['peak']>=.9999:raise ValueError('Clipping after processing')
                if m['intended_words_per_minute']>300:warnings.append(name+': unusually short for intended words; possible omitted/truncated speech')
                if m['duration_seconds']>max(6,len(words(plan['tts_input']))*.9):warnings.append(name+': unusually long duration / possible repetition')
                if m['internal_low_energy_gaps_at_least_350ms']:warnings.append(name+': internal low-energy gaps; not automatically deleted')
            result=dict(**paths,comparison_id=key,created_utc=now(),plan=plan,profile=p,full_raw=full,segments=segment_data,
                sample_rate=rate,metrics=metrics,full_processing=full_processing,assembly_processing=joins,warnings=warnings,
                validation_status='TECHNICAL_WAV_CHECKS_PASSED',review_status='NEEDS REVIEW',content_verification='UNVERIFIED_LISTEN',record_path=str(record))
            create_json(record,result)
            events=self.data/'events.jsonl';events.parent.mkdir(parents=True,exist_ok=True)
            with events.open('a',encoding='utf-8') as f:f.write(json.dumps(dict(event='COMPARISON_READY',record=str(record),comparison_id=key))+'\n')
            return result
    def results(self,cid,id=None):
        found=[]
        for p in (self.output/'comparisons'/cid).glob('*/comparison.json'):
            r=load(p)
            if id is None or r['plan']['dialogue_id']==id:found.append(r)
        return sorted(found,key=lambda r:r['created_utc'],reverse=True)
    def review(self,comparison_id,status,notes):
        if status not in {'APPROVED','REGENERATE','NEEDS REVIEW'}:raise ValueError('Invalid review outcome')
        row=dict(comparison_id=comparison_id,status=status,notes=str(notes),time=now())
        p=self.data/'reviews'/('review_'+uuid4().hex+'.json');create_json(p,row);return str(p)
