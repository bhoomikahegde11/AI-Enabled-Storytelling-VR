"""Simple listening workflow mounted inside the existing Parler studio."""
import json
from pathlib import Path
from indian_testing import TestingStudio,ASSIGNMENTS,JOIN_PRESETS,make_plan,load,sha

LABELS={'CustomerGreeting':'Greeting','AskName':'Introduction','BuyerCounterFirst':'Opening Offer',
    'BuyerCounterMiddle':'Counteroffer','BuyerHoldsFirm':'Holding Firm','SellerPriceAccepted':'Agreement',
    'SellerPriceTooHigh':'Price Too High','HostileWarning':'Frustration / Warning','OfferReason':'Explain Offer',
    'QuantityChanged':'Quantity Change','TransactionSuccess':'Successful Trade','UnclearSpeech':'Clarification / Retry'}

def find_saved(studio,cid,id,values,profile):
    """Never offer a different price, voice draft or join preset as a matching take."""
    plan=make_plan(studio.row(cid,id),**values,strategy=profile['strategy'],singular=profile['singular'],rules=profile['pronunciation_rules'])
    keys=studio.default_profile(cid).keys()
    for result in studio.results(cid,id):
        if any(result['profile'].get(k)!=profile.get(k) for k in keys):continue
        if any(result['plan'].get(k)!=plan.get(k) for k in ['original_template','tts_input','values','strategy']):continue
        if studio.row(cid,id)['placeholders'] and result['plan']['segments']!=plan['segments']:continue
        paths=[(result[k],result[k+'_sha256']) for k in ['full_wav','assembled_wav']]
        paths += [(s['processed_path'],s['processed_sha256']) for s in result['segments']]
        if all(Path(p).is_file() and sha(p)==digest for p,digest in paths):return plan,result
    return plan,None

def mount(gr,engine):
    studio=TestingStudio(engine=engine);first='lakshmi_amma';profile=studio.profile(first)
    def choices(cid):return [(LABELS.get(r['scenario'],r['scenario'])+' · '+r['inventory_id'],r['inventory_id']) for r in studio.rows(cid)]
    def settings_for(p):
        g=p['generation_overrides'];s=p['processing']
        return [p['description'],p['mood'],p['pace'],p['expression'],p['seed'],bool(g),g.get('temperature',1),g.get('top_p',1),g.get('top_k',50),g.get('max_new_tokens',2400),g.get('repetition_penalty',1),
            json.dumps(p['pronunciation_rules']),p['strategy'],p['singular'],s['trim'],s['leading_ms'],s['trailing_ms'],s['gap_ms'],s['crossfade_ms'],s['fade_ms'],s['normalize'],s.get('edge_policy','legacy')]
    def draft(cid,description,mood,pace,expression,seed,custom,temp,top_p,top_k,tokens,rep,rules,strategy,singular,trim,leading,trailing,gap,crossfade,fade,normalize,edge_policy):
        p=studio.default_profile(cid);p.update(description=description,mood=mood,pace=pace,expression=expression,seed=int(seed),
            generation_overrides=dict(temperature=float(temp),top_p=float(top_p),top_k=int(top_k),max_new_tokens=int(tokens),repetition_penalty=float(rep)) if custom else {},
            pronunciation_rules=json.loads(rules or '{}'),strategy=strategy,singular=bool(singular),
            processing=dict(trim=bool(trim),leading_ms=float(leading),trailing_ms=float(trailing),gap_ms=float(gap),crossfade_ms=float(crossfade),fade_ms=float(fade),normalize=bool(normalize)))
        if edge_policy!='legacy':p['processing']['edge_policy']=edge_policy
        return studio.validate_profile(cid,p)
    def values(price,seller,qty,unit,spice):return dict(price=str(price),seller_price=str(seller),quantity=str(qty),unit=unit,spice=spice)
    def display(result):
        if not result:return [None,None,None,gr.update(choices=[],value=None),None,[],None,'No matching saved audio.']
        parts=result['segments'];labels=[(str(i+1)+' · '+p['tts_input'],i) for i,p in enumerate(parts)]
        table=[[p['index']+1,p['kind'],p['tts_input'],p['raw']['fingerprint'][:16],round(p['processing']['gain_db'],2),len(p['processing']['warnings'])] for p in parts]
        warnings='\n'.join(result['warnings']+[w for p in parts for w in p['processing']['warnings']])
        for name,m in result['metrics'].items():
            if m['intended_words_per_minute']>300:warnings+='\n'+name+': unusually short; possible omitted/truncated speech. Listen before approval.'
        return [result['full_wav'],result['assembled_wav'],result['record_path'],gr.update(choices=labels,value=0 if labels else None),parts[0]['processed_path'] if parts else None,table,result,warnings]
    def history(cid,id):
        # The legacy A/B controls cannot represent the HTML browser's zero-number policy.
        found=[r for r in studio.results(cid,id) if not r['profile']['processing'].get('zero_number_boundaries')]
        return gr.update(choices=[(r['created_utc'][:19]+' · '+r['plan']['strategy']+' · '+str(r['plan']['values']),r['record_path']) for r in found],value=None)
    def refresh(cid,id,price,seller,qty,unit,spice,*settings):
        fields=set(studio.row(cid,id)['placeholders'])
        visibility=[gr.update(visible=bool(fields&{'currentBuyerOffer','counterPrice','finalPrice'})),gr.update(visible='offeredPrice' in fields),
            gr.update(visible='quantityLabel' in fields),gr.update(visible='quantityLabel' in fields),gr.update(visible=bool(fields&{'spiceName','spiceDescriptor'}))]
        try:
            p=draft(cid,*settings);plan,result=find_saved(studio,cid,id,values(price,seller,qty,unit,spice),p)
            metadata=dict(source=plan['source_file']+':'+str(plan['source_line']),scenario=plan['scenario'],restrictions=plan['restrictions'],warnings=plan['warnings'])
            text=[plan['original_template'],plan['unity_resolved_text'],plan['tts_input'],json.dumps(plan['segments'],indent=2,ensure_ascii=False),json.dumps(metadata,indent=2,ensure_ascii=False)]
            message='Saved comparison ready. Listening approval pending.' if result else 'No saved comparison for these values/settings. Generate both versions below.'
            if result and any(m['intended_words_per_minute']>300 for m in result['metrics'].values()):message+=' Warning: unusually short take; check for missing words.'
            return [*text,*visibility,*display(result),gr.update(visible=result is None,interactive=True),message,history(cid,id),'NEEDS REVIEW','',None]
        except Exception as exc:
            return ['','','','','',*visibility,*display(None),gr.update(visible=True,interactive=False),'Please check the values/settings: '+str(exc),history(cid,id),'NEEDS REVIEW','',None]
    def run(cid,id,price,seller,qty,unit,spice,*settings,generate=True):
        try:
            p=draft(cid,*settings);studio.run(cid,id,values(price,seller,qty,unit,spice),p,generate=generate)
            return refresh(cid,id,price,seller,qty,unit,spice,*settings)
        except Exception as exc:
            result=refresh(cid,id,price,seller,qty,unit,spice,*settings);result[19]='Could not prepare comparison: '+str(exc)
            return result
    with gr.Tab('Indian NPCs · audited full vs segments'):
        gr.Markdown('## Indian NPC voice comparison\nChoose a character and dialogue. Saved audio loads automatically; new audio is generated only when you ask.')
        with gr.Row():
            char=gr.Dropdown([(name+' — '+speaker,cid) for cid,(name,speaker,_) in ASSIGNMENTS.items()],value=first,label='Character')
            dialogue=gr.Dropdown(choices(first),value=choices(first)[0][1],label='Dialogue')
        with gr.Row():
            price=gr.Textbox(value='234',label='Price (varahas)')
            seller=gr.Textbox(value='300',label='Seller asking price (varahas)')
            qty=gr.Textbox(value='2',label='Quantity')
            unit=gr.Dropdown(['Palam','Seer','Veesai','Manangu','Bahar'],value='Seer',label='Unit')
            spice=gr.Dropdown(['pepper','cinnamon','cardamom'],value='pepper',label='Spice')
        spoken=gr.Textbox(label='Spoken dialogue',interactive=False,lines=3)
        notice=gr.Markdown()
        generate=gr.Button('Generate Comparison',variant='primary')
        with gr.Row():
            full_audio=gr.Audio(label='Full Sentence',type='filepath')
            joined_audio=gr.Audio(label='Assembled from Segments',type='filepath')
        with gr.Row():
            review=gr.Radio(['APPROVED','REGENERATE','NEEDS REVIEW'],value='NEEDS REVIEW',label='Listening review')
            notes=gr.Textbox(label='Listening notes',lines=2)
            save_review=gr.Button('Save Review')
        current=gr.State()
        with gr.Accordion('Advanced Settings',open=False):
            gr.Markdown('Draft settings only. Approved presets and original recordings stay unchanged. Changes never synthesize automatically.')
            voice=gr.Textbox(value=profile['speaker'],label='Locked speaker',interactive=False)
            desc=gr.Textbox(value=profile['description'],label='Voice description',lines=3)
            with gr.Row():
                mood=gr.Dropdown(['neutral','friendly','stern','impatient','happy','persuasive','authoritative'],value=profile['mood'],label='Delivery mood')
                pace=gr.Dropdown(['slow','moderate','brisk'],value=profile['pace'],label='Pace description (not guaranteed speed)')
                expression=gr.Dropdown(['restrained','moderate','high'],value=profile['expression'],label='Expression')
                seed=gr.Number(value=profile['seed'],precision=0,label='Seed (not an identity guarantee)')
            rules=gr.Textbox(value=json.dumps(profile['pronunciation_rules']),label='Pronunciation dictionary JSON (explicit review)',lines=2)
            custom=gr.Checkbox(value=bool(profile['generation_overrides']),label='Override supported model sampling parameters')
            with gr.Row():
                temperature=gr.Slider(.1,2,value=profile['generation_overrides'].get('temperature',1),label='Temperature')
                top_p=gr.Slider(.05,1,value=profile['generation_overrides'].get('top_p',1),label='Top p')
                top_k=gr.Slider(1,200,value=profile['generation_overrides'].get('top_k',50),step=1,label='Top k')
                tokens=gr.Slider(128,2610,value=profile['generation_overrides'].get('max_new_tokens',2400),step=1,label='Maximum new tokens')
                repetition=gr.Slider(1,2,value=profile['generation_overrides'].get('repetition_penalty',1),label='Repetition penalty')
            strategy=gr.Radio(['Phrase numbers','Compact grammar'],value=profile['strategy'],label='Segmentation strategy')
            singular=gr.Checkbox(value=profile['singular'],label='REVIEW policy: one varaha (OFF preserves Unity wording)')
            preset=gr.Radio(list(JOIN_PRESETS),value='Tight',label='Join preset')
            edge_policy=gr.Dropdown(['legacy','hysteresis-v1'],value=profile['processing'].get('edge_policy','legacy'),label='Speech-edge detector (new: hysteresis-v1)')
            with gr.Row():
                trim=gr.Checkbox(value=profile['processing']['trim'],label='Speech-aware outer-edge trim')
                leading=gr.Slider(0,60,value=profile['processing']['leading_ms'],step=1,label='Leading pad ms')
                trailing=gr.Slider(0,60,value=profile['processing']['trailing_ms'],step=1,label='Trailing pad ms')
                gap=gr.Slider(0,100,value=profile['processing']['gap_ms'],step=1,label='Gap ms (punctuation retains 25/55ms minima)')
                crossfade=gr.Slider(0,10,value=profile['processing']['crossfade_ms'],step=.5,label='Quiet-padding crossfade ms (zero gap only)')
                fade=gr.Slider(0,3,value=profile['processing']['fade_ms'],step=.1,label='Padding fade ms')
                normalize=gr.Checkbox(value=profile['processing']['normalize'],label='Speech-active loudness matching')
            with gr.Row():
                rejoin=gr.Button('Reprocess cached audio (no synthesis)')
                save=gr.Button('Save character draft settings')
            template=gr.Textbox(label='Original Unity template',interactive=False,lines=2)
            unity=gr.Textbox(label='Resolved Unity wording',interactive=False,lines=2)
            plan=gr.Textbox(label='Ordered segment plan',interactive=False,lines=5)
            context=gr.Textbox(label='Source and restrictions',interactive=False,lines=4)
            with gr.Row():
                picker=gr.Dropdown([],label='Individual fragment',type='value')
                part_audio=gr.Audio(label='Fragment playback',type='filepath')
            table=gr.Dataframe(headers=['#','kind','spoken text','raw cache ID','gain dB','edge warnings'],interactive=False,label='Cache / fragment details')
            status=gr.Textbox(label='Validation warnings / debug details',lines=5,interactive=False)
            runs=gr.Dropdown([],label='Optional saved take history (restores its values and settings)')
            file=gr.File(label='Comparison / saved settings JSON')
            review_file=gr.File(label='Review JSON export')
            gr.Markdown('Raw masters are immutable. Reviews persist under testing/indian_auditions/reviews. Historical quantities must match Unity’s canonical unit family; fractions/overflow are rejected. Technical checks do not prove intelligible speech.')
        settings=[desc,mood,pace,expression,seed,custom,temperature,top_p,top_k,tokens,repetition,rules,strategy,singular,trim,leading,trailing,gap,crossfade,fade,normalize,edge_policy]
        arguments=[char,dialogue,price,seller,qty,unit,spice,*settings]
        outputs=[template,unity,spoken,plan,context,price,seller,qty,unit,spice,full_audio,joined_audio,file,picker,part_audio,table,current,status,generate,notice,runs,review,notes,review_file]
        event=dict(concurrency_id='indian-listening-ui',concurrency_limit=1)
        # .input fires only for user edits; programmatic visibility/settings updates cannot loop.
        gr.on([c.input for c in [dialogue,price,seller,qty,unit,spice,*settings]],refresh,arguments,outputs,api_name='indian_refresh',trigger_mode='always_last',**event)
        from gradio.context import Context
        Context.root_block.load(refresh,arguments,outputs,api_name='indian_initial',**event)
        def change_character(cid):
            p=studio.profile(cid)
            return [gr.update(choices=choices(cid),value=choices(cid)[0][1]),p['speaker'],*settings_for(p)]
        char.input(change_character,char,[dialogue,voice,*settings],api_name='indian_character',**event).then(refresh,arguments,outputs,**event)
        generate.click(lambda *a:run(*a,generate=True),arguments,outputs,api_name='indian_generate',**event)
        rejoin.click(lambda *a:run(*a,generate=False),arguments,outputs,api_name='indian_reprocess',**event)
        save.click(lambda cid,*a:studio.save_profile(cid,draft(cid,*a)),[char,*settings],file,**event)
        preset.input(lambda name:[JOIN_PRESETS[name][k] for k in ['trim','leading_ms','trailing_ms','gap_ms','crossfade_ms','fade_ms','normalize']]+[JOIN_PRESETS[name].get('edge_policy','legacy')],preset,[trim,leading,trailing,gap,crossfade,fade,normalize,edge_policy],**event).then(refresh,arguments,outputs,**event)
        picker.input(lambda index,r:r['segments'][int(index)]['processed_path'] if r and index is not None else None,[picker,current],part_audio,**event)
        def restore(path,cid,id):
            r=load(path)
            if r['plan']['character_id']!=cid or r['plan']['dialogue_id']!=id:raise ValueError('Take belongs to a different character/dialogue.')
            p=studio.validate_profile(cid,r['profile']);v=r['plan']['values']
            return [str(v['price']),str(v['seller_price']),str(v['quantity']),v['unit'],v['spice'],*settings_for(p)]
        runs.input(restore,[runs,char,dialogue],[price,seller,qty,unit,spice,*settings],api_name='indian_restore',**event).then(refresh,arguments,outputs,**event)
        save_review.click(lambda r,s,n:studio.review(r['comparison_id'],s,n) if r else None,[current,review,notes],review_file,**event)
    return studio
