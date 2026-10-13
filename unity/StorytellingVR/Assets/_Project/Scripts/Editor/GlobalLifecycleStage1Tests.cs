using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Inactive fixtures invoke lifecycle methods directly: no Start, scene loads or gameplay loops.
public class GlobalLifecycleStage1Tests
{
    private readonly List<GameObject> hosts = new List<GameObject>();
    private string saveDirectory;
    private string previousSaveOverride;
    private float previousTimeScale;
    private bool previousAudioPause;
    private bool ownsFixture;

    private static readonly string[] CanonicalScenes =
    {
        "ModernStudyRoom", "GateIntro", "FreeRoam_WithTerrain", "NewSpiceScene",
        "NewTransactionTutorial", "NewCoinScene", "SpicesInteraction", "Level1_MainLoopUpdated"
    };

    [SetUp]
    public void SetUp()
    {
        ownsFixture = false;
        Assert.IsFalse(Application.isPlaying, "Run this fixture in EditMode outside Play Mode.");
        Assert.IsTrue(GameManager.ExistingInstance == null, "Do not replace a live GameManager.");
        Assert.IsTrue(Level1GameState.ExistingInstance == null, "Do not replace a live profile.");
        Assert.IsNull(Field(typeof(PauseMenuController), "instance", true).GetValue(null));
        previousSaveOverride = LocalSaveManager.EditorTestSaveDirectoryOverride;
        saveDirectory = Path.Combine(Path.GetTempPath(), "GlobalLifecycleStage1_" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = saveDirectory;
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        ownsFixture = true;
    }

    [TearDown]
    public void TearDown()
    {
        if (!ownsFixture) return;
        // A shared-host migration / lazy accessor can create a root outside the fixture list.
        if (Level1GameState.ExistingInstance != null)
        {
            Level1GameState state = Level1GameState.ExistingInstance;
            Invoke(state, "OnDestroy");
            Object.DestroyImmediate(state.gameObject);
        }
        // Unity does not send OnDestroy to hosts that have always remained inactive.
        if (GameManager.ExistingInstance != null)
        {
            GameManager manager = GameManager.ExistingInstance;
            PauseMenuController pause = manager.GetComponent<PauseMenuController>();
            if (pause != null) Invoke(pause, "OnDestroy");
            Invoke(manager, "OnDestroy");
        }
        foreach (GameObject host in hosts)
            if (host != null) Object.DestroyImmediate(host);
        hosts.Clear();
        LocalSaveManager.EditorTestSaveDirectoryOverride = previousSaveOverride;
        Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        if (saveDirectory != null && Directory.Exists(saveDirectory)) Directory.Delete(saveDirectory, true);
    }

    [Test]
    public void DuplicateStateRemovesOnlyItsComponentAndRetainsAuthoritativeProfile()
    {
        Level1GameState authoritative = State();
        object profile = Field(typeof(Level1GameState), "profile").GetValue(authoritative);
        int money = authoritative.CurrentMoney;
        GameObject gameplay = Host("GAME_SYSTEM duplicate fixture");
        BoxCollider unrelated = gameplay.AddComponent<BoxCollider>();
        GameObject child = Host("scene-local managers fixture");
        child.transform.SetParent(gameplay.transform);
        Level1GameState duplicate = gameplay.AddComponent<Level1GameState>();

        Invoke(duplicate, "Awake");

        Assert.IsTrue(duplicate == null);
        Assert.IsTrue(gameplay != null && unrelated != null && child != null);
        Assert.AreSame(gameplay.transform, child.transform.parent);
        Assert.AreSame(authoritative, Level1GameState.Instance);
        Assert.AreSame(profile, Field(typeof(Level1GameState), "profile").GetValue(authoritative));
        Assert.AreEqual(money, authoritative.CurrentMoney);
    }

    [Test]
    public void DirectEntrySharedStateHostMigratesOnlyStateToDedicatedRoot()
    {
        GameObject gameplay = Host("legacy shared GAME_SYSTEM");
        BoxCollider unrelated = gameplay.AddComponent<BoxCollider>();
        GameObject child = Host("scene-local manager");
        child.transform.SetParent(gameplay.transform);
        Level1GameState authored = gameplay.AddComponent<Level1GameState>();
        Field(typeof(Level1GameState), "marketDayDurationSeconds").SetValue(authored, 2300f);

        Invoke(authored, "Awake");

        Level1GameState authoritative = Level1GameState.ExistingInstance;
        Assert.IsTrue(authored == null);
        Assert.IsNotNull(authoritative);
        Assert.AreNotSame(gameplay, authoritative.gameObject);
        Assert.IsNull(authoritative.transform.parent);
        Assert.AreEqual(0, authoritative.transform.childCount);
        Assert.AreEqual(2300f, authoritative.MarketDayDurationSeconds);
        Assert.IsTrue(unrelated != null);
        Assert.AreSame(gameplay.transform, child.transform.parent);
        Assert.IsNull(gameplay.GetComponent<Level1GameState>());
    }

    [Test]
    public void DedicatedNestedStateDetachesWithoutPersistingItsParent()
    {
        GameObject parent = Host("scene-local parent");
        Level1GameState state = Host("dedicated data host").AddComponent<Level1GameState>();
        state.transform.SetParent(parent.transform);
        Invoke(state, "Awake");
        Assert.AreSame(state, Level1GameState.ExistingInstance);
        Assert.IsNull(state.transform.parent);
        Assert.IsTrue(parent != null);
    }

    [Test]
    public void StateAccessorAndRepeatedInitializationKeepSameProfile()
    {
        Level1GameState state = State();
        object profile = Field(typeof(Level1GameState), "profile").GetValue(state);
        Invoke(state, "Awake");
        Assert.AreSame(state, Level1GameState.Instance);
        Assert.AreSame(profile, Field(typeof(Level1GameState), "profile").GetValue(state));
    }

    [Test]
    public void GameManagerInitializationAndAccessorAreIdempotent()
    {
        GameManager manager = Manager();
        Invoke(manager, "Awake");
        Assert.AreSame(manager, GameManager.Instance);
        Assert.AreSame(manager, GameManager.Instance);
        Assert.AreEqual(1, manager.GetComponents<PauseMenuController>().Length);
        Assert.AreEqual(SceneManager.GetActiveScene().name, manager.CurrentSceneName);
        GameManager duplicate = Host("duplicate global manager").AddComponent<GameManager>();
        Invoke(duplicate, "Awake");
        Assert.IsTrue(duplicate == null);
        Assert.AreSame(manager, GameManager.ExistingInstance);
    }

    [Test]
    public void FallbackAndEmptyConfigurationUseFullCanonicalJourney()
    {
        GameManager manager = Manager();
        FieldInfo scenes = Field(typeof(GameManager), "scenes");
        CollectionAssert.AreEqual(CanonicalScenes, (string[])scenes.GetValue(manager));
        scenes.SetValue(manager, Array.Empty<string>());
        Invoke(manager, "EnsureSceneDefaults");
        CollectionAssert.AreEqual(CanonicalScenes, (string[])scenes.GetValue(manager));
        Assert.AreEqual(7, Invoke(manager, "GetProgressionIndex", "Level1_MainLoopUpdated"));
        var configured = new[] { "AuthoredSceneA", "AuthoredSceneB" };
        scenes.SetValue(manager, configured);
        Invoke(manager, "EnsureSceneDefaults");
        CollectionAssert.AreEqual(configured, (string[])scenes.GetValue(manager), "Preserve nonempty authored scene configuration.");
    }

    [Test]
    public void BootstrapDisabledPauseComponentIsReusedAndActivationIsExplicit()
    {
        GameObject host = Host("Bootstrap manager fixture");
        GameManager manager = host.AddComponent<GameManager>();
        Field(typeof(GameManager), "enablePauseMenuController").SetValue(manager, false);
        PauseMenuController pause = host.AddComponent<PauseMenuController>();
        pause.enabled = false;
        Invoke(pause, "Awake"); // Also exercises pause Awake before GameManager Awake.
        Invoke(manager, "Awake");
        Invoke(manager, "EnsurePersistentControllers");
        Assert.IsFalse(pause.enabled);
        Assert.AreSame(pause, host.GetComponent<PauseMenuController>());
        Assert.AreSame(pause, Field(typeof(PauseMenuController), "instance", true).GetValue(null));

        Field(typeof(GameManager), "enablePauseMenuController").SetValue(manager, true);
        Invoke(manager, "EnsurePersistentControllers");
        Assert.IsTrue(pause.enabled, "An existing disabled component must obey the ownership flag.");
        Assert.AreEqual(1, host.GetComponents<PauseMenuController>().Length);
        Assert.IsTrue(pause.IsReady, "Stage 2 deliberately enables the completed controller.");
        Time.timeScale = 0.6f;
        AudioListener.pause = true;
        Assert.IsFalse(pause.IsPaused);
        Assert.AreEqual(0.6f, Time.timeScale);
        Assert.IsTrue(AudioListener.pause);
        Assert.IsNull(host.GetComponentInChildren<PauseMenuView>(true), "Ownership alone must not instantiate or display a menu.");
    }

    [Test]
    public void UnownedPauseComponentCannotCompeteWithGlobalOwner()
    {
        GameManager manager = Manager();
        GameObject sceneHost = Host("unowned scene pause controller");
        BoxCollider unrelated = sceneHost.AddComponent<BoxCollider>();
        PauseMenuController duplicate = sceneHost.AddComponent<PauseMenuController>();
        Invoke(duplicate, "Awake");
        Assert.IsTrue(duplicate == null);
        Assert.IsTrue(unrelated != null);
        Assert.AreEqual(1, manager.GetComponents<PauseMenuController>().Length);
    }

    [Test]
    public void BootstrapFadeCanvasIsAdoptedWithoutCloningOrChangingTiming()
    {
        GameManager manager = Manager();
        ScreenFader fader = Fader("Bootstrap FadeCanvas");
        fader.speed = 1.7f;
        fader.fadeColor = Color.black;
        Field(typeof(GameManager), "fader").SetValue(manager, fader);
        Assert.AreSame(fader, Invoke(manager, "ResolveTransitionFader"));
        Assert.AreSame(manager.transform, fader.transform.parent);
        Assert.AreEqual(1, manager.GetComponentsInChildren<ScreenFader>(true).Length);
        Assert.AreEqual(1.7f, fader.speed);
        Assert.IsFalse(fader.fadeImage.raycastTarget);
    }

    [Test]
    public void DestroyedFaderClearsReferenceAndRecoversFromOwnedReplacementOnly()
    {
        GameManager manager = Manager();
        ScreenFader original = Fader("owned transition overlay");
        FieldInfo faderField = Field(typeof(GameManager), "fader");
        faderField.SetValue(manager, original);
        Invoke(manager, "ResolveTransitionFader");
        Object.DestroyImmediate(original.gameObject);
        ScreenFader storyFader = Fader("scene-local story fader");

        Assert.IsNull(Invoke(manager, "ResolveTransitionFader"));
        Assert.IsNull(faderField.GetValue(manager), "Discard the destroyed managed reference too.");
        Assert.IsNull(storyFader.transform.parent, "Do not take over a scene's story fader.");
        ScreenFader replacement = Fader("replacement owned transition overlay");
        replacement.transform.SetParent(manager.transform);
        Assert.AreSame(replacement, Invoke(manager, "ResolveTransitionFader"));
    }

    [Test]
    public void FaderWithDestroyedImageIsSkippedSafely()
    {
        GameManager manager = Manager();
        ScreenFader fader = Fader("damaged owned overlay");
        Field(typeof(GameManager), "fader").SetValue(manager, fader);
        Invoke(manager, "ResolveTransitionFader");
        Object.DestroyImmediate(fader.fadeImage);
        Assert.IsNull(Invoke(manager, "ResolveTransitionFader"));
        Assert.IsNull(Field(typeof(GameManager), "fader").GetValue(manager));
    }

    private GameObject Host(string name)
    {
        var host = new GameObject(name);
        host.SetActive(false);
        hosts.Add(host);
        return host;
    }

    private Level1GameState State()
    {
        var state = Host("authoritative profile fixture").AddComponent<Level1GameState>();
        Invoke(state, "Awake");
        return state;
    }

    private GameManager Manager()
    {
        var manager = Host("global manager fixture").AddComponent<GameManager>();
        Invoke(manager, "Awake");
        return manager;
    }

    private ScreenFader Fader(string name)
    {
        var host = new GameObject(name, typeof(RectTransform), typeof(Canvas));
        host.SetActive(false);
        hosts.Add(host);
        ScreenFader fader = host.AddComponent<ScreenFader>();
        fader.fadeImage = host.AddComponent<Image>();
        return fader;
    }

    private static FieldInfo Field(Type type, string name, bool isStatic = false)
    {
        return type.GetField(name, BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance));
    }

    private static object Invoke(object target, string method, params object[] args)
    {
        return target.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, args);
    }
}
