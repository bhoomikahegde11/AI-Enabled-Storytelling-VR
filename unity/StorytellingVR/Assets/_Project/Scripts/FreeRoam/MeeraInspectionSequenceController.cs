using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MeeraInspectionSequenceController : MonoBehaviour
{
    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }

    [Header("Inspectable Items")]
    [SerializeField] private MeeraInspectableItem lampOrVase;
    [SerializeField] private MeeraInspectableItem foreignTrinket;
    [SerializeField] private MeeraInspectableItem book;

    [Header("Sequence Settings")]
    [SerializeField] private bool inspectionSequenceActive;
    [SerializeField] private bool requireOtherItemsBeforeBook = true;

    [Header("Temporary Dialogue Timing")]
    [SerializeField] private float normalItemDialogueDuration = 4f;
    [SerializeField] private float bookDialogueDuration = 5f;
    [SerializeField] private float pauseBetweenBookLines = 0.75f;

    [Header("Dialogue Events")]
    public StringEvent onMeeraLineRequested;
    public StringEvent onPlayerThoughtRequested;
    public StringEvent onNarratorLineRequested;

    [Header("Sequence Events")]
    public UnityEvent onInspectionSequenceStarted;
    public UnityEvent onAllItemsInspected;

    private bool inspectionBusy;
    private bool sequenceCompleted;

    public bool InspectionSequenceActive => inspectionSequenceActive;
    public bool InspectionBusy => inspectionBusy;
    public bool SequenceCompleted => sequenceCompleted;

    public void BeginInspectionSequence()
    {
        if (inspectionSequenceActive || sequenceCompleted)
            return;

        inspectionSequenceActive = true;
        inspectionBusy = false;

        Debug.Log(
            "[MEERA INSPECTION] Inspection sequence started."
        );

        onInspectionSequenceStarted?.Invoke();
    }

    public void RequestInspection(MeeraInspectableItem item)
    {
        if (!inspectionSequenceActive)
        {
            Debug.Log(
                "[MEERA INSPECTION] Inspection is not currently active."
            );

            return;
        }

        if (inspectionBusy)
        {
            Debug.Log(
                "[MEERA INSPECTION] Meera is already speaking."
            );

            return;
        }

        if (item == null || item.HasBeenInspected)
            return;

        if (item.ItemType ==
                MeeraInspectableItem.InspectableItemType.Book &&
            requireOtherItemsBeforeBook &&
            !OtherItemsHaveBeenInspected())
        {
            Debug.Log(
                "[MEERA INSPECTION] The book is not available yet. " +
                "Inspect the other two objects first."
            );

            onPlayerThoughtRequested?.Invoke(
                "I should examine the other objects first."
            );

            return;
        }

        StartCoroutine(InspectItemSequence(item));
    }

    private IEnumerator InspectItemSequence(
        MeeraInspectableItem item
    )
    {
        inspectionBusy = true;

        item.PlaySelectionFeedback();

        Debug.Log(
            $"[MEERA INSPECTION] Inspecting {item.ItemDisplayName}."
        );

        switch (item.ItemType)
        {
            case MeeraInspectableItem.InspectableItemType.LampOrVase:
                yield return PlayNormalItemSequence(
                    "This was made by craftsmen from a settlement " +
                    "near the kingdom. Objects like this are used " +
                    "both in homes and during ceremonies."
                );
                break;

            case MeeraInspectableItem.InspectableItemType.ForeignTrinket:
                yield return PlayNormalItemSequence(
                    "A foreign trader brought this through the western " +
                    "ports. Its material and decoration are unlike the " +
                    "objects made by our local craftsmen."
                );
                break;

            case MeeraInspectableItem.InspectableItemType.Book:
                yield return PlayBookSequence();
                break;
        }

        item.MarkAsInspected();

        inspectionBusy = false;

        CheckForSequenceCompletion();
    }

    private IEnumerator PlayNormalItemSequence(string meeraLine)
    {
        Debug.Log($"[MEERA] {meeraLine}");

        NarratorUIManager narrator =
            NarratorUIManager.Instance;

        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                "Meera",
                meeraLine
            );
        }
        else
        {
            Debug.LogWarning(
                "[MEERA INSPECTION] NarratorUIManager.Instance is missing."
            );

            yield return new WaitForSecondsRealtime(
                normalItemDialogueDuration
            );
        }
    }

    private IEnumerator PlayBookSequence()
    {
        string playerThought =
            "Why does this book look so familiar?";

        string meeraLine =
            "That book has always puzzled me. It is not made like " +
            "the palm-leaf manuscripts used here, and I cannot " +
            "recognise its writing. A travelling merchant sold it " +
            "to me somewhere within the kingdom.";

        string narratorLine =
            "Some objects appear to belong to more than one time.";

        NarratorUIManager narrator =
            NarratorUIManager.Instance;

        if (narrator == null)
        {
            Debug.LogError(
                "[MEERA INSPECTION] NarratorUIManager.Instance is missing."
            );

            yield break;
        }

        Debug.Log($"[PLAYER THOUGHT] {playerThought}");

        yield return narrator.PlayNarration(
            "You",
            playerThought
        );

        yield return new WaitForSecondsRealtime(
            pauseBetweenBookLines
        );

        Debug.Log($"[MEERA] {meeraLine}");

        yield return narrator.PlayNarration(
            "Meera",
            meeraLine
        );

        yield return new WaitForSecondsRealtime(
            pauseBetweenBookLines
        );

        Debug.Log($"[NARRATOR] {narratorLine}");

        yield return narrator.PlayNarration(
            "Narrator",
            narratorLine
        );
    }

    private bool OtherItemsHaveBeenInspected()
    {
        bool firstItemComplete =
            lampOrVase != null &&
            lampOrVase.HasBeenInspected;

        bool secondItemComplete =
            foreignTrinket != null &&
            foreignTrinket.HasBeenInspected;

        return firstItemComplete && secondItemComplete;
    }

    private void CheckForSequenceCompletion()
    {
        bool lampComplete =
            lampOrVase != null &&
            lampOrVase.HasBeenInspected;

        bool trinketComplete =
            foreignTrinket != null &&
            foreignTrinket.HasBeenInspected;

        bool bookComplete =
            book != null &&
            book.HasBeenInspected;

        if (!lampComplete ||
            !trinketComplete ||
            !bookComplete)
        {
            return;
        }

        sequenceCompleted = true;
        inspectionSequenceActive = false;

        Debug.Log(
            "[MEERA INSPECTION] All three objects inspected."
        );

        onAllItemsInspected?.Invoke();
    }

    public void ResetInspectionSequence()
    {
        StopAllCoroutines();

        inspectionSequenceActive = false;
        inspectionBusy = false;
        sequenceCompleted = false;

        lampOrVase?.ResetInspection();
        foreignTrinket?.ResetInspection();
        book?.ResetInspection();

        Debug.Log(
            "[MEERA INSPECTION] Sequence reset."
        );


    }

}
