using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation and hit testing only; PauseMenuController owns all gameplay state.
public sealed class PauseMenuView : MonoBehaviour
{
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public CanvasScaler scaler;
    public Image worldScrim, backlightHalo;
    public GameObject mainView, confirmationView;
    public TMP_Text chapter, footer, confirmationTitle, confirmationBody;
    public Button resumeButton, restartButton, returnButton, confirmButton, cancelButton;
    private Button hovered;
    private Button pressedButton;
    private float pressRemaining;
    private Action pendingAction;
    private string currentScene;
    private bool desktop;
    private Button[] mainButtons, confirmationButtons;

    public bool IsValid => canvas != null && canvasGroup != null && scaler != null &&
        worldScrim != null && backlightHalo != null && mainView != null &&
        confirmationView != null && chapter != null && footer != null && confirmationTitle != null &&
        confirmationBody != null && resumeButton != null && restartButton != null && returnButton != null &&
        confirmButton != null && cancelButton != null && resumeButton.targetGraphic != null &&
        restartButton.targetGraphic != null && returnButton.targetGraphic != null && confirmButton.targetGraphic != null &&
        cancelButton.targetGraphic != null && confirmButton.GetComponentInChildren<TMP_Text>(true) != null &&
        cancelButton.GetComponentInChildren<TMP_Text>(true) != null;

    public void Bind(Action resume, Action restart, Action returnToMenu)
    {
        mainButtons = new[] { resumeButton, restartButton, returnButton };
        confirmationButtons = new[] { confirmButton, cancelButton };
        resumeButton.onClick.AddListener(() => resume());
        restartButton.onClick.AddListener(() => Confirm("Restart this chapter?",
            "Your unfinished interactions in this scene will be discarded. Completed progress will be kept.", "Restart", "Cancel", restart));
        returnButton.onClick.AddListener(() =>
        {
            if (HasUnfinishedMarketCustomer())
                Confirm("Leave the market?",
                    "Your current customer and any unfinished deal will be abandoned.\nCompleted trades, money, and reputation will remain saved.",
                    "Return to Main Menu", "Stay in Market", returnToMenu);
            else
                Confirm("Return to the Main Menu?",
                    "Your unfinished interactions will be discarded. Completed progress will be saved.",
                    "Return to Main Menu", "Cancel", returnToMenu);
        });
        confirmButton.onClick.AddListener(() => { var action = pendingAction; pendingAction = null; action?.Invoke(); });
        cancelButton.onClick.AddListener(ShowMain);
        HideImmediately();
    }

    public void ConfigureCamera(Camera camera, bool useDesktop)
    {
        desktop = useDesktop;
        canvas.renderMode = desktop ? RenderMode.ScreenSpaceOverlay : RenderMode.WorldSpace;
        canvas.worldCamera = camera;
        scaler.uiScaleMode = desktop ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.referenceResolution = new Vector2(1400f, 1000f) / (desktop ? 0.8f : 1f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        scaler.matchWidthOrHeight = 0.5f;
        if (!desktop)
        {
            var rect = (RectTransform)transform;
            rect.sizeDelta = new Vector2(1100f, 840f);
            rect.localScale = Vector3.one * (0.85f / 1100f);
            var scrimRect = (RectTransform)worldScrim.transform;
            scrimRect.anchorMin = scrimRect.anchorMax = new Vector2(0.5f, 0.5f);
            // Overscan the opening gaze; the fixed world-space menu does not follow later head turns.
            float fieldOfView = camera != null ? Mathf.Max(90f, camera.fieldOfView) : 100f;
            float aspect = camera != null ? Mathf.Max(1.2f, camera.aspect) : 1.2f;
            float height = 2f * 1.4f * Mathf.Tan(fieldOfView * Mathf.Deg2Rad * 0.5f);
            scrimRect.sizeDelta = new Vector2(height * aspect, height) * (1.35f * 1100f / 0.85f);
        }
        else
        {
            transform.localScale = Vector3.one;
            var scrimRect = (RectTransform)worldScrim.transform;
            scrimRect.anchorMin = Vector2.zero;
            scrimRect.anchorMax = Vector2.one;
            scrimRect.sizeDelta = Vector2.zero;
        }
    }

    public void Open(string scene, bool useDesktop)
    {
        currentScene = scene;
        gameObject.SetActive(true);
        chapter.text = ChapterName(scene);
        footer.text = useDesktop ? "F10 / Esc to resume  |  Click to select" : "Left Menu button to resume  |  Right trigger to select";
        canvasGroup.alpha = 0f;
        ShowMain();
    }

    private bool HasUnfinishedMarketCustomer()
    {
        var trade = Level1GameState.ExistingInstance?.ActiveTrade;
        return currentScene == GameManager.DefaultGameplaySceneName && trade != null && !trade.SettlementClaimed;
    }

    public static string ChapterName(string scene)
    {
        switch (scene)
        {
            case "ModernStudyRoom": return "THE STUDY  /  A journey begins";
            case "GateIntro": return "THE CITY GATES  /  Welcome to Vijayanagara";
            case "FreeRoam_WithTerrain": return "HAMPI  /  Explore the city";
            case "NewSpiceScene": return "THE SPICE TRADE  /  Discover the marketplace";
            case "NewTransactionTutorial": return "THE ART OF TRADE  /  Your first negotiation";
            case "NewCoinScene": return "CURRENCY  /  Coins of Vijayanagara";
            case "SpicesInteraction": return "THE MERCHANT'S CRAFT  /  Weigh and deliver";
            case "Level1_MainLoopUpdated": return "THE MARKETPLACE  /  A day as a merchant";
            default: return scene;
        }
    }

    private void Confirm(string title, string body, string label, string cancelLabel, Action action)
    {
        ClearHover();
        mainView.SetActive(false);
        confirmationView.SetActive(true);
        confirmationTitle.text = title;
        confirmationBody.text = body;
        confirmButton.GetComponentInChildren<TMP_Text>(true).text = label;
        cancelButton.GetComponentInChildren<TMP_Text>(true).text = cancelLabel;
        pendingAction = action;
    }

    private void ShowMain()
    {
        ClearHover();
        pendingAction = null;
        mainView.SetActive(true);
        confirmationView.SetActive(false);
    }

    public void Tick(bool open, float unscaledDelta)
    {
        if (!gameObject.activeSelf) return;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, open ? 1f : 0f, unscaledDelta / 0.12f);
        if (open && pressedButton != null)
        {
            pressRemaining -= unscaledDelta;
            if (pressRemaining <= 0f)
            {
                var clicked = pressedButton;
                pressedButton = null;
                if (clicked.isActiveAndEnabled && clicked.interactable) clicked.onClick.Invoke();
            }
        }
        RefreshButtonColors(mainButtons);
        RefreshButtonColors(confirmationButtons);
        if (!open && canvasGroup.alpha <= 0f) HideImmediately();
    }

    // Leave the pressed tint visible briefly before an action closes or replaces the view.
    public void Press(Button target)
    {
        if (pressedButton != null || target == null || !target.isActiveAndEnabled || !target.interactable) return;
        pressedButton = target;
        pressRemaining = 0.09f;
        target.targetGraphic.color = target.colors.pressedColor;
        var outline = target.GetComponent<Outline>();
        if (outline != null) outline.effectDistance = new Vector2(1.5f, -1.5f);
    }

    private void RefreshButtonColors(Button[] buttons)
    {
        if (buttons == null) return;
        foreach (var button in buttons)
        {
            if (button == null || button == hovered || button == pressedButton || button.targetGraphic == null) continue;
            Color color = button.interactable ? button.colors.normalColor : button.colors.disabledColor;
            if (button.targetGraphic.color != color) button.targetGraphic.color = color;
        }
    }

    public void HideImmediately()
    {
        if (pressedButton != null && pressedButton.targetGraphic != null)
            pressedButton.targetGraphic.color = pressedButton.colors.normalColor;
        pressedButton = null;
        ClearHover();
        pendingAction = null;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private Button[] ActiveButtons => confirmationView.activeSelf
        ? confirmationButtons : mainButtons;

    public Button HitMouse(Vector2 screenPosition)
    {
        foreach (var button in ActiveButtons)
            if (button.isActiveAndEnabled && button.interactable && RectTransformUtility.RectangleContainsScreenPoint(
                (RectTransform)button.transform, screenPosition, desktop ? null : canvas.worldCamera)) return button;
        return null;
    }

    // Geometry hit testing has no physics/collider synchronization dependency at timeScale zero.
    public Button HitRay(Ray ray, out Vector3 end)
    {
        end = ray.GetPoint(2f);
        if (!new Plane(transform.forward, transform.position).Raycast(ray, out float distance) || distance > 5f) return null;
        end = ray.GetPoint(distance);
        foreach (var button in ActiveButtons)
        {
            var rect = (RectTransform)button.transform;
            if (button.isActiveAndEnabled && button.interactable && rect.rect.Contains(rect.InverseTransformPoint(end))) return button;
        }
        return null;
    }

    public void Hover(Button target)
    {
        if (hovered == target) return;
        ClearHover();
        hovered = target;
        if (hovered == null || hovered == pressedButton) return;
        hovered.targetGraphic.color = hovered.colors.highlightedColor;
        var outline = hovered.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            outline.effectDistance = new Vector2(2.5f, -2.5f);
        }
    }

    public void ClearHover()
    {
        if (hovered != null)
        {
            if (hovered != pressedButton && hovered.targetGraphic != null)
                hovered.targetGraphic.color = hovered.interactable ? hovered.colors.normalColor : hovered.colors.disabledColor;
            var outline = hovered.GetComponent<Outline>();
            if (outline != null && hovered != pressedButton) outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
        hovered = null;
    }
}
