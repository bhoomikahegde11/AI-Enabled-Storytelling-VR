using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuVRLaserPointer : MonoBehaviour
{
    private enum ControllerHand
    {
        Left,
        Right
    }

    [Header("Controller")]
    [SerializeField] private ControllerHand controllerHand = ControllerHand.Right;
    [SerializeField] private float triggerThreshold = 0.75f;

    [Header("Haptics")]
    [SerializeField] [Range(0f, 1f)] private float hoverHapticStrength = 0.1f;
    [SerializeField] private float hoverHapticDuration = 0.03f;
    [SerializeField] [Range(0f, 1f)] private float clickHapticStrength = 0.22f;
    [SerializeField] private float clickHapticDuration = 0.045f;
    [SerializeField] private float hapticFrequency = 0.3f;

    [Header("Ray")]
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float rayWidth = 0.01f;
    [SerializeField] private Color idleColor = new Color(1f, 0.95f, 0.6f, 0.9f);
    [SerializeField] private Color hoverColor = Color.white;

    private EventSystem eventSystem;
    private LineRenderer lineRenderer;
    private GameObject currentHoverObject;
    private bool wasTriggerPressed;
    private Coroutine hapticRoutine;
    private bool triggerArmed;
    private PointerEventData pointerEventData;
    private Selectable[] selectableBuffer = new Selectable[32];
    private RaycastHit[] hitBuffer = new RaycastHit[32];
    private readonly Dictionary<Selectable, CachedTarget> targetLookup = new Dictionary<Selectable, CachedTarget>();
    private readonly Dictionary<Collider, CachedTarget> hitLookup = new Dictionary<Collider, CachedTarget>();
    private readonly List<CachedTarget> targets = new List<CachedTarget>();
    private int cacheVersion;
    private float nextHierarchyRefresh;
    private Material lineMaterial;

    private sealed class CachedTarget
    {
        public Selectable Selectable;
        public RectTransform Transform;
        public BoxCollider Collider;
        public TMP_InputField InputField;
        public Graphic Graphic;
        public Canvas Canvas;
        public Transform Parent;
        public Rect Rect;
        public int SeenVersion;
        public readonly List<Canvas> Canvases = new List<Canvas>();
        public readonly List<CanvasGroup> Groups = new List<CanvasGroup>();
    }

    private OVRInput.Controller OvrController =>
        controllerHand == ControllerHand.Left
            ? OVRInput.Controller.LTouch
            : OVRInput.Controller.RTouch;

    private int PointerId =>
        controllerHand == ControllerHand.Left ? -101 : -102;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        EnsureLineRenderer();

        Debug.Log($"{nameof(MainMenuVRLaserPointer)} initialized on '{gameObject.name}' for {controllerHand} controller.");
    }

    private bool IsControllerConnected()
    {
        OVRInput.Controller connectedControllers = OVRInput.GetConnectedControllers();
        return (connectedControllers & OvrController) == OvrController;
    }

    private void Update()
    {
        if (!IsControllerConnected())
        {
            ResetPointerState();
            return;
        }

        SyncSelectableColliders();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Selectable hitSelectable = FindSelectableHit(origin, direction, out RaycastHit hitInfo);
        GameObject hoverTarget = hitSelectable != null ? hitSelectable.gameObject : null;

        UpdateHoverTarget(hoverTarget);
        UpdateLine(origin, hitSelectable != null ? hitInfo.point : origin, hoverTarget != null);
        HandleTriggerPress(hoverTarget);
    }

    private void OnDisable()
    {
        ResetPointerState();
        OVRInput.SetControllerVibration(0f, 0f, OvrController);
    }

    private void ResetPointerState()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        ClearHover();
        triggerArmed = false;
        wasTriggerPressed = false;
        StopHaptics();
    }

    private void StopHaptics()
    {
        if (hapticRoutine == null) return;
        StopCoroutine(hapticRoutine);
        hapticRoutine = null;
        OVRInput.SetControllerVibration(0f, 0f, OvrController);
    }

    private void OnDestroy()
    {
        targetLookup.Clear();
        hitLookup.Clear();
        targets.Clear();
        if (lineMaterial != null)
        {
            if (Application.isPlaying) Destroy(lineMaterial);
            else DestroyImmediate(lineMaterial);
        }
    }

    private void EnsureLineRenderer()
    {
        GameObject lineObject = new GameObject($"{nameof(MainMenuVRLaserPointer)}_Line");
        lineObject.transform.SetParent(transform, false);

        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.numCapVertices = 4;
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sharedMaterial = lineMaterial;
        lineRenderer.startColor = idleColor;
        lineRenderer.endColor = idleColor;
        lineRenderer.enabled = false;
    }

    private void SyncSelectableColliders()
    {
        if (Selectable.allSelectableCount > selectableBuffer.Length)
            System.Array.Resize(ref selectableBuffer, Mathf.NextPowerOfTwo(Selectable.allSelectableCount));
        int count = Selectable.AllSelectablesNoAlloc(selectableBuffer);
        cacheVersion++;
        bool changed = false;
        bool refreshHierarchy = Time.unscaledTime >= nextHierarchyRefresh;
        if (refreshHierarchy) nextHierarchyRefresh = Time.unscaledTime + 0.5f;

        // The registry includes newly enabled/cloned slots, without visiting inactive panels or scene canvases.
        for (int i = 0; i < count; i++)
        {
            Selectable selectable = selectableBuffer[i];
            if (selectable == null || selectable.gameObject.scene != gameObject.scene) continue;
            if (!targetLookup.TryGetValue(selectable, out CachedTarget target))
            {
                var rect = selectable.transform as RectTransform;
                if (rect == null || selectable.GetComponentInParent<Canvas>() == null) continue;
                target = new CachedTarget { Selectable = selectable, Transform = rect };
                targetLookup.Add(selectable, target);
                targets.Add(target);
                RefreshHierarchy(target);
                changed = true;
            }
            target.SeenVersion = cacheVersion;
            Graphic graphic = selectable.targetGraphic;
            if (refreshHierarchy || target.Parent != target.Transform.parent || target.Graphic != graphic ||
                (graphic != null && target.Canvas != graphic.canvas))
            {
                RefreshHierarchy(target);
                changed = true;
            }
            if (target.Collider == null)
            {
                target.Collider = selectable.GetComponent<BoxCollider>();
                if (target.Collider == null) target.Collider = selectable.gameObject.AddComponent<BoxCollider>();
                target.Collider.isTrigger = true;
                UpdateColliderRect(target);
                changed = true;
            }
            else if (target.Rect != target.Transform.rect) UpdateColliderRect(target);
            bool available = IsSelectableRaycastable(target);
            if (target.Collider.enabled != available) target.Collider.enabled = available;
        }
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            CachedTarget target = targets[i];
            if (target.SeenVersion == cacheVersion) continue;
            if (target.Collider != null) target.Collider.enabled = false;
            targetLookup.Remove(target.Selectable);
            targets.RemoveAt(i);
            changed = true;
        }
        if (changed) hitLookup.Clear();
        // Do not retain dead registry entries in unused buffer positions.
        System.Array.Clear(selectableBuffer, count, selectableBuffer.Length - count);
    }

    private static void RefreshHierarchy(CachedTarget target)
    {
        target.Parent = target.Transform.parent;
        target.Graphic = target.Selectable.targetGraphic;
        target.Canvas = target.Graphic != null ? target.Graphic.canvas : target.Selectable.GetComponentInParent<Canvas>();
        target.Transform.GetComponentsInParent(true, target.Canvases);
        target.Transform.GetComponentsInParent(true, target.Groups);
        target.InputField = target.Selectable.GetComponentInParent<TMP_InputField>();
    }

    private static void UpdateColliderRect(CachedTarget target)
    {
        target.Rect = target.Transform.rect;
        target.Collider.center = new Vector3(target.Rect.center.x, target.Rect.center.y, 0f);
        target.Collider.size = new Vector3(Mathf.Abs(target.Rect.width), Mathf.Abs(target.Rect.height), 0.02f);
    }

    private Selectable FindSelectableHit(Vector3 origin, Vector3 direction, out RaycastHit closestHit)
    {
        closestHit = default;
        int count;
        while (true)
        {
            count = Physics.RaycastNonAlloc(origin, direction, hitBuffer, maxDistance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            if (count < hitBuffer.Length) break;
            // A full non-alloc buffer is unordered and may omit the closest target; grow and retry.
            System.Array.Resize(ref hitBuffer, hitBuffer.Length * 2);
        }
        Selectable nearest = null;
        TMP_InputField nearestInput = null;
        RaycastHit inputHit = default;
        float nearestDistance = float.PositiveInfinity, inputDistance = float.PositiveInfinity;
        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = hitBuffer[i];
            if (!hitLookup.TryGetValue(hit.collider, out CachedTarget target))
            {
                TMP_InputField input = hit.collider.GetComponentInParent<TMP_InputField>();
                Selectable selectable = input != null ? input : hit.collider.GetComponentInParent<Selectable>();
                target = selectable != null && targetLookup.TryGetValue(selectable, out CachedTarget cached) ? cached : null;
                hitLookup[hit.collider] = target;
            }
            if (target == null || !IsSelectableRaycastable(target)) continue;
            if (target.InputField != null && targetLookup.TryGetValue(target.InputField, out CachedTarget inputTarget) &&
                IsSelectableRaycastable(inputTarget) && hit.distance < inputDistance)
            {
                nearestInput = target.InputField;
                inputDistance = hit.distance;
                inputHit = hit;
            }
            if (hit.distance < nearestDistance)
            {
                nearest = target.Selectable;
                nearestDistance = hit.distance;
                closestHit = hit;
            }
        }
        // Preserve the existing input-field priority, selecting the closest valid hit within each category.
        if (nearestInput != null) { closestHit = inputHit; return nearestInput; }
        return nearest;
    }

    private void UpdateHoverTarget(GameObject newHoverObject)
    {
        if (currentHoverObject == newHoverObject)
            return;

        PointerEventData eventData = CreatePointerEventData();

        if (currentHoverObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(currentHoverObject, eventData, ExecuteEvents.pointerExitHandler);
        }

        currentHoverObject = newHoverObject;

        if (currentHoverObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(currentHoverObject, eventData, ExecuteEvents.pointerEnterHandler);
            MainMenuAudioController.Instance?.PlayHover();
            PlayHapticPulse(hoverHapticStrength, hoverHapticDuration);
        }
    }

    private void HandleTriggerPress(GameObject hoverTarget)
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OvrController) >= triggerThreshold;

        if (ConsumeTriggerPress(isTriggerPressed) && hoverTarget != null)
        {
            Selectable selectable = hoverTarget.GetComponent<Selectable>();
            if (selectable == null || !targetLookup.TryGetValue(selectable, out CachedTarget target) ||
                !IsSelectableRaycastable(target)) return;
            PointerEventData eventData = CreatePointerEventData();
            LogClickTarget(hoverTarget.name);

            TMP_InputField inputField = hoverTarget.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                MainMenuAudioController.Instance?.PlayClick();
                PlayHapticPulse(clickHapticStrength, clickHapticDuration);

                if (eventSystem == null)
                    eventSystem = EventSystem.current;

                eventSystem?.SetSelectedGameObject(hoverTarget, eventData);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.selectHandler);
                inputField.ActivateInputField();
            }
            else
            {
                Toggle toggle = hoverTarget.GetComponent<Toggle>();
                bool previousToggleValue = toggle != null && toggle.isOn;

                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.selectHandler);

                if (toggle != null && toggle.isOn != previousToggleValue)
                {
                    MainMenuAudioController.Instance?.PlayClick();
                    PlayHapticPulse(clickHapticStrength, clickHapticDuration);
                }
                else if (toggle == null)
                {
                    PlayHapticPulse(clickHapticStrength, clickHapticDuration);
                }
            }
        }

    }

    private bool ConsumeTriggerPress(bool pressed)
    {
        bool fresh = triggerArmed && pressed && !wasTriggerPressed;
        if (!pressed) triggerArmed = true;
        wasTriggerPressed = pressed;
        return fresh;
    }

    private PointerEventData CreatePointerEventData()
    {
        if (pointerEventData == null || eventSystem != EventSystem.current)
        {
            eventSystem = EventSystem.current;
            pointerEventData = new PointerEventData(eventSystem);
        }
        pointerEventData.Reset();
        pointerEventData.button = PointerEventData.InputButton.Left;
        pointerEventData.pointerId = PointerId;
        return pointerEventData;
    }

    private void UpdateLine(Vector3 start, Vector3 end, bool isHoveringButton)
    {
        if (lineRenderer == null)
            return;

        lineRenderer.enabled = isHoveringButton;

        if (!isHoveringButton)
            return;

        lineRenderer.startColor = isHoveringButton ? hoverColor : idleColor;
        lineRenderer.endColor = isHoveringButton ? hoverColor : idleColor;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void ClearHover()
    {
        GameObject previous = currentHoverObject;
        currentHoverObject = null;
        if (previous != null)
            ExecuteEvents.ExecuteHierarchy(previous, CreatePointerEventData(), ExecuteEvents.pointerExitHandler);
    }

    private static bool IsSelectableRaycastable(CachedTarget target)
    {
        Selectable selectable = target.Selectable;
        if (selectable == null)
            return false;

        if (!selectable.gameObject.activeInHierarchy || !selectable.isActiveAndEnabled || !selectable.IsInteractable())
            return false;

        if (selectable.targetGraphic != null)
        {
            if (!selectable.targetGraphic.raycastTarget || !selectable.targetGraphic.isActiveAndEnabled)
                return false;

            CanvasRenderer canvasRenderer = selectable.targetGraphic.canvasRenderer;
            if (canvasRenderer != null && canvasRenderer.cull)
                return false;
        }

        if (target.Canvases.Count == 0) return false;
        foreach (Canvas parentCanvas in target.Canvases)
        {
            if (parentCanvas == null || !parentCanvas.enabled || !parentCanvas.gameObject.activeInHierarchy)
                return false;
        }

        foreach (CanvasGroup canvasGroup in target.Groups)
        {
            if (canvasGroup == null || !canvasGroup.enabled)
                continue;

            if (!canvasGroup.interactable || !canvasGroup.blocksRaycasts || canvasGroup.alpha <= 0.001f)
                return false;
        }

        return true;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    private void LogClickTarget(string targetName)
    {
        Debug.Log($"{nameof(MainMenuVRLaserPointer)} clicked '{targetName}' with {controllerHand} controller.");
    }

    private void PlayHapticPulse(float amplitude, float duration)
    {
        if (amplitude <= 0f || !IsControllerConnected())
            return;

        if (hapticRoutine != null)
            StopCoroutine(hapticRoutine);

        hapticRoutine = StartCoroutine(HapticPulseRoutine(Mathf.Clamp01(amplitude), duration));
    }

    private System.Collections.IEnumerator HapticPulseRoutine(float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(hapticFrequency, amplitude, OvrController);
        yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, duration));
        OVRInput.SetControllerVibration(0f, 0f, OvrController);
        hapticRoutine = null;
    }
}
