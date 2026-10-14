using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Scene-local presentation only; the intro controller remains the sole cinematic owner.
public class ModernStudyRoomBookGuidance : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private Collider bookCollider;
    [SerializeField] private Grabbable bookGrabbable;
    [SerializeField] private GrabInteractable bookInteractable;
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Material dustMaterial;
    [SerializeField] private Vector3 objectiveOffset = new Vector3(-0.65f, 0.38f, 1.6f);
    [SerializeField] private float labelBobAmplitude = 0.015f;
    [SerializeField] private float labelBobPeriod = 2.6f;
    [SerializeField] private float headEnterDistance = 1.25f;
    [SerializeField] private float headExitDistance = 1.45f;
    [SerializeField] private float handEnterDistance = 0.25f;
    [SerializeField] private float handExitDistance = 0.35f;

    private readonly HashSet<int> hoveringPointers = new HashSet<int>();
    private RectTransform objective;
    private RectTransform bookCue;
    private TMP_Text gripPrompt;
    private ParticleSystem bookDust;
    private bool ready;
    private bool cinematicStarted;
    private bool promptVisible;
    private float lastContextTime;

    private void OnEnable()
    {
        if (bookGrabbable != null)
            bookGrabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private IEnumerator Start()
    {
        // Allow the scene's Start methods and tracked camera rig to initialize first.
        yield return null;
        if (cinematicStarted) yield break;
        if (playerCamera == null || bookCollider == null || bookGrabbable == null ||
            bookInteractable == null || font == null)
        {
            Debug.LogError("[STUDY GUIDANCE] Assign the CenterEyeAnchor Camera, TimeTravelNotebook " +
                "Collider/Grabbable/GrabInteractable and a valid TMP font on ModernStudyRoomBookGuidance.", this);
            enabled = false;
            yield break;
        }

        while (!cinematicStarted && playerCamera != null && !playerCamera.isActiveAndEnabled)
            yield return null;
        if (cinematicStarted || playerCamera == null) yield break;
        CreateGuidance();
        ready = true;
    }

    private void CreateGuidance()
    {
        objective = CreateCanvas("Study opening objective", new Vector2(600f, 108f));
        AddBackdrop(objective);
        AddText(objective, "Current Objective", new Vector2(0f, 29f), new Vector2(560f, 30f),
            23f, new Color(0.85f, 0.71f, 0.37f), TextAlignmentOptions.Left);
        AddText(objective, "Investigate the mysterious book", new Vector2(0f, -12f),
            new Vector2(560f, 58f), 30f, Color.white, TextAlignmentOptions.Left);

        bookCue = CreateCanvas("Mysterious book indicator", new Vector2(360f, 110f));
        var backing = AddBackdrop(bookCue);
        backing.color = new Color(0.09f, 0.065f, 0.045f, 0.48f);
        backing.rectTransform.anchorMin = backing.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        backing.rectTransform.sizeDelta = new Vector2(340f, 84f);
        var accent = new GameObject("Book accent", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accent.layer = 5;
        accent.transform.SetParent(bookCue, false);
        var accentRect = (RectTransform)accent.transform;
        accentRect.sizeDelta = new Vector2(9f, 9f);
        accentRect.anchoredPosition = new Vector2(0f, 42f);
        accentRect.localRotation = Quaternion.Euler(0f, 0f, 45f);
        accent.GetComponent<Image>().color = new Color(0.85f, 0.71f, 0.37f);
        accent.GetComponent<Image>().raycastTarget = false;
        AddText(bookCue, "Mysterious book", new Vector2(0f, 13f), new Vector2(330f, 35f),
            24f, Color.white, TextAlignmentOptions.Center);
        gripPrompt = AddText(bookCue, "Grip to pick up", new Vector2(0f, -25f), new Vector2(330f, 38f),
            29f, new Color(0.96f, 0.94f, 0.88f), TextAlignmentOptions.Center);
        gripPrompt.gameObject.SetActive(false);
        CreateBookDust();
        PositionGuidance();
    }

    private void CreateBookDust()
    {
        if (dustMaterial == null)
        {
            Debug.LogWarning("[STUDY GUIDANCE] Assign the existing URP ParticlesUnlit material to enable book dust.", this);
            return;
        }
        var host = new GameObject("Book guidance golden dust");
        host.SetActive(false);
        // Keep guidance renderers outside the book's cinematic renderer discovery hierarchy.
        host.transform.SetParent(transform, false);
        Bounds bookBounds = bookCollider.bounds;
        host.transform.SetPositionAndRotation(new Vector3(bookBounds.center.x, bookBounds.max.y + 0.02f, bookBounds.center.z), Quaternion.identity);
        bookDust = host.AddComponent<ParticleSystem>();
        var main = bookDust.main;
        main.playOnAwake = false;
        main.loop = true;
        main.duration = 3f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 3f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.006f, 0.012f);
        main.startColor = new Color(1f, 0.76f, 0.35f, 0.55f);
        main.maxParticles = 24;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        var emission = bookDust.emission;
        emission.rateOverTime = 6f;
        var shape = bookDust.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        Vector3 boundsSize = bookBounds.size;
        shape.scale = new Vector3(boundsSize.x * 0.8f, 0.025f, boundsSize.z * 0.8f);
        var velocity = bookDust.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.008f, 0.008f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.035f, 0.055f);
        velocity.z = new ParticleSystem.MinMaxCurve(-0.008f, 0.008f);
        var fade = bookDust.colorOverLifetime;
        fade.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f),
                new GradientAlphaKey(0.6f, 0.65f), new GradientAlphaKey(0f, 1f) });
        fade.color = gradient;
        var lights = bookDust.lights;
        lights.enabled = false;
        var particleRenderer = host.GetComponent<ParticleSystemRenderer>();
        particleRenderer.sharedMaterial = dustMaterial;
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        particleRenderer.receiveShadows = false;
        particleRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        particleRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        host.SetActive(true);
        if (Application.isPlaying) bookDust.Play();
    }

    private RectTransform CreateCanvas(string objectName, Vector2 size)
    {
        var host = new GameObject(objectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        host.layer = 5;
        host.transform.SetParent(transform, false);
        var rect = (RectTransform)host.transform;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one * 0.001f;
        var canvas = host.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = playerCamera;
        var group = host.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        return rect;
    }

    private static Image AddBackdrop(RectTransform parent)
    {
        var host = new GameObject("Quiet brown backing", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        host.layer = 5;
        host.transform.SetParent(parent, false);
        var rect = (RectTransform)host.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        var image = host.GetComponent<Image>();
        image.color = new Color(0.09f, 0.065f, 0.045f, 0.88f);
        image.raycastTarget = false;
        return image;
    }

    private TMP_Text AddText(RectTransform parent, string text, Vector2 position, Vector2 size,
        float fontSize, Color color, TextAlignmentOptions alignment)
    {
        var host = new GameObject(text, typeof(RectTransform), typeof(CanvasRenderer));
        host.layer = 5;
        host.transform.SetParent(parent, false);
        var label = host.AddComponent<TextMeshProUGUI>();
        label.font = font;
        label.text = text;
        label.fontSize = fontSize;
        label.color = color;
        label.alignment = alignment;
        label.raycastTarget = false;
        label.rectTransform.sizeDelta = size;
        label.rectTransform.anchoredPosition = position;
        return label;
    }

    private void LateUpdate()
    {
        if (!ready || cinematicStarted) return;
        if (playerCamera == null || bookCollider == null || bookGrabbable == null || bookInteractable == null)
        {
            HideForCinematic();
            return;
        }
        PositionGuidance();
        bool interactable = bookCollider.enabled && bookCollider.gameObject.activeInHierarchy &&
            bookGrabbable.isActiveAndEnabled && bookInteractable.isActiveAndEnabled;
        if (!interactable) hoveringPointers.Clear();
        bookCue.gameObject.SetActive(interactable);
        if (bookDust != null && bookDust.gameObject.activeSelf != interactable)
        {
            bookDust.gameObject.SetActive(interactable);
            if (interactable) bookDust.Play();
            else bookDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        bool context = interactable && (hoveringPointers.Count > 0 || IsNearBook());
        if (context) lastContextTime = Time.unscaledTime;
        promptVisible = interactable && (context || (promptVisible && Time.unscaledTime - lastContextTime < 0.2f));
        gripPrompt.gameObject.SetActive(promptVisible);
    }

    private bool IsNearBook()
    {
        float headRange = promptVisible ? headExitDistance : headEnterDistance;
        float handRange = promptVisible ? handExitDistance : handEnterDistance;
        return IsWithinRange(playerCamera.transform, headRange) ||
            IsWithinRange(leftController, handRange) || IsWithinRange(rightController, handRange);
    }

    private bool IsWithinRange(Transform trackedPoint, float range)
    {
        return trackedPoint != null && trackedPoint.gameObject.activeInHierarchy &&
            (bookCollider.ClosestPoint(trackedPoint.position) - trackedPoint.position).sqrMagnitude <= range * range;
    }

    private void PositionGuidance()
    {
        Transform head = playerCamera.transform;
        objective.SetPositionAndRotation(head.TransformPoint(objectiveOffset), head.rotation);
        Vector3 cuePosition = bookCollider.bounds.center;
        if (bookDust != null)
            bookDust.transform.SetPositionAndRotation(new Vector3(cuePosition.x, bookCollider.bounds.max.y + 0.02f, cuePosition.z), Quaternion.identity);
        float bob = Mathf.Sin(Time.unscaledTime * (Mathf.PI * 2f / Mathf.Max(0.1f, labelBobPeriod))) * labelBobAmplitude;
        cuePosition.y = bookCollider.bounds.max.y + 0.22f + bob;
        Vector3 direction = cuePosition - head.position;
        // Keep nearby labels out of the uncomfortable near-eye zone while the player reaches.
        if (direction.sqrMagnitude < 0.85f * 0.85f)
            cuePosition = head.position + (direction.sqrMagnitude > 0.001f ? direction.normalized : head.forward) * 0.85f;
        Quaternion facing = Quaternion.LookRotation(cuePosition - head.position);
        bookCue.SetPositionAndRotation(cuePosition, ready
            ? Quaternion.Slerp(bookCue.rotation, facing, 1f - Mathf.Exp(-8f * Time.unscaledDeltaTime))
            : facing);
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
        if (cinematicStarted) return;
        if (pointerEvent.Type == PointerEventType.Hover) hoveringPointers.Add(pointerEvent.Identifier);
        else if (pointerEvent.Type == PointerEventType.Unhover || pointerEvent.Type == PointerEventType.Cancel)
            hoveringPointers.Remove(pointerEvent.Identifier);
    }

    public void HideForCinematic()
    {
        cinematicStarted = true;
        HideGuidance();
        enabled = false;
    }

    private void HideGuidance()
    {
        if (objective != null) objective.gameObject.SetActive(false);
        if (bookCue != null) bookCue.gameObject.SetActive(false);
        if (bookDust != null)
        {
            bookDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            bookDust.gameObject.SetActive(false);
        }
        hoveringPointers.Clear();
        promptVisible = false;
    }

    private void OnDisable()
    {
        if (bookGrabbable != null)
            bookGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
        HideGuidance();
    }
}
