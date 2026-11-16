"""Bounded existing-Parler audition batch. No production manifest or package writes."""
import os,sys,json,argparse
from pathlib import Path
os.environ['HF_HUB_OFFLINE']='1';os.environ['TRANSFORMERS_OFFLINE']='1';os.environ['GRADIO_ANALYTICS_ENABLED']='False'
sys.dont_write_bytecode=True
from indian_testing import TestingStudio,ASSIGNMENTS,create_json,now

def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--generate',action='store_true');parser.add_argument('--max-new-raw',type=int,default=120);args=parser.parse_args()
    if not 1<=args.max_new_raw<=120:raise ValueError('Audition budget must be1–120, never a full library batch.')
    import app
    studio=TestingStudio(engine=app.SESSION);studio.maximum_new=args.max_new_raw
    outcomes=[];values=dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')
    required={'CustomerGreeting','BuyerCounterFirst','BuyerCounterMiddle','SellerPriceAccepted'}
    for cid in ASSIGNMENTS:
        rows=[r for r in studio.rows(cid) if r['scenario'] in required]
        expressive='TransactionSuccess' if cid=='abdul_rahman' else 'HostileWarning'
        rows.append(next(r for r in studio.rows(cid) if r['scenario']==expressive))
        assert len(rows)==5
        p=studio.profile(cid)
        for row in rows:
            for strategy in ['Phrase numbers','Compact grammar'] if row['placeholders'] else ['Phrase numbers']:
                q=dict(p,strategy=strategy)
                if not args.generate:
                    from indian_testing import make_plan
                    plan=make_plan(row,**values,strategy=strategy,singular=q['singular'],rules=q['pronunciation_rules'])
                    print('PLAN',cid,row['inventory_id'],strategy,len(plan['segments']),'segments',flush=True);continue
                try:
                    r=studio.run(cid,row['inventory_id'],values,q,generate=True);outcomes.append(dict(character_id=cid,dialogue_id=row['inventory_id'],strategy=strategy,status='TECHNICALLY_VALID_NEEDS_LISTENING',record=r['record_path']))
                    print('READY',cid,row['inventory_id'],strategy,r['assembled_wav'],flush=True)
                except Exception as exc:
                    outcomes.append(dict(character_id=cid,dialogue_id=row['inventory_id'],strategy=strategy,status='FAILED',error=str(exc)));print('FAILED',outcomes[-1],flush=True)
                    if 'memory' in str(exc).lower() and 'cuda' in str(exc).lower():
                        create_json(studio.data/('pilot_failed_'+now().replace(':','-')+'.json'),dict(outcomes=outcomes,new_raw=studio.new_generations));raise
    if args.generate:
        report=dict(time=now(),maximum_new_raw=studio.maximum_new,new_raw=studio.new_generations,outcomes=outcomes,
            successful=sum(r['status']=='TECHNICALLY_VALID_NEEDS_LISTENING' for r in outcomes),failed=sum(r['status']=='FAILED' for r in outcomes),
            human_approval=False,full_batch_enabled=False)
        dest=studio.data/('pilot_'+now().replace(':','-')+'.json');create_json(dest,report);print('REPORT',dest,flush=True)
        if report['failed']:sys.exit(1)

if __name__=='__main__':main()
