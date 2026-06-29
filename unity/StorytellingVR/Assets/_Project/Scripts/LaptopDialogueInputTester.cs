using UnityEngine;

public class LaptopDialogueInputTester : MonoBehaviour
{
    public KeyCode openKey = KeyCode.X;
    public KeyCode closeKey = KeyCode.C;
    public GameObject dialoguePanel;
    public GameObject interactionHint;
    public SilkTraderDialogueManager dialogueManager;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            OpenDialogue();
        }

        if (Input.GetKeyDown(closeKey))
        {
            CloseDialogue();
        }
    }

    public void OpenDialogue()
    {
        ResolveReferences();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }

        if (dialogueManager != null)
        {
            dialogueManager.PrepareDialogue();
        }
    }

    public void CloseDialogue()
    {
        ResolveReferences();

        if (dialogueManager != null)
        {
            dialogueManager.CloseDialogue();
            return;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(true);
        }
    }

    private void ResolveReferences()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<SilkTraderDialogueManager>();
        }

        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("DialoguePanel");
        }

        if (interactionHint == null)
        {
            interactionHint = GameObject.Find("InteractionHint");
        }
    }
}
