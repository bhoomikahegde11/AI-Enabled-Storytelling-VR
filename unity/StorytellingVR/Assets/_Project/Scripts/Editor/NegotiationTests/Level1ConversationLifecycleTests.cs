using NUnit.Framework;

public class Level1ConversationLifecycleTests
{
    private static ConversationTurnLifecycle Waiting()
    {
        var lifecycle = new ConversationTurnLifecycle();
        lifecycle.BeginInteraction();
        Assert.IsTrue(lifecycle.WaitForPlayer(lifecycle.InteractionId));
        return lifecycle;
    }

    [Test]
    public void CurrentVoiceResultIsAcceptedOnlyForItsOwnInteractionAndTurn()
    {
        var lifecycle = Waiting();
        Assert.IsTrue(lifecycle.TryBeginCapture(out var first));
        Assert.IsTrue(lifecycle.TryAdvanceVoice(first, ConversationTurnLifecycle.Phase.Recognizing));
        Assert.IsTrue(lifecycle.IsCurrent(first));

        Assert.IsTrue(lifecycle.EndVoiceWithoutSubmission(first));
        Assert.IsTrue(lifecycle.TryBeginCapture(out var second));
        Assert.AreEqual(first.Interaction, second.Interaction);
        Assert.IsFalse(lifecycle.IsCurrent(first));
        Assert.IsFalse(lifecycle.TryAdvanceVoice(first, ConversationTurnLifecycle.Phase.Reviewing));
        Assert.IsTrue(lifecycle.TryAdvanceVoice(second, ConversationTurnLifecycle.Phase.Recognizing));
        Assert.IsTrue(lifecycle.TryAdvanceVoice(second, ConversationTurnLifecycle.Phase.Reviewing));
        Assert.IsTrue(lifecycle.SubmitPlayerTurn());
        Assert.IsFalse(lifecycle.IsCurrent(second));
    }

    [Test]
    public void ResolutionAndNextCustomerRejectPreviousCustomerWork()
    {
        var lifecycle = Waiting();
        Assert.IsTrue(lifecycle.TryBeginCapture(out var old));
        Assert.IsTrue(lifecycle.TryAdvanceVoice(old, ConversationTurnLifecycle.Phase.Recognizing));

        lifecycle.Invalidate(true);
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        Assert.IsFalse(lifecycle.IsCurrent(old));
        Assert.IsFalse(lifecycle.SubmitPlayerTurn());
        Assert.IsFalse(lifecycle.TryBeginCapture(out _));

        lifecycle.BeginInteraction();
        Assert.IsFalse(lifecycle.TryBeginCapture(out _)); // NPC greeting is not a legal player turn.
        Assert.IsFalse(lifecycle.IsCurrent(old));
        Assert.IsFalse(lifecycle.TryAdvanceVoice(old, ConversationTurnLifecycle.Phase.Reviewing));
        Assert.IsTrue(lifecycle.WaitForPlayer(lifecycle.InteractionId));
        Assert.IsTrue(lifecycle.TryBeginCapture(out var current));
        Assert.AreNotEqual(old.Interaction, current.Interaction);
    }

    [Test]
    public void OnlyWaitingForPlayerCountsAsIdle()
    {
        var lifecycle = new ConversationTurnLifecycle();
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        lifecycle.BeginInteraction();
        Assert.IsFalse(lifecycle.IsWaitingForPlayer); // NPC greeting
        lifecycle.WaitForPlayer(lifecycle.InteractionId);
        Assert.IsTrue(lifecycle.IsWaitingForPlayer);

        lifecycle.TryBeginCapture(out var voice);
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        lifecycle.TryAdvanceVoice(voice, ConversationTurnLifecycle.Phase.Recognizing);
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        lifecycle.TryAdvanceVoice(voice, ConversationTurnLifecycle.Phase.Reviewing);
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);

        Assert.IsTrue(lifecycle.SubmitPlayerTurn());
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        lifecycle.BeginNpcResponse();
        Assert.IsFalse(lifecycle.IsWaitingForPlayer);
        Assert.IsTrue(lifecycle.WaitForPlayer(lifecycle.InteractionId));
        Assert.IsTrue(lifecycle.IsWaitingForPlayer);
    }

    [Test]
    public void EmptyOrFailedRecognitionReturnsToWaitingWithoutSubmitting()
    {
        var lifecycle = Waiting();
        lifecycle.TryBeginCapture(out var voice);
        lifecycle.TryAdvanceVoice(voice, ConversationTurnLifecycle.Phase.Recognizing);

        Assert.IsTrue(lifecycle.EndVoiceWithoutSubmission(voice));
        Assert.AreEqual(ConversationTurnLifecycle.Phase.WaitingForPlayer, lifecycle.CurrentPhase);
        Assert.IsFalse(lifecycle.IsCurrent(voice));
        Assert.IsTrue(lifecycle.TryBeginCapture(out var retry));
        Assert.AreNotEqual(voice.Turn, retry.Turn);
    }

    [Test]
    public void ResetClearsReviewAndLateResultCannotReopenNextInteraction()
    {
        var lifecycle = Waiting();
        lifecycle.TryBeginCapture(out var old);
        lifecycle.TryAdvanceVoice(old, ConversationTurnLifecycle.Phase.Recognizing);
        lifecycle.TryAdvanceVoice(old, ConversationTurnLifecycle.Phase.Reviewing);
        lifecycle.Invalidate(false);
        Assert.AreEqual(ConversationTurnLifecycle.Phase.Inactive, lifecycle.CurrentPhase);

        lifecycle.BeginInteraction();
        Assert.AreEqual(ConversationTurnLifecycle.Phase.NPCResponding, lifecycle.CurrentPhase);
        Assert.IsFalse(lifecycle.TryAdvanceVoice(old, ConversationTurnLifecycle.Phase.Reviewing));
        Assert.IsFalse(lifecycle.WaitForPlayer(old.Interaction));
    }
}
