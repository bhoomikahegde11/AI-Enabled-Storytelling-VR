using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class NPCWalker : MonoBehaviour
{
    private const int ObstacleHitBufferSize = 8;
    private const int OverlapHitBufferSize = 8;
    private static readonly RaycastHit[] ObstacleHits = new RaycastHit[ObstacleHitBufferSize];
    private static readonly Collider[] OverlapHits = new Collider[OverlapHitBufferSize];

    private enum ObstacleType
    {
        None,
        BackgroundNpc,
        Caravan,
        PauseBlocker
    }

    private MarketSpawner spawner;
    private Vector3 target;
    private Vector3 exitTarget;
    private StallPoint myStall;
    private Quaternion stallRotation;
    private bool goingToStall = false;
    private bool waiting = false;
    private bool isPaused = false;
    [SerializeField] private float moveSpeed = 1.4f;
    [FormerlySerializedAs("turnSpeed")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float animatorPlaybackSpeedMultiplier = 1.75f;
    [Header("Walking Pauses")]
    [SerializeField] private bool enableWalkingPauses = true;
    [SerializeField] private float minTimeBetweenPauses = 6f;
    [SerializeField] private float maxTimeBetweenPauses = 14f;
    [SerializeField] private float minPauseDuration = 0.5f;
    [SerializeField] private float maxPauseDuration = 1.5f;
    [Range(0f, 1f)]
    [SerializeField] private float pauseChance = 0.35f;
    [SerializeField] private float pauseProximityThreshold = 1.25f;
    [Header("Movement Collision")]
    [SerializeField] private float obstacleCheckRadius = 0.35f;
    [SerializeField] private float obstacleCheckDistance = 0.8f;
    [SerializeField] private float blockedPauseDuration = 0.25f;
    [SerializeField] private LayerMask obstacleLayerMask = ~0;
    [SerializeField] private float maxAvoidanceAngle = 40f;
    [SerializeField] private float avoidanceStrength = 0.9f;


    private float waitTime;
    private float speedMultiplier = 1f;
    private float pauseTimer;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetFloat("Speed", 1f);
            animator.speed = GetWalkingAnimatorSpeed();
        }

        ResetPauseTimer();
    }

    public void Initialize(
    MarketSpawner ownerSpawner,
    Vector3 destination,
    bool stopAtStall,
    Vector3 leaveDestination,
    Quaternion stallRot,
    StallPoint stall,
    float chosenSpeedMultiplier,
    float chosenWaitTime)
    {
        spawner = ownerSpawner;
        target = destination;
        goingToStall = stopAtStall;
        exitTarget = leaveDestination;

        stallRotation = stallRot;
        myStall = stall;
        speedMultiplier = Mathf.Max(0.01f, chosenSpeedMultiplier);
        waitTime = Mathf.Max(0f, chosenWaitTime);
    }

    void Update()
    {
        if (waiting || isPaused)
            return;

        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.magnitude < 0.05f)
        {
            if (goingToStall)
            {
                goingToStall = false;
                StartCoroutine(WaitAtStall());
                return;
            }

            if (spawner != null)
            {
                spawner.NPCRemoved();
            }

            Destroy(gameObject);
            return;
        }

        TryStartWalkingPause(direction.magnitude);

        if (isPaused)
            return;

        direction.Normalize();

        bool shouldPause;
        Vector3 moveDirection = GetMoveDirection(direction, out shouldPause);

        if (shouldPause)
        {
            StartCoroutine(PauseWhileWalking(blockedPauseDuration));
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.position +=
            transform.forward *
            (moveSpeed * speedMultiplier) *
            Time.deltaTime;
    }

    IEnumerator WaitAtStall()
    {
        transform.position = target;
        transform.rotation = stallRotation;
        waiting = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.speed = 1f;
        }

        yield return new WaitForSeconds(waitTime);

        if (myStall != null)
        {
            myStall.occupied = false;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
            animator.speed = GetWalkingAnimatorSpeed();
        }

        target = exitTarget;

        waiting = false;
    }

    IEnumerator PauseWhileWalking(float pauseDuration)
    {
        isPaused = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.speed = 1f;
        }

        yield return new WaitForSeconds(pauseDuration);

        if (animator != null && !waiting)
        {
            animator.SetFloat("Speed", 1f);
            animator.speed = GetWalkingAnimatorSpeed();
        }

        isPaused = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            1.2f
        );
    }

    private float GetWalkingAnimatorSpeed()
    {
        return Mathf.Max(
            0.1f,
            animatorPlaybackSpeedMultiplier * speedMultiplier
        );
    }

    private void TryStartWalkingPause(float distanceToTarget)
    {
        if (!enableWalkingPauses || waiting || isPaused)
            return;

        if (distanceToTarget <= pauseProximityThreshold)
            return;

        pauseTimer -= Time.deltaTime;

        if (pauseTimer > 0f)
            return;

        ResetPauseTimer();

        if (Random.value > pauseChance)
            return;

        float pauseDuration = Random.Range(
            Mathf.Min(minPauseDuration, maxPauseDuration),
            Mathf.Max(minPauseDuration, maxPauseDuration)
        );

        StartCoroutine(PauseWhileWalking(pauseDuration));
    }

    private void ResetPauseTimer()
    {
        pauseTimer = Random.Range(
            Mathf.Min(minTimeBetweenPauses, maxTimeBetweenPauses),
            Mathf.Max(minTimeBetweenPauses, maxTimeBetweenPauses)
        );
    }

    private Vector3 GetMoveDirection(
        Vector3 desiredDirection,
        out bool shouldPause)
    {
        shouldPause = false;
        ObstacleType obstacleType = DetectObstacle(
            desiredDirection,
            out Vector3 obstaclePosition
        );

        if (obstacleType == ObstacleType.None)
            return desiredDirection;

        if (obstacleType == ObstacleType.PauseBlocker)
        {
            shouldPause = true;
            return desiredDirection;
        }

        Vector3 toObstacle = obstaclePosition - transform.position;
        toObstacle.y = 0f;

        Vector3 sideDirection =
            Vector3.Dot(transform.right, toObstacle) >= 0f
            ? -transform.right
            : transform.right;

        Vector3 blendedDirection =
            desiredDirection +
            sideDirection * Mathf.Max(0.1f, avoidanceStrength);

        if (blendedDirection.sqrMagnitude <= 0.001f)
            return desiredDirection;

        Vector3 steeredDirection = Vector3.RotateTowards(
            desiredDirection,
            blendedDirection.normalized,
            Mathf.Deg2Rad * Mathf.Max(0f, maxAvoidanceAngle),
            0f
        );

        return steeredDirection.normalized;
    }

    private ObstacleType DetectObstacle(
        Vector3 moveDirection,
        out Vector3 obstaclePosition)
    {
        obstaclePosition = Vector3.zero;

        if (obstacleCheckRadius <= 0f || obstacleCheckDistance <= 0f)
            return ObstacleType.None;

        Vector3 origin = transform.position + Vector3.up * 0.9f;
        ObstacleType bestType = ObstacleType.None;
        float bestScore = float.MaxValue;

        int overlapCount = Physics.OverlapSphereNonAlloc(
            origin,
            obstacleCheckRadius,
            OverlapHits,
            obstacleLayerMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < overlapCount; i++)
        {
            Collider hitCollider = OverlapHits[i];
            ObstacleType obstacleType = ClassifyObstacle(hitCollider);

            if (obstacleType == ObstacleType.None)
                continue;

            Vector3 candidatePosition =
                hitCollider.ClosestPoint(origin);

            float candidateScore =
                (candidatePosition - origin).sqrMagnitude;

            if (candidateScore < bestScore)
            {
                bestScore = candidateScore;
                bestType = obstacleType;
                obstaclePosition = candidatePosition;
            }
        }

        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            obstacleCheckRadius,
            moveDirection,
            ObstacleHits,
            obstacleCheckDistance + obstacleCheckRadius,
            obstacleLayerMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = ObstacleHits[i].collider;
            ObstacleType obstacleType = ClassifyObstacle(hitCollider);

            if (obstacleType == ObstacleType.None)
                continue;

            float candidateScore = ObstacleHits[i].distance;

            if (candidateScore < bestScore)
            {
                bestScore = candidateScore;
                bestType = obstacleType;
                obstaclePosition = ObstacleHits[i].point;
            }
        }

        return bestType;
    }

    private ObstacleType ClassifyObstacle(Collider hitCollider)
    {
        if (hitCollider == null)
            return ObstacleType.None;

        if (hitCollider.transform.root == transform.root)
            return ObstacleType.None;

        if (hitCollider.GetComponentInParent<NPCWalker>() != null)
            return ObstacleType.BackgroundNpc;

        if (hitCollider.GetComponentInParent<CaravanWalker>() != null)
            return ObstacleType.Caravan;

        return ObstacleType.PauseBlocker;
    }
}
