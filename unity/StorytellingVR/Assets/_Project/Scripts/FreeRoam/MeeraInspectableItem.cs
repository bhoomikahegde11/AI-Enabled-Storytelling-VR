using UnityEngine;

public class MeeraInspectableItem : MonoBehaviour
{
    public enum InspectableItemType
    {
        LampOrVase,
        ForeignTrinket,
        Book
    }

    [Header("Item Identity")]
    [SerializeField] private InspectableItemType itemType;
    [SerializeField] private string itemDisplayName;

    [Header("Inspection Controller")]
    [SerializeField]
    private MeeraInspectionSequenceController inspectionController;

    [Header("Optional Feedback")]
    [SerializeField] private GameObject glowObject;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip inspectionSound;

    [Header("Runtime State")]
    [SerializeField] private bool hasBeenInspected;

    public InspectableItemType ItemType => itemType;
    public string ItemDisplayName => itemDisplayName;
    public bool HasBeenInspected => hasBeenInspected;

    private void Awake()
    {
        if (glowObject != null)
            glowObject.SetActive(false);
    }

    /// <summary>
    /// Called by the controller ray when the player presses Trigger.
    /// </summary>
    public void TryInspect()
    {
        if (hasBeenInspected)
        {
            Debug.Log(
                $"[MEERA INSPECTION] {itemDisplayName} was already inspected."
            );

            return;
        }

        if (inspectionController == null)
        {
            Debug.LogError(
                $"[MEERA INSPECTION] {gameObject.name} has no " +
                $"MeeraInspectionSequenceController assigned."
            );

            return;
        }

        inspectionController.RequestInspection(this);
    }

    public void MarkAsInspected()
    {
        hasBeenInspected = true;

        if (glowObject != null)
            glowObject.SetActive(false);

        Debug.Log(
            $"[MEERA INSPECTION] {itemDisplayName} marked as inspected."
        );
    }

    public void PlaySelectionFeedback()
    {
        if (glowObject != null)
            glowObject.SetActive(true);

        if (audioSource != null && inspectionSound != null)
            audioSource.PlayOneShot(inspectionSound);
    }

    public void StopSelectionFeedback()
    {
        if (glowObject != null)
            glowObject.SetActive(false);
    }

    public void ResetInspection()
    {
        hasBeenInspected = false;

        if (glowObject != null)
            glowObject.SetActive(false);
    }
}