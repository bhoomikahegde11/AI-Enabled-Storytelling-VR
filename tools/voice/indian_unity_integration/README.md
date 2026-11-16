# Level 1 Indian NPC voice integration

The external Indic Parler queue checks the hashes of the live Unity voice scripts and scene again during final validation. The runtime diff, Editor tool and EditMode tests are staged here as `UnityVoiceIntegration.patch`. Keep the live files intact until the existing validator finishes. No WAVs, production files or scene registrations are installed by this preparation.

## Existing Level 1 path

- `ChatManager` builds the input and calls `NegotiationStateManager.ProcessLocalTurn` with `RuleBasedNPCBrain`. `DialogueTableResponseProvider` selects the actual template and captures its original text, scenario, character, resolved values and frozen agreement, where applicable.
- `ChatManager` binds that copied presentation to the active turn. `AudioManager` requests an exact plan from `PreRecordedNpcTtsProvider` for the matching character. The provider reads a separate `DialogueVoiceDatabase` per character. It plays a single fixed clip directly or combines ordered PCM into one transient clip. The same existing error, start, cancellation and subtitle events stay in use.
- The saved scene registers Father Penteado only. Francisco is an eligible customer, with the existing local speech route. His studio has production takes, and this repository currently has no registered Francisco prerecorded bank to migrate.
- The prefab's positional source is on the BuyerNPC root, with volume 1, full 3D blend, logarithmic rolloff, 1 m minimum and 500 m maximum. It is routed directly to output. The existing Father bank temporarily raises the full-volume radius to 4 m at volume 1 and restores the source after playback or cancellation. The two main ambience sources are separate and set to 0.20 and 0.12 in the saved scene. These are Inspector settings. Relative audibility still needs listening on the headset.

## After the completion monitor reports successful final validation

1. In PowerShell from the Unity repository root, run:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File "tools\voice\indian_unity_integration\Apply-VoiceIntegration.ps1"
   ```

   If the studio moved, pass `-VoiceStudio 'path-to-NPC-Voice-Generator'`. The script checks the existing final reports and waits for no running worker or validator, then applies the reviewed patch. It does not spawn production jobs or change Git branches. If the diff no longer applies cleanly, stop and review the changed Unity source.
2. Let the open Unity Editor compile. Open `Level1_MainLoopUpdated` in Edit Mode. Run **Tools → Anubhava → Import Completed Indian NPC Voice Package** once for each of `Lakshmamma_Level1_v1`, `ChinnammaNaik_Level1_v1`, `AbdulRahman_Level1_v1`, and `SaraswatiChetti_Level1_v1` under the studio's `production/packages`.
3. The importer will check final validation, expected actor/speaker, report counts, all operation and vocabulary references, WAV format and SHA-256 before asset writes. It imports processed WAVs and the runtime mapping into `Assets/_Project/Audio/NPCVoices/Level1/IndianProduction/<character_id>`, then creates or reuses a `DialogueVoiceDatabase` and registers it on the existing `GameManager` provider. Importing the same package reuses asset paths and registration. A conflicting existing recording or separate registration is reported for review. Save the scene after the imports. Leave Father and any independently configured Portuguese bank alone.
4. Inspect **GameManager → Pre Recorded Npc Tts Provider → Character Banks**. Each new entry starts at **Override Audibility** on, **Voice Volume** 1.0, **Minimum Voice Distance** 4 m. These affect only the selected bank while it speaks. Adjust an individual bank if normal stall distance or ambience warrants it. The shared AudioSource, spatialization, ambience and mixer routing require no changes for import.

The importer uses native mono 44.1 kHz PCM, Decompress On Load, background loading and preload off, with the same settings explicitly set for Android. This protects exact sample boundaries and avoids preloading all four banks. PCM consumes more build space than compressed audio. Measure APK size and accumulated audio memory on Quest; this change does not add a clip-unload policy.

## Compatibility and known limits

- Whole number and quantity phrases take precedence in Indian banks. A missing phrase key uses the exact existing Int32 composition, including thousands, millions and billions. Missing component WAVs, unknown items or unresolvable values withhold the entire utterance.
- Numeric or quantity joins, including the join to the historical unit or to `this lot`, insert zero samples. Other joins use the package's default pacing and existing punctuation handling. Runtime does not trim, crossfade or normalize the processed clips.
- Indian subtitles retain the original selected text and units. Audio can say the number in words and use singular `varaha` as authored by the package. If a conditional counter would substitute HOLD speech under unchanged counter prose, audio is withheld. Portuguese banks keep their existing subtitle presentation and timing behavior.
- Authored exceptions documented as excluded in a package retain the existing subtitle-only path. A generated take's technical validation does not establish pronunciation or intelligibility. Package integration changes the Unity source/scene hashes after validation. If production needs to resume later, reconcile the locked source snapshot first.

## Editor and Quest smoke test

Run **IndianNpcVoiceIntegrationTests** and **Level1PrerecordedVoiceTests** in EditMode after applying the patch. Also run the project's usual Level 1 negotiation and lifecycle regressions.

On **Level1GameState → Editor Debug**, set **Debug Force Character Id** to one character at a time and re-enter Play Mode. Valid IDs are `lakshmi_amma`, `chinnamma_naik`, `abdul_rahman`, `saraswati_chetti`, `father_penteado`, and `francisco_de_almeida`. Leave **Debug Tts Character Override** blank. After testing, clear the character override to restore standard selection. The forced character code is compiled only into the Editor.

- Compare greeting, first offer, a counter and HOLD to the visible amount. Exercise a phrase price, a composed price and a price above 999 where accepted by the dialogue path; test singular currency where available.
- Test opening and changed quantities, spice, agreement versus rejection, and one historical question per character. Verify that fulfilled and failed orders play the right terminal lines and retain game behaviour.
- Leave or reset while speech is playing. Confirm the old audio stops, the next customer uses the correct bank and the original subtitle appears when a line is unavailable. Verify Father and Francisco through their existing paths.
- At typical Quest stall distance, listen over ambience and turn away to check that the voice stays positional. Verify memory after multiple customers and confirm the field overrides have no effect on a player build.

## Reproduce the static checks

`python -B tools/voice/indian_unity_integration/check_integration.py` uses the locally installed Unity 6000.3.10f1 compiler/reference cache and Mono. It only writes temporary fixtures. It does not launch Unity, import real packages or invoke gameplay.
