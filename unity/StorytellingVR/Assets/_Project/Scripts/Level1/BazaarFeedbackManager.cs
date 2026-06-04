using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BazaarFeedbackManager : MonoBehaviour
{
    [Header("Transaction Complete Popup UI")]
    [Tooltip("The parent Panel GameObject containing the Transaction Summary.")]
    public GameObject transactionPopupPanel;
    public TMP_Text titleText;
    public TMP_Text itemText;
    public TMP_Text moneyText;
    public TMP_Text profitText;
    public TMP_Text respectText;
    public TMP_Text reputationLabelText;
    public TMP_Text buyerNameText;
    public TMP_Text buyerOriginText;

    [Header("Floating Coin Animation Settings")]
    [Tooltip("Text element placed above the stall to show coins earned floating away.")]
    public TMP_Text floatingCoinText;
    public Transform floatingStartPoint;
    public float floatSpeed = 1.0f;
    public float floatDuration = 1.8f;

    [Header("Reputation Alert Toast UI")]
    [Tooltip("Panel for displaying respect gain or loss notification banners.")]
    public GameObject respectToastPanel;
    public TMP_Text respectToastTitle;
    public TMP_Text respectToastSubText;
    public Image respectToastBackground;
    public float toastDuration = 3.0f;

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    // Immersion thinking fillers
    private readonly string[] thinkingFillers = new string[]
    {
        "Hmm...",
        "Let me consider that.",
        "The market has been unusual lately.",
        "A merchant must think carefully.",
        "Interesting proposal...",
        "I traded with another seller earlier today."
    };

    private Coroutine thinkingCoroutine;

    private void Start()
    {
        // Hide panels on start
        if (transactionPopupPanel != null) transactionPopupPanel.SetActive(false);
        if (respectToastPanel != null) respectToastPanel.SetActive(false);
        if (floatingCoinText != null) floatingCoinText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Animates the NPC thinking state using animators and in-character thinking fillers.
    /// </summary>
    public void StartNPCThinking(Animator npcAnimator, TMP_Text npcTextElement, bool allowFillers = true)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[THINK] StartNPCThinking Called, allowFillers: {allowFillers}");
        }

        // Stop any running thinking coroutine first to avoid overlaps
        if (thinkingCoroutine != null)
        {
            StopCoroutine(thinkingCoroutine);
        }

        thinkingCoroutine = StartCoroutine(ThinkingBehaviourRoutine(npcAnimator, npcTextElement, allowFillers));
    }

    private IEnumerator ThinkingBehaviourRoutine(Animator animator, TMP_Text textElement, bool allowFillers)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[THINK] Coroutine Started, allowFillers: {allowFillers}");
        }

        NPCGazeController gaze = null;
        if (animator != null)
        {
            gaze = animator.GetComponent<NPCGazeController>();
        }

        // --- IMMEDIATELY: Play Thinking animation once ---
        if (animator != null)
        {
            animator.SetBool("isThinking", true);
            animator.SetBool("isTalking", false);
            if (showDebugLogs)
            {
                Debug.Log("[ANIM] Thinking ON");
            }

            if (gaze != null)
            {
                gaze.LookAtSpices();
            }
        }

        if (allowFillers && textElement != null)
        {
            textElement.text = "Hmm...";
            if (showDebugLogs)
            {
                Debug.Log($"[BazaarFeedback] Thinking filler text: {textElement.text}");
            }
        }

        // Wait 2.5 seconds for the initial thinking gesture to play
        yield return new WaitForSeconds(2.5f);

        // --- LOOP: waiting for the backend response ---
        while (true)
        {
            int action = Random.Range(1, 5); // 1 to 4 inclusive
            float waitDuration = Random.Range(2.0f, 4.0f);

            switch (action)
            {
                case 1:
                    // 1. Idle breathing
                    if (animator != null)
                    {
                        animator.SetBool("isThinking", false);
                        if (showDebugLogs)
                        {
                            Debug.Log("[ANIM] Thinking OFF");
                        }
                    }
                    if (gaze != null)
                    {
                        gaze.LookAtPlayer();
                    }
                    break;

                case 2:
                    // 2. Thinking animation
                    if (animator != null)
                    {
                        animator.SetBool("isThinking", true);
                        if (showDebugLogs)
                        {
                            Debug.Log("[ANIM] Thinking ON");
                        }
                    }
                    if (gaze != null)
                    {
                        gaze.LookAtSpices();
                    }
                    break;

                case 3:
                    // 3. Look at spices
                    if (gaze != null)
                    {
                        gaze.LookAtSpices();
                    }
                    break;

                case 4:
                    // 4. Filler dialogue text
                    if (allowFillers && textElement != null)
                    {
                        int textIdx = Random.Range(0, thinkingFillers.Length);
                        textElement.text = thinkingFillers[textIdx];
                        if (showDebugLogs)
                        {
                            Debug.Log($"[BazaarFeedback] Thinking filler text: {textElement.text}");
                        }
                    }
                    break;
            }

            yield return new WaitForSeconds(waitDuration);
        }
    }

    /// <summary>
    /// Resets the NPC locomotion animator parameters when real dialogues arrive.
    /// </summary>
    public void StopNPCThinking(Animator npcAnimator)
    {
        if (thinkingCoroutine != null)
        {
            StopCoroutine(thinkingCoroutine);
            thinkingCoroutine = null;
        }

        if (npcAnimator != null)
        {
            npcAnimator.SetBool("isThinking", false);
            if (showDebugLogs)
            {
                Debug.Log("[ANIM] Thinking OFF");
            }

            // Return gaze target to player immediately on response
            NPCGazeController gazeController = npcAnimator.GetComponent<NPCGazeController>();
            if (gazeController != null)
            {
                gazeController.LookAtPlayer();
            }
        }
    }

    /// <summary>
    /// Triggers the respect/reputation toast banner externally.
    /// </summary>
    public void TriggerRespectToast(int respectChange)
    {
        if (respectToastPanel != null)
        {
            StartCoroutine(ShowRespectToastRoutine(respectChange));
        }
    }

    /// <summary>
    /// Triggers the full gamification feedback sequence upon transaction acceptance.
    /// </summary>
    public void ShowTransactionFeedback(TransactionSummary summary, string archetype)
    {
        if (summary == null) return;
        if (showDebugLogs)
        {
            Debug.Log($"[BazaarFeedback] Triggering transaction feedback for {summary.item}. Earned: {summary.earned}");
        }

        // 1. Trigger the Transaction Complete Popup UI
        if (transactionPopupPanel != null)
        {
            if (titleText != null) titleText.text = "TRADE COMPLETE";
            if (moneyText != null) moneyText.text = $"+{summary.earned} Varahas";
            if (itemText != null) itemText.text = $"{summary.item} Sold";
            
            if (profitText != null) profitText.text = "";
            if (respectText != null) respectText.text = "";
            if (reputationLabelText != null) reputationLabelText.text = "";
            if (buyerNameText != null) buyerNameText.text = "";
            if (buyerOriginText != null) buyerOriginText.text = "";

            StartCoroutine(ShowAndHidePopupRoutine());
        }

        // 2. Trigger the Floating Coin Animation above the stall
        if (floatingCoinText != null && floatingStartPoint != null)
        {
            StartCoroutine(FloatTextRoutine(summary.earned));
        }

        // 3. Trigger the Reputation Toast Banner depending on respect changes
        if (respectToastPanel != null)
        {
            StartCoroutine(ShowRespectToastRoutine(summary.respect_change));
        }
    }

    private IEnumerator ShowAndHidePopupRoutine()
    {
        transactionPopupPanel.SetActive(true);
        yield return new WaitForSeconds(5.0f); // Display for 5 seconds
        transactionPopupPanel.SetActive(false);
    }

    private IEnumerator FloatTextRoutine(int coins)
    {
        floatingCoinText.gameObject.SetActive(true);
        floatingCoinText.text = $"+{coins} Varahas";
        
        Vector3 startPos = floatingStartPoint.position;
        floatingCoinText.transform.position = startPos;

        Color originalColor = floatingCoinText.color;
        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / floatDuration;

            // Move upwards
            floatingCoinText.transform.position = startPos + Vector3.up * (progress * floatSpeed);

            // Fade out
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1.0f, 0.0f, progress);
            floatingCoinText.color = newColor;

            yield return null;
        }

        floatingCoinText.color = originalColor; // Reset
        floatingCoinText.gameObject.SetActive(false);
    }

    private IEnumerator ShowRespectToastRoutine(int respectChange)
    {
        if (respectToastTitle != null && respectToastSubText != null)
        {
            if (respectChange >= 0)
            {
                respectToastTitle.text = "🤝 Reputation Improved";
                respectToastSubText.text = "Customers trust your fairness";
                
                if (respectToastBackground != null)
                {
                    respectToastBackground.color = new Color(0.12f, 0.53f, 0.15f, 0.9f); // Dark Green
                }
            }
            else
            {
                respectToastTitle.text = "⚠ Reputation Decreased";
                respectToastSubText.text = "Word spreads about unfair prices";

                if (respectToastBackground != null)
                {
                    respectToastBackground.color = new Color(0.7f, 0.13f, 0.13f, 0.9f); // Dark Red
                }
            }
        }

        respectToastPanel.SetActive(true);
        yield return new WaitForSeconds(toastDuration);
        respectToastPanel.SetActive(false);
    }
}
