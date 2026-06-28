using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiceGuideTutorialPrompt : MonoBehaviour
{
    [Header("Tutorial Prompt UI")]
    [SerializeField] private CanvasGroup promptCanvas;
    [SerializeField] private TMP_Text promptText;

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
        SetCanvas(promptCanvas, 0f);

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

        if (promptText != null)
        {
            promptText.text =
                "NEW REFERENCE TOOL\n\n" +
                "Hold X to open the Spice Cost Guide.\n\n" +
                "Release X after checking the prices.";
        }

        yield return FadeCanvas(promptCanvas, 1f, 0.4f);

        EnableInput();

        //yield return new WaitUntil(() => isHolding);

        //yield return FadeCanvas(promptCanvas, 0f, 0.25f);

        //if (guideController != null)
        //    guideController.ShowGuide();

        //yield return new WaitForSeconds(1.5f);

        //yield return new WaitUntil(() => !isHolding);

        //if (guideController != null)
        //    guideController.HideGuide();

        DisableInput();

        //if (guideController != null)
            //guideController.UnlockGuide();

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
    }

    private void OnHoldEnded(InputAction.CallbackContext context)
    {
        isHolding = false;
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