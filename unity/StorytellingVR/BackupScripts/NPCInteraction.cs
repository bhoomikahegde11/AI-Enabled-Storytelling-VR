using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject interactionHint;
    public SilkTraderDialogueManager dialogueManager;

    public void OpenDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }

        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<SilkTraderDialogueManager>();
        }

        if (dialogueManager != null)
        {
            dialogueManager.PrepareDialogue();
        }
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(true);
        }
    }
}
