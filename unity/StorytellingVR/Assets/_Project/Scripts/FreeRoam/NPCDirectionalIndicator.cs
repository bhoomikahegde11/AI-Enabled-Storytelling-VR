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
    [Range(0.01f, 0.5f)]
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

    // Cached constant-mode Y and Z curves so that all three axes stay
    // in the same ParticleSystemCurveMode when X is updated each frame.
    private ParticleSystem.MinMaxCurve edgeVelocityY;
    private ParticleSystem.MinMaxCurve edgeVelocityZ;

    private void Awake()
    {
        if (edgeIndicator != null)
        {
            edgeTransform = edgeIndicator.transform;
            edgeVelocity = edgeIndicator.velocityOverLifetime;
            edgeVelocity.enabled = true;

            // Force Y and Z into Constant mode and cache their values.
            // This guarantees all three axes share the same curve mode
            // when X is reassigned in ShowEdgeIndicator(), preventing
            // "Particle Velocity curves must all be in the same mode".
            edgeVelocityY = new ParticleSystem.MinMaxCurve(edgeVelocity.y.constant);
            edgeVelocityZ = new ParticleSystem.MinMaxCurve(edgeVelocity.z.constant);
            edgeVelocity.y = edgeVelocityY;
            edgeVelocity.z = edgeVelocityZ;
        }

        HideWorldIndicator();

        if (edgeIndicator != null)
            HideEdgeIndicatorImmediately();

        // Do not run LateUpdate until an NPC explicitly enables this indicator.
        enabled = false;
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

        // Write all three axes together so they remain in the same curve mode.
        edgeVelocity.x = new ParticleSystem.MinMaxCurve(inwardSpeed);
        edgeVelocity.y = edgeVelocityY;
        edgeVelocity.z = edgeVelocityZ;
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

    public void SetTarget(Transform target, GameObject newWorldIndicator)
    {
        // Hide the previous NPC's world marker first.
        HideWorldIndicator();

        if (edgeIndicator != null)
            HideEdgeIndicatorImmediately();

        npcTarget = target;
        worldIndicator = newWorldIndicator;

        // Ensure the newly selected world marker begins hidden.
        HideWorldIndicator();

        Debug.Log($"[EPILOGUE INDICATOR] Target = {(target != null ? target.name : "NULL")}, " +
                  $"World marker = {(newWorldIndicator != null ? newWorldIndicator.name : "NULL")}, " +
                  $"Edge particle = {(edgeIndicator != null ? edgeIndicator.gameObject.name : "NULL")}");
    }

    public void Show()
    {
        if (npcTarget == null)
        {
            Debug.LogWarning(
                "[DIRECTIONAL INDICATOR] Cannot show without a target."
            );

            return;
        }

        Debug.Log($"[EPILOGUE INDICATOR] Show — target={npcTarget.name}, " +
                  $"playerCamera={(playerCamera != null ? playerCamera.name : "NULL")}");

        enabled = true;
    }

    public void Hide()
    {
        HideWorldIndicator();

        if (edgeIndicator != null)
            HideEdgeIndicatorImmediately();

        enabled = false;
    }
}