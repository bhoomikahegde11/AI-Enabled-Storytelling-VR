# Four Indian-production NPCs — authoritative current-dialogue discovery

27October2026. Documentation only. No Unity/gameplay/scene/profile/studio edits, voice generation, Father package changes, Unity launch/tests/builds, commits or pushes.

## Identity, availability and scope

The four requested production characters are **Lakshmamma**, **Chinnamma Naik**, **Saraswati Chetti**, **Abdul Rahman**. The registry and the active scene contain the same IDs, names, roles, base personalities and nonzero model references. **Abdul is an Arab caravan trader in the story**, not an asserted Indian local; “four Indian NPCs” here names the existing Indic-Parler production group. His nationality/region is not rewritten for convenience.

**Current saved Editor scene is forced to `father_penteado`.** The four are normally registered/selectable, but none will appear via the current forced Editor selection. Clear Level1GameState -> Editor Debug -> Debug Force Character Id for normal selection, or later enter the exact desired ID. This audit preserves that setting. The override is Editor-only; it is ignored in Quest builds. Without the override, all six registered buyers have positive selection weights; the previous character is temporarily excluded to prevent immediate repetition. No separate per-character enable flag disables these four.

Current saved local brain/session/intent paths are enabled. LLM understanding/generation are disabled, and authored characters bypass LLM wording replacement. Only Father's prerecorded bank is currently imported/registered; the four Indian banks are not wired. The selected studio voices are retained: Swapna, Priya, Sneha, Kabir respectively. The disabled scene Sherpa fallback's generic en_IN labels are not proof of those production voices or accents.

## Comparison and exact counts

**Fixed/dynamic columns below include all authored templates.** The core/optional columns exclude the explicitly unrouted TimePressure alternatives; Unknown and Lakshmamma's OfferReason route retain caveats. CORE denotes the ordinary trade/lifecycle capability, not an empirical frequency percentage. Shared masked brain sources are not added to authored totals.

| Character / ID | Base personality / role | Authored | Fixed / dynamic | CORE / optional | Unrouted | Routed fixed / dynamic | Approximate assets, authored+recovery / broader defensive |
|---|---|---:|---:|---:|---:|---:|---:|
| Lakshmamma / `lakshmi_amma` | Friendly / Local Household Buyer |206|77 /129|146 /56|4|73 /129|327 /420|
| Chinnamma Naik / `chinnamma_naik` | Strict / Vijayanagara Wholesale Spice Buyer |167|64 /103|116 /48|3|61 /103|288 /378|
| Saraswati Chetti / `saraswati_chetti` | Friendly / Local Vijayanagara Retail Shop Owner |164|67 /97|117 /44|3|64 /97|284 /380|
| Abdul Rahman / `abdul_rahman` | Friendly / Arab Caravan Trader |170|71 /99|117 /50|3|68 /99|290 /385|
| **Total, per-character appearances** | |**707**|**279 /428**|**496 /198**|**13**|**266 /428**|**1189 /1563**|

Each table covers all34 scenario values and has an unrestricted group for every scenario; there are **zero exact authored duplicate strings within each character**. The13 unrouted texts are TimePressure only. The202/164/161/167 provider-addressable counts are not a claim that every unusual input/state has been demonstrated in Unity. All new inventory rows have exactly one of CORE, REACHABLE_OPTIONAL, FALLBACK_ONLY, UNREACHABLE_OR_DEFERRED with source/trigger evidence.

Recording numbers are **estimates, not an approved production queue**. They deduplicate whole fixed lines/static phrase pieces and add a50-piece extended vocabulary per speaker, plus conservative precise-recovery coverage. Context-sensitive currency variants, names, edge prosody, `this lot`, quantity phrases and emotional takes can change totals. Broader estimates add ordinarily masked defensive source text, some needing correction before it should be spoken. Do not treat1189 or1563 as a requested batch, or726 as a quota. A shared semantic fragment still needs a separate speaker-specific recording.

## Lakshmamma verification

All seven pre-existing files under Lakshmamma_Discovery were read. The original206 authored texts, restrictions/IDs and80 shared brain source strings match current C#. All script hashes in the old snapshot remain equal. **Only its recorded scene hash differs**, consistent with later Father test/voice-audibility settings. Its original dialogue inventory is current; it did not need a repeated full content audit.

Gaps addressed in documentation only:

- Added canonical EXACT_LINES.md / DIALOGUE_INVENTORY.csv / .json for consistent filenames, explicit four-way classification, current hashes and more complete source/studio checks.
- Appended a current-verification section and12 actual-ID test selections to AUDIT.md. Original prefixed inventories, COUNTS.json and language proposals remain preserved.
- The old narrative describes normal eligibility, not today's forced-Father Editor scene. The earlier voice-production estimate327 is still reproducible under its stated scope.
- Its OfferReason caveat remains: ordinary pressure questions become QUERY_BUYER_BUDGET and are not remapped to OfferReason for Lakshmamma. The other three **are** explicitly remapped. A constructed-input renderer test does not prove ordinary parser access for her four OfferReason lines.

## Complete actual flow and evidence

Authoritative files under `unity/StorytellingVR/Assets/_Project/Scripts/Level1/`:

| Source | Responsibility / evidence |
|---|---|
| Core/Dialogues/DialogueCharacterRegistry.cs | Names, role metadata, base personalities, registered sets and ID normalization |
| Core/Dialogues/Characters/LakshmiAmmaDialogue.cs |206 Lakshmamma templates; historical name retained for compatibility |
| Core/Dialogues/Characters/ChinappaNaikDialogue.cs |167 **Chinnamma Naik** templates; not a separate Chinappa character |
| Core/Dialogues/Characters/SiddharthChettiDialogue.cs |164 **Saraswati Chetti** templates; not Siddharth dialogue |
| Core/Dialogues/Characters/AbdulRahmanDialogue.cs |170 Abdul templates |
| Core/LocalTradeSessionGenerator.cs | Actual physical spice whitelist, initial quantity, weighted/forced character, effective personality and economic context |
| Core/InputNormalizer.cs | Player input normalization; broad recognized words do not imply a generated item/unit |
| Core/NegotiationStateManager.cs | Input semantics, validity, counted pressure, patience and semantic next state |
| Core/RuleBasedNPCBrain.cs |80 unique domain/fallback utterances and exact clarification cores |
| Core/Level1GameState.cs | Committed quantity, immutable accepted snapshot, settlement ownership and Editor override |
| Core/DialogueTableResponseProvider.cs | Authored selection/restrictions/history, scenario mapping, substitution and presentation snapshot |
| ChatManager.cs | Selected presentation, subtitles, voice dispatch, receipt and terminal-timeout hooks |
| MarketplaceManager.cs | Session preparation, customer identity/model, real waiting/reminders and next customer |
| Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt:346–400 | Explicit character style/backstory guide; not a phonetic/historical authority |

Per-line source path/line/hash, exact wording, scenario, restrictions, trigger, class, raw example, sanitized TTS example and literal/value assembly order are retained in every canonical inventory. Example values are illustrative:300 seller ask,234 current/counter/accepted offer,2Seers pepper, with accepted/low-patience/receipt constraints stated. They are not guaranteed sequential conversations or altered game values.

`L1_LAK/CHI/SAR/ABD_###` are **existing production/export aliases verified against current Unity literals**, not IDs serialized in DialogueLine. No intrinsic stable Unity line-ID field exists. This is why inventories also include exact-template hashes/keys and restrictions. Test selections do not invent new runtime IDs or rename character IDs.

## Shared dynamic and fallback requirements

- Current generated physical products are **pepper, cinnamon, cardamom**. Clove is legacy-catalog/parser only; turmeric's physical enum/parser presence does not make it a generated requested spice. Fallback descriptors are black pepper, cinnamon bark, green cardamom.
- There is **no global1–999 price cap**, including Father’s actual integer grammar. Prices/referrals use positive Int32 values where semantically valid; buyer limits derive from market value/personality/wealth/events, and referred seller prices may exceed those limits. Full support must handle larger referenced amounts or fail honestly for the entire utterance.
- Default requests are1/2Seers and1/2Veesai; remaining stock and valid quantity changes introduce other integer labels. Game output families are Palam(s), Seer(s), Veesai, Manangu(s), Bahar(s). Metric/bag/alias input is re-formatted. Approximate conversions/rounding are **game conventions**, not historical proof. Fractional/huge quantities are not automatically safe merely because numeric text parses.
- Current provider currency is always **varahas**; one-varaha inflection requires an explicit presentation/spoken-text policy. Existing TTS removes parenthesized metric gloss; do not record grams/kg into period speech or change the economic quantity.
- Accepted lines use immutable accepted price/item/quantity. TransactionSuccess requires completed delivery/settlement. Counter wording requires real upward movement; HOLD preserves the standing figure. A round-final category is not an irreversible last-offer promise.
- Every character needs four precise clarification wrappers and the Unknown ruleReply delegate. Nine ordinary nested core branches plus conservative guards are documented. Recording the wrapper alone is not recovery coverage. Core text can include multiple referenced prices, historical references and current goods/quantity.
- Each inventory includes all80 shared brain templates,11 presentation fallbacks,3 provider defaults,43 bypassed generic lines and2 UI-only error occurrences. These appear in each character's documentation for traceability; do not count them as80*4 unique game-source templates or claim each is normally audible. Physical recordings remain voice-specific.
- No explicit reputation/desperation/round-restricted DialogueLine group exists for these characters. Respect affects generation and effective personality; round selects counter stage; low patience changes eligible lines. Do not invent extra emotional/high-respect variants that runtime cannot select.

## Current studio consistency results

Read-only comparison used production/inventory.json, dynamic_assemblies.json, production_manifest.csv and voices.json, rather than old sheets as authority.

| Character | Manifest recording rows | FULL_CLIP / FRAGMENT | ESSENTIAL / DEFERRED | Manifest VALIDATED / NOT_GENERATED | Dynamic authored assembly entries |
|---|---:|---:|---:|---:|---:|
| Lakshmamma |355|104 /251|140 /215|3 /352|129|
| Chinnamma |307|91 /216|137 /170|3 /304|103|
| Saraswati |290|82 /208|123 /167|3 /287|97|
| Abdul |297|86 /211|122 /175|3 /294|99|

For all four: **zero missing authored IDs/texts, zero changed original text, zero duplicate inventory IDs/recording IDs, zero mood/patience restriction mismatches, zero missing/misassigned referenced recording IDs, zero fixed-text or dynamic-order reconstruction mismatches** in this source cross-check. All428 dynamic authored entries reconstruct exactly after the old exporter’s explicit currency binding. Every manifest-validated pilot path exists; format/word correctness was **not** newly tested. No human audio approval is inferred from VALIDATED.

Studio source-file hashes still match current authored files. Some rows have no independent recording because they reuse fragments, or delegate ruleReply. A recording manifest's primary-text column is not a complete per-template map. Outdated studio prose still claims no adapter exists; the shared adapter now exists for Father but no Indian voice bank is imported. Its initial-four-quantity limitation and plural-currency binding remain production gaps. Existing priorities are provisional and do not disable unrecorded Unity alternatives. No studio file was changed.

## Corrections and approvals before recording

| Character | Highest-value specific review |
|---|---|
| Lakshmamma | Proper name pronunciation; household/local origin rather than role-as-homeland or resale-profit fallbacks; ordinary OfferReason route ambiguity; currency singular |
| Chinnamma | Kannada colour explicitly supported but mother tongue unspecified; “A fair dealing” article; do not accidentally synthesize Chinappa from class name; Strict/low-patience versus blanket studio friendly mood |
| Saraswati | “For this2Seers” grammar; named regional expressions need linguistic approval; exact-measure receipt claims versus abstract one-scoop validation; quantity-change/current-offer clarity; do not synthesize Siddharth |
| Abdul | Story role is Arab caravan trader; keep Kabir but approve desired accent interpretation; Marhaban/Shukran and name pronunciation; do not invent a homeland from “Arab Caravan Trader” |
| Shared | Singular currency, safe metric-gloss omission, role-label origin leakage, misleading zero-movement/fallback HOLD wording, no voice borrowing, support beyond999 and exact nested recovery |

Known shared fallback mistakes are normally masked; that is not proof they can be ignored in a full defensive package. Likewise, the resale-profit motivation is wrong for Lakshmamma but generally compatible with the other three. **Do not blindly apply Father's or Lakshmamma's role corrections to everyone.** All optional changes remain separate from original text in each LANGUAGE_AND_PROPOSALS.md.

## Exact deliverable paths

Under repository root `documentation/DialogueAudit/`:

- `Lakshmamma_Discovery/`
- `ChinnammaNaik_Discovery/`
- `SaraswatiChetti_Discovery/`
- `AbdulRahman_Discovery/`

Each contains **AUDIT.md, EXACT_LINES.md, DIALOGUE_INVENTORY.csv, DIALOGUE_INVENTORY.json, LANGUAGE_AND_PROPOSALS.md**. Lakshmamma's original prefixed files are also preserved. Each AUDIT.md contains **12 exact existing source-ID audition selections** with the complete original template and rendered example. There are48 selections total; no clips were synthesized. They cover greeting/identity, character-specific trade reason, first/middle counter, quantity, low-patience HOLD, high-price refusal, acceptance, clarification, receipt and hostile warning. Lakshmamma's reason selection is explicitly conditional.

`FOUR_INDIAN_AUDIT_COUNTS.json` contains traceable counts/consistency results. `build_four_indian_audits.py` is a documentation-only source reader/exporter; it does not execute Unity/C# or generation scripts.

## Ready for the next phase?

**Yes for reviewing the exact scripts and preparing controlled voice auditions with the existing four selected voices. Not yet approved for mass production or Indian prerecorded Unity integration.** Approve pronunciation/accent interpretation, the separately proposed text corrections, single-varaha/subtitle policy, larger-number/quantity/fallback scope and final take selection. Do not silently rewrite the source during synthesis. Current Editor Father override must be changed manually when runtime testing another character later.

Static document checks passed: exact source text/line round trips, lossless literal/value reconstruction, resolved illustrative placeholders, unique IDs, studio mapping checks, scene/profile reference checks and unchanged inspected Unity hashes. No Unity tests, actual runtime route execution, voice playback or pronunciation approval occurred.
