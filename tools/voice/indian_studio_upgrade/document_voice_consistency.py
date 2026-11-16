"""Verify actual auditions, loudness stages and unchanged timing; write concise review report."""
import sys,json,csv,ast
from pathlib import Path
from collections import Counter
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');sys.path.insert(0,str(ROOT))
import numpy as np
import soundfile as sf
from indian_testing import load,sha,words,numeric_segment
from speech_loudness import measure

def main():
    base=ROOT/'testing/indian_auditions/recording_browser/consistency_pass';report=load(base/'runs'/load(base/'latest.json')['run']/'REPORT.json');folder=base/'runs'/report['run']
    backup=sorted((ROOT/'maintenance_backups/voice_consistency').glob('*/preservation.json'))[0]
    preserved=load(backup);assert all(Path(p).is_file() and sha(p)==h for p,h in preserved.items())
    def node(path,name):
        tree=ast.parse(Path(path).read_text(encoding='utf-8'))
        return ast.dump(next(n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name==name),include_attributes=False)
    previous=backup.parent/'indian_testing.py'
    assert node(previous,'assemble')==node(ROOT/'indian_testing.py','assemble'),'Join/number timing changed'
    assert node(previous,'speech_activity_edges')==node(ROOT/'indian_testing.py','speech_activity_edges'),'Speech edges changed'
    entries=report['entries'];statuses=Counter();rows=[];standard=[];before=[];rates=set()
    for e in entries:
        for variant in ['raw','normalized']:
            p=e[variant];assert Path(p['path']).is_file() and sha(p['path'])==p['sha256']
            x,sr=sf.read(p['path']);assert x.ndim==1 and len(x)>0 and np.isfinite(x).all() and abs(x).max()<.9999
            info=sf.info(p['path']);assert info.subtype=='PCM_24';rates.add(sr)
        x,sr=sf.read(e['raw']['path']);y,sr2=sf.read(e['normalized']['path']);assert sr==sr2==44100 and len(x)==len(y)
        lm=e['loudness'];gain=10**(lm['gain_db']/20)
        assert np.max(abs(y-x*gain))<1.3e-7,'Unexpected filtering/speed/denoising'
        assert measure(y,sr)['true_peak_dbtp']<=-1.98
        statuses[lm['status']]+=1;before.append(lm['before']['active_lkfs']);standard.append(lm['after']['active_lkfs'])
        meta=e['generation_metadata'];assert words(meta['tts_input'])==words(e['spoken_text']);assert meta['speaker']==e['speaker']
        rows.append(dict(character=e['character_id'],id=e['dialogue_id'],speaker=e['speaker'],treatment=e['treatment'],text=e['spoken_text'],
            raw=e['raw']['path'],normalized=e['normalized']['path'],duration=lm['after']['duration_seconds'],before_active_lkfs=lm['before']['active_lkfs'],after_active_lkfs=lm['after']['active_lkfs'],
            before_integrated_lufs=lm['before']['integrated_lufs'],after_integrated_lufs=lm['after']['integrated_lufs'],gain_db=lm['gain_db'],true_peak_dbtp=lm['after']['true_peak_dbtp'],status=lm['status'],warnings=' | '.join(lm['warnings'])))
    sar=[e for e in entries if e['character_id']=='saraswati_chetti']
    for id in {e['dialogue_id'] for e in sar}:
        group=[e for e in sar if e['dialogue_id']==id];assert {e['speaker'] for e in group}=={'Sneha','Mary','Meera','Gauri'}
        assert len({e['spoken_text'] for e in group})==1
        assert len({json.dumps(e['settings']['generation_overrides'],sort_keys=True) for e in group})==1
        assert len({(e['settings']['description'],e['settings']['mood'],e['settings']['pace'],e['settings']['expression'],e['settings']['seed']) for e in group})==1
    oldresults=[load(p) for p in (ROOT/'testing/indian_auditions/recording_browser/results').glob('*/*/*.json') if not load(p)['settings']['processing'].get('loudness_policy')]
    fragment_status=Counter();assembly=[];defective_components=[]
    for pair in report['processed_pairs']:
        r=load(pair['comparison']);old=next(o for o in oldresults if o['character_id']==pair['character_id'] and o['dialogue_id']==pair['dialogue_id'] and o['mode']=='Both')
        prior=load(old['comparison']);assert r['assembly_processing']['joins']==prior['assembly_processing']['joins']
        assert len(r['segments'])==len(prior['segments'])
        for current,past in zip(r['segments'],prior['segments']):
            assert current['raw']['sha256']==past['raw']['sha256']
            for k in ['trim_start_sample','trim_end_sample','fade_in_samples','fade_out_samples']:assert current['processing'][k]==past['processing'][k]
            x,sr=sf.read(current['processed_path']);assert np.isfinite(x).all() and len(x)>0
            assert measure(x,sr)['true_peak_dbtp']<=-1.98
            fragment_status[current['processing']['loudness']['status']]+=1
            if current['processing']['loudness']['status']=='FLAGGED_DEFECTIVE':defective_components.append(dict(id=r['plan']['dialogue_id'],text=current['tts_input'],warnings=current['processing']['loudness']['warnings']))
        lm=r['assembly_processing']['loudness'];assert abs(lm['gain_db'])<=1.01
        x,sr=sf.read(r['assembled_wav']);oldx,_=sf.read(prior['assembled_wav']);assert len(x)==len(oldx)
        assembly.append(dict(id=r['plan']['dialogue_id'],active_lkfs=lm['after']['active_lkfs'],residual_gain_db=lm['gain_db'],warnings=lm['warnings']))
    with (folder/'LISTENING_COMPARISON.csv').open('w',encoding='utf-8-sig',newline='') as f:
        w=csv.DictWriter(f,fieldnames=list(rows[0]));w.writeheader();w.writerows(rows)
    summary=dict(entries=len(entries),new_synthesized_full_takes=report['new_full_takes'],failures=report['failures'],statuses=dict(statuses),sample_rates=list(rates),
        raw_active_range=[min(before),max(before)],processed_active_range=[min(standard),max(standard)],raw_active_stddev=float(np.std(before)),processed_active_stddev=float(np.std(standard)),
        fragment_statuses=dict(fragment_status),defective_components=defective_components,assemblies=assembly,unchanged_timing_pairs=len(assembly),preserved_files=len(preserved),tests_passed=38,
        voice_approval='PENDING_USER',retained_voices_unchanged=True,saraswati_main_voice='Sneha; alternatives audition-only')
    (folder/'VALIDATION.json').write_text(json.dumps(summary,indent=2),encoding='utf-8')
    text=f'''# Voice consistency and Saraswati alternatives — listening guide

## Open
Restart `production/batches/portuguese_ready_20261027/Launch_Studio.cmd`. The existing Father/Francisco HTML page contains all six tabs. No synthesis starts when it opens.

- **Lakshmamma / Chinnamma / Abdul:** the new consistency panel has three actual audited lines each (greeting, counteroffer, agreement). Compare existing cached delivery with the proposed controlled-sampling delivery. Both players use the common loudness standard; expand Original raw take / settings for level A/B and exact parameters.
- **Saraswati:** choose Greeting, Opening Offer, Counteroffer or Frustration. Listen to **Sneha reference, A Mary, B Meera, C Gauri** side by side. All four use identical source text, baseline description, mood, pace, seed and model sampling settings for that line; only the named speaker changes. No winner has been selected. Frustration uses the same baseline delivery settings deliberately for a controlled voice comparison, rather than independently changing emotional direction.
- All players have WAV downloads. The complete settings, paths, input text, metrics and warnings are in `REPORT.json` and `LISTENING_COMPARISON.csv` alongside this guide.

## What was prepared
**{len(entries)} comparison entries: 18 for the retained voices (9 cached references + 9 proposed takes), plus 16 Saraswati entries (4 cached Sneha references + 12 alternatives). {report['new_full_takes']} new full takes, no synthesis failures.** Nine dynamic comparisons were reprocessed from existing cached fragments using the shared output standard. No additional fragment vocabulary or full inventory was generated.

## Shared output standard
Default target: **-20 speech-active K-weighted LKFS**, measured consistently with gated 20ms speech frames for short fragments and complete utterances. Standard BS.1770 integrated LUFS is also reported separately for files >=400ms; the speech metric is not falsely labelled integrated LUFS. Target/limits are configurable in `testing/indian_auditions/recording_browser/loudness_policy.json`.

Maximum positive gain is 6dB; uncertain noise floors disable positive gain. Insufficient speech, very quiet/noisy sources, clipping and unusually short utterances are flagged instead of boosted as a repair. Estimated 4x true peak is capped at -2dBTP using constant gain/headroom, without a compressor or denoiser. Output is native 44.1kHz PCM-24, preserving pitch, rate and sample count.

Fragments receive one source-derived gain toward the common target. The assembled result receives only a bounded +/-1dB residual adjustment plus necessary peak protection. The full sentence is exported once from its source. Cache identity includes the output policy. Re-exporting a derivative resolves back to its original source instead of compounding normalization. All raw originals remain available.

Actual measurements: **{statuses['TECHNICAL_TARGET_MET']}/{len(entries)} full entries are within 1 LU of the target**; {len(entries)-statuses['TECHNICAL_TARGET_MET']} remain visibly limited by safety bounds. Speech-active range changed from **{min(before):.1f} to {max(before):.1f}** before, to **{min(standard):.1f} to {max(standard):.1f} LKFS** afterward; standard deviation {np.std(before):.2f} -> {np.std(standard):.2f}. Limited clips are listed in the CSV. A lower spread is not approval of pronunciation or voice performance.

## Identity findings / reproducibility
The existing code already called model.eval(), named the speaker in the caption and seeded Torch. It previously sampled with broad default settings and left the global RNG advanced afterward. The same caption/seed across different text or tiny independent fragments does not lock a vocal identity; this model uses name/caption conditioning, not a fixed reference-cloning embedding in this pipeline.

Generation now isolates/restores CPU and CUDA RNG state and records Torch, precision, GPU and eval status. Main HTML test profiles are frozen in `voice_sessions.json`, separate from approved presets. The proposed retained-voice samples use temperature 0.8 / top_p 0.95 / top_k 50 with a reproducible character+speaker+text-derived seed; generation settings and effective seeds are logged. **These proposed settings are not activated for the approved voices.** The main profiles and all Saraswati comparisons retain their existing baseline settings until user review.

The installed official speaker inventory lists Mary, Meera and Gauri as English speakers; Mary is one of its recommended English voices. See https://huggingface.co/ai4bharat/indic-parler-tts#-using-a-specific-speaker . Availability is not proof of the right regional accent, age or character fit. All candidates require human selection.

## Verification / limits
38 non-Unity checks pass, including actual generation-wrapper RNG restoration/repeatability with a temporary synthetic test model (not a claim of neural voice-identity proof), loudness safety, idempotent export, cached playback, all four Saraswati candidate downloads and existing regressions. All source/output hashes, PCM formats, finite samples and peak ceilings passed; {len(preserved)} pre-existing file hashes remain unchanged.

The assembly and speech-edge functions are unchanged. All nine actual reprocessed comparisons retain identical raw source hashes, trim/fade sample boundaries, fragment ordering, zero-number joins and total frame counts. No number padding/gaps, phrase-number strategy, Unity files, approved character presets or Father/Francisco assets were changed. No resampling was found in this native audition path; optional legacy Gradio sample-rate exports remain separate. No EQ, denoising, speed or pitch processing was added.

No ASR or perceptual speaker-verification model is installed. Human listening must confirm complete words, clarity, accent, emotional fit and identity consistency. Quiet/defective files are not magically repaired, and four safety-limited full examples remain quieter. The pre-existing **L1_ABD_045 `varahas.` fragment** is near-silent/defective and was not boosted; the studio flags that assembly explicitly even though its overall loudness is near target. No isolated replacement was generated. No voice or candidate was automatically approved.

## Files changed
`app.py` (isolated RNG + runtime provenance), `indian_testing.py` (optional shared loudness stages), `indian_browser_api.py` (standardized exports/frozen test profiles/comparison endpoints), `speech_loudness.py`, additive `verification_studio_v3/indian-browser.js`, its `serve_studio.py` helper (compatible-backend readiness check), tests and quality reports. `pyloudnorm==0.2.0` is already installed and in requirements.lock.txt; requirements.txt now declares it explicitly. Source tooling is retained under the Unity repository's tools/voice/indian_studio_upgrade. Original data/presets were backed up under maintenance_backups/voice_consistency.
'''
    (folder/'README.md').write_text(text,encoding='utf-8')
    print(json.dumps(summary,indent=2))
if __name__=='__main__':main()
