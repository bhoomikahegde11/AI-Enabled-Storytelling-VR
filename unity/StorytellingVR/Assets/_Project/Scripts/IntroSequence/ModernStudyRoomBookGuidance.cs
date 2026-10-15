using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

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
    [SerializeField] private Vector3 objectiveOffset = new Vector3(0f, -0.12f, 1.15f);
    [SerializeField, Min(0.01f)] private float announcementFadeIn = 0.4f;
    [SerializeField, Min(0f)] private float announcementHold = 3f;
    [SerializeField, Min(0.01f)] private float announcementFadeOut = 0.5f;
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
    private Coroutine initialization;
    private CanvasGroup announcementGroup;
    private bool announcementStarted;
    private bool announcementComplete;
    private bool markerPositioned;
    private float announcementElapsed;
    private readonly RaycastHit[] placementHits = new RaycastHit[32];
    private readonly Collider[] placementOverlaps = new Collider[32];

    private void OnEnable()
    {
        if (cinematicStarted) return;
        if (bookGrabbable != null)
            bookGrabbable.WhenPointerEventRaised += HandlePointerEvent;
        if (ready)
        {
            if (objective != null) objective.gameObject.SetActive(!announcementComplete);
        }
        else if (Application.isPlaying && initialization == null)
            initialization = StartCoroutine(InitializeGuidance());
    }

    private IEnumerator InitializeGuidance()
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

        while (!cinematicStarted && playerCamera != null &&
            (!playerCamera.isActiveAndEnabled || !HeadTrackingReady()))
            yield return null;
        // Capture after the rig's first tracked frame, including its LateUpdate pose.
        yield return new WaitForEndOfFrame();
        if (cinematicStarted || playerCamera == null) yield break;
        if (objective == null) CreateGuidance();
        var retryDelay = new WaitForSecondsRealtime(0.25f);
        while (!cinematicStarted && playerCamera != null &&
            (PauseMenuController.GameplaySuspended || !BeginAnnouncement()))
            yield return retryDelay;
        if (cinematicStarted || playerCamera == null) yield break;
        ready = true;
        initialization = null;
    }

    private static bool HeadTrackingReady()
    {
        if (!XRSettings.isDeviceActive) return true;
        InputDevice head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        return head.isValid && head.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked;
    }

    private void CreateGuidance()
    {
        if (cinematicStarted || objective != null) return;
        objective = CreateCanvas("Study opening objective", new Vector2(640f, 128f));
        announcementGroup = objective.GetComponent<CanvasGroup>();
        announcementGroup.alpha = 0f;
        objective.gameObject.SetActive(false);
        AddBackdrop(objective);
        AddText(objective, "NEW OBJECTIVE", new Vector2(0f, 32f), new Vector2(600f, 30f),
            23f, new Color(0.85f, 0.71f, 0.37f), TextAlignmentOptions.Center);
        AddText(objective, "Investigate the mysterious book", new Vector2(0f, -12f),
            new Vector2(600f, 58f), 32f, new Color(0.96f, 0.94f, 0.88f), TextAlignmentOptions.Center);

        bookCue = CreateCanvas("Mysterious book indicator", new Vector2(360f, 110f));
        bookCue.gameObject.SetActive(false);
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

    private bool BeginAnnouncement()
    {
        if (cinematicStarted) return false;
        if (announcementStarted) return true;
        if (!TryPositionAnnouncement()) return false;
        announcementStarted = true;
        announcementElapsed = 0f;
        announcementGroup.alpha = 0f;
        objective.gameObject.SetActive(true);
        return true;
    }

    private bool TryPositionAnnouncement()
    {
        Transform head = playerCamera.transform;
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.01f) forward = Vector3.ProjectOnPlane(head.up, Vector3.up);
        forward.Normalize();
        // Check only at announcement startup, never while it is being displayed.
        for (int candidate = 0; candidate < 6; candidate++)
        {
            float yaw = candidate % 3 == 0 ? 0f : (candidate % 3 == 1 ? -25f : 25f);
            Vector3 direction = Quaternion.AngleAxis(yaw, Vector3.up) * forward;
            Quaternion facing = Quaternion.LookRotation(direction, Vector3.up);
            float distance = candidate < 3 ? Mathf.Max(0.85f, objectiveOffset.z) : 0.85f;
            Vector3 position = head.position + facing * new Vector3(objectiveOffset.x, objectiveOffset.y, distance);
            Vector3 halfSize = new Vector3(objective.rect.width * 0.0005f + 0.025f,
                objective.rect.height * 0.0005f + 0.025f, 0.035f);
            Vector3 toCard = position - head.position;
            int mask = Physics.DefaultRaycastLayers & ~(1 << 5);
            int hits = Physics.BoxCastNonAlloc(head.position, halfSize, toCard.normalized, placementHits,
                facing, toCard.magnitude, mask, QueryTriggerInteraction.Ignore);
            int overlaps = Physics.OverlapBoxNonAlloc(position, halfSize, placementOverlaps,
                facing, mask, QueryTriggerInteraction.Ignore);
            bool blocked = hits == placementHits.Length || overlaps == placementOverlaps.Length;
            for (int i = 0; i < hits && !blocked; i++) blocked = IsRoomObstruction(placementHits[i].collider);
            for (int i = 0; i < overlaps && !blocked; i++) blocked = IsRoomObstruction(placementOverlaps[i]);
            if (blocked) continue;
            objective.SetPositionAndRotation(position, facing);
            return true;
        }
        return false; // Wait for an open view rather than forcing the card into a wall or near the eyes.
    }

    private bool IsRoomObstruction(Collider obstacle)
    {
        return obstacle != null && !obstacle.transform.IsChildOf(playerCamera.transform.root);
    }

    private void AdvanceAnnouncement(float deltaTime)
    {
        if (!announcementStarted || announcementComplete || cinematicStarted) return;
        announcementElapsed += Mathf.Max(0f, deltaTime);
        float fadeIn = Mathf.Max(0.01f, announcementFadeIn);
        float fadeOut = Mathf.Max(0.01f, announcementFadeOut);
        float holdEnd = fadeIn + Mathf.Max(0f, announcementHold);
        if (announcementElapsed >= holdEnd + fadeOut)
        {
            announcementGroup.alpha = 0f;
            objective.gameObject.SetActive(false);
            announcementComplete = true;
            bookCue.gameObject.SetActive(true);
            return;
        }
        announcementGroup.alpha = announcementElapsed < fadeIn
            ? Mathf.Clamp01(announcementElapsed / fadeIn)
            : Mathf.Clamp01(1f - (announcementElapsed - holdEnd) / fadeOut);
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
        AdvanceAnnouncement(PauseMenuController.GameplayUnscaledDeltaTime);
        PositionGuidance();
        bool interactable = bookCollider.enabled && bookCollider.gameObject.activeInHierarchy &&
            bookGrabbable.isActiveAndEnabled && bookInteractable.isActiveAndEnabled;
        if (!interactable) hoveringPointers.Clear();
        bookCue.gameObject.SetActive(announcementComplete && interactable);
        if (bookDust != null && bookDust.gameObject.activeSelf != interactable)
        {
            bookDust.gameObject.SetActive(interactable);
            if (interactable) bookDust.Play();
            else bookDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        bool context = announcementComplete && interactable && (hoveringPointers.Count > 0 || IsNearBook());
        if (context) lastContextTime = Time.unscaledTime;
        promptVisible = announcementComplete && interactable &&
            (context || (promptVisible && Time.unscaledTime - lastContextTime < 0.2f));
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
        Vector3 cuePosition = bookCollider.bounds.center;
        if (bookDust != null)
            bookDust.transform.SetPositionAndRotation(new Vector3(cuePosition.x, bookCollider.bounds.max.y + 0.02f, cuePosition.z), Quaternion.identity);
        if (!announcementComplete) return;
        float bob = Mathf.Sin(Time.unscaledTime * (Mathf.PI * 2f / Mathf.Max(0.1f, labelBobPeriod))) * labelBobAmplitude;
        cuePosition.y = bookCollider.bounds.max.y + 0.22f + bob;
        Vector3 direction = cuePosition - head.position;
        // Keep nearby labels out of the uncomfortable near-eye zone while the player reaches.
        if (direction.sqrMagnitude < 0.85f * 0.85f)
            cuePosition = head.position + (direction.sqrMagnitude > 0.001f ? direction.normalized : head.forward) * 0.85f;
        Quaternion facing = Quaternion.LookRotation(cuePosition - head.position);
        bookCue.SetPositionAndRotation(cuePosition, markerPositioned
            ? Quaternion.Slerp(bookCue.rotation, facing, 1f - Mathf.Exp(-8f * Time.unscaledDeltaTime))
            : facing);
        markerPositioned = true;
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
        if (cinematicStarted) return;
        if (pointerEvent.Type == PointerEventType.Select && !PauseMenuController.GameplayInputBlocked)
        {
            HideForCinematic();
            return;
        }
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
        if (initialization != null) StopCoroutine(initialization);
        initialization = null;
        if (bookGrabbable != null)
            bookGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
        HideGuidance();
    }
}
