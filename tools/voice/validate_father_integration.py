"""Targeted compilation, pure planner conformance and import checks. Never starts Unity."""
import hashlib,importlib.util,json,re,subprocess,sys,tempfile,wave
from pathlib import Path
sys.dont_write_bytecode=True
ROOT=Path(__file__).resolve().parents[2];U=ROOT/'unity/StorytellingVR';A=U/'Assets';L=A/'_Project/Scripts/Level1'
V=ROOT.parent/'NPC-Voice-Generator/production/packages/FatherPenteado_Level1_v2';D=A/'_Project/Audio/NPCVoices/Level1/FatherPenteadoV2'
EDITOR=Path(r'E:\Unity\Editor\6000.3.10f1\Editor\Data');CSC=EDITOR/'DotNetSdkRoslyn/csc.dll';MONO=EDITOR/'MonoBleedingEdge/bin/mono.exe';NET=EDITOR/'MonoBleedingEdge/lib/mono/4.8-api'
def load(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))
def sha(p):return hashlib.sha256(Path(p).read_bytes()).hexdigest()
def module(name,p):
    spec=importlib.util.spec_from_file_location(name,p);m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);return m
def run(args,cwd=ROOT):
    p=subprocess.run([str(a) for a in args],cwd=cwd,text=True,encoding='utf-8',errors='replace',stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    if p.returncode:raise RuntimeError(p.stdout)
    return p.stdout
def main():
    source=load(V/'mapping_manifest.json');bank=load(D/'FatherVoiceBank.json');assert len(bank['templates'])==259
    db=(D/'FatherVoiceDatabase.asset').read_text(encoding='utf-8');ids=set(re.findall(r'lineId: (\S+)',db));assert len(ids)==425
    for r in source['recordings']:
        wav=D/'Audio'/(r['id']+'.wav');assert r['id'] in ids and sha(wav)==r['wav_sha256']
        with wave.open(str(wav)) as f:assert f.getframerate()==24000 and f.getnchannels()==1 and f.getsampwidth()==3
        meta=Path(str(wav)+'.meta').read_text();assert 'compressionFormat: 0' in meta and 'loadType: 0' in meta and 'normalize: 0' in meta and 'preloadAudioData: 0' in meta
        guid=re.search(r'^guid: (\w+)',meta,re.M)[1];assert guid in db
    def required_ids():
        for t in bank['templates']:
            for op in t['operations']:
                if op['kind'] in {'CLIP','CURRENCY_CLIP'}:yield op['id']
                if op['kind']=='CURRENCY_CLIP':yield op['singularId']
                for choice in op['choices']:yield choice['id']
        for group in ['numbers','spices','descriptors','units']:
            for p in bank[group]:yield p['id']
    assert set(required_ids())<=ids
    scene=(A/'_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity').read_text(encoding='utf-8-sig')
    assert 'prerecordedNpcProvider: {fileID: 1028356668}' in scene and 'localNpcTtsProvider: {fileID: 1028356663}' in scene
    assert 'audioSource: {fileID: 366259241}' in scene and 'Assembly-CSharp::PreRecordedNpcTtsProvider' in scene
    provider_guid=re.search(r'^guid: (\w+)',Path(str(L/'PreRecordedNpcTtsProvider.cs')+'.meta').read_text(),re.M)[1];assert provider_guid in scene
    reference=module('father_reference',V/'reference_assembler.py');fixtures=module('father_fixtures',V/'test_package.py')
    rule=next(t['id'] for t in source['templates'] if t['source_expression_template']=='The market is busy and your words were unclear. Say it again plainly.')
    ruletext='The market is busy and your words were unclear. Say it again plainly.';cases=[]
    def case(t,v):
        expected=reference.plan(t['id'],v,rule)['spoken_text'];flat={k:str(val).lower() if type(val)is bool else str(val) for k,val in v.items() if k!='acceptedTerms'}
        flat.update(buyerName='Father Penteado',buyerOrigin='Portuguese Cleric',currency='varahas',ruleReply=ruletext,quantity=v['brainQuantityLabel'] if 'brainQuantityLabel' in v else v['quantityLabel'],currentOffer=str(v['previousOffer']),counterOffer=str(v['updatedOffer']),nextOffer=str(v['updatedOffer']))
        flat['spiceDescriptor']={'pepper':'black pepper','cinnamon':'cinnamon bark','cardamom':'green cardamom'}[v['spiceName']]
        flat['spice']=v['spiceName']
        flat['result.updatedOffer']=str(v['updatedOffer']);flat['input.referencedPrices[0]']=str(v['firstReferencedPrice']);flat['input.referencedPrices[input.referencedPrices.Count - 1]']=str(v['lastReferencedPrice'])
        flat['hasAcceptedTerms']='true' if 'acceptedTerms' in v else 'false'
        if 'acceptedTerms' in v:
            frozen=v['acceptedTerms'];flat.update(acceptedPrice=str(frozen['price']),acceptedQuantityLabel=frozen['quantityLabel'],acceptedSpiceName=frozen['spiceName'])
            flat.update({'result.acceptedTerms.Price':str(frozen['price']),'result.acceptedTerms.QuantityLabel':frozen['quantityLabel'],'result.acceptedTerms.SpiceName':frozen['spiceName']})
        flat.setdefault('brainQuantityLabel',flat['quantityLabel'])
        raw=re.sub(r'\{([^{}]+)\}',lambda m:flat[m[1]],t['source_expression_template'])
        cases.append(dict(id=t['id'],scenario=t['scenario'],source=t['source_expression_template'],original=raw,ruleReply=ruletext,values=flat,expected=expected))
    for t in source['templates']:
        for price in [1,19,34,100,105,234,500,909,999]:
            for spice in ['pepper','cinnamon','cardamom']:
                for quantity in ['1 Seer (~280g)','2 Veesai (~2.8 kg)']:
                    v=fixtures.context(t,price,spice,quantity);v['brainQuantityLabel']=quantity;case(t,v)
    t=next(t for t in source['templates'] if t['id']=='L1_FAT_042')
    for price in range(1,1000):
        v=fixtures.context(t,price);v['brainQuantityLabel']=v['quantityLabel'];case(t,v)
    scratch=ROOT/'tools/voice/.checks';scratch.mkdir(exist_ok=True)
    with tempfile.TemporaryDirectory(prefix='AnubhavaFatherChecks_',dir=scratch) as folder:
        assert Path(folder).resolve().is_relative_to(scratch.resolve())
        temp=Path(folder);rsp=temp/'runtime.rsp';out=temp/'FatherRuntimeCompile.dll'
        original=(U/'Library/Bee/artifacts/1900b0aE.dag/Assembly-CSharp.rsp').read_text(encoding='utf-8-sig').splitlines()
        flags=[line for line in original if line.startswith(('-r:','-define:','-langversion:','-nostdlib'))]
        flags+=['-nologo','-target:library','-nowarn:0436','-out:"'+str(out)+'"','-r:"'+str(U/'Library/ScriptAssemblies/Assembly-CSharp.dll')+'"']
        files=[L/'NpcVoicePresentation.cs',L/'PreRecordedNpcTtsProvider.cs',L/'AudioManager.cs',L/'ChatManager.cs',L/'Core/DialogueTableResponseProvider.cs',A/'_Project/Scripts/Editor/NegotiationTests/Level1PrerecordedVoiceTests.cs']
        flags+=['"'+str(p)+'"' for p in files];rsp.write_text('\n'.join(flags),encoding='utf-8')
        print('Targeted edited-source compilation:',run(['dotnet','exec','--roll-forward','Major',CSC,'@'+str(rsp)],U).strip() or 'PASS',flush=True)
        console=temp/'FatherPlannerChecks.exe';refs=[NET/n for n in ['mscorlib.dll','System.dll','System.Core.dll','System.Web.Extensions.dll']]
        args=['dotnet','exec','--roll-forward','Major',CSC,'-nologo','-target:exe','-out:'+str(console),'-nostdlib+']+['-r:'+str(p) for p in refs]+[L/'NpcVoicePresentation.cs',ROOT/'tools/voice/FatherVoiceStaticChecks.cs']
        print(run(args).strip(),flush=True)
        casesfile=temp/'cases.json';casesfile.write_text(json.dumps(cases,ensure_ascii=False),encoding='utf-8')
        result=run([MONO,console,D/'FatherVoiceBank.json',casesfile]);print(result.strip(),flush=True)
    print(json.dumps(dict(result='PASS',targeted_csharp_compilation=True,production_planner_cases=len(cases),imported_wav_hashes_verified=425,scene_and_guid_checks=True,unity_editor_tests_run=False,quest_run=False),indent=2))
if __name__=='__main__':main()
