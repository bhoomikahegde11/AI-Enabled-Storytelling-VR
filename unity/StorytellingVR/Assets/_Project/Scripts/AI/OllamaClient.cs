using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OllamaClient : MonoBehaviour
{
    string url = "http://localhost:11434/api/generate";

    [System.Serializable]
    class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    public delegate void OnResponse(string response);

    public void Generate(string prompt, OnResponse callback)
    {
        StartCoroutine(SendRequest(prompt, callback));
    }

    IEnumerator SendRequest(string prompt, OnResponse callback)
    {
        OllamaRequest requestBody = new OllamaRequest();
        requestBody.model = "llama3";
        requestBody.prompt = prompt;
        requestBody.stream = false;

        string json = JsonUtility.ToJson(requestBody);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }
}