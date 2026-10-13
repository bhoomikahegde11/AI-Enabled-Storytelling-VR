# Level 1 recording-sheet verification

The six current C# tables and `Level1_AllNPC_Dialogues.csv` match on ID, character, scenario, mood, patience and exact text for all **1,047** templates. There are **34** represented scenarios. The six workbooks have one overview row per source template, and every manifest reconstructs its C# template exactly. No duplicate Dialogue_ID, static Segment_ID or planned WAV filename was found. All required static and vocabulary references resolve to rows in the same character workbook. All integers 1–999 decompose into the 36 recorded number components. The exported XLSX files were read back and checked against the source inventory.

| NPC | Templates | Static clips | Number clips | Vocabulary clips | Planned clips | Unresolved dialogue-placeholder cases |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Lakshmi Amma | 206 | 345 | 36 | 17 | 398 | 71 |
| Chinnamma Naik | 167 | 270 | 36 | 17 | 323 | 49 |
| Saraswati Chetti | 164 | 240 | 36 | 17 | 293 | 39 |
| Abdul Rahman | 170 | 250 | 36 | 17 | 303 | 36 |
| Francisco de Almeida | 170 | 258 | 36 | 17 | 311 | 36 |
| Father Penteado | 170 | 252 | 36 | 17 | 305 | 35 |
| **Total** | **1,047** | **1,615** | **216** | **102** | **1,933** | **266** |

Of the templates, 422 contain no placeholders and 625 are dynamic. All clip statuses are initially `PENDING`; the reported recording progress is zero. `Recording_Summary.json` contains these machine-readable totals and per-character values.

## Unresolved before final voice production

- **204 `{quantityLabel}` cases:** the main number/unit can be assembled, but the label's `(~…g/kg)` suffix has no approved spoken treatment. Decimal kilogram values also exceed the integer-only library.
- **32 `{offeredPrice}` cases:** the player can state a price above 999; the approved number library covers only 1–999. Current code does not impose a speech-safe upper bound.
- **30 `{ruleReply}` cases:** the rule brain can supply an arbitrary sentence, including a specific clarification question. A fixed small vocabulary cannot guarantee faithful speech.

`Unresolved_Audio_Cases.csv` gives the exact Dialogue_ID and recommended decision for each case. These 266 counts are **occurrences requiring a design decision**, not 266 additional WAV files. The 1,933 planned clips exclude any recordings that those later decisions may require.

## Other boundaries

The 19 `TimePressure` templates are in the source and the sheets, but the current silence system does not select that scenario. Clove and singular **varaha** are included as catalog/future vocabulary and are not used by the current local NPC provider. There is no evidence of a valid zero-valued trade requiring a zero clip. The current generated buyer price ceiling is approximately 856, but player-entered offers and arbitrary rule replies are not bounded by the 1–999 recording plan.

The audit Dialogue_ID scheme follows template order in each C# file. Audio production should freeze the current ID/text mapping before dialogue insertions or reordering. The generated workbooks are ready to coordinate fixed-line and known vocabulary recording; **the complete dynamic voice system is not ready to freeze** until the quantity suffix, large offers and `{ruleReply}` policies are decided and sample clip joins are heard.
