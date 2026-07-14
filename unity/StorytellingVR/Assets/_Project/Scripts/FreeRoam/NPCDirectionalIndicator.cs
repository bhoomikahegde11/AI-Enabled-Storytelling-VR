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

    [Header("Edge Placement")]
    [Range(0.01f, 0.3f)]
    [SerializeField] private float screenEdgePadding = 0.1f;

    [SerializeField] private float edgeDepth = 1f;

    [SerializeField] private float edgeVerticalPosition = 0.5f;

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

        float viewportX = onLeft
            ? screenEdgePadding
            : 1f - screenEdgePadding;

        Vector3 viewportPosition = new Vector3(
            viewportX,
            edgeVerticalPosition,
            edgeDepth
        );

        Vector3 targetWorldPosition =
            playerCamera.ViewportToWorldPoint(viewportPosition);

        edgeTransform.position = Vector3.Lerp(
            edgeTransform.position,
            targetWorldPosition,
            edgeMovementSpeed * Time.unscaledDeltaTime
        );

        float inwardSpeed = onLeft
            ? horizontalParticleSpeed
            : -horizontalParticleSpeed;

        edgeVelocity.x =
            new ParticleSystem.MinMaxCurve(inwardSpeed);
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