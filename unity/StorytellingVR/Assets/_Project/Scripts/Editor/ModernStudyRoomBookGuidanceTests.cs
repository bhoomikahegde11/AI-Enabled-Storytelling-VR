using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Oculus.Interaction;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ModernStudyRoomBookGuidanceTests
{
    private GameObject host, head, book;
    private ModernStudyRoomBookGuidance guidance;
    private Camera camera;

    [SetUp]
    public void SetUp()
    {
        host = new GameObject("study guidance fixture");
        host.SetActive(false);
        guidance = host.AddComponent<ModernStudyRoomBookGuidance>();
        head = new GameObject("tracked head");
        camera = head.AddComponent<Camera>();
        book = new GameObject("offset book geometry");
        book.transform.position = new Vector3(10f, 2f, 3f);
        var collider = book.AddComponent<BoxCollider>();
        collider.center = new Vector3(-1f, 0f, 0f);
        collider.size = new Vector3(0.6f, 0.2f, 0.4f);
        Set("playerCamera", camera);
        Set("bookCollider", collider);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(host);
        Object.DestroyImmediate(head);
        Object.DestroyImmediate(book);
    }

    [Test]
    public void ProximityUsesColliderSurfaceAndWiderExitRange()
    {
        // The model's pivot is offset: reaching its visible geometry must count.
        head.transform.position = new Vector3(9f, 2f, 4.4f); // 1.2 m from collider surface.
        Physics.SyncTransforms();
        Assert.IsTrue((bool)Invoke("IsNearBook"));
        head.transform.position = new Vector3(9f, 2f, 4.6f); // 1.4 m: outside entry, inside exit.
        Assert.IsFalse((bool)Invoke("IsNearBook"));
        Set("promptVisible", true);
        Assert.IsTrue((bool)Invoke("IsNearBook"));
        head.transform.position = new Vector3(9f, 2f, 5f);
        Assert.IsFalse((bool)Invoke("IsNearBook"));
    }

    [Test]
    public void EitherNearbyControllerCanProvideContextWithoutMovingTheHead()
    {
        head.transform.position = Vector3.zero;
        var hand = new GameObject("nearby controller");
        try
        {
            hand.transform.position = new Vector3(9f, 2f, 3.4f);
            Set("leftController", hand.transform);
            Assert.IsTrue((bool)Invoke("IsNearBook"));
            Set("leftController", null);
            Set("rightController", hand.transform);
            Assert.IsTrue((bool)Invoke("IsNearBook"));
            hand.SetActive(false);
            Assert.IsFalse((bool)Invoke("IsNearBook"));
        }
        finally { Object.DestroyImmediate(hand); }
    }

    [Test]
    public void HoverContextTracksBothPointersAndCannotReturnAfterCinematicStarts()
    {
        Invoke("HandlePointerEvent", new PointerEvent(1, PointerEventType.Hover, Pose.identity));
        Invoke("HandlePointerEvent", new PointerEvent(2, PointerEventType.Hover, Pose.identity));
        var pointers = (HashSet<int>)Get("hoveringPointers");
        Assert.AreEqual(2, pointers.Count);
        Invoke("HandlePointerEvent", new PointerEvent(1, PointerEventType.Unhover, Pose.identity));
        Assert.IsTrue(pointers.Contains(2));
        Invoke("HandlePointerEvent", new PointerEvent(2, PointerEventType.Cancel, Pose.identity));
        Assert.IsEmpty(pointers);
        guidance.HideForCinematic();
        Invoke("HandlePointerEvent", new PointerEvent(3, PointerEventType.Hover, Pose.identity));
        Assert.IsEmpty(pointers);
    }

    [Test]
    public void GuidanceIsSceneLocalNonInteractiveAndPermanentlyHiddenForCinematic()
    {
        CreateVisualFixture();
        Set("ready", true);
        var objective = (RectTransform)Get("objective");
        var cue = (RectTransform)Get("bookCue");
        Assert.AreEqual(host.transform, objective.parent);
        Assert.AreEqual(host.transform, cue.parent);
        Assert.AreEqual(0, host.GetComponentsInChildren<GraphicRaycaster>(true).Length);
        Assert.AreEqual(0, host.GetComponentsInChildren<Collider>(true).Length);
        foreach (var graphic in host.GetComponentsInChildren<Graphic>(true))
            Assert.IsFalse(graphic.raycastTarget);
        foreach (var canvas in host.GetComponentsInChildren<Canvas>(true))
        {
            Assert.AreEqual(RenderMode.WorldSpace, canvas.renderMode);
            Assert.AreEqual(camera, canvas.worldCamera);
            Assert.IsFalse(canvas.GetComponent<CanvasGroup>().blocksRaycasts);
        }
        guidance.HideForCinematic();
        guidance.HideForCinematic();
        Invoke("LateUpdate");
        Assert.IsFalse(objective.gameObject.activeSelf);
        Assert.IsFalse(cue.gameObject.activeSelf);
        Assert.IsFalse(guidance.enabled);
        var dust = (ParticleSystem)Get("bookDust");
        Assert.IsFalse(dust.gameObject.activeSelf);
        Assert.IsFalse(dust.isPlaying);
        Assert.AreEqual(0, dust.particleCount);
    }

    [Test]
    public void DustFollowsBookGeometryAndDoesNotEnterCinematicRendererHierarchy()
    {
        CreateVisualFixture();
        var dust = (ParticleSystem)Get("bookDust");
        Assert.IsFalse(dust.transform.IsChildOf(book.transform));
        Assert.IsTrue(dust.transform.IsChildOf(host.transform));
        Assert.IsFalse(dust.lights.enabled);
        Assert.LessOrEqual(dust.main.maxParticles, 24);
        var renderer = dust.GetComponent<ParticleSystemRenderer>();
        Assert.AreEqual(UnityEngine.Rendering.ShadowCastingMode.Off, renderer.shadowCastingMode);
        Assert.IsFalse(renderer.receiveShadows);
        Vector3 previous = dust.transform.position;
        book.transform.position += Vector3.right;
        Physics.SyncTransforms();
        Invoke("PositionGuidance");
        Assert.Less((dust.transform.position - previous - Vector3.right).magnitude, 0.001f);

        var objective = (RectTransform)Get("objective");
        Vector3 cameraRelative = head.transform.InverseTransformPoint(objective.position);
        Assert.Less(cameraRelative.x, 0f);
        Assert.Greater(cameraRelative.y, 0f);
        Assert.Greater(cameraRelative.z, 1f);
    }

    private void CreateVisualFixture()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/_Project/Fonts/LibertinusSerif-Regular SDF 1.asset");
        var material = AssetDatabase.LoadAssetAtPath<Material>(
            "Packages/com.unity.render-pipelines.universal/Runtime/Materials/ParticlesUnlit.mat");
        Assert.IsNotNull(font, "The scene's assigned font must resolve.");
        Assert.IsNotNull(material, "The reused Hampi particle material must resolve.");
        Set("font", font);
        Set("dustMaterial", material);
        Invoke("CreateGuidance");
    }

    private static FieldInfo Field(string name) => typeof(ModernStudyRoomBookGuidance)
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
    private void Set(string name, object value) => Field(name).SetValue(guidance, value);
    private object Get(string name) => Field(name).GetValue(guidance);
    private object Invoke(string name, params object[] args) => typeof(ModernStudyRoomBookGuidance)
        .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(guidance, args);
}
