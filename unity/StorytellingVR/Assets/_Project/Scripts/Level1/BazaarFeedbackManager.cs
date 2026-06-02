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

    // Immersion thinking fillers
    private readonly string[] thinkingFillers = new string[]
    {
        "Hmm... let me consider the price.",
        "Let me check today's market value...",
        "The spice demand has been changing recently...",
        "I must think about this bargain carefully.",
        "A good trade benefits both sides..."
    };

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
    public void StartNPCThinking(Animator npcAnimator, TMP_Text npcTextElement)
    {
        if (npcAnimator != null)
        {
            // Set both naming styles for maximum animator compatibility
            npcAnimator.SetBool("isThinking", true);
            npcAnimator.SetBool("thinking", true);
            npcAnimator.SetBool("isTalking", false);
            npcAnimator.SetBool("talking", false);

            // Dynamically trigger look at spices
            NPCGazeController gazeController = npcAnimator.GetComponent<NPCGazeController>();
            if (gazeController != null)
            {
                gazeController.LookAtSpices();
            }
        }

        if (npcTextElement != null)
        {
            int randomIndex = Random.Range(0, thinkingFillers.Length);
            npcTextElement.text = thinkingFillers[randomIndex];
            Debug.Log($"[BazaarFeedback] NPC Thinking filler triggered: \"{npcTextElement.text}\"");
        }
    }

    /// <summary>
    /// Resets the NPC locomotion animator parameters when real dialogues arrive.
    /// </summary>
    public void StopNPCThinking(Animator npcAnimator)
    {
        if (npcAnimator != null)
        {
            // Set both naming styles for maximum animator compatibility
            npcAnimator.SetBool("isThinking", false);
            npcAnimator.SetBool("thinking", false);
            npcAnimator.SetBool("isTalking", true);
            npcAnimator.SetBool("talking", true);

            // Dynamically trigger look at player
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
        Debug.Log($"[BazaarFeedback] Triggering transaction feedback for {summary.item}. Earned: {summary.earned}");

        // 1. Trigger the Transaction Complete Popup UI
        if (transactionPopupPanel != null)
        {
            if (titleText != null) titleText.text = "📜 Trade Ledger";
            if (itemText != null) itemText.text = $"Sold: {summary.item} ({summary.quantity})";
            if (moneyText != null) moneyText.text = $"+{summary.earned} Varahas";
            if (profitText != null) profitText.text = $"+{summary.profit} Varahas Profit";
            
            if (respectText != null)
            {
                string sign = summary.respect_change >= 0 ? "+" : "";
                respectText.text = $"{sign}{summary.respect_change} Respect";
            }

            if (reputationLabelText != null)
            {
                reputationLabelText.text = $"Reputation Status: {archetype}";
            }

            if (buyerNameText != null)
            {
                buyerNameText.text = $"Buyer: {summary.buyer_name}";
            }

            if (buyerOriginText != null)
            {
                buyerOriginText.text = $"Origin: {summary.buyer_origin}";
            }

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
        yield return new WaitForSeconds(3.5f); // Display for 3.5 seconds
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
