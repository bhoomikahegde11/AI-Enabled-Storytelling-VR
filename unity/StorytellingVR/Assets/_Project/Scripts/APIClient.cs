using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;

public class APIClient : MonoBehaviour
{
    private string baseURL = "http://localhost:8000";

    public string sessionId;

    public TextMeshPro dialogueText;

    [System.Serializable]
    public class StepRequest
    {
        public string session_id;
        public string player_input;
    }

    [System.Serializable]
    public class StartResponse
    {
        public string session_id;
    }

    void Start()
    {
        StartCoroutine(StartSession());
    }

    // 🚀 START SESSION
    public IEnumerator StartSession()
    {
        UnityWebRequest req = new UnityWebRequest(baseURL + "/start", "POST");
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Start Response: " + req.downloadHandler.text);

            StartResponse res = JsonUtility.FromJson<StartResponse>(req.downloadHandler.text);
            sessionId = res.session_id;

            // 🔥 Optional: show first NPC line
            string dialogue = ExtractDialogue(req.downloadHandler.text);
            if (dialogueText != null)
                dialogueText.text = dialogue;
        }
        else
        {
            Debug.LogError(req.error);
        }
    }

    // 🔁 SEND INPUT
    public IEnumerator SendMessage(string input)
    {
        StepRequest data = new StepRequest
        {
            session_id = sessionId,
            player_input = input
        };

        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest(baseURL + "/step", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = req.downloadHandler.text;
            Debug.Log("NPC Response: " + jsonResponse);

            string dialogue = ExtractDialogue(jsonResponse);

            if (dialogueText != null)
            {
                dialogueText.text = dialogue;
            }
        }
        else
        {
            Debug.LogError(req.error);
        }
    }

    // 🔥 Manual JSON parsing
    string ExtractDialogue(string json)
    {
        string key = "\"dialogue\":\"";
        int start = json.IndexOf(key);

        if (start == -1) return "No dialogue";

        start += key.Length;
        int end = json.IndexOf("\"", start);

        if (end == -1) return "No dialogue";

        return json.Substring(start, end - start);
    }
}