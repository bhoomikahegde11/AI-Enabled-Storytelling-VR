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
using UnityEngine.TestTools;

// Test Framework 1.6 runs these Editor-assembly tests in actual Editor PlayMode via EnterPlayMode.
// Keeping them in the Editor assembly lets the fixture reference the real Assembly-CSharp components.
public class Level1ConversationRuntimeSoakTests
{
    public const int DefaultSoakSeed = 12345;
    private const float MaxWaitGameSeconds = 90f;
    private const float MaxTransitionGameSeconds = 35f;
    private const int MaxFramesWithoutGameTime = 120;
    private const string FixtureName = "Level1ConversationSoakFixture";
    private static readonly FieldInfo LocalDialogueTurnField = typeof(ChatManager).GetField("localDialogueTurnId", BindingFlags.Instance | BindingFlags.NonPublic);
    private SoakFixture fixture;
    private float originalTimeScale;
    private int originalCaptureFramerate;
    private UnityEngine.Random.State originalUnityRandomState;
    private readonly List<string> errors = new List<string>();

    [UnityTest, Explicit("Manual runtime soak: select this test from an empty scene.")]
    public IEnumerator SmokeSoak_50Interactions()
    {
        RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunSoak(DefaultSoakSeed, 50);
    }

    [UnityTest, Explicit("Manual standard soak: select this test from an empty scene.")]
    public IEnumerator StandardSoak_250Interactions()
    {
        RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunSoak(DefaultSoakSeed + 1, 250);
    }

    [UnityTest, Explicit("Manual long-running soak; select this test explicitly.")]
    public IEnumerator HeavySoak_1000Interactions()
    {
        RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunSoak(DefaultSoakSeed + 2, 1000);
    }

    // Replace the seed here with one printed in a failure to replay that schedule.
    [UnityTest, Explicit("Set ReplaySeed to the reported seed, then select this test.")]
    public IEnumerator ReplayOneSeed()
    {
        const int ReplaySeed = DefaultSoakSeed;
        const int ReplayInteractions = 250;
        RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunSoak(ReplaySeed, ReplayInteractions);
    }

    [UnityTest, Explicit("Manual focused regression for clarification followed by both idle reminders.")]
    public IEnumerator ClarificationReminders_DoNotRestartIdleTimeout()
    {
        const int seed = 12346;
        RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunClarificationReminderRegression(seed);
    }

    // Construct the captured wait predicates after EnterPlayMode's domain reload.
    private IEnumerator RunClarificationReminderRegression(int seed)
    {
        originalTimeScale = Time.timeScale;
        originalCaptureFramerate = Time.captureFramerate;
        originalUnityRandomState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(seed);
        Time.captureFramerate = 60;
        Time.timeScale = 20f;
        fixture = new SoakFixture(seed);
        Application.logMessageReceived += CaptureError;
        yield return null;

        fixture.IterationStartFrame = Time.frameCount;
        fixture.IterationStartTime = Time.time;
        fixture.ShowConversationUI();
        fixture.Chat.StartNewSession();
        yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer, "greeting never yielded input");
        LocalTradeState trade = Level1GameState.Instance.ActiveTrade;
        int initialTurn = trade.turnIndex;
        yield return SpeakAndSubmit("the purple moon dances", new System.Random(seed), trade);
        yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer, "clarification never yielded input");
        Assert.AreEqual(initialTurn, trade.turnIndex, fixture.Diagnostics());
        int waitingLogsAfterClarification = fixture.WaitingForPlayerLogs;
        float waitingStartedAt = Time.time;

        yield return WaitUntil(() => fixture.WaitingForPlayerLogs > waitingLogsAfterClarification,
            "first idle reminder never returned to waiting", 30f);
        float idleAnchorAfterReminder = fixture.PlayerIdleStartedAt;
        fixture.Marketplace.ResumePlayerIdleWindowAfterReminder(); // Duplicate completion of the same reminder.
        Assert.AreEqual(idleAnchorAfterReminder, fixture.PlayerIdleStartedAt, fixture.Diagnostics());
        yield return WaitUntil(() => fixture.Marketplace.IsTransitioning, "Marketplace idle NO DEAL never resolved");
        Assert.GreaterOrEqual(fixture.WaitingForPlayerLogs, waitingLogsAfterClarification + 2,
            "Both reminder presentations should return to the same player-idle window. " + fixture.Diagnostics());
        Assert.AreEqual(1, fixture.IdleExpiryLogs, fixture.Diagnostics());
        Assert.AreEqual(1, fixture.CustomerLeavingLogs, fixture.Diagnostics());
        Assert.Less(Time.time - waitingStartedAt, fixture.WalkAwaySeconds + 10f, fixture.Diagnostics());
        Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
        Assert.IsNull(Level1GameState.Instance.ActiveTrade, fixture.Diagnostics());

        int moneyAfter = Level1GameState.Instance.CurrentMoney;
        int reputationAfter = Level1GameState.Instance.CurrentReputation;
        Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
        fixture.Marketplace.OnNegotiationFinished(false);
        yield return DelayFrames(2);
        Assert.AreEqual(1, fixture.IdleExpiryLogs, fixture.Diagnostics());
        Assert.AreEqual(1, fixture.CustomerLeavingLogs, fixture.Diagnostics());
        Assert.AreEqual(moneyAfter, Level1GameState.Instance.CurrentMoney, fixture.Diagnostics());
        Assert.AreEqual(reputationAfter, Level1GameState.Instance.CurrentReputation, fixture.Diagnostics());
        yield return WaitUntil(() => !fixture.Marketplace.IsTransitioning, "customer departure stuck", MaxTransitionGameSeconds);
    }

    private static void RequireEmptyEditorScene()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var roots = scene.IsValid() && scene.isLoaded ? scene.GetRootGameObjects() : Array.Empty<GameObject>();
        int cameraCount = 0;
        int lightCount = 0;
        var descriptions = new List<string>();
        bool hasUnexpectedRoot = false;

        foreach (var root in roots)
        {
            var components = root.GetComponents<Component>();
            var types = new List<string>();
            foreach (var component in components)
                types.Add(component == null ? "<missing script>" : component.GetType().FullName);
            descriptions.Add($"{root.name} [{string.Join(", ", types)}], children={root.transform.childCount}");

            if (IsDefaultSceneRoot(root, components, true) && cameraCount++ == 0)
                continue;
            if (IsDefaultSceneRoot(root, components, false) && lightCount++ == 0)
                continue;
            hasUnexpectedRoot = true;
        }

        Assert.IsTrue(scene.IsValid() && scene.isLoaded && string.IsNullOrEmpty(scene.path) && !hasUnexpectedRoot,
            $"Run the runtime soak from an unsaved disposable scene with no gameplay objects. " +
            $"Active scene: '{scene.name}', path: '{scene.path}', roots: {string.Join("; ", descriptions)}");
    }

    private static bool IsDefaultSceneRoot(GameObject root, Component[] components, bool camera)
    {
        if (root.name != (camera ? "Main Camera" : "Directional Light") || root.transform.childCount != 0)
            return false;
        if (camera ? root.GetComponent<Camera>() == null || root.GetComponent<AudioListener>() == null
                   : root.GetComponent<Light>() == null || root.GetComponent<Light>().type != LightType.Directional)
            return false;

        foreach (var component in components)
        {
            if (component == null)
                return false;
            var type = component.GetType();
            if (type == typeof(Transform) || (camera && (type == typeof(Camera) || type == typeof(AudioListener))) ||
                (!camera && type == typeof(Light)) ||
                type.FullName == (camera ? "UnityEngine.Rendering.Universal.UniversalAdditionalCameraData"
                                        : "UnityEngine.Rendering.Universal.UniversalAdditionalLightData"))
                continue;
            return false;
        }
        return true;
    }

    private IEnumerator RunSoak(int seed, int interactions)
    {
        Assert.IsTrue(EditorApplication.isPlaying, "The runtime soak must be inside Editor PlayMode.");
        originalTimeScale = Time.timeScale;
        originalCaptureFramerate = Time.captureFramerate;
        originalUnityRandomState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(seed);
        Time.captureFramerate = 60; // Fixed simulated time per frame makes the seed replayable.
        Time.timeScale = 20f;
        var random = new System.Random(seed);
        int[] scenarios = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        fixture = new SoakFixture(seed);
        Application.logMessageReceived += CaptureError;
        yield return null; // Run Start() on the minimal production components.

        for (int index = 0; index < interactions; index++)
        {
            if (index % scenarios.Length == 0)
            {
                for (int slot = scenarios.Length - 1; slot > 0; slot--)
                {
                    int other = random.Next(slot + 1);
                    (scenarios[slot], scenarios[other]) = (scenarios[other], scenarios[slot]);
                }
            }
            fixture.Iteration = index;
            fixture.IterationStartFrame = Time.frameCount;
            fixture.IterationStartTime = Time.time;
            fixture.LastSelectedNextCustomerGap = "not logged";
            fixture.Tts.NextDelayFrames = random.Next(1, 7);
            fixture.Note("new customer");
            Assert.IsFalse(fixture.Marketplace.IsTransitioning, fixture.Diagnostics());
            fixture.ShowConversationUI();
            fixture.Chat.StartNewSession();
            int ttsStopsAtStart = fixture.Tts.StopCount;
            int session = fixture.Chat.InteractionId;
            LocalTradeState trade = Level1GameState.Instance.ActiveTrade;
            Assert.IsNotNull(trade, fixture.Diagnostics());
            Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
            Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
            Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
            Assert.AreEqual(0, fixture.Speech.PendingCount, fixture.Diagnostics());
            Assert.AreEqual(Level1VoiceInputManager.VoiceInputState.Idle, fixture.Voice.CurrentState, fixture.Diagnostics());
            yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer, "greeting never yielded input");
            yield return DelayFrames(random.Next(0, 13)); // Immediate through short player-response delays.

            int scenario = scenarios[index % scenarios.Length];
            fixture.Note("scenario=" + scenario);
            if (scenario == 3) // Reset/new customer while recognition is still pending.
            {
                yield return BeginRecognition(random);
                var stale = fixture.LastToken;
                fixture.Chat.ResetConversationUI("Waiting for next customer...");
                Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(stale), fixture.Diagnostics());
                fixture.ShowConversationUI();
                fixture.Chat.StartNewSession();
                Assert.AreNotEqual(session, fixture.Chat.InteractionId, fixture.Diagnostics());
                LocalTradeState nextTrade = Level1GameState.Instance.ActiveTrade;
                string nextReply = fixture.NpcText.text;
                string nextSubtitle = fixture.Subtitle.text;
                fixture.Note("late old-customer recognition");
                yield return DelayFrames(random.Next(1, 5));
                fixture.Speech.CompleteNext("I accept your offer");
                yield return DelayFrames(2);
                Assert.AreEqual(0, fixture.Speech.PendingCount, fixture.Diagnostics());
                Assert.AreSame(nextTrade, Level1GameState.Instance.ActiveTrade, fixture.Diagnostics());
                Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
                Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(stale), fixture.Diagnostics());
                Assert.AreNotEqual(ConversationTurnLifecycle.Phase.Reviewing, fixture.Chat.TurnPhase, fixture.Diagnostics());
                fixture.Tts.EmitLatePlaybackStarted();
                fixture.Tts.EmitLatePlaybackFailure();
                Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
                Assert.AreEqual(nextReply, fixture.NpcText.text, fixture.Diagnostics());
                Assert.AreEqual(nextSubtitle, fixture.Subtitle.text, fixture.Diagnostics());
                fixture.Chat.ResetConversationUI();
                Assert.AreEqual(ConversationTurnLifecycle.Phase.Inactive, fixture.Chat.TurnPhase, fixture.Diagnostics());
                Assert.Greater(fixture.Tts.StopCount, ttsStopsAtStart, fixture.Diagnostics());
                continue;
            }

            if (scenario == 5) // Cancel a turn and deliver its result after a newer turn begins.
            {
                yield return BeginRecognition(random);
                var stale = fixture.LastToken;
                fixture.Voice.ClearTranscript();
                Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(stale), fixture.Diagnostics());
                yield return BeginRecognition(random);
                var current = fixture.LastToken;
                fixture.Note("late same-customer recognition");
                fixture.Speech.CompleteNext("I accept your offer");
                yield return DelayFrames(2);
                Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(stale), fixture.Diagnostics());
                Assert.IsTrue(fixture.Chat.IsCurrentVoiceTurn(current), fixture.Diagnostics());
                Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
                fixture.Speech.CompleteNext("What is your price?");
                yield return WaitUntil(() => fixture.Chat.TurnPhase == ConversationTurnLifecycle.Phase.Reviewing, "current review did not arrive");
                Assert.AreEqual("What is your price?", fixture.Input.text, fixture.Diagnostics());
                SubmitOnceAndRejectDuplicate(trade);
                yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer || IsResolved(), "NPC reply did not finish");
            }
            else if (scenario == 2) // Failure and empty recognition are recoverable, with no trade mutation.
            {
                yield return BeginRecognition(random);
                int offer = trade.npcOffer;
                int quantity = trade.quantityGrams;
                int turn = trade.turnIndex;
                if ((index / scenarios.Length) % 2 == 0) fixture.Speech.FailNext();
                else fixture.Speech.CompleteNext(" ");
                yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer, "failed recognition did not recover");
                Assert.AreEqual(offer, trade.npcOffer, fixture.Diagnostics());
                Assert.AreEqual(quantity, trade.quantityGrams, fixture.Diagnostics());
                Assert.AreEqual(turn, trade.turnIndex, fixture.Diagnostics());
                Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
                Assert.AreEqual(Level1VoiceInputManager.VoiceInputState.Idle, fixture.Voice.CurrentState, fixture.Diagnostics());
            }
            else if (scenario == 4) // Actual MarketplaceManager timer, including a response near timeout.
            {
                int randomMargin = random.Next(1, 4);
                yield return WaitUntil(() => fixture.IsNearIdleTimeout(randomMargin), "never reached a genuine near-timeout player window");
                Assert.IsTrue(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
                yield return BeginRecognition(random);
                yield return DelayFrames(120); // Longer than the walk-away window while busy.
                Assert.AreEqual(ConversationTurnLifecycle.Phase.Recognizing, fixture.Chat.TurnPhase, fixture.Diagnostics());
                Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
                fixture.Speech.CompleteNext("What is your price?");
                yield return WaitUntil(() => fixture.Chat.TurnPhase == ConversationTurnLifecycle.Phase.Reviewing, "recognition did not reach review");
                yield return DelayFrames(120); // Review must also pause silence penalties.
                Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
                SubmitOnceAndRejectDuplicate(trade);
                yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer || IsResolved(), "NPC reply did not finish");
                Assert.IsTrue(fixture.Chat.IsWaitingForPlayer, "Busy time caused premature NO DEAL. " + fixture.Diagnostics());
            }
            else if (scenario == 0) // A real price question creates an outstanding offer, then accept it.
            {
                yield return SpeakAndSubmit("What is your price?", random, trade);
                yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer || IsResolved(), "price reply did not finish");
                Assert.IsFalse(IsResolved(), fixture.Diagnostics());
                int offered = trade.npcOffer;
                yield return SpeakAndSubmit($"I accept {offered} varahas", random, trade);
                Assert.IsTrue(fixture.Chat.HasPendingFulfillment, fixture.Diagnostics());
                Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(fixture.LastToken), fixture.Diagnostics());
                Assert.AreEqual(offered, fixture.Chat.CurrentPendingFulfillment.agreedPrice, fixture.Diagnostics());
                Assert.IsNotNull(trade.AcceptedTerms, fixture.Diagnostics());
                Assert.AreEqual(offered, trade.AcceptedTerms.Price, fixture.Diagnostics());
                Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
                int moneyBefore = Level1GameState.Instance.CurrentMoney;
                trade.npcOffer = offered + 37; // A late mutable offer must not change frozen settlement.
                fixture.Chat.CompleteAcceptedFulfillment();
                int moneyAfter = Level1GameState.Instance.CurrentMoney;
                Assert.IsFalse(fixture.Chat.HasPendingFulfillment, fixture.Diagnostics());
                Assert.IsNull(Level1GameState.Instance.ActiveTrade, fixture.Diagnostics());
                fixture.Chat.CompleteAcceptedFulfillment(); // Must not resolve the transaction twice.
                Assert.AreEqual(moneyAfter, Level1GameState.Instance.CurrentMoney, fixture.Diagnostics());
                Assert.AreEqual(offered, moneyAfter - moneyBefore, fixture.Diagnostics());
            }
            else
            {
                int turnBefore = trade.turnIndex;
                string text = scenario == 1 ? "I reject that offer" :
                    scenario == 6 ? $"I offer {trade.npcOffer + 20} varahas" :
                    scenario == 8 ? "please leave" : "the purple moon dances";
                yield return SpeakAndSubmit(text, random, trade);
                yield return WaitUntil(() => fixture.Chat.IsWaitingForPlayer || IsResolved(), "NPC reply did not finish");
                if (scenario == 1) // Bare rejection while expecting a price is clarification, not a bargaining round.
                    Assert.AreEqual(turnBefore, trade.turnIndex, fixture.Diagnostics());
                if (scenario == 6) // A priced proposal must reach the trade once.
                    Assert.AreEqual(turnBefore + 1, trade.turnIndex, fixture.Diagnostics());
                if (scenario == 8) Assert.IsTrue(IsResolved(), "Dismissal did not resolve. " + fixture.Diagnostics());
            }

            if (!IsResolved())
            {
                fixture.Note("finish by genuine waiting-player timeout");
                Assert.IsTrue(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
                // Every nonterminal route waits for MarketplaceManager's actual idle coroutine.
                yield return WaitUntil(IsResolved, "Marketplace idle NO DEAL never resolved");
                Assert.IsFalse(fixture.Chat.HasPendingFulfillment, fixture.Diagnostics());
                Assert.IsNull(Level1GameState.Instance.ActiveTrade, fixture.Diagnostics());
            }

            Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
            Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
            Assert.IsFalse(fixture.Chat.IsCurrentVoiceTurn(fixture.LastToken), fixture.Diagnostics());
            Assert.Greater(fixture.Tts.StopCount, ttsStopsAtStart, fixture.Diagnostics());
            Assert.IsTrue(fixture.Marketplace.IsTransitioning, fixture.Diagnostics());
            int resolvedInteraction = fixture.Chat.InteractionId;
            fixture.Marketplace.OnNegotiationFinished(false); // Duplicate completion must not start another exit.
            yield return WaitUntil(() => !fixture.Marketplace.IsTransitioning, "customer departure stuck", MaxTransitionGameSeconds);
            Assert.AreEqual(resolvedInteraction + 1, fixture.Chat.InteractionId, fixture.Diagnostics());
            Assert.AreEqual(ConversationTurnLifecycle.Phase.Inactive, fixture.Chat.TurnPhase, fixture.Diagnostics());
            Assert.IsEmpty(fixture.Input.text, fixture.Diagnostics());
            Assert.AreEqual(0, fixture.Speech.PendingCount, fixture.Diagnostics());
            Assert.AreEqual(Level1VoiceInputManager.VoiceInputState.Idle, fixture.Voice.CurrentState, fixture.Diagnostics());
            string clearedSubtitle = fixture.Subtitle.text;
            fixture.Tts.EmitLatePlaybackStarted();
            fixture.Tts.EmitLatePlaybackFailure();
            Assert.AreEqual(clearedSubtitle, fixture.Subtitle.text, fixture.Diagnostics());
            Assert.IsEmpty(errors, fixture.Diagnostics());
        }
    }

    private bool IsResolved() => fixture.Chat.TurnPhase == ConversationTurnLifecycle.Phase.Resolved ||
        fixture.Chat.TurnPhase == ConversationTurnLifecycle.Phase.Inactive;

    private IEnumerator BeginRecognition(System.Random random)
    {
        Assert.IsTrue(fixture.Chat.TryBeginVoiceTurn(out var token), fixture.Diagnostics());
        fixture.LastToken = token;
        fixture.Note($"capture session={token.Interaction} turn={token.Turn}");
        Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
        yield return DelayFrames(random.Next(0, 4));
        Assert.IsTrue(fixture.Voice.BeginSimulatedRecognition(token), fixture.Diagnostics());
        Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
        yield return null; // The production coroutine requests the fake provider here.
        Assert.Greater(fixture.Speech.PendingCount, 0, fixture.Diagnostics());
    }

    private IEnumerator SpeakAndSubmit(string text, System.Random random, LocalTradeState trade)
    {
        yield return BeginRecognition(random);
        yield return DelayFrames(random.Next(0, 5));
        fixture.Note("recognition result: " + text);
        fixture.Speech.CompleteNext(text);
        yield return WaitUntil(() => fixture.Chat.TurnPhase == ConversationTurnLifecycle.Phase.Reviewing, "valid transcript never reached review");
        Assert.AreEqual(text, fixture.Input.text, fixture.Diagnostics());
        Assert.AreEqual(Level1VoiceInputManager.VoiceInputState.Review, fixture.Voice.CurrentState, fixture.Diagnostics());
        Assert.IsFalse(fixture.Chat.TryAdvanceVoiceTurn(fixture.LastToken, ConversationTurnLifecycle.Phase.Reviewing), fixture.Diagnostics());
        Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
        yield return DelayFrames(random.Next(0, 4));
        SubmitOnceAndRejectDuplicate(trade);
    }

    private void SubmitOnceAndRejectDuplicate(LocalTradeState trade)
    {
        Assert.IsNotNull(LocalDialogueTurnField, "ChatManager local turn counter changed; update the soak's duplicate-submission probe.");
        int beforeTurn = trade.turnIndex;
        int beforeProcessed = (int)LocalDialogueTurnField.GetValue(fixture.Chat);
        int beforeOffer = trade.npcOffer;
        int beforeQuantity = trade.quantityGrams;
        var beforeAccepted = trade.AcceptedTerms;
        fixture.Note("submit and attempted duplicate");
        fixture.Chat.OnSend();
        int afterTurn = trade.turnIndex;
        int afterProcessed = (int)LocalDialogueTurnField.GetValue(fixture.Chat);
        // Terminal replies also advance this version when obsolete presentation is invalidated.
        Assert.AreEqual(beforeProcessed + (IsResolved() ? 2 : 1), afterProcessed, fixture.Diagnostics());
        Assert.That(afterTurn - beforeTurn, Is.InRange(0, 1), fixture.Diagnostics());
        if (afterTurn == beforeTurn && !IsResolved())
        {
            // A clarification can be processed without committing trade terms.
            Assert.AreEqual(beforeOffer, trade.npcOffer, fixture.Diagnostics());
            Assert.AreEqual(beforeQuantity, trade.quantityGrams, fixture.Diagnostics());
            Assert.AreSame(beforeAccepted, trade.AcceptedTerms, fixture.Diagnostics());
        }
        int afterOffer = trade.npcOffer;
        int afterQuantity = trade.quantityGrams;
        var afterAccepted = trade.AcceptedTerms;
        int afterNpcOffers = trade.npcOfferHistory.Count;
        int afterPlayerOffers = trade.playerOfferHistory.Count;
        fixture.Chat.OnSend();
        Assert.AreEqual(afterProcessed, (int)LocalDialogueTurnField.GetValue(fixture.Chat), fixture.Diagnostics());
        Assert.AreEqual(afterTurn, trade.turnIndex, fixture.Diagnostics());
        Assert.AreEqual(afterOffer, trade.npcOffer, fixture.Diagnostics());
        Assert.AreEqual(afterQuantity, trade.quantityGrams, fixture.Diagnostics());
        Assert.AreSame(afterAccepted, trade.AcceptedTerms, fixture.Diagnostics());
        Assert.AreEqual(afterNpcOffers, trade.npcOfferHistory.Count, fixture.Diagnostics());
        Assert.AreEqual(afterPlayerOffers, trade.playerOfferHistory.Count, fixture.Diagnostics());
        Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
    }

    private IEnumerator WaitUntil(Func<bool> condition, string failure, float maxGameSeconds = MaxWaitGameSeconds)
    {
        float startedAt = Time.time;
        int startedFrame = Time.frameCount;
        float lastGameTime = startedAt;
        int framesWithoutGameTime = 0;
        while (true)
        {
            AssertRuntimeInvariants();
            if (condition()) yield break;
            Assert.Less(Time.time - startedAt, maxGameSeconds,
                $"{failure}; wait exceeded {maxGameSeconds:0.0} game seconds, framesWaited={Time.frameCount - startedFrame}. " + fixture.Diagnostics());
            framesWithoutGameTime = Time.time > lastGameTime ? 0 : framesWithoutGameTime + 1;
            Assert.Less(framesWithoutGameTime, MaxFramesWithoutGameTime,
                $"{failure}; game time stopped advancing, framesWaited={Time.frameCount - startedFrame}. " + fixture.Diagnostics());
            lastGameTime = Time.time;
            yield return null;
        }
    }

    private IEnumerator DelayFrames(int count)
    {
        for (int frame = 0; frame < count; frame++)
        {
            AssertRuntimeInvariants();
            yield return null;
        }
    }

    private void AssertRuntimeInvariants()
    {
        var phase = fixture.Chat.TurnPhase;
        if (phase != ConversationTurnLifecycle.Phase.WaitingForPlayer)
            Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
        if (phase == ConversationTurnLifecycle.Phase.Capturing ||
            phase == ConversationTurnLifecycle.Phase.Recognizing ||
            phase == ConversationTurnLifecycle.Phase.Reviewing ||
            phase == ConversationTurnLifecycle.Phase.Submitted ||
            phase == ConversationTurnLifecycle.Phase.NPCResponding)
            Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
        if (phase == ConversationTurnLifecycle.Phase.Resolved || phase == ConversationTurnLifecycle.Phase.Inactive)
            Assert.IsFalse(fixture.Chat.IsWaitingForPlayer, fixture.Diagnostics());
        if (phase != ConversationTurnLifecycle.Phase.WaitingForPlayer)
            Assert.IsFalse(fixture.Chat.TryHandleNegotiationTimeout(), fixture.Diagnostics());
        Assert.IsEmpty(errors, fixture.Diagnostics());
    }

    private void CaptureError(string message, string stack, LogType type)
    {
        const string gapPrefix = "[MARKET LOOP] Customer left. Next customer gap chosen: ";
        if (type == LogType.Log && fixture != null)
        {
            if (message.StartsWith(gapPrefix, StringComparison.Ordinal))
                fixture.LastSelectedNextCustomerGap = message.Substring(gapPrefix.Length);
            if (message.Contains("waiting-for-player started")) fixture.WaitingForPlayerLogs++;
            if (message.StartsWith("[MARKET LOOP] Player idle patience expired", StringComparison.Ordinal)) fixture.IdleExpiryLogs++;
            if (message.StartsWith("[MARKET LOOP] Customer leaving stall", StringComparison.Ordinal)) fixture.CustomerLeavingLogs++;
        }
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            errors.Add(type + ": " + message + "\n" + stack);
    }

    [UnityTearDown]
    public IEnumerator CleanUp()
    {
        Application.logMessageReceived -= CaptureError;
        if (EditorApplication.isPlaying)
        {
            if (fixture != null)
            {
                fixture.DestroyObjects();
                yield return null; // Let Unity destroy objects while the save override is still active.
                fixture.DeleteSaveDirectory();
                fixture = null;
            }
            LocalSaveManager.EditorTestSaveDirectoryOverride = null;
            UnityEngine.Random.state = originalUnityRandomState;
            Time.timeScale = originalTimeScale > 0 ? originalTimeScale : 1f;
            Time.captureFramerate = originalCaptureFramerate;
            yield return new ExitPlayMode();
        }
    }

    private sealed class SoakFixture
    {
        public readonly ChatManager Chat;
        public readonly MarketplaceManager Marketplace;
        public readonly Level1VoiceInputManager Voice;
        public readonly Level1SoakSpeechProvider Speech;
        public readonly Level1SoakTtsProvider Tts;
        public readonly TMP_InputField Input;
        public readonly TextMeshProUGUI NpcText;
        public readonly TextMeshProUGUI Subtitle;
        public ConversationTurnLifecycle.VoiceToken LastToken;
        public int Iteration;
        public int IterationStartFrame;
        public float IterationStartTime;
        public string LastSelectedNextCustomerGap = "not logged";
        public int WaitingForPlayerLogs;
        public int IdleExpiryLogs;
        public int CustomerLeavingLogs;
        private readonly int seed;
        private readonly Queue<string> history = new Queue<string>();
        private readonly string saveDirectory;
        private readonly GameObject root;
        private readonly GameObject conversationUi;
        private readonly GameObject gameStateObject;
        private static readonly FieldInfo IdleStartedField = typeof(MarketplaceManager).GetField("playerIdleStartedAt", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo WalkAwayField = typeof(MarketplaceManager).GetField("walkAwaySeconds", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CountdownField = typeof(MarketplaceManager).GetField("nextCustomerCountdownCoroutine", BindingFlags.Instance | BindingFlags.NonPublic);

        public SoakFixture(int seed)
        {
            this.seed = seed;
            Assert.IsTrue(Level1GameState.ExistingInstance == null, "Close any existing Level 1 scene before the soak: a live profile must not be touched.");
            saveDirectory = Path.Combine(Application.temporaryCachePath, "Level1Soak-" + Guid.NewGuid().ToString("N"));
            LocalSaveManager.EditorTestSaveDirectoryOverride = saveDirectory;
            gameStateObject = new GameObject("Level1SoakGameState");
            gameStateObject.AddComponent<Level1GameState>();
            Assert.AreEqual(Path.Combine(saveDirectory, LocalSaveManager.ProfileFileName), Level1GameState.Instance.ActiveSavePath);

            root = new GameObject(FixtureName);
            var canvasObject = new GameObject("SoakCanvas", typeof(RectTransform), typeof(Canvas));
            conversationUi = canvasObject;
            canvasObject.transform.SetParent(root.transform);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var eventSystem = new GameObject("SoakEventSystem", typeof(EventSystem));
            eventSystem.transform.SetParent(root.transform);
            var inputObject = new GameObject("Input", typeof(RectTransform));
            inputObject.SetActive(false);
            inputObject.transform.SetParent(canvasObject.transform);
            Input = inputObject.AddComponent<TMP_InputField>();
            Input.textViewport = inputObject.GetComponent<RectTransform>();
            Input.textComponent = MakeText("InputText", inputObject.transform);
            inputObject.SetActive(true);
            NpcText = MakeText("NpcText", canvasObject.transform);
            var statusText = MakeText("VoiceStatus", canvasObject.transform);
            Subtitle = MakeText("SubtitleText", canvasObject.transform);
            var speakerText = MakeText("SpeakerText", canvasObject.transform);
            var subtitlePanel = new GameObject("SubtitlePanel", typeof(RectTransform));
            subtitlePanel.transform.SetParent(canvasObject.transform);

            var buyer = new GameObject("SoakBuyer");
            buyer.transform.SetParent(root.transform);
            var spawn = new GameObject("SoakSpawn");
            spawn.transform.SetParent(root.transform);
            var exit = new GameObject("SoakExit");
            exit.transform.SetParent(root.transform);

            var host = new GameObject("SoakManagers");
            host.transform.SetParent(root.transform);
            var api = host.AddComponent<APIManager>();
            var feedback = host.AddComponent<BazaarFeedbackManager>();
            var hud = host.AddComponent<Level1HUDManager>();
            hud.subtitlePanel = subtitlePanel;
            hud.npcSubtitleText = Subtitle;
            hud.speakerNameText = speakerText;
            hud.playerInput = Input;
            hud.voiceStatusText = statusText;
            var audio = host.AddComponent<AudioManager>();
            Tts = host.AddComponent<Level1SoakTtsProvider>();
            audio.localNpcTtsProvider = Tts;
            Marketplace = host.AddComponent<MarketplaceManager>();
            Marketplace.enabled = false; // Its scene-start NavMesh/VR loop is intentionally outside this fixture.
            Marketplace.buyerNPC = buyer;
            Marketplace.spawnPoint = spawn.transform;
            Marketplace.tradePoint = buyer.transform;
            Marketplace.exitPoint = exit.transform;
            Marketplace.conversationUI = canvasObject;
            Chat = host.AddComponent<ChatManager>();
            Chat.api = api;
            Chat.inputField = Input;
            Chat.npcText = NpcText;
            Chat.audioManager = audio;
            Chat.feedbackManager = feedback;
            Chat.hudManager = hud;
            Chat.marketplaceManager = Marketplace;
            Chat.useLocalSessionGeneration = true;
            Chat.useLocalNpcBrain = true;
            Chat.enableNpcTTS = true;
            Marketplace.chatManager = Chat;
            Speech = host.AddComponent<Level1SoakSpeechProvider>();
            Voice = host.AddComponent<Level1VoiceInputManager>();
            Voice.chatManager = Chat;
            Voice.inputField = Input;
            Voice.voiceStatusText = statusText;
            Voice.speechProviderOverride = Speech;
        }

        private static TextMeshProUGUI MakeText(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent);
            return go.GetComponent<TextMeshProUGUI>();
        }

        public void Note(string operation)
        {
            history.Enqueue(operation);
            if (history.Count > 16) history.Dequeue();
        }

        public string Diagnostics()
        {
            return $"seed={seed} iteration={Iteration} session={Chat.InteractionId} turn={Chat.TurnId} phase={Chat.TurnPhase} " +
                   $"pendingSTT={Speech.PendingCount} pendingTTS={Tts.HasPendingPlayback} voice={Voice.CurrentState} marketTransition={Marketplace.IsTransitioning} " +
                   $"countdownCoroutineActive={CountdownField != null && CountdownField.GetValue(Marketplace) != null} " +
                   $"exitCoroutineActive=unobservable(no stored handle) nextCustomerGap={LastSelectedNextCustomerGap} " +
                   $"elapsedGameSeconds={Time.time - IterationStartTime:0.000} frames={Time.frameCount - IterationStartFrame} " +
                   $"deltaTime={Time.deltaTime:0.0000} timeScale={Time.timeScale:0.00} marketDayEnded={Level1GameState.Instance.MarketDayEnded} input='{Input.text}'\n" +
                   string.Join("\n", history);
        }

        public bool IsNearIdleTimeout(int marginSeconds)
        {
            Assert.IsNotNull(IdleStartedField, "Marketplace idle start field changed; update this test's timer probe.");
            Assert.IsNotNull(WalkAwayField, "Marketplace walk-away field changed; update this test's timer probe.");
            float started = (float)IdleStartedField.GetValue(Marketplace);
            int threshold = (int)WalkAwayField.GetValue(Marketplace);
            return Chat.IsWaitingForPlayer && Time.time - started >= threshold - marginSeconds;
        }

        public int WalkAwaySeconds
        {
            get
            {
                Assert.IsNotNull(WalkAwayField, "Marketplace walk-away field changed; update this test's timer probe.");
                return (int)WalkAwayField.GetValue(Marketplace);
            }
        }

        public float PlayerIdleStartedAt
        {
            get
            {
                Assert.IsNotNull(IdleStartedField, "Marketplace idle start field changed; update this test's timer probe.");
                return (float)IdleStartedField.GetValue(Marketplace);
            }
        }

        public void ShowConversationUI() => conversationUi.SetActive(true);

        public void DestroyObjects()
        {
            if (Chat != null)
            {
                Chat.ResetConversationUI();
                Chat.enabled = false;
            }
            if (Marketplace != null) Marketplace.StopNegotiationTimer();
            if (Voice != null) Voice.enabled = false;
            if (root != null) UnityEngine.Object.Destroy(root);
            if (gameStateObject != null) UnityEngine.Object.Destroy(gameStateObject);
        }

        public void DeleteSaveDirectory()
        {
            if (Directory.Exists(saveDirectory)) Directory.Delete(saveDirectory, true);
        }
    }
}
