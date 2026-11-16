# Lakshmamma name correction

The NPC previously displayed as Lakshmi Amma now displays as **Lakshmamma**. This is the same character, with 206 templates across 34 scenarios. Only four spoken templates changed, by substituting her name. Total inventory remains 1,047 templates.

## Compatibility

Preserved: `lakshmi_amma`, `LakshmiAmmaDialogue.cs`, its class name, internal method/field identifiers, all Unity GUIDs, model references, audio filenames and save keys. Legacy spellings remain accepted by lookup and are normalized only for presentation. Raw trade/save names are not rewritten. Legacy-name test fixtures deliberately remain to exercise compatibility. Other characters are unchanged.

Found variants include display spelling `Lakshmi Amma`, internal `LakshmiAmma` and `lakshmi_amma`, and bare `Lakshmi` in comments/internal identifiers. Additional case/separator variants are covered as compatibility inputs rather than blindly renamed.

Updated current CSV NPC labels and four template texts, extractor display metadata, audit summary and statistics. IDs, ordering, counts and review metadata are unchanged. Historical reports are not retroactively rewritten.

## Audio follow-up

No Level 1 Lakshmi prerecorded voice bank, mapped name-bearing WAV, or the planned files below was found in this checkout. No audio was synthesized, replaced, renamed or listened to. The scene retains a disabled legacy Sherpa provider reference; this task does not change audio architecture. Confirm the production voice bank/integration separately before claiming spoken-name coverage.

If these lines have already been recorded externally, refresh them manually using the corrected exact text:

| Dialogue ID | Corrected spoken line | Planned filename (not found in Unity assets) |
|---|---|---|
| L1_LAK_105 | I am Lakshmamma. And you, merchant? | `lak_105_s01.wav` |
| L1_LAK_106 | Lakshmamma. That is what you may call me. | `lak_106_s01.wav` |
| L1_LAK_107 | My name is Lakshmamma. It is good to meet you. | `lak_107_s01.wav` |
| L1_LAK_108 | You may call me Lakshmamma. Now we are properly acquainted. | `lak_108_s01.wav` |

Do not regenerate clips that do not speak her name. Recording-planning workbooks and their filenames/statuses were preserved; `RecordingSheets/LakshmiAmma_RecordingSheet.xlsx` and derived recording summaries still need a targeted refresh of NPC metadata and these four name-bearing rows before voice production. Never carry recorded/approved status onto changed speech without manual review.

## Verification

Static source/CSV comparison confirms all 1,047 IDs and texts match, each character still covers 34 scenarios, and only the four IDs above have changed dialogue text. CSV review metadata is preserved. The scene change is exactly one displayName scalar; no serialized IDs, GUIDs or model references changed. Added EditMode checks for new/legacy routing and self-introductions, unchanged other names, and HUD display of legacy/current names.

Run manually: `Level1LakshmiDialogueTests`, `Level1NegotiationParserTests`, `Level1FulfillmentTests`, `Level1TransactionIntegrityTests`. In gameplay inspect the NPC introduction, speaker label, identity question, browsing subtitle, ledger and repeat visit, including a legacy saved journey. Verify any externally managed audio bank separately.

Unity, its tests and builds were not run. No save files were edited.

## Exact files changed

Paths below are relative to the repository root:

- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/LakshmiAmmaDialogue.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1HUDManager.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs`
- `unity/StorytellingVR/Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity`
- `unity/StorytellingVR/Assets/_Project/Scripts/Editor/NegotiationTests/Level1LakshmiDialogueTests.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Editor/NegotiationTests/Level1FulfillmentTests.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Editor/Level1NegotiationParserTests.cs`
- `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt`
- `unity/StorytellingVR/Assets/StreamingAssets/Sherpa/README_SherpaVoices.txt`
- `documentation/DialogueAudit/Level1_AllNPC_Dialogues.csv`
- `documentation/DialogueAudit/Level1_Dialogue_Audit_Summary.md`
- `documentation/DialogueAudit/audit_stats.json`
- `documentation/DialogueAudit/extract_level1_dialogues.py`
- `documentation/DialogueAudit/Lakshmamma_Name_Correction.md` (this report)
