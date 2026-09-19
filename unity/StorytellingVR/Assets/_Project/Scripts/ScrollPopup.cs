using System.Collections;
using UnityEngine;

public class ScrollPopup : MonoBehaviour
{
    [Header("Pop Animation")]
    public float popDuration = 0.6f;
    public float overshootScale = 1.08f;

    [Header("Floating Animation")]
    public float floatAmplitude = 0.03f;
    public float floatSpeed = 1.2f;

    private Vector3 startPosition;
    private Vector3 targetScale;
    private bool isFloating;

    private void OnEnable()
    {
        startPosition = transform.position;

        targetScale = transform.localScale;

        transform.localScale = Vector3.zero;

        StartCoroutine(PopAndFloat());
    }

    IEnumerator PopAndFloat()
    {
        float timer = 0f;

        // Pop In
        while (timer < popDuration)
        {
            timer += Time.deltaTime;

            float t = timer / popDuration;

            float scale = Mathf.Lerp(
                0f,
                overshootScale,
                Mathf.SmoothStep(0f, 1f, t)
            );

            transform.localScale = targetScale * scale;

            yield return null;
        }

        // Settle Back
        timer = 0f;

        while (timer < 0.15f)
        {
            timer += Time.deltaTime;

            transform.localScale = Vector3.Lerp(
                targetScale * overshootScale,
                targetScale,
                timer / 0.15f
            );

            yield return null;
        }

        transform.localScale = targetScale;

        isFloating = true;
    }

    private void Update()
    {
        if (!isFloating)
            return;

        Vector3 pos = startPosition;

        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.position = pos;
    }
}