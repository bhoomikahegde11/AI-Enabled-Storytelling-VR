using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIHighlighter : MonoBehaviour
{
    public float highlightScale = 1.35f;
    public float speed = 2.5f;

    private Vector3 originalScale;
    private Coroutine pulseRoutine;
    private Image panelImage;
    private Color originalColor;

    [Header("Glow")]
    public Color glowColor = new Color(1f, 0.95f, 0.45f, 1f);
    void Awake()
    {
        originalScale = transform.localScale;

        panelImage = GetComponent<Image>();

        if (panelImage != null)
            originalColor = panelImage.color;
    }

    public void Highlight()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        if (panelImage != null)
            panelImage.color = glowColor;

        pulseRoutine = StartCoroutine(Pulse());
    }

    public void StopHighlight()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        transform.localScale = originalScale;

        if (panelImage != null)
            panelImage.color = originalColor;
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            float scale =
                1f + Mathf.Sin(Time.time * 3f) * 0.18f;

            transform.localScale = originalScale * scale;

            yield return null;
        }
    }
}