using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MarketplaceManager : MonoBehaviour
{
    [Header("NPC Settings")]
    [Tooltip("The active NPC GameObject in the scene (e.g., BuyerNPC).")]
    public GameObject buyerNPC;

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

    [Header("Settings")]
    [Tooltip("Movement speed multiplier for the NavMeshAgent.")]
    public float movementSpeed = 1.3f;
    
    [Tooltip("Delay in seconds to wait at the exit point before resetting.")]
    public float resetDelay = 3f;

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isTransitioning = false;
    private bool negotiationWasAccepted = false;
    private Coroutine negotiationIdleCoroutine;
    private Coroutine nextCustomerCountdownCoroutine;
    private Coroutine marketDayStartupCoroutine;
    private bool isAwaitingPlayerInput;
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
        Debug.Log("[SCENE FLOW] " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " loaded");
        Level1GameState.Instance.EnsureInitialized();

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

        animator = buyerNPC.GetComponent<Animator>();

        // Auto-mount NPCGazeController if not present
        NPCGazeController gazeController = buyerNPC.GetComponent<NPCGazeController>();
        if (gazeController == null)
        {
            gazeController = buyerNPC.AddComponent<NPCGazeController>();
            if (showDebugLogs)
            {
                Debug.Log("[MarketplaceManager] Automatically added NPCGazeController to BuyerNPC.");
            }
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

    /// <summary>
    /// Coroutine that drives the complete sequential walk-trade lifecycle.
    /// </summary>
    private IEnumerator StartBargainingLifecycle()
    {
        if (!Level1GameState.Instance.MarketDayStarted || Level1GameState.Instance.MarketDayEnded)
        {
            yield break;
        }

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

        if (showDebugLogs)
        {
            Debug.Log("[MarketplaceManager] Moving BuyerNPC from SpawnPoint -> TradePoint");
        }

        // 2. Move NPC to Trade Point and wait until reached
        yield return StartCoroutine(WalkToDestinationRoutine(tradePoint.position));

        // 3. NPC Arrived at Stall - Orient smoothly
        buyerNPC.transform.rotation = tradePoint.rotation;
        if (showDebugLogs)
        {
            Debug.Log("[MarketplaceManager] NPC reached TradePoint. Triggering browsing behavior.");
        }

        // Cache animator
        if (animator == null)
        {
            animator = buyerNPC.GetComponent<Animator>();
            if (animator == null) animator = buyerNPC.GetComponentInChildren<Animator>();
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
            chatManager.feedbackManager.StartNPCThinking(animator, chatManager.npcText, false);
        }

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
        if (showDebugLogs)
        {
            Debug.Log($"[MarketplaceManager] Negotiation concluded. Accepted: {wasAccepted}. Playing farewell and outcome animation.");
        }
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
            if (animator == null && buyerNPC != null)
            {
                animator = buyerNPC.GetComponent<Animator>();
                if (animator == null) animator = buyerNPC.GetComponentInChildren<Animator>();
            }

            chatManager.feedbackManager.StopNPCThinking(animator);

            if (animator != null)
            {
                if (negotiationWasAccepted)
                {
                    animator.SetTrigger("happy");
                    if (showDebugLogs)
                    {
                        Debug.Log("[ANIM] Triggered Agree (happy) animation after speech complete");
                    }
                }
                else
                {
                    animator.SetTrigger("reject");
                    if (showDebugLogs)
                    {
                        Debug.Log("[ANIM] Triggered Reject (reject) animation after speech complete");
                    }
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
        if (showDebugLogs)
        {
            Debug.Log($"[MarketplaceManager] NPC reached ExitPoint. Waiting {nextCustomerGap:0.0} seconds before resetting.");
        }

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
        else if (showDebugLogs)
        {
            Debug.Log("[MARKET LOOP] Market day ended. No new negotiable customers will start.");
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
        if (animator == null) return;
        
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetBool("isThinking", false);
            animator.SetBool("isTalking", false);
        }
    }

    public void BeginNegotiationTimer(int buyerPatience)
    {
        StopNegotiationTimer();
        ConfigureNegotiationPatience(buyerPatience);

        if (showDebugLogs)
        {
            Debug.Log($"[MARKET LOOP] Starting idle patience tracking. Patience={buyerPatience}, first={firstReminderSeconds}s, second={secondReminderSeconds}s, walkAway={walkAwaySeconds}s");
        }

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

                    if (showDebugLogs)
                    {
                        Debug.Log($"[MARKET LOOP] Player idle patience expired after {idleSeconds:0.0}s. Triggering walk-away flow.");
                    }

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
                if (showDebugLogs)
                {
                    Debug.Log($"[MARKET LOOP] Next customer arriving in {secondsToShow}s");
                }

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

        if (showDebugLogs)
        {
            Debug.Log("[MARKET LOOP] Market day active. Starting negotiable customer lifecycle.");
        }

        StartCoroutine(StartBargainingLifecycle());
    }

    private void ConfigureNegotiationPatience(int buyerPatience)
    {
        if (buyerPatience <= 3)
        {
            firstReminderSeconds = 6;
            secondReminderSeconds = 12;
            walkAwaySeconds = Random.Range(20, 31);
            return;
        }

        if (buyerPatience <= 5)
        {
            firstReminderSeconds = 10;
            secondReminderSeconds = 22;
            walkAwaySeconds = Random.Range(35, 46);
            return;
        }

        firstReminderSeconds = 15;
        secondReminderSeconds = 35;
        walkAwaySeconds = Random.Range(50, 61);
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

        if (reputation < 35f)
        {
            minGap = 18f;
            maxGap = 25f;
        }
        else if (reputation < 70f)
        {
            minGap = 10f;
            maxGap = 16f;
        }
        else
        {
            minGap = 4f;
            maxGap = 8f;
        }

        float selectedGap = Random.Range(minGap, maxGap);

        Debug.Log($"[MARKET LOOP] Reputation={reputation:0.0}, selected next customer gap={selectedGap:0.0}s");

        return selectedGap;
    }
}
