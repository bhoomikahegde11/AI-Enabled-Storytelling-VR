using UnityEngine;
using UnityEngine.XR;

public class StallEntryTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptCanvas;

    [Header("Merchant")]
    public SpiceMerchantGuideSequence merchantSequence;

    private bool playerInside = false;
    private bool buttonHeld = false;

    private bool arrivalDialogueStarted = false;
    private bool arrivalDialogueFinished = false;

    private void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        // Player cannot enter until merchant finishes speaking
        if (!arrivalDialogueFinished)
            return;

        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool xPressed = false;

        leftHand.TryGetFeatureValue(
            CommonUsages.secondaryButton,
            out xPressed
        );

        if (xPressed && !buttonHeld)
        {
            buttonHeld = true;

            if (promptCanvas != null)
                promptCanvas.SetActive(false);

            Debug.Log(
                "[STALL ENTRY] Entering stall through GameManager"
            );

            GameManager.Instance.LoadNextScene();
        }

        if (!xPressed)
        {
            buttonHeld = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = true;

        Debug.Log("[STALL ENTRY] Player near stall");

        // Do not show Press X yet
        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        // Play merchant dialogue once
        if (!arrivalDialogueStarted)
        {
            arrivalDialogueStarted = true;

            if (merchantSequence != null)
            {
                Debug.Log(
                    "[STALL ENTRY] Starting merchant arrival dialogue"
                );

                merchantSequence.PlayStallArrivalDialogue(
                    OnArrivalDialogueFinished
                );
            }
            else
            {
                Debug.LogWarning(
                    "[STALL ENTRY] Merchant Sequence is not assigned!"
                );

                OnArrivalDialogueFinished();
            }
        }
        else if (arrivalDialogueFinished)
        {
            ShowEntryPrompt();
        }
    }

    private void OnArrivalDialogueFinished()
    {
        arrivalDialogueFinished = true;

        Debug.Log(
            "[STALL ENTRY] Merchant arrival dialogue finished"
        );

        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(
                "Enter the spice stall"
            );
        }

        if (playerInside)
            ShowEntryPrompt();
    }

    private void ShowEntryPrompt()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        Debug.Log("[STALL ENTRY] Player left stall");
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player")
            || other.transform.root.CompareTag("Player")
            || other.GetComponentInParent<CharacterController>() != null;
    }
}