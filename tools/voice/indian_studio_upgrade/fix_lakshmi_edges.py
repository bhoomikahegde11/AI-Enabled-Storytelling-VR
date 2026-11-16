"""Install the optional DSP policy and rebuild only L1_LAK_001 from cached masters."""
import sys,json,hashlib,shutil,copy
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator')
STAGE=Path(__file__).resolve().parent
def digest(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def main():
    data=ROOT/'testing/indian_auditions';stamp=datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')
    backup=ROOT/'maintenance_backups/indian_speech_edges'/stamp;backup.mkdir(parents=True)
    protected=json.loads((data/'preservation_baseline.json').read_text())
    protected.update({str(p):digest(p) for folder in [ROOT/'samples/09_Indian_Dialogue_AB',data/'settings'] for p in folder.rglob('*') if p.is_file()})
    (backup/'preservation.json').write_text(json.dumps(protected,indent=2),encoding='utf-8')
    for name in ['indian_testing.py','indian_testing_ui.py','test_indian_testing_ui.py','test_indian_speech_edges.py']:
        target=ROOT/name
        if target.exists():shutil.copyfile(target,backup/name)
        target.write_bytes((STAGE/name).read_bytes())
    sys.path.insert(0,str(ROOT))
    import numpy as np
    import soundfile as sf
    from indian_testing import TestingStudio,JOIN_PRESETS,load,words
    from audio_quality import measure_wav
    studio=TestingStudio();old=load(studio.output/'comparisons/lakshmi_amma/51a60ea04e8a5ed8f654/comparison.json')
    folder=studio.output/'targeted_quality'/('L1_LAK_001_'+stamp);folder.mkdir(parents=True)
    original=folder/'L1_LAK_001_Original_Full.wav';shutil.copyfile(old['full_wav'],original)
    result={};rows=[]
    for strategy,label in [('Phrase numbers','Improved_PhraseNumbers'),('Compact grammar','Improved_CompactFragments')]:
        p=copy.deepcopy(old['profile']);p['strategy']=strategy;p['processing']=dict(JOIN_PRESETS['Speech-aware 60 ms'])
        r=studio.run('lakshmi_amma','L1_LAK_001',dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper'),p,generate=False)
        target=folder/('L1_LAK_001_'+label+'.wav');shutil.copyfile(r['assembled_wav'],target)
        assert words(old['plan']['tts_input'])==words(r['plan']['tts_input'])==words(' '.join(s['tts_input'] for s in r['segments']))
        result[strategy]=dict(wav=str(target),comparison_json=r['record_path'],metrics=r['metrics']['assembled'],settings=p['processing'],warnings=r['warnings'])
        t=0
        for i,part in enumerate(r['segments']):
            x,sr=sf.read(part['raw']['path']);y,_=sf.read(part['processed_path']);m=part['processing']
            rows.append(dict(strategy=strategy,index=i,text=part['tts_input'],raw=part['raw']['path'],raw_seconds=len(x)/sr,processed_seconds=len(y)/sr,
                timeline_start=t,timeline_end=t+len(y)/sr,removed_lead_seconds=m['trim_start_sample']/sr,removed_tail_seconds=(len(x)-m['trim_end_sample'])/sr,
                gain_db=m['gain_db'],speech_edges=m['speech_edges'],fade_in_samples=m['fade_in_samples'],fade_out_samples=m['fade_out_samples']))
            t+=len(y)/sr
            if i<len(r['assembly_processing']['joins']):t+=r['assembly_processing']['joins'][i]['gap_ms']/1000
    for path in folder.glob('*.wav'):
        x,sr=sf.read(path);assert sr==44100 and len(x)>0 and np.isfinite(x).all() and abs(x).max()<.9999
    assert studio.new_generations==0
    assert all(Path(p).is_file() and digest(p)==h for p,h in protected.items())
    report=dict(original_full=str(original),original_full_metrics=old['metrics']['full'],original_assembled=old['assembled_wav'],original_assembled_metrics=old['metrics']['assembled'],
        original_joins=old['assembly_processing'],outputs=result,segment_timeline=rows,new_synthesis_count=0,protected_hashes=len(protected),
        transcript_check='TTS input and ordered segment words identical; actual spoken words require listening, no offline ASR installed.',human_approval=False)
    (folder/'REPORT.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    (data/'LAK_001_QUALITY_COMPARISON.json').write_text(json.dumps(dict(folder=str(folder),report=str(folder/'REPORT.json')),indent=2),encoding='utf-8')
    lines=['# Lakshmamma L1_LAK_001 — cached-fragment quality correction','',
        'Original full sentence: **'+str(old['metrics']['full']['duration_seconds'])+' s**. Original phrase assembly: **'+str(old['metrics']['assembled']['duration_seconds'])+' s**.',
        'No TTS was run. All original WAVs, comparisons, character drafts/presets and Father assets passed preservation hashes.','',
        '## Cause','The old 4ms RMS detector used peak-minus-50dB thresholds (about -59 to -66dBFS), retaining generated noise as speech. Original inserted gaps were only 6/6/55ms. Raw leading/trailing quiet regions dominated the long breaks. Internal punctuation pauses remain intentionally untouched.','',
        '## Listening outputs','- Original complete sentence: `'+str(original)+'`']
    for strategy,r in result.items():lines.append('- '+strategy+': **'+str(r['metrics']['duration_seconds'])+' s**, `'+r['wav']+'`')
    lines+=['','## Processing','New optional **Speech-aware 60 ms** preset uses a sustained 10ms/2ms activity core above the measured noise bed, a lower hysteresis support threshold to retain soft attacks/decays, and 25/35ms outer padding (15ms mandatory minimum). No internal gaps are deleted. Fixed boundary gap is 60ms; compact number-to-number joins use at most 40ms. Fades are 1ms and limited to protected outer padding. Crossfades are disabled. Existing limited speech-active weighted RMS matching stays unchanged (+4/-9dB, -2dB true-peak estimate ceiling). No pitch, speed, compressor or global gap removal.',
        '', '## Tone / timbre diagnosis','The compared raw metadata use the same Swapna description, mood, pace, expression, seed and model defaults. Recordings were independently synthesized from different text contexts. Assembly applies only trimming, tiny padding fades and constant gain; no timbral EQ, pitch or rate changes. Independent Parler generation is therefore the supported explanation for delivery variation, while constant gain can also change perceived clarity. Metadata equality cannot prove identical vocal identity or pronunciation. Normalization cannot repair inconsistent delivery.',
        '', '## Use in existing studio','Restart launch.bat. Select Lakshmamma / Greeting. In Advanced Settings choose **Speech-aware 60 ms**. The rebuilt Phrase numbers comparison loads automatically. Select Compact grammar for the other rebuilt comparison. Both use the same cached raw takes. The preset and cached-only reprocessing work for every character, but only this one Lakshmamma dialogue was rebuilt.',
        '', '## Limitations','Word-for-word input/segment equality, hashes, mono PCM-24/44.1kHz integrity, finite samples and no sample clipping were checked. Energy is not phoneme recognition. All clips still need listening approval for quiet consonants, word endings, missing/repeated words, unnatural joins, accent and identity. Long internal sentence pauses and synthesis timbre changes remain; no automatic approval or regeneration was performed. Detailed removed-edge timings/gains/fades and final timelines are in REPORT.json.']
    (folder/'REPORT.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')
    print(json.dumps(dict(folder=str(folder),durations={k:v['metrics']['duration_seconds'] for k,v in result.items()},new_synthesis=0,protected_hashes=len(protected)),indent=2))
if __name__=='__main__':main()
