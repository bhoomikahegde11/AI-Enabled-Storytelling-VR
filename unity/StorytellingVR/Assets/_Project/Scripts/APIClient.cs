using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class StepResponse
{
    public string action;
    public int price;
    public int quantity_grams;
    public string item;
    public int total_price;
    public string dialogue;
}

[System.Serializable]
public class ApiResponse
{
    public string session_id;
    public StepResponse response;
}

public class APIClient : MonoBehaviour
{
    public string baseUrl = "http://127.0.0.1:8000";
    public string sessionId;

    public TMP_Text dialogueText;

    void Start()
    {
        StartCoroutine(StartSession());
    }

    public IEnumerator StartSession()
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(baseUrl + "/start", "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ApiResponse res = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
            sessionId = res.session_id;

            Debug.Log("Session started: " + sessionId);
            UpdateText(res.response.dialogue);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    public IEnumerator SendMessage(string playerInput)
    {
        string json = "{\"session_id\":\"" + sessionId + "\",\"player_input\":\"" + playerInput + "\"}";

        UnityWebRequest request = new UnityWebRequest(baseUrl + "/step", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ApiResponse res = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

            Debug.Log("NPC: " + res.response.dialogue);
            UpdateText(res.response.dialogue);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    void UpdateText(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
        }
    }
}