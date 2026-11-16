"""Deploy only the UI and its focused tests, with preservation checks."""
import hashlib,json,shutil
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator')
STAGE=Path(__file__).resolve().parent
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
    data=ROOT/'testing/indian_auditions'
    files=[ROOT/'app.py',ROOT/'indian_testing.py',ROOT/'launch.bat']
    files += [p for folder in [ROOT/'samples/09_Indian_Dialogue_AB',data/'settings'] for p in folder.rglob('*') if p.is_file()]
    baseline={str(p):sha(p) for p in files}
    baseline.update(json.loads((data/'preservation_baseline.json').read_text()))
    stamp=datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')
    backup=ROOT/'maintenance_backups/indian_ui_simplification'/stamp;backup.mkdir(parents=True)
    (backup/'preservation.json').write_text(json.dumps(baseline,indent=2),encoding='utf-8')
    for name in ['indian_testing_ui.py','test_indian_testing_ui.py']:
        target=ROOT/name
        if target.exists():shutil.copyfile(target,backup/name)
        target.write_bytes((STAGE/name).read_bytes())
    guide=ROOT/'INDIAN_AB_TESTING.md'
    text=guide.read_text(encoding='utf-8')
    begin=text.index('## Open and listen');end=text.index('## Actual starter batch')
    updated='''## Open and listen
1. Restart the existing `launch.bat` and open http://127.0.0.1:7860. Keep only one studio process on that port.
2. Select **Indian NPCs · audited full vs segments**, then choose a character and readable dialogue label (Greeting, Opening Offer, Counteroffer, Agreement, etc.). Matching saved audio loads automatically, including on initial page load.
3. Read **Spoken dialogue** and compare **Full Sentence** with **Assembled from Segments**. Only relevant price/spice/quantity/unit controls appear. Editing values refreshes text and matching audio; unavailable audio is cleared.
4. If no matching comparison exists, press **Generate Comparison** to prepare both versions. Cached recordings are reused. Merely opening or changing controls never synthesizes.
5. **Advanced Settings** starts collapsed. It retains independent voice drafts, model controls, phrase/compact strategies, join presets, cached-only reprocessing, fragments, provenance, optional saved-take history and JSON exports. Choosing an old take restores its actual values/settings before loading audio.
6. Save listening outcomes with **Save Review**; notes persist in review JSON. All older studio tabs and approved presets are unchanged. Changing a draft/settings requires a matching take, so incompatible cached audio is never presented as the current text.

'''
    replacement=text[:begin]+updated+text[end:]
    if replacement!=text:
        shutil.copyfile(guide,backup/guide.name);guide.write_text(replacement,encoding='utf-8')
    assert all(Path(p).is_file() and sha(Path(p))==h for p,h in baseline.items())
    print('UI-only update. Preserved',len(baseline),'file hashes; no synthesis. Backup:',backup)
if __name__=='__main__':main()
