# Changelog: June 15, 2026

## [2026-06-15] - Level 1 Standalone VR Dialogue Tables, Quest Speech Stack, and Vosk Offline STT

### Added
- Created a standalone Level 1 dialogue table system for VR negotiation mode:
  - `DialogueTableResponseProvider.cs`
  - `DialogueScenario.cs`
  - `DialogueLine.cs`
  - `DialogueBuckets.cs`
  - `CharacterDialogueSet.cs`
  - `DialogueCharacterRegistry.cs`
- Added character-specific dialogue libraries for:
  - `AbdulRahmanDialogue.cs`
  - `FranciscoDialogue.cs`
  - `LakshmiAmmaDialogue.cs`
- Added dialogue authoring and expansion notes in:
  - `Assets/_Project/Scripts/Level1/Core/Dialogues/DIALOGUE_AUTHORING_GUIDE.txt`
- Added Quest local NPC speech stack pieces:
  - `INpcTtsProvider.cs`
  - `AndroidNativeTtsProvider.cs`
- Added offline Vosk STT runtime for standalone Quest:
  - `Assets/_Project/Scripts/Level1/VoskSpeechProvider.cs`
  - Vosk wrapper files under `Assets/_Project/Scripts/ThirdParty/Vosk/`
  - `Assets/Plugins/Android/libvosk.so`
  - `Assets/StreamingAssets/Vosk/vosk-model-small-en-us-0.15/`

### Changed
- Updated `ChatManager.cs` so standalone NPC replies are post-processed through the dialogue table while preserving the rule-based negotiation result as the source of truth.
- Added one-time customer greeting playback when a buyer reaches the trade point, using dialogue-table greetings with fallback text.
- Restricted standalone local customer selection to dialogue-registered characters only, avoiding unsupported NPCs from spawning in dialogue-table mode.
- Updated `AudioManager.cs` and `ChatManager.cs` so the final standalone reply string can also flow through local TTS playback.
- Fixed Android Whisper packaging/linkage investigation and vendored Android static archives for package validation, while leaving the Whisper integration in place for future recovery.
- Enhanced `Level1VoiceInputManager.cs` to support inspector-based `ISpeechToTextProvider` override selection, allowing Quest Vosk STT without removing backend or Whisper code paths.
- Expanded `InputNormalizer.cs` with generalized spoken-number normalization for bargaining phrases and added normalization logs:
  - `five hundred` -> `500`
  - `one hundred and ten` -> `110`
  - `ninety eight` -> `98`
  - `deal for one hundred and twenty` -> `deal for 120`

### Diagnostics & Reliability
- Added Quest STT diagnostics across the local recording path:
  - microphone permission/device logging
  - clip duration, peak amplitude, RMS amplitude, silence detection
  - model path, model existence, raw transcription, normalized transcription, failure reasons
  - optional last-recording WAV dump for debugging
- Added Android native TTS lifecycle and failure logs:
  - initialization
  - ready state
  - speaking
  - shutdown
- Added Vosk-specific Quest logs:
  - initialization
  - model source/runtime path
  - model existence
  - native library availability
  - raw recognition output
  - final transcription

### Asset & Repository Hygiene
- Removed tracked Whisper `.bin` runtime binaries from git history flow and added download instructions / ignore protection for large local models.
- Removed the unused local Whisper `ggml-small.en.bin` runtime model from project content after Vosk validation to reduce repository and build weight.
- Cleaned generated Unity/Quest build artifacts such as:
  - `Library/Bee/`
  - `Library/BuildPlayerData/`
  - `Library/Il2cppBuildCache/`
  - `Library/PlayerDataCache/`
  - APK outputs and player backup folders
  - Burst debug output folders

### Result
- Standalone Quest flow now supports:
  - rule-grounded character dialogue variation
  - character-specific customer greetings
  - Android-native NPC voice output
  - offline Vosk speech-to-text as a practical Quest fallback path
  - stronger spoken-price normalization before negotiation parsing
