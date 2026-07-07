using UnityEngine;

public class TravellerInteraction : MonoBehaviour
{
    public TravellerDialogueManager dialogueManager;

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            playerInside = false;
            dialogueManager.CloseDialogue();
        }
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            dialogueManager.OpenDialogue();
        }
    }
}