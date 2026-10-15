using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueStoryManager : MonoBehaviour
{
    public static EpilogueStoryManager Instance { get; private set; }

    public enum EpilogueStage
    {
        BhaskaraPaycheck,
        ReturnToMeera,
        PurchaseBook,
        OpenBook,
        Transitioning,
        Complete
    }

    [Header("Current Stage")]
    public EpilogueStage currentStage = EpilogueStage.BhaskaraPaycheck;

    [Header("Configuration")]
    [Tooltip("Final ending scene loaded after the book is selected.")]
    public string endingSceneName = "Epilogue_ModernStudyRoom";
    [Tooltip("Matches ModernStudyRoomIntroController's whiteout duration.")]
    public float whiteFadeDuration = 1.2f;
    public int bookPrice = 18;
    public int defaultPaycheck = 15;

    [Header("UI")]
    public EpilogueWalletHUD walletHUD;

    [Header("Objective UI")]
    [SerializeField] private ObjectiveUIManager objectiveUI;

    [Header("Meera Directional Indicator")]
    [Tooltip("Assign the NPCDirectionalIndicator from the scene (same one used in FreeRoam).")]
    [SerializeField] private NPCDirectionalIndicator meeraDirectionalIndicator;

    [Tooltip("Assign Meera's Transform so the arrow points toward her.")]
    [SerializeField] private Transform meeraTarget;

    [Tooltip("Assign the world-space particle GameObject that appears above Meera.")]
    [SerializeField] private GameObject meeraWorldMarker;

    [Header("Book Interaction")]
    [Tooltip("Assign the EndingBookInteraction component on the epilogue book.")]
    [SerializeField] private EndingBookInteraction endingBookInteraction;

    [Header("Meera Zone")]
    [Tooltip("Assign the EpilogueMeeraApproachTrigger so purchase input is enabled after approach dialogue.")]
    [SerializeField] private EpilogueMeeraApproachTrigger meeraTrigger;

    [Header("System References")]
    [SerializeField] private ScreenFader fader;
    [SerializeField] private TeleportManager teleportManager;

    private bool sequenceRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Resolve optional singletons if not wired in Inspector.
        if (objectiveUI == null)
            objectiveUI = ObjectiveUIManager.Instance;

        if (teleportManager == null && TeleportManager.Instance != null)
            teleportManager = TeleportManager.Instance;

        if (teleportManager != null)
            teleportManager.DisableAll();

        // Indicator and book both start hidden/locked.
        meeraDirectionalIndicator?.Hide();
        objectiveUI?.HideObjective();
        endingBookInteraction?.Disable();

        StartCoroutine(BhaskaraSequence());
    }

    // ─── BHASKARA SEQUENCE ────────────────────────────────────────────────────

    private IEnumerator BhaskaraSequence()
    {
        sequenceRunning = true;

        yield return new WaitForSeconds(1f);

        if (NarratorUIManager.Instance != null)
        {
            // Exactly matches the single recorded BHASKARA_EPILOGUE_01.wav.
            yield return NarratorUIManager.Instance.PlayNarration(
                "BHASKARA_EPILOGUE_01", "Bhaskara",
                "You did well today. Here is your share of the earnings.", 5f);
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }

        // ── Paycheck ──────────────────────────────────────────────────────────

        LocalSaveManager saveManager = new LocalSaveManager();
        LocalProfileData profile = saveManager.LoadProfile();

        int balanceBefore =
            profile.global_metrics.total_varahas;

        int paycheck = defaultPaycheck;

        if (balanceBefore + paycheck < bookPrice)
            paycheck = bookPrice - balanceBefore;

        profile.global_metrics.total_varahas += paycheck;

        saveManager.SaveProfile(profile);

        if (Level1GameState.Instance != null)
            Level1GameState.Instance.ReloadProfileFromDisk();

        if (walletHUD != null)
        {
            walletHUD.SetBalance(
                profile.global_metrics.total_varahas
            );

            walletHUD.ShowGain(paycheck);
        }

        yield return new WaitForSeconds(1f);

        // Narrator line
        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                "NARRATOR_EPILOGUE_01",
                "Narrator",
                "With enough Varahas now in hand, perhaps it is finally time to return to Meera.",
                6f
            );
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }

        // ── Advance to ReturnToMeera ─────────────────────────────────────────

        currentStage = EpilogueStage.ReturnToMeera;

        objectiveUI?.SetObjective(
            "Purchase the book from Meera"
        );

        // Point directional indicator at Meera.
        if (meeraDirectionalIndicator != null &&
            meeraTarget != null)
        {
            meeraDirectionalIndicator.SetTarget(
                meeraTarget,
                meeraWorldMarker
            );

            meeraDirectionalIndicator.Show();

            Debug.Log(
                "[EPILOGUE INDICATOR] Show Meera indicator"
            );
        }
        else
        {
            Debug.LogWarning(
                "[EPILOGUE INDICATOR] Could not show — " +
                $"indicator={(meeraDirectionalIndicator != null ? "OK" : "NULL")}, " +
                $"target={(meeraTarget != null ? "OK" : "NULL")}"
            );
        }

        if (teleportManager != null)
            teleportManager.EnableGroup("General");

        sequenceRunning = false;
    }

    // ─── MEERA APPROACHED ─────────────────────────────────────────────────────

    public void NotifyMeeraApproached()
    {
        if (currentStage != EpilogueStage.ReturnToMeera)
            return;

        currentStage = EpilogueStage.PurchaseBook;

        // Player has reached Meera, so navigation indicator can disappear.
        meeraDirectionalIndicator?.Hide();

        StartCoroutine(MeeraApproachSequence());
    }

    private IEnumerator MeeraApproachSequence()
    {
        sequenceRunning = true;

        if (teleportManager != null)
            teleportManager.DisableAll();

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                "MEERA_EPILOGUE_01",
                "Meera",
                "You came back.",
                4f
            );
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        // Meera's greeting is complete.
        // Continue directly to the purchase without requiring X.
        sequenceRunning = false;

        Debug.Log("[EPILOGUE] Automatically beginning book purchase.");

        yield return MeeraPurchaseSequence();
    }

    // ─── MEERA INTERACTED / PURCHASE ─────────────────────────────────────────

    public void NotifyMeeraInteracted()
    {
        if (currentStage != EpilogueStage.PurchaseBook ||
            sequenceRunning)
        {
            return;
        }

        StartCoroutine(MeeraPurchaseSequence());
    }

    private IEnumerator MeeraPurchaseSequence()
    {
        sequenceRunning = true;

        if (teleportManager != null)
            teleportManager.DisableAll();

        if (NarratorUIManager.Instance != null)
        {
            // PLAYER_EPILOGUE_01 can be subtitle-only if no WAV was created.
            yield return NarratorUIManager.Instance.PlayNarration(
                "PLAYER_EPILOGUE_01", "You",
                "I'd like to buy that book.", 3f);

            yield return NarratorUIManager.Instance.PlayNarration(
                "MEERA_EPILOGUE_02", "Meera",
                $"It is yours for {bookPrice} Varahas.", 4f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        // ── Deduct book price ─────────────────────────────────────────────────

        LocalSaveManager saveManager =
            new LocalSaveManager();

        LocalProfileData profile =
            saveManager.LoadProfile();

        profile.global_metrics.total_varahas -=
            bookPrice;

        if (profile.global_metrics.total_varahas < 0)
        {
            profile.global_metrics.total_varahas = 0;
        }

        saveManager.SaveProfile(profile);

        if (Level1GameState.Instance != null)
            Level1GameState.Instance.ReloadProfileFromDisk();

        if (walletHUD != null)
        {
            walletHUD.ShowSpend(bookPrice);

            walletHUD.SetBalance(
                profile.global_metrics.total_varahas
            );
        }

        // Final Meera line
        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                "MEERA_EPILOGUE_03",
                "Meera",
                "Take it. Perhaps it will answer your questions.",
                5f
            );
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        // ── Advance to OpenBook ───────────────────────────────────────────────

        currentStage = EpilogueStage.OpenBook;

        objectiveUI?.SetObjective("Take the book");

        if (endingBookInteraction != null)
        {
            endingBookInteraction.Enable();
            Debug.Log("[EPILOGUE BOOK] OpenBook reached; interaction enabled.");
        }
        else
        {
            Debug.LogError(
                "[EPILOGUE BOOK] EndingBookInteraction reference is missing on EpilogueStoryManager."
            );
        }

        // NEW: Show the book selection tutorial prompt
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowPrompt(
                "Take the Book",
                "Point at the book and press Right Trigger to take it.",
                this
            );
        }

        if (teleportManager != null)
            teleportManager.EnableGroup("General");

        sequenceRunning = false;
    }

    // ─── BOOK INTERACTED ──────────────────────────────────────────────────────

    public void NotifyBookInteracted()
    {
        Debug.Log($"[EPILOGUE BOOK] NotifyBookInteracted received. Stage={currentStage}");
        if (currentStage != EpilogueStage.OpenBook)
        {
            Debug.LogWarning($"[EPILOGUE BOOK] Ignored: expected OpenBook, got {currentStage}.");
            return;
        }

        currentStage = EpilogueStage.Transitioning;
        objectiveUI?.HideObjective();
        endingBookInteraction?.Disable();
        Debug.Log("[EPILOGUE BOOK] Transition accepted. Starting white fade.");
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        if (teleportManager != null)
            teleportManager.DisableAll();

        meeraDirectionalIndicator?.Hide();

        if (fader == null)
            fader =
                FindFirstObjectByType<ScreenFader>();

        if (fader != null)
        {
            // Fade the (persistent) ScreenFader to full white.
            // It is left white on purpose: GameManager's FadeOut is then a
            // no-op, the load happens under white, and ModernStudyRoomEndingController
            // restores the fader colour to black once the load has finished.
            float originalSpeed = fader.speed;
            fader.fadeColor = Color.white;
            fader.speed = 1f / Mathf.Max(0.01f, whiteFadeDuration);
            fader.RepositionToCamera();

            yield return fader.FadeOut();

            fader.speed = originalSpeed;
        }

        currentStage =
            EpilogueStage.Complete;

        Debug.Log($"[EPILOGUE BOOK] Loading scene '{endingSceneName}'.");
        if (GameManager.ExistingInstance != null)
        {
            GameManager.ExistingInstance.LoadSceneByName(endingSceneName);
        }
        else
        {
            // Direct Editor testing without Bootstrap: load the authored ending scene.
            Debug.LogWarning("[EPILOGUE BOOK] Bootstrap GameManager absent; using direct scene-load fallback.");
            SceneManager.LoadScene(endingSceneName);
        }
    }
}