import unittest
import numpy as np
from indian_testing import process_piece,speech_activity_edges,JOIN_PRESETS,assemble

class SpeechAwareEdgesTests(unittest.TestCase):
    def test_noisy_edges_trimmed_without_removing_quiet_attacks_or_internal_pause(self):
        sr=44100;rng=np.random.default_rng(1522)
        def tone(seconds,level):return level*np.sin(np.arange(round(sr*seconds))*2*np.pi*180/sr)
        # A quiet fricative/decay proxy adjoining louder voiced cores; a real internal pause.
        content=np.r_[tone(.08,.006),tone(.25,.12),np.zeros(round(sr*.18)),tone(.25,.12),tone(.08,.006)]
        prefix=round(sr*.6);x=np.r_[np.zeros(prefix),content,np.zeros(round(sr*.7))]
        x+=rng.normal(0,.0015,len(x))
        p=JOIN_PRESETS['Speech-aware 60 ms'];y,m=process_piece(x,sr,p)
        self.assertLess(m['trim_start_sample'],prefix)
        self.assertGreater(m['trim_end_sample'],prefix+len(content))
        self.assertGreater(m['trim_start_sample'],round(sr*.5))
        self.assertLess(m['trim_end_sample'],prefix+len(content)+round(sr*.1))
        self.assertTrue(np.array_equal(y[prefix-m['trim_start_sample']:prefix-m['trim_start_sample']+len(content)],x[prefix:prefix+len(content)]))
        self.assertLessEqual(m['fade_in_samples'],m['leading_safe_samples'])
        self.assertLessEqual(m['fade_out_samples'],m['trailing_safe_samples'])
    def test_safe_gain_and_no_phoneme_crossfade(self):
        sr=44100;x=np.r_[np.zeros(10000),.1*np.sin(np.arange(22050)*.05),np.zeros(10000)]
        settings=dict(JOIN_PRESETS['Speech-aware 60 ms'],crossfade_ms=8)
        y,m=process_piece(x,sr,settings,target=0)
        self.assertLessEqual(m['gain_db'],4)
        result,meta=assemble([(y,m),(y,m)],sr,[dict(text='two',kind='number'),dict(text='Seers',kind='phrase')],settings)
        self.assertEqual(60,meta['joins'][0]['gap_ms']);self.assertEqual(0,meta['joins'][0]['crossfade_samples'])
        self.assertLess(abs(result).max(),.8)
    def test_silence_rejected_instead_of_producing_empty_clip(self):
        with self.assertRaises(ValueError):speech_activity_edges(np.zeros(44100),44100)

if __name__=='__main__':unittest.main()
