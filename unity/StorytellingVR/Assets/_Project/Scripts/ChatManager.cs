using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class APIResponse
{
    public string npc_text;
    public string audio_url;
}

public class ChatManager : MonoBehaviour
{
    public APIManager api;

    public TMP_InputField inputField;
    public TextMeshProUGUI npcText;

    public AudioManager audioManager;

    // 🔥 Prevent STT spam / multiple requests
    private bool isProcessing = false;
    private string lastProcessedText = "";
    private float lastProcessedTime = 0f;

    void Start()
    {
        StartCoroutine(api.StartSession(OnNPCReply));
    }

    // 📝 TEXT INPUT (unchanged behavior)
    public void OnSend()
    {
        if (isProcessing) return;

        string playerText = inputField.text;

        if (string.IsNullOrEmpty(playerText)) return;

        isProcessing = true;

        StartCoroutine(SendMessageRoutine(playerText));

        inputField.text = "";
    }

    // 🎤 VOICE INPUT (fixed + throttled)
    public void OnVoiceInput(string spokenText)
    {
        if (string.IsNullOrEmpty(spokenText)) return;

        // 🔥 Ignore repeated / similar inputs
        if (spokenText == lastProcessedText) return;

        // 🔥 Cooldown check (3 seconds)
        if (Time.time - lastProcessedTime < 3f) return;

        Debug.Log("Voice Input: " + spokenText);

        lastProcessedText = spokenText;
        lastProcessedTime = Time.time;

        if (npcText != null)
            npcText.text = "You: " + spokenText;

        StartCoroutine(SendMessageRoutine(spokenText));
    }

    // 🔁 COMMON SEND ROUTINE (prevents duplication)
    IEnumerator SendMessageRoutine(string text)
    {
        yield return api.SendMessage(text, OnNPCReply);

        // 🔥 cooldown to prevent API spam (VERY IMPORTANT)
        yield return new WaitForSeconds(2.5f);

        isProcessing = false;
    }

    // 🤖 NPC RESPONSE (unchanged but safer)
    void OnNPCReply(string text, string audioUrl)
    {
        Debug.Log("NPC Reply: " + text);

        npcText.text = text;

        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Debug.Log("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }
        else
        {
            Debug.LogWarning("Audio URL missing or AudioManager not assigned!");
        }
    }
}