using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class Phase4BTutorialTests
{
    [Test]
    public void RahimSceneHidesLegacyPriceOverlayAndKeepsLivePresentation()
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/NewTransactionTutorial.unity");
        try
        {
            var roots = scene.GetRootGameObjects();
            var overlay = roots.Single(root => root.name == "Voice Prompt");
            Assert.IsFalse(overlay.activeSelf);
            var tutorial = roots.SelectMany(root => root.GetComponentsInChildren<TutorialManager>(true)).Single();
            var voice = roots.SelectMany(root => root.GetComponentsInChildren<Level1VoiceInputManager>(true)).Single();
            var prompt = roots.SelectMany(root => root.GetComponentsInChildren<PromptManager>(true)).Single();
            var hud = tutorial.hudManager;
            Assert.IsNull(tutorial.spokenPriceText);
            Assert.NotNull(prompt.panel);
            Assert.NotNull(prompt.promptText);
            Assert.AreSame(hud.playerInput, voice.inputField);
            Assert.NotNull(hud.voiceStatusText);
            Assert.NotNull(hud.currentTradePanel);
            Assert.NotNull(new SerializedObject(hud).FindProperty("npcSubtitleText").objectReferenceValue);
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [Test]
    public void DisabledInspectionControllerLeavesTheEpilogueBookRayAlone()
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Epilogue_BookPurchase.unity");
        try
        {
            var controller = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<MeeraInspectionSequenceController>(true)).Single();
            Assert.IsFalse(controller.enabled);
            var data = new SerializedObject(controller);
            var configured = (Behaviour)data.FindProperty("bookRayInteractable").objectReferenceValue;
            var ray = (Behaviour)configured.GetComponent("RayInteractable");
            Assert.IsTrue(ray.enabled);
            typeof(MeeraInspectionSequenceController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(controller, null);
            Assert.IsTrue(ray.enabled, "The separate epilogue book flow owns this ray.");
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [Test]
    public void FreeRoamInspectionHasThreeDistinctRayTargetsAndTheNotebookHandoff()
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/FreeRoam_WithTerrain.unity");
        try
        {
            var roots = scene.GetRootGameObjects();
            var controller = roots.SelectMany(root => root.GetComponentsInChildren<MeeraInspectionSequenceController>(true)).Single();
            var data = new SerializedObject(controller);
            string[] items = { "lampOrVase", "foreignTrinket", "book" };
            string[] rays = { "lampRayInteractable", "compassRayInteractable", "bookRayInteractable" };
            var targets = new Behaviour[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                var item = (MeeraInspectableItem)data.FindProperty(items[i]).objectReferenceValue;
                var ray = (Behaviour)data.FindProperty(rays[i]).objectReferenceValue;
                Assert.NotNull(item);
                Assert.AreEqual(i, (int)item.ItemType);
                Assert.IsTrue(item.gameObject.activeInHierarchy);
                Assert.NotNull(new SerializedObject(item).FindProperty("glowObject").objectReferenceValue);
                Assert.NotNull(ray);
                targets[i] = (Behaviour)ray.GetComponent("RayInteractable");
                Assert.NotNull(targets[i]);
            }
            Assert.AreEqual(3, targets.Distinct().Count());
            Assert.NotNull(data.FindProperty("meeraNPCInteraction").objectReferenceValue);
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }
}

public class Phase4BInspectionRuntimeTests
{
    [UnityTest, Explicit("Runs the production sequence in Editor Play Mode from a disposable scene.")]
    public IEnumerator DialogueGatesEachItemAndResetCancelsTheActiveInspection()
    {
        Level1ConversationRuntimeSoakTests.RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunInspection();
        yield return new ExitPlayMode();
    }

    private static IEnumerator RunInspection()
    {
        var host = new GameObject("Meera inspection fixture");
        host.SetActive(false);
        var controller = host.AddComponent<MeeraInspectionSequenceController>();
        var lamp = Item(host.transform, controller, MeeraInspectableItem.InspectableItemType.LampOrVase);
        var compass = Item(host.transform, controller, MeeraInspectableItem.InspectableItemType.ForeignTrinket);
        var book = Item(host.transform, controller, MeeraInspectableItem.InspectableItemType.Book);
        Set(controller, "lampOrVase", lamp.item);
        Set(controller, "foreignTrinket", compass.item);
        Set(controller, "book", book.item);
        Set(controller, "lampRayInteractable", lamp.ray);
        Set(controller, "compassRayInteractable", compass.ray);
        Set(controller, "bookRayInteractable", book.ray);
        var rayRoot = new GameObject("Inspection ray fixture");
        rayRoot.transform.SetParent(host.transform);
        Set(controller, "inspectionRayRoot", rayRoot);
        Set(controller, "normalItemDialogueDuration", 0.15f);
        Set(controller, "bookDialogueDuration", 0.15f);
        controller.onAllItemsInspected = new UnityEvent();
        int completions = 0;
        controller.onAllItemsInspected.AddListener(() => completions++);
        host.SetActive(true);

        try
        {
            controller.BeginInspectionSequence();
            AssertAvailability(0);
            compass.item.TryInspect();
            book.item.TryInspect();
            Assert.IsFalse(controller.InspectionBusy);

            lamp.item.TryInspect();
            Assert.IsTrue(controller.InspectionBusy);
            Assert.IsFalse(compass.item.CanInspect);
            controller.ResetInspectionSequence();
            AssertAvailability(-1);
            controller.BeginInspectionSequence();
            yield return new WaitForSecondsRealtime(0.25f);
            AssertAvailability(0);
            Assert.IsFalse(lamp.item.HasBeenInspected);

            lamp.item.TryInspect();
            lamp.item.TryInspect();
            compass.item.TryInspect();
            Assert.IsTrue(controller.InspectionBusy);
            Assert.IsFalse(compass.ray.enabled);
            Assert.IsFalse(lamp.ray.enabled);
            yield return WaitFor(() => lamp.item.HasBeenInspected);
            AssertAvailability(1);

            book.item.TryInspect();
            Assert.IsFalse(controller.InspectionBusy);
            compass.item.TryInspect();
            Assert.IsFalse(book.ray.enabled);
            yield return WaitFor(() => compass.item.HasBeenInspected);
            AssertAvailability(2);

            LogAssert.Expect(LogType.Error, "[MEERA INSPECTION] NarratorUIManager.Instance is missing.");
            book.item.TryInspect();
            book.item.TryInspect();
            Assert.IsFalse(controller.SequenceCompleted);
            yield return WaitFor(() => controller.SequenceCompleted);
            Assert.AreEqual(1, completions);
            AssertAvailability(-1);

            controller.ResetInspectionSequence();
            controller.BeginInspectionSequence();
            AssertAvailability(0);
            Assert.IsFalse(book.item.HasBeenInspected);
            Assert.IsFalse(compass.item.HasBeenInspected);
        }
        finally { Object.Destroy(host); }

        void AssertAvailability(int current)
        {
            int index = 0;
            foreach (var target in new[] { lamp, compass, book })
            {
                Assert.IsTrue(target.item.gameObject.activeInHierarchy, "Every prop stays visible.");
                bool available = index++ == current;
                Assert.AreEqual(available, target.item.CanInspect);
                Assert.AreEqual(available, target.ray.enabled);
                Assert.AreEqual(available, target.highlight.activeSelf);
            }
        }
    }

    private static (MeeraInspectableItem item, Light ray, GameObject highlight) Item(
        Transform parent, MeeraInspectionSequenceController controller, MeeraInspectableItem.InspectableItemType type)
    {
        var child = new GameObject(type.ToString());
        child.transform.SetParent(parent);
        var item = child.AddComponent<MeeraInspectableItem>();
        var ray = child.AddComponent<Light>();
        var highlight = new GameObject("Existing highlight");
        highlight.transform.SetParent(child.transform);
        Set(item, "itemType", type);
        Set(item, "inspectionController", controller);
        Set(item, "glowObject", highlight);
        return (item, ray, highlight);
    }

    private static void Set(object instance, string name, object value) =>
        instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);

    private static IEnumerator WaitFor(Func<bool> condition)
    {
        float deadline = Time.realtimeSinceStartup + 5f;
        while (!condition() && Time.realtimeSinceStartup < deadline)
            yield return null;
        Assert.IsTrue(condition(), "The inspection dialogue did not finish.");
    }
}
