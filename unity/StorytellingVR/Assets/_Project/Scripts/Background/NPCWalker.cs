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
    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float avoidanceRadius = 0.8f;
    [SerializeField] private float avoidanceStrength = 0.5f;

    private float waitTime;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetFloat("Speed", 1f);
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
        waitTime =
    Random.Range(6f, 14f);
    }

    void Update()
    {
        if (waiting)
            return;
        RaycastHit hit;

        if (Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            transform.forward,
            out hit))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                return;
            }
        }
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        // Avoid nearby NPCs
        Collider[] nearbyNPCs = Physics.OverlapSphere(
            transform.position,
            avoidanceRadius
        );

        Vector3 avoidance = Vector3.zero;

        foreach (Collider c in nearbyNPCs)
        {
            if (c.gameObject == gameObject)
                continue;

            if (c.CompareTag("NPC"))
            {
                Vector3 away =
                    transform.position -
                    c.transform.position;

                away.y = 0f;

                float distance = away.magnitude;

                if (distance > 0.01f)
                {
                    avoidance +=
                        away.normalized / distance;
                }
            }
        }

        // Blend movement toward target with avoidance
        direction += avoidance * avoidanceStrength;

        direction.y = 0f;
        if (direction.magnitude < 0.05f)
        {
            if (goingToStall)
            {
                goingToStall = false;
                StartCoroutine(WaitAtStall());
                return;
            }

            MarketSpawner spawner =
                FindFirstObjectByType<MarketSpawner>();

            if (spawner != null)
            {
                spawner.NPCRemoved();
            }

            Destroy(gameObject);
            return;
        }

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
}