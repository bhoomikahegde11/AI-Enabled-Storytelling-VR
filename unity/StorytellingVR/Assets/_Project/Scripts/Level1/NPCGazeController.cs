using UnityEngine;

public class NPCGazeController : MonoBehaviour
{
    [Header("Gaze Targets")]
    [Tooltip("The transform representing the player (e.g. VR main camera).")]
    public Transform playerTarget;
    
    [Tooltip("The transform representing the spice flatstone/bazar species.")]
    public Transform spicesTarget;
    
    [Header("Gaze Parameters")]
    [Range(0f, 1f)]
    public float gazeWeight = 1.0f;
    public float transitionSpeed = 3.5f;

    private Animator animator;
    private Transform activeTarget;
    private float currentWeight = 0f;
    private Transform headBone;
    private Quaternion initialHeadRotation;
    private Transform preferredRigRoot;
    private Animator preferredAnimator;
    private bool restrictToPreferredRig;

    private void Start()
    {
        // Auto-discover Main Camera as the VR player's head target
        if (playerTarget == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                playerTarget = mainCam.transform;
            }
        }

        // Auto-discover spices flatstone
        if (spicesTarget == null)
        {
            GameObject spices = GameObject.Find("bazarspecies");
            if (spices == null) spices = GameObject.Find("bazarflatstone");
            if (spices != null)
            {
                spicesTarget = spices.transform;
            }
        }

        RefreshRigBindings();

        // Default gaze state: Look at player
        activeTarget = playerTarget;
    }

    private Transform FindChildRecursive(Transform parent, string boneName)
    {
        if (parent.name.ToLower().Contains(boneName))
        {
            return parent;
        }
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, boneName);
            if (found != null) return found;
        }
        return null;
    }

    public void LookAtPlayer()
    {
        activeTarget = playerTarget;
        Debug.Log("[NPCGaze] Target switched to Player");
    }

    public void LookAtSpices()
    {
        activeTarget = spicesTarget;
        Debug.Log("[NPCGaze] Target switched to Spices");
    }

    public void LookAtIdle()
    {
        activeTarget = null;
        Debug.Log("[NPCGaze] Gaze disabled");
    }

    public void RefreshRigBindings()
    {
        animator = preferredAnimator;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }
        }

        Transform searchRoot = preferredRigRoot != null ? preferredRigRoot : transform;
        headBone = FindChildRecursive(searchRoot, "head");
        if (headBone != null)
        {
            initialHeadRotation = headBone.localRotation;
            Debug.Log($"[NPCGaze] Auto-discovered head bone: {GetHierarchyPath(headBone)}");
        }
        else if (!restrictToPreferredRig)
        {
            Debug.LogWarning("[NPCGaze] Could not find head bone recursively. Falling back to whole-body orientation or OnAnimatorIK.");
        }
    }

    public void SetRigBindingSource(Transform rigRoot, Animator rigAnimator, bool restrictToRigRoot)
    {
        preferredRigRoot = rigRoot;
        preferredAnimator = rigAnimator;
        restrictToPreferredRig = restrictToRigRoot;
    }

    public string GetHeadBindingPath()
    {
        return headBone != null ? GetHierarchyPath(headBone) : "<none>";
    }

    private void Update()
    {
        float targetWeight = (activeTarget != null) ? gazeWeight : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);
    }

    // Official Unity IK gaze system (works perfectly if IK Pass is enabled on animator layer)
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null) return;

        if (activeTarget != null)
        {
            // Set look weights: overall weight, body weight, head weight, eyes weight, clamp weight
            animator.SetLookAtWeight(currentWeight, currentWeight * 0.4f, currentWeight, currentWeight * 0.2f, 0.5f);
            animator.SetLookAtPosition(activeTarget.position);
        }
        else
        {
            animator.SetLookAtWeight(0f);
        }
    }

    // Direct bone rotation fallback in LateUpdate in case IK Pass is disabled
    private void LateUpdate()
    {
        // Smoothly blend neck/head bone rotation towards target rotation
        if (headBone != null && activeTarget != null && currentWeight > 0.01f)
        {
            Vector3 targetDir = activeTarget.position - headBone.position;
            if (targetDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                
                // Prevent twisting neck in unrealistic angles (restrict to 75 degrees)
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                if (angle < 75f)
                {
                    // Blends slerp smoothly into the animator's current frame bone rotation
                    headBone.rotation = Quaternion.Slerp(headBone.rotation, targetRotation, currentWeight * Time.deltaTime * transitionSpeed);
                }
            }
        }
    }

    private static string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return "<null>";
        }

        string path = target.name;
        Transform current = target.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
