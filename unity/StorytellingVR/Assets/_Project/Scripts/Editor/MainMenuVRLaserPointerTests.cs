using System;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MainMenuVRLaserPointerTests
{
    private GameObject root, panel;
    private Canvas canvas;
    private MainMenuVRLaserPointer pointer;
    private Selectable[] previousSelectables;
    private int previousSelectableCount;
    private bool ownsSelectableRegistry;
    private static readonly FieldInfo SelectablesField = typeof(Selectable)
        .GetField("s_Selectables", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly FieldInfo SelectableCountField = typeof(Selectable)
        .GetField("s_SelectableCount", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly Vector3 Origin = new Vector3(10000f, 10000f, 10000f);

    [SetUp]
    public void SetUp()
    {
        Assert.IsFalse(Application.isPlaying);
        ownsSelectableRegistry = false;
        Assert.IsNotNull(SelectablesField);
        Assert.IsNotNull(SelectableCountField);
        previousSelectables = (Selectable[])SelectablesField.GetValue(null);
        previousSelectableCount = (int)SelectableCountField.GetValue(null);
        // Use the open scene without opening/closing/saving anything. Isolate the registry
        // so production collider synchronization cannot touch the user's scene controls.
        ownsSelectableRegistry = true;
        SelectablesField.SetValue(null, new Selectable[10]);
        SelectableCountField.SetValue(null, 0);
        root = new GameObject("isolated menu UI", typeof(RectTransform), typeof(Canvas));
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        root.transform.position = Origin;
        panel = new GameObject("panel", typeof(RectTransform), typeof(CanvasGroup));
        panel.transform.SetParent(root.transform, false);
        pointer = CreatePointer("right pointer");
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (root != null) Object.DestroyImmediate(root);
            root = null;
        }
        finally
        {
            if (ownsSelectableRegistry)
            {
                SelectablesField.SetValue(null, previousSelectables);
                SelectableCountField.SetValue(null, previousSelectableCount);
                ownsSelectableRegistry = false;
            }
        }
    }

    [Test]
    public void DynamicSlotsPanelSwitchesAndConfirmationNeverLeaveDuplicateOrStaleTargets()
    {
        var initial = CreateButton("form action", 2f);
        Sync(pointer);
        Assert.AreEqual(initial, Hit(pointer));
        panel.SetActive(false);
        Sync(pointer);
        Assert.IsNull(Hit(pointer));
        var slots = new GameObject("slots", typeof(RectTransform));
        slots.transform.SetParent(root.transform, false);
        var slot = CreateButton("runtime slot", 3f, slots.transform);
        Sync(pointer);
        Assert.AreEqual(slot, Hit(pointer));
        slots.SetActive(false);
        var confirmPanel = new GameObject("confirmation", typeof(RectTransform));
        confirmPanel.transform.SetParent(root.transform, false);
        var confirm = CreateButton("confirm", 2f, confirmPanel.transform);
        Sync(pointer);
        Assert.AreEqual(confirm, Hit(pointer));
        confirmPanel.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            slots.SetActive(true);
            Sync(pointer);
            Assert.AreEqual(slot, Hit(pointer));
            Assert.AreEqual(1, slot.GetComponents<BoxCollider>().Length);
            slots.SetActive(false);
            Sync(pointer);
            Assert.IsNull(Hit(pointer));
        }
        Object.DestroyImmediate(slot.gameObject);
        slots.SetActive(true);
        Sync(pointer);
        Assert.IsNull(Hit(pointer));
    }

    [TestCase("alpha")]
    [TestCase("blocksRaycasts")]
    [TestCase("interactable")]
    [TestCase("canvas")]
    [TestCase("graphic")]
    [TestCase("button")]
    public void AvailabilityChangesRejectCachedHitsImmediately(string change)
    {
        var button = CreateButton("action", 2f);
        Sync(pointer);
        Assert.AreEqual(button, Hit(pointer));
        var group = panel.GetComponent<CanvasGroup>();
        switch (change)
        {
            case "alpha": group.alpha = 0f; break;
            case "blocksRaycasts": group.blocksRaycasts = false; break;
            case "interactable": group.interactable = false; break;
            case "canvas": canvas.enabled = false; break;
            case "graphic": button.targetGraphic.raycastTarget = false; break;
            case "button": button.interactable = false; break;
        }
        Assert.IsNull(Hit(pointer), "A stale collider must not make a hidden or disabled control selectable.");
        Sync(pointer);
        Assert.IsFalse(button.GetComponent<BoxCollider>().enabled);
    }

    [Test]
    public void NonAllocBufferGrowsWithoutLosingClosestValidHitAndPreservesLayerFilter()
    {
        var near = CreateButton("near", 2f);
        var far = CreateButton("far", 4f);
        for (int i = 0; i < 6; i++)
        {
            var decoration = new GameObject("non-UI collider", typeof(BoxCollider));
            decoration.transform.SetParent(root.transform, false);
            decoration.transform.localPosition = new Vector3(0f, 0f, 0.4f + i * 0.1f);
            decoration.GetComponent<BoxCollider>().size = Vector3.one * 0.03f;
        }
        Set(pointer, "hitBuffer", new RaycastHit[1]);
        Sync(pointer);
        Assert.AreEqual(near, Hit(pointer));
        Assert.Greater(((RaycastHit[])Get(pointer, "hitBuffer")).Length, 1);
        near.gameObject.layer = 2; // Ignore Raycast: the existing DefaultRaycastLayers contract.
        Assert.AreEqual(far, Hit(pointer));
    }

    [Test]
    public void InputFieldPriorityIsPreservedAndInactiveFieldsCannotStealSelection()
    {
        var button = CreateButton("front action", 2f);
        var fieldHost = new GameObject("name field", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_InputField));
        fieldHost.transform.SetParent(panel.transform, false);
        fieldHost.transform.position = Origin + Vector3.forward * 3f;
        ((RectTransform)fieldHost.transform).sizeDelta = Vector2.one;
        var input = fieldHost.GetComponent<TMP_InputField>();
        input.targetGraphic = fieldHost.GetComponent<Image>();
        Sync(pointer);
        Assert.AreEqual(input, Hit(pointer), "Preserve the existing form-field selection priority.");
        fieldHost.SetActive(false);
        Assert.AreEqual(button, Hit(pointer));
        Sync(pointer);
        Assert.AreEqual(button, Hit(pointer));
    }

    [Test]
    public void ReparentedControlsRefreshCanvasGroupRestrictions()
    {
        var button = CreateButton("moved slot", 2f);
        Sync(pointer);
        Assert.AreEqual(button, Hit(pointer));
        var hidden = new GameObject("hidden group", typeof(RectTransform), typeof(CanvasGroup));
        hidden.transform.SetParent(root.transform, false);
        hidden.GetComponent<CanvasGroup>().alpha = 0f;
        button.transform.SetParent(hidden.transform, true);
        Sync(pointer);
        Assert.IsNull(Hit(pointer));
        button.transform.SetParent(panel.transform, true);
        Sync(pointer);
        Assert.AreEqual(button, Hit(pointer));
    }

    [Test]
    public void RectChangesResizeExistingColliderAndControllersReuseIt()
    {
        var button = CreateButton("slot", 2f);
        var other = CreatePointer("left pointer");
        Sync(pointer);
        Sync(other);
        var collider = button.GetComponent<BoxCollider>();
        ((RectTransform)button.transform).sizeDelta = new Vector2(2f, 0.75f);
        Sync(pointer);
        Sync(other);
        Assert.AreSame(collider, button.GetComponent<BoxCollider>());
        Assert.AreEqual(1, button.GetComponents<BoxCollider>().Length);
        Assert.AreEqual(2f, collider.size.x, 0.001f);
        Assert.AreEqual(0.75f, collider.size.y, 0.001f);
    }

    [Test]
    public void FreshTriggerIsIndependentPerHandAndMustBeReleasedAfterDisconnect()
    {
        var other = CreatePointer("left pointer");
        var handField = Field("controllerHand");
        handField.SetValue(other, Enum.ToObject(handField.FieldType, 0));
        Assert.IsFalse(Press(pointer, true), "Connecting with a held trigger must not select.");
        Assert.IsFalse(Press(pointer, false));
        Assert.IsTrue(Press(pointer, true));
        Assert.IsFalse(Press(pointer, true));
        Assert.IsFalse(Press(other, true));
        Assert.IsFalse(Press(other, false));
        Assert.IsTrue(Press(other, true));
        Invoke(pointer, "ResetPointerState"); // Shared by disconnect and OnDisable.
        Assert.IsFalse(Press(pointer, true));
        Assert.IsFalse(Press(pointer, false));
        Assert.IsTrue(Press(pointer, true));
        var rightData = (PointerEventData)Invoke(pointer, "CreatePointerEventData");
        var leftData = (PointerEventData)Invoke(other, "CreatePointerEventData");
        Assert.AreNotEqual(rightData.pointerId, leftData.pointerId);
        Assert.AreNotSame(rightData, leftData);
        Assert.AreSame(rightData, Invoke(pointer, "CreatePointerEventData"));
    }

    [Test]
    public void HoverExitAndReentryAndDisconnectClearPointerState()
    {
        var button = CreateButton("hover target", 2f);
        int enters = 0, exits = 0;
        var events = button.gameObject.AddComponent<EventTrigger>();
        var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => enters++);
        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => exits++);
        events.triggers.Add(enter);
        events.triggers.Add(exit);
        Invoke(pointer, "UpdateHoverTarget", button.gameObject);
        Invoke(pointer, "UpdateHoverTarget", button.gameObject);
        Invoke(pointer, "ClearHover");
        Invoke(pointer, "UpdateHoverTarget", button.gameObject);
        Invoke(pointer, "ResetPointerState");
        Assert.AreEqual(2, enters);
        Assert.AreEqual(2, exits);
        Assert.IsNull(Get(pointer, "currentHoverObject"));
    }

    [Test]
    public void PanelFadeRejectsCachedTargetsImmediatelyAndRestoresThemAfterOpening()
    {
        var button = CreateButton("fading action", 2f);
        var transition = panel.AddComponent<MainMenuPanelTransition>();
        transition.Initialize(true);
        Sync(pointer);
        Assert.AreEqual(button, Hit(pointer));
        transition.SetVisible(false);
        Assert.IsTrue(panel.activeSelf);
        Assert.IsNull(Hit(pointer), "An outgoing CanvasGroup must reject even an already cached collider.");
        transition.SetVisible(true);
        Assert.IsNull(Hit(pointer), "An incoming screen is not selectable until its fade completes.");
        typeof(MainMenuPanelTransition).GetMethod("Advance", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(transition, new object[] { MainMenuPanelTransition.Duration });
        Assert.AreEqual(button, Hit(pointer));
        Assert.AreEqual(1, button.GetComponents<BoxCollider>().Length);
    }

    private MainMenuVRLaserPointer CreatePointer(string name)
    {
        var host = new GameObject(name);
        host.transform.SetParent(root.transform, false);
        host.SetActive(false); // No Awake, hardware polling or haptics loop in this fixture.
        var result = host.AddComponent<MainMenuVRLaserPointer>();
        Set(result, "hoverHapticStrength", 0f);
        Set(result, "clickHapticStrength", 0f);
        return result;
    }

    private Button CreateButton(string name, float distance, Transform parent = null)
    {
        var host = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        host.transform.SetParent(parent != null ? parent : panel.transform, false);
        host.transform.position = Origin + Vector3.forward * distance;
        ((RectTransform)host.transform).sizeDelta = Vector2.one;
        var button = host.GetComponent<Button>();
        button.targetGraphic = host.GetComponent<Image>();
        return button;
    }

    private static void Sync(MainMenuVRLaserPointer target) => Invoke(target, "SyncSelectableColliders");
    private static bool Press(MainMenuVRLaserPointer target, bool pressed) => (bool)Invoke(target, "ConsumeTriggerPress", pressed);
    private static Selectable Hit(MainMenuVRLaserPointer target)
    {
        Physics.SyncTransforms();
        return (Selectable)Invoke(target, "FindSelectableHit", Origin, Vector3.forward, default(RaycastHit));
    }
    private static FieldInfo Field(string name) => typeof(MainMenuVRLaserPointer).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
    private static object Get(MainMenuVRLaserPointer target, string name) => Field(name).GetValue(target);
    private static void Set(MainMenuVRLaserPointer target, string name, object value) => Field(name).SetValue(target, value);
    private static object Invoke(MainMenuVRLaserPointer target, string name, params object[] args) => typeof(MainMenuVRLaserPointer)
        .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, args);
}
