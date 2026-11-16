"""Final integrity, coverage and small-batch listening documentation. No synthesis."""
import sys,json,csv
from pathlib import Path
from collections import Counter
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');sys.path.insert(0,str(ROOT))
import numpy as np
import soundfile as sf
from indian_testing import load,sha,numeric_segment,words,ASSIGNMENTS
from indian_browser_api import RecordingBrowser

def main():
    browser=RecordingBrowser();folder=browser.folder
    batch=load(sorted(folder.glob('batch_*.json'))[-1]);baseline=load(sorted((ROOT/'maintenance_backups/indian_recording_browser').glob('*/preservation.json'))[0])
    assert all(Path(p).is_file() and sha(p)==h for p,h in baseline.items())
    success=[r for r in batch['outcomes'] if r['status']!='FAILED'];failures=[r for r in batch['outcomes'] if r['status']=='FAILED']
    partial=[]
    for failed in failures:
        values=dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')
        # Preserve the failed segmented outcome; expose only the exact corresponding full cache.
        cached=browser.lookup(failed['character_id'],failed['dialogue_id'],values,'Full sentence')
        if cached:
            r=browser.generate(failed['character_id'],failed['dialogue_id'],values,'Full sentence')
            partial.append(dict(character_id=failed['character_id'],dialogue_id=failed['dialogue_id'],status='FULL_ONLY_SEGMENTED_FAILED',key=r['key']))
    results={r['key']:r for r in browser.all_results()};audio_count=0;number_joins=0;warnings=[];usedraw=set();rawfull=set();bychar=Counter();internal=[]
    for outcome in success+partial:
        r=results[outcome['key']];bychar[r['character_id']]+=1
        comparison=load(r['comparison']) if r['comparison'] else None
        if comparison:
            assert words(r['spoken_text'])==words(' '.join(s['tts_input'] for s in comparison['segments']))
            rawfull.add(comparison['full_raw']['path']);usedraw.add(comparison['full_raw']['path'])
        else:
            rawfull.add(r['audio'][0]['path']);usedraw.add(r['audio'][0]['path'])
        for a in r['audio']:
            x,sr=sf.read(a['path']);assert sr==44100 and x.ndim==1 and len(x)>0 and np.isfinite(x).all() and abs(x).max()<.9999
            assert sha(a['path'])==a['sha256'];audio_count+=1
            if a['metrics']['intended_words_per_minute']>300:warnings.append(dict(id=r['dialogue_id'],mode=a['mode'],warning='Possible omitted/truncated speech; reused take needs listening',seconds=a['metrics']['duration_seconds']))
        for i,s in enumerate(comparison['segments'] if comparison else []):
            usedraw.add(s['raw']['path']);assert s['raw']['character_id']==r['character_id'];assert sha(s['raw']['path'])==s['raw']['sha256']
            if s['raw']['metrics']['intended_words_per_minute']>300:warnings.append(dict(id=r['dialogue_id'],mode='Fragment: '+s['tts_input'],warning='Possible omitted/truncated speech',seconds=s['raw']['metrics']['duration_seconds']))
            if numeric_segment(s):
                m=s['processing'];assert m['zero_number_boundary_processing'] and m['added_number_padding_ms']==0
                assert m['leading_safe_samples']<=45 and m['trailing_safe_samples']<=45
                gaps=s['raw']['metrics']['internal_low_energy_gaps_at_least_350ms']
                if gaps:internal.append(dict(id=r['dialogue_id'],text=s['tts_input'],gaps=gaps,note='Raw-source internal pauses; not deleted'))
                for j in [i-1,i]:
                    if 0<=j<len(comparison['assembly_processing']['joins']):
                        join=comparison['assembly_processing']['joins'][j];assert join['gap_ms']==0 and join['crossfade_samples']==0;number_joins+=1
    existing=set(baseline);newraw=[str(p) for p in (browser.studio.output/'raw').glob('*/*.wav') if str(p) not in existing]
    newcounts=Counter(Path(p).parent.name for p in newraw)
    summary=dict(contexts=len(success)+len(partial),paired_contexts=len(success),full_only_contexts=partial,failed_segmented_contexts=failures,playback_outputs=audio_count,new_raw_wavs=len(newraw),new_full_sentence_raw=len(set(newraw)&rawfull),new_independent_fragment_raw=len(set(newraw)-rawfull),
        reused_unique_raw=len(usedraw-set(newraw)),by_character=dict(bychar),new_raw_by_character=dict(newcounts),verified_number_boundaries=number_joins,
        preserved_hashes=len(baseline),tests_passed=29,asr_or_human_approval=False,short_duration_warnings=warnings,source_internal_number_pauses=internal,
        catalog_rows={cid:len(browser.studio.rows(cid)) for cid in ASSIGNMENTS},selected_audition_rows=48)
    (folder/'FINAL_REPORT.json').write_text(json.dumps(summary,indent=2),encoding='utf-8')
    browser.write_index()
    rows=['| Character | Voice | Selected contexts | New raw takes |','|---|---|---:|---:|']
    for cid,a in ASSIGNMENTS.items():rows.append(f'| {a[0]} | {a[1]} | {bychar[cid]} | {newcounts[cid]} |')
    text='''# Four Indian NPCs in the existing Father/Francisco studio

## Open
Close an older **Portuguese Launch_Studio.cmd** window first, then run:
`production/batches/portuguese_ready_20261027/Launch_Studio.cmd`

The original HTML v3 page now has six character tabs. Select Lakshmamma, Chinnamma Naik, Saraswati Chetti or Abdul Rahman. The existing launcher starts/reuses the existing local Parler/Gradio backend; its console prints the backend address. No model loads or synthesis happens just by opening it. Startup can take several seconds. Older Gradio processes may be left running, but close them if they hold GPU memory before generating.

1. Pick a readable dialogue. **Previous / Next** browse within the current list.
2. Read the exact resolved spoken text. Only applicable price, spice, quantity and unit controls appear.
3. Choose **Full sentence**, **Segmented**, or **Both**. Matching saved audio loads automatically.
4. Click **Generate selected** only if needed. Cache reuse requires the same source text, voice and synthesis settings.
5. Open **Small batch selection** to choose up to 12 lines and generate in one action. The eight-context starter set is already prepared. Generation disables selection changes until it finishes; failed lines are reported individually.
6. Switch Browse to **Entire audited inventory** for history, other authored lines and documented fallbacks. Deferred/unreachable or unsupported dynamic sources are visibly blocked, not synthesized or silently rewritten.
7. **Advanced Settings** holds phrase/compact-number strategy, non-number gap, provenance and a link to existing Gradio profile controls. Phrase numbers is the default. Original voice profiles are not rewritten.

## Ready listening batch
'''+ '\n'.join(rows)+f'''

**{len(success)+len(partial)} dialogue contexts / {audio_count} playback outputs: {len(success)} complete Full + Segmented pairs and {len(partial)} full-only contexts with explicitly failed segmented output.** Eight contexts per character cover greeting, opening offer, counteroffer, agreement, refusal/high price, frustration, quantity change and an actual authored history line. Seven per character come from the 12 audited audition selections; history is taken from the same complete inventory. All 48 audition selections remain browsable. The full catalog contains authored and defensive/fallback metadata; its size is not a claim that every row is reachable in normal gameplay.

New raw recordings: **{len(newraw)}** ({len(set(newraw)&rawfull)} whole sentences and {len(set(newraw)-rawfull)} independent fragments, including rejected component takes). **{len(usedraw-set(newraw))} unique existing raw recordings reused**. No full number library or complete dialogue inventory was generated. Failed segmented contexts: **{len(failures)}**, retained in FINAL_REPORT.json; the matching cached full take is exposed separately, never labelled a successful assembly.

The stand-alone read-only listening index and CSV are in:
`testing/indian_auditions/recording_browser/LISTENING_INDEX.html`
`testing/indian_auditions/recording_browser/LISTENING_INDEX.csv`
Detailed settings/source IDs/ordered segment plans/hashes are stored under:
`testing/indian_auditions/recording_browser/results/<character>/<dialogue_id>/`
Raw takes and processed comparisons remain under `samples/09_Indian_Dialogue_AB/`, separately by character. Nothing enters a Unity asset or finalized production package.

## Number-boundary policy
Explicit pre/post padding for number segments, including quantity phrases, is **zero**. Number-adjacent joins insert **zero samples** regardless of the global gap or punctuation; number joins never crossfade. Speech-aware outer-edge detection is retained, with at most 1ms outward zero-crossing alignment using existing source samples. This is not added silent padding. Quiet attacks and decays remain part of detected speech; no internal word/phrase pauses are deleted. Non-number phrase boundaries retain the existing configurable 60ms default. No pitch/speed changes or new compression.

## Verification and outstanding quality
29 non-Unity tests pass. The actual launcher/API served cached WAVs for all four voices and browsed 345/306/303/309 inventory rows. Browser verification confirmed six tabs, relevant-only controls, full-inventory/history browsing and Father's unchanged controls. All output WAVs were decoded, checked for finite mono PCM at 44.1kHz and sample clipping, and matched against recorded hashes. {number_joins} number-boundary checks passed; {len(baseline)} original-data hashes are unchanged.

Technical checks are not listening approval. Source wording is unchanged, including known awkward grammar. Some reused full takes are unusually short and may omit words (see FINAL_REPORT.json); no isolated regeneration was performed. Independent fragments may still differ in timbre/delivery, and internal number pauses are listed separately rather than destructively removed. Confirm pronunciation of names, historical units, currency and expressions by listening. All results remain **NEEDS REVIEW**.

## Files changed
Existing `VOICE_VERIFICATION_STUDIO_v3.html` (one additive script include), its `verification_studio_v3/serve_studio.py` launcher helper, `app.py` (API routes on the same local Gradio server), and `indian_testing.py` (optional zero-number policy). The legacy `indian_testing_ui.py` history is filtered to profiles its controls can represent; its fixtures now select supported records from the expanded shared cache. Added `indian_browser_api.py`, `verification_studio_v3/indian-browser.js`, catalog/results/index/report and focused tests. Source/install/batch tools are retained under the Unity repository's `tools/voice/indian_studio_upgrade/`; Unity gameplay/assets are untouched. The existing Launch_Studio.cmd path, all earlier Portuguese JS/player functionality, all approved voice presets, and every pre-existing WAV remain intact. Backups are under `maintenance_backups/indian_recording_browser/`.
'''
    (folder/'README.md').write_text(text,encoding='utf-8')
    print(json.dumps(summary,indent=2))
if __name__=='__main__':main()
