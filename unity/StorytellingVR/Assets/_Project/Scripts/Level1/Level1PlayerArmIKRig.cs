using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[DefaultExecutionOrder(100)]
public class Level1PlayerArmIKRig : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform maleBodyRoot;
    [SerializeField] private Transform femaleBodyRoot;
    [SerializeField] private Transform leftHandTargetSource;
    [SerializeField] private Transform rightHandTargetSource;

    [Header("Target Offsets")]
    [SerializeField] private Vector3 leftTargetLocalPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 rightTargetLocalPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 leftTargetLocalEulerOffset = Vector3.zero;
    [SerializeField] private Vector3 rightTargetLocalEulerOffset = Vector3.zero;

    [Header("Hint Offsets")]
    [SerializeField] private Vector3 leftHintOffset = new Vector3(-0.25f, -0.05f, 0.25f);
    [SerializeField] private Vector3 rightHintOffset = new Vector3(0.25f, -0.05f, 0.25f);

    [Header("Constraint Weights")]
    [SerializeField] [Range(0f, 1f)] private float targetPositionWeight = 1f;
    [SerializeField] [Range(0f, 1f)] private float targetRotationWeight = 0.35f;
    [SerializeField] [Range(0f, 1f)] private float hintWeight = 1f;

    private const string RigRootName = "ControllerDrivenArmRig";
    private const string RigName = "ArmIKRig";
    private const string HintRootName = "ArmIKHints";
    private const string LeftTargetName = "LeftArmIKTarget";
    private const string RightTargetName = "RightArmIKTarget";
    private const string LeftHintName = "LeftArmIKHint";
    private const string RightHintName = "RightArmIKHint";
    private const string LeftConstraintName = "LeftArmIKConstraint";
    private const string RightConstraintName = "RightArmIKConstraint";

    private void Start()
    {
        InitializeRig();
    }

    private void InitializeRig()
    {
        ResolveReferences();

        Transform activeBodyRoot = ResolveActiveBodyRoot();
        if (activeBodyRoot == null || leftHandTargetSource == null || rightHandTargetSource == null)
        {
            Debug.LogWarning("[Level1PlayerArmIKRig] Missing required references. Arm IK setup skipped.", this);
            return;
        }

        if (!activeBodyRoot.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[Level1PlayerArmIKRig] Active body root is inactive. Arm IK setup skipped.", this);
            return;
        }

        Animator animator = activeBodyRoot.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("[Level1PlayerArmIKRig] Active body root has no Animator. Arm IK setup skipped.", this);
            return;
        }

        animator.enabled = true;

        Transform leftUpperArm = ResolveBone(animator, activeBodyRoot, HumanBodyBones.LeftUpperArm, "LeftArm", "mixamorig:LeftArm");
        Transform leftLowerArm = ResolveBone(animator, activeBodyRoot, HumanBodyBones.LeftLowerArm, "LeftForeArm", "mixamorig:LeftForeArm");
        Transform leftHand = ResolveBone(animator, activeBodyRoot, HumanBodyBones.LeftHand, "LeftHand", "mixamorig:LeftHand");
        Transform rightUpperArm = ResolveBone(animator, activeBodyRoot, HumanBodyBones.RightUpperArm, "RightArm", "mixamorig:RightArm");
        Transform rightLowerArm = ResolveBone(animator, activeBodyRoot, HumanBodyBones.RightLowerArm, "RightForeArm", "mixamorig:RightForeArm");
        Transform rightHand = ResolveBone(animator, activeBodyRoot, HumanBodyBones.RightHand, "RightHand", "mixamorig:RightHand");

        if (leftUpperArm == null || leftLowerArm == null || leftHand == null ||
            rightUpperArm == null || rightLowerArm == null || rightHand == null)
        {
            Debug.LogWarning("[Level1PlayerArmIKRig] Could not resolve arm bones on active body root. Arm IK setup skipped.", this);
            return;
        }

        Transform rigRoot = GetOrCreateChild(activeBodyRoot, RigRootName);
        Transform rigTransform = GetOrCreateChild(rigRoot, RigName);
        Transform hintRoot = GetOrCreateChild(rigRoot, HintRootName);

        Rig rig = rigTransform.GetComponent<Rig>();
        if (rig == null)
            rig = rigTransform.gameObject.AddComponent<Rig>();
        rig.weight = 1f;

        Transform leftTarget = GetOrCreateChild(leftHandTargetSource, LeftTargetName);
        leftTarget.localPosition = leftTargetLocalPositionOffset;
        leftTarget.localRotation = Quaternion.Euler(leftTargetLocalEulerOffset);

        Transform rightTarget = GetOrCreateChild(rightHandTargetSource, RightTargetName);
        rightTarget.localPosition = rightTargetLocalPositionOffset;
        rightTarget.localRotation = Quaternion.Euler(rightTargetLocalEulerOffset);

        Transform leftHint = GetOrCreateChild(hintRoot, LeftHintName);
        leftHint.position = leftLowerArm.position
            + (activeBodyRoot.right * leftHintOffset.x)
            + (activeBodyRoot.up * leftHintOffset.y)
            + (activeBodyRoot.forward * leftHintOffset.z);

        Transform rightHint = GetOrCreateChild(hintRoot, RightHintName);
        rightHint.position = rightLowerArm.position
            + (activeBodyRoot.right * rightHintOffset.x)
            + (activeBodyRoot.up * rightHintOffset.y)
            + (activeBodyRoot.forward * rightHintOffset.z);

        ConfigureConstraint(
            rigTransform,
            LeftConstraintName,
            leftUpperArm,
            leftLowerArm,
            leftHand,
            leftTarget,
            leftHint
        );

        ConfigureConstraint(
            rigTransform,
            RightConstraintName,
            rightUpperArm,
            rightLowerArm,
            rightHand,
            rightTarget,
            rightHint
        );

        RigBuilder rigBuilder = activeBodyRoot.GetComponent<RigBuilder>();
        if (rigBuilder == null)
            rigBuilder = activeBodyRoot.gameObject.AddComponent<RigBuilder>();

        List<RigLayer> layers = rigBuilder.layers;
        layers.RemoveAll(layer => layer == null || layer.rig == null);
        if (!layers.Exists(layer => layer.rig == rig))
            layers.Add(new RigLayer(rig));

        rigBuilder.enabled = true;
        rigBuilder.Clear();
        rigBuilder.Build();
    }

    private void ResolveReferences()
    {
        if (maleBodyRoot == null)
        {
            Transform resolved = transform.Find("MaleBodyRoot");
            if (resolved != null)
                maleBodyRoot = resolved;
        }

        if (femaleBodyRoot == null)
        {
            Transform resolved = transform.Find("FemaleBodyRoot");
            if (resolved != null)
                femaleBodyRoot = resolved;
        }

        if (leftHandTargetSource == null && transform.parent != null)
        {
            Transform resolved = transform.parent.Find("LeftHandAnchor/LeftControllerInHandAnchor/LeftHandOnControllerAnchor");
            if (resolved != null)
                leftHandTargetSource = resolved;
        }

        if (rightHandTargetSource == null && transform.parent != null)
        {
            Transform resolved = transform.parent.Find("RightHandAnchor/RightControllerInHandAnchor/RightHandOnControllerAnchor");
            if (resolved != null)
                rightHandTargetSource = resolved;
        }
    }

    private Transform ResolveActiveBodyRoot()
    {
        Transform activeRoot = ResolveActiveRootWithAnimator(maleBodyRoot);
        if (activeRoot != null)
            return activeRoot;

        return ResolveActiveRootWithAnimator(femaleBodyRoot);
    }

    private static Transform ResolveActiveRootWithAnimator(Transform bodyRoot)
    {
        if (bodyRoot == null || !bodyRoot.gameObject.activeInHierarchy)
            return null;

        Animator directAnimator = bodyRoot.GetComponent<Animator>();
        if (directAnimator != null)
            return bodyRoot;

        Animator childAnimator = bodyRoot.GetComponentInChildren<Animator>(true);
        return childAnimator != null ? childAnimator.transform : null;
    }

    private static Transform ResolveBone(Animator animator, Transform activeBodyRoot, HumanBodyBones humanoidBone, params string[] fallbackNames)
    {
        if (animator != null && animator.isHuman)
        {
            Transform bone = animator.GetBoneTransform(humanoidBone);
            if (bone != null)
                return bone;
        }

        foreach (string boneName in fallbackNames)
        {
            Transform bone = FindChildRecursive(activeBodyRoot, boneName);
            if (bone != null)
                return bone;
        }

        return null;
    }

    private static Transform FindChildRecursive(Transform root, string childName)
    {
        if (root == null)
            return null;

        if (root.name == childName)
            return root;

        foreach (Transform child in root)
        {
            Transform match = FindChildRecursive(child, childName);
            if (match != null)
                return match;
        }

        return null;
    }

    private void ConfigureConstraint(
        Transform rigTransform,
        string constraintName,
        Transform root,
        Transform mid,
        Transform tip,
        Transform target,
        Transform hint)
    {
        Transform constraintTransform = GetOrCreateChild(rigTransform, constraintName);
        TwoBoneIKConstraint constraint = constraintTransform.GetComponent<TwoBoneIKConstraint>();
        if (constraint == null)
            constraint = constraintTransform.gameObject.AddComponent<TwoBoneIKConstraint>();

        constraint.weight = 1f;
        constraint.data.root = root;
        constraint.data.mid = mid;
        constraint.data.tip = tip;
        constraint.data.target = target;
        constraint.data.hint = hint;
        constraint.data.targetPositionWeight = targetPositionWeight;
        constraint.data.targetRotationWeight = targetRotationWeight;
        constraint.data.hintWeight = hintWeight;
        constraint.data.maintainTargetPositionOffset = false;
        constraint.data.maintainTargetRotationOffset = false;
    }

    private static Transform GetOrCreateChild(Transform parent, string childName)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
            return existing;

        GameObject child = new GameObject(childName);
        child.transform.SetParent(parent, false);
        return child.transform;
    }
}
