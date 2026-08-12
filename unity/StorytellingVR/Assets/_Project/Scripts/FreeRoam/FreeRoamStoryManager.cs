using System.Collections;
using UnityEngine;

public class FreeRoamStoryManager : MonoBehaviour
{
    public static FreeRoamStoryManager Instance { get; private set; }

    public enum StoryStage
    {
        None,
        Intro,
        TeleportTutorial,
        TalkToLocal,
        TalkToForeigner,
        VisitTrinketStall,
        MeeraIntroduction,
        InspectTrinkets,
        InspectNotebook,
        FindWork,
        TalkToMerchant,
        FollowMerchant,
        EnterSpiceStall,
        Complete
    }

    [Header("Current Story State")]
    [SerializeField]
    private StoryStage currentStage = StoryStage.None;

    public StoryStage CurrentStage => currentStage;

    [Header("System References")]
    [SerializeField] private NarratorUIManager narrator;
    [SerializeField] private ObjectiveUIManager objectiveUI;
    [SerializeField] private TutorialPromptUIManager promptUI;
    [SerializeField] private TeleportManager teleportManager;
    [SerializeField] private NPCDirectionalIndicator directionalIndicator;
    [SerializeField]
    private MeeraInspectionSequenceController meeraInspectionSequenceController;

    [Header("Story Targets")]
    [SerializeField] private Transform localNPCTarget;
    [SerializeField] private GameObject localNPCWorldIndicator;

    [SerializeField] private Transform foreignNPCTarget;
    [SerializeField] private GameObject foreignNPCWorldIndicator;

    [SerializeField] private Transform trinketStallTarget;
    [SerializeField] private GameObject trinketStallWorldIndicator;

    [SerializeField] private Transform merchantTarget;
    [SerializeField] private GameObject merchantWorldIndicator;

    [Header("Story Actors")]
    [SerializeField] private NPCInteraction localNPCInteraction;
    [SerializeField] private NPCInteraction foreignNPCInteraction;
    [SerializeField] private NPCInteraction meeraNPCInteraction;    
    [SerializeField] private GameObject trinketSequenceRoot;

    [SerializeField] private SpiceMerchantGuideSequence merchantSequence;


    [Header("Startup")]
    [SerializeField] private bool playIntroOnStart = true;
    [SerializeField] private float startupDelay = 1f;

    private Coroutine activeSequence;
    private bool transitionInProgress;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "[FREE ROAM STORY] Duplicate manager found. " +
                "Destroying duplicate component."
            );

            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResolveMissingReferences();
        PrepareInitialState();

        if (playIntroOnStart)
            activeSequence = StartCoroutine(IntroSequence());
    }

    private void ResolveMissingReferences()
    {
        if (narrator == null)
            narrator = NarratorUIManager.Instance;

        if (objectiveUI == null)
            objectiveUI = ObjectiveUIManager.Instance;

        if (promptUI == null)
            promptUI = TutorialPromptUIManager.Instance;

        if (teleportManager == null)
            teleportManager = TeleportManager.Instance;

        if (directionalIndicator == null)
        {
            directionalIndicator =
                FindFirstObjectByType<NPCDirectionalIndicator>();
        }
    }

    private void PrepareInitialState()
    {
        SetStage(StoryStage.None);

        directionalIndicator?.Hide();

        if (trinketSequenceRoot != null)
            trinketSequenceRoot.SetActive(false);

        if (merchantSequence != null)
            merchantSequence.gameObject.SetActive(false);

        teleportManager?.DisableAll();
    }

    private IEnumerator IntroSequence()
    {
        transitionInProgress = true;

        yield return new WaitForSecondsRealtime(startupDelay);

        SetStage(StoryStage.Intro);

        objectiveUI?.SetObjective("Listen");

        yield return PlayNarration(
            "Narrator",
            "Welcome to Hampi Bazaar. Take a moment to observe the people around you.",
            5f
        );

        yield return new WaitForSecondsRealtime(0.5f);

        yield return PlayNarration(
            "Narrator",
            "This marketplace is alive with merchants, travelers, craftsmen, and pilgrims. For many here, this is simply another ordinary morning.",
            7f
        );

        SetStage(StoryStage.TeleportTutorial);

        promptUI?.ShowPrompt(
            "Teleport",
            "Use the RIGHT JOYSTICK to aim at a hotspot, then release it to teleport."
        );

        objectiveUI?.SetObjective(
            "Learn to move using teleport"
        );

        teleportManager?.EnableGroup("Tutorial");

        transitionInProgress = false;
        activeSequence = null;
    }

    public void NotifyTeleportTutorialCompleted()
    {
        if (!CanAdvanceFrom(StoryStage.TeleportTutorial))
            return;

        StartManagedSequence(
            TeleportTutorialCompletedSequence()
        );
    }

    private IEnumerator TeleportTutorialCompletedSequence()
    {
        teleportManager?.DisableAll();
        promptUI?.HidePrompt();

        objectiveUI?.CompleteObjective(
            "Learn to move using teleport"
        );

        yield return new WaitForSecondsRealtime(1.5f);

        SetStage(StoryStage.TalkToLocal);

        objectiveUI?.SetObjective(
            "Talk to the local resident"
        );

        yield return PlayNarration(
            "Narrator",
            "Good. Now that you can move through the bazaar, speak with someone nearby. A local resident may help you understand this place.",
            6f
        );

        SetIndicator(
            localNPCTarget,
            localNPCWorldIndicator
        );

        localNPCInteraction?.EnableIndicator();

        teleportManager?.EnableGroup("General");
        Debug.Log("[FREE ROAM STORY] TeleportTutorialCompletedSequence finished.");
    }

    public void NotifyLocalNPCCompleted()
    {
        if (!CanAdvanceFrom(StoryStage.TalkToLocal))
            return;

        StartManagedSequence(
            LocalNPCCompletedSequence()
        );
    }

    private IEnumerator LocalNPCCompletedSequence()
    {
        HideIndicator();

        SetStage(StoryStage.TalkToForeigner);

        objectiveUI?.SetObjective(
            "Talk to the foreign traveler"
        );

        yield return PlayNarration(
            "Narrator",
            "Hampi draws people from far beyond the empire. Speak with the traveler nearby and learn how far these trade routes reach.",
            6f
        );

        SetIndicator(
            foreignNPCTarget,
            foreignNPCWorldIndicator
        );

        foreignNPCInteraction?.EnableIndicator();

        teleportManager?.EnableGroup("General");

        Debug.Log(
            "[FREE ROAM STORY] Foreign NPC unlocked and General hotspots enabled."
        );
    }

    public void NotifyForeignNPCCompleted()
    {
        if (!CanAdvanceFrom(StoryStage.TalkToForeigner))
            return;

        StartManagedSequence(
            ForeignNPCCompletedSequence()
        );
    }

    private IEnumerator ForeignNPCCompletedSequence()
    {
        // Remove the foreign traveler's indicator immediately.
        HideIndicator();

        Debug.Log(
            "[FREE ROAM STORY] Foreign NPC completed. " +
            "Beginning trinket stall introduction."
        );

        // Let the foreigner's conversation visually settle.
        yield return new WaitForSecondsRealtime(0.8f);

        // Meera calls from somewhere nearby.
        // Her identity remains hidden for now.
        yield return PlayNarration(
            "???",
            "Curiosities from near and far! Fine trinkets, rare keepsakes—come, take a look!",
            5f
        );

        // Small pause between the distant call and narrator response.
        yield return new WaitForSecondsRealtime(0.6f);

        yield return PlayNarration(
            "Narrator",
            "That sounds like an interesting stall. Perhaps it is worth taking a closer look.",
            6f
        );

        /*
         * Only reveal the new task after the player has heard
         * both the mysterious call and the narrator's response.
         */

        SetStage(StoryStage.VisitTrinketStall);

        objectiveUI?.SetObjective(
            "Visit the nearby trinket stall"
        );



        teleportManager?.EnableGroup("TrinketPath");
        teleportManager?.EnableGroup("General");

        SetIndicator(
    trinketStallTarget,
    trinketStallWorldIndicator
);

        // Unlock X interaction on Meera.
        meeraNPCInteraction?.EnableIndicator();

        Debug.Log(
            "[FREE ROAM STORY] Trinket stall objective revealed. " +
            "Meera interaction, TrinketPath and General hotspots enabled."
        );
    }

    public void NotifyTrinketStallReached()
    {
        Debug.Log(
            "[FREE ROAM STORY] Trinket stall arrival trigger reached."
        );

        /*
         * Reaching the stall should not immediately start the
         * Meera introduction. The player must still press X.
         *
         * Therefore, for now, this method only confirms arrival
         * and hides the route-specific indicator if desired.
         */

        if (currentStage != StoryStage.VisitTrinketStall)
            return;

        Debug.Log(
            "[FREE ROAM STORY] Waiting for the player to interact with Meera."
        );
    }

    public void NotifyMeeraInteractionStarted()
    {
        if (!CanAdvanceFrom(StoryStage.VisitTrinketStall))
            return;

        SetStage(StoryStage.MeeraIntroduction);

        HideIndicator();

        meeraNPCInteraction?.DisableIndicator();

        teleportManager?.DisableAll();

        promptUI?.HidePrompt();

        objectiveUI?.SetObjective(
            "Listen to the stall owner"
        );

        Debug.Log(
            "[FREE ROAM STORY] Meera introduction started."
        );
    }

    public void NotifyMeeraIntroductionCompleted()
    {
        if (currentStage != StoryStage.MeeraIntroduction)
        {
            Debug.LogWarning(
                "[FREE ROAM STORY] Cannot complete Meera introduction. " +
                $"Current stage is {currentStage}."
            );

            return;
        }

        SetStage(StoryStage.InspectTrinkets);

        if (meeraInspectionSequenceController != null)
        {
            meeraInspectionSequenceController.BeginInspectionSequence();
        }
        else
        {
            Debug.LogError(
                "[FREE ROAM STORY] MeeraInspectionSequenceController is not assigned."
            );
        }

        objectiveUI?.SetObjective(
            "Inspect the objects on Meera's stall"
        );

        Debug.Log(
            "[FREE ROAM STORY] Meera introduction complete. " +
            "Inspection stage started."
        );
    }

    public void NotifyNotebookConversationCompleted()
    {
        Debug.Log(
            $"[MEERA HANDOFF] NotifyNotebookConversationCompleted entered. " +
            $"Object={gameObject.name}, active={gameObject.activeInHierarchy}, " +
            $"enabled={enabled}, stage={currentStage}, transitionRunning={transitionInProgress}"
        );

        if (currentStage != StoryStage.InspectTrinkets &&
            currentStage != StoryStage.MeeraIntroduction)
        {
            Debug.Log($"[MEERA HANDOFF] Aborted: expected InspectTrinkets or MeeraIntroduction but stage was {currentStage}.");
            return;
        }

        Debug.Log("[MEERA HANDOFF] Starting FindWorkSequence.");
        StartManagedSequence(
            FindWorkSequence()
        );
    }

    private IEnumerator FindWorkSequence()
    {
        Debug.Log("[FIND WORK] Sequence entered.");
        
        HideIndicator();

        Debug.Log("[FIND WORK] Setting stage to FindWork.");
        SetStage(StoryStage.FindWork);

        Debug.Log("[FIND WORK] Setting objective.");
        if (objectiveUI != null)
            objectiveUI.SetObjective("Find work in the bazaar");
        else
            Debug.LogWarning("[FIND WORK] objectiveUI is null!");

        Debug.Log("[FIND WORK] Starting narrator transition.");
        yield return PlayNarration(
            "Narrator",
            "Without coin, the answers you seek remain beyond reach. But a city this busy always has work for someone willing to earn their place.",
            7f
        );

        Debug.Log("[FIND WORK] Waiting 7 seconds completed.");

        Debug.Log("[FIND WORK] Activating merchant sequence object.");
        if (merchantSequence != null)
            merchantSequence.gameObject.SetActive(true);
        else
            Debug.LogWarning("[FIND WORK] merchantSequence is null!");

        Debug.Log("[FIND WORK] Enabling teleport groups.");
        if (teleportManager != null)
        {
            teleportManager.EnableGroup("General");
            teleportManager.EnableGroup("MerchantPath");
        }
        else
        {
            Debug.LogWarning("[FIND WORK] teleportManager is null!");
        }

        Debug.Log("[FIND WORK] Showing merchant indicator.");
        if (merchantTarget != null)
        {
            SetIndicator(
                merchantTarget,
                merchantWorldIndicator
            );
        }
        else
        {
            Debug.LogWarning("[FIND WORK] merchantTarget is null!");
        }

        Debug.Log("[FIND WORK] Sequence completed.");

        SetStage(StoryStage.TalkToMerchant);
    }

    public void NotifyMerchantConversationStarted()
    {
        if (currentStage != StoryStage.TalkToMerchant)
            return;

        HideIndicator();
    }

    public void NotifyMerchantStartedWalking()
    {
        SetStage(StoryStage.FollowMerchant);

        objectiveUI?.SetObjective(
            "Follow the spice merchant"
        );

        teleportManager?.EnableGroup("MerchantPath");
    }

    public void NotifyMerchantReachedStall()
    {
        SetStage(StoryStage.EnterSpiceStall);
        objectiveUI?.SetObjective(
            "Enter the spice stall"
        );

        teleportManager?.EnableGroup("StallEntry");
    }

    public void NotifySpiceStallEntered()
    {
        SetStage(StoryStage.Complete);

        HideIndicator();
        teleportManager?.DisableAll();
    }

    private void StartManagedSequence(IEnumerator sequence)
    {
        if (transitionInProgress)
        {
            Debug.LogWarning(
                "[FREE ROAM STORY] A story transition is already running."
            );

            return;
        }

        activeSequence = StartCoroutine(
            ManagedSequence(sequence)
        );
    }

    private IEnumerator ManagedSequence(IEnumerator sequence)
    {
        transitionInProgress = true;

        Debug.Log("[FREE ROAM STORY] Managed sequence START");

        yield return StartCoroutine(sequence);

        Debug.Log("[FREE ROAM STORY] Managed sequence END");

        transitionInProgress = false;

        Debug.Log("[FREE ROAM STORY] Transition = FALSE");

        activeSequence = null;
    }

    private IEnumerator PlayNarration(
        string speaker,
        string line,
        float duration)
    {
        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                speaker,
                line,
                duration
            );
        }
        else
        {
            Debug.Log($"[{speaker}] {line}");
            yield return new WaitForSecondsRealtime(duration);
        }
    }

    private void SetIndicator(
        Transform target,
        GameObject worldIndicator)
    {
        if (directionalIndicator == null ||
            target == null)
        {
            return;
        }

        directionalIndicator.SetTarget(
            target,
            worldIndicator
        );

        directionalIndicator.Show();
    }

    private void HideIndicator()
    {
        directionalIndicator?.Hide();
    }

    private bool CanAdvanceFrom(StoryStage requiredStage)
    {
        Debug.Log(
            $"[FREE ROAM STORY] CanAdvanceFrom({requiredStage}) " +
            $"Current={currentStage} " +
            $"Transition={transitionInProgress}"
        );

        if (transitionInProgress)
        {
            Debug.LogWarning(
                "[FREE ROAM STORY] Transition still running."
            );

            return false;
        }

        if (currentStage != requiredStage)
        {
            Debug.LogWarning(
                $"[FREE ROAM STORY] Wrong stage. Current={currentStage}"
            );

            return false;
        }

        return true;
    }

    private void SetStage(StoryStage newStage)
    {
        currentStage = newStage;

        Debug.Log(
            $"[FREE ROAM STORY] Stage changed to {newStage}."
        );
    }
}