# Father Penteado — audibility correction

27 October 2026. Source/WAV inspection only; no Unity launch, Unity tests, synthesis or audio writes.

## Diagnosis

`Assets/_Project/Prefabs/Level1/NPC/BuyerNPC.prefab` AudioSource2082649019093156146 supplies the scene's shared AudioSource366259241. The active instance has no source-level volume override: volume1, pitch1, mute off, logarithmic rolloff, minDistance1m, maxDistance500m. Spatial blend is1 (panLevelCustomCurve constant1); Spatialize is off, so the plugin spatializer is not enabled although ordinary Unity3D attenuation/panning is. OutputAudioMixerGroup is null; no mixer attenuation is configured on this source. No Level1 code writes a persistent global listener-volume reduction. Pause controls listener pause, not gain. The similarly named disabled prefab source elsewhere in the scene is not the source assigned to AudioManager/Father.

Saved transform composition places BuyerTradePoint at approximately(31.0887,0.3358,6.5016) and the CenterEyeAnchor listener at(31.3,0.5,3.8): distance2.715m. The source is on the buyer root; runtime NavMesh placement and headset pose can change this distance. The saved scene is evidence of a distance mismatch, not a live measurement. A1m logarithmic reference distance at2.7m is consistent with roughly9dB distance loss; exact live attenuation must be checked in Editor. MaxDistance500 does not make a logarithmic source full-volume at conversational distances.

All425 imported WAVs were measured at native24kHz, without writing audio. Speech-active RMS uses20ms frames and a gate max(-50dBFS, loudest-frame RMS minus35dB); it is **not integrated LUFS** or proof of perceived loudness.

| Group | Count | Median speech-active RMS dBFS | Median whole-file RMS dBFS | Median / highest peak dBFS |
|---|---:|---:|---:|---:|
| Full clips |106|-22.65|-23.33|-6.35 / -2.36|
| Non-number fragments |285|-23.10|-23.65|-7.26 / -2.19|
| Number vocabulary |34|-23.20|-23.56|-8.95 / -5.69|

Sources are moderate/quiet, but numbers and full dialogue have comparable active levels. No extra gain reduction exists in PreRecordedNpcTtsProvider; it copies samples and inserts zero gaps. The default log-distance attenuation is the strongest identified playback-side contributor. Runtime hardware/OS volume and actual listener distance are unmeasured. Increasing raw sample gain blindly would risk clipping: the highest existing peak is already approximately-2.19dBFS.

In-memory reference assemblies,234varahas/pepper/oneSeer where relevant:

| ID | Clips | Duration | Added gaps total | Active RMS | Peak |
|---|---:|---:|---:|---:|---:|
| L1_FAT_042 |8|5.552s|16ms|-22.27dBFS|-5.57dBFS|
| L1_FAT_058 |12|8.533s|83ms|-22.17dBFS|-4.78dBFS|

These are reference assemblies measured in memory; no new/exported WAV was saved and Unity output was not recorded. They do not show systematic assembly loudness loss versus fixed clips.

## Minimal correction

`PreRecordedNpcTtsProvider.CharacterVoiceBank` now exposes opt-in `Override Audibility`, `Voice Volume` and `Minimum Voice Distance`. The saved Level1 Father bank enables it, volume1, minimum4m. New/unconfigured banks default to override OFF.

The provider applies these settings just before playing both a complete fixed clip and a continuous assembled clip. It captures and restores the shared source's previous volume/minimum distance on normal completion, cancellation, failure and disable/destruction. No PCM gain, compression, resampling, playback-speed change, source relocation, mixer/global setting change, or change to3D blend/maxDistance/rolloff was made. Other providers/NPCs receive the original settings after Father stops.

At the saved conversational distance, the new radius is intended to remove roughly9dB of distance loss. This is a reversible scene-bank adjustment, not a claim of a measured acoustic improvement. If the live source/listener is farther than4m, attenuation still applies. Editor/development logs include `[NPC VOICE LEVEL]` (actual settings before/after) and `[NPC VOICE DISTANCE]` (live listener distance once per utterance).

## Number-announcement inspection — recommendations only

Unity assembles one continuous PCM buffer, so it does not schedule each number as a separate AudioSource playback. Number-component gaps are1ms, ordinary joins4ms, clause joins25ms, sentence joins55ms; source-edge flagged clips impose at least15ms. These values were not changed.

For234 the source durations are two0.511s, hundred0.554s, and0.344s, thirty0.604s, four0.542s. At a-45dBFS threshold using10ms frames, measured leading/trailing low-energy regions are respectively10/121ms,20/24ms,0/4ms,20/4ms,40/52ms. The two->hundred boundary therefore contains about142ms of low-energy edge material plus the join, despite a configured gap of only1ms. Low energy can include quiet phonemes/breaths; it is not automatically removable silence.

After the volume correction, compare a fixed sentence against234,105,999. If announcement rhythm persists, the isolated number recordings' emphasis/intonation may contribute; measurements alone cannot establish that performance defect. A later approved correction could selectively refine verified source-edge silence on reversible assembly copies while preserving consonants, or record a few common contextual number/quantity phrases. Do not globally remove internal pauses, crossfade audible syllables or generate999 price clips. No source takes, voice-package metadata, fragment ordering or gap configuration were changed here.

## Manual verification in the already-open Editor

1. Let scripts compile. Reload the saved `Level1_MainLoopUpdated` scene, preserving unrelated unsaved work. Alternatively copy the three bank settings above into the existing scene Inspector so reloading is unnecessary.
2. On **GameManager -> PreRecordedNpcTtsProvider -> Character Banks -> Father**, verify Override Audibility ON, Voice Volume1, Minimum Voice Distance4. Keep the existing source and bank references. Do not apply changes to BuyerNPC.prefab.
3. Force `father_penteado` using the existing **Level1GameState -> Editor Debug -> Debug Force Character Id**. Press Play and negotiate from the ordinary stall position.
4. Compare a fixed identity/social line, greeting and dynamic price response. Check `[NPC VOICE DISTANCE]` and ensure the shared AudioSource temporarily reports minDistance4, volume1, spatialBlend1. Move/turn the listener to confirm positioning remains perceptible. Source stays full-volume within4m; this is deliberately broader than the old1m radius.
5. If the listener is just outside4m and still too quiet, temporarily try5 or6 in the bank's Minimum Voice Distance and replay the next line. Bank settings are applied per utterance. Keep Voice Volume1; lower it if now too loud. Do not amplify source samples or change all NPC sources.
6. Stop/reset Father mid-speech and then test a different customer. Verify the original source minDistance/volume return (normally1m/1), no stale speech restarts, and the next customer is not louder due to Father's override. Clear the force-character field after testing.
7. Optional focused Editor test to run manually: `Level1PrerecordedVoiceTests.VoiceAudibilityOverrideIsOptInAndStopRestoresTheSharedSource` (two cases). The tests were added but not executed here.

Runtime audibility, actual listener transforms, device output volume and perceived number delivery still require listening. Negotiation, text, subtitles and the approved audio package are unchanged.
