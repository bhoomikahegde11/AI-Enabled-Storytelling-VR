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

    [Header("Conversation Access")]
    [SerializeField]
    private bool conversationUnlocked = true;

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
    private bool questionConversationStarted;

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
                    conversationUnlocked &&
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

    /// <summary>
    /// Called programmatically to unlock the conversation gate
    /// and immediately start the standard conversation flow.
    /// Used by MeeraInspectionSequenceController after inspection.
    /// </summary>
    public void UnlockAndStartConversation()
    {
        if (conversationCompleted)
        {
            Debug.Log(
                "[NPC] UnlockAndStartConversation skipped; " +
                "conversation already completed."
            );

            return;
        }

        if (questionConversationStarted)
        {
            Debug.Log(
                "[NPC] UnlockAndStartConversation skipped; " +
                "question conversation already started."
            );

            return;
        }

        questionConversationStarted = true;
        conversationUnlocked = true;

        StartQuestionConversation();
    }

    /// <summary>
    /// Starts the standard conversation flow (opening dialogue
    /// then question canvas) without going through the dedicated
    /// Meera sequence, even if this NPC is of type Meera.
    /// </summary>
    private void StartQuestionConversation()
    {
        if (dialogue == null ||
            NPCQuestionUIManager.Instance == null)
        {
            Debug.LogError(
                "[NPC] Cannot start question conversation. " +
                "Dialogue or manager reference is missing."
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

        if (indicator != null)
            indicator.Hide();

        if (string.IsNullOrWhiteSpace(dialogue.openingDialogue) ||
            NarratorUIManager.Instance == null)
        {
            NPCQuestionUIManager.Instance.Open(
                dialogue,
                this,
                questionCanvasView
            );

            Debug.Log(
                "[NPC] Question conversation started (no opening dialogue)."
            );
        }
        else
        {
            conversationRoutine =
                StartCoroutine(ConversationFlowRoutine());

            Debug.Log(
                "[NPC] Question conversation started (with opening dialogue)."
            );
        }
    }

    public void DisableIndicator()
    {
        if (indicator != null)
            indicator.Hide();
    }



    private void BeginClosingDialogue()
    {
        if (!inConversation || closingDialoguePlaying)
            return;

        closingDialoguePlaying = true;

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

        StartCoroutine(ClosingConversationRoutine());

        Debug.Log("[NPC] Closing dialogue started.");
    }

    public void OnAllQuestionsAsked()
    {
        BeginClosingDialogue();
    }

    private IEnumerator ClosingConversationRoutine()
    {

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

    /// <summary>
    /// Safely completes a special out-of-bounds conversation (like Meera's 
    /// notebook sequence) using the standard internal cleanup logic.
    /// </summary>
    public void CompleteSpecialConversation()
    {
        Debug.Log($"[NPC] Special conversation completion triggered for {gameObject.name}.");
        CompleteConversation();
    }

    private void CompleteConversation()
    {
        inConversation = false;
        conversationCompleted = true;
        closingDialoguePlaying = false;

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

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