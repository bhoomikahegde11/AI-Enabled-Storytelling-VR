using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MarketplaceManager : MonoBehaviour
{
    [Header("NPC Settings")]
    [Tooltip("The active NPC GameObject in the scene (e.g., BuyerNPC).")]
    public GameObject buyerNPC;
    [SerializeField] private Transform modelAnchor;
    [SerializeField] private GameObject placeholderVisualRoot;
    [SerializeField] private List<DialogueCharacterProfile> characterProfiles = new List<DialogueCharacterProfile>();

    [Header("Movement Points")]
    [Tooltip("Point where the customer starts (e.g., BuyerSpawnPoint).")]
    public Transform spawnPoint;
    
    [Tooltip("Point at the player stall where customers stop and bargain (e.g., BuyerTradePoint).")]
    public Transform tradePoint;
    
    [Tooltip("Point where customers walk to exit the scene (e.g., BuyerExitPoint).")]
    public Transform exitPoint;

    [Header("System References")]
    [Tooltip("The Conversation UI GameObject to enable and disable.")]
    public GameObject conversationUI;
    
    [Tooltip("Reference to the ChatManager script on the GameManager.")]
    public ChatManager chatManager;

    [Header("Animation")]
    [Tooltip("Controller applied to each runtime-spawned customer visual. Movement remains owned by BuyerNPC/NavMeshAgent.")]
    [SerializeField] private RuntimeAnimatorController sharedBuyerAnimationController;

    [Header("Settings")]
    [Tooltip("Movement speed multiplier for the NavMeshAgent.")]
    public float movementSpeed = 1.3f;
    
    [Tooltip("Delay in seconds to wait at the exit point before resetting.")]
    public float resetDelay = 3f;

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    private NavMeshAgent navMeshAgent;
    private static readonly string[] OriginalPlaceholderVisualObjectNames =
    {
        "AvatarBody",
        "AvatarEyelashes",
        "AvatarHead",
        "AvatarLeftCornea",
        "AvatarLeftEyeball",
        "AvatarRightCornea",
        "AvatarRightEyeball",
        "AvatarTeethLower",
        "AvatarTeethUpper",
        "haircut",
        "outfit"
    };

    private Animator rootAnimator;
    private Animator animator;
    private GameObject activeCharacterModelInstance;
    private Animator activeCharacterAnimator;
    private readonly List<GameObject> originalPlaceholderVisualObjects = new List<GameObject>();
    private readonly List<Renderer> originalPlaceholderRenderers = new List<Renderer>();
    private readonly List<Renderer> originalPlaceholderHipsRenderers = new List<Renderer>();
    private LocalGeneratedTradeSession preparedLocalSession;
    private int nextRuntimeSessionId = 1;
    private int activeVisualSessionId = -1;
    private string activeCharacterId = string.Empty;
    private bool isTransitioning = false;
    private bool negotiationWasAccepted = false;
    private Coroutine negotiationIdleCoroutine;
    private Coroutine nextCustomerCountdownCoroutine;
    private Coroutine marketDayStartupCoroutine;
    private bool isAwaitingPlayerInput;
    private bool isOriginalPlaceholderVisible = true;
    private bool keepsHipsActive = true;
    private float playerIdleStartedAt;
    private int reminderStage;
    private int firstReminderSeconds;
    private int secondReminderSeconds;
    private int walkAwaySeconds;
    private static readonly string[] FirstReminderLines =
    {
        "Well?",
        "Take your time, but don't keep me waiting."
    };
    private static readonly string[] SecondReminderLines =
    {
        "I don't have all day. What's your decision?",
        "Other merchants are waiting for my business."
    };
    private static readonly string[] FinalReminderLines =
    {
        "Enough. I am leaving.",
        "You have wasted enough of my time."
    };

    private void Start()
    {
        Level1DebugForceAccept.LogVerbose("[SCENE FLOW] " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " loaded");
        Level1GameState.Instance.EnsureInitialized();
        EnsureCharacterProfilesInitialized();

        // 1. Auto-discover references if they are not manually dragged in Inspector
        if (buyerNPC == null)
        {
            buyerNPC = GameObject.Find("BuyerNPC");
            if (buyerNPC == null) buyerNPC = GameObject.Find("indian m in kurta (1)");
        }

        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("BuyerSpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
        }

        if (tradePoint == null)
        {
            GameObject tp = GameObject.Find("BuyerTradePoint");
            if (tp != null) tradePoint = tp.transform;
        }

        if (exitPoint == null)
        {
            GameObject ep = GameObject.Find("BuyerExitPoint");
            if (ep != null) exitPoint = ep.transform;
        }

        if (conversationUI == null)
        {
            conversationUI = GameObject.Find("ConversationUI");
            if (conversationUI == null) conversationUI = GameObject.Find("speechPoint");
        }

        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }

        // 2. Validate essential references
        if (buyerNPC == null || spawnPoint == null || tradePoint == null || exitPoint == null)
        {
            Debug.LogError("[MarketplaceManager] Critical references (NPC, SpawnPoint, TradePoint, or ExitPoint) are missing in the scene!");
            return;
        }

        // 3. Cache agent and animator
        navMeshAgent = buyerNPC.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogWarning("[MarketplaceManager] BuyerNPC is missing a NavMeshAgent. Adding one dynamically.");
            navMeshAgent = buyerNPC.AddComponent<NavMeshAgent>();
        }
        navMeshAgent.speed = movementSpeed;

        rootAnimator = buyerNPC.GetComponent<Animator>() ?? buyerNPC.GetComponentInChildren<Animator>(true);
        animator = rootAnimator;
        activeCharacterAnimator = rootAnimator;
        CacheOriginalPlaceholderRenderers();
        SetOriginalPlaceholderVisible(true);

        // Auto-mount NPCGazeController if not present
        NPCGazeController gazeController = buyerNPC.GetComponent<NPCGazeController>();
        if (gazeController == null)
        {
            gazeController = buyerNPC.AddComponent<NPCGazeController>();
            Level1DebugForceAccept.LogVerbose("[MarketplaceManager] Automatically added NPCGazeController to BuyerNPC.");
        }

        // 4. Hide Conversation UI Canvas on scene start
        if (conversationUI != null)
        {
            conversationUI.SetActive(false);
        }

        // 5. Configure ChatManager lifecycle hooks
        if (chatManager != null)
        {
            chatManager.autoStart = false; // Disable HTTP start on scene load
            chatManager.marketplaceManager = this; // Subscribe this manager to completed signals
            if (chatManager.hudManager != null)
            {
                chatManager.hudManager.UpdateMoney(Level1GameState.Instance.CurrentMoney);
                chatManager.hudManager.UpdateRespect(Level1GameState.Instance.CurrentReputation);
            }
        }

        // 6. Reset NPC position and begin lifecycle loop
        ResetNPCToSpawnPoint();
        Level1GameState.Instance.StartMarketDay();
        marketDayStartupCoroutine = StartCoroutine(WaitForMarketDayThenStartLifecycle());
    }

    private void OnValidate()
    {
        EnsureCharacterProfilesInitialized();
    }

    private void OnDestroy()
    {
        ClearInstantiatedCharacterModel();
    }

    /// <summary>
    /// Teleports and resets the active NPC safely back to the SpawnPoint.
    /// </summary>
    private void ResetNPCToSpawnPoint()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false; // Disable temporarily to allow coordinate teleportation
        }

        buyerNPC.transform.position = spawnPoint.position;
        buyerNPC.transform.rotation = spawnPoint.rotation;

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true; // Re-enable agent pathing
            navMeshAgent.isStopped = true;
        }

        SetWalkingAnimation(false);

        // Reset gaze target back to player
        NPCGazeController gaze = buyerNPC.GetComponent<NPCGazeController>();
        if (gaze != null)
        {
            gaze.LookAtPlayer();
        }
    }

    public Animator GetActiveNpcAnimator()
    {
        if (activeCharacterAnimator != null)
        {
            return activeCharacterAnimator;
        }

        // A custom visual without an Animator is a safe static fallback. Do not drive the
        // hidden BuyerNPC Animator while that visual is the active customer.
        if (activeCharacterModelInstance != null)
        {
            return null;
        }

        if (rootAnimator == null && buyerNPC != null)
        {
            rootAnimator = buyerNPC.GetComponent<Animator>() ?? buyerNPC.GetComponentInChildren<Animator>(true);
        }

        animator = rootAnimator;
        activeCharacterAnimator = rootAnimator;
        return animator;
    }

    public static bool CanDriveAnimator(Animator targetAnimator)
    {
        return targetAnimator != null &&
               targetAnimator.isActiveAndEnabled &&
               targetAnimator.runtimeAnimatorController != null;
    }

    public NPCGazeController GetBuyerNpcGazeController()
    {
        return buyerNPC != null ? buyerNPC.GetComponent<NPCGazeController>() : null;
    }

    public bool TryConsumePreparedLocalSession(out LocalGeneratedTradeSession session)
    {
        session = preparedLocalSession;
        preparedLocalSession = null;
        return session != null;
    }

    public void AssignCharacterVisualForSession(LocalGeneratedTradeSession session, string lifecycleStage)
    {
        if (session == null)
        {
            return;
        }

        if (session.runtimeSessionId == 0)
        {
            session.runtimeSessionId = nextRuntimeSessionId++;
        }

        if (session.runtimeSessionId == activeVisualSessionId)
        {
            return;
        }

        activeVisualSessionId = session.runtimeSessionId;
        activeCharacterId = session.characterId ?? string.Empty;
        ClearInstantiatedCharacterModel();

        if (!TryGetCharacterProfile(session.characterId, out DialogueCharacterProfile characterProfile))
        {
            SetOriginalPlaceholderVisible(true);
            Debug.LogWarning("[MarketplaceManager] No character profile found for visual assignment. Character ID='" + session.characterId + "'.");
            RefreshNpcRigBindings();
            return;
        }

        string modelName = characterProfile.modelPrefab != null ? characterProfile.modelPrefab.name : "<none>";
        Debug.Log("[MARKET NPC] Assigning model | characterId=" + characterProfile.characterId +
                  " | displayName=" + characterProfile.displayName +
                  " | modelPrefab=" + modelName +
                  " | lifecycleStage=" + lifecycleStage);

        if (characterProfile.modelPrefab == null)
        {
            SetOriginalPlaceholderVisible(true);
            Debug.LogWarning("[MarketplaceManager] No modelPrefab assigned for character '" + characterProfile.displayName + "' (" + characterProfile.characterId + ").");
            RefreshNpcRigBindings();
            return;
        }

        if (modelAnchor == null)
        {
            SetOriginalPlaceholderVisible(true);
            Debug.LogWarning("[MarketplaceManager] modelAnchor is not assigned. Cannot attach custom visual model for character '" + characterProfile.displayName + "' (" + characterProfile.characterId + ").");
            RefreshNpcRigBindings();
            return;
        }

        SetOriginalPlaceholderVisible(false);

        activeCharacterModelInstance = Instantiate(characterProfile.modelPrefab, modelAnchor, false);
        ApplyCharacterVisualOffsets(activeCharacterModelInstance.transform, characterProfile);
        StabilizeInstantiatedVisual(activeCharacterModelInstance);
        ApplyCharacterVisualOffsets(activeCharacterModelInstance.transform, characterProfile);

        activeCharacterAnimator = activeCharacterModelInstance.GetComponent<Animator>() ?? activeCharacterModelInstance.GetComponentInChildren<Animator>(true);
        if (activeCharacterAnimator == null)
        {
            Debug.LogWarning("[MarketplaceManager] Spawned customer visual has no Animator. It will remain static, but the customer lifecycle will continue.");
            animator = null;
        }
        else
        {
            ConfigureCustomModelAnimator(activeCharacterAnimator);
            animator = activeCharacterAnimator;
        }

        RefreshNpcRigBindings();
        LogActiveVisualDiagnostics("post-assignment");
        ValidateNpcVisualState("post-assignment");
    }

    private void ConfigureCustomModelAnimator(Animator customAnimator)
    {
        if (customAnimator == null)
        {
            return;
        }

        // BuyerNPC/NavMeshAgent owns world movement; imported animation may never move this transform.
        customAnimator.applyRootMotion = false;

        if (sharedBuyerAnimationController == null)
        {
            Debug.LogWarning("[MarketplaceManager] Shared buyer animation controller is not assigned. Spawned customer will keep its current controller.");
            return;
        }

        try
        {
            customAnimator.runtimeAnimatorController = sharedBuyerAnimationController;
            customAnimator.applyRootMotion = false;
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("[MarketplaceManager] Could not assign the shared buyer animation controller to spawned customer '" +
                             customAnimator.name + "'. Customer lifecycle will continue. " + exception.Message);
        }
    }

    /// <summary>
    /// Coroutine that drives the complete sequential walk-trade lifecycle.
    /// </summary>
    private IEnumerator StartBargainingLifecycle()
    {
        if (!Level1GameState.Instance.MarketDayStarted || Level1GameState.Instance.MarketDayEnded)
        {
            yield break;
        }

        PrepareIncomingCustomerLifecycle();

        StopNextCustomerCountdown();

        if (chatManager != null)
        {
            chatManager.ClearSubtitle();
        }

        // 1. Reset UI to show "Customer approaching..." and lock inputs immediately
        if (chatManager != null)
        {
            chatManager.ResetConversationUI("Customer approaching...");
        }

        if (conversationUI != null)
        {
            conversationUI.SetActive(true); // Show canvas early so status text is visible
        }

        yield return new WaitForSeconds(1.0f); // Load buffer

        if (Level1GameState.Instance.MarketDayEnded)
        {
            if (conversationUI != null && !Level1GameState.Instance.MarketDayEnded)
            {
                conversationUI.SetActive(false);
            }
            yield break;
        }

        Level1DebugForceAccept.LogTrade("[MARKET LOOP] Customer spawned and approaching stall.");

        // 2. Move NPC to Trade Point and wait until reached
        yield return StartCoroutine(WalkToDestinationRoutine(tradePoint.position));

        // 3. NPC Arrived at Stall - Orient smoothly
        buyerNPC.transform.rotation = tradePoint.rotation;
        Level1DebugForceAccept.LogVerbose("[MarketplaceManager] NPC reached TradePoint. Triggering browsing behavior.");
        EnsureOriginalPlaceholderHiddenIfNeeded();
        ValidateNpcVisualState("arrival");

        // Cache animator
        if (animator == null)
        {
            animator = GetActiveNpcAnimator();
        }

        // 4. Make NPC look at spices using NPCGazeController
        NPCGazeController gaze = buyerNPC.GetComponent<NPCGazeController>();
        if (gaze != null)
        {
            gaze.LookAtSpices();
        }

        if (chatManager != null)
        {
            chatManager.ClearSubtitle();
        }

        // 1 & 2. Trigger idle/thinking behaviour and show browsing subtitle
        if (chatManager != null && chatManager.hudManager != null)
        {
            // Show temporary subtitle: Speaker "Customer", Text "Customer is browsing your goods..."
            chatManager.hudManager.ShowSubtitle("Customer", "Customer is browsing your goods...");
        }

        if (chatManager != null && chatManager.feedbackManager != null)
        {
            chatManager.feedbackManager.StartNPCThinking(GetActiveNpcAnimator(), chatManager.npcText, false);
        }

        ValidateNpcVisualState("conversation-start");

        // 5. Start backend request in parallel
        if (chatManager != null)
        {
            if (chatManager.inputField != null)
            {
                chatManager.inputField.interactable = false; // Lock inputs during arrival and browsing
            }
            chatManager.StartNewSession();
        }

        isTransitioning = false;
    }

    /// <summary>
    /// Invoked dynamically by ChatManager when done = true is returned by the API response.
    /// </summary>
    public void OnNegotiationFinished(bool wasAccepted)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StopNegotiationTimer();

        negotiationWasAccepted = wasAccepted;
        Level1DebugForceAccept.LogTrade($"[MARKET LOOP] Customer leaving stall. Accepted={wasAccepted}");
        StartCoroutine(ExitLifecycleRoutine());
    }

    private IEnumerator ExitLifecycleRoutine()
    {
        // 1. Audio-aware waiting with safety timeouts
        float elapsed = 0f;
        float maxWaitTime = 6.5f;

        // Brief delay buffer (0.6 seconds) to allow the web request download to trigger
        yield return new WaitForSeconds(0.6f);
        elapsed += 0.6f;

        if (chatManager != null && chatManager.audioManager != null && chatManager.audioManager.audioSource != null)
        {
            AudioSource source = chatManager.audioManager.audioSource;
            while (elapsed < maxWaitTime)
            {
                if (source.isPlaying)
                {
                    // Audio is actively playing, keep waiting
                }
                else if (elapsed < 1.8f)
                {
                    // Allow the web request downloader up to 1.8 seconds to start playback
                }
                else
                {
                    // No audio playing and outside initial download buffer, exit
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Fallback if no audio system is present
            yield return new WaitForSeconds(3.5f);
        }

        // Play outcome animation (Agree/Reject) after TTS speech completes
        if (chatManager != null && chatManager.feedbackManager != null)
        {
            animator = GetActiveNpcAnimator();
            chatManager.feedbackManager.StopNPCThinking(animator);

            if (CanDriveAnimator(animator))
            {
                if (negotiationWasAccepted)
                {
                    animator.SetTrigger("happy");
                    Level1DebugForceAccept.LogVerbose("[ANIM] Triggered Agree (happy) animation after speech complete");
                }
                else
                {
                    animator.SetTrigger("reject");
                    Level1DebugForceAccept.LogVerbose("[ANIM] Triggered Reject (reject) animation after speech complete");
                }
            }
        }

        // Wait for the animation to play fully while standing before walking
        yield return new WaitForSeconds(1.8f);

        if (chatManager != null)
        {
            chatManager.ClearSubtitle();
        }

        // 2. Lock conversation UI input field but leave farewell message visible during walking
        if (chatManager != null)
        {
            if (chatManager.inputField != null)
            {
                chatManager.inputField.text = "";
                chatManager.inputField.interactable = false;
            }
        }

        // 3. Move NPC to BuyerExitPoint and wait until reached
        yield return StartCoroutine(WalkToDestinationRoutine(exitPoint.position));

        // 4. Now reset/clear conversation UI text and show approaching/waiting state
        if (chatManager != null)
        {
            chatManager.ResetConversationUI("Waiting for next customer...");
        }

        float nextCustomerGap = GetRespectBasedCustomerGap();
        Level1DebugForceAccept.LogTrade($"[MARKET LOOP] Customer left. Next customer gap chosen: {nextCustomerGap:0.0}s");

        // 5. Wait for a respect-based delay before spawning next customer
        if (!Level1GameState.Instance.MarketDayEnded)
        {
            yield return StartCoroutine(NextCustomerCountdownRoutine(nextCustomerGap));
        }

        // 6. Hide conversation canvas during teleportation step
        if (conversationUI != null && !Level1GameState.Instance.MarketDayEnded)
        {
            conversationUI.SetActive(false);
        }

        // 7. Reset NPC back to spawn point coordinates
        ResetNPCToSpawnPoint();

        isTransitioning = false;

        // 8. Repeat lifecycle loop
        if (!Level1GameState.Instance.MarketDayEnded)
        {
            StartCoroutine(StartBargainingLifecycle());
        }
        else
        {
            Level1DebugForceAccept.LogTrade("[MARKET LOOP] Market day ended. No new negotiable customers will start.");
        }
    }

    /// <summary>
    /// General NavMeshAgent locomotion pathfinder.
    /// </summary>
    private IEnumerator WalkToDestinationRoutine(Vector3 targetPosition)
    {
        yield return null; // Wait for frame

        if (navMeshAgent == null) yield break;

        bool hasLoggedInvalidNavMeshAgent = false;

        if (!IsNavMeshAgentReady(navMeshAgent))
        {
            if (!hasLoggedInvalidNavMeshAgent)
            {
                Debug.LogWarning("[MarketplaceManager] Cannot move BuyerNPC because the NavMeshAgent is not active on a NavMesh.");
                hasLoggedInvalidNavMeshAgent = true;
            }

            SetWalkingAnimation(false);
            yield break;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);

        SetWalkingAnimation(true);

        bool arrived = false;
        float timeoutTimer = 0f;

        // Locomotion loop with a 25-second failsafe timeout to prevent blocking
        while (!arrived && timeoutTimer < 25f)
        {
            timeoutTimer += Time.deltaTime;

            if (!IsNavMeshAgentReady(navMeshAgent))
            {
                if (!hasLoggedInvalidNavMeshAgent)
                {
                    Debug.LogWarning("[MarketplaceManager] BuyerNPC NavMeshAgent became invalid during movement. Stopping walk routine safely.");
                    hasLoggedInvalidNavMeshAgent = true;
                }

                break;
            }

            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.15f)
                {
                    arrived = true;
                }
            }

            yield return null;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }

        SetWalkingAnimation(false);
    }

    private static bool IsNavMeshAgentReady(NavMeshAgent agent)
    {
        return agent != null &&
            agent.enabled &&
            agent.isActiveAndEnabled &&
            agent.isOnNavMesh;
    }

    private void SetWalkingAnimation(bool isWalking)
    {
        EnsureOriginalPlaceholderHiddenIfNeeded();
        animator = GetActiveNpcAnimator();
        if (!CanDriveAnimator(animator)) return;
        
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetBool("isThinking", false);
            animator.SetBool("isTalking", false);
        }
        EnsureOriginalPlaceholderHiddenIfNeeded();
    }

    public void BeginNegotiationTimer(int buyerPatience)
    {
        StopNegotiationTimer();
        ConfigureNegotiationPatience(buyerPatience);

        Level1DebugForceAccept.LogVerbose($"[MARKET LOOP] Starting idle patience tracking. Patience={buyerPatience}, first={firstReminderSeconds}s, second={secondReminderSeconds}s, walkAway={walkAwaySeconds}s");

        negotiationIdleCoroutine = StartCoroutine(NegotiationIdleRoutine());
    }

    public void StopNegotiationTimer()
    {
        if (negotiationIdleCoroutine != null)
        {
            StopCoroutine(negotiationIdleCoroutine);
            negotiationIdleCoroutine = null;
        }
        isAwaitingPlayerInput = false;
        reminderStage = 0;
    }

    public void StartPlayerIdleWindow()
    {
        if (isTransitioning)
        {
            return;
        }

        isAwaitingPlayerInput = true;
        playerIdleStartedAt = Time.time;
        reminderStage = 0;
    }

    public void MarkMeaningfulPlayerInput()
    {
        isAwaitingPlayerInput = false;
        playerIdleStartedAt = Time.time;
        reminderStage = 0;
    }

    private IEnumerator NegotiationIdleRoutine()
    {
        while (true)
        {
            if (isAwaitingPlayerInput)
            {
                float idleSeconds = Time.time - playerIdleStartedAt;

                if (reminderStage == 0 && idleSeconds >= firstReminderSeconds)
                {
                    reminderStage = 1;
                    if (chatManager != null)
                    {
                        chatManager.PlayNegotiationIdleReminder(FirstReminderLines[Random.Range(0, FirstReminderLines.Length)]);
                    }
                }
                else if (reminderStage == 1 && idleSeconds >= secondReminderSeconds)
                {
                    reminderStage = 2;
                    if (chatManager != null)
                    {
                        chatManager.PlayNegotiationIdleReminder(SecondReminderLines[Random.Range(0, SecondReminderLines.Length)]);
                    }
                }
                else if (idleSeconds >= walkAwaySeconds)
                {
                    negotiationIdleCoroutine = null;

                    Level1DebugForceAccept.LogTrade($"[MARKET LOOP] Player idle patience expired after {idleSeconds:0.0}s. Triggering walk-away flow.");

                    if (chatManager != null)
                    {
                        chatManager.TryHandleNegotiationTimeout(FinalReminderLines[Random.Range(0, FinalReminderLines.Length)]);
                    }

                    yield break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator NextCustomerCountdownRoutine(float nextCustomerGap)
    {
        StopNextCustomerCountdown();
        nextCustomerCountdownCoroutine = StartCoroutine(NextCustomerCountdownDisplayRoutine(nextCustomerGap));
        float elapsed = 0f;
        while (elapsed < nextCustomerGap)
        {
            if (Level1GameState.Instance.MarketDayEnded)
            {
                StopNextCustomerCountdown();
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        StopNextCustomerCountdown();
    }

    private IEnumerator NextCustomerCountdownDisplayRoutine(float nextCustomerGap)
    {
        float remainingSeconds = Mathf.Max(0f, nextCustomerGap);
        int lastShownSeconds = -1;

        while (remainingSeconds > 0f)
        {
            if (Level1GameState.Instance.MarketDayEnded)
            {
                nextCustomerCountdownCoroutine = null;
                yield break;
            }

            int secondsToShow = Mathf.CeilToInt(remainingSeconds);
            if (secondsToShow != lastShownSeconds)
            {
                if (chatManager != null && chatManager.hudManager != null)
                {
                    chatManager.hudManager.ShowNextCustomerCountdown(secondsToShow);
                }

                lastShownSeconds = secondsToShow;
            }

            remainingSeconds -= Time.deltaTime;
            yield return null;
        }

        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.ShowNextCustomerCountdown(0);
        }

        nextCustomerCountdownCoroutine = null;
    }

    private void StopNextCustomerCountdown()
    {
        if (nextCustomerCountdownCoroutine != null)
        {
            StopCoroutine(nextCustomerCountdownCoroutine);
            nextCustomerCountdownCoroutine = null;
        }

        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.HideNextCustomerCountdown();
        }
    }

    private IEnumerator WaitForMarketDayThenStartLifecycle()
    {
        while (!Level1GameState.Instance.MarketDayStarted && !Level1GameState.Instance.MarketDayEnded)
        {
            yield return null;
        }

        if (Level1GameState.Instance.MarketDayEnded)
        {
            yield break;
        }

        Level1DebugForceAccept.LogTrade("[MARKET LOOP] Market day active. Starting negotiable customer lifecycle.");

        StartCoroutine(StartBargainingLifecycle());
    }

    private void ConfigureNegotiationPatience(int buyerPatience)
    {
        if (buyerPatience <= 3)
        {
            firstReminderSeconds = 10;
            secondReminderSeconds = 20;
            walkAwaySeconds = Random.Range(30, 37);
            return;
        }

        if (buyerPatience <= 5)
        {
            firstReminderSeconds = 10;
            secondReminderSeconds = 24;
            walkAwaySeconds = Random.Range(38, 46);
            return;
        }

        firstReminderSeconds = 10;
        secondReminderSeconds = 30;
        walkAwaySeconds = Random.Range(48, 58);
    }

    private float GetRespectBasedCustomerGap()
    {
        if (Level1GameState.Instance == null)
        {
            Debug.LogWarning("[MARKET LOOP] Level1GameState missing. Falling back to resetDelay.");
            return resetDelay;
        }

        float reputation = Level1GameState.Instance.CurrentReputation;

        float minGap;
        float maxGap;

        if (reputation < 40f)
        {
            minGap = 15f;
            maxGap = 15f;
        }
        else if (reputation < 70f)
        {
            minGap = 10f;
            maxGap = 10f;
        }
        else
        {
            minGap = 5f;
            maxGap = 5f;
        }

        float selectedGap = Random.Range(minGap, maxGap);

        Level1DebugForceAccept.LogTrade($"[MARKET LOOP] Respect={reputation:0.0}, selected next customer gap={selectedGap:0.0}s");

        return selectedGap;
    }

    private void EnsureCharacterProfilesInitialized()
    {
        if (characterProfiles == null)
        {
            characterProfiles = new List<DialogueCharacterProfile>();
        }

        List<DialogueCharacterProfile> defaults = DialogueCharacterRegistry.CreateDefaultProfiles();
        if (characterProfiles.Count == 0)
        {
            characterProfiles.AddRange(defaults);
            return;
        }

        foreach (DialogueCharacterProfile defaultProfile in defaults)
        {
            bool alreadyPresent = false;
            for (int i = 0; i < characterProfiles.Count; i++)
            {
                if (string.Equals(characterProfiles[i].characterId, defaultProfile.characterId, System.StringComparison.OrdinalIgnoreCase))
                {
                    alreadyPresent = true;
                    if (string.IsNullOrWhiteSpace(characterProfiles[i].displayName))
                    {
                        characterProfiles[i].displayName = defaultProfile.displayName;
                    }

                    if (string.IsNullOrWhiteSpace(characterProfiles[i].buyerOrigin))
                    {
                        characterProfiles[i].buyerOrigin = defaultProfile.buyerOrigin;
                    }

                    if (string.IsNullOrWhiteSpace(characterProfiles[i].buyerPersonality))
                    {
                        characterProfiles[i].buyerPersonality = defaultProfile.buyerPersonality;
                    }

                    if (characterProfiles[i].modelLocalScale == Vector3.zero)
                    {
                        characterProfiles[i].modelLocalScale = Vector3.one;
                    }

                    break;
                }
            }

            if (!alreadyPresent)
            {
                characterProfiles.Add(defaultProfile);
            }
        }
    }

    private bool TryGetCharacterProfile(string characterId, out DialogueCharacterProfile characterProfile)
    {
        EnsureCharacterProfilesInitialized();

        for (int i = 0; i < characterProfiles.Count; i++)
        {
            DialogueCharacterProfile candidate = characterProfiles[i];
            if (candidate != null && string.Equals(candidate.characterId, characterId, System.StringComparison.OrdinalIgnoreCase))
            {
                characterProfile = candidate;
                return true;
            }
        }

        characterProfile = null;
        return false;
    }

    private void ClearInstantiatedCharacterModel()
    {
        if (activeCharacterModelInstance == null)
        {
            activeCharacterAnimator = rootAnimator;
            animator = rootAnimator;
            return;
        }

        activeCharacterModelInstance.SetActive(false);
        Destroy(activeCharacterModelInstance);
        activeCharacterModelInstance = null;
        activeCharacterAnimator = rootAnimator;
        animator = rootAnimator;
        RefreshNpcRigBindings();
    }

    private void SetOriginalPlaceholderVisible(bool visible)
    {
        if (activeCharacterModelInstance != null && visible)
        {
            visible = false;
        }

        bool stateChanged = isOriginalPlaceholderVisible != visible;

        int toggledVisualObjects = 0;
        for (int i = 0; i < originalPlaceholderVisualObjects.Count; i++)
        {
            GameObject visualObject = originalPlaceholderVisualObjects[i];
            if (visualObject == null)
            {
                continue;
            }

            if (visualObject.activeSelf != visible)
            {
                visualObject.SetActive(visible);
                toggledVisualObjects++;
            }
        }

        int toggledRenderers = 0;
        for (int i = 0; i < originalPlaceholderRenderers.Count; i++)
        {
            Renderer renderer = originalPlaceholderRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            if (renderer.enabled != visible)
            {
                renderer.enabled = visible;
                toggledRenderers++;
            }
        }

        for (int i = 0; i < originalPlaceholderHipsRenderers.Count; i++)
        {
            Renderer renderer = originalPlaceholderHipsRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            if (renderer.enabled != visible)
            {
                renderer.enabled = visible;
                toggledRenderers++;
            }
        }

        isOriginalPlaceholderVisible = visible;
        if (stateChanged || toggledVisualObjects > 0 || toggledRenderers > 0)
        {
            Debug.Log("[MARKET NPC] Original placeholder visible=" + visible +
                      " | toggledVisualObjects=" + toggledVisualObjects +
                      " | toggledRenderers=" + toggledRenderers +
                      " | activeCustomModel=" + (activeCharacterModelInstance != null ? activeCharacterModelInstance.name : "<none>") +
                      " | modelAnchorHasRuntimeChild=" + (modelAnchor != null && modelAnchor.childCount > 0));
        }
    }

    private void RefreshNpcRigBindings()
    {
        NPCGazeController gazeController = GetBuyerNpcGazeController();
        if (gazeController != null)
        {
            Transform preferredRigRoot = activeCharacterModelInstance != null ? activeCharacterModelInstance.transform : buyerNPC.transform;
            Animator preferredAnimator = activeCharacterModelInstance != null ? activeCharacterAnimator : rootAnimator;
            gazeController.SetRigBindingSource(preferredRigRoot, preferredAnimator, activeCharacterModelInstance != null);
            gazeController.RefreshRigBindings();
        }
    }

    private void CacheOriginalPlaceholderRenderers()
    {
        originalPlaceholderVisualObjects.Clear();
        originalPlaceholderRenderers.Clear();
        originalPlaceholderHipsRenderers.Clear();

        if (buyerNPC == null)
        {
            return;
        }

        for (int rootIndex = 0; rootIndex < OriginalPlaceholderVisualObjectNames.Length; rootIndex++)
        {
            Transform placeholderRoot = buyerNPC.transform.Find(OriginalPlaceholderVisualObjectNames[rootIndex]);
            if (placeholderRoot == null)
            {
                continue;
            }

            if (!originalPlaceholderVisualObjects.Contains(placeholderRoot.gameObject))
            {
                originalPlaceholderVisualObjects.Add(placeholderRoot.gameObject);
            }

            Renderer[] renderers = placeholderRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (modelAnchor != null && renderer.transform.IsChildOf(modelAnchor))
                {
                    continue;
                }

                if (!originalPlaceholderRenderers.Contains(renderer))
                {
                    originalPlaceholderRenderers.Add(renderer);
                }
            }
        }

        Transform hipsRoot = buyerNPC.transform.Find("Hips");
        keepsHipsActive = hipsRoot != null;
        if (hipsRoot != null)
        {
            Renderer[] hipsRenderers = hipsRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < hipsRenderers.Length; i++)
            {
                Renderer renderer = hipsRenderers[i];
                if (renderer == null)
                {
                    continue;
                }

                if (modelAnchor != null && renderer.transform.IsChildOf(modelAnchor))
                {
                    continue;
                }

                if (!originalPlaceholderHipsRenderers.Contains(renderer))
                {
                    originalPlaceholderHipsRenderers.Add(renderer);
                }
            }
        }

        Debug.Log("[MARKET NPC] Cached original placeholder renderers at startup: " + (originalPlaceholderRenderers.Count + originalPlaceholderHipsRenderers.Count));
    }

    private void EnsureOriginalPlaceholderHiddenIfNeeded()
    {
        if (activeCharacterModelInstance != null)
        {
            SetOriginalPlaceholderVisible(false);
        }
    }

    private void LogActiveVisualDiagnostics(string stage)
    {
        NPCGazeController gazeController = GetBuyerNpcGazeController();
        string animatorName = activeCharacterAnimator != null ? activeCharacterAnimator.name : "<none>";
        bool hasController = activeCharacterAnimator != null && activeCharacterAnimator.runtimeAnimatorController != null;
        string headBindingPath = gazeController != null ? gazeController.GetHeadBindingPath() : "<none>";

        Debug.Log("[MARKET NPC] Visual diagnostics | stage=" + stage +
                  " | customModelInstance=" + (activeCharacterModelInstance != null ? activeCharacterModelInstance.name : "<none>") +
                  " | deactivatedOriginalVisualObjects=" + originalPlaceholderVisualObjects.Count +
                  " | hipsDeactivated=" + (!keepsHipsActive) +
                  " | activeAnimator=" + animatorName +
                  " | activeAnimatorHasController=" + hasController +
                  " | gazeHeadBindingPath=" + headBindingPath +
                  " | activeOriginalRenderersOutsideModelAnchor=" + CountActiveOriginalRenderersOutsideModelAnchor());
    }

    private void ValidateNpcVisualState(string stage)
    {
        if (activeCharacterModelInstance == null || buyerNPC == null)
        {
            return;
        }

        List<string> activeOriginalRenderers = new List<string>();
        CollectActiveOriginalRenderers(activeOriginalRenderers, originalPlaceholderVisualObjects);
        CollectActiveOriginalRenderers(activeOriginalRenderers, originalPlaceholderHipsRenderers);

        if (activeOriginalRenderers.Count > 0)
        {
            Debug.LogWarning("[MARKET NPC] ValidateNpcVisualState failed at " + stage +
                             ". Enabled original avatar renderers still active: " +
                             string.Join(", ", activeOriginalRenderers));
        }
    }

    private void CollectActiveOriginalRenderers(List<string> results, List<GameObject> visualObjects)
    {
        for (int i = 0; i < visualObjects.Count; i++)
        {
            GameObject visualObject = visualObjects[i];
            if (visualObject == null || !visualObject.activeInHierarchy)
            {
                continue;
            }

            Renderer[] renderers = visualObject.GetComponentsInChildren<Renderer>(true);
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Renderer renderer = renderers[rendererIndex];
                if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (modelAnchor != null && renderer.transform.IsChildOf(modelAnchor))
                {
                    continue;
                }

                results.Add(GetHierarchyPath(renderer.transform));
            }
        }
    }

    private void CollectActiveOriginalRenderers(List<string> results, List<Renderer> renderers)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (modelAnchor != null && renderer.transform.IsChildOf(modelAnchor))
            {
                continue;
            }

            results.Add(GetHierarchyPath(renderer.transform));
        }
    }

    private int CountActiveOriginalRenderersOutsideModelAnchor()
    {
        List<string> activeOriginalRenderers = new List<string>();
        CollectActiveOriginalRenderers(activeOriginalRenderers, originalPlaceholderVisualObjects);
        CollectActiveOriginalRenderers(activeOriginalRenderers, originalPlaceholderHipsRenderers);
        return activeOriginalRenderers.Count;
    }

    private static string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return "<null>";
        }

        string path = target.name;
        Transform current = target.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private void PrepareIncomingCustomerLifecycle()
    {
        if (chatManager == null || !chatManager.useLocalSessionGeneration)
        {
            preparedLocalSession = null;
            return;
        }

        Level1GameState.Instance.PrepareForNewCustomer();
        preparedLocalSession = Level1GameState.Instance.GenerateLocalSession();
        preparedLocalSession.runtimeSessionId = nextRuntimeSessionId++;
        AssignCharacterVisualForSession(preparedLocalSession, "incoming-customer");
    }

    private static void ApplyCharacterVisualOffsets(Transform visualTransform, DialogueCharacterProfile profile)
    {
        if (visualTransform == null || profile == null)
        {
            return;
        }

        visualTransform.localPosition = profile.modelLocalPosition;
        visualTransform.localRotation = Quaternion.Euler(profile.modelLocalEulerAngles);
        visualTransform.localScale = profile.modelLocalScale == Vector3.zero
            ? Vector3.one
            : profile.modelLocalScale;
    }

    private void StabilizeInstantiatedVisual(GameObject visualRoot)
    {
        if (visualRoot == null)
        {
            return;
        }

        NavMeshAgent[] childAgents = visualRoot.GetComponentsInChildren<NavMeshAgent>(true);
        for (int i = 0; i < childAgents.Length; i++)
        {
            childAgents[i].enabled = false;
        }

        CharacterController[] childCharacterControllers = visualRoot.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < childCharacterControllers.Length; i++)
        {
            childCharacterControllers[i].enabled = false;
        }

        Collider[] colliders = visualRoot.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        Rigidbody[] rigidbodies = visualRoot.GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody body = rigidbodies[i];
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            body.useGravity = false;
        }

        NPCWalker[] npcWalkers = visualRoot.GetComponentsInChildren<NPCWalker>(true);
        for (int i = 0; i < npcWalkers.Length; i++)
        {
            npcWalkers[i].enabled = false;
        }

        AudioSource[] audioSources = visualRoot.GetComponentsInChildren<AudioSource>(true);
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].enabled = false;
        }

        Animator[] animators = visualRoot.GetComponentsInChildren<Animator>(true);
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].applyRootMotion = false;
        }
    }
}
