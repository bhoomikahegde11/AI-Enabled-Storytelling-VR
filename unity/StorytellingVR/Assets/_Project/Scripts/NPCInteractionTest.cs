using UnityEngine;

public class NPCInteractionTest : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask npcLayer;
    public APIClient apiClient;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, npcLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("NPC Clicked");

                if (apiClient != null)
                {
                    StartCoroutine(apiClient.SendMessage("hello"));
                }
                else
                {
                    Debug.LogError("API CLIENT NOT ASSIGNED");
                }
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        }
    }
}