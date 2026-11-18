using System.Reflection;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class Level1BackgroundNPCWalkerTests
{
    private static readonly MethodInfo MovementDirection = typeof(NPCWalker).GetMethod(
        "TryGetMovementDirection", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo Encounter = typeof(NPCWalker).GetField(
        "avoidanceNPC", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo SafeStep = typeof(NPCWalker).GetMethod(
        "GetSafeStep", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo Register = typeof(NPCWalker).GetMethod(
        "OnEnable", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo Unregister = typeof(NPCWalker).GetMethod(
        "OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);

    private GameObject first;
    private GameObject second;
    private NPCWalker walker;
    private UnityEngine.Random.State previousRandom;

    [SetUp]
    public void SetUp()
    {
        previousRandom = UnityEngine.Random.state;
        first = CreateNPC("Walker regression A", new Vector3(10000f, 0f, 10000f));
        second = CreateNPC("Walker regression B", first.transform.position + Vector3.forward * 0.4f);
        walker = RegisterWalker(first);
        RegisterWalker(second);
        SetTarget(walker, first.transform.position + Vector3.forward * 10f);
    }

    [TearDown]
    public void TearDown()
    {
        DestroyTestNpc(first);
        DestroyTestNpc(second);
        UnityEngine.Random.state = previousRandom;
    }

    [Test]
    public void ArrivalUsesRawTargetDistanceDespiteNearbyNPC()
    {
        Assert.IsTrue(GetDirection(walker, out _));
        Assert.IsNotNull(Encounter.GetValue(walker), "Establish avoidance before approaching the target.");

        first.transform.position += Vector3.forward * 9.98f;
        second.transform.position = first.transform.position + Vector3.forward * 0.4f;
        Assert.IsFalse(GetDirection(walker, out _), "Nearby NPC steering must not prevent arrival.");
        Assert.IsNull(Encounter.GetValue(walker));

        SetTarget(walker, first.transform.position + Vector3.forward * 0.06f);
        Assert.IsTrue(GetDirection(walker, out _), "Preserve the existing 0.05 arrival threshold.");
    }

    [Test]
    public void PassingDirectionRemainsStableWhenWalkerTurns()
    {
        Assert.IsTrue(GetDirection(walker, out Vector3 before));
        first.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        Assert.IsTrue(GetDirection(walker, out Vector3 after));
        Assert.Less((after - before).sqrMagnitude, 0.000001f,
            "Changing facing during the same close encounter must not change the passing direction.");
    }

    [Test]
    public void EncounterResetsAfterSeparationAndChoosesNewPassingDirection()
    {
        GetDirection(walker, out Vector3 before);
        second.transform.position = first.transform.position + Vector3.forward * 1.3f;
        GetDirection(walker, out Vector3 separated);
        Assert.IsNull(Encounter.GetValue(walker));
        Assert.Less((separated - Vector3.forward * 10f).sqrMagnitude, 0.000001f);

        second.transform.position = first.transform.position + Vector3.right * 0.4f;
        first.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        SetTarget(walker, first.transform.position + Vector3.right * 10f);
        GetDirection(walker, out Vector3 nextEncounter);
        Assert.IsNotNull(Encounter.GetValue(walker));
        Assert.Greater(nextEncounter.z, 1f, "The new encounter should use its new separation axis.");
        Assert.Greater((nextEncounter - before).sqrMagnitude, 1f);
    }

    [Test]
    public void HeadOnWalkersChooseOppositeWorldSpacePassingDirections()
    {
        NPCWalker otherWalker = second.GetComponent<NPCWalker>();
        second.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        SetTarget(otherWalker, second.transform.position - Vector3.forward * 10f);

        GetDirection(walker, out Vector3 firstDirection);
        GetDirection(otherWalker, out Vector3 secondDirection);
        Assert.Less(firstDirection.x, 0f);
        Assert.Greater(secondDirection.x, 0f);
        Assert.AreEqual(-firstDirection.x, secondDirection.x, 0.0001f);
    }

    [Test]
    public void WalkersCompleteOpposingRoutesWithoutPassingThroughEachOther()
    {
        Vector3 start = first.transform.position;
        Vector3 far = start + Vector3.forward * 4f;
        second.transform.position = far;
        var other = second.GetComponent<NPCWalker>();
        SetTarget(walker, far);
        SetTarget(other, start);

        for (int frame = 0; frame < 500; frame++)
        {
            bool movingFirst = Tick(walker);
            bool movingSecond = Tick(other);
            Assert.That(Vector3.Distance(first.transform.position, second.transform.position), Is.AtLeast(0.678f));
            if (!movingFirst && !movingSecond)
            {
                Assert.That(Vector3.Distance(first.transform.position, far), Is.LessThan(0.05f));
                Assert.That(Vector3.Distance(second.transform.position, start), Is.LessThan(0.05f));
                return;
            }
        }
        Assert.Fail("Both walkers must make it past the crossing and reach their original exits.");
    }

    [Test]
    public void WalkerPassesUntaggedStationaryCharacterAndGivesAgentsAnObstacle()
    {
        var secondWalker = second.GetComponent<NPCWalker>();
        Unregister.Invoke(secondWalker, null);
        Object.DestroyImmediate(secondWalker);
        Vector3 start = first.transform.position;
        second.transform.position = start + Vector3.forward * 1.2f;
        Vector3 target = start + Vector3.forward * 4f;
        walker.Initialize(target, false, target, Quaternion.identity, null, new[] { second.transform });

        var obstacle = walker.GetComponent<NavMeshObstacle>();
        Assert.IsNotNull(obstacle);
        Assert.IsFalse(obstacle.carving);
        Vector3 largeStep = (Vector3)SafeStep.Invoke(walker, new object[] { Vector3.forward, 2f });
        Assert.AreEqual(Vector3.zero, largeStep, "A large frame step cannot cross the stationary body.");

        for (int frame = 0; frame < 350; frame++)
        {
            bool travelling = Tick(walker);
            Assert.That(Vector3.Distance(first.transform.position, second.transform.position), Is.AtLeast(0.678f));
            if (!travelling)
            {
                Assert.That(Vector3.Distance(first.transform.position, target), Is.LessThan(0.05f));
                return;
            }
        }
        Assert.Fail("The walker must route around the stationary character.");
    }

    [TestCase("FreeRoam_WithTerrain", 25)]
    [TestCase("Level1_MainLoopUpdated", 5)]
    public void SceneCrowdBlockersResolveWithoutMovingTheActiveBuyer(string sceneName, int minimumBlockers)
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            $"Assets/_Project/Scenes/CompleteFlowWithFinalScenes/{sceneName}.unity");
        try
        {
            bool freeRoam = sceneName == "FreeRoam_WithTerrain";
            var roots = scene.GetRootGameObjects();
            var spawner = roots.SelectMany(root => root.GetComponentsInChildren<MonoBehaviour>(true))
                .Single(component => freeRoam ? component is FreeRoamMarketSpawner : component is MarketSpawner);
            var blockers = new SerializedObject(spawner).FindProperty("pedestrianBlockers");
            Assert.That(blockers.arraySize, Is.AtLeast(minimumBlockers));
            for (int index = 0; index < blockers.arraySize; index++)
                Assert.IsInstanceOf<Transform>(blockers.GetArrayElementAtIndex(index).objectReferenceValue);
            if (freeRoam)
            {
                var guide = roots.SelectMany(root => root.GetComponentsInChildren<SpiceMerchantGuideSequence>(true)).Single();
                var guideData = new SerializedObject(guide);
                var merchant = (Transform)guideData.FindProperty("merchant").objectReferenceValue;
                var stop = (Transform)guideData.FindProperty("guideStopSpot").objectReferenceValue;
                Assert.IsNull(guideData.FindProperty("merchantAgent").objectReferenceValue);
                first.transform.position = merchant.position;
                var sceneCharacters = ((FreeRoamMarketSpawner)spawner).PedestrianBlockers.Where(t => t != merchant).ToArray();
                walker.ConfigureScriptedMovement(stop.position, guideData.FindProperty("walkSpeed").floatValue, 8f, sceneCharacters);
                int maxSteps = Mathf.CeilToInt(guideData.FindProperty("maxWalkTime").floatValue / 0.05f);
                for (int step = 0; step < maxSteps && Tick(walker); step++) { }
                var remaining = first.transform.position - stop.position;
                remaining.y = 0f;
                Assert.That(remaining.magnitude, Is.LessThan(0.05f), "Bhaskara should reach the same guide stop.");
                return;
            }

            var market = roots.SelectMany(root => root.GetComponentsInChildren<MarketplaceManager>(true)).Single();
            var buyer = market.buyerNPC.GetComponent<NavMeshAgent>();
            Assert.AreEqual(0.3f, buyer.radius);
            Assert.AreEqual(0f, buyer.stoppingDistance);
            Assert.AreEqual(35, buyer.avoidancePriority);
            Assert.IsTrue(Enumerable.Range(0, blockers.arraySize).Any(i =>
                blockers.GetArrayElementAtIndex(i).objectReferenceValue == buyer.transform));
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    [Test]
    public void MainMarketBakedNavMeshConnectsTheOriginalBuyerWaypoints()
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            "Assets/_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity");
        try
        {
            var roots = scene.GetRootGameObjects();
            var market = roots.SelectMany(root => root.GetComponentsInChildren<MarketplaceManager>(true)).Single();
            var surface = roots.SelectMany(root => root.GetComponentsInChildren<Unity.AI.Navigation.NavMeshSurface>(true))
                .Single(item => item.navMeshData != null);
            var instance = NavMesh.AddNavMeshData(surface.navMeshData, surface.transform.position, surface.transform.rotation);
            try
            {
                foreach (var segment in new[] { (market.spawnPoint, market.tradePoint), (market.tradePoint, market.exitPoint) })
                {
                    Assert.IsTrue(NavMesh.SamplePosition(segment.Item1.position, out var from, 0.5f, NavMesh.AllAreas));
                    Assert.IsTrue(NavMesh.SamplePosition(segment.Item2.position, out var to, 0.5f, NavMesh.AllAreas));
                    var path = new NavMeshPath();
                    Assert.IsTrue(NavMesh.CalculatePath(from.position, to.position, NavMesh.AllAreas, path));
                    Assert.AreEqual(NavMeshPathStatus.PathComplete, path.status);
                }
            }
            finally { instance.Remove(); }
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    private static GameObject CreateNPC(string name, Vector3 position)
    {
        var npc = new GameObject(name);
        npc.transform.position = position;
        return npc;
    }

    private static NPCWalker RegisterWalker(GameObject npc)
    {
        // EditMode does not invoke the runtime component's OnEnable callback.
        var component = npc.AddComponent<NPCWalker>();
        Register.Invoke(component, null);
        return component;
    }

    private static void DestroyTestNpc(GameObject npc)
    {
        if (npc.TryGetComponent(out NPCWalker component))
            Unregister.Invoke(component, null);
        Object.DestroyImmediate(npc);
    }

    private static bool Tick(NPCWalker subject)
    {
        if (!GetDirection(subject, out Vector3 direction))
            return false;
        subject.transform.position += (Vector3)SafeStep.Invoke(subject, new object[] { direction, 0.05f });
        return true;
    }

    private static void SetTarget(NPCWalker subject, Vector3 destination)
    {
        subject.Initialize(destination, false, destination, Quaternion.identity, null);
    }

    private static bool GetDirection(NPCWalker subject, out Vector3 direction)
    {
        var arguments = new object[] { Vector3.zero };
        bool travelling = (bool)MovementDirection.Invoke(subject, arguments);
        direction = (Vector3)arguments[0];
        return travelling;
    }
}

public class Level1BackgroundNpcRuntimeTests
{
    [UnityTest, Explicit("Runs in Editor Play Mode from an empty disposable scene.")]
    public IEnumerator ActualWalkersYieldAndDespawnAtTheirAssignedExits()
    {
        Level1ConversationRuntimeSoakTests.RequireEmptyEditorScene();
        yield return new EnterPlayMode();

        float timeScale = Time.timeScale;
        int captureRate = Time.captureFramerate;
        var left = new GameObject("Crossing walker A");
        var right = new GameObject("Crossing walker B");
        var bystander = new GameObject("Stationary character");
        try
        {
            Time.captureFramerate = 60;
            Time.timeScale = 5f;
            right.transform.position = Vector3.forward * 4f;
            bystander.transform.position = Vector3.right * 0.62f + Vector3.forward * 2.5f;
            var blockers = new[] { bystander.transform };
            left.AddComponent<NPCWalker>().Initialize(right.transform.position, false, right.transform.position,
                Quaternion.identity, null, blockers);
            right.AddComponent<NPCWalker>().Initialize(Vector3.zero, false, Vector3.zero,
                Quaternion.identity, null, blockers);

            for (int frame = 0; frame < 240 && (left != null || right != null); frame++)
            {
                yield return null;
                if (left != null && right != null)
                    Assert.That(Vector3.Distance(left.transform.position, right.transform.position), Is.AtLeast(0.678f));
                if (left != null)
                    Assert.That(Vector3.Distance(left.transform.position, bystander.transform.position), Is.AtLeast(0.678f));
                if (right != null)
                    Assert.That(Vector3.Distance(right.transform.position, bystander.transform.position), Is.AtLeast(0.678f));
            }
            Assert.IsTrue(left == null && right == null, "Both walkers should finish their routes and despawn.");
        }
        finally
        {
            Object.Destroy(left);
            Object.Destroy(right);
            Object.Destroy(bystander);
            Time.timeScale = timeScale;
            Time.captureFramerate = captureRate;
        }
        yield return new ExitPlayMode();
    }
}
