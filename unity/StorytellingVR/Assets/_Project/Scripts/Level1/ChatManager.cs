using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class APIResponse
{
    public string npc_text;
    public string audio_url;
}

public class ChatManager : MonoBehaviour
{
    public APIManager api;

    public TMP_InputField inputField;
    public TextMeshProUGUI npcText;

    public AudioManager audioManager;

    [Header("UI Metrics")]
    public RespectUIManager respectUIManager;
    public TextMeshProUGUI coinsEarnedText;

    [Header("Bazaar Feedback Control")]
    public BazaarFeedbackManager feedbackManager;

    [Header("Lifecycle Control")]
    public bool autoStart = false;
    public MarketplaceManager marketplaceManager;

    // 🔥 Prevent STT spam / multiple requests
    private bool isProcessing = false;
    private string lastProcessedText = "";
    private float lastProcessedTime = 0f;

    void Start()
    {
        if (autoStart)
        {
            StartNewSession();
        }
    }

    public void StartNewSession()
    {
        isProcessing = false; // Reset lock for new session
        StartCoroutine(api.StartSession(OnNPCReply));
    }

    public void ResetConversationUI(string statusText = "Customer approaching...")
    {
        if (npcText != null)
        {
            npcText.text = statusText;
        }

        if (inputField != null)
        {
            inputField.text = "";
            inputField.interactable = false;
        }
        
        lastProcessedText = ""; // Reset STT filter history for the new customer
    }

    public void EnableConversationUI()
    {
        if (inputField != null)
        {
            inputField.interactable = true;
            inputField.ActivateInputField(); // Focus the input field automatically for seamless typing
        }
    }

    // 📝 TEXT INPUT (unchanged behavior)
    public void OnSend()
    {
        if (isProcessing) return;

        string playerText = inputField.text;

        if (string.IsNullOrEmpty(playerText)) return;

        isProcessing = true;

        StartCoroutine(SendMessageRoutine(playerText));

        inputField.text = "";
    }

    // 🎤 VOICE INPUT (fixed + throttled)
    public void OnVoiceInput(string spokenText)
    {
        if (string.IsNullOrEmpty(spokenText)) return;

        // 🔥 Ignore repeated / similar inputs
        if (spokenText == lastProcessedText) return;

        // 🔥 Cooldown check (3 seconds)
        if (Time.time - lastProcessedTime < 3f) return;

        Debug.Log("Voice Input: " + spokenText);

        lastProcessedText = spokenText;
        lastProcessedTime = Time.time;

        if (npcText != null)
            npcText.text = "You: " + spokenText;

        StartCoroutine(SendMessageRoutine(spokenText));
    }

    // 🔁 COMMON SEND ROUTINE (prevents duplication)
    IEnumerator SendMessageRoutine(string text)
    {
        // 1. Trigger the thinking behavior if feedbackManager is assigned
        Animator npcAnim = null;
        if (marketplaceManager != null && marketplaceManager.buyerNPC != null)
        {
            npcAnim = marketplaceManager.buyerNPC.GetComponent<Animator>();
        }

        if (feedbackManager != null)
        {
            feedbackManager.StartNPCThinking(npcAnim, npcText);
        }

        yield return api.SendMessage(text, OnNPCReply);

        // 🔥 cooldown to prevent API spam (VERY IMPORTANT)
        yield return new WaitForSeconds(2.5f);

        isProcessing = false;
    }

    // 🤖 NPC RESPONSE (unchanged but safer)
    void OnNPCReply(string text, string audioUrl, int reputation, int totalVarahas, bool done, TransactionSummary transaction)
    {
        Debug.Log("NPC Reply: " + text);

        // 1. Stop thinking animations
        Animator npcAnim = null;
        if (marketplaceManager != null && marketplaceManager.buyerNPC != null)
        {
            npcAnim = marketplaceManager.buyerNPC.GetComponent<Animator>();
        }

        if (feedbackManager != null)
        {
            feedbackManager.StopNPCThinking(npcAnim);
        }

        // Intercept API / server errors
        if (text == null)
        {
            if (npcText != null)
            {
                npcText.text = "The market is too noisy, could you repeat that?";
            }
            EnableConversationUI();
            isProcessing = false;
            return;
        }

        npcText.text = text;

        if (respectUIManager != null)
        {
            respectUIManager.SetRespect(reputation);
        }

        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = "Coins Earned: " + totalVarahas;
        }

        // 2. Trigger transaction completed feedback popups or respect warnings on done
        if (done && feedbackManager != null)
        {
            if (transaction != null)
            {
                // Determine player archetype based on reputation score
                string archetype = "Standard Merchant";
                if (reputation >= 80) archetype = "Fair Trader";
                else if (reputation <= 35) archetype = "Greedy Haggler";

                feedbackManager.ShowTransactionFeedback(transaction, archetype);
            }
            else
            {
                // Walk-away/failure - trigger negative reputation toast
                feedbackManager.TriggerRespectToast(-15);
            }
        }

        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Debug.Log("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }
        else
        {
            Debug.LogWarning("Audio URL missing or AudioManager not assigned!");
        }

        if (done && marketplaceManager != null)
        {
            marketplaceManager.OnNegotiationFinished();
        }
    }
}