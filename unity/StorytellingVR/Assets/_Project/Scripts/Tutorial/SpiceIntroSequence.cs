using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpiceIntroSequence : MonoBehaviour
{
    private const string IntroLineId = "BHASKARA_SPICE_INTRO_WELCOME_01";
    private const string EndingLineId = "BHASKARA_SPICE_INTRO_REMEMBER_01";

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
    [SerializeField] private DialogueVoiceDatabase voiceDatabase;

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

    [Header("Spice Cost Guide")]
    [SerializeField] private SpiceCostGuideController spiceCostGuideController;

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
            "Before you begin trading, you should know the spices we keep here and what each one has cost us.",
            IntroLineId,
            introClip,
            5f
        );

        foreach (SpiceStep spice in spices)
        {
            yield return FocusOnSpice(spice);
        }

        yield return ShowSubtitle(
            "Remember, the price we paid matters when you bargain. Sell above your cost to earn a profit, but choose your price carefully.",
            EndingLineId,
            endingClip,
            4f
        );

        yield return ShowSubtitle(finalPromptText, null, null, finalPromptDuration);

        if (spiceGuideTutorialPrompt != null)
        {
            yield return spiceGuideTutorialPrompt.RunTutorial();
            spiceCostGuideController.UnlockGuide();
        }
        



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

        yield return ShowSubtitle(
            spice.narration,
            GetSpiceLineId(spice.spiceName),
            spice.narrationClip,
            4f
        );

        yield return new WaitForSeconds(0.5f);

        yield return FadeCanvas(spiceInfoCanvas, 0f, 0.4f);
    }

    private IEnumerator ShowSubtitle(string message, string lineId, AudioClip clip, float fallbackTime)
    {
        if (subtitleText != null)
            subtitleText.text = message;

        AudioClip resolvedClip = ResolveVoiceClip(lineId, clip);

        if (resolvedClip != null && narratorAudioSource != null)
        {
            narratorAudioSource.Stop();
            narratorAudioSource.clip = resolvedClip;
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

    private AudioClip ResolveVoiceClip(string lineId, AudioClip fallbackClip)
    {
        if (voiceDatabase == null || string.IsNullOrEmpty(lineId))
            return fallbackClip;

        AudioClip databaseClip = voiceDatabase.GetAudioClip(lineId);
        return databaseClip != null ? databaseClip : fallbackClip;
    }

    private string GetSpiceLineId(string spiceName)
    {
        if (string.IsNullOrWhiteSpace(spiceName))
            return null;

        switch (spiceName.Trim().ToLowerInvariant())
        {
            case "pepper":
                return "BHASKARA_SPICE_INTRO_PEPPER_01";
            case "turmeric":
                return "BHASKARA_SPICE_INTRO_TURMERIC_01";
            case "cardamom":
                return "BHASKARA_SPICE_INTRO_CARDAMOM_01";
            case "cinnamon":
                return "BHASKARA_SPICE_INTRO_CINNAMON_01";
            default:
                return null;
        }
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
