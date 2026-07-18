using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public NPCDialogueData dialogue;

    [Header("Question Canvas")]
    [Tooltip(
        "Assign the NPCQuestionCanvasView beside this specific NPC."
    )]
    [SerializeField]
    private NPCQuestionCanvasView questionCanvasView;

    [Header("Optional Prompt")]
    public GameObject talkPromptObject;

    [Header("Optional Legacy Movement Object")]
    public GameObject teleportSystem;

    [Header("Progression")]
    [SerializeField]
    private bool availableOnStart;

    [Header("Shared Directional Indicator")]
    [SerializeField]
    private NPCDirectionalIndicator indicator;

    [Tooltip(
        "The position that the shared indicator should point toward " +
        "for this NPC."
    )]
    [SerializeField]
    private Transform indicatorTarget;

    [Tooltip("The world-space particle object above this NPC.")]
    [SerializeField]
    private GameObject worldIndicator;

    [Header("Next NPCs")]
    [SerializeField]
    private NPCInteraction[] nextNPCsToUnlock;

    private bool closingDialoguePlaying;
    private bool playerNearby;
    private bool inConversation;
    private bool conversationCompleted;
    private bool buttonHeld;
    private bool interactionUnlocked;

    private Coroutine conversationRoutine;
    private Coroutine closingRoutine;

    private void Start()
    {
        interactionUnlocked = availableOnStart;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (questionCanvasView != null)
            questionCanvasView.SetVisible(false);

        // Do not hide the shared indicator here.
        // Another NPC may own it.
        //
        // Only show it here when this NPC is intentionally
        // available when the scene starts.
        if (interactionUnlocked)
            ShowIndicatorForThisNPC();
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
                NPCQuestionUIManager questionManager =
                    NPCQuestionUIManager.Instance;

                // X ends the conversation only while the
                // question selection canvas is visibly open.
                //
                // X does nothing during:
                // - opening dialogue
                // - an NPC answer
                // - closing dialogue
                if (!closingDialoguePlaying &&
                    questionManager != null &&
                    questionManager.IsOpen &&
                    !questionManager.IsAnswerPlaying)
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
        if (dialogue == null)
        {
            Debug.LogError(
                $"[NPC] {gameObject.name} has no dialogue assigned."
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

        if (NarratorUIManager.Instance == null ||
            NPCQuestionUIManager.Instance == null)
        {
            Debug.LogError(
                "[NPC] Cannot start conversation because a " +
                "dialogue manager is missing."
            );

            return;
        }

        inConversation = true;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance
                .SetAllTeleportEnabled(false);
        }

        if (teleportSystem != null)
            teleportSystem.SetActive(false);

        // Hide the one shared indicator when interaction begins.
        if (indicator != null)
            indicator.Hide();

        SetLaserPointerEnabled(false);

        conversationRoutine =
            StartCoroutine(ConversationFlowRoutine());

        Debug.Log(
            $"[NPC] Conversation started with {gameObject.name}."
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

        ShowIndicatorForThisNPC();

        if (playerNearby &&
            !inConversation &&
            talkPromptObject != null)
        {
            talkPromptObject.SetActive(true);
        }

        Debug.Log(
            $"[NPC] {gameObject.name} unlocked."
        );
    }

    public void DisableIndicator()
    {
        if (indicator != null)
            indicator.Hide();
    }

    private void ShowIndicatorForThisNPC()
    {
        if (indicator == null)
        {
            Debug.LogWarning(
                $"[NPC] {gameObject.name} has no shared " +
                "NPCDirectionalIndicator assigned."
            );

            return;
        }

        if (indicatorTarget == null)
        {
            Debug.LogWarning(
                $"[NPC] {gameObject.name} has no indicator target assigned."
            );

            return;
        }

        indicator.SetTarget(
            indicatorTarget,
            worldIndicator
        );

        indicator.Show();
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
        if (!inConversation ||
            closingDialoguePlaying)
        {
            return;
        }

        NPCQuestionUIManager manager =
            NPCQuestionUIManager.Instance;

        // Safety check: never interrupt an answer.
        if (manager != null &&
            manager.IsAnswerPlaying)
        {
            return;
        }

        closingDialoguePlaying = true;

        if (manager != null)
            manager.Close();

        SetLaserPointerEnabled(false);

        closingRoutine =
            StartCoroutine(ClosingConversationRoutine());

        Debug.Log(
            $"[NPC] Closing dialogue started for {gameObject.name}."
        );
    }

    public void OnAllQuestionsAsked()
    {
        BeginClosingDialogue();
    }

    private IEnumerator ClosingConversationRoutine()
    {
        SetLaserPointerEnabled(false);

        if (!string.IsNullOrWhiteSpace(
                dialogue.closingDialogue))
        {
            yield return NarratorUIManager.Instance
                .PlayNarrationLineByLine(
                    dialogue.npcName,
                    dialogue.closingDialogue
                );
        }

        closingRoutine = null;

        CompleteConversation();
    }

    private void CompleteConversation()
    {
        inConversation = false;
        conversationCompleted = true;
        closingDialoguePlaying = false;
        interactionUnlocked = false;

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

        if (questionCanvasView != null)
            questionCanvasView.SetVisible(false);

        SetLaserPointerEnabled(false);

        if (teleportSystem != null)
            teleportSystem.SetActive(true);

        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance
                .SetGeneralHotspotsEnabled(true);
        }

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        if (nextNPCsToUnlock != null)
        {
            foreach (
                NPCInteraction nextNPC
                in nextNPCsToUnlock)
            {
                if (nextNPC != null)
                    nextNPC.EnableIndicator();
            }
        }

        Debug.Log(
            "[NPC] Conversation fully completed. " +
            "Next NPC unlocked."
        );
    }
}