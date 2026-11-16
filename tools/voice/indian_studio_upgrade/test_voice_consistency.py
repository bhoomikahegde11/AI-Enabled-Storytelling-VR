import unittest,tempfile
from pathlib import Path
import numpy as np
import soundfile as sf
from unittest.mock import patch
from types import SimpleNamespace
from speech_loudness import POLICY,normalize,measure,derivative
from indian_testing import ZERO_NUMBER_SETTINGS,numeric_segment,assemble,process_piece,TestingStudio

def voice(level=.1):
    sr=44100;t=np.arange(sr)/sr;env=.35+.65*np.sin(2*np.pi*3*t)**2
    return np.r_[np.zeros(sr//4),level*env*(np.sin(2*np.pi*160*t)+.2*np.sin(2*np.pi*310*t)),np.zeros(sr//4)],sr

class VoiceConsistencyTests(unittest.TestCase):
    def test_speech_loudness_matches_and_preserves_length_and_dynamics(self):
        results=[]
        for level in [.14,.28]:
            x,sr=voice(level);y,m=normalize(x,sr,text='A fair price')
            self.assertEqual(len(x),len(y));self.assertLessEqual(m['after']['true_peak_dbtp'],-1.98)
            self.assertTrue(np.allclose(y,x*10**(m['gain_db']/20)))
            self.assertAlmostEqual(-20,m['after']['active_lkfs'],places=2);results.append(m['after']['active_lkfs'])
        self.assertAlmostEqual(*results,places=2)
    def test_defective_and_noise_only_audio_are_not_boosted(self):
        for x in [np.zeros(44100),np.random.default_rng(4).normal(0,1e-4,44100)]:
            y,m=normalize(x,44100,text='A full sentence')
            self.assertLessEqual(m['gain_db'],0);self.assertEqual('FLAGGED_DEFECTIVE',m['status'])
        x,sr=voice();x=x[:int(.15*sr)];y,m=normalize(x,sr,text='This sentence cannot fit')
        self.assertLessEqual(m['gain_db'],0)
    def test_peak_headroom_and_assembly_residual_are_bounded(self):
        x,sr=voice();x[len(x)//2]=.99;y,m=normalize(x,sr)
        self.assertLessEqual(m['after']['true_peak_dbtp'],-1.98)
        y,m=normalize(voice(.03)[0],sr,stage='assembly',trusted_sources=True)
        self.assertLessEqual(m['gain_db'],1)
    def test_derivative_is_immutable_idempotent_and_retargets_original(self):
        with tempfile.TemporaryDirectory() as tmp:
            root=Path(tmp);p=root/'raw.wav';x,sr=voice();sf.write(p,x,sr,subtype='PCM_24');original=p.read_bytes()
            a=derivative(p,root/'outputs',text='A fair price');b=derivative(a['path'],root/'outputs',text='A fair price')
            self.assertEqual(a['path'],b['path']);self.assertEqual(original,p.read_bytes())
            c=derivative(a['path'],root/'outputs',dict(POLICY,target_lkfs=-22),text='A fair price')
            self.assertEqual(str(p),c['source_path']);self.assertNotEqual(a['path'],c['path'])
    def test_number_timing_unchanged_by_loudness(self):
        x,sr=voice();part,meta=process_piece(x,sr,dict(ZERO_NUMBER_SETTINGS,normalize=False),number=True)
        out,joins=assemble([(part,meta)]*3,sr,[dict(text='offer',kind='literal'),dict(text='two',kind='number'),dict(text='varahas',kind='literal')],ZERO_NUMBER_SETTINGS)
        y,m=normalize(out,sr,stage='assembly',trusted_sources=True)
        self.assertEqual(len(out),len(y));self.assertTrue(all(j['gap_ms']==0 and j['crossfade_samples']==0 for j in joins['joins']))
    def test_seed_policy_is_separate_from_legacy_cache(self):
        s=TestingStudio();p=s.profile('lakshmi_amma');q=dict(p,seed_policy='character-text-v1')
        self.assertNotEqual(s.raw_key('lakshmi_amma','same text',p),s.raw_key('lakshmi_amma','same text',q))
        self.assertEqual(s.raw_key('lakshmi_amma','same text',q),s.raw_key('lakshmi_amma','same text',dict(q)))
    def test_unsafe_or_unknown_loudness_parameters_are_rejected(self):
        x,sr=voice()
        for p in [dict(target_lkfs=float('nan')),dict(maximum_boost_db=30),dict(unknown=1)]:
            with self.assertRaises(ValueError):normalize(x,sr,p)
    def test_generation_rng_is_restored_and_identical_seed_repeats(self):
        import torch,app
        if not torch.cuda.is_available():self.skipTest('CUDA required for the existing generation wrapper')
        class Tokens:
            input_ids=attention_mask=torch.ones((1,2),dtype=torch.long)
            def to(self,device):return self
        class Model:
            config=SimpleNamespace(sampling_rate=44100,audio_encoder=SimpleNamespace(sampling_rate=44100))
            generation_config=SimpleNamespace(to_json_string=lambda **kwargs:'{}')
            dtype=torch.float16;training=False
            def generate(self,**kwargs):
                torch.rand(4,device='cuda:0')
                return torch.rand((1,44100))*.2-.1
        engine=app.ModelSession();engine.model=Model();engine.torch=torch
        engine.prompt_tokenizer=engine.description_tokenizer=lambda *args,**kwargs:Tokens()
        cpu=torch.get_rng_state().clone();gpu=torch.cuda.get_rng_state(0).clone()
        with tempfile.TemporaryDirectory() as tmp,patch.object(app,'SAMPLES',Path(tmp)):
            paths=[]
            for label in ['first','repeat']:
                path,meta=engine.generate('Test line','Swapna','a conversational woman','friendly','moderate','moderate',seed=1522,output_directory=Path(tmp),sample_label=label)
                paths.append(path)
                self.assertTrue(torch.equal(cpu,torch.get_rng_state()));self.assertTrue(torch.equal(gpu,torch.cuda.get_rng_state(0)))
            self.assertEqual(Path(paths[0]).read_bytes(),Path(paths[1]).read_bytes())
    def test_quality_report_plays_all_saraswati_candidates_without_selecting_winner(self):
        from fastapi import FastAPI
        from fastapi.testclient import TestClient
        from indian_browser_api import mount_api
        from io import BytesIO
        app=FastAPI();browser=mount_api(app,None);client=TestClient(app)
        self.assertEqual('indian-browser-consistency-v1',client.get('/indian-api/capabilities').json()['version'])
        report=client.get('/indian-api/quality').json();sar=[e for e in report['entries'] if e['character_id']=='saraswati_chetti']
        self.assertEqual(16,len(sar));self.assertEqual('Sneha',browser.profile('saraswati_chetti')['speaker'])
        for speaker in ['Sneha','Mary','Meera','Gauri']:
            e=next(e for e in sar if e['speaker']==speaker);response=client.get(e['normalized']['url']);self.assertEqual(200,response.status_code)
            x,sr=sf.read(BytesIO(response.content));self.assertEqual(44100,sr);self.assertGreater(len(x),1000);self.assertLess(abs(x).max(),.8)
        self.assertEqual(404,client.get('/indian-api/quality-audio/not-a-run/0/raw').status_code)

if __name__=='__main__':unittest.main()
