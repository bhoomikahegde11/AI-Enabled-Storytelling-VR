using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCWalker : MonoBehaviour
{
    private Vector3 target;
    private Vector3 exitTarget;
    private StallPoint myStall;
    private Quaternion stallRotation;
    private bool goingToStall = false;
    private bool waiting = false;
    private bool scriptedMovement;
    private bool walking;
    private FreeRoamMarketSpawner freeRoamSpawner;
    private const float PersonalSpace = 0.68f;
    private const float LookAhead = 1.2f;
    private static readonly List<NPCWalker> ActiveWalkers = new List<NPCWalker>();
    private Transform[] sceneCharacters;
    private Transform avoidanceNPC;
    private Vector3 passingDirection;
    [SerializeField] private float moveSpeed = 0.85f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private Transform visualAnchor;


    private float waitTime;
    private Animator animator;

    public Transform VisualAnchor => visualAnchor != null ? visualAnchor : transform;

    private void OnEnable() => ActiveWalkers.Add(this);
    private void OnDisable() => ActiveWalkers.Remove(this);

    void Start()
    {
        if (scriptedMovement)
            return;
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
            walking = !waiting;
            animator.SetFloat("Speed", walking ? 1f : 0f);
        }
    }

    public void ConfigureScriptedMovement(Vector3 destination, float speed, float rotationSpeed, Transform[] characters)
    {
        scriptedMovement = true;
        target = destination;
        moveSpeed = speed;
        turnSpeed = rotationSpeed;
        sceneCharacters = characters;
    }

    public bool StepScriptedMovement()
    {
        if (!TryGetMovementDirection(out Vector3 direction))
            return false;
        Advance(direction);
        return true;
    }

    public void Initialize(
    Vector3 destination,
    bool stopAtStall,
    Vector3 leaveDestination,
    Quaternion stallRot,
    StallPoint stall,
    Transform[] characters = null)
    {
        target = destination;
        goingToStall = stopAtStall;
        exitTarget = leaveDestination;

        stallRotation = stallRot;
        myStall = stall;
        sceneCharacters = characters;
        avoidanceNPC = null;
        passingDirection = Vector3.zero;
        waitTime = Random.Range(6f, 14f);

        if (characters != null && characters.Length > 0 && !TryGetComponent<NavMeshObstacle>(out _))
        {
            // Let the guide and buyer see these transform-driven walkers without carving their paths.
            var obstacle = gameObject.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.center = Vector3.up * 0.8f;
            obstacle.height = 1.6f;
            obstacle.radius = PersonalSpace * 0.5f;
            obstacle.carving = false;
        }
    }

    void Update()
    {
        if (scriptedMovement || waiting)
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
        Advance(direction);
    }

    private void Advance(Vector3 direction)
    {
        Vector3 step = GetSafeStep(direction, Time.deltaTime);
        SetWalking(step.sqrMagnitude > 0f);
        if (step.sqrMagnitude == 0f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(step);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position += step;
    }

    private void SetWalking(bool moving)
    {
        if (animator != null && walking != moving)
            animator.SetFloat("Speed", moving ? 1f : 0f);
        walking = moving;
    }

    private bool TryGetMovementDirection(out Vector3 direction)
    {
        Vector3 position = transform.position;
        direction = Planar(target - position);

        // Arrival belongs to the assigned target, never to avoidance steering.
        if (direction.magnitude < 0.05f)
        {
            avoidanceNPC = null;
            passingDirection = Vector3.zero;
            return false;
        }

        float distance = direction.magnitude;
        Vector3 towardTarget = direction / distance;
        if (avoidanceNPC != null &&
            (!avoidanceNPC.gameObject.activeInHierarchy ||
             Planar(avoidanceNPC.position - position).sqrMagnitude >= LookAhead * LookAhead ||
             Vector3.Dot(Planar(avoidanceNPC.position - position), towardTarget) < -0.15f))
        {
            avoidanceNPC = null;
            passingDirection = Vector3.zero;
        }

        if (avoidanceNPC == null)
        {
            avoidanceNPC = FindBlocker(towardTarget * Mathf.Min(distance, LookAhead), LookAhead * LookAhead);
            if (avoidanceNPC != null)
            {
                Vector3 toOther = Planar(avoidanceNPC.position - position);
                // Opposing walkers see opposite separation vectors and pick opposite world sides.
                Vector3 approach = toOther.sqrMagnitude > 0.0001f ? toOther.normalized : towardTarget;
                passingDirection = Vector3.Cross(approach, Vector3.up);
            }
        }

        if (avoidanceNPC != null)
            direction = towardTarget + passingDirection * 1.25f;

        return true;
    }

    private Vector3 GetSafeStep(Vector3 direction, float deltaTime)
    {
        float distance = Mathf.Min(moveSpeed * deltaTime, Planar(target - transform.position).magnitude);
        Vector3 step = direction.normalized * distance;
        if (FindBlocker(step) == null)
            return step;
        if (avoidanceNPC == null)
            return Vector3.zero;

        // If forward motion is blocked, step sideways before trying to make progress again.
        step = passingDirection * distance;
        if (FindBlocker(step) == null)
            return step;
        if (FindBlocker(-step) == null)
        {
            passingDirection = -passingDirection;
            return -step;
        }
        return Vector3.zero;
    }

    private Transform FindBlocker(Vector3 step, float nearest = float.PositiveInfinity)
    {
        if (step.sqrMagnitude < 0.000001f)
            return null;
        Vector3 position = transform.position;
        Transform blocker = null;
        foreach (NPCWalker walker in ActiveWalkers)
        {
            if (walker != this && walker.gameObject.scene == gameObject.scene)
                ConsiderBlocker(walker.transform, position, step, ref blocker, ref nearest);
        }
        if (sceneCharacters != null)
        {
            foreach (Transform character in sceneCharacters)
            {
                if (character != null && character.gameObject.activeInHierarchy)
                    ConsiderBlocker(character, position, step, ref blocker, ref nearest);
            }
        }
        return blocker;
    }

    private static void ConsiderBlocker(Transform character, Vector3 position, Vector3 step,
        ref Transform blocker, ref float nearest)
    {
        Vector3 separation = character.position - position;
        if (Mathf.Abs(separation.y) >= 1.2f)
            return;
        separation.y = 0f;
        float currentDistance = separation.sqrMagnitude;
        float projection = Vector3.Dot(separation, step);
        if (currentDistance >= nearest || projection <= 0f)
            return; // Always allow walking away, including recovery from initial overlap.
        float fraction = Mathf.Min(1f, projection / step.sqrMagnitude);
        if ((separation - step * fraction).sqrMagnitude < PersonalSpace * PersonalSpace)
        {
            blocker = character;
            nearest = currentDistance;
        }
    }

    private static Vector3 Planar(Vector3 vector)
    {
        vector.y = 0f;
        return vector;
    }

    IEnumerator WaitAtStall()
    {
        transform.position = new Vector3(transform.position.x, target.y, transform.position.z);
        transform.rotation = stallRotation;
        waiting = true;

        SetWalking(false);

        yield return new WaitForSeconds(waitTime);

        if (myStall != null)
        {
            myStall.occupied = false;
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
