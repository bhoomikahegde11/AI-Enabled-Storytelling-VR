"""Dialogue browsing endpoints on the existing Gradio/Parler server, no second engine."""
import csv,json,html,threading,hashlib
from pathlib import Path
from fastapi import HTTPException
from fastapi.responses import FileResponse
from fastapi.middleware.cors import CORSMiddleware
from indian_testing import TestingStudio,ASSIGNMENTS,ZERO_NUMBER_SETTINGS,make_plan,load,sha,fingerprint,create_json,now,words,numeric_segment
from speech_loudness import POLICY,derivative
from audio_quality import measure_wav

ROOT=Path(__file__).resolve().parent
LABELS={'CustomerGreeting':'Greeting','BuyerCounterFirst':'Opening Offer','BuyerCounterMiddle':'Counteroffer',
    'SellerPriceAccepted':'Agreement','SellerPriceTooHigh':'Refusal / Price Too High','HostileWarning':'Frustration',
    'QuantityChanged':'Quantity Change','UnclearSpeech':'Clarification','AskName':'Introduction',
    'TransactionSuccess':'Successful Trade','HistoryKing':'History / King','HistoryTemple':'History / Temple','HistoryQuestion':'History / Market'}
MODES={'Full sentence','Segmented','Both'}
LOCK=threading.RLock()

class RecordingBrowser:
    def __init__(self,engine=None):
        self.studio=TestingStudio(engine=engine);self.folder=self.studio.data/'recording_browser'
        catalog=load(self.folder/'catalog.json');self.selected={}
        for cid,entry in catalog['characters'].items():
            self.selected[cid]=entry['audition_ids'];self.studio.library['characters'][cid]['selections']=entry['records']
        self.batch_ids=catalog['starter_batches'];self.engine=engine;self.previews={}
    def profile(self,cid,strategy='Phrase numbers',gap=60):
        frozen=self.folder/'voice_sessions.json'
        p=load(frozen)[cid] if frozen.exists() else self.studio.profile(cid)
        p=dict(p);p['strategy']=strategy
        p['processing']=dict(ZERO_NUMBER_SETTINGS,gap_ms=float(gap))
        config=self.folder/'loudness_policy.json'
        p['processing']['loudness_policy']=load(config) if config.exists() else dict(POLICY)
        if not 0<=p['processing']['gap_ms']<=100:raise ValueError('Gap must be 0–100 ms')
        return self.studio.validate_profile(cid,p)
    def preview(self,cid,id,values,strategy='Phrase numbers',gap=60):
        row=self.studio.row(cid,id)
        if row['coverage_class']=='UNREACHABLE_OR_DEFERRED':raise ValueError('This source is deferred/unreachable; browse only, not a production test.')
        p=self.profile(cid,strategy,gap)
        plan=make_plan(row,**values,strategy=strategy,singular=p['singular'],rules=p['pronunciation_rules'])
        return row,p,plan
    def key(self,cid,id,values,mode,strategy,gap):
        row,p,plan=self.preview(cid,id,values,strategy,gap)
        # Full mode is independent of assembly settings and uses the existing raw cache.
        data=dict(cid=cid,id=id,text=plan['tts_input'],raw_key=self.studio.raw_key(cid,plan['tts_input'],p),mode=mode,output_standard=p['processing']['loudness_policy'])
        if mode!='Full sentence':data.update(plan=plan,processing=p['processing'])
        return fingerprint(data),row,p,plan
    def lookup(self,cid,id,values,mode='Both',strategy='Phrase numbers',gap=60):
        key,row,p,plan=self.key(cid,id,values,mode,strategy,gap);path=self.folder/'results'/cid/id/(key+'.json')
        if path.exists():
            r=load(path)
            if all(Path(a['path']).is_file() and sha(a['path'])==a['sha256'] for a in r['audio']):return r
        # Reuse full recordings immediately without synthesis or a receipt write.
        if mode=='Full sentence':
            try:
                raw=self.studio.raw(cid,plan['tts_input'],p,id,False)
                d=derivative(raw['path'],self.studio.output/'consistent_exports'/cid,p['processing']['loudness_policy'],plan['tts_input'])
                return self.result(key,row,p,plan,mode,[dict(mode=mode,path=d['path'],sha256=d['sha256'],metrics=measure_wav(d['path'],plan['tts_input']),loudness=d)],None)
            except FileNotFoundError:pass
        return None
    def result(self,key,row,p,plan,mode,audio,comparison):
        return dict(key=key,character_id=row['character_id'],speaker=p['speaker'],dialogue_id=row['inventory_id'],
            original_text=row['exact_template'],resolved_text=plan['unity_resolved_text'],spoken_text=plan['tts_input'],
            mode=mode,settings=p,plan=plan,source_file=row['source_file'],source_line=row['source_line'],
            comparison=comparison,audio=audio,warnings=plan['warnings']+['Actual spoken content, accent and identity require listening approval.'],review_status='NEEDS REVIEW',created_utc=now())
    def quality_warnings(self,r):
        r=dict(r);warnings=list(r['warnings'])
        report=self.folder/'FINAL_REPORT.json'
        if report.exists() and r['mode']=='Full sentence':
            try:
                for failed in load(report).get('failed_segmented_contexts',[]):
                    if failed['character_id']==r['character_id'] and failed['dialogue_id']==r['dialogue_id']:warnings.append('Batch segmented attempt failed: '+failed['error']+'. This is the matching full-sentence take only.')
            except json.JSONDecodeError:pass
        for a in r['audio']:
            if a.get('loudness'):warnings.extend(a['loudness']['warnings'])
            if a['metrics']['intended_words_per_minute']>300:warnings.append(a['mode']+': unusually short for intended words; possible omitted speech.')
        if r.get('comparison'):
            for s in load(r['comparison'])['segments']:
                if s['processing'].get('loudness',{}).get('status')=='FLAGGED_DEFECTIVE':
                    warnings.append('DEFECTIVE COMPONENT '+repr(s['tts_input'])+': insufficient/unclear speech; normalization cannot repair it.')
                    r['validation_status']='DEFECTIVE_COMPONENT_REVIEW_REQUIRED'
                if s['raw']['metrics']['intended_words_per_minute']>300:warnings.append('Fragment '+repr(s['tts_input'])+' is unusually short; possible missing speech. Needs listening/regeneration review.')
                gaps=s['raw']['metrics']['internal_low_energy_gaps_at_least_350ms']
                if numeric_segment(s) and gaps:warnings.append('Number/quantity phrase '+repr(s['tts_input'])+' has source-internal pauses '+str(gaps)+'; not deleted.')
        r['warnings']=list(dict.fromkeys(warnings));return r
    def generate(self,cid,id,values,mode='Both',strategy='Phrase numbers',gap=60,allow_synthesis=True):
        if mode not in MODES:raise ValueError('Unsupported generation mode')
        with LOCK:
            key,row,p,plan=self.key(cid,id,values,mode,strategy,gap);path=self.folder/'results'/cid/id/(key+'.json')
            existing=self.lookup(cid,id,values,mode,strategy,gap)
            if existing and path.exists():return existing
            audio=[];comparison=None
            if mode=='Full sentence':
                raw=self.studio.raw(cid,plan['tts_input'],p,id,allow_synthesis)
                d=derivative(raw['path'],self.studio.output/'consistent_exports'/cid,p['processing']['loudness_policy'],plan['tts_input'])
                audio=[dict(mode=mode,path=d['path'],sha256=d['sha256'],metrics=measure_wav(d['path'],plan['tts_input']),loudness=d)]
            else:
                r=self.studio.run(cid,id,values,p,allow_synthesis);comparison=r['record_path']
                for label,k in [('Full sentence','full'),('Segmented','assembled')]:
                    if mode=='Both' or label=='Segmented':audio.append(dict(mode=label,path=r[k+'_wav'],sha256=r[k+'_wav_sha256'],metrics=r['metrics'][k]))
            result=self.result(key,row,p,plan,mode,audio,comparison)
            if comparison:
                compared=load(comparison);result['warnings']+=compared['warnings']
                for part in compared['segments']:
                    gaps=part['raw']['metrics']['internal_low_energy_gaps_at_least_350ms']
                    if numeric_segment(part) and gaps:result['warnings'].append('Number/quantity phrase '+repr(part['tts_input'])+' has source-internal low-energy pauses: '+str(gaps)+'; not deleted.')
            result['warnings']=list(dict.fromkeys(result['warnings']))
            if path.exists():path=path.with_name(key+'_'+fingerprint(result)[:8]+'.json')
            create_json(path,result);self.write_index();return result
    def all_results(self):
        results=[]
        for p in (self.folder/'results').glob('*/*/*.json'):
            try:results.append(self.quality_warnings(load(p)))
            except json.JSONDecodeError:continue  # A different request may still be writing this receipt.
        return results
    def write_index(self):
        results=self.all_results();rows=[];body=['<!doctype html><meta charset="utf-8"><title>Indian NPC Listening Index</title><style>body{font:16px system-ui;max-width:1000px;margin:30px auto;background:#101922;color:#eee}article{border-bottom:1px solid #555;padding:15px}audio{width:100%}p{line-height:1.5}</style><h1>Indian NPC listening batch</h1><p>Exact source wording; all audio needs listening approval. The interactive browser is in the existing Portuguese Voice Verification Studio.</p>']
        import os
        for cid in ASSIGNMENTS:
            body.append('<h2>'+ASSIGNMENTS[cid][0]+'</h2>')
            for r in sorted([r for r in results if r['character_id']==cid],key=lambda r:r['dialogue_id']):
                body.append('<article><h3>'+html.escape(r['dialogue_id']+' · '+LABELS.get(r['plan']['scenario'],r['plan']['scenario']))+'</h3><p>'+html.escape(r['spoken_text'])+'</p>')
                for a in r['audio']:
                    relative=os.path.relpath(a['path'],self.folder).replace('\\','/')
                    body.append('<p>'+html.escape(a['mode'])+' · '+str(a['metrics']['duration_seconds'])+' s</p><audio controls preload="none" src="'+html.escape(relative,quote=True)+'"></audio>')
                    rows.append(dict(character=ASSIGNMENTS[cid][0],dialogue_id=r['dialogue_id'],mode=a['mode'],text=r['spoken_text'],wav=a['path'],duration=a['metrics']['duration_seconds'],review='NEEDS REVIEW',warnings=' | '.join(r['warnings'])))
                body.append('</article>')
        (self.folder/'LISTENING_INDEX.html').write_text('\n'.join(body),encoding='utf-8')
        if rows:
            with (self.folder/'LISTENING_INDEX.csv').open('w',encoding='utf-8-sig',newline='') as f:
                w=csv.DictWriter(f,fieldnames=list(rows[0]));w.writeheader();w.writerows(rows)

def mount_api(app,engine):
    browser=RecordingBrowser(engine)
    app.add_middleware(CORSMiddleware,allow_origin_regex=r'http://127\.0\.0\.1:(?:876[5-9]|877[0-4])',allow_methods=['GET','POST'],allow_headers=['Content-Type'])
    def unpack(body):
        if body.get('character_id') not in ASSIGNMENTS:raise ValueError('Unknown Indian character')
        return [body['character_id'],body['dialogue_id'],body.get('values',dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')),
            body.get('mode','Both'),body.get('strategy','Phrase numbers'),body.get('gap_ms',60)]
    def public(r):
        if not r:return None
        r=browser.quality_warnings(r)
        browser.previews[r['key']]=r
        if len(browser.previews)>256:browser.previews.pop(next(iter(browser.previews)))
        r=dict(r);r['audio']=[dict(a,url='/indian-api/audio/'+r['key']+'/'+str(i)) for i,a in enumerate(r['audio'])];return r
    @app.get('/indian-api/capabilities')
    def capabilities():return dict(version='indian-browser-consistency-v1',features=['quality-comparison','shared-loudness','frozen-profiles'])
    @app.get('/indian-api/catalog')
    def catalog():
        return dict(characters={cid:dict(name=a[0],voice=a[1],audition_ids=browser.selected[cid],batch_ids=browser.batch_ids[cid],rows=browser.studio.rows(cid)) for cid,a in ASSIGNMENTS.items()})
    @app.post('/indian-api/preview')
    def preview(body:dict):
        try:
            cid,id,values,mode,strategy,gap=unpack(body);row,p,plan=browser.preview(cid,id,values,strategy,gap)
            return dict(plan=plan,saved=public(browser.lookup(cid,id,values,mode,strategy,gap)))
        except (ValueError,KeyError,StopIteration) as e:raise HTTPException(400,str(e))
    @app.get('/indian-api/results/{cid}')
    def results(cid:str):
        if cid not in ASSIGNMENTS:raise HTTPException(404)
        return [public(r) for r in browser.all_results() if r['character_id']==cid]
    @app.post('/indian-api/generate')
    def generate(body:dict):
        try:return public(browser.generate(*unpack(body)))
        except Exception as e:raise HTTPException(400,str(e))
    @app.get('/indian-api/audio/{key}/{index}')
    def audio(key:str,index:int):
        if not key.isalnum() or len(key)!=64:raise HTTPException(404)
        r=browser.previews.get(key) or next((r for r in browser.all_results() if r['key']==key),None)
        if not r:raise HTTPException(404)
        if index<0 or index>=len(r['audio']):raise HTTPException(404)
        item=r['audio'][index];path=Path(item['path']).resolve()
        if not path.is_relative_to(browser.studio.output.resolve()) or not path.is_file() or sha(path)!=item['sha256']:raise HTTPException(404)
        return FileResponse(path,media_type='audio/wav',filename=path.name)
    def quality_report(run=None):
        folder=browser.folder/'consistency_pass'
        if run is None:
            if not (folder/'latest.json').exists():return dict(entries=[])
            run=load(folder/'latest.json')['run']
        if len(run)!=15 or not run.replace('T','').isdigit() or run[8]!='T':raise HTTPException(404)
        path=folder/'runs'/run/'REPORT.json'
        if not path.is_file():raise HTTPException(404)
        return load(path)
    @app.get('/indian-api/quality')
    def quality():
        r=quality_report()
        for i,e in enumerate(r['entries']):
            for variant in ['raw','normalized']:e[variant]['url']=f"/indian-api/quality-audio/{r['run']}/{i}/{variant}"
        return r
    @app.get('/indian-api/quality-audio/{run}/{index}/{variant}')
    def quality_audio(run:str,index:int,variant:str):
        r=quality_report(run)
        if variant not in {'raw','normalized'} or index<0 or index>=len(r['entries']):raise HTTPException(404)
        entry=r['entries'][index][variant];path=Path(entry['path']).resolve()
        if not path.is_relative_to((ROOT/'samples').resolve()) or not path.is_file() or sha(path)!=entry['sha256']:raise HTTPException(404)
        e=r['entries'][index]
        return FileResponse(path,media_type='audio/wav',filename=f"{e['speaker']}_{e['dialogue_id']}_{variant}.wav")
    return browser
