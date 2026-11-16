"""Install shared output standard and isolated sampling, without changing approved voices."""
import json,shutil,hashlib
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');STAGE=Path(__file__).resolve().parent
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def main():
    data=ROOT/'testing/indian_auditions';stamp=datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')
    backup=ROOT/'maintenance_backups/voice_consistency'/stamp;backup.mkdir(parents=True)
    protected=json.loads((data/'preservation_baseline.json').read_text())
    for folder in [ROOT/'samples',ROOT/'production/validated',data/'settings']:
        protected.update({str(p):sha(p) for p in folder.rglob('*') if p.is_file()})
    (backup/'preservation.json').write_text(json.dumps(protected,indent=2),encoding='utf-8')
    for name in ['speech_loudness.py','indian_testing.py','indian_browser_api.py','test_voice_consistency.py']:
        target=ROOT/name
        if target.exists():shutil.copyfile(target,backup/name)
        target.write_bytes((STAGE/name).read_bytes())
    app=ROOT/'app.py';text=app.read_text(encoding='utf-8')
    if 'isolated-seed-v1' not in text:
        shutil.copyfile(app,backup/'app.py')
        text=text.replace('        torch.manual_seed(seed)\n','',1)
        text=text.replace('            with torch.inference_mode():\n','            with torch.random.fork_rng(devices=[0]), torch.inference_mode():\n                torch.manual_seed(seed)\n',1)
        text=text.replace('                "seed": seed,','''                "seed": seed,
                "seed_policy": "isolated-seed-v1; global CUDA/CPU RNG restored after inference",
                "runtime": {"torch": torch.__version__, "dtype": str(self.model.dtype), "device": torch.cuda.get_device_name(0), "eval": not self.model.training},''',1)
        compile(text,str(app),'exec');app.write_text(text,encoding='utf-8')
    requirements=ROOT/'requirements.txt';text=requirements.read_text(encoding='utf-8')
    if 'pyloudnorm==' not in text:
        shutil.copyfile(requirements,backup/'requirements.txt');requirements.write_text(text.rstrip()+'\npyloudnorm==0.2.0\n',encoding='utf-8')
    import sys
    sys.path.insert(0,str(ROOT))
    from indian_testing import TestingStudio,ASSIGNMENTS
    from speech_loudness import POLICY
    studio=TestingStudio();browser=data/'recording_browser'
    path=browser/'voice_sessions.json'
    if not path.exists():path.write_text(json.dumps({cid:studio.profile(cid) for cid in ASSIGNMENTS},indent=2),encoding='utf-8')
    path=browser/'loudness_policy.json'
    if not path.exists():path.write_text(json.dumps(POLICY,indent=2),encoding='utf-8')
    assert all(Path(p).is_file() and sha(p)==digest for p,digest in protected.items())
    print('Installed shared loudness policy; frozen test profiles; original presets, sources and number timing preserved. Backup:',backup)
if __name__=='__main__':main()
