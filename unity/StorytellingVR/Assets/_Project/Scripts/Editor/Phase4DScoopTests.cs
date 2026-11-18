using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class Phase4DScoopTests
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

    [TestCase("SpicesInteraction")]
    [TestCase("Level1_MainLoopUpdated")]
    public void TradingSceneHasReachableAndSeparatedBowlTargets(string sceneName)
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/" + sceneName + ".unity");
        try
        {
            var roots = scene.GetRootGameObjects();
            var player = roots.Single(root => root.name == "Player").transform;
            var guide = roots.SelectMany(root => root.GetComponentsInChildren<PlayerStallSpiceGuide>(true)).Single();
            Assert.IsFalse(guide.alignCollidersOnStart, "Keep the authored opening volumes.");
            var zones = guide.playerStallRoot.GetComponentsInChildren<SpiceZone>()
                .Where(zone => zone.fillOpening != null).ToArray();
            CollectionAssert.AreEquivalent(new[] { SpiceType.Pepper, SpiceType.Cardamom, SpiceType.Cinnamon, SpiceType.Turmeric },
                zones.Select(zone => zone.spiceType));

            var scoop = player.GetComponentInChildren<ScooperFill>(true);
            var bowl = scoop.GetComponent<SphereCollider>();
            Assert.NotNull(bowl);
            var fillVisual = scoop.spiceVisuals.Single(entry => entry.spiceType == SpiceType.Cardamom).visual;
            Assert.That(Vector3.Distance(bowl.transform.TransformPoint(bowl.center), fillVisual.GetComponent<Renderer>().bounds.center),
                Is.LessThan(0.02f), "The sensor follows the visible bowl.");
            float bowlDiameter = 2 * bowl.radius * bowl.transform.lossyScale.x;
            foreach (var zone in zones)
            {
                var opening = (BoxCollider)zone.fillOpening;
                Assert.AreSame(zone.gameObject, opening.gameObject);
                Assert.IsTrue(opening.enabled && opening.isTrigger);
                Vector3 center = opening.transform.TransformPoint(opening.center);
                if (zone.spiceType != SpiceType.Turmeric)
                    Assert.That(HorizontalDistance(player.position, center), Is.LessThan(0.98f), zone.spiceType.ToString());

                foreach (var neighbor in zones.Where(candidate => candidate != zone))
                {
                    float edgeClearance = Mathf.Abs(opening.bounds.center.x - neighbor.fillOpening.bounds.center.x)
                        - opening.bounds.extents.x - neighbor.fillOpening.bounds.extents.x;
                    Assert.That(edgeClearance, Is.GreaterThan(bowlDiameter),
                        "The bowl cannot touch both " + zone.spiceType + " and " + neighbor.spiceType + ".");
                }
            }

            var bagTarget = roots.Single(root => root.name == "BagFillPosition").transform;
            var receiver = roots.SelectMany(root => root.GetComponentsInChildren<BagReceiver>(true)).Single();
            Assert.IsTrue(receiver.GetComponent<BoxCollider>().isTrigger);
            Assert.That(HorizontalDistance(player.position, bagTarget.position), Is.LessThan(0.6f));
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [UnityTest, Explicit("Runs physical trigger delivery in Editor Play Mode with a disposable scene.")]
    public IEnumerator EmptyOrWrongScoopCanRetryInBagAndCorrectScoopCompletesOnlyOnce()
    {
        Level1ConversationRuntimeSoakTests.RequireEmptyEditorScene();
        yield return new EnterPlayMode();
        yield return RunDelivery();
        yield return new ExitPlayMode();
    }

    private static IEnumerator RunDelivery()
    {
        var host = new GameObject("Scoop delivery fixture");
        var order = Child(host, "Order").AddComponent<OrderManager>();
        var tutorial = Child(host, "Tutorial").AddComponent<SpiceTutorialManager>();
        var receiverObject = Child(host, "Bag");
        receiverObject.AddComponent<BoxCollider>().isTrigger = true;
        var receiver = receiverObject.AddComponent<BagReceiver>();
        var controller = Child(host, "Controller");
        var scoopObject = Child(controller, "Scooper");
        scoopObject.transform.position = Vector3.forward * 2;
        var bowl = scoopObject.AddComponent<SphereCollider>();
        bowl.radius = 0.075f;
        scoopObject.AddComponent<Rigidbody>().isKinematic = true;
        var scoop = scoopObject.AddComponent<ScooperFill>();
        scoop.wrongSpiceClearDelay = 5f;
        var controllerCollider = controller.AddComponent<BoxCollider>();
        var source = Child(host, "Sack");
        source.transform.position = Vector3.forward * 4;
        var zone = source.AddComponent<SpiceZone>();
        zone.fillOpening = source.AddComponent<BoxCollider>();
        zone.fillOpening.isTrigger = true;
        var sackBody = source.AddComponent<BoxCollider>();
        var fill = typeof(ScooperFill).GetMethod("TryFillFromOpening", PrivateInstance);
        var bagFilled = typeof(SpiceTutorialManager).GetField("bagFilled", PrivateInstance);

        try
        {
            yield return null;
            yield return new WaitForFixedUpdate();
            fill.Invoke(scoop, new object[] { sackBody, true });
            fill.Invoke(scoop, new object[] { zone.fillOpening, false });
            Assert.IsFalse(scoop.IsFilled());

            zone.spiceType = SpiceType.Pepper;
            fill.Invoke(scoop, new object[] { zone.fillOpening, true });
            typeof(BagReceiver).GetMethod("TryReceive", PrivateInstance).Invoke(receiver, new object[] { controllerCollider });
            Assert.IsTrue(scoop.IsFilled(), "The hand collider cannot deliver a scoop.");
            scoopObject.transform.position = Vector3.zero;
            float deadline = Time.realtimeSinceStartup + 1f;
            while (scoop.IsFilled() && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.IsFalse(scoop.IsFilled(), "Wrong spice clears the bowl.");
            Assert.IsFalse((bool)bagFilled.GetValue(tutorial));

            zone.spiceType = order.requestedSpice;
            fill.Invoke(scoop, new object[] { zone.fillOpening, true });
            deadline = Time.realtimeSinceStartup + 1f;
            while (!(bool)bagFilled.GetValue(tutorial) && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.IsTrue((bool)bagFilled.GetValue(tutorial), "The bag can accept on Stay without a second Enter.");
            Assert.IsFalse(scoop.IsFilled());

            fill.Invoke(scoop, new object[] { zone.fillOpening, true });
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(scoop.IsFilled(), "A completed bag cannot consume another scoop.");
        }
        finally { Object.Destroy(host); }
    }

    private static GameObject Child(GameObject parent, string name)
    {
        var result = new GameObject(name);
        result.transform.SetParent(parent.transform);
        return result;
    }

    private static float HorizontalDistance(Vector3 first, Vector3 second) =>
        new Vector2(first.x - second.x, first.z - second.z).magnitude;
}
