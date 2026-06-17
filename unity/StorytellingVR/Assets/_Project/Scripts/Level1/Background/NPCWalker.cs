using UnityEngine;
using System.Collections;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;
    private Vector3 exitTarget;

    private bool goingToStall = false;
    private bool waiting = false;

    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.speed = 1f;
        }
    }

    public void Initialize(
        Vector3 destination,
        bool stopAtStall,
        Vector3 leaveDestination)
    {
        target = destination;
        goingToStall = stopAtStall;
        exitTarget = leaveDestination;
    }

    void Update()
    {
        if (waiting)
            return;

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.4f)
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
        waiting = true;

        if (animator != null)
        {
            animator.speed = 0f;
        }

        yield return new WaitForSeconds(10f);

        if (animator != null)
        {
            animator.speed = 1f;
        }

        target = exitTarget;

        waiting = false;
    }
}