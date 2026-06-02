using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class Level1AnimatorSetup : EditorWindow
{
    private const string BASE_PATH = "Assets/_Project/Animations/Level1";
    private const string CLIPS_PATH = BASE_PATH + "/Clips";
    private const string CONTROLLERS_PATH = BASE_PATH + "/Controllers";
    private const string CONTROLLER_ASSET_PATH = CONTROLLERS_PATH + "/Level1Buyer.controller";

    [MenuItem("Tools/Setup Level 1 NPC Animator")]
    public static void SetupController()
    {
        Debug.Log("[Level1Setup] Starting Level 1 Animator Setup...");

        // 1. Create folders if they are missing
        CreateDirectoryIfMissing(CLIPS_PATH);
        CreateDirectoryIfMissing(CONTROLLERS_PATH);

        // 2. Extract and duplicate clips
        ExtractClip("Breathing Idle.fbx", "L1_Idle_Breathing.anim", true);
        ExtractClip("Walking With Shopping Bag.fbx", "L1_Walk.anim", true);
        ExtractClip("Talking.fbx", "L1_Talking.anim", true);
        ExtractClip("Thinking.fbx", "L1_Thinking.anim", true);
        ExtractClip("Agreeing.fbx", "L1_Agree.anim", false);
        ExtractClip("Head Nod Yes.fbx", "L1_HeadNod.anim", false);
        ExtractClip("Shaking Head No.fbx", "L1_Reject.anim", false);

        AssetDatabase.Refresh();

        // 3. Create or load Animator Controller
        AnimatorController controller = CreateOrGetController();
        if (controller == null)
        {
            Debug.LogError("[Level1Setup] Failed to create or load Animator Controller!");
            return;
        }

        // 4. Configure Parameters
        ConfigureParameters(controller);

        // 5. Configure States and transitions
        ConfigureStateMachine(controller);

        // 6. Assign Controller to BuyerNPC in active scene if open
        AssignControllerToNPC(controller);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[Level1Setup] Setup complete! All animations, states, parameters, and transitions configured successfully.");
        EditorUtility.DisplayDialog("Level 1 Animator Setup", "NPC Animator Controller set up successfully! Clips extracted, loop settings configured, and controller assigned to the NPC.", "OK");
    }

    private static void CreateDirectoryIfMissing(string relativePath)
    {
        string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (!Directory.Exists(absolutePath))
        {
            Directory.CreateDirectory(absolutePath);
            Debug.Log($"[Level1Setup] Created folder: {relativePath}");
        }
    }

    private static void ExtractClip(string fbxFileName, string destClipName, bool loopTime)
    {
        string fbxPath = Path.Combine(BASE_PATH, fbxFileName).Replace('\\', '/');
        string destPath = Path.Combine(CLIPS_PATH, destClipName).Replace('\\', '/');

        // Check if FBX exists
        if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), fbxPath)))
        {
            Debug.LogWarning($"[Level1Setup] FBX file not found at: {fbxPath}");
            return;
        }

        // Load FBX assets to find AnimationClip
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        AnimationClip srcClip = null;

        foreach (var asset in assets)
        {
            if (asset is AnimationClip && !asset.name.Contains("__preview__"))
            {
                srcClip = (AnimationClip)asset;
                break;
            }
        }

        if (srcClip == null)
        {
            Debug.LogError($"[Level1Setup] No AnimationClip found inside: {fbxPath}");
            return;
        }

        // Duplicate/Instantiate to make it standalone and editable
        AnimationClip duplicate = Instantiate(srcClip);
        duplicate.name = Path.GetFileNameWithoutExtension(destClipName);

        // Set Loop Settings
        var settings = AnimationUtility.GetAnimationClipSettings(duplicate);
        settings.loopTime = loopTime;
        if (loopTime)
        {
            settings.loopBlend = true;
            settings.keepOriginalOrientation = true;
        }
        AnimationUtility.SetAnimationClipSettings(duplicate, settings);

        // Save Clip
        AssetDatabase.CreateAsset(duplicate, destPath);
        EditorUtility.SetDirty(duplicate);
        Debug.Log($"[Level1Setup] Extracted standalone clip: {fbxFileName} -> {destPath} (Loop: {loopTime})");
    }

    private static AnimatorController CreateOrGetController()
    {
        // Delete existing controller if any to ensure clean rebuild of parameters and states
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), CONTROLLER_ASSET_PATH)))
        {
            AssetDatabase.DeleteAsset(CONTROLLER_ASSET_PATH);
        }

        Debug.Log($"[Level1Setup] Creating new Animator Controller at: {CONTROLLER_ASSET_PATH}");
        return AnimatorController.CreateAnimatorControllerAtPath(CONTROLLER_ASSET_PATH);
    }

    private static void ConfigureParameters(AnimatorController controller)
    {
        // Ensure clean parameters
        while (controller.parameters.Length > 0)
        {
            controller.RemoveParameter(0);
        }

        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isTalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isThinking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("happy", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("reject", AnimatorControllerParameterType.Trigger);

        Debug.Log("[Level1Setup] Added animator parameters: isWalking, isTalking, isThinking, happy, reject");
    }

    private static void ConfigureStateMachine(AnimatorController controller)
    {
        var rootStateMachine = controller.layers[0].stateMachine;

        // Clear existing states
        while (rootStateMachine.states.Length > 0)
        {
            rootStateMachine.RemoveState(rootStateMachine.states[0].state);
        }

        // Load Extracted Clips
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Idle_Breathing.anim");
        AnimationClip walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Walk.anim");
        AnimationClip talkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Talking.anim");
        AnimationClip thinkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Thinking.anim");
        AnimationClip agreeClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Agree.anim");
        AnimationClip rejectClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(CLIPS_PATH + "/L1_Reject.anim");

        // Create States
        var idleState = rootStateMachine.AddState("Idle");
        idleState.motion = idleClip;

        var walkState = rootStateMachine.AddState("Walk");
        walkState.motion = walkClip;

        var talkingState = rootStateMachine.AddState("Talking");
        talkingState.motion = talkClip;

        var thinkingState = rootStateMachine.AddState("Thinking");
        thinkingState.motion = thinkClip;

        var agreeState = rootStateMachine.AddState("Agree");
        agreeState.motion = agreeClip;

        var rejectState = rootStateMachine.AddState("Reject");
        rejectState.motion = rejectClip;

        // Set default state
        rootStateMachine.defaultState = idleState;

        // --- Add Transitions ---

        // Idle <-> Walk
        var idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
        idleToWalk.hasExitTime = false;

        var walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
        walkToIdle.hasExitTime = false;

        // Idle <-> Talking
        var idleToTalking = idleState.AddTransition(talkingState);
        idleToTalking.AddCondition(AnimatorConditionMode.If, 0, "isTalking");
        idleToTalking.hasExitTime = false;

        var talkingToIdle = talkingState.AddTransition(idleState);
        talkingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isTalking");
        talkingToIdle.hasExitTime = false;

        // Idle <-> Thinking
        var idleToThinking = idleState.AddTransition(thinkingState);
        idleToThinking.AddCondition(AnimatorConditionMode.If, 0, "isThinking");
        idleToThinking.hasExitTime = false;

        var thinkingToIdle = thinkingState.AddTransition(idleState);
        thinkingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isThinking");
        thinkingToIdle.hasExitTime = false;

        // Any State -> Agree
        var anyToAgree = rootStateMachine.AddAnyStateTransition(agreeState);
        anyToAgree.AddCondition(AnimatorConditionMode.If, 0, "happy");
        anyToAgree.hasExitTime = false;

        // Any State -> Reject
        var anyToReject = rootStateMachine.AddAnyStateTransition(rejectState);
        anyToReject.AddCondition(AnimatorConditionMode.If, 0, "reject");
        anyToReject.hasExitTime = false;

        // Agree -> Idle (Exit Time ON)
        var agreeToIdle = agreeState.AddTransition(idleState);
        agreeToIdle.hasExitTime = true;
        agreeToIdle.exitTime = 0.9f;

        // Reject -> Idle (Exit Time ON)
        var rejectToIdle = rejectState.AddTransition(idleState);
        rejectToIdle.hasExitTime = true;
        rejectToIdle.exitTime = 0.9f;

        Debug.Log("[Level1Setup] Standard states and state transition conditions successfully wired.");
    }

    private static void AssignControllerToNPC(AnimatorController controller)
    {
        GameObject buyerNPC = GameObject.Find("BuyerNPC");
        if (buyerNPC == null) buyerNPC = GameObject.Find("indian m in kurta (1)");

        if (buyerNPC != null)
        {
            Animator animator = buyerNPC.GetComponent<Animator>();
            if (animator == null)
            {
                animator = buyerNPC.AddComponent<Animator>();
            }
            animator.runtimeAnimatorController = controller;

            // Mark scene dirty to save changes
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log($"[Level1Setup] Assigned runtime animator controller to {buyerNPC.name} in the current scene.");
        }
        else
        {
            Debug.LogWarning("[Level1Setup] BuyerNPC or 'indian m in kurta (1)' not found in the active scene. The Animator Controller is compiled and ready at Assets/_Project/Animations/Level1/Controllers/Level1Buyer.controller, but was not assigned in-scene.");
        }
    }
}
