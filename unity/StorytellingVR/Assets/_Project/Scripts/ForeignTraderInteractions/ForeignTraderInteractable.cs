using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// NOTE: If you're on XR Interaction Toolkit 3.x, the namespace for
// XRSimpleInteractable moved to:
// using UnityEngine.XR.Interaction.Toolkit.Interactables;
// Swap the using statement above if you get a "type not found" error.

/// <summary>
/// Put this on the Foreign Trader GameObject, alongside an XRSimpleInteractable
/// component and a Collider. When the player points the ray interactor at the
/// trader and pulls the trigger (Select), this starts the conversation.
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class ForeignTraderInteractable : MonoBehaviour
{
    [Tooltip("Drag the ConversationCanvas GameObject's TraderConversationManager here.")]
    [SerializeField] private TraderConversationManager conversationManager;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (conversationManager == null)
        {
            Debug.LogWarning("ForeignTraderInteractable: conversationManager not assigned.");
            return;
        }

        conversationManager.StartConversation();
    }
}