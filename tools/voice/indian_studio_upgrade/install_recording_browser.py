"""Extend the existing Portuguese page and existing Gradio backend, preserve assets."""
import sys,json,shutil,hashlib
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');STAGE=Path(__file__).resolve().parent;REPO=ROOT.parent/'AI-Enabled-Storytelling-VR'
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def main():
    stamp=datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f');backup=ROOT/'maintenance_backups/indian_recording_browser'/stamp;backup.mkdir(parents=True)
    data=ROOT/'testing/indian_auditions/recording_browser';data.mkdir(parents=True,exist_ok=True)
    protected=json.loads((data.parent/'preservation_baseline.json').read_text())
    for folder in [ROOT/'samples',ROOT/'production/validated',ROOT/'testing/indian_auditions/settings']:
        protected.update({str(p):sha(p) for p in folder.rglob('*') if p.is_file()})
    (backup/'preservation.json').write_text(json.dumps(protected,indent=2),encoding='utf-8')
    catalog=dict(characters={},starter_batches={})
    names={'Lakshmamma':'lakshmi_amma','ChinnammaNaik':'chinnamma_naik','SaraswatiChetti':'saraswati_chetti','AbdulRahman':'abdul_rahman'}
    for folder,cid in names.items():
        source=REPO/'documentation/DialogueAudit'/(folder+'_Discovery')/'DIALOGUE_INVENTORY.json';inv=json.loads(source.read_text(encoding='utf-8'))
        for r in inv['records']:
            if r['category']=='AUTHORED':assert r['exact_template'] in (REPO/r['source_file']).read_text(encoding='utf-8-sig').splitlines()[r['source_line']-1],r['inventory_id']
        catalog['characters'][cid]=dict(audition_ids=inv['test_selection_ids'],records=inv['records'],audit_path=str(source),audit_sha256=sha(source))
        scenarios=['CustomerGreeting','BuyerCounterFirst','BuyerCounterMiddle','SellerPriceAccepted','SellerPriceTooHigh','HostileWarning','QuantityChanged']
        rows=[r for r in inv['records'] if r['inventory_id'] in inv['test_selection_ids']]
        ids=[next(r['inventory_id'] for r in rows if r['scenario']==scenario) for scenario in scenarios]
        ids.append(next(r['inventory_id'] for r in inv['records'] if r['category']=='AUTHORED' and r['scenario']=='HistoryKing' and r['coverage_class']!='UNREACHABLE_OR_DEFERRED'))
        catalog['starter_batches'][cid]=ids
    if (data/'catalog.json').exists():shutil.copyfile(data/'catalog.json',backup/'catalog.json')
    (data/'catalog.json').write_text(json.dumps(catalog,indent=2,ensure_ascii=False),encoding='utf-8')
    base=ROOT/'production/batches/portuguese_ready_20261027';v3=base/'verification_studio_v3'
    for name,target in [('indian_testing.py',ROOT/'indian_testing.py'),('indian_browser_api.py',ROOT/'indian_browser_api.py'),('indian-browser.js',v3/'indian-browser.js'),('serve_studio.py',v3/'serve_studio.py')]:
        if target.exists():shutil.copyfile(target,backup/target.name)
        target.write_bytes((STAGE/name).read_bytes())
    page=base/'VOICE_VERIFICATION_STUDIO_v3.html';text=page.read_text(encoding='utf-8')
    if 'src="verification_studio_v3/indian-browser.js"' not in text:
        shutil.copyfile(page,backup/page.name);text=text.replace('</body>','<script src="verification_studio_v3/indian-browser.js"></script></body>');page.write_text(text,encoding='utf-8')
    app=ROOT/'app.py';text=app.read_text(encoding='utf-8')
    if '# INDIAN_BROWSER_API' not in text:
        shutil.copyfile(app,backup/'app.py')
        old='    demo.launch(server_name="127.0.0.1", server_port=7860, share=False, inbrowser=True, allowed_paths=[str(SAMPLES), str(EXPORTS), str(ROOT / "testing/indian_auditions")])'
        assert old in text
        new='''    # INDIAN_BROWSER_API: same Parler session and Gradio app, extra local browsing routes.
    from fastapi import FastAPI
    import uvicorn, webbrowser
    from indian_browser_api import mount_api
    service = FastAPI()
    recording_browser = mount_api(service, SESSION)
    service = gr.mount_gradio_app(service, demo, path="/", allowed_paths=[str(SAMPLES), str(EXPORTS), str(ROOT / "testing/indian_auditions")])
    port = int(os.environ.get("ANUBHAVA_STUDIO_PORT", "7860"))
    if os.environ.get("ANUBHAVA_STUDIO_NO_BROWSER") != "1":
        threading.Timer(2, lambda: webbrowser.open(f"http://127.0.0.1:{port}")).start()
    uvicorn.run(service, host="127.0.0.1", port=port)
'''
        text=text.replace(old,new);compile(text,str(app),'exec');app.write_text(text,encoding='utf-8')
    assert all(Path(p).is_file() and sha(p)==h for p,h in protected.items()),'Protected data changed'
    print('Installed into EXISTING Portuguese HTML + existing Gradio server. Inventory counts:',{cid:len(c['records']) for cid,c in catalog['characters'].items()},'Protected:',len(protected),'Backup:',backup)
if __name__=='__main__':main()
