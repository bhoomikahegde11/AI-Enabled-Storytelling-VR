using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanWalker : MonoBehaviour
{
    private const int ObstacleHitBufferSize = 8;
    private static readonly RaycastHit[] ObstacleHits = new RaycastHit[ObstacleHitBufferSize];
    private const float NpcIgnoreDuration = 1f;

    private enum ObstacleType
    {
        None,
        BackgroundNpc,
        PauseBlocker
    }

    [Header("Movement")]
    [SerializeField] private float speed = 1.8f;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private float stopDistance = 0.35f;
    [Header("Movement Collision")]
    [SerializeField] private float obstacleCheckRadius = 0.9f;
    [SerializeField] private float obstacleCheckDistance = 1.8f;
    [SerializeField] private float blockedPauseDuration = 0.35f;
    [SerializeField] private LayerMask obstacleLayerMask = ~0;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource;

    private CaravanSpawner spawner;
    private List<Transform> routePoints = new List<Transform>();
    private int currentRouteIndex = 0;
    private bool isInitialized = false;
    private bool isBlockedPause = false;
    private float npcIgnoreUntil = -1f;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Initialize(
        CaravanSpawner ownerSpawner,
        List<Transform> assignedRoute)
    {
        spawner = ownerSpawner;
        routePoints = assignedRoute != null
            ? new List<Transform>(assignedRoute)
            : new List<Transform>();

        currentRouteIndex = 0;
        isInitialized = routePoints.Count > 0;

        if (isInitialized && routePoints[0] != null)
        {
            Vector3 direction =
                routePoints[0].position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction.normalized);
            }
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!isInitialized || isBlockedPause)
            return;

        if (currentRouteIndex >= routePoints.Count)
        {
            FinishRoute();
            return;
        }

        Transform currentPoint = routePoints[currentRouteIndex];

        if (currentPoint == null)
        {
            currentRouteIndex++;
            return;
        }

        Vector3 direction =
            currentPoint.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= Mathf.Max(0.01f, stopDistance))
        {
            currentRouteIndex++;
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        ObstacleType obstacleType = DetectObstacle(direction.normalized);

        if (obstacleType == ObstacleType.BackgroundNpc)
        {
            if (Time.time >= npcIgnoreUntil)
            {
                npcIgnoreUntil = Time.time + NpcIgnoreDuration;
                StartCoroutine(PauseMovement(blockedPauseDuration));
                return;
            }
        }
        else
        {
            npcIgnoreUntil = -1f;
        }

        if (obstacleType == ObstacleType.PauseBlocker)
        {
            StartCoroutine(PauseMovement(blockedPauseDuration));
            return;
        }

        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;
    }

    private void FinishRoute()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (spawner != null)
        {
            spawner.CaravanRemoved();
        }

        Destroy(gameObject);
    }

    private IEnumerator PauseMovement(float duration)
    {
        isBlockedPause = true;
        yield return new WaitForSeconds(duration);
        isBlockedPause = false;
    }

    private ObstacleType DetectObstacle(Vector3 moveDirection)
    {
        if (obstacleCheckRadius <= 0f || obstacleCheckDistance <= 0f)
            return ObstacleType.None;

        Vector3 origin = transform.position + Vector3.up * 1.2f;

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            obstacleCheckRadius,
            moveDirection,
            ObstacleHits,
            obstacleCheckDistance,
            obstacleLayerMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = ObstacleHits[i].collider;

            if (hitCollider == null)
                continue;

            if (hitCollider.transform.root == transform.root)
                continue;

            if (hitCollider.GetComponentInParent<NPCWalker>() != null)
                return ObstacleType.BackgroundNpc;

            return ObstacleType.PauseBlocker;
        }

        return ObstacleType.None;
    }
}
