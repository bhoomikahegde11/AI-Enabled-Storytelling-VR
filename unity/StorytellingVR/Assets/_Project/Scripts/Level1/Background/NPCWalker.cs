using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class NPCWalker : MonoBehaviour
{
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
        Collider[] nearby = Physics.OverlapSphere(
    transform.position,
    1.2f
);

        foreach (Collider c in nearby)
        {
            if (c.gameObject == gameObject)
                continue;

            if (c.CompareTag("NPC"))
            {
                Vector3 toOther =
                    c.transform.position - transform.position;

                toOther.y = 0f;

                float dot =
                    Vector3.Dot(
                        transform.forward,
                        toOther.normalized
                    );

                // Only avoid NPCs roughly in front
                float distance = toOther.magnitude;

                if (dot > 0.3f && distance < 0.8f)
                {
                    Vector3 sideStep = -transform.right;

                    direction += sideStep * 2f;
                    direction -= toOther.normalized * 1f;
                }
            }
        }
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
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

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
}
