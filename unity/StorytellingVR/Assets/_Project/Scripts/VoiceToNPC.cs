using UnityEngine;
using Oculus.Voice;

public class VoiceToNPC : MonoBehaviour
{
    public AppVoiceExperience voice;
    public APIClient apiClient;

    void Start()
    {
        if (voice != null)
        {
            voice.VoiceEvents.OnFullTranscription.AddListener(OnTranscription);
        }
        else
        {
            Debug.LogError("Voice not assigned!");
        }
    }

    void OnTranscription(string text)
    {
        Debug.Log("User said: " + text);

        if (apiClient != null)
        {
            StartCoroutine(apiClient.SendMessage(text));
        }
        else
        {
            Debug.LogError("APIClient not assigned!");
        }
    }
}