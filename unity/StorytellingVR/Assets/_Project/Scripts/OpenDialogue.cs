using UnityEngine;

public class OpenDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public SilkTraderDialogueManager dialogueManager;

    public void OpenPanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
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

    public void ClosePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
}
