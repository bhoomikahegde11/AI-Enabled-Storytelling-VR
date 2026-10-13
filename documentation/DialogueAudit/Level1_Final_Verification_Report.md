# Level 1 final dialogue verification report

## Result and scope

This pass independently compared the current six C# character tables, registry, provider, gameplay call path, scene profile, test suites and `documentation/DialogueAudit/`. Unity, Unity Hub, builds and Unity tests were **not** run. The generated inventory matches all 1,047 current source templates exactly, with 1,047 unique IDs and 34 authored scenarios for each NPC. The counts are Lakshmi 206, Chinnamma 167, Saraswati 164, Abdul 170, Francisco 170 and Father Penteado 170. Antigravity's alternate 179/174/175/171/173/175 distribution has no support in the present source; only its grand total is correct.

The table has 263 `DialogueLine` entries. The only exact text shared across NPCs is the six intentional `{ruleReply}` passthrough templates in `Unknown`. The exporter validates placeholder names from `DialogueTableResponseProvider.ReplacePlaceholders`; it does not validate how a future voice recording will pronounce them. Its approval column is pending manual verification, not automatically approved.

## Problems found and corrections

| Finding | Evidence and impact | Correction |
|---|---|---|
| Conflicting counts and blanket approval | Narrative report contradicted the C# files and exporter; exporter wrote `APPROVED` for every row without editorial review. | Recomputed the source counts; regenerated CSV/stats; separated `Machine_Validation`, `Reachability` and `Review_Status`. |
| Generic templates bypassed specific mood wording outside HOLD | `FindRestrictedTemplate` applied the 70/30 mood/generic blend to every scenario, including `SocialGreeting`. | Confined the blend to `BuyerHoldsFirm`. Eligible low-patience/high-specificity lines still win; non-HOLD mood lines keep priority. |
| Agreement category depended on negotiation round | `MapScenario` chose `SellerPriceAccepted` on early rounds and `PlayerAcceptedDeal` on later rounds, regardless of how acceptance happened. | Map the accepted `ACCEPT` intent to `PlayerAcceptedDeal`; accepted seller-price inputs to `SellerPriceAccepted`. No price, patience or state change. |
| Father's fulfillment instruction pointed to the wrong action | `L1_FAT_060` and `L1_FAT_064` said “bring the measure”; the actual accepted-order flow fills a bag with a scooper. | Changed only those two texts to bag-filling language. Prices, quantities, placeholders, IDs and scenario assignments are preserved. |
| Father's scene profile retained an anachronistic label | The Level 1 scene serialized `buyerOrigin: Jesuit Missionary` while the registry said `Portuguese Cleric`. | Corrected that one scene value to `Portuguese Cleric`; retained the character ID and serialized linkage. |

The [Society of Jesus history](https://www.jesuits.global/about-us/our-history/) places its founding in 1540, after the game's 1522 setting. Francisco's runtime name remains “Francisco de Almeida,” but the dialogue and profile do not assert the first Portuguese viceroy's office or biography. [The historical viceroy died in 1510](https://www.infopedia.pt/artigos/%24francisco-de-almeida?intlink=true). The intended same-name fictional merchant should be made explicit in player-facing character notes before recording or marketing; changing his serialized ID is outside this pass.

## Selection and gameplay integrity

`DialogueCharacterRegistry` routes all six active IDs to their respective authored tables. `ChatManager` bypasses optional LLM rewriting for those six, preserving their authored words. `FindRestrictedTemplate` filters by character, scenario, personality, patience and other supported buckets before selection. Its highest matching specificity bucket is selected first. For `BuyerHoldsFirm` alone, when both mood and generic candidates have unused templates, the selector favors mood 70% of the time and generic 30%; when one subset is exhausted, it takes the other. The recent-four queue is reset on trade change and reused for the active trade. These are code-path findings; statistical frequency and headset playback require runtime validation.

The parser and `RuleBasedNPCBrain` remain authoritative for price extraction, quantity changes, acceptance, refusal, clarification, patience and walk-away. A rule-level acceptance begins `ChatManager.BeginAcceptedFulfillment` with frozen terms. Physical bag delivery calls `CompleteAcceptedFulfillment` through `BagReceiver`; settlement is claimed once and only then can `GetTransactionSuccess` return a completion line. The development/debug scooper-bypass setting can intentionally skip the physical step, so disable it when validating production fulfillment. No economics, transaction, inventory or save code was changed here.

Nineteen `TimePressure` templates are intentionally inactive: the current silence system selects `IdleReminderFirst`, `IdleReminderSecond`, then terminal `TransactionFailure`. No missing active timer route was found that requires activating `TimePressure`. The CSV distinguishes these rows from conditionally reachable templates.

## Remaining verification and voice preparation

Static source checks found no unsupported placeholder names or duplicate IDs, and the edited agreement lines no longer imply the unsupported “bring the measure” action. This does **not** certify all 1,047 lines as natural in performance or guarantee every conditional line is encountered. Run the six character EditMode suites plus parser, flow and transaction-integrity suites; then sample each NPC in Level 1 with high/low patience, HOLD/counter, social questions, suspicious low prices, accepted bag fulfillment and walk-away. Verify subtitle text, quantity/currency pronunciation and a single settlement under repeated interaction.

Before recording, freeze reviewed text and stable IDs, decide whether to include or archive the inactive `TimePressure` lines, and prepare per-character voice sheets with exact spoken text, substituted variable rules, pronunciation guidance, emotional delivery and file/clip mapping. `Review_Status` should change only after a human editorial and in-game review. This table is ready as a **recording-planning inventory**, but **not yet ready for final recording freeze**.

## Manual Unity test classes

- `Level1LakshmiDialogueTests`
- `Level1ChinnammaDialogueTests`
- `Level1SaraswatiDialogueTests`
- `Level1AbdulRahmanDialogueTests`
- `Level1FranciscoDialogueTests`
- `Level1FatherPenteadoDialogueTests`
- `Level1NegotiationParserTests`
- `Level1NegotiationFlowTests`
- `Level1TransactionIntegrityTests`
