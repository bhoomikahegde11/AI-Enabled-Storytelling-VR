# Saraswati Chetti — Level1 dialogue discovery

27October2026. Documentation only. No source/scene/audio/studio edits, Unity launch, Unity tests, synthesis, commits or pushes. Every original string is in EXACT_LINES.md and CSV/JSON, including optional/defensive/excluded sources. Language proposals are separate.

## Identity and exact counts

Registry/profile: **Saraswati Chetti** / `saraswati_chetti`; role **Local Vijayanagara Retail Shop Owner**; base personality **Friendly**. Authoritative file `SiddharthChettiDialogue.cs`. Scene model reference is nonzero and resolves to a local imported-model asset meta GUID. Effective runtime personality can differ with respect; see selection below. No new biographical details are inferred.

**164 authored templates, 164 distinct strings, 43 groups, 34 scenarios.** All authored: 67 fixed / 97 dynamic. Excluding TimePressure: 161 provider-addressable (64 fixed / 97 dynamic). Unknown remains conditional/residual. Class counts (authored only): `{"CORE": 117, "REACHABLE_OPTIONAL": 44, "UNREACHABLE_OR_DEFERRED": 3}`. No exact authored duplicates: `[]`.

| Scenario | Templates | Trigger |
|---|---:|---|
| CustomerGreeting | 4 | ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice. |
| RepeatCustomerGreeting | 4 | GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead. |
| AskWhatBuyerWants | 4 | ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice. |
| AskBuyerBudget | 4 | QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE. |
| AskQuantity | 4 | QUANTITY_QUERY; answers committed trade quantity. |
| AskSellerPrice | 4 | ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs. |
| QuantityChanged | 4 | QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept. |
| SellerPriceTooHigh | 7 | Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3). |
| SellerPriceSlightlyHigh | 4 | No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2). |
| SellerPriceBelowExpected | 4 | Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority. |
| BuyerCounterFirst | 4 | Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here. |
| BuyerCounterMiddle | 7 | Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here. |
| BuyerCounterFinal | 4 | Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee. |
| BuyerHoldsFirm | 30 | No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget. |
| SellerPriceAccepted | 4 | result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending. |
| PlayerAcceptedDeal | 4 | Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending. |
| PlayerRejectedBuyerOffer | 4 | REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band. |
| OfferReason | 4 | PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma). |
| AskName | 4 | SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check. |
| AskOrigin | 4 | SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks. |
| WeatherQuestion | 3 | SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read. |
| HistoryKing | 3 | SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name. |
| HistoryTemple | 3 | SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king. |
| HistoryQuestion | 4 | SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks. |
| SocialGreeting | 7 | Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE. |
| OffTopic | 4 | Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure. |
| HostileWarning | 3 | Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure. |
| UnclearSpeech | 4 | result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}. |
| Unknown | 1 | MapScenario residual outcome; e.g. nonfinal ACCEPT suspicious-price guard without a price on the input. Not equivalent to input UNKNOWN (which maps UnclearSpeech). |
| TransactionSuccess | 4 | GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance. |
| TransactionFailure | 7 | walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout. |
| TimePressure | 3 | No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path. |
| IdleReminderFirst | 3 | Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts. |
| IdleReminderSecond | 3 | Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time. |

Restricted groups (unlisted groups are unrestricted):
```json
[
  {
    "scenario": "BuyerHoldsFirm",
    "templates": 5,
    "mood": "Friendly",
    "patience": "Any"
  },
  {
    "scenario": "BuyerHoldsFirm",
    "templates": 5,
    "mood": "Normal",
    "patience": "Any"
  },
  {
    "scenario": "BuyerHoldsFirm",
    "templates": 5,
    "mood": "Strict",
    "patience": "Any"
  },
  {
    "scenario": "BuyerHoldsFirm",
    "templates": 5,
    "mood": "Impatient",
    "patience": "Any"
  },
  {
    "scenario": "BuyerHoldsFirm",
    "templates": 5,
    "mood": "Any",
    "patience": "Low"
  },
  {
    "scenario": "SellerPriceTooHigh",
    "templates": 3,
    "mood": "Any",
    "patience": "Low"
  },
  {
    "scenario": "BuyerCounterMiddle",
    "templates": 3,
    "mood": "Any",
    "patience": "Low"
  },
  {
    "scenario": "SocialGreeting",
    "templates": 3,
    "mood": "Any",
    "patience": "Low"
  },
  {
    "scenario": "TransactionFailure",
    "templates": 3,
    "mood": "Any",
    "patience": "Low"
  }
]
```

## Actual current runtime and selection

The saved scene currently has **debugForceCharacterId=father_penteado**. Thus Saraswati Chetti is registered and has a configured model/profile, but the current Editor forced-selection branch chooses Father instead. Clear that existing field or explicitly force `saraswati_chetti` when testing later. This audit does not change it. The field/use are UNITY_EDITOR-only; normal builds use weighted registry selection. There is no per-profile enabled checkbox excluding this character. All six registry characters have positive weights; only immediate previous-character removal temporarily excludes a candidate in the normal random branch.

Trace: `MarketplaceManager.PrepareIncomingCustomerLifecycle` -> `Level1GameState.GenerateLocalSession` -> `LocalTradeSessionGenerator.Generate` -> configured/weighted character plus real terms -> `ChatManager.StartNewSession/GetGreeting`. Player input -> `InputNormalizer` -> `NegotiationStateManager.ClassifyInput/ProcessLocalTurn` -> `RuleBasedNPCBrain.GenerateReply` -> `LocalTradeState.ApplyDecision` -> `DialogueTableResponseProvider.GetReply/MapScenario/MapAuthoredScenario` -> exact selected template + substitution + `LastPresentation` -> `ChatManager.PresentNpcSubtitleAndTts` -> existing AudioManager. Expected reply state comes from the semantic decision, not from the selected prose. Terminal timeout/reminders and successful delivery have separate provider hooks.

Evidence: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:32`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/NegotiationStateManager.cs:2508`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:416`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:581`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:1142`. Per-line source references and trigger evidence are in the inventory, not inferred from old sheet descriptions.

Scene local intent/brain/session enabled; LLM understanding and generation disabled. Authored characters additionally skip LLM generation even if the local generation flag is enabled. Current prerecorded bank is Father only. Other characters retain their existing provider wiring; no `saraswati_chetti` prerecorded bank is registered. Studio assignment is Indic Parler / **Sneha**. This records an existing production decision, not an assertion that the model proves a native regional accent. Other characters' WAVs must never supply this character's number vocabulary.

`FindTemplate` returns `FindRestrictedTemplate` immediately for authored characters: its43 generic registry lines are bypassed, not normally heard emergency alternatives. A missing/exceptional character rendering preserves the selected brain string. All34 scenarios have an unrestricted character group. Restricted lines must satisfy every specified bucket; low patience<=2 outranks mood wording. HOLD alone blends eligible mood and generic pools (70% mood only when both unused pools are available); this is not an observed usage probability. The recent-template queue holds at most4, releasing older entries if a small bucket is exhausted. History resets on a new trade object, not after every utterance.

Personality options are Friendly/Normal/Strict/Impatient. Starting patience7/5/4/3 respectively. Respect changes effective personality/wealth and customer weights; no special new emotion FSM exists. None of this character's lines has an explicit reputation, desperation or round bucket restriction. Round still selects first<=1/middle2–4/final>=5 counter scenarios. Low-patience wording is selected after the decision's patience cost. Normal price/counter/bargain/reject/ultimatum pressure costs1; hostility2; OFF_TOPIC1. Clarification rolls back a counted round and has no extra patience cost. Basic queries/social/quantity-only changes do not count as bargaining rounds.

Acceptance/failure dominate other scenarios. A real upward offer chooses the counter round band; no increase chooses HOLD or the more specific price objection. Explicit PRICE and COUNTER share the economic path. `offeredPrice>maxBuyerPrice` with no raise uses SlightlyHigh only within max(6,minIncrement*3), otherwise TooHigh. Above the previous offer within max(5,minIncrement*2) without a raise also uses SlightlyHigh. Below previous offer without acceptance uses BelowExpected. CounterFinal is a round label, not a binding never-negotiate-again promise. Hard REJECT and DISMISS terminate; soft REJECT can raise or ask the seller's price.

ITEM_QUERY before price introduction maps AskSellerPrice; price/budget questions map AskBuyerBudget. CONTINUE restates budget. Committed QUANTITY_CHANGE maps QuantityChanged. For this character, QUERY_BUYER_BUDGET+asksReason maps OfferReason (Lakshmamma is the explicit exception). Weather/name/king/temple/origin/history are ordered keyword checks only within classified SOCIAL/GENERAL_DIALOGUE; combined questions follow that ordering. No arbitrary question is guaranteed a dedicated answer.

Successful price agreement uses frozen accepted item/quantity/price but does not confirm payment/delivery. TransactionSuccess requires real accepted fulfillment plus SettlementClaimed. Walk-away and idle NO DEAL use TransactionFailure. The idle path uses IdleReminderFirst, IdleReminderSecond then TransactionFailure; it never selects TimePressure. Source terminal customer identity is retained after ActiveTrade clears. Busy capture/recognition/review/NPC response does not advance player-idle time.

## Dynamic values and real range

| Actual authored placeholder | Templates | Value owner |
|---|---:|---|
| quantityLabel | 28 | Committed label; frozen accepted label at agreement/receipt |
| spiceName | 27 | Current lowercase display spice or accepted snapshot spice |
| currentBuyerOffer | 8 | Committed current offer |
| currency | 70 | Current provider literal varahas (not inflected) |
| offeredPrice | 6 | Seller input/last price; SafeNumber fallback |
| counterPrice | 51 | Positive updated offer, else current offer |
| finalPrice | 8 | Frozen agreement price when accepted |
| ruleReply | 5 | Exact brain core/delegate, never independently randomized |

All three characters use the same runtime market/quantity/parser policies; this is verified shared code, not a Father price restriction. Prices are positive Int32 inputs when semantically valid, with **no999 ceiling**. Initial requests normally use280/560/1400/2800g. The game formats these as1Seer/2Seers/1Veesai/2Veesai, with parenthesized metric UI gloss. Remaining-stock fallbacks and player quantity changes can produce other integer labels. Market values/events/wealth/personality determine offers; no authored list fixes a global economic price range. Arbitrary player-referenced amounts can be much larger than generated offers, including objection/clarification without agreement.

Physical generated products: **pepper, cinnamon, cardamom**. Brain descriptors: black pepper, cinnamon bark, green cardamom. Clove remains in the legacy catalog/parser but is excluded by the physical generator whitelist; turmeric is a player-side token, not a generated trade. Do not record them as required current products.

| Output unit forms | Game conversion / accepted aliases |
|---|---|
| Palam / Palams |35g; palam(s)|
| Seer / Seers |280g; seer(s)|
| Veesai |1400g; veesai/viss|
| Manangu / Manangus |11200g; manangu(s)/maund(s)|
| Bahar / Bahars |448000g; bahar(s)/candy/candies|
| Other input |g/gm/gram(s), kg/kgs/kilogram(s) and bag(s) reformat to the traditional label; a bag is the current lot (default280g), not a new universal unit|

These are game conventions, not verified historical metrology. Formatter thresholds are403200g Bahar,10080g Manangu,1260g Veesai,224g Seer, otherwise Palam; amounts are rounded. The displayed approximate label can differ from stored grams. Preserve the committed label and accepted snapshot; do not recompute an independent spoken quantity or claim arbitrary fractions are supported. Positive integer quantity extraction/conversion has no practical business cap; Int32 overflow/unchecked multiplications are implementation limits, not support guarantees for enormous trades.

Existing ChatManager speech sanitization removes parenthesized/bracketed UI gloss. The inventory provides exact raw rendered examples AND sanitized TTS examples; numerical spelling-out in a future generator must not change subtitle/game numbers. Source currency is always varahas, so1varahas remains a real presentation-grammar issue. Approve singular varaha and matching subtitle policy; never change an amount to avoid the problem.

Reusable vocabulary estimate:29 components for1–999 (one–nineteen, eight tens, hundred, and),3 products,2 currency forms,9 traditional unit forms =43 core recordings **per character voice**. Add zero/thousand/million/billion and3 descriptors ->50 extended pieces. `this lot`, contextual currency tails, names and boundary variants are additional where needed; estimates are not exact final queues.234 must compose two+hundred+and+thirty+four, never digit labels or Father audio. Unit amounts and referenced prices may exceed999. Unknown/unsupported labels require an explicit whole-utterance fallback, never missing words or unrelated audio.

Evidence: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/MarketManager.cs:143`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/NegotiationStateManager.cs:3085`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:777`, `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Level1GameState.cs:92`.

## Optional, defensive and excluded source coverage

The inventory includes80 distinct brain utterances (82 occurrences) and11 presentation fallback sources plus3 provider defaults. These are shared C# source templates, **not interchangeable NPC recordings**. Most brain greetings/counters/social/role answers are replaced by the character table, so their coverage class is FALLBACK_ONLY. Fourteen source strings form a conservative clarification envelope:11 GetClarificationLine branches plus null-input, missing-ultimatum-price and missing-price guards. Nine normal nested-recovery branches are labelled REACHABLE_OPTIONAL; empty transcript, fulfillment-state and impossible/null-input cases remain defensive. Blank voice input normally recovers in voice UI before negotiation.

GetClarificationLine checks unresolved terms, fulfillment, empty/unrecognized speech, multiple prices, historical reference, quantity state, seller-price state, accept/counter state, repeat/confused, then default. Thus a repeat request can receive a state-specific prompt first. All four character wrappers must retain the exact core; Unknown is a pure ruleReply delegate, not a recordable word. Nested dynamic fields include first/last referenced prices, quantities and descriptors. Agreement source words use accepted terms. Do not claim static-wrapper coverage alone covers all recoveries.

All43 generic registry lines are listed but classified UNREACHABLE_OR_DEFERRED for known authored characters because the provider returns early. The hard-REJECT raise line at RuleBasedNPCBrain:211 is preempted by the unconditional hard-REJECT return at200; the hard ternary at213 is likewise preempted. Duplicate Agreed/Then-we-shall-not-trade source occurrences share exact words but retain additional locations in notes. TimePressure has no active route; defer all its authored variants.

Two ChatManager “The market is too noisy, could you repeat that?” assignments are UI-only at those call sites. Voice recovery “Sorry, I didn’t catch that. Try again.” and browsing/approaching status are not NPC speech recordings. No arbitrary backend/LLM prose, debug speech or other character table is counted as a current finite inventory requirement. The inventory retains exceptional/masked sources explicitly instead of confusing them with normal essential lines.

## Read-only voice-studio cross-check

Current studio inventory rows for this character: **164**. Manifest recording rows: **290**; modes `{"FRAGMENT": 208, "FULL_CLIP": 82}`; provisional priorities `{"ESSENTIAL": 123, "DEFERRED": 167}`; statuses `{"NOT_GENERATED": 287, "VALIDATED": 3}`. These are recordings/fragments, not authored template counts or reviewed audio approvals. Studio voice is **Indic Parler / Sneha**; no voice/profile changed.

Text/ID/restriction/mapping check results:

```json
{
  "inventory_duplicate_ids": [],
  "manifest_duplicate_ids": [],
  "missing_templates": [],
  "changed_text": [],
  "restriction_mismatches": [],
  "missing_recording_ids": [],
  "wrong_speaker_ids": [],
  "assembly_reconstruction_mismatches": [],
  "fixed_mapping_mismatches": [],
  "dynamic_assembly_missing": [],
  "raw_source_hash_drift_ids": []
}
```

Exact authored texts were compared by ID against inventory.json. Duplicate template and recording IDs were checked. Dynamic assembly operations were reconstructed in order against current templates with the studio's explicit currency binding (`{currency}` -> varahas); every referenced recording ID and character was checked. Fixed mappings were checked against current original text. This validates textual mapping structure, not pronunciation or actual WAV spoken words. VALIDATED pilot paths were checked for existence only; no audio was played, synthesized or approved.

Outdated studio prose still says “No Level1 prerecorded/assembly adapter exists.” A shared adapter now exists for Father; this character still lacks an imported bank. “Only four quantities” is also inadequate for arbitrary current quantity changes. `SiddharthChettiDialogue.cs` is the authoritative current source, not a historical character implied by its filename. Existing manifest ESSENTIAL/DEFERRED flags do not control Unity random selection; deferred variations remain reachable. No priorities or studio files were edited. New review decisions must not be inferred from the historical726 target.

Manifest-validated paths (not newly approved):

```json
[
  {
    "id": "REC_SAR_2D8A56FAD34B984B",
    "dialogue": "L1_SAR_066",
    "wav": "G:\\Users\\chitr\\Desktop\\Capstone\\NPC-Voice-Generator\\production\\validated\\saraswati_chetti\\REC_SAR_2D8A56FAD34B984B__take01.wav",
    "exists": true
  },
  {
    "id": "REC_SAR_776C472F0C7CF8B9",
    "dialogue": "L1_SAR_111",
    "wav": "G:\\Users\\chitr\\Desktop\\Capstone\\NPC-Voice-Generator\\production\\validated\\saraswati_chetti\\REC_SAR_776C472F0C7CF8B9__take01.wav",
    "exists": true
  },
  {
    "id": "REC_SAR_5DE114704BFD8F0A",
    "dialogue": "L1_SAR_122",
    "wav": "G:\\Users\\chitr\\Desktop\\Capstone\\NPC-Voice-Generator\\production\\validated\\saraswati_chetti\\REC_SAR_5DE114704BFD8F0A__take01.wav",
    "exists": true
  }
]
```

## Voice-production scope — estimates, not an approved batch

Whole fixed authored clips: **64** routed lines. Dynamic/nested templates: **97**. Conservative authored+clarification pool: **235** exact trimmed static/full text pieces. With50 reusable vocabulary pieces (deduplicated overlap), approximately **284** takes; broader nonpreempted defensive catalog approximately **380**. These are source-segmentation estimates: boundary/inflection/emotion variants can change them. They do not claim all conservative guard strings are normally heard or that every fragmented line will sound natural.

Prefer complete fixed sentences. Use dynamic voice-specific vocabulary only for genuinely dynamic values and preserve the exact selected template/core. Currency-in-phrase singular variants and safe traditional-quantity labels must be mapped explicitly. Do not reuse Father/another NPC audio, silently generalize a product to spices, omit quantities, or count template appearances as unique recordings. Unrecorded random alternatives need an explicit honest fallback; a sheet priority alone cannot make them unreachable. Source/role corrections, pronunciation and production scope require review before generation.

## Twelve documented audition selections — no synthesis

These are actual current Unity/export IDs; rendered examples below use300varahas as a rejected seller ask,234 as the current/counter/agreed offer,2Seers and pepper. They are illustrative snapshots, not a guaranteed twelve-turn conversation. The low-patience HOLD requires actual patience<=2; receipt requires completed delivery/settlement. Clarification uses the exact unrecognized-speech core. Repetition/random selection can choose another table variant in gameplay.
For number articulation, repeat the counter template at1/19/34/105/234/999 and a referenced price above999. Keep original quantities/items dynamic. These are test selections only, not new historical priority flags.

| ID / phase | Exact Unity template | Illustrative complete TTS input / context |
|---|---|---|
| L1_SAR_001 / CustomerGreeting | Namaskaram, merchant. I am Saraswati Chetti. I need {quantityLabel} of {spiceName} for my shop. | Namaskaram, merchant. I am Saraswati Chetti. I need 2 Seers of pepper for my shop. |
| L1_SAR_074 / AskName | Saraswati Chetti. I keep a shop here and buy what my customers ask for. | Saraswati Chetti. I keep a shop here and buy what my customers ask for. |
| L1_SAR_070 / OfferReason | I must resell this lot in my shop. At {currentBuyerOffer} {currency}, the measure still makes sense. | I must resell this lot in my shop. At 234 varahas, the measure still makes sense. |
| L1_SAR_041 / BuyerCounterFirst | For this {quantityLabel}, I can offer {counterPrice} {currency}. What do you think? | For this 2 Seers , I can offer 234 varahas. What do you think? |
| L1_SAR_045 / BuyerCounterMiddle | Your case has merit. I can increase my offer to {counterPrice} {currency}. | Your case has merit. I can increase my offer to 234 varahas. |
| L1_SAR_025 / QuantityChanged | Then we are discussing {quantityLabel} of {spiceName}. Let us price that quantity. | Then we are discussing 2 Seers of pepper. Let us price that quantity. |
| L1_SAR_148 / BuyerHoldsFirm | My patience is short, but my offer is unchanged: {counterPrice} {currency}. | My patience is short, but my offer is unchanged: 234 varahas. |
| L1_SAR_029 / SellerPriceTooHigh | {offeredPrice} {currency} is beyond what I can justify for this measure. My offer is {counterPrice} {currency}. | 300 varahas is beyond what I can justify for this measure. My offer is 234 varahas. |
| L1_SAR_058 / SellerPriceAccepted | Avunu, the price is agreed: {finalPrice} {currency} for {quantityLabel} of {spiceName}. Please bring the goods. | Avunu, the price is agreed: 234 varahas for 2 Seers of pepper. Please bring the goods. |
| L1_SAR_106 / UnclearSpeech | Konchem more detail, please: {ruleReply} | Konchem more detail, please: The market is busy and your words were unclear. Say it again plainly. |
| L1_SAR_111 / TransactionSuccess | The goods are received and the measure is right. Thank you, merchant. | The goods are received and the measure is right. Thank you, merchant. |
| L1_SAR_103 / HostileWarning | I will consider a price, not an insult. Speak plainly and respectfully. | I will consider a price, not an insult. Speak plainly and respectfully. |

## Static verification and remaining uncertainty

The documentation builder checks every source string at its exact current line, lossless literal/value decomposition, resolved examples, unique inventory IDs, current studio text/restrictions/assembly references and source hashes before/after. It never calls gameplay code. No claim is made that Unity tests passed or every route has run on device. Large-number/fraction handling, regional pronunciation, actual recording contents and natural delivery remain their own testing/listening gates. Current Editor Father override must be changed manually for later Indian-character runtime tests.
