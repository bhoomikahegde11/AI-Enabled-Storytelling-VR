using UnityEngine;
using TMPro;
using System.Collections;

public class Level1VoiceInputManager : MonoBehaviour
{
    // Editor:
    // http://localhost:8000/stt
    //
    // Quest:
    // http://LAPTOP_WIFI_IP:8000/stt
    //
    // Example:
    // http://192.168.1.50:8000/stt
    [Header("STT Service Configuration")]
    [Tooltip("FastAPI endpoint URL for Whisper transcription.")]
    public string serverUrl = "http://172.20.10.5:8000/stt";
    public bool useLocalSpeech = false;

    [System.Serializable]
    private class BackendConfig
    {
        public string baseUrl;
    }

    private void Awake()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "backend_config.json");
        if (System.IO.File.Exists(path))
        {
            try
            {
                string jsonText = System.IO.File.ReadAllText(path);
                BackendConfig config = JsonUtility.FromJson<BackendConfig>(jsonText);
                if (config != null && !string.IsNullOrEmpty(config.baseUrl))
                {
                    serverUrl = config.baseUrl.TrimEnd('/') + "/stt";
                    Debug.Log("[BACKEND CONFIG] Using URL: " + serverUrl);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[BACKEND CONFIG] Error reading config file: " + ex.Message);
            }
        }
    }

    [Tooltip("Microphone device name. Set empty/null to use default device.")]
    public string deviceName = null;

    [Tooltip("Maximum allowed recording duration in seconds.")]
    public int maxRecordingDuration = 20;

    [Tooltip("Sample rate to record audio at (optimal for Whisper: 16000).")]
    public int sampleRate = 16000;

    [Header("System References")]
    [Tooltip("Reference to the ChatManager script.")]
    public ChatManager chatManager;

    [Tooltip("Reference to the input text box.")]
    public TMP_InputField inputField;

    [Header("Optional UI References")]
    [Tooltip("Optional text element to display voice recording/processing status.")]
    public TMP_Text voiceStatusText;

    // Recording tracking state
    private AudioClip recordingClip;
    private float startListeningTime;
    private bool isListening = false;
    private ISpeechToTextProvider speechProvider;

    public enum VoiceInputState
    {
        Idle,
        Recording,
        Review
    }
    private VoiceInputState currentState = VoiceInputState.Idle;

    private Level1HUDManager hudManager
    {
        get { return (chatManager != null) ? chatManager.hudManager : null; }
    }

    private string GetIdleText()
    {
        return IsXRDeviceActive() ? "Hold A to bargain" : "Hold V to bargain";
    }

    private string GetReviewText()
    {
        return IsXRDeviceActive() ? "A Confirm | B Retry" : "Press Enter to send  |  R to reset";
    }

    private bool IsXRDeviceActive()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return true;
        #else
        try
        {
            return OVRManager.isHmdPresent || (UnityEngine.XR.XRSettings.enabled && !string.IsNullOrEmpty(UnityEngine.XR.XRSettings.loadedDeviceName));
        }
        catch
        {
            return false;
        }
        #endif
    }

    private void SetVoiceStatusText(string status)
    {
        if (voiceStatusText != null)
        {
            voiceStatusText.text = status;
        }
        if (hudManager != null)
        {
            hudManager.SetVoiceStatus(status);
        }
    }

    private string GetCurrentVoiceStatusText()
    {
        if (voiceStatusText != null) return voiceStatusText.text;
        if (hudManager != null && hudManager.voiceStatusText != null) return hudManager.voiceStatusText.text;
        return "";
    }

    public void ClearTranscript()
    {
        if (inputField != null)
        {
            inputField.text = "";
        }
        currentState = VoiceInputState.Idle;
        SetVoiceStatusText(GetIdleText());
        Debug.Log("[VOICE CONFIRM] Transcript cleared");
    }

    private void Start()
    {
        // 1. Auto-discover references if not assigned in Inspector
        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }

        if (inputField == null && chatManager != null)
        {
            inputField = chatManager.inputField;
        }

        if (voiceStatusText == null && hudManager != null)
        {
            voiceStatusText = hudManager.voiceStatusText;
        }

        // Set initial status text
        SetVoiceStatusText(GetIdleText());
        currentState = VoiceInputState.Idle;
        speechProvider = useLocalSpeech ? new LocalSpeechProvider() : new BackendSpeechProvider(serverUrl);

        Debug.Log("[BACKEND] Using URL: " + serverUrl);
    }

    private void Update()
    {
        // Auto-discover voiceStatusText if it is null (in case hudManager initialized later)
        if (voiceStatusText == null && hudManager != null)
        {
            voiceStatusText = hudManager.voiceStatusText;
        }

        // Hold V (keyboard) or Right Trigger (controller) to record
        if (Input.GetKeyDown(KeyCode.V)
            || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            StartListening();
        }
        if (Input.GetKeyUp(KeyCode.V)
            || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            StopListening();
        }

        // Enter / A confirm  |  R / B reset  (keyboard + controller)
        if (currentState == VoiceInputState.Review)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
                || OVRInput.GetDown(OVRInput.Button.One))
            {
                Debug.Log("[VOICE CONFIRM] Confirm triggered (Enter / A)");
                if (chatManager != null)
                {
                    chatManager.OnSend();
                }
                currentState = VoiceInputState.Idle;
                SetVoiceStatusText(GetIdleText());
            }

            if (Input.GetKeyDown(KeyCode.R)
                || OVRInput.GetDown(OVRInput.Button.Two))
            {
                Debug.Log("[VOICE CONFIRM] Reset triggered (R / B)");
                ClearTranscript();
            }
        }
        else
        {
            // Standard R / B when not reviewing: Clear transcript
            if (Input.GetKeyDown(KeyCode.R)
                || OVRInput.GetDown(OVRInput.Button.Two))
            {
                ClearTranscript();
            }
        }

        // (A / B controller inputs handled above alongside Enter / R)

        // Auto-reset status back to Idle when the input text box is cleared
        if (GetCurrentVoiceStatusText() == GetReviewText())
        {
            if (inputField != null && string.IsNullOrEmpty(inputField.text))
            {
                currentState = VoiceInputState.Idle;
                SetVoiceStatusText(GetIdleText());
            }
        }
    }

    public void StartListening()
    {
        if (isListening) return;

        Debug.Log("[STT] Recording started");
        isListening = true;
        currentState = VoiceInputState.Recording;
        startListeningTime = Time.time;

        // Trigger UI expansion and glow animation
        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.StartListeningAnimation();
        }

        // Disable typing and clear focus to prevent push-to-talk key from typing into field
        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.DisablePlayerTyping();
        }

        // Clear input box
        if (inputField != null)
        {
            inputField.text = "";
        }

        // Set status
        SetVoiceStatusText("Listening...");

        // Start Unity Microphone capture
        recordingClip = Microphone.Start(deviceName, false, maxRecordingDuration, sampleRate);
    }

    public void StopListening()
    {
        if (!isListening) return;

        isListening = false;
        float duration = Time.time - startListeningTime;
        Debug.Log("[STT] Recording stopped");

        // Trigger UI scale-down and return to idle animation
        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.StopListeningAnimation();
        }

        // Stop Unity Microphone capture
        Microphone.End(deviceName);

        // Enforce minimum duration constraint of 0.5s
        if (duration < 0.5f)
        {
            Debug.LogWarning("[STT] Recording too short (< 0.5s), ignored.");
            if (chatManager != null && chatManager.hudManager != null)
            {
                chatManager.hudManager.EnablePlayerTyping();
            }
            currentState = VoiceInputState.Idle;
            SetVoiceStatusText(GetIdleText());
            return;
        }

        if (recordingClip == null)
        {
            Debug.LogError("[STT] Microphone recording failed (clip is null)!");
            if (chatManager != null && chatManager.hudManager != null)
            {
                chatManager.hudManager.EnablePlayerTyping();
            }
            currentState = VoiceInputState.Idle;
            SetVoiceStatusText(GetIdleText());
            return;
        }

        SetVoiceStatusText("Understanding speech...");

        AudioClip trimmedClip = CreateTrimmedClip(recordingClip, duration);
        if (trimmedClip != null)
        {
            StartCoroutine(TranscribeAudioRoutine(trimmedClip));
        }
    }

    private AudioClip CreateTrimmedClip(AudioClip clip, float recordedDuration)
    {
        int channels = clip.channels;
        int frequency = clip.frequency;
        int totalSamples = Mathf.RoundToInt(recordedDuration * frequency * channels);
        totalSamples = Mathf.Min(totalSamples, clip.samples * channels);
        if (totalSamples <= 0) return null;

        float[] samples = new float[totalSamples];
        clip.GetData(samples, 0);
        int sampleFrames = totalSamples / channels;
        AudioClip trimmedClip = AudioClip.Create("RecordedSpeech", sampleFrames, channels, frequency, false);
        trimmedClip.SetData(samples, 0);
        return trimmedClip;
    }

    private IEnumerator TranscribeAudioRoutine(AudioClip clip)
    {
        if (speechProvider == null)
        {
            speechProvider = useLocalSpeech ? new LocalSpeechProvider() : new BackendSpeechProvider(serverUrl);
        }

        var task = speechProvider.Transcribe(clip);
        while (!task.IsCompleted)
        {
            yield return null;
        }

        string transcript = task.IsFaulted || task.Result == null ? "" : task.Result.Trim();

        if (IsValidTranscript(transcript))
        {
            Debug.Log("[VOICE CONFIRM] Awaiting player approval");

            if (inputField != null)
            {
                inputField.text = transcript;
            }

            currentState = VoiceInputState.Review;
            SetVoiceStatusText(GetReviewText());
        }
        else
        {
            currentState = VoiceInputState.Idle;
            SetVoiceStatusText(useLocalSpeech ? GetIdleText() : "Could not hear clearly. Please repeat.");
        }

        // Re-enable player typing after STT processes
        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.EnablePlayerTyping();
        }
    }

    private bool IsValidTranscript(string transcript)
    {
        if (string.IsNullOrEmpty(transcript)) return false;

        string trimmed = transcript.Trim();
        if (trimmed.Length == 0) return false;

        // Reject random single characters (except single digits or single letter words like 'a', 'A', 'i', 'I')
        if (trimmed.Length == 1)
        {
            char c = trimmed[0];
            if (!char.IsDigit(c) && c != 'a' && c != 'A' && c != 'i' && c != 'I')
            {
                return false;
            }
        }

        // Filter common Whisper silence/hallucination artifacts (exact match or trailing punctuation)
        string lower = trimmed.ToLower();
        string[] whisperArtifacts = new string[] 
        {
            "thank you", "thanks", "you", "bye", "subtitles by", "subtitles", "watching", "transcript", "subscribe"
        };
        
        foreach (string artifact in whisperArtifacts)
        {
            if (lower == artifact || lower == artifact + "." || lower == artifact + "!" || lower == artifact + "?")
            {
                return false;
            }
        }

        return true;
    }
}
