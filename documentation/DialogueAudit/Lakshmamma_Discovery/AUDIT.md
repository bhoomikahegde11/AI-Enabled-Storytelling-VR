# Lakshmamma — current Level 1 dialogue and voice-production discovery

Read-only source inspection, 27 October 2026. No Unity launch, Editor tests, build, synthesis, package rebuild, voice-studio change or Father integration change. Only this documentation directory was created. This is a source-reachable inventory, not a measured gameplay-frequency study or a claim that every branch was exercised.

## Review files

- **LAKSHMAMMA_EXACT_LINES.md**: complete phase-organized verbatim script, including every authored line, brain source, presentation fallback and excluded generic line, with source line, conditions and rendered example.
- **LAKSHMAMMA_DIALOGUE_INVENTORY.csv / .json**: the same script-friendly inventory; JSON also carries source hashes, vocabulary and counts. UTF-8 CSV preserves Kannada-influenced expressions as written.
- **COUNTS.json**: machine-readable totals. `build_inventory.py` reads source text and writes these audit documents only; it does not execute C# or invoke Unity.
- **LANGUAGE_AND_PROPOSALS.md**: voice direction, pronunciation decisions and optional wording proposals. None are applied to inventory originals.

Paths below are repository-relative. Line numbers in the inventory are exact for the hashed source revision; they will move after edits. `L1_LAK_###` identifiers follow the existing recording-sheet exporter’s source order. They are audit aliases, not stable IDs stored in the Unity table. Exact template/hash plus character, scenario and restrictions are the safer production match. No IDs or priority flags were changed.

## 1. Verified totals and scope

| Catalog | Count | Meaning |
|---|---:|---|
| Authored templates | **206** | 206 distinct texts, 43 DialogueLine groups, all 34 DialogueScenario values |
| Authored with a provider route | **202** | Includes four conditional OfferReason lines and one residual Unknown delegate; not 202 independently demonstrated player-input paths |
| Unrouted authored TimePressure | **4** | L1_LAK_164–167; no normal scenario-selection call routes here |
| Routed authored fixed lines | **73** | No runtime placeholder |
| Routed authored dynamic/nested templates | **129** | Includes four clarification wrappers and one `{ruleReply}` delegate |
| Unique brain speech sources | **80** | 82 source occurrences; most normally replaced by authored presentation |
| Presentation fallback sources | **11** | Four generator greetings, six generic reminder/timeout lines, one ChatManager timeout default |
| Provider defaults | **3** | Receipt thanks, blank-rule reply, greeting-context rule reply; last is context-only |
| Generic registry lines | **43** | Explicitly catalogued as bypassed for a correctly identified authored Lakshmamma |
| Text-only recovery occurrences | **2** | Same ChatManager error sentence at two source locations; not sent to NPC voice playback there |
| Total documented rows | **345** | Includes excluded/bypassed/defensive sources, not 345 required recordings |
| Unique exact source strings across rows | **342** | Includes those excluded sources; not a generation target |

The old “204 entries” description does not match this revision. There are no exact duplicates within the 206 authored texts. Near-duplicates and reuse across categories do exist. Repeated numerical utterances are not separate templates.

## 2. Actual execution and source ownership

1. `Core/Dialogues/DialogueCharacterRegistry.cs:106–115` registers `lakshmi_amma`, display **Lakshmamma**, role metadata **Local Household Buyer**, base **Friendly**. Legacy Lakshmi names normalize to the same ID; that does not authorize speaking the old name. The scene’s MarketplaceManager profile repeats this configuration at `Level1_MainLoopUpdated.unity:18891`.
2. `Core/LocalTradeSessionGenerator.cs:27` chooses a physical spice, in-stock starting quantity, market value, character, effective personality and starting/max buyer offers. Its own four greetings are defensive substitutes, not the normal authored greeting.
3. `ChatManager.cs:239–305` starts the local session and selects a first/repeat authored greeting. Repeat recognition is tracked during the ChatManager lifetime; a mid-conversation “hello” is not another customer visit. `FirstReplyIntroRoutine` retains the existing approximately three-second introduction delay.
4. `InputNormalizer.Normalize` repairs/transforms incoming **player** text; it does not rewrite Lakshmamma’s authored lines. It recognizes more product/unit words than the generator can physically trade. This distinction matters for voice vocabulary.
5. `NegotiationStateManager.ClassifyInput` and `ProcessLocalTurn` produce the semantic input and brain decision. `ProcessLocalTurn:2508` commits valid quantity changes and updates patience/expected response before presentation. Accepted terms remain an immutable snapshot. No dialogue text should be used to invent a new decision.
6. `RuleBasedNPCBrain.GenerateReply` constructs a domain reply. `DialogueTableResponseProvider.GetReply:149` selects a character-specific presentation, or preserves the domain reply. `MapScenario:578` and `MapAuthoredScenario:419` decide which table category is eligible.
7. `ReplacePlaceholders:777` renders the selected text. `LastPresentation` retains the exact selected template, scenario and copied values. `ChatManager.PresentNpcSubtitleAndTts:1142` sends that presentation through the existing AudioManager/subtitle path.
8. `GetIdleReminder`, `GetTransactionSuccess` and the terminal timeout path are separate presentation entry points, not ordinary parser responses. Successful transaction thanks are gated by `AcceptedTerms` and `SettlementClaimed`. Agreement lines request the goods; they are not receipts.

Saved scene flags at `Level1_MainLoopUpdated.unity:11860–11868`: local intent/brain/session enabled; LLM understanding/generation disabled; NPC TTS enabled. The new prerecorded bank is **Father only**. Lakshmamma still routes to the existing local provider (scene Sherpa component disabled, configured `en_IN_female`); there is no imported Lakshmamma prerecorded bank. Studio **Swapna** is the approved production assignment, not the scene Sherpa voice identity. A completed spreadsheet alone will not make her prerecorded audio play.

`useLocalLLMGeneration` is additionally suppressed for authored characters in `ChatManager:634`. Backend/API prose and debug arbitrary text are not finite current-scene Lakshmamma recording requirements. A different scene configuration could enable those paths; that would invalidate this finite-coverage boundary.

## 3. Authored phases and trigger coverage

The exact script file includes per-line restrictions; this table summarizes routing, not relative usage probability.

| Phase/category | Templates | Trigger/constraint |
|---|---:|---|
| CustomerGreeting | 6 | First encounter; asks seller price and specifies requested goods |
| RepeatCustomerGreeting | 6 | Actual subsequent visit |
| AskWhatBuyerWants | 6 | ITEM_QUERY after price introduced, without current-offer query |
| AskSellerPrice | 6 | ITEM_QUERY leaves ExpectOfferPrice; **not** generic PRICE_QUERY |
| AskBuyerBudget | 6 | Budget/offer question, item+current-offer question, CONTINUE |
| AskQuantity | 6 | QUANTITY_QUERY |
| QuantityChanged | 4 | Valid QUANTITY_CHANGE with commitsQuantity; quantity+price can counter/accept instead |
| SellerPriceTooHigh | 9 | Nonacceptance above buyer max, no upward movement; 3 low-patience alternatives |
| SellerPriceSlightlyHigh | 6 | Above-max difference <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2), without a raise |
| SellerPriceBelowExpected | 6 | Nonaccepted seller price below previous NPC offer; does not imply agreement |
| BuyerCounterFirst | 6 | Buyer offer moved up, bargaining round <=1 |
| BuyerCounterMiddle | 9 | Buyer offer moved up, rounds 2–4; 3 low-patience alternatives |
| BuyerCounterFinal | 6 | Buyer offer moved up, round >=5; does not impose an economic final-offer promise |
| BuyerHoldsFirm | 21 | 6 unrestricted, 5 low-patience, 5 Strict, 5 Impatient |
| SellerPriceAccepted | 8 | Accepted seller proposal; 2 low-patience variants; goods pending |
| PlayerAcceptedDeal | 8 | Player ACCEPT of outstanding terms; 2 low-patience variants |
| PlayerRejectedBuyerOffer | 6 | Nonterminal REJECT without buyer raise. Hard rejection departs; a raised response counters |
| OfferReason | 4 | PRICE_QUERY+asksReason and !asksItem; see reachability caution below |
| AskName | 4 | Social/general name question or exact normalized who are [you] |
| AskOrigin | 4 | Social/general where/from/origin, after specific topic checks |
| WeatherQuestion | 4 | Weather/rain/sun/sunshine; not a weather simulation |
| HistoryKing | 4 | King/Krishnadevaraya |
| HistoryTemple | 4 | Temple/Virupaksha |
| HistoryQuestion | 4 | City/market/bazaar/history/Hampi/Vijayanagara |
| SocialGreeting | 9 | Mid-visit greeting or other social/general; 3 low-patience alternatives |
| OffTopic | 6 | Nonterminal out-of-world topic |
| HostileWarning | 6 | Nonterminal hostility; no special measured anger bucket |
| UnclearSpeech | 4 | Clarification wrapper + exact brain ruleReply |
| Unknown | 1 | Precise ruleReply delegate for residual outcomes; input UNKNOWN normally routes UnclearSpeech |
| TransactionSuccess | 6 | Actual goods delivered and settlement claimed |
| TransactionFailure | 9 | Walk-away/dismiss/hard reject/terminal timeout; 3 low-patience alternatives |
| TimePressure | 4 | Authored, currently unrouted |
| IdleReminderFirst | 4 | First genuine player-idle reminder |
| IdleReminderSecond | 4 | Second genuine player-idle reminder |

All nonaccepted explicit price paths use current code thresholds, **not** old exporter assumptions such as “25% above max” or “12% above offer.” The inherited exporter’s human-written gating descriptions are stale; this inventory uses the actual current provider conditions.

### Selection, personality, repetition and respect

- `FindTemplate:250` immediately returns `FindRestrictedTemplate` for authored characters. It does not fall through to the generic registry if a character line is missing.
- Every Lakshmamma scenario has an unrestricted authored group. Low patience means <=2; medium 3–5; high >=6. A matching low-patience group (specificity >=2) takes priority over general personality wording.
- For HOLD at non-low patience, Strict/Impatient mood wording blends with unrestricted HOLD wording: 70% mood choice only when both unused pools exist, otherwise the remaining unused pool. This is a real selection weight, not a global frequency estimate.
- Recent-template history is a queue of at most four exact templates. When a small bucket is exhausted it releases old entries until one is available. Four-line clarification buckets can therefore cycle sooner than a “never repeat the last four” absolute promise. History resets on a different trade object; it does not remember every prior visit.
- There are **no explicit reputation-, desperation- or round-restricted Lakshmamma DialogueLine groups**. Round still chooses counter scenario; respect indirectly changes effective personality, selection chances and economic context. Do not manufacture “high respect” voice variants that Unity cannot select.
- Generator effective personality probabilities for Lakshmamma’s Friendly base: reputation<35 -> 45% Impatient /35% Strict /20% Normal; 35–69 ->15% Impatient /40% Strict /45% Friendly; >=70 ->15% Strict /40% Friendly /45% Normal. These probabilities are actual generator branches, not line frequencies.
- Initial patience: Friendly7 /Strict4 /Impatient3 /Normal5. Normal bargaining pressure costs one; HOSTILE costs two; OFF_TOPIC costs one; ordinary queries/social/quantity-only changes and clarification do not consume a bargaining round/patience cost. Frustration/trust can still change and cause departure. The voice should express this context, not alter it.

## 4. Fallbacks: what really needs speech

The inventory includes all **80 distinct** brain utterance templates, not merely clarification examples. Their source locations and branch conditions are recorded individually. Most greetings, counters, social answers, rejections and departures are **masked by authored lines** in the normal local path.

Fourteen distinct sources can supply clarification/recovery directly or through a wrapper: 11 in `GetClarificationLine:727–779`, null-input “Speak plainly, merchant.”, missing-ultimatum-price “State your final price clearly, merchant.” and invalid/missing PRICE “I did not catch your price. Say it plainly.” This is a conservative coverage envelope: empty transcripts are normally stopped by voice/ChatManager guards, and FulfillmentExpected is defensive because an existing accepted snapshot exits the brain before clarification. Do not count these all as frequently heard.

The clarification priority is significant: unresolved attached terms -> fulfillment expected -> empty -> unrecognized speech -> multiple actionable prices -> historical price -> missing quantity/ExpectQuantity -> missing price/ExpectOfferPrice -> ambiguous accept/ExpectAcceptOrCounter -> repeat/confused -> default. A repeat request can receive a price-state clarification before reaching the dedicated “I will say it again” branch.

Nested requirements include two distinct referenced prices, last historical price, current offer, current quantity and exact spice descriptor. Four wrappers must combine with the **selected** core, not a random generic “please repeat.” The `Unknown` delegate contributes no standalone spoken words and must preserve its actual brain reply.

All 11 presentation fallbacks and 3 provider defaults are included, with masking/context-only status. The generic registry’s 43 templates are explicitly excluded from Lakshmamma’s normal generation queue. Text such as “Lakshmamma is browsing your goods...” and microphone retry status is UI/narration, not her speech. The two identical ChatManager “market is too noisy” assignments are also UI-only at those call sites. The voice manager’s “Sorry, I didn’t catch that. Try again.” is status UI via `RecoverFromVoiceFailure:648`, not an NPC response to record.

### Unused, ambiguous and risky coverage

1. **TimePressure L1_LAK_164–167 is unrouted.** Keep in documentation, defer recording unless a real route is added.
2. **OfferReason L1_LAK_101–104 has a provider route but uncertain ordinary-input access.** The test `PriceReasonIsHouseholdBuying_NotResaleProfit` manually constructs PRICE_QUERY+asksReason. Actual explicit pressure questions are classified QUERY_BUYER_BUDGET (`NegotiationStateManager:907–923`), and Lakshmamma is deliberately excluded from the provider’s budget+reason remap (`MapAuthoredScenario:435`). Thus ordinary “why only...” generally repeats the budget rather than using the household explanation. A passing constructed-input renderer test does not establish parser reachability. Keep these four as conditional, not falsely “confirmed frequent.”
3. **Unknown L1_LAK_151 is a residual delegate.** Normal garbage maps to clarification; do not advertise a separate common unknown recording. Some defensive nonfinal decisions can still select it. Missing/externally supplied brain strings are not covered simply by recording the token `{ruleReply}`.
4. **Preempted brain counter wording:** the hard-REJECT raised-offer sentence at `RuleBasedNPCBrain:211` cannot execute after the unconditional hard-REJECT return at200; the hard ternary at213 is likewise preempted. Inventory retains it as source evidence, not a requirement. Identical “Agreed...” and “Then we shall not trade today.” literals at multiple sites are deduplicated with additional locations retained.
5. **Role-label leakage exists in shared defensive text.** `{buyerOrigin}` is `Local Household Buyer`, not a place. Shared brain greetings/origin replies can form “of Local Household Buyer” or “travelled far from Local Household Buyer.” Her authored origin answers correctly say she is local. Do not synthesize that metadata literally in defensive Lakshmamma speech.
6. **Shared resale-profit reason is wrong for her household role**, but her authored OfferReason overrides it when that scenario is selected. Preserve the distinction between masked bad source and audible gameplay defect.
7. **Currency inflection:** Unity always substitutes `{currency}` as `varahas`; its raw result can be “1 varahas.” Father v2 intentionally speaks singular currency. A Lakshmamma bank must explicitly preserve that presentation correction and matching subtitles, or await an approved source/presentation change. Do not change the economic number.
8. **Metric gloss vs speech:** Unity quantity labels include approximate grams/kg. Existing ChatManager TTS sanitization removes parentheses/brackets (`SanitizeTextForTts:1265`). Audio should speak traditional quantities only; the exact-source inventory deliberately preserves the raw label and shows raw Unity examples separately.
9. **Rounded labels are not exact physical measurements.** `FormatTraditionalQuantity` snaps arbitrary integer grams to an approximate traditional amount; its displayed label may differ from stored grams. That is existing gameplay behavior. A voice bank must use the committed label and must not silently invent a different measure/conversion.
10. **Semantic duplicates are intentional variation, not exact duplicates.** Many HOLD sentences, paired agreement categories and six quantity replies share meaning. They are independently selectable; dropping their recordings requires an explicit playback fallback policy or a later selection change, not silently claiming coverage.

## 5. Dynamic values, products and ranges

Only **eight placeholders actually occur** in the authored table:

| Token | Templates using token | Authoritative value |
|---|---:|---|
| quantityLabel | 59 | Committed trade label; accepted snapshot label for agreement/receipt |
| spiceName | 60 | Lowercase display spice; frozen accepted spice where applicable |
| currentBuyerOffer | 14 | Standing offer after the decision |
| currency | 93 | Literal `varahas` in current provider |
| offeredPrice | 7 | Valid seller price, otherwise last seller price; SafeNumber fallback applies |
| counterPrice | 63 | Positive updatedOffer, otherwise current buyer offer |
| finalPrice | 16 | Frozen accepted price when bound; otherwise positive resolvedPrice/counter fallback |
| ruleReply | 5 | Exact domain-selected fallback, not another random table selection |

Provider additionally supports buyerName, characterName, minimumPrice, round, reputation, patience and desperation, but none occur in her current authored table. Shared fallbacks introduce buyerOrigin, quantity, spiceDescriptor, currentOffer, sellerPrice, counterOffer, nextOffer, result.updatedOffer, accepted snapshot properties and the first/last referenced prices. The CSV/JSON lists each literal’s precise tokens and ordering.

**Physical products:** pepper, cinnamon, cardamom (`LocalTradeSessionGenerator.SpiceKeys:24`). **Brain descriptors:** black pepper, cinnamon bark, green cardamom (`GetSpiceDescriptor:1091`). Clove remains in MarketManager’s legacy catalog and the parser’s vocabulary but is excluded from generated physical orders. Turmeric is understood as a player-side token; it is not a generated Level 1 item. Neither should be added to the required Lakshmamma spice recordings for this build.

**Normal generated requests:** 280g -> `1 Seer (~280g)`, 560g -> `2 Seers (~560g)`, 1400g -> `1 Veesai (~1.4 kg)`, 2800g -> `2 Veesai (~2.8 kg)`. If stock cannot satisfy those choices, `PickQuantity` can select a different positive remaining-stock amount with a minimum35g fallback. Do not assume the four default labels exhaust runtime quantities.

**Input unit aliases and game conversions** (`ConvertToGrams:3084`, `IsQuantityUnit:3126`):

| Accepted input | Game grams per amount | Output unit family |
|---|---:|---|
| palam/palams | 35 | Palam/Palams |
| seer/seers | 280 | Seer/Seers |
| veesai/viss | 1400 | Veesai (same plural spelling) |
| manangu/manangus/maund/maunds | 11200 | Manangu/Manangus |
| bahar/bahars/candy/candies | 448000 | Bahar/Bahars |
| g/gm/gram/grams; kg/kgs/kilogram/kilograms | 1;1000 | Re-formatted traditional label |
| bag/bags | Current trade grams, default280 | Re-formatted traditional label, not a universal bag size |

These are game conventions, **not a verified historical metrology claim**. Integer quantities are parsed; arbitrary fractional quantity speech is not promised. The formatter chooses Bahar at >=403200g, Manangu at >=10080g, Veesai at >=1260g, Seer at >=224g, otherwise Palam, then rounds to an integer unit amount. Accepted quantity, money and physical one-scoop fulfillment remain separate concerns. No new physical measurement rule is proposed.

**Prices:** positive C# integers are accepted where valid; no universal 999 cap exists. Player-stated/referenced prices can exceed ordinary buyer offers and still appear in objections/clarifications. Market values depend on spice, quantity and active event; a typical fresh default request has market value roughly27–525, starting offers roughly22–604 and max offers up to roughly856 under current multiplier extremes. These are illustrative static envelopes for the four normal quantities/events, **not validation limits**. Quantity changes/stock fallbacks, saved data and arbitrary player proposals invalidate those small envelopes. Generate compact 1–999 grammar as a minimum; support higher integer reference prices with thousand/million/billion or an honest whole-utterance fallback. Int32 parsing and unchecked quantity multiplication are implementation limits, not a promise that every huge integer trade is safe.

## 6. Recording requirements and reuse estimate

**Recommended strategy, pending approval:** preserve the selected **Indic Parler / Swapna** identity; record fixed lines whole, and reserve dynamic assembly for values. Reuse the existing player-facing presentation architecture later; do not use Father audio as Lakshmamma vocabulary. Names, emotional delivery and all speech must remain her own recordings.

The following counts are reproducible **text-segmentation estimates**, not an approved queue or verified audio-quality count. The estimator splits at placeholders, removes punctuation-only pieces, deduplicates exact trimmed static text, and assumes each remaining piece is one take. Boundary variants, currency-in-phrase inflection and emotion-specific takes may raise the count; contextual grouping may lower or raise it.

| Requirement | Estimated unique recordings/text pieces |
|---|---:|
| Routed fixed authored lines | 73 |
| Static phrase pieces from routed dynamic authored lines | 184 |
| Combined character-specific pool above | 257 |
| With conservative 14 clarification/recovery source envelope deduplicated | 278 |
| Core reusable vocabulary: number words + products + currency + units | 43 |
| Extended vocabulary with zero/scales/three descriptors | 50 |
| Combined authored/recovery + extended vocabulary, exact-text deduplication | **327** |
| Broader defensive catalog estimate (all nonpreempted brain/presentation sources, excluding generic/UI/TimePressure) | **420** |

The 327 estimate is **not full coverage of every defensive brain fallback**. The 420 estimate includes normally masked and conditionally reachable material, including text defects that must not simply be spoken. Neither number is a request to generate now. There is no exact “all required WAVs” count until wording, defensive coverage, currency boundaries and expression variants are approved.

Core vocabulary: 29 number components (one–nineteen; twenty/thirty/forty/fifty/sixty/seventy/eighty/ninety; hundred; and), 3 product names, 2 currency forms, 9 traditional unit forms. This composes every integer1–999, e.g.234 -> two + hundred + and + thirty + four. It does not mean 999 number recordings. Add zero/thousand/million/billion and black pepper/cinnamon bark/green cardamom for extended references/fallbacks. “this lot” and connectors such as “of” remain phrase requirements where used. All reusable vocabulary still needs **Swapna’s** voice; semantic reusability does not permit cross-speaker audio reuse.

For more natural default requests, optional complete quantity phrases (“one Seer”, “two Seers”, “one Veesai”, “two Veesai”) can reduce joins without hardcoding a particular price or spice. This is an optional four-take contextual optimization, not a replacement for arbitrary supported quantity labels. Keep price + currency phrase boundaries under review rather than assuming an isolated “varaha” will join naturally everywhere.

### Existing studio data is not an approval manifest

`production/production_manifest.csv` contains **355 Lakshmamma recording rows**: 140 ESSENTIAL /215 DEFERRED; 104 FULL_CLIP /251 FRAGMENT; 3 VALIDATED /352 NOT_GENERATED. That is a provisional recording inventory, not 355 full dialogue lines. All **206 current authored texts/IDs match** `production/inventory.json`; all **129 routed dynamic templates** have entries in `dynamic_assemblies.json`. Some of that file’s integration-gap prose predates the Father adapter and is now stale.

The apparent omissions L1_LAK_032 and059 from the manifest’s primary-text column are covered by shared recording IDs in `inventory.json` and assembly maps; L1_LAK_151 has no own audio because it delegates ruleReply. Do not regenerate them solely because the CSV stores another template as the fragment’s primary source. The three manifest-validated pilot rows are L1_LAK_095 (rejection),152 (thanks),168 (first reminder). Format/manifest validation does not establish that their pronunciation/delivery is approved. No audio was played or regenerated during this audit.

## 7. Comparison with Father v2

| Requirement | Father v2 | Lakshmamma current discovery |
|---|---|---|
| Authored table size | 170 including3 unrouted TimePressure | 206 including4 unrouted TimePressure |
| Routed authored | 167 | 202 provider-addressable, with query/residual caveats |
| Shared brain sources | 80 | Same80 source templates, different character binding and ordinary masking |
| Packaged mappings / actual WAVs | 259 /425 | No new package created;355 provisional manifest rows is a different unit |
| Fixed packaged clips | 106 | 73 routed authored fixed lines before shared recovery/presentation material |
| Dynamic sounds | 319 Father-specific fragments | Estimate depends on approved Lakshmamma segmentation; do not copy counts or voices |
| Currency and traditional quantities | Inflected audio, integer labels, metric gloss omitted | Same technical issues must be handled; original authored currency remains plural |
| Identity substitutions | Portugal/priest corrections | Local household buyer; never substitute role metadata as homeland or resale motivation |
| Clarification | Exact nested core, not random recovery | Four distinct wrappers + Unknown delegate; same precision requirement |
| Runtime | Imported Father bank, device testing pending | Shared provider can host a separate bank; none wired for her yet |

Father’s approved wording is not permission to rewrite Lakshmamma. Several Father v2 corrections are role-specific. Father’s 425 recordings are not an estimate for every other character. The existing provider supports the same basic operation grammar, copied accepted terms, exact selection and continuous PCM assembly; native sample rate must come from the Indian bank, not be copied blindly from Father’s24kHz files.

## 8. Approval decisions before production

1. Confirm the voice direction in LANGUAGE_AND_PROPOSALS.md while retaining **Swapna**, including how much Kannada influence and which pronunciations to approve. Do not treat a model speaker name as proof of a regional accent.
2. Approve a **wording freeze**: preserve all current authored texts, or explicitly accept the separately listed small wording changes. No silent product generalization or quantity removal.
3. Decide generation scope: all provider-addressable authored variation plus precise recovery, versus selected common variations with an explicit missing-audio policy. Equal random selection means an unrecorded alternative remains reachable. Historical726 is not a quota.
4. Confirm singular **one varaha**, omission of metric UI gloss from speech, support beyond999, and defensive coverage of arbitrary referred prices. Subtitles must follow the chosen spoken plan where presentation wording differs.
5. Decide whether to defer the four uncertain OfferReason routes unchanged or authorize a later targeted routing correction. This audit does not change parser/presentation behavior.
6. Decide whether to correct role-label/resale wording in defensive voice-side mappings, following a documented subtitle/audio difference policy, rather than recording known bad source literally.

No runtime tests were run. The documentation extractor checked every inventory text against its exact source line, rendered placeholder examples without unresolved fields, validated unique inventory IDs and rechecked inspected Unity source hashes. Branch reachability, regional pronunciation and actual TTS delivery still require their respective runtime/listening review.

<!-- FOUR_INDIAN_VERIFICATION -->

## Current-source verification and added classification

27October2026: all seven original audit artifacts were read. All206 exact authored texts and all80 shared brain strings remain current; all hashed script sources match the original snapshot. The saved scene hash changed (Father testing/audibility configuration), with `debugForceCharacterId=father_penteado` currently active. The earlier audit describes ordinary selection eligibility, not the current forced Editor selection. No original authored audit was repeated or rewritten.

Canonical `EXACT_LINES.md`, `DIALOGUE_INVENTORY.csv` and `.json` now add the requested four coverage classes, current hashes and explicit studio mapping checks. Original prefixed inventories and COUNTS.json remain as historical audit artifacts. Their scene hash is intentionally not silently refreshed. See the master summary for source consistency/limitations.

## Twelve documented audition selections — no synthesis

These are actual current Unity/export IDs; rendered examples below use300varahas as a rejected seller ask,234 as the current/counter/agreed offer,2Seers and pepper. They are illustrative snapshots, not a guaranteed twelve-turn conversation. The low-patience HOLD requires actual patience<=2; receipt requires completed delivery/settlement. Clarification uses the exact unrecognized-speech core. Repetition/random selection can choose another table variant in gameplay.
For number articulation, repeat the counter template at1/19/34/105/234/999 and a referenced price above999. Keep original quantities/items dynamic. These are test selections only, not new historical priority flags.

| ID / phase | Exact Unity template | Illustrative complete TTS input / context |
|---|---|---|
| L1_LAK_001 / CustomerGreeting | Good day, merchant. I need {quantityLabel} of {spiceName}. What are you asking? | Good day, merchant. I need 2 Seers of pepper. What are you asking? |
| L1_LAK_105 / AskName | I am Lakshmamma. And you, merchant? | I am Lakshmamma. And you, merchant? |
| L1_LAK_101 / OfferReason | I have other provisions to buy as well. My offer is {currentBuyerOffer} {currency} for {quantityLabel} of {spiceName}. | I have other provisions to buy as well. My offer is 234 varahas for 2 Seers of pepper. **Conditional provider-input route; normal price-pressure questions use budget wording for Lakshmamma.** |
| L1_LAK_059 / BuyerCounterFirst | I can offer {counterPrice} {currency} for {quantityLabel} of {spiceName}. What do you say? | I can offer 234 varahas for 2 Seers of pepper. What do you say? |
| L1_LAK_065 / BuyerCounterMiddle | Sari, I can come up to {counterPrice} {currency}. What do you say? | Sari, I can come up to 234 varahas. What do you say? |
| L1_LAK_037 / QuantityChanged | Sari, {quantityLabel} of {spiceName}, then. My offer is {currentBuyerOffer} {currency}. | Sari, 2 Seers of pepper, then. My offer is 234 varahas. |
| L1_LAK_177 / BuyerHoldsFirm | Ayyo, let us come to an answer. I remain at {counterPrice} {currency}. | Ayyo, let us come to an answer. I remain at 234 varahas. |
| L1_LAK_041 / SellerPriceTooHigh | Ayyo, {offeredPrice} {currency}? That is too much. My offer remains {counterPrice} {currency}. | Ayyo, 300 varahas? That is too much. My offer remains 234 varahas. |
| L1_LAK_083 / SellerPriceAccepted | Sari, agreed at {finalPrice} {currency}. Please bring the {quantityLabel} of {spiceName}. | Sari, agreed at 234 varahas. Please bring the 2 Seers of pepper. |
| L1_LAK_149 / UnclearSpeech | Sari, let us be clear. {ruleReply} | Sari, let us be clear. The market is busy and your words were unclear. Say it again plainly. |
| L1_LAK_154 / TransactionSuccess | Sari, I have my spice. My thanks to you. | Sari, I have my spice. My thanks to you. |
| L1_LAK_141 / HostileWarning | Ayyo, there is no need to speak to me like that. Let us bargain with respect. | Ayyo, there is no need to speak to me like that. Let us bargain with respect. |
