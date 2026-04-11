using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Displays customer information and stats during the tutorial.
/// Shows patience, desperation, and price knowledge visually.
/// </summary>
public class TutorialCustomerDisplay : MonoBehaviour
{
    [Header("Customer Display UI")]
    public GameObject customerStatsPanel;
    public TextMeshProUGUI customerNameText;
    public Image customerPortrait;
    public Sprite defaultCustomerPortrait;

    [Header("Stat Displays")]
    public Slider patienceSlider;
    public TextMeshProUGUI patienceLabel;
    public TextMeshProUGUI patienceDescription;
    
    public Slider desperationSlider;
    public TextMeshProUGUI desperationLabel;
    public TextMeshProUGUI desperationDescription;
    
    public Slider priceKnowledgeSlider;
    public TextMeshProUGUI priceKnowledgeLabel;
    public TextMeshProUGUI priceKnowledgeDescription;

    [Header("Visual Feedback")]
    public Image patienceBarFill;
    public Image desperationBarFill;
    public Image priceKnowledgeBarFill;
    
    public Color lowStatColor = new Color(0.9f, 0.3f, 0.2f);
    public Color mediumStatColor = new Color(0.9f, 0.7f, 0.2f);
    public Color highStatColor = new Color(0.2f, 0.8f, 0.3f);

    [Header("Animation")]
    public float statRevealDelay = 0.5f;
    public float statFillDuration = 1f;
    public AnimationCurve fillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Info Tooltips")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("VR Settings")]
    public bool vrMode = false;
    public Transform vrAnchor;

    void Start()
    {
        if (customerStatsPanel != null) 
            customerStatsPanel.SetActive(false);
        
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    /// <summary>
    /// Shows customer stats with animated reveals.
    /// </summary>
    public IEnumerator ShowCustomerStats(string name, float patience, float desperation, float priceKnowledge)
    {
        if (customerStatsPanel != null)
        {
            customerStatsPanel.SetActive(true);
            
            // Set customer name and portrait
            if (customerNameText != null)
                customerNameText.text = name;
            
            if (customerPortrait != null && defaultCustomerPortrait != null)
                customerPortrait.sprite = defaultCustomerPortrait;
            
            // Animate panel entrance
            yield return StartCoroutine(AnimatePanelEntrance());
            
            // Reveal stats one by one
            yield return StartCoroutine(RevealStat("Patience", patience, patienceSlider, patienceLabel, 
                patienceDescription, patienceBarFill, GetPatienceDescription(patience)));
            
            yield return new WaitForSeconds(statRevealDelay);
            
            yield return StartCoroutine(RevealStat("Desperation", desperation, desperationSlider, desperationLabel, 
                desperationDescription, desperationBarFill, GetDesperationDescription(desperation)));
            
            yield return new WaitForSeconds(statRevealDelay);
            
            yield return StartCoroutine(RevealStat("Price Knowledge", priceKnowledge, priceKnowledgeSlider, 
                priceKnowledgeLabel, priceKnowledgeBarFill, GetPriceKnowledgeDescription(priceKnowledge)));
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator AnimatePanelEntrance()
    {
        if (customerStatsPanel == null) yield break;
        
        customerStatsPanel.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        float duration = 0.4f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = fillCurve.Evaluate(elapsed / duration);
            customerStatsPanel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        
        customerStatsPanel.transform.localScale = Vector3.one;
    }

    IEnumerator RevealStat(string statName, float value, Slider slider, TextMeshProUGUI label, 
        TextMeshProUGUI description, Image barFill, string descriptionText)
    {
        // Ensure value is normalized (0-1 range)
        float normalizedValue = Mathf.Clamp01(value);
        
        if (slider != null)
        {
            slider.value = 0f;
            
            if (label != null)
                label.text = $"{statName}: 0%";
            
            if (description != null)
            {
                description.text = descriptionText;
                description.gameObject.SetActive(false);
            }
            
            // Animate the slider filling up
            float elapsed = 0f;
            while (elapsed < statFillDuration)
            {
                elapsed += Time.deltaTime;
                float t = fillCurve.Evaluate(elapsed / statFillDuration);
                float currentValue = Mathf.Lerp(0f, normalizedValue, t);
                
                slider.value = currentValue;
                
                if (label != null)
                    label.text = $"{statName}: {(currentValue * 100):F0}%";
                
                // Update color based on current value
                if (barFill != null)
                    barFill.color = GetStatColor(currentValue);
                
                yield return null;
            }
            
            slider.value = normalizedValue;
            
            if (label != null)
                label.text = $"{statName}: {(normalizedValue * 100):F0}%";
            
            if (barFill != null)
                barFill.color = GetStatColor(normalizedValue);
            
            // Show description with fade-in
            if (description != null)
            {
                description.gameObject.SetActive(true);
                yield return StartCoroutine(FadeInText(description));
            }
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI text)
    {
        if (text == null) yield break;
        
        Color originalColor = text.color;
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        float elapsed = 0f;
        float duration = 0.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / duration;
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        text.color = originalColor;
    }

    Color GetStatColor(float value)
    {
        if (value < 0.33f)
            return lowStatColor;
        else if (value < 0.66f)
            return mediumStatColor;
        else
            return highStatColor;
    }

    string GetPatienceDescription(float patience)
    {
        // Patience is typically 1-5 in the game, normalize to 0-1
        float normalized = (patience - 1f) / 4f;
        
        if (normalized < 0.33f)
            return "Impatient - Will walk away quickly if negotiations stall";
        else if (normalized < 0.66f)
            return "Moderate - Can handle a few rounds of negotiation";
        else
            return "Patient - Willing to negotiate extensively";
    }

    string GetDesperationDescription(float desperation)
    {
        if (desperation < 0.33f)
            return "Not Desperate - Has other options, won't overpay";
        else if (desperation < 0.66f)
            return "Moderately Desperate - Willing to pay above fair price";
        else
            return "Very Desperate - May accept high prices if needed";
    }

    string GetPriceKnowledgeDescription(float priceKnowledge)
    {
        if (priceKnowledge < 0.33f)
            return "Low Knowledge - Unfamiliar with market prices";
        else if (priceKnowledge < 0.66f)
            return "Moderate Knowledge - Has some market awareness";
        else
            return "High Knowledge - Knows fair prices well";
    }

    /// <summary>
    /// Hides the customer stats panel.
    /// </summary>
    public void HideCustomerStats()
    {
        if (customerStatsPanel != null)
        {
            StartCoroutine(FadeOutPanel(customerStatsPanel));
        }
    }

    IEnumerator FadeOutPanel(GameObject panel)
    {
        if (panel == null) yield break;
        
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        
        float elapsed = 0f;
        float duration = 0.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / duration);
            yield return null;
        }
        
        panel.SetActive(false);
        cg.alpha = 1f;
    }

    /// <summary>
    /// Shows a tooltip with additional information.
    /// </summary>
    public void ShowTooltip(string text, Vector3 position)
    {
        if (tooltipPanel != null && tooltipText != null)
        {
            tooltipPanel.SetActive(true);
            tooltipText.text = text;
            tooltipPanel.transform.position = position;
        }
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Updates a specific stat bar (used during active negotiation).
    /// </summary>
    public void UpdatePatienceDisplay(float currentPatience, float maxPatience)
    {
        if (patienceSlider != null)
        {
            float normalized = currentPatience / maxPatience;
            patienceSlider.value = normalized;
            
            if (patienceLabel != null)
                patienceLabel.text = $"Patience: {currentPatience}/{maxPatience}";
            
            if (patienceBarFill != null)
                patienceBarFill.color = GetStatColor(normalized);
        }
    }
}