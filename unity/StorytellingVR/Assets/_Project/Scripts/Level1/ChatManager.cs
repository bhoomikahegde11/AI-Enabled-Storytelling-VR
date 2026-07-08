using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[System.Serializable]
public class APIResponse
{
    public string npc_text;
    public string audio_url;
}

public class ChatManager : MonoBehaviour
{
    public class PendingFulfillmentData
    {
        public string spiceName;
        public string quantityLabel;
        public int quantityGrams;
        public int agreedPrice;
        public float buyerTrust;
        public float buyerFrustration;
        public int outOfWorldCount;
    }

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

    [Header("Local Intent Migration")]
    public bool useLocalIntentSystem = false;
    public bool useLocalNpcBrain = false;
    public bool useLocalSessionGeneration = false;
    public bool useOfflineTypedInputFallback = false;
    public bool useLocalLLMUnderstanding = false;
    public bool useLocalLLMGeneration = false;

    [Header("Level 1 HUD References")]
    public Level1HUDManager hudManager;
    public GameObject sendButtonObject;

    [Header("Standalone NPC Audio")]
    public bool enableNpcTTS = true;
    public string debugTtsCharacterOverride = "";

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    private bool isFirstReplyOfSession = false;
    private readonly NegotiationStateManager negotiationStateManager = new NegotiationStateManager();
    private readonly RuleBasedNPCBrain localNpcBrain = new RuleBasedNPCBrain();
    private readonly DialogueTableResponseProvider dialogueTableResponseProvider = new DialogueTableResponseProvider();
    private readonly LocalLLMInterpreter localLlmInterpreter = new LocalLLMInterpreter();
    private readonly LocalLLMDialogueGenerator localLlmDialogueGenerator = new LocalLLMDialogueGenerator();
    private int localDialogueTurnId = 0;
    private bool hasPlayedGreetingForCurrentCustomer = false;
    private string lastCustomerCharacterId = string.Empty;
    private Coroutine ttsSubtitleFallbackCoroutine;
    private string pendingTtsSubtitleSpeaker = string.Empty;
    private string pendingTtsSubtitleText = string.Empty;
    private INpcTtsPlaybackAware subscribedTtsPlaybackProvider;
    private bool sessionOutcomeResolved;
    private PendingFulfillmentData pendingFulfillment;

    // 🔥 Prevent STT spam / multiple requests
    private bool isProcessing = false;
    private string lastProcessedText = "";
    private float lastProcessedTime = 0f;

    public bool HasPendingFulfillment => pendingFulfillment != null;
    public PendingFulfillmentData CurrentPendingFulfillment => pendingFulfillment;

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

        if (useOfflineTypedInputFallback && inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Debug.Log("[INPUT] Controller confirm triggered");
                OnSend();
            }
        }
    }

    public void StartNewSession()
    {
        isProcessing = false; // Reset lock for new session
        isFirstReplyOfSession = true;
        hasPlayedGreetingForCurrentCustomer = false;
        sessionOutcomeResolved = false;
        pendingFulfillment = null;
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.CancelMarketplaceFulfillment();
        }
        Level1GameState.Instance.PrepareForNewCustomer();
        negotiationStateManager.ResetState(0);
        negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, "new customer session");
        if (api != null)
        {
            api.currentBuyerName = "";
            api.currentBuyerOrigin = "";
            api.currentSpiceName = "";
            api.currentSpiceQuantity = "";
        }

        if (useLocalSessionGeneration)
        {
            LocalGeneratedTradeSession localSession = Level1GameState.Instance.GenerateLocalSession();
            negotiationStateManager.ResetState(localSession.startingOffer, localSession.buyerPatience);
            if (marketplaceManager != null)
            {
                marketplaceManager.BeginNegotiationTimer(localSession.buyerPatience);
            }

            if (api != null)
            {
                api.currentBuyerName = localSession.buyerName;
                api.currentBuyerOrigin = localSession.buyerOrigin;
                api.currentSpiceName = localSession.spiceName;
                api.currentSpiceQuantity = localSession.quantityLabel;
            }

            string currentCustomerCharacterId = DialogueCharacterRegistry.NormalizeCharacterId(localSession.buyerName);
            bool isRepeatCustomer = !string.IsNullOrWhiteSpace(currentCustomerCharacterId) &&
                currentCustomerCharacterId == lastCustomerCharacterId;
            string greetingReply = dialogueTableResponseProvider.GetGreeting(
                Level1GameState.Instance.ActiveTrade,
                Level1GameState.Instance.CurrentReputation,
                isRepeatCustomer);

            if (string.IsNullOrWhiteSpace(greetingReply))
            {
                greetingReply = localSession.greetingText;
            }

            if (!hasPlayedGreetingForCurrentCustomer)
            {
                hasPlayedGreetingForCurrentCustomer = true;
                lastCustomerCharacterId = currentCustomerCharacterId;
                Level1DebugForceAccept.LogVerbose("[GREETING] Triggered for customer: " + localSession.buyerName);
                Level1DebugForceAccept.LogVerbose("[GREETING] Scenario: " + (isRepeatCustomer ? "RepeatCustomerGreeting" : "CustomerGreeting"));
                Level1DebugForceAccept.LogVerbose("[GREETING] Reply: " + greetingReply);
            }

            negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, "local session greeting");

            StartCoroutine(FirstReplyIntroRoutine(
                greetingReply,
                string.Empty,
                Level1GameState.Instance.CurrentReputation,
                Level1GameState.Instance.CurrentMoney,
                false,
                null,
                null,
                Level1GameState.Instance.BuildCurrentTradeForHud(),
                0));
            return;
        }

        if (marketplaceManager != null)
        {
            marketplaceManager.BeginNegotiationTimer(5);
        }

        StartCoroutine(api.StartSession(OnNPCReply));
    }

    public void ResetConversationUI(string statusText = "Customer approaching...")
    {
        hasPlayedGreetingForCurrentCustomer = false;
        negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.None, "conversation ui reset");

        if (npcText != null)
        {
            npcText.text = statusText;
        }

        ClearSubtitle();

        if (hudManager != null)
        {
            hudManager.HideCurrentTrade();
            hudManager.HidePlayerInputPanel();
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
        if (sessionOutcomeResolved)
        {
            return;
        }

        if (marketplaceManager != null)
        {
            marketplaceManager.StartPlayerIdleWindow();
        }

        if (hudManager != null)
        {
            hudManager.ShowPlayerInputPanel();
        }

        if (inputField != null)
        {
            inputField.interactable = true;
            inputField.ActivateInputField(); // Focus the input field automatically for seamless typing
        }
    }

    // 📝 TEXT INPUT (unchanged behavior)
    public void OnSend()
    {
        if (isProcessing || sessionOutcomeResolved) return;

        string playerText = inputField.text;

        if (string.IsNullOrEmpty(playerText)) return;

        isProcessing = true;
        if (marketplaceManager != null)
        {
            marketplaceManager.MarkMeaningfulPlayerInput();
        }
        if (hudManager != null)
        {
            hudManager.HidePlayerInputPanel();
        }

        StartCoroutine(SendMessageRoutine(playerText));

        inputField.text = "";
    }

    public void SubmitOfflineText(string text)
    {
        if (!useOfflineTypedInputFallback || string.IsNullOrWhiteSpace(text) || isProcessing || sessionOutcomeResolved)
        {
            return;
        }

        if (inputField != null)
        {
            inputField.text = text;
        }

        OnSend();
    }

    // 🎤 VOICE INPUT (fixed + throttled)
public void OnVoiceInput(string spokenText)
{
    if (isProcessing || sessionOutcomeResolved) return;

    isProcessing = true;

    if (string.IsNullOrEmpty(spokenText))
    {
        isProcessing = false;
        return;
    }
    if (spokenText == lastProcessedText)
    {
        isProcessing = false;
        return;
    }

        // 🔥 Cooldown check (3 seconds)
        if (Time.time - lastProcessedTime < 3f)
        {
            isProcessing = false;
            return;
        }

        Level1DebugForceAccept.LogVoice("[VOICE INPUT] " + spokenText);

        lastProcessedText = spokenText;
        lastProcessedTime = Time.time;
        if (marketplaceManager != null)
        {
            marketplaceManager.MarkMeaningfulPlayerInput();
        }
        if (hudManager != null)
        {
            hudManager.HidePlayerInputPanel();
        }

        if (npcText != null)
            npcText.text = "You: " + spokenText;

        StartCoroutine(SendMessageRoutine(spokenText));
    }

    // 🔁 COMMON SEND ROUTINE (prevents duplication)
    IEnumerator SendMessageRoutine(string text)
    {
        Level1DebugForceAccept.LogVerbose($"[THINK] Request Sent: {text}");

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
            Level1DebugForceAccept.LogVerbose("[THINK] Calling feedbackManager.StartNPCThinking");
            feedbackManager.StartNPCThinking(npcAnim, npcText, true);
        }
        else
        {
            Debug.LogError("[THINK] feedbackManager is NULL in ChatManager!");
        }

        if (useLocalNpcBrain || useLocalSessionGeneration)
        {
            HandleLocalNpcTurn(text, npcAnim);
            yield return new WaitForSeconds(0.2f);
            isProcessing = false;
            yield break;
        }

        if (useLocalIntentSystem)
        {
            NegotiationInput localInput = BuildNegotiationInput(text, Level1GameState.Instance.ActiveTrade);
            negotiationStateManager.ProcessNegotiationTurn(localInput);
        }

        yield return api.SendMessage(text, OnNPCReply);

        // 🔥 cooldown to prevent API spam (VERY IMPORTANT)
        yield return new WaitForSeconds(2.5f);

        isProcessing = false;
    }

    private void HandleLocalNpcTurn(string playerText, Animator npcAnim)
    {
        Level1GameState localGameState = Level1GameState.Instance;
        LocalTradeState trade = localGameState.ActiveTrade;

        if (trade == null)
        {
            if (feedbackManager != null)
            {
                feedbackManager.StopNPCThinking(npcAnim);
            }
            npcText.text = "The market is too noisy, could you repeat that?";
            EnableConversationUI();
            return;
        }

        NegotiationInput localInput = BuildNegotiationInput(playerText, trade);
        if (localInput.hasQuantity && localInput.quantityGrams > 0)
        {
            localGameState.UpdateActiveTradeQuantity(localInput.quantityGrams);
            trade = localGameState.ActiveTrade;
        }
        negotiationStateManager.ProcessNegotiationTurn(localInput);

        RuleBasedNPCBrainResult brainResult = localNpcBrain.GenerateReply(
            playerText,
            localInput,
            trade,
            negotiationStateManager.CurrentRound,
            negotiationStateManager.BuyerPatience
        );
        string fallbackReplyText = brainResult.replyText;
        int dialogueTurnId = ++localDialogueTurnId;

        string tableReplyText = fallbackReplyText;
        try
        {
            string resolvedReply = dialogueTableResponseProvider.GetReply(
                localInput,
                trade,
                brainResult,
                negotiationStateManager.CurrentRound);

            if (!string.IsNullOrWhiteSpace(resolvedReply))
            {
                tableReplyText = resolvedReply;
            }
        }
        catch (System.Exception ex)
        {
            if (Level1DebugForceAccept.VerboseLogsEnabled())
            {
                Debug.LogWarning("[DIALOGUE-TABLE] Provider failed, using rule reply. Reason: " + ex.Message);
            }
        }

        brainResult.replyText = tableReplyText;

        localGameState.UpdateActiveTradeOffer(brainResult.updatedOffer);
        negotiationStateManager.SetLastOffer(brainResult.updatedOffer);

        if (feedbackManager != null)
        {
            feedbackManager.StopNPCThinking(npcAnim);
        }

        npcText.text = brainResult.replyText;

        CurrentTrade currentTrade = localGameState.BuildCurrentTradeForHud();
        int localMoney = localGameState.CurrentMoney;
        int localReputation = localGameState.CurrentReputation;
        int localReputationDelta = 0;
        TransactionSummary localTransaction = null;

        if (hudManager != null)
        {
            hudManager.UpdateMoney(localMoney);
            hudManager.UpdateRespect(localReputation);
            if (currentTrade != null)
            {
                hudManager.UpdateCurrentTrade(currentTrade);
            }
        }

        PresentNpcSubtitleAndTts(
            !string.IsNullOrEmpty(trade.buyerName) ? trade.buyerName : "Customer",
            brainResult.replyText,
            DialogueCharacterRegistry.NormalizeCharacterId(trade.buyerName));

        if (useLocalLLMGeneration)
        {
            StartCoroutine(ApplyLocalLlmDialogueWhenReady(
                dialogueTurnId,
                playerText,
                localInput,
                trade,
                brainResult,
                fallbackReplyText));
        }

        if (brainResult.isFinished)
        {
            string action = brainResult.resolutionAction;
            int resolvedPrice = brainResult.resolvedPrice > 0 ? brainResult.resolvedPrice : brainResult.updatedOffer;
            int resolvedQuantityGrams = brainResult.resolvedQuantityGrams > 0 ? brainResult.resolvedQuantityGrams : trade.quantityGrams;

            if (brainResult.isAccepted)
            {
                negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.ExpectFulfillment, "local accepted trade");
                BeginAcceptedFulfillment(trade, resolvedPrice, resolvedQuantityGrams, brainResult.trust, brainResult.frustration, brainResult.outOfWorldCount);
                return;
            }

            sessionOutcomeResolved = true;
            negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.None, "local negotiation finished");
            LocalTradeOutcome localOutcome = localGameState.ResolveTradeFromBackend(
                action,
                resolvedPrice,
                resolvedQuantityGrams,
                brainResult.trust,
                brainResult.frustration,
                brainResult.outOfWorldCount
            );

            localMoney = localOutcome.currentMoney;
            localReputation = localOutcome.currentReputation;
            localReputationDelta = localOutcome.reputationDelta;
            localTransaction = localOutcome.transaction;

            if (respectUIManager != null)
            {
                respectUIManager.SetRespect(localReputation);
            }

            if (coinsEarnedText != null)
            {
                coinsEarnedText.text = "Coins Earned: " + localMoney;
            }

            if (hudManager != null)
            {
                hudManager.UpdateMoney(localMoney);
                hudManager.UpdateRespect(localReputation);
                hudManager.HideCurrentTrade();
                if (localReputationDelta != 0)
                {
                    hudManager.ShowReputationChange(localReputationDelta);
                }
                hudManager.ShowTradeComplete(localTransaction, brainResult.isAccepted, localReputationDelta);
            }

            if (feedbackManager != null)
            {
                if (brainResult.isAccepted && localTransaction != null)
                {
                    string archetype = "Standard Merchant";
                    if (localReputation >= 80) archetype = "Fair Trader";
                    else if (localReputation <= 35) archetype = "Greedy Haggler";
                    feedbackManager.ShowTransactionFeedback(localTransaction, archetype);
                }
                else
                {
                    feedbackManager.TriggerRespectToast(localReputationDelta != 0 ? localReputationDelta : -5);
                }
            }

            if (marketplaceManager != null)
            {
                marketplaceManager.OnNegotiationFinished(brainResult.isAccepted);
            }
        }
        else
        {
            negotiationStateManager.UpdateExpectedReplyStateFromNpcReply(brainResult.replyText, false, false);
        }

        EnableConversationUI();
    }

    private IEnumerator ApplyLocalLlmDialogueWhenReady(int dialogueTurnId, string playerText, NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult, string fallbackReplyText)
    {
        Task<LocalLLMDialogueGenerator.GenerationResult> task = localLlmDialogueGenerator.BeginGenerate(dialogueTurnId, playerText, input, trade, brainResult);
        float startedAt = Time.unscaledTime;
        float waitLimit = localLlmDialogueGenerator.TimeoutSeconds;

        while (!task.IsCompleted && (Time.unscaledTime - startedAt) < waitLimit)
        {
            if (dialogueTurnId != localDialogueTurnId)
            {
                Debug.Log("[LLM-GEN] Not applied reason: stale turn before completion");
                yield break;
            }
            yield return null;
        }

        if (!task.IsCompleted)
        {
            Debug.Log("[LLM-GEN] Timeout after seconds: " + (Time.unscaledTime - startedAt).ToString("0.00"));
            Debug.Log("[LLM-GEN] Not applied reason: task still running");
            yield break;
        }

        Debug.Log("[LLM-GEN] Task completed: true");

        if (dialogueTurnId != localDialogueTurnId)
        {
            Debug.Log("[LLM-GEN] Not applied reason: stale turn after completion");
            yield break;
        }

        if (sessionOutcomeResolved)
        {
            Debug.Log("[LLM-GEN] Not applied reason: negotiation already resolved");
            yield break;
        }

        if (task.IsFaulted)
        {
            Debug.Log("[LLM-GEN] Not applied reason: task faulted");
            yield break;
        }

        if (task.Result == null)
        {
            Debug.Log("[LLM-GEN] Not applied reason: null result");
            yield break;
        }

        LocalLLMDialogueGenerator.GenerationResult generation = task.Result;
        Debug.Log("[LLM-GEN] Raw output: " + generation.rawOutput);
        Debug.Log("[LLM-GEN] Cleaned output: " + generation.cleanedOutput);
        Debug.Log("[LLM-GEN] Validation passed: " + generation.validationPassed);
        if (!string.IsNullOrWhiteSpace(generation.fallbackReason))
        {
            if (generation.fallbackReason == "validation failed")
            {
                Debug.Log("[LLM-GEN] Validation failed reason: " + generation.validationFailureReason);
            }
            if (generation.fallbackReason == "timeout")
            {
                Debug.Log("[LLM-GEN] Timeout after seconds: " + generation.elapsedSeconds.ToString("0.00"));
            }
            Debug.Log("[LLM-GEN] Not applied reason: " + generation.fallbackReason);
            yield break;
        }

        string generatedLine = generation.finalLine;
        if (string.IsNullOrWhiteSpace(generatedLine) || generatedLine == fallbackReplyText)
        {
            Debug.Log("[LLM-GEN] Not applied reason: empty or unchanged line");
            yield break;
        }
        Debug.Log("[LLM-GEN] Final line: " + generatedLine);

        if (npcText != null)
        {
            npcText.text = generatedLine;
        }

        string speaker = trade != null && !string.IsNullOrEmpty(trade.buyerName) ? trade.buyerName : "Customer";
        TriggerSubtitleDisplay(speaker, generatedLine);
        Debug.Log("[LLM-GEN] Applied to subtitle: true");
    }

    private NegotiationInput BuildNegotiationInput(string playerText, LocalTradeState trade)
    {
        return negotiationStateManager.ClassifyInput(playerText, trade);
    }

    // 🤖 NPC RESPONSE (unchanged but safer)
    void OnNPCReply(string text, string audioUrl, int reputation, int totalVarahas, bool done, TransactionSummary transaction, string action, CurrentTrade currentTrade, int reputationDelta)
    {
        if (sessionOutcomeResolved)
        {
            if (showDebugLogs)
            {
            Level1DebugForceAccept.LogVerbose("[NPC REPLY] Ignoring late reply because the negotiation has already been resolved.");
            }
            return;
        }

        Level1DebugForceAccept.LogVerbose("NPC Reply: " + text);

        Level1GameState localGameState = Level1GameState.Instance;
        SyncLocalTradeState(currentTrade);

        CurrentTrade localCurrentTrade = localGameState.BuildCurrentTradeForHud();
        int localReputation = localGameState.CurrentReputation;
        int localMoney = localGameState.CurrentMoney;
        int localReputationDelta = 0;
        TransactionSummary localTransaction = transaction;
        bool pendingAcceptedFulfillment = false;
        int pendingAcceptedPrice = 0;
        int pendingAcceptedQuantity = 0;
        float pendingAcceptedTrust = 0f;
        float pendingAcceptedFrustration = 0f;
        int pendingAcceptedOutOfWorldCount = 0;

        if (done)
        {
            pendingAcceptedPrice = GetLatestResponsePrice();
            pendingAcceptedQuantity = GetLatestResponseQuantity();
            pendingAcceptedTrust = GetLatestBuyerTrust();
            pendingAcceptedFrustration = GetLatestBuyerFrustration();
            pendingAcceptedOutOfWorldCount = GetLatestOutOfWorldCount();

            if (string.Equals(action, "ACCEPT", System.StringComparison.OrdinalIgnoreCase))
            {
                pendingAcceptedFulfillment = true;
                negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.ExpectFulfillment, "backend accepted trade");
            }
            else
            {
                sessionOutcomeResolved = true;
                negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.None, "backend negotiation finished");
                LocalTradeOutcome localOutcome = localGameState.ResolveTradeFromBackend(
                    action,
                    pendingAcceptedPrice,
                    pendingAcceptedQuantity,
                    pendingAcceptedTrust,
                    pendingAcceptedFrustration,
                    pendingAcceptedOutOfWorldCount
                );

                localReputation = localOutcome.currentReputation;
                localMoney = localOutcome.currentMoney;
                localReputationDelta = localOutcome.reputationDelta;
                localTransaction = localOutcome.transaction;
            }
        }

        if (showDebugLogs)
        {
            Level1DebugForceAccept.LogTrade($"[TRADE] Respect={localReputation} (delta: {localReputationDelta})");
            if (useLocalIntentSystem)
            {
                Level1DebugForceAccept.LogVerbose($"[LOCAL INTENT] round={negotiationStateManager.CurrentRound}, patience={negotiationStateManager.BuyerPatience}, finished={negotiationStateManager.IsNegotiationFinished}, lastIntent={negotiationStateManager.LastIntent}");
            }
        }

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
            StartCoroutine(FirstReplyIntroRoutine(text, audioUrl, localReputation, localMoney, done, localTransaction, npcAnim, localCurrentTrade, localReputationDelta));
            return;
        }

        npcText.text = text;

        if (respectUIManager != null)
        {
            respectUIManager.SetRespect(localReputation);
        }

        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = "Coins Earned: " + localMoney;
        }

        // Update Level 1 HUD Economy metrics
        if (hudManager != null)
        {
            hudManager.UpdateMoney(localMoney);
            hudManager.UpdateRespect(localReputation);
            TriggerSubtitleDisplay(!string.IsNullOrEmpty(api.currentBuyerName) ? api.currentBuyerName : "Customer", text);
            if (localCurrentTrade != null)
            {
                hudManager.UpdateCurrentTrade(localCurrentTrade);
            }
            if (localReputationDelta != 0)
            {
                hudManager.ShowReputationChange(localReputationDelta);
            }
        }

        if (pendingAcceptedFulfillment)
        {
            BeginAcceptedFulfillment(localGameState.ActiveTrade, pendingAcceptedPrice, pendingAcceptedQuantity, pendingAcceptedTrust, pendingAcceptedFrustration, pendingAcceptedOutOfWorldCount);
        }
        else if (!done)
        {
            negotiationStateManager.UpdateExpectedReplyStateFromNpcReply(text, false, false);
        }

        bool isSuccess = (action == "ACCEPT" && localTransaction != null && localTransaction.earned > 0);

        // 2. Trigger transaction completed feedback popups or respect warnings on done
        if (done && !pendingAcceptedFulfillment && feedbackManager != null)
        {
            if (isSuccess)
            {
                // Determine player archetype based on reputation score
                string archetype = "Standard Merchant";
                if (localReputation >= 80) archetype = "Fair Trader";
                else if (localReputation <= 35) archetype = "Greedy Haggler";

                feedbackManager.ShowTransactionFeedback(localTransaction, archetype);
            }
            else
            {
                feedbackManager.TriggerRespectToast(localReputationDelta != 0 ? localReputationDelta : -5);
            }
        }

        if (done && !pendingAcceptedFulfillment && hudManager != null)
        {
            hudManager.HideCurrentTrade();
            hudManager.ShowTradeComplete(localTransaction, isSuccess, localReputationDelta);
        }

        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Level1DebugForceAccept.LogVerbose("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }
        else
        {
            Debug.LogWarning("Audio URL missing or AudioManager not assigned!");
        }

        if (done && !pendingAcceptedFulfillment && marketplaceManager != null)
        {
            marketplaceManager.OnNegotiationFinished(isSuccess);
        }

        if (currentTrade != null)
        {
            negotiationStateManager.SetLastOffer(currentTrade.npc_offer);
        }
    }

    private IEnumerator FirstReplyIntroRoutine(string text, string audioUrl, int reputation, int totalVarahas, bool done, TransactionSummary transaction, Animator npcAnim, CurrentTrade currentTrade, int reputationDelta)
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
            if (currentTrade != null)
            {
                hudManager.ShowCurrentTrade(currentTrade.spice, currentTrade.quantity, bName);
                hudManager.UpdateCurrentTrade(currentTrade);
                negotiationStateManager.ResetState(currentTrade.npc_offer);
            }
            hudManager.UpdateMoney(totalVarahas);
            hudManager.UpdateRespect(reputation);
            if (reputationDelta != 0)
            {
                hudManager.ShowReputationChange(reputationDelta);
            }
        }

        negotiationStateManager.UpdateExpectedReplyStateFromNpcReply(text, done, false);

        // 2. Wait exactly 3.0 seconds to allow the intro card to play fully before greeting text/speech
        yield return new WaitForSeconds(3.0f);

        // 3. Render greeting dialogue and subtitles
        if (npcText != null)
        {
            npcText.text = text;
        }

        if (useLocalSessionGeneration || useLocalNpcBrain)
        {
            PresentNpcSubtitleAndTts(bName, text, DialogueCharacterRegistry.NormalizeCharacterId(bName));
        }
        else
        {
            TriggerSubtitleDisplay(bName, text);
        }

        // 4. Trigger speech audio playback
        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Level1DebugForceAccept.LogVerbose("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }

        // 5. Unlock conversation inputs
        EnableConversationUI();
    }

    private bool TrySpeakNpcReply(string replyText, string characterId = "")
    {
        if (!enableNpcTTS)
        {
            Level1DebugForceAccept.LogVerbose("[TTS] Skipped: NPC TTS disabled");
            return false;
        }

        if (string.IsNullOrWhiteSpace(replyText))
        {
            Level1DebugForceAccept.LogVerbose("[TTS] Skipped: empty NPC reply");
            return false;
        }

        if (audioManager == null)
        {
            Debug.LogWarning("[TTS] Skipped: AudioManager missing");
            return false;
        }

#if UNITY_EDITOR
        string ttsCharacterId = !string.IsNullOrWhiteSpace(debugTtsCharacterOverride)
            ? debugTtsCharacterOverride
            : characterId;
#else
        string ttsCharacterId = characterId;
#endif

        string cleanedReplyText = SanitizeTextForTts(replyText);
        if (string.IsNullOrWhiteSpace(cleanedReplyText))
        {
            cleanedReplyText = replyText;
        }

        Level1DebugForceAccept.LogVerbose("[TTS] Speaking NPC reply: " + cleanedReplyText);
        Level1DebugForceAccept.LogVerbose("[TTS] Character: " + ttsCharacterId);

        return audioManager.TrySpeakText(cleanedReplyText, ttsCharacterId);
    }

    private void PresentNpcSubtitleAndTts(string speaker, string replyText, string characterId)
    {
        if (!enableNpcTTS || audioManager == null)
        {
            TriggerSubtitleDisplay(speaker, replyText);
            TrySpeakNpcReply(replyText, characterId);
            return;
        }

        INpcTtsPlaybackAware playbackAwareProvider = audioManager.localNpcTtsProvider as INpcTtsPlaybackAware;
        if (playbackAwareProvider == null)
        {
            TriggerSubtitleDisplay(speaker, replyText);
            TrySpeakNpcReply(replyText, characterId);
            return;
        }

        CleanupPendingTtsSubtitleWait();
        pendingTtsSubtitleSpeaker = speaker;
        pendingTtsSubtitleText = replyText;
        subscribedTtsPlaybackProvider = playbackAwareProvider;
        subscribedTtsPlaybackProvider.PlaybackStarted += OnNpcTtsPlaybackStarted;
        subscribedTtsPlaybackProvider.PlaybackFailed += OnNpcTtsPlaybackFailed;

        bool ttsAccepted = TrySpeakNpcReply(replyText, characterId);
        if (!ttsAccepted)
        {
            CleanupPendingTtsSubtitleWait();
            TriggerSubtitleDisplay(speaker, replyText);
            return;
        }

        ttsSubtitleFallbackCoroutine = StartCoroutine(TtsSubtitleFallbackRoutine(5f));
    }

    private void OnNpcTtsPlaybackStarted()
    {
        if (string.IsNullOrWhiteSpace(pendingTtsSubtitleText))
        {
            CleanupPendingTtsSubtitleWait();
            return;
        }

        TriggerSubtitleDisplay(pendingTtsSubtitleSpeaker, pendingTtsSubtitleText);
        CleanupPendingTtsSubtitleWait();
    }

    private void OnNpcTtsPlaybackFailed(string reason)
    {
        Debug.LogWarning("[TTS] Failed reason: " + reason);

        if (!string.IsNullOrWhiteSpace(pendingTtsSubtitleText))
        {
            TriggerSubtitleDisplay(pendingTtsSubtitleSpeaker, pendingTtsSubtitleText);
        }

        CleanupPendingTtsSubtitleWait();
    }

    private IEnumerator TtsSubtitleFallbackRoutine(float timeoutSeconds)
    {
        yield return new WaitForSeconds(timeoutSeconds);

        if (!string.IsNullOrWhiteSpace(pendingTtsSubtitleText))
        {
            Debug.LogWarning("[TTS] Failed reason: playback start timeout");
            TriggerSubtitleDisplay(pendingTtsSubtitleSpeaker, pendingTtsSubtitleText);
        }

        CleanupPendingTtsSubtitleWait();
    }

    private void CleanupPendingTtsSubtitleWait()
    {
        if (ttsSubtitleFallbackCoroutine != null)
        {
            StopCoroutine(ttsSubtitleFallbackCoroutine);
            ttsSubtitleFallbackCoroutine = null;
        }

        if (subscribedTtsPlaybackProvider != null)
        {
            subscribedTtsPlaybackProvider.PlaybackStarted -= OnNpcTtsPlaybackStarted;
            subscribedTtsPlaybackProvider.PlaybackFailed -= OnNpcTtsPlaybackFailed;
            subscribedTtsPlaybackProvider = null;
        }

        pendingTtsSubtitleSpeaker = string.Empty;
        pendingTtsSubtitleText = string.Empty;
    }

    private static string SanitizeTextForTts(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        string cleaned = Regex.Replace(text, "\\([^\\)]*\\)", string.Empty);
        cleaned = Regex.Replace(cleaned, "\\[[^\\]]*\\]", string.Empty);
        cleaned = Regex.Replace(cleaned, "\\s+", " ").Trim();
        return cleaned;
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

    private void SyncLocalTradeState(CurrentTrade currentTrade)
    {
        if (api == null)
        {
            return;
        }

        MarketEventData activeEvent = api.LastStepResponse != null ? api.LastStepResponse.active_event : null;
        if (activeEvent == null && api.LastStartResponse != null)
        {
            activeEvent = api.LastStartResponse.active_event;
        }

        int quantityGrams = GetLatestResponseQuantity();
        int npcOffer = currentTrade != null ? currentTrade.npc_offer : GetLatestResponsePrice();

        Level1GameState.Instance.SyncTradeFromBackend(
            api.currentBuyerName,
            api.currentBuyerOrigin,
            api.currentSpiceName,
            api.currentSpiceQuantity,
            quantityGrams,
            npcOffer,
            activeEvent
        );
    }

    private int GetLatestResponseQuantity()
    {
        if (api != null && api.LastStepResponse != null)
        {
            return api.LastStepResponse.quantity;
        }

        if (api != null && api.LastStartResponse != null)
        {
            return api.LastStartResponse.quantity;
        }

        return 0;
    }

    private int GetLatestResponsePrice()
    {
        if (api != null && api.LastStepResponse != null)
        {
            return api.LastStepResponse.price;
        }

        if (api != null && api.LastStartResponse != null)
        {
            return api.LastStartResponse.price;
        }

        return 0;
    }

    private float GetLatestBuyerTrust()
    {
        if (api != null && api.LastStepResponse != null)
        {
            return api.LastStepResponse.buyer_trust;
        }

        if (api != null && api.LastStartResponse != null)
        {
            return api.LastStartResponse.buyer_trust;
        }

        return 0.5f;
    }

    private float GetLatestBuyerFrustration()
    {
        if (api != null && api.LastStepResponse != null)
        {
            return api.LastStepResponse.buyer_frustration;
        }

        if (api != null && api.LastStartResponse != null)
        {
            return api.LastStartResponse.buyer_frustration;
        }

        return 0.1f;
    }

    private int GetLatestOutOfWorldCount()
    {
        if (api != null && api.LastStepResponse != null)
        {
            return api.LastStepResponse.out_of_world_count;
        }

        if (api != null && api.LastStartResponse != null)
        {
            return api.LastStartResponse.out_of_world_count;
        }

        return 0;
    }

    private void BeginAcceptedFulfillment(LocalTradeState trade, int agreedPrice, int quantityGrams, float trust, float frustration, int outOfWorldCount)
    {
        if (trade == null)
        {
            Debug.LogWarning("[FULFILLMENT] Cannot enter pending fulfillment because there is no active trade.");
            return;
        }

        sessionOutcomeResolved = true;
        isProcessing = false;

        pendingFulfillment = new PendingFulfillmentData
        {
            spiceName = trade.spiceDisplayName,
            quantityLabel = trade.quantityLabel,
            quantityGrams = quantityGrams > 0 ? quantityGrams : trade.quantityGrams,
            agreedPrice = agreedPrice,
            buyerTrust = trust,
            buyerFrustration = frustration,
            outOfWorldCount = outOfWorldCount
        };

        if (showDebugLogs)
        {
            Level1DebugForceAccept.LogTrade($"[FULFILLMENT] Start. Spice={pendingFulfillment.spiceName}, Quantity={pendingFulfillment.quantityLabel}, Price={pendingFulfillment.agreedPrice}");
        }

        if (marketplaceManager != null)
        {
            marketplaceManager.StopNegotiationTimer();
        }

        negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.ExpectFulfillment, "accepted fulfillment started");

        if (inputField != null)
        {
            inputField.text = string.Empty;
            inputField.interactable = false;
        }

        if (hudManager != null)
        {
            hudManager.HidePlayerInputPanel();
        }

        OrderManager orderManager = OrderManager.Instance;
        if (orderManager == null)
        {
            orderManager = FindFirstObjectByType<OrderManager>();
        }

        if (orderManager != null)
        {
            orderManager.BeginMarketplaceFulfillment(pendingFulfillment.spiceName);
        }
        else
        {
            Debug.LogWarning("[FULFILLMENT] Pending accepted trade created, but OrderManager was not found.");
        }

        if (Level1DebugForceAccept.ShouldBypassScoopFulfillment())
        {
            Debug.Log("[TEMP DEBUG] bypassScoopFulfillmentForTesting is enabled. Completing accepted fulfillment without scooping.");
            CompleteAcceptedFulfillment();
        }
    }

    public bool TryHandleNegotiationTimeout(string finalLine = "")
    {
        if (sessionOutcomeResolved)
        {
            return false;
        }

        sessionOutcomeResolved = true;
        isProcessing = false;
        negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.None, "negotiation timeout");

        if (inputField != null)
        {
            inputField.text = string.Empty;
            inputField.interactable = false;
        }
        if (hudManager != null)
        {
            hudManager.HidePlayerInputPanel();
        }

        ClearSubtitle();
        CleanupPendingTtsSubtitleWait();

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

        Level1GameState localGameState = Level1GameState.Instance;
        LocalTradeState trade = localGameState.ActiveTrade;
        if (trade == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("[TIMEOUT] Negotiation silence timeout expired, but there was no active trade to resolve.");
            }

            if (marketplaceManager != null)
            {
                marketplaceManager.OnNegotiationFinished(false);
            }
            return false;
        }

        float frustration = Mathf.Max(trade.buyerFrustration, 0.8f);
        LocalTradeOutcome localOutcome = localGameState.ResolveTradeFromBackend(
            "WALK_AWAY",
            trade.npcOffer,
            trade.quantityGrams,
            trade.buyerTrust,
            frustration,
            trade.outOfWorldCount
        );

        if (showDebugLogs)
        {
            Level1DebugForceAccept.LogTrade($"[TRADE] Customer walked away after silence timeout. Respect delta={localOutcome.reputationDelta}, money={localOutcome.currentMoney}");
        }

        string timeoutLine = !string.IsNullOrWhiteSpace(finalLine)
            ? finalLine
            : "My patience is spent. I will take my leave.";

        if (npcText != null)
        {
            npcText.text = timeoutLine;
        }

        PlayNegotiationIdleReminder(timeoutLine);

        if (respectUIManager != null)
        {
            respectUIManager.SetRespect(localOutcome.currentReputation);
        }

        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = "Coins Earned: " + localOutcome.currentMoney;
        }

        if (hudManager != null)
        {
            hudManager.UpdateMoney(localOutcome.currentMoney);
            hudManager.UpdateRespect(localOutcome.currentReputation);
            hudManager.HideCurrentTrade();
            if (localOutcome.reputationDelta != 0)
            {
                hudManager.ShowReputationChange(localOutcome.reputationDelta);
            }
            hudManager.ShowTradeComplete(localOutcome.transaction, false, localOutcome.reputationDelta);
        }

        if (feedbackManager != null)
        {
            feedbackManager.TriggerRespectToast(localOutcome.reputationDelta != 0 ? localOutcome.reputationDelta : -5);
        }

        if (marketplaceManager != null)
        {
            marketplaceManager.OnNegotiationFinished(false);
        }

        return true;
    }

    public void CompleteAcceptedFulfillment()
    {
        if (pendingFulfillment == null)
        {
            Debug.LogWarning("[FULFILLMENT] CompleteAcceptedFulfillment called with no pending accepted trade.");
            return;
        }

        LocalTradeOutcome localOutcome = Level1GameState.Instance.ResolveTradeFromBackend(
            "ACCEPT",
            pendingFulfillment.agreedPrice,
            pendingFulfillment.quantityGrams,
            pendingFulfillment.buyerTrust,
            pendingFulfillment.buyerFrustration,
            pendingFulfillment.outOfWorldCount
        );

        int localReputation = localOutcome.currentReputation;
        int localMoney = localOutcome.currentMoney;
        int localReputationDelta = localOutcome.reputationDelta;
        TransactionSummary localTransaction = localOutcome.transaction;

        pendingFulfillment = null;
        negotiationStateManager.SetExpectedReplyState(ExpectedReplyState.None, "accepted fulfillment completed");
        Level1DebugForceAccept.LogTrade($"[FULFILLMENT] Complete. Respect={localReputation}, Money={localMoney}");

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.CompleteMarketplaceFulfillment();
        }

        if (respectUIManager != null)
        {
            respectUIManager.SetRespect(localReputation);
        }

        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = "Coins Earned: " + localMoney;
        }

        if (hudManager != null)
        {
            hudManager.UpdateMoney(localMoney);
            hudManager.UpdateRespect(localReputation);
            hudManager.HideCurrentTrade();
            if (localReputationDelta != 0)
            {
                hudManager.ShowReputationChange(localReputationDelta);
            }
            hudManager.ShowTradeComplete(localTransaction, true, localReputationDelta);
        }

        if (feedbackManager != null && localTransaction != null)
        {
            string archetype = "Standard Merchant";
            if (localReputation >= 80) archetype = "Fair Trader";
            else if (localReputation <= 35) archetype = "Greedy Haggler";
            feedbackManager.ShowTransactionFeedback(localTransaction, archetype);
        }

        if (marketplaceManager != null)
        {
            marketplaceManager.OnNegotiationFinished(true);
        }
    }

    // TEMP DEBUG: Reuse the accepted-fulfillment entry path without duplicating trade resolution logic.
    public bool TryForceDebugAcceptCurrentTrade()
    {
        if (sessionOutcomeResolved || pendingFulfillment != null)
        {
            Debug.Log("[TEMP DEBUG] Force accept ignored because the negotiation is already resolved or pending fulfillment.");
            return false;
        }

        LocalTradeState trade = Level1GameState.Instance != null ? Level1GameState.Instance.ActiveTrade : null;
        if (trade == null)
        {
            Debug.Log("[TEMP DEBUG] Force accept ignored because there is no active trade.");
            return false;
        }

        int agreedPrice = GetLatestResponsePrice();
        if (agreedPrice <= 0)
        {
            agreedPrice = trade.npcOffer;
        }

        int quantityGrams = GetLatestResponseQuantity();
        if (quantityGrams <= 0)
        {
            quantityGrams = trade.quantityGrams;
        }

        float trust = GetLatestBuyerTrust();
        float frustration = GetLatestBuyerFrustration();
        int outOfWorldCount = GetLatestOutOfWorldCount();

        Debug.Log($"[TEMP DEBUG] Force accepting trade. Spice={trade.spiceDisplayName}, Quantity={trade.quantityLabel}, Price={agreedPrice}");
        BeginAcceptedFulfillment(trade, agreedPrice, quantityGrams, trust, frustration, outOfWorldCount);
        return pendingFulfillment != null;
    }

    public void PlayNegotiationIdleReminder(string reminderLine)
    {
        if (string.IsNullOrWhiteSpace(reminderLine))
        {
            return;
        }

        LocalTradeState trade = Level1GameState.Instance != null ? Level1GameState.Instance.ActiveTrade : null;
        string buyerName = trade != null && !string.IsNullOrWhiteSpace(trade.buyerName)
            ? trade.buyerName
            : (!string.IsNullOrWhiteSpace(api != null ? api.currentBuyerName : string.Empty) ? api.currentBuyerName : "Customer");
        string characterId = DialogueCharacterRegistry.NormalizeCharacterId(buyerName);

        if (npcText != null)
        {
            npcText.text = reminderLine;
        }

        PresentNpcSubtitleAndTts(buyerName, reminderLine, characterId);
    }
}
