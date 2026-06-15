using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;

    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;

    private Animator animator;

    public void Initialize(Vector3 destination)
    {
        target = destination;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetFloat("Speed", 1f);
        }
    }

    void Update()
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.2f)
        {
            Destroy(gameObject);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}