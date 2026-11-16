"""Read C# source text only; write audit documents beside this file. No Unity/game execution."""
import collections
import csv
import hashlib
import importlib.util
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
OUT = Path(__file__).resolve().parent
U = ROOT / 'unity/StorytellingVR'
CORE = U / 'Assets/_Project/Scripts/Level1/Core'
STUDIO = ROOT.parent / 'NPC-Voice-Generator'
SPEC = importlib.util.spec_from_file_location('source_text_extractor', OUT.parent / 'extract_level1_dialogues.py')
extract = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(extract)  # main is guarded; only the source-text parser is reused.

TRIGGERS = {
 'CustomerGreeting': 'ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.',
 'RepeatCustomerGreeting': 'GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.',
 'AskWhatBuyerWants': 'ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.',
 'AskSellerPrice': 'ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.',
 'AskBuyerBudget': 'QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.',
 'AskQuantity': 'QUANTITY_QUERY; answers committed trade quantity.',
 'QuantityChanged': 'QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.',
 'SellerPriceTooHigh': 'Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).',
 'SellerPriceSlightlyHigh': 'No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).',
 'SellerPriceBelowExpected': 'Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.',
 'BuyerCounterFirst': 'Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.',
 'BuyerCounterMiddle': 'Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.',
 'BuyerCounterFinal': 'Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.',
 'BuyerHoldsFirm': 'No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.',
 'SellerPriceAccepted': 'result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.',
 'PlayerAcceptedDeal': 'Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.',
 'PlayerRejectedBuyerOffer': 'REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.',
 'OfferReason': 'PRICE_QUERY with asksReason=true and asksItem=false. Lakshmi QUERY_BUYER_BUDGET+asksReason deliberately stays AskBuyerBudget.',
 'AskName': 'SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.',
 'AskOrigin': 'SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.',
 'WeatherQuestion': 'SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.',
 'HistoryKing': 'SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.',
 'HistoryTemple': 'SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.',
 'HistoryQuestion': 'SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.',
 'SocialGreeting': 'Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.',
 'OffTopic': 'Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.',
 'HostileWarning': 'Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.',
 'UnclearSpeech': 'result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.',
 'Unknown': 'MapScenario residual outcome; e.g. nonfinal ACCEPT suspicious-price guard without a price on the input. Not equivalent to input UNKNOWN (which maps UnclearSpeech).',
 'TransactionSuccess': 'GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.',
 'TransactionFailure': 'walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.',
 'TimePressure': 'No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.',
 'IdleReminderFirst': 'Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.',
 'IdleReminderSecond': 'Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.',
}

def phase(s):
    if s in {'CustomerGreeting','RepeatCustomerGreeting'}: return '01 Arrival and repeat visit'
    if s.startswith('Ask') or s in {'QuantityChanged','OfferReason'}: return '02 Trade information and quantity'
    if s in {'SellerPriceAccepted','PlayerAcceptedDeal'}: return '04 Price agreement, goods pending'
    if s.startswith('Buyer') or s.startswith('Seller') or s in {'PlayerRejectedBuyerOffer','COUNTER_OR_HOLD'}: return '03 Negotiation and rejection'
    if s in {'UnclearSpeech','Unknown'}: return '06 Clarification and precise continuation'
    if s in {'TransactionSuccess','TransactionFailure','TimePressure','IdleReminderFirst','IdleReminderSecond'}: return '07 Waiting, completion and departure'
    return '05 Social, history and conduct'

def digest(s): return hashlib.sha256(s.encode('utf-8')).hexdigest()
def rel(p): return str(p.relative_to(ROOT)).replace('\\','/')
def literals(s):
    for m in re.finditer(r'\$?"((?:\\.|[^"\\])*)"',s):
        yield m.start(),m.group(1).replace('\\"','"').replace('\\n','\n').replace('\\\\','\\')

VALUES = dict(quantityLabel='2 Seers (~560g)',quantity='2 Seers (~560g)',brainQuantityLabel='2 Seers (~560g)',
    spiceName='pepper',spice='pepper',spiceDescriptor='black pepper',offeredPrice='300',counterPrice='234',
    finalPrice='234',currentBuyerOffer='234',currentOffer='234',sellerPrice='300',counterOffer='234',nextOffer='234',
    updatedOffer='234',minimumPrice='280',buyerName='Lakshmamma',characterName='Lakshmamma',
    buyerOrigin='Local Household Buyer',currency='varahas',round='3',reputation='50',patience='2',desperation='0.50',
    ruleReply='The market is busy and your words were unclear. Say it again plainly.')
VALUES.update({'result.updatedOffer':'234','result.acceptedTerms.QuantityLabel':'2 Seers (~560g)',
 'result.acceptedTerms.SpiceName':'Pepper','result.acceptedTerms.Price':'234',
 'input.referencedPrices[0]':'250','input.referencedPrices[input.referencedPrices.Count - 1]':'300'})

def example(text): return re.sub(r'\{([^{}]+)\}',lambda m:VALUES.get(m[1],m[0]),text)
def add(records,category,text,path,line,scenario,trigger,reach,identifier=None,restrictions=None,notes=''):
    fields=list(dict.fromkeys(re.findall(r'\{([^{}]+)\}',text)))
    sequence=[]
    for part in re.split(r'(\{[^{}]+\})',text):
        if part:sequence.append({'kind':'VALUE' if part.startswith('{') and part.endswith('}') else 'LITERAL','text':part})
    rendered=example(text)
    records.append(dict(inventory_id=identifier or 'AUD_LAK_'+digest(category+'|'+rel(path)+'|'+str(line)+'|'+text)[:14].upper(),
        character_id='lakshmi_amma',speaker='Lakshmamma',category=category,phase=phase(scenario),scenario=scenario,
        exact_template=text,placeholders=fields,audio_type='NESTED_DYNAMIC' if 'ruleReply' in fields else 'DYNAMIC_TEMPLATE' if fields else 'FULL_CLIP',
        restrictions=restrictions or {},reachability=reach,trigger=trigger,source_file=rel(path),source_line=line,
        template_sha256=digest(text),example_rendered=rendered,
        example_tts_input=re.sub(r'\s+',' ',re.sub(r'\([^)]*\)|\[[^]]*\]','',rendered)).strip(),
        assembly_pattern=sequence,notes=notes))

def main():
    records=[];entries=extract.parse_character_file('Lakshmamma','lakshmi_amma','LakshmiAmmaDialogue.cs','LAK')
    table=CORE/'Dialogues/Characters/LakshmiAmmaDialogue.cs';seq=0
    for e in entries:
        restrictions={k:e[k] for k in ['mood_bucket','patience_bucket','reputation_bucket','desperation_bucket','round_bucket']}
        for text,line in e['templates']:
            seq+=1;s=e['scenario']
            add(records,'AUTHORED',text,table,line,s,TRIGGERS[s],
                'UNROUTED' if s=='TimePressure' else 'CONDITIONAL_RESIDUAL' if s=='Unknown' else 'CONDITIONAL_QUERY_INPUT' if s=='OfferReason' else 'ROUTED',
                'L1_LAK_'+str(seq).zfill(3),restrictions,
                'Audit/export ordinal ID, not a serialized runtime dialogue ID. Runtime retains selected source template/scenario. Frequency is not measured.')
    brain=CORE/'RuleBasedNPCBrain.cs';s=brain.read_text(encoding='utf-8-sig');seen={}
    conditions={
      47:'AcceptedTerms already bound; brain returns finished ACCEPT; ordinarily authored agreement replaces this.',
      88:'Null trade or input; result.requiresClarification=true.',122:'Explicit DISMISS.',
      135:'ITEM_QUERY with asksCurrentOffer.',140:'ITEM_QUERY before price introduced.',156:'QUERY_BUYER_BUDGET.',
      172:'OFF_TOPIC reaches outOfWorldCount>=4.',183:'HOSTILE count>=4, or count>=3 with frustration>=0.75.',190:'CONTINUE.',
      200:'Explicit hard REJECT; early terminal return before counter computation.',
      211:'REJECT raises offer and hasHardRejection.',212:'REJECT raises offer without hard rejection.',
      213:'REJECT does not raise offer; hard -> too far apart, soft -> name a better price.',
      228:'ACCEPT hasSellerPrice and lastSellerPrice <0.3*targetMarketValue; nonfinal refusal.',
      236:'Validated ACCEPT binds input.acceptedOffer snapshot.',246:'QUANTITY_CHANGE smaller than old quantity.',247:'Other QUANTITY_CHANGE.',
      255:'ULTIMATUM without seller price; requiresClarification=true.',267:'Ultimatum above max and ShouldWalkAwayFromHighPrice.',
      272:'Ultimatum above max, still negotiable.',279:'Ultimatum ShouldHoldPosition.',285:'Other valid ultimatum.',
      298:'COUNTER without explicit price.',308:'PRICE/QUANTITY_PRICE missing valid price; requiresClarification=true.',
      326:'Explicit price per gram <35% target market value per gram, lowPriceCount>=3.',
      328:'Explicit price below honest trade floor; nonterminal.',340:'Explicit price above upper walk-away range and pressure exhausted.',
      353:'Seller price exactly equals current offer, after suspicious-price guards.',368:'Price below current offer and CanAcceptNow plus valid accepted terms.',
      377:'Price below current offer but acceptance gate not satisfied (no acceptance).',388:'Within acceptance threshold and near-accept gap, with valid terms.',
      406:'Repeated identical over-max price; early refusal.',413:'Above-max seller price with buyer near ceiling.',
      433:'BARGAIN fails to raise offer.',440:'PRICE_QUERY asksItem.',444:'PRICE_QUERY asksReason but not asksItem.',448:'Other PRICE_QUERY.',
      459:'Brain default intent path.',732:'clarificationKind=UnresolvedTerms; first clarification priority.',
      735:'parseReason=FulfillmentExpected, after UnresolvedTerms check.',740:'parseReason=EmptyTranscript.',745:'parseReason=UnrecognizedSpeech.',
      750:'MultipleActionablePrices and >=2 referenced prices.',755:'HistoricalPriceOnly and >=1 referenced price.',
      760:'MissingQuantity or ExpectQuantity.',765:'StateBlockedAccept/MissingPrice or ExpectOfferPrice.',
      770:'AmbiguousAcceptOrCounter or ExpectAcceptOrCounter.',775:'CONFUSED/ConfusedPlayer only if earlier state branches did not match.',
      779:'Default clarification branch.',956:'GetGreetingLine Strict.',960:'GetGreetingLine Impatient.',962:'Other greeting personality.',
      970:'Repeated ITEM_QUERY helper.',974:'First ITEM_QUERY helper.',982:'Repeated QUANTITY_QUERY.',986:'First QUANTITY_QUERY.',
      994:'Weather/social helper Strict.',995:'Weather/social helper other personality.',1000:'Origin helper Friendly.',1001:'Origin helper not Friendly.',
      1005:'Name helper.',1009:'King/temple helper.',1011:'Other social helper.',
      1018:'OFF_TOPIC first count.',1022:'OFF_TOPIC second count.',1024:'OFF_TOPIC third count.',
      1031:'HOSTILE first count.',1035:'HOSTILE second count.',1037:'HOSTILE third count only if frustration remains <0.75; earlier walk-away gates otherwise apply.',
      1044:'GetCounterLine sellerPrice>max.',1048:'Counter Strict.',1052:'Counter Friendly.',1056:'Counter Impatient.',1058:'Counter Normal.',
      1065:'BARGAIN repeatedBargains>1.',1069:'BARGAIN Strict.',1073:'BARGAIN Friendly.',1075:'Other BARGAIN.',
      1082:'Pressure walk-away Impatient.',1086:'Pressure walk-away Strict (same wording also ultimatum).',1088:'Other pressure walk-away.'}
    brain_scenarios={47:'SellerPriceAccepted',122:'TransactionFailure',135:'AskBuyerBudget',140:'AskSellerPrice',156:'AskBuyerBudget',
        172:'TransactionFailure',183:'TransactionFailure',190:'AskBuyerBudget',200:'TransactionFailure',211:'COUNTER_OR_HOLD',212:'COUNTER_OR_HOLD',
        213:'PlayerRejectedBuyerOffer',228:'Unknown',236:'PlayerAcceptedDeal',246:'QuantityChanged',247:'QuantityChanged',267:'TransactionFailure',
        272:'COUNTER_OR_HOLD',279:'BuyerHoldsFirm',285:'COUNTER_OR_HOLD',298:'COUNTER_OR_HOLD',326:'TransactionFailure',328:'SellerPriceBelowExpected',
        340:'TransactionFailure',353:'SellerPriceAccepted',368:'SellerPriceAccepted',377:'SellerPriceBelowExpected',388:'SellerPriceAccepted',
        406:'COUNTER_OR_HOLD',413:'COUNTER_OR_HOLD',433:'BuyerHoldsFirm',440:'AskBuyerBudget',444:'OfferReason',448:'AskBuyerBudget',
        459:'UnclearSpeech',956:'SocialGreeting',960:'SocialGreeting',962:'SocialGreeting',970:'AskWhatBuyerWants',974:'AskWhatBuyerWants',
        982:'AskQuantity',986:'AskQuantity',994:'WeatherQuestion',995:'WeatherQuestion',1000:'AskOrigin',1001:'AskOrigin',1005:'AskName',
        1009:'HistoryQuestion',1011:'SocialGreeting',1018:'OffTopic',1022:'OffTopic',1024:'OffTopic',1031:'HostileWarning',1035:'HostileWarning',
        1037:'HostileWarning',1044:'COUNTER_OR_HOLD',1048:'COUNTER_OR_HOLD',1052:'COUNTER_OR_HOLD',1056:'COUNTER_OR_HOLD',1058:'COUNTER_OR_HOLD',
        1065:'COUNTER_OR_HOLD',1069:'COUNTER_OR_HOLD',1073:'COUNTER_OR_HOLD',1075:'COUNTER_OR_HOLD',1082:'TransactionFailure',1086:'TransactionFailure',1088:'TransactionFailure'}
    for pos,text in literals(s):
        if ' ' not in text or not re.search(r'[.!?]$',text) or text.startswith('['):continue
        line=s.count('\n',0,pos)+1
        assert line in conditions,(line,text)
        if text in seen:
            seen[text]['notes']+=' Additional source '+rel(brain)+':'+str(line)+'; '+conditions[line]
            continue
        core=(727<=line<=779) or line in {88,255,308}
        reach='PREEMPTED_BRANCH' if line==211 else 'NESTED_REACHABLE' if core else 'DEFENSIVE_USUALLY_MASKED'
        add(records,'BRAIN_FALLBACK',text,brain,line,'UnclearSpeech' if core else brain_scenarios[line],conditions[line],reach,
            notes='Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.')
        seen[text]=records[-1]
    # Shared generic rows are real fallback assets, but the authored-character early return bypasses them.
    registry=CORE/'Dialogues/DialogueCharacterRegistry.cs';s=registry.read_text(encoding='utf-8-sig');start=s.index('private static List<DialogueLine> CreateGenericLines')
    for m in re.finditer(r'new DialogueLine\(DialogueScenario\.(\w+), new\[\]\s*\{(.*?)\}\s*\)',s[start:],re.S):
        for p,text in literals(m[2]):
            pos=start+m.start(2)+p
            add(records,'GENERIC_BYPASSED',text,registry,s.count('\n',0,pos)+1,m[1],
                'FindTemplate returns FindRestrictedTemplate directly for known authored lakshmi_amma; generic lookup is bypassed.',
                'NOT_ROUTED_FOR_LAKSHMAMMA')
    generator=CORE/'LocalTradeSessionGenerator.cs';s=generator.read_text(encoding='utf-8-sig')
    for pos,text in literals(s):
        if text.startswith(('Greetings, merchant.','Good day. I am','Merchant, I am')):
            add(records,'PRESENTATION_FALLBACK',text,generator,s.count('\n',0,pos)+1,'CustomerGreeting',
                'ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.',
                'DEFENSIVE_USUALLY_MASKED')
    market=U/'Assets/_Project/Scripts/Level1/MarketplaceManager.cs';s=market.read_text(encoding='utf-8-sig')
    for arr,scenario in [('FirstReminderLines','IdleReminderFirst'),('SecondReminderLines','IdleReminderSecond'),('FinalReminderLines','TransactionFailure')]:
        m=re.search(arr+r'\s*=\s*\{(.*?)\}',s,re.S)
        for pos,text in literals(m[1]):
            add(records,'PRESENTATION_FALLBACK',text,market,s.count('\n',0,m.start(1)+pos)+1,scenario,
                'Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.',
                'DEFENSIVE_USUALLY_MASKED')
    provider=CORE/'DialogueTableResponseProvider.cs';s=provider.read_text(encoding='utf-8-sig')
    for text,scenario,trigger in [('Thank you, merchant.','TransactionSuccess','GetPresentation fallback after settlement, if no authored match or exception.'),
       ('Speak plainly, merchant.','UnclearSpeech','Blank brain.replyText fallback for context.ruleReply and ReplacePlaceholders.'),
       ('Greetings, merchant.','CustomerGreeting','Greeting context.ruleReply only; greeting templates do not include ruleReply.')]:
        pos=s.index('"'+text+'"');add(records,'PROVIDER_FALLBACK',text,provider,s.count('\n',0,pos)+1,scenario,trigger,
            'CONTEXT_ONLY' if text=='Greetings, merchant.' else 'DEFENSIVE_FALLBACK')
    chat=U/'Assets/_Project/Scripts/Level1/ChatManager.cs';s=chat.read_text(encoding='utf-8-sig');text='My patience is spent. I will take my leave.';pos=s.index('"'+text+'"')
    add(records,'PRESENTATION_FALLBACK',text,chat,s.count('\n',0,pos)+1,'TransactionFailure',
        'TryHandleNegotiationTimeout default finalLine; authored terminal GetIdleReminder replaces it.', 'DEFENSIVE_USUALLY_MASKED')
    text='The market is too noisy, could you repeat that?'
    for pos,found in literals(s):
        if found==text:add(records,'UI_ONLY_RECOVERY',found,chat,s.count('\n',0,pos)+1,'UnclearSpeech',
            'Null trade in local turn or error in legacy OnNPCReply; assigned npcText only, not sent to PresentNpcSubtitleAndTts.', 'TEXT_ONLY_NOT_SPOKEN')
    assert len([r for r in records if r['category']=='AUTHORED'])==206
    assert len(seen)==80,len(seen)
    assert len(set(r['inventory_id'] for r in records))==len(records), 'duplicate audit IDs'
    # Source-line round trip establishes these are exact source strings, not generated paraphrases.
    for r in records:
        source=(ROOT/r['source_file']).read_text(encoding='utf-8-sig').splitlines()
        assert r['exact_template'].replace('"','\\"') in source[r['source_line']-1],r['inventory_id']
        assert not re.search(r'\{[^{}]+\}',r['example_rendered']),r['inventory_id']
    def chunks(rows):
        return set(p.strip() for r in rows for p in re.split(r'\{[^{}]+\}',r['exact_template']) if re.search(r'\w',p))
    authored=[r for r in records if r['category']=='AUTHORED'];routed=[r for r in authored if r['reachability']!='UNROUTED']
    full=[r for r in routed if not r['placeholders']];dyn=[r for r in routed if r['placeholders']];nested=[r for r in dyn if 'ruleReply' in r['placeholders']]
    fixedpieces=chunks(dyn);fulltexts={r['exact_template'] for r in full}
    core=[r for r in records if r['reachability']=='NESTED_REACHABLE'];corefull={r['exact_template'] for r in core if not r['placeholders']};corepieces=chunks([r for r in core if r['placeholders']])
    numwords=set('one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen twenty thirty forty fifty sixty seventy eighty ninety hundred and'.split())
    vocabulary=sorted(numwords)+['pepper','cinnamon','cardamom','varaha','varahas','Palam','Palams','Seer','Seers','Veesai','Manangu','Manangus','Bahar','Bahars']
    vocab_extended=vocabulary+['zero','thousand','million','billion','black pepper','cinnamon bark','green cardamom']
    phrasebase=fulltexts|fixedpieces;phrasecore=phrasebase|corefull|corepieces
    manifest=list(csv.DictReader((STUDIO/'production/production_manifest.csv').open(encoding='utf-8-sig',newline='')))
    old=[r for r in manifest if r['speaker']=='Lakshmamma'];oldtexts={r['original_dialogue_text'] for r in old}
    documented=[r for r in records if r['category'] not in {'GENERIC_BYPASSED','UI_ONLY_RECOVERY'} and r['reachability'] not in {'UNROUTED','PREEMPTED_BRANCH','CONTEXT_ONLY'}]
    oldinventory=json.loads((STUDIO/'production/inventory.json').read_text(encoding='utf-8-sig'))
    oldlookup={r['source_dialogue_id']:r for r in oldinventory if r.get('character_id')=='lakshmi_amma' or r.get('speaker')=='Lakshmamma'}
    oldassemblies=json.loads((STUDIO/'production/dynamic_assemblies.json').read_text(encoding='utf-8-sig'))
    assemblylookup={r['source_dialogue_id']:r for r in oldassemblies if r['speaker']=='Lakshmamma'}
    missing_inventory=[r['inventory_id'] for r in authored if r['inventory_id'] not in oldlookup or oldlookup[r['inventory_id']]['original_dialogue_text']!=r['exact_template']]
    summary=dict(authored_entries=len(authored),authored_unique=len({r['exact_template'] for r in authored}),groups=len(entries),
        scenarios=len({r['scenario'] for r in authored}),routed_authored=len(routed),unrouted_authored=len(authored)-len(routed),
        routed_authored_fixed=len(full),routed_authored_dynamic=len(dyn),nested_authored=len(nested),
        authored_static_fragment_texts=len(fixedpieces),unique_character_recordings_before_vocab=len(phrasebase),
        direct_or_nested_brain_sources=len(core),brain_fixed=len(corefull),brain_static_chunks=len(corepieces),
        unique_character_recordings_including_clarification_cores=len(phrasecore),
        compact_vocabulary=len(set(vocabulary)),extended_vocabulary=len(set(vocab_extended)),
        estimated_routed_asset_count=len(phrasecore|set(vocab_extended)),
        estimated_with_defensive_source_catalog=len(chunks([r for r in documented if r['placeholders']])|{r['exact_template'] for r in documented if not r['placeholders']}|set(vocab_extended)),
        categories=dict(collections.Counter(r['category'] for r in records)),
        total_inventory_rows=len(records),total_unique_source_texts=len({r['exact_template'] for r in records}),
        per_scenario=dict(collections.Counter(r['scenario'] for r in authored)),
        placeholders=dict(collections.Counter(v for r in authored for v in r['placeholders'])),
        existing_manifest_rows=len(old),existing_manifest_priorities=dict(collections.Counter(r['priority'] for r in old)),
        existing_manifest_modes=dict(collections.Counter(r['generation_mode'] for r in old)),
        existing_manifest_statuses=dict(collections.Counter(r['generation_status'] for r in old)),
        existing_manifest_unique_templates=len(oldtexts),authored_templates_not_in_manifest_primary_text=sorted(set(r['exact_template'] for r in authored)-oldtexts),
        current_authored_missing_or_changed_in_studio_inventory=missing_inventory,
        authored_dynamic_assembly_entries=sum(r['inventory_id'] in assemblylookup for r in dyn),
        authored_duplicate_texts=[t for t,c in collections.Counter(r['exact_template'] for r in authored).items() if c>1])
    inputs={ROOT/r['source_file'] for r in records}|{CORE/'InputNormalizer.cs',CORE/'NegotiationStateManager.cs',CORE/'MarketManager.cs',CORE/'Level1GameState.cs',
        U/'Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity'}
    hashes={rel(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in inputs}
    payload=dict(scope='Read-only current-source audit; examples are illustrative, no Unity/game executed.',source_hashes=hashes,summary=summary,
        vocabulary=dict(core=vocabulary,extended=vocab_extended),records=records)
    (OUT/'LAKSHMAMMA_DIALOGUE_INVENTORY.json').write_text(json.dumps(payload,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    with (OUT/'LAKSHMAMMA_DIALOGUE_INVENTORY.csv').open('w',encoding='utf-8-sig',newline='') as f:
        writer=csv.DictWriter(f,fieldnames=list(records[0]));writer.writeheader()
        for r in records:writer.writerow({**r,'placeholders':'; '.join(r['placeholders']),'restrictions':json.dumps(r['restrictions'],ensure_ascii=False),'assembly_pattern':json.dumps(r['assembly_pattern'],ensure_ascii=False)})
    lines=['# Lakshmamma — exact source dialogue inventory','',
        'Generated by reading current C# source text only. See AUDIT.md for reachability, pronunciation and recording decisions.',
        'Exact source text is preserved below, including grammar defects and metric parentheticals in rendered examples. No suggested change is applied.',
        'L1_LAK IDs are existing export ordinals, not serialized runtime IDs. AUD_LAK IDs are documentation-only hashes. Prices/examples are illustrative, not new dialogue.',
        '']
    for category in dict.fromkeys(r['category'] for r in records):
        lines+=['## '+category,'']
        subset=[r for r in records if r['category']==category]
        for ph in sorted(set(r['phase'] for r in subset)):
            lines+=['### '+ph,'']
            for r in [x for x in subset if x['phase']==ph]:
                lines+=['#### '+r['inventory_id']+' — '+r['scenario'],'',r['exact_template'],'',
                    '- Source: `'+r['source_file']+':'+str(r['source_line'])+'`',
                    '- Trigger: '+r['trigger'],'- Reachability: '+r['reachability']+'; constraints: '+json.dumps(r['restrictions']),
                    '- Variables: '+(', '.join(r['placeholders']) or 'none')+'; type: '+r['audio_type']]
                if r['placeholders']:lines+=['- Example (raw Unity formatting): '+r['example_rendered'],
                    '- Example sent to existing TTS (before engine number pronunciation): '+r['example_tts_input'],
                    '- Assembly order: '+json.dumps(r['assembly_pattern'],ensure_ascii=False)]
                if r['notes']:lines+=['- Note: '+r['notes']]
                lines+=['']
    (OUT/'LAKSHMAMMA_EXACT_LINES.md').write_text('\n'.join(lines),encoding='utf-8')
    (OUT/'COUNTS.json').write_text(json.dumps(summary,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    print(json.dumps(summary,indent=2,ensure_ascii=False))
    assert all(hashlib.sha256((ROOT/p).read_bytes()).hexdigest()==h for p,h in hashes.items()),'Inspected source changed during audit'
    print('PASS: source-string/line round-trip, placeholder examples, unique inventory IDs, inspected Unity hashes unchanged.')

if __name__=='__main__':main()
