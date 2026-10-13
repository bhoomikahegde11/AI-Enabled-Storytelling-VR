using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class Level1BackgroundNPCWalkerTests
{
    private static readonly MethodInfo MovementDirection = typeof(NPCWalker).GetMethod(
        "TryGetMovementDirection", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo Encounter = typeof(NPCWalker).GetField(
        "avoidanceNPC", BindingFlags.Instance | BindingFlags.NonPublic);

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
        walker = first.AddComponent<NPCWalker>();
        SetTarget(walker, first.transform.position + Vector3.forward * 10f);
        Physics.SyncTransforms();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(first);
        Object.DestroyImmediate(second);
        UnityEngine.Random.state = previousRandom;
        Physics.SyncTransforms();
    }

    [Test]
    public void ArrivalUsesRawTargetDistanceDespiteNearbyNPC()
    {
        Assert.IsTrue(GetDirection(walker, out _));
        Assert.IsNotNull(Encounter.GetValue(walker), "Establish avoidance before approaching the target.");

        first.transform.position += Vector3.forward * 9.98f;
        second.transform.position = first.transform.position + Vector3.forward * 0.4f;
        Physics.SyncTransforms();
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
        Physics.SyncTransforms();

        Assert.IsTrue(GetDirection(walker, out Vector3 after));
        Assert.Less((after - before).sqrMagnitude, 0.000001f,
            "Changing facing during the same close encounter must not change the passing direction.");
    }

    [Test]
    public void EncounterResetsAfterSeparationAndChoosesNewPassingDirection()
    {
        GetDirection(walker, out Vector3 before);
        second.transform.position = first.transform.position + Vector3.forward * 1.3f;
        Physics.SyncTransforms();
        GetDirection(walker, out Vector3 separated);
        Assert.IsNull(Encounter.GetValue(walker));
        Assert.Less((separated - Vector3.forward * 10f).sqrMagnitude, 0.000001f);

        second.transform.position = first.transform.position + Vector3.right * 0.4f;
        first.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        Physics.SyncTransforms();
        GetDirection(walker, out Vector3 nextEncounter);
        Assert.IsNotNull(Encounter.GetValue(walker));
        Assert.Greater(nextEncounter.z, 10f, "The new encounter should use its new separation axis.");
        Assert.Greater((nextEncounter - before).sqrMagnitude, 1f);
    }

    [Test]
    public void HeadOnWalkersChooseOppositeWorldSpacePassingDirections()
    {
        NPCWalker otherWalker = second.AddComponent<NPCWalker>();
        second.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        SetTarget(otherWalker, second.transform.position - Vector3.forward * 10f);
        Physics.SyncTransforms();

        GetDirection(walker, out Vector3 firstDirection);
        GetDirection(otherWalker, out Vector3 secondDirection);
        Assert.Less(firstDirection.x, 0f);
        Assert.Greater(secondDirection.x, 0f);
        Assert.AreEqual(-firstDirection.x, secondDirection.x, 0.0001f);
    }

    private static GameObject CreateNPC(string name, Vector3 position)
    {
        var npc = new GameObject(name);
        npc.tag = "NPC";
        npc.transform.position = position;
        CapsuleCollider collider = npc.AddComponent<CapsuleCollider>();
        collider.radius = 0.5f;
        collider.height = 1f;
        return npc;
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
