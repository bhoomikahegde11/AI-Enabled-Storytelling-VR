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

    [Header("Continue Input")]
    public ContinueInputType continueInput =
        ContinueInputType.RightTrigger;

    [Header("Typewriter")]
    [Tooltip("Number of visible characters revealed every second.")]
    [Min(1f)]
    public float charactersPerSecond = 40f;

    [Tooltip("Adds a slightly longer pause after commas.")]
    [Min(0f)]
    public float commaPause = 0.08f;

    [Tooltip("Adds a slightly longer pause after full stops, question marks and exclamation marks.")]
    [Min(0f)]
    public float sentencePause = 0.16f;

    [Tooltip("While enabled, dialogue waits for a fresh button press after the complete line is visible.")]
    public bool waitForManualContinue = true;

    [Header("Editor Testing")]
    [Tooltip("In the Editor, Space or left mouse click acts as the continue input.")]
    public bool allowEditorInput = true;

    private Coroutine currentRoutine;

    private NPCDialogueVRLaserPointer cachedLaserPointer;
    private bool previousLaserEnabledState;
    private bool laserStateCaptured;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        HideNarrator();
    }

    /// <summary>
    /// Starts narration without waiting for it to finish.
    /// Existing calls can continue using this method.
    /// </summary>
    public void ShowNarration(
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle)
        );
    }

    /// <summary>
    /// Plays narration and waits until all lines have been completed.
    /// The duration parameter is retained for compatibility with existing calls.
    /// </summary>
    public IEnumerator PlayNarration(
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle)
        );

        yield return currentRoutine;

        currentRoutine = null;
    }

    /// <summary>
    /// Plays text one line at a time.
    /// Each newline becomes a separate dialogue page.
    /// </summary>
    public IEnumerator PlayNarrationLineByLine(
        string speaker,
        string fullText)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, fullText)
        );

        yield return currentRoutine;

        currentRoutine = null;
    }

    private IEnumerator NarrationRoutine(
        string speaker,
        string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText))
            yield break;

        BeginDialoguePresentation();

        if (speakerText != null)
            speakerText.text = speaker;

        string[] lines = fullText.Split(
            new[] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        // Prevent a trigger already being held from instantly
        // completing the first line.
        bool previousPressed = GetContinueButtonPressed();

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            yield return TypeAndWaitForLine(
                line,
                previousPressed,
                pressedState =>
                {
                    previousPressed = pressedState;
                }
            );

            // One frame of separation between lines.
            yield return null;
        }

        EndDialoguePresentation();
    }

    private IEnumerator TypeAndWaitForLine(
        string line,
        bool startingPressedState,
        System.Action<bool> updatePressedState)
    {
        if (subtitleText == null)
            yield break;

        subtitleText.text = line;
        subtitleText.maxVisibleCharacters = 0;

        // TMP needs to calculate the text layout before we can
        // reliably read the visible character count.
        subtitleText.ForceMeshUpdate();

        TMP_TextInfo textInfo = subtitleText.textInfo;
        int totalCharacters = textInfo.characterCount;

        bool previousPressed = startingPressedState;
        bool textCompletedInstantly = false;

        for (int visibleCount = 0;
             visibleCount < totalCharacters;
             visibleCount++)
        {
            float characterDelay =
                1f / Mathf.Max(1f, charactersPerSecond);

            char currentCharacter =
                textInfo.characterInfo[visibleCount].character;

            if (currentCharacter == ',')
            {
                characterDelay += commaPause;
            }
            else if (
                currentCharacter == '.' ||
                currentCharacter == '!' ||
                currentCharacter == '?' ||
                currentCharacter == ':' ||
                currentCharacter == ';')
            {
                characterDelay += sentencePause;
            }

            float elapsed = 0f;

            while (elapsed < characterDelay)
            {
                bool currentlyPressed =
                    GetContinueButtonPressed();

                bool freshPress =
                    currentlyPressed &&
                    !previousPressed;

                previousPressed = currentlyPressed;
                updatePressedState?.Invoke(previousPressed);

                if (freshPress)
                {
                    subtitleText.maxVisibleCharacters =
                        totalCharacters;

                    textCompletedInstantly = true;
                    break;
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (textCompletedInstantly)
                break;

            subtitleText.maxVisibleCharacters =
                visibleCount + 1;
        }

        // Ensure the complete line is visible.
        subtitleText.maxVisibleCharacters =
            totalCharacters;

        if (!waitForManualContinue)
            yield break;

        /*
         * Important debounce:
         *
         * If the player pressed the trigger to complete the text,
         * they must release it before another press can advance.
         *
         * This prevents one trigger press from both:
         * 1. Completing the line
         * 2. Advancing the line
         */

        while (GetContinueButtonPressed())
        {
            previousPressed = true;
            updatePressedState?.Invoke(true);
            yield return null;
        }

        previousPressed = false;
        updatePressedState?.Invoke(false);

        bool continuePressed = false;

        while (!continuePressed)
        {
            bool currentlyPressed =
                GetContinueButtonPressed();

            if (currentlyPressed && !previousPressed)
                continuePressed = true;

            previousPressed = currentlyPressed;
            updatePressedState?.Invoke(previousPressed);

            yield return null;
        }

        // Wait for release before the next line starts.
        while (GetContinueButtonPressed())
        {
            updatePressedState?.Invoke(true);
            yield return null;
        }

        updatePressedState?.Invoke(false);
    }

    private void BeginDialoguePresentation()
    {
        ShowCanvas();
        CaptureAndDisableLaserPointer();

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }
    }

    private void EndDialoguePresentation()
    {
        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }

        RestoreLaserPointer();
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
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        RestoreLaserPointer();

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }

        HideNarrator();
    }

    private bool GetContinueButtonPressed()
    {
#if UNITY_EDITOR
        if (allowEditorInput)
        {
            if (Input.GetKey(KeyCode.Space) ||
                Input.GetMouseButton(0))
            {
                return true;
            }
        }
#endif

        InputDevice device;
        bool pressed;

        switch (continueInput)
        {
            case ContinueInputType.RightTrigger:
                device = InputDevices.GetDeviceAtXRNode(
                    XRNode.RightHand
                );

                return device.TryGetFeatureValue(
                    CommonUsages.triggerButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.RightAButton:
                device = InputDevices.GetDeviceAtXRNode(
                    XRNode.RightHand
                );

                return device.TryGetFeatureValue(
                    CommonUsages.primaryButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.LeftTrigger:
                device = InputDevices.GetDeviceAtXRNode(
                    XRNode.LeftHand
                );

                return device.TryGetFeatureValue(
                    CommonUsages.triggerButton,
                    out pressed
                ) && pressed;

            case ContinueInputType.LeftXButton:
                device = InputDevices.GetDeviceAtXRNode(
                    XRNode.LeftHand
                );

                return device.TryGetFeatureValue(
                    CommonUsages.primaryButton,
                    out pressed
                ) && pressed;
        }

        return false;
    }

    private void CaptureAndDisableLaserPointer()
    {
        cachedLaserPointer =
            Object.FindAnyObjectByType<
                NPCDialogueVRLaserPointer
            >(
                FindObjectsInactive.Include
            );

        if (cachedLaserPointer == null)
            return;

        previousLaserEnabledState =
            cachedLaserPointer.enabled;

        laserStateCaptured = true;
        cachedLaserPointer.enabled = false;
    }

    private void RestoreLaserPointer()
    {
        if (!laserStateCaptured)
            return;

        if (cachedLaserPointer != null)
        {
            cachedLaserPointer.enabled =
                previousLaserEnabledState;
        }

        cachedLaserPointer = null;
        laserStateCaptured = false;
    }
}