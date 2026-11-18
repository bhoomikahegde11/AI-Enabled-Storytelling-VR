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
    [SerializeField]
    private MeeraInspectableItem lampOrVase;

    [SerializeField]
    private MeeraInspectableItem foreignTrinket;

    [SerializeField]
    private MeeraInspectableItem book;

    [Header("Meta XR Inspection Ray")]
    [Tooltip(
        "Assign the Meta XR Building Block GameObject whose activation " +
        "shows or hides the controller ray."
    )]
    [SerializeField]
    private GameObject inspectionRayRoot;

    [SerializeField]
    [Tooltip("Assign the left hand XR ray so it can be disabled during inspection.")]
    private GameObject leftHandRayRoot;

    [Header("Book Interaction")]
    [Tooltip(
        "Drag the RayInteractable component from the book interaction object."
    )]
    [SerializeField]
    private Behaviour lampRayInteractable;

    [SerializeField]
    private Behaviour compassRayInteractable;

    [SerializeField]
    private Behaviour bookRayInteractable;

    [Header("Compass Highlight")]
    [Tooltip(
        "Assign the point light that turns on when the compass is selected."
    )]
    [SerializeField]
    private Light compassPointLight;

    [Header("Sequence Settings")]
    [SerializeField]
    private bool inspectionSequenceActive;

    [Header("Temporary Dialogue Timing")]
    [SerializeField]
    private float normalItemDialogueDuration = 4f;

    [SerializeField]
    private float bookDialogueDuration = 5f;

    [SerializeField]
    private float pauseBetweenBookLines = 0.75f;

    [Header("Dialogue Events")]
    public StringEvent onMeeraLineRequested;
    public StringEvent onPlayerThoughtRequested;
    public StringEvent onNarratorLineRequested;

    [Header("Sequence Events")]
    public UnityEvent onInspectionSequenceStarted;
    public UnityEvent onAllItemsInspected;

    [Header("Post-Inspection Question Flow")]
    [Tooltip(
        "Assign Meera's NPCInteraction component to open " +
        "her question canvas after inspection completes."
    )]
    [SerializeField]
    private NPCInteraction meeraNPCInteraction;

    private bool inspectionBusy;
    private bool sequenceCompleted;
    private NarratorUIManager activeNarrator;
    private bool firstItemInspected = false;

    public bool InspectionSequenceActive =>
        inspectionSequenceActive;

    public bool InspectionBusy =>
        inspectionBusy;

    public bool SequenceCompleted =>
        sequenceCompleted;

    private void Awake()
    {
        lampRayInteractable = GetActualInteractable(lampRayInteractable, lampOrVase);
        compassRayInteractable = GetActualInteractable(compassRayInteractable, foreignTrinket);
        bookRayInteractable = GetActualInteractable(bookRayInteractable, book);
        SetItemInteraction(lampRayInteractable, lampOrVase, null);
        SetItemInteraction(compassRayInteractable, foreignTrinket, null);
    }

    private void OnEnable()
    {
        // The disabled copy of this controller in the epilogue does not own the book ray.
        RefreshItemAvailability();
        if (!inspectionSequenceActive)
            StartCoroutine(PauseMenuController.RunGameplay(EnforceInitialInteractionLock()));
    }

    private IEnumerator EnforceInitialInteractionLock()
    {
        yield return null;
        if (!inspectionSequenceActive)
            RefreshItemAvailability();
    }

    public void BeginInspectionSequence()
    {
        if (inspectionSequenceActive || sequenceCompleted)
            return;

        inspectionSequenceActive = true;
        RefreshItemAvailability();
        SetInspectionRay(true);
        ShowInspectionPrompt();
        onInspectionSequenceStarted?.Invoke();
    }

    private MeeraInspectableItem NextItem()
    {
        if (lampOrVase == null || !lampOrVase.HasBeenInspected)
            return lampOrVase;
        if (foreignTrinket == null || !foreignTrinket.HasBeenInspected)
            return foreignTrinket;
        return book != null && !book.HasBeenInspected ? book : null;
    }

    public bool CanInspect(MeeraInspectableItem item) =>
        isActiveAndEnabled && inspectionSequenceActive && !inspectionBusy &&
        !PauseMenuController.GameplayInputBlocked && item != null && item.isActiveAndEnabled && item == NextItem();

    public void RequestInspection(MeeraInspectableItem item)
    {
        if (!CanInspect(item))
            return;

        // Both rays can report the selection. Lock before running the dialogue.
        inspectionBusy = true;
        RefreshItemAvailability();
        StartCoroutine(PauseMenuController.RunGameplay(InspectItemSequence(item)));
    }

    private IEnumerator InspectItemSequence(MeeraInspectableItem item)
    {
        firstItemInspected = true;
        HideInspectionPrompt();
        SetInspectionRay(false);
        item.PlaySelectionFeedback();

        Debug.Log(
            $"[MEERA INSPECTION] Inspecting " +
            $"{item.ItemDisplayName}."
        );

        switch (item.ItemType)
        {
            case MeeraInspectableItem
                .InspectableItemType.LampOrVase:

                yield return PlayNormalItemSequence(
                    "MEERA_LAMP_01",
                    "This was made by craftsmen from a settlement " +
                    "near the kingdom. Objects like this are used " +
                    "both in homes and during ceremonies."
                );

                break;

            case MeeraInspectableItem
                .InspectableItemType.ForeignTrinket:

                /*
                 * The existing selection feedback may already turn
                 * the compass light on. We explicitly ensure it is on.
                 */
                SetCompassLight(true);

                yield return PlayNormalItemSequence(
                    "MEERA_COMPASS_01",
                    "A foreign trader brought this through the western " +
                    "ports. Its material and decoration are unlike the " +
                    "objects made by our local craftsmen."
                );

                /*
                 * Turn it off immediately after Meera finishes
                 * discussing the compass.
                 */
                SetCompassLight(false);

                break;

            case MeeraInspectableItem
                .InspectableItemType.Book:

                yield return PlayBookSequence();

                break;
        }

        FinishInspection(item);
    }

    private void FinishInspection(MeeraInspectableItem item)
    {
        item.MarkAsInspected();
        activeNarrator = null;
        inspectionBusy = false;

        if (item != book)
        {
            RefreshItemAvailability();
            SetInspectionRay(true);
            return;
        }

        sequenceCompleted = true;
        inspectionSequenceActive = false;
        RefreshItemAvailability();
        onAllItemsInspected?.Invoke();
        if (sequenceCompleted)
            StartCoroutine(PauseMenuController.RunGameplay(PostInspectionQuestionFlow()));
    }

    private IEnumerator PlayNormalItemSequence(
        string lineId,
        string meeraLine
    )
    {
        Debug.Log($"[MEERA] {meeraLine}");

        NarratorUIManager narrator = activeNarrator = NarratorUIManager.Instance;

        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                lineId,
                "Meera",
                meeraLine
            );
        }
        else
        {
            Debug.LogWarning(
                "[MEERA INSPECTION] " +
                "NarratorUIManager.Instance is missing."
            );

            yield return PauseMenuController.WaitRealtime(
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

        NarratorUIManager narrator = activeNarrator = NarratorUIManager.Instance;

        if (narrator == null)
        {
            Debug.LogError(
                "[MEERA INSPECTION] " +
                "NarratorUIManager.Instance is missing."
            );

            yield return PauseMenuController.WaitRealtime(
                bookDialogueDuration
            );

            yield break;
        }

        Debug.Log(
            $"[PLAYER THOUGHT] {playerThought}"
        );

        yield return narrator.PlayNarration(
            "You",
            playerThought
        );

        yield return PauseMenuController.WaitRealtime(
            pauseBetweenBookLines
        );

        Debug.Log($"[MEERA] {meeraLine}");

        yield return narrator.PlayNarration(
            "MEERA_NOTEBOOK_01",
            "Meera",
            meeraLine
        );

        yield return PauseMenuController.WaitRealtime(
            pauseBetweenBookLines
        );

        Debug.Log(
            $"[NARRATOR] {narratorLine}"
        );

        yield return narrator.PlayNarration(
            "NARRATOR_NOTEBOOK_01",
            "Narrator",
            narratorLine
        );
    }

    public void ResetInspectionSequence()
    {
        StopAllCoroutines();
        if (activeNarrator != null)
            activeNarrator.StopCurrentRoutine();
        activeNarrator = null;

        HideInspectionPrompt();
        firstItemInspected = false;
        inspectionSequenceActive = false;
        inspectionBusy = false;
        sequenceCompleted = false;

        lampOrVase?.ResetInspection();
        foreignTrinket?.ResetInspection();
        book?.ResetInspection();

        RefreshItemAvailability();
        SetInspectionRay(false);
    }

    private void ShowInspectionPrompt()
    {
        if (firstItemInspected)
            return;

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowPrompt(
                "Inspect",
                "Aim the RIGHT RAY at an object and press the RIGHT TRIGGER to inspect it.",
                this
            );
        }
    }

    private void HideInspectionPrompt()
    {
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.HidePrompt(this);
        }
    }

    public void SetInspectionRay(bool enabled)
    {
        if (inspectionRayRoot != null)
        {
            inspectionRayRoot.SetActive(enabled);
        }
        else
        {
            Debug.LogWarning(
                "[MEERA INSPECTION] Inspection Ray Root is not assigned."
            );
        }

        if (leftHandRayRoot != null)
        {
            leftHandRayRoot.SetActive(!enabled);
        }

        Debug.Log(
            $"[MEERA INSPECTION] Meta inspection ray active: {enabled}."
        );
    }

    private Behaviour GetActualInteractable(Behaviour serializedRef, MeeraInspectableItem item)
    {
        if (serializedRef != null)
        {
            Component c = serializedRef.GetComponent("RayInteractable");
            if (c != null && c is Behaviour b) return b;
            return serializedRef;
        }

        if (item != null)
        {
            Component[] comps = item.GetComponentsInChildren<Component>(true);
            foreach (Component c in comps)
            {
                if (c is Behaviour interactable && interactable.GetType().Name == "RayInteractable")
                {
                    return interactable;
                }
            }
        }
        return null;
    }

    private void RefreshItemAvailability()
    {
        var current = inspectionSequenceActive && !inspectionBusy ? NextItem() : null;
        SetItemInteraction(lampRayInteractable, lampOrVase, current);
        SetItemInteraction(compassRayInteractable, foreignTrinket, current);
        SetItemInteraction(bookRayInteractable, book, current);
        SetCompassLight(current != null && current == foreignTrinket);
    }

    private static void SetItemInteraction(Behaviour interactable, MeeraInspectableItem item, MeeraInspectableItem current)
    {
        bool available = item != null && item == current;
        if (interactable != null)
            interactable.enabled = available;
        if (item != null)
            item.SetInspectionHighlight(available);
    }

    private void SetCompassLight(bool enabled)
    {
        if (compassPointLight == null)
            return;

        compassPointLight.enabled = enabled;
    }

    private IEnumerator PostInspectionQuestionFlow()
    {
        string buyPrompt =
            "Would you like to buy anything?";

        NarratorUIManager narrator = activeNarrator = NarratorUIManager.Instance;

        Debug.Log(
            $"[MEERA INSPECTION] Post-inspection prompt: {buyPrompt}"
        );

        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                "MEERA_BUY_PROMPT_01",
                "Meera",
                buyPrompt
            );
        }
        else
        {
            yield return PauseMenuController.WaitRealtime(3f);
        }

        activeNarrator = null;
        if (meeraNPCInteraction != null)
        {
            Debug.Log(
                "[MEERA INSPECTION] Opening Meera question canvas " +
                "via UnlockAndStartConversation."
            );

            meeraNPCInteraction.UnlockAndStartConversation();
        }
        else
        {
            Debug.LogError(
                "[MEERA INSPECTION] meeraNPCInteraction is not assigned. " +
                "Cannot open the question canvas."
            );
        }
    }
}
