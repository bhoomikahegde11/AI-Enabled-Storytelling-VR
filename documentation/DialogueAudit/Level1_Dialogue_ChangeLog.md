# Level 1 dialogue correction log

This log describes the current uncommitted correction set. The source files and generated CSV, rather than earlier narrative counts, are authoritative. Do not treat extraction as editorial approval.

## Antigravity correction set independently reviewed

- Six active character tables were revised for conversational wording, character voice, suspiciously low prices, agreement versus fulfillment, and removal of unsupported pre-agreement inspection actions. Missing `{currency}` units in some Lakshmi and Chinnamma counter lines were added. The six tables now contain **206, 167, 164, 170, 170, 170** templates respectively; 1,047 total.
- The provider gained authored presentation, scenario-specific selection, low-patience restrictions and recent-four anti-repeat history. The optional LLM rewrite is bypassed for these six authored characters only.
- `TimePressure` remained authored but inactive. Existing idle reminders and transaction failure handle silence; no new timer was introduced.
- The registry and authoring guide describe Father Penteado as a Portuguese cleric rather than a Jesuit. Francisco's code comments avoid identifying the fictional 1522 merchant with the historical viceroy who died in 1510. Runtime character IDs were preserved.
- Six focused character test suites were added/updated in `unity/StorytellingVR/Assets/_Project/Scripts/Editor/NegotiationTests/`. They have **not** been executed in this verification pass.

## Independent corrections in this pass

1. `DialogueTableResponseProvider.FindRestrictedTemplate`: restricted the 70% mood / 30% generic preference to `BuyerHoldsFirm`. Applying it to all scenarios allowed generic social wording to bypass eligible mood-specific lines. Low-patience matches still take priority, and recent-four avoidance remains active.
2. `DialogueTableResponseProvider.MapScenario`: acceptance presentation now follows the accepted input intent (`ACCEPT` selects `PlayerAcceptedDeal`; accepted seller price selects `SellerPriceAccepted`), rather than the negotiation round. This changes wording selection only, not economics or the trade state.
3. `FatherPenteadoDialogue.cs`: changed `L1_FAT_060` and `L1_FAT_064` from “bring the measure” to “fill the bag,” matching the physical scooper/bag fulfillment action. Their prices, quantities, agreement status and dialogue IDs remain unchanged.
4. `Level1_MainLoopUpdated.unity`: corrected Father Penteado's serialized `buyerOrigin` from “Jesuit Missionary” to “Portuguese Cleric.” The registry correction alone had left the scene-authored profile inconsistent. No ID, object, progression or gameplay setting was changed.
5. `Level1FatherPenteadoDialogueTests.cs`: added focused regression coverage for generic HOLD reachability with recent mood wording, specificity in non-HOLD social dialogue, low-patience priority and intent-based acceptance categories.
6. `extract_level1_dialogues.py`: made paths portable, validated the current authoring format and source placeholders, and separated machine validation, reachability and editorial review. Regenerated the CSV and `audit_stats.json` without blanket `APPROVED` labels.

See `Level1_Final_Verification_Report.md` for evidence, remaining issues and recording preparation.
