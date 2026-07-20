using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class NPCInteractionVR : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject interactionHint;
    public SilkTraderDialogueManager dialogueManager;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (interactable == null)
        {
            interactable = GetComponent<XRSimpleInteractable>();
        }

        interactable.selectEntered.AddListener(OpenDialogue);
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OpenDialogue);
        }
    }

    public void OpenDialogue(SelectEnterEventArgs args)
    {
        OpenDialogue();
    }

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
