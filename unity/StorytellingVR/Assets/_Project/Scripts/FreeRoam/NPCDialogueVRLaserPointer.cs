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

    private enum RayTargetType
    {
        None,
        Button,
        InspectableItem
    }

    private struct RayTarget
    {
        public RayTargetType targetType;
        public GameObject targetObject;
        public Button button;
        public MeeraInspectableItem inspectableItem;
        public RaycastHit hitInfo;

        public bool HasTarget =>
            targetType != RayTargetType.None &&
            targetObject != null;
    }

    [Header("Controller")]
    [SerializeField]
    private ControllerHand controllerHand =
        ControllerHand.Right;

    [SerializeField]
    private float triggerThreshold = 0.75f;

    [Header("Ray")]
    [SerializeField]
    private float maxDistance = 8f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private Color idleColor =
        new Color(1f, 0.95f, 0.6f, 0.9f);

    [SerializeField]
    private Color hoverColor = Color.white;

    private EventSystem eventSystem;
    private LineRenderer lineRenderer;

    // UI hover is kept separate because inspectable objects
    // do not use Unity pointer enter/exit events.
    private GameObject currentHoverButtonObject;

    private bool wasTriggerPressed;

    private OVRInput.Controller OvrController =>
        controllerHand == ControllerHand.Left
            ? OVRInput.Controller.LTouch
            : OVRInput.Controller.RTouch;

    private int PointerId =>
        controllerHand == ControllerHand.Left
            ? -101
            : -102;

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

        ClearButtonHover();

        // Prevent a trigger that is already being held from
        // immediately clicking something when the ray enables.
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
            ClearButtonHover();
            return;
        }

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RayTarget rayTarget =
            FindClosestInteractiveTarget(origin, direction);

        UpdateButtonHover(rayTarget);

        Vector3 lineEnd =
            rayTarget.HasTarget
                ? rayTarget.hitInfo.point
                : origin + direction * maxDistance;

        UpdateLine(
            origin,
            lineEnd,
            rayTarget.HasTarget
        );

        HandleTriggerPress(rayTarget);
    }

    private void OnDisable()
    {
        HideLine();
        ClearButtonHover();

        wasTriggerPressed = false;
    }

    private bool IsControllerConnected()
    {
        OVRInput.Controller connectedControllers =
            OVRInput.GetConnectedControllers();

        return
            (connectedControllers & OvrController) ==
            OvrController;
    }

    private void EnsureLineRenderer()
    {
        GameObject lineObject =
            new GameObject(
                $"{nameof(NPCDialogueVRLaserPointer)}_Line"
            );

        lineObject.transform.SetParent(transform, false);

        lineRenderer =
            lineObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;

        lineRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        lineRenderer.receiveShadows = false;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.numCapVertices = 4;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
        {
            lineRenderer.material =
                new Material(shader);
        }
        else
        {
            Debug.LogWarning(
                "[VR LASER] Sprites/Default shader was not found."
            );
        }

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
            {
                boxCollider =
                    button.gameObject.AddComponent<BoxCollider>();
            }

            Rect rect = rectTransform.rect;

            boxCollider.center =
                new Vector3(
                    rect.center.x,
                    rect.center.y,
                    0f
                );

            boxCollider.size =
                new Vector3(
                    Mathf.Abs(rect.width),
                    Mathf.Abs(rect.height),
                    0.02f
                );

            boxCollider.isTrigger = true;
        }
    }

    private RayTarget FindClosestInteractiveTarget(
        Vector3 origin,
        Vector3 direction
    )
    {
        RayTarget result = new RayTarget
        {
            targetType = RayTargetType.None
        };

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

            if (button != null &&
                button.isActiveAndEnabled &&
                button.interactable)
            {
                result.targetType =
                    RayTargetType.Button;

                result.targetObject =
                    button.gameObject;

                result.button = button;
                result.inspectableItem = null;
                result.hitInfo = hit;

                return result;
            }

            MeeraInspectableItem inspectableItem =
                hit.collider.GetComponentInParent<
                    MeeraInspectableItem
                >();

            if (inspectableItem != null &&
                inspectableItem.isActiveAndEnabled)
            {
                result.targetType =
                    RayTargetType.InspectableItem;

                result.targetObject =
                    inspectableItem.gameObject;

                result.button = null;
                result.inspectableItem =
                    inspectableItem;

                result.hitInfo = hit;

                return result;
            }
        }

        return result;
    }

    private void UpdateButtonHover(RayTarget rayTarget)
    {
        GameObject newHoverButton = null;

        if (rayTarget.targetType ==
            RayTargetType.Button)
        {
            newHoverButton =
                rayTarget.targetObject;
        }

        if (currentHoverButtonObject ==
            newHoverButton)
        {
            return;
        }

        PointerEventData eventData =
            CreatePointerEventData();

        if (currentHoverButtonObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentHoverButtonObject,
                eventData,
                ExecuteEvents.pointerExitHandler
            );
        }

        currentHoverButtonObject =
            newHoverButton;

        if (currentHoverButtonObject != null)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentHoverButtonObject,
                eventData,
                ExecuteEvents.pointerEnterHandler
            );
        }
    }

    private void HandleTriggerPress(
        RayTarget rayTarget
    )
    {
        bool isTriggerPressed =
            OVRInput.Get(
                OVRInput.Axis1D.PrimaryIndexTrigger,
                OvrController
            ) >= triggerThreshold;

        bool triggerPressedThisFrame =
            isTriggerPressed &&
            !wasTriggerPressed;

        if (triggerPressedThisFrame &&
            rayTarget.HasTarget)
        {
            switch (rayTarget.targetType)
            {
                case RayTargetType.Button:
                    ClickButton(
                        rayTarget.targetObject
                    );
                    break;

                case RayTargetType.InspectableItem:
                    InspectItem(
                        rayTarget.inspectableItem
                    );
                    break;
            }
        }

        wasTriggerPressed =
            isTriggerPressed;
    }

    private void ClickButton(
        GameObject buttonObject
    )
    {
        if (buttonObject == null)
            return;

        PointerEventData eventData =
            CreatePointerEventData();

        ExecuteEvents.ExecuteHierarchy(
            buttonObject,
            eventData,
            ExecuteEvents.pointerClickHandler
        );
    }

    private void InspectItem(
        MeeraInspectableItem inspectableItem
    )
    {
        if (inspectableItem == null)
            return;

        Debug.Log(
            $"[VR LASER] Trigger clicked inspectable item: " +
            $"{inspectableItem.ItemDisplayName}"
        );

        inspectableItem.TryInspect();
    }

    private PointerEventData CreatePointerEventData()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        return new PointerEventData(eventSystem)
        {
            button =
                PointerEventData.InputButton.Left,

            pointerId = PointerId
        };
    }

    private void UpdateLine(
        Vector3 start,
        Vector3 end,
        bool hasInteractiveTarget
    )
    {
        if (lineRenderer == null)
            return;

        // Keeps the laser hidden unless it is pointing
        // at a valid button or inspectable object.
        lineRenderer.enabled =
            hasInteractiveTarget;

        if (!hasInteractiveTarget)
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

    private void ClearButtonHover()
    {
        if (currentHoverButtonObject == null)
            return;

        ExecuteEvents.ExecuteHierarchy(
            currentHoverButtonObject,
            CreatePointerEventData(),
            ExecuteEvents.pointerExitHandler
        );

        currentHoverButtonObject = null;
    }
}