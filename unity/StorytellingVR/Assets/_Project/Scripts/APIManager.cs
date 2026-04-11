using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class APIManager : MonoBehaviour
{
    private string baseURL = "http://127.0.0.1:8000";
    private string sessionId;

    public IEnumerator StartSession(System.Action<string> callback)
    {
        string url = baseURL + "/start";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<StartResponse>(request.downloadHandler.text);
            sessionId = response.session_id;
            callback(response.npc_text);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    public IEnumerator SendMessage(string playerInput, System.Action<string> callback)
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
            var response = JsonUtility.FromJson<StepResponse>(request.downloadHandler.text);
            callback(response.npc_text);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}