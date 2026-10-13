using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent, DefaultExecutionOrder(-30000)]
public sealed class PauseMenuController : MonoBehaviour
{
    private static PauseMenuController instance;
    private GameManager owner;
    private PauseMenuView view;
    private Camera sceneCamera;
    private OVRCameraRig rig;
    private LineRenderer rayLine;
    private Material rayMaterial;
    private bool isPaused, releaseInput, exiting;
    private bool previousAudioPause, previousCursorVisible;
    private float previousTimeScale, pausedAt, pausedDuration, releaseAfter;
    private CursorLockMode previousCursorLock;
    private bool toggleHeld, selectionHeld;
    private int resumeVersion;
    public static int ResumeVersion => instance != null ? instance.resumeVersion : 0;
    private readonly Dictionary<Behaviour, bool> suspended = new Dictionary<Behaviour, bool>();
    private readonly Dictionary<Animator, AnimatorUpdateMode> animatorModes = new Dictionary<Animator, AnimatorUpdateMode>();
    private readonly Dictionary<AudioSource, bool> audioModes = new Dictionary<AudioSource, bool>();
    private readonly List<Rigidbody> releasedBodies = new List<Rigidbody>();

    public bool IsPaused => isPaused;
    public bool IsReady => true;
    public static bool GameplaySuspended => instance != null && (instance.isPaused || instance.exiting);
    public static bool GameplayInputBlocked => GameplaySuspended ||
        (instance != null && instance.releaseInput) ||
        (GameManager.ExistingInstance != null && GameManager.ExistingInstance.IsLoading);
    public static float GameplayUnscaledDeltaTime => GameplaySuspended ? 0f : Time.unscaledDeltaTime;
    public static float GameplayUnscaledTime => Time.unscaledTime - (instance == null ? 0f :
        instance.pausedDuration + (instance.isPaused ? Time.unscaledTime - instance.pausedAt : 0f));

    public static IEnumerator WaitForGameplay()
    {
        while (GameplaySuspended) yield return null;
    }

    // Gate coroutine continuations without disabling story components or discarding their state.
    public static IEnumerator RunGameplay(IEnumerator routine)
    {
        try
        {
            while (true)
            {
                while (GameplaySuspended) yield return null;
                if (!routine.MoveNext()) yield break;
                if (routine.Current is IEnumerator nested) yield return RunGameplay(nested);
                else yield return routine.Current;
            }
        }
        finally { (routine as IDisposable)?.Dispose(); }
    }

    public static IEnumerator WaitRealtime(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds || GameplaySuspended)
        {
            yield return null;
            elapsed += GameplayUnscaledDeltaTime;
        }
    }

    // Shared by hardware polling and EditMode edge tests.
    public static bool FreshPress(bool pressed, ref bool held)
    {
        bool fresh = pressed && !held;
        held = pressed;
        return fresh;
    }

    public static bool FreshTriggerPress(float value, ref bool held) =>
        FreshPress(value >= (held ? 0.15f : 0.75f), ref held);

    internal bool InitializeOwnership(GameManager manager)
    {
        if (manager == null || manager != GameManager.ExistingInstance || manager.gameObject != gameObject ||
            (instance != null && instance != this))
        {
            enabled = false;
            if (Application.isPlaying) Destroy(this); else DestroyImmediate(this);
            return false;
        }
        owner = manager;
        instance = this;
        return true;
    }

    private void Awake()
    {
        var manager = GetComponent<GameManager>();
        if (manager == null) InitializeOwnership(null);
        else if (manager == GameManager.ExistingInstance) InitializeOwnership(manager);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        toggleHeld = ReadToggle();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        CloseImmediately();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        CloseImmediately();
        if (instance == this) instance = null;
        if (rayMaterial != null)
        {
            if (Application.isPlaying) Destroy(rayMaterial); else DestroyImmediate(rayMaterial);
        }
    }

    private void Update()
    {
        if (instance != this) return;
        if (FreshPress(ReadToggle(), ref toggleHeld))
        {
            if (isPaused) ResumeGameplay(); else PauseGameplay();
        }
        if (releaseInput && !isPaused && !exiting && owner != null && !owner.IsLoading &&
            Time.unscaledTime >= releaseAfter && ControlsReleased())
        {
            releaseInput = false;
            RestoreSceneComponents();
        }
        if (isPaused && view == null) ResumeGameplay();
        if (isPaused && !IsDesktop() && (sceneCamera == null || !sceneCamera.isActiveAndEnabled))
        {
            RebindSceneReferences();
            if (sceneCamera == null) ResumeGameplay();
            else if (view != null) { view.ConfigureCamera(sceneCamera, IsDesktop()); if (!IsDesktop()) PositionMenu(); }
        }
        if (view != null) view.Tick(isPaused, Time.unscaledDeltaTime);
    }

    private static bool ReadToggle()
    {
        bool pressed = false;
#if UNITY_EDITOR
        pressed = Input.GetKey(KeyCode.F10) || Input.GetKey(KeyCode.Escape);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        pressed |= OVRInput.Get(OVRInput.Button.Start, OVRInput.Controller.LTouch);
#else
        if (OVRManager.isHmdPresent) pressed |= OVRInput.Get(OVRInput.Button.Start, OVRInput.Controller.LTouch);
#endif
        return pressed;
    }

    private static bool IsDesktop()
    {
#if UNITY_EDITOR
        return !OVRManager.isHmdPresent;
#else
        return false;
#endif
    }

    private static bool ControlsReleased()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.F10) || Input.GetKey(KeyCode.Escape) ||
            Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.V) ||
            Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.Tab) || Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.N)) return false;
#endif
        if (!OVRManager.isHmdPresent) return true;
        return !OVRInput.Get(OVRInput.Button.One | OVRInput.Button.Two | OVRInput.Button.Three | OVRInput.Button.Four | OVRInput.Button.Start, OVRInput.Controller.Touch) &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) < 0.15f &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) < 0.15f &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) < 0.15f &&
            OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) < 0.15f &&
            OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).sqrMagnitude < 0.04f &&
            OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).sqrMagnitude < 0.04f;
    }

    private bool IsPauseBlockedInCurrentScene()
    {
        return !CanPauseInScene(SceneManager.GetActiveScene().name);
    }

    private bool CanPauseInScene(string sceneName) => owner != null && !owner.IsLoading && !exiting && owner.IsGameplayScene(sceneName);

    private void PauseGameplay()
    {
        TryPause();
    }

    public bool TryPause()
    {
        if (instance != this || !isActiveAndEnabled || isPaused || releaseInput || IsPauseBlockedInCurrentScene()) return false;
        try
        {
            RebindSceneReferences();
            if ((!IsDesktop() && sceneCamera == null) || !EnsureView()) return false;
            view.ConfigureCamera(sceneCamera, IsDesktop());
            view.Open(SceneManager.GetActiveScene().name, IsDesktop());
            if (!IsDesktop()) PositionMenu();
            selectionHeld = IsDesktop() ? Input.GetMouseButton(0) : OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= 0.15f;
            CapturePauseState();
            SuspendSceneComponents();
            foreach (var voice in FindObjectsByType<Level1VoiceInputManager>(FindObjectsSortMode.None)) voice.OnGlobalPause();
            foreach (var voice in FindObjectsByType<VoiceRecognitionManager>(FindObjectsSortMode.None)) voice.OnGlobalPause();
            foreach (var native in FindObjectsByType<AndroidNativeTtsProvider>(FindObjectsSortMode.None)) native.StopSpeaking();
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
            return true;
        }
        catch (Exception error)
        {
            CloseImmediately(); // An incomplete UI or failed dependency must never strand the global clock.
            Debug.LogError("[PAUSE] Could not open the menu: " + error.Message);
            return false;
        }
    }

    private void CapturePauseState()
    {
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        pausedAt = Time.unscaledTime;
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = IsDesktop();
    }

    private bool EnsureView()
    {
        if (view != null && view.IsValid) return true;
        if (view != null) Destroy(view.gameObject);
        view = null;
        var prefab = Resources.Load<GameObject>("GlobalPauseMenu");
        if (prefab == null)
        {
            Debug.LogError("[PAUSE] Resources/GlobalPauseMenu prefab is missing; gameplay remains running.");
            return false;
        }
        var host = Instantiate(prefab, transform, false);
        view = host.GetComponent<PauseMenuView>();
        if (view == null || !view.IsValid)
        {
            Debug.LogError("[PAUSE] Pause menu references are incomplete; gameplay remains running.");
            Destroy(host);
            view = null;
            return false;
        }
        view.Bind(ResumeGameplay, ConfirmRestart, ConfirmReturn);
        return true;
    }

    public void ResumeGameplay()
    {
        if (!isPaused) return;
        RestoreClockAndAudio();
        RestorePresentationModes();
        releaseInput = true;
        releaseAfter = Time.unscaledTime + 0.15f;
        if (view != null) view.ClearHover();
        if (rayLine != null) rayLine.enabled = false;
    }

    private void RestoreClockAndAudio()
    {
        if (!isPaused) return;
        pausedDuration += Time.unscaledTime - pausedAt;
        isPaused = false;
        ++resumeVersion;
        Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    private void ConfirmRestart()
    {
        if (isPaused && owner != null) owner.RestartCurrentScene();
    }

    private void ConfirmReturn()
    {
        if (isPaused && owner != null) owner.ReturnToMainMenu();
    }

    internal void PrepareForTransition()
    {
        exiting = true;
        if (suspended.Count == 0 && Application.isPlaying) SuspendSceneComponents();
        RestoreClockAndAudio();
        RestorePresentationModes();
        if (view != null) view.HideImmediately();
        if (rayLine != null) rayLine.enabled = false;
        foreach (var voice in FindObjectsByType<Level1VoiceInputManager>(FindObjectsSortMode.None)) voice.CancelForSceneExit();
        foreach (var voice in FindObjectsByType<VoiceRecognitionManager>(FindObjectsSortMode.None)) voice.CancelForSceneExit();
        foreach (var chat in FindObjectsByType<ChatManager>(FindObjectsSortMode.None)) chat.ResetConversationUI();
    }

    internal void FinishTransition()
    {
        exiting = false;
        // Outgoing scene objects have been destroyed; restore only surviving references.
        RestoreSceneComponents();
        if (Application.isPlaying) SuspendSceneComponents(); // Fence the destination rig until the menu trigger is released.
        releaseInput = true;
        releaseAfter = Time.unscaledTime + 0.15f;
        RebindSceneReferences();
    }

    private void CloseImmediately()
    {
        RestoreClockAndAudio();
        exiting = releaseInput = false;
        RestoreSceneComponents();
        if (view != null) view.HideImmediately();
        if (rayLine != null) rayLine.enabled = false;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance != this || mode != LoadSceneMode.Single) return;
        RestoreClockAndAudio();
        if (view != null) view.HideImmediately();
        if (!exiting) FinishTransition();
        RebindSceneReferences();
    }

    private void RebindSceneReferences()
    {
        sceneCamera = Camera.main;
        rig = sceneCamera != null ? sceneCamera.GetComponentInParent<OVRCameraRig>() : null;
        if (rig == null) rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig != null && rig.centerEyeAnchor != null)
        {
            var eye = rig.centerEyeAnchor.GetComponent<Camera>();
            if (eye != null && eye.isActiveAndEnabled) sceneCamera = eye;
        }
        if (sceneCamera == null)
            foreach (var candidate in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (candidate.isActiveAndEnabled) { sceneCamera = candidate; break; }
    }

    private void PositionMenu()
    {
        Vector3 forward = Vector3.ProjectOnPlane(sceneCamera.transform.forward, Vector3.up).normalized;
        if (forward.sqrMagnitude < 0.01f) forward = Vector3.forward;
        float distance = 1.4f;
        foreach (var hit in Physics.SphereCastAll(sceneCamera.transform.position, 0.15f, forward, 1.55f,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance < 0.2f || hit.transform.IsChildOf(view.transform) ||
                (rig != null && hit.transform.IsChildOf(rig.transform))) continue;
            distance = Mathf.Min(distance, Mathf.Max(0.65f, hit.distance - 0.15f));
        }
        view.transform.SetPositionAndRotation(sceneCamera.transform.position + forward * distance - Vector3.up * 0.05f,
            Quaternion.LookRotation(forward, Vector3.up));
    }

    private void SuspendSceneComponents()
    {
        foreach (var grab in FindObjectsByType<Grabbable>(FindObjectsSortMode.None))
        {
            if (grab.SelectingPointsCount == 0) continue;
            var body = grab.GetComponentInParent<Rigidbody>(true);
            if (body != null) releasedBodies.Add(body);
        }
        var components = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        // Disconnect sinks before cancelling selection: snap-turn broadcasters can fire on unselect.
        foreach (var component in components)
            if (component is LocomotionEventsConnection) SuspendBehaviour(component);
        foreach (var component in components)
            if ((component is ILocomotionEventBroadcaster && !(component is IInteractor)) ||
                component is FirstPersonLocomotor || component is OVRPlayerController) SuspendBehaviour(component);
        foreach (var component in components)
            if (component is IInteractor || component is BaseInputModule ||
                component is NPCDialogueVRLaserPointer || component is MainMenuVRLaserPointer) SuspendBehaviour(component);
        foreach (var animator in FindObjectsByType<Animator>(FindObjectsSortMode.None))
        {
            if (animator.updateMode != AnimatorUpdateMode.UnscaledTime) continue;
            animatorModes[animator] = animator.updateMode;
            animator.updateMode = AnimatorUpdateMode.Normal;
        }
        foreach (var audio in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            audioModes[audio] = audio.ignoreListenerPause;
            audio.ignoreListenerPause = false;
        }
        StopReleasedBodyThrows();
    }

    private void SuspendBehaviour(Behaviour component)
    {
        if (!suspended.ContainsKey(component)) suspended.Add(component, component.enabled);
        component.enabled = false;
    }

    private void StopReleasedBodyThrows()
    {
        foreach (var body in releasedBodies)
            if (body != null && !body.isKinematic) { body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
    }

    private void RestoreSceneComponents()
    {
        StopReleasedBodyThrows();
        foreach (var entry in suspended) if (entry.Key != null) entry.Key.enabled = entry.Value;
        suspended.Clear(); releasedBodies.Clear();
        RestorePresentationModes();
    }

    private void RestorePresentationModes()
    {
        foreach (var entry in animatorModes) if (entry.Key != null) entry.Key.updateMode = entry.Value;
        foreach (var entry in audioModes) if (entry.Key != null) entry.Key.ignoreListenerPause = entry.Value;
        animatorModes.Clear(); audioModes.Clear();
    }

    private void LateUpdate()
    {
        if (instance != this) return;
        if (isPaused || releaseInput) StopReleasedBodyThrows();
        if (!isPaused || view == null || (!IsDesktop() && sceneCamera == null)) return;
        bool desktop = IsDesktop();
        bool fresh = desktop ? FreshPress(Input.GetMouseButton(0), ref selectionHeld) :
            FreshTriggerPress(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch), ref selectionHeld);
        Button target = null;
        if (desktop) target = view.HitMouse(Input.mousePosition);
        else if (rig != null && rig.rightControllerAnchor != null)
        {
            Transform anchor = rig.rightControllerAnchor;
            Ray ray = new Ray(anchor.position, anchor.forward);
            target = view.HitRay(ray, out Vector3 end);
            EnsureRay();
            if (rayLine != null)
            {
                rayLine.enabled = true;
                rayLine.SetPosition(0, ray.origin);
                rayLine.SetPosition(1, end);
            }
        }
        view.Hover(target);
        if (fresh && target != null) view.Press(target);
    }

    private void EnsureRay()
    {
        if (rayLine != null) return;
        var material = Resources.Load<Material>("PauseMenuRay");
        if (material == null) return;
        var host = new GameObject("Pause Menu Ray");
        host.transform.SetParent(transform, false);
        rayLine = host.AddComponent<LineRenderer>();
        rayMaterial = new Material(material);
        rayLine.sharedMaterial = rayMaterial;
        rayLine.positionCount = 2;
        rayLine.startWidth = 0.002f; rayLine.endWidth = 0.001f;
        rayLine.useWorldSpace = true;
        rayLine.numCapVertices = 3;
        rayLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rayLine.receiveShadows = false;
    }
}
