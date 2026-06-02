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

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isTransitioning = false;

    private void Start()
    {
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
            chatManager = FindObjectOfType<ChatManager>();
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
            Debug.Log("[MarketplaceManager] Automatically added NPCGazeController to BuyerNPC.");
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
        }

        // 6. Reset NPC position and begin lifecycle loop
        ResetNPCToSpawnPoint();
        StartCoroutine(StartBargainingLifecycle());
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

        Debug.Log("[MarketplaceManager] Moving BuyerNPC from SpawnPoint -> TradePoint");

        // 2. Move NPC to Trade Point and wait until reached
        yield return StartCoroutine(WalkToDestinationRoutine(tradePoint.position));

        // 3. NPC Arrived at Stall - Orient smoothly
        buyerNPC.transform.rotation = tradePoint.rotation;
        Debug.Log("[MarketplaceManager] NPC reached TradePoint. Activating UI and starting session.");

        // 4. Enable input fields on arrival and start session
        if (chatManager != null)
        {
            chatManager.EnableConversationUI();
            chatManager.StartNewSession();
        }

        isTransitioning = false;
    }

    /// <summary>
    /// Invoked dynamically by ChatManager when done = true is returned by the API response.
    /// </summary>
    public void OnNegotiationFinished()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        Debug.Log("[MarketplaceManager] Negotiation concluded. Moving NPC to BuyerExitPoint.");
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

        Debug.Log($"[MarketplaceManager] NPC reached ExitPoint. Waiting {resetDelay} seconds before resetting.");

        // 5. Wait for the reset delay (e.g. 3 seconds)
        yield return new WaitForSeconds(resetDelay);

        // 6. Hide conversation canvas during teleportation step
        if (conversationUI != null)
        {
            conversationUI.SetActive(false);
        }

        // 7. Reset NPC back to spawn point coordinates
        ResetNPCToSpawnPoint();

        isTransitioning = false;

        // 8. Repeat lifecycle loop
        StartCoroutine(StartBargainingLifecycle());
    }

    /// <summary>
    /// General NavMeshAgent locomotion pathfinder.
    /// </summary>
    private IEnumerator WalkToDestinationRoutine(Vector3 targetPosition)
    {
        yield return null; // Wait for frame

        if (navMeshAgent == null) yield break;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);

        SetWalkingAnimation(true);

        bool arrived = false;
        float timeoutTimer = 0f;

        // Locomotion loop with a 25-second failsafe timeout to prevent blocking
        while (!arrived && timeoutTimer < 25f)
        {
            timeoutTimer += Time.deltaTime;

            if (navMeshAgent != null && !navMeshAgent.pathPending)
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

    private void SetWalkingAnimation(bool isWalking)
    {
        if (animator == null) return;
        
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("Speed", isWalking ? 1f : 0f);
        animator.SetBool("isAtStall", !isWalking);

        if (isWalking)
        {
            animator.SetBool("isThinking", false);
            animator.SetBool("thinking", false);
            animator.SetBool("isTalking", false);
            animator.SetBool("talking", false);
        }
    }
}
