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

    [SerializeField]
    private bool requireOtherItemsBeforeBook = true;

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
    private bool inspectionSequenceStarted = false;
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

        SetItemInteraction(lampRayInteractable, lampOrVase, false);
        SetItemInteraction(compassRayInteractable, foreignTrinket, false);

        Debug.Log("[MEERA INSPECTION] Initial lamp interaction locked.");
        Debug.Log("[MEERA INSPECTION] Initial compass interaction locked.");
    }

    private void OnEnable()
    {
        if (!inspectionSequenceStarted)
        {
            SetItemInteraction(lampRayInteractable, lampOrVase, false);
            SetItemInteraction(compassRayInteractable, foreignTrinket, false);
            StartCoroutine(EnforceInitialInteractionLock());
        }
    }

    private IEnumerator EnforceInitialInteractionLock()
    {
        yield return null;

        if (!inspectionSequenceStarted)
        {
            SetItemInteraction(lampRayInteractable, lampOrVase, false);
            SetItemInteraction(compassRayInteractable, foreignTrinket, false);
        }
    }

    private void Start()
    {
        /*
         * Before Meera interaction: do not change the ray’s normal scene state.
         */

        /*
         * The book must not be hoverable or selectable yet.
         */
        SetBookInteraction(false);

        /*
         * Make sure the compass light does not begin enabled.
         */
        SetCompassLight(false);
    }

    public void BeginInspectionSequence()
    {
        if (inspectionSequenceActive || sequenceCompleted)
            return;

        inspectionSequenceStarted = true;
        inspectionSequenceActive = true;
        inspectionBusy = false;

        /*
         * Meera has now prompted the player to inspect.
         * Show the Meta XR ray and enable interactables.
         */
        SetInspectionRay(true);
        SetItemInteraction(lampRayInteractable, lampOrVase, true);
        SetItemInteraction(compassRayInteractable, foreignTrinket, true);
        
        Debug.Log("[MEERA INSPECTION] Inspection started; lamp and compass unlocked.");

        /*
         * Keep the book non-interactable until the lamp
         * and compass have both been inspected.
         */
        SetBookInteraction(
            !requireOtherItemsBeforeBook ||
            OtherItemsHaveBeenInspected()
        );

        SetCompassLight(false);

        ShowInspectionPrompt();

        Debug.Log(
            "[MEERA INSPECTION] Inspection sequence started. " +
            "Inspection ray enabled."
        );

        onInspectionSequenceStarted?.Invoke();
    }

    public void RequestInspection(
        MeeraInspectableItem item
    )
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
                MeeraInspectableItem
                    .InspectableItemType.Book &&
            requireOtherItemsBeforeBook &&
            !OtherItemsHaveBeenInspected())
        {
            /*
             * This is a backup check.
             * Normally the book RayInteractable is already disabled.
             */
            Debug.Log(
                "[MEERA INSPECTION] The book is not available yet. " +
                "Inspect the other two objects first."
            );

            SetBookInteraction(false);

            return;
        }

        StartCoroutine(
            InspectItemSequence(item)
        );
    }

    private IEnumerator InspectItemSequence(
        MeeraInspectableItem item
    )
    {
        firstItemInspected = true;
        HideInspectionPrompt();

        inspectionBusy = true;

        /*
         * Hide the ray while dialogue is playing.
         */
        SetInspectionRay(false);

        // Temporarily disable this item's interaction to prevent double triggers
        if (item == lampOrVase) SetItemInteraction(lampRayInteractable, lampOrVase, false);
        else if (item == foreignTrinket) SetItemInteraction(compassRayInteractable, foreignTrinket, false);

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

        item.MarkAsInspected();

        inspectionBusy = false;

        /*
         * After each normal item, check whether the book can now
         * become interactable.
         */
        bool bookUnlocked =
            !requireOtherItemsBeforeBook ||
            OtherItemsHaveBeenInspected();

        SetBookInteraction(bookUnlocked);

        /*
         * End of sequence logic for this item
         */
        if (item.HasBeenInspected)
        {
            if (item == lampOrVase) SetItemInteraction(lampRayInteractable, lampOrVase, false);
            else if (item == foreignTrinket) SetItemInteraction(compassRayInteractable, foreignTrinket, false);
        }

        CheckForSequenceCompletion();

        /*
         * Only restore the ray if the full sequence is not finished.
         */
        if (!sequenceCompleted)
        {
            SetInspectionRay(true);
        }
    }

    private IEnumerator PlayNormalItemSequence(
        string meeraLine
    )
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
                "[MEERA INSPECTION] " +
                "NarratorUIManager.Instance is missing."
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
                "[MEERA INSPECTION] " +
                "NarratorUIManager.Instance is missing."
            );

            yield return new WaitForSecondsRealtime(
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

        Debug.Log(
            $"[NARRATOR] {narratorLine}"
        );

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

        return firstItemComplete &&
               secondItemComplete;
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

        SetInspectionRay(false);
        SetBookInteraction(false);
        SetCompassLight(false);

        Debug.Log(
            "[MEERA INSPECTION] All three objects inspected. " +
            "Inspection ray disabled for question canvas."
        );

        onAllItemsInspected?.Invoke();

        StartCoroutine(PostInspectionQuestionFlow());
    }

    public void ResetInspectionSequence()
    {
        StopAllCoroutines();

        HideInspectionPrompt();
        firstItemInspected = false;

        inspectionSequenceActive = false;
        inspectionBusy = false;
        sequenceCompleted = false;

        lampOrVase?.ResetInspection();
        foreignTrinket?.ResetInspection();
        book?.ResetInspection();

        SetInspectionRay(false);
        SetBookInteraction(false);
        SetCompassLight(false);

        Debug.Log(
            "[MEERA INSPECTION] Sequence reset."
        );
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
        if (inspectionRayRoot == null)
        {
            Debug.LogWarning(
                "[MEERA INSPECTION] Inspection Ray Root is not assigned."
            );

            return;
        }

        inspectionRayRoot.SetActive(enabled);

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
                if (c.GetType().Name == "RayInteractable")
                {
                    return c as Behaviour;
                }
            }
        }
        return null;
    }

    private void SetItemInteraction(Behaviour serializedRef, MeeraInspectableItem item, bool enabled)
    {
        Behaviour actualInteractable = GetActualInteractable(serializedRef, item);
        if (actualInteractable != null)
        {
            actualInteractable.enabled = enabled;
            Debug.Log($"[MEERA INSPECTION] Item {item?.ItemDisplayName} RayInteractable enabled: {enabled}.");
        }
    }

    private void SetBookInteraction(bool enabled)
    {
        if (bookRayInteractable == null)
        {
            Debug.LogWarning(
                "[MEERA INSPECTION] Book RayInteractable is not assigned."
            );

            return;
        }

        Behaviour actualInteractable = bookRayInteractable;
        Component rayInteractable = bookRayInteractable.GetComponent("RayInteractable");
        if (rayInteractable != null && rayInteractable is Behaviour b)
        {
            actualInteractable = b;
        }

        actualInteractable.enabled = enabled;

        Debug.Log(
            $"[MEERA INSPECTION] Book RayInteractable enabled: {enabled}."
        );
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

        NarratorUIManager narrator =
            NarratorUIManager.Instance;

        Debug.Log(
            $"[MEERA INSPECTION] Post-inspection prompt: {buyPrompt}"
        );

        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                "Meera",
                buyPrompt
            );
        }
        else
        {
            yield return new WaitForSecondsRealtime(3f);
        }

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