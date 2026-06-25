using UnityEngine;
using UnityEngine.XR;

public class ControllerRayInteractor : MonoBehaviour
{
    public float rayLength = 8f;
    public Color normalColor = new Color(1f, 0.86f, 0.35f, 0.85f);
    public Color hitColor = new Color(0.2f, 0.9f, 1f, 1f);
    public LayerMask interactionMask = ~0;
    public float hoverHapticAmplitude = 0.18f;
    public float hoverHapticDuration = 0.035f;
    public float clickHapticAmplitude = 0.45f;
    public float clickHapticDuration = 0.08f;

    private InputDevice rightHandDevice;
    private LineRenderer lineRenderer;
    private bool wasTriggerPressed;
    private SilkTraderQuestionHitbox hoveredHitbox;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.012f;
        lineRenderer.endWidth = 0.004f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
    }

    private void Update()
    {
        EnsureRightHandDevice();

        Ray ray = new Ray(transform.position, transform.forward);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, rayLength, interactionMask, QueryTriggerInteraction.Collide);
        Vector3 endPoint = hasHit ? hit.point : ray.origin + ray.direction * rayLength;
        SilkTraderQuestionHitbox hitQuestion = hasHit ? hit.collider.GetComponentInParent<SilkTraderQuestionHitbox>() : null;

        if (hoveredHitbox != hitQuestion)
        {
            if (hoveredHitbox != null)
            {
                hoveredHitbox.SetHovered(false);
            }

            hoveredHitbox = hitQuestion;

            if (hoveredHitbox != null)
            {
                hoveredHitbox.SetHovered(true);
                SendHapticImpulse(hoverHapticAmplitude, hoverHapticDuration);
            }
        }

        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.startColor = hasHit ? hitColor : normalColor;
        lineRenderer.endColor = hasHit ? hitColor : normalColor;

        bool triggerPressed = false;
        if (rightHandDevice.isValid)
        {
            rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);
        }

        if (triggerPressed && !wasTriggerPressed && hasHit)
        {
            SendHapticImpulse(clickHapticAmplitude, clickHapticDuration);
            OpenHitTarget(hit.collider);
        }

        wasTriggerPressed = triggerPressed;
    }

    private void EnsureRightHandDevice()
    {
        if (!rightHandDevice.isValid)
        {
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
    }

    private static void OpenHitTarget(Collider hitCollider)
    {
        SilkTraderQuestionHitbox questionHitbox = hitCollider.GetComponentInParent<SilkTraderQuestionHitbox>();
        if (questionHitbox != null)
        {
            questionHitbox.SelectQuestion();
            return;
        }

        NPCInteractionVR vrInteraction = hitCollider.GetComponentInParent<NPCInteractionVR>();
        if (vrInteraction != null)
        {
            vrInteraction.OpenDialogue();
            return;
        }

        NPCInteraction interaction = hitCollider.GetComponentInParent<NPCInteraction>();
        if (interaction != null)
        {
            interaction.OpenDialogue();
        }
    }

    private void SendHapticImpulse(float amplitude, float duration)
    {
        EnsureRightHandDevice();
        if (rightHandDevice.isValid)
        {
            rightHandDevice.SendHapticImpulse(0u, amplitude, duration);
        }
    }
}
