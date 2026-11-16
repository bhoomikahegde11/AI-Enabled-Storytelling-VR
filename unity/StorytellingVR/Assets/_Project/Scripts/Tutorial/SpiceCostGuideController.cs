using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class SpiceCostGuideController : MonoBehaviour
{
    [Header("Guide UI")]
    [SerializeField] private CanvasGroup guideCanvas;
    [SerializeField] private TMP_Text pepperPrice;
    [SerializeField] private TMP_Text cardamomPrice;
    [SerializeField] private TMP_Text cinnamonPrice;

    [Header("Guide State")]
    [SerializeField] private bool guideUnlocked = false;

    private bool guideOpen = false;
    private bool buttonPreviouslyPressed = false;
    public bool IsOpen => guideOpen;

    private void Start()
    {
        var market = new MarketManager();
        SetPrice(pepperPrice, "pepper");
        SetPrice(cardamomPrice, "cardamom");
        SetPrice(cinnamonPrice, "cinnamon");
        SetGuideVisible(false);

        void SetPrice(TMP_Text label, string spice)
        {
            if (label != null)
                label.text = market.CalculateBaseValue(spice, MarketManager.GramsPerVeesai).ToString();
        }
    }

    private void Update()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed);
        HandleToggleInput(xPressed || Input.GetKey(KeyCode.X), PauseMenuController.GameplayInputBlocked);
    }

    private void HandleToggleInput(bool pressed, bool inputBlocked)
    {
        if (guideUnlocked && !inputBlocked && pressed && !buttonPreviouslyPressed)
            ToggleGuide();
        buttonPreviouslyPressed = pressed;
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
