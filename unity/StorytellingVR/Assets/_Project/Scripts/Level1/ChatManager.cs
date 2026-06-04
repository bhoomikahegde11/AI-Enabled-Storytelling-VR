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

    [Header("Level 1 HUD References")]
    public Level1HUDManager hudManager;
    public GameObject sendButtonObject;

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    private bool isFirstReplyOfSession = false;

    // 🔥 Prevent STT spam / multiple requests
    private bool isProcessing = false;
    private string lastProcessedText = "";
    private float lastProcessedTime = 0f;

    void Start()
    {
        if (sendButtonObject != null)
        {
            sendButtonObject.SetActive(false); // Hide send button for the demo
        }

        if (autoStart)
        {
            StartNewSession();
        }
    }

    void Update()
    {
        // Detect Enter / Return key press for confirmation
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputField != null && !string.IsNullOrEmpty(inputField.text))
            {
                Debug.Log("[INPUT] Keyboard confirm triggered");
                OnSend();
            }
        }
    }

    public void StartNewSession()
    {
        isProcessing = false; // Reset lock for new session
        isFirstReplyOfSession = true;
        if (api != null)
        {
            api.currentBuyerName = "";
            api.currentBuyerOrigin = "";
            api.currentSpiceName = "";
            api.currentSpiceQuantity = "";
        }
        StartCoroutine(api.StartSession(OnNPCReply));
    }

    public void ResetConversationUI(string statusText = "Customer approaching...")
    {
        if (npcText != null)
        {
            npcText.text = statusText;
        }

        ClearSubtitle();

        if (hudManager != null)
        {
            hudManager.HideCurrentTrade();
        }

        if (inputField != null)
        {
            inputField.text = "";
            inputField.interactable = false;
        }
        
        lastProcessedText = ""; // Reset STT filter history for the new customer
    }

    public void ClearSubtitle()
    {
        if (subtitleHideCoroutine != null)
        {
            StopCoroutine(subtitleHideCoroutine);
            subtitleHideCoroutine = null;
        }
        if (hudManager != null)
        {
            hudManager.ClearSubtitle();
        }
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
        Debug.Log($"[THINK] Request Sent: {text}");

        // 1. Trigger the thinking behavior if feedbackManager is assigned
        Animator npcAnim = null;
        if (marketplaceManager != null && marketplaceManager.buyerNPC != null)
        {
            npcAnim = marketplaceManager.buyerNPC.GetComponent<Animator>();
            if (npcAnim == null)
            {
                npcAnim = marketplaceManager.buyerNPC.GetComponentInChildren<Animator>();
            }
        }

        if (feedbackManager != null)
        {
            Debug.Log("[THINK] Calling feedbackManager.StartNPCThinking");
            feedbackManager.StartNPCThinking(npcAnim, npcText, true);
        }
        else
        {
            Debug.LogError("[THINK] feedbackManager is NULL in ChatManager!");
        }

        yield return api.SendMessage(text, OnNPCReply);

        // 🔥 cooldown to prevent API spam (VERY IMPORTANT)
        yield return new WaitForSeconds(2.5f);

        isProcessing = false;
    }

    // 🤖 NPC RESPONSE (unchanged but safer)
    void OnNPCReply(string text, string audioUrl, int reputation, int totalVarahas, bool done, TransactionSummary transaction)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[REP HUD] {reputation}");
        }

        Debug.Log("NPC Reply: " + text);

        // 1. Stop thinking animations
        Animator npcAnim = null;
        if (marketplaceManager != null && marketplaceManager.buyerNPC != null)
        {
            npcAnim = marketplaceManager.buyerNPC.GetComponent<Animator>();
            if (npcAnim == null)
            {
                npcAnim = marketplaceManager.buyerNPC.GetComponentInChildren<Animator>();
            }
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
            if (npcAnim != null)
            {
                npcAnim.SetBool("isTalking", false);
                Debug.Log("[ANIM] Talking OFF");
            }
            EnableConversationUI();
            isProcessing = false;
            return;
        }

        // Check if this is the first greeting reply of the session
        if (isFirstReplyOfSession)
        {
            isFirstReplyOfSession = false;
            if (inputField != null)
            {
                inputField.interactable = false; // Lock inputs during introduction sequence
            }
            StartCoroutine(FirstReplyIntroRoutine(text, audioUrl, reputation, totalVarahas, done, transaction, npcAnim));
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

        // Update Level 1 HUD Economy metrics
        if (hudManager != null)
        {
            hudManager.UpdateMoney(totalVarahas);
            hudManager.UpdateRespect(reputation);
            TriggerSubtitleDisplay(!string.IsNullOrEmpty(api.currentBuyerName) ? api.currentBuyerName : "Customer", text);
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

        if (done && hudManager != null)
        {
            hudManager.HideCurrentTrade();
            if (transaction != null)
            {
                hudManager.ShowTradeComplete(transaction);
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
            marketplaceManager.OnNegotiationFinished(transaction != null);
        }
    }

    private IEnumerator FirstReplyIntroRoutine(string text, string audioUrl, int reputation, int totalVarahas, bool done, TransactionSummary transaction, Animator npcAnim)
    {
        // Stop browsing/thinking state and look at player when the response arrives
        if (feedbackManager != null)
        {
            feedbackManager.StopNPCThinking(npcAnim);
        }
        else if (npcAnim != null)
        {
            npcAnim.SetBool("isThinking", false);
            NPCGazeController gaze = npcAnim.GetComponent<NPCGazeController>();
            if (gaze != null)
            {
                gaze.LookAtPlayer();
            }
        }

        string bName = !string.IsNullOrEmpty(api.currentBuyerName) ? api.currentBuyerName : "Customer";
        string bOrigin = !string.IsNullOrEmpty(api.currentBuyerOrigin) ? api.currentBuyerOrigin : "Merchant";

        // 1. Trigger HUD NPC Introduction Card and active trade details immediately
        if (hudManager != null)
        {
            hudManager.ShowNPCIntro(bName, bOrigin);
            hudManager.ShowCurrentTrade(api.currentSpiceName, api.currentSpiceQuantity, bName);
            hudManager.UpdateMoney(totalVarahas);
            hudManager.UpdateRespect(reputation);
        }

        // 2. Wait exactly 3.0 seconds to allow the intro card to play fully before greeting text/speech
        yield return new WaitForSeconds(3.0f);

        // 3. Render greeting dialogue and subtitles
        if (npcText != null)
        {
            npcText.text = text;
        }

        TriggerSubtitleDisplay(bName, text);

        // 4. Trigger speech audio playback
        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Debug.Log("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }

        // 5. Unlock conversation inputs
        EnableConversationUI();
    }

    private Coroutine subtitleHideCoroutine;

    private void TriggerSubtitleDisplay(string speaker, string text)
    {
        if (hudManager == null) return;

        hudManager.ShowSubtitle(speaker, text);

        if (subtitleHideCoroutine != null)
        {
            StopCoroutine(subtitleHideCoroutine);
        }
        subtitleHideCoroutine = StartCoroutine(SubtitleHideRoutine());
    }

    private IEnumerator SubtitleHideRoutine()
    {
        // Give audio a tiny fraction of a second to start loading / playing if triggered concurrently
        yield return new WaitForSeconds(0.3f);

        // 1. If audio is playing, wait until it finishes
        if (audioManager != null && audioManager.audioSource != null)
        {
            while (audioManager.audioSource.isPlaying)
            {
                yield return null;
            }
        }

        // 2. Wait exactly 5.0 seconds
        yield return new WaitForSeconds(5.0f);

        // 3. Hide subtitle
        if (hudManager != null)
        {
            hudManager.HideSubtitle();
        }
    }
}