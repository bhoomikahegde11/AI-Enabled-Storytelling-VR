import unittest,copy
import numpy as np
from fastapi import FastAPI
from fastapi.testclient import TestClient
from indian_browser_api import RecordingBrowser,mount_api
from indian_testing import ASSIGNMENTS,ZERO_NUMBER_SETTINGS,process_piece,assemble,words

class RecordingBrowserTests(unittest.TestCase):
    def setUp(self):self.browser=RecordingBrowser()
    def test_all_selected_and_starter_lines_preserve_exact_word_order(self):
        for cid in ASSIGNMENTS:
            self.assertEqual(12,len(self.browser.selected[cid]));self.assertEqual(8,len(self.browser.batch_ids[cid]))
            self.assertGreater(len(self.browser.studio.rows(cid)),200)
            for id in set(self.browser.selected[cid]+self.browser.batch_ids[cid]):
                for strategy in ['Phrase numbers','Compact grammar']:
                    row,p,plan=self.browser.preview(cid,id,dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper'),strategy)
                    self.assertEqual(words(plan['tts_input']),words(' '.join(s['tts_input'] for s in plan['segments'])))
                    self.assertEqual(ASSIGNMENTS[cid][1],p['speaker']);self.assertTrue(p['processing']['zero_number_boundaries'])
    def test_zero_number_gaps_override_global_and_punctuation_without_crossfading(self):
        sr=44100;x=np.r_[np.zeros(10000),.1*np.sin(np.arange(22050)*.05),np.zeros(10000)]
        s=dict(ZERO_NUMBER_SETTINGS,gap_ms=100,crossfade_ms=10)
        y,m=process_piece(x,sr,s,number=True)
        self.assertLessEqual(m['leading_safe_samples'],round(sr*.001));self.assertLessEqual(m['trailing_safe_samples'],round(sr*.001))
        self.assertEqual(0,m['added_number_padding_ms'])
        for kind,field in [('number','counterPrice'),('phrase','quantityLabel')]:
            pieces=[(y,m)]*3;parts=[dict(text='Offer.',kind='literal'),dict(text='two',kind=kind,field=field),dict(text='varahas',kind='literal')]
            out,joins=assemble(pieces,sr,parts,s)
            self.assertTrue(all(j['gap_ms']==0 and j['crossfade_samples']==0 for j in joins['joins']))
            self.assertEqual(len(y)*3,len(out))
    def test_preview_reuses_full_cache_without_synthesis_and_serves_valid_audio(self):
        app=FastAPI();b=mount_api(app,None);client=TestClient(app)
        data=client.get('/indian-api/catalog').json();self.assertEqual(4,len(data['characters']))
        body=dict(character_id='lakshmi_amma',dialogue_id='L1_LAK_001',mode='Full sentence')
        response=client.post('/indian-api/preview',json=body);self.assertEqual(200,response.status_code)
        r=response.json()['saved'];self.assertIsNotNone(r);wav=client.get(r['audio'][0]['url'])
        self.assertEqual(200,wav.status_code);self.assertEqual(b'RIFF',wav.content[:4]);self.assertGreater(len(wav.content),1000)
        self.assertEqual(0,b.studio.new_generations)
        body['character_id']='father_penteado';self.assertEqual(400,client.post('/indian-api/preview',json=body).status_code)
    def test_assembly_changes_do_not_change_voice_cache_keys(self):
        s=self.browser.studio;p=s.profile('lakshmi_amma');q=self.browser.profile('lakshmi_amma')
        self.assertEqual(s.raw_key('lakshmi_amma','two',p),s.raw_key('lakshmi_amma','two',q))
        self.assertIsNone(self.browser.lookup('lakshmi_amma','L1_LAK_001',dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='cardamom'),'Full sentence'))

if __name__=='__main__':unittest.main()
