using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenuVRLaserPointer : MonoBehaviour
{
    private enum ControllerHand
    {
        Left,
        Right
    }

    private static bool collidersInitialized;

    [Header("Controller")]
    [SerializeField] private ControllerHand controllerHand = ControllerHand.Right;
    [SerializeField] private float triggerThreshold = 0.75f;

    [Header("Ray")]
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float rayWidth = 0.01f;
    [SerializeField] private Color idleColor = new Color(1f, 0.95f, 0.6f, 0.9f);
    [SerializeField] private Color hoverColor = Color.white;

    private EventSystem eventSystem;
    private LineRenderer lineRenderer;
    private GameObject currentHoverObject;
    private bool wasTriggerPressed;

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
        EnsureButtonColliders();

        Debug.Log($"{nameof(MainMenuVRLaserPointer)} initialized on '{gameObject.name}' for {controllerHand} controller.");
    }

    private bool IsControllerConnected()
    {
        OVRInput.Controller connectedControllers = OVRInput.GetConnectedControllers();
        return (connectedControllers & OvrController) == OvrController;
    }

    private void Update()
    {
        EnsureButtonColliders();

        if (!IsControllerConnected())
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;

            ClearHover();
            return;
        }

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
        ClearHover();
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
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = idleColor;
        lineRenderer.endColor = idleColor;
        lineRenderer.enabled = false;
    }

    private void EnsureButtonColliders()
    {
        if (collidersInitialized)
            return;

        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        if (canvases == null || canvases.Length == 0)
        {
            Debug.LogWarning($"{nameof(MainMenuVRLaserPointer)} could not find any canvases to prepare menu colliders.");
            return;
        }

        bool foundAnySelectable = false;

        foreach (Canvas canvas in canvases)
        {
            if (canvas == null)
                continue;

            if (!canvas.gameObject.scene.IsValid())
                continue;

            Selectable[] selectables = canvas.GetComponentsInChildren<Selectable>(true);
            foreach (Selectable selectable in selectables)
            {
                if (selectable == null)
                    continue;

                RectTransform rectTransform = selectable.GetComponent<RectTransform>();
                if (rectTransform == null)
                    continue;

                BoxCollider boxCollider = selectable.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    boxCollider = selectable.gameObject.AddComponent<BoxCollider>();
                    Debug.Log($"{nameof(MainMenuVRLaserPointer)} added runtime BoxCollider to '{selectable.gameObject.name}'.");
                }

                Rect rect = rectTransform.rect;
                boxCollider.center = new Vector3(rect.center.x, rect.center.y, 0f);
                boxCollider.size = new Vector3(Mathf.Abs(rect.width), Mathf.Abs(rect.height), 0.02f);
                boxCollider.isTrigger = true;
                foundAnySelectable = true;
            }
        }

        if (!foundAnySelectable)
        {
            Debug.LogWarning($"{nameof(MainMenuVRLaserPointer)} found canvases but no selectable menu controls to prepare.");
            return;
        }

        collidersInitialized = true;
    }

    private Selectable FindSelectableHit(Vector3 origin, Vector3 direction, out RaycastHit closestHit)
    {
        closestHit = default;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        if (hits.Length == 0)
            return null;

        System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

        foreach (RaycastHit hit in hits)
        {
            Selectable selectable = hit.collider.GetComponentInParent<Selectable>();
            if (selectable == null || !selectable.isActiveAndEnabled || !selectable.interactable)
                continue;

            closestHit = hit;
            return selectable;
        }

        return null;
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
        }
    }

    private void HandleTriggerPress(GameObject hoverTarget)
    {
        bool isTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OvrController) >= triggerThreshold;

        if (isTriggerPressed && !wasTriggerPressed && hoverTarget != null)
        {
            PointerEventData eventData = CreatePointerEventData();
            Debug.Log($"{nameof(MainMenuVRLaserPointer)} clicked '{hoverTarget.name}' with {controllerHand} controller.");

            TMP_InputField inputField = hoverTarget.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                if (eventSystem == null)
                    eventSystem = EventSystem.current;

                eventSystem?.SetSelectedGameObject(hoverTarget, eventData);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.selectHandler);
                inputField.ActivateInputField();
            }
            else
            {
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(hoverTarget, eventData, ExecuteEvents.selectHandler);
            }
        }

        wasTriggerPressed = isTriggerPressed;
    }

    private PointerEventData CreatePointerEventData()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        return new PointerEventData(eventSystem)
        {
            button = PointerEventData.InputButton.Left,
            pointerId = PointerId
        };
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
        if (currentHoverObject == null)
            return;

        ExecuteEvents.ExecuteHierarchy(currentHoverObject, CreatePointerEventData(), ExecuteEvents.pointerExitHandler);
        currentHoverObject = null;
    }
}
