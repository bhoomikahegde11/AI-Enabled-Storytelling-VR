"""UI callbacks use real saved comparisons; never load a model or synthesize."""
import copy,unittest
from unittest.mock import patch
from pathlib import Path
from indian_testing import TestingStudio,ASSIGNMENTS,load,sha
from indian_testing_ui import find_saved,LABELS

def settings(p):
    g=p['generation_overrides'];s=p['processing']
    return [p['description'],p['mood'],p['pace'],p['expression'],p['seed'],bool(g),g.get('temperature',1),g.get('top_p',1),g.get('top_k',50),g.get('max_new_tokens',2400),g.get('repetition_penalty',1),
        '{}',p['strategy'],p['singular'],s['trim'],s['leading_ms'],s['trailing_ms'],s['gap_ms'],s['crossfade_ms'],s['fade_ms'],s['normalize'],s.get('edge_policy','legacy')]

class SimpleListeningTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        import app
        cls.app=app;cls.studio=app.indian_studio
        cls.callbacks={f.api_name:f.fn for f in app.demo.fns.values()}
    def args(self,cid='lakshmi_amma',id=None):
        return [cid,id or self.studio.rows(cid)[0]['inventory_id'],'234','300','2','Seer','pepper',*settings(self.studio.profile(cid))]
    def test_all_36_original_comparisons_can_be_found_without_generation(self):
        with patch.object(self.studio,'run',side_effect=AssertionError('Auto-load must not synthesize')):
            pilot=load(sorted((self.studio.data).glob('pilot_*.json'))[-1])
            self.assertEqual(36,len(pilot['outcomes']))
            for outcome in pilot['outcomes']:
                    cid=outcome['character_id'];r=load(outcome['record'])
                    v=r['plan']['values'];values=dict(price=str(v['price']),seller_price=str(v['seller_price']),quantity=str(v['quantity']),unit=v['unit'],spice=v['spice'])
                    plan,loaded=find_saved(self.studio,cid,r['plan']['dialogue_id'],values,r['profile'])
                    self.assertIsNotNone(loaded,(cid,r['plan']['dialogue_id'],r['plan']['strategy']))
                    self.assertEqual(plan['tts_input'],loaded['plan']['tts_input'])
    def test_initial_and_character_dialogue_callbacks_load_matching_players(self):
        for cid in ASSIGNMENTS:
            change=self.callbacks['indian_character'](cid)
            self.assertTrue(change[0]['choices'][0][0].startswith('Greeting'))
            self.assertEqual(ASSIGNMENTS[cid][1],change[1])
            result=self.callbacks['indian_initial'](*self.args(cid))
            self.assertEqual(24,len(result));self.assertTrue(Path(result[10]).is_file());self.assertTrue(Path(result[11]).is_file())
            self.assertEqual(cid,result[16]['plan']['character_id']);self.assertEqual(result[2],result[16]['plan']['tts_input'])
            self.assertFalse(result[18]['visible']);self.assertFalse(result[5]['visible']);self.assertTrue(result[7]['visible'])
    def test_changed_price_clears_both_players_and_shows_generate(self):
        cid='lakshmi_amma';row=next(r for r in self.studio.rows(cid) if r['scenario']=='BuyerCounterFirst')
        args=self.args(cid,row['inventory_id']);ready=self.callbacks['indian_refresh'](*args)
        self.assertIsNotNone(ready[10]);args[2]='235';missing=self.callbacks['indian_refresh'](*args)
        self.assertIsNone(missing[10]);self.assertIsNone(missing[11]);self.assertIsNone(missing[16]);self.assertTrue(missing[18]['visible'])
        self.assertIn('thirty-five',missing[2]);self.assertTrue(missing[5]['visible'])
    def test_fixed_line_and_hidden_unused_values(self):
        row=next(r for r in self.studio.rows('abdul_rahman') if r['scenario']=='TransactionSuccess')
        args=self.args('abdul_rahman',row['inventory_id']);args[2:5]=['unused','unused','unused']
        result=self.callbacks['indian_refresh'](*args)
        self.assertIsNotNone(result[10]);self.assertTrue(all(not r['visible'] for r in result[5:10]))
    def test_wrong_voice_preset_or_invalid_value_never_plays_stale_audio(self):
        args=self.args();args[7]+=' different delivery'
        self.assertIsNone(self.callbacks['indian_refresh'](*args)[10])
        args=self.args();args[4]='invalid';result=self.callbacks['indian_refresh'](*args)
        self.assertIsNone(result[10]);self.assertFalse(result[18]['interactive'])
    def test_generate_button_invokes_backend_once_and_reports_failures(self):
        with patch.object(self.studio,'run') as run:
            result=self.callbacks['indian_generate'](*self.args())
            run.assert_called_once();self.assertTrue(run.call_args.kwargs['generate']);self.assertIsNotNone(result[10])
        with patch.object(self.studio,'run',side_effect=RuntimeError('test failure')):
            result=self.callbacks['indian_generate'](*self.args())
            self.assertIn('Could not prepare comparison: test failure',result[19])
    def test_restore_syncs_values_settings_and_rejects_other_character(self):
        supported={r['inventory_id'] for r in self.studio.rows('lakshmi_amma')}
        r=next(r for r in self.studio.results('lakshmi_amma') if r['plan']['dialogue_id'] in supported and not r['profile']['processing'].get('zero_number_boundaries'))
        restored=self.callbacks['indian_restore'](r['record_path'],'lakshmi_amma',r['plan']['dialogue_id'])
        refreshed=self.callbacks['indian_refresh']('lakshmi_amma',r['plan']['dialogue_id'],*restored)
        self.assertIsNotNone(refreshed[10]);self.assertEqual(r['plan']['tts_input'],refreshed[2])
        with self.assertRaises(ValueError):self.callbacks['indian_restore'](r['record_path'],'abdul_rahman',r['plan']['dialogue_id'])
    def test_rebuilt_speech_aware_variants_load_through_existing_ui(self):
        for strategy in ['Phrase numbers','Compact grammar']:
            r=next(r for r in self.studio.results('lakshmi_amma','L1_LAK_001') if r['plan']['strategy']==strategy and r['profile']['processing'].get('edge_policy')=='hysteresis-v1' and not r['profile']['processing'].get('zero_number_boundaries'))
            args=['lakshmi_amma','L1_LAK_001','234','300','2','Seer','pepper',*settings(r['profile'])]
            result=self.callbacks['indian_refresh'](*args)
            self.assertEqual(r['assembled_wav'],result[11]);self.assertFalse(result[18]['visible'])
    def test_advanced_collapsed_and_original_tabs_preserved(self):
        configs=self.app.demo.config['components']
        accordion=next(c for c in configs if c['type']=='accordion' and c['props'].get('label')=='Advanced Settings')
        self.assertFalse(accordion['props']['open'])
        tabs=[c['props'].get('label') for c in configs if c['type']=='tabitem']
        for label in ['Voice audition','Speaker comparison','Clip join preview']:self.assertIn(label,tabs)
        self.assertIsNone(self.app.SESSION.model)

if __name__=='__main__':unittest.main()
