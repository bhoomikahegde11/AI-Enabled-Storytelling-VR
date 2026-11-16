using UnityEngine;

// Rahim's practice lesson uses the same capture and review pipeline as the live market.
public class VoiceRecognitionManager : MonoBehaviour
{
    [SerializeField] private Level1VoiceInputManager voiceInput;
    public TutorialManager tutorialManager;
    public Level1HUDManager hudManager;

    internal ConversationTurnLifecycle Turns { get; } = new ConversationTurnLifecycle();
    internal string OfferHint { get; private set; }

    public void ListenForPrice(string priceHint = "")
    {
        if (voiceInput == null)
        {
            Debug.LogError("[STT] Practice voice needs a shared voice input component.", this);
            return;
        }
        OfferHint = string.IsNullOrWhiteSpace(priceHint) ? "a price" : priceHint;
        Turns.BeginInteraction();
        Turns.WaitForPlayer(Turns.InteractionId);
        voiceInput.InvalidateVoiceWork();
        hudManager?.ShowPlayerInputPanel();
    }

    internal void SubmitPrice(string transcript)
    {
        if (!NegotiationStateManager.TryExtractPracticePrice(transcript, out int price) ||
            !Turns.SubmitPlayerTurn()) return;

        hudManager?.HidePlayerInputPanel();
        tutorialManager.HandlePlayerOffer(price);
    }
}
