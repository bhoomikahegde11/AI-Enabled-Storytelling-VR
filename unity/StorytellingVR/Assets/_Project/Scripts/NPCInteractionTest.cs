using UnityEngine;

public class NPCInteractionTest : MonoBehaviour
{
    public APIClient apiClient;

    void Start()
    {
        StartCoroutine(apiClient.StartSession());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(apiClient.SendMessage("yes we do"));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(apiClient.SendMessage("give for 50"));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(apiClient.SendMessage("ok deal"));
        }
    }
}