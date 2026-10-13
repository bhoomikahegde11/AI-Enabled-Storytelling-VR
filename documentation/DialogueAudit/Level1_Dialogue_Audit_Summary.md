# Level 1 dialogue inventory — verified source counts

The six active character tables under `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/` are the authority for this inventory. `Level1_AllNPC_Dialogues.csv` is generated from those files by `extract_level1_dialogues.py`; `audit_stats.json` contains the corresponding counts. This is a source inventory and static review, **not** an approval of every line's in-game delivery or recorded performance.

| Runtime character | Active source file | `DialogueLine` entries | Authored templates |
|---|---|---:|---:|
| Lakshmi Amma (`lakshmi_amma`) | `LakshmiAmmaDialogue.cs` | 43 | 206 |
| Chinnamma Naik (`chinnamma_naik`) | `ChinappaNaikDialogue.cs` | 42 | 167 |
| Saraswati Chetti (`saraswati_chetti`) | `SiddharthChettiDialogue.cs` | 43 | 164 |
| Abdul Rahman (`abdul_rahman`) | `AbdulRahmanDialogue.cs` | 45 | 170 |
| Francisco de Almeida (`francisco_de_almeida`) | `FranciscoDialogue.cs` | 45 | 170 |
| Father Penteado (`father_penteado`) | `FatherPenteadoDialogue.cs` | 45 | 170 |
| **Total** | **Six active tables** | **263** | **1,047** |

The alternate distribution in Antigravity's narrative report (179/174/175/171/173/175) sums to the same 1,047 but does not match the C# source. It appears to be a reporting or count-transcription error; the repository provides no evidence for a redistribution. No dialogue was moved or removed to force either distribution.

All **34** `DialogueScenario` enum values occur in each character table, but authored presence is not proof of runtime reachability. Nineteen `TimePressure` templates (4 Lakshmi, 3 each for the other five) have no present selection route. The active silence path uses `IdleReminderFirst`, `IdleReminderSecond`, then terminal `TransactionFailure`. These rows are marked `INTENTIONALLY_INACTIVE` in the CSV. The remaining rows are marked `CONDITIONALLY_REACHABLE`, since individual eligibility depends on intent, outcome, personality, patience, trade state and recent history. The `Unknown` rows are `{ruleReply}` passthroughs. Six identical `{ruleReply}` strings across different NPCs are intentional; dialogue IDs are unique.

The extractor validates character IDs and names, scenario coverage, nonempty templates, supported placeholder names, duplicate IDs and source locations. Its `Machine_Validation=PASS` records those checks only. `Review_Status=PENDING_MANUAL_VERIFICATION` is deliberately separate. Manual Unity EditMode and gameplay review, plus future voice/subtitle review, are still required before an editorial freeze. IDs are stable for the current source order; inserting an earlier template shifts later positional IDs, so preserve or map IDs when revising tables.

Runtime integration: `DialogueCharacterRegistry` maps all six IDs to their active tables. `DialogueTableResponseProvider` uses authored selection for these six and bypasses optional LLM rewriting in `ChatManager`. Matching low-patience/high-specificity lines win first. Only `BuyerHoldsFirm` blends eligible mood and generic lines; other scenarios retain mood-specific priority. Recent-four history is per active trade. An accepted price remains a pending order until physical fulfillment and claimed settlement; `TransactionSuccess` is gated separately.

The inventory is reproduced with `python documentation/DialogueAudit/extract_level1_dialogues.py` from any working directory. See `Level1_Final_Verification_Report.md` for corrections, limitations and manual test scope.
