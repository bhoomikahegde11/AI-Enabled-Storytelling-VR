Stage 2 implements one global pause menu across the canonical journey. Unity, Unity Hub, test runners and builds were not launched. Import, compilation, visual rendering and runtime behavior still require your manual validation.

**1. Exact files changed**

This manifest lists Stage 2 edits only. Stage 1's Level 1 scene ownership correction remains intact; Stage 2 does not edit that scene. The existing Stage 1 test file retains all 11 tests, with its readiness/activation test updated for Stage 2.

Modified:

- [unity/StorytellingVR/Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Bootstrap.unity](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Bootstrap.unity)
- [unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinInteraction.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinInteraction.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinSceneManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinSceneManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinSequenceManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Coin/CoinSequenceManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Coin/VRInspectRotate.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Coin/VRInspectRotate.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalLifecycleStage1Tests.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalLifecycleStage1Tests.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/FreeRoamStoryManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/FreeRoamStoryManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/GateIntroFlow.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/GateIntroFlow.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/MeeraInspectionSequenceController.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/MeeraInspectionSequenceController.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/MeeraSequenceController.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/MeeraSequenceController.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCDialogueVRLaserPointer.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCDialogueVRLaserPointer.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCInteraction.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCInteraction.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCQuestionUIManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NPCQuestionUIManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NarratorUIManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/NarratorUIManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/SpiceMerchantGuideSequence.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/SpiceMerchantGuideSequence.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/StallEntryTrigger.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/StallEntryTrigger.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/StoryTrigger.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/StoryTrigger.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/TeleportTutorialCompleteTrigger.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/FreeRoam/TeleportTutorialCompleteTrigger.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/GameManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/GameManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/IntroSequence/ModernStudyRoomIntroController.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/IntroSequence/ModernStudyRoomIntroController.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/AudioManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/AudioManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/BazaarFeedbackManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/BazaarFeedbackManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Level1GameState.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Level1GameState.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1DebugForceAccept.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1DebugForceAccept.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1HUDManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1HUDManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1VoiceInputManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/Level1VoiceInputManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Level1/SherpaEditorTtsProvider.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Level1/SherpaEditorTtsProvider.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/MainMenu/MainMenuVRLaserPointer.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/MainMenu/MainMenuVRLaserPointer.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/NPCInteractionVR.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/NPCInteractionVR.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuController.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuController.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SceneSkipTester.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SceneSkipTester.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/ScreenFader.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/ScreenFader.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/BagReceiver.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/BagReceiver.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/HandBagAnimation.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/HandBagAnimation.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/ScooperFill.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/ScooperFill.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/ScooperToggle.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/ScooperToggle.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/SpiceTutorialManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/SpiceInteraction/SpiceTutorialManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/SpiceCostGuideController.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/SpiceCostGuideController.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/SpiceIntroSequence.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/SpiceIntroSequence.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/TutorialDialogueUIHelper.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Tutorial/TutorialDialogueUIHelper.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/TutorialManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/TutorialManager.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/VoiceRecognitionManager.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/VoiceRecognitionManager.cs)

Added:

- [unity/StorytellingVR/Assets/_Project/Resources.meta](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Resources.meta)
- [unity/StorytellingVR/Assets/_Project/Resources/GlobalPauseMenu.prefab](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Resources/GlobalPauseMenu.prefab)
- [unity/StorytellingVR/Assets/_Project/Resources/GlobalPauseMenu.prefab.meta](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Resources/GlobalPauseMenu.prefab.meta)
- [unity/StorytellingVR/Assets/_Project/Resources/PauseMenuRay.mat](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Resources/PauseMenuRay.mat)
- [unity/StorytellingVR/Assets/_Project/Resources/PauseMenuRay.mat.meta](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Resources/PauseMenuRay.mat.meta)
- [unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuView.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuView.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuView.cs.meta](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/PauseMenuView.cs.meta)
- [unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalPauseStage2Tests.cs](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalPauseStage2Tests.cs)
- [unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalPauseStage2Tests.cs.meta](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/unity/StorytellingVR/Assets/_Project/Scripts/Editor/GlobalPauseStage2Tests.cs.meta)
- [docs/GLOBAL_PAUSE_STAGE2.md](G:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/docs/GLOBAL_PAUSE_STAGE2.md)

Existing/concurrent changes to NPCWalker, its tests, fonts, OpenXR/Oculus settings, build settings, time settings and URP settings were left untouched by this implementation. The existing Libertinus and Liberation Sans font assets are referenced, not edited.

**2. Architecture implemented**

Bootstrap retains the persistent GameManager and its authored PauseMenuController component. GameManager explicitly enables that component; the lazy/direct-Editor fallback enables the same controller type with the same complete scene list. The Stage 1 singleton and independent Level1GameState root are retained.

PauseMenuController owns the single pause state, input polling, reversible component snapshots, camera/rig binding, transition cleanup and one lazily instantiated Resources/GlobalPauseMenu prefab. PauseMenuView owns presentation, confirmations and menu-only hit testing. It does not own a second pause state. Nothing is copied into gameplay scenes.

Bootstrap, MainMenu_Optimized, noncanonical scenes and an active GameManager transition commit reject pause. Scene loading rebinds typed camera/OVRCameraRig references and clears outgoing component references. Losing the menu restores gameplay; invalid menu construction rolls back clock/audio changes.

**3. Keyboard F10 implementation**

UNITY_EDITOR polls F10 as the primary control, with Escape secondary. (F10 replaced P so typing in the chat input field cannot open the menu.) A shared fresh-press latch requires release before another toggle. The desktop view is a responsive screen-space overlay, including when no camera/headset is available. Click the Game view before pressing F10. Cursor visibility/lock are saved and restored. No project input mappings were changed.

**4. Quest left Menu implementation**

The controller reads OVRInput.Button.Start explicitly on OVRInput.Controller.LTouch. The right system/Home button is never queried for pause. Quest uses the headset view; Editor with an attached headset also supports the Quest input path.

**5. Mouse and controller-ray interaction**

Desktop mouse positions hit only the active pause buttons' RectTransforms; a fresh left mouse press selects. Quest uses OVRCameraRig.rightControllerAnchor, a small unlit ray and right index trigger. Trigger hysteresis requires a press at 0.75 followed by release below 0.15 before another selection. Opening while a trigger is held also requires release first.

Hit testing uses the actual button geometry rather than physics raycasts, so it updates while timeScale is zero. The ray cannot select world targets. Existing Meta interactors, locomotion connections/broadcasters/controllers, scene laser pointers and EventSystem input modules are suspended with their previous enabled values recorded. Locomotion connections are disconnected before cancelling interactor selection because the installed SDK can fire snap-turn events on unselect. Head/controller tracking components remain active.

Resume restores presentation/time/audio immediately and fences gameplay input for at least 0.15 unscaled seconds and until both controllers' buttons, grips, triggers and sticks are neutral. This prevents the selecting trigger or mouse click from activating gameplay. No EventSystem, second XR framework or scene input module is added.

**6. Actual authored UI**

Resources/GlobalPauseMenu.prefab contains the complete Canvas, TMP typography, five buttons, divider/border surfaces, main view and confirmation view. PauseMenuRay.mat explicitly references the installed URP unlit shader for build inclusion.

The panel combines ivory #E9DFC9, charcoal #25231F, restrained antique brass #9A783E and muted sandstone. Libertinus Serif provides the title, with Liberation Sans for controls. The title is 64 design units, buttons 40, body 36 and chapter/footer 28/26. Main buttons are 760 x 100 design units with 25-unit gaps. Dark controls keep light text; secondary controls keep dark text. Hover changes the surface and adds a brass outline.

VR size is approximately 0.85 x 0.65 metres. It opens 1.4 metres ahead along horizontal opening gaze, 5 cm below eye level, then stays fixed. A sphere cast shortens the distance in front of an obstruction, down to 0.65 metres when needed; it excludes the rig and its own menu. Reopening recentres it. The 0.12-second fade uses unscaled time. There is no blur, heavy shader, continuous head following, Settings or Quit button.

Restart Scene and Return to Main Menu both require confirmation. Cancel returns to the main pause view; confirmation actions are consumed once. Resume is available by button or the same pause toggle, including from a confirmation view.

**7. Canonical scene compatibility**

All rows share the global locomotion/grab/world-input, scaled physics, animation and Unity audio suspension.

| Scene | Pause-specific integration |
|---|---|
| ModernStudyRoom | Notebook selection and debug controls are guarded; scripted notebook/lamp/cutscene continuations wait; haptics stop. |
| GateIntro | The automatic Start coroutine, narration gaps and look-around sequence wait. Head motion while reading the menu is excluded by rebasing the look objective on Resume. |
| FreeRoam_WithTerrain | Story/Meera/merchant/NPC conversations and narration continuations wait; ray questions, item inspection and scene-entry controls are guarded. Teleportation is blocked globally. |
| NewSpiceScene | Spice introduction and nested tutorial dialogue wait; the X cost-guide input is guarded. |
| NewTransactionTutorial | Tutorial dialogue and offers wait; its Voice SDK request is aborted and invalidated. An already reviewed offer remains available. |
| NewCoinScene | Coin pickup, inspection rotation and next-coin trigger are guarded; coin movement/narration sequences wait. |
| SpicesInteraction | Scooping and receiver callbacks are guarded; tutorial dialogue and bag animation wait. Filled/accepted local orders are not reset by pause. |
| Level1_MainLoopUpdated | Market/customer coroutines wait; scaled patience and day timers stop; chat input, voice advancement, delivery and completion entry points are guarded. Dialogue/LLM presentation deadlines exclude paused wall time. |

The disabled legacy FreeRoamIntroFlow is unchanged. Background NPC steering code is unchanged; scaled simulation is paused globally.

**8. Story, voice and transaction protection**

The shared RunGameplay coroutine gate preserves existing iterators and nested continuations. Realtime waits/typing in active narration use pause-aware unscaled timing. Components such as ChatManager remain enabled; their destructive OnDisable cleanup is not used for pause.

Unity AudioListener.pause, source ignoreListenerPause values and unscaled Animator modes are snapshotted. Unity audio and its DSP scheduling remain suspended; source/animator modes are restored exactly. Download/synthesis continuations wait before playback. Deferred TTS presentation callbacks retain the original interaction/turn identity and reject obsolete conversations.

Level 1 recording or microphone permission work is cancelled using the existing voice token lifecycle, ending the microphone without trimming, transcribing or submitting the incomplete recording. Already-started recognition can finish externally, but publication waits for Resume and still checks the current token. Existing Review text remains intact. Tutorial Voice SDK activation uses request-local events with a generation check; pause aborts/invalidate its incomplete request, so a late result cannot be applied to a later capture. Automatic listening is rearmed only after the pause/input fence ends.

Pause itself never resets pending fulfillment, accepted terms, inventory, money or reputation. Delivery/completion guards prevent menu input from settling an order. The existing one-time settlement claim and calculations are unchanged.

Meta disabling interactors cancels physical grabs. Held rigidbodies are released at their current position and throw velocities are cleared through the input-release fence; the player must regrab them. Logical order/scoop progress is retained.

**9. Restart, Return and persistence**

Both buttons call GameManager APIs and its established LoadRoutine, not arbitrary SceneManager calls from UI. Unavailable destinations are rejected before local work, saves or progression are changed.

The transition releases this controller's time/audio pause, blocks gameplay input, cancels voice work, invalidates conversation presentation and clears unfinished local fulfillment. The existing committed profile is saved, then reloaded to discard unfinished in-memory trade state and stop the market clock. Restart reloads the active canonical scene and keeps its progression index. Return uses MainMenu_Optimized and retains the saved gameplay destination for Continue. Neither action calls New Journey or deletes a save.

A verified initialization correction now restores saved remaining market time when Level1GameState first loads its profile, without starting a timer. Previously an early tutorial access could leave the default in-memory duration and a later save could overwrite the saved remainder. This uses the existing reload helper and changes neither the save schema nor settlement rules.

Bootstrap's Stage 1 adopted FadeCanvas remains owned by GameManager and is reused. Fade appearance/speed and scaled timing remain intact; during a transition with a preexisting zero timeScale, the fade uses unscaled delta to avoid a stall. A destroyed fade image ends the fade safely. A direct Editor fallback that never visited Bootstrap can transition safely without an authored fade overlay, as in Stage 1.

The destination releases this pause system and rebinds its rig. Prior external timeScale/audio conditions are respected, including a preexisting zero timeScale or already paused listener.

**10. Exact tests to run manually**

- GlobalLifecycleStage1Tests: all 11 ownership/fader tests retained, with the activation/readiness expectation updated.
- GlobalPauseStage2Tests: 23 parameter-expanded EditMode cases covering repeated state restoration (including non-default/zero values), fresh digital/analog presses, scene eligibility, unique owner/view, confirmation/cancel, ray geometry at zero timeScale, exact component/audio/animator restoration, story/head-look suspension, transition cleanup, incomplete/late/reviewed voice work, deferred presentation identity, committed save boundaries, unavailable routes, destroyed fader images and one-time accepted-order settlement.
- Existing Level1ConversationLifecycleTests and Level1TransactionIntegrityTests are useful additional regression checks; their files/assertions are unchanged.

No tests have been executed. Fixtures isolate saves in a temporary directory and avoid scene loads, microphones and network requests. Run outside Play Mode with no live game singletons.

**11. Inspector steps and focused manual checks**

No Inspector wiring is required. Bootstrap's existing ownership flag is serialized on, all new prefab references/metas are authored, and rigs/cameras rebind by component type. Do not add scene-local pause canvases/controllers.

1. Import the project yourself, inspect Console compilation and run the two global EditMode classes. Keep your real save backed up when manually checking restart/return; the test fixtures use isolated saves.
2. From Bootstrap, confirm no pause in Bootstrap/Main Menu. Start the journey, focus Game view, press F10 repeatedly and use the mouse for Resume, Restart/Cancel and Return/Cancel. Confirm one persistent controller/view across scenes.
3. Enter each canonical scene directly in Editor and repeat F10/mouse testing. Check dialogue typing/audio and story cutscenes before/after a long pause; check the Gate look objective does not count menu head motion.
4. In Level 1, pause during recording, recognition, Review, narration, accepted-but-unfilled orders and immediately after settlement. Resume, complete an order once, then Restart/Return and Continue. Compare committed money, reputation, inventory, saved chapter and remaining day time.
5. On Quest, verify left Menu and right-trigger selection, holding/jittered triggers, both sticks/buttons held through Resume, fresh regrab, teleport cancellation, eye/controller tracking, comfort, wall placement and ray alignment. Repeat after a scene transition and Main Menu return.

**12. Genuine limitations and remaining validation**

- Unity import/compilation, rendered appearance, tests and builds are unverified. Static source delimiter checks, prefab internal references, external GUID/font fileIDs and the script whitespace diff were inspected; these do not establish runtime correctness.
- Quest input, tracking, ray alignment, SDK cancellation behavior, grab release and panel comfort require hardware validation. Obstruction avoidance is a placement heuristic; dense geometry or physically walking into the fixed panel can still clip it. Close/reopen recentres it.
- Android native TTS has no mid-utterance pause/resume API here. If that existing provider is used, its utterance is stopped and not replayed. Subtitles/state remain available; exact spoken continuity is not claimed.
- The canonical Level 1 scene currently assigns SherpaEditorTtsProvider, which remains Editor-only. This stage does not switch voice providers or fix its existing lack of Quest TTS playback. Recorded Unity narration still uses Unity audio pause/resume.

Implementation stops at Stage 2 pending your manual results.
