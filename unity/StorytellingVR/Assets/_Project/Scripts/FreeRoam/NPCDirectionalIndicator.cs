using UnityEngine;

public class NPCDirectionalIndicator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform npcTarget;
    [SerializeField] private Camera playerCamera;

    [Header("Indicators")]
    [Tooltip("Particle system positioned above the NPC.")]
    [SerializeField] private GameObject worldIndicator;

    [Tooltip("Duplicated particle system parented to CenterEyeAnchor.")]
    [SerializeField] private ParticleSystem edgeIndicator;

    [Header("Edge Positions - Camera Local Space")]
    [SerializeField]
    private Vector3 leftEdgePosition =
        new Vector3(-0.42f, 0f, 1f);

    [SerializeField]
    private Vector3 rightEdgePosition =
        new Vector3(0.42f, 0f, 1f);

    [Header("Visibility")]
    [Range(0f, 0.25f)]
    [SerializeField] private float horizontalViewportPadding = 0.08f;

    [Range(0f, 0.25f)]
    [SerializeField] private float verticalViewportPadding = 0.10f;

    [Header("Motion")]
    [SerializeField] private float edgeMovementSpeed = 8f;

    [Tooltip("Particles move inward toward the middle of the view.")]
    [SerializeField] private float horizontalParticleSpeed = 0.12f;

    private Transform edgeTransform;
    private ParticleSystem.VelocityOverLifetimeModule edgeVelocity;
    private bool edgeVisible;

    private void Awake()
    {
        if (edgeIndicator != null)
        {
            edgeTransform = edgeIndicator.transform;
            edgeVelocity = edgeIndicator.velocityOverLifetime;
            edgeVelocity.enabled = true;

            HideEdgeIndicatorImmediately();
        }
    }

    private void LateUpdate()
    {
        if (npcTarget == null ||
            playerCamera == null ||
            edgeIndicator == null)
        {
            return;
        }

        Vector3 viewportPoint =
            playerCamera.WorldToViewportPoint(npcTarget.position);

        bool targetIsInFront = viewportPoint.z > 0f;

        bool targetIsOnScreen =
            targetIsInFront &&
            viewportPoint.x > horizontalViewportPadding &&
            viewportPoint.x < 1f - horizontalViewportPadding &&
            viewportPoint.y > verticalViewportPadding &&
            viewportPoint.y < 1f - verticalViewportPadding;

        if (targetIsOnScreen)
        {
            ShowWorldIndicator();
            HideEdgeIndicator();
            return;
        }

        HideWorldIndicator();

        bool targetIsOnLeft = IsTargetOnLeftSide(viewportPoint);

        ShowEdgeIndicator(targetIsOnLeft);
    }

    private bool IsTargetOnLeftSide(Vector3 viewportPoint)
    {
        // Normal case: target is somewhere beside the visible viewport.
        if (viewportPoint.z > 0f)
            return viewportPoint.x < 0.5f;

        // If target is behind the player, viewport values can be reversed.
        // Use the camera's right direction instead.
        Vector3 directionToTarget =
            (npcTarget.position - playerCamera.transform.position).normalized;

        float side =
            Vector3.Dot(playerCamera.transform.right, directionToTarget);

        return side < 0f;
    }

    private void ShowWorldIndicator()
    {
        if (worldIndicator != null && !worldIndicator.activeSelf)
            worldIndicator.SetActive(true);
    }

    private void HideWorldIndicator()
    {
        if (worldIndicator != null && worldIndicator.activeSelf)
            worldIndicator.SetActive(false);
    }

    private void ShowEdgeIndicator(bool onLeft)
    {
        if (!edgeVisible)
        {
            edgeVisible = true;
            edgeIndicator.gameObject.SetActive(true);
            edgeIndicator.Play(true);
        }

        Vector3 targetPosition =
            onLeft ? leftEdgePosition : rightEdgePosition;

        edgeTransform.localPosition = Vector3.Lerp(
            edgeTransform.localPosition,
            targetPosition,
            edgeMovementSpeed * Time.unscaledDeltaTime
        );

        // Left edge particles drift right, toward the centre.
        // Right edge particles drift left, toward the centre.
        float inwardSpeed =
            onLeft
                ? horizontalParticleSpeed
                : -horizontalParticleSpeed;

        edgeVelocity.x = new ParticleSystem.MinMaxCurve(inwardSpeed);
    }

    private void HideEdgeIndicator()
    {
        if (!edgeVisible)
            return;

        edgeVisible = false;

        edgeIndicator.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        edgeIndicator.gameObject.SetActive(false);
    }

    private void HideEdgeIndicatorImmediately()
    {
        edgeVisible = false;

        edgeIndicator.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        edgeIndicator.gameObject.SetActive(false);
    }

    public void Show()
    {
        enabled = true;
    }

    public void Hide()
    {
        enabled = false;

        HideWorldIndicator();
        HideEdgeIndicator();
    }
}