using System;
using System.Collections.Generic;
using NUnit.Framework;

public class Level1ConversationLifecycleStressTests
{
    public const int DefaultStressSeed = 12345;
    private const int OperationsPerSeed = 2000;

    [TestCase(DefaultStressSeed)]
    [TestCase(71931)]
    [TestCase(20260924)]
    public void RandomizedLifecycleOperationsPreserveTurnAndIdleInvariants(int seed)
    {
        RunSeed(seed, OperationsPerSeed);
    }

    [Test, Explicit("Run manually for a longer lifecycle fuzz run.")]
    public void HeavyLifecycleStress()
    {
        RunSeed(DefaultStressSeed, 100000);
    }

    // To replay a failure, replace DefaultStressSeed (or add its value as a TestCase).
    public static void RunSeed(int seed, int operationCount)
    {
        var random = new Random(seed);
        var lifecycle = new ConversationTurnLifecycle();
        var tokens = new List<ConversationTurnLifecycle.VoiceToken>();
        var recent = new Queue<string>();
        int idleTicks = 0;
        int priorInteraction = lifecycle.InteractionId;
        int priorTurn = lifecycle.TurnId;

        for (int index = 0; index < operationCount; index++)
        {
            int choice = random.Next(15);
            var before = lifecycle.CurrentPhase;
            int idleBefore = idleTicks;
            var token = tokens.Count == 0
                ? default(ConversationTurnLifecycle.VoiceToken)
                : tokens[random.Next(tokens.Count)];
            string operation;
            bool result = false;

            switch (choice)
            {
                case 0:
                    lifecycle.BeginInteraction();
                    operation = "new customer";
                    break;
                case 1:
                    lifecycle.Invalidate(true);
                    operation = "resolve";
                    break;
                case 2:
                    lifecycle.Invalidate(false);
                    operation = "reset";
                    break;
                case 3:
                    result = lifecycle.WaitForPlayer(lifecycle.InteractionId);
                    operation = "NPC finished presentation";
                    Check(result == (before == ConversationTurnLifecycle.Phase.NPCResponding), "unexpected player-unlock result");
                    break;
                case 4:
                    result = lifecycle.WaitForPlayer(token.Interaction);
                    operation = "stale NPC presentation callback";
                    Check(!result || token.Interaction == lifecycle.InteractionId, "old customer unlocked input");
                    break;
                case 5:
                    result = lifecycle.TryBeginCapture(out var newToken);
                    operation = "start capture";
                    Check(result == (before == ConversationTurnLifecycle.Phase.WaitingForPlayer), "capture began outside player window");
                    if (result)
                    {
                        tokens.Add(newToken);
                        if (tokens.Count > 32) tokens.RemoveAt(0);
                    }
                    break;
                case 6:
                    result = lifecycle.TryAdvanceVoice(token, ConversationTurnLifecycle.Phase.Recognizing);
                    operation = "recognizing callback";
                    Check(!result || before == ConversationTurnLifecycle.Phase.Capturing, "recognition bypassed capture");
                    break;
                case 7:
                    result = lifecycle.TryAdvanceVoice(token, ConversationTurnLifecycle.Phase.Reviewing);
                    operation = "recognition completed / duplicate callback";
                    Check(!result || before == ConversationTurnLifecycle.Phase.Recognizing, "review bypassed recognition");
                    break;
                case 8:
                    result = lifecycle.EndVoiceWithoutSubmission(token);
                    operation = "recognition failure";
                    Check(!result || lifecycle.CurrentPhase == ConversationTurnLifecycle.Phase.WaitingForPlayer, "failed recognition submitted");
                    break;
                case 9:
                    result = lifecycle.EndVoiceWithoutSubmission(token);
                    operation = "empty recognition";
                    Check(!result || lifecycle.CurrentPhase == ConversationTurnLifecycle.Phase.WaitingForPlayer, "empty recognition submitted");
                    break;
                case 10:
                    result = lifecycle.EndVoiceWithoutSubmission(token);
                    operation = "cancel review";
                    break;
                case 11:
                    result = lifecycle.SubmitPlayerTurn();
                    operation = "submit / duplicate submit";
                    Check(result == (before == ConversationTurnLifecycle.Phase.WaitingForPlayer || before == ConversationTurnLifecycle.Phase.Reviewing), "duplicate or busy submission committed");
                    break;
                case 12:
                    lifecycle.BeginNpcResponse();
                    operation = "NPC responding";
                    break;
                case 13:
                    result = lifecycle.IsCurrent(token);
                    operation = "late result validity probe";
                    Check(!result || (token.Interaction == lifecycle.InteractionId && token.Turn == lifecycle.TurnId), "stale result became current");
                    break;
                default:
                    operation = "idle timer tick";
                    if (lifecycle.IsWaitingForPlayer) idleTicks++;
                    break;
            }

            recent.Enqueue($"{index}: {operation}, before={before}, result={result}, after={lifecycle.CurrentPhase}, session={lifecycle.InteractionId}, turn={lifecycle.TurnId}");
            if (recent.Count > 12) recent.Dequeue();
            string context = $"seed={seed} op={index} session={lifecycle.InteractionId} turn={lifecycle.TurnId} phase={lifecycle.CurrentPhase} idleTicks={idleTicks}\n{string.Join("\n", recent)}";

            Assert.GreaterOrEqual(lifecycle.InteractionId, priorInteraction, context);
            Assert.GreaterOrEqual(lifecycle.TurnId, priorTurn, context);
            Assert.AreEqual(idleBefore + (choice == 14 && before == ConversationTurnLifecycle.Phase.WaitingForPlayer ? 1 : 0), idleTicks, context);
            Assert.AreEqual(lifecycle.CurrentPhase == ConversationTurnLifecycle.Phase.WaitingForPlayer, lifecycle.IsWaitingForPlayer, context);
            if (lifecycle.CurrentPhase == ConversationTurnLifecycle.Phase.Inactive || lifecycle.CurrentPhase == ConversationTurnLifecycle.Phase.Resolved)
            {
                Assert.IsFalse(lifecycle.SubmitPlayerTurn(), context);
                Assert.IsFalse(lifecycle.IsWaitingForPlayer, context);
            }
            foreach (var oldToken in tokens)
            {
                if (oldToken.Interaction != lifecycle.InteractionId || oldToken.Turn != lifecycle.TurnId)
                {
                    Assert.IsFalse(lifecycle.IsCurrent(oldToken), context);
                    Assert.IsFalse(lifecycle.TryAdvanceVoice(oldToken, ConversationTurnLifecycle.Phase.Reviewing), context);
                }
            }
            priorInteraction = lifecycle.InteractionId;
            priorTurn = lifecycle.TurnId;

            void Check(bool condition, string message)
            {
                if (!condition) Assert.Fail($"{message}; seed={seed} op={index} choice={choice} session={lifecycle.InteractionId} turn={lifecycle.TurnId} phase={lifecycle.CurrentPhase}\n{string.Join("\n", recent)}");
            }
        }
    }
}
