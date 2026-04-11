using UnityEngine;

public class NPCInteractor : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask npcLayer;

    public APIClient apiClient;

    void Update()
    {
        // 🔥 Ray from camera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, npcLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);

            // 🔥 Press E to interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("NPC Clicked!");

                // Start conversation or send message
                StartCoroutine(apiClient.SendMessage("hello"));
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        }
    }
}