using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiceGuideTutorialPrompt : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Level1HUDManager hudManager;

    [Header("Bhaskara Audio")]
    [SerializeField] private AudioSource merchantAudioSource;
    [SerializeField] private AudioClip spiceGuideIntroClip;

    [Header("Spice Guide Controller")]
    [SerializeField] private SpiceCostGuideController guideController;

    [Header("Input")]
    [SerializeField] private InputActionReference holdGuideAction;

    [Header("World Pause")]
    [SerializeField] private Animator[] animatorsToPause;
    [SerializeField] private MonoBehaviour[] scriptsToPause;
    [SerializeField] private AudioSource[] audioSourcesToLower;

    [Header("Audio Fade")]
    [SerializeField] private float loweredVolume = 0.2f;

    private bool isHolding;
    private float[] originalVolumes;

    private void Awake()
    {

        if (guideController != null)
            guideController.LockGuide();

        originalVolumes = new float[audioSourcesToLower.Length];

        for (int i = 0; i < audioSourcesToLower.Length; i++)
        {
            if (audioSourcesToLower[i] != null)
                originalVolumes[i] = audioSourcesToLower[i].volume;
        }
    }

    public IEnumerator RunTutorial()
    {
        PauseWorld();

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara",
                merchantAudioSource,
                spiceGuideIntroClip,

                new string[]
                {
                "Welcome to your workspace. The marketplace is moving fast today, and your success depends entirely on knowing the baseline worth of what is under your feet.",

                "Look down. Pepper is the black gold of the empire, highly sought after by foreign traders. Turmeric is crucial for everyday meals and medicine. Cardamom is rare and fragrant, prized by temple kitchens. And Cinnamon travels long land routes, making it highly valuable."
                },

                new float[]
                {
                0f,
                8.2f
                }
            )
        );

        PromptManager.Instance.ShowPrompt(
            "Press X to view the Spice Cost Price List.",
            PromptManager.Instance.xButton
        );

        EnableInput();

        yield return new WaitUntil(() => isHolding);

        PromptManager.Instance.HidePrompt();

        DisableInput();

        if (guideController != null)
            guideController.UnlockGuide();

        ResumeWorld();
    }

    private void EnableInput()
    {
        if (holdGuideAction == null)
            return;

        holdGuideAction.action.Enable();
        holdGuideAction.action.performed += OnHoldStarted;
        holdGuideAction.action.canceled += OnHoldEnded;
    }

    private void DisableInput()
    {
        if (holdGuideAction == null)
            return;

        holdGuideAction.action.performed -= OnHoldStarted;
        holdGuideAction.action.canceled -= OnHoldEnded;
        holdGuideAction.action.Disable();
    }

    private void OnHoldStarted(InputAction.CallbackContext context)
    {
        isHolding = true;

        if (guideController != null)
            guideController.ShowGuide();
    }
    private void OnHoldEnded(InputAction.CallbackContext context)
    {
        isHolding = false;

        if (guideController != null)
            guideController.HideGuide();
    }
    private void PauseWorld()
    {
        foreach (Animator animator in animatorsToPause)
        {
            if (animator != null)
                animator.speed = 0f;
        }

        foreach (MonoBehaviour script in scriptsToPause)
        {
            if (script != null)
                script.enabled = false;
        }

        for (int i = 0; i < audioSourcesToLower.Length; i++)
        {
            if (audioSourcesToLower[i] != null)
                audioSourcesToLower[i].volume = originalVolumes[i] * loweredVolume;
        }
    }

    private void ResumeWorld()
    {
        foreach (Animator animator in animatorsToPause)
        {
            if (animator != null)
                animator.speed = 1f;
        }

        foreach (MonoBehaviour script in scriptsToPause)
        {
            if (script != null)
                script.enabled = true;
        }

        for (int i = 0; i < audioSourcesToLower.Length; i++)
        {
            if (audioSourcesToLower[i] != null)
                audioSourcesToLower[i].volume = originalVolumes[i];
        }
    }
    private IEnumerator ShowDialogueSequence(
    string speaker,
    AudioSource audioSource,
    AudioClip clip,
    string[] lines,
    float[] startTimes)
    {
        if (speakerNameText != null)
            speakerNameText.text = speaker + ":";

        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (clip != null && audioSource != null)
            {
                yield return new WaitUntil(() =>
                    audioSource.time >= startTimes[i] ||
                    !audioSource.isPlaying);
            }

            if (dialogueText != null)
                dialogueText.text = lines[i];

            if (hudManager != null)
                hudManager.ShowSubtitle(speaker, lines[i]);
        }

        while (audioSource != null && audioSource.isPlaying)
            yield return null;

        if (hudManager != null)
            hudManager.HideSubtitle();
    }
}