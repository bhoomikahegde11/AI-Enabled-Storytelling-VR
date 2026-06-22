using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

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
    [Tooltip("Optional provider component implementing ISpeechToTextProvider, such as VoskSpeechProvider. Overrides backend/local selection when assigned.")]
    public MonoBehaviour speechProviderOverride;

    [System.Serializable]
    private class BackendConfig
    {
        public string baseUrl;
    }

    private void Awake()
    {
        LoadConfig();
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!debugTestNormalize)
        {
            return;
        }

        debugTestNormalize = false;
        string normalized = InputNormalizer.Normalize(debugRawInput, false);
        Debug.Log("RAW:\n" + (debugRawInput ?? string.Empty) + "\n\nNORMALIZED:\n" + normalized);
    }
    #endif

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
    public bool debugKeepLastRecording = true;

    #if UNITY_EDITOR
    [Header("Editor Debug")]
    [SerializeField] private string debugRawInput;
    [SerializeField] private bool debugTestNormalize;
    #endif

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
    private bool isRequestingMicrophonePermission = false;
    private ISpeechToTextProvider speechProvider;
    private const float ExtremelyLowPeakThreshold = 0.005f;

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
        return IsXRDeviceActive()
            ? "Hold Trigger to speak\nRelease to transcribe"
            : "Hold V to speak\nRelease to transcribe";
    }

    private string GetReviewText()
    {
        return IsXRDeviceActive()
            ? "Press A to send\nPress B to clear"
            : "Press Enter to send\nPress R to clear";
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
        speechProvider = ResolveSpeechProvider();

        if (speechProviderOverride == null)
        {
            Debug.LogWarning("[STT] Speech provider not assigned. Assign VoskSpeechProvider to Level1VoiceInputManager.");
        }

        Debug.Log("[BACKEND] Using URL: " + serverUrl);
    }

    private void Update()
    {
        // Auto-discover voiceStatusText if it is null (in case hudManager initialized later)
        if (voiceStatusText == null && hudManager != null)
        {
            voiceStatusText = hudManager.voiceStatusText;
        }

        bool editorRecordDown = Input.GetKeyDown(KeyCode.V);
        bool editorRecordUp = Input.GetKeyUp(KeyCode.V);
        bool leftTriggerDown = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        bool leftTriggerUp = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);

        // Hold V (keyboard) or Left Trigger (controller) to record
        if (editorRecordDown || leftTriggerDown)
        {
            if (leftTriggerDown)
            {
                Debug.Log("[INPUT] Left trigger down");
            }
            Debug.Log("[STT-QUEST] Input held/down: true");
            StartListening();
        }
        if (editorRecordUp || leftTriggerUp)
        {
            if (leftTriggerUp)
            {
                Debug.Log("[INPUT] Left trigger up");
            }
            Debug.Log("[STT-QUEST] Input held/down: false");
            StopListening();
        }

        // Enter / A confirm  |  R / B reset  (keyboard + controller)
        if (currentState == VoiceInputState.Review)
        {
            bool confirmPressed = Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.KeypadEnter)
                || OVRInput.GetDown(OVRInput.Button.One);
            if (confirmPressed)
            {
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    Debug.Log("[INPUT] A pressed");
                }
                Debug.Log("[VOICE CONFIRM] Confirm triggered (Enter / A)");
                if (chatManager != null)
                {
                    Debug.Log("[STT] Sent to ChatManager: " + (inputField != null ? inputField.text : string.Empty));
                    chatManager.OnSend();
                }
                currentState = VoiceInputState.Idle;
                SetVoiceStatusText(GetIdleText());
            }

            bool clearPressed = Input.GetKeyDown(KeyCode.R)
                || OVRInput.GetDown(OVRInput.Button.Two);
            if (clearPressed)
            {
                if (OVRInput.GetDown(OVRInput.Button.Two))
                {
                    Debug.Log("[INPUT] B pressed");
                }
                Debug.Log("[VOICE CONFIRM] Reset triggered (R / B)");
                ClearTranscript();
            }
        }
        else
        {
            // Standard R / B when not reviewing: Clear transcript
            bool clearPressed = Input.GetKeyDown(KeyCode.R)
                || OVRInput.GetDown(OVRInput.Button.Two);
            if (clearPressed)
            {
                if (OVRInput.GetDown(OVRInput.Button.Two))
                {
                    Debug.Log("[INPUT] B pressed");
                }
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
        if (isListening || isRequestingMicrophonePermission) return;

        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.LogWarning("[STT-QUEST] Failure reason: Microphone permission not granted. Requesting permission.");
            StartCoroutine(RequestMicrophonePermissionAndStartListening());
            return;
        }

        Debug.Log("[STT] Recording started");
        Debug.Log("[STT-QUEST] Recording started: true");
        isListening = true;
        currentState = VoiceInputState.Recording;
        startListeningTime = Time.time;
        LogMicrophoneDiagnostics();

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

        // Starting a new recording always replaces any unsent transcript.
        if (inputField != null)
        {
            inputField.text = "";
        }

        // Set status
        SetVoiceStatusText("Listening...");

        // Start Unity Microphone capture
        recordingClip = Microphone.Start(deviceName, false, maxRecordingDuration, sampleRate);
        Debug.Log("[STT-QUEST] Microphone device: " + ResolveMicrophoneDeviceName());
        Debug.Log("[STT-QUEST] Sample rate: " + sampleRate);
    }

    public void StopListening()
    {
        if (!isListening) return;

        isListening = false;
        float duration = Time.time - startListeningTime;
        Debug.Log("[STT] Recording stopped");
        Debug.Log("[STT-QUEST] Recording stopped: true");
        Debug.Log("[STT-QUEST] Recording duration seconds: " + duration.ToString("0.000"));

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
            Debug.LogWarning("[STT-QUEST] Recording too short");
            Debug.LogWarning("[STT-QUEST] Failure reason: Recording duration below 0.5 seconds.");
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
            Debug.LogError("[STT-QUEST] Failure reason: Microphone recording clip is null.");
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
            LogAudioDiagnostics(trimmedClip);
            if (debugKeepLastRecording)
            {
                SaveRecordingDebug(trimmedClip);
            }
            StartCoroutine(TranscribeAudioRoutine(trimmedClip));
        }
        else
        {
            Debug.LogError("[STT-QUEST] Failure reason: Trimmed recording clip is null.");
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
        speechProvider = ResolveSpeechProvider();
        if (speechProvider == null)
        {
            Debug.LogError("[STT-QUEST] Failure reason: No speech provider available.");
            currentState = VoiceInputState.Idle;
            SetVoiceStatusText(GetIdleText());
            if (chatManager != null && chatManager.hudManager != null)
            {
                chatManager.hudManager.EnablePlayerTyping();
            }
            yield break;
        }

        Debug.Log("[STT-QUEST] Provider: " + (speechProvider != null ? speechProvider.GetType().Name : "(null)"));
        bool usingWhisperProvider = speechProvider is LocalSpeechProvider;
        if (usingWhisperProvider)
        {
            Debug.Log("[STT-QUEST] Whisper model path: " + LocalSpeechProvider.WhisperModelPath);
            Debug.Log("[STT-QUEST] Whisper model exists: " + LocalSpeechProvider.WhisperModelExists);
            if (!LocalSpeechProvider.WhisperModelExists)
            {
                Debug.LogError("[STT-QUEST] Failure reason: Whisper model missing at " + LocalSpeechProvider.WhisperModelPath);
            }
        }

        Debug.Log("[STT] Transcription started");
        var task = speechProvider.Transcribe(clip);
        while (!task.IsCompleted)
        {
            yield return null;
        }

        string transcript = task.IsFaulted || task.Result == null ? "" : task.Result.Trim();
        string rawTranscript = usingWhisperProvider ? LocalSpeechProvider.LastRawTranscription : transcript;
        string normalizedTranscript = !string.IsNullOrWhiteSpace(transcript) ? InputNormalizer.Normalize(transcript, false) : string.Empty;

        Debug.Log("[STT-QUEST] Transcription raw: " + rawTranscript);
        Debug.Log("[STT-QUEST] Transcription normalized: " + normalizedTranscript);
        Debug.Log("[STT] Transcription result: " + transcript);

        if (task.IsFaulted)
        {
            Debug.LogError("[STT-QUEST] Failure reason: " + task.Exception?.GetBaseException().Message);
        }
        else if (string.IsNullOrWhiteSpace(transcript))
        {
            string reason = usingWhisperProvider ? LocalSpeechProvider.LastFailureReason : "Transcription returned empty text.";
            Debug.LogWarning("[STT-QUEST] Failure reason: " + reason);
        }

        if (IsValidTranscript(transcript))
        {
            Debug.Log("[VOICE CONFIRM] Awaiting player approval");
            Debug.Log("[STT] Transcript ready for confirm: " + transcript);

            if (inputField != null)
            {
                inputField.text = transcript;
            }

            currentState = VoiceInputState.Review;
            SetVoiceStatusText(GetReviewText());
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(transcript))
            {
                Debug.LogWarning("[STT-QUEST] Failure reason: Transcript rejected by validation.");
            }
            currentState = VoiceInputState.Idle;
            SetVoiceStatusText((speechProviderOverride != null || usingWhisperProvider) ? GetIdleText() : "Could not hear clearly. Please repeat.");
        }

        // Re-enable player typing after STT processes
        if (chatManager != null && chatManager.hudManager != null)
        {
            chatManager.hudManager.EnablePlayerTyping();
        }
    }

    private IEnumerator RequestMicrophonePermissionAndStartListening()
    {
        isRequestingMicrophonePermission = true;
        SetVoiceStatusText("Microphone permission required");
        Debug.Log("[STT-QUEST] Microphone permission: false");

        AsyncOperation permissionRequest = Application.RequestUserAuthorization(UserAuthorization.Microphone);
        while (!permissionRequest.isDone)
        {
            yield return null;
        }

        isRequestingMicrophonePermission = false;

        bool hasPermission = Application.HasUserAuthorization(UserAuthorization.Microphone);
        Debug.Log("[STT-QUEST] Microphone permission: " + hasPermission);

        if (!hasPermission)
        {
            Debug.LogWarning("[STT-QUEST] Failure reason: Microphone permission denied.");
            SetVoiceStatusText("Microphone permission denied");
            yield break;
        }

        SetVoiceStatusText(GetIdleText());
        StartListening();
    }

    private void LogMicrophoneDiagnostics()
    {
        bool hasPermission = Application.HasUserAuthorization(UserAuthorization.Microphone);
        Debug.Log("[STT-QUEST] Microphone permission: " + hasPermission);
        Debug.Log("[STT-QUEST] Microphone devices count: " + Microphone.devices.Length);
        Debug.Log("[STT-QUEST] Microphone device: " + ResolveMicrophoneDeviceName());
    }

    private string ResolveMicrophoneDeviceName()
    {
        if (!string.IsNullOrWhiteSpace(deviceName))
        {
            return deviceName;
        }

        return Microphone.devices.Length > 0 ? Microphone.devices[0] : "(default/no device)";
    }

    private void LogAudioDiagnostics(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("[STT-QUEST] Failure reason: Cannot inspect null clip.");
            return;
        }

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float peak = 0f;
        float sumSquares = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            float abs = Mathf.Abs(samples[i]);
            if (abs > peak)
            {
                peak = abs;
            }
            sumSquares += samples[i] * samples[i];
        }

        float rms = samples.Length > 0 ? Mathf.Sqrt(sumSquares / samples.Length) : 0f;
        bool isSilence = peak < ExtremelyLowPeakThreshold;

        Debug.Log("[STT-QUEST] Clip samples: " + clip.samples);
        Debug.Log("[STT-QUEST] Peak amplitude: " + peak.ToString("0.000000"));
        Debug.Log("[STT-QUEST] RMS amplitude: " + rms.ToString("0.000000"));
        Debug.Log("[STT-QUEST] Is silence: " + isSilence);

        if (peak < ExtremelyLowPeakThreshold)
        {
            Debug.LogWarning("[STT-QUEST] Audio too quiet");
        }
    }

    private void SaveRecordingDebug(AudioClip clip)
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "last_stt_recording.wav");
            File.WriteAllBytes(path, EncodeWav(clip));
            Debug.Log("[STT-QUEST] Saved recording to: " + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[STT-QUEST] Failure reason: Failed to save recording. " + ex.Message);
        }
    }

    private byte[] EncodeWav(AudioClip clip)
    {
        int sampleCount = clip.samples * clip.channels;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        byte[] wavData = new byte[44 + sampleCount * 2];
        System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(wavData, 0);
        System.BitConverter.GetBytes(36 + sampleCount * 2).CopyTo(wavData, 4);
        System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(wavData, 8);
        System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(wavData, 12);
        System.BitConverter.GetBytes(16).CopyTo(wavData, 16);
        System.BitConverter.GetBytes((short)1).CopyTo(wavData, 20);
        System.BitConverter.GetBytes((short)clip.channels).CopyTo(wavData, 22);
        System.BitConverter.GetBytes(clip.frequency).CopyTo(wavData, 24);
        System.BitConverter.GetBytes(clip.frequency * clip.channels * 2).CopyTo(wavData, 28);
        System.BitConverter.GetBytes((short)(clip.channels * 2)).CopyTo(wavData, 32);
        System.BitConverter.GetBytes((short)16).CopyTo(wavData, 34);
        System.Text.Encoding.ASCII.GetBytes("data").CopyTo(wavData, 36);
        System.BitConverter.GetBytes(sampleCount * 2).CopyTo(wavData, 40);

        int offset = 44;
        for (int i = 0; i < sampleCount; i++)
        {
            short value = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
            System.BitConverter.GetBytes(value).CopyTo(wavData, offset);
            offset += 2;
        }

        return wavData;
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

    private ISpeechToTextProvider ResolveSpeechProvider()
    {
        if (speechProviderOverride != null)
        {
            ISpeechToTextProvider overrideProvider = speechProviderOverride as ISpeechToTextProvider;
            if (overrideProvider != null)
            {
                return overrideProvider;
            }

            Debug.LogWarning("[STT] Assigned speechProviderOverride does not implement ISpeechToTextProvider. Falling back to existing selection.");
        }
        else
        {
            Debug.LogWarning("[STT] Speech provider not assigned. Assign VoskSpeechProvider to Level1VoiceInputManager.");
        }

        return useLocalSpeech ? new LocalSpeechProvider() : new BackendSpeechProvider(serverUrl);
    }
}
