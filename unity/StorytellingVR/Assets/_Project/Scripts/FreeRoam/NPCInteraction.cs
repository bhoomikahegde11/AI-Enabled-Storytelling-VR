using UnityEngine;
using UnityEngine.XR;

public class NPCInteraction : MonoBehaviour
{
    public NPCDialogueData dialogue;
    public GameObject prompt;

    private bool playerNearby;
    private bool inConversation;
    private bool pressed;

    private void Start()
    {
        Debug.Log("[NPC] Script active on " + gameObject.name);

        if (prompt != null)
            prompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby && !inConversation)
            return;

        bool xPressed = GetXPressed();

        if (xPressed && !pressed)
        {
            pressed = true;

            if (!inConversation)
            {
                StartConversation();
            }
            else
            {
                EndConversation();
            }
        }

        if (!xPressed)
            pressed = false;
    }

    private bool GetXPressed()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool xPressed = false;

        leftHand.TryGetFeatureValue(
            CommonUsages.secondaryButton,
            out xPressed
        );

        return xPressed;
    }

    private void StartConversation()
    {
        inConversation = true;

        if (prompt != null)
            prompt.SetActive(false);

        NarratorUIManager.Instance.ShowNarration(
            dialogue.npcName,
            dialogue.openingDialogue,
            5f
        );

        NPCQuestionUIManager.Instance.Open(dialogue);

        Debug.Log("[NPC] Conversation started");
    }

    private void EndConversation()
    {
        inConversation = false;

        NPCQuestionUIManager.Instance.Close();

        if (playerNearby && prompt != null)
            prompt.SetActive(true);

        Debug.Log("[NPC] Conversation ended");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[NPC] Something entered trigger: " + other.name);

        playerNearby = true;

        if (!inConversation && prompt != null)
            prompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("[NPC] Something exited trigger: " + other.name);

        playerNearby = false;

        if (prompt != null)
            prompt.SetActive(false);
    }
}