using System.Collections;
using UnityEngine;

public class MeeraSequenceController : MonoBehaviour
{
    public enum MeeraSequenceState
    {
        Inactive,
        Introduction,
        WaitingForInspection,
        Complete
    }

    [Header("Current State")]
    [SerializeField]
    private MeeraSequenceState currentState =
        MeeraSequenceState.Inactive;

    public MeeraSequenceState CurrentState => currentState;

    [Header("References")]
    [SerializeField]
    private NPCInteraction meeraInteraction;

    [SerializeField]
    private Animator meeraAnimator;

    [SerializeField]
    private GameObject sequenceObjectsRoot;

    [Header("Introduction Dialogue")]
    [SerializeField]
    private string speakerName = "Meera";

    [TextArea(2, 5)]
    [SerializeField]
    private string greetingLine =
        "Ah, so my call caught your attention. Come closer, traveller. There may be something here that interests you.";

    [TextArea(2, 5)]
    [SerializeField]
    private string inspectionInvitation =
        "Take your time and look around. Every object on this stall has a story of its own.";

    [Header("Timing")]
    [SerializeField]
    private float pauseBetweenLines = 0.5f;

    private Coroutine activeRoutine;
    private bool sequenceStarted;

    private void Awake()
    {
        if (meeraInteraction == null)
        {
            meeraInteraction =
                GetComponent<NPCInteraction>();
        }
    }

    public void BeginSequence()
    {
        if (sequenceStarted)
        {
            Debug.LogWarning(
                "[MEERA SEQUENCE] Sequence has already started."
            );

            return;
        }

        FreeRoamStoryManager storyManager =
            FreeRoamStoryManager.Instance;

        if (storyManager == null)
        {
            Debug.LogError(
                "[MEERA SEQUENCE] FreeRoamStoryManager.Instance is missing."
            );

            return;
        }

        if (storyManager.CurrentStage !=
            FreeRoamStoryManager.StoryStage.VisitTrinketStall)
        {
            Debug.LogWarning(
                "[MEERA SEQUENCE] Cannot begin Meera sequence. " +
                $"Current story stage is {storyManager.CurrentStage}."
            );

            return;
        }

        sequenceStarted = true;

        activeRoutine = StartCoroutine(
            IntroductionRoutine()
        );
    }

    private IEnumerator IntroductionRoutine()
    {
        currentState = MeeraSequenceState.Introduction;

        Debug.Log(
            "[MEERA SEQUENCE] Introduction started."
        );

        FreeRoamStoryManager storyManager =
            FreeRoamStoryManager.Instance;

        storyManager.NotifyMeeraInteractionStarted();

        if (TeleportManager.Instance != null)
            TeleportManager.Instance.DisableAll();

        if (sequenceObjectsRoot != null)
            sequenceObjectsRoot.SetActive(true);

        PlayGreetingAnimation();

        NarratorUIManager narrator =
            NarratorUIManager.Instance;

        if (narrator != null)
        {
            yield return narrator.PlayNarration(
                speakerName,
                greetingLine
            );

            yield return new WaitForSecondsRealtime(
                pauseBetweenLines
            );

            yield return narrator.PlayNarration(
                speakerName,
                inspectionInvitation
            );
        }
        else
        {
            Debug.Log($"[{speakerName}] {greetingLine}");

            yield return new WaitForSecondsRealtime(4f);

            Debug.Log(
                $"[{speakerName}] {inspectionInvitation}"
            );

            yield return new WaitForSecondsRealtime(4f);
        }

        currentState =
            MeeraSequenceState.WaitingForInspection;

        storyManager.NotifyMeeraIntroductionCompleted();

        activeRoutine = null;

        Debug.Log(
            "[MEERA SEQUENCE] Introduction complete. " +
            "Waiting for object inspection."
        );
    }

    private void PlayGreetingAnimation()
    {
        if (meeraAnimator == null)
            return;

        /*
         * Add a Trigger parameter named "Greet" to the
         * Animator Controller when the greeting animation
         * is added.
         */

        meeraAnimator.SetTrigger("Greet");
    }

    public void MarkSequenceComplete()
    {
        currentState = MeeraSequenceState.Complete;

        Debug.Log(
            "[MEERA SEQUENCE] Sequence marked complete."
        );
    }
}