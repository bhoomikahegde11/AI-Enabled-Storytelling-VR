using UnityEngine;
using System.Collections;

public class InspectManager : MonoBehaviour
{
    [Header("References")]
    public GameObject coin;              // assign in Inspector
    public CanvasGroup overlay;          // dark background (with CanvasGroup)
    public GameObject coinUI;            // info panel UI

    [Header("Settings")]
    public float moveDuration = 0.6f;
    public float startDelay = 1.5f;

    void Start()
    {
        // Initial state
        overlay.alpha = 0f;
        coinUI.SetActive(false);

        // Start full sequence
        StartCoroutine(StartInspectSequence());
    }

    IEnumerator StartInspectSequence()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(MoveToInspect(coin));

        // After animation finishes → show UI
        coinUI.SetActive(true);
    }

    IEnumerator MoveToInspect(GameObject obj)
    {
        Transform t = obj.transform;
        Transform cam = Camera.main.transform;

        Vector3 startPos = t.position;

        //  FINAL POSITION
        Vector3 targetPos = cam.position
                  + cam.forward * 1.3f   // 🔥 main fix (distance)
                  - cam.right * 0.5f     // slight left
                  - cam.up * -0.2f;       // slight down

        Vector3 originalScale = obj.transform.localScale;

        Vector3 startScale = originalScale * 0.3f;
        Vector3 endScale = originalScale * 1.2f;

        float time = 0;

        while (time < moveDuration)
        {
            float tVal = time / moveDuration;

            // Move forward
            t.position = Vector3.Lerp(startPos, targetPos, tVal);

            // Scale up
            t.localScale = Vector3.Lerp(startScale, endScale, tVal);

            // Fade overlay
            overlay.alpha = Mathf.Lerp(0f, 0.6f, tVal);

            time += Time.deltaTime;
            yield return null;
        }

        // Final snap
        t.position = targetPos;
        t.localScale = endScale;
    }
}