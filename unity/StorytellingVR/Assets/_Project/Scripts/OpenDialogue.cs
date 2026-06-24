using UnityEngine;

public class OpenDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;

    public void OpenPanel()
    {
        dialoguePanel.SetActive(true);
    }

    public void ClosePanel()
    {
        dialoguePanel.SetActive(false);
    }
}