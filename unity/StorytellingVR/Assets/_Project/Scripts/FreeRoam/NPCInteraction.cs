using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public NPCDialogueData dialogue;

    [Header("Optional Prompt")]
    public GameObject talkPromptObject;

    [Header("Optional Legacy Movement Object")]
    public GameObject teleportSystem;

    [Header("Indicator")]
    [SerializeField] private NPCIndicator indicator;
    
    [Header("Next NPCs")]
    [SerializeField] private NPCInteraction[] nextNPCsToUnlock;

    private bool closingDialoguePlaying;
    private bool playerNearby;
    private bool inConversation;
    private bool conversationCompleted;
    private bool buttonHeld;

    private Coroutine conversationRoutine;

    private void Start()
    {
        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (indicator != null)
            indicator.Hide();
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
                if (!conversationCompleted)
                    StartConversation();
            }
            else
            {
                // Ignore X while opening dialogue or an answer is playing.
                // Exit only while the question canvas is visible.
                if (NPCQuestionUIManager.Instance != null &&
                    NPCQuestionUIManager.Instance.IsOpen)
                {
                    EndConversation();
                }
            }
        }

        if (!pressed)
            buttonHeld = false;
    }

    private void StartConversation()
    {
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

        inConversation = true;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (TeleportLockController.Instance != null)
            TeleportLockController.Instance
                .SetAllTeleportEnabled(false);

        if (teleportSystem != null)
            teleportSystem.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        SetLaserPointerEnabled(false);

        conversationRoutine =
            StartCoroutine(ConversationFlowRoutine());

        Debug.Log("[NPC] Conversation started.");
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

        NPCQuestionUIManager.Instance.Open(dialogue, this);

        Debug.Log(
            "[NPC] Opening dialogue complete; questions opened."
        );
    }

    private void EndConversation()
    {
        if (!inConversation)
            return;

        inConversation = false;
        conversationCompleted = true;

        if (conversationRoutine != null)
        {
            StopCoroutine(conversationRoutine);
            conversationRoutine = null;
        }

        if (NPCQuestionUIManager.Instance != null)
            NPCQuestionUIManager.Instance.Close();

        NarratorUIManager.Instance?.HideNarrator();
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

        Debug.Log("[NPC] Conversation ended.");
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

        if (!inConversation &&
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
        if (indicator != null && !conversationCompleted)
            indicator.Show();
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
    public void OnAllQuestionsAsked()
    {
        if (!inConversation || closingDialoguePlaying)
            return;

        closingDialoguePlaying = true;

        StartCoroutine(ClosingConversationRoutine());
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

        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance
                .SetGeneralHotspotsEnabled(true);
        }

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        if (indicator != null)
            indicator.Hide();

        foreach (NPCInteraction nextNPC in nextNPCsToUnlock)
        {
            if (nextNPC != null)
                nextNPC.EnableIndicator();
        }

        Debug.Log(
            "[NPC] Conversation fully completed. " +
            "General teleport restored and next NPCs unlocked."
        );
    }
}