using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;
    private Vector3 exitTarget;

    private bool goingToStall = false;
    private bool waiting = false;
    private Quaternion stallRotation;
    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float detectionDistance = 1.0f;

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
    Quaternion stallRot)
    {
        target = destination;
        goingToStall = stopAtStall;
        exitTarget = leaveDestination;
        stallRotation = stallRot;
    }

    void Update()
    {
        if (waiting)
            return;
        RaycastHit hit;

        if (Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            transform.forward,
            out hit,
            detectionDistance))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                return;
            }
        }
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

        yield return new WaitForSeconds(10f);

        if (animator != null)
        {
            animator.SetFloat("Speed", 1f);
        }

        target = exitTarget;

        waiting = false;
    }
}