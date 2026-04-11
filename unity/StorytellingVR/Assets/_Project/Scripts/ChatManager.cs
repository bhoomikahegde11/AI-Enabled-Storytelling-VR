using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public APIManager api;

    public TMP_InputField inputField;
    public TextMeshProUGUI npcText;

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

    void OnNPCReply(string text)
    {
        npcText.text = text;
    }
}