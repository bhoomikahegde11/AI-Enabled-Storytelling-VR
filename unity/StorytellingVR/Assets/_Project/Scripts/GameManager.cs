using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public const string DefaultBootstrapSceneName = "Bootstrap";
    public const string DefaultMainMenuSceneName = "MainMenu_Optimized";
    public const string DefaultIntroSceneName = "ModernStudyRoom";
    public const string DefaultGameplaySceneName = "Level1_MainLoopUpdated";

    private static readonly string[] DefaultSceneOrder =
    {
        DefaultIntroSceneName,
        "GateIntro",
        "FreeRoam_WithTerrain",
        "NewSpiceScene",
        "NewTransactionTutorial",
        "NewCoinScene",
        "SpicesInteraction",
        DefaultGameplaySceneName
    };

    private static GameManager instance;

    [Header("Scene Flow")]
    [SerializeField] private string bootstrapSceneName = DefaultBootstrapSceneName;
    [SerializeField] private string mainMenuSceneName = DefaultMainMenuSceneName;
    [SerializeField] private string[] scenes = (string[])DefaultSceneOrder.Clone();
    [SerializeField] private ScreenFader fader;

    [Header("Persistent Controllers")]
    [SerializeField] private bool enablePauseMenuController = true;

    [Header("Skip")]
    private bool yButtonHeld;
    private bool skipSuppressed = false;

    public void SuppressSkip(bool suppress)
    {
        skipSuppressed = suppress;
    }

    [Header("Developer Testing")]
    [SerializeField] private bool enableDeveloperSceneSkip = true;
    [SerializeField] private KeyCode developerSkipKey = KeyCode.F8;

    [Header("Development Debug (Editor / Development Build Only)")]
    [Tooltip("Allow F7 or right-controller B to skip Level 1 to Epilogue_BookPurchase. Does not complete trades or award results.")]
    [SerializeField] private bool enableDebugSceneSkip = false;

    private int currentIndex = -1;
    private string currentSceneName = string.Empty;
    private bool isLoading;
    private bool runtimeStateInitialized;

    public static GameManager ExistingInstance => instance;

    #if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureEditorSceneEntry()
    {
        // Direct Play uses the same manager, without redirecting the selected scene.
        _ = Instance;
    }
    #endif

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameManager existing = FindFirstObjectByType<GameManager>();
                if (existing != null)
                {
                    existing.InitializeSingleton();
                }
                else
                {
                    GameObject host = new GameObject(nameof(GameManager));
                    host.AddComponent<GameManager>().InitializeSingleton();
                    Debug.Log("[SCENE FLOW] Runtime GameManager instance created.");
                }
            }

            instance.InitializeRuntimeState();
            instance.EnsurePersistentControllers();
            instance.ResolveTransitionFader();
            return instance;
        }
    }

    public string CurrentSceneName => currentSceneName;
    public int CurrentProgressionIndex => currentIndex;
    public bool IsLoading => isLoading;
    public bool IsGameplayScene(string sceneName) => GetProgressionIndex(sceneName) >= 0;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (instance != null && instance != this)
        {
            enabled = false;
            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
            return;
        }

        if (instance == this) return;
        instance = this;
        transform.SetParent(null, true);
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        InitializeRuntimeState();
        EnsurePersistentControllers();
        ResolveTransitionFader();
        Debug.Log("[SCENE FLOW] GameManager initialized.");
    }

    private void OnEnable()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
    }

    private void Start()
    {
        if (instance == this && ShouldLoadMainMenuOnStart())
        {
            Debug.Log($"[SCENE FLOW] Bootstrap -> {mainMenuSceneName}");
            LoadSceneByName(mainMenuSceneName);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        if (instance == this) instance = null;
    }

    private void Update()
    {
        if (instance != this || PauseMenuController.GameplayInputBlocked) return;
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool yPressed = false;
        leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out yPressed);

        if (!skipSuppressed && ControllerSceneSkipAllowedIn(currentSceneName) && yPressed && !yButtonHeld)
        {
            yButtonHeld = true;
            Debug.Log("[SCENE FLOW] Skip triggered");
            SkipScene();
        }

        if (!yPressed)
        {
            yButtonHeld = false;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool keyboardSkipDown = Input.GetKeyDown(KeyCode.F7);
        bool controllerSkipDown = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        if (keyboardSkipDown || controllerSkipDown) TryDebugSkipMainLoop(keyboardInput: !controllerSkipDown);
        #endif

        if (!skipSuppressed && ShouldHandleDeveloperSceneSkip() && Input.GetKeyDown(developerSkipKey))
        {
            TryDeveloperSkipCurrentScene();
        }
    }

    public void StartNewJourney()
    {
        if (isLoading)
        {
            return;
        }

        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogError("[SCENE FLOW] Cannot start a new journey because no canonical scene order is configured.");
            return;
        }

        bool created = Level1GameState.ExistingInstance != null
            ? Level1GameState.ExistingInstance.ResetProfileToDefaults()
            : new LocalSaveManager().CreateFreshProfile();
        if (!created) return;

        currentIndex = -1;
        Debug.Log($"[SCENE FLOW] Starting new journey at index 0: {scenes[0]}");
        LoadSceneByName(scenes[0]);
    }

    public bool TryStartNewJourneyInSlot(int slot, string saveName, string characterName, string gender, bool overwriteConfirmed)
    {
        if (isLoading || !LocalSaveManager.IsValidSlot(slot) || scenes == null || scenes.Length == 0 ||
            !Application.CanStreamedLevelBeLoaded(scenes[0]) ||
            !LocalSaveManager.TryCreateJourneyInSlot(slot, saveName, characterName, gender, overwriteConfirmed))
            return false;

        Level1GameState.ExistingInstance?.ReloadProfileFromDisk();
        ApplyPlayerIdentity(saveName.Trim(), characterName.Trim(), gender);
        currentIndex = -1;
        LoadSceneByName(scenes[0]);
        return true;
    }

    public void ContinueJourney()
    {
        if (isLoading)
        {
            return;
        }

        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogError("[SCENE FLOW] Cannot continue because no canonical scene order is configured.");
            return;
        }

        if (!LocalSaveManager.ActiveProfileExists())
        {
            Debug.LogWarning("[SCENE FLOW] No save found for Continue.");
            return;
        }

        LocalSaveManager saveManager = new LocalSaveManager();
        if (!saveManager.TryLoadExistingProfile(out LocalProfileData profile))
        {
            Debug.LogError("[SCENE FLOW] Continue failed because the profile cannot be read.");
            return;
        }
        if (!TryResolveContinueScene(profile, out string targetScene, out int targetIndex))
        {
            Debug.LogError("[SCENE FLOW] Continue failed because the save does not reference a canonical progression scene.");
            return;
        }

        Debug.Log($"[SCENE FLOW] Continue resolved '{targetScene}' to index {targetIndex}");
        LoadSceneByName(targetScene);
    }

    public bool TryContinueJourneyInSlot(int slot)
    {
        if (isLoading || !LocalSaveManager.IsValidSlot(slot) ||
            !LocalSaveManager.TryLoadSlot(slot, out LocalProfileData profile) ||
            !TryResolveContinueScene(profile, out string targetScene, out _) ||
            !Application.CanStreamedLevelBeLoaded(targetScene)) return false;

        LocalSaveManager.SelectSlot(slot);
        Level1GameState.ExistingInstance?.ReloadProfileFromDisk();
        ApplyPlayerIdentity(profile.save_name, profile.character_name, profile.selected_gender);
        LoadSceneByName(targetScene);
        return true;
    }

    public bool TryGetContinueScene(LocalProfileData profile, out string sceneName)
    {
        return TryResolveContinueScene(profile, out sceneName, out _) &&
               Application.CanStreamedLevelBeLoaded(sceneName);
    }

    private static void ApplyPlayerIdentity(string saveName, string characterName, string gender)
    {
        PlayerPrefs.SetString(MainMenuSaveFlow.SaveNameKey, string.IsNullOrWhiteSpace(saveName) ? "Journey" : saveName);
        PlayerPrefs.SetString(MainMenuSaveFlow.CharacterNameKey, string.IsNullOrWhiteSpace(characterName) ? "Merchant" : characterName);
        PlayerPrefs.SetString(MainMenuSaveFlow.GenderKey, string.IsNullOrWhiteSpace(gender) ? "Male" : gender);
        PlayerPrefs.Save();
    }

    public void LoadNextScene()
    {
        if (isLoading || PauseMenuController.GameplaySuspended)
        {
            return;
        }

        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogError("[SCENE FLOW] Cannot load next scene because no canonical scene order is configured.");
            return;
        }

        int nextIndex = currentIndex >= 0 ? currentIndex + 1 : 0;
        if (nextIndex >= scenes.Length)
        {
            Debug.Log("[SCENE FLOW] GAME COMPLETE");
            return;
        }

        PrepareCurrentSceneForAdvance(nextIndex);
        Debug.Log($"[SCENE FLOW] Advancing {currentIndex} -> {nextIndex}: {scenes[nextIndex]}");
        LoadSceneByName(scenes[nextIndex]);
    }

    public void RestartCurrentScene()
    {
        if (isLoading) return;
        string sceneName = SceneManager.GetActiveScene().name;
        if (!IsGameplayScene(sceneName)) return;
        if (!Application.CanStreamedLevelBeLoaded(sceneName)) return;
        if (sceneName == DefaultGameplaySceneName && !Level1GameState.Instance.RestoreLevel1EntrySnapshot())
            return; // Do not reload into a partially reset or unsaved attempt.
        LoadSceneByName(sceneName, abandonCurrentWork: true);
    }

    public void ReturnToMainMenu()
    {
        if (!isLoading) LoadSceneByName(mainMenuSceneName, abandonCurrentWork: true);
    }

    public void LoadSceneByName(string sceneName, bool abandonCurrentWork = false)
    {
        if (isLoading)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SCENE FLOW] Cannot load an empty scene name.");
            return;
        }

        string targetSceneName = sceneName.Trim();
        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError("[SCENE FLOW] Scene is not available in the build scene list: " + targetSceneName);
            return; // Do not abandon local work or write progression for an unavailable destination.
        }
        int targetIndex = GetProgressionIndex(targetSceneName);
        if (targetIndex >= 0)
        {
            currentIndex = targetIndex;
        }
        else if (string.Equals(targetSceneName, mainMenuSceneName, System.StringComparison.Ordinal))
        {
            currentIndex = -1;
        }
        else
        {
            currentIndex = -1;
            Debug.LogWarning($"[SCENE FLOW] Loading non-progression scene '{targetSceneName}'. Progression index reset to -1.");
        }

        StartCoroutine(LoadRoutine(targetSceneName, abandonCurrentWork || targetSceneName == mainMenuSceneName));
    }

    // Level 1 uses Y for its ledger; it must not also advance the scene/progression.
    public static bool ControllerSceneSkipAllowedIn(string sceneName)
    {
        return !string.Equals(sceneName, DefaultGameplaySceneName, System.StringComparison.Ordinal);
    }

    public void SkipScene()
    {
        if (isLoading)
        {
            return;
        }

        Debug.Log("[SCENE FLOW] Scene skipped");
        LoadNextScene();
    }

    private IEnumerator LoadRoutine(string targetSceneName, bool abandonCurrentWork)
    {
        isLoading = true;
        PauseMenuController pause = GetComponent<PauseMenuController>();
        try
        {
            pause?.PrepareForTransition();

            PreserveCommittedProgress(abandonCurrentWork);

            // A new Level 1 visit gets its own entry baseline. Continue and Restart retain
            // the baseline already stored in the selected save slot.
            if (targetSceneName == DefaultGameplaySceneName && currentSceneName != DefaultGameplaySceneName)
            {
                var saves = new LocalSaveManager();
                if (!saves.TryLoadExistingProfile(out LocalProfileData saved) ||
                    (saved.current_scene != DefaultGameplaySceneName &&
                     !Level1GameState.Instance.CaptureLevel1EntrySnapshot(true)))
                {
                    Debug.LogError("[SCENE FLOW] Cannot enter Level 1 without saving its restart baseline.");
                    yield break;
                }
            }

            int targetIndex = GetProgressionIndex(targetSceneName);

            ResolveTransitionFader();
            if (fader != null)
            {
                fader.RepositionToCamera();
                yield return fader.FadeOut();
            }

            Debug.Log("[SCENE FLOW] Loading " + targetSceneName);

            AsyncOperation loadOperation = null;
            try { loadOperation = SceneManager.LoadSceneAsync(targetSceneName); }
            catch (System.Exception error) { Debug.LogError("[SCENE FLOW] Scene load failed: " + error.Message); }
            if (loadOperation == null)
            {
                Debug.LogError("[SCENE FLOW] Failed to start loading scene '" + targetSceneName + "'.");
                if (fader != null) yield return fader.FadeIn();
                yield break;
            }

            yield return loadOperation;

            Debug.Log("[SCENE FLOW] " + targetSceneName + " loaded");

            // A destination becomes a resume checkpoint only after Unity has loaded it.
            if (targetIndex >= 0)
                PersistProgressionState(targetSceneName, targetIndex);

            ResolveTransitionFader();
            if (fader != null)
            {
                yield return fader.WaitForCamera();
                yield return fader.FadeIn();
            }

        }
        finally
        {
            isLoading = false;
            pause?.FinishTransition();
        }
    }

    private void PreserveCommittedProgress(bool abandonCurrentWork)
    {
        var state = Level1GameState.ExistingInstance;
        if (state == null) return;
        state.SaveProfileToDisk();
        if (abandonCurrentWork) state.ReloadProfileFromDisk();
    }

    private void InitializeRuntimeState()
    {
        if (runtimeStateInitialized)
        {
            return;
        }

        EnsureSceneDefaults();
        SyncCurrentScene(SceneManager.GetActiveScene().name);
        runtimeStateInitialized = true;
    }

    private void EnsurePersistentControllers()
    {
        PauseMenuController controller = GetComponent<PauseMenuController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<PauseMenuController>();
            Debug.Log("[SCENE FLOW] Added missing PauseMenuController to persistent GameManager.");
        }

        if (controller.InitializeOwnership(this))
        {
            // Component activation and gameplay readiness are deliberately separate.
            controller.enabled = enablePauseMenuController;
        }
    }

    private ScreenFader ResolveTransitionFader()
    {
        if (fader == null)
        {
            // Never adopt a story sequence's independent scene-local fader.
            fader = GetComponentInChildren<ScreenFader>(true);
        }

        if (fader != null && !fader.transform.IsChildOf(transform))
        {
            // Bootstrap authors the fade canvas as a separate root.
            Canvas canvas = fader.GetComponentInParent<Canvas>(true);
            if (canvas != null && canvas.transform.parent == null)
            {
                canvas.transform.SetParent(transform, true);
            }
        }

        if (fader != null && (!fader.transform.IsChildOf(transform) || fader.fadeImage == null))
        {
            fader = null;
        }

        if (fader != null && fader.fadeImage != null)
        {
            // A transparent persistent overlay must not intercept scene UI rays.
            fader.fadeImage.raycastTarget = false;
        }

        return fader;
    }

    private void EnsureSceneDefaults()
    {
        if (string.IsNullOrWhiteSpace(bootstrapSceneName))
        {
            bootstrapSceneName = DefaultBootstrapSceneName;
        }

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            mainMenuSceneName = DefaultMainMenuSceneName;
        }

        if (scenes == null || scenes.Length == 0)
        {
            scenes = (string[])DefaultSceneOrder.Clone();
            return;
        }

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = scenes[i] != null ? scenes[i].Trim() : string.Empty;
        }
    }

    private bool ShouldLoadMainMenuOnStart()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return string.Equals(activeScene.name, bootstrapSceneName, System.StringComparison.Ordinal);
    }

    private bool ShouldHandleDeveloperSceneSkip()
    {
        // Level 1 uses only the opt-in F7/B shortcut; its missing authored index makes Next unsafe.
        return currentSceneName != DefaultGameplaySceneName &&
            enableDeveloperSceneSkip && (Debug.isDebugBuild || Application.isEditor);
    }

    private bool CanDebugSkipMainLoop(bool keyboardInput)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!enableDebugSceneSkip || isLoading || skipSuppressed ||
            PauseMenuController.GameplayInputBlocked ||
            currentSceneName != DefaultGameplaySceneName) return false;

        if (!keyboardInput) return true;

        if (TouchScreenKeyboard.visible || !string.IsNullOrEmpty(Input.compositionString)) return false;

        GameObject selected = UnityEngine.EventSystems.EventSystem.current != null
            ? UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject : null;
        if (selected != null &&
            (selected.GetComponentInParent<TMPro.TMP_InputField>() != null ||
             selected.GetComponentInParent<UnityEngine.UI.InputField>() != null)) return false;

        // Only evaluated on the debug key-down, never as a per-frame UI scan.
        foreach (UnityEngine.UI.Selectable control in UnityEngine.UI.Selectable.allSelectablesArray)
        {
            if (control is TMPro.TMP_InputField tmp && tmp.isFocused) return false;
            if (control is UnityEngine.UI.InputField input && input.isFocused) return false;
        }
        return true;
        #else
        return false;
        #endif
    }

    private void TryDebugSkipMainLoop(bool keyboardInput)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!CanDebugSkipMainLoop(keyboardInput)) return;
        Debug.Log("[SCENE FLOW] Debug F7/B: skipping Level 1 to Epilogue_BookPurchase without awarding results.");
        LoadSceneByName("Epilogue_BookPurchase", abandonCurrentWork: true);
        #endif
    }

    private void TryDeveloperSkipCurrentScene()
    {
        if (isLoading)
        {
            return;
        }

        if (scenes == null || scenes.Length == 0)
        {
            Debug.LogWarning("[SCENE FLOW] Developer skip requested, but no canonical scene order is configured.");
            return;
        }

        int nextIndex = currentIndex >= 0 ? currentIndex + 1 : 0;
        string intendedScene = nextIndex >= 0 && nextIndex < scenes.Length
            ? scenes[nextIndex]
            : "<end-of-flow>";

        Debug.Log($"[SCENE FLOW] Developer skip requested with {developerSkipKey}. Current='{currentSceneName}', Next='{intendedScene}'.");
        LoadNextScene();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SyncCurrentScene(scene.name);
        ResolveTransitionFader();
    }

    private void SyncCurrentScene(string sceneName)
    {
        currentSceneName = sceneName ?? string.Empty;
        currentIndex = GetProgressionIndex(sceneName);
    }

    private int GetProgressionIndex(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || scenes == null)
        {
            return -1;
        }

        for (int i = 0; i < scenes.Length; i++)
        {
            if (string.Equals(scenes[i], sceneName, System.StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private bool TryResolveContinueScene(LocalProfileData profile, out string targetScene, out int targetIndex)
    {
        targetScene = string.Empty;
        targetIndex = -1;
        string savedScene = profile != null ? profile.current_scene : string.Empty;
        int savedIndex = profile != null ? profile.progression_index : -1;

        int sceneIndex = GetProgressionIndex(savedScene);
        if (sceneIndex >= 0)
        {
            targetIndex = sceneIndex;
            targetScene = scenes[targetIndex];
            return true;
        }

        if (savedIndex >= 0 && savedIndex < scenes.Length)
        {
            targetIndex = savedIndex;
            targetScene = scenes[targetIndex];
            return true;
        }

        // A newly created slot may exist before its first scene transition finishes.
        if (string.IsNullOrEmpty(savedScene) && savedIndex == LocalSaveManager.DefaultProgressionIndex)
        {
            targetIndex = 0;
            targetScene = scenes[0];
            return true;
        }

        return false;
    }

    private void PrepareCurrentSceneForAdvance(int nextIndex)
    {
        if (nextIndex <= currentIndex)
        {
            return;
        }

        if (string.Equals(currentSceneName, DefaultIntroSceneName, System.StringComparison.Ordinal))
        {
            LocalSaveManager saveManager = new LocalSaveManager();
            LocalProfileData profile = saveManager.LoadProfile();
            profile.intro_completed = true;
            saveManager.SaveProfile(profile);
        }
    }

    private void PersistProgressionState(string sceneName, int progressionIndex)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || progressionIndex < 0)
        {
            return;
        }

        Debug.Log($"[SCENE FLOW] Persisting destination scene: {sceneName}, index: {progressionIndex}");
        LocalSaveManager saveManager = new LocalSaveManager();
        LocalProfileData profile = saveManager.LoadProfile();
        profile.current_scene = sceneName;
        profile.progression_index = progressionIndex;
        profile.intro_completed = progressionIndex > 0;
        saveManager.SaveProfile(profile);
    }
}
