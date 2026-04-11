using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Displays spices and their details during the tutorial introduction phase.
/// Shows all available spices, then highlights the one used in the tutorial.
/// </summary>
public class TutorialSpiceDisplay : MonoBehaviour
{
    [System.Serializable]
    public class SpiceData
    {
        public string name;
        public string unit;
        public float costPerUnit;
        public string category;
        public Sprite icon;
    }

    [Header("Spice Data")]
    public List<SpiceData> tutorialSpices = new List<SpiceData>();

    [Header("Display UI")]
    public GameObject spiceDisplayPanel;
    public Transform spiceGridContainer;
    public GameObject spiceCardPrefab;
    
    [Header("Highlight UI")]
    public GameObject highlightPanel;
    public Image highlightSpiceIcon;
    public TextMeshProUGUI highlightSpiceName;
    public TextMeshProUGUI highlightCostText;
    public TextMeshProUGUI highlightUnitText;
    public TextMeshProUGUI highlightCategoryText;
    public Image highlightGlow;

    [Header("Animation")]
    public float cardSpawnDelay = 0.2f;
    public float cardAnimDuration = 0.3f;
    public AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("VR Settings")]
    public bool vrMode = false;
    public Transform vrAnchor;

    private List<GameObject> spawnedCards = new List<GameObject>();

    void Start()
    {
        if (spiceDisplayPanel != null) spiceDisplayPanel.SetActive(false);
        if (highlightPanel != null) highlightPanel.SetActive(false);
        
        InitializeTutorialSpices();
    }

    /// <summary>
    /// Initialize hardcoded tutorial spices.
    /// In the real game, this will load from trader_goods.json.
    /// </summary>
    void InitializeTutorialSpices()
    {
        tutorialSpices.Clear();
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Cardamom", 
            unit = "measure", 
            costPerUnit = 15f, 
            category = "Premium Spice" 
        });
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Black Pepper", 
            unit = "measure", 
            costPerUnit = 8f, 
            category = "Common Spice" 
        });
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Cinnamon", 
            unit = "measure", 
            costPerUnit = 12f, 
            category = "Premium Spice" 
        });
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Turmeric", 
            unit = "measure", 
            costPerUnit = 5f, 
            category = "Common Spice" 
        });
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Cloves", 
            unit = "measure", 
            costPerUnit = 20f, 
            category = "Luxury Spice" 
        });
        
        tutorialSpices.Add(new SpiceData 
        { 
            name = "Nutmeg", 
            unit = "measure", 
            costPerUnit = 18f, 
            category = "Premium Spice" 
        });
    }

    /// <summary>
    /// Shows all spices in an animated grid.
    /// </summary>
    public IEnumerator ShowAllSpices()
    {
        if (spiceDisplayPanel != null)
        {
            spiceDisplayPanel.SetActive(true);
        }

        ClearSpawnedCards();

        foreach (SpiceData spice in tutorialSpices)
        {
            GameObject card = CreateSpiceCard(spice);
            if (card != null)
            {
                spawnedCards.Add(card);
                StartCoroutine(AnimateCardSpawn(card));
                yield return new WaitForSeconds(cardSpawnDelay);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    GameObject CreateSpiceCard(SpiceData spice)
    {
        if (spiceCardPrefab == null || spiceGridContainer == null)
        {
            Debug.LogWarning("Spice card prefab or container not assigned!");
            return null;
        }

        GameObject card = Instantiate(spiceCardPrefab, spiceGridContainer);
        
        // Find and set UI elements in the card
        TextMeshProUGUI nameText = card.transform.Find("SpiceName")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null) nameText.text = spice.name;
        
        TextMeshProUGUI costText = card.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
        if (costText != null) costText.text = $"{spice.costPerUnit:F1} pagodas/{spice.unit}";
        
        TextMeshProUGUI categoryText = card.transform.Find("CategoryText")?.GetComponent<TextMeshProUGUI>();
        if (categoryText != null) categoryText.text = spice.category;
        
        Image iconImage = card.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null && spice.icon != null) iconImage.sprite = spice.icon;
        
        // Start invisible for animation
        card.transform.localScale = Vector3.zero;
        
        return card;
    }

    IEnumerator AnimateCardSpawn(GameObject card)
    {
        if (card == null) yield break;
        
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one;
        
        while (elapsed < cardAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = spawnCurve.Evaluate(elapsed / cardAnimDuration);
            card.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        
        card.transform.localScale = targetScale;
    }

    /// <summary>
    /// Highlights a specific spice for the tutorial transaction.
    /// </summary>
    public void HighlightSpice(string spiceName, float costPerUnit)
    {
        SpiceData spice = tutorialSpices.Find(s => s.name == spiceName);
        
        if (spice == null)
        {
            Debug.LogWarning($"Spice '{spiceName}' not found in tutorial spices!");
            return;
        }
        
        StartCoroutine(ShowHighlightPanel(spice));
    }

    IEnumerator ShowHighlightPanel(SpiceData spice)
    {
        // Hide the grid
        if (spiceDisplayPanel != null)
        {
            yield return StartCoroutine(FadeOut(spiceDisplayPanel));
        }
        
        // Show highlight panel
        if (highlightPanel != null)
        {
            highlightPanel.SetActive(true);
            
            if (highlightSpiceName != null) 
                highlightSpiceName.text = spice.name;
            
            if (highlightCostText != null) 
                highlightCostText.text = $"Cost: {spice.costPerUnit:F1} pagodas per {spice.unit}";
            
            if (highlightUnitText != null)
                highlightUnitText.text = $"Unit: {spice.unit}";
            
            if (highlightCategoryText != null)
                highlightCategoryText.text = spice.category;
            
            if (highlightSpiceIcon != null && spice.icon != null)
                highlightSpiceIcon.sprite = spice.icon;
            
            yield return StartCoroutine(AnimateHighlight());
        }
    }

    IEnumerator AnimateHighlight()
    {
        if (highlightPanel == null) yield break;
        
        // Scale animation
        highlightPanel.transform.localScale = Vector3.zero;
        float elapsed = 0f;
        
        while (elapsed < cardAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = spawnCurve.Evaluate(elapsed / cardAnimDuration);
            highlightPanel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        
        highlightPanel.transform.localScale = Vector3.one;
        
        // Pulsing glow effect
        if (highlightGlow != null)
        {
            StartCoroutine(PulseGlow());
        }
    }

    IEnumerator PulseGlow()
    {
        if (highlightGlow == null) yield break;
        
        Color originalColor = highlightGlow.color;
        
        while (highlightPanel != null && highlightPanel.activeSelf)
        {
            // Pulse up
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.2f, 0.6f, elapsed);
                highlightGlow.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            
            // Pulse down
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.6f, 0.2f, elapsed);
                highlightGlow.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
    }

    public void HideHighlight()
    {
        if (highlightPanel != null)
        {
            StartCoroutine(FadeOut(highlightPanel));
        }
    }

    IEnumerator FadeOut(GameObject panel)
    {
        if (panel == null) yield break;
        
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / 0.3f);
            yield return null;
        }
        
        panel.SetActive(false);
        cg.alpha = 1f;
    }

    void ClearSpawnedCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        spawnedCards.Clear();
    }

    void OnDestroy()
    {
        ClearSpawnedCards();
    }
}