"""Validate the limited real audition batch and write listening instructions."""
import csv,json,hashlib,sys
from collections import Counter
from pathlib import Path
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator')
sys.path.insert(0,str(ROOT))
import soundfile as sf
import numpy as np
from indian_testing import words,ASSIGNMENTS
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def main():
    data=ROOT/'testing/indian_auditions'
    pilot=json.loads(sorted(data.glob('pilot_*.json'))[-1].read_text())
    outcomes=pilot['outcomes'];assert len(outcomes)==36
    rows=[];raws={};durations=[]
    for outcome in outcomes:
        assert outcome['status']=='TECHNICALLY_VALID_NEEDS_LISTENING',outcome
        r=json.loads(Path(outcome['record']).read_text());cid=outcome['character_id']
        assert r['profile']['speaker']==ASSIGNMENTS[cid][1]
        assert words(r['plan']['tts_input'])==words(' '.join(s['tts_input'] for s in r['segments']))
        for entry in [r['full_raw']]+[s['raw'] for s in r['segments']]:
            assert entry['character_id']==cid and entry['speaker']==ASSIGNMENTS[cid][1]
            assert sha(entry['path'])==entry['sha256'];raws[entry['path']]=entry
        for key in ['full_wav','assembled_wav']:
            assert sha(r[key])==r[key+'_sha256']
            x,sr=sf.read(r[key]);assert sr==44100 and len(x)>0 and np.isfinite(x).all() and abs(x).max()<.9999
        for s in r['segments']:assert sha(s['processed_path'])==s['processed_sha256']
        full=r['metrics']['full']['duration_seconds'];assembled=r['metrics']['assembled']['duration_seconds']
        durations.append(assembled/full)
        rows.append(dict(character=ASSIGNMENTS[cid][0],speaker=ASSIGNMENTS[cid][1],dialogue_id=outcome['dialogue_id'],strategy=outcome['strategy'],
            exact_spoken_text=r['plan']['tts_input'],full_seconds=full,assembled_seconds=assembled,segment_count=len(r['segments']),
            full_wav=r['full_wav'],assembled_wav=r['assembled_wav'],comparison_json=outcome['record'],warnings=' | '.join(r['warnings']+[
                name+': unusually short; possible missing speech' for name,m in r['metrics'].items() if m['intended_words_per_minute']>300]),review='NEEDS REVIEW'))
    for entry in raws.values():
        x,sr=sf.read(entry['path']);assert sr==44100 and len(x)>0 and np.isfinite(x).all() and abs(x).max()<.9999
    protected=json.loads((data/'preservation_baseline.json').read_text())
    assert all(sha(p)==h for p,h in protected.items())
    counts=Counter(entry['character_id'] for entry in raws.values())
    fulls={json.loads(Path(o['record']).read_text())['full_raw']['path'] for o in outcomes}
    target=data/'PILOT_LISTENING_INDEX.csv'
    with target.open('w',encoding='utf-8-sig',newline='') as f:
        w=csv.DictWriter(f,fieldnames=list(rows[0]));w.writeheader();w.writerows(rows)
    summary=dict(comparisons=len(rows),unique_dialogue_contexts=len({(r['character'],r['dialogue_id']) for r in rows}),
        unique_raw=len(raws),full_sentence_raw=len(fulls),independent_fragment_raw=len(set(raws)-fulls),raw_by_character=dict(counts),
        failures=0,possible_short_full_takes=sorted({r['dialogue_id'] for r in rows if 'full: unusually short' in r['warnings']}),native_sample_rate=44100,protected_hashes_verified=len(protected),unit_tests_passed=13,
        localhost_cached_reprocess_and_wav_download='PASS',human_listening='NOT_PERFORMED',
        maximum_assembled_to_full_duration_ratio=round(max(durations),2))
    (data/'VALIDATION_SUMMARY.json').write_text(json.dumps(summary,indent=2),encoding='utf-8')
    table='\n'.join('| '+ASSIGNMENTS[c][0]+' | '+ASSIGNMENTS[c][1]+' | '+str(counts[c])+' | 5 | 9 |' for c in ASSIGNMENTS)
    text=f'''# Four Indian NPCs — existing-studio A/B testing

## Open and listen
1. Run `{ROOT / 'launch.bat'}`. Close/restart an older studio process first if port 7860 is occupied. This launches the existing Gradio app, using only the locally cached Parler model.
2. Open http://127.0.0.1:7860 and select **Indian NPCs · audited full vs segments** (the first tab). Existing studio tabs remain available.
3. Choose a character and dialogue. Click **Refresh saved comparisons**, select **Phrase numbers** or **Compact grammar** in the history list, then **Load saved comparison**. Loading/replaying does not synthesize.
4. Compare A (complete sentence) and B (independent segments); use the individual segment player and download controls. The stored comparison includes the actual source values/configuration in its provenance JSON. To edit a loaded comparison, set the controls to those recorded values first; loading history does not silently overwrite draft voice settings.
5. To compare joins, select Tight, Near contiguous or Original edges, then **Reprocess cached raw audio only**. It fails explicitly if the required raw takes/settings are not cached. **Generate / reuse** synthesizes only missing components for the selected test, with a 120-new-take session ceiling. It never launches a full-number library.
6. Save independent character drafts with **Save this character’s TEST draft**. Record listening outcomes/notes with **Save / export review JSON**. Existing approved presets remain untouched.

## Actual starter batch
| Character | Locked speaker | New raw WAVs | Unique dialogue contexts | A/B comparisons |
|---|---|---:|---:|---:|
{table}

Total: **{len(raws)} raw WAVs = {len(fulls)} complete-sentence takes + {len(set(raws)-fulls)} independent fragments**, reused across **36 comparison records / 20 dialogue contexts**. Every character has greeting, opening offer, middle counter, agreement and a fixed expressive line. Four dynamic contexts have both segmentation strategies; the fixed line uses one intact recording. No generation failures. No further audio generated during validation.

All 48 audited selections (12/character) are available; only five/character were synthesized. Pilot dynamic values are 234 varahas, seller ask 300, two Seers, pepper. This is an audition batch, not production coverage.

## Files
- Raw immutable masters: `samples/09_Indian_Dialogue_AB/raw/<character_id>/` (WAV + original generator metadata).
- Full/assembled/individual processed WAVs: `samples/09_Indian_Dialogue_AB/comparisons/<character_id>/<comparison_hash>/`.
- Exact paths, intended words, durations and warnings: `testing/indian_auditions/PILOT_LISTENING_INDEX.csv`.
- Machine validation: `testing/indian_auditions/VALIDATION_SUMMARY.json`.
- Imported source metadata: `testing/indian_auditions/library.json`.
- Independent current drafts and immutable snapshots: `testing/indian_auditions/settings/<character_id>/`.
- Review JSON: `testing/indian_auditions/reviews/`. These files survive reopening; export downloads a copy. Reviews are not automatic approval or production-manifest changes.

## Supported comparison policies
- Phrase numbers records a resolved number/quantity as a larger natural phrase. Compact grammar records ordered components (e.g. two / hundred / and / thirty / four), cached only within the same character/configuration. Both must contain exactly the same spoken words as the full take.
- Positive Int32 prices through 2,147,483,647 are expanded to English words; no Father-specific 999 limit. This is a technical text bound, not a claim every price is economically sensible. Large-number audio is generated on demand only.
- Products: pepper, cinnamon, cardamom. Historical unit families: Palam, Seer, Veesai, Manangu, Bahar. Whole-number counts must preserve Unity’s canonical quantity family and fit integer grams. Fractions are rejected rather than silently approximated.
- Original Unity wording, including awkward grammar, remains separate and unchanged. Singular `one varaha` is an explicit optional review policy; default preserves source `one varahas`. Explicit pronunciation dictionaries apply identically to A/B.
- Trimming operates only on outer speech-aware edges; risky edges retain protective padding. Internal pauses remain intact. Tiny fades stay inside padding; crossfades require zero gap and protected quiet padding. Punctuation retains clause/sentence pauses. No pitch/speed changes.
- Speech-active weighted RMS matching is limited to the current character/full take (+4/-9 dB gain limits, -2 dB peak ceiling). It is not LUFS normalization and cannot repair a theatrical performance.
- Pace is a text description, not a guaranteed speed control. Optional actual sampling overrides are separately exposed; defaults preserve the existing model settings. Repeating a seed/description does not guarantee an identical delivery.

## Checks and honest limitations
**13 non-Unity tests passed**, covering existing normalization, all 48 plans, prices 1–999 with both grammars, selected larger bounds, quantity guards, word equality, voice isolation, cache reuse, draft safety and protected-edge DSP. Existing app construction and localhost cached-reprocess endpoint passed; downloaded A/B WAVs decoded at 44.1 kHz. All 94 real raw WAVs and 72 A/B output paths validated; source/processed hashes and {len(protected)} preservation hashes passed.

No audio was listened to or approved by software. No offline ASR is installed in this environment. Pronunciation, missing/repeated speech, accent authenticity, voice drift and consonant integrity require human review. Some takes have internal pauses or speech close to source edges; the UI exposes warnings. Assembled/full duration ratios reach {max(durations):.2f}×: compact components can sound substantially slower despite tight joins. Do not mistake fewer milliseconds between files for natural connected speech. Prefer comparing phrase-number assemblies first.

Abdul’s retained Kabir identity is a selected production voice; matching his Arab caravan-trader background remains a listening/creative decision. Suspected source wording is shown, never silently fixed. Unity scripts/assets, Father’s package, Qwen settings, approved presets and production manifests were not changed. No Unity tests, builds, mass production, commits or pushes were performed.

## Studio implementation files
Modified `app.py` (existing shared Parler generation hook, serialized model calls, new tab) and `launch.bat` (offline flags). Added `indian_testing.py`, `indian_testing_ui.py`, `run_indian_test_pilot.py`, `test_indian_testing.py`. Original app/launcher and changed extension versions are retained under `maintenance_backups/indian_studio_upgrade/`. Installer/testing source is also retained in the Unity repository’s `tools/voice/indian_studio_upgrade/`; no Unity gameplay or assets were edited.
'''
    (ROOT/'INDIAN_AB_TESTING.md').write_text(text,encoding='utf-8')
    print(json.dumps(summary,indent=2))
if __name__=='__main__':main()
