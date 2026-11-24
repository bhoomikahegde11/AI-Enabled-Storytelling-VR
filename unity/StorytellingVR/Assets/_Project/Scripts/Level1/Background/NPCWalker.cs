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
    [Header("Ground following")]
    [SerializeField] private LayerMask groundLayers = Physics.DefaultRaycastLayers;
    private readonly RaycastHit[] groundHits = new RaycastHit[12];
    private float nextGroundCheck;
    private float groundHeight;
    private bool hasGround;

    private const float ConversationClearance = 0.8f;
    private sealed class ConversationSpace
    {
        public Behaviour owner;
        public Transform listener;
        public Transform speaker;
    }
    private static readonly List<ConversationSpace> ConversationSpaces = new List<ConversationSpace>();

    public static void ProtectConversation(Behaviour owner, Transform listener, Transform speaker)
    {
        ReleaseConversation(owner);
        if (owner != null && listener != null && speaker != null)
            ConversationSpaces.Add(new ConversationSpace { owner = owner, listener = listener, speaker = speaker });
    }

    public static void ReleaseConversation(Behaviour owner)
    {
        for (int i = ConversationSpaces.Count - 1; i >= 0; i--)
            if (ConversationSpaces[i].owner == null || ConversationSpaces[i].owner == owner)
                ConversationSpaces.RemoveAt(i);
    }


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
        if (scriptedMovement)
            return;
        if (waiting)
        {
            UpdateGrounding(Time.deltaTime);
            return;
        }

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
        if (!scriptedMovement) UpdateGrounding(Time.deltaTime);
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
        Vector3 corridorDirection = SteerAroundConversation(position, towardTarget);
        if ((corridorDirection - towardTarget).sqrMagnitude > 0.000001f) direction = corridorDirection;
        towardTarget = direction.normalized;
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
        if (FindBlocker(step) == null && !BlocksConversationStep(transform.position, step))
            return step;
        if (avoidanceNPC == null)
            return Vector3.zero;

        // If forward motion is blocked, step sideways before trying to make progress again.
        step = passingDirection * distance;
        if (FindBlocker(step) == null && !BlocksConversationStep(transform.position, step))
            return step;
        if (FindBlocker(-step) == null && !BlocksConversationStep(transform.position, -step))
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

    private Vector3 SteerAroundConversation(Vector3 position, Vector3 towardTarget)
    {
        PruneConversationSpaces();
        foreach (var space in ConversationSpaces)
        {
            if (!Applies(space)) continue;
            Vector3 a = Planar(space.listener.position), b = Planar(space.speaker.position);
            Vector3 p = Planar(position);
            if (!BlocksSpace(p, towardTarget * LookAhead, a, b)) continue;
            Vector3 axis = (b - a).normalized;
            Vector3 side = Vector3.Cross(axis, Vector3.up);
            if (side.sqrMagnitude < 0.01f) side = Vector3.right;
            Vector3 nearest = ClosestOnSegment(p, a, b);
            float sideSign = Vector3.Dot(p - nearest, side) < 0f ? -1f : 1f;
            if ((p - nearest).sqrMagnitude < ConversationClearance * ConversationClearance)
                return side * sideSign; // A pedestrian already inside must be able to escape.
            bool aroundListener = Vector3.Distance(p, a) + Vector3.Distance(a, Planar(target)) <
                Vector3.Distance(p, b) + Vector3.Distance(b, Planar(target));
            Vector3 end = aroundListener ? a - axis : b + axis;
            // Once beyond an endpoint, cross toward the destination's side instead of circling the near corner.
            float beyondEnd = aroundListener ? Vector3.Dot(a - p, axis) : Vector3.Dot(p - b, axis);
            if (beyondEnd >= ConversationClearance + 0.1f)
                sideSign = Vector3.Dot(Planar(target) - end, side) < 0f ? -1f : 1f;
            return (end + side * sideSign * (ConversationClearance + 0.2f) - p).normalized;
        }
        return towardTarget;
    }

    private bool Applies(ConversationSpace space) => space.owner != this &&
        space.owner.gameObject.scene == gameObject.scene &&
        Mathf.Abs(space.speaker.position.y - transform.position.y) < 1.2f;

    private bool BlocksConversationStep(Vector3 position, Vector3 step)
    {
        PruneConversationSpaces();
        foreach (var space in ConversationSpaces)
            if (Applies(space) && BlocksSpace(Planar(position), step,
                Planar(space.listener.position), Planar(space.speaker.position))) return true;
        return false;
    }

    private static void PruneConversationSpaces()
    {
        for (int i = ConversationSpaces.Count - 1; i >= 0; i--)
        {
            var space = ConversationSpaces[i];
            if (space.owner == null || !space.owner.isActiveAndEnabled || space.listener == null ||
                space.speaker == null || space.speaker.gameObject.scene != space.owner.gameObject.scene)
                ConversationSpaces.RemoveAt(i);
        }
    }

    private static bool BlocksSpace(Vector3 p, Vector3 step, Vector3 a, Vector3 b)
    {
        float before = (p - ClosestOnSegment(p, a, b)).sqrMagnitude;
        Vector3 next = p + step;
        float after = (next - ClosestOnSegment(next, a, b)).sqrMagnitude;
        if (before < ConversationClearance * ConversationClearance)
            return after <= before + 0.000001f;
        // Swept segment check prevents a slow frame stepping straight across the corridor.
        Vector3 axis = b - a;
        float cross = step.x * axis.z - step.z * axis.x;
        if (Mathf.Abs(cross) > 0.000001f)
        {
            Vector3 offset = a - p;
            float t = (offset.x * axis.z - offset.z * axis.x) / cross;
            float u = (offset.x * step.z - offset.z * step.x) / cross;
            if (t >= 0f && t <= 1f && u >= 0f && u <= 1f) return true;
        }
        float distance = Mathf.Min(Mathf.Min(before, after), Mathf.Min(
            (a - ClosestOnSegment(a, p, next)).sqrMagnitude,
            (b - ClosestOnSegment(b, p, next)).sqrMagnitude));
        return distance < ConversationClearance * ConversationClearance;
    }

    private static Vector3 ClosestOnSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 axis = b - a;
        return a + axis * (axis.sqrMagnitude > 0.000001f
            ? Mathf.Clamp01(Vector3.Dot(p - a, axis) / axis.sqrMagnitude) : 0f);
    }

    private void UpdateGrounding(float deltaTime)
    {
        if (Time.time >= nextGroundCheck)
        {
            nextGroundCheck = Time.time + 0.15f;
            Vector3 position = transform.position;
            int count = Physics.RaycastNonAlloc(position + Vector3.up * 0.6f, Vector3.down,
                groundHits, 1.2f, groundLayers, QueryTriggerInteraction.Ignore);
            hasGround = false;
            float nearest = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
            {
                var hit = groundHits[i];
                if (hit.normal.y < 0.65f || hit.collider.transform.IsChildOf(transform) ||
                    hit.collider.GetComponentInParent<NPCWalker>() != null || IsSceneCharacter(hit.collider.transform)) continue;
                // Prefer the upper support, including a ramp above a terrain/base collider.
                if (hit.distance < nearest)
                {
                    nearest = hit.distance;
                    groundHeight = hit.point.y;
                    hasGround = true;
                }
            }
        }
        if (hasGround)
        {
            Vector3 position = transform.position;
            position.y = Mathf.MoveTowards(position.y, groundHeight, 3f * deltaTime);
            transform.position = position;
        }
    }

    private bool IsSceneCharacter(Transform candidate)
    {
        if (sceneCharacters != null)
            foreach (var character in sceneCharacters)
                if (character != null && candidate.IsChildOf(character)) return true;
        return false;
    }

    IEnumerator WaitAtStall()
    {
        // Keep the foot anchor on its sampled support; waypoint Y is not terrain height.
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
