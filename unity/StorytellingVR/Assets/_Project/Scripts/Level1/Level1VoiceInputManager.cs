using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Level1VoiceInputManager : MonoBehaviour
{
    [Header("STT Service Configuration")]
    [Tooltip("FastAPI endpoint URL for Whisper transcription.")]
    public string serverUrl = "http://127.0.0.1:8000/stt";

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

    // Trigger tracking state
    private bool wasKeyboardTriggered = false;

    private string GetIdleText()
    {
        return IsXRDeviceActive() ? "Hold A to Speak" : "Hold V to Speak";
    }

    private string GetReviewText()
    {
        return IsXRDeviceActive() ? "A Confirm | B Retry" : "ENTER Confirm | R Retry";
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

    public void ClearTranscript()
    {
        if (inputField != null)
        {
            inputField.text = "";
        }
        if (voiceStatusText != null)
        {
            voiceStatusText.text = GetIdleText();
        }
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

        // Set initial status text if reference is available
        if (voiceStatusText != null)
        {
            voiceStatusText.text = GetIdleText();
        }
    }

    private void Update()
    {
        // Desktop testing hotkey: hold V to record, release to send
        if (Input.GetKeyDown(KeyCode.V))
        {
            wasKeyboardTriggered = true;
            StartListening();
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            StopListening();
        }

        // R key behavior: Clear transcript
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearTranscript();
        }

        // VR Controller button checks safely
        #if UNITY_ANDROID || UNITY_STANDALONE_WIN
        try
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Debug.Log("[VR INPUT] A confirm");
                if (chatManager != null)
                {
                    chatManager.OnSend();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                Debug.Log("[VR INPUT] B retry");
                ClearTranscript();
            }
        }
        catch (System.Exception)
        {
            // OVRInput call failed or library missing/not initialized
        }
        #endif

        // Auto-reset status back to Idle when the input text box is cleared
        if (voiceStatusText != null && voiceStatusText.text == GetReviewText())
        {
            if (inputField != null && string.IsNullOrEmpty(inputField.text))
            {
                voiceStatusText.text = GetIdleText();
            }
        }
    }

    public void StartListening()
    {
        if (isListening) return;

        // Reset keyboard trigger flag if key V is not actually held down
        if (!Input.GetKey(KeyCode.V))
        {
            wasKeyboardTriggered = false;
        }

        Debug.Log("[STT] Recording started");
        isListening = true;
        startListeningTime = Time.time;

        // Clear input box
        if (inputField != null)
        {
            inputField.text = "";
        }

        // Set status
        if (voiceStatusText != null)
        {
            voiceStatusText.text = "Listening...";
        }

        // Start Unity Microphone capture
        recordingClip = Microphone.Start(deviceName, false, maxRecordingDuration, sampleRate);
    }

    public void StopListening()
    {
        if (!isListening) return;

        isListening = false;
        float duration = Time.time - startListeningTime;
        Debug.Log("[STT] Recording stopped");

        // Stop Unity Microphone capture
        Microphone.End(deviceName);

        // Enforce minimum duration constraint of 0.5s
        if (duration < 0.5f)
        {
            Debug.LogWarning("[STT] Recording too short (< 0.5s), ignored.");
            if (voiceStatusText != null)
            {
                voiceStatusText.text = GetIdleText();
            }
            return;
        }

        if (recordingClip == null)
        {
            Debug.LogError("[STT] Microphone recording failed (clip is null)!");
            if (voiceStatusText != null)
            {
                voiceStatusText.text = GetIdleText();
            }
            return;
        }

        if (voiceStatusText != null)
        {
            voiceStatusText.text = "Understanding speech...";
        }

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
                        inputField.ActivateInputField();
                    }

                    if (voiceStatusText != null)
                    {
                        voiceStatusText.text = GetReviewText();
                    }
                }
                else
                {
                    if (voiceStatusText != null)
                    {
                        voiceStatusText.text = "Could not hear clearly. Please repeat.";
                    }
                }
            }
            else
            {
                Debug.LogError($"[STT] Audio upload failed: {request.error}");
                if (voiceStatusText != null)
                {
                    voiceStatusText.text = GetIdleText();
                }
            }
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
