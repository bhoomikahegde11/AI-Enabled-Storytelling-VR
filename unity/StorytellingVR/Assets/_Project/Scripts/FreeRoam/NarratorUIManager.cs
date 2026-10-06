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

        string[] lines = fullText.Split(
            new[] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        // Prevent a trigger that was already being held from
        // immediately skipping the first line.
        bool previousPressed = GetContinueButtonPressed();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Get the audio for THIS subtitle line.
            AudioClip clip = null;

            if (!string.IsNullOrEmpty(lineId) &&
                voiceDatabase != null &&
                dialogueVoiceSource != null)
            {
                string audioLineId = $"{lineId}_{i + 1:00}";

                clip = voiceDatabase.GetAudioClip(audioLineId);

                Debug.Log(
                    $"[VOICE] Subtitle line {i + 1}: '{line}' | " +
                    $"Audio ID: {audioLineId} | " +
                    $"Found: {clip != null}"
                );
            }

            // Show subtitle and play its corresponding audio.
            yield return StartCoroutine(
                TypeAndWaitForLine(
                    line,
                    clip,
                    previousPressed,
                    pressedState =>
                    {
                        previousPressed = pressedState;
                    }
                )
            );

            // Small separation between lines.
            yield return null;
        }

        EndDialoguePresentation();
    }

    private IEnumerator TypeAndWaitForLine(
    string line,
    AudioClip clip,
    bool startingPressedState,
    System.Action<bool> updatePressedState)
    {
        if (subtitleText == null)
            yield break;

        // Show the subtitle.
        subtitleText.text = line;
        subtitleText.maxVisibleCharacters = 0;

        // Force TMP to calculate the text.
        subtitleText.ForceMeshUpdate();

        TMP_TextInfo textInfo = subtitleText.textInfo;
        int totalCharacters = textInfo.characterCount;

        // Start the audio for THIS subtitle line.
        StopCurrentVoice();

        if (clip != null && dialogueVoiceSource != null)
        {
            dialogueVoiceSource.clip = clip;
            dialogueVoiceSource.Play();

            Debug.Log($"[VOICE] Playing clip: {clip.name}");
        }
        else
        {
            Debug.LogWarning(
                $"[VOICE] No audio clip found for subtitle: '{line}'"
            );
        }

        bool previousPressed = startingPressedState;

        // --------------------------------------------------
        // TYPEWRITER EFFECT
        // --------------------------------------------------

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
                    currentlyPressed && !previousPressed;

                previousPressed = currentlyPressed;
                updatePressedState?.Invoke(previousPressed);

                // Trigger skips the current line AND audio.
                if (freshPress)
                {
                    subtitleText.maxVisibleCharacters =
                        totalCharacters;

                    StopCurrentVoice();

                    yield break;
                }

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            subtitleText.maxVisibleCharacters =
                visibleCount + 1;
        }

        // Make sure the entire subtitle is visible.
        subtitleText.maxVisibleCharacters =
            totalCharacters;

        // --------------------------------------------------
        // WAIT FOR AUDIO TO FINISH
        // --------------------------------------------------

        while (dialogueVoiceSource != null &&
               dialogueVoiceSource.isPlaying)
        {
            bool currentlyPressed =
                GetContinueButtonPressed();

            bool freshPress =
                currentlyPressed && !previousPressed;

            previousPressed = currentlyPressed;
            updatePressedState?.Invoke(previousPressed);

            // Trigger skips the rest of the audio.
            if (freshPress)
            {
                StopCurrentVoice();
                yield break;
            }

            yield return null;
        }

        // Audio has finished naturally.
        StopCurrentVoice();
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