"""Bounded 21-new-full-clip audition; reuse baseline takes and raw dynamic fragments."""
import sys,os,json,copy
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');sys.path.insert(0,str(ROOT))
os.environ['HF_HUB_OFFLINE']='1';os.environ['TRANSFORMERS_OFFLINE']='1'

def main():
    if '--generate' not in sys.argv:raise SystemExit('Explicit --generate required; no more than 21 new full takes.')
    import app
    from indian_browser_api import RecordingBrowser
    from indian_testing import load,sha,fingerprint,create_json,make_plan,ASSIGNMENTS
    from speech_loudness import derivative,measure
    import soundfile as sf
    browser=RecordingBrowser(app.SESSION);base=browser.folder/'consistency_pass';base.mkdir(parents=True,exist_ok=True)
    run=datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S');folder=base/'runs'/run;folder.mkdir(parents=True)
    policy=load(browser.folder/'loudness_policy.json');values=dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')
    entries=[];failures=[];processed_pairs=[];new_alt=0
    report=dict(run=run,entries=entries,failures=failures,processed_pairs=processed_pairs,policy=policy,approved=False,maximum_new_full_takes=21,
        candidate_voices=['Sneha','Mary','Meera','Gauri'],candidate_inventory_source='https://huggingface.co/ai4bharat/indic-parler-tts#-using-a-specific-speaker')
    def save():
        report['new_full_takes']=browser.studio.new_generations+new_alt
        temp=folder/'REPORT.tmp';temp.write_text(json.dumps(report,indent=2),encoding='utf-8');temp.replace(folder/'REPORT.json')
        pointer=base/'latest.tmp';pointer.write_text(json.dumps(dict(run=run)),encoding='utf-8');pointer.replace(base/'latest.json')
    def entry(cid,row,plan,profile,raw_path,treatment,source_meta):
        exported=derivative(raw_path,browser.studio.output/'consistent_exports'/cid,policy,plan['tts_input'])
        record=dict(character_id=cid,dialogue_id=row['inventory_id'],scenario=row['scenario'],speaker=profile['speaker'],treatment=treatment,
            original_template=row['exact_template'],spoken_text=plan['tts_input'],source_file=row['source_file'],source_line=row['source_line'],settings=profile,
            raw=dict(path=str(raw_path),sha256=sha(raw_path)),normalized=dict(path=exported['path'],sha256=exported['sha256']),
            loudness=exported,status='NEEDS LISTENING',generation_metadata=source_meta)
        entries.append(record);save();print('QUALITY READY',cid,row['inventory_id'],profile['speaker'],treatment,exported['status'],flush=True)
    save()
    # Retained voices: identical descriptions and texts; only a clearly labelled sampling/seed proposal changes.
    for cid in ['lakshmi_amma','chinnamma_naik','abdul_rahman']:
        for scenario in ['CustomerGreeting','BuyerCounterMiddle','SellerPriceAccepted']:
            row=next(r for r in browser.studio.rows(cid) if r['inventory_id'] in browser.selected[cid] and r['scenario']==scenario)
            profile=browser.profile(cid);plan=make_plan(row,**values,strategy='Phrase numbers',singular=profile['singular'],rules=profile['pronunciation_rules'])
            try:
                raw=browser.studio.raw(cid,plan['tts_input'],profile,row['inventory_id'],False)
                entry(cid,row,plan,profile,raw['path'],'Existing settings · cached reference',load(raw['metadata_path']))
                paired=browser.generate(cid,row['inventory_id'],values,'Both',allow_synthesis=False);processed_pairs.append(dict(character_id=cid,dialogue_id=row['inventory_id'],key=paired['key'],comparison=paired['comparison']))
            except Exception as exc:failures.append(dict(character_id=cid,dialogue_id=row['inventory_id'],stage='cached processing',error=str(exc)));save()
            candidate=copy.deepcopy(profile);candidate['seed_policy']='character-text-v1';candidate['generation_overrides']=dict(temperature=.8,top_p=.95,top_k=50)
            try:
                raw=browser.studio.raw(cid,plan['tts_input'],candidate,row['inventory_id'],True)
                entry(cid,row,plan,candidate,raw['path'],'Controlled sampling · proposed, not approved',load(raw['metadata_path']))
            except Exception as exc:
                failures.append(dict(character_id=cid,dialogue_id=row['inventory_id'],stage='controlled synthesis',error=str(exc)));save();print('QUALITY FAILED',repr(exc),flush=True)
                if 'out of memory' in str(exc).lower():return
    cid='saraswati_chetti';profile=browser.profile(cid)
    for scenario in ['CustomerGreeting','BuyerCounterFirst','BuyerCounterMiddle','HostileWarning']:
        row=next(r for r in browser.studio.rows(cid) if r['inventory_id'] in browser.selected[cid] and r['scenario']==scenario)
        plan=make_plan(row,**values,strategy='Phrase numbers',singular=profile['singular'],rules=profile['pronunciation_rules'])
        for speaker in ['Sneha','Mary','Meera','Gauri']:
            candidate=copy.deepcopy(profile);candidate['speaker']=speaker
            try:
                if speaker=='Sneha':
                    raw=browser.studio.raw(cid,plan['tts_input'],profile,row['inventory_id'],False);path=raw['path'];meta=load(raw['metadata_path'])
                else:
                    assert speaker in app.SPEAKERS
                    key=fingerprint(dict(text=plan['tts_input'],profile=candidate,model=browser.studio.baseline_generation));pointer=base/'candidate_cache'/(key+'.json')
                    if pointer.exists():
                        raw=load(pointer);assert sha(raw['path'])==raw['sha256'];path=raw['path'];meta=load(Path(path).with_suffix('.json'))
                    else:
                        if browser.studio.new_generations+new_alt>=21:raise RuntimeError('Audition cap reached')
                        new_alt+=1
                        print('QUALITY GENERATE',speaker,row['inventory_id'],flush=True)
                        path,meta=app.SESSION.generate(plan['tts_input'],speaker,candidate['description'],candidate['mood'],candidate['pace'],candidate['expression'],
                            preset_name='Saraswati voice alternatives · unapproved',seed=candidate['seed'],dialogue_id=row['inventory_id'],generation_overrides=candidate['generation_overrides'],
                            sample_label='QUALITY_'+speaker+'_'+key[:16],output_directory=ROOT/'samples/10_Voice_Consistency/raw/saraswati_chetti'/speaker)
                        create_json(pointer,dict(path=path,sha256=sha(path),speaker=speaker,character_id=cid,approved=False))
                entry(cid,row,plan,candidate,path,'Sneha · retained reference' if speaker=='Sneha' else 'Saraswati alternative · unapproved',meta)
            except Exception as exc:
                failures.append(dict(character_id=cid,dialogue_id=row['inventory_id'],speaker=speaker,stage='voice alternative',error=str(exc)));save();print('QUALITY FAILED',speaker,repr(exc),flush=True)
                if 'out of memory' in str(exc).lower():return
    save();print('QUALITY REPORT',folder/'REPORT.json','NEW',report['new_full_takes'],'ENTRIES',len(entries),'FAILURES',len(failures),flush=True)
if __name__=='__main__':main()
