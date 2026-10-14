using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MainMenuPresentationTests
{
    private readonly List<GameObject> roots = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject root in roots) if (root != null) Object.DestroyImmediate(root);
        roots.Clear();
    }

    [Test]
    public void OpeningUsesExplicitUnscaledStepsAndMovesOnlyTheHeading()
    {
        GameObject panel = Root("panel");
        TMP_Text heading = Text(panel.transform, "heading");
        heading.rectTransform.anchoredPosition = new Vector2(0f, 445f);
        panel.transform.position = new Vector3(1f, 2f, 3f);
        MainMenuPanelTransition transition = panel.AddComponent<MainMenuPanelTransition>();
        transition.Initialize(false, heading.rectTransform);
        transition.SetVisible(true);
        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        Assert.IsFalse(group.interactable);
        Assert.IsFalse(group.blocksRaycasts);
        Step(transition, MainMenuPanelTransition.Duration * 0.5f);
        Assert.That(group.alpha, Is.InRange(0.1f, 0.9f));
        Assert.Less(heading.rectTransform.anchoredPosition.y, 445f);
        Assert.AreEqual(new Vector3(1f, 2f, 3f), panel.transform.position);
        Step(transition, MainMenuPanelTransition.Duration);
        Assert.AreEqual(1f, group.alpha);
        Assert.IsTrue(group.interactable && group.blocksRaycasts);
        Assert.AreEqual(new Vector2(0f, 445f), heading.rectTransform.anchoredPosition);
    }

    [Test]
    public void ClosingBlocksButtonInvocationBeforeTheVisualFadeCompletes()
    {
        GameObject panel = Root("panel");
        Button button = CreateButton(panel.transform, "action");
        GameObject eventHost = Root("events");
        EventSystem events = eventHost.AddComponent<EventSystem>();
        var data = new PointerEventData(events) { button = PointerEventData.InputButton.Left };
        int calls = 0;
        button.onClick.AddListener(() => calls++);
        MainMenuPanelTransition transition = panel.AddComponent<MainMenuPanelTransition>();
        transition.Initialize(true);
        button.OnPointerClick(data);
        Assert.AreEqual(1, calls);
        transition.SetVisible(false);
        Assert.IsTrue(panel.activeSelf, "The outgoing visuals may remain active while fading.");
        Assert.IsFalse(transition.IsVisible, "Navigation must immediately regard the panel as closed.");
        button.OnPointerClick(data);
        Assert.AreEqual(1, calls, "A fading-out button cannot perform another action.");
        Step(transition, MainMenuPanelTransition.Duration);
        Assert.IsFalse(panel.activeSelf);
        transition.SetVisible(true);
        Step(transition, MainMenuPanelTransition.Duration);
        button.OnPointerClick(data);
        Assert.AreEqual(2, calls, "The original callback still works after reopening.");
    }

    [Test]
    public void RapidReversalsFinishInTheLastRequestedState()
    {
        GameObject panel = Root("panel");
        MainMenuPanelTransition transition = panel.AddComponent<MainMenuPanelTransition>();
        transition.Initialize(false);
        for (int i = 0; i < 6; i++)
        {
            transition.SetVisible(true);
            Step(transition, 0.05f);
            transition.SetVisible(false);
            Step(transition, 0.03f);
        }
        transition.SetVisible(true);
        Step(transition, 1f);
        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        Assert.IsTrue(panel.activeSelf && transition.IsVisible);
        Assert.IsFalse(transition.IsAnimating);
        Assert.AreEqual(1f, group.alpha);
        Assert.IsTrue(group.interactable && group.blocksRaycasts);
        panel.SetActive(false);
        Invoke(transition, "OnDisable"); // Explicit lifecycle stepping in EditMode, without starting Play Mode.
        Assert.IsFalse(transition.IsVisible || transition.IsAnimating);
        Assert.AreEqual(0f, group.alpha);
    }

    [Test]
    public void BackUsesLogicalVisibilityWhenAnOutgoingPanelIsStillActive()
    {
        GameObject host = Root("flow", false);
        MainMenuSaveFlow flow = host.AddComponent<MainMenuSaveFlow>();
        GameObject main = Root("main");
        GameObject form = Root("form");
        GameObject slots = Root("slots");
        GameObject confirmation = Root("confirmation");
        Set(flow, "mainMenuRoot", main); Set(flow, "newSavePanel", form);
        Set(flow, "saveSelectPanel", slots); Set(flow, "deleteConfirmation", confirmation);
        MainMenuPanelTransition mainTransition = main.AddComponent<MainMenuPanelTransition>();
        MainMenuPanelTransition formTransition = form.AddComponent<MainMenuPanelTransition>();
        mainTransition.Initialize(true);
        formTransition.Initialize(false);
        slots.SetActive(false); confirmation.SetActive(false);
        mainTransition.SetVisible(false);
        formTransition.SetVisible(true);
        Assert.IsTrue(main.activeSelf && form.activeSelf);
        flow.Back();
        Assert.IsTrue(mainTransition.IsVisible && main.activeSelf);
        Assert.IsFalse(formTransition.IsVisible || form.activeSelf);
    }

    [Test]
    public void DynamicSlotsHaveSeparateMetadataAndNonOverlappingDeleteTargets()
    {
        GameObject root = Root("UI", false);
        GameObject slotPanel = Child(root.transform, "slots", typeof(Image));
        ((RectTransform)slotPanel.transform).sizeDelta = new Vector2(1557.3333f, 1280f);
        GameObject host = Child(root.transform, "flow");
        MainMenuSaveFlow flow = host.AddComponent<MainMenuSaveFlow>();
        Button template = CreateButton(slotPanel.transform, "template");
        Button secondary = CreateButton(slotPanel.transform, "secondary");
        Button back = CreateButton(slotPanel.transform, "Back");
        Set(flow, "saveSelectPanel", slotPanel); Set(flow, "slotButtonTemplate", template);
        Set(flow, "secondaryButtonTemplate", secondary); Set(flow, "slotBackButton", back);
        Set(flow, "slotTitle", Text(slotPanel.transform, "title"));
        Set(flow, "slotHint", Text(slotPanel.transform, "hint"));
        Invoke(flow, "BuildSlotControls");
        var buttons = (Button[])Get(flow, "slotButtons");
        var deletes = (Button[])Get(flow, "deleteButtons");
        Assert.AreEqual(3, buttons.Length);
        for (int i = 0; i < 3; i++)
        {
            RectTransform rect = (RectTransform)buttons[i].transform;
            RectTransform deleteRect = (RectTransform)deletes[i].transform;
            Assert.AreEqual(new Vector2(940f, 210f), rect.sizeDelta);
            Assert.AreEqual(new Vector2(0f, 160f - i * 225f), rect.anchoredPosition);
            Assert.Greater(deleteRect.anchoredPosition.x - deleteRect.sizeDelta.x / 2f, rect.sizeDelta.x / 2f);
            Assert.Less(deleteRect.anchoredPosition.x + deleteRect.sizeDelta.x / 2f, 1557.3333f / 2f);
            MainMenuSlotView view = buttons[i].GetComponent<MainMenuSlotView>();
            Assert.IsNotNull(view);
            Assert.GreaterOrEqual(view.Title.rectTransform.rect.yMin + view.Title.rectTransform.anchoredPosition.y,
                view.Location.rectTransform.rect.yMax + view.Location.rectTransform.anchoredPosition.y);
            Assert.GreaterOrEqual(view.Location.rectTransform.rect.yMin + view.Location.rectTransform.anchoredPosition.y,
                view.SavedDate.rectTransform.rect.yMax + view.SavedDate.rectTransform.anchoredPosition.y);
        }
        Assert.AreEqual(new Vector2(0f, -490f), ((RectTransform)back.transform).anchoredPosition);
        GameObject confirmation = (GameObject)Get(flow, "deleteConfirmation");
        Assert.AreEqual(((RectTransform)slotPanel.transform).sizeDelta, ((RectTransform)confirmation.transform).sizeDelta);
        Assert.IsFalse(confirmation.activeSelf);
    }

    [Test]
    public void LongJourneyNamesStayLiteralAndCannotDisplaceMetadata()
    {
        GameObject root = Root("slot", false);
        MainMenuSlotView view = root.AddComponent<MainMenuSlotView>();
        view.Initialize(Text(root.transform, "title"), Text(root.transform, "metadata style"));
        string name = "  <size=500>" + new string('W', 180) + "</size>  ";
        Vector2 location = view.Location.rectTransform.anchoredPosition;
        view.SetContent(2, name, "The Marketplace", "15 Oct 2026, 18:00", true, true, true, true);
        Assert.AreEqual(name.Trim(), view.Title.text);
        Assert.IsFalse(view.Title.richText);
        Assert.AreEqual(TextWrappingModes.NoWrap, view.Title.textWrappingMode);
        Assert.AreEqual(TextOverflowModes.Ellipsis, view.Title.overflowMode);
        Assert.AreEqual(location, view.Location.rectTransform.anchoredPosition);
        StringAssert.Contains("The Marketplace", view.Location.text);
        StringAssert.Contains("15 Oct", view.SavedDate.text);
        Assert.IsFalse(view.Title.raycastTarget || view.Location.raycastTarget || view.SavedDate.raycastTarget);
    }

    [Test]
    public void EmptyAndUnreadableSlotsRemainReadableWithoutInventedCheckpointMetadata()
    {
        GameObject root = Root("slot", false);
        MainMenuSlotView view = root.AddComponent<MainMenuSlotView>();
        view.Initialize(Text(root.transform, "title"), Text(root.transform, "metadata style"));
        view.SetContent(1, null, null, null, false, false, false, false);
        StringAssert.Contains("Empty Slot", view.Title.text);
        Assert.IsEmpty(view.SavedDate.text);
        Assert.GreaterOrEqual(view.Title.color.a, 0.7f);
        view.SetContent(1, null, null, null, true, false, false, false);
        StringAssert.Contains("repair", view.Title.text);
        StringAssert.Contains("preserved", view.Location.text);
        Assert.IsEmpty(view.SavedDate.text);
    }

    [Test]
    public void SlotTextHasASeparateClipBoundaryAndPaddingBetweenAllThreeLines()
    {
        GameObject root = Root("slot", false);
        ((RectTransform)root.transform).sizeDelta = new Vector2(940f, 210f);
        MainMenuSlotView view = root.AddComponent<MainMenuSlotView>();
        view.Initialize(Text(root.transform, "title"), Text(root.transform, "metadata style"));
        Assert.IsNotNull(view.Content.GetComponent<RectMask2D>());
        Rect frame = ((RectTransform)root.transform).rect;
        Vector2 contentPosition = view.Content.anchoredPosition;
        Assert.GreaterOrEqual(contentPosition.x + view.Content.rect.xMin, frame.xMin);
        Assert.LessOrEqual(contentPosition.x + view.Content.rect.xMax, frame.xMax);
        Assert.GreaterOrEqual(contentPosition.y + view.Content.rect.yMin, frame.yMin);
        Assert.LessOrEqual(contentPosition.y + view.Content.rect.yMax, frame.yMax);
        foreach (TMP_Text text in new[] { view.Title, view.Location, view.SavedDate })
        {
            Assert.AreSame(view.Content, text.rectTransform.parent);
            Assert.IsTrue(text.maskable);
            Assert.IsFalse(text.enableAutoSizing);
            Assert.AreEqual(1, text.maxVisibleLines);
            Assert.AreEqual(TextOverflowModes.Ellipsis, text.overflowMode);
            Rect rect = text.rectTransform.rect;
            Vector2 position = text.rectTransform.anchoredPosition;
            float scale = text == view.Title ? 1.018f : 1f; // Include the established hover enlargement.
            Assert.GreaterOrEqual(position.x + rect.xMin * scale, view.Content.rect.xMin);
            Assert.LessOrEqual(position.x + rect.xMax * scale, view.Content.rect.xMax);
            Assert.GreaterOrEqual(position.y + rect.yMin * scale, view.Content.rect.yMin);
            Assert.LessOrEqual(position.y + rect.yMax * scale, view.Content.rect.yMax);
        }
        float titleBottom = view.Title.rectTransform.anchoredPosition.y - view.Title.rectTransform.rect.height / 2f;
        float locationTop = view.Location.rectTransform.anchoredPosition.y + view.Location.rectTransform.rect.height / 2f;
        Assert.Greater(titleBottom, locationTop);
        Assert.Greater(view.Title.fontSize, view.Location.fontSize);
        Assert.Greater(view.Location.fontSize, view.SavedDate.fontSize);
    }

    [Test]
    public void ExplicitLineBreaksInSavedNamesCannotCreateExtraSlotTextRows()
    {
        GameObject root = Root("slot", false);
        MainMenuSlotView view = root.AddComponent<MainMenuSlotView>();
        view.Initialize(Text(root.transform, "title"), Text(root.transform, "metadata style"));
        string original = "  Hampi\r\nMerchant\tJournal\u2028Second\u2029Line  ";
        view.SetContent(1, original, "The Study", "15 Oct 2026, 18:00", true, true, true, true);
        foreach (char separator in new[] { '\r', '\n', '\t', '\u2028', '\u2029' })
            Assert.IsFalse(view.Title.text.Contains(separator.ToString()));
        StringAssert.Contains("Hampi", view.Title.text);
        StringAssert.Contains("Merchant", view.Title.text);
        StringAssert.Contains("The Study", view.Location.text);
        StringAssert.Contains("15 Oct", view.SavedDate.text);
    }

    [Test]
    public void HoverAndPressMoveOnlyTheLabelAndCancelOnDisable()
    {
        GameObject root = Root("button", false);
        Image image = root.AddComponent<Image>();
        Button button = root.AddComponent<Button>();
        button.targetGraphic = image;
        TMP_Text label = Text(root.transform, "label");
        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = new Vector3(2f, 1f, 0.02f);
        MenuButtonHoverEffect effect = root.AddComponent<MenuButtonHoverEffect>();
        Set(effect, "stableHitbox", true);
        root.SetActive(true);
        Invoke(effect, "Awake");
        Vector3 size = collider.size, center = collider.center, scale = root.transform.localScale;
        effect.OnPointerEnter(null);
        Invoke(effect, "AdvanceVisuals", 0.2f);
        Assert.Greater(label.transform.localScale.x, 1f);
        effect.OnPointerDown(null);
        Invoke(effect, "AdvanceVisuals", 0.2f);
        Assert.Less(label.transform.localScale.x, 1f);
        Assert.AreEqual(scale, root.transform.localScale);
        Assert.AreEqual(size, collider.size);
        Assert.AreEqual(center, collider.center);
        effect.OnPointerClick(null);
        Assert.AreEqual(Time.unscaledTime + 0.09f, (float)Get(effect, "pressUntil"), 0.001f);
        root.SetActive(false);
        Invoke(effect, "OnDisable");
        Assert.AreEqual(0f, (float)Get(effect, "pressUntil"));
        Assert.AreEqual(Vector3.one, label.transform.localScale);
        Assert.AreEqual(Color.white, image.color);
    }

    private GameObject Root(string name, bool active = true)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Canvas));
        root.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        root.SetActive(active);
        roots.Add(root);
        return root;
    }

    private static GameObject Child(Transform parent, string name, params System.Type[] components)
    {
        var child = new GameObject(name, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        foreach (System.Type component in components) child.AddComponent(component);
        return child;
    }

    private static TMP_Text Text(Transform parent, string name) =>
        Child(parent, name, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();

    private static Button CreateButton(Transform parent, string name)
    {
        GameObject host = Child(parent, name, typeof(Image), typeof(Button));
        Button button = host.GetComponent<Button>();
        button.targetGraphic = host.GetComponent<Image>();
        Text(host.transform, "label");
        return button;
    }

    private static void Step(MainMenuPanelTransition target, float dt) => Invoke(target, "Advance", dt);
    private static object Get(object target, string field) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    private static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    private static void Invoke(object target, string method, params object[] arguments) => target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, arguments);
}
