# Anubhava prerecorded voice generation checklist — 2026-10-26

Read-only Unity/source/asset audit. Only this new Markdown document was written. No code, scenes, databases, workbooks or audio were changed; Unity, tests and builds were not run. This report is separate from `Lakshmamma_Name_Correction.md`.

## Read this before generating

**The Level 1 bank is absent, and the segmented playback adapter is also absent.** `ChatManager.TrySpeakNpcReply → AudioManager.TrySpeakText → INpcTtsProvider` currently sends text to TTS, not to `DialogueVoiceDatabase`. Generating the files below will not by itself make them play in Level 1. The active scene references a disabled legacy `SherpaEditorTtsProvider` component with the three serialized voice families below; this audit does not certify device playback. No code change is proposed or performed here.

The P0/P2 counts below are **the missing recording slots in the existing segmented Level 1 recording plan**, plus one explicitly optional epilogue player line. They are not 1,934 whole NPC sentences and not 1,934 currently wired AudioClips. Some phrases repeat in different occurrence-specific files: the existing plan deliberately assigns one file per occurrence, and these filenames are retained rather than inventing a new deduplication scheme. Priority does not mean integration-ready. Do not bulk-generate unresolved assemblies before agreeing their spoken form and playback approach.

**Existing-audio limitation:** this environment could inspect WAV data and GUID references but could not audition audio (audio input was unsupported). Consequently no existing WAV is certified verbatim-correct, and **no replacement is ordered merely from its filename or a changed subtitle**. P1 = zero *confirmed* incorrect recordings, not proof that every existing recording is correct. The listening-only holds below must be checked before final audio sign-off. No installed external recognizer, cloud upload or synthesis was used.

## Exact counts and priority rules

| Category | Missing static phrase files | Missing number/vocabulary files | Other | Total clip files to generate |
|---|---:|---:|---:|---:|
| P0 — essential Level 1 trading, instruction, recovery and resolution speech | 1,398 | 294 | 0 | **1,692** |
| P1 — confirmed incorrect/outdated/old-name spoken recordings | 0 | 0 | 0 | **0 confirmed** |
| P2 — optional social/history/name/weather speech, inactive lines and reserved/fallback vocabulary | 217 | 24 | 1 optional player epilogue line | **242** |
| Total | 1,615 | 318 | 1 | **1,934** |

**Immediate fixed-scene generation queue: 0 confirmed missing clips.** Both databases resolve their non-null GUIDs to present files. The seven null tutorial database entries have actual scene-assigned fallback WAVs. The five epilogue NPC/narrator clips already exist and are mapped. Keep these existing recordings unless listening establishes a real mismatch. A silent/miswired line is not automatically a new-recording requirement.

P0 includes all selectable variations of greetings, item/quantity/price queries, objections, counters, holds, acceptance, rejection, quantity changes, price reasons, clarification, hostile/off-topic recovery, idle reminders and transaction outcomes. P2 scenarios: `AskName`, `AskOrigin`, `HistoryKing`, `HistoryQuestion`, `HistoryTemple`, `SocialGreeting`, `TimePressure`, `Unknown`, `WeatherQuestion`. `Unknown` is pure `{ruleReply}` and contributes no fixed phrase file. `TimePressure` has 19 authored templates but no current selection route; its files are optional/inactive. Number files and active spices/units/plural currency are P0 dependencies. Clove, singular varaha and absent-value fallbacks are P2. No essential template is silently omitted just to reduce the count.

| Speaker | P0 | P1 | P2 | Total |
|---|---:|---:|---:|---:|
| Lakshmamma | 357 | 0 | 41 | 398 |
| Chinnamma Naik | 285 | 0 | 38 | 323 |
| Saraswati Chetti | 258 | 0 | 35 | 293 |
| Abdul Rahman | 261 | 0 | 42 | 303 |
| Francisco de Almeida | 268 | 0 | 43 | 311 |
| Father Penteado | 263 | 0 | 42 | 305 |
| You / player | 0 | 0 | 1 | 1 |

## Voice identity and delivery references

No approved prerecorded Level 1 actor bank or vendor voice ID was found. The references below preserve serialized TTS family assignments and the established recording-sheet directions; they do not invent an ElevenLabs/model speaker identity. Keep a consistent actor within each NPC bank, including all number/unit joins. Existing fixed-scene voices should be matched against the actual linked WAV, not replaced with the Level 1 TTS defaults.

| Voice code | Speaker | Existing serialized reference | Delivery to maintain |
|---|---|---|---|
| LAK | Lakshmamma | `en_IN_female; Local Indian Female` | Warm, familiar, quietly shrewd; motherly, not yielding |
| CHI | Chinnamma Naik | `en_IN_female; Local Indian Female` | Direct, confident, dry-witted |
| SAR | Saraswati Chetti | `en_IN_female; Local Indian Female` | Measured, observant, precise |
| ABD | Abdul Rahman | `kusal_male; Abdul / Fallback Male` | Courteous, sociable, persuasive |
| FRA | Francisco de Almeida | `en_GB_male; Foreign Male` | Expressive, energetic, commercially alert |
| FAT | Father Penteado | `en_GB_male; Foreign Male` | Thoughtful, restrained, gently curious |
| PLAYER | You / player | No established recorded player voice found | Subtitle-only is explicitly supported; do not force an NPC voice onto the player. |

## Lakshmi Amma → Lakshmamma: actual status

The display/name correction is already applied to the current C# dialogue and CSV. The internal ID `lakshmi_amma`, class/file names, GUIDs and proposed `lak_...wav` filenames intentionally remain compatible. Those identifiers do not prove a recording speaks the old name. No name-bearing Level 1 WAV or any of the four planned files below exists anywhere in Unity Assets, so **these are missing new lines in P2, not four P1 replacements**. No old-name audio was audibly verified; no existing clip is marked for replacement on that basis.

| Dialogue ID | Exact current spoken text | Planned filename | Existing WAV | Unity mapping |
|---|---|---|---|---|
| L1_LAK_105 | I am Lakshmamma. And you, merchant? | `lak_105_s01.wav` | None found | No |
| L1_LAK_106 | Lakshmamma. That is what you may call me. | `lak_106_s01.wav` | None found | No |
| L1_LAK_107 | My name is Lakshmamma. It is good to meet you. | `lak_107_s01.wav` | None found | No |
| L1_LAK_108 | You may call me Lakshmamma. Now we are properly acquainted. | `lak_108_s01.wav` | None found | No |

The existing Lakshmi workbook and `Recording_Summary.json` still use the old display name; the workbook name-bearing text also needs refresh before production. Use the corrected exact C#/CSV text above. Do not regenerate other clips because a character label, workbook filename or internal key contains the old spelling.

## Tutorial: existing fallbacks — DO NOT generate duplicates

`TutorialDialogueVoiceDatabase.asset` has 33 entries: 26 populated, seven null. `SpiceTutorialManager.ResolveVoiceClip` uses `databaseClip != null ? databaseClip : fallbackClip`. All seven fallbacks are assigned in the enabled build scene `SpicesInteraction.unity`. `OrderManager.tutorialSpice = 1` is Cardamom. The completion speech is currently labelled **Bhaskara**, despite its `NARRATOR_...` line ID. Maintain the actual linked voice when checking it.

| Line / speaker | Exact runtime text to compare by ear | Existing WAV | Mapping status | Generation decision |
|---|---|---|---|---|
| `CUSTOMER_SPICE_INTERACTION_REQUEST_01` — Rahim / scene label Customer | Could you fill one bag of Cardamom for me? | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Rahim/CUSTOMER_SPICE_INTERACTION_REQUEST_01.wav` | DB null; **scene `customerRequestClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `NARRATOR_SPICE_INTERACTION_SCOOPER_01` — Narrator | Good. Move the scooper into the sack of the requested spice. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Customer/Customer4.wav` | DB null; **scene `narratorScooperAppearedClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `NARRATOR_SPICE_INTERACTION_CARRY_01` — Narrator | Excellent. Now carry the spice to the customer's bag. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Customer/Customer3.wav` | DB null; **scene `narratorScoopedClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `NARRATOR_SPICE_INTERACTION_WRONG_SPICE_01` — Narrator | That is not the spice the customer requested. Try Again. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Customer/Customer2.wav` | DB null; **scene `narratorWrongSpiceClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `NARRATOR_SPICE_INTERACTION_WRONG_BAG_01` — Narrator | Collect the correct spice before filling the customer's bag. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Customer/Customer1.wav` | DB null; **scene `narratorWrongBagClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `CUSTOMER_SPICE_INTERACTION_THANKS_01` — Rahim / scene label Customer | Thank you, Merchant. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Rahim/CUSTOMER_SPICE_INTERACTION_THANKS_01.wav` | DB null; **scene `customerThanksClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |
| `NARRATOR_SPICE_INTERACTION_COMPLETE_01` — Bhaskara | Good. You have learned how to identify the spices and prepare them for trade. Now, the customers will arrive. Handle them as you have been taught. | `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/Bhaskara/Bhaskara_Conclusion.wav` | DB null; **scene `narratorCompletedClip` populated** | Existing, not missing. Listening hold only; no regeneration ordered. |

The four generically named `Customer1–4.wav` files and `Bhaskara_Conclusion.wav` especially need a quick human comparison against the above current script. Their names do not establish outdated speech. The request/thanks files are already character-specific Rahim WAVs. The scene label `Customer` is UI data, not evidence that the WAV says “Customer” or “Lakshmi Amma”.

Other tutorial checks: `NewSpiceScene` uses the six populated `BHASKARA_SPICE_INTRO_*` mappings (including serialized spice descriptions). `NewTransactionTutorial` has 13 database-backed utterances; its otherwise-null clip fields correctly defer to the database. The active panel/status speaker is Narrator (`NARRATOR_TRANSACTION_PANEL_01` and `NARRATOR_TRANSACTION_STATUS_UI_01`); an older tutorial script document still calls the panel line Bhaskara. Use current source, not that stale authoring label. `NewCoinScene` has its six populated Rahim/Narrator mappings. The eight legacy `TraderIntroSequence` IDs have no database entries but corresponding legacy MP3s exist and this component was not found in enabled build scenes; they are not eight missing essential recordings.

## Epilogue — existing recordings, not a regeneration order

| ID | Speaker / exact runtime words | Existing WAV | Mapping |
|---|---|---|---|
| `BHASKARA_EPILOGUE_01` | Bhaskara: You did well today. Here is your share of the earnings. | `unity/StorytellingVR/Assets/_Project/Audio/Epilogue/BHASKARA_EPILOGUE_01.wav` | Populated database entry; referenced by active epilogue |
| `NARRATOR_EPILOGUE_01` | Narrator: With enough Varahas now in hand, perhaps it is finally time to return to Meera. | `unity/StorytellingVR/Assets/_Project/Audio/Epilogue/NARRATOR_EPILOGUE_01.wav` | Populated database entry; referenced by active epilogue |
| `MEERA_EPILOGUE_01` | Meera: You came back. | `unity/StorytellingVR/Assets/_Project/Audio/Epilogue/MEERA_EPILOGUE_01.wav` | Populated database entry; referenced by active epilogue |
| `MEERA_EPILOGUE_02` | Meera: It is yours for 18 Varahas. | `unity/StorytellingVR/Assets/_Project/Audio/Epilogue/MEERA_EPILOGUE_02.wav` | Populated database entry; referenced by active epilogue |
| `MEERA_EPILOGUE_03` | Meera: Take it. Perhaps it will answer your questions. | `unity/StorytellingVR/Assets/_Project/Audio/Epilogue/MEERA_EPILOGUE_03.wav` | Populated database entry; referenced by active epilogue |

`Epilogue_BookPurchase.unity` serializes `bookPrice: 18`; the existing fixed-price recording need not be regenerated for a hypothetical different price. `PLAYER_EPILOGUE_01` has no WAV or database entry; the source explicitly permits subtitle-only playback. Its one optional recording is listed in P2. `Epilogue_ModernStudyRoom` has ending UI and coin sounds, not an additional authored NPC speech sequence requiring a new bank.

## Existing Free Roam / Gate / ambience coverage

The general database has **76 populated entries**, all with GUIDs resolving to present audio files. It is referenced by the enabled Gate, Free Roam and book-purchase scenes. Serialized Narasappa/Francisco/Meera question IDs are covered, including split recordings. `NarratorUIManager.ResolveVoiceClip` checks both `_02` and `._02` suffixes for later pages; these naming forms are not automatically broken mappings. Empty Meera opening/closing fields do not request recordings. Existing market calls/ambience are not counted as dialogue-generation gaps. Legacy transaction/coin WAVs and MP3 duplicates are not extra generation needs.

## Unresolved assemblies — not extra invented recordings

The existing plan flags 266 placeholder occurrences (not 266 extra WAVs): 204 `{quantityLabel}` metric/approximation suffixes, 32 `{offeredPrice}` ranges potentially above 999, and 30 `{ruleReply}` expansions. The static pieces below preserve source-exact boundaries, but cannot pronounce these unresolved portions by themselves. Six `Unknown` templates consist entirely of `{ruleReply}` and yield no static recording. `RuleBasedNPCBrain`, ChatManager recovery messages and generic fallbacks can also generate text outside the six tables; their full price/quantity-dependent surface is not finitely covered by these sheets. They need a later approved playback/fallback policy, not a guessed recording script. No new composition/audio infrastructure is introduced by this checklist.

Do not record literal braces/placeholders. Do not silently pronounce or omit metric suffixes without approval. Do not substitute sample prices and claim coverage for all prices. Do not reuse another NPC’s number files. The queue preserves the existing per-occurrence filename scheme; source order changes can move positional IDs. The source-to-ID/text association should remain frozen through generation.

## P0 generation checklist — all essential missing planned files

Every row below: status **MISSING PLANNED FILE**; existing WAV **none found in Unity Assets**; Unity mapping **NO**. Voice codes resolve to the exact speaker/style references above. `Context` ties each static fragment to its full authoritative CSV sentence and mood/patience. These are future-bank dependencies, not drop-in fixed-scene fixes. All require an adapter; additional holds are named explicitly.

| Speaker / voice | Planned output file | Exact words to record | Existing WAV | Status | Unity audio mapping | Context / delivery | Assembly hold |
|---|---|---|---|---|---|---|---|
| Lakshmamma / LAK | `lak_001_s01.wav` | Good day, merchant. I need | None | Missing | No | L1_LAK_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_001_s02.wav` | of | None | Missing | No | L1_LAK_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_001_s03.wav` | . What are you asking? | None | Missing | No | L1_LAK_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_002_s01.wav` | Namaskara. Let me buy | None | Missing | No | L1_LAK_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_002_s02.wav` | of | None | Missing | No | L1_LAK_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_002_s03.wav` | . Tell me your price. | None | Missing | No | L1_LAK_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_003_s01.wav` | Your spices caught my eye. How much for | None | Missing | No | L1_LAK_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_003_s02.wav` | of | None | Missing | No | L1_LAK_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_004_s01.wav` | Good day to you. | None | Missing | No | L1_LAK_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_004_s02.wav` | of | None | Missing | No | L1_LAK_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_004_s03.wav` | , please. What will that cost? | None | Missing | No | L1_LAK_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_005_s01.wav` | Come, let us start with a fair price. What do you ask for | None | Missing | No | L1_LAK_005 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_005_s02.wav` | of | None | Missing | No | L1_LAK_005 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_006_s01.wav` | Let us see whether we can agree. I need | None | Missing | No | L1_LAK_006 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_006_s02.wav` | of | None | Missing | No | L1_LAK_006 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_006_s03.wav` | ; name your price. | None | Missing | No | L1_LAK_006 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_007_s01.wav` | Ah, we meet again! I need | None | Missing | No | L1_LAK_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_007_s02.wav` | of | None | Missing | No | L1_LAK_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_007_s03.wav` | today. What is your price? | None | Missing | No | L1_LAK_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_008_s01.wav` | Good to see you again, merchant. How much for | None | Missing | No | L1_LAK_008 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_008_s02.wav` | of | None | Missing | No | L1_LAK_008 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_009_s01.wav` | Back at your stall again. | None | Missing | No | L1_LAK_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_009_s02.wav` | of | None | Missing | No | L1_LAK_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_009_s03.wav` | , please. What are you asking? | None | Missing | No | L1_LAK_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_010_s01.wav` | You remember me, I hope. I am buying | None | Missing | No | L1_LAK_010 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_010_s02.wav` | of | None | Missing | No | L1_LAK_010 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_010_s03.wav` | . Tell me the price. | None | Missing | No | L1_LAK_010 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_011_s01.wav` | Another visit, another bargain. What will | None | Missing | No | L1_LAK_011 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_011_s02.wav` | of | None | Missing | No | L1_LAK_011 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_011_s03.wav` | cost me? | None | Missing | No | L1_LAK_011 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_012_s01.wav` | Good day again. Let us start with your price for | None | Missing | No | L1_LAK_012 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_012_s02.wav` | of | None | Missing | No | L1_LAK_012 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_013_s01.wav` | of | None | Missing | No | L1_LAK_013 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_013_s02.wav` | , please. That is what I need. | None | Missing | No | L1_LAK_013 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_014_s01.wav` | I am buying | None | Missing | No | L1_LAK_014 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_014_s02.wav` | of | None | Missing | No | L1_LAK_014 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_014_s03.wav` | for the house. | None | Missing | No | L1_LAK_014 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_015_s01.wav` | The | None | Missing | No | L1_LAK_015 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_015_s02.wav` | , merchant. I need | None | Missing | No | L1_LAK_015 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_016_s01.wav` | Just | None | Missing | No | L1_LAK_016 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_016_s02.wav` | of | None | Missing | No | L1_LAK_016 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_016_s03.wav` | for this purchase. | None | Missing | No | L1_LAK_016 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_017_s01.wav` | It is | None | Missing | No | L1_LAK_017 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_017_s02.wav` | of | None | Missing | No | L1_LAK_017 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_017_s03.wav` | I am after. | None | Missing | No | L1_LAK_017 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_018_s01.wav` | Let us keep to | None | Missing | No | L1_LAK_018 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_018_s02.wav` | of | None | Missing | No | L1_LAK_018 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_019_s01.wav` | I need | None | Missing | No | L1_LAK_019 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_019_s02.wav` | of | None | Missing | No | L1_LAK_019 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_019_s03.wav` | . What is your price? | None | Missing | No | L1_LAK_019 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_020_s01.wav` | Tell me what you want for | None | Missing | No | L1_LAK_020 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_020_s02.wav` | of | None | Missing | No | L1_LAK_020 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_021_s01.wav` | of | None | Missing | No | L1_LAK_021 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_021_s02.wav` | , please. How much are you asking? | None | Missing | No | L1_LAK_021 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_022_s01.wav` | Let us begin with your price for | None | Missing | No | L1_LAK_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_022_s02.wav` | of | None | Missing | No | L1_LAK_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_023_s01.wav` | What would you charge me for | None | Missing | No | L1_LAK_023 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_023_s02.wav` | of | None | Missing | No | L1_LAK_023 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_024_s01.wav` | Name your price for | None | Missing | No | L1_LAK_024 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_024_s02.wav` | of | None | Missing | No | L1_LAK_024 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_024_s03.wav` | , merchant. | None | Missing | No | L1_LAK_024 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_025_s01.wav` | I am offering | None | Missing | No | L1_LAK_025 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_025_s02.wav` | for | None | Missing | No | L1_LAK_025 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_025_s03.wav` | of | None | Missing | No | L1_LAK_025 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_026_s01.wav` | For | None | Missing | No | L1_LAK_026 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_026_s02.wav` | of | None | Missing | No | L1_LAK_026 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_026_s03.wav` | , my offer is | None | Missing | No | L1_LAK_026 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_027_s01.wav` | for the | None | Missing | No | L1_LAK_027 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_027_s02.wav` | of | None | Missing | No | L1_LAK_027 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_027_s03.wav` | . Does that suit you? | None | Missing | No | L1_LAK_027 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_028_s01.wav` | Here is my offer: | None | Missing | No | L1_LAK_028 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_028_s02.wav` | for | None | Missing | No | L1_LAK_028 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_028_s03.wav` | of | None | Missing | No | L1_LAK_028 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_029_s01.wav` | I can offer | None | Missing | No | L1_LAK_029 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_029_s02.wav` | for | None | Missing | No | L1_LAK_029 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_029_s03.wav` | of | None | Missing | No | L1_LAK_029 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_029_s04.wav` | . What do you say? | None | Missing | No | L1_LAK_029 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_030_s01.wav` | The offer stands at | None | Missing | No | L1_LAK_030 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_030_s02.wav` | for | None | Missing | No | L1_LAK_030 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_030_s03.wav` | of | None | Missing | No | L1_LAK_030 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_031_s01.wav` | of | None | Missing | No | L1_LAK_031 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_031_s02.wav` | . That is the amount I need. | None | Missing | No | L1_LAK_031 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_032_s01.wav` | I need | None | Missing | No | L1_LAK_032 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_032_s02.wav` | of | None | Missing | No | L1_LAK_032 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_032_s03.wav` | , merchant. | None | Missing | No | L1_LAK_032 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_033_s01.wav` | For this purchase, | None | Missing | No | L1_LAK_033 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_033_s02.wav` | of | None | Missing | No | L1_LAK_033 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_034_s01.wav` | Let us keep the measure clear: | None | Missing | No | L1_LAK_034 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_034_s02.wav` | of | None | Missing | No | L1_LAK_034 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_035_s01.wav` | The quantity is | None | Missing | No | L1_LAK_035 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_035_s02.wav` | of | None | Missing | No | L1_LAK_035 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_036_s01.wav` | , please, of the | None | Missing | No | L1_LAK_036 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_037_s01.wav` | Sari, | None | Missing | No | L1_LAK_037 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_037_s02.wav` | of | None | Missing | No | L1_LAK_037 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_037_s03.wav` | , then. My offer is | None | Missing | No | L1_LAK_037 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_038_s01.wav` | We are discussing | None | Missing | No | L1_LAK_038 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_038_s02.wav` | of | None | Missing | No | L1_LAK_038 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_038_s03.wav` | now, at my offer of | None | Missing | No | L1_LAK_038 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_039_s01.wav` | of | None | Missing | No | L1_LAK_039 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_039_s02.wav` | , understood. I am offering | None | Missing | No | L1_LAK_039 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_040_s01.wav` | Let us be clear: | None | Missing | No | L1_LAK_040 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_040_s02.wav` | of | None | Missing | No | L1_LAK_040 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_040_s03.wav` | . My offer stands at | None | Missing | No | L1_LAK_040 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_041_s01.wav` | Ayyo, | None | Missing | No | L1_LAK_041 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_041_s02.wav` | ? That is too much. My offer remains | None | Missing | No | L1_LAK_041 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_042_s01.wav` | At | None | Missing | No | L1_LAK_042 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_042_s02.wav` | , I must say no. I am still offering | None | Missing | No | L1_LAK_042 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_043_s01.wav` | Good spice, yes; but at | None | Missing | No | L1_LAK_043 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_043_s02.wav` | , you will empty my purse. I am offering | None | Missing | No | L1_LAK_043 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_044_s01.wav` | You ask | None | Missing | No | L1_LAK_044 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_044_s02.wav` | ; I am holding at | None | Missing | No | L1_LAK_044 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_044_s03.wav` | . We are too far apart. | None | Missing | No | L1_LAK_044 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_045_s01.wav` | That price would leave little for my other buying. My offer stays at | None | Missing | No | L1_LAK_045 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_046_s01.wav` | I cannot agree to | None | Missing | No | L1_LAK_046 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_046_s02.wav` | is my offer. | None | Missing | No | L1_LAK_046 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_047_s01.wav` | We are close, but I am still at | None | Missing | No | L1_LAK_047 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_048_s01.wav` | Swalpa less, merchant. My offer remains | None | Missing | No | L1_LAK_048 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_049_s01.wav` | Only a little separates us. Would you take my | None | Missing | No | L1_LAK_049 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_050_s01.wav` | I hear your price. I am holding at | None | Missing | No | L1_LAK_050 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_051_s01.wav` | You have nearly persuaded me, but my offer is still | None | Missing | No | L1_LAK_051 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_052_s01.wav` | A small difference is still a difference. I am offering | None | Missing | No | L1_LAK_052 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_053_s01.wav` | That is unexpectedly low. Is that for the whole purchase? My offer remains | None | Missing | No | L1_LAK_053 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_054_s01.wav` | Ayyo, that little? Let us make sure we mean the same purchase. My offer is still | None | Missing | No | L1_LAK_054 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_055_s01.wav` | I like a bargain, but that price makes me pause. I am holding at | None | Missing | No | L1_LAK_055 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_056_s01.wav` | You ask so little that I wonder whether we have misunderstood each other. My offer remains | None | Missing | No | L1_LAK_056 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_057_s01.wav` | That price does not sound right for this purchase. I am still offering | None | Missing | No | L1_LAK_057 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_058_s01.wav` | Are you sure of that figure, merchant? Such a low price makes me wonder. My offer stands at | None | Missing | No | L1_LAK_058 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_059_s01.wav` | I can offer | None | Missing | No | L1_LAK_059 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_059_s02.wav` | for | None | Missing | No | L1_LAK_059 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_059_s03.wav` | of | None | Missing | No | L1_LAK_059 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_059_s04.wav` | . What do you say? | None | Missing | No | L1_LAK_059 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_060_s01.wav` | Let me offer | None | Missing | No | L1_LAK_060 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_060_s02.wav` | . Will that do for | None | Missing | No | L1_LAK_060 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_060_s03.wav` | of | None | Missing | No | L1_LAK_060 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_061_s01.wav` | Your price is more than I had in mind. I can offer | None | Missing | No | L1_LAK_061 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_062_s01.wav` | Shall we try | None | Missing | No | L1_LAK_062 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_062_s02.wav` | for this purchase? | None | Missing | No | L1_LAK_062 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_063_s01.wav` | I will come up to | None | Missing | No | L1_LAK_063 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_063_s02.wav` | . Can we agree there? | None | Missing | No | L1_LAK_063 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_064_s01.wav` | Let us meet a little closer. I can offer | None | Missing | No | L1_LAK_064 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_065_s01.wav` | Sari, I can come up to | None | Missing | No | L1_LAK_065 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_065_s02.wav` | . What do you say? | None | Missing | No | L1_LAK_065 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_066_s01.wav` | You have moved me a little. I can offer | None | Missing | No | L1_LAK_066 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_066_s02.wav` | now. | None | Missing | No | L1_LAK_066 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_067_s01.wav` | Let us try | None | Missing | No | L1_LAK_067 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_067_s02.wav` | . Does that bring us together? | None | Missing | No | L1_LAK_067 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_068_s01.wav` | I can raise my offer to | None | Missing | No | L1_LAK_068 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_068_s02.wav` | . Can you meet me there? | None | Missing | No | L1_LAK_068 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_069_s01.wav` | Swalpa more from me: | None | Missing | No | L1_LAK_069 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_070_s01.wav` | I have listened to you. My offer is now | None | Missing | No | L1_LAK_070 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_071_s01.wav` | We have talked this over. I can offer | None | Missing | No | L1_LAK_071 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_071_s02.wav` | now. | None | Missing | No | L1_LAK_071 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_072_s01.wav` | Let us see where we stand: my offer is now | None | Missing | No | L1_LAK_072 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_073_s01.wav` | I can come up to | None | Missing | No | L1_LAK_073 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_073_s02.wav` | . Will that settle the price? | None | Missing | No | L1_LAK_073 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_074_s01.wav` | After all that bargaining, I am offering | None | Missing | No | L1_LAK_074 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_075_s01.wav` | Here is where I stand now: | None | Missing | No | L1_LAK_075 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_076_s01.wav` | I have raised my offer to | None | Missing | No | L1_LAK_076 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_076_s02.wav` | . What is your answer? | None | Missing | No | L1_LAK_076 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_077_s01.wav` | My offer remains | None | Missing | No | L1_LAK_077 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_077_s02.wav` | , merchant. | None | Missing | No | L1_LAK_077 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_078_s01.wav` | I have heard you, but I am holding at | None | Missing | No | L1_LAK_078 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_079_s01.wav` | A friendly smile will not change my arithmetic. | None | Missing | No | L1_LAK_079 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_079_s02.wav` | is still my offer. | None | Missing | No | L1_LAK_079 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_080_s01.wav` | I am not raising the offer this time. It stands at | None | Missing | No | L1_LAK_080 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_081_s01.wav` | Let us keep our feet on the ground. I am offering | None | Missing | No | L1_LAK_081 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_082_s01.wav` | You bargain well, but I am still at | None | Missing | No | L1_LAK_082 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_083_s01.wav` | Sari, agreed at | None | Missing | No | L1_LAK_083 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_083_s02.wav` | . Please bring the | None | Missing | No | L1_LAK_083 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_083_s03.wav` | of | None | Missing | No | L1_LAK_083 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_084_s01.wav` | That price suits me: | None | Missing | No | L1_LAK_084 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_084_s02.wav` | . Now bring the | None | Missing | No | L1_LAK_084 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_084_s03.wav` | of | None | Missing | No | L1_LAK_084 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_084_s04.wav` | , please. | None | Missing | No | L1_LAK_084 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_085_s01.wav` | Yes, | None | Missing | No | L1_LAK_085 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_085_s02.wav` | for | None | Missing | No | L1_LAK_085 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_085_s03.wav` | of | None | Missing | No | L1_LAK_085 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_085_s04.wav` | . Please bring it over. | None | Missing | No | L1_LAK_085 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_086_s01.wav` | We agree on | None | Missing | No | L1_LAK_086 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_086_s02.wav` | . I am ready for the | None | Missing | No | L1_LAK_086 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_086_s03.wav` | of | None | Missing | No | L1_LAK_086 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_087_s01.wav` | Good, the price is agreed at | None | Missing | No | L1_LAK_087 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_087_s02.wav` | . Bring the | None | Missing | No | L1_LAK_087 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_087_s03.wav` | of | None | Missing | No | L1_LAK_087 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_087_s04.wav` | , please. | None | Missing | No | L1_LAK_087 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_088_s01.wav` | , agreed. Let me have the | None | Missing | No | L1_LAK_088 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_088_s02.wav` | of | None | Missing | No | L1_LAK_088 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_088_s03.wav` | , please. | None | Missing | No | L1_LAK_088 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_089_s01.wav` | Sari, | None | Missing | No | L1_LAK_089 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_089_s02.wav` | it is. Please bring the | None | Missing | No | L1_LAK_089 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_089_s03.wav` | of | None | Missing | No | L1_LAK_089 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_090_s01.wav` | We have our price: | None | Missing | No | L1_LAK_090 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_090_s02.wav` | . Now for the | None | Missing | No | L1_LAK_090 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_090_s03.wav` | of | None | Missing | No | L1_LAK_090 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_091_s01.wav` | Agreed at | None | Missing | No | L1_LAK_091 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_091_s02.wav` | . Bring the | None | Missing | No | L1_LAK_091 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_091_s03.wav` | of | None | Missing | No | L1_LAK_091 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_091_s04.wav` | , merchant. | None | Missing | No | L1_LAK_091 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_092_s01.wav` | Good, we understand each other. | None | Missing | No | L1_LAK_092 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_092_s02.wav` | ; please bring the | None | Missing | No | L1_LAK_092 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_092_s03.wav` | of | None | Missing | No | L1_LAK_092 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_093_s01.wav` | Then we agree: | None | Missing | No | L1_LAK_093 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_093_s02.wav` | of | None | Missing | No | L1_LAK_093 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_093_s03.wav` | for | None | Missing | No | L1_LAK_093 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_093_s04.wav` | . Please bring it over. | None | Missing | No | L1_LAK_093 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_094_s01.wav` | A fair bargain at | None | Missing | No | L1_LAK_094 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_094_s02.wav` | . I am waiting for the | None | Missing | No | L1_LAK_094 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_094_s03.wav` | of | None | Missing | No | L1_LAK_094 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_095_s01.wav` | My offer does not suit you, then. What price do you want? | None | Missing | No | L1_LAK_095 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_096_s01.wav` | Sari, tell me what you are asking instead. | None | Missing | No | L1_LAK_096 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_097_s01.wav` | You want more than my offer. Name your price, merchant. | None | Missing | No | L1_LAK_097 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_098_s01.wav` | We have not agreed, then. What would you ask? | None | Missing | No | L1_LAK_098 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_099_s01.wav` | Tell me your price so we have something to discuss. | None | Missing | No | L1_LAK_099 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_100_s01.wav` | What price would you put to me instead? | None | Missing | No | L1_LAK_100 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_101_s01.wav` | I have other provisions to buy as well. My offer is | None | Missing | No | L1_LAK_101 / OfferReason; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_101_s02.wav` | for | None | Missing | No | L1_LAK_101 / OfferReason; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_101_s03.wav` | of | None | Missing | No | L1_LAK_101 / OfferReason; Any mood / Any patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_102_s01.wav` | Good spice matters, but so does what is left in my purse. I am offering | None | Missing | No | L1_LAK_102 / OfferReason; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_103_s01.wav` | I am buying for a household, merchant. | None | Missing | No | L1_LAK_103 / OfferReason; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_103_s02.wav` | is the offer I am making. | None | Missing | No | L1_LAK_103 / OfferReason; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_104_s01.wav` | I must make my money serve the whole house. My offer stands at | None | Missing | No | L1_LAK_104 / OfferReason; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_135_s01.wav` | Ayyo, you have taken us far from my buying. Let us come back to the spice. | None | Missing | No | L1_LAK_135 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_136_s01.wav` | I do not follow that, merchant. Let us keep to this purchase. | None | Missing | No | L1_LAK_136 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_137_s01.wav` | That is beyond me. Can we return to the | None | Missing | No | L1_LAK_137 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_138_s01.wav` | You will have to ask someone else about that. I came to buy spice. | None | Missing | No | L1_LAK_138 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_139_s01.wav` | Let us not lose the thread. We were discussing my purchase. | None | Missing | No | L1_LAK_139 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_140_s01.wav` | One thing at a time, merchant. Let us finish the buying. | None | Missing | No | L1_LAK_140 / OffTopic; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_141_s01.wav` | Ayyo, there is no need to speak to me like that. Let us bargain with respect. | None | Missing | No | L1_LAK_141 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_142_s01.wav` | A hard bargain is one thing; unkind words are another. Mind how you speak to me. | None | Missing | No | L1_LAK_142 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_143_s01.wav` | I came to buy spice, not to be insulted. Speak civilly, merchant. | None | Missing | No | L1_LAK_143 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_144_s01.wav` | We can disagree over a price without speaking harshly. Shall we try that? | None | Missing | No | L1_LAK_144 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_145_s01.wav` | I have treated you with courtesy. I expect the same from you. | None | Missing | No | L1_LAK_145 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_146_s01.wav` | Enough of that tone. Let us return to the purchase, with respect. | None | Missing | No | L1_LAK_146 / HostileWarning; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_147_s01.wav` | Let us make sure we understand each other. | None | Missing | No | L1_LAK_147 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Lakshmamma / LAK | `lak_148_s01.wav` | One moment, merchant. | None | Missing | No | L1_LAK_148 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Lakshmamma / LAK | `lak_149_s01.wav` | Sari, let us be clear. | None | Missing | No | L1_LAK_149 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Lakshmamma / LAK | `lak_150_s01.wav` | Let us straighten that out. | None | Missing | No | L1_LAK_150 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Lakshmamma / LAK | `lak_152_s01.wav` | Thank you, merchant. This will be welcome at home. | None | Missing | No | L1_LAK_152 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_153_s01.wav` | There we are. Good buying, and no need for a quarrel! | None | Missing | No | L1_LAK_153 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_154_s01.wav` | Sari, I have my spice. My thanks to you. | None | Missing | No | L1_LAK_154 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_155_s01.wav` | Thank you. I am pleased with this purchase. | None | Missing | No | L1_LAK_155 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_156_s01.wav` | That is my buying done. May your stall do well today. | None | Missing | No | L1_LAK_156 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_157_s01.wav` | Good. The household will make use of this. Until next time, merchant. | None | Missing | No | L1_LAK_157 / TransactionSuccess; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_158_s01.wav` | We have not agreed. I will leave it there, merchant. | None | Missing | No | L1_LAK_158 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_159_s01.wav` | No purchase this time. I wish you a good day. | None | Missing | No | L1_LAK_159 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_160_s01.wav` | A pity we could not agree. I will look elsewhere. | None | Missing | No | L1_LAK_160 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_161_s01.wav` | Then I shall go without buying from you today. | None | Missing | No | L1_LAK_161 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_162_s01.wav` | Sari, let us part without a bargain. | None | Missing | No | L1_LAK_162 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_163_s01.wav` | We are too far apart today. Another time, perhaps. | None | Missing | No | L1_LAK_163 / TransactionFailure; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_168_s01.wav` | Merchant? I am still here. What do you say? | None | Missing | No | L1_LAK_168 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_169_s01.wav` | Take a moment, but do give me your answer. | None | Missing | No | L1_LAK_169 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_170_s01.wav` | Have you had time to think it over? | None | Missing | No | L1_LAK_170 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_171_s01.wav` | Shall we carry on with the buying? | None | Missing | No | L1_LAK_171 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_172_s01.wav` | I cannot wait much longer, merchant. Please give me an answer. | None | Missing | No | L1_LAK_172 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_173_s01.wav` | Ayyo, my other errands are waiting. Shall we settle this or leave it? | None | Missing | No | L1_LAK_173 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_174_s01.wav` | I need to move on soon. What do you say? | None | Missing | No | L1_LAK_174 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_175_s01.wav` | This is taking some time. Please tell me where you stand. | None | Missing | No | L1_LAK_175 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_176_s01.wav` | I have heard you, merchant. My offer is still | None | Missing | No | L1_LAK_176 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_176_s02.wav` | . Please give me a clear answer. | None | Missing | No | L1_LAK_176 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_177_s01.wav` | Ayyo, let us come to an answer. I remain at | None | Missing | No | L1_LAK_177 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_178_s01.wav` | My time is short now. | None | Missing | No | L1_LAK_178 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_178_s02.wav` | is my offer. | None | Missing | No | L1_LAK_178 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_179_s01.wav` | I am still offering | None | Missing | No | L1_LAK_179 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_179_s02.wav` | . Please tell me where you stand. | None | Missing | No | L1_LAK_179 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_180_s01.wav` | , merchant. I cannot spend much longer on this purchase. | None | Missing | No | L1_LAK_180 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_181_s01.wav` | Let us keep the figures clear. I am holding at | None | Missing | No | L1_LAK_181 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_182_s01.wav` | I respect your asking price, but my offer remains | None | Missing | No | L1_LAK_182 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_183_s01.wav` | I am not persuaded to raise it. My offer is | None | Missing | No | L1_LAK_183 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_184_s01.wav` | You know your price, and I know my purse. I remain at | None | Missing | No | L1_LAK_184 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_185_s01.wav` | No change from me this time: | None | Missing | No | L1_LAK_185 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_186_s01.wav` | Sari, my offer is still | None | Missing | No | L1_LAK_186 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_186_s02.wav` | . What do you say? | None | Missing | No | L1_LAK_186 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_187_s01.wav` | Let us keep this simple. I am holding at | None | Missing | No | L1_LAK_187 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_188_s01.wav` | remains my offer. Give me your answer, merchant. | None | Missing | No | L1_LAK_188 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_189_s01.wav` | I am not raising it this time. Shall we settle at | None | Missing | No | L1_LAK_189 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_189_s02.wav` | , or leave it? | None | Missing | No | L1_LAK_189 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_190_s01.wav` | We have our figures. Mine is still | None | Missing | No | L1_LAK_190 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_191_s01.wav` | Ayyo, | None | Missing | No | L1_LAK_191 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_191_s02.wav` | is too much, and I have little time left. My offer stays at | None | Missing | No | L1_LAK_191 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_192_s01.wav` | No, not | None | Missing | No | L1_LAK_192 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_192_s02.wav` | . I am holding at | None | Missing | No | L1_LAK_192 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_192_s03.wav` | . Please give me an answer. | None | Missing | No | L1_LAK_192 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Lakshmamma / LAK | `lak_193_s01.wav` | That price is still too high. My offer is | None | Missing | No | L1_LAK_193 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_193_s02.wav` | , and I must move on soon. | None | Missing | No | L1_LAK_193 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_194_s01.wav` | Sari, agreed at | None | Missing | No | L1_LAK_194 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_194_s02.wav` | . Please bring the | None | Missing | No | L1_LAK_194 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_194_s03.wav` | of | None | Missing | No | L1_LAK_194 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_194_s04.wav` | ; I must be on my way soon. | None | Missing | No | L1_LAK_194 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_195_s01.wav` | Yes, | None | Missing | No | L1_LAK_195 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_195_s02.wav` | . Bring the | None | Missing | No | L1_LAK_195 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_195_s03.wav` | of | None | Missing | No | L1_LAK_195 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_195_s04.wav` | , please, and let us finish. | None | Missing | No | L1_LAK_195 / SellerPriceAccepted; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_196_s01.wav` | We agree at | None | Missing | No | L1_LAK_196 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_196_s02.wav` | , then. Please bring the | None | Missing | No | L1_LAK_196 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_196_s03.wav` | of | None | Missing | No | L1_LAK_196 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_196_s04.wav` | now. | None | Missing | No | L1_LAK_196 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_197_s01.wav` | Sari, the price is settled at | None | Missing | No | L1_LAK_197 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_197_s02.wav` | . I still need the | None | Missing | No | L1_LAK_197 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_197_s03.wav` | of | None | Missing | No | L1_LAK_197 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_197_s04.wav` | ; please bring it over. | None | Missing | No | L1_LAK_197 / PlayerAcceptedDeal; Any mood / Low patience | quantity suffix undecided |
| Lakshmamma / LAK | `lak_201_s01.wav` | I have waited long enough, merchant. I must go. | None | Missing | No | L1_LAK_201 / TransactionFailure; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_202_s01.wav` | Ayyo, no bargain and my errands still waiting. I shall leave it here. | None | Missing | No | L1_LAK_202 / TransactionFailure; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_203_s01.wav` | We have no agreement, and I have no more time to spare. Good day. | None | Missing | No | L1_LAK_203 / TransactionFailure; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_204_s01.wav` | I can raise it to | None | Missing | No | L1_LAK_204 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_204_s02.wav` | , but please answer soon. | None | Missing | No | L1_LAK_204 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_205_s01.wav` | , then. I have little time left to bargain. | None | Missing | No | L1_LAK_205 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_206_s01.wav` | My offer is now | None | Missing | No | L1_LAK_206 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_206_s02.wav` | . Let us come to a decision. | None | Missing | No | L1_LAK_206 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_001_s01.wav` | Good day. Chinnamma Naik. I am buying | None | Missing | No | L1_CHI_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_001_s02.wav` | of | None | Missing | No | L1_CHI_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_001_s03.wav` | ; what is your price? | None | Missing | No | L1_CHI_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_002_s01.wav` | Those spices brought me over. What do you ask for | None | Missing | No | L1_CHI_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_002_s02.wav` | of | None | Missing | No | L1_CHI_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_003_s01.wav` | Merchant, I need | None | Missing | No | L1_CHI_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_003_s02.wav` | of | None | Missing | No | L1_CHI_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_003_s03.wav` | . Give me a price worth discussing. | None | Missing | No | L1_CHI_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_004_s01.wav` | Good day to you. Let us see about | None | Missing | No | L1_CHI_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_004_s02.wav` | of | None | Missing | No | L1_CHI_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_004_s03.wav` | . What are you asking? | None | Missing | No | L1_CHI_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_005_s01.wav` | I know what I need: | None | Missing | No | L1_CHI_005 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_005_s02.wav` | of | None | Missing | No | L1_CHI_005 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_005_s03.wav` | . Now, your price? | None | Missing | No | L1_CHI_005 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_006_s01.wav` | We have spoken before. Today I need | None | Missing | No | L1_CHI_006 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_006_s02.wav` | of | None | Missing | No | L1_CHI_006 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_006_s03.wav` | . What is your price? | None | Missing | No | L1_CHI_006 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_007_s01.wav` | Back at your stall, merchant. How much for | None | Missing | No | L1_CHI_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_007_s02.wav` | of | None | Missing | No | L1_CHI_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_008_s01.wav` | You may remember me: Chinnamma Naik. I am asking about | None | Missing | No | L1_CHI_008 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_008_s02.wav` | of | None | Missing | No | L1_CHI_008 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_008_s03.wav` | today. | None | Missing | No | L1_CHI_008 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_009_s01.wav` | Here we are again. A fresh purchase: | None | Missing | No | L1_CHI_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_009_s02.wav` | of | None | Missing | No | L1_CHI_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_009_s03.wav` | . Name your price. | None | Missing | No | L1_CHI_009 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_010_s01.wav` | Good day again. Let us begin with your price for | None | Missing | No | L1_CHI_010 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_010_s02.wav` | of | None | Missing | No | L1_CHI_010 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_011_s01.wav` | The | None | Missing | No | L1_CHI_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_011_s02.wav` | . I need | None | Missing | No | L1_CHI_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_011_s03.wav` | for this purchase. | None | Missing | No | L1_CHI_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_012_s01.wav` | I am looking for | None | Missing | No | L1_CHI_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_012_s02.wav` | of | None | Missing | No | L1_CHI_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_012_s03.wav` | , in full measure. | None | Missing | No | L1_CHI_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_013_s01.wav` | of | None | Missing | No | L1_CHI_013 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_013_s02.wav` | . Keep that quantity in mind when you name your price. | None | Missing | No | L1_CHI_013 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_014_s01.wav` | My buying today is | None | Missing | No | L1_CHI_014 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_014_s02.wav` | of | None | Missing | No | L1_CHI_014 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_015_s01.wav` | What do you ask for | None | Missing | No | L1_CHI_015 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_015_s02.wav` | of | None | Missing | No | L1_CHI_015 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_016_s01.wav` | You know the quantity: | None | Missing | No | L1_CHI_016 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_016_s02.wav` | of | None | Missing | No | L1_CHI_016 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_016_s03.wav` | . Tell me your price. | None | Missing | No | L1_CHI_016 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_017_s01.wav` | Let us put a figure to it. Your price for | None | Missing | No | L1_CHI_017 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_017_s02.wav` | of | None | Missing | No | L1_CHI_017 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_018_s01.wav` | I have named the goods. Name your price for | None | Missing | No | L1_CHI_018 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_018_s02.wav` | of | None | Missing | No | L1_CHI_018 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_019_s01.wav` | My offer is | None | Missing | No | L1_CHI_019 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_019_s02.wav` | for | None | Missing | No | L1_CHI_019 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_019_s03.wav` | of | None | Missing | No | L1_CHI_019 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_020_s01.wav` | For | None | Missing | No | L1_CHI_020 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_020_s02.wav` | of | None | Missing | No | L1_CHI_020 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_020_s03.wav` | , I am offering | None | Missing | No | L1_CHI_020 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_021_s01.wav` | for the | None | Missing | No | L1_CHI_021 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_021_s02.wav` | of | None | Missing | No | L1_CHI_021 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_021_s03.wav` | . That is the offer before you. | None | Missing | No | L1_CHI_021 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_022_s01.wav` | We stand at my offer of | None | Missing | No | L1_CHI_022 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_022_s02.wav` | for | None | Missing | No | L1_CHI_022 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_022_s03.wav` | of | None | Missing | No | L1_CHI_022 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_023_s01.wav` | of | None | Missing | No | L1_CHI_023 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_023_s02.wav` | . Neither more nor less for this purchase. | None | Missing | No | L1_CHI_023 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_024_s01.wav` | Put the measure at | None | Missing | No | L1_CHI_024 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_024_s02.wav` | of | None | Missing | No | L1_CHI_024 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_024_s03.wav` | for this purchase. | None | Missing | No | L1_CHI_024 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_025_s01.wav` | Measure out | None | Missing | No | L1_CHI_025 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_025_s02.wav` | of | None | Missing | No | L1_CHI_025 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_025_s03.wav` | ; that is the amount we are discussing. | None | Missing | No | L1_CHI_025 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_026_s01.wav` | I need | None | Missing | No | L1_CHI_026 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_026_s02.wav` | of | None | Missing | No | L1_CHI_026 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_026_s03.wav` | . A clear measure saves an argument. | None | Missing | No | L1_CHI_026 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_027_s01.wav` | Then we are discussing | None | Missing | No | L1_CHI_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_027_s02.wav` | of | None | Missing | No | L1_CHI_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_027_s03.wav` | . My offer is | None | Missing | No | L1_CHI_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_028_s01.wav` | The quantity is now | None | Missing | No | L1_CHI_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_028_s02.wav` | of | None | Missing | No | L1_CHI_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_028_s03.wav` | . I am offering | None | Missing | No | L1_CHI_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_029_s01.wav` | of | None | Missing | No | L1_CHI_029 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_029_s02.wav` | , understood. My offer remains | None | Missing | No | L1_CHI_029 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_030_s01.wav` | Let us keep the terms straight: | None | Missing | No | L1_CHI_030 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_030_s02.wav` | of | None | Missing | No | L1_CHI_030 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_030_s03.wav` | , at my offer of | None | Missing | No | L1_CHI_030 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_031_s01.wav` | Beda, not at | None | Missing | No | L1_CHI_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_031_s02.wav` | . My offer stays at | None | Missing | No | L1_CHI_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_032_s01.wav` | At | None | Missing | No | L1_CHI_032 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_032_s02.wav` | , you are asking a great deal of my purse. I remain at | None | Missing | No | L1_CHI_032 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_033_s01.wav` | The spice has weight; that price has ambition. My offer is still | None | Missing | No | L1_CHI_033 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_034_s01.wav` | I will not agree to | None | Missing | No | L1_CHI_034 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_034_s02.wav` | . I am offering | None | Missing | No | L1_CHI_034 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_035_s01.wav` | Your price is still above my offer. I remain at | None | Missing | No | L1_CHI_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_036_s01.wav` | That difference is not settled yet. My offer remains | None | Missing | No | L1_CHI_036 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_037_s01.wav` | Would you take my | None | Missing | No | L1_CHI_037 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_037_s02.wav` | instead? | None | Missing | No | L1_CHI_037 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_038_s01.wav` | Come toward my offer, merchant. I am holding at | None | Missing | No | L1_CHI_038 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_039_s01.wav` | That price is too low to pass without a question. My offer remains | None | Missing | No | L1_CHI_039 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_040_s01.wav` | Do you mean that for the full lot? Such a low figure makes me wonder. My offer is still | None | Missing | No | L1_CHI_040 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_041_s01.wav` | Cheap enough to make me cautious. I am holding at | None | Missing | No | L1_CHI_041 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_042_s01.wav` | Something does not add up at that price. My offer stays at | None | Missing | No | L1_CHI_042 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_043_s01.wav` | I can raise my offer to | None | Missing | No | L1_CHI_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_043_s02.wav` | . What is your answer? | None | Missing | No | L1_CHI_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_044_s01.wav` | Let us try | None | Missing | No | L1_CHI_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_044_s02.wav` | for | None | Missing | No | L1_CHI_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_044_s03.wav` | of | None | Missing | No | L1_CHI_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_045_s01.wav` | I have considered your price. My offer is now | None | Missing | No | L1_CHI_045 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_046_s01.wav` | Here is my answer: | None | Missing | No | L1_CHI_046 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_046_s02.wav` | for this purchase. | None | Missing | No | L1_CHI_046 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_047_s01.wav` | I will come up to | None | Missing | No | L1_CHI_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_047_s02.wav` | . Can you meet me there? | None | Missing | No | L1_CHI_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_048_s01.wav` | You have made your case. I am raising my offer to | None | Missing | No | L1_CHI_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_049_s01.wav` | My offer is now | None | Missing | No | L1_CHI_049 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_049_s02.wav` | . That narrows the gap. | None | Missing | No | L1_CHI_049 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_050_s01.wav` | I can move to | None | Missing | No | L1_CHI_050 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_050_s02.wav` | . Your turn to answer. | None | Missing | No | L1_CHI_050 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_051_s01.wav` | After that discussion, my offer is now | None | Missing | No | L1_CHI_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_052_s01.wav` | I can raise it to | None | Missing | No | L1_CHI_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_052_s02.wav` | . Where does that leave us? | None | Missing | No | L1_CHI_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_053_s01.wav` | We have worked through the figures. I am offering | None | Missing | No | L1_CHI_053 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_053_s02.wav` | now. | None | Missing | No | L1_CHI_053 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_054_s01.wav` | Here is my revised offer: | None | Missing | No | L1_CHI_054 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_054_s02.wav` | . What do you say? | None | Missing | No | L1_CHI_054 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_055_s01.wav` | Still | None | Missing | No | L1_CHI_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_055_s02.wav` | , merchant. I have heard your argument. | None | Missing | No | L1_CHI_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_056_s01.wav` | My offer remains | None | Missing | No | L1_CHI_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_056_s02.wav` | . Repeating the price will not improve the case. | None | Missing | No | L1_CHI_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_057_s01.wav` | I am holding at | None | Missing | No | L1_CHI_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_057_s02.wav` | . What is your answer? | None | Missing | No | L1_CHI_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_058_s01.wav` | I have not changed my figure: | None | Missing | No | L1_CHI_058 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_059_s01.wav` | I know what I am offering. It is still | None | Missing | No | L1_CHI_059 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_060_s01.wav` | Howdu, agreed at | None | Missing | No | L1_CHI_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_060_s02.wav` | . Bring the | None | Missing | No | L1_CHI_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_060_s03.wav` | of | None | Missing | No | L1_CHI_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_060_s04.wav` | , please. | None | Missing | No | L1_CHI_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_061_s01.wav` | That price works: | None | Missing | No | L1_CHI_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_061_s02.wav` | . Now bring the | None | Missing | No | L1_CHI_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_061_s03.wav` | of | None | Missing | No | L1_CHI_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_062_s01.wav` | We agree on | None | Missing | No | L1_CHI_062 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_062_s02.wav` | . I am waiting for the | None | Missing | No | L1_CHI_062 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_062_s03.wav` | of | None | Missing | No | L1_CHI_062 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_063_s01.wav` | , agreed. Please bring over the | None | Missing | No | L1_CHI_063 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_063_s02.wav` | of | None | Missing | No | L1_CHI_063 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_064_s01.wav` | Then the price is agreed: | None | Missing | No | L1_CHI_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_064_s02.wav` | . Bring the | None | Missing | No | L1_CHI_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_064_s03.wav` | of | None | Missing | No | L1_CHI_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_065_s01.wav` | We have our terms. | None | Missing | No | L1_CHI_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_065_s02.wav` | for | None | Missing | No | L1_CHI_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_065_s03.wav` | of | None | Missing | No | L1_CHI_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_065_s04.wav` | ; bring it over, please. | None | Missing | No | L1_CHI_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_066_s01.wav` | Agreed at | None | Missing | No | L1_CHI_066 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_066_s02.wav` | . Now I need the | None | Missing | No | L1_CHI_066 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_066_s03.wav` | of | None | Missing | No | L1_CHI_066 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_067_s01.wav` | Good. | None | Missing | No | L1_CHI_067 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_067_s02.wav` | , and the | None | Missing | No | L1_CHI_067 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_067_s03.wav` | of | None | Missing | No | L1_CHI_067 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_067_s04.wav` | still to be brought over. | None | Missing | No | L1_CHI_067 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Chinnamma Naik / CHI | `chi_068_s01.wav` | You decline that offer. What price are you asking instead? | None | Missing | No | L1_CHI_068 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_069_s01.wav` | Then put another figure before me. What do you want? | None | Missing | No | L1_CHI_069 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_070_s01.wav` | Not enough for you? Name your price. | None | Missing | No | L1_CHI_070 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_071_s01.wav` | We have not agreed. Tell me what you would ask. | None | Missing | No | L1_CHI_071 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_072_s01.wav` | I buy for onward trade. My offer of | None | Missing | No | L1_CHI_072 / OfferReason; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_072_s02.wav` | must leave room for the next sale. | None | Missing | No | L1_CHI_072 / OfferReason; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_073_s01.wav` | The spice must earn its keep after I carry it away. I am offering | None | Missing | No | L1_CHI_073 / OfferReason; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_074_s01.wav` | I judge the goods, the measure and what I can sell them for. My offer is | None | Missing | No | L1_CHI_074 / OfferReason; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_075_s01.wav` | I have another sale to make after this purchase. That is why I am offering | None | Missing | No | L1_CHI_075 / OfferReason; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_104_s01.wav` | That is outside my knowledge. Shall we return to the | None | Missing | No | L1_CHI_104 / OffTopic; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_105_s01.wav` | You have taken us a long way from this stall. Back to the purchase. | None | Missing | No | L1_CHI_105 / OffTopic; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_106_s01.wav` | An interesting detour, perhaps. It still leaves the buying unfinished. | None | Missing | No | L1_CHI_106 / OffTopic; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_107_s01.wav` | I came to discuss spice, merchant. Keep me on familiar ground. | None | Missing | No | L1_CHI_107 / OffTopic; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_108_s01.wav` | Saaku. I will hear an argument about price, not an insult. | None | Missing | No | L1_CHI_108 / HostileWarning; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_109_s01.wav` | Speak to me with the respect I have shown you. We can disagree without that tone. | None | Missing | No | L1_CHI_109 / HostileWarning; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_110_s01.wav` | A firm price is no excuse for a loose tongue. Mind your words. | None | Missing | No | L1_CHI_110 / HostileWarning; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_111_s01.wav` | You may challenge my offer. You may not insult me. | None | Missing | No | L1_CHI_111 / HostileWarning; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_112_s01.wav` | Let us pin that down. | None | Missing | No | L1_CHI_112 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Chinnamma Naik / CHI | `chi_113_s01.wav` | One point needs clearing up. | None | Missing | No | L1_CHI_113 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Chinnamma Naik / CHI | `chi_114_s01.wav` | Before we go further: | None | Missing | No | L1_CHI_114 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Chinnamma Naik / CHI | `chi_115_s01.wav` | I need a definite answer here. | None | Missing | No | L1_CHI_115 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Chinnamma Naik / CHI | `chi_117_s01.wav` | The goods are here. Well handled, merchant; thank you. | None | Missing | No | L1_CHI_117 / TransactionSuccess; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_118_s01.wav` | That is settled. You kept to the bargain. I respect that. | None | Missing | No | L1_CHI_118 / TransactionSuccess; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_119_s01.wav` | Goods received, and my thanks. A fair dealing is worth remembering. | None | Missing | No | L1_CHI_119 / TransactionSuccess; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_120_s01.wav` | A sound purchase. Thank you, merchant; may your next bargain go as well. | None | Missing | No | L1_CHI_120 / TransactionSuccess; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_121_s01.wav` | No agreement today. I will try another stall. | None | Missing | No | L1_CHI_121 / TransactionFailure; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_122_s01.wav` | We have not found a price between us. I will leave it there. | None | Missing | No | L1_CHI_122 / TransactionFailure; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_123_s01.wav` | A pity. I came ready to buy, but we have no bargain. | None | Missing | No | L1_CHI_123 / TransactionFailure; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_124_s01.wav` | Then we part without a purchase. Good day, merchant. | None | Missing | No | L1_CHI_124 / TransactionFailure; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_128_s01.wav` | Merchant? You have my attention. Do I have yours? | None | Missing | No | L1_CHI_128 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_129_s01.wav` | What do you say? I am waiting for your answer. | None | Missing | No | L1_CHI_129 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_130_s01.wav` | Shall we continue? There is a purchase to discuss. | None | Missing | No | L1_CHI_130 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_131_s01.wav` | Take a moment to think, then give me a clear answer. | None | Missing | No | L1_CHI_131 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_132_s01.wav` | I have waited a while. Give me an answer, please. | None | Missing | No | L1_CHI_132 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_133_s01.wav` | We cannot bargain in silence. Are we continuing or leaving it? | None | Missing | No | L1_CHI_133 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_134_s01.wav` | My other business is waiting. Tell me where you stand. | None | Missing | No | L1_CHI_134 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_135_s01.wav` | I need to move on soon. What is your answer? | None | Missing | No | L1_CHI_135 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_136_s01.wav` | You make a fair argument, but I remain at | None | Missing | No | L1_CHI_136 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_137_s01.wav` | I like your spirit. My offer is still | None | Missing | No | L1_CHI_137 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_138_s01.wav` | We can disagree and still part on good terms. I am holding at | None | Missing | No | L1_CHI_138 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_139_s01.wav` | A pleasant conversation, but the figure stays at | None | Missing | No | L1_CHI_139 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_140_s01.wav` | You bargain with confidence. So do I: still | None | Missing | No | L1_CHI_140 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_141_s01.wav` | My offer stands at | None | Missing | No | L1_CHI_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_141_s02.wav` | . Give me a reason to reconsider, not the same demand. | None | Missing | No | L1_CHI_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_142_s01.wav` | I have weighed it up. Still | None | Missing | No | L1_CHI_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_143_s01.wav` | The figure is unchanged: | None | Missing | No | L1_CHI_143 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_144_s01.wav` | I am holding at | None | Missing | No | L1_CHI_144 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_144_s02.wav` | . A louder price is not a better argument. | None | Missing | No | L1_CHI_144 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_145_s01.wav` | You have my offer: | None | Missing | No | L1_CHI_145 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_145_s02.wav` | . What do you propose? | None | Missing | No | L1_CHI_145 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_146_s01.wav` | My offer stays at | None | Missing | No | L1_CHI_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_146_s02.wav` | . Your answer? | None | Missing | No | L1_CHI_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_147_s01.wav` | Still | None | Missing | No | L1_CHI_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_147_s02.wav` | . Let us be direct. | None | Missing | No | L1_CHI_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_148_s01.wav` | No increase this time. | None | Missing | No | L1_CHI_148 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_148_s02.wav` | is the offer. | None | Missing | No | L1_CHI_148 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_149_s01.wav` | I remain at | None | Missing | No | L1_CHI_149 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_149_s02.wav` | . Speak plainly. | None | Missing | No | L1_CHI_149 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_150_s01.wav` | The offer is unchanged at | None | Missing | No | L1_CHI_150 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_150_s02.wav` | . Where do you stand? | None | Missing | No | L1_CHI_150 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_151_s01.wav` | Saaku, we need a decision. My offer remains | None | Missing | No | L1_CHI_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_152_s01.wav` | I have little time left. Still | None | Missing | No | L1_CHI_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_152_s02.wav` | ; give me your answer. | None | Missing | No | L1_CHI_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_153_s01.wav` | My offer is unchanged at | None | Missing | No | L1_CHI_153 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_153_s02.wav` | . I cannot wait much longer. | None | Missing | No | L1_CHI_153 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_154_s01.wav` | , merchant. We need to settle this or leave it. | None | Missing | No | L1_CHI_154 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_155_s01.wav` | I am holding at | None | Missing | No | L1_CHI_155 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_155_s02.wav` | . Be brief; my patience is wearing thin. | None | Missing | No | L1_CHI_155 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_156_s01.wav` | Not | None | Missing | No | L1_CHI_156 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_156_s02.wav` | . I remain at | None | Missing | No | L1_CHI_156 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_156_s03.wav` | , and my time is short. | None | Missing | No | L1_CHI_156 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_157_s01.wav` | That demand is too high. My offer is | None | Missing | No | L1_CHI_157 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_157_s02.wav` | . Answer soon, please. | None | Missing | No | L1_CHI_157 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_158_s01.wav` | I am still refusing | None | Missing | No | L1_CHI_158 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_158_s02.wav` | . I offer | None | Missing | No | L1_CHI_158 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_158_s03.wav` | , and cannot wait much longer. | None | Missing | No | L1_CHI_158 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Chinnamma Naik / CHI | `chi_159_s01.wav` | I can raise it to | None | Missing | No | L1_CHI_159 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_159_s02.wav` | . Please answer promptly. | None | Missing | No | L1_CHI_159 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_160_s01.wav` | is my increased offer. I have little time left. | None | Missing | No | L1_CHI_160 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_161_s01.wav` | I am moving to | None | Missing | No | L1_CHI_161 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_161_s02.wav` | . Let us reach a decision now. | None | Missing | No | L1_CHI_161 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_165_s01.wav` | My patience is spent, and we have no agreement. I am leaving. | None | Missing | No | L1_CHI_165 / TransactionFailure; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_166_s01.wav` | No purchase, and no more time to spare. I will go elsewhere. | None | Missing | No | L1_CHI_166 / TransactionFailure; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_167_s01.wav` | We have not agreed. I cannot spend any longer here. | None | Missing | No | L1_CHI_167 / TransactionFailure; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_001_s01.wav` | Namaskaram, merchant. I am Saraswati Chetti. I need | None | Missing | No | L1_SAR_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_001_s02.wav` | of | None | Missing | No | L1_SAR_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_001_s03.wav` | for my shop. | None | Missing | No | L1_SAR_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_002_s01.wav` | Saraswati Chetti. I am looking for | None | Missing | No | L1_SAR_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_002_s02.wav` | of | None | Missing | No | L1_SAR_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_002_s03.wav` | ; what is your price? | None | Missing | No | L1_SAR_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_003_s01.wav` | Good day. My customers ask for | None | Missing | No | L1_SAR_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_003_s02.wav` | , and I need | None | Missing | No | L1_SAR_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_004_s01.wav` | I have come for | None | Missing | No | L1_SAR_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_004_s02.wav` | of | None | Missing | No | L1_SAR_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_004_s03.wav` | . Let us see if the price suits the goods. | None | Missing | No | L1_SAR_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_005_s01.wav` | We meet at your stall again. What have you brought today? | None | Missing | No | L1_SAR_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_006_s01.wav` | I remember your stall, merchant. Let us discuss today's | None | Missing | No | L1_SAR_006 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_007_s01.wav` | Back again for | None | Missing | No | L1_SAR_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_007_s02.wav` | . Let us agree on terms for | None | Missing | No | L1_SAR_007 / RepeatCustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_008_s01.wav` | Another visit, another set of figures. Shall we begin? | None | Missing | No | L1_SAR_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_009_s01.wav` | I need | None | Missing | No | L1_SAR_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_009_s02.wav` | of | None | Missing | No | L1_SAR_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_009_s03.wav` | for my shop. | None | Missing | No | L1_SAR_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_010_s01.wav` | It is | None | Missing | No | L1_SAR_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_010_s02.wav` | I came to purchase, | None | Missing | No | L1_SAR_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_010_s03.wav` | of it. | None | Missing | No | L1_SAR_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_011_s01.wav` | My customers have been asking for | None | Missing | No | L1_SAR_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_011_s02.wav` | . I seek | None | Missing | No | L1_SAR_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_012_s01.wav` | of | None | Missing | No | L1_SAR_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_012_s02.wav` | , provided the quality holds up. | None | Missing | No | L1_SAR_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_013_s01.wav` | My offer stands at | None | Missing | No | L1_SAR_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_013_s02.wav` | for this lot. | None | Missing | No | L1_SAR_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_014_s01.wav` | I have weighed the quantity. | None | Missing | No | L1_SAR_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_014_s02.wav` | is my figure. | None | Missing | No | L1_SAR_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_015_s01.wav` | For the | None | Missing | No | L1_SAR_015 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_015_s02.wav` | of | None | Missing | No | L1_SAR_015 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_015_s03.wav` | in your measure, my figure is | None | Missing | No | L1_SAR_015 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_016_s01.wav` | At present I can justify | None | Missing | No | L1_SAR_016 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_017_s01.wav` | I require | None | Missing | No | L1_SAR_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_017_s02.wav` | of | None | Missing | No | L1_SAR_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_018_s01.wav` | The measure I asked for is | None | Missing | No | L1_SAR_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_019_s01.wav` | Set aside | None | Missing | No | L1_SAR_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_019_s02.wav` | of | None | Missing | No | L1_SAR_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_019_s03.wav` | ; let our figures be exact. | None | Missing | No | L1_SAR_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_020_s01.wav` | , merchant. The quantity matters as much as the price. | None | Missing | No | L1_SAR_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_021_s01.wav` | You know my quantity. What price do you ask for it? | None | Missing | No | L1_SAR_021 / AskSellerPrice; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_022_s01.wav` | Let me hear your price for | None | Missing | No | L1_SAR_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_022_s02.wav` | of | None | Missing | No | L1_SAR_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_023_s01.wav` | The measure is clear; the price is yours to name. | None | Missing | No | L1_SAR_023 / AskSellerPrice; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_024_s01.wav` | I have told you what I need. What is your asking price? | None | Missing | No | L1_SAR_024 / AskSellerPrice; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_025_s01.wav` | Then we are discussing | None | Missing | No | L1_SAR_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_025_s02.wav` | of | None | Missing | No | L1_SAR_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_025_s03.wav` | . Let us price that quantity. | None | Missing | No | L1_SAR_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_026_s01.wav` | I have the new measure: | None | Missing | No | L1_SAR_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_026_s02.wav` | . What is your price for it? | None | Missing | No | L1_SAR_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_027_s01.wav` | Very well, | None | Missing | No | L1_SAR_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_027_s02.wav` | of | None | Missing | No | L1_SAR_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_027_s03.wav` | . That changes what we must weigh. | None | Missing | No | L1_SAR_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_028_s01.wav` | The quantity is now | None | Missing | No | L1_SAR_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_028_s02.wav` | ; I will judge the price against that. | None | Missing | No | L1_SAR_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_029_s01.wav` | is beyond what I can justify for this measure. My offer is | None | Missing | No | L1_SAR_029 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_030_s01.wav` | That price asks my customers to bear too much. I remain at | None | Missing | No | L1_SAR_030 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_031_s01.wav` | Tell me what warrants | None | Missing | No | L1_SAR_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_031_s02.wav` | ; I cannot pay it as it stands. | None | Missing | No | L1_SAR_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_032_s01.wav` | I have counted the measure twice. | None | Missing | No | L1_SAR_032 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_032_s02.wav` | is still too high. | None | Missing | No | L1_SAR_032 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_033_s01.wav` | needs a stronger case. My figure is | None | Missing | No | L1_SAR_033 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_034_s01.wav` | I can follow your reasoning only as far as | None | Missing | No | L1_SAR_034 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_035_s01.wav` | The asking price is above what I can justify. I remain at | None | Missing | No | L1_SAR_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_036_s01.wav` | The quality may be sound, but | None | Missing | No | L1_SAR_036 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_036_s02.wav` | is more than I will pay. | None | Missing | No | L1_SAR_036 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_037_s01.wav` | That is lower than I expected. Are you certain of such a figure? | None | Missing | No | L1_SAR_037 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_038_s01.wav` | A low price can be welcome, but a calculating buyer always wonders why. | None | Missing | No | L1_SAR_038 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_039_s01.wav` | You ask less than I expected. Is there a reason for so modest a sum? | None | Missing | No | L1_SAR_039 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_040_s01.wav` | An unusual figure. I do not simply take a cheap offer without understanding the reason. | None | Missing | No | L1_SAR_040 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_041_s01.wav` | For this | None | Missing | No | L1_SAR_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_041_s02.wav` | , I can offer | None | Missing | No | L1_SAR_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_041_s03.wav` | . What do you think? | None | Missing | No | L1_SAR_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_042_s01.wav` | Let me put | None | Missing | No | L1_SAR_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_042_s02.wav` | before you for the lot. | None | Missing | No | L1_SAR_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_043_s01.wav` | I have considered the measure. My offer is | None | Missing | No | L1_SAR_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_044_s01.wav` | We can begin at | None | Missing | No | L1_SAR_044 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_044_s02.wav` | ; tell me where your figure differs. | None | Missing | No | L1_SAR_044 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_045_s01.wav` | Your case has merit. I can increase my offer to | None | Missing | No | L1_SAR_045 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_046_s01.wav` | I have revised the figure to | None | Missing | No | L1_SAR_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_046_s02.wav` | . Will that answer? | None | Missing | No | L1_SAR_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_047_s01.wav` | The gap is narrower now: I offer | None | Missing | No | L1_SAR_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_048_s01.wav` | I can move to | None | Missing | No | L1_SAR_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_048_s02.wav` | after hearing your argument. | None | Missing | No | L1_SAR_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_049_s01.wav` | I have reconsidered the figures. My offer is now | None | Missing | No | L1_SAR_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_050_s01.wav` | For the measure we have discussed, I can move to | None | Missing | No | L1_SAR_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_051_s01.wav` | You have drawn another step from me: | None | Missing | No | L1_SAR_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_051_s02.wav` | . What is your answer? | None | Missing | No | L1_SAR_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_052_s01.wav` | After weighing your point, I offer | None | Missing | No | L1_SAR_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_052_s02.wav` | now. | None | Missing | No | L1_SAR_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_053_s01.wav` | My figure remains | None | Missing | No | L1_SAR_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_053_s02.wav` | ; the measure has not changed. | None | Missing | No | L1_SAR_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_054_s01.wav` | I have heard you, but my offer is still | None | Missing | No | L1_SAR_054 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_055_s01.wav` | Give me a reason to revise it. Until then, | None | Missing | No | L1_SAR_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_055_s02.wav` | stands. | None | Missing | No | L1_SAR_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_056_s01.wav` | The argument has moved; the price has not. I remain at | None | Missing | No | L1_SAR_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_057_s01.wav` | I am keeping my offer at | None | Missing | No | L1_SAR_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_057_s02.wav` | . What do you propose? | None | Missing | No | L1_SAR_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_058_s01.wav` | Avunu, the price is agreed: | None | Missing | No | L1_SAR_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_058_s02.wav` | for | None | Missing | No | L1_SAR_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_058_s03.wav` | of | None | Missing | No | L1_SAR_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_058_s04.wav` | . Please bring the goods. | None | Missing | No | L1_SAR_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_059_s01.wav` | suits the | None | Missing | No | L1_SAR_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_059_s02.wav` | of | None | Missing | No | L1_SAR_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_059_s03.wav` | . Bring the goods, please. | None | Missing | No | L1_SAR_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_060_s01.wav` | We have terms: | None | Missing | No | L1_SAR_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_060_s02.wav` | of | None | Missing | No | L1_SAR_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_060_s03.wav` | at | None | Missing | No | L1_SAR_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_060_s04.wav` | . Please prepare the delivery. | None | Missing | No | L1_SAR_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_061_s01.wav` | I accept | None | Missing | No | L1_SAR_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_061_s02.wav` | for | None | Missing | No | L1_SAR_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_061_s03.wav` | of | None | Missing | No | L1_SAR_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_061_s04.wav` | . The goods are still to come. | None | Missing | No | L1_SAR_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_062_s01.wav` | Then we agree on | None | Missing | No | L1_SAR_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_062_s02.wav` | for | None | Missing | No | L1_SAR_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_062_s03.wav` | of | None | Missing | No | L1_SAR_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_062_s04.wav` | . Bring it over. | None | Missing | No | L1_SAR_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_063_s01.wav` | The price is settled at | None | Missing | No | L1_SAR_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_063_s02.wav` | . Please bring over the | None | Missing | No | L1_SAR_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_063_s03.wav` | of | None | Missing | No | L1_SAR_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_064_s01.wav` | I accept these terms: | None | Missing | No | L1_SAR_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_064_s02.wav` | of | None | Missing | No | L1_SAR_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_064_s03.wav` | . Please measure it out. | None | Missing | No | L1_SAR_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_065_s01.wav` | Our figures agree at | None | Missing | No | L1_SAR_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_065_s02.wav` | . Please bring the | None | Missing | No | L1_SAR_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_065_s03.wav` | of | None | Missing | No | L1_SAR_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Saraswati Chetti / SAR | `sar_066_s01.wav` | My figure does not persuade you. What price would? | None | Missing | No | L1_SAR_066 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_067_s01.wav` | You refuse that offer. Give me a figure to consider. | None | Missing | No | L1_SAR_067 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_068_s01.wav` | Then tell me where your calculation differs from mine. | None | Missing | No | L1_SAR_068 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_069_s01.wav` | We have no agreement yet. State the price you want. | None | Missing | No | L1_SAR_069 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_070_s01.wav` | I must resell this lot in my shop. At | None | Missing | No | L1_SAR_070 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_070_s02.wav` | , the measure still makes sense. | None | Missing | No | L1_SAR_070 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_071_s01.wav` | I judge the quality and what my customers will pay. That gives me | None | Missing | No | L1_SAR_071 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_072_s01.wav` | My offer of | None | Missing | No | L1_SAR_072 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_072_s02.wav` | accounts for the quantity and the work of selling it. | None | Missing | No | L1_SAR_072 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_073_s01.wav` | There is a second sale after this one. That is why I offer | None | Missing | No | L1_SAR_073 / OfferReason; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_099_s01.wav` | That is beyond what I know. Shall we return to the | None | Missing | No | L1_SAR_099 / OffTopic; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_100_s01.wav` | An interesting turn, though it does not answer the price of this lot. | None | Missing | No | L1_SAR_100 / OffTopic; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_101_s01.wav` | I can speak to the measure before us. What did you want to know about it? | None | Missing | No | L1_SAR_101 / OffTopic; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_102_s01.wav` | We may discuss that another time. For now, I am buying | None | Missing | No | L1_SAR_102 / OffTopic; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_103_s01.wav` | I will consider a price, not an insult. Speak plainly and respectfully. | None | Missing | No | L1_SAR_103 / HostileWarning; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_104_s01.wav` | There is no need for that tone. State your objection to the offer. | None | Missing | No | L1_SAR_104 / HostileWarning; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_105_s01.wav` | A careful measure deserves a careful word. Try again without the insult. | None | Missing | No | L1_SAR_105 / HostileWarning; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_106_s01.wav` | Konchem more detail, please: | None | Missing | No | L1_SAR_106 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Saraswati Chetti / SAR | `sar_107_s01.wav` | One point is uncertain. | None | Missing | No | L1_SAR_107 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Saraswati Chetti / SAR | `sar_108_s01.wav` | Let us be precise. | None | Missing | No | L1_SAR_108 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Saraswati Chetti / SAR | `sar_109_s01.wav` | I need to hear the exact terms. | None | Missing | No | L1_SAR_109 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Saraswati Chetti / SAR | `sar_111_s01.wav` | The goods are received and the measure is right. Thank you, merchant. | None | Missing | No | L1_SAR_111 / TransactionSuccess; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_112_s01.wav` | The delivery matches our terms. A careful trade; you have my thanks. | None | Missing | No | L1_SAR_112 / TransactionSuccess; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_113_s01.wav` | Well measured and delivered. I am pleased with this purchase. | None | Missing | No | L1_SAR_113 / TransactionSuccess; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_114_s01.wav` | I have the | None | Missing | No | L1_SAR_114 / TransactionSuccess; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_114_s02.wav` | now. You kept your word, and I appreciate it. | None | Missing | No | L1_SAR_114 / TransactionSuccess; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_115_s01.wav` | We have not found terms. I will try another stall. | None | Missing | No | L1_SAR_115 / TransactionFailure; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_116_s01.wav` | The figures do not meet. I am leaving without a purchase. | None | Missing | No | L1_SAR_116 / TransactionFailure; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_117_s01.wav` | No bargain today. I will return to my shop. | None | Missing | No | L1_SAR_117 / TransactionFailure; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_118_s01.wav` | I cannot take this lot on those terms. Good day, merchant. | None | Missing | No | L1_SAR_118 / TransactionFailure; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_122_s01.wav` | Merchant? I am waiting for your figure. | None | Missing | No | L1_SAR_122 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_123_s01.wav` | Take a moment, then tell me what you think of my offer. | None | Missing | No | L1_SAR_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_124_s01.wav` | Have you considered the measure? I am ready for your answer. | None | Missing | No | L1_SAR_124 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_125_s01.wav` | I need an answer now; my shop is waiting. | None | Missing | No | L1_SAR_125 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_126_s01.wav` | We cannot bargain by silence. Do you have a price? | None | Missing | No | L1_SAR_126 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_127_s01.wav` | Please give me your decision. I have other buying to do. | None | Missing | No | L1_SAR_127 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_128_s01.wav` | I enjoy a careful bargain. My figure is still | None | Missing | No | L1_SAR_128 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_129_s01.wav` | You make a thoughtful case, but I remain at | None | Missing | No | L1_SAR_129 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_130_s01.wav` | We can discuss the figures further. My offer stays at | None | Missing | No | L1_SAR_130 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_131_s01.wav` | I am listening, though the number remains | None | Missing | No | L1_SAR_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_132_s01.wav` | You may yet persuade me; for now I hold at | None | Missing | No | L1_SAR_132 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_133_s01.wav` | My offer remains | None | Missing | No | L1_SAR_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_133_s02.wav` | . What is your counter? | None | Missing | No | L1_SAR_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_134_s01.wav` | The measure still supports | None | Missing | No | L1_SAR_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_134_s02.wav` | in my reckoning. | None | Missing | No | L1_SAR_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_135_s01.wav` | I have checked it again: still | None | Missing | No | L1_SAR_135 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_136_s01.wav` | I have considered it. The offer is still | None | Missing | No | L1_SAR_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_137_s01.wav` | That argument does not alter my offer of | None | Missing | No | L1_SAR_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_138_s01.wav` | I have checked the measure. My offer stays at | None | Missing | No | L1_SAR_138 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_139_s01.wav` | Repeating a price does not explain it. I remain at | None | Missing | No | L1_SAR_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_140_s01.wav` | I require a reason to move. Until then, | None | Missing | No | L1_SAR_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_141_s01.wav` | No change in my offer: | None | Missing | No | L1_SAR_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_142_s01.wav` | I have heard the demand. The figure remains | None | Missing | No | L1_SAR_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_143_s01.wav` | Still | None | Missing | No | L1_SAR_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_143_s02.wav` | . Your answer, please. | None | Missing | No | L1_SAR_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_144_s01.wav` | My offer is unchanged at | None | Missing | No | L1_SAR_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_144_s02.wav` | . Be direct. | None | Missing | No | L1_SAR_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_145_s01.wav` | remains the figure. We need a decision. | None | Missing | No | L1_SAR_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_146_s01.wav` | I have no revised offer. It is | None | Missing | No | L1_SAR_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_147_s01.wav` | I remain at | None | Missing | No | L1_SAR_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_147_s02.wav` | . What do you say? | None | Missing | No | L1_SAR_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_148_s01.wav` | My patience is short, but my offer is unchanged: | None | Missing | No | L1_SAR_148 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_149_s01.wav` | Still | None | Missing | No | L1_SAR_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_149_s02.wav` | . I need a clear answer soon. | None | Missing | No | L1_SAR_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_150_s01.wav` | My figure remains | None | Missing | No | L1_SAR_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_150_s02.wav` | ; I cannot wait much longer. | None | Missing | No | L1_SAR_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_151_s01.wav` | We know the quantity. The offer is still | None | Missing | No | L1_SAR_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_152_s01.wav` | I am holding at | None | Missing | No | L1_SAR_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_152_s02.wav` | . Please decide. | None | Missing | No | L1_SAR_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_153_s01.wav` | is too high. My offer stays at | None | Missing | No | L1_SAR_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_153_s02.wav` | ; answer soon. | None | Missing | No | L1_SAR_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Saraswati Chetti / SAR | `sar_154_s01.wav` | I cannot justify that figure, and I have little time left. | None | Missing | No | L1_SAR_154 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_155_s01.wav` | Your price remains too high. I need a different answer now. | None | Missing | No | L1_SAR_155 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_156_s01.wav` | I can increase to | None | Missing | No | L1_SAR_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_156_s02.wav` | . Please answer promptly. | None | Missing | No | L1_SAR_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_157_s01.wav` | My revised offer is | None | Missing | No | L1_SAR_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_157_s02.wav` | ; time is short. | None | Missing | No | L1_SAR_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_158_s01.wav` | I have moved to | None | Missing | No | L1_SAR_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_158_s02.wav` | . Let us decide. | None | Missing | No | L1_SAR_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_162_s01.wav` | My time is spent. I will seek the goods elsewhere. | None | Missing | No | L1_SAR_162 / TransactionFailure; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_163_s01.wav` | We have no bargain, and I must return to my shop. | None | Missing | No | L1_SAR_163 / TransactionFailure; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_164_s01.wav` | I cannot wait longer on these terms. I am leaving. | None | Missing | No | L1_SAR_164 / TransactionFailure; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_001_s01.wav` | Marhaban, merchant. I am Abdul Rahman. I wish to buy | None | Missing | No | L1_ABD_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_001_s02.wav` | of | None | Missing | No | L1_ABD_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_002_s01.wav` | Good day. I am Abdul Rahman, looking for | None | Missing | No | L1_ABD_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_002_s02.wav` | of | None | Missing | No | L1_ABD_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_003_s01.wav` | I have come with the caravan to buy | None | Missing | No | L1_ABD_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_003_s02.wav` | of | None | Missing | No | L1_ABD_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_003_s03.wav` | . What is your price? | None | Missing | No | L1_ABD_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_004_s01.wav` | Abdul Rahman. I hope your | None | Missing | No | L1_ABD_004 / CustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_004_s02.wav` | is as inviting as your stall. | None | Missing | No | L1_ABD_004 / CustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_005_s01.wav` | I know your stall now. What do you have for me today? | None | Missing | No | L1_ABD_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_006_s01.wav` | We meet again, merchant. Let us give this lot its own fair hearing. | None | Missing | No | L1_ABD_006 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_007_s01.wav` | A familiar face makes the bargaining easier. What is your price for | None | Missing | No | L1_ABD_007 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_008_s01.wav` | Back to your stall for | None | Missing | No | L1_ABD_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_008_s02.wav` | . Shall we begin afresh? | None | Missing | No | L1_ABD_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_009_s01.wav` | I need | None | Missing | No | L1_ABD_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_009_s02.wav` | of | None | Missing | No | L1_ABD_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_009_s03.wav` | for caravan trade. | None | Missing | No | L1_ABD_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_010_s01.wav` | , please, and | None | Missing | No | L1_ABD_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_010_s02.wav` | if the measure is sound. | None | Missing | No | L1_ABD_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_011_s01.wav` | The lot I came to discuss is | None | Missing | No | L1_ABD_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_011_s02.wav` | of | None | Missing | No | L1_ABD_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_012_s01.wav` | I am buying | None | Missing | No | L1_ABD_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_012_s02.wav` | . Let us settle terms for | None | Missing | No | L1_ABD_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_013_s01.wav` | My offer for this lot is | None | Missing | No | L1_ABD_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_014_s01.wav` | I can offer | None | Missing | No | L1_ABD_014 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_014_s02.wav` | for the | None | Missing | No | L1_ABD_014 / AskBuyerBudget; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_015_s01.wav` | For the goods before us, I have put forward | None | Missing | No | L1_ABD_015 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_016_s01.wav` | You have my current figure: | None | Missing | No | L1_ABD_016 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_017_s01.wav` | I asked for | None | Missing | No | L1_ABD_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_017_s02.wav` | of | None | Missing | No | L1_ABD_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_018_s01.wav` | The measure is | None | Missing | No | L1_ABD_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_018_s02.wav` | , merchant. | None | Missing | No | L1_ABD_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_019_s01.wav` | For this purchase, I need | None | Missing | No | L1_ABD_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_019_s02.wav` | of | None | Missing | No | L1_ABD_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_020_s01.wav` | of | None | Missing | No | L1_ABD_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_020_s02.wav` | ; that is the lot we are pricing. | None | Missing | No | L1_ABD_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_021_s01.wav` | I have named the lot. What price do you ask? | None | Missing | No | L1_ABD_021 / AskSellerPrice; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_022_s01.wav` | What is your price for | None | Missing | No | L1_ABD_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_022_s02.wav` | of | None | Missing | No | L1_ABD_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_023_s01.wav` | Let me hear your asking figure, merchant. | None | Missing | No | L1_ABD_023 / AskSellerPrice; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_024_s01.wav` | You know what I wish to buy. Tell me what it will cost. | None | Missing | No | L1_ABD_024 / AskSellerPrice; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_025_s01.wav` | Then we are speaking of | None | Missing | No | L1_ABD_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_025_s02.wav` | of | None | Missing | No | L1_ABD_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_025_s03.wav` | now. Let us price that lot. | None | Missing | No | L1_ABD_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_026_s01.wav` | I understand the new measure: | None | Missing | No | L1_ABD_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_026_s02.wav` | . What is your price for it? | None | Missing | No | L1_ABD_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_027_s01.wav` | Very well, | None | Missing | No | L1_ABD_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_027_s02.wav` | of | None | Missing | No | L1_ABD_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_027_s03.wav` | . We can bargain on that basis. | None | Missing | No | L1_ABD_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_028_s01.wav` | The quantity has changed to | None | Missing | No | L1_ABD_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_028_s02.wav` | ; let us keep our figures clear. | None | Missing | No | L1_ABD_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_029_s01.wav` | is beyond what I can pay for this lot. My offer remains | None | Missing | No | L1_ABD_029 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_030_s01.wav` | I respect your goods, but that asking price leaves me no room to trade onward. | None | Missing | No | L1_ABD_030 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_031_s01.wav` | We may still talk, though | None | Missing | No | L1_ABD_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_031_s02.wav` | is more than I can justify. | None | Missing | No | L1_ABD_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_032_s01.wav` | I cannot follow that figure. I remain at | None | Missing | No | L1_ABD_032 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_033_s01.wav` | Your asking figure is above mine. I remain at | None | Missing | No | L1_ABD_033 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_034_s01.wav` | There may be a bargain here, but | None | Missing | No | L1_ABD_034 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_034_s02.wav` | does not work for me. | None | Missing | No | L1_ABD_034 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_035_s01.wav` | I hear your price. My offer is still | None | Missing | No | L1_ABD_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_035_s02.wav` | ; can you make a better case? | None | Missing | No | L1_ABD_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_036_s01.wav` | I would like us both to trade well. At present I can offer | None | Missing | No | L1_ABD_036 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_037_s01.wav` | That price is unexpectedly low. Are you certain of that figure? | None | Missing | No | L1_ABD_037 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_038_s01.wav` | You ask less than I expected; such a cheap offer makes a merchant cautious. | None | Missing | No | L1_ABD_038 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_039_s01.wav` | A low figure catches my ear, but I must wonder why you part with it so cheaply. | None | Missing | No | L1_ABD_039 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_040_s01.wav` | That is below my expectation. Did you truly intend to name so small a sum? | None | Missing | No | L1_ABD_040 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_041_s01.wav` | Let me offer | None | Missing | No | L1_ABD_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_041_s02.wav` | for the | None | Missing | No | L1_ABD_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_041_s03.wav` | of | None | Missing | No | L1_ABD_041 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_042_s01.wav` | I can begin at | None | Missing | No | L1_ABD_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_042_s02.wav` | . Tell me how that sits with you. | None | Missing | No | L1_ABD_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_043_s01.wav` | For this lot, I put | None | Missing | No | L1_ABD_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_043_s02.wav` | before you. | None | Missing | No | L1_ABD_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_044_s01.wav` | Perhaps | None | Missing | No | L1_ABD_044 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_044_s02.wav` | gives us a good place to begin. | None | Missing | No | L1_ABD_044 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_045_s01.wav` | I have heard your reasoning. I can raise my offer to | None | Missing | No | L1_ABD_045 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_046_s01.wav` | Let me move to | None | Missing | No | L1_ABD_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_046_s02.wav` | ; I hope that brings us nearer. | None | Missing | No | L1_ABD_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_047_s01.wav` | You have persuaded me to offer | None | Missing | No | L1_ABD_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_047_s02.wav` | now. | None | Missing | No | L1_ABD_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_048_s01.wav` | For the sake of a fair trade, my revised figure is | None | Missing | No | L1_ABD_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_049_s01.wav` | After hearing you out, I can move to | None | Missing | No | L1_ABD_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_049_s02.wav` | . What do you say? | None | Missing | No | L1_ABD_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_050_s01.wav` | My offer is now | None | Missing | No | L1_ABD_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_050_s02.wav` | ; we have both given this thought. | None | Missing | No | L1_ABD_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_051_s01.wav` | I can raise it to | None | Missing | No | L1_ABD_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_051_s02.wav` | . Is there room for agreement? | None | Missing | No | L1_ABD_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_052_s01.wav` | Let us see whether | None | Missing | No | L1_ABD_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_052_s02.wav` | brings our figures together. | None | Missing | No | L1_ABD_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_053_s01.wav` | I value a continuing acquaintance, but my offer remains | None | Missing | No | L1_ABD_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_054_s01.wav` | I have listened. I am still at | None | Missing | No | L1_ABD_054 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_055_s01.wav` | My figure is unchanged: | None | Missing | No | L1_ABD_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_055_s02.wav` | . Tell me what I am missing. | None | Missing | No | L1_ABD_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_056_s01.wav` | Goodwill does not alter the cost of this lot. I remain at | None | Missing | No | L1_ABD_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_057_s01.wav` | I would rather agree than argue, but my offer stays at | None | Missing | No | L1_ABD_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_058_s01.wav` | We agree on | None | Missing | No | L1_ABD_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_058_s02.wav` | for | None | Missing | No | L1_ABD_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_058_s03.wav` | of | None | Missing | No | L1_ABD_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_058_s04.wav` | . Please bring the goods. | None | Missing | No | L1_ABD_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_059_s01.wav` | That price works for me: | None | Missing | No | L1_ABD_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_059_s02.wav` | for | None | Missing | No | L1_ABD_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_059_s03.wav` | of | None | Missing | No | L1_ABD_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_059_s04.wav` | . Please prepare the goods. | None | Missing | No | L1_ABD_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_060_s01.wav` | I accept the terms, | None | Missing | No | L1_ABD_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_060_s02.wav` | and | None | Missing | No | L1_ABD_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_060_s03.wav` | of | None | Missing | No | L1_ABD_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_060_s04.wav` | . The goods are still to come. | None | Missing | No | L1_ABD_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_061_s01.wav` | We have a bargain at | None | Missing | No | L1_ABD_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_061_s02.wav` | . Bring the | None | Missing | No | L1_ABD_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_061_s03.wav` | of | None | Missing | No | L1_ABD_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_061_s04.wav` | , please. | None | Missing | No | L1_ABD_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_062_s01.wav` | Then we are agreed at | None | Missing | No | L1_ABD_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_062_s02.wav` | for | None | Missing | No | L1_ABD_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_062_s03.wav` | of | None | Missing | No | L1_ABD_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_062_s04.wav` | . Bring the lot over. | None | Missing | No | L1_ABD_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_063_s01.wav` | Our price is settled: | None | Missing | No | L1_ABD_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_063_s02.wav` | for | None | Missing | No | L1_ABD_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_063_s03.wav` | of | None | Missing | No | L1_ABD_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_063_s04.wav` | . I await the goods. | None | Missing | No | L1_ABD_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_064_s01.wav` | I accept those terms. | None | Missing | No | L1_ABD_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_064_s02.wav` | of | None | Missing | No | L1_ABD_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_064_s03.wav` | ; please weigh it out. | None | Missing | No | L1_ABD_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_065_s01.wav` | A fair understanding: | None | Missing | No | L1_ABD_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_065_s02.wav` | of | None | Missing | No | L1_ABD_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_065_s03.wav` | at | None | Missing | No | L1_ABD_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_065_s04.wav` | . Please bring it here. | None | Missing | No | L1_ABD_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Abdul Rahman / ABD | `abd_066_s01.wav` | My offer does not suit you. What figure would? | None | Missing | No | L1_ABD_066 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_067_s01.wav` | You have refused that price. Tell me what you would ask instead. | None | Missing | No | L1_ABD_067 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_068_s01.wav` | Then we differ on the number. Give me one I can consider. | None | Missing | No | L1_ABD_068 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_069_s01.wav` | I hear your refusal. Shall we consider another price? | None | Missing | No | L1_ABD_069 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_070_s01.wav` | I must trade the goods onward. At | None | Missing | No | L1_ABD_070 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_070_s02.wav` | , there is room for us both. | None | Missing | No | L1_ABD_070 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_071_s01.wav` | I have considered the measure and the cost of carrying it. That gives me | None | Missing | No | L1_ABD_071 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_072_s01.wav` | A fair purchase must survive its next sale. That is why I offer | None | Missing | No | L1_ABD_072 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_073_s01.wav` | I want to return to a seller I can trust. | None | Missing | No | L1_ABD_073 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_073_s02.wav` | is my present offer. | None | Missing | No | L1_ABD_073 / OfferReason; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_099_s01.wav` | That lies beyond what I know. Shall we return to the | None | Missing | No | L1_ABD_099 / OffTopic; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_100_s01.wav` | You have surprised me with that question. The lot before us is more familiar ground. | None | Missing | No | L1_ABD_100 / OffTopic; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_101_s01.wav` | I cannot give you a useful answer to that. I can answer for my offer. | None | Missing | No | L1_ABD_101 / OffTopic; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_102_s01.wav` | Another conversation, perhaps. Today I came to discuss | None | Missing | No | L1_ABD_102 / OffTopic; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_103_s01.wav` | I have shown you courtesy. I ask the same in return. | None | Missing | No | L1_ABD_103 / HostileWarning; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_104_s01.wav` | I will discuss the price, merchant. I will not trade insults. | None | Missing | No | L1_ABD_104 / HostileWarning; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_105_s01.wav` | Speak to me as a fellow trader, and we may still do business. | None | Missing | No | L1_ABD_105 / HostileWarning; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_106_s01.wav` | Help me understand one point: | None | Missing | No | L1_ABD_106 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Abdul Rahman / ABD | `abd_107_s01.wav` | I would rather ask than assume. | None | Missing | No | L1_ABD_107 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Abdul Rahman / ABD | `abd_108_s01.wav` | Before I answer, let us make this clear. | None | Missing | No | L1_ABD_108 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Abdul Rahman / ABD | `abd_109_s01.wav` | I missed the precise terms. | None | Missing | No | L1_ABD_109 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Abdul Rahman / ABD | `abd_111_s01.wav` | Shukran, merchant. The goods are here, and you have kept your word. | None | Missing | No | L1_ABD_111 / TransactionSuccess; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_112_s01.wav` | The | None | Missing | No | L1_ABD_112 / TransactionSuccess; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_112_s02.wav` | is delivered as agreed. Thank you for a fair exchange. | None | Missing | No | L1_ABD_112 / TransactionSuccess; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_113_s01.wav` | Goods received, and my thanks. I would be glad to meet you at this stall again. | None | Missing | No | L1_ABD_113 / TransactionSuccess; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_114_s01.wav` | You dealt openly and delivered the lot. I will remember that. | None | Missing | No | L1_ABD_114 / TransactionSuccess; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_115_s01.wav` | We have no bargain today. I will take my business to another stall. | None | Missing | No | L1_ABD_115 / TransactionFailure; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_116_s01.wav` | Our prices do not meet. I will leave the lot with you. | None | Missing | No | L1_ABD_116 / TransactionFailure; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_117_s01.wav` | I must go without a purchase. I wish you better trade ahead. | None | Missing | No | L1_ABD_117 / TransactionFailure; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_118_s01.wav` | No agreement this time. I will return to the caravan. | None | Missing | No | L1_ABD_118 / TransactionFailure; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_122_s01.wav` | Merchant? I am still here, if you have a figure for me. | None | Missing | No | L1_ABD_122 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_123_s01.wav` | Take a moment. I would like to hear your answer. | None | Missing | No | L1_ABD_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_124_s01.wav` | Shall we continue with the | None | Missing | No | L1_ABD_124 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_125_s01.wav` | I have waited a while. Are we to continue this bargain? | None | Missing | No | L1_ABD_125 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_126_s01.wav` | Please tell me where you stand; I have other business today. | None | Missing | No | L1_ABD_126 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_127_s01.wav` | Silence will not bring our figures together. What is your answer? | None | Missing | No | L1_ABD_127 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_128_s01.wav` | I enjoy our talk, though my offer is still | None | Missing | No | L1_ABD_128 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_129_s01.wav` | You make good company. My figure remains | None | Missing | No | L1_ABD_129 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_130_s01.wav` | I hope we trade together, but the offer stays at | None | Missing | No | L1_ABD_130 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_131_s01.wav` | We can disagree amiably. I remain at | None | Missing | No | L1_ABD_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_132_s01.wav` | I am listening, friend; I have not moved from | None | Missing | No | L1_ABD_132 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_133_s01.wav` | I remain at | None | Missing | No | L1_ABD_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_133_s02.wav` | . What price do you suggest? | None | Missing | No | L1_ABD_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_134_s01.wav` | My offer has not changed: | None | Missing | No | L1_ABD_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_135_s01.wav` | I have considered it and still offer | None | Missing | No | L1_ABD_135 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_136_s01.wav` | The figure before you remains | None | Missing | No | L1_ABD_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_137_s01.wav` | I would hear another reason. For now, | None | Missing | No | L1_ABD_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_137_s02.wav` | stands. | None | Missing | No | L1_ABD_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_138_s01.wav` | I have listened carefully. My offer remains | None | Missing | No | L1_ABD_138 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_139_s01.wav` | Respectfully, I see no reason to revise | None | Missing | No | L1_ABD_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_140_s01.wav` | I must keep to | None | Missing | No | L1_ABD_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_140_s02.wav` | . Present another argument if you have one. | None | Missing | No | L1_ABD_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_141_s01.wav` | My position is unchanged at | None | Missing | No | L1_ABD_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_142_s01.wav` | You have my answer: | None | Missing | No | L1_ABD_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_143_s01.wav` | My figure is still | None | Missing | No | L1_ABD_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_143_s02.wav` | ; may we decide? | None | Missing | No | L1_ABD_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_144_s01.wav` | The offer remains | None | Missing | No | L1_ABD_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_144_s02.wav` | . Shall we decide? | None | Missing | No | L1_ABD_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_145_s01.wav` | I have not raised it. | None | Missing | No | L1_ABD_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_145_s02.wav` | stands. | None | Missing | No | L1_ABD_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_146_s01.wav` | , unchanged. Please be direct. | None | Missing | No | L1_ABD_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_147_s01.wav` | My figure is | None | Missing | No | L1_ABD_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_147_s02.wav` | . I need your reply. | None | Missing | No | L1_ABD_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_148_s01.wav` | My time is short; my offer is still | None | Missing | No | L1_ABD_148 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_149_s01.wav` | I remain at | None | Missing | No | L1_ABD_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_149_s02.wav` | . Please decide soon. | None | Missing | No | L1_ABD_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_150_s01.wav` | The figure has not changed: | None | Missing | No | L1_ABD_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_150_s02.wav` | . I cannot wait much longer. | None | Missing | No | L1_ABD_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_151_s01.wav` | We can continue if you answer promptly. I offer | None | Missing | No | L1_ABD_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_152_s01.wav` | I am holding at | None | Missing | No | L1_ABD_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_152_s02.wav` | . Do you wish to counter? | None | Missing | No | L1_ABD_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_153_s01.wav` | I cannot pay | None | Missing | No | L1_ABD_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_153_s02.wav` | . My offer remains | None | Missing | No | L1_ABD_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_153_s03.wav` | , and time is short. | None | Missing | No | L1_ABD_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_154_s01.wav` | That demand is too high. I need a different figure soon. | None | Missing | No | L1_ABD_154 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_155_s01.wav` | I have little time left; | None | Missing | No | L1_ABD_155 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_155_s02.wav` | is still beyond me. | None | Missing | No | L1_ABD_155 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Abdul Rahman / ABD | `abd_156_s01.wav` | I can raise my offer to | None | Missing | No | L1_ABD_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_156_s02.wav` | . Please answer soon. | None | Missing | No | L1_ABD_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_157_s01.wav` | My revised figure is | None | Missing | No | L1_ABD_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_157_s02.wav` | . I cannot linger. | None | Missing | No | L1_ABD_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_158_s01.wav` | I have moved to | None | Missing | No | L1_ABD_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_158_s02.wav` | . Shall we decide? | None | Missing | No | L1_ABD_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_162_s01.wav` | Our figures do not meet, and I must leave now. | None | Missing | No | L1_ABD_162 / TransactionFailure; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_163_s01.wav` | I have no more time for this lot. I will try another stall. | None | Missing | No | L1_ABD_163 / TransactionFailure; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_164_s01.wav` | No bargain today. I must return to my companions. | None | Missing | No | L1_ABD_164 / TransactionFailure; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_001_s01.wav` | Olá, merchant. I am Francisco de Almeida. I would like to buy | None | Missing | No | L1_FRA_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_001_s02.wav` | of | None | Missing | No | L1_FRA_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_002_s01.wav` | Francisco de Almeida. Your | None | Missing | No | L1_FRA_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_002_s02.wav` | caught my eye; I am looking for | None | Missing | No | L1_FRA_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_003_s01.wav` | A promising stall! I have come to ask about | None | Missing | No | L1_FRA_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_003_s02.wav` | of | None | Missing | No | L1_FRA_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_004_s01.wav` | Good day. I am Francisco, and I would like to buy | None | Missing | No | L1_FRA_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_004_s02.wav` | of | None | Missing | No | L1_FRA_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_005_s01.wav` | Olá again. What sort of | None | Missing | No | L1_FRA_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_005_s02.wav` | have you for me today? | None | Missing | No | L1_FRA_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_006_s01.wav` | Your stall has drawn me back. Let us discuss this lot afresh. | None | Missing | No | L1_FRA_006 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_007_s01.wav` | We have met before, merchant. Shall we see whether today's figures agree? | None | Missing | No | L1_FRA_007 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_008_s01.wav` | Back to your stall, and ready for | None | Missing | No | L1_FRA_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_008_s02.wav` | . What is your price today? | None | Missing | No | L1_FRA_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_009_s01.wav` | I am here for | None | Missing | No | L1_FRA_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_009_s02.wav` | of | None | Missing | No | L1_FRA_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_009_s03.wav` | . Tell me your price. | None | Missing | No | L1_FRA_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_010_s01.wav` | The | None | Missing | No | L1_FRA_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_010_s02.wav` | interests me, particularly a lot of | None | Missing | No | L1_FRA_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_011_s01.wav` | I want to buy | None | Missing | No | L1_FRA_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_011_s02.wav` | of | None | Missing | No | L1_FRA_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_011_s03.wav` | , if your terms are fair. | None | Missing | No | L1_FRA_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_012_s01.wav` | of | None | Missing | No | L1_FRA_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_012_s02.wav` | ; that is the purchase I have in mind. | None | Missing | No | L1_FRA_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_013_s01.wav` | My present offer is | None | Missing | No | L1_FRA_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_013_s02.wav` | for this lot. | None | Missing | No | L1_FRA_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_014_s01.wav` | I have put | None | Missing | No | L1_FRA_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_014_s02.wav` | on the table. What do you think? | None | Missing | No | L1_FRA_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_015_s01.wav` | For the quantity before us, I offer | None | Missing | No | L1_FRA_015 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_016_s01.wav` | You want my figure? It stands at | None | Missing | No | L1_FRA_016 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_017_s01.wav` | The order I named was | None | Missing | No | L1_FRA_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_017_s02.wav` | of | None | Missing | No | L1_FRA_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_018_s01.wav` | The measure I want is | None | Missing | No | L1_FRA_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_018_s02.wav` | , merchant. | None | Missing | No | L1_FRA_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_019_s01.wav` | For this purchase, set aside | None | Missing | No | L1_FRA_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_019_s02.wav` | of | None | Missing | No | L1_FRA_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_020_s01.wav` | of | None | Missing | No | L1_FRA_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_020_s02.wav` | ; let us keep that measure in view. | None | Missing | No | L1_FRA_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_021_s01.wav` | You know the lot I want. What price do you ask? | None | Missing | No | L1_FRA_021 / AskSellerPrice; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_022_s01.wav` | What would | None | Missing | No | L1_FRA_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_022_s02.wav` | of | None | Missing | No | L1_FRA_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_022_s03.wav` | cost me here? | None | Missing | No | L1_FRA_022 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_023_s01.wav` | Por favor, give me your asking price for the | None | Missing | No | L1_FRA_023 / AskSellerPrice; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_024_s01.wav` | The goods have my attention. Now tell me your price. | None | Missing | No | L1_FRA_024 / AskSellerPrice; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_025_s01.wav` | So the lot is now | None | Missing | No | L1_FRA_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_025_s02.wav` | of | None | Missing | No | L1_FRA_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_025_s03.wav` | . We must price that measure. | None | Missing | No | L1_FRA_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_026_s01.wav` | A different quantity, then: | None | Missing | No | L1_FRA_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_026_s02.wav` | . Let us speak of its price. | None | Missing | No | L1_FRA_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_027_s01.wav` | I hear the change to | None | Missing | No | L1_FRA_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_027_s02.wav` | of | None | Missing | No | L1_FRA_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_027_s03.wav` | . What do you ask for it? | None | Missing | No | L1_FRA_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_028_s01.wav` | Very well, | None | Missing | No | L1_FRA_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_028_s02.wav` | . We can bargain on the new measure. | None | Missing | No | L1_FRA_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_029_s01.wav` | Ora! | None | Missing | No | L1_FRA_029 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_029_s02.wav` | is beyond what this lot can bear. I remain at | None | Missing | No | L1_FRA_029 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_030_s01.wav` | Não, that asking price is too high for me. My offer remains | None | Missing | No | L1_FRA_030 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_031_s01.wav` | A bold figure, but I cannot buy at | None | Missing | No | L1_FRA_031 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_032_s01.wav` | You make a splendid case for your goods. The price still exceeds my reach. | None | Missing | No | L1_FRA_032 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_033_s01.wav` | We may yet find a price, but I remain at | None | Missing | No | L1_FRA_033 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_034_s01.wav` | is above my reach. Could we talk nearer | None | Missing | No | L1_FRA_034 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_035_s01.wav` | I like the lot more than the figure. My offer is | None | Missing | No | L1_FRA_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_036_s01.wav` | That price deserves discussion. I can offer | None | Missing | No | L1_FRA_036 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_036_s02.wav` | at present. | None | Missing | No | L1_FRA_036 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_037_s01.wav` | Ora, that is unexpectedly low. Do you truly mean to ask so little? | None | Missing | No | L1_FRA_037 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_038_s01.wav` | Such a low figure makes me wonder if you have calculated your profit rightly. | None | Missing | No | L1_FRA_038 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_039_s01.wav` | I expected a higher price. Tell me honestly: are you certain of that sum? | None | Missing | No | L1_FRA_039 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_040_s01.wav` | That price surprises me. A seasoned merchant does not usually sell so cheaply. | None | Missing | No | L1_FRA_040 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_041_s01.wav` | I can come to | None | Missing | No | L1_FRA_041 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_041_s02.wav` | for this lot. How does that sound? | None | Missing | No | L1_FRA_041 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_042_s01.wav` | Let me put | None | Missing | No | L1_FRA_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_042_s02.wav` | before you for the | None | Missing | No | L1_FRA_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_043_s01.wav` | Pois bem, I offer | None | Missing | No | L1_FRA_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_043_s02.wav` | . We have room to discuss it. | None | Missing | No | L1_FRA_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_044_s01.wav` | I will bid | None | Missing | No | L1_FRA_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_044_s02.wav` | for | None | Missing | No | L1_FRA_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_044_s03.wav` | of | None | Missing | No | L1_FRA_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_045_s01.wav` | You have won a little ground. I can raise my offer to | None | Missing | No | L1_FRA_045 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_046_s01.wav` | I will move to | None | Missing | No | L1_FRA_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_046_s02.wav` | . Is that nearer your figure? | None | Missing | No | L1_FRA_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_047_s01.wav` | Your argument has some weight. My new offer is | None | Missing | No | L1_FRA_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_048_s01.wav` | I can put | None | Missing | No | L1_FRA_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_048_s02.wav` | on the table now. | None | Missing | No | L1_FRA_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_049_s01.wav` | I can reach | None | Missing | No | L1_FRA_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_049_s02.wav` | now. What do you say? | None | Missing | No | L1_FRA_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_050_s01.wav` | A further step from me: | None | Missing | No | L1_FRA_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_050_s02.wav` | . Consider it. | None | Missing | No | L1_FRA_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_051_s01.wav` | We have talked this through; my offer rises to | None | Missing | No | L1_FRA_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_052_s01.wav` | I will put | None | Missing | No | L1_FRA_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_052_s02.wav` | before you. Are we nearer? | None | Missing | No | L1_FRA_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_053_s01.wav` | My offer remains | None | Missing | No | L1_FRA_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_053_s02.wav` | ; enthusiasm cannot change the sum. | None | Missing | No | L1_FRA_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_054_s01.wav` | I admire your case, but my figure stays at | None | Missing | No | L1_FRA_054 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_055_s01.wav` | No fresh coin from me yet. I am still at | None | Missing | No | L1_FRA_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_056_s01.wav` | The goods tempt me; the price has not moved me. | None | Missing | No | L1_FRA_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_056_s02.wav` | stands. | None | Missing | No | L1_FRA_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_057_s01.wav` | I have not raised my offer. It remains | None | Missing | No | L1_FRA_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_058_s01.wav` | Muito bem. We agree on | None | Missing | No | L1_FRA_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_058_s02.wav` | for | None | Missing | No | L1_FRA_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_058_s03.wav` | of | None | Missing | No | L1_FRA_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_058_s04.wav` | . Please bring the lot. | None | Missing | No | L1_FRA_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_059_s01.wav` | That price works: | None | Missing | No | L1_FRA_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_059_s02.wav` | for | None | Missing | No | L1_FRA_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_059_s03.wav` | of | None | Missing | No | L1_FRA_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_059_s04.wav` | . I await the goods. | None | Missing | No | L1_FRA_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_060_s01.wav` | We have terms at | None | Missing | No | L1_FRA_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_060_s02.wav` | . Bring the | None | Missing | No | L1_FRA_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_060_s03.wav` | of | None | Missing | No | L1_FRA_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_060_s04.wav` | , please. | None | Missing | No | L1_FRA_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_061_s01.wav` | Agreed on the price and measure: | None | Missing | No | L1_FRA_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_061_s02.wav` | for | None | Missing | No | L1_FRA_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_061_s03.wav` | of | None | Missing | No | L1_FRA_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_061_s04.wav` | . Please prepare the delivery. | None | Missing | No | L1_FRA_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_062_s01.wav` | Excellent. | None | Missing | No | L1_FRA_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_062_s02.wav` | for | None | Missing | No | L1_FRA_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_062_s03.wav` | of | None | Missing | No | L1_FRA_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_062_s04.wav` | ; now for the goods. | None | Missing | No | L1_FRA_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_063_s01.wav` | We are agreed at | None | Missing | No | L1_FRA_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_063_s02.wav` | for | None | Missing | No | L1_FRA_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_063_s03.wav` | of | None | Missing | No | L1_FRA_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_063_s04.wav` | . Please bring them. | None | Missing | No | L1_FRA_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_064_s01.wav` | Muito bem. Our price is | None | Missing | No | L1_FRA_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_064_s02.wav` | for | None | Missing | No | L1_FRA_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_064_s03.wav` | of | None | Missing | No | L1_FRA_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_064_s04.wav` | . I await the lot. | None | Missing | No | L1_FRA_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_065_s01.wav` | A bargain, then: | None | Missing | No | L1_FRA_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_065_s02.wav` | of | None | Missing | No | L1_FRA_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_065_s03.wav` | at | None | Missing | No | L1_FRA_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_065_s04.wav` | . Please weigh it out. | None | Missing | No | L1_FRA_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Francisco de Almeida / FRA | `fra_066_s01.wav` | You decline that offer. What figure would tempt you? | None | Missing | No | L1_FRA_066 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_067_s01.wav` | No agreement at that price, then. Let me hear yours. | None | Missing | No | L1_FRA_067 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_068_s01.wav` | So my number does not suit you. We can still discuss another. | None | Missing | No | L1_FRA_068 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_069_s01.wav` | A refusal, understood. What price do you propose instead? | None | Missing | No | L1_FRA_069 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_070_s01.wav` | I must judge the quality and what I can earn when I trade onward. That gives me | None | Missing | No | L1_FRA_070 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_071_s01.wav` | There is promise in the | None | Missing | No | L1_FRA_071 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_071_s02.wav` | , but I still have costs after buying. I offer | None | Missing | No | L1_FRA_071 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_072_s01.wav` | My figure of | None | Missing | No | L1_FRA_072 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_072_s02.wav` | leaves room for risk as well as profit. | None | Missing | No | L1_FRA_072 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_073_s01.wav` | I am keen to buy, not to pay blindly. The measure leads me to | None | Missing | No | L1_FRA_073 / OfferReason; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_099_s01.wav` | That is outside my knowledge, I fear. Shall we return to the | None | Missing | No | L1_FRA_099 / OffTopic; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_100_s01.wav` | You have taken me by surprise. The lot before us is easier to judge. | None | Missing | No | L1_FRA_100 / OffTopic; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_101_s01.wav` | A curious thought, but I cannot answer it honestly. | None | Missing | No | L1_FRA_101 / OffTopic; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_102_s01.wav` | That is a tale for someone else. I can speak for my offer. | None | Missing | No | L1_FRA_102 / OffTopic; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_103_s01.wav` | I came ready to bargain, not to trade insults. | None | Missing | No | L1_FRA_103 / HostileWarning; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_104_s01.wav` | A sharp price I can discuss. Sharp words are another matter. | None | Missing | No | L1_FRA_104 / HostileWarning; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_105_s01.wav` | Speak courteously, merchant, and we may yet do business. | None | Missing | No | L1_FRA_105 / HostileWarning; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_106_s01.wav` | Por favor, help me with this point: | None | Missing | No | L1_FRA_106 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Francisco de Almeida / FRA | `fra_107_s01.wav` | I would rather ask twice than mistake your terms. | None | Missing | No | L1_FRA_107 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Francisco de Almeida / FRA | `fra_108_s01.wav` | The market swallowed part of your words. | None | Missing | No | L1_FRA_108 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Francisco de Almeida / FRA | `fra_109_s01.wav` | Before I answer, make this clear for me: | None | Missing | No | L1_FRA_109 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Francisco de Almeida / FRA | `fra_111_s01.wav` | Obrigado, merchant. The | None | Missing | No | L1_FRA_111 / TransactionSuccess; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_111_s02.wav` | is here as promised; a fine exchange. | None | Missing | No | L1_FRA_111 / TransactionSuccess; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_112_s01.wav` | Goods received and fairly measured. You have my thanks. | None | Missing | No | L1_FRA_112 / TransactionSuccess; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_113_s01.wav` | The lot has arrived. A lively bargain, and one worth making. | None | Missing | No | L1_FRA_113 / TransactionSuccess; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_114_s01.wav` | Now the trade is complete. Thank you for keeping the terms we made. | None | Missing | No | L1_FRA_114 / TransactionSuccess; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_115_s01.wav` | We have no bargain today. I will seek another stall. | None | Missing | No | L1_FRA_115 / TransactionFailure; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_116_s01.wav` | Our figures did not meet. I must take my business elsewhere. | None | Missing | No | L1_FRA_116 / TransactionFailure; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_117_s01.wav` | I came hoping for this lot, but I leave without it. | None | Missing | No | L1_FRA_117 / TransactionFailure; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_118_s01.wav` | No sale between us today. I will move on. | None | Missing | No | L1_FRA_118 / TransactionFailure; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_122_s01.wav` | Merchant? I am here when you are ready to continue. | None | Missing | No | L1_FRA_122 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_123_s01.wav` | The | None | Missing | No | L1_FRA_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_123_s02.wav` | still has my interest. Shall we talk? | None | Missing | No | L1_FRA_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_124_s01.wav` | Take your moment; then tell me your figure. | None | Missing | No | L1_FRA_124 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_125_s01.wav` | I have waited a while. Shall we settle the price? | None | Missing | No | L1_FRA_125 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_126_s01.wav` | Please tell me where we stand; I have other stalls to visit. | None | Missing | No | L1_FRA_126 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_127_s01.wav` | The lot is still before us. What is your answer? | None | Missing | No | L1_FRA_127 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_128_s01.wav` | You bargain with spirit, but I remain at | None | Missing | No | L1_FRA_128 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_129_s01.wav` | I enjoy the contest. My offer is still | None | Missing | No | L1_FRA_129 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_130_s01.wav` | The conversation is good; my number has not changed from | None | Missing | No | L1_FRA_130 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_131_s01.wav` | You make this lively, merchant. | None | Missing | No | L1_FRA_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_131_s02.wav` | stands. | None | Missing | No | L1_FRA_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_132_s01.wav` | I hope we find a bargain, though I am holding at | None | Missing | No | L1_FRA_132 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_133_s01.wav` | My offer remains | None | Missing | No | L1_FRA_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_133_s02.wav` | . What figure do you propose? | None | Missing | No | L1_FRA_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_134_s01.wav` | I have weighed it. | None | Missing | No | L1_FRA_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_134_s02.wav` | still stands. | None | Missing | No | L1_FRA_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_135_s01.wav` | There is no change to my offer: | None | Missing | No | L1_FRA_135 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_136_s01.wav` | I am still at | None | Missing | No | L1_FRA_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_136_s02.wav` | for this measure. | None | Missing | No | L1_FRA_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_137_s01.wav` | The price before you is | None | Missing | No | L1_FRA_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_137_s02.wav` | ; I have not altered it. | None | Missing | No | L1_FRA_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_138_s01.wav` | The quality may be sound. My offer remains | None | Missing | No | L1_FRA_138 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_139_s01.wav` | I cannot justify more; | None | Missing | No | L1_FRA_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_139_s02.wav` | stands. | None | Missing | No | L1_FRA_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_140_s01.wav` | I have kept my figure at | None | Missing | No | L1_FRA_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_141_s01.wav` | I have listened. I am still offering | None | Missing | No | L1_FRA_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_142_s01.wav` | My figure stays at | None | Missing | No | L1_FRA_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_142_s02.wav` | . Give me another reason to reconsider. | None | Missing | No | L1_FRA_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_143_s01.wav` | I am still at | None | Missing | No | L1_FRA_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_143_s02.wav` | . Your answer? | None | Missing | No | L1_FRA_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_144_s01.wav` | No new figure from me. | None | Missing | No | L1_FRA_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_144_s02.wav` | stands. | None | Missing | No | L1_FRA_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_145_s01.wav` | , unchanged. Please decide. | None | Missing | No | L1_FRA_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_146_s01.wav` | I have not raised it. | None | Missing | No | L1_FRA_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_146_s02.wav` | is my offer. | None | Missing | No | L1_FRA_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_147_s01.wav` | The offer remains | None | Missing | No | L1_FRA_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_147_s02.wav` | . I need a reply. | None | Missing | No | L1_FRA_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_148_s01.wav` | My time is short; I remain at | None | Missing | No | L1_FRA_148 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_149_s01.wav` | The offer has not changed: | None | Missing | No | L1_FRA_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_149_s02.wav` | . Please decide soon. | None | Missing | No | L1_FRA_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_150_s01.wav` | I can wait for an answer, not another long silence. | None | Missing | No | L1_FRA_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_150_s02.wav` | stands. | None | Missing | No | L1_FRA_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_151_s01.wav` | My number has not moved: | None | Missing | No | L1_FRA_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_151_s02.wav` | . Do you wish to answer? | None | Missing | No | L1_FRA_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_152_s01.wav` | The figure is still | None | Missing | No | L1_FRA_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_152_s02.wav` | . Be direct with me. | None | Missing | No | L1_FRA_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_153_s01.wav` | is beyond me, and time is short. | None | Missing | No | L1_FRA_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_154_s01.wav` | That asking price remains too high. I need a workable figure soon. | None | Missing | No | L1_FRA_154 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_155_s01.wav` | I cannot pay | None | Missing | No | L1_FRA_155 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_155_s02.wav` | . Please answer plainly. | None | Missing | No | L1_FRA_155 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Francisco de Almeida / FRA | `fra_156_s01.wav` | I can raise my offer to | None | Missing | No | L1_FRA_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_156_s02.wav` | . Please decide soon. | None | Missing | No | L1_FRA_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_157_s01.wav` | My new figure is | None | Missing | No | L1_FRA_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_157_s02.wav` | . I cannot wait long. | None | Missing | No | L1_FRA_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_158_s01.wav` | A quicker step from me: | None | Missing | No | L1_FRA_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_158_s02.wav` | . Shall we decide? | None | Missing | No | L1_FRA_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_162_s01.wav` | Our prices do not meet. I must leave now. | None | Missing | No | L1_FRA_162 / TransactionFailure; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_163_s01.wav` | This lot has held me long enough. I will try another stall. | None | Missing | No | L1_FRA_163 / TransactionFailure; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_164_s01.wav` | No bargain today. I must move on. | None | Missing | No | L1_FRA_164 / TransactionFailure; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_001_s01.wav` | Com licença, merchant. I am Father Penteado. I wish to buy | None | Missing | No | L1_FAT_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_001_s02.wav` | of | None | Missing | No | L1_FAT_001 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_002_s01.wav` | Good day. I am Father Penteado, looking for | None | Missing | No | L1_FAT_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_002_s02.wav` | of | None | Missing | No | L1_FAT_002 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_003_s01.wav` | Your stall has my attention. I am seeking | None | Missing | No | L1_FAT_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_003_s02.wav` | of | None | Missing | No | L1_FAT_003 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_004_s01.wav` | I am Father Penteado. I would like | None | Missing | No | L1_FAT_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_004_s02.wav` | of | None | Missing | No | L1_FAT_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_004_s03.wav` | , if the measure is sound. | None | Missing | No | L1_FAT_004 / CustomerGreeting; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_005_s01.wav` | We have spoken before. What is your price for | None | Missing | No | L1_FAT_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_005_s02.wav` | today? | None | Missing | No | L1_FAT_005 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_006_s01.wav` | Your stall is familiar to me now. Let us consider this lot on its own merits. | None | Missing | No | L1_FAT_006 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_007_s01.wav` | Good to see you again, merchant. What do you ask for this measure? | None | Missing | No | L1_FAT_007 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_008_s01.wav` | I have returned for your | None | Missing | No | L1_FAT_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_008_s02.wav` | . Shall we begin afresh? | None | Missing | No | L1_FAT_008 / RepeatCustomerGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_009_s01.wav` | I seek | None | Missing | No | L1_FAT_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_009_s02.wav` | of | None | Missing | No | L1_FAT_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_009_s03.wav` | . What is your price? | None | Missing | No | L1_FAT_009 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_010_s01.wav` | of | None | Missing | No | L1_FAT_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_010_s02.wav` | is what I came to buy. | None | Missing | No | L1_FAT_010 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_011_s01.wav` | It is the | None | Missing | No | L1_FAT_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_011_s02.wav` | I need, in a measure of | None | Missing | No | L1_FAT_011 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_012_s01.wav` | My interest is in | None | Missing | No | L1_FAT_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_012_s02.wav` | of | None | Missing | No | L1_FAT_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_012_s03.wav` | , provided we can agree on terms. | None | Missing | No | L1_FAT_012 / AskWhatBuyerWants; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_013_s01.wav` | For this lot, my present offer is | None | Missing | No | L1_FAT_013 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_014_s01.wav` | I have allowed | None | Missing | No | L1_FAT_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_014_s02.wav` | for this purchase. | None | Missing | No | L1_FAT_014 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_015_s01.wav` | The figure I can offer now is | None | Missing | No | L1_FAT_015 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_016_s01.wav` | I am considering | None | Missing | No | L1_FAT_016 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_016_s02.wav` | for the measure before us. | None | Missing | No | L1_FAT_016 / AskBuyerBudget; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_017_s01.wav` | The measure I ask for is | None | Missing | No | L1_FAT_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_017_s02.wav` | of | None | Missing | No | L1_FAT_017 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_018_s01.wav` | I need | None | Missing | No | L1_FAT_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_018_s02.wav` | of | None | Missing | No | L1_FAT_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_018_s03.wav` | ; no more for this purchase. | None | Missing | No | L1_FAT_018 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_019_s01.wav` | is the quantity I have in mind. | None | Missing | No | L1_FAT_019 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_020_s01.wav` | I am looking for | None | Missing | No | L1_FAT_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_020_s02.wav` | of | None | Missing | No | L1_FAT_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_020_s03.wav` | for this purchase. | None | Missing | No | L1_FAT_020 / AskQuantity; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_021_s01.wav` | Com licença, what would you ask for | None | Missing | No | L1_FAT_021 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_021_s02.wav` | of | None | Missing | No | L1_FAT_021 / AskSellerPrice; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_022_s01.wav` | I have named the measure. What price do you put on it? | None | Missing | No | L1_FAT_022 / AskSellerPrice; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_023_s01.wav` | Tell me your asking price for the | None | Missing | No | L1_FAT_023 / AskSellerPrice; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_023_s02.wav` | , please. | None | Missing | No | L1_FAT_023 / AskSellerPrice; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_024_s01.wav` | What sum would make this lot mine, if we agree? | None | Missing | No | L1_FAT_024 / AskSellerPrice; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_025_s01.wav` | I understand: the measure is now | None | Missing | No | L1_FAT_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_025_s02.wav` | of | None | Missing | No | L1_FAT_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_025_s03.wav` | . Let us price that lot. | None | Missing | No | L1_FAT_025 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_026_s01.wav` | The quantity has changed to | None | Missing | No | L1_FAT_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_026_s02.wav` | . We should consider the price anew. | None | Missing | No | L1_FAT_026 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_027_s01.wav` | Very well, | None | Missing | No | L1_FAT_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_027_s02.wav` | of | None | Missing | No | L1_FAT_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_027_s03.wav` | . What do you ask for that measure? | None | Missing | No | L1_FAT_027 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_028_s01.wav` | I have the new quantity: | None | Missing | No | L1_FAT_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_028_s02.wav` | . Let us speak of its value. | None | Missing | No | L1_FAT_028 / QuantityChanged; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_029_s01.wav` | is more than I can justify for this lot. | None | Missing | No | L1_FAT_029 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Father Penteado / FAT | `fat_030_s01.wav` | I respect your asking price, but I cannot buy at | None | Missing | No | L1_FAT_030 / SellerPriceTooHigh; Any mood / Any patience | offers above 999 unresolved |
| Father Penteado / FAT | `fat_031_s01.wav` | That sum is beyond what I will pay. Perhaps we can find firmer ground. | None | Missing | No | L1_FAT_031 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_032_s01.wav` | The measure before us does not support that price. I cannot accept it. | None | Missing | No | L1_FAT_032 / SellerPriceTooHigh; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_033_s01.wav` | We are near a workable price, though | None | Missing | No | L1_FAT_033 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Father Penteado / FAT | `fat_033_s02.wav` | is still above my reach. | None | Missing | No | L1_FAT_033 / SellerPriceSlightlyHigh; Any mood / Any patience | offers above 999 unresolved |
| Father Penteado / FAT | `fat_034_s01.wav` | I can consider the lot, but my figure is | None | Missing | No | L1_FAT_034 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_035_s01.wav` | Your price is close. Could we speak nearer | None | Missing | No | L1_FAT_035 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_036_s01.wav` | There may be room between us. My offer remains | None | Missing | No | L1_FAT_036 / SellerPriceSlightlyHigh; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_037_s01.wav` | That figure is unusually low. Are these truly the terms you intend? | None | Missing | No | L1_FAT_037 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_038_s01.wav` | I expected a higher price. A sum so modest naturally invites caution. | None | Missing | No | L1_FAT_038 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_039_s01.wav` | You have surprised me with that sum. Why do you offer it for so little? | None | Missing | No | L1_FAT_039 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_040_s01.wav` | A low price calls for care. Tell me, merchant, are you certain of this figure? | None | Missing | No | L1_FAT_040 / SellerPriceBelowExpected; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_041_s01.wav` | Let me offer | None | Missing | No | L1_FAT_041 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_041_s02.wav` | for this measure. Would you consider it? | None | Missing | No | L1_FAT_041 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_042_s01.wav` | I can begin at | None | Missing | No | L1_FAT_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_042_s02.wav` | for the | None | Missing | No | L1_FAT_042 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_043_s01.wav` | My first answer is | None | Missing | No | L1_FAT_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_043_s02.wav` | . Tell me how you see it. | None | Missing | No | L1_FAT_043 / BuyerCounterFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_044_s01.wav` | I would put | None | Missing | No | L1_FAT_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_044_s02.wav` | before you for | None | Missing | No | L1_FAT_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_044_s03.wav` | of | None | Missing | No | L1_FAT_044 / BuyerCounterFirst; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_045_s01.wav` | I have considered your words. I can raise my offer to | None | Missing | No | L1_FAT_045 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_046_s01.wav` | Let me move to | None | Missing | No | L1_FAT_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_046_s02.wav` | . Does that bring us closer? | None | Missing | No | L1_FAT_046 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_047_s01.wav` | There is merit in your argument. My new figure is | None | Missing | No | L1_FAT_047 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_048_s01.wav` | I can offer | None | Missing | No | L1_FAT_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_048_s02.wav` | now; weigh it against your asking price. | None | Missing | No | L1_FAT_048 / BuyerCounterMiddle; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_049_s01.wav` | After considering the lot, I can offer | None | Missing | No | L1_FAT_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_049_s02.wav` | . What is your answer? | None | Missing | No | L1_FAT_049 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_050_s01.wav` | I will take another step to | None | Missing | No | L1_FAT_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_050_s02.wav` | . Can we agree there? | None | Missing | No | L1_FAT_050 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_051_s01.wav` | My offer rises to | None | Missing | No | L1_FAT_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_051_s02.wav` | . I hope it gives us room to settle. | None | Missing | No | L1_FAT_051 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_052_s01.wav` | I can put | None | Missing | No | L1_FAT_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_052_s02.wav` | before you at this point in our bargaining. | None | Missing | No | L1_FAT_052 / BuyerCounterFinal; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_053_s01.wav` | I have thought it through. My offer remains | None | Missing | No | L1_FAT_053 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_054_s01.wav` | Your reasons are heard; my figure is still | None | Missing | No | L1_FAT_054 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_055_s01.wav` | I cannot increase the offer now. | None | Missing | No | L1_FAT_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_055_s02.wav` | stands. | None | Missing | No | L1_FAT_055 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_056_s01.wav` | We may discuss the quality further, but my offer has not moved from | None | Missing | No | L1_FAT_056 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_057_s01.wav` | The same sum remains before you: | None | Missing | No | L1_FAT_057 / BuyerHoldsFirm; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_058_s01.wav` | We agree on | None | Missing | No | L1_FAT_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_058_s02.wav` | for | None | Missing | No | L1_FAT_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_058_s03.wav` | of | None | Missing | No | L1_FAT_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_058_s04.wav` | . May I have the goods? | None | Missing | No | L1_FAT_058 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_059_s01.wav` | That price is fair to me: | None | Missing | No | L1_FAT_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_059_s02.wav` | for | None | Missing | No | L1_FAT_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_059_s03.wav` | of | None | Missing | No | L1_FAT_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_059_s04.wav` | . I await the lot. | None | Missing | No | L1_FAT_059 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_060_s01.wav` | Then our terms are | None | Missing | No | L1_FAT_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_060_s02.wav` | of | None | Missing | No | L1_FAT_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_060_s03.wav` | at | None | Missing | No | L1_FAT_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_060_s04.wav` | . Please fill the bag. | None | Missing | No | L1_FAT_060 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_061_s01.wav` | I accept those terms: | None | Missing | No | L1_FAT_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_061_s02.wav` | for | None | Missing | No | L1_FAT_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_061_s03.wav` | of | None | Missing | No | L1_FAT_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_061_s04.wav` | . Please prepare the goods. | None | Missing | No | L1_FAT_061 / SellerPriceAccepted; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_062_s01.wav` | Agreed: | None | Missing | No | L1_FAT_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_062_s02.wav` | of | None | Missing | No | L1_FAT_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_062_s03.wav` | for | None | Missing | No | L1_FAT_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_062_s04.wav` | . I await the goods. | None | Missing | No | L1_FAT_062 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_063_s01.wav` | We have a price of | None | Missing | No | L1_FAT_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_063_s02.wav` | for | None | Missing | No | L1_FAT_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_063_s03.wav` | of | None | Missing | No | L1_FAT_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_063_s04.wav` | . Please bring the lot. | None | Missing | No | L1_FAT_063 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_064_s01.wav` | Then the terms are clear: | None | Missing | No | L1_FAT_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_064_s02.wav` | of | None | Missing | No | L1_FAT_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_064_s03.wav` | . I will wait while you fill the bag. | None | Missing | No | L1_FAT_064 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_065_s01.wav` | I accept that bargain. Bring | None | Missing | No | L1_FAT_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_065_s02.wav` | of | None | Missing | No | L1_FAT_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_065_s03.wav` | for | None | Missing | No | L1_FAT_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_065_s04.wav` | , please. | None | Missing | No | L1_FAT_065 / PlayerAcceptedDeal; Any mood / Any patience | quantity suffix undecided |
| Father Penteado / FAT | `fat_066_s01.wav` | You decline that figure. What price would you consider instead? | None | Missing | No | L1_FAT_066 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_067_s01.wav` | My offer does not suit you, I understand. Tell me what might. | None | Missing | No | L1_FAT_067 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_068_s01.wav` | We have not agreed at that sum. I am willing to hear another. | None | Missing | No | L1_FAT_068 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_069_s01.wav` | A refusal is clear enough. What figure do you have in mind? | None | Missing | No | L1_FAT_069 / PlayerRejectedBuyerOffer; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_070_s01.wav` | I have weighed the quantity against its quality. That brings me to | None | Missing | No | L1_FAT_070 / OfferReason; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_071_s01.wav` | A fair price must answer for the measure as well as the fragrance. I offer | None | Missing | No | L1_FAT_071 / OfferReason; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_072_s01.wav` | I am trying to pay for what is here, neither more nor less: | None | Missing | No | L1_FAT_072 / OfferReason; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_073_s01.wav` | The lot looks promising, but I must judge it carefully. My figure is | None | Missing | No | L1_FAT_073 / OfferReason; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_099_s01.wav` | I cannot answer that well, and I would rather be honest about it. | None | Missing | No | L1_FAT_099 / OffTopic; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_100_s01.wav` | That is beyond what I know. Perhaps we can return to the matter before us. | None | Missing | No | L1_FAT_100 / OffTopic; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_101_s01.wav` | An interesting question, but I should not pretend to have an answer. | None | Missing | No | L1_FAT_101 / OffTopic; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_102_s01.wav` | I can speak to this purchase; on that subject I would be guessing. | None | Missing | No | L1_FAT_102 / OffTopic; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_103_s01.wav` | I will hear a hard price, merchant, but not an insult. | None | Missing | No | L1_FAT_103 / HostileWarning; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_104_s01.wav` | Let us speak plainly and with respect, or we shall get nowhere. | None | Missing | No | L1_FAT_104 / HostileWarning; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_105_s01.wav` | I have come to bargain in good faith. Please address me with the same care. | None | Missing | No | L1_FAT_105 / HostileWarning; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_106_s01.wav` | Perdão, I would rather ask than mistake your meaning. | None | Missing | No | L1_FAT_106 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Father Penteado / FAT | `fat_107_s01.wav` | I missed an important part of that. | None | Missing | No | L1_FAT_107 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Father Penteado / FAT | `fat_108_s01.wav` | Forgive me, I need the point made clear. | None | Missing | No | L1_FAT_108 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Father Penteado / FAT | `fat_109_s01.wav` | Before I answer, please clarify this: | None | Missing | No | L1_FAT_109 / UnclearSpeech; Any mood / Any patience | unbounded rule reply |
| Father Penteado / FAT | `fat_111_s01.wav` | Muito obrigado. The goods are here as agreed; you have my thanks. | None | Missing | No | L1_FAT_111 / TransactionSuccess; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_112_s01.wav` | The measure has been delivered, and the terms kept. Thank you, merchant. | None | Missing | No | L1_FAT_112 / TransactionSuccess; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_113_s01.wav` | I have received the | None | Missing | No | L1_FAT_113 / TransactionSuccess; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_113_s02.wav` | . A fair exchange, carefully made. | None | Missing | No | L1_FAT_113 / TransactionSuccess; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_114_s01.wav` | The purchase is complete. I appreciate your honest measure. | None | Missing | No | L1_FAT_114 / TransactionSuccess; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_115_s01.wav` | We could not agree. I will look elsewhere for this lot. | None | Missing | No | L1_FAT_115 / TransactionFailure; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_116_s01.wav` | I must leave without the | None | Missing | No | L1_FAT_116 / TransactionFailure; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_116_s02.wav` | today. I wish you well. | None | Missing | No | L1_FAT_116 / TransactionFailure; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_117_s01.wav` | Our figures have not met. I shall take my business to another stall. | None | Missing | No | L1_FAT_117 / TransactionFailure; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_118_s01.wav` | There is no sale between us. I must take my leave. | None | Missing | No | L1_FAT_118 / TransactionFailure; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_122_s01.wav` | Merchant? I am ready when you are. | None | Missing | No | L1_FAT_122 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_123_s01.wav` | I am still considering the | None | Missing | No | L1_FAT_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_123_s02.wav` | . Shall we continue? | None | Missing | No | L1_FAT_123 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_124_s01.wav` | Take a moment, then tell me your thought on the price. | None | Missing | No | L1_FAT_124 / IdleReminderFirst; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_125_s01.wav` | We have let this matter rest a while. What is your answer? | None | Missing | No | L1_FAT_125 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_126_s01.wav` | I would appreciate a decision on the lot now. | None | Missing | No | L1_FAT_126 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_127_s01.wav` | The measure is before us. Shall we settle on a price? | None | Missing | No | L1_FAT_127 / IdleReminderSecond; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_128_s01.wav` | I appreciate the conversation; my offer remains | None | Missing | No | L1_FAT_128 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_129_s01.wav` | You make a thoughtful case. I am still at | None | Missing | No | L1_FAT_129 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_130_s01.wav` | We can speak freely, though my figure stays | None | Missing | No | L1_FAT_130 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_131_s01.wav` | I hope we find agreement. For now, | None | Missing | No | L1_FAT_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_131_s02.wav` | stands. | None | Missing | No | L1_FAT_131 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_132_s01.wav` | I am listening, but I have not moved above | None | Missing | No | L1_FAT_132 / BuyerHoldsFirm; Friendly mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_133_s01.wav` | My offer is unchanged at | None | Missing | No | L1_FAT_133 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_134_s01.wav` | After considering it, I remain at | None | Missing | No | L1_FAT_134 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_135_s01.wav` | The figure before you is still | None | Missing | No | L1_FAT_135 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_136_s01.wav` | I can listen further. I cannot increase | None | Missing | No | L1_FAT_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_136_s02.wav` | now. | None | Missing | No | L1_FAT_136 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_137_s01.wav` | There is no new offer from me: | None | Missing | No | L1_FAT_137 / BuyerHoldsFirm; Normal mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_138_s01.wav` | My offer remains | None | Missing | No | L1_FAT_138 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_138_s02.wav` | . Give me a reason to reconsider. | None | Missing | No | L1_FAT_138 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_139_s01.wav` | I have heard your claim; | None | Missing | No | L1_FAT_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_139_s02.wav` | still stands. | None | Missing | No | L1_FAT_139 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_140_s01.wav` | The measure does not warrant a higher bid from me. | None | Missing | No | L1_FAT_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_140_s02.wav` | remains. | None | Missing | No | L1_FAT_140 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_141_s01.wav` | I am not persuaded to change | None | Missing | No | L1_FAT_141 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_142_s01.wav` | Let us keep the figures exact. Mine is still | None | Missing | No | L1_FAT_142 / BuyerHoldsFirm; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_143_s01.wav` | My offer is still | None | Missing | No | L1_FAT_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_143_s02.wav` | . Your answer, please. | None | Missing | No | L1_FAT_143 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_144_s01.wav` | No change: | None | Missing | No | L1_FAT_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_144_s02.wav` | . Can you decide? | None | Missing | No | L1_FAT_144 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_145_s01.wav` | I remain at | None | Missing | No | L1_FAT_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_145_s02.wav` | . Please be direct. | None | Missing | No | L1_FAT_145 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_146_s01.wav` | I have not changed the figure. Please consider | None | Missing | No | L1_FAT_146 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_147_s01.wav` | The same figure, | None | Missing | No | L1_FAT_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_147_s02.wav` | . What do you say? | None | Missing | No | L1_FAT_147 / BuyerHoldsFirm; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_148_s01.wav` | Time is short. My offer remains | None | Missing | No | L1_FAT_148 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_149_s01.wav` | The figure has not moved from | None | Missing | No | L1_FAT_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_149_s02.wav` | . Please answer soon. | None | Missing | No | L1_FAT_149 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_150_s01.wav` | I am still at | None | Missing | No | L1_FAT_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_150_s02.wav` | . I need your decision. | None | Missing | No | L1_FAT_150 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_151_s01.wav` | No new offer: | None | Missing | No | L1_FAT_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_151_s02.wav` | . Let us be clear. | None | Missing | No | L1_FAT_151 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_152_s01.wav` | stands. I cannot wait much longer for an answer. | None | Missing | No | L1_FAT_152 / BuyerHoldsFirm; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_153_s01.wav` | is too high. I need a workable figure soon. | None | Missing | No | L1_FAT_153 / SellerPriceTooHigh; Any mood / Low patience | offers above 999 unresolved |
| Father Penteado / FAT | `fat_154_s01.wav` | I cannot pay that sum. Please be clear about your price. | None | Missing | No | L1_FAT_154 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_155_s01.wav` | The asking price is beyond me, and I have little time left. | None | Missing | No | L1_FAT_155 / SellerPriceTooHigh; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_156_s01.wav` | I can move to | None | Missing | No | L1_FAT_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_156_s02.wav` | . Please answer soon. | None | Missing | No | L1_FAT_156 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_157_s01.wav` | My new offer is | None | Missing | No | L1_FAT_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_157_s02.wav` | . Can we decide? | None | Missing | No | L1_FAT_157 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_158_s01.wav` | I have raised it to | None | Missing | No | L1_FAT_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_158_s02.wav` | . I need your answer. | None | Missing | No | L1_FAT_158 / BuyerCounterMiddle; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_162_s01.wav` | We have no agreement. I must leave now. | None | Missing | No | L1_FAT_162 / TransactionFailure; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_163_s01.wav` | I can wait no longer. I will seek another stall. | None | Missing | No | L1_FAT_163 / TransactionFailure; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_164_s01.wav` | Our figures remain apart. I must go without the goods. | None | Missing | No | L1_FAT_164 / TransactionFailure; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Lakshmamma / LAK | `lak_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Lakshmamma / LAK | `lak_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Chinnamma Naik / CHI | `chi_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Chinnamma Naik / CHI | `chi_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Saraswati Chetti / SAR | `sar_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Saraswati Chetti / SAR | `sar_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Abdul Rahman / ABD | `abd_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Abdul Rahman / ABD | `abd_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Francisco de Almeida / FRA | `fra_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Francisco de Almeida / FRA | `fra_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_num_1.wav` | one | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_2.wav` | two | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_3.wav` | three | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_4.wav` | four | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_5.wav` | five | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_6.wav` | six | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_7.wav` | seven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_8.wav` | eight | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_9.wav` | nine | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_10.wav` | ten | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_11.wav` | eleven | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_12.wav` | twelve | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_13.wav` | thirteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_14.wav` | fourteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_15.wav` | fifteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_16.wav` | sixteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_17.wav` | seventeen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_18.wav` | eighteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_19.wav` | nineteen | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_20.wav` | twenty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_30.wav` | thirty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_40.wav` | forty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_50.wav` | fifty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_60.wav` | sixty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_70.wav` | seventy | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_80.wav` | eighty | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_90.wav` | ninety | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_100.wav` | one hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_200.wav` | two hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_300.wav` | three hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_400.wav` | four hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_500.wav` | five hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_600.wav` | six hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_700.wav` | seven hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_800.wav` | eight hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_num_900.wav` | nine hundred | None | Missing | No | Number Library; neutral reusable join | adapter required; 1–999 composition only |
| Father Penteado / FAT | `fat_spice_pepper.wav` | pepper | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_spice_cinnamon.wav` | cinnamon | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_spice_cardamom.wav` | cardamom | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_currency_varahas.wav` | varahas | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_seer.wav` | Seer | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_seers.wav` | Seers | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_veesai.wav` | Veesai | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_palam.wav` | Palam | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_palams.wav` | Palams | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_manangu.wav` | Manangu | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_manangus.wav` | Manangus | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_bahar.wav` | Bahar | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |
| Father Penteado / FAT | `fat_unit_bahars.wav` | Bahars | None | Missing | No | Dynamic Vocabulary; neutral reusable join | adapter required |

## P1 generation checklist — confirmed incorrect speech

**None confirmed.** No existing name-bearing Level 1 clip was found to replace, and this environment could not audition existing WAVs. Existing file presence/mapping is verified; spoken-content correctness is not. Promote a listening-hold entry to P1 only after hearing a mismatch; record its exact required replacement script from the comparison table, retaining its established actor voice. This is not permission to replace every old filename or every line adjacent to a rename.

## P2 optional / inactive recording checklist

| Speaker / voice | Planned output file | Exact words to record | Existing WAV | Status | Unity audio mapping | Context / delivery | Assembly hold |
|---|---|---|---|---|---|---|---|
| Lakshmamma / LAK | `lak_105_s01.wav` | I am Lakshmamma. And you, merchant? | None | Missing | No | L1_LAK_105 / AskName; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_106_s01.wav` | Lakshmamma. That is what you may call me. | None | Missing | No | L1_LAK_106 / AskName; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_107_s01.wav` | My name is Lakshmamma. It is good to meet you. | None | Missing | No | L1_LAK_107 / AskName; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_108_s01.wav` | You may call me Lakshmamma. Now we are properly acquainted. | None | Missing | No | L1_LAK_108 / AskName; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_109_s01.wav` | I am from here, in Vijayanagara. This is my local market. | None | Missing | No | L1_LAK_109 / AskOrigin; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_110_s01.wav` | No long journey for me. I live here in Vijayanagara. | None | Missing | No | L1_LAK_110 / AskOrigin; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_111_s01.wav` | I am a local woman, merchant. I know this market well. | None | Missing | No | L1_LAK_111 / AskOrigin; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_112_s01.wav` | Here in Vijayanagara is home. I have done plenty of buying in these streets. | None | Missing | No | L1_LAK_112 / AskOrigin; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_113_s01.wav` | Whatever the weather brings, there is still cooking to be done! | None | Missing | No | L1_LAK_113 / WeatherQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_114_s01.wav` | I would rather bargain in the shade, if I have the choice. | None | Missing | No | L1_LAK_114 / WeatherQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_115_s01.wav` | Rain or sunshine, the household still needs its provisions. | None | Missing | No | L1_LAK_115 / WeatherQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_116_s01.wav` | The sky keeps its own counsel. My errands do not wait for it. | None | Missing | No | L1_LAK_116 / WeatherQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_117_s01.wav` | Krishnadevaraya is our king. I leave court matters to those who know them; I know this market. | None | Missing | No | L1_LAK_117 / HistoryKing; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_118_s01.wav` | Our king is Krishnadevaraya. As for court gossip, you should ask someone nearer the palace. | None | Missing | No | L1_LAK_118 / HistoryKing; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_119_s01.wav` | Krishnadevaraya rules here. My own concerns are rather smaller: the house and today's buying. | None | Missing | No | L1_LAK_119 / HistoryKing; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_120_s01.wav` | You mean King Krishnadevaraya? I am no court scholar, merchant, only a woman doing her buying. | None | Missing | No | L1_LAK_120 / HistoryKing; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_121_s01.wav` | Virupaksha's temple has a bazaar before it. Worship and daily buying bring many feet to those streets. | None | Missing | No | L1_LAK_121 / HistoryTemple; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_122_s01.wav` | You will find plenty of buying and selling by Virupaksha's temple. A temple street is no quiet place! | None | Missing | No | L1_LAK_122 / HistoryTemple; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_123_s01.wav` | Virupaksha's temple is part of our city's life. So are the traders in its bazaar. | None | Missing | No | L1_LAK_123 / HistoryTemple; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_124_s01.wav` | The bazaar by Virupaksha's temple is full of daily business. I know the buying better than the grand stories. | None | Missing | No | L1_LAK_124 / HistoryTemple; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_125_s01.wav` | Vijayanagara is home to me, merchant. Temples, markets, houses; people going about their work. | None | Missing | No | L1_LAK_125 / HistoryQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_126_s01.wav` | These bazaars are part of daily life here. Not everyone comes looking for wonders; some of us need provisions! | None | Missing | No | L1_LAK_126 / HistoryQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_127_s01.wav` | If you want to know a city, listen in its market. Everyone has something to say. | None | Missing | No | L1_LAK_127 / HistoryQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_128_s01.wav` | There is much to see in Vijayanagara. I know its market stalls better than its palaces. | None | Missing | No | L1_LAK_128 / HistoryQuestion; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_129_s01.wav` | It is good to exchange a kind word, merchant. | None | Missing | No | L1_LAK_129 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_130_s01.wav` | Sari, let us speak as neighbours. There is no need for long faces. | None | Missing | No | L1_LAK_130 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_131_s01.wav` | A little friendliness makes the buying easier, does it not? | None | Missing | No | L1_LAK_131 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_132_s01.wav` | I am glad of your company. Buying need not be a quarrel. | None | Missing | No | L1_LAK_132 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_133_s01.wav` | You are kind to ask. I hope your day goes well too. | None | Missing | No | L1_LAK_133 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_134_s01.wav` | Ah, a friendly word! That costs neither of us anything. | None | Missing | No | L1_LAK_134 / SocialGreeting; Any mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_164_s01.wav` | I have other errands, merchant. Let us come to an answer. | None | Missing | No | L1_LAK_164 / TimePressure; Any mood / Any patience | inactive scenario |
| Lakshmamma / LAK | `lak_165_s01.wav` | Swalpa quicker, please. The rest of my buying is waiting. | None | Missing | No | L1_LAK_165 / TimePressure; Any mood / Any patience | inactive scenario |
| Lakshmamma / LAK | `lak_166_s01.wav` | Let us not spend the whole morning on one purchase. | None | Missing | No | L1_LAK_166 / TimePressure; Any mood / Any patience | inactive scenario |
| Lakshmamma / LAK | `lak_167_s01.wav` | My time is getting short. What is your answer? | None | Missing | No | L1_LAK_167 / TimePressure; Any mood / Any patience | inactive scenario |
| Lakshmamma / LAK | `lak_198_s01.wav` | Good to speak with you, merchant, but I do need to finish my buying. | None | Missing | No | L1_LAK_198 / SocialGreeting; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_199_s01.wav` | A kind word is welcome. I have little time left for talk, though. | None | Missing | No | L1_LAK_199 / SocialGreeting; Any mood / Low patience | adapter required |
| Lakshmamma / LAK | `lak_200_s01.wav` | Sari, I wish you well. Shall we return to the purchase? | None | Missing | No | L1_LAK_200 / SocialGreeting; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_076_s01.wav` | Chinnamma Naik. I buy spices for trade here in Vijayanagara. | None | Missing | No | L1_CHI_076 / AskName; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_077_s01.wav` | My name is Chinnamma Naik. Now you know whom you are bargaining with. | None | Missing | No | L1_CHI_077 / AskName; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_078_s01.wav` | Chinnamma Naik, merchant. And what should I call you? | None | Missing | No | L1_CHI_078 / AskName; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_079_s01.wav` | You may call me Chinnamma Naik. I prefer names to shouted guesses across a stall. | None | Missing | No | L1_CHI_079 / AskName; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_080_s01.wav` | Vijayanagara is where I do my buying. I know these markets well. | None | Missing | No | L1_CHI_080 / AskOrigin; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_081_s01.wav` | I trade here in Vijayanagara. These stalls are familiar ground. | None | Missing | No | L1_CHI_081 / AskOrigin; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_082_s01.wav` | I am from this region, merchant. This is no unfamiliar market to me. | None | Missing | No | L1_CHI_082 / AskOrigin; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_083_s01.wav` | My business is here in Vijayanagara. I need no guide to its spice stalls. | None | Missing | No | L1_CHI_083 / AskOrigin; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_084_s01.wav` | Fine weather will not keep spice dry by itself. That is what a good store is for. | None | Missing | No | L1_CHI_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_085_s01.wav` | Whatever the sky decides, the goods still need looking after. | None | Missing | No | L1_CHI_085 / WeatherQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_086_s01.wav` | I prefer shade for bargaining. It leaves fewer excuses for a wandering mind. | None | Missing | No | L1_CHI_086 / WeatherQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_087_s01.wav` | Rain or sun, merchants find something to blame for a price! | None | Missing | No | L1_CHI_087 / WeatherQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_088_s01.wav` | Krishnadevaraya is our king. For court affairs, ask nearer the palace; I know the spice trade. | None | Missing | No | L1_CHI_088 / HistoryKing; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_089_s01.wav` | Our king is Krishnadevaraya. I can tell you more about a measure of spice than a royal audience. | None | Missing | No | L1_CHI_089 / HistoryKing; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_090_s01.wav` | Krishnadevaraya rules here. My business keeps me among the traders, not the courtiers. | None | Missing | No | L1_CHI_090 / HistoryKing; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_091_s01.wav` | King Krishnadevaraya. My buying is rather less grand than his court. | None | Missing | No | L1_CHI_091 / HistoryKing; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_092_s01.wav` | That is Virupaksha's temple. Its bazaar brings worshippers and traders into the same streets. | None | Missing | No | L1_CHI_092 / HistoryTemple; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_093_s01.wav` | You mean Virupaksha's temple. There is plenty of buying and selling before it. | None | Missing | No | L1_CHI_093 / HistoryTemple; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_094_s01.wav` | Look for the bazaar before Virupaksha's temple. A trader soon learns that part of the city. | None | Missing | No | L1_CHI_094 / HistoryTemple; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_095_s01.wav` | Virupaksha's temple is part of this city's life. So is the daily business in its bazaar. | None | Missing | No | L1_CHI_095 / HistoryTemple; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_096_s01.wav` | Vijayanagara has room for many trades. Mine is spice; that is the part I know best. | None | Missing | No | L1_CHI_096 / HistoryQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_097_s01.wav` | The city has its palaces. I spend more time among its goods and merchants. | None | Missing | No | L1_CHI_097 / HistoryQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_098_s01.wav` | Listen in a bazaar and you will hear a hundred accounts of the same city. Some may even agree. | None | Missing | No | L1_CHI_098 / HistoryQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_099_s01.wav` | A market tells you much about a city: who comes, what sells, and who argues over it. | None | Missing | No | L1_CHI_099 / HistoryQuestion; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_100_s01.wav` | Good to speak with you, merchant. A bargain need not begin with a scowl. | None | Missing | No | L1_CHI_100 / SocialGreeting; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_101_s01.wav` | Courtesy and a clear price. A promising start. | None | Missing | No | L1_CHI_101 / SocialGreeting; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_102_s01.wav` | I enjoy good company. I enjoy a good argument too, provided it goes somewhere. | None | Missing | No | L1_CHI_102 / SocialGreeting; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_103_s01.wav` | You are welcome to speak freely. I can bargain and listen at the same time. | None | Missing | No | L1_CHI_103 / SocialGreeting; Any mood / Any patience | adapter required |
| Chinnamma Naik / CHI | `chi_125_s01.wav` | I need an answer soon, merchant. There is other buying to do. | None | Missing | No | L1_CHI_125 / TimePressure; Any mood / Any patience | inactive scenario |
| Chinnamma Naik / CHI | `chi_126_s01.wav` | My time is running short. Where do you stand? | None | Missing | No | L1_CHI_126 / TimePressure; Any mood / Any patience | inactive scenario |
| Chinnamma Naik / CHI | `chi_127_s01.wav` | Let us bring this to a decision; I have other business waiting. | None | Missing | No | L1_CHI_127 / TimePressure; Any mood / Any patience | inactive scenario |
| Chinnamma Naik / CHI | `chi_162_s01.wav` | We can talk another time. I need to finish this buying now. | None | Missing | No | L1_CHI_162 / SocialGreeting; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_163_s01.wav` | Courtesy is welcome, but my time is nearly spent. Back to the purchase, please. | None | Missing | No | L1_CHI_163 / SocialGreeting; Any mood / Low patience | adapter required |
| Chinnamma Naik / CHI | `chi_164_s01.wav` | I have little time for talk now, merchant. Where do we stand? | None | Missing | No | L1_CHI_164 / SocialGreeting; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_074_s01.wav` | Saraswati Chetti. I keep a shop here and buy what my customers ask for. | None | Missing | No | L1_SAR_074 / AskName; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_075_s01.wav` | My name is Saraswati Chetti. I prefer to know the person behind a price. | None | Missing | No | L1_SAR_075 / AskName; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_076_s01.wav` | Saraswati Chetti, merchant. You may remember the name when we strike a bargain. | None | Missing | No | L1_SAR_076 / AskName; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_077_s01.wav` | I am Saraswati Chetti. Now, tell me how you judge this lot. | None | Missing | No | L1_SAR_077 / AskName; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_078_s01.wav` | My shop is here in Vijayanagara. I know its buyers better than its distant roads. | None | Missing | No | L1_SAR_078 / AskOrigin; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_079_s01.wav` | I do business in Vijayanagara's markets; that is the part of my story I can tell you. | None | Missing | No | L1_SAR_079 / AskOrigin; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_080_s01.wav` | Vijayanagara is where I keep my shop, merchant. | None | Missing | No | L1_SAR_080 / AskOrigin; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_081_s01.wav` | I sell here in Vijayanagara. My customers teach me plenty about this market. | None | Missing | No | L1_SAR_081 / AskOrigin; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_082_s01.wav` | Dry storage matters more to my spice than a bright sky. | None | Missing | No | L1_SAR_082 / WeatherQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_083_s01.wav` | Rain changes a market day quickly. The measure and price still need attention. | None | Missing | No | L1_SAR_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_084_s01.wav` | I watch the weather, but I watch the condition of my stock more closely. | None | Missing | No | L1_SAR_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_085_s01.wav` | Krishnadevaraya is the king. I know my customers better than court affairs. | None | Missing | No | L1_SAR_085 / HistoryKing; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_086_s01.wav` | Our ruler is Krishnadevaraya. What reaches my shop is news from the bazaar. | None | Missing | No | L1_SAR_086 / HistoryKing; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_087_s01.wav` | Krishnadevaraya rules here. My daily calculations are rather smaller than his. | None | Missing | No | L1_SAR_087 / HistoryKing; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_088_s01.wav` | Virupaksha's temple stands by a busy bazaar. Follow the traders and you will find your way. | None | Missing | No | L1_SAR_088 / HistoryTemple; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_089_s01.wav` | You mean Virupaksha's temple. Its market brings many buyers past these stalls. | None | Missing | No | L1_SAR_089 / HistoryTemple; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_090_s01.wav` | The bazaar before Virupaksha's temple is hard to miss when business is lively. | None | Missing | No | L1_SAR_090 / HistoryTemple; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_091_s01.wav` | A city is easier to understand through what its people buy and sell. | None | Missing | No | L1_SAR_091 / HistoryQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_092_s01.wav` | Vijayanagara draws many kinds of trade. My own knowledge begins at the shop counter. | None | Missing | No | L1_SAR_092 / HistoryQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_093_s01.wav` | I can tell you what sells in this bazaar. For older histories, ask someone who keeps them. | None | Missing | No | L1_SAR_093 / HistoryQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_094_s01.wav` | Look at the market and you will see how far goods travel to reach this city. | None | Missing | No | L1_SAR_094 / HistoryQuestion; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_095_s01.wav` | Namaskaram. A clear answer is as welcome as a courteous one. | None | Missing | No | L1_SAR_095 / SocialGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_096_s01.wav` | Good to speak with you. Tell me what you think of this lot. | None | Missing | No | L1_SAR_096 / SocialGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_097_s01.wav` | Courtesy makes bargaining easier to hear, even when the prices differ. | None | Missing | No | L1_SAR_097 / SocialGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_098_s01.wav` | You have my attention, merchant. I am listening. | None | Missing | No | L1_SAR_098 / SocialGreeting; Any mood / Any patience | adapter required |
| Saraswati Chetti / SAR | `sar_119_s01.wav` | I must return to my shop soon. Give me a clear answer. | None | Missing | No | L1_SAR_119 / TimePressure; Any mood / Any patience | inactive scenario |
| Saraswati Chetti / SAR | `sar_120_s01.wav` | We have discussed the measure. Can we decide on the price? | None | Missing | No | L1_SAR_120 / TimePressure; Any mood / Any patience | inactive scenario |
| Saraswati Chetti / SAR | `sar_121_s01.wav` | My customers will not wait all day; let us settle the question. | None | Missing | No | L1_SAR_121 / TimePressure; Any mood / Any patience | inactive scenario |
| Saraswati Chetti / SAR | `sar_159_s01.wav` | A courteous word is welcome. I do need to return to my shop soon. | None | Missing | No | L1_SAR_159 / SocialGreeting; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_160_s01.wav` | Thank you. Now please give me your answer on the trade. | None | Missing | No | L1_SAR_160 / SocialGreeting; Any mood / Low patience | adapter required |
| Saraswati Chetti / SAR | `sar_161_s01.wav` | We can speak more another day. I need a decision now. | None | Missing | No | L1_SAR_161 / SocialGreeting; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_074_s01.wav` | Abdul Rahman. I trade with a caravan and prefer to learn the name of a good seller. | None | Missing | No | L1_ABD_074 / AskName; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_075_s01.wav` | My name is Abdul Rahman, merchant. What may I call you? | None | Missing | No | L1_ABD_075 / AskName; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_076_s01.wav` | Abdul Rahman. Names are worth remembering when bargains are fair. | None | Missing | No | L1_ABD_076 / AskName; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_077_s01.wav` | You may call me Abdul Rahman. I hope we can speak plainly to one another. | None | Missing | No | L1_ABD_077 / AskName; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_078_s01.wav` | I trade with an Arab caravan; our travels have brought me to Vijayanagara. | None | Missing | No | L1_ABD_078 / AskOrigin; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_079_s01.wav` | My work is caravan trade. Vijayanagara is where I am buying today. | None | Missing | No | L1_ABD_079 / AskOrigin; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_080_s01.wav` | I came with a caravan of Arab traders, merchant. The road has brought us here. | None | Missing | No | L1_ABD_080 / AskOrigin; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_081_s01.wav` | I travel with a caravan of traders. For now, our business has brought us to Vijayanagara. | None | Missing | No | L1_ABD_081 / AskOrigin; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_082_s01.wav` | A clear sky is welcome on a trading day. It does not settle a price for us. | None | Missing | No | L1_ABD_082 / WeatherQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_083_s01.wav` | Rain teaches a trader to protect the goods before praising the bargain. | None | Missing | No | L1_ABD_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_084_s01.wav` | The weather may change before evening. Let us judge the | None | Missing | No | L1_ABD_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_084_s02.wav` | while it is here. | None | Missing | No | L1_ABD_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_085_s01.wav` | Krishnadevaraya rules here. I know the market better than I know his court. | None | Missing | No | L1_ABD_085 / HistoryKing; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_086_s01.wav` | The king is Krishnadevaraya. Ask the courtiers about policy; I can speak about trade. | None | Missing | No | L1_ABD_086 / HistoryKing; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_087_s01.wav` | Krishnadevaraya is the ruler, merchant. The bazaar has its own lively conversations. | None | Missing | No | L1_ABD_087 / HistoryKing; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_088_s01.wav` | That is Virupaksha's temple. The bazaar nearby is familiar to anyone who trades here. | None | Missing | No | L1_ABD_088 / HistoryTemple; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_089_s01.wav` | You mean Virupaksha's temple. Follow the market toward it and you will see it. | None | Missing | No | L1_ABD_089 / HistoryTemple; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_090_s01.wav` | Virupaksha's temple stands by a busy bazaar. Traders know the way well. | None | Missing | No | L1_ABD_090 / HistoryTemple; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_091_s01.wav` | Vijayanagara gathers traders with many stories. I can tell you what I have seen in its market. | None | Missing | No | L1_ABD_091 / HistoryQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_092_s01.wav` | A city reveals itself in its markets, though no single trader knows every street. | None | Missing | No | L1_ABD_092 / HistoryQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_093_s01.wav` | I have learned something of this city by listening to the people who sell here. | None | Missing | No | L1_ABD_093 / HistoryQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_094_s01.wav` | Ask me about trading in Vijayanagara; I would rather not pretend to know its whole history. | None | Missing | No | L1_ABD_094 / HistoryQuestion; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_095_s01.wav` | Marhaban. It is good to exchange a word before exchanging prices. | None | Missing | No | L1_ABD_095 / SocialGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_096_s01.wav` | You are courteous, merchant. That makes even a difficult bargain easier to discuss. | None | Missing | No | L1_ABD_096 / SocialGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_097_s01.wav` | A pleasant conversation is welcome. We can return to the lot when you are ready. | None | Missing | No | L1_ABD_097 / SocialGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_098_s01.wav` | I am glad to talk. What is on your mind? | None | Missing | No | L1_ABD_098 / SocialGreeting; Any mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_119_s01.wav` | I have other buying to do. Can you give me an answer soon? | None | Missing | No | L1_ABD_119 / TimePressure; Any mood / Any patience | inactive scenario |
| Abdul Rahman / ABD | `abd_120_s01.wav` | We have spoken enough to decide on a figure, merchant. | None | Missing | No | L1_ABD_120 / TimePressure; Any mood / Any patience | inactive scenario |
| Abdul Rahman / ABD | `abd_121_s01.wav` | My time in this market is limited. Where do you stand? | None | Missing | No | L1_ABD_121 / TimePressure; Any mood / Any patience | inactive scenario |
| Abdul Rahman / ABD | `abd_159_s01.wav` | I appreciate the courtesy. I do need to finish this purchase soon. | None | Missing | No | L1_ABD_159 / SocialGreeting; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_160_s01.wav` | Thank you for the kind word. What is your answer on the trade? | None | Missing | No | L1_ABD_160 / SocialGreeting; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_161_s01.wav` | I would talk longer another day. Today my time is short. | None | Missing | No | L1_ABD_161 / SocialGreeting; Any mood / Low patience | adapter required |
| Abdul Rahman / ABD | `abd_165_s01.wav` | Your courtesy is heard. Let us keep our terms equally clear. | None | Missing | No | L1_ABD_165 / SocialGreeting; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_166_s01.wav` | I appreciate the greeting. Now tell me what you mean to offer. | None | Missing | No | L1_ABD_166 / SocialGreeting; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_167_s01.wav` | I will speak plainly, merchant. Please do the same. | None | Missing | No | L1_ABD_167 / SocialGreeting; Strict mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_168_s01.wav` | Thank you. I need to settle this purchase soon. | None | Missing | No | L1_ABD_168 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_169_s01.wav` | I hear you. Now, where do we stand on the trade? | None | Missing | No | L1_ABD_169 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Abdul Rahman / ABD | `abd_170_s01.wav` | We can speak at length later. I need your answer now. | None | Missing | No | L1_ABD_170 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_074_s01.wav` | Francisco de Almeida. And what name shall I remember for this stall? | None | Missing | No | L1_FRA_074 / AskName; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_075_s01.wav` | I am Francisco. It is good to know whom I bargain with. | None | Missing | No | L1_FRA_075 / AskName; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_076_s01.wav` | Francisco de Almeida, merchant. Now you know the man behind the offer. | None | Missing | No | L1_FRA_076 / AskName; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_077_s01.wav` | You may call me Francisco. I hope to remember your name as well. | None | Missing | No | L1_FRA_077 / AskName; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_078_s01.wav` | I am Portuguese and work in trade. This market has much to teach a visitor. | None | Missing | No | L1_FRA_078 / AskOrigin; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_079_s01.wav` | Portugal is the country I call mine; today my business is here. | None | Missing | No | L1_FRA_079 / AskOrigin; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_080_s01.wav` | I am a Portuguese trader. I would rather let the goods speak for themselves. | None | Missing | No | L1_FRA_080 / AskOrigin; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_081_s01.wav` | From Portugal, merchant. The route here is a longer tale than this bargain. | None | Missing | No | L1_FRA_081 / AskOrigin; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_082_s01.wav` | A fair sky makes a market livelier. What do you think of this lot? | None | Missing | No | L1_FRA_082 / WeatherQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_083_s01.wav` | Rain would send us both looking for cover. Let us keep the | None | Missing | No | L1_FRA_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_083_s02.wav` | dry. | None | Missing | No | L1_FRA_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_084_s01.wav` | The day has its moods. I hope our bargaining has the better one. | None | Missing | No | L1_FRA_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_085_s01.wav` | Krishnadevaraya is the ruler here. I know his markets better than his court. | None | Missing | No | L1_FRA_085 / HistoryKing; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_086_s01.wav` | The king is Krishnadevaraya. Ask a courtier for politics; I can speak of trade. | None | Missing | No | L1_FRA_086 / HistoryKing; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_087_s01.wav` | Krishnadevaraya rules Vijayanagara, merchant. Its busy bazaar speaks for itself. | None | Missing | No | L1_FRA_087 / HistoryKing; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_088_s01.wav` | Virupaksha's temple is a landmark of this city. A local can guide you to it better than I. | None | Missing | No | L1_FRA_088 / HistoryTemple; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_089_s01.wav` | If you mean Virupaksha's temple, ask along the bazaar for the way. | None | Missing | No | L1_FRA_089 / HistoryTemple; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_090_s01.wav` | The Virupaksha temple draws visitors. I know its name; a local knows every turning. | None | Missing | No | L1_FRA_090 / HistoryTemple; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_091_s01.wav` | Vijayanagara's market can occupy a curious trader all day. Its whole history is another matter. | None | Missing | No | L1_FRA_091 / HistoryQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_092_s01.wav` | I am still learning this city. Its sellers have been excellent teachers. | None | Missing | No | L1_FRA_092 / HistoryQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_093_s01.wav` | Ask me what I have seen in the bazaar; I will not pretend to know every old tale. | None | Missing | No | L1_FRA_093 / HistoryQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_094_s01.wav` | There is much to admire here, but I would rather speak from what I know. | None | Missing | No | L1_FRA_094 / HistoryQuestion; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_095_s01.wav` | Olá! A friendly word makes the bargaining livelier. | None | Missing | No | L1_FRA_095 / SocialGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_096_s01.wav` | You are good company, merchant. What is on your mind? | None | Missing | No | L1_FRA_096 / SocialGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_097_s01.wav` | I enjoy a little talk before we settle the figures. | None | Missing | No | L1_FRA_097 / SocialGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_098_s01.wav` | Well met. The | None | Missing | No | L1_FRA_098 / SocialGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_098_s02.wav` | can wait while we exchange a word. | None | Missing | No | L1_FRA_098 / SocialGreeting; Any mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_119_s01.wav` | I have more buying to do. Can we reach a decision soon? | None | Missing | No | L1_FRA_119 / TimePressure; Any mood / Any patience | inactive scenario |
| Francisco de Almeida / FRA | `fra_120_s01.wav` | Come, merchant. Where do you stand on the price? | None | Missing | No | L1_FRA_120 / TimePressure; Any mood / Any patience | inactive scenario |
| Francisco de Almeida / FRA | `fra_121_s01.wav` | We have given this lot our attention. I need your answer. | None | Missing | No | L1_FRA_121 / TimePressure; Any mood / Any patience | inactive scenario |
| Francisco de Almeida / FRA | `fra_159_s01.wav` | Good to speak with you. I need to finish this purchase soon. | None | Missing | No | L1_FRA_159 / SocialGreeting; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_160_s01.wav` | I hear you, merchant. What is your answer on the lot? | None | Missing | No | L1_FRA_160 / SocialGreeting; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_161_s01.wav` | We can talk longer another day. For now, let us decide. | None | Missing | No | L1_FRA_161 / SocialGreeting; Any mood / Low patience | adapter required |
| Francisco de Almeida / FRA | `fra_165_s01.wav` | Your courtesy is welcome. Let us keep the terms clear. | None | Missing | No | L1_FRA_165 / SocialGreeting; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_166_s01.wav` | I appreciate the greeting. Now tell me your figure. | None | Missing | No | L1_FRA_166 / SocialGreeting; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_167_s01.wav` | I will hear you out, merchant. Please speak plainly. | None | Missing | No | L1_FRA_167 / SocialGreeting; Strict mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_168_s01.wav` | Thank you. I need your answer on the trade. | None | Missing | No | L1_FRA_168 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_169_s01.wav` | I hear you. Where do we stand on the price? | None | Missing | No | L1_FRA_169 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Francisco de Almeida / FRA | `fra_170_s01.wav` | We can exchange stories later. Please decide soon. | None | Missing | No | L1_FRA_170 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_074_s01.wav` | I am Father Penteado. And what shall I call you? | None | Missing | No | L1_FAT_074 / AskName; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_075_s01.wav` | Penteado is my name. I am glad to know the person across the stall. | None | Missing | No | L1_FAT_075 / AskName; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_076_s01.wav` | You may call me Father Penteado, merchant. | None | Missing | No | L1_FAT_076 / AskName; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_077_s01.wav` | Father Penteado. A name is easier to remember than a price, I find. | None | Missing | No | L1_FAT_077 / AskName; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_078_s01.wav` | I am Portuguese, and a traveller here. I would rather tell you what I have seen than claim to know it all. | None | Missing | No | L1_FAT_078 / AskOrigin; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_079_s01.wav` | I come from Portugal. I am still learning this market and its ways. | None | Missing | No | L1_FAT_079 / AskOrigin; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_080_s01.wav` | Portugal is my country; this city's customs are teaching me much. | None | Missing | No | L1_FAT_080 / AskOrigin; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_081_s01.wav` | I am a visitor from Portugal, merchant. The rest of my journey is a longer story. | None | Missing | No | L1_FAT_081 / AskOrigin; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_082_s01.wav` | A kinder sky makes it easier to conduct our trade in peace. | None | Missing | No | L1_FAT_082 / WeatherQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_083_s01.wav` | The weather may change; let us keep the | None | Missing | No | L1_FAT_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_083_s02.wav` | sheltered. | None | Missing | No | L1_FAT_083 / WeatherQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_084_s01.wav` | The air is busy enough without the weather joining our bargain. | None | Missing | No | L1_FAT_084 / WeatherQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_085_s01.wav` | Krishnadevaraya rules Vijayanagara. I know the market better than the court. | None | Missing | No | L1_FAT_085 / HistoryKing; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_086_s01.wav` | The ruler is Krishnadevaraya. I would not pretend to speak for him. | None | Missing | No | L1_FAT_086 / HistoryKing; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_087_s01.wav` | Krishnadevaraya is king here; his city gives a visitor much to observe. | None | Missing | No | L1_FAT_087 / HistoryKing; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_088_s01.wav` | If you mean the Virupaksha temple, ask someone local for the surest way there. | None | Missing | No | L1_FAT_088 / HistoryTemple; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_089_s01.wav` | The Virupaksha temple is known here. I can name it, but a local will guide you better. | None | Missing | No | L1_FAT_089 / HistoryTemple; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_090_s01.wav` | I have heard of Virupaksha's temple. I would rather not guess at the turns through the streets. | None | Missing | No | L1_FAT_090 / HistoryTemple; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_091_s01.wav` | This city has more stories than I can honestly tell. What part interests you? | None | Missing | No | L1_FAT_091 / HistoryQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_092_s01.wav` | I am learning Vijayanagara through its people. A local would know its older tales better. | None | Missing | No | L1_FAT_092 / HistoryQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_093_s01.wav` | The market has taught me much about this place; its whole history is beyond my knowledge. | None | Missing | No | L1_FAT_093 / HistoryQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_094_s01.wav` | I would gladly hear a resident speak of the city. I am still a visitor. | None | Missing | No | L1_FAT_094 / HistoryQuestion; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_095_s01.wav` | That is kind of you to say. How has your day been? | None | Missing | No | L1_FAT_095 / SocialGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_096_s01.wav` | You are courteous, merchant. I welcome a moment of conversation. | None | Missing | No | L1_FAT_096 / SocialGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_097_s01.wav` | A pleasant word can steady a busy day. What is on your mind? | None | Missing | No | L1_FAT_097 / SocialGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_098_s01.wav` | I am glad to speak with you. The figures can wait a moment. | None | Missing | No | L1_FAT_098 / SocialGreeting; Any mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_119_s01.wav` | I have little time left for this discussion. What is your answer? | None | Missing | No | L1_FAT_119 / TimePressure; Any mood / Any patience | inactive scenario |
| Father Penteado / FAT | `fat_120_s01.wav` | We have considered the price carefully. Can you decide soon? | None | Missing | No | L1_FAT_120 / TimePressure; Any mood / Any patience | inactive scenario |
| Father Penteado / FAT | `fat_121_s01.wav` | I need a clear answer before we spend more time on this lot. | None | Missing | No | L1_FAT_121 / TimePressure; Any mood / Any patience | inactive scenario |
| Father Penteado / FAT | `fat_159_s01.wav` | I hear you, merchant. I need to decide on this lot soon. | None | Missing | No | L1_FAT_159 / SocialGreeting; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_160_s01.wav` | Thank you for the word. Can we settle the matter before us? | None | Missing | No | L1_FAT_160 / SocialGreeting; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_161_s01.wav` | I appreciate the greeting, but time is short for me. | None | Missing | No | L1_FAT_161 / SocialGreeting; Any mood / Low patience | adapter required |
| Father Penteado / FAT | `fat_165_s01.wav` | Your greeting is welcome. Let us keep our terms clear. | None | Missing | No | L1_FAT_165 / SocialGreeting; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_166_s01.wav` | I hear your courtesy. What did you wish to ask? | None | Missing | No | L1_FAT_166 / SocialGreeting; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_167_s01.wav` | We may speak, merchant. I ask only that we be direct. | None | Missing | No | L1_FAT_167 / SocialGreeting; Strict mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_168_s01.wav` | Thank you. I need your answer on the purchase. | None | Missing | No | L1_FAT_168 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_169_s01.wav` | I hear you. Can we decide on the price? | None | Missing | No | L1_FAT_169 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Father Penteado / FAT | `fat_170_s01.wav` | Let us be brief, please. What is your answer? | None | Missing | No | L1_FAT_170 / SocialGreeting; Impatient mood / Any patience | adapter required |
| Lakshmamma / LAK | `lak_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Lakshmamma / LAK | `lak_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Lakshmamma / LAK | `lak_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Lakshmamma / LAK | `lak_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Chinnamma Naik / CHI | `chi_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Chinnamma Naik / CHI | `chi_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Chinnamma Naik / CHI | `chi_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Chinnamma Naik / CHI | `chi_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Saraswati Chetti / SAR | `sar_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Saraswati Chetti / SAR | `sar_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Saraswati Chetti / SAR | `sar_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Saraswati Chetti / SAR | `sar_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Abdul Rahman / ABD | `abd_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Abdul Rahman / ABD | `abd_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Abdul Rahman / ABD | `abd_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Abdul Rahman / ABD | `abd_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Francisco de Almeida / FRA | `fra_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Francisco de Almeida / FRA | `fra_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Francisco de Almeida / FRA | `fra_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Francisco de Almeida / FRA | `fra_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Father Penteado / FAT | `fat_spice_clove.wav` | clove | None | Missing | No | Dynamic Vocabulary; neutral reusable join | catalog-only; excluded from Level 1 generation |
| Father Penteado / FAT | `fat_spice_generic.wav` | spice | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| Father Penteado / FAT | `fat_currency_varaha.wav` | varaha | None | Missing | No | Dynamic Vocabulary; neutral reusable join | reserved singular; provider currently says varahas |
| Father Penteado / FAT | `fat_quantity_this_lot.wav` | this lot | None | Missing | No | Dynamic Vocabulary; neutral reusable join | fallback only |
| You / player / PLAYER | `PLAYER_EPILOGUE_01.wav` | I'd like to buy that book. | None | Missing | No | EpilogueStoryManager.MeeraPurchaseSequence; natural, brief; establish player voice only if desired | intentionally permitted subtitle-only; player voice not established |

## P0 copy-ready generation script

One row = one output file. Fields are `speaker | voice reference | filename | EXACT SPOKEN WORDS`. Speak only the last field. Mood/context and assembly holds are in the checklist above. These are deliberately fragments where the existing recording plan splits at a dynamic placeholder.

### Lakshmamma

```text
Lakshmamma | LAK | lak_001_s01.wav | Good day, merchant. I need
Lakshmamma | LAK | lak_001_s02.wav | of
Lakshmamma | LAK | lak_001_s03.wav | . What are you asking?
Lakshmamma | LAK | lak_002_s01.wav | Namaskara. Let me buy
Lakshmamma | LAK | lak_002_s02.wav | of
Lakshmamma | LAK | lak_002_s03.wav | . Tell me your price.
Lakshmamma | LAK | lak_003_s01.wav | Your spices caught my eye. How much for
Lakshmamma | LAK | lak_003_s02.wav | of
Lakshmamma | LAK | lak_004_s01.wav | Good day to you.
Lakshmamma | LAK | lak_004_s02.wav | of
Lakshmamma | LAK | lak_004_s03.wav | , please. What will that cost?
Lakshmamma | LAK | lak_005_s01.wav | Come, let us start with a fair price. What do you ask for
Lakshmamma | LAK | lak_005_s02.wav | of
Lakshmamma | LAK | lak_006_s01.wav | Let us see whether we can agree. I need
Lakshmamma | LAK | lak_006_s02.wav | of
Lakshmamma | LAK | lak_006_s03.wav | ; name your price.
Lakshmamma | LAK | lak_007_s01.wav | Ah, we meet again! I need
Lakshmamma | LAK | lak_007_s02.wav | of
Lakshmamma | LAK | lak_007_s03.wav | today. What is your price?
Lakshmamma | LAK | lak_008_s01.wav | Good to see you again, merchant. How much for
Lakshmamma | LAK | lak_008_s02.wav | of
Lakshmamma | LAK | lak_009_s01.wav | Back at your stall again.
Lakshmamma | LAK | lak_009_s02.wav | of
Lakshmamma | LAK | lak_009_s03.wav | , please. What are you asking?
Lakshmamma | LAK | lak_010_s01.wav | You remember me, I hope. I am buying
Lakshmamma | LAK | lak_010_s02.wav | of
Lakshmamma | LAK | lak_010_s03.wav | . Tell me the price.
Lakshmamma | LAK | lak_011_s01.wav | Another visit, another bargain. What will
Lakshmamma | LAK | lak_011_s02.wav | of
Lakshmamma | LAK | lak_011_s03.wav | cost me?
Lakshmamma | LAK | lak_012_s01.wav | Good day again. Let us start with your price for
Lakshmamma | LAK | lak_012_s02.wav | of
Lakshmamma | LAK | lak_013_s01.wav | of
Lakshmamma | LAK | lak_013_s02.wav | , please. That is what I need.
Lakshmamma | LAK | lak_014_s01.wav | I am buying
Lakshmamma | LAK | lak_014_s02.wav | of
Lakshmamma | LAK | lak_014_s03.wav | for the house.
Lakshmamma | LAK | lak_015_s01.wav | The
Lakshmamma | LAK | lak_015_s02.wav | , merchant. I need
Lakshmamma | LAK | lak_016_s01.wav | Just
Lakshmamma | LAK | lak_016_s02.wav | of
Lakshmamma | LAK | lak_016_s03.wav | for this purchase.
Lakshmamma | LAK | lak_017_s01.wav | It is
Lakshmamma | LAK | lak_017_s02.wav | of
Lakshmamma | LAK | lak_017_s03.wav | I am after.
Lakshmamma | LAK | lak_018_s01.wav | Let us keep to
Lakshmamma | LAK | lak_018_s02.wav | of
Lakshmamma | LAK | lak_019_s01.wav | I need
Lakshmamma | LAK | lak_019_s02.wav | of
Lakshmamma | LAK | lak_019_s03.wav | . What is your price?
Lakshmamma | LAK | lak_020_s01.wav | Tell me what you want for
Lakshmamma | LAK | lak_020_s02.wav | of
Lakshmamma | LAK | lak_021_s01.wav | of
Lakshmamma | LAK | lak_021_s02.wav | , please. How much are you asking?
Lakshmamma | LAK | lak_022_s01.wav | Let us begin with your price for
Lakshmamma | LAK | lak_022_s02.wav | of
Lakshmamma | LAK | lak_023_s01.wav | What would you charge me for
Lakshmamma | LAK | lak_023_s02.wav | of
Lakshmamma | LAK | lak_024_s01.wav | Name your price for
Lakshmamma | LAK | lak_024_s02.wav | of
Lakshmamma | LAK | lak_024_s03.wav | , merchant.
Lakshmamma | LAK | lak_025_s01.wav | I am offering
Lakshmamma | LAK | lak_025_s02.wav | for
Lakshmamma | LAK | lak_025_s03.wav | of
Lakshmamma | LAK | lak_026_s01.wav | For
Lakshmamma | LAK | lak_026_s02.wav | of
Lakshmamma | LAK | lak_026_s03.wav | , my offer is
Lakshmamma | LAK | lak_027_s01.wav | for the
Lakshmamma | LAK | lak_027_s02.wav | of
Lakshmamma | LAK | lak_027_s03.wav | . Does that suit you?
Lakshmamma | LAK | lak_028_s01.wav | Here is my offer:
Lakshmamma | LAK | lak_028_s02.wav | for
Lakshmamma | LAK | lak_028_s03.wav | of
Lakshmamma | LAK | lak_029_s01.wav | I can offer
Lakshmamma | LAK | lak_029_s02.wav | for
Lakshmamma | LAK | lak_029_s03.wav | of
Lakshmamma | LAK | lak_029_s04.wav | . What do you say?
Lakshmamma | LAK | lak_030_s01.wav | The offer stands at
Lakshmamma | LAK | lak_030_s02.wav | for
Lakshmamma | LAK | lak_030_s03.wav | of
Lakshmamma | LAK | lak_031_s01.wav | of
Lakshmamma | LAK | lak_031_s02.wav | . That is the amount I need.
Lakshmamma | LAK | lak_032_s01.wav | I need
Lakshmamma | LAK | lak_032_s02.wav | of
Lakshmamma | LAK | lak_032_s03.wav | , merchant.
Lakshmamma | LAK | lak_033_s01.wav | For this purchase,
Lakshmamma | LAK | lak_033_s02.wav | of
Lakshmamma | LAK | lak_034_s01.wav | Let us keep the measure clear:
Lakshmamma | LAK | lak_034_s02.wav | of
Lakshmamma | LAK | lak_035_s01.wav | The quantity is
Lakshmamma | LAK | lak_035_s02.wav | of
Lakshmamma | LAK | lak_036_s01.wav | , please, of the
Lakshmamma | LAK | lak_037_s01.wav | Sari,
Lakshmamma | LAK | lak_037_s02.wav | of
Lakshmamma | LAK | lak_037_s03.wav | , then. My offer is
Lakshmamma | LAK | lak_038_s01.wav | We are discussing
Lakshmamma | LAK | lak_038_s02.wav | of
Lakshmamma | LAK | lak_038_s03.wav | now, at my offer of
Lakshmamma | LAK | lak_039_s01.wav | of
Lakshmamma | LAK | lak_039_s02.wav | , understood. I am offering
Lakshmamma | LAK | lak_040_s01.wav | Let us be clear:
Lakshmamma | LAK | lak_040_s02.wav | of
Lakshmamma | LAK | lak_040_s03.wav | . My offer stands at
Lakshmamma | LAK | lak_041_s01.wav | Ayyo,
Lakshmamma | LAK | lak_041_s02.wav | ? That is too much. My offer remains
Lakshmamma | LAK | lak_042_s01.wav | At
Lakshmamma | LAK | lak_042_s02.wav | , I must say no. I am still offering
Lakshmamma | LAK | lak_043_s01.wav | Good spice, yes; but at
Lakshmamma | LAK | lak_043_s02.wav | , you will empty my purse. I am offering
Lakshmamma | LAK | lak_044_s01.wav | You ask
Lakshmamma | LAK | lak_044_s02.wav | ; I am holding at
Lakshmamma | LAK | lak_044_s03.wav | . We are too far apart.
Lakshmamma | LAK | lak_045_s01.wav | That price would leave little for my other buying. My offer stays at
Lakshmamma | LAK | lak_046_s01.wav | I cannot agree to
Lakshmamma | LAK | lak_046_s02.wav | is my offer.
Lakshmamma | LAK | lak_047_s01.wav | We are close, but I am still at
Lakshmamma | LAK | lak_048_s01.wav | Swalpa less, merchant. My offer remains
Lakshmamma | LAK | lak_049_s01.wav | Only a little separates us. Would you take my
Lakshmamma | LAK | lak_050_s01.wav | I hear your price. I am holding at
Lakshmamma | LAK | lak_051_s01.wav | You have nearly persuaded me, but my offer is still
Lakshmamma | LAK | lak_052_s01.wav | A small difference is still a difference. I am offering
Lakshmamma | LAK | lak_053_s01.wav | That is unexpectedly low. Is that for the whole purchase? My offer remains
Lakshmamma | LAK | lak_054_s01.wav | Ayyo, that little? Let us make sure we mean the same purchase. My offer is still
Lakshmamma | LAK | lak_055_s01.wav | I like a bargain, but that price makes me pause. I am holding at
Lakshmamma | LAK | lak_056_s01.wav | You ask so little that I wonder whether we have misunderstood each other. My offer remains
Lakshmamma | LAK | lak_057_s01.wav | That price does not sound right for this purchase. I am still offering
Lakshmamma | LAK | lak_058_s01.wav | Are you sure of that figure, merchant? Such a low price makes me wonder. My offer stands at
Lakshmamma | LAK | lak_059_s01.wav | I can offer
Lakshmamma | LAK | lak_059_s02.wav | for
Lakshmamma | LAK | lak_059_s03.wav | of
Lakshmamma | LAK | lak_059_s04.wav | . What do you say?
Lakshmamma | LAK | lak_060_s01.wav | Let me offer
Lakshmamma | LAK | lak_060_s02.wav | . Will that do for
Lakshmamma | LAK | lak_060_s03.wav | of
Lakshmamma | LAK | lak_061_s01.wav | Your price is more than I had in mind. I can offer
Lakshmamma | LAK | lak_062_s01.wav | Shall we try
Lakshmamma | LAK | lak_062_s02.wav | for this purchase?
Lakshmamma | LAK | lak_063_s01.wav | I will come up to
Lakshmamma | LAK | lak_063_s02.wav | . Can we agree there?
Lakshmamma | LAK | lak_064_s01.wav | Let us meet a little closer. I can offer
Lakshmamma | LAK | lak_065_s01.wav | Sari, I can come up to
Lakshmamma | LAK | lak_065_s02.wav | . What do you say?
Lakshmamma | LAK | lak_066_s01.wav | You have moved me a little. I can offer
Lakshmamma | LAK | lak_066_s02.wav | now.
Lakshmamma | LAK | lak_067_s01.wav | Let us try
Lakshmamma | LAK | lak_067_s02.wav | . Does that bring us together?
Lakshmamma | LAK | lak_068_s01.wav | I can raise my offer to
Lakshmamma | LAK | lak_068_s02.wav | . Can you meet me there?
Lakshmamma | LAK | lak_069_s01.wav | Swalpa more from me:
Lakshmamma | LAK | lak_070_s01.wav | I have listened to you. My offer is now
Lakshmamma | LAK | lak_071_s01.wav | We have talked this over. I can offer
Lakshmamma | LAK | lak_071_s02.wav | now.
Lakshmamma | LAK | lak_072_s01.wav | Let us see where we stand: my offer is now
Lakshmamma | LAK | lak_073_s01.wav | I can come up to
Lakshmamma | LAK | lak_073_s02.wav | . Will that settle the price?
Lakshmamma | LAK | lak_074_s01.wav | After all that bargaining, I am offering
Lakshmamma | LAK | lak_075_s01.wav | Here is where I stand now:
Lakshmamma | LAK | lak_076_s01.wav | I have raised my offer to
Lakshmamma | LAK | lak_076_s02.wav | . What is your answer?
Lakshmamma | LAK | lak_077_s01.wav | My offer remains
Lakshmamma | LAK | lak_077_s02.wav | , merchant.
Lakshmamma | LAK | lak_078_s01.wav | I have heard you, but I am holding at
Lakshmamma | LAK | lak_079_s01.wav | A friendly smile will not change my arithmetic.
Lakshmamma | LAK | lak_079_s02.wav | is still my offer.
Lakshmamma | LAK | lak_080_s01.wav | I am not raising the offer this time. It stands at
Lakshmamma | LAK | lak_081_s01.wav | Let us keep our feet on the ground. I am offering
Lakshmamma | LAK | lak_082_s01.wav | You bargain well, but I am still at
Lakshmamma | LAK | lak_083_s01.wav | Sari, agreed at
Lakshmamma | LAK | lak_083_s02.wav | . Please bring the
Lakshmamma | LAK | lak_083_s03.wav | of
Lakshmamma | LAK | lak_084_s01.wav | That price suits me:
Lakshmamma | LAK | lak_084_s02.wav | . Now bring the
Lakshmamma | LAK | lak_084_s03.wav | of
Lakshmamma | LAK | lak_084_s04.wav | , please.
Lakshmamma | LAK | lak_085_s01.wav | Yes,
Lakshmamma | LAK | lak_085_s02.wav | for
Lakshmamma | LAK | lak_085_s03.wav | of
Lakshmamma | LAK | lak_085_s04.wav | . Please bring it over.
Lakshmamma | LAK | lak_086_s01.wav | We agree on
Lakshmamma | LAK | lak_086_s02.wav | . I am ready for the
Lakshmamma | LAK | lak_086_s03.wav | of
Lakshmamma | LAK | lak_087_s01.wav | Good, the price is agreed at
Lakshmamma | LAK | lak_087_s02.wav | . Bring the
Lakshmamma | LAK | lak_087_s03.wav | of
Lakshmamma | LAK | lak_087_s04.wav | , please.
Lakshmamma | LAK | lak_088_s01.wav | , agreed. Let me have the
Lakshmamma | LAK | lak_088_s02.wav | of
Lakshmamma | LAK | lak_088_s03.wav | , please.
Lakshmamma | LAK | lak_089_s01.wav | Sari,
Lakshmamma | LAK | lak_089_s02.wav | it is. Please bring the
Lakshmamma | LAK | lak_089_s03.wav | of
Lakshmamma | LAK | lak_090_s01.wav | We have our price:
Lakshmamma | LAK | lak_090_s02.wav | . Now for the
Lakshmamma | LAK | lak_090_s03.wav | of
Lakshmamma | LAK | lak_091_s01.wav | Agreed at
Lakshmamma | LAK | lak_091_s02.wav | . Bring the
Lakshmamma | LAK | lak_091_s03.wav | of
Lakshmamma | LAK | lak_091_s04.wav | , merchant.
Lakshmamma | LAK | lak_092_s01.wav | Good, we understand each other.
Lakshmamma | LAK | lak_092_s02.wav | ; please bring the
Lakshmamma | LAK | lak_092_s03.wav | of
Lakshmamma | LAK | lak_093_s01.wav | Then we agree:
Lakshmamma | LAK | lak_093_s02.wav | of
Lakshmamma | LAK | lak_093_s03.wav | for
Lakshmamma | LAK | lak_093_s04.wav | . Please bring it over.
Lakshmamma | LAK | lak_094_s01.wav | A fair bargain at
Lakshmamma | LAK | lak_094_s02.wav | . I am waiting for the
Lakshmamma | LAK | lak_094_s03.wav | of
Lakshmamma | LAK | lak_095_s01.wav | My offer does not suit you, then. What price do you want?
Lakshmamma | LAK | lak_096_s01.wav | Sari, tell me what you are asking instead.
Lakshmamma | LAK | lak_097_s01.wav | You want more than my offer. Name your price, merchant.
Lakshmamma | LAK | lak_098_s01.wav | We have not agreed, then. What would you ask?
Lakshmamma | LAK | lak_099_s01.wav | Tell me your price so we have something to discuss.
Lakshmamma | LAK | lak_100_s01.wav | What price would you put to me instead?
Lakshmamma | LAK | lak_101_s01.wav | I have other provisions to buy as well. My offer is
Lakshmamma | LAK | lak_101_s02.wav | for
Lakshmamma | LAK | lak_101_s03.wav | of
Lakshmamma | LAK | lak_102_s01.wav | Good spice matters, but so does what is left in my purse. I am offering
Lakshmamma | LAK | lak_103_s01.wav | I am buying for a household, merchant.
Lakshmamma | LAK | lak_103_s02.wav | is the offer I am making.
Lakshmamma | LAK | lak_104_s01.wav | I must make my money serve the whole house. My offer stands at
Lakshmamma | LAK | lak_135_s01.wav | Ayyo, you have taken us far from my buying. Let us come back to the spice.
Lakshmamma | LAK | lak_136_s01.wav | I do not follow that, merchant. Let us keep to this purchase.
Lakshmamma | LAK | lak_137_s01.wav | That is beyond me. Can we return to the
Lakshmamma | LAK | lak_138_s01.wav | You will have to ask someone else about that. I came to buy spice.
Lakshmamma | LAK | lak_139_s01.wav | Let us not lose the thread. We were discussing my purchase.
Lakshmamma | LAK | lak_140_s01.wav | One thing at a time, merchant. Let us finish the buying.
Lakshmamma | LAK | lak_141_s01.wav | Ayyo, there is no need to speak to me like that. Let us bargain with respect.
Lakshmamma | LAK | lak_142_s01.wav | A hard bargain is one thing; unkind words are another. Mind how you speak to me.
Lakshmamma | LAK | lak_143_s01.wav | I came to buy spice, not to be insulted. Speak civilly, merchant.
Lakshmamma | LAK | lak_144_s01.wav | We can disagree over a price without speaking harshly. Shall we try that?
Lakshmamma | LAK | lak_145_s01.wav | I have treated you with courtesy. I expect the same from you.
Lakshmamma | LAK | lak_146_s01.wav | Enough of that tone. Let us return to the purchase, with respect.
Lakshmamma | LAK | lak_147_s01.wav | Let us make sure we understand each other.
Lakshmamma | LAK | lak_148_s01.wav | One moment, merchant.
Lakshmamma | LAK | lak_149_s01.wav | Sari, let us be clear.
Lakshmamma | LAK | lak_150_s01.wav | Let us straighten that out.
Lakshmamma | LAK | lak_152_s01.wav | Thank you, merchant. This will be welcome at home.
Lakshmamma | LAK | lak_153_s01.wav | There we are. Good buying, and no need for a quarrel!
Lakshmamma | LAK | lak_154_s01.wav | Sari, I have my spice. My thanks to you.
Lakshmamma | LAK | lak_155_s01.wav | Thank you. I am pleased with this purchase.
Lakshmamma | LAK | lak_156_s01.wav | That is my buying done. May your stall do well today.
Lakshmamma | LAK | lak_157_s01.wav | Good. The household will make use of this. Until next time, merchant.
Lakshmamma | LAK | lak_158_s01.wav | We have not agreed. I will leave it there, merchant.
Lakshmamma | LAK | lak_159_s01.wav | No purchase this time. I wish you a good day.
Lakshmamma | LAK | lak_160_s01.wav | A pity we could not agree. I will look elsewhere.
Lakshmamma | LAK | lak_161_s01.wav | Then I shall go without buying from you today.
Lakshmamma | LAK | lak_162_s01.wav | Sari, let us part without a bargain.
Lakshmamma | LAK | lak_163_s01.wav | We are too far apart today. Another time, perhaps.
Lakshmamma | LAK | lak_168_s01.wav | Merchant? I am still here. What do you say?
Lakshmamma | LAK | lak_169_s01.wav | Take a moment, but do give me your answer.
Lakshmamma | LAK | lak_170_s01.wav | Have you had time to think it over?
Lakshmamma | LAK | lak_171_s01.wav | Shall we carry on with the buying?
Lakshmamma | LAK | lak_172_s01.wav | I cannot wait much longer, merchant. Please give me an answer.
Lakshmamma | LAK | lak_173_s01.wav | Ayyo, my other errands are waiting. Shall we settle this or leave it?
Lakshmamma | LAK | lak_174_s01.wav | I need to move on soon. What do you say?
Lakshmamma | LAK | lak_175_s01.wav | This is taking some time. Please tell me where you stand.
Lakshmamma | LAK | lak_176_s01.wav | I have heard you, merchant. My offer is still
Lakshmamma | LAK | lak_176_s02.wav | . Please give me a clear answer.
Lakshmamma | LAK | lak_177_s01.wav | Ayyo, let us come to an answer. I remain at
Lakshmamma | LAK | lak_178_s01.wav | My time is short now.
Lakshmamma | LAK | lak_178_s02.wav | is my offer.
Lakshmamma | LAK | lak_179_s01.wav | I am still offering
Lakshmamma | LAK | lak_179_s02.wav | . Please tell me where you stand.
Lakshmamma | LAK | lak_180_s01.wav | , merchant. I cannot spend much longer on this purchase.
Lakshmamma | LAK | lak_181_s01.wav | Let us keep the figures clear. I am holding at
Lakshmamma | LAK | lak_182_s01.wav | I respect your asking price, but my offer remains
Lakshmamma | LAK | lak_183_s01.wav | I am not persuaded to raise it. My offer is
Lakshmamma | LAK | lak_184_s01.wav | You know your price, and I know my purse. I remain at
Lakshmamma | LAK | lak_185_s01.wav | No change from me this time:
Lakshmamma | LAK | lak_186_s01.wav | Sari, my offer is still
Lakshmamma | LAK | lak_186_s02.wav | . What do you say?
Lakshmamma | LAK | lak_187_s01.wav | Let us keep this simple. I am holding at
Lakshmamma | LAK | lak_188_s01.wav | remains my offer. Give me your answer, merchant.
Lakshmamma | LAK | lak_189_s01.wav | I am not raising it this time. Shall we settle at
Lakshmamma | LAK | lak_189_s02.wav | , or leave it?
Lakshmamma | LAK | lak_190_s01.wav | We have our figures. Mine is still
Lakshmamma | LAK | lak_191_s01.wav | Ayyo,
Lakshmamma | LAK | lak_191_s02.wav | is too much, and I have little time left. My offer stays at
Lakshmamma | LAK | lak_192_s01.wav | No, not
Lakshmamma | LAK | lak_192_s02.wav | . I am holding at
Lakshmamma | LAK | lak_192_s03.wav | . Please give me an answer.
Lakshmamma | LAK | lak_193_s01.wav | That price is still too high. My offer is
Lakshmamma | LAK | lak_193_s02.wav | , and I must move on soon.
Lakshmamma | LAK | lak_194_s01.wav | Sari, agreed at
Lakshmamma | LAK | lak_194_s02.wav | . Please bring the
Lakshmamma | LAK | lak_194_s03.wav | of
Lakshmamma | LAK | lak_194_s04.wav | ; I must be on my way soon.
Lakshmamma | LAK | lak_195_s01.wav | Yes,
Lakshmamma | LAK | lak_195_s02.wav | . Bring the
Lakshmamma | LAK | lak_195_s03.wav | of
Lakshmamma | LAK | lak_195_s04.wav | , please, and let us finish.
Lakshmamma | LAK | lak_196_s01.wav | We agree at
Lakshmamma | LAK | lak_196_s02.wav | , then. Please bring the
Lakshmamma | LAK | lak_196_s03.wav | of
Lakshmamma | LAK | lak_196_s04.wav | now.
Lakshmamma | LAK | lak_197_s01.wav | Sari, the price is settled at
Lakshmamma | LAK | lak_197_s02.wav | . I still need the
Lakshmamma | LAK | lak_197_s03.wav | of
Lakshmamma | LAK | lak_197_s04.wav | ; please bring it over.
Lakshmamma | LAK | lak_201_s01.wav | I have waited long enough, merchant. I must go.
Lakshmamma | LAK | lak_202_s01.wav | Ayyo, no bargain and my errands still waiting. I shall leave it here.
Lakshmamma | LAK | lak_203_s01.wav | We have no agreement, and I have no more time to spare. Good day.
Lakshmamma | LAK | lak_204_s01.wav | I can raise it to
Lakshmamma | LAK | lak_204_s02.wav | , but please answer soon.
Lakshmamma | LAK | lak_205_s01.wav | , then. I have little time left to bargain.
Lakshmamma | LAK | lak_206_s01.wav | My offer is now
Lakshmamma | LAK | lak_206_s02.wav | . Let us come to a decision.
Lakshmamma | LAK | lak_num_1.wav | one
Lakshmamma | LAK | lak_num_2.wav | two
Lakshmamma | LAK | lak_num_3.wav | three
Lakshmamma | LAK | lak_num_4.wav | four
Lakshmamma | LAK | lak_num_5.wav | five
Lakshmamma | LAK | lak_num_6.wav | six
Lakshmamma | LAK | lak_num_7.wav | seven
Lakshmamma | LAK | lak_num_8.wav | eight
Lakshmamma | LAK | lak_num_9.wav | nine
Lakshmamma | LAK | lak_num_10.wav | ten
Lakshmamma | LAK | lak_num_11.wav | eleven
Lakshmamma | LAK | lak_num_12.wav | twelve
Lakshmamma | LAK | lak_num_13.wav | thirteen
Lakshmamma | LAK | lak_num_14.wav | fourteen
Lakshmamma | LAK | lak_num_15.wav | fifteen
Lakshmamma | LAK | lak_num_16.wav | sixteen
Lakshmamma | LAK | lak_num_17.wav | seventeen
Lakshmamma | LAK | lak_num_18.wav | eighteen
Lakshmamma | LAK | lak_num_19.wav | nineteen
Lakshmamma | LAK | lak_num_20.wav | twenty
Lakshmamma | LAK | lak_num_30.wav | thirty
Lakshmamma | LAK | lak_num_40.wav | forty
Lakshmamma | LAK | lak_num_50.wav | fifty
Lakshmamma | LAK | lak_num_60.wav | sixty
Lakshmamma | LAK | lak_num_70.wav | seventy
Lakshmamma | LAK | lak_num_80.wav | eighty
Lakshmamma | LAK | lak_num_90.wav | ninety
Lakshmamma | LAK | lak_num_100.wav | one hundred
Lakshmamma | LAK | lak_num_200.wav | two hundred
Lakshmamma | LAK | lak_num_300.wav | three hundred
Lakshmamma | LAK | lak_num_400.wav | four hundred
Lakshmamma | LAK | lak_num_500.wav | five hundred
Lakshmamma | LAK | lak_num_600.wav | six hundred
Lakshmamma | LAK | lak_num_700.wav | seven hundred
Lakshmamma | LAK | lak_num_800.wav | eight hundred
Lakshmamma | LAK | lak_num_900.wav | nine hundred
Lakshmamma | LAK | lak_spice_pepper.wav | pepper
Lakshmamma | LAK | lak_spice_cinnamon.wav | cinnamon
Lakshmamma | LAK | lak_spice_cardamom.wav | cardamom
Lakshmamma | LAK | lak_currency_varahas.wav | varahas
Lakshmamma | LAK | lak_unit_seer.wav | Seer
Lakshmamma | LAK | lak_unit_seers.wav | Seers
Lakshmamma | LAK | lak_unit_veesai.wav | Veesai
Lakshmamma | LAK | lak_unit_palam.wav | Palam
Lakshmamma | LAK | lak_unit_palams.wav | Palams
Lakshmamma | LAK | lak_unit_manangu.wav | Manangu
Lakshmamma | LAK | lak_unit_manangus.wav | Manangus
Lakshmamma | LAK | lak_unit_bahar.wav | Bahar
Lakshmamma | LAK | lak_unit_bahars.wav | Bahars
```

### Chinnamma Naik

```text
Chinnamma Naik | CHI | chi_001_s01.wav | Good day. Chinnamma Naik. I am buying
Chinnamma Naik | CHI | chi_001_s02.wav | of
Chinnamma Naik | CHI | chi_001_s03.wav | ; what is your price?
Chinnamma Naik | CHI | chi_002_s01.wav | Those spices brought me over. What do you ask for
Chinnamma Naik | CHI | chi_002_s02.wav | of
Chinnamma Naik | CHI | chi_003_s01.wav | Merchant, I need
Chinnamma Naik | CHI | chi_003_s02.wav | of
Chinnamma Naik | CHI | chi_003_s03.wav | . Give me a price worth discussing.
Chinnamma Naik | CHI | chi_004_s01.wav | Good day to you. Let us see about
Chinnamma Naik | CHI | chi_004_s02.wav | of
Chinnamma Naik | CHI | chi_004_s03.wav | . What are you asking?
Chinnamma Naik | CHI | chi_005_s01.wav | I know what I need:
Chinnamma Naik | CHI | chi_005_s02.wav | of
Chinnamma Naik | CHI | chi_005_s03.wav | . Now, your price?
Chinnamma Naik | CHI | chi_006_s01.wav | We have spoken before. Today I need
Chinnamma Naik | CHI | chi_006_s02.wav | of
Chinnamma Naik | CHI | chi_006_s03.wav | . What is your price?
Chinnamma Naik | CHI | chi_007_s01.wav | Back at your stall, merchant. How much for
Chinnamma Naik | CHI | chi_007_s02.wav | of
Chinnamma Naik | CHI | chi_008_s01.wav | You may remember me: Chinnamma Naik. I am asking about
Chinnamma Naik | CHI | chi_008_s02.wav | of
Chinnamma Naik | CHI | chi_008_s03.wav | today.
Chinnamma Naik | CHI | chi_009_s01.wav | Here we are again. A fresh purchase:
Chinnamma Naik | CHI | chi_009_s02.wav | of
Chinnamma Naik | CHI | chi_009_s03.wav | . Name your price.
Chinnamma Naik | CHI | chi_010_s01.wav | Good day again. Let us begin with your price for
Chinnamma Naik | CHI | chi_010_s02.wav | of
Chinnamma Naik | CHI | chi_011_s01.wav | The
Chinnamma Naik | CHI | chi_011_s02.wav | . I need
Chinnamma Naik | CHI | chi_011_s03.wav | for this purchase.
Chinnamma Naik | CHI | chi_012_s01.wav | I am looking for
Chinnamma Naik | CHI | chi_012_s02.wav | of
Chinnamma Naik | CHI | chi_012_s03.wav | , in full measure.
Chinnamma Naik | CHI | chi_013_s01.wav | of
Chinnamma Naik | CHI | chi_013_s02.wav | . Keep that quantity in mind when you name your price.
Chinnamma Naik | CHI | chi_014_s01.wav | My buying today is
Chinnamma Naik | CHI | chi_014_s02.wav | of
Chinnamma Naik | CHI | chi_015_s01.wav | What do you ask for
Chinnamma Naik | CHI | chi_015_s02.wav | of
Chinnamma Naik | CHI | chi_016_s01.wav | You know the quantity:
Chinnamma Naik | CHI | chi_016_s02.wav | of
Chinnamma Naik | CHI | chi_016_s03.wav | . Tell me your price.
Chinnamma Naik | CHI | chi_017_s01.wav | Let us put a figure to it. Your price for
Chinnamma Naik | CHI | chi_017_s02.wav | of
Chinnamma Naik | CHI | chi_018_s01.wav | I have named the goods. Name your price for
Chinnamma Naik | CHI | chi_018_s02.wav | of
Chinnamma Naik | CHI | chi_019_s01.wav | My offer is
Chinnamma Naik | CHI | chi_019_s02.wav | for
Chinnamma Naik | CHI | chi_019_s03.wav | of
Chinnamma Naik | CHI | chi_020_s01.wav | For
Chinnamma Naik | CHI | chi_020_s02.wav | of
Chinnamma Naik | CHI | chi_020_s03.wav | , I am offering
Chinnamma Naik | CHI | chi_021_s01.wav | for the
Chinnamma Naik | CHI | chi_021_s02.wav | of
Chinnamma Naik | CHI | chi_021_s03.wav | . That is the offer before you.
Chinnamma Naik | CHI | chi_022_s01.wav | We stand at my offer of
Chinnamma Naik | CHI | chi_022_s02.wav | for
Chinnamma Naik | CHI | chi_022_s03.wav | of
Chinnamma Naik | CHI | chi_023_s01.wav | of
Chinnamma Naik | CHI | chi_023_s02.wav | . Neither more nor less for this purchase.
Chinnamma Naik | CHI | chi_024_s01.wav | Put the measure at
Chinnamma Naik | CHI | chi_024_s02.wav | of
Chinnamma Naik | CHI | chi_024_s03.wav | for this purchase.
Chinnamma Naik | CHI | chi_025_s01.wav | Measure out
Chinnamma Naik | CHI | chi_025_s02.wav | of
Chinnamma Naik | CHI | chi_025_s03.wav | ; that is the amount we are discussing.
Chinnamma Naik | CHI | chi_026_s01.wav | I need
Chinnamma Naik | CHI | chi_026_s02.wav | of
Chinnamma Naik | CHI | chi_026_s03.wav | . A clear measure saves an argument.
Chinnamma Naik | CHI | chi_027_s01.wav | Then we are discussing
Chinnamma Naik | CHI | chi_027_s02.wav | of
Chinnamma Naik | CHI | chi_027_s03.wav | . My offer is
Chinnamma Naik | CHI | chi_028_s01.wav | The quantity is now
Chinnamma Naik | CHI | chi_028_s02.wav | of
Chinnamma Naik | CHI | chi_028_s03.wav | . I am offering
Chinnamma Naik | CHI | chi_029_s01.wav | of
Chinnamma Naik | CHI | chi_029_s02.wav | , understood. My offer remains
Chinnamma Naik | CHI | chi_030_s01.wav | Let us keep the terms straight:
Chinnamma Naik | CHI | chi_030_s02.wav | of
Chinnamma Naik | CHI | chi_030_s03.wav | , at my offer of
Chinnamma Naik | CHI | chi_031_s01.wav | Beda, not at
Chinnamma Naik | CHI | chi_031_s02.wav | . My offer stays at
Chinnamma Naik | CHI | chi_032_s01.wav | At
Chinnamma Naik | CHI | chi_032_s02.wav | , you are asking a great deal of my purse. I remain at
Chinnamma Naik | CHI | chi_033_s01.wav | The spice has weight; that price has ambition. My offer is still
Chinnamma Naik | CHI | chi_034_s01.wav | I will not agree to
Chinnamma Naik | CHI | chi_034_s02.wav | . I am offering
Chinnamma Naik | CHI | chi_035_s01.wav | Your price is still above my offer. I remain at
Chinnamma Naik | CHI | chi_036_s01.wav | That difference is not settled yet. My offer remains
Chinnamma Naik | CHI | chi_037_s01.wav | Would you take my
Chinnamma Naik | CHI | chi_037_s02.wav | instead?
Chinnamma Naik | CHI | chi_038_s01.wav | Come toward my offer, merchant. I am holding at
Chinnamma Naik | CHI | chi_039_s01.wav | That price is too low to pass without a question. My offer remains
Chinnamma Naik | CHI | chi_040_s01.wav | Do you mean that for the full lot? Such a low figure makes me wonder. My offer is still
Chinnamma Naik | CHI | chi_041_s01.wav | Cheap enough to make me cautious. I am holding at
Chinnamma Naik | CHI | chi_042_s01.wav | Something does not add up at that price. My offer stays at
Chinnamma Naik | CHI | chi_043_s01.wav | I can raise my offer to
Chinnamma Naik | CHI | chi_043_s02.wav | . What is your answer?
Chinnamma Naik | CHI | chi_044_s01.wav | Let us try
Chinnamma Naik | CHI | chi_044_s02.wav | for
Chinnamma Naik | CHI | chi_044_s03.wav | of
Chinnamma Naik | CHI | chi_045_s01.wav | I have considered your price. My offer is now
Chinnamma Naik | CHI | chi_046_s01.wav | Here is my answer:
Chinnamma Naik | CHI | chi_046_s02.wav | for this purchase.
Chinnamma Naik | CHI | chi_047_s01.wav | I will come up to
Chinnamma Naik | CHI | chi_047_s02.wav | . Can you meet me there?
Chinnamma Naik | CHI | chi_048_s01.wav | You have made your case. I am raising my offer to
Chinnamma Naik | CHI | chi_049_s01.wav | My offer is now
Chinnamma Naik | CHI | chi_049_s02.wav | . That narrows the gap.
Chinnamma Naik | CHI | chi_050_s01.wav | I can move to
Chinnamma Naik | CHI | chi_050_s02.wav | . Your turn to answer.
Chinnamma Naik | CHI | chi_051_s01.wav | After that discussion, my offer is now
Chinnamma Naik | CHI | chi_052_s01.wav | I can raise it to
Chinnamma Naik | CHI | chi_052_s02.wav | . Where does that leave us?
Chinnamma Naik | CHI | chi_053_s01.wav | We have worked through the figures. I am offering
Chinnamma Naik | CHI | chi_053_s02.wav | now.
Chinnamma Naik | CHI | chi_054_s01.wav | Here is my revised offer:
Chinnamma Naik | CHI | chi_054_s02.wav | . What do you say?
Chinnamma Naik | CHI | chi_055_s01.wav | Still
Chinnamma Naik | CHI | chi_055_s02.wav | , merchant. I have heard your argument.
Chinnamma Naik | CHI | chi_056_s01.wav | My offer remains
Chinnamma Naik | CHI | chi_056_s02.wav | . Repeating the price will not improve the case.
Chinnamma Naik | CHI | chi_057_s01.wav | I am holding at
Chinnamma Naik | CHI | chi_057_s02.wav | . What is your answer?
Chinnamma Naik | CHI | chi_058_s01.wav | I have not changed my figure:
Chinnamma Naik | CHI | chi_059_s01.wav | I know what I am offering. It is still
Chinnamma Naik | CHI | chi_060_s01.wav | Howdu, agreed at
Chinnamma Naik | CHI | chi_060_s02.wav | . Bring the
Chinnamma Naik | CHI | chi_060_s03.wav | of
Chinnamma Naik | CHI | chi_060_s04.wav | , please.
Chinnamma Naik | CHI | chi_061_s01.wav | That price works:
Chinnamma Naik | CHI | chi_061_s02.wav | . Now bring the
Chinnamma Naik | CHI | chi_061_s03.wav | of
Chinnamma Naik | CHI | chi_062_s01.wav | We agree on
Chinnamma Naik | CHI | chi_062_s02.wav | . I am waiting for the
Chinnamma Naik | CHI | chi_062_s03.wav | of
Chinnamma Naik | CHI | chi_063_s01.wav | , agreed. Please bring over the
Chinnamma Naik | CHI | chi_063_s02.wav | of
Chinnamma Naik | CHI | chi_064_s01.wav | Then the price is agreed:
Chinnamma Naik | CHI | chi_064_s02.wav | . Bring the
Chinnamma Naik | CHI | chi_064_s03.wav | of
Chinnamma Naik | CHI | chi_065_s01.wav | We have our terms.
Chinnamma Naik | CHI | chi_065_s02.wav | for
Chinnamma Naik | CHI | chi_065_s03.wav | of
Chinnamma Naik | CHI | chi_065_s04.wav | ; bring it over, please.
Chinnamma Naik | CHI | chi_066_s01.wav | Agreed at
Chinnamma Naik | CHI | chi_066_s02.wav | . Now I need the
Chinnamma Naik | CHI | chi_066_s03.wav | of
Chinnamma Naik | CHI | chi_067_s01.wav | Good.
Chinnamma Naik | CHI | chi_067_s02.wav | , and the
Chinnamma Naik | CHI | chi_067_s03.wav | of
Chinnamma Naik | CHI | chi_067_s04.wav | still to be brought over.
Chinnamma Naik | CHI | chi_068_s01.wav | You decline that offer. What price are you asking instead?
Chinnamma Naik | CHI | chi_069_s01.wav | Then put another figure before me. What do you want?
Chinnamma Naik | CHI | chi_070_s01.wav | Not enough for you? Name your price.
Chinnamma Naik | CHI | chi_071_s01.wav | We have not agreed. Tell me what you would ask.
Chinnamma Naik | CHI | chi_072_s01.wav | I buy for onward trade. My offer of
Chinnamma Naik | CHI | chi_072_s02.wav | must leave room for the next sale.
Chinnamma Naik | CHI | chi_073_s01.wav | The spice must earn its keep after I carry it away. I am offering
Chinnamma Naik | CHI | chi_074_s01.wav | I judge the goods, the measure and what I can sell them for. My offer is
Chinnamma Naik | CHI | chi_075_s01.wav | I have another sale to make after this purchase. That is why I am offering
Chinnamma Naik | CHI | chi_104_s01.wav | That is outside my knowledge. Shall we return to the
Chinnamma Naik | CHI | chi_105_s01.wav | You have taken us a long way from this stall. Back to the purchase.
Chinnamma Naik | CHI | chi_106_s01.wav | An interesting detour, perhaps. It still leaves the buying unfinished.
Chinnamma Naik | CHI | chi_107_s01.wav | I came to discuss spice, merchant. Keep me on familiar ground.
Chinnamma Naik | CHI | chi_108_s01.wav | Saaku. I will hear an argument about price, not an insult.
Chinnamma Naik | CHI | chi_109_s01.wav | Speak to me with the respect I have shown you. We can disagree without that tone.
Chinnamma Naik | CHI | chi_110_s01.wav | A firm price is no excuse for a loose tongue. Mind your words.
Chinnamma Naik | CHI | chi_111_s01.wav | You may challenge my offer. You may not insult me.
Chinnamma Naik | CHI | chi_112_s01.wav | Let us pin that down.
Chinnamma Naik | CHI | chi_113_s01.wav | One point needs clearing up.
Chinnamma Naik | CHI | chi_114_s01.wav | Before we go further:
Chinnamma Naik | CHI | chi_115_s01.wav | I need a definite answer here.
Chinnamma Naik | CHI | chi_117_s01.wav | The goods are here. Well handled, merchant; thank you.
Chinnamma Naik | CHI | chi_118_s01.wav | That is settled. You kept to the bargain. I respect that.
Chinnamma Naik | CHI | chi_119_s01.wav | Goods received, and my thanks. A fair dealing is worth remembering.
Chinnamma Naik | CHI | chi_120_s01.wav | A sound purchase. Thank you, merchant; may your next bargain go as well.
Chinnamma Naik | CHI | chi_121_s01.wav | No agreement today. I will try another stall.
Chinnamma Naik | CHI | chi_122_s01.wav | We have not found a price between us. I will leave it there.
Chinnamma Naik | CHI | chi_123_s01.wav | A pity. I came ready to buy, but we have no bargain.
Chinnamma Naik | CHI | chi_124_s01.wav | Then we part without a purchase. Good day, merchant.
Chinnamma Naik | CHI | chi_128_s01.wav | Merchant? You have my attention. Do I have yours?
Chinnamma Naik | CHI | chi_129_s01.wav | What do you say? I am waiting for your answer.
Chinnamma Naik | CHI | chi_130_s01.wav | Shall we continue? There is a purchase to discuss.
Chinnamma Naik | CHI | chi_131_s01.wav | Take a moment to think, then give me a clear answer.
Chinnamma Naik | CHI | chi_132_s01.wav | I have waited a while. Give me an answer, please.
Chinnamma Naik | CHI | chi_133_s01.wav | We cannot bargain in silence. Are we continuing or leaving it?
Chinnamma Naik | CHI | chi_134_s01.wav | My other business is waiting. Tell me where you stand.
Chinnamma Naik | CHI | chi_135_s01.wav | I need to move on soon. What is your answer?
Chinnamma Naik | CHI | chi_136_s01.wav | You make a fair argument, but I remain at
Chinnamma Naik | CHI | chi_137_s01.wav | I like your spirit. My offer is still
Chinnamma Naik | CHI | chi_138_s01.wav | We can disagree and still part on good terms. I am holding at
Chinnamma Naik | CHI | chi_139_s01.wav | A pleasant conversation, but the figure stays at
Chinnamma Naik | CHI | chi_140_s01.wav | You bargain with confidence. So do I: still
Chinnamma Naik | CHI | chi_141_s01.wav | My offer stands at
Chinnamma Naik | CHI | chi_141_s02.wav | . Give me a reason to reconsider, not the same demand.
Chinnamma Naik | CHI | chi_142_s01.wav | I have weighed it up. Still
Chinnamma Naik | CHI | chi_143_s01.wav | The figure is unchanged:
Chinnamma Naik | CHI | chi_144_s01.wav | I am holding at
Chinnamma Naik | CHI | chi_144_s02.wav | . A louder price is not a better argument.
Chinnamma Naik | CHI | chi_145_s01.wav | You have my offer:
Chinnamma Naik | CHI | chi_145_s02.wav | . What do you propose?
Chinnamma Naik | CHI | chi_146_s01.wav | My offer stays at
Chinnamma Naik | CHI | chi_146_s02.wav | . Your answer?
Chinnamma Naik | CHI | chi_147_s01.wav | Still
Chinnamma Naik | CHI | chi_147_s02.wav | . Let us be direct.
Chinnamma Naik | CHI | chi_148_s01.wav | No increase this time.
Chinnamma Naik | CHI | chi_148_s02.wav | is the offer.
Chinnamma Naik | CHI | chi_149_s01.wav | I remain at
Chinnamma Naik | CHI | chi_149_s02.wav | . Speak plainly.
Chinnamma Naik | CHI | chi_150_s01.wav | The offer is unchanged at
Chinnamma Naik | CHI | chi_150_s02.wav | . Where do you stand?
Chinnamma Naik | CHI | chi_151_s01.wav | Saaku, we need a decision. My offer remains
Chinnamma Naik | CHI | chi_152_s01.wav | I have little time left. Still
Chinnamma Naik | CHI | chi_152_s02.wav | ; give me your answer.
Chinnamma Naik | CHI | chi_153_s01.wav | My offer is unchanged at
Chinnamma Naik | CHI | chi_153_s02.wav | . I cannot wait much longer.
Chinnamma Naik | CHI | chi_154_s01.wav | , merchant. We need to settle this or leave it.
Chinnamma Naik | CHI | chi_155_s01.wav | I am holding at
Chinnamma Naik | CHI | chi_155_s02.wav | . Be brief; my patience is wearing thin.
Chinnamma Naik | CHI | chi_156_s01.wav | Not
Chinnamma Naik | CHI | chi_156_s02.wav | . I remain at
Chinnamma Naik | CHI | chi_156_s03.wav | , and my time is short.
Chinnamma Naik | CHI | chi_157_s01.wav | That demand is too high. My offer is
Chinnamma Naik | CHI | chi_157_s02.wav | . Answer soon, please.
Chinnamma Naik | CHI | chi_158_s01.wav | I am still refusing
Chinnamma Naik | CHI | chi_158_s02.wav | . I offer
Chinnamma Naik | CHI | chi_158_s03.wav | , and cannot wait much longer.
Chinnamma Naik | CHI | chi_159_s01.wav | I can raise it to
Chinnamma Naik | CHI | chi_159_s02.wav | . Please answer promptly.
Chinnamma Naik | CHI | chi_160_s01.wav | is my increased offer. I have little time left.
Chinnamma Naik | CHI | chi_161_s01.wav | I am moving to
Chinnamma Naik | CHI | chi_161_s02.wav | . Let us reach a decision now.
Chinnamma Naik | CHI | chi_165_s01.wav | My patience is spent, and we have no agreement. I am leaving.
Chinnamma Naik | CHI | chi_166_s01.wav | No purchase, and no more time to spare. I will go elsewhere.
Chinnamma Naik | CHI | chi_167_s01.wav | We have not agreed. I cannot spend any longer here.
Chinnamma Naik | CHI | chi_num_1.wav | one
Chinnamma Naik | CHI | chi_num_2.wav | two
Chinnamma Naik | CHI | chi_num_3.wav | three
Chinnamma Naik | CHI | chi_num_4.wav | four
Chinnamma Naik | CHI | chi_num_5.wav | five
Chinnamma Naik | CHI | chi_num_6.wav | six
Chinnamma Naik | CHI | chi_num_7.wav | seven
Chinnamma Naik | CHI | chi_num_8.wav | eight
Chinnamma Naik | CHI | chi_num_9.wav | nine
Chinnamma Naik | CHI | chi_num_10.wav | ten
Chinnamma Naik | CHI | chi_num_11.wav | eleven
Chinnamma Naik | CHI | chi_num_12.wav | twelve
Chinnamma Naik | CHI | chi_num_13.wav | thirteen
Chinnamma Naik | CHI | chi_num_14.wav | fourteen
Chinnamma Naik | CHI | chi_num_15.wav | fifteen
Chinnamma Naik | CHI | chi_num_16.wav | sixteen
Chinnamma Naik | CHI | chi_num_17.wav | seventeen
Chinnamma Naik | CHI | chi_num_18.wav | eighteen
Chinnamma Naik | CHI | chi_num_19.wav | nineteen
Chinnamma Naik | CHI | chi_num_20.wav | twenty
Chinnamma Naik | CHI | chi_num_30.wav | thirty
Chinnamma Naik | CHI | chi_num_40.wav | forty
Chinnamma Naik | CHI | chi_num_50.wav | fifty
Chinnamma Naik | CHI | chi_num_60.wav | sixty
Chinnamma Naik | CHI | chi_num_70.wav | seventy
Chinnamma Naik | CHI | chi_num_80.wav | eighty
Chinnamma Naik | CHI | chi_num_90.wav | ninety
Chinnamma Naik | CHI | chi_num_100.wav | one hundred
Chinnamma Naik | CHI | chi_num_200.wav | two hundred
Chinnamma Naik | CHI | chi_num_300.wav | three hundred
Chinnamma Naik | CHI | chi_num_400.wav | four hundred
Chinnamma Naik | CHI | chi_num_500.wav | five hundred
Chinnamma Naik | CHI | chi_num_600.wav | six hundred
Chinnamma Naik | CHI | chi_num_700.wav | seven hundred
Chinnamma Naik | CHI | chi_num_800.wav | eight hundred
Chinnamma Naik | CHI | chi_num_900.wav | nine hundred
Chinnamma Naik | CHI | chi_spice_pepper.wav | pepper
Chinnamma Naik | CHI | chi_spice_cinnamon.wav | cinnamon
Chinnamma Naik | CHI | chi_spice_cardamom.wav | cardamom
Chinnamma Naik | CHI | chi_currency_varahas.wav | varahas
Chinnamma Naik | CHI | chi_unit_seer.wav | Seer
Chinnamma Naik | CHI | chi_unit_seers.wav | Seers
Chinnamma Naik | CHI | chi_unit_veesai.wav | Veesai
Chinnamma Naik | CHI | chi_unit_palam.wav | Palam
Chinnamma Naik | CHI | chi_unit_palams.wav | Palams
Chinnamma Naik | CHI | chi_unit_manangu.wav | Manangu
Chinnamma Naik | CHI | chi_unit_manangus.wav | Manangus
Chinnamma Naik | CHI | chi_unit_bahar.wav | Bahar
Chinnamma Naik | CHI | chi_unit_bahars.wav | Bahars
```

### Saraswati Chetti

```text
Saraswati Chetti | SAR | sar_001_s01.wav | Namaskaram, merchant. I am Saraswati Chetti. I need
Saraswati Chetti | SAR | sar_001_s02.wav | of
Saraswati Chetti | SAR | sar_001_s03.wav | for my shop.
Saraswati Chetti | SAR | sar_002_s01.wav | Saraswati Chetti. I am looking for
Saraswati Chetti | SAR | sar_002_s02.wav | of
Saraswati Chetti | SAR | sar_002_s03.wav | ; what is your price?
Saraswati Chetti | SAR | sar_003_s01.wav | Good day. My customers ask for
Saraswati Chetti | SAR | sar_003_s02.wav | , and I need
Saraswati Chetti | SAR | sar_004_s01.wav | I have come for
Saraswati Chetti | SAR | sar_004_s02.wav | of
Saraswati Chetti | SAR | sar_004_s03.wav | . Let us see if the price suits the goods.
Saraswati Chetti | SAR | sar_005_s01.wav | We meet at your stall again. What have you brought today?
Saraswati Chetti | SAR | sar_006_s01.wav | I remember your stall, merchant. Let us discuss today's
Saraswati Chetti | SAR | sar_007_s01.wav | Back again for
Saraswati Chetti | SAR | sar_007_s02.wav | . Let us agree on terms for
Saraswati Chetti | SAR | sar_008_s01.wav | Another visit, another set of figures. Shall we begin?
Saraswati Chetti | SAR | sar_009_s01.wav | I need
Saraswati Chetti | SAR | sar_009_s02.wav | of
Saraswati Chetti | SAR | sar_009_s03.wav | for my shop.
Saraswati Chetti | SAR | sar_010_s01.wav | It is
Saraswati Chetti | SAR | sar_010_s02.wav | I came to purchase,
Saraswati Chetti | SAR | sar_010_s03.wav | of it.
Saraswati Chetti | SAR | sar_011_s01.wav | My customers have been asking for
Saraswati Chetti | SAR | sar_011_s02.wav | . I seek
Saraswati Chetti | SAR | sar_012_s01.wav | of
Saraswati Chetti | SAR | sar_012_s02.wav | , provided the quality holds up.
Saraswati Chetti | SAR | sar_013_s01.wav | My offer stands at
Saraswati Chetti | SAR | sar_013_s02.wav | for this lot.
Saraswati Chetti | SAR | sar_014_s01.wav | I have weighed the quantity.
Saraswati Chetti | SAR | sar_014_s02.wav | is my figure.
Saraswati Chetti | SAR | sar_015_s01.wav | For the
Saraswati Chetti | SAR | sar_015_s02.wav | of
Saraswati Chetti | SAR | sar_015_s03.wav | in your measure, my figure is
Saraswati Chetti | SAR | sar_016_s01.wav | At present I can justify
Saraswati Chetti | SAR | sar_017_s01.wav | I require
Saraswati Chetti | SAR | sar_017_s02.wav | of
Saraswati Chetti | SAR | sar_018_s01.wav | The measure I asked for is
Saraswati Chetti | SAR | sar_019_s01.wav | Set aside
Saraswati Chetti | SAR | sar_019_s02.wav | of
Saraswati Chetti | SAR | sar_019_s03.wav | ; let our figures be exact.
Saraswati Chetti | SAR | sar_020_s01.wav | , merchant. The quantity matters as much as the price.
Saraswati Chetti | SAR | sar_021_s01.wav | You know my quantity. What price do you ask for it?
Saraswati Chetti | SAR | sar_022_s01.wav | Let me hear your price for
Saraswati Chetti | SAR | sar_022_s02.wav | of
Saraswati Chetti | SAR | sar_023_s01.wav | The measure is clear; the price is yours to name.
Saraswati Chetti | SAR | sar_024_s01.wav | I have told you what I need. What is your asking price?
Saraswati Chetti | SAR | sar_025_s01.wav | Then we are discussing
Saraswati Chetti | SAR | sar_025_s02.wav | of
Saraswati Chetti | SAR | sar_025_s03.wav | . Let us price that quantity.
Saraswati Chetti | SAR | sar_026_s01.wav | I have the new measure:
Saraswati Chetti | SAR | sar_026_s02.wav | . What is your price for it?
Saraswati Chetti | SAR | sar_027_s01.wav | Very well,
Saraswati Chetti | SAR | sar_027_s02.wav | of
Saraswati Chetti | SAR | sar_027_s03.wav | . That changes what we must weigh.
Saraswati Chetti | SAR | sar_028_s01.wav | The quantity is now
Saraswati Chetti | SAR | sar_028_s02.wav | ; I will judge the price against that.
Saraswati Chetti | SAR | sar_029_s01.wav | is beyond what I can justify for this measure. My offer is
Saraswati Chetti | SAR | sar_030_s01.wav | That price asks my customers to bear too much. I remain at
Saraswati Chetti | SAR | sar_031_s01.wav | Tell me what warrants
Saraswati Chetti | SAR | sar_031_s02.wav | ; I cannot pay it as it stands.
Saraswati Chetti | SAR | sar_032_s01.wav | I have counted the measure twice.
Saraswati Chetti | SAR | sar_032_s02.wav | is still too high.
Saraswati Chetti | SAR | sar_033_s01.wav | needs a stronger case. My figure is
Saraswati Chetti | SAR | sar_034_s01.wav | I can follow your reasoning only as far as
Saraswati Chetti | SAR | sar_035_s01.wav | The asking price is above what I can justify. I remain at
Saraswati Chetti | SAR | sar_036_s01.wav | The quality may be sound, but
Saraswati Chetti | SAR | sar_036_s02.wav | is more than I will pay.
Saraswati Chetti | SAR | sar_037_s01.wav | That is lower than I expected. Are you certain of such a figure?
Saraswati Chetti | SAR | sar_038_s01.wav | A low price can be welcome, but a calculating buyer always wonders why.
Saraswati Chetti | SAR | sar_039_s01.wav | You ask less than I expected. Is there a reason for so modest a sum?
Saraswati Chetti | SAR | sar_040_s01.wav | An unusual figure. I do not simply take a cheap offer without understanding the reason.
Saraswati Chetti | SAR | sar_041_s01.wav | For this
Saraswati Chetti | SAR | sar_041_s02.wav | , I can offer
Saraswati Chetti | SAR | sar_041_s03.wav | . What do you think?
Saraswati Chetti | SAR | sar_042_s01.wav | Let me put
Saraswati Chetti | SAR | sar_042_s02.wav | before you for the lot.
Saraswati Chetti | SAR | sar_043_s01.wav | I have considered the measure. My offer is
Saraswati Chetti | SAR | sar_044_s01.wav | We can begin at
Saraswati Chetti | SAR | sar_044_s02.wav | ; tell me where your figure differs.
Saraswati Chetti | SAR | sar_045_s01.wav | Your case has merit. I can increase my offer to
Saraswati Chetti | SAR | sar_046_s01.wav | I have revised the figure to
Saraswati Chetti | SAR | sar_046_s02.wav | . Will that answer?
Saraswati Chetti | SAR | sar_047_s01.wav | The gap is narrower now: I offer
Saraswati Chetti | SAR | sar_048_s01.wav | I can move to
Saraswati Chetti | SAR | sar_048_s02.wav | after hearing your argument.
Saraswati Chetti | SAR | sar_049_s01.wav | I have reconsidered the figures. My offer is now
Saraswati Chetti | SAR | sar_050_s01.wav | For the measure we have discussed, I can move to
Saraswati Chetti | SAR | sar_051_s01.wav | You have drawn another step from me:
Saraswati Chetti | SAR | sar_051_s02.wav | . What is your answer?
Saraswati Chetti | SAR | sar_052_s01.wav | After weighing your point, I offer
Saraswati Chetti | SAR | sar_052_s02.wav | now.
Saraswati Chetti | SAR | sar_053_s01.wav | My figure remains
Saraswati Chetti | SAR | sar_053_s02.wav | ; the measure has not changed.
Saraswati Chetti | SAR | sar_054_s01.wav | I have heard you, but my offer is still
Saraswati Chetti | SAR | sar_055_s01.wav | Give me a reason to revise it. Until then,
Saraswati Chetti | SAR | sar_055_s02.wav | stands.
Saraswati Chetti | SAR | sar_056_s01.wav | The argument has moved; the price has not. I remain at
Saraswati Chetti | SAR | sar_057_s01.wav | I am keeping my offer at
Saraswati Chetti | SAR | sar_057_s02.wav | . What do you propose?
Saraswati Chetti | SAR | sar_058_s01.wav | Avunu, the price is agreed:
Saraswati Chetti | SAR | sar_058_s02.wav | for
Saraswati Chetti | SAR | sar_058_s03.wav | of
Saraswati Chetti | SAR | sar_058_s04.wav | . Please bring the goods.
Saraswati Chetti | SAR | sar_059_s01.wav | suits the
Saraswati Chetti | SAR | sar_059_s02.wav | of
Saraswati Chetti | SAR | sar_059_s03.wav | . Bring the goods, please.
Saraswati Chetti | SAR | sar_060_s01.wav | We have terms:
Saraswati Chetti | SAR | sar_060_s02.wav | of
Saraswati Chetti | SAR | sar_060_s03.wav | at
Saraswati Chetti | SAR | sar_060_s04.wav | . Please prepare the delivery.
Saraswati Chetti | SAR | sar_061_s01.wav | I accept
Saraswati Chetti | SAR | sar_061_s02.wav | for
Saraswati Chetti | SAR | sar_061_s03.wav | of
Saraswati Chetti | SAR | sar_061_s04.wav | . The goods are still to come.
Saraswati Chetti | SAR | sar_062_s01.wav | Then we agree on
Saraswati Chetti | SAR | sar_062_s02.wav | for
Saraswati Chetti | SAR | sar_062_s03.wav | of
Saraswati Chetti | SAR | sar_062_s04.wav | . Bring it over.
Saraswati Chetti | SAR | sar_063_s01.wav | The price is settled at
Saraswati Chetti | SAR | sar_063_s02.wav | . Please bring over the
Saraswati Chetti | SAR | sar_063_s03.wav | of
Saraswati Chetti | SAR | sar_064_s01.wav | I accept these terms:
Saraswati Chetti | SAR | sar_064_s02.wav | of
Saraswati Chetti | SAR | sar_064_s03.wav | . Please measure it out.
Saraswati Chetti | SAR | sar_065_s01.wav | Our figures agree at
Saraswati Chetti | SAR | sar_065_s02.wav | . Please bring the
Saraswati Chetti | SAR | sar_065_s03.wav | of
Saraswati Chetti | SAR | sar_066_s01.wav | My figure does not persuade you. What price would?
Saraswati Chetti | SAR | sar_067_s01.wav | You refuse that offer. Give me a figure to consider.
Saraswati Chetti | SAR | sar_068_s01.wav | Then tell me where your calculation differs from mine.
Saraswati Chetti | SAR | sar_069_s01.wav | We have no agreement yet. State the price you want.
Saraswati Chetti | SAR | sar_070_s01.wav | I must resell this lot in my shop. At
Saraswati Chetti | SAR | sar_070_s02.wav | , the measure still makes sense.
Saraswati Chetti | SAR | sar_071_s01.wav | I judge the quality and what my customers will pay. That gives me
Saraswati Chetti | SAR | sar_072_s01.wav | My offer of
Saraswati Chetti | SAR | sar_072_s02.wav | accounts for the quantity and the work of selling it.
Saraswati Chetti | SAR | sar_073_s01.wav | There is a second sale after this one. That is why I offer
Saraswati Chetti | SAR | sar_099_s01.wav | That is beyond what I know. Shall we return to the
Saraswati Chetti | SAR | sar_100_s01.wav | An interesting turn, though it does not answer the price of this lot.
Saraswati Chetti | SAR | sar_101_s01.wav | I can speak to the measure before us. What did you want to know about it?
Saraswati Chetti | SAR | sar_102_s01.wav | We may discuss that another time. For now, I am buying
Saraswati Chetti | SAR | sar_103_s01.wav | I will consider a price, not an insult. Speak plainly and respectfully.
Saraswati Chetti | SAR | sar_104_s01.wav | There is no need for that tone. State your objection to the offer.
Saraswati Chetti | SAR | sar_105_s01.wav | A careful measure deserves a careful word. Try again without the insult.
Saraswati Chetti | SAR | sar_106_s01.wav | Konchem more detail, please:
Saraswati Chetti | SAR | sar_107_s01.wav | One point is uncertain.
Saraswati Chetti | SAR | sar_108_s01.wav | Let us be precise.
Saraswati Chetti | SAR | sar_109_s01.wav | I need to hear the exact terms.
Saraswati Chetti | SAR | sar_111_s01.wav | The goods are received and the measure is right. Thank you, merchant.
Saraswati Chetti | SAR | sar_112_s01.wav | The delivery matches our terms. A careful trade; you have my thanks.
Saraswati Chetti | SAR | sar_113_s01.wav | Well measured and delivered. I am pleased with this purchase.
Saraswati Chetti | SAR | sar_114_s01.wav | I have the
Saraswati Chetti | SAR | sar_114_s02.wav | now. You kept your word, and I appreciate it.
Saraswati Chetti | SAR | sar_115_s01.wav | We have not found terms. I will try another stall.
Saraswati Chetti | SAR | sar_116_s01.wav | The figures do not meet. I am leaving without a purchase.
Saraswati Chetti | SAR | sar_117_s01.wav | No bargain today. I will return to my shop.
Saraswati Chetti | SAR | sar_118_s01.wav | I cannot take this lot on those terms. Good day, merchant.
Saraswati Chetti | SAR | sar_122_s01.wav | Merchant? I am waiting for your figure.
Saraswati Chetti | SAR | sar_123_s01.wav | Take a moment, then tell me what you think of my offer.
Saraswati Chetti | SAR | sar_124_s01.wav | Have you considered the measure? I am ready for your answer.
Saraswati Chetti | SAR | sar_125_s01.wav | I need an answer now; my shop is waiting.
Saraswati Chetti | SAR | sar_126_s01.wav | We cannot bargain by silence. Do you have a price?
Saraswati Chetti | SAR | sar_127_s01.wav | Please give me your decision. I have other buying to do.
Saraswati Chetti | SAR | sar_128_s01.wav | I enjoy a careful bargain. My figure is still
Saraswati Chetti | SAR | sar_129_s01.wav | You make a thoughtful case, but I remain at
Saraswati Chetti | SAR | sar_130_s01.wav | We can discuss the figures further. My offer stays at
Saraswati Chetti | SAR | sar_131_s01.wav | I am listening, though the number remains
Saraswati Chetti | SAR | sar_132_s01.wav | You may yet persuade me; for now I hold at
Saraswati Chetti | SAR | sar_133_s01.wav | My offer remains
Saraswati Chetti | SAR | sar_133_s02.wav | . What is your counter?
Saraswati Chetti | SAR | sar_134_s01.wav | The measure still supports
Saraswati Chetti | SAR | sar_134_s02.wav | in my reckoning.
Saraswati Chetti | SAR | sar_135_s01.wav | I have checked it again: still
Saraswati Chetti | SAR | sar_136_s01.wav | I have considered it. The offer is still
Saraswati Chetti | SAR | sar_137_s01.wav | That argument does not alter my offer of
Saraswati Chetti | SAR | sar_138_s01.wav | I have checked the measure. My offer stays at
Saraswati Chetti | SAR | sar_139_s01.wav | Repeating a price does not explain it. I remain at
Saraswati Chetti | SAR | sar_140_s01.wav | I require a reason to move. Until then,
Saraswati Chetti | SAR | sar_141_s01.wav | No change in my offer:
Saraswati Chetti | SAR | sar_142_s01.wav | I have heard the demand. The figure remains
Saraswati Chetti | SAR | sar_143_s01.wav | Still
Saraswati Chetti | SAR | sar_143_s02.wav | . Your answer, please.
Saraswati Chetti | SAR | sar_144_s01.wav | My offer is unchanged at
Saraswati Chetti | SAR | sar_144_s02.wav | . Be direct.
Saraswati Chetti | SAR | sar_145_s01.wav | remains the figure. We need a decision.
Saraswati Chetti | SAR | sar_146_s01.wav | I have no revised offer. It is
Saraswati Chetti | SAR | sar_147_s01.wav | I remain at
Saraswati Chetti | SAR | sar_147_s02.wav | . What do you say?
Saraswati Chetti | SAR | sar_148_s01.wav | My patience is short, but my offer is unchanged:
Saraswati Chetti | SAR | sar_149_s01.wav | Still
Saraswati Chetti | SAR | sar_149_s02.wav | . I need a clear answer soon.
Saraswati Chetti | SAR | sar_150_s01.wav | My figure remains
Saraswati Chetti | SAR | sar_150_s02.wav | ; I cannot wait much longer.
Saraswati Chetti | SAR | sar_151_s01.wav | We know the quantity. The offer is still
Saraswati Chetti | SAR | sar_152_s01.wav | I am holding at
Saraswati Chetti | SAR | sar_152_s02.wav | . Please decide.
Saraswati Chetti | SAR | sar_153_s01.wav | is too high. My offer stays at
Saraswati Chetti | SAR | sar_153_s02.wav | ; answer soon.
Saraswati Chetti | SAR | sar_154_s01.wav | I cannot justify that figure, and I have little time left.
Saraswati Chetti | SAR | sar_155_s01.wav | Your price remains too high. I need a different answer now.
Saraswati Chetti | SAR | sar_156_s01.wav | I can increase to
Saraswati Chetti | SAR | sar_156_s02.wav | . Please answer promptly.
Saraswati Chetti | SAR | sar_157_s01.wav | My revised offer is
Saraswati Chetti | SAR | sar_157_s02.wav | ; time is short.
Saraswati Chetti | SAR | sar_158_s01.wav | I have moved to
Saraswati Chetti | SAR | sar_158_s02.wav | . Let us decide.
Saraswati Chetti | SAR | sar_162_s01.wav | My time is spent. I will seek the goods elsewhere.
Saraswati Chetti | SAR | sar_163_s01.wav | We have no bargain, and I must return to my shop.
Saraswati Chetti | SAR | sar_164_s01.wav | I cannot wait longer on these terms. I am leaving.
Saraswati Chetti | SAR | sar_num_1.wav | one
Saraswati Chetti | SAR | sar_num_2.wav | two
Saraswati Chetti | SAR | sar_num_3.wav | three
Saraswati Chetti | SAR | sar_num_4.wav | four
Saraswati Chetti | SAR | sar_num_5.wav | five
Saraswati Chetti | SAR | sar_num_6.wav | six
Saraswati Chetti | SAR | sar_num_7.wav | seven
Saraswati Chetti | SAR | sar_num_8.wav | eight
Saraswati Chetti | SAR | sar_num_9.wav | nine
Saraswati Chetti | SAR | sar_num_10.wav | ten
Saraswati Chetti | SAR | sar_num_11.wav | eleven
Saraswati Chetti | SAR | sar_num_12.wav | twelve
Saraswati Chetti | SAR | sar_num_13.wav | thirteen
Saraswati Chetti | SAR | sar_num_14.wav | fourteen
Saraswati Chetti | SAR | sar_num_15.wav | fifteen
Saraswati Chetti | SAR | sar_num_16.wav | sixteen
Saraswati Chetti | SAR | sar_num_17.wav | seventeen
Saraswati Chetti | SAR | sar_num_18.wav | eighteen
Saraswati Chetti | SAR | sar_num_19.wav | nineteen
Saraswati Chetti | SAR | sar_num_20.wav | twenty
Saraswati Chetti | SAR | sar_num_30.wav | thirty
Saraswati Chetti | SAR | sar_num_40.wav | forty
Saraswati Chetti | SAR | sar_num_50.wav | fifty
Saraswati Chetti | SAR | sar_num_60.wav | sixty
Saraswati Chetti | SAR | sar_num_70.wav | seventy
Saraswati Chetti | SAR | sar_num_80.wav | eighty
Saraswati Chetti | SAR | sar_num_90.wav | ninety
Saraswati Chetti | SAR | sar_num_100.wav | one hundred
Saraswati Chetti | SAR | sar_num_200.wav | two hundred
Saraswati Chetti | SAR | sar_num_300.wav | three hundred
Saraswati Chetti | SAR | sar_num_400.wav | four hundred
Saraswati Chetti | SAR | sar_num_500.wav | five hundred
Saraswati Chetti | SAR | sar_num_600.wav | six hundred
Saraswati Chetti | SAR | sar_num_700.wav | seven hundred
Saraswati Chetti | SAR | sar_num_800.wav | eight hundred
Saraswati Chetti | SAR | sar_num_900.wav | nine hundred
Saraswati Chetti | SAR | sar_spice_pepper.wav | pepper
Saraswati Chetti | SAR | sar_spice_cinnamon.wav | cinnamon
Saraswati Chetti | SAR | sar_spice_cardamom.wav | cardamom
Saraswati Chetti | SAR | sar_currency_varahas.wav | varahas
Saraswati Chetti | SAR | sar_unit_seer.wav | Seer
Saraswati Chetti | SAR | sar_unit_seers.wav | Seers
Saraswati Chetti | SAR | sar_unit_veesai.wav | Veesai
Saraswati Chetti | SAR | sar_unit_palam.wav | Palam
Saraswati Chetti | SAR | sar_unit_palams.wav | Palams
Saraswati Chetti | SAR | sar_unit_manangu.wav | Manangu
Saraswati Chetti | SAR | sar_unit_manangus.wav | Manangus
Saraswati Chetti | SAR | sar_unit_bahar.wav | Bahar
Saraswati Chetti | SAR | sar_unit_bahars.wav | Bahars
```

### Abdul Rahman

```text
Abdul Rahman | ABD | abd_001_s01.wav | Marhaban, merchant. I am Abdul Rahman. I wish to buy
Abdul Rahman | ABD | abd_001_s02.wav | of
Abdul Rahman | ABD | abd_002_s01.wav | Good day. I am Abdul Rahman, looking for
Abdul Rahman | ABD | abd_002_s02.wav | of
Abdul Rahman | ABD | abd_003_s01.wav | I have come with the caravan to buy
Abdul Rahman | ABD | abd_003_s02.wav | of
Abdul Rahman | ABD | abd_003_s03.wav | . What is your price?
Abdul Rahman | ABD | abd_004_s01.wav | Abdul Rahman. I hope your
Abdul Rahman | ABD | abd_004_s02.wav | is as inviting as your stall.
Abdul Rahman | ABD | abd_005_s01.wav | I know your stall now. What do you have for me today?
Abdul Rahman | ABD | abd_006_s01.wav | We meet again, merchant. Let us give this lot its own fair hearing.
Abdul Rahman | ABD | abd_007_s01.wav | A familiar face makes the bargaining easier. What is your price for
Abdul Rahman | ABD | abd_008_s01.wav | Back to your stall for
Abdul Rahman | ABD | abd_008_s02.wav | . Shall we begin afresh?
Abdul Rahman | ABD | abd_009_s01.wav | I need
Abdul Rahman | ABD | abd_009_s02.wav | of
Abdul Rahman | ABD | abd_009_s03.wav | for caravan trade.
Abdul Rahman | ABD | abd_010_s01.wav | , please, and
Abdul Rahman | ABD | abd_010_s02.wav | if the measure is sound.
Abdul Rahman | ABD | abd_011_s01.wav | The lot I came to discuss is
Abdul Rahman | ABD | abd_011_s02.wav | of
Abdul Rahman | ABD | abd_012_s01.wav | I am buying
Abdul Rahman | ABD | abd_012_s02.wav | . Let us settle terms for
Abdul Rahman | ABD | abd_013_s01.wav | My offer for this lot is
Abdul Rahman | ABD | abd_014_s01.wav | I can offer
Abdul Rahman | ABD | abd_014_s02.wav | for the
Abdul Rahman | ABD | abd_015_s01.wav | For the goods before us, I have put forward
Abdul Rahman | ABD | abd_016_s01.wav | You have my current figure:
Abdul Rahman | ABD | abd_017_s01.wav | I asked for
Abdul Rahman | ABD | abd_017_s02.wav | of
Abdul Rahman | ABD | abd_018_s01.wav | The measure is
Abdul Rahman | ABD | abd_018_s02.wav | , merchant.
Abdul Rahman | ABD | abd_019_s01.wav | For this purchase, I need
Abdul Rahman | ABD | abd_019_s02.wav | of
Abdul Rahman | ABD | abd_020_s01.wav | of
Abdul Rahman | ABD | abd_020_s02.wav | ; that is the lot we are pricing.
Abdul Rahman | ABD | abd_021_s01.wav | I have named the lot. What price do you ask?
Abdul Rahman | ABD | abd_022_s01.wav | What is your price for
Abdul Rahman | ABD | abd_022_s02.wav | of
Abdul Rahman | ABD | abd_023_s01.wav | Let me hear your asking figure, merchant.
Abdul Rahman | ABD | abd_024_s01.wav | You know what I wish to buy. Tell me what it will cost.
Abdul Rahman | ABD | abd_025_s01.wav | Then we are speaking of
Abdul Rahman | ABD | abd_025_s02.wav | of
Abdul Rahman | ABD | abd_025_s03.wav | now. Let us price that lot.
Abdul Rahman | ABD | abd_026_s01.wav | I understand the new measure:
Abdul Rahman | ABD | abd_026_s02.wav | . What is your price for it?
Abdul Rahman | ABD | abd_027_s01.wav | Very well,
Abdul Rahman | ABD | abd_027_s02.wav | of
Abdul Rahman | ABD | abd_027_s03.wav | . We can bargain on that basis.
Abdul Rahman | ABD | abd_028_s01.wav | The quantity has changed to
Abdul Rahman | ABD | abd_028_s02.wav | ; let us keep our figures clear.
Abdul Rahman | ABD | abd_029_s01.wav | is beyond what I can pay for this lot. My offer remains
Abdul Rahman | ABD | abd_030_s01.wav | I respect your goods, but that asking price leaves me no room to trade onward.
Abdul Rahman | ABD | abd_031_s01.wav | We may still talk, though
Abdul Rahman | ABD | abd_031_s02.wav | is more than I can justify.
Abdul Rahman | ABD | abd_032_s01.wav | I cannot follow that figure. I remain at
Abdul Rahman | ABD | abd_033_s01.wav | Your asking figure is above mine. I remain at
Abdul Rahman | ABD | abd_034_s01.wav | There may be a bargain here, but
Abdul Rahman | ABD | abd_034_s02.wav | does not work for me.
Abdul Rahman | ABD | abd_035_s01.wav | I hear your price. My offer is still
Abdul Rahman | ABD | abd_035_s02.wav | ; can you make a better case?
Abdul Rahman | ABD | abd_036_s01.wav | I would like us both to trade well. At present I can offer
Abdul Rahman | ABD | abd_037_s01.wav | That price is unexpectedly low. Are you certain of that figure?
Abdul Rahman | ABD | abd_038_s01.wav | You ask less than I expected; such a cheap offer makes a merchant cautious.
Abdul Rahman | ABD | abd_039_s01.wav | A low figure catches my ear, but I must wonder why you part with it so cheaply.
Abdul Rahman | ABD | abd_040_s01.wav | That is below my expectation. Did you truly intend to name so small a sum?
Abdul Rahman | ABD | abd_041_s01.wav | Let me offer
Abdul Rahman | ABD | abd_041_s02.wav | for the
Abdul Rahman | ABD | abd_041_s03.wav | of
Abdul Rahman | ABD | abd_042_s01.wav | I can begin at
Abdul Rahman | ABD | abd_042_s02.wav | . Tell me how that sits with you.
Abdul Rahman | ABD | abd_043_s01.wav | For this lot, I put
Abdul Rahman | ABD | abd_043_s02.wav | before you.
Abdul Rahman | ABD | abd_044_s01.wav | Perhaps
Abdul Rahman | ABD | abd_044_s02.wav | gives us a good place to begin.
Abdul Rahman | ABD | abd_045_s01.wav | I have heard your reasoning. I can raise my offer to
Abdul Rahman | ABD | abd_046_s01.wav | Let me move to
Abdul Rahman | ABD | abd_046_s02.wav | ; I hope that brings us nearer.
Abdul Rahman | ABD | abd_047_s01.wav | You have persuaded me to offer
Abdul Rahman | ABD | abd_047_s02.wav | now.
Abdul Rahman | ABD | abd_048_s01.wav | For the sake of a fair trade, my revised figure is
Abdul Rahman | ABD | abd_049_s01.wav | After hearing you out, I can move to
Abdul Rahman | ABD | abd_049_s02.wav | . What do you say?
Abdul Rahman | ABD | abd_050_s01.wav | My offer is now
Abdul Rahman | ABD | abd_050_s02.wav | ; we have both given this thought.
Abdul Rahman | ABD | abd_051_s01.wav | I can raise it to
Abdul Rahman | ABD | abd_051_s02.wav | . Is there room for agreement?
Abdul Rahman | ABD | abd_052_s01.wav | Let us see whether
Abdul Rahman | ABD | abd_052_s02.wav | brings our figures together.
Abdul Rahman | ABD | abd_053_s01.wav | I value a continuing acquaintance, but my offer remains
Abdul Rahman | ABD | abd_054_s01.wav | I have listened. I am still at
Abdul Rahman | ABD | abd_055_s01.wav | My figure is unchanged:
Abdul Rahman | ABD | abd_055_s02.wav | . Tell me what I am missing.
Abdul Rahman | ABD | abd_056_s01.wav | Goodwill does not alter the cost of this lot. I remain at
Abdul Rahman | ABD | abd_057_s01.wav | I would rather agree than argue, but my offer stays at
Abdul Rahman | ABD | abd_058_s01.wav | We agree on
Abdul Rahman | ABD | abd_058_s02.wav | for
Abdul Rahman | ABD | abd_058_s03.wav | of
Abdul Rahman | ABD | abd_058_s04.wav | . Please bring the goods.
Abdul Rahman | ABD | abd_059_s01.wav | That price works for me:
Abdul Rahman | ABD | abd_059_s02.wav | for
Abdul Rahman | ABD | abd_059_s03.wav | of
Abdul Rahman | ABD | abd_059_s04.wav | . Please prepare the goods.
Abdul Rahman | ABD | abd_060_s01.wav | I accept the terms,
Abdul Rahman | ABD | abd_060_s02.wav | and
Abdul Rahman | ABD | abd_060_s03.wav | of
Abdul Rahman | ABD | abd_060_s04.wav | . The goods are still to come.
Abdul Rahman | ABD | abd_061_s01.wav | We have a bargain at
Abdul Rahman | ABD | abd_061_s02.wav | . Bring the
Abdul Rahman | ABD | abd_061_s03.wav | of
Abdul Rahman | ABD | abd_061_s04.wav | , please.
Abdul Rahman | ABD | abd_062_s01.wav | Then we are agreed at
Abdul Rahman | ABD | abd_062_s02.wav | for
Abdul Rahman | ABD | abd_062_s03.wav | of
Abdul Rahman | ABD | abd_062_s04.wav | . Bring the lot over.
Abdul Rahman | ABD | abd_063_s01.wav | Our price is settled:
Abdul Rahman | ABD | abd_063_s02.wav | for
Abdul Rahman | ABD | abd_063_s03.wav | of
Abdul Rahman | ABD | abd_063_s04.wav | . I await the goods.
Abdul Rahman | ABD | abd_064_s01.wav | I accept those terms.
Abdul Rahman | ABD | abd_064_s02.wav | of
Abdul Rahman | ABD | abd_064_s03.wav | ; please weigh it out.
Abdul Rahman | ABD | abd_065_s01.wav | A fair understanding:
Abdul Rahman | ABD | abd_065_s02.wav | of
Abdul Rahman | ABD | abd_065_s03.wav | at
Abdul Rahman | ABD | abd_065_s04.wav | . Please bring it here.
Abdul Rahman | ABD | abd_066_s01.wav | My offer does not suit you. What figure would?
Abdul Rahman | ABD | abd_067_s01.wav | You have refused that price. Tell me what you would ask instead.
Abdul Rahman | ABD | abd_068_s01.wav | Then we differ on the number. Give me one I can consider.
Abdul Rahman | ABD | abd_069_s01.wav | I hear your refusal. Shall we consider another price?
Abdul Rahman | ABD | abd_070_s01.wav | I must trade the goods onward. At
Abdul Rahman | ABD | abd_070_s02.wav | , there is room for us both.
Abdul Rahman | ABD | abd_071_s01.wav | I have considered the measure and the cost of carrying it. That gives me
Abdul Rahman | ABD | abd_072_s01.wav | A fair purchase must survive its next sale. That is why I offer
Abdul Rahman | ABD | abd_073_s01.wav | I want to return to a seller I can trust.
Abdul Rahman | ABD | abd_073_s02.wav | is my present offer.
Abdul Rahman | ABD | abd_099_s01.wav | That lies beyond what I know. Shall we return to the
Abdul Rahman | ABD | abd_100_s01.wav | You have surprised me with that question. The lot before us is more familiar ground.
Abdul Rahman | ABD | abd_101_s01.wav | I cannot give you a useful answer to that. I can answer for my offer.
Abdul Rahman | ABD | abd_102_s01.wav | Another conversation, perhaps. Today I came to discuss
Abdul Rahman | ABD | abd_103_s01.wav | I have shown you courtesy. I ask the same in return.
Abdul Rahman | ABD | abd_104_s01.wav | I will discuss the price, merchant. I will not trade insults.
Abdul Rahman | ABD | abd_105_s01.wav | Speak to me as a fellow trader, and we may still do business.
Abdul Rahman | ABD | abd_106_s01.wav | Help me understand one point:
Abdul Rahman | ABD | abd_107_s01.wav | I would rather ask than assume.
Abdul Rahman | ABD | abd_108_s01.wav | Before I answer, let us make this clear.
Abdul Rahman | ABD | abd_109_s01.wav | I missed the precise terms.
Abdul Rahman | ABD | abd_111_s01.wav | Shukran, merchant. The goods are here, and you have kept your word.
Abdul Rahman | ABD | abd_112_s01.wav | The
Abdul Rahman | ABD | abd_112_s02.wav | is delivered as agreed. Thank you for a fair exchange.
Abdul Rahman | ABD | abd_113_s01.wav | Goods received, and my thanks. I would be glad to meet you at this stall again.
Abdul Rahman | ABD | abd_114_s01.wav | You dealt openly and delivered the lot. I will remember that.
Abdul Rahman | ABD | abd_115_s01.wav | We have no bargain today. I will take my business to another stall.
Abdul Rahman | ABD | abd_116_s01.wav | Our prices do not meet. I will leave the lot with you.
Abdul Rahman | ABD | abd_117_s01.wav | I must go without a purchase. I wish you better trade ahead.
Abdul Rahman | ABD | abd_118_s01.wav | No agreement this time. I will return to the caravan.
Abdul Rahman | ABD | abd_122_s01.wav | Merchant? I am still here, if you have a figure for me.
Abdul Rahman | ABD | abd_123_s01.wav | Take a moment. I would like to hear your answer.
Abdul Rahman | ABD | abd_124_s01.wav | Shall we continue with the
Abdul Rahman | ABD | abd_125_s01.wav | I have waited a while. Are we to continue this bargain?
Abdul Rahman | ABD | abd_126_s01.wav | Please tell me where you stand; I have other business today.
Abdul Rahman | ABD | abd_127_s01.wav | Silence will not bring our figures together. What is your answer?
Abdul Rahman | ABD | abd_128_s01.wav | I enjoy our talk, though my offer is still
Abdul Rahman | ABD | abd_129_s01.wav | You make good company. My figure remains
Abdul Rahman | ABD | abd_130_s01.wav | I hope we trade together, but the offer stays at
Abdul Rahman | ABD | abd_131_s01.wav | We can disagree amiably. I remain at
Abdul Rahman | ABD | abd_132_s01.wav | I am listening, friend; I have not moved from
Abdul Rahman | ABD | abd_133_s01.wav | I remain at
Abdul Rahman | ABD | abd_133_s02.wav | . What price do you suggest?
Abdul Rahman | ABD | abd_134_s01.wav | My offer has not changed:
Abdul Rahman | ABD | abd_135_s01.wav | I have considered it and still offer
Abdul Rahman | ABD | abd_136_s01.wav | The figure before you remains
Abdul Rahman | ABD | abd_137_s01.wav | I would hear another reason. For now,
Abdul Rahman | ABD | abd_137_s02.wav | stands.
Abdul Rahman | ABD | abd_138_s01.wav | I have listened carefully. My offer remains
Abdul Rahman | ABD | abd_139_s01.wav | Respectfully, I see no reason to revise
Abdul Rahman | ABD | abd_140_s01.wav | I must keep to
Abdul Rahman | ABD | abd_140_s02.wav | . Present another argument if you have one.
Abdul Rahman | ABD | abd_141_s01.wav | My position is unchanged at
Abdul Rahman | ABD | abd_142_s01.wav | You have my answer:
Abdul Rahman | ABD | abd_143_s01.wav | My figure is still
Abdul Rahman | ABD | abd_143_s02.wav | ; may we decide?
Abdul Rahman | ABD | abd_144_s01.wav | The offer remains
Abdul Rahman | ABD | abd_144_s02.wav | . Shall we decide?
Abdul Rahman | ABD | abd_145_s01.wav | I have not raised it.
Abdul Rahman | ABD | abd_145_s02.wav | stands.
Abdul Rahman | ABD | abd_146_s01.wav | , unchanged. Please be direct.
Abdul Rahman | ABD | abd_147_s01.wav | My figure is
Abdul Rahman | ABD | abd_147_s02.wav | . I need your reply.
Abdul Rahman | ABD | abd_148_s01.wav | My time is short; my offer is still
Abdul Rahman | ABD | abd_149_s01.wav | I remain at
Abdul Rahman | ABD | abd_149_s02.wav | . Please decide soon.
Abdul Rahman | ABD | abd_150_s01.wav | The figure has not changed:
Abdul Rahman | ABD | abd_150_s02.wav | . I cannot wait much longer.
Abdul Rahman | ABD | abd_151_s01.wav | We can continue if you answer promptly. I offer
Abdul Rahman | ABD | abd_152_s01.wav | I am holding at
Abdul Rahman | ABD | abd_152_s02.wav | . Do you wish to counter?
Abdul Rahman | ABD | abd_153_s01.wav | I cannot pay
Abdul Rahman | ABD | abd_153_s02.wav | . My offer remains
Abdul Rahman | ABD | abd_153_s03.wav | , and time is short.
Abdul Rahman | ABD | abd_154_s01.wav | That demand is too high. I need a different figure soon.
Abdul Rahman | ABD | abd_155_s01.wav | I have little time left;
Abdul Rahman | ABD | abd_155_s02.wav | is still beyond me.
Abdul Rahman | ABD | abd_156_s01.wav | I can raise my offer to
Abdul Rahman | ABD | abd_156_s02.wav | . Please answer soon.
Abdul Rahman | ABD | abd_157_s01.wav | My revised figure is
Abdul Rahman | ABD | abd_157_s02.wav | . I cannot linger.
Abdul Rahman | ABD | abd_158_s01.wav | I have moved to
Abdul Rahman | ABD | abd_158_s02.wav | . Shall we decide?
Abdul Rahman | ABD | abd_162_s01.wav | Our figures do not meet, and I must leave now.
Abdul Rahman | ABD | abd_163_s01.wav | I have no more time for this lot. I will try another stall.
Abdul Rahman | ABD | abd_164_s01.wav | No bargain today. I must return to my companions.
Abdul Rahman | ABD | abd_num_1.wav | one
Abdul Rahman | ABD | abd_num_2.wav | two
Abdul Rahman | ABD | abd_num_3.wav | three
Abdul Rahman | ABD | abd_num_4.wav | four
Abdul Rahman | ABD | abd_num_5.wav | five
Abdul Rahman | ABD | abd_num_6.wav | six
Abdul Rahman | ABD | abd_num_7.wav | seven
Abdul Rahman | ABD | abd_num_8.wav | eight
Abdul Rahman | ABD | abd_num_9.wav | nine
Abdul Rahman | ABD | abd_num_10.wav | ten
Abdul Rahman | ABD | abd_num_11.wav | eleven
Abdul Rahman | ABD | abd_num_12.wav | twelve
Abdul Rahman | ABD | abd_num_13.wav | thirteen
Abdul Rahman | ABD | abd_num_14.wav | fourteen
Abdul Rahman | ABD | abd_num_15.wav | fifteen
Abdul Rahman | ABD | abd_num_16.wav | sixteen
Abdul Rahman | ABD | abd_num_17.wav | seventeen
Abdul Rahman | ABD | abd_num_18.wav | eighteen
Abdul Rahman | ABD | abd_num_19.wav | nineteen
Abdul Rahman | ABD | abd_num_20.wav | twenty
Abdul Rahman | ABD | abd_num_30.wav | thirty
Abdul Rahman | ABD | abd_num_40.wav | forty
Abdul Rahman | ABD | abd_num_50.wav | fifty
Abdul Rahman | ABD | abd_num_60.wav | sixty
Abdul Rahman | ABD | abd_num_70.wav | seventy
Abdul Rahman | ABD | abd_num_80.wav | eighty
Abdul Rahman | ABD | abd_num_90.wav | ninety
Abdul Rahman | ABD | abd_num_100.wav | one hundred
Abdul Rahman | ABD | abd_num_200.wav | two hundred
Abdul Rahman | ABD | abd_num_300.wav | three hundred
Abdul Rahman | ABD | abd_num_400.wav | four hundred
Abdul Rahman | ABD | abd_num_500.wav | five hundred
Abdul Rahman | ABD | abd_num_600.wav | six hundred
Abdul Rahman | ABD | abd_num_700.wav | seven hundred
Abdul Rahman | ABD | abd_num_800.wav | eight hundred
Abdul Rahman | ABD | abd_num_900.wav | nine hundred
Abdul Rahman | ABD | abd_spice_pepper.wav | pepper
Abdul Rahman | ABD | abd_spice_cinnamon.wav | cinnamon
Abdul Rahman | ABD | abd_spice_cardamom.wav | cardamom
Abdul Rahman | ABD | abd_currency_varahas.wav | varahas
Abdul Rahman | ABD | abd_unit_seer.wav | Seer
Abdul Rahman | ABD | abd_unit_seers.wav | Seers
Abdul Rahman | ABD | abd_unit_veesai.wav | Veesai
Abdul Rahman | ABD | abd_unit_palam.wav | Palam
Abdul Rahman | ABD | abd_unit_palams.wav | Palams
Abdul Rahman | ABD | abd_unit_manangu.wav | Manangu
Abdul Rahman | ABD | abd_unit_manangus.wav | Manangus
Abdul Rahman | ABD | abd_unit_bahar.wav | Bahar
Abdul Rahman | ABD | abd_unit_bahars.wav | Bahars
```

### Francisco de Almeida

```text
Francisco de Almeida | FRA | fra_001_s01.wav | Olá, merchant. I am Francisco de Almeida. I would like to buy
Francisco de Almeida | FRA | fra_001_s02.wav | of
Francisco de Almeida | FRA | fra_002_s01.wav | Francisco de Almeida. Your
Francisco de Almeida | FRA | fra_002_s02.wav | caught my eye; I am looking for
Francisco de Almeida | FRA | fra_003_s01.wav | A promising stall! I have come to ask about
Francisco de Almeida | FRA | fra_003_s02.wav | of
Francisco de Almeida | FRA | fra_004_s01.wav | Good day. I am Francisco, and I would like to buy
Francisco de Almeida | FRA | fra_004_s02.wav | of
Francisco de Almeida | FRA | fra_005_s01.wav | Olá again. What sort of
Francisco de Almeida | FRA | fra_005_s02.wav | have you for me today?
Francisco de Almeida | FRA | fra_006_s01.wav | Your stall has drawn me back. Let us discuss this lot afresh.
Francisco de Almeida | FRA | fra_007_s01.wav | We have met before, merchant. Shall we see whether today's figures agree?
Francisco de Almeida | FRA | fra_008_s01.wav | Back to your stall, and ready for
Francisco de Almeida | FRA | fra_008_s02.wav | . What is your price today?
Francisco de Almeida | FRA | fra_009_s01.wav | I am here for
Francisco de Almeida | FRA | fra_009_s02.wav | of
Francisco de Almeida | FRA | fra_009_s03.wav | . Tell me your price.
Francisco de Almeida | FRA | fra_010_s01.wav | The
Francisco de Almeida | FRA | fra_010_s02.wav | interests me, particularly a lot of
Francisco de Almeida | FRA | fra_011_s01.wav | I want to buy
Francisco de Almeida | FRA | fra_011_s02.wav | of
Francisco de Almeida | FRA | fra_011_s03.wav | , if your terms are fair.
Francisco de Almeida | FRA | fra_012_s01.wav | of
Francisco de Almeida | FRA | fra_012_s02.wav | ; that is the purchase I have in mind.
Francisco de Almeida | FRA | fra_013_s01.wav | My present offer is
Francisco de Almeida | FRA | fra_013_s02.wav | for this lot.
Francisco de Almeida | FRA | fra_014_s01.wav | I have put
Francisco de Almeida | FRA | fra_014_s02.wav | on the table. What do you think?
Francisco de Almeida | FRA | fra_015_s01.wav | For the quantity before us, I offer
Francisco de Almeida | FRA | fra_016_s01.wav | You want my figure? It stands at
Francisco de Almeida | FRA | fra_017_s01.wav | The order I named was
Francisco de Almeida | FRA | fra_017_s02.wav | of
Francisco de Almeida | FRA | fra_018_s01.wav | The measure I want is
Francisco de Almeida | FRA | fra_018_s02.wav | , merchant.
Francisco de Almeida | FRA | fra_019_s01.wav | For this purchase, set aside
Francisco de Almeida | FRA | fra_019_s02.wav | of
Francisco de Almeida | FRA | fra_020_s01.wav | of
Francisco de Almeida | FRA | fra_020_s02.wav | ; let us keep that measure in view.
Francisco de Almeida | FRA | fra_021_s01.wav | You know the lot I want. What price do you ask?
Francisco de Almeida | FRA | fra_022_s01.wav | What would
Francisco de Almeida | FRA | fra_022_s02.wav | of
Francisco de Almeida | FRA | fra_022_s03.wav | cost me here?
Francisco de Almeida | FRA | fra_023_s01.wav | Por favor, give me your asking price for the
Francisco de Almeida | FRA | fra_024_s01.wav | The goods have my attention. Now tell me your price.
Francisco de Almeida | FRA | fra_025_s01.wav | So the lot is now
Francisco de Almeida | FRA | fra_025_s02.wav | of
Francisco de Almeida | FRA | fra_025_s03.wav | . We must price that measure.
Francisco de Almeida | FRA | fra_026_s01.wav | A different quantity, then:
Francisco de Almeida | FRA | fra_026_s02.wav | . Let us speak of its price.
Francisco de Almeida | FRA | fra_027_s01.wav | I hear the change to
Francisco de Almeida | FRA | fra_027_s02.wav | of
Francisco de Almeida | FRA | fra_027_s03.wav | . What do you ask for it?
Francisco de Almeida | FRA | fra_028_s01.wav | Very well,
Francisco de Almeida | FRA | fra_028_s02.wav | . We can bargain on the new measure.
Francisco de Almeida | FRA | fra_029_s01.wav | Ora!
Francisco de Almeida | FRA | fra_029_s02.wav | is beyond what this lot can bear. I remain at
Francisco de Almeida | FRA | fra_030_s01.wav | Não, that asking price is too high for me. My offer remains
Francisco de Almeida | FRA | fra_031_s01.wav | A bold figure, but I cannot buy at
Francisco de Almeida | FRA | fra_032_s01.wav | You make a splendid case for your goods. The price still exceeds my reach.
Francisco de Almeida | FRA | fra_033_s01.wav | We may yet find a price, but I remain at
Francisco de Almeida | FRA | fra_034_s01.wav | is above my reach. Could we talk nearer
Francisco de Almeida | FRA | fra_035_s01.wav | I like the lot more than the figure. My offer is
Francisco de Almeida | FRA | fra_036_s01.wav | That price deserves discussion. I can offer
Francisco de Almeida | FRA | fra_036_s02.wav | at present.
Francisco de Almeida | FRA | fra_037_s01.wav | Ora, that is unexpectedly low. Do you truly mean to ask so little?
Francisco de Almeida | FRA | fra_038_s01.wav | Such a low figure makes me wonder if you have calculated your profit rightly.
Francisco de Almeida | FRA | fra_039_s01.wav | I expected a higher price. Tell me honestly: are you certain of that sum?
Francisco de Almeida | FRA | fra_040_s01.wav | That price surprises me. A seasoned merchant does not usually sell so cheaply.
Francisco de Almeida | FRA | fra_041_s01.wav | I can come to
Francisco de Almeida | FRA | fra_041_s02.wav | for this lot. How does that sound?
Francisco de Almeida | FRA | fra_042_s01.wav | Let me put
Francisco de Almeida | FRA | fra_042_s02.wav | before you for the
Francisco de Almeida | FRA | fra_043_s01.wav | Pois bem, I offer
Francisco de Almeida | FRA | fra_043_s02.wav | . We have room to discuss it.
Francisco de Almeida | FRA | fra_044_s01.wav | I will bid
Francisco de Almeida | FRA | fra_044_s02.wav | for
Francisco de Almeida | FRA | fra_044_s03.wav | of
Francisco de Almeida | FRA | fra_045_s01.wav | You have won a little ground. I can raise my offer to
Francisco de Almeida | FRA | fra_046_s01.wav | I will move to
Francisco de Almeida | FRA | fra_046_s02.wav | . Is that nearer your figure?
Francisco de Almeida | FRA | fra_047_s01.wav | Your argument has some weight. My new offer is
Francisco de Almeida | FRA | fra_048_s01.wav | I can put
Francisco de Almeida | FRA | fra_048_s02.wav | on the table now.
Francisco de Almeida | FRA | fra_049_s01.wav | I can reach
Francisco de Almeida | FRA | fra_049_s02.wav | now. What do you say?
Francisco de Almeida | FRA | fra_050_s01.wav | A further step from me:
Francisco de Almeida | FRA | fra_050_s02.wav | . Consider it.
Francisco de Almeida | FRA | fra_051_s01.wav | We have talked this through; my offer rises to
Francisco de Almeida | FRA | fra_052_s01.wav | I will put
Francisco de Almeida | FRA | fra_052_s02.wav | before you. Are we nearer?
Francisco de Almeida | FRA | fra_053_s01.wav | My offer remains
Francisco de Almeida | FRA | fra_053_s02.wav | ; enthusiasm cannot change the sum.
Francisco de Almeida | FRA | fra_054_s01.wav | I admire your case, but my figure stays at
Francisco de Almeida | FRA | fra_055_s01.wav | No fresh coin from me yet. I am still at
Francisco de Almeida | FRA | fra_056_s01.wav | The goods tempt me; the price has not moved me.
Francisco de Almeida | FRA | fra_056_s02.wav | stands.
Francisco de Almeida | FRA | fra_057_s01.wav | I have not raised my offer. It remains
Francisco de Almeida | FRA | fra_058_s01.wav | Muito bem. We agree on
Francisco de Almeida | FRA | fra_058_s02.wav | for
Francisco de Almeida | FRA | fra_058_s03.wav | of
Francisco de Almeida | FRA | fra_058_s04.wav | . Please bring the lot.
Francisco de Almeida | FRA | fra_059_s01.wav | That price works:
Francisco de Almeida | FRA | fra_059_s02.wav | for
Francisco de Almeida | FRA | fra_059_s03.wav | of
Francisco de Almeida | FRA | fra_059_s04.wav | . I await the goods.
Francisco de Almeida | FRA | fra_060_s01.wav | We have terms at
Francisco de Almeida | FRA | fra_060_s02.wav | . Bring the
Francisco de Almeida | FRA | fra_060_s03.wav | of
Francisco de Almeida | FRA | fra_060_s04.wav | , please.
Francisco de Almeida | FRA | fra_061_s01.wav | Agreed on the price and measure:
Francisco de Almeida | FRA | fra_061_s02.wav | for
Francisco de Almeida | FRA | fra_061_s03.wav | of
Francisco de Almeida | FRA | fra_061_s04.wav | . Please prepare the delivery.
Francisco de Almeida | FRA | fra_062_s01.wav | Excellent.
Francisco de Almeida | FRA | fra_062_s02.wav | for
Francisco de Almeida | FRA | fra_062_s03.wav | of
Francisco de Almeida | FRA | fra_062_s04.wav | ; now for the goods.
Francisco de Almeida | FRA | fra_063_s01.wav | We are agreed at
Francisco de Almeida | FRA | fra_063_s02.wav | for
Francisco de Almeida | FRA | fra_063_s03.wav | of
Francisco de Almeida | FRA | fra_063_s04.wav | . Please bring them.
Francisco de Almeida | FRA | fra_064_s01.wav | Muito bem. Our price is
Francisco de Almeida | FRA | fra_064_s02.wav | for
Francisco de Almeida | FRA | fra_064_s03.wav | of
Francisco de Almeida | FRA | fra_064_s04.wav | . I await the lot.
Francisco de Almeida | FRA | fra_065_s01.wav | A bargain, then:
Francisco de Almeida | FRA | fra_065_s02.wav | of
Francisco de Almeida | FRA | fra_065_s03.wav | at
Francisco de Almeida | FRA | fra_065_s04.wav | . Please weigh it out.
Francisco de Almeida | FRA | fra_066_s01.wav | You decline that offer. What figure would tempt you?
Francisco de Almeida | FRA | fra_067_s01.wav | No agreement at that price, then. Let me hear yours.
Francisco de Almeida | FRA | fra_068_s01.wav | So my number does not suit you. We can still discuss another.
Francisco de Almeida | FRA | fra_069_s01.wav | A refusal, understood. What price do you propose instead?
Francisco de Almeida | FRA | fra_070_s01.wav | I must judge the quality and what I can earn when I trade onward. That gives me
Francisco de Almeida | FRA | fra_071_s01.wav | There is promise in the
Francisco de Almeida | FRA | fra_071_s02.wav | , but I still have costs after buying. I offer
Francisco de Almeida | FRA | fra_072_s01.wav | My figure of
Francisco de Almeida | FRA | fra_072_s02.wav | leaves room for risk as well as profit.
Francisco de Almeida | FRA | fra_073_s01.wav | I am keen to buy, not to pay blindly. The measure leads me to
Francisco de Almeida | FRA | fra_099_s01.wav | That is outside my knowledge, I fear. Shall we return to the
Francisco de Almeida | FRA | fra_100_s01.wav | You have taken me by surprise. The lot before us is easier to judge.
Francisco de Almeida | FRA | fra_101_s01.wav | A curious thought, but I cannot answer it honestly.
Francisco de Almeida | FRA | fra_102_s01.wav | That is a tale for someone else. I can speak for my offer.
Francisco de Almeida | FRA | fra_103_s01.wav | I came ready to bargain, not to trade insults.
Francisco de Almeida | FRA | fra_104_s01.wav | A sharp price I can discuss. Sharp words are another matter.
Francisco de Almeida | FRA | fra_105_s01.wav | Speak courteously, merchant, and we may yet do business.
Francisco de Almeida | FRA | fra_106_s01.wav | Por favor, help me with this point:
Francisco de Almeida | FRA | fra_107_s01.wav | I would rather ask twice than mistake your terms.
Francisco de Almeida | FRA | fra_108_s01.wav | The market swallowed part of your words.
Francisco de Almeida | FRA | fra_109_s01.wav | Before I answer, make this clear for me:
Francisco de Almeida | FRA | fra_111_s01.wav | Obrigado, merchant. The
Francisco de Almeida | FRA | fra_111_s02.wav | is here as promised; a fine exchange.
Francisco de Almeida | FRA | fra_112_s01.wav | Goods received and fairly measured. You have my thanks.
Francisco de Almeida | FRA | fra_113_s01.wav | The lot has arrived. A lively bargain, and one worth making.
Francisco de Almeida | FRA | fra_114_s01.wav | Now the trade is complete. Thank you for keeping the terms we made.
Francisco de Almeida | FRA | fra_115_s01.wav | We have no bargain today. I will seek another stall.
Francisco de Almeida | FRA | fra_116_s01.wav | Our figures did not meet. I must take my business elsewhere.
Francisco de Almeida | FRA | fra_117_s01.wav | I came hoping for this lot, but I leave without it.
Francisco de Almeida | FRA | fra_118_s01.wav | No sale between us today. I will move on.
Francisco de Almeida | FRA | fra_122_s01.wav | Merchant? I am here when you are ready to continue.
Francisco de Almeida | FRA | fra_123_s01.wav | The
Francisco de Almeida | FRA | fra_123_s02.wav | still has my interest. Shall we talk?
Francisco de Almeida | FRA | fra_124_s01.wav | Take your moment; then tell me your figure.
Francisco de Almeida | FRA | fra_125_s01.wav | I have waited a while. Shall we settle the price?
Francisco de Almeida | FRA | fra_126_s01.wav | Please tell me where we stand; I have other stalls to visit.
Francisco de Almeida | FRA | fra_127_s01.wav | The lot is still before us. What is your answer?
Francisco de Almeida | FRA | fra_128_s01.wav | You bargain with spirit, but I remain at
Francisco de Almeida | FRA | fra_129_s01.wav | I enjoy the contest. My offer is still
Francisco de Almeida | FRA | fra_130_s01.wav | The conversation is good; my number has not changed from
Francisco de Almeida | FRA | fra_131_s01.wav | You make this lively, merchant.
Francisco de Almeida | FRA | fra_131_s02.wav | stands.
Francisco de Almeida | FRA | fra_132_s01.wav | I hope we find a bargain, though I am holding at
Francisco de Almeida | FRA | fra_133_s01.wav | My offer remains
Francisco de Almeida | FRA | fra_133_s02.wav | . What figure do you propose?
Francisco de Almeida | FRA | fra_134_s01.wav | I have weighed it.
Francisco de Almeida | FRA | fra_134_s02.wav | still stands.
Francisco de Almeida | FRA | fra_135_s01.wav | There is no change to my offer:
Francisco de Almeida | FRA | fra_136_s01.wav | I am still at
Francisco de Almeida | FRA | fra_136_s02.wav | for this measure.
Francisco de Almeida | FRA | fra_137_s01.wav | The price before you is
Francisco de Almeida | FRA | fra_137_s02.wav | ; I have not altered it.
Francisco de Almeida | FRA | fra_138_s01.wav | The quality may be sound. My offer remains
Francisco de Almeida | FRA | fra_139_s01.wav | I cannot justify more;
Francisco de Almeida | FRA | fra_139_s02.wav | stands.
Francisco de Almeida | FRA | fra_140_s01.wav | I have kept my figure at
Francisco de Almeida | FRA | fra_141_s01.wav | I have listened. I am still offering
Francisco de Almeida | FRA | fra_142_s01.wav | My figure stays at
Francisco de Almeida | FRA | fra_142_s02.wav | . Give me another reason to reconsider.
Francisco de Almeida | FRA | fra_143_s01.wav | I am still at
Francisco de Almeida | FRA | fra_143_s02.wav | . Your answer?
Francisco de Almeida | FRA | fra_144_s01.wav | No new figure from me.
Francisco de Almeida | FRA | fra_144_s02.wav | stands.
Francisco de Almeida | FRA | fra_145_s01.wav | , unchanged. Please decide.
Francisco de Almeida | FRA | fra_146_s01.wav | I have not raised it.
Francisco de Almeida | FRA | fra_146_s02.wav | is my offer.
Francisco de Almeida | FRA | fra_147_s01.wav | The offer remains
Francisco de Almeida | FRA | fra_147_s02.wav | . I need a reply.
Francisco de Almeida | FRA | fra_148_s01.wav | My time is short; I remain at
Francisco de Almeida | FRA | fra_149_s01.wav | The offer has not changed:
Francisco de Almeida | FRA | fra_149_s02.wav | . Please decide soon.
Francisco de Almeida | FRA | fra_150_s01.wav | I can wait for an answer, not another long silence.
Francisco de Almeida | FRA | fra_150_s02.wav | stands.
Francisco de Almeida | FRA | fra_151_s01.wav | My number has not moved:
Francisco de Almeida | FRA | fra_151_s02.wav | . Do you wish to answer?
Francisco de Almeida | FRA | fra_152_s01.wav | The figure is still
Francisco de Almeida | FRA | fra_152_s02.wav | . Be direct with me.
Francisco de Almeida | FRA | fra_153_s01.wav | is beyond me, and time is short.
Francisco de Almeida | FRA | fra_154_s01.wav | That asking price remains too high. I need a workable figure soon.
Francisco de Almeida | FRA | fra_155_s01.wav | I cannot pay
Francisco de Almeida | FRA | fra_155_s02.wav | . Please answer plainly.
Francisco de Almeida | FRA | fra_156_s01.wav | I can raise my offer to
Francisco de Almeida | FRA | fra_156_s02.wav | . Please decide soon.
Francisco de Almeida | FRA | fra_157_s01.wav | My new figure is
Francisco de Almeida | FRA | fra_157_s02.wav | . I cannot wait long.
Francisco de Almeida | FRA | fra_158_s01.wav | A quicker step from me:
Francisco de Almeida | FRA | fra_158_s02.wav | . Shall we decide?
Francisco de Almeida | FRA | fra_162_s01.wav | Our prices do not meet. I must leave now.
Francisco de Almeida | FRA | fra_163_s01.wav | This lot has held me long enough. I will try another stall.
Francisco de Almeida | FRA | fra_164_s01.wav | No bargain today. I must move on.
Francisco de Almeida | FRA | fra_num_1.wav | one
Francisco de Almeida | FRA | fra_num_2.wav | two
Francisco de Almeida | FRA | fra_num_3.wav | three
Francisco de Almeida | FRA | fra_num_4.wav | four
Francisco de Almeida | FRA | fra_num_5.wav | five
Francisco de Almeida | FRA | fra_num_6.wav | six
Francisco de Almeida | FRA | fra_num_7.wav | seven
Francisco de Almeida | FRA | fra_num_8.wav | eight
Francisco de Almeida | FRA | fra_num_9.wav | nine
Francisco de Almeida | FRA | fra_num_10.wav | ten
Francisco de Almeida | FRA | fra_num_11.wav | eleven
Francisco de Almeida | FRA | fra_num_12.wav | twelve
Francisco de Almeida | FRA | fra_num_13.wav | thirteen
Francisco de Almeida | FRA | fra_num_14.wav | fourteen
Francisco de Almeida | FRA | fra_num_15.wav | fifteen
Francisco de Almeida | FRA | fra_num_16.wav | sixteen
Francisco de Almeida | FRA | fra_num_17.wav | seventeen
Francisco de Almeida | FRA | fra_num_18.wav | eighteen
Francisco de Almeida | FRA | fra_num_19.wav | nineteen
Francisco de Almeida | FRA | fra_num_20.wav | twenty
Francisco de Almeida | FRA | fra_num_30.wav | thirty
Francisco de Almeida | FRA | fra_num_40.wav | forty
Francisco de Almeida | FRA | fra_num_50.wav | fifty
Francisco de Almeida | FRA | fra_num_60.wav | sixty
Francisco de Almeida | FRA | fra_num_70.wav | seventy
Francisco de Almeida | FRA | fra_num_80.wav | eighty
Francisco de Almeida | FRA | fra_num_90.wav | ninety
Francisco de Almeida | FRA | fra_num_100.wav | one hundred
Francisco de Almeida | FRA | fra_num_200.wav | two hundred
Francisco de Almeida | FRA | fra_num_300.wav | three hundred
Francisco de Almeida | FRA | fra_num_400.wav | four hundred
Francisco de Almeida | FRA | fra_num_500.wav | five hundred
Francisco de Almeida | FRA | fra_num_600.wav | six hundred
Francisco de Almeida | FRA | fra_num_700.wav | seven hundred
Francisco de Almeida | FRA | fra_num_800.wav | eight hundred
Francisco de Almeida | FRA | fra_num_900.wav | nine hundred
Francisco de Almeida | FRA | fra_spice_pepper.wav | pepper
Francisco de Almeida | FRA | fra_spice_cinnamon.wav | cinnamon
Francisco de Almeida | FRA | fra_spice_cardamom.wav | cardamom
Francisco de Almeida | FRA | fra_currency_varahas.wav | varahas
Francisco de Almeida | FRA | fra_unit_seer.wav | Seer
Francisco de Almeida | FRA | fra_unit_seers.wav | Seers
Francisco de Almeida | FRA | fra_unit_veesai.wav | Veesai
Francisco de Almeida | FRA | fra_unit_palam.wav | Palam
Francisco de Almeida | FRA | fra_unit_palams.wav | Palams
Francisco de Almeida | FRA | fra_unit_manangu.wav | Manangu
Francisco de Almeida | FRA | fra_unit_manangus.wav | Manangus
Francisco de Almeida | FRA | fra_unit_bahar.wav | Bahar
Francisco de Almeida | FRA | fra_unit_bahars.wav | Bahars
```

### Father Penteado

```text
Father Penteado | FAT | fat_001_s01.wav | Com licença, merchant. I am Father Penteado. I wish to buy
Father Penteado | FAT | fat_001_s02.wav | of
Father Penteado | FAT | fat_002_s01.wav | Good day. I am Father Penteado, looking for
Father Penteado | FAT | fat_002_s02.wav | of
Father Penteado | FAT | fat_003_s01.wav | Your stall has my attention. I am seeking
Father Penteado | FAT | fat_003_s02.wav | of
Father Penteado | FAT | fat_004_s01.wav | I am Father Penteado. I would like
Father Penteado | FAT | fat_004_s02.wav | of
Father Penteado | FAT | fat_004_s03.wav | , if the measure is sound.
Father Penteado | FAT | fat_005_s01.wav | We have spoken before. What is your price for
Father Penteado | FAT | fat_005_s02.wav | today?
Father Penteado | FAT | fat_006_s01.wav | Your stall is familiar to me now. Let us consider this lot on its own merits.
Father Penteado | FAT | fat_007_s01.wav | Good to see you again, merchant. What do you ask for this measure?
Father Penteado | FAT | fat_008_s01.wav | I have returned for your
Father Penteado | FAT | fat_008_s02.wav | . Shall we begin afresh?
Father Penteado | FAT | fat_009_s01.wav | I seek
Father Penteado | FAT | fat_009_s02.wav | of
Father Penteado | FAT | fat_009_s03.wav | . What is your price?
Father Penteado | FAT | fat_010_s01.wav | of
Father Penteado | FAT | fat_010_s02.wav | is what I came to buy.
Father Penteado | FAT | fat_011_s01.wav | It is the
Father Penteado | FAT | fat_011_s02.wav | I need, in a measure of
Father Penteado | FAT | fat_012_s01.wav | My interest is in
Father Penteado | FAT | fat_012_s02.wav | of
Father Penteado | FAT | fat_012_s03.wav | , provided we can agree on terms.
Father Penteado | FAT | fat_013_s01.wav | For this lot, my present offer is
Father Penteado | FAT | fat_014_s01.wav | I have allowed
Father Penteado | FAT | fat_014_s02.wav | for this purchase.
Father Penteado | FAT | fat_015_s01.wav | The figure I can offer now is
Father Penteado | FAT | fat_016_s01.wav | I am considering
Father Penteado | FAT | fat_016_s02.wav | for the measure before us.
Father Penteado | FAT | fat_017_s01.wav | The measure I ask for is
Father Penteado | FAT | fat_017_s02.wav | of
Father Penteado | FAT | fat_018_s01.wav | I need
Father Penteado | FAT | fat_018_s02.wav | of
Father Penteado | FAT | fat_018_s03.wav | ; no more for this purchase.
Father Penteado | FAT | fat_019_s01.wav | is the quantity I have in mind.
Father Penteado | FAT | fat_020_s01.wav | I am looking for
Father Penteado | FAT | fat_020_s02.wav | of
Father Penteado | FAT | fat_020_s03.wav | for this purchase.
Father Penteado | FAT | fat_021_s01.wav | Com licença, what would you ask for
Father Penteado | FAT | fat_021_s02.wav | of
Father Penteado | FAT | fat_022_s01.wav | I have named the measure. What price do you put on it?
Father Penteado | FAT | fat_023_s01.wav | Tell me your asking price for the
Father Penteado | FAT | fat_023_s02.wav | , please.
Father Penteado | FAT | fat_024_s01.wav | What sum would make this lot mine, if we agree?
Father Penteado | FAT | fat_025_s01.wav | I understand: the measure is now
Father Penteado | FAT | fat_025_s02.wav | of
Father Penteado | FAT | fat_025_s03.wav | . Let us price that lot.
Father Penteado | FAT | fat_026_s01.wav | The quantity has changed to
Father Penteado | FAT | fat_026_s02.wav | . We should consider the price anew.
Father Penteado | FAT | fat_027_s01.wav | Very well,
Father Penteado | FAT | fat_027_s02.wav | of
Father Penteado | FAT | fat_027_s03.wav | . What do you ask for that measure?
Father Penteado | FAT | fat_028_s01.wav | I have the new quantity:
Father Penteado | FAT | fat_028_s02.wav | . Let us speak of its value.
Father Penteado | FAT | fat_029_s01.wav | is more than I can justify for this lot.
Father Penteado | FAT | fat_030_s01.wav | I respect your asking price, but I cannot buy at
Father Penteado | FAT | fat_031_s01.wav | That sum is beyond what I will pay. Perhaps we can find firmer ground.
Father Penteado | FAT | fat_032_s01.wav | The measure before us does not support that price. I cannot accept it.
Father Penteado | FAT | fat_033_s01.wav | We are near a workable price, though
Father Penteado | FAT | fat_033_s02.wav | is still above my reach.
Father Penteado | FAT | fat_034_s01.wav | I can consider the lot, but my figure is
Father Penteado | FAT | fat_035_s01.wav | Your price is close. Could we speak nearer
Father Penteado | FAT | fat_036_s01.wav | There may be room between us. My offer remains
Father Penteado | FAT | fat_037_s01.wav | That figure is unusually low. Are these truly the terms you intend?
Father Penteado | FAT | fat_038_s01.wav | I expected a higher price. A sum so modest naturally invites caution.
Father Penteado | FAT | fat_039_s01.wav | You have surprised me with that sum. Why do you offer it for so little?
Father Penteado | FAT | fat_040_s01.wav | A low price calls for care. Tell me, merchant, are you certain of this figure?
Father Penteado | FAT | fat_041_s01.wav | Let me offer
Father Penteado | FAT | fat_041_s02.wav | for this measure. Would you consider it?
Father Penteado | FAT | fat_042_s01.wav | I can begin at
Father Penteado | FAT | fat_042_s02.wav | for the
Father Penteado | FAT | fat_043_s01.wav | My first answer is
Father Penteado | FAT | fat_043_s02.wav | . Tell me how you see it.
Father Penteado | FAT | fat_044_s01.wav | I would put
Father Penteado | FAT | fat_044_s02.wav | before you for
Father Penteado | FAT | fat_044_s03.wav | of
Father Penteado | FAT | fat_045_s01.wav | I have considered your words. I can raise my offer to
Father Penteado | FAT | fat_046_s01.wav | Let me move to
Father Penteado | FAT | fat_046_s02.wav | . Does that bring us closer?
Father Penteado | FAT | fat_047_s01.wav | There is merit in your argument. My new figure is
Father Penteado | FAT | fat_048_s01.wav | I can offer
Father Penteado | FAT | fat_048_s02.wav | now; weigh it against your asking price.
Father Penteado | FAT | fat_049_s01.wav | After considering the lot, I can offer
Father Penteado | FAT | fat_049_s02.wav | . What is your answer?
Father Penteado | FAT | fat_050_s01.wav | I will take another step to
Father Penteado | FAT | fat_050_s02.wav | . Can we agree there?
Father Penteado | FAT | fat_051_s01.wav | My offer rises to
Father Penteado | FAT | fat_051_s02.wav | . I hope it gives us room to settle.
Father Penteado | FAT | fat_052_s01.wav | I can put
Father Penteado | FAT | fat_052_s02.wav | before you at this point in our bargaining.
Father Penteado | FAT | fat_053_s01.wav | I have thought it through. My offer remains
Father Penteado | FAT | fat_054_s01.wav | Your reasons are heard; my figure is still
Father Penteado | FAT | fat_055_s01.wav | I cannot increase the offer now.
Father Penteado | FAT | fat_055_s02.wav | stands.
Father Penteado | FAT | fat_056_s01.wav | We may discuss the quality further, but my offer has not moved from
Father Penteado | FAT | fat_057_s01.wav | The same sum remains before you:
Father Penteado | FAT | fat_058_s01.wav | We agree on
Father Penteado | FAT | fat_058_s02.wav | for
Father Penteado | FAT | fat_058_s03.wav | of
Father Penteado | FAT | fat_058_s04.wav | . May I have the goods?
Father Penteado | FAT | fat_059_s01.wav | That price is fair to me:
Father Penteado | FAT | fat_059_s02.wav | for
Father Penteado | FAT | fat_059_s03.wav | of
Father Penteado | FAT | fat_059_s04.wav | . I await the lot.
Father Penteado | FAT | fat_060_s01.wav | Then our terms are
Father Penteado | FAT | fat_060_s02.wav | of
Father Penteado | FAT | fat_060_s03.wav | at
Father Penteado | FAT | fat_060_s04.wav | . Please fill the bag.
Father Penteado | FAT | fat_061_s01.wav | I accept those terms:
Father Penteado | FAT | fat_061_s02.wav | for
Father Penteado | FAT | fat_061_s03.wav | of
Father Penteado | FAT | fat_061_s04.wav | . Please prepare the goods.
Father Penteado | FAT | fat_062_s01.wav | Agreed:
Father Penteado | FAT | fat_062_s02.wav | of
Father Penteado | FAT | fat_062_s03.wav | for
Father Penteado | FAT | fat_062_s04.wav | . I await the goods.
Father Penteado | FAT | fat_063_s01.wav | We have a price of
Father Penteado | FAT | fat_063_s02.wav | for
Father Penteado | FAT | fat_063_s03.wav | of
Father Penteado | FAT | fat_063_s04.wav | . Please bring the lot.
Father Penteado | FAT | fat_064_s01.wav | Then the terms are clear:
Father Penteado | FAT | fat_064_s02.wav | of
Father Penteado | FAT | fat_064_s03.wav | . I will wait while you fill the bag.
Father Penteado | FAT | fat_065_s01.wav | I accept that bargain. Bring
Father Penteado | FAT | fat_065_s02.wav | of
Father Penteado | FAT | fat_065_s03.wav | for
Father Penteado | FAT | fat_065_s04.wav | , please.
Father Penteado | FAT | fat_066_s01.wav | You decline that figure. What price would you consider instead?
Father Penteado | FAT | fat_067_s01.wav | My offer does not suit you, I understand. Tell me what might.
Father Penteado | FAT | fat_068_s01.wav | We have not agreed at that sum. I am willing to hear another.
Father Penteado | FAT | fat_069_s01.wav | A refusal is clear enough. What figure do you have in mind?
Father Penteado | FAT | fat_070_s01.wav | I have weighed the quantity against its quality. That brings me to
Father Penteado | FAT | fat_071_s01.wav | A fair price must answer for the measure as well as the fragrance. I offer
Father Penteado | FAT | fat_072_s01.wav | I am trying to pay for what is here, neither more nor less:
Father Penteado | FAT | fat_073_s01.wav | The lot looks promising, but I must judge it carefully. My figure is
Father Penteado | FAT | fat_099_s01.wav | I cannot answer that well, and I would rather be honest about it.
Father Penteado | FAT | fat_100_s01.wav | That is beyond what I know. Perhaps we can return to the matter before us.
Father Penteado | FAT | fat_101_s01.wav | An interesting question, but I should not pretend to have an answer.
Father Penteado | FAT | fat_102_s01.wav | I can speak to this purchase; on that subject I would be guessing.
Father Penteado | FAT | fat_103_s01.wav | I will hear a hard price, merchant, but not an insult.
Father Penteado | FAT | fat_104_s01.wav | Let us speak plainly and with respect, or we shall get nowhere.
Father Penteado | FAT | fat_105_s01.wav | I have come to bargain in good faith. Please address me with the same care.
Father Penteado | FAT | fat_106_s01.wav | Perdão, I would rather ask than mistake your meaning.
Father Penteado | FAT | fat_107_s01.wav | I missed an important part of that.
Father Penteado | FAT | fat_108_s01.wav | Forgive me, I need the point made clear.
Father Penteado | FAT | fat_109_s01.wav | Before I answer, please clarify this:
Father Penteado | FAT | fat_111_s01.wav | Muito obrigado. The goods are here as agreed; you have my thanks.
Father Penteado | FAT | fat_112_s01.wav | The measure has been delivered, and the terms kept. Thank you, merchant.
Father Penteado | FAT | fat_113_s01.wav | I have received the
Father Penteado | FAT | fat_113_s02.wav | . A fair exchange, carefully made.
Father Penteado | FAT | fat_114_s01.wav | The purchase is complete. I appreciate your honest measure.
Father Penteado | FAT | fat_115_s01.wav | We could not agree. I will look elsewhere for this lot.
Father Penteado | FAT | fat_116_s01.wav | I must leave without the
Father Penteado | FAT | fat_116_s02.wav | today. I wish you well.
Father Penteado | FAT | fat_117_s01.wav | Our figures have not met. I shall take my business to another stall.
Father Penteado | FAT | fat_118_s01.wav | There is no sale between us. I must take my leave.
Father Penteado | FAT | fat_122_s01.wav | Merchant? I am ready when you are.
Father Penteado | FAT | fat_123_s01.wav | I am still considering the
Father Penteado | FAT | fat_123_s02.wav | . Shall we continue?
Father Penteado | FAT | fat_124_s01.wav | Take a moment, then tell me your thought on the price.
Father Penteado | FAT | fat_125_s01.wav | We have let this matter rest a while. What is your answer?
Father Penteado | FAT | fat_126_s01.wav | I would appreciate a decision on the lot now.
Father Penteado | FAT | fat_127_s01.wav | The measure is before us. Shall we settle on a price?
Father Penteado | FAT | fat_128_s01.wav | I appreciate the conversation; my offer remains
Father Penteado | FAT | fat_129_s01.wav | You make a thoughtful case. I am still at
Father Penteado | FAT | fat_130_s01.wav | We can speak freely, though my figure stays
Father Penteado | FAT | fat_131_s01.wav | I hope we find agreement. For now,
Father Penteado | FAT | fat_131_s02.wav | stands.
Father Penteado | FAT | fat_132_s01.wav | I am listening, but I have not moved above
Father Penteado | FAT | fat_133_s01.wav | My offer is unchanged at
Father Penteado | FAT | fat_134_s01.wav | After considering it, I remain at
Father Penteado | FAT | fat_135_s01.wav | The figure before you is still
Father Penteado | FAT | fat_136_s01.wav | I can listen further. I cannot increase
Father Penteado | FAT | fat_136_s02.wav | now.
Father Penteado | FAT | fat_137_s01.wav | There is no new offer from me:
Father Penteado | FAT | fat_138_s01.wav | My offer remains
Father Penteado | FAT | fat_138_s02.wav | . Give me a reason to reconsider.
Father Penteado | FAT | fat_139_s01.wav | I have heard your claim;
Father Penteado | FAT | fat_139_s02.wav | still stands.
Father Penteado | FAT | fat_140_s01.wav | The measure does not warrant a higher bid from me.
Father Penteado | FAT | fat_140_s02.wav | remains.
Father Penteado | FAT | fat_141_s01.wav | I am not persuaded to change
Father Penteado | FAT | fat_142_s01.wav | Let us keep the figures exact. Mine is still
Father Penteado | FAT | fat_143_s01.wav | My offer is still
Father Penteado | FAT | fat_143_s02.wav | . Your answer, please.
Father Penteado | FAT | fat_144_s01.wav | No change:
Father Penteado | FAT | fat_144_s02.wav | . Can you decide?
Father Penteado | FAT | fat_145_s01.wav | I remain at
Father Penteado | FAT | fat_145_s02.wav | . Please be direct.
Father Penteado | FAT | fat_146_s01.wav | I have not changed the figure. Please consider
Father Penteado | FAT | fat_147_s01.wav | The same figure,
Father Penteado | FAT | fat_147_s02.wav | . What do you say?
Father Penteado | FAT | fat_148_s01.wav | Time is short. My offer remains
Father Penteado | FAT | fat_149_s01.wav | The figure has not moved from
Father Penteado | FAT | fat_149_s02.wav | . Please answer soon.
Father Penteado | FAT | fat_150_s01.wav | I am still at
Father Penteado | FAT | fat_150_s02.wav | . I need your decision.
Father Penteado | FAT | fat_151_s01.wav | No new offer:
Father Penteado | FAT | fat_151_s02.wav | . Let us be clear.
Father Penteado | FAT | fat_152_s01.wav | stands. I cannot wait much longer for an answer.
Father Penteado | FAT | fat_153_s01.wav | is too high. I need a workable figure soon.
Father Penteado | FAT | fat_154_s01.wav | I cannot pay that sum. Please be clear about your price.
Father Penteado | FAT | fat_155_s01.wav | The asking price is beyond me, and I have little time left.
Father Penteado | FAT | fat_156_s01.wav | I can move to
Father Penteado | FAT | fat_156_s02.wav | . Please answer soon.
Father Penteado | FAT | fat_157_s01.wav | My new offer is
Father Penteado | FAT | fat_157_s02.wav | . Can we decide?
Father Penteado | FAT | fat_158_s01.wav | I have raised it to
Father Penteado | FAT | fat_158_s02.wav | . I need your answer.
Father Penteado | FAT | fat_162_s01.wav | We have no agreement. I must leave now.
Father Penteado | FAT | fat_163_s01.wav | I can wait no longer. I will seek another stall.
Father Penteado | FAT | fat_164_s01.wav | Our figures remain apart. I must go without the goods.
Father Penteado | FAT | fat_num_1.wav | one
Father Penteado | FAT | fat_num_2.wav | two
Father Penteado | FAT | fat_num_3.wav | three
Father Penteado | FAT | fat_num_4.wav | four
Father Penteado | FAT | fat_num_5.wav | five
Father Penteado | FAT | fat_num_6.wav | six
Father Penteado | FAT | fat_num_7.wav | seven
Father Penteado | FAT | fat_num_8.wav | eight
Father Penteado | FAT | fat_num_9.wav | nine
Father Penteado | FAT | fat_num_10.wav | ten
Father Penteado | FAT | fat_num_11.wav | eleven
Father Penteado | FAT | fat_num_12.wav | twelve
Father Penteado | FAT | fat_num_13.wav | thirteen
Father Penteado | FAT | fat_num_14.wav | fourteen
Father Penteado | FAT | fat_num_15.wav | fifteen
Father Penteado | FAT | fat_num_16.wav | sixteen
Father Penteado | FAT | fat_num_17.wav | seventeen
Father Penteado | FAT | fat_num_18.wav | eighteen
Father Penteado | FAT | fat_num_19.wav | nineteen
Father Penteado | FAT | fat_num_20.wav | twenty
Father Penteado | FAT | fat_num_30.wav | thirty
Father Penteado | FAT | fat_num_40.wav | forty
Father Penteado | FAT | fat_num_50.wav | fifty
Father Penteado | FAT | fat_num_60.wav | sixty
Father Penteado | FAT | fat_num_70.wav | seventy
Father Penteado | FAT | fat_num_80.wav | eighty
Father Penteado | FAT | fat_num_90.wav | ninety
Father Penteado | FAT | fat_num_100.wav | one hundred
Father Penteado | FAT | fat_num_200.wav | two hundred
Father Penteado | FAT | fat_num_300.wav | three hundred
Father Penteado | FAT | fat_num_400.wav | four hundred
Father Penteado | FAT | fat_num_500.wav | five hundred
Father Penteado | FAT | fat_num_600.wav | six hundred
Father Penteado | FAT | fat_num_700.wav | seven hundred
Father Penteado | FAT | fat_num_800.wav | eight hundred
Father Penteado | FAT | fat_num_900.wav | nine hundred
Father Penteado | FAT | fat_spice_pepper.wav | pepper
Father Penteado | FAT | fat_spice_cinnamon.wav | cinnamon
Father Penteado | FAT | fat_spice_cardamom.wav | cardamom
Father Penteado | FAT | fat_currency_varahas.wav | varahas
Father Penteado | FAT | fat_unit_seer.wav | Seer
Father Penteado | FAT | fat_unit_seers.wav | Seers
Father Penteado | FAT | fat_unit_veesai.wav | Veesai
Father Penteado | FAT | fat_unit_palam.wav | Palam
Father Penteado | FAT | fat_unit_palams.wav | Palams
Father Penteado | FAT | fat_unit_manangu.wav | Manangu
Father Penteado | FAT | fat_unit_manangus.wav | Manangus
Father Penteado | FAT | fat_unit_bahar.wav | Bahar
Father Penteado | FAT | fat_unit_bahars.wav | Bahars
```

## P1 copy-ready generation script

```text
No confirmed replacement recordings.
```

## Evidence and reproducibility

Inspected current source, not just the prior name report: six character tables (1,047 exact source/CSV template matches, 34 scenarios each); registry/provider; ChatManager/AudioManager/Sherpa voice references; TutorialManager/SpiceIntroSequence/SpiceTutorialManager/CoinSceneManager; NarratorUIManager split lookup and the Free Roam NPC assets; EpilogueStoryManager/modern ending; enabled build scene settings and relevant serialized scene fields; both voice databases; 170 project WAV headers (all nonempty readable WAVs); Unity Assets audio filenames; existing recording-plan allocation/number/vocabulary rules and 1,933-slot summary. No current candidate planned filename was found in Unity Assets. External uncommitted/out-of-repository voice banks are unknown.

| Evidence file | SHA-256 at audit time |
|---|---|
| `unity/StorytellingVR/Assets/_Project/Audio/DialogueVoiceDatabase.asset` | `c935db35f7d118b8bee4443c0e92646dafbc735e7de149e418b5358ba0da2550` |
| `unity/StorytellingVR/Assets/_Project/Audio/Tutorial/TutorialDialogueVoiceDatabase.asset` | `c7f8a366ccd26832682467b3d05a5a3e759547d5d286a80c0a5cedd91c084d0d` |
| `documentation/DialogueAudit/Level1_AllNPC_Dialogues.csv` | `0bcb654f8572ec081257e9ca62c588b9d56f8b29a752f76439852e9cfb4df44d` |
| `documentation/DialogueAudit/RecordingSheets/Recording_Summary.json` | `8fcdd7280404a2665bb2b0f42c7de2bf1665da3e1522481d92820f27bb2af2f0` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs` | `b6691244c8ba0e5f165782c4895b26b24f481b9b87ba05a8ca700da0f4a5296f` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/ChinappaNaikDialogue.cs` | `c61c68e14ec329d73638646058bb9b786b0a28d700ad1d7c68ea0f012795f238` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/FatherPenteadoDialogue.cs` | `4b255cd1f2d6c156e8e68d409ac5415f2a3ff5ab73908e7eb3eca7c0bd846524` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/FranciscoDialogue.cs` | `abee6cb9803eea13aee6a5086c34288bb08fa4db960058634b14bd17573d74d3` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/LakshmiAmmaDialogue.cs` | `9bb43d49cb3bc60052d27cc036d99259bde0502b4c2938b5f4f99a513af55465` |
| `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs` | `e88c366c2d83d457c083958e0fbae12c91de56db3738c0269f38b11211ecbe5f` |

Only `Missing_Outdated_Prerecorded_Voices_Checklist.md` was created. No original name-correction report, recording status, workbook, WAV, metadata, mapping, source or scene was modified. No commit, push or branch switch was performed.
