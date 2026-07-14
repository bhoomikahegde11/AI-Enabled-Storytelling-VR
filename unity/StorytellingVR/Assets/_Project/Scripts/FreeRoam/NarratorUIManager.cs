using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public enum ContinueInputType
{
    RightTrigger,
    RightAButton,
    LeftTrigger,
    LeftXButton
}

public class NarratorUIManager : MonoBehaviour
{
    public static NarratorUIManager Instance;

    [Header("UI")]
    public GameObject narratorCanvas;
    public TMP_Text speakerText;
    public TMP_Text subtitleText;

    [Header("Normal Narration")]
    public float defaultDuration = 5f;

    [Header("Line-by-Line Subtitles")]
    public ContinueInputType continueInput = ContinueInputType.RightTrigger;

    [Tooltip("How long each subtitle line remains before advancing automatically.")]
    public float lineAutoAdvanceDuration = 3.5f;

    [Tooltip("Allows the configured button to advance before the timer ends.")]
    public bool allowManualAdvance = true;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideNarrator();
    }

    public void ShowNarration(
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle, duration)
        );
    }

    public IEnumerator PlayNarration(
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle, duration)
        );

        yield return currentRoutine;
        currentRoutine = null;
    }

    public IEnumerator PlayNarrationLineByLine(
        string speaker,
        string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText))
            yield break;

        StopCurrentRoutine();
        SetLaserPointerEnabled(false);

        string[] lines = fullText.Split(
            new[] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        ShowCanvas();

        if (speakerText != null)
            speakerText.text = speaker;

        // Record the current state so a trigger held from a UI click
        // does not instantly skip the first subtitle.
        bool previousPressed = GetContinueButtonPressed();

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (subtitleText != null)
                subtitleText.text = line;

            float elapsed = 0f;
            bool advanceLine = false;

            while (!advanceLine)
            {
                bool currentlyPressed = GetContinueButtonPressed();

                // Only treat a fresh press as a manual advance.
                if (allowManualAdvance &&
                    currentlyPressed &&
                    !previousPressed)
                {
                    advanceLine = true;
                }

                previousPressed = currentlyPressed;

                elapsed += Time.unscaledDeltaTime;

                if (elapsed >= lineAutoAdvanceDuration)
                    advanceLine = true;

                yield return null;
            }

            // Small separation prevents accidental double-skips.
            yield return null;
        }

        HideNarrator();
    }

    private IEnumerator NarrationRoutine(
        string speaker,
        string subtitle,
        float duration)
    {
        ShowCanvas();

        if (speakerText != null)
            speakerText.text = speaker;

        if (subtitleText != null)
            subtitleText.text = subtitle;

        float actualDuration =
            duration > 0f ? duration : defaultDuration;

        yield return new WaitForSecondsRealtime(actualDuration);

        HideNarrator();
    }

    private void ShowCanvas()
    {
        if (narratorCanvas != null)
            narratorCanvas.SetActive(true);
    }

    public void HideNarrator()
    {
        if (narratorCanvas != null)
            narratorCanvas.SetActive(false);
    }

    private void StopCurrentRoutine()
    {
        if (currentRoutine == null)
            return;

        StopCoroutine(currentRoutine);
        currentRoutine = null;
    }

    private bool GetContinueButtonPressed()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space) ||
            Input.GetMouseButton(0))
        {
            return true;
        }
#endif

        InputDevice device;
        bool pressed;

        switch (continueInput)
        {
            case ContinueInputType.RightTrigger:
                device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                return device.TryGetFeatureValue(
                    CommonUsages.triggerButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.RightAButton:
                device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                return device.TryGetFeatureValue(
                    CommonUsages.primaryButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.LeftTrigger:
                device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                return device.TryGetFeatureValue(
                    CommonUsages.triggerButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.LeftXButton:
                device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                return device.TryGetFeatureValue(
                    CommonUsages.primaryButton,
                    out pressed
                ) && pressed;
        }

        return false;
    }

    private void SetLaserPointerEnabled(bool enabled)
    {
        NPCDialogueVRLaserPointer laser =
            Object.FindAnyObjectByType<NPCDialogueVRLaserPointer>(
                FindObjectsInactive.Include
            );

        if (laser != null)
            laser.enabled = enabled;
    }
}