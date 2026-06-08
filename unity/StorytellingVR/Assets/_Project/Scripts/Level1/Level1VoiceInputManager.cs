using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

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
            chatManager = FindObjectOfType<ChatManager>();
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

        // Encode recorded sample data to WAV byte format
        byte[] wavBytes = EncodeWav(recordingClip, duration);
        if (wavBytes != null && wavBytes.Length > 0)
        {
            StartCoroutine(UploadAudioRoutine(wavBytes));
        }
    }

    private byte[] EncodeWav(AudioClip clip, float recordedDuration)
    {
        int channels = clip.channels;
        int frequency = clip.frequency;

        // Calculate sample counts to read based on recorded time
        int totalSamples = Mathf.RoundToInt(recordedDuration * frequency * channels);
        totalSamples = Mathf.Min(totalSamples, clip.samples * channels);

        if (totalSamples <= 0) return null;

        float[] samples = new float[totalSamples];
        clip.GetData(samples, 0);

        byte[] wavData = new byte[44 + totalSamples * 2];

        // 1. WAV Header (RIFF / WAVE descriptor chunk)
        System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(wavData, 0);
        System.BitConverter.GetBytes(36 + totalSamples * 2).CopyTo(wavData, 4);
        System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(wavData, 8);

        // 2. Format subchunk ("fmt ")
        System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(wavData, 12);
        System.BitConverter.GetBytes(16).CopyTo(wavData, 16); // Subchunk1Size
        System.BitConverter.GetBytes((short)1).CopyTo(wavData, 20); // AudioFormat (1 = PCM)
        System.BitConverter.GetBytes((short)channels).CopyTo(wavData, 22); // NumChannels
        System.BitConverter.GetBytes(frequency).CopyTo(wavData, 24); // SampleRate
        System.BitConverter.GetBytes(frequency * channels * 2).CopyTo(wavData, 28); // ByteRate
        System.BitConverter.GetBytes((short)(channels * 2)).CopyTo(wavData, 32); // BlockAlign
        System.BitConverter.GetBytes((short)16).CopyTo(wavData, 34); // BitsPerSample

        // 3. Data subchunk
        System.Text.Encoding.ASCII.GetBytes("data").CopyTo(wavData, 36);
        System.BitConverter.GetBytes(totalSamples * 2).CopyTo(wavData, 40); // Subchunk2Size

        // 4. Copy data samples and convert to 16-bit PCM shorts
        int offset = 44;
        for (int i = 0; i < totalSamples; i++)
        {
            short value = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
            System.BitConverter.GetBytes(value).CopyTo(wavData, offset);
            offset += 2;
        }

        return wavData;
    }

    private IEnumerator UploadAudioRoutine(byte[] wavBytes)
    {
        Debug.Log("[STT] Sending audio");
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Construct multipart form data
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("file", wavBytes, "voice.wav", "audio/wav"));

        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, formData))
        {
            yield return request.SendWebRequest();
            stopwatch.Stop();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                STTResponse response = JsonUtility.FromJson<STTResponse>(jsonResponse);
                
                string transcript = (response != null && !string.IsNullOrEmpty(response.text)) ? response.text.Trim() : "";

                Debug.Log($"[STT] Transcript: {transcript}");
                Debug.Log($"[PERF STT] {stopwatch.ElapsedMilliseconds} ms");

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
                    SetVoiceStatusText("Could not hear clearly. Please repeat.");
                }
            }
            else
            {
                Debug.LogError($"[BACKEND] Request failed.\nURL Attempted: {serverUrl}\nError: {request.error}");
                currentState = VoiceInputState.Idle;
                SetVoiceStatusText(GetIdleText());
            }
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

    [System.Serializable]
    private class STTResponse
    {
        public string text;
    }
}
