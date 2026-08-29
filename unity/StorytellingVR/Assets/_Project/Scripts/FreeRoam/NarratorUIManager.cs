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

    [Header("Voice")]
    [SerializeField] DialogueVoiceDatabase voiceDatabase;
    [SerializeField] AudioSource dialogueVoiceSource;

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

    [Header("Automatic Advance")]

    [Tooltip("Automatically advances after the full line is visible.")]
    public bool autoAdvance = true;

    [Tooltip("How long a completed line stays visible before advancing automatically.")]
    [Min(0f)]
    public float autoAdvanceDelay = 3.5f;


    [Header("Editor Testing")]
    [Tooltip("In the Editor, Space or left mouse click acts as the continue input.")]
    public bool allowEditorInput = true;

    private static bool continueTutorialTaught = false;
    private bool isShowingContinueTutorial = false;

    private Coroutine currentRoutine;

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
            NarrationRoutine(speaker, subtitle, null)
        );
    }

    public void ShowNarration(
        string lineId,
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle, lineId)
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
            NarrationRoutine(speaker, subtitle, null)
        );

        yield return currentRoutine;

        currentRoutine = null;
    }

    public IEnumerator PlayNarration(
        string lineId,
        string speaker,
        string subtitle,
        float duration = -1f)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, subtitle, lineId)
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
            NarrationRoutine(speaker, fullText, null)
        );

        yield return currentRoutine;

        currentRoutine = null;
    }

    public IEnumerator PlayNarrationLineByLine(
        string lineId,
        string speaker,
        string fullText)
    {
        StopCurrentRoutine();

        currentRoutine = StartCoroutine(
            NarrationRoutine(speaker, fullText, lineId)
        );

        yield return currentRoutine;

        currentRoutine = null;
    }

    private IEnumerator NarrationRoutine(
        string speaker,
        string fullText,
        string lineId)
    {
        if (string.IsNullOrWhiteSpace(fullText))
            yield break;

        BeginDialoguePresentation();

        if (speakerText != null)
            speakerText.text = speaker;

        Debug.Log($"[VOICE] Request lineId={(string.IsNullOrEmpty(lineId) ? "NULL" : lineId)}");
        Debug.Log($"[VOICE] Database assigned={(voiceDatabase != null)}");
        Debug.Log($"[VOICE] AudioSource assigned={(dialogueVoiceSource != null)}");

        StopCurrentVoice();
        if (!string.IsNullOrEmpty(lineId) && voiceDatabase != null && dialogueVoiceSource != null)
        {
            AudioClip clip = voiceDatabase.GetAudioClip(lineId);
            Debug.Log($"[VOICE] Lookup success={(clip != null)}");
            if (clip != null)
            {
                Debug.Log($"[VOICE] Clip={clip.name}");
                dialogueVoiceSource.clip = clip;
                Debug.Log($"[VOICE] AudioSource active={dialogueVoiceSource.gameObject.activeInHierarchy} enabled={dialogueVoiceSource.enabled} volume={dialogueVoiceSource.volume}");
                Debug.Log("[VOICE] Calling Play");
                dialogueVoiceSource.Play();
                Debug.Log($"[VOICE] isPlaying after Play={dialogueVoiceSource.isPlaying}");
            }
            else
            {
                Debug.LogWarning($"[NarratorUIManager] Missing audio clip for lineId: {lineId}");
            }
        }

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

        if (!waitForManualContinue && !autoAdvance)
            yield break;

        // If the trigger was used to reveal the whole line,
        // require it to be released before it can advance.
        while (GetContinueButtonPressed())
        {
            previousPressed = true;
            updatePressedState?.Invoke(true);
            yield return null;
        }

        previousPressed = false;
        updatePressedState?.Invoke(false);

        if (waitForManualContinue && !continueTutorialTaught)
        {
            ShowContinueTutorialPrompt();
        }

        float completedLineElapsed = 0f;
        bool advanceLine = false;

        while (!advanceLine)
        {
            bool currentlyPressed =
                GetContinueButtonPressed();

            bool freshPress =
                currentlyPressed &&
                !previousPressed;

            if (waitForManualContinue && freshPress)
            {
                StopCurrentVoice();
                advanceLine = true;

                if (isShowingContinueTutorial)
                {
                    continueTutorialTaught = true;
                    HideContinueTutorialPrompt();
                }
            }

            previousPressed = currentlyPressed;
            updatePressedState?.Invoke(previousPressed);

            if (autoAdvance)
            {
                completedLineElapsed +=
                    Time.unscaledDeltaTime;

                if (completedLineElapsed >= autoAdvanceDelay)
                {
                    advanceLine = true;
                }
            }

            yield return null;
        }

        HideContinueTutorialPrompt();

        // Prevent a held trigger from skipping the next line.
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

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }
    }

    private void EndDialoguePresentation()
    {
        HideContinueTutorialPrompt();

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }

        HideNarrator();
    }

    private void ShowCanvas()
    {
        if (narratorCanvas != null)
            narratorCanvas.SetActive(true);
    }

    public void HideNarrator()
    {
        StopCurrentVoice();
        if (narratorCanvas != null)
            narratorCanvas.SetActive(false);
    }

    public void StopCurrentVoice()
    {
        Debug.Log($"[VOICE] StopCurrentVoice called. Source assigned={(dialogueVoiceSource != null)}, isPlaying={(dialogueVoiceSource != null && dialogueVoiceSource.isPlaying)}");
        if (dialogueVoiceSource != null && dialogueVoiceSource.isPlaying)
        {
            dialogueVoiceSource.Stop();
        }
    }

    private void StopCurrentRoutine()
    {
        HideContinueTutorialPrompt();
        StopCurrentVoice();

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
            subtitleText.maxVisibleCharacters =
                int.MaxValue;
        }

        HideNarrator();
    }

    private void ShowContinueTutorialPrompt()
    {
        if (continueTutorialTaught || isShowingContinueTutorial)
            return;

        isShowingContinueTutorial = true;

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowPrompt(
                "Continue Dialogue",
                "Press the RIGHT TRIGGER to continue.",
                this
            );
        }
    }

    private void HideContinueTutorialPrompt()
    {
        if (!isShowingContinueTutorial)
            return;

        isShowingContinueTutorial = false;

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.HidePrompt(this);
        }
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
}