"""Non-Unity checks. Synthetic test PCM stays in a temporary directory, never the real sample library."""
import copy,json,tempfile,unittest,shutil
from pathlib import Path
import numpy as np
import soundfile as sf
from indian_testing import *

class IndianAuditionTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):cls.library=load(DATA/'library.json')
    def test_all_48_plans_and_999_prices_both_strategies(self):
        for cid,character in self.library['characters'].items():
            counter=next(r for r in character['selections'] if r['scenario']=='BuyerCounterFirst')
            for number in range(1,1000):
                for strategy in ['Phrase numbers','Compact grammar']:
                    p=make_plan(counter,price=str(number),strategy=strategy)
                    self.assertEqual(words(p['tts_input']),words(' '.join(s['tts_input'] for s in p['segments'])))
                    self.assertIn(number_text(number).replace('-',' '),p['spoken_text'].replace('-',' '))
            for row in character['selections']:
                for number in [1,19,34,105,234,999,1001,1000000,2147483647]:
                    for spice in ['pepper','cinnamon','cardamom']:
                        for unit in ['Palam','Seer','Veesai','Manangu','Bahar']:
                            p=make_plan(row,price=str(number),quantity='2',unit=unit,spice=spice)
                            self.assertEqual(words(p['tts_input']),words(' '.join(s['tts_input'] for s in p['segments'])))
    def test_grammar_range_currency(self):
        self.assertEqual(number_text(234),'two hundred and thirty-four')
        self.assertEqual(number_text(2147483647),'two billion one hundred and forty-seven million four hundred and eighty-three thousand six hundred and forty-seven')
        row=next(r for r in self.library['characters']['lakshmi_amma']['selections'] if r['scenario']=='SellerPriceAccepted')
        self.assertIn('one varahas',make_plan(row,price='1')['tts_input'])
        self.assertIn('one varaha.',make_plan(row,price='1',singular=True)['tts_input'])
        for bad in ['1.5','-1','1e3','2147483648']:
            with self.assertRaises(ValueError):make_plan(row,price=bad)
        with self.assertRaises(ValueError):make_plan(row,spice='clove')
        with self.assertRaises(ValueError):make_plan(row,quantity='8',unit='Palam')
        with self.assertRaises(ValueError):make_plan(row,quantity='999999',unit='Bahar')
        fixed=next(r for r in self.library['characters']['lakshmi_amma']['selections'] if r['scenario']=='AskName')
        self.assertTrue(make_plan(fixed,price='not used',seller_price='not used',quantity='not used')['tts_input'])
    def test_unknown_tokens_and_explicit_pronunciation(self):
        row=copy.deepcopy(self.library['characters']['lakshmi_amma']['selections'][0]);row['exact_template']='Offer {unsupported}.'
        with self.assertRaises(ValueError):make_plan(row)
        row=self.library['characters']['lakshmi_amma']['selections'][1]
        p=make_plan(row,rules={'Lakshmamma':'Laksh mam ma'})
        self.assertEqual(words(p['tts_input']),words(' '.join(s['tts_input'] for s in p['segments'])))
    def test_speech_edges_and_quiet_crossfade(self):
        sr=44100;t=np.arange(sr//2)/sr;voice=.15*np.sin(2*np.pi*180*t);x=np.r_[np.zeros(sr//5),voice,np.zeros(sr//5)]
        settings=dict(JOIN_PRESETS['Tight'],gap_ms=0,crossfade_ms=4)
        y,m=process_piece(x,sr,settings)
        self.assertLess(m['trim_start_sample'],sr//5);self.assertGreater(m['trim_end_sample'],sr//5+len(voice));self.assertEqual(m['gain_db'],0)
        joined,meta=assemble([(y,m),(y,m)],sr,[dict(text='two',kind='number'),dict(text='hundred',kind='number')],settings)
        n=meta['joins'][0]['crossfade_samples'];self.assertLessEqual(n,min(m['trailing_safe_samples'],m['leading_safe_samples']))
        self.assertEqual(len(joined),len(y)*2-n);self.assertLess(np.max(np.abs(joined)),.99)
        _,boundary=assemble([(y,m),(y,m)],sr,[dict(text='pepper',kind='phrase'),dict(text='. What do you say?',kind='phrase')],settings)
        self.assertEqual(55,boundary['joins'][0]['gap_ms'])
    def test_resume_isolation_rejoin_and_draft_safety(self):
        class Fake:
            calls=0
            def generate(self,text,speaker,*a,**kw):
                self.calls+=1;folder=Path(kw['output_directory']);folder.mkdir(parents=True,exist_ok=True);p=folder/(kw['sample_label']+'.wav')
                with p.open('xb') as f:sf.write(f,np.r_[np.zeros(1000),.15*np.sin(np.arange(22050)*.03),np.zeros(1000)],44100,format='WAV',subtype='PCM_24')
                create_json(p.with_suffix('.json'),{'test_only':True});return str(p),dict(model_output_sample_rate=44100)
        with tempfile.TemporaryDirectory(prefix='IndianABPolicy_') as temp:
            root=Path(temp);(root/'testing/indian_auditions').mkdir(parents=True)
            create_json(root/'testing/indian_auditions/library.json',self.library);shutil.copyfile(ROOT/'character_presets.json',root/'character_presets.json')
            engine=Fake();s=TestingStudio(root,engine);p=s.default_profile('lakshmi_amma');row=next(r for r in s.rows('lakshmi_amma') if r['scenario']=='BuyerCounterFirst')
            values=dict(price='234',seller_price='300',quantity='2',unit='Seer',spice='pepper')
            first=s.run('lakshmi_amma',row['inventory_id'],values,p);count=engine.calls
            second=s.run('lakshmi_amma',row['inventory_id'],values,p);self.assertEqual(count,engine.calls);self.assertEqual(first['comparison_id'],second['comparison_id'])
            p['processing']=dict(JOIN_PRESETS['Near contiguous']);s.run('lakshmi_amma',row['inventory_id'],values,p,generate=False);self.assertEqual(count,engine.calls)
            with self.assertRaises(ValueError):s.validate_profile('lakshmi_amma',dict(p,character_id='abdul_rahman'))
            self.assertNotEqual(s.raw_key('lakshmi_amma','two',p),s.raw_key('abdul_rahman','two',p))
            with self.assertRaises(FileNotFoundError):s.run('lakshmi_amma',row['inventory_id'],dict(values,price='999'),p,generate=False)
            original=sha(root/'character_presets.json');s.save_profile('lakshmi_amma',p);self.assertEqual(original,sha(root/'character_presets.json'))

if __name__=='__main__':unittest.main()
