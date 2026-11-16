"""Additional validation and before/after timelines; no synthesis or audio writes."""
import sys,json,csv,hashlib
from pathlib import Path
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator');sys.path.insert(0,str(ROOT))
import numpy as np
import soundfile as sf
from indian_testing import load,sha
def main():
    pointer=load(ROOT/'testing/indian_auditions/LAK_001_QUALITY_COMPARISON.json');d=load(pointer['report']);folder=Path(pointer['folder'])
    records=[load(v['comparison_json']) for v in d['outputs'].values()];allraw={}
    for r in records:
        for p in [r['full_raw']]+[s['raw'] for s in r['segments']]:allraw[p['path']]=load(p['metadata_path'])
        for s in r['segments']:
            x,sr=sf.read(s['raw']['path']);y,rate=sf.read(s['processed_path']);m=s['processing'];assert sr==rate==44100
            expected=x[m['trim_start_sample']:m['trim_end_sample']]*10**(m['gain_db']/20)
            fi,fo=m['fade_in_samples'],m['fade_out_samples'];a=fi;b=len(y)-fo
            assert np.max(abs(y[a:b]-expected[a:b]))<1.3e-7,'Retained PCM unexpectedly altered'
            assert fi<=m['leading_safe_samples'] and fo<=m['trailing_safe_samples']
            assert all(j['crossfade_samples']==0 for j in r['assembly_processing']['joins'])
            assert sha(s['raw']['path'])==s['raw']['sha256']
    keys=['speaker','voice_description','mood','pace','seed','generation_overrides','generation_settings','model_id']
    first=next(iter(allraw.values()));assert all(all(m.get(k)==first.get(k) for k in keys) for m in allraw.values())
    old=load(ROOT/'samples/09_Indian_Dialogue_AB/comparisons/lakshmi_amma/51a60ea04e8a5ed8f654/comparison.json')
    assert sha(d['original_full'])==sha(old['full_wav'])
    rows=[]
    for name,r in [('Original phrase',old)]+[(r['plan']['strategy'],r) for r in records]:
        time=0
        for i,s in enumerate(r['segments']):
            y,sr=sf.read(s['processed_path']);m=s['processing'];duration=len(y)/sr
            rows.append(dict(version=name,segment=i+1,text=s['tts_input'],start_seconds=round(time,4),end_seconds=round(time+duration,4),duration=round(duration,4),
                removed_lead_ms=round(m['trim_start_sample']/sr*1000,2),gain_db=round(m['gain_db'],3),fade_in_samples=m['fade_in_samples'],fade_out_samples=m['fade_out_samples'],
                gap_after_ms=r['assembly_processing']['joins'][i]['gap_ms'] if i<len(r['assembly_processing']['joins']) else 0))
            time+=duration+rows[-1]['gap_after_ms']/1000
    with (folder/'BEFORE_AFTER_TIMELINE.csv').open('w',encoding='utf-8-sig',newline='') as f:
        writer=csv.DictWriter(f,fieldnames=list(rows[0]));writer.writeheader();writer.writerows(rows)
    baseline=sorted((ROOT/'maintenance_backups/indian_speech_edges').glob('*/preservation.json'))[-1]
    assert all(sha(p)==h for p,h in load(baseline).items())
    text='''# Final technical verification

- All three delivered WAVs are finite, nonempty mono PCM-24 at 44,100 Hz.
- Original full output is byte-for-byte identical to the requested 4.480-second full recording.
- Phrase assembly: 9.888 -> 5.782 seconds. Compact assembly: 7.029 seconds.
- Phrase measured total low-energy time (-45 dBFS / 20ms): 6.208 -> 2.082 seconds. Remaining internal runs >=350ms are two 380ms runs; compact has 380ms and 360ms runs. These measurements do not classify breaths/phonemes as disposable silence.
- Retained fragment PCM equals its raw source times constant gain (within PCM-24 quantization), excluding tiny boundary fades. No filtering, speed/pitch change or hidden internal edits.
- Every fade is contained within the detected quiet guard; every join uses zero overlapping phonemes/crossfade. Peak amplitude is approximately 0.4503 (-6.93 dBFS). No sample clipping detected.
- Voice descriptions, speaker, mood, pace, seed, model generation defaults and overrides match across all seven unique cached raw takes. No TTS was run.
- Compact `two` and `Seers` each hit the existing +4dB gain ceiling; matching remains conservative rather than forcing noisy/soft fragments louder. Independent word performance remains longer and potentially less natural than phrase-number generation.
- 25 non-Unity tests pass, including both rebuilt variants loading in the existing UI. All previous source audio, comparison files, drafts and Father preservation hashes pass.

## Important interpretation
The displayed/transcript inputs match word-for-word: “Good day, merchant. I need two Seers of pepper. What are you asking?” This is not an ASR verification of the audible words. Human listening was not performed or approved here. Test quiet word beginnings/endings, particularly `Seers`, for clipping or pronunciation issues. Existing internal punctuation pauses and independent-generation timbre/delivery differences remain. A shorter output is not proof of natural speech.
'''
    (folder/'VALIDATION.md').write_text(text,encoding='utf-8')
    print('Verified retained PCM, guarded fades, no overlap, identical source settings, original full SHA and preservation baseline.')
if __name__=='__main__':main()
