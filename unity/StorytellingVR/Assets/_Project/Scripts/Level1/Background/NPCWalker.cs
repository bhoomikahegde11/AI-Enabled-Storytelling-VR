using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;
    private Vector3 exitTarget;
    private StallPoint myStall;
    private Quaternion stallRotation;
    private bool goingToStall = false;
    private bool waiting = false;
    private FreeRoamMarketSpawner freeRoamSpawner;
    private Collider avoidanceNPC;
    private Vector3 passingDirection;
    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private Transform visualAnchor;


    private float waitTime;
    private Animator animator;

    public Transform VisualAnchor => visualAnchor != null ? visualAnchor : transform;

    void Start()
    {
        if (animator == null)
        {
            animator = visualAnchor != null
                ? visualAnchor.GetComponentInChildren<Animator>(true)
                : GetComponent<Animator>();
        }
        freeRoamSpawner =
            FindFirstObjectByType<FreeRoamMarketSpawner>();

        ConfigureAnimator();
        Debug.Log($"[BG NPC] Walker movement started: {name}.", gameObject);
    }

    public void SetVisualAnimator(Animator visualAnimator)
    {
        animator = visualAnimator;
        ConfigureAnimator();
    }

    private void ConfigureAnimator()
    {
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetFloat("Speed", waiting ? 0f : 1f);
        }
    }

    public void Initialize(
    Vector3 destination,
    bool stopAtStall,
    Vector3 leaveDestination,
    Quaternion stallRot,
    StallPoint stall)
    {
        target = destination;
        goingToStall = stopAtStall;
        exitTarget = leaveDestination;

        stallRotation = stallRot;
        myStall = stall;
        avoidanceNPC = null;
        passingDirection = Vector3.zero;
        waitTime =
    Random.Range(6f, 14f);
    }

    void Update()
    {
        if (waiting)
            return;

        if (!TryGetMovementDirection(out Vector3 direction))
        {
            if (goingToStall)
            {
                goingToStall = false;
                StartCoroutine(WaitAtStall());
                return;
            }

            if (freeRoamSpawner != null)
            {
                freeRoamSpawner.NPCRemoved();
            }
            else
            {
                MarketSpawner spawner =
                FindFirstObjectByType<MarketSpawner>();

                if (spawner != null)
                {
                    spawner.NPCRemoved();
                }
            }

            Destroy(gameObject);
            return;
        }
        direction.Normalize();
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position +=
            transform.forward *
            moveSpeed *
            Time.deltaTime;
    }

    private bool TryGetMovementDirection(out Vector3 direction)
    {
        direction = target - transform.position;
        direction.y = 0f;

        // Arrival belongs to the assigned target, never to avoidance steering.
        if (direction.magnitude < 0.05f)
        {
            avoidanceNPC = null;
            passingDirection = Vector3.zero;
            return false;
        }

        Vector3 rawTargetDirection = direction;
        Vector3 toOther = Vector3.zero;
        if (avoidanceNPC != null)
        {
            toOther = avoidanceNPC.transform.position - transform.position;
            toOther.y = 0f;
            if (!avoidanceNPC.enabled || !avoidanceNPC.gameObject.activeInHierarchy ||
                toOther.magnitude >= 1.2f)
            {
                avoidanceNPC = null;
                passingDirection = Vector3.zero;
            }
        }

        if (avoidanceNPC == null)
        {
            passingDirection = Vector3.zero;
            float closestDistance = 0.8f;
            foreach (Collider c in Physics.OverlapSphere(transform.position, 1.2f))
            {
                if (c.transform.IsChildOf(transform) || !c.CompareTag("NPC"))
                    continue;

                Vector3 offset = c.transform.position - transform.position;
                offset.y = 0f;
                float distance = offset.magnitude;
                if (distance < closestDistance &&
                    Vector3.Dot(transform.forward, offset.normalized) > 0.3f)
                {
                    avoidanceNPC = c;
                    toOther = offset;
                    closestDistance = distance;
                }
            }

            if (avoidanceNPC != null)
            {
                // Freeze world-space left for this encounter. Opposing walkers
                // see opposite separation vectors, so pass on opposite sides.
                passingDirection = Vector3.Cross(toOther.normalized, Vector3.up);
            }
        }

        if (avoidanceNPC != null && toOther.magnitude < 0.8f)
        {
            direction += passingDirection * 2f;
            direction -= toOther.normalized;
            if (direction.sqrMagnitude < 0.000001f)
                direction = rawTargetDirection;
        }

        return true;
    }

    IEnumerator WaitAtStall()
    {
        transform.position = target;
        transform.rotation = stallRotation;
        waiting = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }

        yield return new WaitForSeconds(waitTime);

        if (myStall != null)
        {
            myStall.occupied = false;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }

        target = exitTarget;

        waiting = false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            1.2f
        );
    }
}
