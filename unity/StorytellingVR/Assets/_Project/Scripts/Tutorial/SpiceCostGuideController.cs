using UnityEngine;
using UnityEngine.XR;

public class SpiceCostGuideController : MonoBehaviour
{
    [Header("Guide UI")]
    [SerializeField] private CanvasGroup guideCanvas;

    [Header("Guide State")]
    [SerializeField] private bool guideUnlocked = false;

    private bool guideOpen = false;
    private bool buttonPreviouslyPressed = false;

    private void Start()
    {
        SetGuideVisible(false);
    }

    private void Update()
    {
        if (!guideUnlocked)
            return;

        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed))
        {
            if (xPressed && !buttonPreviouslyPressed)
            {
                ToggleGuide();
            }

            buttonPreviouslyPressed = xPressed;
        }
    }

    private void ToggleGuide()
    {
        guideOpen = !guideOpen;
        SetGuideVisible(guideOpen);
    }

    private void SetGuideVisible(bool visible)
    {
        if (guideCanvas == null)
            return;

        guideCanvas.alpha = visible ? 1f : 0f;
        guideCanvas.interactable = visible;
        guideCanvas.blocksRaycasts = visible;
    }

    public void UnlockGuide()
    {
        guideUnlocked = true;
    }

    public void LockGuide()
    {
        guideUnlocked = false;
        guideOpen = false;
        SetGuideVisible(false);
    }

    public void ShowGuide()
    {
        guideOpen = true;
        SetGuideVisible(true);
    }

    public void HideGuide()
    {
        guideOpen = false;
        SetGuideVisible(false);
    }
}