using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class APIManager : MonoBehaviour
{
    private string baseURL = "http://127.0.0.1:8000";
    private string sessionId;

    // 🔥 START SESSION
    public IEnumerator StartSession(System.Action<string, string> callback)
    {
        string url = baseURL + "/start";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string raw = request.downloadHandler.text;
            Debug.Log("START RAW RESPONSE: " + raw);

            StartResponse response = JsonUtility.FromJson<StartResponse>(raw);

            sessionId = response.session_id;

            Debug.Log("Session ID: " + sessionId);
            Debug.Log("NPC Text: " + response.npc_text);
            Debug.Log("Audio URL: " + response.audio_url);

            callback(response.npc_text, response.audio_url);
        }
        else
        {
            Debug.LogError("StartSession Error: " + request.error);
        }
    }

    // 🔥 SEND PLAYER MESSAGE
    public IEnumerator SendMessage(string playerInput, System.Action<string, string> callback)
    {
        string url = baseURL + "/step";

        StepRequest data = new StepRequest
        {
            session_id = sessionId,
            player_input = playerInput
        };

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string raw = request.downloadHandler.text;
            Debug.Log("STEP RAW RESPONSE: " + raw);

            StepResponse response = JsonUtility.FromJson<StepResponse>(raw);

            Debug.Log("NPC Text: " + response.npc_text);
            Debug.Log("Audio URL: " + response.audio_url);

            callback(response.npc_text, response.audio_url);
        }
        else
        {
            Debug.LogError("SendMessage Error: " + request.error);
        }
    }
}