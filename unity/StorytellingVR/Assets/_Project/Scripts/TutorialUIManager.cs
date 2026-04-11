using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages all tutorial UI elements including respect score, profit display, and visual feedback.
/// VR-ready with world-space canvas support.
/// </summary>
public class TutorialUIManager : MonoBehaviour
{
    [Header("Respect Score UI")]
    public TextMeshProUGUI respectScoreText;
    public TextMeshProUGUI respectLabelText;
    public Image respectScoreBackground;
    public GameObject respectChangeIndicator;
    public TextMeshProUGUI respectChangeText;
    public Color respectGainColor = new Color(0.2f, 0.8f, 0.3f);
    public Color respectLossColor = new Color(0.9f, 0.2f, 0.2f);
    public float respectFlashDuration = 0.5f;

    [Header("Profit Display")]
    public GameObject profitPanel;
    public TextMeshProUGUI revenueText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI profitText;
    public Image profitBackground;
    public Color profitPositiveColor = new Color(0.2f, 0.8f, 0.3f, 0.3f);
    public Color profitNegativeColor = new Color(0.9f, 0.2f, 0.2f, 0.3f);

    [Header("Transaction Preview")]
    public GameObject previewPanel;
    public TextMeshProUGUI previewProfitText;
    public TextMeshProUGUI previewRespectText;
    public Image previewBackground;

    [Header("Tutorial Complete Screen")]
    public GameObject tutorialCompletePanel;
    public TextMeshProUGUI finalProfitText;
    public TextMeshProUGUI finalRespectText;
    public Button continueButton;

    [Header("General UI")]
    public GameObject tutorialOverlay;
    public Image fadeOverlay;

    [Header("VR Settings")]
    public bool vrMode = false;
    public float vrCanvasDistance = 2f;
    public Transform vrCameraTransform;

    [Header("Animation Settings")]
    public AnimationCurve bounceScale = AnimationCurve.EaseInOut(0, 1, 1, 1);
    public float animationDuration = 0.3f;

    private int currentRespectScore = 50;
    private Coroutine respectFlashCoroutine;

    void Start()
    {
        InitializeUI();
        
        if (vrMode && vrCameraTransform != null)
        {
            SetupVRCanvas();
        }
    }

    void InitializeUI()
    {
        if (profitPanel != null) profitPanel.SetActive(true);
        if (previewPanel != null) previewPanel.SetActive(false);
        if (tutorialCompletePanel != null) tutorialCompletePanel.SetActive(false);
        
        UpdateRespectScore(50, 0);
        UpdateProfitDisplay(0, 0, 0);
    }

    void SetupVRCanvas()
    {
        // Position canvas in front of VR camera
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = vrCameraTransform.position + vrCameraTransform.forward * vrCanvasDistance;
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - vrCameraTransform.position);
            
            // Scale for VR readability
            canvas.transform.localScale = Vector3.one * 0.001f;
        }
    }

    /// <summary>
    /// Updates the respect score display with optional change indicator.
    /// </summary>
    public void UpdateRespectScore(int newScore, int change)
    {
        currentRespectScore = Mathf.Clamp(newScore, 0, 100);
        
        if (respectScoreText != null)
        {
            respectScoreText.text = currentRespectScore.ToString();
        }
        
        if (respectLabelText != null)
        {
            respectLabelText.text = GetRespectLabel(currentRespectScore);
        }
        
        if (change != 0)
        {
            ShowRespectChange(change);
        }
    }

    /// <summary>
    /// Shows a temporary respect change indicator (doesn't update actual score).
    /// </summary>
    public void ShowTemporaryRespectChange(float change)
    {
        if (respectChangeIndicator != null && respectChangeText != null)
        {
            respectChangeIndicator.SetActive(true);
            
            if (change > 0)
            {
                respectChangeText.text = $"+{change:F0}";
                respectChangeText.color = respectGainColor;
            }
            else
            {
                respectChangeText.text = $"{change:F0}";
                respectChangeText.color = respectLossColor;
            }
            
            StartCoroutine(HideTemporaryIndicator(respectChangeIndicator, 2f));
        }
    }

    void ShowRespectChange(int change)
    {
        if (respectChangeIndicator != null && respectChangeText != null)
        {
            respectChangeIndicator.SetActive(true);
            
            if (change > 0)
            {
                respectChangeText.text = $"+{change}";
                respectChangeText.color = respectGainColor;
            }
            else
            {
                respectChangeText.text = change.ToString();
                respectChangeText.color = respectLossColor;
            }
            
            StartCoroutine(AnimateRespectChange(change > 0));
        }
        
        // Flash the background
        if (respectFlashCoroutine != null)
        {
            StopCoroutine(respectFlashCoroutine);
        }
        respectFlashCoroutine = StartCoroutine(FlashRespectBackground(change > 0));
    }

    IEnumerator AnimateRespectChange(bool positive)
    {
        if (respectChangeIndicator == null) yield break;
        
        Vector3 originalScale = respectChangeIndicator.transform.localScale;
        respectChangeIndicator.transform.localScale = originalScale * 0.5f;
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0.5f, 1.2f, bounceScale.Evaluate(elapsed / animationDuration));
            respectChangeIndicator.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        // Hold for a moment
        yield return new WaitForSeconds(1.5f);
        
        // Fade out
        CanvasGroup cg = respectChangeIndicator.GetComponent<CanvasGroup>();
        if (cg == null) cg = respectChangeIndicator.AddComponent<CanvasGroup>();
        
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / 0.5f);
            yield return null;
        }
        
        respectChangeIndicator.SetActive(false);
        cg.alpha = 1f;
        respectChangeIndicator.transform.localScale = originalScale;
    }

    IEnumerator FlashRespectBackground(bool positive)
    {
        if (respectScoreBackground == null) yield break;
        
        Color originalColor = respectScoreBackground.color;
        Color flashColor = positive ? respectGainColor : respectLossColor;
        flashColor.a = 0.5f;
        
        float elapsed = 0f;
        while (elapsed < respectFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / respectFlashDuration;
            respectScoreBackground.color = Color.Lerp(flashColor, originalColor, t);
            yield return null;
        }
        
        respectScoreBackground.color = originalColor;
    }

    /// <summary>
    /// Updates the profit display panel.
    /// </summary>
    public void UpdateProfitDisplay(float revenue, float cost, float profit)
    {
        if (revenueText != null)
        {
            revenueText.text = $"Revenue: {revenue:F1} pagodas";
        }
        
        if (costText != null)
        {
            costText.text = $"Cost: {cost:F1} pagodas";
        }
        
        if (profitText != null)
        {
            profitText.text = $"Profit: {profit:F1} pagodas";
            profitText.color = profit >= 0 ? respectGainColor : respectLossColor;
        }
        
        if (profitBackground != null)
        {
            profitBackground.color = profit >= 0 ? profitPositiveColor : profitNegativeColor;
        }
    }

    /// <summary>
    /// Shows a preview of what profit would be (temporary, doesn't update main display).
    /// </summary>
    public void ShowProfitPreview(float profit)
    {
        if (previewPanel == null) return;
        
        previewPanel.SetActive(true);
        
        if (previewProfitText != null)
        {
            previewProfitText.text = $"Potential Profit: {profit:F1} pagodas";
            previewProfitText.color = profit >= 0 ? respectGainColor : respectLossColor;
        }
        
        if (previewBackground != null)
        {
            previewBackground.color = profit >= 0 ? profitPositiveColor : profitNegativeColor;
        }
        
        StartCoroutine(HideTemporaryIndicator(previewPanel, 2.5f));
    }

    IEnumerator HideTemporaryIndicator(GameObject indicator, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (indicator != null)
        {
            CanvasGroup cg = indicator.GetComponent<CanvasGroup>();
            if (cg == null) cg = indicator.AddComponent<CanvasGroup>();
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                cg.alpha = 1f - (elapsed / 0.5f);
                yield return null;
            }
            
            indicator.SetActive(false);
            cg.alpha = 1f;
        }
    }

    /// <summary>
    /// Shows the tutorial complete screen.
    /// </summary>
    public void ShowTutorialCompleteScreen(float totalProfit, int finalRespect)
    {
        if (tutorialCompletePanel == null) return;
        
        tutorialCompletePanel.SetActive(true);
        
        if (finalProfitText != null)
        {
            finalProfitText.text = $"Total Profit: {totalProfit:F1} pagodas";
            finalProfitText.color = totalProfit >= 0 ? respectGainColor : respectLossColor;
        }
        
        if (finalRespectText != null)
        {
            finalRespectText.text = $"Respect: {finalRespect} - {GetRespectLabel(finalRespect)}";
        }
        
        StartCoroutine(AnimateTutorialComplete());
    }

    IEnumerator AnimateTutorialComplete()
    {
        if (tutorialCompletePanel == null) yield break;
        
        CanvasGroup cg = tutorialCompletePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = tutorialCompletePanel.AddComponent<CanvasGroup>();
        
        cg.alpha = 0f;
        tutorialCompletePanel.transform.localScale = Vector3.one * 0.8f;
        
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.5f;
            cg.alpha = t;
            tutorialCompletePanel.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);
            yield return null;
        }
        
        cg.alpha = 1f;
        tutorialCompletePanel.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Fades the screen in or out.
    /// </summary>
    public IEnumerator FadeScreen(bool fadeOut, float duration = 0.5f)
    {
        if (fadeOverlay == null) yield break;
        
        fadeOverlay.gameObject.SetActive(true);
        
        float start = fadeOut ? 0f : 1f;
        float end = fadeOut ? 1f : 0f;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, elapsed / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        fadeOverlay.color = new Color(0, 0, 0, end);
        
        if (!fadeOut)
        {
            fadeOverlay.gameObject.SetActive(false);
        }
    }

    string GetRespectLabel(int score)
    {
        if (score >= 80) return "Beloved";
        if (score >= 60) return "Respected";
        if (score >= 40) return "Neutral";
        if (score >= 20) return "Distrusted";
        return "Infamous";
    }

    // VR Update - keep UI facing camera
    void LateUpdate()
    {
        if (vrMode && vrCameraTransform != null)
        {
            Canvas[] canvases = GetComponentsInChildren<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    canvas.transform.LookAt(vrCameraTransform);
                    canvas.transform.Rotate(0, 180, 0); // Face camera
                }
            }
        }
    }
}