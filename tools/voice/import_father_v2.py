"""Deterministic Father-only asset import without launching Unity or synthesizing audio."""
import hashlib,json,re,shutil,uuid,wave
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
UNITY=ROOT/'unity/StorytellingVR';ASSETS=UNITY/'Assets'
SOURCE=ROOT.parent/'NPC-Voice-Generator/production/packages/FatherPenteado_Level1_v2'
DEST=ASSETS/'_Project/Audio/NPCVoices/Level1/FatherPenteadoV2'
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))
def guid(p):return uuid.uuid5(uuid.NAMESPACE_URL,'anubhava:'+Path(p).relative_to(ASSETS).as_posix()).hex
def meta(p,body):
    path=Path(str(p)+'.meta')
    if path.exists():return re.search(r'^guid: (\w+)',path.read_text(),re.M)[1]
    path.write_text('fileFormatVersion: 2\nguid: '+guid(p)+'\n'+body,encoding='utf-8');return guid(p)
def folders(p):
    if p==ASSETS:return
    folders(p.parent);p.mkdir(exist_ok=True);meta(p,'folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
def text(p,value):
    folders(p.parent);p.write_text(value,encoding='utf-8');return meta(p,'TextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
def copy_original(source,target):
    folders(target.parent)
    if target.exists():assert sha(source)==sha(target),('Refusing to overwrite original audio/metadata',target)
    else:shutil.copyfile(source,target)
def pairs(items):return [dict(key=k,id=v) for k,v in items.items()]
def main():
    m=read(SOURCE/'mapping_manifest.json');c=read(SOURCE/'runtime_assembly.json');assert m['character_id']=='father_penteado' and len(m['recordings'])==425
    before={str(p):sha(p) for p in SOURCE.rglob('*') if p.is_file()}
    templates=[]
    for t in m['templates']:
        ops=[]
        for o in t['operations']:
            ops.append(dict(kind=o['kind'],id=o.get('id',''),singularId=o.get('singular_id',''),priceSlot=o.get('price_slot',''),name=o.get('name',''),sourcePiece=o.get('source_piece',''),text=o.get('text',''),choices=pairs(o.get('choices',{}))))
        templates.append(dict(id=t['id'],scenario=t['scenario'],category=t['category'],sourceTemplate=t['source_expression_template'],spokenTemplate=t['spoken_template'],operations=ops,
            requiresAgreement=t['id'] in c['agreement_sources'],requiresSettlement=t['scenario']=='TransactionSuccess',conditionalCounter=t['id'] in c['conditional_counter_sources']))
    runtime=dict(characterId=m['character_id'],sampleRate=24000,templates=templates,numbers=pairs(c['numbers']),spices=pairs(c['spices']),descriptors=pairs(c['descriptor_vocabulary']),units=pairs(c['quantity_units']),aliases=pairs(c['slot_aliases']),quantityFallback=c['quantity_fallback'],holdTemplateId='L1_FAT_053',edgeGuardClipIds=[r['id'] for r in m['recordings'] if r.get('source_edge_warning')])
    byid={r['id']:r for r in m['recordings']}
    for entry in runtime['descriptors']:entry['text']=byid[entry['id']]['spoken_text']
    mapping=DEST/'FatherVoiceBank.json';mapping_guid=text(mapping,json.dumps(runtime,indent=2,ensure_ascii=False)+'\n')
    dbscript=ASSETS/'_Project/Scripts/FreeRoam/DialogueVoiceDatabase.cs';dbguid=re.search(r'^guid: (\w+)',Path(str(dbscript)+'.meta').read_text(),re.M)[1]
    entries=[]
    for r in m['recordings']:
        source=SOURCE/r['wav_path'];assert sha(source)==r['wav_sha256'];target=DEST/'Audio'/(r['id']+'.wav');copy_original(source,target)
        with wave.open(str(target)) as f:assert f.getframerate()==24000 and f.getnchannels()==1 and f.getsampwidth()==3
        clipguid=meta(target,'''AudioImporter:
  externalObjects: {}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 24000
    compressionFormat: 0
    quality: 1
    conversionMode: 0
    preloadAudioData: 0
  platformSettingOverrides: {}
  forceToMono: 0
  normalize: 0
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData:
  assetBundleName:
  assetBundleVariant:
''')
        entries.append('  - lineId: '+r['id']+'\n    audioClip: {fileID: 8300000, guid: '+clipguid+', type: 3}\n    description: '+json.dumps(r['spoken_text'],ensure_ascii=False))
        metadata=DEST/'Metadata'/(r['id']+'.json');copy_original(SOURCE/r['metadata_path'],metadata);meta(metadata,'TextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    asset=DEST/'FatherVoiceDatabase.asset'
    asset.write_text('''%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: '''+dbguid+''', type: 3}
  m_Name: FatherVoiceDatabase
  m_EditorClassIdentifier: Assembly-CSharp::DialogueVoiceDatabase
  entries:
'''+ '\n'.join(entries)+'\n',encoding='utf-8')
    dbassetguid=meta(asset,'NativeFormatImporter:\n  externalObjects: {}\n  mainObjectFileID: 11400000\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    for name in ['mapping_manifest.json','runtime_assembly.json','recording_aliases.json','FATHER_DIALOGUE_ISSUES.csv','FATHER_DIALOGUE_AUDIT.md','FATHER_DIALOGUE_COVERAGE.md']:
        target=DEST/'Provenance'/name;copy_original(SOURCE/name,target);meta(target,'TextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    scripts=ASSETS/'_Project/Scripts/Level1'
    for filename in ['NpcVoicePresentation.cs','PreRecordedNpcTtsProvider.cs']:
        meta(scripts/filename,'MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    test=ASSETS/'_Project/Scripts/Editor/NegotiationTests/Level1PrerecordedVoiceTests.cs'
    if test.exists():meta(test,'MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
    providerguid=meta(scripts/'PreRecordedNpcTtsProvider.cs','')
    scene=ASSETS/'_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity';source=scene.read_text(encoding='utf-8-sig')
    component_id='1028356668'
    if 'prerecordedNpcProvider:' not in source:
        assert source.count('  localNpcTtsProvider: {fileID: 1028356663}')==1
        source=source.replace('  localNpcTtsProvider: {fileID: 1028356663}','  localNpcTtsProvider: {fileID: 1028356663}\n  prerecordedNpcProvider: {fileID: '+component_id+'}')
    if '--- !u!114 &'+component_id not in source:
        assert source.count('  - component: {fileID: 1028356663}')==1
        source=source.replace('  - component: {fileID: 1028356663}','  - component: {fileID: 1028356663}\n  - component: {fileID: '+component_id+'}')
        source+='''--- !u!114 &'''+component_id+'''
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1028356660}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: '''+providerguid+''', type: 3}
  m_Name:
  m_EditorClassIdentifier: Assembly-CSharp::PreRecordedNpcTtsProvider
  audioSource: {fileID: 366259241}
  characterBanks:
  - characterId: father_penteado
    mapping: {fileID: 4900000, guid: '''+mapping_guid+''', type: 3}
    clips: {fileID: 11400000, guid: '''+dbassetguid+''', type: 2}
'''
        scene.write_text(source,encoding='utf-8')
    assert all(sha(p)==h for p,h in before.items()),'Voice package changed'
    print(json.dumps(dict(imported_wavs=len(m['recordings']),mapped_templates=len(templates),source_package_unchanged=True,scene=str(scene),bank=str(mapping),database=str(asset)),indent=2))
if __name__=='__main__':main()
