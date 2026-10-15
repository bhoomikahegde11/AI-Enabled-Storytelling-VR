using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Inactive, isolated fixtures: no scene loading, microphones, network requests or gameplay loops.
public class GlobalPauseStage2Tests
{
    private readonly List<GameObject> hosts = new List<GameObject>();
    private GameManager manager;
    private PauseMenuController pause;
    private string saveDirectory, previousSaveDirectory;
    private float previousScale;
    private bool previousAudio;
    private bool ownsFixture;

    [SetUp]
    public void SetUp()
    {
        ownsFixture = false;
        Assert.IsFalse(Application.isPlaying);
        Assert.IsTrue(GameManager.ExistingInstance == null && Level1GameState.ExistingInstance == null);
        Assert.IsNull(Field(typeof(PauseMenuController), "instance", true).GetValue(null));
        Assert.IsNull(OrderManager.Instance, "Do not replace a live order manager.");
        previousScale = Time.timeScale;
        previousAudio = AudioListener.pause;
        previousSaveDirectory = LocalSaveManager.EditorTestSaveDirectoryOverride;
        saveDirectory = Path.Combine(Path.GetTempPath(), "GlobalPauseStage2_" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = saveDirectory;
        ownsFixture = true;
        manager = Host("global pause fixture").AddComponent<GameManager>();
        Invoke(manager, "Awake");
        pause = manager.GetComponent<PauseMenuController>();
    }

    [TearDown]
    public void TearDown()
    {
        if (!ownsFixture) return;
        if (pause != null) Invoke(pause, "OnDestroy");
        if (manager != null) Invoke(manager, "OnDestroy");
        if (Level1GameState.ExistingInstance != null) Invoke(Level1GameState.ExistingInstance, "OnDestroy");
        foreach (var host in hosts) if (host != null) Object.DestroyImmediate(host);
        hosts.Clear();
        OrderManager.Instance = null;
        Time.timeScale = previousScale;
        AudioListener.pause = previousAudio;
        LocalSaveManager.EditorTestSaveDirectoryOverride = previousSaveDirectory;
        if (Directory.Exists(saveDirectory)) Directory.Delete(saveDirectory, true);
    }

    [TestCase(0.6f, false)]
    [TestCase(0.35f, true)]
    [TestCase(0f, true)]
    public void RepeatedPauseResumeRestoresPreviousClockAudioAndCursor(float scale, bool audio)
    {
        Time.timeScale = scale;
        AudioListener.pause = audio;
        var cursorLock = Cursor.lockState;
        bool cursorVisible = Cursor.visible;
        for (int cycle = 0; cycle < 4; cycle++)
        {
            Invoke(pause, "CapturePauseState");
            Assert.IsTrue(pause.IsPaused && PauseMenuController.GameplayInputBlocked);
            Assert.AreEqual(0f, Time.timeScale);
            Assert.IsTrue(AudioListener.pause);
            Assert.AreEqual(0f, PauseMenuController.GameplayUnscaledDeltaTime);
            pause.ResumeGameplay();
            pause.ResumeGameplay(); // A duplicate resume must not overwrite the saved state.
            Assert.IsFalse(pause.IsPaused);
            Assert.AreEqual(scale, Time.timeScale);
            Assert.AreEqual(audio, AudioListener.pause);
            Assert.AreEqual(cursorLock, Cursor.lockState);
            Assert.AreEqual(cursorVisible, Cursor.visible);
            Assert.IsTrue(PauseMenuController.GameplayInputBlocked, "Keep the selecting input fenced until release.");
            Invoke(pause, "CloseImmediately"); // EditMode has no input frames to drain the release fence.
            Assert.IsFalse(PauseMenuController.GameplayInputBlocked);
        }
    }

    [Test]
    public void ToggleRequiresReleaseBeforeAnotherPress()
    {
        bool held = false;
        Assert.IsFalse(PauseMenuController.FreshPress(false, ref held));
        Assert.IsTrue(PauseMenuController.FreshPress(true, ref held));
        for (int frame = 0; frame < 10; frame++) Assert.IsFalse(PauseMenuController.FreshPress(true, ref held));
        Assert.IsFalse(PauseMenuController.FreshPress(false, ref held));
        Assert.IsTrue(PauseMenuController.FreshPress(true, ref held));
    }

    [Test]
    public void SelectionTriggerRequiresNeutralReleaseDespiteThresholdJitter()
    {
        bool held = false;
        Assert.IsTrue(PauseMenuController.FreshTriggerPress(0.8f, ref held));
        Assert.IsFalse(PauseMenuController.FreshTriggerPress(0.5f, ref held));
        Assert.IsFalse(PauseMenuController.FreshTriggerPress(0.76f, ref held));
        Assert.IsFalse(PauseMenuController.FreshTriggerPress(0.14f, ref held));
        Assert.IsTrue(PauseMenuController.FreshTriggerPress(0.8f, ref held));
    }

    [Test]
    public void BootstrapMainMenuAndTransitionCommitRejectPause()
    {
        Assert.IsFalse((bool)Invoke(pause, "CanPauseInScene", "Bootstrap"));
        Assert.IsFalse((bool)Invoke(pause, "CanPauseInScene", "MainMenu_Optimized"));
        foreach (string scene in new[] { "ModernStudyRoom", "GateIntro", "FreeRoam_WithTerrain", "NewSpiceScene",
            "NewTransactionTutorial", "NewCoinScene", "SpicesInteraction", "Level1_MainLoopUpdated" })
            Assert.IsTrue((bool)Invoke(pause, "CanPauseInScene", scene), scene);
        Field(typeof(GameManager), "isLoading").SetValue(manager, true);
        Assert.IsFalse((bool)Invoke(pause, "CanPauseInScene", "ModernStudyRoom"));
        Assert.IsTrue(PauseMenuController.GameplayInputBlocked);
    }

    [Test]
    public void ExistingOwnerCreatesOnlyOneReusableAuthoredView()
    {
        Invoke(manager, "EnsurePersistentControllers");
        Assert.AreEqual(1, manager.GetComponents<PauseMenuController>().Length);
        Assert.IsTrue((bool)Invoke(pause, "EnsureView"));
        var first = manager.GetComponentInChildren<PauseMenuView>(true);
        Assert.IsTrue(first.IsValid);
        Assert.IsFalse(first.gameObject.activeSelf);
        Assert.IsTrue((bool)Invoke(pause, "EnsureView"));
        Assert.AreSame(first, manager.GetComponentInChildren<PauseMenuView>(true));
        Assert.AreEqual(1, manager.GetComponentsInChildren<PauseMenuView>(true).Length);
        Assert.AreEqual(0, first.GetComponentsInChildren<PauseMenuController>(true).Length);
        Assert.AreEqual(0, first.GetComponentsInChildren<EventSystem>(true).Length);
        foreach (var button in new[] { first.resumeButton, first.restartButton, first.returnButton,
            first.confirmButton, first.cancelButton })
        {
            var image = button.targetGraphic as Image;
            Assert.IsNotNull(image);
            Assert.IsNotNull(image.sprite, "All pause actions need the shared rounded sprite.");
            Assert.AreEqual(Image.Type.Sliced, image.type);
        }
    }

    [Test]
    public void PausedWorldEffectsStayBehindCardAndFadeWithTheView()
    {
        var view = Menu();
        Assert.AreSame(view.worldScrim.transform, view.transform.GetChild(0));
        Assert.AreSame(view.backlightHalo.transform, view.transform.GetChild(1));
        Assert.IsFalse(view.worldScrim.raycastTarget);
        Assert.IsFalse(view.backlightHalo.raycastTarget);
        Assert.IsNotNull(view.backlightHalo.sprite);
        Assert.That(view.worldScrim.color.a, Is.InRange(0.25f, 0.35f));

        view.ConfigureCamera(null, true);
        var scrimRect = (RectTransform)view.worldScrim.transform;
        Assert.AreEqual(Vector2.zero, scrimRect.anchorMin);
        Assert.AreEqual(Vector2.one, scrimRect.anchorMax);
        Assert.AreEqual(Vector2.zero, scrimRect.sizeDelta);
        view.Bind(() => { }, () => { }, () => { });
        view.Open("ModernStudyRoom", true);
        view.Tick(true, 0.06f);
        Assert.That(view.canvasGroup.alpha, Is.InRange(0.4f, 0.6f));
        view.Tick(false, 0.12f);
        Assert.IsFalse(view.gameObject.activeSelf);
        Assert.AreEqual(0f, view.canvasGroup.alpha);

        var camera = Host("Quest camera geometry fixture").AddComponent<Camera>();
        view.ConfigureCamera(camera, false);
        Assert.AreEqual(RenderMode.WorldSpace, view.canvas.renderMode);
        Assert.AreEqual(new Vector2(0.5f, 0.5f), scrimRect.anchorMin);
        Assert.AreEqual(scrimRect.anchorMin, scrimRect.anchorMax);
        Assert.Greater(scrimRect.rect.width, ((RectTransform)view.transform).rect.width);
        Assert.Greater(scrimRect.rect.height, ((RectTransform)view.transform).rect.height);
    }

    [Test]
    public void AuthoredConfirmationsRequireConfirmationAndCancelNeverCommits()
    {
        var view = Menu();
        view.ConfigureCamera(null, true);
        Assert.AreEqual(RenderMode.ScreenSpaceOverlay, view.canvas.renderMode, "Desktop interaction needs no headset or camera.");
        int resumes = 0, restarts = 0, returns = 0;
        view.Bind(() => resumes++, () => restarts++, () => returns++);
        view.Open("GateIntro", true);
        view.restartButton.onClick.Invoke();
        Assert.IsTrue(view.confirmationView.activeSelf);
        Assert.AreEqual(0, restarts);
        view.cancelButton.onClick.Invoke();
        Assert.IsTrue(view.mainView.activeSelf);
        Assert.AreEqual(0, restarts);
        view.restartButton.onClick.Invoke();
        view.confirmButton.onClick.Invoke();
        view.confirmButton.onClick.Invoke();
        Assert.AreEqual(1, restarts, "Consume a confirmation once, even if the callback is repeated.");
        view.Open("GateIntro", true);
        view.returnButton.onClick.Invoke();
        Assert.AreEqual("Return to the Main Menu?", view.confirmationTitle.text);
        Assert.AreEqual("Cancel", view.cancelButton.GetComponentInChildren<TMP_Text>(true).text);
        Assert.AreEqual(0, returns);
        view.confirmButton.onClick.Invoke();
        Assert.AreEqual(1, returns);
        var state = State();
        view.Open("Level1_MainLoopUpdated", true);
        view.returnButton.onClick.Invoke();
        Assert.AreEqual("Return to the Main Menu?", view.confirmationTitle.text);
        Assert.AreEqual("Cancel", view.cancelButton.GetComponentInChildren<TMP_Text>(true).text);
        view.cancelButton.onClick.Invoke();
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        view.returnButton.onClick.Invoke();
        Assert.AreEqual("Leave the market?", view.confirmationTitle.text);
        Assert.AreEqual("Your current customer and any unfinished deal will be abandoned.\nCompleted trades, money, and reputation will remain saved.", view.confirmationBody.text);
        Assert.AreEqual("Return to Main Menu", view.confirmButton.GetComponentInChildren<TMP_Text>(true).text);
        Assert.AreEqual("Stay in Market", view.cancelButton.GetComponentInChildren<TMP_Text>(true).text);
        view.cancelButton.onClick.Invoke();
        Assert.AreEqual(1, returns);
        view.Open("GateIntro", true);
        view.resumeButton.onClick.Invoke();
        Assert.AreEqual(1, resumes);
    }

    [Test]
    public void PressFeedbackUsesUnscaledTimeAndCancelsOnClose()
    {
        var view = Menu();
        view.transform.parent.gameObject.SetActive(true);
        int resumes = 0;
        view.Bind(() => resumes++, () => { }, () => { });
        view.Open("GateIntro", true);
        view.Hover(view.resumeButton);
        Assert.AreEqual(view.resumeButton.colors.highlightedColor, view.resumeButton.targetGraphic.color);
        view.Press(view.resumeButton);
        view.Press(view.resumeButton);
        Assert.AreEqual(view.resumeButton.colors.pressedColor, view.resumeButton.targetGraphic.color);
        view.Tick(true, 0.05f);
        Assert.AreEqual(0, resumes);
        Assert.AreEqual(view.resumeButton.colors.pressedColor, view.resumeButton.targetGraphic.color);
        view.Tick(true, 0.05f);
        Assert.AreEqual(1, resumes);
        view.Tick(true, 0.2f);
        Assert.AreEqual(1, resumes, "One press invokes its action only once.");
        view.Press(view.resumeButton);
        view.HideImmediately();
        Assert.AreEqual(view.resumeButton.colors.normalColor, view.resumeButton.targetGraphic.color);
        view.Tick(true, 0.2f);
        Assert.AreEqual(1, resumes, "Closing the view cancels pending selection.");
    }

    [Test]
    public void RayHitsOnlyActiveMenuButtonsWithoutPhysicsOrAnEventSystem()
    {
        var view = Menu();
        view.Bind(() => { }, () => { }, () => { });
        view.Open("NewCoinScene", false);
        view.transform.parent.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
        Time.timeScale = 0f;
        var button = view.resumeButton;
        Vector3 center = button.transform.TransformPoint(((RectTransform)button.transform).rect.center);
        var ray = new Ray(center - view.transform.forward, view.transform.forward);
        Assert.AreSame(button, view.HitRay(ray, out _));
        view.restartButton.onClick.Invoke();
        Assert.IsNull(view.HitRay(ray, out _), "Hidden main buttons cannot consume confirmation input.");
        Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/Resources/PauseMenuRay.mat"));
    }

    [Test]
    public void ComponentRestoreKeepsInitiallyDisabledComponentsDisabled()
    {
        var enabledModule = Host("enabled input module").AddComponent<StandaloneInputModule>();
        var disabledModule = Host("disabled input module").AddComponent<StandaloneInputModule>();
        disabledModule.enabled = false;
        Invoke(pause, "SuspendBehaviour", enabledModule);
        Invoke(pause, "SuspendBehaviour", enabledModule); // Do not replace the original snapshot with false.
        Invoke(pause, "SuspendBehaviour", disabledModule);
        Assert.IsFalse(enabledModule.enabled || disabledModule.enabled);
        Invoke(pause, "RestoreSceneComponents");
        Assert.IsTrue(enabledModule.enabled);
        Assert.IsFalse(disabledModule.enabled);
    }

    [Test]
    public void ResumeRestoresUnscaledAnimatorAndListenerBypassModes()
    {
        var animator = Host("unscaled animator").AddComponent<Animator>();
        var audio = Host("listener bypass source").AddComponent<AudioSource>();
        var modes = (Dictionary<Animator, AnimatorUpdateMode>)Field(typeof(PauseMenuController), "animatorModes").GetValue(pause);
        var sources = (Dictionary<AudioSource, bool>)Field(typeof(PauseMenuController), "audioModes").GetValue(pause);
        modes.Add(animator, AnimatorUpdateMode.UnscaledTime);
        sources.Add(audio, true);
        animator.updateMode = AnimatorUpdateMode.Normal;
        audio.ignoreListenerPause = false;
        Invoke(pause, "CapturePauseState");
        pause.ResumeGameplay();
        Assert.AreEqual(AnimatorUpdateMode.UnscaledTime, animator.updateMode);
        Assert.IsTrue(audio.ignoreListenerPause);
        Assert.AreEqual(0, modes.Count + sources.Count);
    }

    [Test]
    public void StoryContinuationWaitsWhilePausedAndRunsOnceAfterResume()
    {
        int events = 0;
        IEnumerator Story() { events++; yield return null; events++; }
        var routine = PauseMenuController.RunGameplay(Story());
        Assert.IsTrue(routine.MoveNext());
        Assert.AreEqual(1, events);
        Invoke(pause, "CapturePauseState");
        for (int frame = 0; frame < 8; frame++) Assert.IsTrue(routine.MoveNext());
        Assert.AreEqual(1, events);
        pause.ResumeGameplay();
        Assert.IsFalse(routine.MoveNext());
        Assert.AreEqual(2, events);
    }

    [Test]
    public void GateLookAroundDoesNotCountHeadMovementMadeWhileReadingMenu()
    {
        var gate = Host("gate introduction").AddComponent<GateIntroFlow>();
        var head = Host("head pose").transform;
        Field(typeof(GateIntroFlow), "playerHeadCamera").SetValue(gate, head);
        var routine = PauseMenuController.RunGameplay((IEnumerator)Invoke(gate, "WaitForPlayerLookAround"));
        Assert.IsTrue(routine.MoveNext());
        Invoke(pause, "CapturePauseState");
        head.rotation = Quaternion.Euler(0f, 60f, 0f);
        Assert.IsTrue(routine.MoveNext());
        pause.ResumeGameplay();
        Assert.IsTrue(routine.MoveNext());
        Assert.IsTrue(routine.MoveNext(), "Menu reading must not finish the look-around objective on Resume.");
        head.rotation = Quaternion.Euler(0f, 100f, 0f);
        Assert.IsTrue(routine.MoveNext());
        Assert.IsFalse(routine.MoveNext());
    }

    [Test]
    public void SceneUnloadOrControllerDestructionCannotLeaveClockPaused()
    {
        Time.timeScale = 0.7f;
        AudioListener.pause = false;
        Invoke(pause, "CapturePauseState");
        Invoke(pause, "HandleSceneLoaded", SceneManager.GetActiveScene(), LoadSceneMode.Single);
        Assert.AreEqual(0.7f, Time.timeScale);
        Assert.IsFalse(AudioListener.pause || pause.IsPaused);
        Invoke(pause, "CloseImmediately");
        Invoke(pause, "CapturePauseState");
        Invoke(pause, "PrepareForTransition");
        Assert.IsTrue(PauseMenuController.GameplaySuspended);
        Assert.AreEqual(0.7f, Time.timeScale);
        Invoke(pause, "FinishTransition");
        Assert.IsFalse(PauseMenuController.GameplaySuspended);
        Invoke(pause, "CloseImmediately");
        Invoke(pause, "CapturePauseState");
        Invoke(pause, "OnDestroy");
        Assert.AreEqual(0.7f, Time.timeScale);
        Assert.IsFalse(AudioListener.pause || PauseMenuController.GameplayInputBlocked);
    }

    [Test]
    public void IncompleteCaptureIsInvalidatedWithoutSubmittingOrEndingConversation()
    {
        var chat = Chat();
        var voice = Voice(chat);
        Assert.IsTrue(chat.TryBeginVoiceTurn(out var token));
        Field(typeof(Level1VoiceInputManager), "activeVoiceToken").SetValue(voice, token);
        Field(typeof(Level1VoiceInputManager), "currentState").SetValue(voice, Level1VoiceInputManager.VoiceInputState.Recording);
        int interaction = chat.InteractionId;
        Invoke(pause, "CapturePauseState");
        voice.OnGlobalPause();
        Assert.AreEqual(interaction, chat.InteractionId);
        Assert.IsFalse(chat.IsCurrentVoiceTurn(token));
        Assert.AreEqual(ConversationTurnLifecycle.Phase.WaitingForPlayer, chat.TurnPhase);
        Assert.AreEqual(Level1VoiceInputManager.VoiceInputState.Idle, voice.CurrentState);
        Assert.IsFalse(chat.TryBeginVoiceTurn(out _));
    }

    [Test]
    public void RecognitionAndReviewedTranscriptSurvivePauseButOldCallbacksCannotAdvance()
    {
        var chat = Chat();
        var voice = Voice(chat);
        Assert.IsTrue(chat.TryBeginVoiceTurn(out var token));
        Assert.IsTrue(chat.TryAdvanceVoiceTurn(token, ConversationTurnLifecycle.Phase.Recognizing));
        Field(typeof(Level1VoiceInputManager), "activeVoiceToken").SetValue(voice, token);
        Field(typeof(Level1VoiceInputManager), "currentState").SetValue(voice, Level1VoiceInputManager.VoiceInputState.Recognizing);
        Invoke(pause, "CapturePauseState");
        voice.OnGlobalPause();
        Assert.IsTrue(chat.IsCurrentVoiceTurn(token));
        Assert.IsFalse(chat.TryAdvanceVoiceTurn(token, ConversationTurnLifecycle.Phase.Reviewing));
        pause.ResumeGameplay();
        Assert.IsTrue(chat.TryAdvanceVoiceTurn(token, ConversationTurnLifecycle.Phase.Reviewing));
        Field(typeof(Level1VoiceInputManager), "currentState").SetValue(voice, Level1VoiceInputManager.VoiceInputState.Review);
        voice.inputField.text = "173 varahas";
        Invoke(pause, "CloseImmediately");
        Invoke(pause, "CapturePauseState");
        voice.OnGlobalPause();
        chat.OnSend();
        Assert.AreEqual(ConversationTurnLifecycle.Phase.Reviewing, chat.TurnPhase);
        Assert.AreEqual("173 varahas", voice.inputField.text);
        voice.CancelForSceneExit();
        Assert.IsFalse(chat.IsCurrentVoiceTurn(token));
        Assert.AreEqual(string.Empty, voice.inputField.text);
    }

    [Test]
    public void AbortedTutorialRequestCannotApplyAfterResumeOrReplaceReviewedOffer()
    {
        var voice = Host("tutorial voice fixture").AddComponent<VoiceRecognitionManager>();
        voice.gameObject.SetActive(true); // Exercise generation rejection, not the inactive-component guard.
        Field(typeof(VoiceRecognitionManager), "waitingForInput").SetValue(voice, true);
        Field(typeof(VoiceRecognitionManager), "hasPendingOffer").SetValue(voice, true);
        Field(typeof(VoiceRecognitionManager), "pendingOffer").SetValue(voice, 50);
        int oldGeneration = (int)Field(typeof(VoiceRecognitionManager), "requestGeneration").GetValue(voice);
        Invoke(pause, "CapturePauseState");
        voice.OnGlobalPause();
        pause.ResumeGameplay();
        Invoke(pause, "CloseImmediately");
        Invoke(voice, "ReceiveTranscription", oldGeneration, "999");
        Assert.IsTrue((bool)Field(typeof(VoiceRecognitionManager), "hasPendingOffer").GetValue(voice));
        Assert.AreEqual(50, Field(typeof(VoiceRecognitionManager), "pendingOffer").GetValue(voice));
        Assert.AreEqual(oldGeneration + 1, Field(typeof(VoiceRecognitionManager), "requestGeneration").GetValue(voice));
    }

    [Test]
    public void DeferredPresentationRunsOnceAndRejectsAChangedConversation()
    {
        var chat = Chat();
        Field(typeof(ChatManager), "pendingTtsSubtitleText").SetValue(chat, "A line awaiting playback");
        int presentations = 0;
        Action present = () => presentations++;
        Invoke(pause, "CapturePauseState");
        var pending = (IEnumerator)Invoke(chat, "DeferredPresentation", present, chat.InteractionId, chat.TurnId);
        Assert.IsTrue(pending.MoveNext());
        var wait = (IEnumerator)pending.Current;
        Assert.IsTrue(wait.MoveNext());
        Assert.AreEqual(0, presentations);
        pause.ResumeGameplay();
        Assert.IsFalse(wait.MoveNext());
        Assert.IsFalse(pending.MoveNext());
        Assert.AreEqual(1, presentations);
        var obsolete = (IEnumerator)Invoke(chat, "DeferredPresentation", present, chat.InteractionId, chat.TurnId);
        var lifecycle = (ConversationTurnLifecycle)Field(typeof(ChatManager), "turnLifecycle").GetValue(chat);
        lifecycle.Invalidate(false);
        Assert.IsTrue(obsolete.MoveNext());
        Assert.IsFalse(((IEnumerator)obsolete.Current).MoveNext());
        Assert.IsFalse(obsolete.MoveNext());
        Assert.AreEqual(1, presentations);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void TransitionSaveBoundaryKeepsCommittedProgressAndOnlyAbandonsWhenRequested(bool abandon)
    {
        var saves = new LocalSaveManager();
        var profile = saves.LoadProfile();
        profile.global_metrics.total_varahas = 333;
        profile.global_metrics.reputation = 74;
        profile.inventory[0].grams = 1234;
        profile.current_scene = "Level1_MainLoopUpdated";
        profile.progression_index = 7;
        profile.hasSavedMarketDayTimer = true;
        profile.remainingMarketDaySeconds = 247f;
        saves.SaveProfile(profile);
        var state = State();
        Assert.AreEqual(247f, state.MarketDayRemainingSeconds, "Pre-market initialization must preserve the saved clock too.");
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        var unfinished = state.ActiveTrade;
        Invoke(manager, "PreserveCommittedProgress", abandon);
        Assert.AreEqual(333, state.CurrentMoney);
        Assert.AreEqual(74, state.CurrentReputation);
        Assert.AreEqual(247f, state.MarketDayRemainingSeconds);
        var saved = saves.LoadProfile();
        Assert.AreEqual(1234, saved.inventory[0].grams);
        Assert.AreEqual("Level1_MainLoopUpdated", saved.current_scene);
        Assert.AreEqual(7, saved.progression_index);
        if (abandon) Assert.IsNull(state.ActiveTrade);
        else Assert.AreSame(unfinished, state.ActiveTrade);
    }

    [Test]
    public void UnavailableSceneIsRejectedBeforeSaveProgressionOrPauseStateChanges()
    {
        int index = manager.CurrentProgressionIndex;
        Invoke(pause, "CapturePauseState");
        LogAssert.Expect(LogType.Error, "[SCENE FLOW] Scene is not available in the build scene list: MissingPauseTestScene");
        manager.LoadSceneByName("MissingPauseTestScene", abandonCurrentWork: true);
        Assert.AreEqual(index, manager.CurrentProgressionIndex);
        Assert.IsTrue(pause.IsPaused);
        Assert.IsFalse(manager.IsLoading);
        Assert.IsFalse(LocalSaveManager.ActiveProfileExists());
    }

    [Test]
    public void SharedMarketClockDoesNotAdvanceThroughPausedCoroutineContinuations()
    {
        var state = State();
        Field(typeof(Level1GameState), "<MarketDayStarted>k__BackingField").SetValue(state, true);
        Field(typeof(Level1GameState), "marketDayRemainingSeconds").SetValue(state, 500f);
        var clock = PauseMenuController.RunGameplay((IEnumerator)Invoke(state, "MarketDayTimerRoutine"));
        Invoke(pause, "CapturePauseState");
        Field(typeof(Level1GameState), "marketDayStartedAt").SetValue(state,
            Time.time - (state.MarketDayDurationSeconds - 490f));
        try
        {
            for (int frame = 0; frame < 5; frame++) Assert.IsTrue(clock.MoveNext());
            Assert.AreEqual(500f, state.MarketDayRemainingSeconds);
            Assert.AreEqual(0f, Time.timeScale);
            pause.ResumeGameplay();
            Assert.IsTrue(clock.MoveNext());
            Assert.AreEqual(490f, state.MarketDayRemainingSeconds, 0.01f);
        }
        finally { (clock as IDisposable)?.Dispose(); }
    }

    [Test]
    public void DebugSkipEligibilityDoesNotDependOnOrExpireTheMarketClock()
    {
        var state = State();
        Field(typeof(Level1GameState), "<MarketDayStarted>k__BackingField").SetValue(state, true);
        Field(typeof(Level1GameState), "marketDayRemainingSeconds").SetValue(state, 500f);
        Invoke(manager, "SyncCurrentScene", GameManager.DefaultGameplaySceneName);
        Field(typeof(GameManager), "enableDebugSceneSkip").SetValue(manager, true);
        Assert.IsTrue((bool)Invoke(manager, "CanDebugSkipMainLoop", false));
        Assert.AreEqual(500f, state.MarketDayRemainingSeconds);
        Assert.IsFalse(state.MarketDayEnded);
        state.EndMarketDay();
        Assert.IsTrue((bool)Invoke(manager, "CanDebugSkipMainLoop", false));
        Field(typeof(GameManager), "enableDebugSceneSkip").SetValue(manager, false);
        Assert.IsFalse((bool)Invoke(manager, "CanDebugSkipMainLoop", false));
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TransitionFadeStopsSafelyIfItsImageIsDestroyedMidFade(bool fadingIn)
    {
        var host = Host("transition overlay fixture");
        var fader = host.AddComponent<ScreenFader>();
        fader.fadeImage = host.AddComponent<UnityEngine.UI.Image>();
        fader.fadeImage.color = new Color(0f, 0f, 0f, 0.5f);
        fader.speed = 0f; // Keep the routine in progress regardless of EditMode delta time.
        var fade = fadingIn ? fader.FadeIn() : fader.FadeOut();
        Assert.IsTrue(fade.MoveNext());
        Object.DestroyImmediate(fader.fadeImage);
        Assert.IsFalse(fade.MoveNext());
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TransitionFadeStopsSafelyIfItsImageIsAlreadyMissing(bool fadingIn)
    {
        var fader = Host("missing transition image fixture").AddComponent<ScreenFader>();
        var fade = fadingIn ? fader.FadeIn() : fader.FadeOut();
        Assert.IsFalse(fade.MoveNext());
    }

    [Test]
    public void PauseDuringAcceptedOrderRetainsContractAndSettlementOccursOnlyOnceAfterResume()
    {
        var state = State();
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", 560, 173, null);
        var agreement = TradeTermsSnapshot.Capture(state.ActiveTrade);
        Assert.IsTrue(state.ActiveTrade.BindAcceptedTerms(agreement));
        var order = Host("accepted physical order").AddComponent<OrderManager>();
        OrderManager.Instance = order;
        order.tutorialMode = false;
        Field(typeof(OrderManager), "marketplaceOrder").SetValue(order, new MarketplaceFulfillmentOrder(agreement));
        var chat = Chat();
        Field(typeof(ChatManager), "pendingFulfillment").SetValue(chat, new ChatManager.PendingFulfillmentData
        { sourceTrade = agreement.Source, acceptedTerms = agreement, agreedPrice = 173, quantityGrams = 560, buyerTrust = 0.7f });
        int money = state.CurrentMoney;
        Invoke(pause, "CapturePauseState");
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.AreSame(agreement, chat.CurrentPendingFulfillment.acceptedTerms);
        Assert.AreSame(agreement, order.MarketplaceOrder.Agreement);
        Assert.AreEqual(money, state.CurrentMoney);
        pause.ResumeGameplay();
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.AreEqual(money, state.CurrentMoney, "Release fencing must also block completion.");
        Invoke(pause, "CloseImmediately");
        Assert.IsTrue(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Pepper, true));
        chat.CompleteAcceptedFulfillment(agreement);
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.AreEqual(money + 173, state.CurrentMoney);
        Assert.IsFalse(chat.HasPendingFulfillment);
        Assert.AreEqual(1, SavedProfile(state).shift_stats.total_deals_made);
    }

    private GameObject Host(string name)
    {
        var host = new GameObject(name);
        host.SetActive(false);
        hosts.Add(host);
        return host;
    }

    private Level1GameState State()
    {
        var state = Host("persistent profile").AddComponent<Level1GameState>();
        Invoke(state, "Awake");
        return state;
    }

    private ChatManager Chat()
    {
        var chat = Host("conversation fixture").AddComponent<ChatManager>();
        var lifecycle = (ConversationTurnLifecycle)Field(typeof(ChatManager), "turnLifecycle").GetValue(chat);
        lifecycle.BeginInteraction();
        Assert.IsTrue(lifecycle.WaitForPlayer(lifecycle.InteractionId));
        return chat;
    }

    private Level1VoiceInputManager Voice(ChatManager chat)
    {
        var voice = Host("voice fixture").AddComponent<Level1VoiceInputManager>();
        voice.chatManager = chat;
        voice.inputField = Host("transcript fixture").AddComponent<TMP_InputField>();
        return voice;
    }

    private PauseMenuView Menu()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Resources/GlobalPauseMenu.prefab");
        Assert.IsNotNull(prefab);
        var host = Object.Instantiate(prefab, Host("menu fixture").transform);
        var view = host.GetComponent<PauseMenuView>();
        Assert.IsTrue(view.IsValid);
        return view;
    }

    private static LocalProfileData SavedProfile(Level1GameState state) =>
        (LocalProfileData)Field(typeof(Level1GameState), "profile").GetValue(state);

    private static FieldInfo Field(Type type, string name, bool isStatic = false) =>
        type.GetField(name, BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance));

    private static object Invoke(object target, string method, params object[] args) =>
        target.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, args);
}
