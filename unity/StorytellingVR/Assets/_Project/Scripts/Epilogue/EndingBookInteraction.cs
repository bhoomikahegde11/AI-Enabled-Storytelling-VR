using UnityEngine;
using UnityEngine.XR;

public class EndingBookInteraction : MonoBehaviour
{
    private bool playerNearby = false;
    private bool buttonHeld = false;
    
    private void Update()
    {
        if (!playerNearby) return;

        bool pressed = GetInteractButton();
        if (pressed && !buttonHeld)
        {
            buttonHeld = true;
            TryInspect();
        }
        if (!pressed) buttonHeld = false;
    }
    
    public void TryInspect()
    {
        if (EpilogueStoryManager.Instance != null && EpilogueStoryManager.Instance.currentStage == EpilogueStoryManager.EpilogueStage.OpenBook)
        {
            EpilogueStoryManager.Instance.NotifyBookInteracted();
        }
    }

    private bool GetInteractButton()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool pressed = false;
        if (leftHand.isValid) leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);
        #if UNITY_EDITOR
        pressed |= Input.GetKey(KeyCode.X);
        #endif
        return pressed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other)) playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other)) playerNearby = false;
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.transform.root.CompareTag("Player") || other.GetComponentInParent<CharacterController>() != null;
    }
}
