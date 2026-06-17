using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;
    private float speed = 1.5f;

    public void Initialize(Vector3 destination)
    {
        target = destination;

        Vector3 lookPos = target;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}