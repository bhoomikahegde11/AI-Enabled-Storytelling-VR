using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class Phase2VRInteractionTests
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    [TestCase("SpicesInteraction")]
    [TestCase("Level1_MainLoopUpdated")]
    public void CompleteSpiceAssembliesKeepTheirSurfacesInsideTheirContainers(string name)
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/" + name + ".unity");
        try
        {
            var guide = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<PlayerStallSpiceGuide>(true)).Single();
            var stall = guide.playerStallRoot.GetComponentsInParent<Transform>().Single(t => t.name == "PlayerStall");
            Assert.That(stall.localPosition.y, Is.EqualTo(name == "SpicesInteraction" ? -0.52802f : 0.8819798f).Within(0.00001f),
                "Aligning spice parts must not move the stall hierarchy.");
            var renderers = guide.playerStallRoot.GetComponentsInChildren<MeshRenderer>(true);
            string[] surfaces = { "Cone", "CardamomCone", "Cone_1", "Cinnamon" };
            string[] containers = { "Cylinder_4", "Cylinder_1_4", "Cylinder_1_5", "Cylinder_3" };
            for (int i = 0; i < surfaces.Length; i++)
            {
                var surface = renderers.Single(r => r.name == surfaces[i]);
                var container = renderers.Single(r => r.name == containers[i]);
                Vector3 difference = surface.bounds.center - container.bounds.center;
                difference.y = 0f;
                Assert.That(difference.magnitude, Is.LessThan(0.015f), surfaces[i] + " must stay over its sack");
                Assert.That(Mathf.Abs(surface.bounds.max.y - container.bounds.max.y), Is.LessThan(0.08f));
                var zone = surface.GetComponent<SpiceZone>();
                Assert.NotNull(zone.fillOpening);
                // Imported surface origins need not be the dip point; the complete opening must remain over the sack.
                Bounds opening = zone.fillOpening.bounds;
                Assert.That(opening.min.x, Is.GreaterThanOrEqualTo(container.bounds.min.x));
                Assert.That(opening.max.x, Is.LessThanOrEqualTo(container.bounds.max.x));
                Assert.That(opening.min.z, Is.GreaterThanOrEqualTo(container.bounds.min.z));
                Assert.That(opening.max.z, Is.LessThanOrEqualTo(container.bounds.max.z));
            }
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [Test]
    public void CoinManagerReferencesTheSceneCoinAndItsExistingGripRotation()
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/NewCoinScene.unity");
        try
        {
            var manager = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<CoinSceneManager>(true)).Single();
            Assert.AreEqual(scene, manager.varahaCoin.scene);
            var interaction = manager.varahaCoin.GetComponent<CoinInteraction>();
            Assert.NotNull(interaction);
            Assert.AreSame(manager, interaction.sceneManager);
            Assert.NotNull(manager.varahaCoin.GetComponent<VRInspectRotate>());
            Assert.NotNull(manager.instructionPrompt);
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [TestCase(false, true, true)] // Released while paused.
    [TestCase(false, false, true)] // Release edge was missed before resume.
    [TestCase(true, false, false)] // Tracking was interrupted.
    public void ScoopClearsAfterReleaseOrTrackingLoss(bool held, bool paused, bool tracked)
    {
        var host = new GameObject("Scoop lifecycle fixture");
        var scoopObject = new GameObject("Scoop");
        scoopObject.transform.SetParent(host.transform);
        var scoop = scoopObject.AddComponent<ScooperFill>();
        var toggle = host.AddComponent<ScooperToggle>();
        toggle.scooper = scoopObject;
        try
        {
            Invoke(toggle, "Start");
            Invoke(toggle, "UpdateHeldState", true, false, true);
            typeof(ScooperFill).GetField("filled", Private).SetValue(scoop, true);
            scoop.currentSpice = SpiceType.Pepper;
            Invoke(toggle, "UpdateHeldState", held, paused, tracked);
            Assert.IsFalse(scoopObject.activeSelf);
            Assert.IsFalse(scoop.IsFilled());
            Assert.AreEqual(SpiceType.None, scoop.currentSpice);
            Invoke(toggle, "UpdateHeldState", true, false, true);
            Assert.IsTrue(scoopObject.activeSelf, "A later tracked hold must still work.");
            Invoke(toggle, "OnDisable");
            Assert.IsFalse(scoopObject.activeSelf);
        }
        finally { Object.DestroyImmediate(host); }
    }

    [Test]
    public void CoinGripPromptSurvivesSequenceStartAndContinueWaitsForNarration()
    {
        var host = new GameObject("Coin prompt fixture");
        var textObject = new GameObject("Instruction", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(host.transform);
        var prompt = host.AddComponent<InstructionPromptManager>();
        prompt.panel = textObject;
        prompt.instructionText = textObject.GetComponent<TMP_Text>();
        var manager = host.AddComponent<CoinSceneManager>();
        var sequence = host.AddComponent<CoinSequenceManager>();
        sequence.sceneManager = manager;
        sequence.instructionPrompt = prompt;
        try
        {
            sequence.StartSequence();
            Assert.That(prompt.instructionText.text, Does.Contain("right grip"));
            SetNarrating(manager, true);
            Invoke(sequence, "Next");
            Assert.AreEqual(0, typeof(CoinSequenceManager).GetField("index", Private).GetValue(sequence));
            sequence.ShowInspectionPrompt();
            Assert.That(prompt.instructionText.text, Does.Not.Contain("continue"));
            SetNarrating(manager, false);
            sequence.ShowInspectionPrompt();
            Assert.That(prompt.instructionText.text, Does.Contain("right grip").And.Contain("right trigger to continue"));
        }
        finally { Object.DestroyImmediate(host); }
    }

    [Test]
    public void WalkerDetoursAroundTheConversationAndReachesItsOriginalDestination()
    {
        var host = new GameObject("Conversation space fixture");
        var owner = Child(host, "Speaker").AddComponent<NPCInteraction>();
        var listener = Child(host, "Listener").transform;
        var walker = Child(host, "Walker").AddComponent<NPCWalker>();
        listener.position = new Vector3(10000f, 1.6f, 9999f);
        owner.transform.position = new Vector3(10000f, 0f, 10001f);
        walker.transform.position = new Vector3(9998f, 0f, 10000f);
        Vector3 target = new Vector3(10002f, 0f, 10000f);
        walker.Initialize(target, false, target, Quaternion.identity, null);
        NPCWalker.ProtectConversation(owner, listener, owner.transform);
        try
        {
            bool reached = false;
            for (int frame = 0; frame < 600; frame++)
            {
                var args = new object[] { Vector3.zero };
                if (!(bool)typeof(NPCWalker).GetMethod("TryGetMovementDirection", Private).Invoke(walker, args))
                { reached = true; break; }
                var step = (Vector3)Invoke(walker, "GetSafeStep", args[0], 0.05f);
                walker.transform.position += step;
                Vector3 p = walker.transform.position;
                float z = Mathf.Clamp(p.z, 9999f, 10001f);
                Assert.That(Vector2.Distance(new Vector2(p.x, p.z), new Vector2(10000f, z)), Is.GreaterThanOrEqualTo(0.798f));
            }
            Assert.IsTrue(reached, "Protecting a conversation must not permanently stop an unrelated route.");
        }
        finally { NPCWalker.ReleaseConversation(owner); Object.DestroyImmediate(host); }
    }

    [Test]
    public void ConversationSpaceAllowsEscapeAndCleansUpOnDisableOrDestruction()
    {
        var host = new GameObject("Corridor cleanup fixture");
        var owner = Child(host, "Speaker").AddComponent<NPCInteraction>();
        var listener = Child(host, "Listener").transform;
        var walker = Child(host, "Walker").AddComponent<NPCWalker>();
        listener.position = new Vector3(10000f, 1.6f, 9999f);
        owner.transform.position = new Vector3(10000f, 0f, 10001f);
        walker.transform.position = new Vector3(10000f, 0f, 10000f);
        NPCWalker.ProtectConversation(owner, listener, owner.transform);
        try
        {
            Assert.IsFalse((bool)Invoke(walker, "BlocksConversationStep", walker.transform.position, Vector3.right * 0.05f));
            Vector3 outside = walker.transform.position - Vector3.right * 2f;
            Assert.IsTrue((bool)Invoke(walker, "BlocksConversationStep", outside, Vector3.right * 4f));
            Invoke(owner, "OnDisable");
            Assert.IsFalse((bool)Invoke(walker, "BlocksConversationStep", outside, Vector3.right * 4f));
            NPCWalker.ProtectConversation(owner, listener, owner.transform);
            Object.DestroyImmediate(owner);
            Assert.IsFalse((bool)Invoke(walker, "BlocksConversationStep", outside, Vector3.right * 4f));
        }
        finally { NPCWalker.ReleaseConversation(owner); Object.DestroyImmediate(host); }
    }

    [Test]
    public void WalkerFollowsSupportWithoutSnappingToWaypointHeightOrOtherWalkers()
    {
        var host = new GameObject("Ground fixture");
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.SetParent(host.transform);
        floor.transform.position = new Vector3(10000f, -0.1f, 10000f);
        floor.transform.localScale = new Vector3(3f, 0.2f, 3f);
        var walker = Child(host, "Walker").AddComponent<NPCWalker>();
        walker.transform.position = new Vector3(10000f, 0.25f, 10000f);
        var pedestrian = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pedestrian.transform.SetParent(host.transform);
        pedestrian.transform.position = new Vector3(10000f, 0.15f, 10000f);
        pedestrian.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
        pedestrian.AddComponent<NPCWalker>();
        var raisedSupport = GameObject.CreatePrimitive(PrimitiveType.Cube);
        raisedSupport.transform.SetParent(host.transform);
        raisedSupport.transform.position = new Vector3(10000f, 0.2f, 10000f);
        raisedSupport.transform.localScale = new Vector3(0.5f, 0.2f, 0.5f);
        raisedSupport.SetActive(false);
        try
        {
            Physics.SyncTransforms();
            Invoke(walker, "UpdateGrounding", 0.02f);
            Assert.That(walker.transform.position.y, Is.EqualTo(0.19f).Within(0.001f), "Move smoothly, rather than snapping.");
            Invoke(walker, "UpdateGrounding", 0.1f);
            Assert.That(walker.transform.position.y, Is.EqualTo(0f).Within(0.001f));
            raisedSupport.SetActive(true); // A walkway above the base terrain must win over the lower hit.
            Physics.SyncTransforms();
            typeof(NPCWalker).GetField("nextGroundCheck", Private).SetValue(walker, 0f);
            Invoke(walker, "UpdateGrounding", 0.1f);
            Assert.That(walker.transform.position.y, Is.EqualTo(0.3f).Within(0.001f));
            walker.Initialize(new Vector3(10000f, 42f, 10000f), true, Vector3.zero, Quaternion.identity, null);
            var wait = (IEnumerator)Invoke(walker, "WaitAtStall");
            wait.MoveNext();
            Assert.That(walker.transform.position.y, Is.EqualTo(0.3f).Within(0.001f));
            ((IDisposable)wait).Dispose();
            floor.SetActive(false);
            raisedSupport.SetActive(false);
            typeof(NPCWalker).GetField("nextGroundCheck", Private).SetValue(walker, 0f);
            Invoke(walker, "UpdateGrounding", 0.1f);
            Assert.That(walker.transform.position.y, Is.EqualTo(0.3f).Within(0.001f), "Missing support cannot teleport the walker.");
        }
        finally { Object.DestroyImmediate(host); Physics.SyncTransforms(); }
    }

    [UnityTest, Explicit("Runs coin audio gating and arrival presentation in an isolated disposable Play Mode scene.")]
    public IEnumerator CoinNarrationAndArrivalPresentationCompleteWithoutPrematureAdvancement()
    {
        Level1ConversationRuntimeSoakTests.RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunNarration();
        yield return new ExitPlayMode();
    }

    private static IEnumerator RunNarration()
    {
        var host = new GameObject("Coin narration fixture");
        var manager = host.AddComponent<CoinSceneManager>();
        manager.enabled = false; // Drive only the actual narration, not the scene introduction.
        manager.voiceSource = host.AddComponent<AudioSource>();
        manager.subtitlePanel = Child(host, "Subtitles");
        manager.speakerNameText = Text(host, "Speaker");
        manager.dialogueText = Text(host, "Dialogue");
        manager.varahaClip = AudioClip.Create("Coin test narration", 4410, 1, 44100, false);
        var sequence = host.AddComponent<CoinSequenceManager>();
        sequence.enabled = false;
        sequence.sceneManager = manager;
        manager.coinSequence = sequence;
        var narrator = Child(host, "Narrator").AddComponent<NarratorUIManager>();
        narrator.enabled = false;
        narrator.subtitleText = Text(host, "Arrival subtitle");
        narrator.charactersPerSecond = 1000f;
        narrator.allowEditorInput = false;
        try
        {
            sequence.StartSequence();
            manager.NarrateVaraha();
            Assert.IsTrue(manager.IsNarrating);
            Invoke(sequence, "Next");
            Assert.AreEqual(0, typeof(CoinSequenceManager).GetField("index", Private).GetValue(sequence));
            float deadline = Time.realtimeSinceStartup + 2f;
            while (manager.IsNarrating && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.IsFalse(manager.IsNarrating);
            Assert.IsFalse(manager.subtitlePanel.activeSelf);
            float start = Time.realtimeSinceStartup;
            yield return (IEnumerator)typeof(NarratorUIManager).GetMethod("TypeAndWaitForLine", Private).Invoke(narrator,
                new object[] { "Arrival", null, false, (Action<bool>)(_ => { }), true });
            Assert.That(Time.realtimeSinceStartup - start, Is.LessThan(1f), "Arrival must not add the global 3.5 second reading wait.");
            Assert.AreEqual(3.5f, narrator.autoAdvanceDelay, "Other narration retains its authored timing.");
            manager.NarrateVaraha();
            Assert.IsTrue(manager.IsNarrating);
            Invoke(manager, "OnDisable");
            Assert.IsFalse(manager.IsNarrating);
            Assert.IsFalse(manager.voiceSource.isPlaying);
        }
        finally
        {
            Object.Destroy(manager.varahaClip);
            Object.Destroy(host);
        }
    }

    private static TMP_Text Text(GameObject host, string name)
    {
        var child = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        child.transform.SetParent(host.transform);
        return child.GetComponent<TMP_Text>();
    }
    private static GameObject Child(GameObject host, string name)
    {
        var child = new GameObject(name);
        child.transform.SetParent(host.transform);
        return child;
    }
    private static object Invoke(object target, string name, params object[] args) =>
        target.GetType().GetMethod(name, Private).Invoke(target, args);
    private static void SetNarrating(CoinSceneManager manager, bool value) =>
        typeof(CoinSceneManager).GetField("<IsNarrating>k__BackingField", Private).SetValue(manager, value);
}
