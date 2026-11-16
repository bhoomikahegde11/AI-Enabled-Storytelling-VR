# Recovered offline NPC voice workflow — 2026-10-26

## What was recovered

**The existing production audition tool is the sibling folder `G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator`.** The previous static audio checklist missed this out-of-repository studio and the branch-only prerecorded provider. Its P0/P2 segment allocation is not the original essential-line manifest and must not be treated as one. This recovery reuses the existing engine; it does not introduce Piper, another TTS engine, cloud TTS, or a new full-batch system.

No audio has been generated in this recovery. Full generation remains disabled pending listening approval and recovery/confirmation of the essential set. The only executable addition is a sample-only driver calling the existing `app.py` `ModelSession.generate`. It cannot generate a full bank.

## Recovered manifests and exact counts

| Source | What actually exists | Count / priority information |
|---|---|---|
| `feature/prerecorded-npc-voices` at `3fafa4f33fd3e59bf89dd4aa8dacbe33a94a8b8b`, `docs/voice-recording/level1/*.csv` | Four initial character sheets plus shared sheet | 4 × 41 + 34 = **198 rows**. No essential/priority column. |
| Same branch, `docs/voice-recording/level1/final/*.csv` | Four final scenario-aware sheets plus shared variable sheet | **439 rows**, **411 distinct `(unity_folder, file_name)` recording slots**. Repeated rows reuse files with identical text; no conflicting text for a slot found. No essential/priority column. |
| Current `documentation/DialogueAudit/Level1_AllNPC_Dialogues.csv` and six character tables | Source-exact authored-template inventory | **1,047 templates**, **34 scenarios per character**. Reachability/review columns, no saved essential/priority selection. |
| Current `documentation/DialogueAudit/RecordingSheets/*.xlsx` and summary | Six segmented recording plans | **1,615 static phrase files + 318 number/vocabulary files = 1,933 planned slots**. No saved essential/priority tags in the workbook values. |
| Sibling `NPC-Voice-Generator` | Audition/comparison UI, presets, cached model and samples | README explicitly says workbook importing, batch queue, resume/retry and bulk generation are **not implemented yet**. No essential manifest found there. |

**The approximately 700–800 ESSENTIAL-line manifest was not recovered.** I inspected current documentation/workbooks, both similarly named feature branches, their relevant Git history, and the sibling studio. I will not silently redefine “essential,” claim the 1,933-clip plan is a 700–800-line list, add initial and final sheet versions together, or infer priority from every authored line being reachable. An external/untracked selection not saved in these locations remains possible. Confirm its path or recover that specific selection before bulk generation.

The exact final historical CSVs and README have been exported read-only from the Git blobs into `historical_3fafa4f3/` beside this report. No checkout, cherry-pick or branch switch was used. They are evidence, **not** a current text authority.

| Historical final sheet | Rows | Distinct output slots |
|---|---:|---:|
| Lakshmi | 100 | 93 |
| Chinnamma | 101 | 93 |
| Saraswati | 100 | 93 |
| Abdul | 98 | 92 |
| Shared variables | 40 | 40 |
| Total | **439** | **411** |

The older final sheets cover four characters, not all six. Their content predates the negotiation fixes: for example, the old Lakshmi low-price recording says “Ah, that price is kinder than I expected.” It must not be used over the current suspicious-low-price refusal. The current CSV/C# source is authoritative for the audition texts.

## Existing engine, dependencies and selected voices

- Studio: `G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator/app.py`.
- Existing launch command: `launch.bat` starts its local Gradio audition UI, not a batch runner.
- Engine: **AI4Bharat Indic Parler-TTS**, model `ai4bharat/indic-parler-tts`.
- Cached model revision: `660bc53c64ae83150613264111708a0465211237`; `model.safetensors` is **3,751,321,772 bytes**.
- Cached description tokenizer: `google/flan-t5-large`, snapshot `0613663d0d48ea86ba8cb3d7a44f0f65dc596a2a`.
- Cache is directly under `cache/huggingface/models--...`, not just the empty `hub/` subfolder. Existing `cache_dir` and `HF_HOME` settings must be retained.
- Existing Python 3.12 virtualenv: `.venv/Scripts/python.exe`. Verified installed: PyTorch **2.5.1+cu121**, Parler-TTS **0.2.2** (requirements pin official commit `d108732cd57788ec86bc857d99a6cabd66663d68`), transformers **4.46.1**, gradio **5.23.3**, soundfile **0.13.1**, numpy **1.26.4**, accelerate **1.0.1**.
- Read-only preflight confirmed **CUDA available / NVIDIA GeForce RTX 4060 Laptop GPU**, successful Parler class import and both tokenizer loads with `local_files_only=True`. No synthesis, dependency installation or downloads were performed. The optional “Flash attention 2 is not installed” message is not evidence that this existing setup needs replacing.
- Existing generation: CUDA fp16, native **44,100 Hz**, **PCM_24** WAV masters, one model load reused; validation rejects empty/non-finite/near-silent audio. Do not silently resample or apply the 1.08×/1.15× audition previews to masters.

| Current character | Existing preset lookup key | Shortlisted speaker | Status |
|---|---|---|---|
| Lakshmamma | `Lakshmi Amma` | **Swapna** | Shortlisted; `approved:false`. Legacy key retained read-only, corrected spoken text supplied. |
| Chinnamma Naik | `Chinnamma Naik` | **Priya** | Shortlisted; `approved:false`. |
| Saraswati Chetti | `Saraswati Chetti` | **Sneha** | Shortlisted; `approved:false`. |
| Abdul Rahman | `Abdul Rahman` | **Kabir** | Shortlisted; `approved:false`. |
| Francisco de Almeida | `Francisco` | **Thoma** | Draft candidate; Portuguese characterization/accent unapproved. |
| Father Penteado | `Father Penteado` | **Raghav** | Draft candidate; Portuguese characterization/accent unapproved. |

The first four have Natural Conversational / Lively Negotiation / Brisk Conversational descriptions. The audition below retains the saved base description and speaker, varying only supported mood/pace/expression controls. No preset file or approval flag is changed. Older Piper/Sherpa experiments and the three runtime voice families are separate history; they are not substituted for this later Parler audition workflow.

## Existing generated samples and why full generation stopped

- `samples/` has **35 WAVs**, **34 generated-master JSON sidecars** and one join-preview WAV. All 34 sidecars are `PENDING`.
- Master speaker counts: Mary 8, Swapna 6, Meera 3, Dinesh 3, Aakash 1, Raghav 1, Vikas 1, Kabir 4, Gauri 1, Sneha 3, Priya 1, Kavya 2. No Thoma master found in this folder.
- `exports/` contains **7 WAV derivatives**, separate from the native masters.
- Latest controlled comparison: Kabir Natural / Lively / Brisk, same text and seed, at `samples/20261013T063631Z_kabir_22de0c1a.wav`, `...063637Z_kabir_7e8b4899.wav`, `...063643Z_kabir_1b3a5a1f.wav`.
- Earlier branch `9b11d29f` added **31 generic `test_merchant_voice` WAVs**. Their Git LFS objects are present locally, but these are not a completed six-character essential bank in the current worktree.
- Documented reasons: voice/delivery audition still pending, no voices approved in presets, fragment silence/prosody/join quality still under review, and workbook-to-batch queue was never implemented. Dynamic quantity suffixes, prices above 999 and unbounded `{ruleReply}` also remain unresolved in the newer segmented plan. These are documented limitations; no unrecorded crash or download failure is invented as the cause.

## Intended Unity mapping: two different historical contracts

1. The older feature branch implements **`PreRecordedNpcTtsProvider`**, `IScenarioNpcTtsProvider`, scenario-to-intent routing and per-character `full/prefix/variable/suffix` folders under `Assets/_Project/Audio/NPCVoices/Level1/`. Its Inspector auto-fill groups clips by filename/intent. Its CSVs prescribe names such as `greeting_01.wav` or `price_too_high_01.wav`. **It does not map the files into `DialogueVoiceDatabase`.** Do not assume these contracts are interchangeable.
2. The newer source inventory uses positional `L1_LAK_105` dialogue IDs and proposed `lak_105_s01.wav` fragment names, with per-character number/vocabulary libraries. Its recording-plan documentation explicitly leaves the ordered playback adapter for later.
3. The current `DialogueVoiceDatabase` handles fixed Free Roam/tutorial/epilogue line-ID-to-AudioClip entries. The current Level 1 path calls text TTS; neither the old scenario provider nor a new full `L1_*` mapping was restored by this recovery.
4. Old scenario-first playback can choose a generic/random clip for an intent; it is not proof that the recording matches today’s exact authored subtitle or frozen terms. No old brain/parser/table changes or serialized references have been brought across. Approved generation later needs an explicit coverage report against the chosen current contract, not a blind branch merge.

## Smallest useful audition batch: eight clips

These are **seven trading/response diagnostics plus one deliberately included name-pronunciation probe**. They are not falsely marked as members of a recovered essential manifest. The source ID/template is frozen; the two price examples render declared diagnostic values only and do not claim to cover every price. All six character candidates are covered, with three Swapna moods for identity consistency.

| # | Source ID | Character / voice | Delivery | Exact text to synthesize |
|---|---|---|---|---|
| 1 | `L1_LAK_105` | Lakshmamma / Swapna | friendly, moderate, moderate expression | I am Lakshmamma. And you, merchant? |
| 2 | `L1_LAK_041` | Lakshmamma / Swapna | stern, moderate, moderate expression | Ayyo, 250 varahas? That is too much. My offer remains 173 varahas. |
| 3 | `L1_CHI_108` | Chinnamma Naik / Priya | stern, brisk, moderate expression | Saaku. I will hear an argument about price, not an insult. |
| 4 | `L1_SAR_125` | Saraswati Chetti / Sneha | impatient, brisk, moderate expression | I need an answer now; my shop is waiting. |
| 5 | `L1_ABD_111` | Abdul Rahman / Kabir | happy, moderate, moderate expression | Shukran, merchant. The goods are here, and you have kept your word. |
| 6 | `L1_FRA_029` | Francisco de Almeida / Thoma | persuasive, brisk, high expression | Ora! 300 varahas is beyond what this lot can bear. I remain at 180 varahas. |
| 7 | `L1_FAT_111` | Father Penteado / Raghav | happy, moderate, restrained expression | Muito obrigado. The goods are here as agreed; you have my thanks. |
| 8 | `L1_LAK_152` | Lakshmamma / Swapna | happy, moderate, moderate expression | Thank you, merchant. This will be welcome at home. |

Listen for: Lakshmamma’s name (do not assume that a proposed phonetic spelling fixes it), Ayyo/Saaku/Shukran/Ora/Muito obrigado, **varahas**, 173/180/250/300, natural pauses, stance/emotion, clipped endings, same-actor consistency, and whether the two foreign characters’ drafts are suitable. No text normalization/pronunciation rewrite is silently applied. If pronunciation fails, review a separate explicit spoken-text override while retaining the exact canonical source; do not rewrite Unity dialogue to suit a model.

## Exact commands — no installations, model downloads or Unity writes

Run in **PowerShell**. First command is a dry-run only (already checked during recovery):

```powershell
$studio = 'G:\Users\chitr\Desktop\Capstone\NPC-Voice-Generator'
$sampleRunner = 'G:\Users\chitr\Desktop\Capstone\AI-Enabled-Storytelling-VR\documentation\DialogueAudit\OfflineVoiceRecovery\run_sample_batch.py'
& "$studio\.venv\Scripts\python.exe" -B $sampleRunner
```

When ready to generate **only these eight** audition masters:

```powershell
& "$studio\.venv\Scripts\python.exe" -B $sampleRunner --generate-samples
```

No inference was run during this recovery. The second command is provided for you to run; full-bank generation is not available from this driver. Keep other GPU-heavy applications in mind if the existing engine reports CUDA OOM; do not silently fall back to CPU or a different model.

The driver forces offline mode, imports the original studio without launching its guarded UI, loads the existing model once, and calls its existing generator. Masters and JSON sidecars receive the studio’s fresh timestamp/UUID names in its existing `samples/` folder. It does not copy or overwrite canonical Unity WAVs. `sample_runs.jsonl` beside this report records source IDs, speakers, exact text, seeds, actual master paths and WAV hashes. Native WAVs must be nonempty mono 44.1 kHz with finite, non-silent samples. Re-running skips prior matching, hash-valid samples; failures are logged and can be retried by rerunning the same command, producing fresh masters. Source CSV, studio app, presets and model revision are checked against the manifest; unexpected changes stop generation rather than silently changing an actor/text. Every result remains PENDING, never automatically approved.

## After listening approval — next authorized step, not implemented prematurely

Keep full-batch generation OFF until the samples and the actual essential manifest are confirmed. Then finish the missing queue around the **same ModelSession**: freeze the approved ID/text/voice/preset/pronunciation hashes; import only confirmed essential rows; resolve the selected filename/Unity mapping contract; append durable attempt/failure records; skip only outputs whose text/config hashes and WAV validation still match; retry failed rows with a bounded policy; produce a source-ID-to-WAV coverage/mapping report. Preserve all existing files unless an explicit replacement is approved. This is completion of the recovered studio, not a new synthesis engine. No full queue, importer or Unity mapping is falsely claimed as already present.

## Files changed and checks

- Added this recovery report, `sample_batch.json`, and the thin `run_sample_batch.py` audition driver.
- Added exact historical exports of five final CSVs plus their README in `historical_3fafa4f3/`.
- No production source, Unity references, WAVs, presets, credentials, installed dependencies or existing Lakshmamma edits were modified. No Unity/Hub/build/tests, commit, push or branch switch.
- Executed only read-only environment/cache/tokenizer checks and the driver’s default no-inference dry-run. No model weights were loaded for synthesis and no audio was generated.
- Current branch remains `integration/GameLoop`. Historical old branch head is `3fafa4f33fd3e59bf89dd4aa8dacbe33a94a8b8b`; newer `feature/npc-prerecorded-voices` head is `fd1b2167444d3b1019d283fe7f1b1a60b581bcf0`.

If an approved 700–800-line manifest lives outside the inspected folders or in an unsaved selection, its location remains the one missing input before bulk preparation. The recovered model and audition setup themselves are available locally.
