"""Extend the existing Gradio studio; preserve approved presets and Father audio."""
import json,hashlib,shutil,sys
from pathlib import Path
from datetime import datetime,timezone
ROOT=Path(r'G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator')
REPO=ROOT.parent/'AI-Enabled-Storytelling-VR'
STAGE=Path(__file__).resolve().parent
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def copy_new(source,dest):
    dest.parent.mkdir(parents=True,exist_ok=True)
    with Path(source).open('rb') as s,dest.open('xb') as d:shutil.copyfileobj(s,d)
def main():
    data=ROOT/'testing/indian_auditions';data.mkdir(parents=True,exist_ok=True)
    protected={str(p):sha(p) for p in [ROOT/'character_presets.json',ROOT/'tts_text.py',ROOT/'production/voices.json',ROOT/'production/production_manifest.csv']}
    protected.update({str(p):sha(p) for p in (ROOT/'production/packages/FatherPenteado_Level1_v2').rglob('*') if p.is_file()})
    marker=data/'preservation_baseline.json'
    if not marker.exists():marker.write_text(json.dumps(protected,indent=2),encoding='utf-8')
    else:assert all(sha(Path(p))==h for p,h in json.loads(marker.read_text()).items()),'Protected data changed since extension install'
    mapping={'Lakshmamma':'lakshmi_amma','ChinnammaNaik':'chinnamma_naik','SaraswatiChetti':'saraswati_chetti','AbdulRahman':'abdul_rahman'}
    library=dict(version='four-indian-audited-selections-v1',characters={})
    for folder,cid in mapping.items():
        audit=REPO/'documentation/DialogueAudit'/(folder+'_Discovery')
        for name in ['AUDIT.md','EXACT_LINES.md','DIALOGUE_INVENTORY.csv','DIALOGUE_INVENTORY.json','LANGUAGE_AND_PROPOSALS.md']:(audit/name).read_text(encoding='utf-8-sig')
        inv=json.loads((audit/'DIALOGUE_INVENTORY.json').read_text(encoding='utf-8'));ids=inv['test_selection_ids'];assert len(ids)==12
        rows=[next(r for r in inv['records'] if r['inventory_id']==id) for id in ids]
        for r in rows:
            source=REPO/r['source_file'];assert r['exact_template'] in source.read_text(encoding='utf-8-sig').splitlines()[r['source_line']-1]
        library['characters'][cid]=dict(character=inv['character'],selections=rows,audit_hashes={str(p):sha(p) for p in audit.iterdir() if p.is_file()})
    library_path=data/'library.json';rendered=json.dumps(library,indent=2,ensure_ascii=False)+'\n'
    if library_path.exists() and library_path.read_text(encoding='utf-8')!=rendered:
        copy_new(library_path,data/('library_previous_'+datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')+'.json'))
    library_path.write_text(rendered,encoding='utf-8')
    for name in ['indian_testing.py','indian_testing_ui.py','run_indian_test_pilot.py','test_indian_testing.py']:
        target=ROOT/name
        if target.exists() and sha(target)!=sha(STAGE/name):
            copy_new(target,ROOT/'maintenance_backups/indian_studio_upgrade'/datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%f')/name)
        target.write_bytes((STAGE/name).read_bytes())
    p=ROOT/'app.py';text=p.read_text(encoding='utf-8')
    if '# INDIAN_AUDITION_EXTENSION' not in text:
        backup=ROOT/'maintenance_backups/indian_studio_upgrade/original_app.py'
        if not backup.exists():copy_new(p,backup)
        text=text.replace('import secrets\n','import secrets\nimport re\nimport threading\nimport functools\n',1)
        helper='''# INDIAN_AUDITION_EXTENSION: serialize old/new Parler calls on the shared model.
_MODEL_LOCK = threading.RLock()
def _serialized_generation(function):
    @functools.wraps(function)
    def call(*args, **kwargs):
        with _MODEL_LOCK:
            return function(*args, **kwargs)
    return call

'''
        text=text.replace('class ModelSession:\n',helper+'class ModelSession:\n',1)
        text=text.replace('    def generate(self, text:', '    @_serialized_generation\n    def generate(self, text:',1)
        old='dialogue_id: str | None = None) -> tuple[str, dict]:';assert old in text
        text=text.replace(old,'dialogue_id: str | None = None, output_directory: Path | None = None,\n                 generation_overrides: dict | None = None) -> tuple[str, dict]:',1)
        text=text.replace('        description = describe(speaker, base, mood, pace, expression)\n',
            '        overrides = generation_overrides or {}\n        if not set(overrides) <= {"temperature", "top_p", "top_k", "max_new_tokens", "repetition_penalty"}:\n            raise ValueError("Unsupported Parler generation override.")\n        description = describe(speaker, base, mood, pace, expression)\n',1)
        text=text.replace('                    prompt_attention_mask=prompt.attention_mask,\n','                    prompt_attention_mask=prompt.attention_mask,\n                    **overrides,\n',1)
        text=text.replace('            path = new_sample_path(speaker, sample_stage, sample_label)\n','''            if output_directory is None:
                path = new_sample_path(speaker, sample_stage, sample_label)
            else:
                directory = Path(output_directory).resolve()
                if not directory.is_relative_to(SAMPLES.resolve()):
                    raise ValueError("Audition outputs must remain under samples.")
                if not sample_label or not re.fullmatch(r"[A-Za-z0-9_-]+", sample_label):
                    raise ValueError("Safe descriptive sample label required.")
                directory.mkdir(parents=True, exist_ok=True)
                path = fresh_export_path(directory, sample_label)
''',1)
        text=text.replace('                "generation_settings": json.loads(self.model.generation_config.to_json_string(use_diff=False)),',
            '                "generation_settings": json.loads(self.model.generation_config.to_json_string(use_diff=False)),\n                "generation_overrides": overrides,',1)
        marker='    with gr.Tab("Voice audition"):';assert marker in text
        text=text.replace(marker,'    from indian_testing_ui import mount\n    indian_studio = mount(gr, SESSION)\n'+marker,1)
        text=text.replace('allowed_paths=[str(SAMPLES), str(EXPORTS)]','allowed_paths=[str(SAMPLES), str(EXPORTS), str(ROOT / "testing/indian_auditions")]')
        compile(text,str(p),'exec');p.write_text(text,encoding='utf-8')
    launcher=ROOT/'launch.bat';launch_text=launcher.read_text(encoding='utf-8')
    if 'HF_HUB_OFFLINE' not in launch_text:
        backup=ROOT/'maintenance_backups/indian_studio_upgrade/original_launch.bat'
        if not backup.exists():copy_new(launcher,backup)
        launch_text=launch_text.replace('set "GRADIO_ANALYTICS_ENABLED=False"','set "GRADIO_ANALYTICS_ENABLED=False"\nset "HF_HUB_OFFLINE=1"\nset "TRANSFORMERS_OFFLINE=1"')
        launcher.write_text(launch_text,encoding='utf-8')
    assert all(sha(Path(p))==h for p,h in protected.items()),'Protected data changed'
    sys.path.insert(0,str(ROOT))
    from indian_testing import TestingStudio,ASSIGNMENTS
    studio=TestingStudio()
    for cid in ASSIGNMENTS:
        if not (studio.data/'settings'/cid/'CURRENT.json').exists():studio.save_profile(cid,studio.default_profile(cid))
    print('Installed existing-studio extension;48 audited selections. Presets/Father preserved. No audio generated by installer.')
if __name__=='__main__':main()
