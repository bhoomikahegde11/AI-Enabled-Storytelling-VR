using System.Collections;
using TMPro;
using UnityEngine;

public class NarratorUIManager : MonoBehaviour
{
    public static NarratorUIManager Instance;

    [Header("UI")]
    public GameObject narratorCanvas;
    public TMP_Text speakerText;
    public TMP_Text subtitleText;

    [Header("Timing")]
    public float defaultDuration = 5f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideNarrator();
    }

    public void ShowNarration(string speaker, string subtitle, float duration = -1f)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(NarrationRoutine(speaker, subtitle, duration));
    }

    private IEnumerator NarrationRoutine(string speaker, string subtitle, float duration)
    {
        narratorCanvas.SetActive(true);

        speakerText.text = speaker;
        subtitleText.text = subtitle;

        yield return new WaitForSeconds(duration > 0 ? duration : defaultDuration);

        HideNarrator();
    }

    public void HideNarrator()
    {
        if (narratorCanvas != null)
            narratorCanvas.SetActive(false);
    }
}