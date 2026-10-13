// Tracks activity, not negotiation meaning. All transitions are made on Unity's main thread.
public sealed class ConversationTurnLifecycle
{
    public enum Phase
    {
        Inactive,
        WaitingForPlayer,
        Capturing,
        Recognizing,
        Reviewing,
        Submitted,
        NPCResponding,
        Resolved
    }

    public readonly struct VoiceToken
    {
        public readonly int Interaction;
        public readonly int Turn;

        public VoiceToken(int interaction, int turn)
        {
            Interaction = interaction;
            Turn = turn;
        }
    }

    public int InteractionId { get; private set; }
    public int TurnId { get; private set; }
    public Phase CurrentPhase { get; private set; } = Phase.Inactive;
    public bool IsWaitingForPlayer => CurrentPhase == Phase.WaitingForPlayer;

    public void BeginInteraction()
    {
        InteractionId++;
        TurnId++;
        CurrentPhase = Phase.NPCResponding;
    }

    public void Invalidate(bool resolved)
    {
        InteractionId++;
        TurnId++;
        CurrentPhase = resolved ? Phase.Resolved : Phase.Inactive;
    }

    public bool IsActiveInteraction(int interaction) =>
        InteractionId == interaction && CurrentPhase != Phase.Inactive && CurrentPhase != Phase.Resolved;

    public bool TryBeginCapture(out VoiceToken token)
    {
        token = default;
        if (!IsWaitingForPlayer) return false;
        TurnId++;
        token = new VoiceToken(InteractionId, TurnId);
        CurrentPhase = Phase.Capturing;
        return true;
    }

    public bool IsCurrent(VoiceToken token) => IsActiveInteraction(token.Interaction) && TurnId == token.Turn;

    public bool TryAdvanceVoice(VoiceToken token, Phase phase)
    {
        if (!IsCurrent(token)) return false;
        if (phase == Phase.Recognizing && CurrentPhase != Phase.Capturing) return false;
        if (phase == Phase.Reviewing && CurrentPhase != Phase.Recognizing) return false;
        if (phase != Phase.Recognizing && phase != Phase.Reviewing) return false;
        CurrentPhase = phase;
        return true;
    }

    public bool EndVoiceWithoutSubmission(VoiceToken token)
    {
        if (!IsCurrent(token)) return false;
        TurnId++;
        CurrentPhase = Phase.WaitingForPlayer;
        return true;
    }

    public bool SubmitPlayerTurn()
    {
        if (CurrentPhase != Phase.WaitingForPlayer && CurrentPhase != Phase.Reviewing) return false;
        TurnId++;
        CurrentPhase = Phase.Submitted;
        return true;
    }

    public void BeginNpcResponse()
    {
        if (CurrentPhase == Phase.Submitted || CurrentPhase == Phase.WaitingForPlayer)
            CurrentPhase = Phase.NPCResponding;
    }

    public bool WaitForPlayer(int interaction)
    {
        if (!IsActiveInteraction(interaction) || CurrentPhase != Phase.NPCResponding) return false;
        CurrentPhase = Phase.WaitingForPlayer;
        return true;
    }
}
