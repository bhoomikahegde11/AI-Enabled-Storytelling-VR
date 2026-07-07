using UnityEngine;
using UnityEngine.XR;

public class NPCInteraction : MonoBehaviour
{
    public NPCDialogueData dialogue;

    public GameObject prompt;

    bool playerNearby;
    bool pressed;

    private void Start()
    {
        prompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby)
            return;

        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool xPressed;

        leftHand.TryGetFeatureValue(
            CommonUsages.secondaryButton,
            out xPressed);

        if (xPressed && !pressed)
        {
            pressed = true;

            prompt.SetActive(false);

            NarratorUIManager.Instance.ShowNarration(
                dialogue.npcName,
                dialogue.openingDialogue,
                5f);

            NPCQuestionUIManager.Instance.Open(dialogue);
        }

        if (!xPressed)
            pressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        playerNearby = true;

        prompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        playerNearby = false;

        prompt.SetActive(false);
    }
}