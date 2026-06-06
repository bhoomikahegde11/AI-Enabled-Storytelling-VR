using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class APIManager : MonoBehaviour
{
    private string baseURL = "http://127.0.0.1:8000";
    private string sessionId;

    [Header("Debug Logging")]
    [SerializeField]
    private bool showDebugLogs = true;

    [Header("Current Buyer / Trade Data cached from Backend")]
    public string currentBuyerName;
    public string currentBuyerOrigin;
    public string currentSpiceName;
    public string currentSpiceQuantity;

    // 🔥 START SESSION
    public IEnumerator StartSession(System.Action<string, string, int, int, bool, TransactionSummary, string, CurrentTrade, int> callback)
    {
        string url = baseURL + "/start";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (showDebugLogs)
        {
            Debug.Log($"[PERF API] Request sent timestamp: {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[PERF API] Response received timestamp: {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            }

            string raw = request.downloadHandler.text;
            Debug.Log("START RAW RESPONSE: " + raw);

            StartResponse response = JsonUtility.FromJson<StartResponse>(raw);

            sessionId = response.session_id;
            currentBuyerName = response.buyer_name;
            currentBuyerOrigin = response.buyer_origin;
            currentSpiceName = response.spice_name;
            currentSpiceQuantity = response.spice_quantity;

            if (showDebugLogs)
            {
                Debug.Log($"[REP API] {response.player_reputation}");
            }

            Debug.Log("Session ID: " + sessionId);
            Debug.Log("NPC Text: " + response.npc_text);
            Debug.Log("Audio URL: " + response.audio_url);
            Debug.Log("Reputation: " + response.reputation);
            Debug.Log("Total Varahas: " + response.total_varahas);
            Debug.Log("Done: " + response.done);

            callback(response.npc_text, response.audio_url, response.player_reputation, response.player_money, response.done, null, response.action, response.current_trade, response.reputation_delta);
        }
        else
        {
            Debug.LogError("StartSession Error: " + request.error);
            callback(null, null, -1, -1, false, null, null, null, 0);
        }
    }

    // 🔥 SEND PLAYER MESSAGE
    public IEnumerator SendMessage(string playerInput, System.Action<string, string, int, int, bool, TransactionSummary, string, CurrentTrade, int> callback)
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

        if (showDebugLogs)
        {
            Debug.Log($"[PERF API] Request sent timestamp: {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[PERF API] Response received timestamp: {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            }

            string raw = request.downloadHandler.text;
            Debug.Log("STEP RAW RESPONSE: " + raw);

            StepResponse response = JsonUtility.FromJson<StepResponse>(raw);

            if (response != null && !string.IsNullOrEmpty(response.buyer_name))
            {
                currentBuyerName = response.buyer_name;
                currentBuyerOrigin = response.buyer_origin;
                currentSpiceName = response.spice_name;
                currentSpiceQuantity = response.spice_quantity;
            }

            if (showDebugLogs)
            {
                Debug.Log($"[REP API] {response.player_reputation}");
            }

            Debug.Log("NPC Text: " + response.npc_text);
            Debug.Log("Audio URL: " + response.audio_url);
            Debug.Log("Reputation: " + response.reputation);
            Debug.Log("Total Varahas: " + response.total_varahas);
            Debug.Log("Done: " + response.done);

            callback(response.npc_text, response.audio_url, response.player_reputation, response.player_money, response.done, response.transaction, response.action, response.current_trade, response.reputation_delta);
        }
        else
        {
            Debug.LogError("SendMessage Error: " + request.error);
            callback(null, null, -1, -1, false, null, null, null, 0);
        }
    }
}