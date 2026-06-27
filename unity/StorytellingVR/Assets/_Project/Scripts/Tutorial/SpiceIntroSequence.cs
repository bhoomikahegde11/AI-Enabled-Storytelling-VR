using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpiceIntroSequence : MonoBehaviour
{
    [System.Serializable]
    public class SpiceStep
    {
        public string spiceName;
        public string costText;

        [TextArea(2, 4)]
        public string narration;

        public AudioClip narrationClip;
        public Transform uiAnchor;
    }

    [Header("Audio")]
    [SerializeField] private AudioSource narratorAudioSource;

    [Header("Intro Audio")]
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip endingClip;

    [Header("Subtitle UI")]
    [SerializeField] private CanvasGroup subtitleCanvas;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Spice Popup UI")]
    [SerializeField] private CanvasGroup spiceInfoCanvas;
    [SerializeField] private TMP_Text spiceNameText;
    [SerializeField] private TMP_Text spicePriceText;

    [Header("Spices")]
    [SerializeField] private SpiceStep[] spices;

    [Header("Final Tutorial Prompt")]
    [SerializeField] private float finalPromptDuration = 4f;
    [SerializeField]
    private string finalPromptText =
        "You can refer to these cost prices during trade by pressing X.";

    [Header("Optional Camera Focus")]
    [SerializeField] private Volume globalVolume;
    private DepthOfField dof;

    [Header("Scene Transition")]
    [SerializeField] private bool loadNextSceneAfterSequence = true;

    [Header("Spice Guide Tutorial")]
    [SerializeField] private SpiceGuideTutorialPrompt spiceGuideTutorialPrompt;
    private void Awake()
    {
        if (globalVolume != null)
            globalVolume.profile.TryGet(out dof);

        SetCanvas(subtitleCanvas, 0f);
        SetCanvas(spiceInfoCanvas, 0f);
    }

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (dof != null)
            dof.active = true;

        yield return FadeCanvas(subtitleCanvas, 1f, 0.5f);

        yield return ShowSubtitle(
            "Welcome to your spice stall in the Hampi Bazaar. Traders from distant lands come here seeking valuable goods, but your success depends on knowing your costs.",
            introClip,
            5f
        );

        foreach (SpiceStep spice in spices)
        {
            yield return FocusOnSpice(spice);
        }

        yield return ShowSubtitle(
            "Remember these goods well. Knowing their worth may decide the success of your trade.",
            endingClip,
            4f
        );

        if (spiceGuideTutorialPrompt != null)
        {
            yield return spiceGuideTutorialPrompt.RunTutorial();
        }

        yield return ShowSubtitle(finalPromptText, null, finalPromptDuration);

        yield return new WaitForSeconds(1f);

        if (loadNextSceneAfterSequence)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.LoadNextScene();
            else
                Debug.LogWarning("GameManager.Instance is missing. Cannot load next scene.");
        }
    }

    private IEnumerator FocusOnSpice(SpiceStep spice)
    {
        if (spice == null)
            yield break;

        if (spice.uiAnchor != null && spiceInfoCanvas != null)
        {
            spiceInfoCanvas.transform.position = spice.uiAnchor.position;
            spiceInfoCanvas.transform.rotation = spice.uiAnchor.rotation;
        }

        if (spiceNameText != null)
            spiceNameText.text = spice.spiceName;

        if (spicePriceText != null)
            spicePriceText.text = spice.costText;

        if (dof != null)
        {
            dof.focusDistance.value = 2f;
            dof.gaussianStart.value = 1f;
            dof.gaussianEnd.value = 3f;
        }

        yield return FadeCanvas(spiceInfoCanvas, 1f, 0.4f);

        yield return ShowSubtitle(spice.narration, spice.narrationClip, 4f);

        yield return new WaitForSeconds(0.5f);

        yield return FadeCanvas(spiceInfoCanvas, 0f, 0.4f);
    }

    private IEnumerator ShowSubtitle(string message, AudioClip clip, float fallbackTime)
    {
        if (subtitleText != null)
            subtitleText.text = message;

        if (clip != null && narratorAudioSource != null)
        {
            narratorAudioSource.Stop();
            narratorAudioSource.clip = clip;
            narratorAudioSource.Play();

            yield return new WaitWhile(() => narratorAudioSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(fallbackTime);
        }
    }

    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.gameObject.SetActive(true);

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        SetCanvas(canvasGroup, targetAlpha);
    }

    private void SetCanvas(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = alpha > 0.01f;
        canvasGroup.blocksRaycasts = alpha > 0.01f;
    }
}