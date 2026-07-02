using UnityEngine;
using UnityEngine.XR;

public class StallEntryTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject promptCanvas;

    private bool playerInside = false;
    private bool buttonHeld = false;

    private void Start()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

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

            Debug.Log("[STALL ENTRY] Entering stall through GameManager");

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

        if (promptCanvas != null)
            promptCanvas.SetActive(true);

        Debug.Log("[STALL ENTRY] Player near stall");
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