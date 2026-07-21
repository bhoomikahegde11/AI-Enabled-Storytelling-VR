using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public enum StoryTriggerEvent
    {
        TeleportTutorialCompleted,
        TrinketStallReached,
        NotebookConversationCompleted,
        MerchantConversationStarted,
        MerchantStartedWalking,
        MerchantReachedStall,
        SpiceStallEntered
    }

    [Header("Story Event")]
    [SerializeField]
    private StoryTriggerEvent triggerEvent;

    [Header("Behaviour")]
    [SerializeField]
    private bool triggerOnlyOnce = true;

    [SerializeField]
    private bool disableObjectAfterTrigger = false;

    [SerializeField]
    private bool requirePlayer = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnlyOnce)
            return;

        if (requirePlayer && !IsPlayer(other))
            return;

        TriggerStoryEvent();
    }

    [ContextMenu("Trigger Story Event")]
    public void TriggerStoryEvent()
    {
        if (hasTriggered && triggerOnlyOnce)
            return;

        if (FreeRoamStoryManager.Instance == null)
        {
            Debug.LogError(
                "[STORY TRIGGER] FreeRoamStoryManager.Instance is missing."
            );

            return;
        }

        hasTriggered = true;

        switch (triggerEvent)
        {
            case StoryTriggerEvent.TeleportTutorialCompleted:
                FreeRoamStoryManager.Instance
                    .NotifyTeleportTutorialCompleted();
                break;

            case StoryTriggerEvent.TrinketStallReached:
                FreeRoamStoryManager.Instance
                    .NotifyTrinketStallReached();
                break;

            case StoryTriggerEvent.NotebookConversationCompleted:
                FreeRoamStoryManager.Instance
                    .NotifyNotebookConversationCompleted();
                break;

            case StoryTriggerEvent.MerchantConversationStarted:
                FreeRoamStoryManager.Instance
                    .NotifyMerchantConversationStarted();
                break;

            case StoryTriggerEvent.MerchantStartedWalking:
                FreeRoamStoryManager.Instance
                    .NotifyMerchantStartedWalking();
                break;

            case StoryTriggerEvent.MerchantReachedStall:
                FreeRoamStoryManager.Instance
                    .NotifyMerchantReachedStall();
                break;

            case StoryTriggerEvent.SpiceStallEntered:
                FreeRoamStoryManager.Instance
                    .NotifySpiceStallEntered();
                break;
        }

        if (disableObjectAfterTrigger)
            gameObject.SetActive(false);
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        Transform root = other.transform.root;

        if (root != null &&
            root.CompareTag("Player"))
        {
            return true;
        }

        if (other.GetComponentInParent<CharacterController>() != null)
            return true;

        return root != null &&
               root.name.Contains("PlayerController");
    }
}