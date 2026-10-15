using UnityEngine;

public class EndingBookInteraction : MonoBehaviour
{
    [Header("Book Item (optional)")]
    [SerializeField] private MeeraInspectableItem bookItem;

    private bool interactionEnabled;
    private bool hasTriggered;

    private void Start()
    {
        // The manager enables this only after Meera's purchase dialogue.
        Disable();
    }

    public void Enable()
    {
        interactionEnabled = true;
        hasTriggered = false;
        // Do not toggle bookItem.enabled: the Meta ray UnityEvent may still call
        // its listener even when a MonoBehaviour is disabled. Our gate is here.
        Debug.Log($"[ENDING BOOK] Enabled on {name}. Book item assigned={bookItem != null}.");
    }

    public void Disable()
    {
        interactionEnabled = false;
        Debug.Log($"[ENDING BOOK] Disabled on {name}.");
    }

    // Wire Meta InteractableUnityEventWrapper -> When Select to THIS method.
    public void TryInspect()
    {
        EpilogueStoryManager manager = EpilogueStoryManager.Instance;
        if (manager == null)
            manager = FindFirstObjectByType<EpilogueStoryManager>();

        Debug.Log($"[ENDING BOOK] TryInspect FIRED on {name}. " +
                  $"enabled={interactionEnabled}, triggered={hasTriggered}, " +
                  $"managerFound={manager != null}, " +
                  $"stage={(manager != null ? manager.currentStage.ToString() : "UNKNOWN")}");

        if (hasTriggered)
        {
            Debug.LogWarning("[ENDING BOOK] Ignored duplicate selection.");
            return;
        }
        if (!interactionEnabled)
        {
            Debug.LogWarning("[ENDING BOOK] Ignored selection: interaction has not been enabled yet.");
            return;
        }
        if (manager == null)
        {
            Debug.LogError("[ENDING BOOK] No EpilogueStoryManager found.");
            return;
        }
        if (manager.currentStage != EpilogueStoryManager.EpilogueStage.OpenBook)
        {
            Debug.LogWarning($"[ENDING BOOK] Stage gate rejected selection: {manager.currentStage}.");
            return;
        }

        hasTriggered = true;
        if (bookItem != null)
            bookItem.MarkAsInspected();

        Disable();
        Debug.Log("[ENDING BOOK] Valid selection; notifying EpilogueStoryManager.");
        manager.NotifyBookInteracted();
    }
}
