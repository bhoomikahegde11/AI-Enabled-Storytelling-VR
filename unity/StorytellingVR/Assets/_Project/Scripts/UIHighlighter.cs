using UnityEngine;
using System.Collections;

public class UIHighlighter : MonoBehaviour
{
    public float highlightScale = 1.15f;
    public float speed = 4f;

    private Vector3 originalScale;
    private Coroutine pulseRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Highlight()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(Pulse());
    }

    public void StopHighlight()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        transform.localScale = originalScale;
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            float scale = 1 + Mathf.Sin(Time.time * speed) * (highlightScale - 1);

            transform.localScale = originalScale * scale;

            yield return null;
        }
    }
}