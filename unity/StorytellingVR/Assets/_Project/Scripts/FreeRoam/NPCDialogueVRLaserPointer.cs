using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCDialogueVRLaserPointer : MonoBehaviour
{
    private enum ControllerHand
    {
        Left,
        Right
    }


    [Header("Controller")]
    [SerializeField]
    private ControllerHand controllerHand =
        ControllerHand.Right;

    [SerializeField] private float triggerThreshold = 0.75f;

    [Header("Ray")]
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float rayWidth = 0.01f;

    [SerializeField]
    private Color idleColor =
        new Color(1f, 0.95f, 0.6f, 0.9f);

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

        Debug.Log(
            $"{nameof(NPCDialogueVRLaserPointer)} initialized on " +
            $"'{gameObject.name}' for {controllerHand} controller."
        );
    }

    private void OnEnable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        ClearHover();

        // If trigger is still held from advancing dialogue,
        // do not treat it as an immediate question click.
        wasTriggerPressed =
            OVRInput.Get(
                OVRInput.Axis1D.PrimaryIndexTrigger,
                OvrController
            ) >= triggerThreshold;
    }

    private void Update()
    {
        EnsureButtonColliders();

        if (!IsControllerConnected())
        {
            HideLine();
            ClearHover();
            return;
        }

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Button hitButton = FindButtonHit(
            origin,
            direction,
            out RaycastHit hitInfo
        );

        GameObject hoverTarget =
            hitButton != null ? hitButton.gameObject : null;

        UpdateHoverTarget(hoverTarget);

        UpdateLine(
            origin,
            hitButton != null ? hitInfo.point : origin,
            hoverTarget != null
        );

        HandleTriggerPress(hoverTarget);
    }

    private void OnDisable()
    {
        HideLine();
        ClearHover();
        wasTriggerPressed = false;
    }

    private bool IsControllerConnected()
    {
        OVRInput.Controller connectedControllers =
            OVRInput.GetConnectedControllers();

        return (connectedControllers & OvrController) == OvrController;
    }

    private void EnsureLineRenderer()
    {
        GameObject lineObject =
            new GameObject(
                $"{nameof(NPCDialogueVRLaserPointer)}_Line"
            );

        lineObject.transform.SetParent(transform, false);

        lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.numCapVertices = 4;
        lineRenderer.material =
            new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = idleColor;
        lineRenderer.endColor = idleColor;
        lineRenderer.enabled = false;
    }

    private void EnsureButtonColliders()
    {
        

        Button[] buttons =
            Object.FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Button button in buttons)
        {
            RectTransform rectTransform =
                button.GetComponent<RectTransform>();

            if (rectTransform == null)
                continue;

            BoxCollider boxCollider =
                button.GetComponent<BoxCollider>();

            if (boxCollider == null)
                boxCollider =
                    button.gameObject.AddComponent<BoxCollider>();

            Rect rect = rectTransform.rect;

            boxCollider.center =
                new Vector3(rect.center.x, rect.center.y, 0f);

            boxCollider.size =
                new Vector3(
                    Mathf.Abs(rect.width),
                    Mathf.Abs(rect.height),
                    0.02f
                );

            boxCollider.isTrigger = true;
        }

    }

    private Button FindButtonHit(
        Vector3 origin,
        Vector3 direction,
        out RaycastHit closestHit)
    {
        closestHit = default;

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            direction,
            maxDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        System.Array.Sort(
            hits,
            (left, right) =>
                left.distance.CompareTo(right.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            Button button =
                hit.collider.GetComponentInParent<Button>();

            if (button == null ||
                !button.isActiveAndEnabled ||
                !button.interactable)
            {
                continue;
            }

            closestHit = hit;
            return button;
        }

        return null;
    }

    private void UpdateHoverTarget(GameObject newHoverObject)
    {
        if (currentHoverObject == newHoverObject)
            return;

        PointerEventData eventData =
            CreatePointerEventData();

        if (currentHoverObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentHoverObject,
                eventData,
                ExecuteEvents.pointerExitHandler
            );
        }

        currentHoverObject = newHoverObject;

        if (currentHoverObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentHoverObject,
                eventData,
                ExecuteEvents.pointerEnterHandler
            );
        }
    }

    private void HandleTriggerPress(GameObject hoverTarget)
    {
        bool isTriggerPressed =
            OVRInput.Get(
                OVRInput.Axis1D.PrimaryIndexTrigger,
                OvrController
            ) >= triggerThreshold;

        if (isTriggerPressed &&
            !wasTriggerPressed &&
            hoverTarget != null)
        {
            PointerEventData eventData =
                CreatePointerEventData();

            ExecuteEvents.ExecuteHierarchy(
                hoverTarget,
                eventData,
                ExecuteEvents.pointerClickHandler
            );
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

    private void UpdateLine(
        Vector3 start,
        Vector3 end,
        bool isHoveringButton)
    {
        if (lineRenderer == null)
            return;

        lineRenderer.enabled = isHoveringButton;

        if (!isHoveringButton)
            return;

        lineRenderer.startColor = hoverColor;
        lineRenderer.endColor = hoverColor;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void HideLine()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private void ClearHover()
    {
        if (currentHoverObject == null)
            return;

        ExecuteEvents.ExecuteHierarchy(
            currentHoverObject,
            CreatePointerEventData(),
            ExecuteEvents.pointerExitHandler
        );

        currentHoverObject = null;
    }
}