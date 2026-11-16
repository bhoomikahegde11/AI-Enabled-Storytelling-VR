"""Explicit eight-context-per-character audition, never the full inventory."""
import os,sys,json
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');sys.path.insert(0,str(ROOT))
os.environ['HF_HUB_OFFLINE']='1';os.environ['TRANSFORMERS_OFFLINE']='1'
def main():
    if '--generate' not in sys.argv:raise SystemExit('Explicit --generate required; maximum 32 selected contexts, 100 new raw takes.')
    import app
    from indian_browser_api import RecordingBrowser
    from indian_testing import ASSIGNMENTS,create_json
    browser=RecordingBrowser(app.SESSION);browser.studio.maximum_new=100;outcomes=[]
    values=dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')
    for cid in ASSIGNMENTS:
        for id in browser.batch_ids[cid]:
            print('BATCH START',cid,id,flush=True);before=browser.studio.new_generations
            try:
                r=browser.generate(cid,id,values,'Both');outcomes.append(dict(character_id=cid,dialogue_id=id,status='NEEDS LISTENING',key=r['key'],new_raw=browser.studio.new_generations-before,audio=r['audio']))
                print('BATCH READY',cid,id,'new_raw',browser.studio.new_generations-before,flush=True)
            except Exception as e:
                outcomes.append(dict(character_id=cid,dialogue_id=id,status='FAILED',error=str(e)));print('BATCH FAILED',cid,id,repr(e),flush=True)
                if 'out of memory' in str(e).lower() or browser.studio.new_generations>=100:break
        if browser.studio.new_generations>=100 or (outcomes and 'out of memory' in outcomes[-1].get('error','').lower()):break
    report=dict(created=datetime.now(timezone.utc).isoformat(),new_raw=browser.studio.new_generations,outcomes=outcomes,scope='8 contexts per Indian character; phrase numbers; cached reuse; no number libraries')
    path=browser.folder/('batch_'+datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S')+'.json');create_json(path,report)
    print('BATCH REPORT',path,'NEW RAW',report['new_raw'],'SUCCEEDED',sum(r['status']!='FAILED' for r in outcomes),'FAILED',sum(r['status']=='FAILED' for r in outcomes),flush=True)
if __name__=='__main__':main()
