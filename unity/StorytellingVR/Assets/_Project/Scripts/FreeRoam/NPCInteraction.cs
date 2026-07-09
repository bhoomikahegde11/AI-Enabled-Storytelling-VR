using UnityEngine;
using UnityEngine.XR;
using System.Collections;
public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public NPCDialogueData dialogue;

    [Header("Optional Prompt")]
    public GameObject talkPromptObject; // optional "Press X to talk" object
    
    [Header("Movement")]
    public GameObject teleportSystem;
    
    [Header("Indicator")]
    [SerializeField] private NPCIndicator indicator;

    private bool playerNearby = false;
    private bool inConversation = false;
    private bool buttonHeld = false;

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
                StartConversation();
            else
                EndConversation();
        }

        if (!pressed)
            buttonHeld = false;
    }
    private IEnumerator OpenQuestionsAfterDelay()
    {
        yield return new WaitForSeconds(5.2f);
        NPCQuestionUIManager.Instance.Open(dialogue);
    }
    private void StartConversation()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("[NPC] Dialogue data missing.");
            return;
        }

        inConversation = true;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);

        NarratorUIManager.Instance.ShowNarration(
            dialogue.npcName,
            dialogue.openingDialogue,
            5f
        );

        if (teleportSystem != null)
            teleportSystem.SetActive(false);\

        if (indicator != null)
            indicator.Hide();

        StartCoroutine(OpenQuestionsAfterDelay());

        Debug.Log("[NPC] Conversation started");
    }

    private void EndConversation()
    {
        inConversation = false;

        NPCQuestionUIManager.Instance.Close();

        if (playerNearby && talkPromptObject != null)
            talkPromptObject.SetActive(true);
        
        if (teleportSystem != null)
            teleportSystem.SetActive(true);
        Debug.Log("[NPC] Conversation ended");
    }

    private bool GetInteractButton()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool pressed = false;

        // Your logs showed left primaryButton works
        leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);

        return pressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[NPC] Entered by: " + other.name);

        playerNearby = true;

        if (!inConversation && talkPromptObject != null)
            talkPromptObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("[NPC] Exited by: " + other.name);

        playerNearby = false;

        if (talkPromptObject != null)
            talkPromptObject.SetActive(false);
    }
    public void EnableIndicator()
    {
        if (indicator != null)
            indicator.Show();
    }

    public void DisableIndicator()
    {
        if (indicator != null)
            indicator.Hide();
    }
}