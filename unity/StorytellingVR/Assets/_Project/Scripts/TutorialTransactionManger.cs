using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Manages the transaction interface during tutorial.
/// Handles price input, cost board display, and visual feedback for deals.
/// </summary>
public class TutorialTransactionManager : MonoBehaviour
{
    [Header("Transaction UI")]
    public GameObject transactionPanel;
    public TextMeshProUGUI transactionTitleText;
    
    [Header("Cost Board")]
    public GameObject costBoardPanel;
    public TextMeshProUGUI costSpiceNameText;
    public TextMeshProUGUI costPriceText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI totalCostText;
    public Image costBoardHighlight;

    [Header("Price Input")]
    public Slider priceSlider;
    public TMP_InputField priceInputField;
    public TextMeshProUGUI currentPriceText;
    public TextMeshProUGUI currencyLabel;
    public Button submitButton;
    public TextMeshProUGUI submitButtonText;

    [Header("Price Feedback")]
    public GameObject priceFeedbackPanel;
    public TextMeshProUGUI feedbackText;
    public Image feedbackBackground;
    public Color fairPriceColor = new Color(0.2f, 0.8f, 0.3f, 0.3f);
    public Color highPriceColor = new Color(0.9f, 0.5f, 0.2f, 0.3f);
    public Color absurdPriceColor = new Color(0.9f, 0.2f, 0.2f, 0.3f);
    public Color belowCostColor = new Color(0.5f, 0.5f, 0.9f, 0.3f);

    [Header("Deal Outcome")]
    public GameObject dealSuccessEffect;
    public GameObject dealFailureEffect;
    public ParticleSystem successParticles;
    public ParticleSystem failureParticles;

    [Header("Profit Preview")]
    public GameObject profitPreviewPanel;
    public TextMeshProUGUI profitPreviewText;
    public Image profitPreviewBackground;

    [Header("Settings")]
    public float minPrice = 0f;
    public float maxPrice = 500f;
    public float priceStep = 5f;
    public bool allowFreeInput = true;

    [Header("VR Interaction")]
    public bool vrMode = false;
    public Transform vrSliderHandle;

    [Header("Events")]
    public UnityEvent<float> OnPriceChanged;
    public UnityEvent<float> OnPriceSubmitted;

    private string currentSpiceName;
    private int currentQuantity;
    private float currentCostPerUnit;
    private float currentTotalCost;
    private float currentPlayerPrice;
    private bool inputEnabled = false;

    void Start()
    {
        InitializeUI();
        SetupInputListeners();
    }

    void InitializeUI()
    {
        if (transactionPanel != null) 
            transactionPanel.SetActive(false);
        
        if (costBoardPanel != null)
            costBoardPanel.SetActive(false);
        
        if (priceFeedbackPanel != null)
            priceFeedbackPanel.SetActive(false);
        
        if (profitPreviewPanel != null)
            profitPreviewPanel.SetActive(false);
        
        if (dealSuccessEffect != null)
            dealSuccessEffect.SetActive(false);
        
        if (dealFailureEffect != null)
            dealFailureEffect.SetActive(false);

        if (currencyLabel != null)
            currencyLabel.text = "pagodas";
    }

    void SetupInputListeners()
    {
        if (priceSlider != null)
        {
            priceSlider.minValue = minPrice;
            priceSlider.maxValue = maxPrice;
            priceSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (priceInputField != null)
        {
            priceInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    /// <summary>
    /// Sets up the transaction with spice details.
    /// </summary>
    public void SetupTransaction(string spiceName, int quantity, float costPerUnit)
    {
        currentSpiceName = spiceName;
        currentQuantity = quantity;
        currentCostPerUnit = costPerUnit;
        currentTotalCost = costPerUnit * quantity;

        if (transactionPanel != null)
            transactionPanel.SetActive(true);

        if (transactionTitleText != null)
            transactionTitleText.text = $"Transaction: {spiceName}";

        UpdateCostBoard();
        
        // Set initial price to fair price
        float fairPrice = currentTotalCost * 1.3f;
        SetPrice(fairPrice);
    }

    void UpdateCostBoard()
    {
        if (costBoardPanel != null)
        {
            costBoardPanel.SetActive(true);

            if (costSpiceNameText != null)
                costSpiceNameText.text = currentSpiceName;

            if (costPriceText != null)
                costPriceText.text = $"Cost: {currentCostPerUnit:F1} pagodas/unit";

            if (quantityText != null)
                quantityText.text = $"Quantity: {currentQuantity} units";

            if (totalCostText != null)
                totalCostText.text = $"Total Cost: {currentTotalCost:F1} pagodas";
        }
    }

    /// <summary>
    /// Highlights the cost price (called by narrator).
    /// </summary>
    public void HighlightCostPrice(float costPerUnit)
    {
        if (costBoardHighlight != null)
        {
            StartCoroutine(PulseCostBoard());
        }

        if (costPriceText != null)
        {
            StartCoroutine(FlashText(costPriceText));
        }
    }

    IEnumerator PulseCostBoard()
    {
        if (costBoardHighlight == null) yield break;

        Color originalColor = costBoardHighlight.color;
        Color highlightColor = new Color(1f, 0.8f, 0.2f, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            // Pulse up
            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                costBoardHighlight.color = Color.Lerp(originalColor, highlightColor, elapsed / 0.3f);
                yield return null;
            }

            // Pulse down
            elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.deltaTime;
                costBoardHighlight.color = Color.Lerp(highlightColor, originalColor, elapsed / 0.3f);
                yield return null;
            }
        }

        costBoardHighlight.color = originalColor;
    }

    IEnumerator FlashText(TextMeshProUGUI text)
    {
        if (text == null) yield break;

        Color originalColor = text.color;
        Color flashColor = new Color(1f, 1f, 0.3f, 1f);

        for (int i = 0; i < 3; i++)
        {
            text.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            text.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// Enables or disables price input.
    /// </summary>
    public void EnablePriceInput(bool enabled)
    {
        inputEnabled = enabled;

        if (priceSlider != null)
            priceSlider.interactable = enabled;

        if (priceInputField != null)
            priceInputField.interactable = enabled;

        if (submitButton != null)
            submitButton.interactable = enabled;

        if (submitButtonText != null)
            submitButtonText.text = enabled ? "Submit Offer" : "Waiting...";
    }

    void OnSliderValueChanged(float value)
    {
        if (!inputEnabled) return;

        // Snap to step
        value = Mathf.Round(value / priceStep) * priceStep;
        currentPlayerPrice = value;

        UpdatePriceDisplay();
        UpdatePriceFeedback();
        
        OnPriceChanged?.Invoke(value);
    }

    void OnInputFieldValueChanged(string value)
    {
        if (!inputEnabled || !allowFreeInput) return;

        if (float.TryParse(value, out float price))
        {
            price = Mathf.Clamp(price, minPrice, maxPrice);
            currentPlayerPrice = price;

            if (priceSlider != null)
                priceSlider.value = price;

            UpdatePriceDisplay();
            UpdatePriceFeedback();
            
            OnPriceChanged?.Invoke(price);
        }
    }

    void OnSubmitButtonClicked()
    {
        if (!inputEnabled) return;

        Debug.Log($"Price submitted: {currentPlayerPrice}");
        OnPriceSubmitted?.Invoke(currentPlayerPrice);

        // Trigger tutorial manager callback
        TutorialManagerNew tutorialManager = FindObjectOfType<TutorialManagerNew>();
        if (tutorialManager != null)
        {
            tutorialManager.OnPlayerSubmitPrice(currentPlayerPrice);
        }
    }

    void SetPrice(float price)
    {
        currentPlayerPrice = price;

        if (priceSlider != null)
            priceSlider.value = price;

        if (priceInputField != null)
            priceInputField.text = price.ToString("F1");

        UpdatePriceDisplay();
    }

    void UpdatePriceDisplay()
    {
        if (currentPriceText != null)
        {
            currentPriceText.text = $"{currentPlayerPrice:F1}";
        }

        if (priceInputField != null && !priceInputField.isFocused)
        {
            priceInputField.text = currentPlayerPrice.ToString("F1");
        }
    }

    void UpdatePriceFeedback()
    {
        if (priceFeedbackPanel == null) return;

        priceFeedbackPanel.SetActive(true);

        float fairPrice = currentTotalCost * 1.3f;
        float percentAboveFair = ((currentPlayerPrice - fairPrice) / fairPrice) * 100f;

        string feedback = "";
        Color bgColor = fairPriceColor;

        if (currentPlayerPrice < currentTotalCost)
        {
            feedback = "Below Cost!";
            bgColor = belowCostColor;
        }
        else if (percentAboveFair < -10f)
        {
            feedback = "Very Generous";
            bgColor = fairPriceColor;
        }
        else if (percentAboveFair <= 20f)
        {
            feedback = "Fair Price";
            bgColor = fairPriceColor;
        }
        else if (percentAboveFair <= 40f)
        {
            feedback = "Slightly High";
            bgColor = highPriceColor;
        }
        else if (percentAboveFair <= 80f)
        {
            feedback = "Very High!";
            bgColor = absurdPriceColor;
        }
        else
        {
            feedback = "ABSURD!";
            bgColor = absurdPriceColor;
        }

        if (feedbackText != null)
            feedbackText.text = feedback;

        if (feedbackBackground != null)
            feedbackBackground.color = bgColor;
    }

    /// <summary>
    /// Shows profit preview without committing.
    /// </summary>
    public void ShowProfitPreview(float profit)
    {
        if (profitPreviewPanel != null)
        {
            profitPreviewPanel.SetActive(true);

            if (profitPreviewText != null)
            {
                profitPreviewText.text = $"Potential Profit: {profit:F1} pagodas";
            }

            if (profitPreviewBackground != null)
            {
                profitPreviewBackground.color = profit >= 0 
                    ? new Color(0.2f, 0.8f, 0.3f, 0.3f)
                    : new Color(0.9f, 0.2f, 0.2f, 0.3f);
            }

            StartCoroutine(HideAfterDelay(profitPreviewPanel, 2f));
        }
    }

    /// <summary>
    /// Shows deal success effect.
    /// </summary>
    public void ShowDealSuccess()
    {
        if (dealSuccessEffect != null)
        {
            dealSuccessEffect.SetActive(true);
            StartCoroutine(HideAfterDelay(dealSuccessEffect, 2f));
        }

        if (successParticles != null)
        {
            successParticles.Play();
        }

        StartCoroutine(FlashScreen(new Color(0.2f, 0.8f, 0.3f, 0.2f)));
    }

    /// <summary>
    /// Shows deal failure effect.
    /// </summary>
    public void ShowDealFailure()
    {
        if (dealFailureEffect != null)
        {
            dealFailureEffect.SetActive(true);
            StartCoroutine(HideAfterDelay(dealFailureEffect, 2f));
        }

        if (failureParticles != null)
        {
            failureParticles.Play();
        }

        StartCoroutine(FlashScreen(new Color(0.9f, 0.2f, 0.2f, 0.2f)));
    }

    IEnumerator FlashScreen(Color flashColor)
    {
        GameObject flash = new GameObject("ScreenFlash");
        Canvas canvas = flash.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        Image flashImage = flash.AddComponent<Image>();
        flashImage.color = flashColor;

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsed / 0.5f);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        Destroy(flash);
    }

    IEnumerator HideAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }

    void OnDestroy()
    {
        if (priceSlider != null)
            priceSlider.onValueChanged.RemoveListener(OnSliderValueChanged);

        if (submitButton != null)
            submitButton.onClick.RemoveListener(OnSubmitButtonClicked);
    }
}