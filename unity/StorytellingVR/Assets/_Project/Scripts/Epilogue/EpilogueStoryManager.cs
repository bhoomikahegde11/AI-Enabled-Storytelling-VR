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
    public int defaultPaycheck = 20;

    [Header("Recording / Testing")]
    [Tooltip("Replay the full epilogue on an already-completed save without awarding or spending money again. Turn OFF for normal gameplay.")]
    [SerializeField] private bool replayFullEpilogue = false;

    [Header("UI")]
    public EpilogueWalletHUD walletHUD;

    [Header("Objective UI")]
    [SerializeField] private ObjectiveUIManager objectiveUI;

    [Header("Coin UI")]
    [SerializeField] private EpilogueEarningsPopup earningsPopup;

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
    private int currentWalletBalance;
    private bool bookPurchasedBeforeThisSequence;

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
        currentStage = EpilogueStage.BhaskaraPaycheck;

        // Let the book's Start and the scene checkpoint finish before restoring its stage.
        yield return null;

        Level1GameState.Instance.EnsureInitialized();
        var saves = new LocalSaveManager();
        bool profileLoaded = saves.TryLoadExistingProfile(out LocalProfileData checkpoint);
        bool paycheckAlreadyApplied = profileLoaded && checkpoint.epilogue_paycheck_applied;
        bookPurchasedBeforeThisSequence = profileLoaded && checkpoint.epilogue_book_purchased;

        Debug.Log($"[EPILOGUE] replay={replayFullEpilogue}, paid={paycheckAlreadyApplied}, purchased={bookPurchasedBeforeThisSequence}");

        //if ((!paycheckAlreadyApplied || replayFullEpilogue) && NarratorUIManager.Instance != null)
        //{
        //    yield return new WaitForSeconds(2f);
        //    if (checkpoint?.market_day?.NetTradingProfit > 0)
        //    {
        //        yield return NarratorUIManager.Instance.PlayNarration(
        //            "BHASKARA_EPILOGUE_01", "Bhaskara",
        //            "You did well today. Here is your share of the earnings.", 5f);
        //    }
        //    else
        //    {
        //        yield return NarratorUIManager.Instance.PlayNarration(
        //            "Bhaskara", "The market has closed. Here is your pay for the day.", 5f);
        //    }
        //}
        //else if (!paycheckAlreadyApplied || replayFullEpilogue) yield return new WaitForSeconds(6f);

        if (!paycheckAlreadyApplied || replayFullEpilogue)
        {
            yield return new WaitForSeconds(2f);

            if (NarratorUIManager.Instance != null)
            {
                yield return NarratorUIManager.Instance.PlayNarration(
                    "BHASKARA_EPILOGUE_01",
                    "Bhaskara",
                    "You did well today. Here is your share of the earnings.",
                    5f
                );
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        }


        int balance;
        int paycheck;

        while (!saves.TryApplyEpiloguePaycheck(
            defaultPaycheck, bookPrice, out balance, out paycheck))
        {
            yield return WaitForFinanceRetry("Could not save your pay");
        }

        Level1GameState.ExistingInstance?.ReloadProfileFromDisk();
        currentWalletBalance = balance;
        walletHUD?.SetBalance(balance);

        if (paycheck > 0)
        {
            // Actual paycheck; award was performed by LocalSaveManager above.
            walletHUD?.ShowGain(paycheck);
            objectiveUI?.SetObjective(
                "Collect your earnings from Bhaskara");
            //$"Bhaskara's pay: +{paycheck} Varahas\nWallet: {balance} Varahas");
        }

        // In replay mode, show a presentation-only amount when pay was already saved.
        // This NEVER credits money to the wallet again.
        if (paycheck > 0 || replayFullEpilogue)
        {
            int shownPaycheck = paycheck > 0 ? paycheck : defaultPaycheck;
            if (earningsPopup != null)
            {
                earningsPopup.ShowEarnings(shownPaycheck);
                Debug.Log($"[EPILOGUE EARNINGS] +{shownPaycheck} (actual payout={paycheck}; replay={replayFullEpilogue})");
            }
            else
            {
                Debug.LogWarning("[EPILOGUE EARNINGS] Assign Earnings Popup in Inspector.");
            }
        }

        // Normal gameplay resumes at the book if it was previously purchased.
        // Recording mode replays the whole story without modifying an already bought book.
        if (bookPurchasedBeforeThisSequence && !replayFullEpilogue)
        {
            EnableBookPickup(balance);
            yield break;
        }

        if (!paycheckAlreadyApplied || replayFullEpilogue)
        {
            // Allow the coin popup to finish before the narrator starts.
            yield return new WaitForSeconds(3.5f);
            if (NarratorUIManager.Instance != null)
            {
                yield return NarratorUIManager.Instance.PlayNarration(
                    "NARRATOR_EPILOGUE_01", "Narrator",
                    "With enough Varahas now in hand, perhaps it is finally time to return to Meera.", 6f);
            }
            else yield return new WaitForSeconds(5f);
        }

        // ── Advance to ReturnToMeera ─────────────────────────────────────────

        currentStage = EpilogueStage.ReturnToMeera;

        objectiveUI?.SetObjective(
            //$"Purchase the book from Meera ({bookPrice} Varahas)\nWallet: {balance} Varahas"
            "Return to Meera to purchase the book"
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

        // ── Deduct book price only when it has NOT already been purchased ────
        int balance = currentWalletBalance;
        if (replayFullEpilogue && bookPurchasedBeforeThisSequence)
        {
            // Cinematic replay only: do not charge again or change save data.
            Debug.Log("[EPILOGUE] Book was already purchased; replaying dialogue without recharging.");
        }
        else
        {
            var saves = new LocalSaveManager();
            int spent;
            while (!saves.TryPurchaseEpilogueBook(bookPrice, out balance, out spent))
                yield return WaitForFinanceRetry("Could not save the book purchase");
            Level1GameState.ExistingInstance?.ReloadProfileFromDisk();
            currentWalletBalance = balance;
            walletHUD?.SetBalance(balance);
            if (spent > 0) walletHUD?.ShowSpend(spent);
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

        EnableBookPickup(balance);
    }

    private IEnumerator WaitForFinanceRetry(string message)
    {
        objectiveUI?.SetObjective($"{message}. Press A / Enter to retry.");
        yield return null;
        while (PauseMenuController.GameplayInputBlocked ||
            (!OVRInput.GetDown(OVRInput.RawButton.A) && !Input.GetKeyDown(KeyCode.Return)))
            yield return null;
    }

    private void EnableBookPickup(int balance)
    {
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
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.HidePrompt(this);
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
