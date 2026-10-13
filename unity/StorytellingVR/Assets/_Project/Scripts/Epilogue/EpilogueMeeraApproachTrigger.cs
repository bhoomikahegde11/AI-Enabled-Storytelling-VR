using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Handles both the Meera approach (fires once on trigger enter) and
/// the subsequent X-button purchase interaction — all from one collider zone.
/// Uses the same IsPlayer detection pattern as NPCInteraction / StallEntryTrigger.
/// </summary>
public class EpilogueMeeraApproachTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private bool hasApproached = false;
    private bool purchaseEnabled = false;
    private bool buttonHeld = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) 
        {
            Debug.Log($"[EPILOGUE MEERA] Ignored collider: {other.name} tag={other.tag}");
            return;
        }

        playerInside = true;
        Debug.Log("[EPILOGUE MEERA] Player entered approach zone");

        if (!hasApproached)
        {
            hasApproached = true;
            Debug.Log("[EPILOGUE MEERA] Approach accepted");
            EpilogueStoryManager.Instance?.NotifyMeeraApproached();
        }
        else if (purchaseEnabled)
        {
            Debug.Log("[EPILOGUE MEERA] Purchase interaction enabled (re-enter)");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;
    }

    private void Update()
    {
        if (!playerInside) return;
        if (!purchaseEnabled) return;
        if (PauseMenuController.GameplayInputBlocked) return;

        bool pressed = GetInteractButton();

        if (pressed && !buttonHeld)
        {
            buttonHeld = true;
            Debug.Log("[EPILOGUE MEERA] X pressed while in range");

            if (EpilogueStoryManager.Instance != null &&
                EpilogueStoryManager.Instance.currentStage == EpilogueStoryManager.EpilogueStage.PurchaseBook)
            {
                Debug.Log("[EPILOGUE MEERA] Purchase interaction accepted");
                EpilogueStoryManager.Instance.NotifyMeeraInteracted();
            }
        }

        if (!pressed)
            buttonHeld = false;
    }

    /// <summary>
    /// Called by EpilogueStoryManager after the approach dialogue finishes,
    /// to allow the player to press X for the purchase.
    /// </summary>
    public void EnablePurchaseInput()
    {
        purchaseEnabled = true;
        Debug.Log("[EPILOGUE MEERA] Purchase interaction enabled");
    }

    private bool GetInteractButton()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool pressed = false;
        if (leftHand.isValid)
            leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);
#if UNITY_EDITOR
        pressed |= Input.GetKey(KeyCode.X);
#endif
        return pressed;
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;

        // Exact same chain used by NPCInteraction and StallEntryTrigger.
        if (other.CompareTag("Player")) return true;
        if (other.GetComponentInParent<CharacterController>() != null) return true;
        if (other.name.Contains("PlayerController")) return true;

        Transform root = other.transform.root;
        return root != null && root.name.Contains("PlayerController");
    }
}
