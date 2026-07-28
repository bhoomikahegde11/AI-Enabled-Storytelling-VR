using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class NPCInteraction : MonoBehaviour
{
    public enum StoryNPCType
    {
        None,
        LocalResident,
        ForeignTraveller,
        Meera,
        Bhaskara
    }

    [Header("Dialogue")]
    public NPCDialogueData dialogue;

    [Header("Question Canvas")]
    [SerializeField]
    private NPCQuestionCanvasView questionCanvasView;

    [Header("Story Identity")]
    [SerializeField]
    private StoryNPCType storyNPCType = StoryNPCType.None;

    [Header("Optional Prompt")]
    public GameObject talkPromptObject;

    [Header("Optional Legacy Movement Object")]
    public GameObject teleportSystem;

    [Header("Progression")]
    [SerializeField]
    private bool availableOnStart;

    [Header("Indicator")]
    [SerializeField]
    private NPCDirectionalIndicator indicator;

    [Header("Optional Dedicated Sequence")]
    [SerializeField]
    private MeeraSequenceController meeraSequenceController;

    [SerializeField]
    private NPCSubtitlePositionController subtitlePositionController;

    [Header("Legacy Next NPCs")]
    [Tooltip(
        "Temporary legacy field. Leave empty when using FreeRoamStoryManager."
    )]
    [SerializeField]
    private NPCInteraction[] nextNPCsToUnlock;

    private bool closingDialoguePlaying;
    private bool playerNearby;
    private bool inConversation;
    private bool conversationCompleted;
    private bool buttonHeld;
    private bool interactionUnlocked;

    private Coroutine conversationRoutine;

    private void Start()
    {
        interactionUnlocked = availableOnStart;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (indicator != null)
        {
            if (interactionUnlocked)
                indicator.Show();
            else
                indicator.Hide();
        }

        if (questionCanvasView != null)
            questionCanvasView.SetVisible(false);
    }

    private void Update()
    {
        if (!playerNearby && !inConversation)
            return;

        bool pressed = GetInteractButton();

        if (pressed && !buttonHeld)
        {
            buttonHeld = true;

            if (!inConversation)
            {
                if (interactionUnlocked &&
                    !conversationCompleted)
                {
                    StartConversation();
                }
            }
            else
            {
                if (!closingDialoguePlaying &&
                    NPCQuestionUIManager.Instance != null &&
                    NPCQuestionUIManager.Instance.IsOpen)
                {
                    BeginClosingDialogue();
                }
            }
        }

        if (!pressed)
            buttonHeld = false;
    }

    private void StartConversation()
    {
        if (storyNPCType == StoryNPCType.Meera &&
            meeraSequenceController != null)
        {
            StartDedicatedMeeraSequence();
            return;
        }

        if (dialogue == null ||
            NarratorUIManager.Instance == null ||
            NPCQuestionUIManager.Instance == null)

            if (dialogue == null ||
            NarratorUIManager.Instance == null ||
            NPCQuestionUIManager.Instance == null)
        {
            Debug.LogError(
                "[NPC] Cannot start conversation. " +
                "A dialogue or manager reference is missing."
            );

            return;
        }

        if (questionCanvasView == null)
        {
            Debug.LogError(
                $"[NPC] {gameObject.name} has no " +
                "NPCQuestionCanvasView assigned."
            );

            return;
        }

        inConversation = true;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (TeleportManager.Instance != null)
            TeleportManager.Instance.DisableAll();

        if (teleportSystem != null)
            teleportSystem.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        SetLaserPointerEnabled(false);

        conversationRoutine =
            StartCoroutine(ConversationFlowRoutine());

        Debug.Log(
            $"[NPC] Conversation started for {storyNPCType}."
        );
    }

    private void StartDedicatedMeeraSequence()
    {
        if (meeraSequenceController == null)
        {
            Debug.LogError(
                "[NPC] Meera has no MeeraSequenceController assigned."
            );

            return;
        }

        inConversation = true;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (TeleportManager.Instance != null)
            TeleportManager.Instance.DisableAll();

        if (teleportSystem != null)
            teleportSystem.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        SetLaserPointerEnabled(false);

        meeraSequenceController.BeginSequence();

        Debug.Log(
            "[NPC] Dedicated Meera sequence requested."
        );
    }

    private IEnumerator ConversationFlowRoutine()
    {
        yield return NarratorUIManager.Instance
            .PlayNarrationLineByLine(
                dialogue.npcName,
                dialogue.openingDialogue
            );

        conversationRoutine = null;

        if (!inConversation)
            yield break;

        NPCQuestionUIManager.Instance.Open(
            dialogue,
            this,
            questionCanvasView
        );

        Debug.Log(
            "[NPC] Opening dialogue complete; questions opened."
        );
    }

    private bool GetInteractButton()
    {
        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool pressed = false;

        if (leftHand.isValid)
        {
            leftHand.TryGetFeatureValue(
                CommonUsages.primaryButton,
                out pressed
            );
        }

#if UNITY_EDITOR
        pressed |= Input.GetKey(KeyCode.X);
#endif

        return pressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerNearby = true;

        if (interactionUnlocked &&
            !inConversation &&
            !conversationCompleted &&
            talkPromptObject != null)
        {
            talkPromptObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerNearby = false;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        if (other.GetComponentInParent<CharacterController>() != null)
            return true;

        if (other.name.Contains("PlayerController"))
            return true;

        Transform root = other.transform.root;

        return root != null &&
               root.name.Contains("PlayerController");
    }

    public void EnableIndicator()
    {
        if (conversationCompleted)
            return;

        interactionUnlocked = true;

        if (indicator != null)
            indicator.Show();

        if (playerNearby &&
            !inConversation &&
            talkPromptObject != null)
        {
            talkPromptObject.SetActive(true);
        }

        Debug.Log(
            $"[NPC] {storyNPCType} unlocked."
        );
    }

    public void DisableIndicator()
    {
        if (indicator != null)
            indicator.Hide();
    }

    private void SetLaserPointerEnabled(bool enabled)
    {
        NPCDialogueVRLaserPointer laser =
            Object.FindAnyObjectByType<NPCDialogueVRLaserPointer>(
                FindObjectsInactive.Include
            );

        if (laser != null)
            laser.enabled = enabled;
    }

    private void BeginClosingDialogue()
    {
        if (!inConversation || closingDialoguePlaying)
            return;

        closingDialoguePlaying = true;

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

        SetLaserPointerEnabled(false);

        StartCoroutine(ClosingConversationRoutine());

        Debug.Log("[NPC] Closing dialogue started.");
    }

    public void OnAllQuestionsAsked()
    {
        BeginClosingDialogue();
    }

    private IEnumerator ClosingConversationRoutine()
    {
        SetLaserPointerEnabled(false);

        if (!string.IsNullOrWhiteSpace(dialogue.closingDialogue))
        {
            yield return NarratorUIManager.Instance
                .PlayNarrationLineByLine(
                    dialogue.npcName,
                    dialogue.closingDialogue
                );
        }

        CompleteConversation();
    }

    private void CompleteConversation()
    {
        inConversation = false;
        conversationCompleted = true;
        closingDialoguePlaying = false;

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

        SetLaserPointerEnabled(false);

        if (teleportSystem != null)
            teleportSystem.SetActive(true);

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        NotifyStoryManager();

        Debug.Log(
            $"[NPC] Conversation fully completed for {storyNPCType}."
        );
    }

    private void NotifyStoryManager()
    {
        Debug.Log(
            $"[NPC STORY] NotifyStoryManager called. " +
            $"NPC type: {storyNPCType}"
        );

        FreeRoamStoryManager storyManager =
            FreeRoamStoryManager.Instance;

        if (storyManager == null)
        {
            Debug.LogError(
                "[NPC STORY] FreeRoamStoryManager.Instance is null."
            );

            EnableLegacyNextNPCs();
            RestoreGeneralTeleport();
            return;
        }

        Debug.Log(
            $"[NPC STORY] Current story stage before notification: " +
            $"{storyManager.CurrentStage}"
        );

        switch (storyNPCType)
        {
            case StoryNPCType.LocalResident:
                Debug.Log(
                    "[NPC STORY] Sending Local NPC completed event."
                );

                storyManager.NotifyLocalNPCCompleted();
                break;

            case StoryNPCType.ForeignTraveller:
                Debug.Log(
                    "[NPC STORY] Sending Foreign Traveller completed event."
                );

                storyManager.NotifyForeignNPCCompleted();
                break;

            case StoryNPCType.Meera:
                Debug.Log(
                    "[NPC STORY] Sending Meera completed event."
                );

                storyManager.NotifyNotebookConversationCompleted();
                break;

            case StoryNPCType.Bhaskara:
                Debug.Log(
                    "[NPC STORY] Sending Bhaskara interaction event."
                );

                storyManager.NotifyMerchantConversationStarted();
                break;

            default:
                Debug.LogWarning(
                    $"[NPC STORY] '{gameObject.name}' has Story NPC Type None."
                );

                EnableLegacyNextNPCs();
                RestoreGeneralTeleport();
                break;
        }
    }

    private void EnableLegacyNextNPCs()
    {
        if (nextNPCsToUnlock == null)
            return;

        foreach (NPCInteraction nextNPC in nextNPCsToUnlock)
        {
            if (nextNPC != null)
                nextNPC.EnableIndicator();
        }
    }

    private void RestoreGeneralTeleport()
    {
        if (TeleportManager.Instance != null)
            TeleportManager.Instance.EnableGroup("General");
    }
}