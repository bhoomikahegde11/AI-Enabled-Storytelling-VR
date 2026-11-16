"""Documentation-only source inspection. Never invokes Unity, C# or a speech engine."""
import collections
import copy
import csv
import hashlib
import importlib.util
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent
U = ROOT/'unity/StorytellingVR'
CORE = U/'Assets/_Project/Scripts/Level1/Core'
STUDIO = ROOT.parent/'NPC-Voice-Generator/production'

def module(name,path):
    spec=importlib.util.spec_from_file_location(name,path);m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);return m

ex=module('dialogue_source_reader',OUT/'extract_level1_dialogues.py')
lak=module('lak_source_reference',OUT/'Lakshmamma_Discovery/build_inventory.py')
baseline=json.loads((OUT/'Lakshmamma_Discovery/LAKSHMAMMA_DIALOGUE_INVENTORY.json').read_text(encoding='utf-8'))
CONFIGS=[
 dict(name='Lakshmamma',cid='lakshmi_amma',prefix='LAK',file='LakshmiAmmaDialogue.cs',folder='Lakshmamma_Discovery',base='Friendly',role='Local Household Buyer',voice='Swapna'),
 dict(name='Chinnamma Naik',cid='chinnamma_naik',prefix='CHI',file='ChinappaNaikDialogue.cs',folder='ChinnammaNaik_Discovery',base='Strict',role='Vijayanagara Wholesale Spice Buyer',voice='Priya'),
 dict(name='Saraswati Chetti',cid='saraswati_chetti',prefix='SAR',file='SiddharthChettiDialogue.cs',folder='SaraswatiChetti_Discovery',base='Friendly',role='Local Vijayanagara Retail Shop Owner',voice='Sneha'),
 dict(name='Abdul Rahman',cid='abdul_rahman',prefix='ABD',file='AbdulRahmanDialogue.cs',folder='AbdulRahman_Discovery',base='Friendly',role='Arab Caravan Trader',voice='Kabir')]
CLASS_EVIDENCE={
 'CORE':'Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency.',
 'REACHABLE_OPTIONAL':'Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions.',
 'FALLBACK_ONLY':'Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists.',
 'UNREACHABLE_OR_DEFERRED':'No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement.'}
OPTIONAL={'SellerPriceBelowExpected','OfferReason','AskName','AskOrigin','WeatherQuestion','HistoryKing','HistoryTemple','HistoryQuestion','SocialGreeting','OffTopic','HostileWarning','Unknown'}

def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def path(p):return str(p.relative_to(ROOT)).replace('\\','/')
def line(p,text):return p.read_text(encoding='utf-8-sig')[:p.read_text(encoding='utf-8-sig').index(text)].count('\n')+1
def ref(p,text):return '`'+path(p)+':'+str(line(p,text))+'`'
def counts(xs,key):return dict(collections.Counter(x[key] for x in xs))
def static_chunks(xs):return {p.strip() for r in xs if r['placeholders'] for p in re.split(r'\{[^{}]+\}',r['exact_template']) if re.search(r'\w',p)}
def text_pool(xs):return static_chunks(xs)|{r['exact_template'] for r in xs if not r['placeholders']}

inventory=json.loads((STUDIO/'inventory.json').read_text(encoding='utf-8-sig'))
assemblies=json.loads((STUDIO/'dynamic_assemblies.json').read_text(encoding='utf-8-sig'))
manifest=list(csv.DictReader((STUDIO/'production_manifest.csv').open(encoding='utf-8-sig',newline='')))
voices=json.loads((STUDIO/'voices.json').read_text(encoding='utf-8-sig'))
scene_path=U/'Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity'
scene=scene_path.read_text(encoding='utf-8-sig')
forced=re.search(r'^  debugForceCharacterId:\s*([^\n]*)',scene,re.M)[1].strip()
source_files={ROOT/p for p in baseline['source_hashes']}|{CORE/'Dialogues/Characters'/c['file'] for c in CONFIGS}|{
 CORE/'Dialogues/DialogueLine.cs',CORE/'Dialogues/DialogueScenario.cs',CORE/'Dialogues/DialogueBuckets.cs',CORE/'Dialogues/DIALOGUE_AUTHORING_GUIDE.txt',
 U/'Assets/_Project/Scripts/Level1/AudioManager.cs',U/'Assets/_Project/Scripts/Level1/PreRecordedNpcTtsProvider.cs',
 U/'Assets/_Project/Scripts/Level1/NpcVoicePresentation.cs',U/'Assets/_Project/Scripts/Level1/ConversationTurnLifecycle.cs'}
before={path(p):sha(p) for p in source_files}
studio_hashes={str(p):sha(p) for p in [STUDIO/'inventory.json',STUDIO/'dynamic_assemblies.json',STUDIO/'production_manifest.csv',STUDIO/'voices.json']}
# Read all old audit artifacts; validate content rather than regenerating the previous audit.
old_artifacts={p.name:p.read_text(encoding='utf-8-sig') for p in (OUT/'Lakshmamma_Discovery').iterdir() if p.is_file()}
assert len(list(csv.DictReader(old_artifacts['LAKSHMAMMA_DIALOGUE_INVENTORY.csv'].splitlines())))==345
old_drift=[p for p,h in baseline['source_hashes'].items() if sha(ROOT/p)!=h]
assert all(p.endswith('Level1_MainLoopUpdated.unity') for p in old_drift),'Shared source changed; re-audit shared extraction before reuse'
brain=CORE/'RuleBasedNPCBrain.cs';brain_text=brain.read_text(encoding='utf-8-sig')
brain_literals={t for _,t in lak.literals(brain_text) if ' ' in t and re.search(r'[.!?]$',t) and not t.startswith('[')}
assert brain_literals=={r['exact_template'] for r in baseline['records'] if r['category']=='BRAIN_FALLBACK'}

def classify(r):
    if r['reachability'] in {'UNROUTED','PREEMPTED_BRANCH','CONTEXT_ONLY','NOT_ROUTED_FOR_LAKSHMAMMA','TEXT_ONLY_NOT_SPOKEN'} or r['category']=='GENERIC_BYPASSED':return 'UNREACHABLE_OR_DEFERRED'
    if r['category']!='AUTHORED':return 'REACHABLE_OPTIONAL' if r['reachability']=='NESTED_REACHABLE' and r['source_line'] in {732,745,750,755,760,765,770,775,779} else 'FALLBACK_ONLY'
    return 'REACHABLE_OPTIONAL' if r['scenario'] in OPTIONAL else 'CORE'

def create_records(c):
    es=ex.parse_character_file(c['name'],c['cid'],c['file'],c['prefix']);out=[];seq=0
    values=dict(lak.VALUES,buyerName=c['name'],characterName=c['name'],buyerOrigin=c['role'])
    for e in es:
        for text,ln in e['templates']:
            seq+=1;s=e['scenario'];trigger=lak.TRIGGERS[s]
            if s=='OfferReason' and c['prefix']!='LAK':trigger='PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).'
            r=dict(inventory_id=f'L1_{c["prefix"]}_{seq:03d}',character_id=c['cid'],speaker=c['name'],category='AUTHORED',phase=lak.phase(s),scenario=s,
                exact_template=text,placeholders=list(dict.fromkeys(re.findall(r'\{([^{}]+)\}',text))),
                restrictions={k:e[k] for k in ['mood_bucket','patience_bucket','reputation_bucket','desperation_bucket','round_bucket']},
                reachability='UNROUTED' if s=='TimePressure' else 'CONDITIONAL_QUERY_INPUT' if c['prefix']=='LAK' and s=='OfferReason' else 'CONDITIONAL_RESIDUAL' if s=='Unknown' else 'ROUTED',
                trigger=trigger,source_file=path(CORE/'Dialogues/Characters'/c['file']),source_line=ln,
                notes='Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.')
            out.append(r)
    for shared in baseline['records']:
        if shared['category']=='AUTHORED':continue
        r=copy.deepcopy(shared);r['character_id']=c['cid'];r['speaker']=c['name'];r['inventory_id']=r['inventory_id'].replace('AUD_LAK_',f'AUD_{c["prefix"]}_')
        if r['category']=='GENERIC_BYPASSED':r['trigger']=r['trigger'].replace('lakshmi_amma',c['cid']).replace('Lakshmamma',c['name']);r['reachability']='NOT_ROUTED_FOR_CHARACTER'
        out.append(r)
    for r in out:
        text=r['exact_template'];r['audio_type']='NESTED_DYNAMIC' if 'ruleReply' in r['placeholders'] else 'DYNAMIC_TEMPLATE' if r['placeholders'] else 'FULL_CLIP'
        r['coverage_class']=classify(r);r['coverage_evidence']=CLASS_EVIDENCE[r['coverage_class']]+' '+r['trigger']
        r['template_sha256']=hashlib.sha256(text.encode()).hexdigest();r['template_key']=c['cid']+'|'+r['scenario']+'|'+r['template_sha256']
        r['example_rendered']=re.sub(r'\{([^{}]+)\}',lambda m:values[m[1]],text)
        r['example_tts_input']=re.sub(r'\s+',' ',re.sub(r'\([^)]*\)|\[[^]]*\]','',r['example_rendered'])).strip()
        r['assembly_pattern']=[{'kind':'VALUE' if p.startswith('{') and p.endswith('}') else 'LITERAL','text':p} for p in re.split(r'(\{[^{}]+\})',text) if p]
        # These checks inspect text; they do not invoke gameplay/parser/Unity tests.
        assert text.replace('"','\\"') in (ROOT/r['source_file']).read_text(encoding='utf-8-sig').splitlines()[r['source_line']-1]
        assert ''.join(x['text'] for x in r['assembly_pattern'])==text
        assert not re.search(r'\{[^{}]+\}',r['example_rendered'])
    assert len({r['inventory_id'] for r in out})==len(out)
    if c['prefix']=='LAK':
        assert [(r['inventory_id'],r['exact_template']) for r in out if r['category']=='AUTHORED']==[(r['inventory_id'],r['exact_template']) for r in baseline['records'] if r['category']=='AUTHORED']
    return out

def studio_check(c,rows):
    authored=[r for r in rows if r['category']=='AUTHORED'];iv=[r for r in inventory if r.get('speaker')==c['name']];mf=[r for r in manifest if r['speaker']==c['name']];ass=[r for r in assemblies if r['speaker']==c['name']]
    byid={r['source_dialogue_id']:r for r in iv};mi={r['recording_id']:r for r in mf};am={r['source_dialogue_id']:r for r in ass}
    result=dict(inventory_rows=len(iv),inventory_duplicate_ids=[k for k,n in collections.Counter(r['source_dialogue_id'] for r in iv).items() if n>1],
        manifest_duplicate_ids=[k for k,n in collections.Counter(r['recording_id'] for r in mf).items() if n>1],
        missing_templates=[],changed_text=[],restriction_mismatches=[],missing_recording_ids=[],wrong_speaker_ids=[],
        assembly_reconstruction_mismatches=[],fixed_mapping_mismatches=[],dynamic_assembly_missing=[],
        manifest_rows=len(mf),manifest_modes=counts(mf,'generation_mode'),manifest_priorities=counts(mf,'priority'),manifest_statuses=counts(mf,'generation_status'),
        raw_source_hash_drift_ids=[],validated_manifest_rows=[])
    for r in authored:
        id=r['inventory_id'];old=byid.get(id)
        if not old:result['missing_templates'].append(id);continue
        if old['original_dialogue_text']!=r['exact_template']:result['changed_text'].append(id)
        if old.get('source',{}).get('sha256')!=sha(ROOT/r['source_file']):result['raw_source_hash_drift_ids'].append(id)
        rr=old.get('restrictions',{});actual=r['restrictions']
        if rr.get('mood','Any')!=actual['mood_bucket'] or rr.get('patience','Any')!=actual['patience_bucket']:result['restriction_mismatches'].append(id)
        for rid in old.get('recording_ids',[]):
            if rid not in mi:result['missing_recording_ids'].append([id,rid])
            elif mi[rid]['speaker']!=c['name']:result['wrong_speaker_ids'].append([id,rid])
        if not r['placeholders']:
            texts=[mi[rid]['original_dialogue_text'] for rid in old.get('recording_ids',[]) if rid in mi]
            if r['exact_template'] not in texts:result['fixed_mapping_mismatches'].append(id)
            continue
        a=am.get(id)
        if not a:result['dynamic_assembly_missing'].append(id);continue
        rebuilt=''
        for op in a['operations']:
            if op['kind']=='CLIP':
                rid=op['recording_id'];rebuilt+=op['source_exact_piece']
                if rid not in mi:result['missing_recording_ids'].append([id,rid])
                elif mi[rid]['speaker']!=c['name']:result['wrong_speaker_ids'].append([id,rid])
            elif op['kind']=='DYNAMIC_VALUE':rebuilt+='{'+op['placeholder']+'}'
            elif op['kind']=='PUNCTUATION':rebuilt+=op['text']
            else:raise ValueError(op)
        # Old studio material binds currency at authoring export; retain this difference explicitly.
        expected=r['exact_template'].replace('{currency}','varahas')
        if rebuilt!=expected:result['assembly_reconstruction_mismatches'].append(dict(id=id,expected=expected,actual=rebuilt))
    for r in mf:
        if r['generation_status']=='VALIDATED':result['validated_manifest_rows'].append(dict(id=r['recording_id'],dialogue=r['source_dialogue_id'],wav=r['validated_wav_path'],exists=Path(r['validated_wav_path']).is_file()))
    return result

def summarize(rows,studio):
    a=[r for r in rows if r['category']=='AUTHORED'];routed=[r for r in a if r['coverage_class']!='UNREACHABLE_OR_DEFERRED'];cores=[r for r in rows if r['reachability']=='NESTED_REACHABLE']
    broad=[r for r in rows if r['category'] not in {'GENERIC_BYPASSED','UI_ONLY_RECOVERY'} and r['coverage_class']!='UNREACHABLE_OR_DEFERRED']
    vocab=set(baseline['vocabulary']['extended']);p=text_pool(routed+cores);b=text_pool(broad)
    return dict(authored=len(a),unique_authored=len({r['exact_template'] for r in a}),authored_classes=counts(a,'coverage_class'),all_classes=counts(rows,'coverage_class'),
        authored_fixed=sum(not r['placeholders'] for r in a),authored_dynamic=sum(bool(r['placeholders']) for r in a),routed=len(routed),
        routed_fixed=sum(not r['placeholders'] for r in routed),routed_dynamic=sum(bool(r['placeholders']) for r in routed),
        scenarios=counts(a,'scenario'),categories=counts(rows,'category'),placeholders=dict(collections.Counter(k for r in a for k in r['placeholders'])),
        authored_duplicate_texts=[t for t,n in collections.Counter(r['exact_template'] for r in a).items() if n>1],
        per_voice_estimate=len(p|vocab),defensive_estimate=len(b|vocab),character_piece_estimate=len(p),vocabulary_estimate=len(vocab),
        studio=studio)

def samples(c,rows):
    a=[r for r in rows if r['category']=='AUTHORED'];chosen=[]
    specs=[('CustomerGreeting','Any','Any'),('AskName','Any','Any'),('OfferReason','Any','Any'),('BuyerCounterFirst','Any','Any'),
        ('BuyerCounterMiddle','Any','Any'),('QuantityChanged','Any','Any'),('BuyerHoldsFirm','Any','Low'),
        ('SellerPriceTooHigh','Any','Any'),('SellerPriceAccepted','Any','Any'),('UnclearSpeech','Any','Any'),
        ('TransactionSuccess','Any','Any'),('HostileWarning','Any','Any')]
    for scenario,mood,pat in specs:
        candidates=[r for r in a if r['scenario']==scenario and r['restrictions']['mood_bucket']==mood and r['restrictions']['patience_bucket']==pat]
        # Put character colour in the sample selection without changing any text.
        terms={'CHI':['Howdu','Saaku','Beda'],'SAR':['Avunu','Konchem'],'ABD':['Shukran','Marhaban'],'LAK':['Sari','Ayyo']}[c['prefix']]
        selected=next((r for r in candidates if any(w in r['exact_template'] for w in terms)),candidates[0]);chosen.append(selected)
    assert len(chosen)==12 and len({r['inventory_id'] for r in chosen})==12
    return chosen

def sample_md(c,rows):
    ss=samples(c,rows);parts=['## Twelve documented audition selections — no synthesis','',
        'These are actual current Unity/export IDs; rendered examples below use300varahas as a rejected seller ask,234 as the current/counter/agreed offer,2Seers and pepper. They are illustrative snapshots, not a guaranteed twelve-turn conversation. The low-patience HOLD requires actual patience<=2; receipt requires completed delivery/settlement. Clarification uses the exact unrecognized-speech core. Repetition/random selection can choose another table variant in gameplay.',
        'For number articulation, repeat the counter template at1/19/34/105/234/999 and a referenced price above999. Keep original quantities/items dynamic. These are test selections only, not new historical priority flags.','',
        '| ID / phase | Exact Unity template | Illustrative complete TTS input / context |','|---|---|---|']
    for r in ss:
        caveat=' **Conditional provider-input route; normal price-pressure questions use budget wording for Lakshmamma.**' if c['prefix']=='LAK' and r['scenario']=='OfferReason' else ''
        parts+=['| '+r['inventory_id']+' / '+r['scenario']+' | '+r['exact_template'].replace('|','\\|')+' | '+r['example_tts_input'].replace('|','\\|')+caveat+' |']
    return '\n'.join(parts)+'\n'

def common_runtime(c):
    return f'''## Actual current runtime and selection

The saved scene currently has **debugForceCharacterId={forced or '(blank)'}**. Thus {c['name']} is registered and has a configured model/profile, but the current Editor forced-selection branch chooses Father instead. Clear that existing field or explicitly force `{c['cid']}` when testing later. This audit does not change it. The field/use are UNITY_EDITOR-only; normal builds use weighted registry selection. There is no per-profile enabled checkbox excluding this character. All six registry characters have positive weights; only immediate previous-character removal temporarily excludes a candidate in the normal random branch.

Trace: `MarketplaceManager.PrepareIncomingCustomerLifecycle` -> `Level1GameState.GenerateLocalSession` -> `LocalTradeSessionGenerator.Generate` -> configured/weighted character plus real terms -> `ChatManager.StartNewSession/GetGreeting`. Player input -> `InputNormalizer` -> `NegotiationStateManager.ClassifyInput/ProcessLocalTurn` -> `RuleBasedNPCBrain.GenerateReply` -> `LocalTradeState.ApplyDecision` -> `DialogueTableResponseProvider.GetReply/MapScenario/MapAuthoredScenario` -> exact selected template + substitution + `LastPresentation` -> `ChatManager.PresentNpcSubtitleAndTts` -> existing AudioManager. Expected reply state comes from the semantic decision, not from the selected prose. Terminal timeout/reminders and successful delivery have separate provider hooks.

Evidence: {ref(CORE/'LocalTradeSessionGenerator.cs','public LocalGeneratedTradeSession Generate(')}, {ref(CORE/'NegotiationStateManager.cs','public RuleBasedNPCBrainResult ProcessLocalTurn(')}, {ref(CORE/'DialogueTableResponseProvider.cs','private static DialogueScenario MapAuthoredScenario(')}, {ref(CORE/'DialogueTableResponseProvider.cs','private static DialogueScenario MapScenario(')}, {ref(U/'Assets/_Project/Scripts/Level1/ChatManager.cs','private void PresentNpcSubtitleAndTts(')}. Per-line source references and trigger evidence are in the inventory, not inferred from old sheet descriptions.

Scene local intent/brain/session enabled; LLM understanding and generation disabled. Authored characters additionally skip LLM generation even if the local generation flag is enabled. Current prerecorded bank is Father only. Other characters retain their existing provider wiring; no `{c['cid']}` prerecorded bank is registered. Studio assignment is Indic Parler / **{c['voice']}**. This records an existing production decision, not an assertion that the model proves a native regional accent. Other characters' WAVs must never supply this character's number vocabulary.

`FindTemplate` returns `FindRestrictedTemplate` immediately for authored characters: its43 generic registry lines are bypassed, not normally heard emergency alternatives. A missing/exceptional character rendering preserves the selected brain string. All34 scenarios have an unrestricted character group. Restricted lines must satisfy every specified bucket; low patience<=2 outranks mood wording. HOLD alone blends eligible mood and generic pools (70% mood only when both unused pools are available); this is not an observed usage probability. The recent-template queue holds at most4, releasing older entries if a small bucket is exhausted. History resets on a new trade object, not after every utterance.

Personality options are Friendly/Normal/Strict/Impatient. Starting patience7/5/4/3 respectively. Respect changes effective personality/wealth and customer weights; no special new emotion FSM exists. None of this character's lines has an explicit reputation, desperation or round bucket restriction. Round still selects first<=1/middle2–4/final>=5 counter scenarios. Low-patience wording is selected after the decision's patience cost. Normal price/counter/bargain/reject/ultimatum pressure costs1; hostility2; OFF_TOPIC1. Clarification rolls back a counted round and has no extra patience cost. Basic queries/social/quantity-only changes do not count as bargaining rounds.

Acceptance/failure dominate other scenarios. A real upward offer chooses the counter round band; no increase chooses HOLD or the more specific price objection. Explicit PRICE and COUNTER share the economic path. `offeredPrice>maxBuyerPrice` with no raise uses SlightlyHigh only within max(6,minIncrement*3), otherwise TooHigh. Above the previous offer within max(5,minIncrement*2) without a raise also uses SlightlyHigh. Below previous offer without acceptance uses BelowExpected. CounterFinal is a round label, not a binding never-negotiate-again promise. Hard REJECT and DISMISS terminate; soft REJECT can raise or ask the seller's price.

ITEM_QUERY before price introduction maps AskSellerPrice; price/budget questions map AskBuyerBudget. CONTINUE restates budget. Committed QUANTITY_CHANGE maps QuantityChanged. For this character, QUERY_BUYER_BUDGET+asksReason maps OfferReason (Lakshmamma is the explicit exception). Weather/name/king/temple/origin/history are ordered keyword checks only within classified SOCIAL/GENERAL_DIALOGUE; combined questions follow that ordering. No arbitrary question is guaranteed a dedicated answer.

Successful price agreement uses frozen accepted item/quantity/price but does not confirm payment/delivery. TransactionSuccess requires real accepted fulfillment plus SettlementClaimed. Walk-away and idle NO DEAL use TransactionFailure. The idle path uses IdleReminderFirst, IdleReminderSecond then TransactionFailure; it never selects TimePressure. Source terminal customer identity is retained after ActiveTrade clears. Busy capture/recognition/review/NPC response does not advance player-idle time.
'''

def dynamic_md(c,s):
    lines=['## Dynamic values and real range','',
        '| Actual authored placeholder | Templates | Value owner |','|---|---:|---|']
    owners={'quantityLabel':'Committed label; frozen accepted label at agreement/receipt','spiceName':'Current lowercase display spice or accepted snapshot spice','currency':'Current provider literal varahas (not inflected)','currentBuyerOffer':'Committed current offer','counterPrice':'Positive updated offer, else current offer','offeredPrice':'Seller input/last price; SafeNumber fallback','finalPrice':'Frozen agreement price when accepted','ruleReply':'Exact brain core/delegate, never independently randomized'}
    for k,n in s['placeholders'].items():lines.append(f'| {k} | {n} | {owners[k]} |')
    lines+=['',
        'All three characters use the same runtime market/quantity/parser policies; this is verified shared code, not a Father price restriction. Prices are positive Int32 inputs when semantically valid, with **no999 ceiling**. Initial requests normally use280/560/1400/2800g. The game formats these as1Seer/2Seers/1Veesai/2Veesai, with parenthesized metric UI gloss. Remaining-stock fallbacks and player quantity changes can produce other integer labels. Market values/events/wealth/personality determine offers; no authored list fixes a global economic price range. Arbitrary player-referenced amounts can be much larger than generated offers, including objection/clarification without agreement.',
        '',
        'Physical generated products: **pepper, cinnamon, cardamom**. Brain descriptors: black pepper, cinnamon bark, green cardamom. Clove remains in the legacy catalog/parser but is excluded by the physical generator whitelist; turmeric is a player-side token, not a generated trade. Do not record them as required current products.',
        '',
        '| Output unit forms | Game conversion / accepted aliases |','|---|---|',
        '| Palam / Palams |35g; palam(s)|','| Seer / Seers |280g; seer(s)|','| Veesai |1400g; veesai/viss|',
        '| Manangu / Manangus |11200g; manangu(s)/maund(s)|','| Bahar / Bahars |448000g; bahar(s)/candy/candies|',
        '| Other input |g/gm/gram(s), kg/kgs/kilogram(s) and bag(s) reformat to the traditional label; a bag is the current lot (default280g), not a new universal unit|',
        '',
        'These are game conventions, not verified historical metrology. Formatter thresholds are403200g Bahar,10080g Manangu,1260g Veesai,224g Seer, otherwise Palam; amounts are rounded. The displayed approximate label can differ from stored grams. Preserve the committed label and accepted snapshot; do not recompute an independent spoken quantity or claim arbitrary fractions are supported. Positive integer quantity extraction/conversion has no practical business cap; Int32 overflow/unchecked multiplications are implementation limits, not support guarantees for enormous trades.',
        '',
        'Existing ChatManager speech sanitization removes parenthesized/bracketed UI gloss. The inventory provides exact raw rendered examples AND sanitized TTS examples; numerical spelling-out in a future generator must not change subtitle/game numbers. Source currency is always varahas, so1varahas remains a real presentation-grammar issue. Approve singular varaha and matching subtitle policy; never change an amount to avoid the problem.',
        '',
        'Reusable vocabulary estimate:29 components for1–999 (one–nineteen, eight tens, hundred, and),3 products,2 currency forms,9 traditional unit forms =43 core recordings **per character voice**. Add zero/thousand/million/billion and3 descriptors ->50 extended pieces. `this lot`, contextual currency tails, names and boundary variants are additional where needed; estimates are not exact final queues.234 must compose two+hundred+and+thirty+four, never digit labels or Father audio. Unit amounts and referenced prices may exceed999. Unknown/unsupported labels require an explicit whole-utterance fallback, never missing words or unrelated audio.',
        '',
        f'Evidence: {ref(CORE/"MarketManager.cs","public string FormatTraditionalQuantity(")}, {ref(CORE/"NegotiationStateManager.cs","private static int ConvertToGrams(")}, {ref(CORE/"DialogueTableResponseProvider.cs","private static string ReplacePlaceholders(")}, {ref(CORE/"Level1GameState.cs","public bool BindAcceptedTerms(")}.']
    return '\n'.join(lines)+'\n'

def fallback_md():
    return '''## Optional, defensive and excluded source coverage

The inventory includes80 distinct brain utterances (82 occurrences) and11 presentation fallback sources plus3 provider defaults. These are shared C# source templates, **not interchangeable NPC recordings**. Most brain greetings/counters/social/role answers are replaced by the character table, so their coverage class is FALLBACK_ONLY. Fourteen source strings form a conservative clarification envelope:11 GetClarificationLine branches plus null-input, missing-ultimatum-price and missing-price guards. Nine normal nested-recovery branches are labelled REACHABLE_OPTIONAL; empty transcript, fulfillment-state and impossible/null-input cases remain defensive. Blank voice input normally recovers in voice UI before negotiation.

GetClarificationLine checks unresolved terms, fulfillment, empty/unrecognized speech, multiple prices, historical reference, quantity state, seller-price state, accept/counter state, repeat/confused, then default. Thus a repeat request can receive a state-specific prompt first. All four character wrappers must retain the exact core; Unknown is a pure ruleReply delegate, not a recordable word. Nested dynamic fields include first/last referenced prices, quantities and descriptors. Agreement source words use accepted terms. Do not claim static-wrapper coverage alone covers all recoveries.

All43 generic registry lines are listed but classified UNREACHABLE_OR_DEFERRED for known authored characters because the provider returns early. The hard-REJECT raise line at RuleBasedNPCBrain:211 is preempted by the unconditional hard-REJECT return at200; the hard ternary at213 is likewise preempted. Duplicate Agreed/Then-we-shall-not-trade source occurrences share exact words but retain additional locations in notes. TimePressure has no active route; defer all its authored variants.

Two ChatManager “The market is too noisy, could you repeat that?” assignments are UI-only at those call sites. Voice recovery “Sorry, I didn’t catch that. Try again.” and browsing/approaching status are not NPC speech recordings. No arbitrary backend/LLM prose, debug speech or other character table is counted as a current finite inventory requirement. The inventory retains exceptional/masked sources explicitly instead of confusing them with normal essential lines.
'''

def studio_md(c,s):
    t=s['studio'];problems={k:t[k] for k in ['inventory_duplicate_ids','manifest_duplicate_ids','missing_templates','changed_text','restriction_mismatches','missing_recording_ids','wrong_speaker_ids','assembly_reconstruction_mismatches','fixed_mapping_mismatches','dynamic_assembly_missing','raw_source_hash_drift_ids']}
    return f'''## Read-only voice-studio cross-check

Current studio inventory rows for this character: **{t['inventory_rows']}**. Manifest recording rows: **{t['manifest_rows']}**; modes `{json.dumps(t['manifest_modes'])}`; provisional priorities `{json.dumps(t['manifest_priorities'])}`; statuses `{json.dumps(t['manifest_statuses'])}`. These are recordings/fragments, not authored template counts or reviewed audio approvals. Studio voice is **Indic Parler / {c['voice']}**; no voice/profile changed.

Text/ID/restriction/mapping check results:

```json
{json.dumps(problems,indent=2,ensure_ascii=False)}
```

Exact authored texts were compared by ID against inventory.json. Duplicate template and recording IDs were checked. Dynamic assembly operations were reconstructed in order against current templates with the studio's explicit currency binding (`{{currency}}` -> varahas); every referenced recording ID and character was checked. Fixed mappings were checked against current original text. This validates textual mapping structure, not pronunciation or actual WAV spoken words. VALIDATED pilot paths were checked for existence only; no audio was played, synthesized or approved.

Outdated studio prose still says “No Level1 prerecorded/assembly adapter exists.” A shared adapter now exists for Father; this character still lacks an imported bank. “Only four quantities” is also inadequate for arbitrary current quantity changes. `{c['file']}` is the authoritative current source, not a historical character implied by its filename. Existing manifest ESSENTIAL/DEFERRED flags do not control Unity random selection; deferred variations remain reachable. No priorities or studio files were edited. New review decisions must not be inferred from the historical726 target.

Manifest-validated paths (not newly approved):

```json
{json.dumps(t['validated_manifest_rows'],indent=2,ensure_ascii=False)}
```
'''

def write_inventory(c,rows,s):
    folder=OUT/c['folder'];folder.mkdir(exist_ok=True)
    payload=dict(character=c,scope='Current-source static discovery; no Unity/game/audio execution.',editor_forced_character_id=forced,source_hashes=before,studio_source_hashes=studio_hashes,
        coverage_classes=CLASS_EVIDENCE,summary=s,vocabulary=baseline['vocabulary'],records=rows,test_selection_ids=[r['inventory_id'] for r in samples(c,rows)])
    (folder/'DIALOGUE_INVENTORY.json').write_text(json.dumps(payload,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    keys=list(rows[0]);assert all(set(r)==set(keys) for r in rows)
    with (folder/'DIALOGUE_INVENTORY.csv').open('w',encoding='utf-8-sig',newline='') as h:
        w=csv.DictWriter(h,fieldnames=keys);w.writeheader()
        for r in rows:w.writerow({k:json.dumps(v,ensure_ascii=False) if isinstance(v,(dict,list)) else v for k,v in r.items()})
    md=[f'# {c["name"]} — exact current source script','',
        'Verbatim Unity strings, not proposed rewrites. CORE means functional negotiation coverage, not a frequency statistic. Editor currently forces Father; classifications describe routes when this character is selected. Source/export IDs are ordinal aliases, not serialized runtime IDs. Shared AUD IDs are documentation-only. Examples use one illustrative state and do not form a guaranteed conversation.','']
    for cat in dict.fromkeys(r['category'] for r in rows):
        md+=['## '+cat,'']
        for ph in sorted({r['phase'] for r in rows if r['category']==cat}):
            md+=['### '+ph,'']
            for r in [x for x in rows if x['category']==cat and x['phase']==ph]:
                md += ['#### '+r['inventory_id']+' — '+r['scenario'],'',r['exact_template'],'',
                    '- Classification: **'+r['coverage_class']+'** / '+r['reachability'],
                    '- Source: `'+r['source_file']+':'+str(r['source_line'])+'`',
                    '- Trigger/evidence: '+r['coverage_evidence'],'- Restrictions: '+json.dumps(r['restrictions']),
                    '- Placeholders: '+(', '.join(r['placeholders']) or 'none')+'; type: '+r['audio_type']]
                if r['placeholders']:md+=['- Raw Unity example: '+r['example_rendered'],'- Existing TTS input example: '+r['example_tts_input'],'- Exact assembly pattern: '+json.dumps(r['assembly_pattern'],ensure_ascii=False)]
                md+=['- Note: '+r['notes'],'']
    (folder/'EXACT_LINES.md').write_text('\n'.join(md),encoding='utf-8')

def audit_md(c,rows,s):
    a=[r for r in rows if r['category']=='AUTHORED'];groups=ex.parse_character_file(c['name'],c['cid'],c['file'],c['prefix'])
    restrictions=[dict(scenario=e['scenario'],templates=len(e['templates']),mood=e['mood_bucket'],patience=e['patience_bucket']) for e in groups if e['mood_bucket']!='Any' or e['patience_bucket']!='Any']
    portions=[f'# {c["name"]} — Level1 dialogue discovery','',
        '27October2026. Documentation only. No source/scene/audio/studio edits, Unity launch, Unity tests, synthesis, commits or pushes. Every original string is in EXACT_LINES.md and CSV/JSON, including optional/defensive/excluded sources. Language proposals are separate.','',
        f'## Identity and exact counts\n\nRegistry/profile: **{c["name"]}** / `{c["cid"]}`; role **{c["role"]}**; base personality **{c["base"]}**. Authoritative file `{c["file"]}`. Scene model reference is nonzero and resolves to a local imported-model asset meta GUID. Effective runtime personality can differ with respect; see selection below. No new biographical details are inferred.','',
        f'**{s["authored"]} authored templates, {s["unique_authored"]} distinct strings, {len(groups)} groups, {len(s["scenarios"])} scenarios.** All authored: {s["authored_fixed"]} fixed / {s["authored_dynamic"]} dynamic. Excluding TimePressure: {s["routed"]} provider-addressable ({s["routed_fixed"]} fixed / {s["routed_dynamic"]} dynamic). Unknown remains conditional/residual. Class counts (authored only): `{json.dumps(s["authored_classes"])}`. No exact authored duplicates: `{json.dumps(s["authored_duplicate_texts"])}`.','',
        '| Scenario | Templates | Trigger |','|---|---:|---|']
    for sc,n in s['scenarios'].items():portions.append('| '+sc+' | '+str(n)+' | '+next(r['trigger'] for r in a if r['scenario']==sc).replace('|','\\|')+' |')
    portions+=['','Restricted groups (unlisted groups are unrestricted):','```json',json.dumps(restrictions,indent=2),'```','',common_runtime(c),dynamic_md(c,s),fallback_md(),studio_md(c,s),
        '## Voice-production scope — estimates, not an approved batch','',
        f'Whole fixed authored clips: **{s["routed_fixed"]}** routed lines. Dynamic/nested templates: **{s["routed_dynamic"]}**. Conservative authored+clarification pool: **{s["character_piece_estimate"]}** exact trimmed static/full text pieces. With50 reusable vocabulary pieces (deduplicated overlap), approximately **{s["per_voice_estimate"]}** takes; broader nonpreempted defensive catalog approximately **{s["defensive_estimate"]}**. These are source-segmentation estimates: boundary/inflection/emotion variants can change them. They do not claim all conservative guard strings are normally heard or that every fragmented line will sound natural.','',
        'Prefer complete fixed sentences. Use dynamic voice-specific vocabulary only for genuinely dynamic values and preserve the exact selected template/core. Currency-in-phrase singular variants and safe traditional-quantity labels must be mapped explicitly. Do not reuse Father/another NPC audio, silently generalize a product to spices, omit quantities, or count template appearances as unique recordings. Unrecorded random alternatives need an explicit honest fallback; a sheet priority alone cannot make them unreachable. Source/role corrections, pronunciation and production scope require review before generation.','',sample_md(c,rows),
        '## Static verification and remaining uncertainty','',
        'The documentation builder checks every source string at its exact current line, lossless literal/value decomposition, resolved examples, unique inventory IDs, current studio text/restrictions/assembly references and source hashes before/after. It never calls gameplay code. No claim is made that Unity tests passed or every route has run on device. Large-number/fraction handling, regional pronunciation, actual recording contents and natural delivery remain their own testing/listening gates. Current Editor Father override must be changed manually for later Indian-character runtime tests.']
    return '\n'.join(portions)+'\n'

def main():
    summaries=[]
    for c in CONFIGS:
        rows=create_records(c);sc=studio_check(c,rows);s=summarize(rows,sc);summaries.append((c,s))
        write_inventory(c,rows,s)
        if c['prefix']!='LAK':(OUT/c['folder']/'AUDIT.md').write_text(audit_md(c,rows,s),encoding='utf-8')
        else:
            p=OUT/c['folder']/'AUDIT.md';old=p.read_text(encoding='utf-8');old=old.split('\n<!-- FOUR_INDIAN_VERIFICATION -->')[0]
            old+='\n<!-- FOUR_INDIAN_VERIFICATION -->\n\n## Current-source verification and added classification\n\n27October2026: all seven original audit artifacts were read. All206 exact authored texts and all80 shared brain strings remain current; all hashed script sources match the original snapshot. The saved scene hash changed (Father testing/audibility configuration), with `debugForceCharacterId=father_penteado` currently active. The earlier audit describes ordinary selection eligibility, not the current forced Editor selection. No original authored audit was repeated or rewritten.\n\nCanonical `EXACT_LINES.md`, `DIALOGUE_INVENTORY.csv` and `.json` now add the requested four coverage classes, current hashes and explicit studio mapping checks. Original prefixed inventories and COUNTS.json remain as historical audit artifacts. Their scene hash is intentionally not silently refreshed. See the master summary for source consistency/limitations.\n\n'+sample_md(c,rows)
            p.write_text(old,encoding='utf-8')
        print(c['name'],json.dumps({k:s[k] for k in ['authored','authored_fixed','authored_dynamic','routed','authored_classes','per_voice_estimate','defensive_estimate']},ensure_ascii=False))
    (OUT/'FOUR_INDIAN_AUDIT_COUNTS.json').write_text(json.dumps({c['cid']:s for c,s in summaries},indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    assert all(sha(ROOT/p)==h for p,h in before.items()),'Inspected Unity source changed during documentation generation'
    assert all(sha(Path(p))==h for p,h in studio_hashes.items()),'Voice-studio input changed'
    print('PASS: source text/line checks, all exact decomposition/examples, all Unity hashes unchanged. No Unity tests/game/audio executed.')

if __name__=='__main__':main()
