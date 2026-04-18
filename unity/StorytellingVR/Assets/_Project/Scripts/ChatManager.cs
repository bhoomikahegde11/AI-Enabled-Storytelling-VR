using UnityEngine;
using TMPro;

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

    void Start()
    {
        StartCoroutine(api.StartSession(OnNPCReply));
    }

    public void OnSend()
    {
        string playerText = inputField.text;

        if (string.IsNullOrEmpty(playerText)) return;

        StartCoroutine(api.SendMessage(playerText, OnNPCReply));

        inputField.text = "";
    }

    void OnNPCReply(string text, string audioUrl)
    {
        npcText.text = text;

        if (audioManager != null && !string.IsNullOrEmpty(audioUrl))
        {
            Debug.Log("Playing audio: " + audioUrl);
            audioManager.PlayAudioFromUrl(audioUrl);
        }
        else
        {
            Debug.LogWarning("Audio URL missing!");
        }
    }
}