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
    [SerializeField] private string[] scenes =
    {
        DefaultIntroSceneName,
        DefaultGameplaySceneName
    };
    [SerializeField] private ScreenFader fader;

    [Header("Persistent Controllers")]
    [SerializeField] private bool enablePauseMenuController;

    [Header("Skip")]
    private bool yButtonHeld;

    [Header("Developer Testing")]
    [SerializeField] private bool enableDeveloperSceneSkip = true;
    [SerializeField] private KeyCode developerSkipKey = KeyCode.F8;

    private int currentIndex = -1;
    private string currentSceneName = string.Empty;
    private bool isLoading;
    private bool runtimeStateInitialized;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject host = new GameObject(nameof(GameManager));
                    instance = host.AddComponent<GameManager>();
                    Debug.Log("[SCENE FLOW] Runtime GameManager instance created.");
                }
            }

            instance.InitializeRuntimeState();
            instance.EnsurePersistentControllers();
            return instance;
        }
    }

    public string CurrentSceneName => currentSceneName;
    public int CurrentProgressionIndex => currentIndex;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeRuntimeState();
        EnsurePersistentControllers();
        Debug.Log("[SCENE FLOW] Bootstrap loaded");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        if (ShouldLoadMainMenuOnStart())
        {
            Debug.Log($"[SCENE FLOW] Bootstrap -> {mainMenuSceneName}");
            LoadSceneByName(mainMenuSceneName);
        }
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    private void Update()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        bool yPressed = false;
        leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out yPressed);

        if (yPressed && !yButtonHeld)
        {
            yButtonHeld = true;
            Debug.Log("[SCENE FLOW] Skip triggered");
            SkipScene();
        }

        if (!yPressed)
        {
            yButtonHeld = false;
        }

        if (ShouldHandleDeveloperSceneSkip() && Input.GetKeyDown(developerSkipKey))
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

        if (Level1GameState.ExistingInstance != null)
        {
            Level1GameState.ExistingInstance.ResetProfileToDefaults();
        }
        else
        {
            LocalSaveManager.DeleteActiveProfile();
        }

        currentIndex = -1;
        Debug.Log($"[SCENE FLOW] Starting new journey at index 0: {scenes[0]}");
        LoadSceneByName(scenes[0]);
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
            Debug.Log("[SCENE FLOW] No save found for Continue. Falling back to New Journey.");
            StartNewJourney();
            return;
        }

        LocalSaveManager saveManager = new LocalSaveManager();
        LocalProfileData profile = saveManager.LoadProfile();
        if (!TryResolveContinueScene(profile, out string targetScene, out int targetIndex))
        {
            Debug.LogError("[SCENE FLOW] Continue failed because the save does not reference a canonical progression scene.");
            return;
        }

        Debug.Log($"[SCENE FLOW] Continue resolved '{targetScene}' to index {targetIndex}");
        LoadSceneByName(targetScene);
    }

    public void LoadNextScene()
    {
        if (isLoading)
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

    public void LoadSceneByName(string sceneName)
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

        StartCoroutine(LoadRoutine(targetSceneName));
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

    private IEnumerator LoadRoutine(string targetSceneName)
    {
        isLoading = true;

        if (Level1GameState.ExistingInstance != null)
        {
            Level1GameState.ExistingInstance.SaveProfileToDisk();
        }

        int targetIndex = GetProgressionIndex(targetSceneName);
        if (targetIndex >= 0)
        {
            PersistProgressionState(targetSceneName, targetIndex);
        }

        if (fader != null)
        {
            yield return fader.FadeOut();
        }

        Debug.Log("[SCENE FLOW] Loading " + targetSceneName);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName);
        if (loadOperation == null)
        {
            Debug.LogError("[SCENE FLOW] Failed to start loading scene '" + targetSceneName + "'.");
            isLoading = false;
            yield break;
        }

        yield return loadOperation;

        Debug.Log("[SCENE FLOW] " + targetSceneName + " loaded");

        if (fader != null)
        {
            yield return fader.FadeIn();
        }

        isLoading = false;
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
        if (!enablePauseMenuController)
        {
            return;
        }

        if (GetComponent<PauseMenuController>() == null)
        {
            gameObject.AddComponent<PauseMenuController>();
            Debug.Log("[SCENE FLOW] Added missing PauseMenuController to persistent GameManager.");
        }
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
        return enableDeveloperSceneSkip && (Debug.isDebugBuild || Application.isEditor);
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
