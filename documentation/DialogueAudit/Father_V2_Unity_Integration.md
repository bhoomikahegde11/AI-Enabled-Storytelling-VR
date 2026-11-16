# Father v2 Unity integration

Father's bank is wired in `Level1_MainLoopUpdated` on GameManager. `AudioManager.prerecordedNpcProvider` references the new enabled `PreRecordedNpcTtsProvider`; its AudioSource is the existing BuyerNPC source. The original `localNpcTtsProvider` reference and other NPCs' providers are unchanged.

Imported bank: `unity/StorytellingVR/Assets/_Project/Audio/NPCVoices/Level1/FatherPenteadoV2/`. It contains 425 original processed WAVs, per-recording metadata, the original package manifests, a Unity-readable operation map and a `DialogueVoiceDatabase` asset. WAV bytes are unchanged. Import settings preserve native mono 24 kHz PCM with normalization/compression disabled, Decompress On Load and lazy audio-data loading.

## Runtime contract

- The existing dialogue provider still selects the template and returns the original string. It also captures a copied `LastPresentation` with the exact selected source template/scenario and values. The bank resolves that exact selection to its original recording/dialogue ID. No second random choice, intent detection or approximate sentence matching occurs.
- ChatManager passes that presentation through the existing AudioManager. For Father, one prepared plan supplies the displayed subtitle and the actual clip sequence. Domain reply/history and negotiation decisions remain unchanged; HUD monetary/quantity values remain authoritative.
- Greetings, repeat greetings, negotiation, precise clarification cores, reminders, NO DEAL and settled delivery thanks use this path. Exact raw fallback matching renders known source templates with captured values; it never classifies prose into an intent.
- Accepted speech consumes copied immutable accepted terms. Successful-receipt speech additionally requires settlement. Quantity, price and spice stay dynamic. Unsupported terms or missing audio leave the subtitle and do not play a partial/unrelated sentence.
- All fragments are already processed Near-Contiguous + QuietEdgesV1 copies. They are joined into one utterance at their native sample rate with 1/4/25/55 ms semantic gaps and the package's 15 ms source-edge guard. No new trimming, gain, speed, pitch or crossfade processing is applied.
- The existing playback-start/failure hooks drive subtitles. Loading/playback is exposed through AudioManager's busy state. Stop/reset/customer replacement cancels the coroutine, invalidates its generation and destroys only the assembled temporary clip. Imported source clips remain intact.
- Character banks are data-driven; another NPC can use the same provider with its own mapping/database/native sample rate. No Qwen/Parler engine or Python is needed in the Quest player.

The implementation selectively reuses the historical `feature/prerecorded-npc-voices` provider's bank/AudioSource/event design. It does not restore its intent guessing, random complete-line selection, generic voice fallback, repeated-one-hundred number algorithm or old gameplay code.

## Check manually in Unity

1. Allow Unity to finish importing the new scripts/audio. Reload the saved `Level1_MainLoopUpdated` scene so external scene wiring is present; preserve any unrelated unsaved Inspector work first.
2. Run EditMode `Level1PrerecordedVoiceTests`, then the existing normal negotiation/conversation tests and `Level1ConversationRuntimeSoakTests.SmokeSoak_50Interactions`.
3. For deterministic manual testing, use the existing Level1GameState **Debug Force Character Id** = `father_penteado` before starting the interaction. Do not leave that override enabled for normal customer selection. Otherwise wait for Father to be selected normally; this integration does not force him first.
4. Check first/repeat greetings, 173/234/999-varaha offers, all three products, quantity changes, an unclear sentence, and acceptance followed by one correct physical scoop. Verify the accepted price/item/quantity and exactly-once settlement are unchanged.
5. Check subtitles against the recorded wording, wait for the full utterance before input opens, and inspect `[NPC VOICE]` logs for the exact dialogue ID/session/turn.
6. Interrupt speech with reset/customer departure/scene change, then start the next customer. No previous clip or delayed subtitle should restart. Check pause/resume mid-utterance.
7. On Quest, check loading/start latency, audibility/spatial placement, memory and uninterrupted joins. The component uses Unity AudioClips rather than Windows TTS or external model execution.

Father should speak when his greeting is presented, after the existing approach/greeting delay. This is not immediate on the first Play Mode frame and he is not guaranteed to be the first randomly selected customer.

## Automated checks and limits

`tools/voice/validate_father_integration.py` performs a targeted compilation of the edited C# sources/new EditMode tests against installed Unity assemblies, runs the actual pure C# planner outside Unity against Father v2 reference cases, checks continuous PCM ordering/gaps, and validates imported WAV hashes, GUIDs and saved scene references. It does not launch Unity, run Unity tests or build the player.

Existing v2 pronunciation/ending warnings remain pending listening review. The original scene's disabled Editor-only Sherpa assignment remains unchanged for other NPCs; this task does not manufacture new Quest voices for them. Father does not depend on Sherpa.
