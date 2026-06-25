using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private MarketSpawner spawner;
    private Vector3 target;
    private Vector3 exitTarget;
    private StallPoint myStall;
    private Quaternion stallRotation;
    private bool goingToStall = false;
    private bool waiting = false;
    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;


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
    MarketSpawner ownerSpawner,
    Vector3 destination,
    bool stopAtStall,
    Vector3 leaveDestination,
    Quaternion stallRot,
    StallPoint stall)
    {
        spawner = ownerSpawner;
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
