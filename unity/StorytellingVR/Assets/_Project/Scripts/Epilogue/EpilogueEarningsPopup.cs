
using System.Collections;
using TMPro;
using UnityEngine;

public class EpilogueEarningsPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private float fadeDuration = 0.4f;

    private Coroutine animationRoutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowEarnings(int amount)
    {
        if (canvasGroup == null || amountText == null)
        {
            Debug.LogError("[EPILOGUE EARNINGS] UI references missing.");
            return;
        }

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        amountText.text = $"+{amount} Varahas";
        animationRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        animationRoutine = null;
    }
}
