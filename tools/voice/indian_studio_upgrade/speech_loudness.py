"""Conservative, immutable dialogue derivatives; no trimming/resampling/pitch changes."""
import hashlib,json,math,re
from pathlib import Path
import numpy as np
import soundfile as sf
import pyloudnorm as pyln
from scipy.signal import resample_poly

POLICY=dict(version='k-weighted-speech-v1',target_lkfs=-20.0,true_peak_ceiling_dbtp=-2.0,
    maximum_boost_db=6.0,maximum_attenuation_db=18.0,assembly_residual_db=1.0,
    minimum_active_seconds=.14,minimum_active_dbfs=-45.0,minimum_noise_margin_db=12.0)

def measure(x,sr):
    x=np.asarray(x,dtype=np.float64)
    if x.ndim!=1 or not len(x) or not np.isfinite(x).all():raise ValueError('Nonempty finite mono PCM required')
    if sr<8000:raise ValueError('Unsupported source sample rate')
    meter=pyln.Meter(sr)
    weighted=x.copy()
    for stage in meter._filters.values():weighted=stage.apply_filter(weighted)
    win=max(1,round(sr*.02));hop=max(1,round(sr*.01))
    f=np.lib.stride_tricks.sliding_window_view(np.pad(x,(0,max(0,win-len(x)))),win)[::hop]
    wf=np.lib.stride_tricks.sliding_window_view(np.pad(weighted,(0,max(0,win-len(x)))),win)[::hop]
    levels=20*np.log10(np.maximum(np.sqrt(np.mean(f*f,axis=1)),1e-12))
    gate=max(-55,float(levels.max())-25);active=levels>gate
    if np.any(active):
        # Same speech gate for short fragments and final dialogue; no padding short files to fake LUFS.
        power=float(np.mean(wf[active]**2));active_lkfs=-.691+10*math.log10(max(power,1e-24))
        active_dbfs=float(10*np.log10(max(float(np.mean(f[active]**2)),1e-24)))
    else:active_lkfs=active_dbfs=-240.0
    integrated=None
    if len(x)>=round(.4*sr):
        value=float(meter.integrated_loudness(x))
        if math.isfinite(value):integrated=value
    return dict(duration_seconds=len(x)/sr,sample_rate=sr,active_lkfs=active_lkfs,integrated_lufs=integrated,
        loudness_method='K-weighted energy of gated 20ms speech frames; NOT BS.1770 integrated LUFS',
        active_seconds=float(np.sum(active)*hop/sr),active_dbfs=active_dbfs,
        noise_margin_db=float(np.percentile(levels,90)-np.percentile(levels,20)),
        sample_peak=float(abs(x).max()),true_peak_dbtp=20*math.log10(max(float(abs(resample_poly(x,4,1)).max()),1e-12)))

def normalize(x,sr,policy=None,text='',stage='source',trusted_sources=False,quality_reference=None):
    """One source gain, then at most 1dB documented final assembly correction."""
    p=dict(POLICY,**(policy or {}));before=measure(x,sr);quality=quality_reference or before;warnings=[];blocked=[]
    if set(p)!=set(POLICY) or p['version']!=POLICY['version']:raise ValueError('Unknown loudness policy field/version')
    if any(not math.isfinite(float(v)) for k,v in p.items() if k!='version'):raise ValueError('Loudness parameters must be finite')
    if not -30<=p['target_lkfs']<=-16:raise ValueError('Target must be -30 through -16 speech-active LKFS')
    if not -9<=p['true_peak_ceiling_dbtp']<=-1:raise ValueError('True-peak ceiling must be -9 through -1 dBTP')
    if not 0<=p['maximum_boost_db']<=6 or not 0<=p['maximum_attenuation_db']<=30 or not 0<=p['assembly_residual_db']<=1:raise ValueError('Gain limits exceed conservative bounds')
    if before['sample_peak']>=.9999:blocked.append('Source clipping; do not repair/approve automatically')
    if quality['active_seconds']<p['minimum_active_seconds']:blocked.append('Insufficient sustained speech')
    if quality['active_dbfs']<p['minimum_active_dbfs']:blocked.append('Speech too quiet/defective to amplify safely')
    count=len(re.findall(r"[\w']+",text.replace('-',' ')))
    if count>=2 and quality['duration_seconds']<count/5:blocked.append('Unusually short for intended words; possible omitted speech')
    if not trusted_sources and quality['noise_margin_db']<p['minimum_noise_margin_db']:warnings.append('Noise estimate uncertain; positive gain disabled')
    desired=p['target_lkfs']-before['active_lkfs'];boost=p['maximum_boost_db'];atten=p['maximum_attenuation_db']
    if stage=='assembly':boost=atten=p['assembly_residual_db']
    gain=min(boost,max(-atten,desired))
    if blocked or (warnings and not trusted_sources):gain=min(0,gain)
    gain=min(gain,p['true_peak_ceiling_dbtp']-before['true_peak_dbtp'])
    y=np.asarray(x,dtype=np.float64)*10**(gain/20);after=measure(y,sr)
    if after['true_peak_dbtp']>p['true_peak_ceiling_dbtp']+.02:raise ValueError('True-peak guard failed')
    if abs(after['active_lkfs']-p['target_lkfs'])>1.0:warnings.append('Target not reached within conservative gain/peak limits')
    warnings=blocked+warnings
    return y,dict(policy=p,stage=stage,gain_db=gain,requested_gain_db=desired,before=before,after=after,
        status='FLAGGED_DEFECTIVE' if blocked else ('NEEDS_REVIEW_LIMITED' if warnings else 'TECHNICAL_TARGET_MET'),warnings=warnings,
        pitch_or_speed_change=False,sample_rate_conversion=False,limiting='Constant gain constrained by estimated 4x true peak; no compressor')

def derivative(source,directory,policy=None,text=''):
    source=Path(source);p=dict(POLICY,**(policy or {}))
    if source.with_suffix('.json').exists():
        prior=json.loads(source.with_suffix('.json').read_text(encoding='utf-8'))
        if prior.get('policy',{}).get('version')==POLICY['version'] and prior.get('source_path'):
            original=Path(prior['source_path'])
            if hashlib.sha256(original.read_bytes()).hexdigest()!=prior['source_sha256']:raise ValueError('Original source integrity mismatch')
            source=original  # A different target is always re-exported from the original, never from a normalized copy.
    digest=hashlib.sha256(source.read_bytes()).hexdigest()
    key=hashlib.sha256(json.dumps(dict(source=digest,policy=p,text=text),sort_keys=True).encode()).hexdigest()
    folder=Path(directory);folder.mkdir(parents=True,exist_ok=True);wav=folder/(key+'.wav');sidecar=wav.with_suffix('.json')
    if sidecar.exists():
        m=json.loads(sidecar.read_text())
        if wav.is_file() and hashlib.sha256(wav.read_bytes()).hexdigest()==m['sha256']:return m
        raise ValueError('Derivative integrity mismatch; preserve and inspect instead of overwriting')
    x,sr=sf.read(source,dtype='float64');y,m=normalize(x,sr,p,text)
    with wav.open('xb') as f:sf.write(f,y,sr,format='WAV',subtype='PCM_24')
    m.update(path=str(wav),source_path=str(source),source_sha256=digest,sha256=hashlib.sha256(wav.read_bytes()).hexdigest(),text=text)
    with sidecar.open('x',encoding='utf-8') as f:json.dump(m,f,indent=2)
    return m
