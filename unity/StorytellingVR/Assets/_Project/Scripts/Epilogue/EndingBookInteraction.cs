using UnityEngine;

public class EndingBookInteraction : MonoBehaviour
{
    [Header("Book Item")]
    [Tooltip("Assign the MeeraInspectableItem on the epilogue book so it can be disabled after selection.")]
    [SerializeField] private MeeraInspectableItem bookItem;

    private bool interactionEnabled = false;
    private bool hasTriggered = false;

    private void Start()
    {
        // Book is locked until EpilogueStoryManager explicitly enables it.
        Disable();
    }

    /// <summary>
    /// Called by EpilogueStoryManager when the OpenBook stage begins.
    /// Enables the MeeraInspectableItem so the ray can detect and highlight it.
    /// </summary>
    public void Enable()
    {
        interactionEnabled = true;
        if (bookItem != null)
            bookItem.enabled = true;
        Debug.Log("[ENDING BOOK] Interaction enabled — ray can now select the book.");
    }

    /// <summary>
    /// Disables the MeeraInspectableItem so the ray ignores this book entirely.
    /// </summary>
    public void Disable()
    {
        interactionEnabled = false;
        if (bookItem != null)
            bookItem.enabled = false;
        Debug.Log("[ENDING BOOK] Interaction disabled.");
    }

    /// <summary>
    /// Called by MeeraInspectableItem.TryInspect() via the epilogue override path
    /// when the player selects the book with the ray.
    /// </summary>
    public void TryInspect()
    {
        if (!interactionEnabled || hasTriggered) return;
        hasTriggered = true;

        // Mark the item inspected immediately so the ray no longer targets it.
        if (bookItem != null)
            bookItem.MarkAsInspected();

        Disable();

        Debug.Log("[ENDING BOOK] Book selected — notifying EpilogueStoryManager.");

        if (EpilogueStoryManager.Instance != null)
            EpilogueStoryManager.Instance.NotifyBookInteracted();
    }
}
