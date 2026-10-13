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
    public string presentDaySceneName = "PresentDayEnding";
    public int bookPrice = 10;
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
            // Separate clip 1
            yield return NarratorUIManager.Instance.PlayNarration(
                "BHASKARA_EPILOGUE_01",
                "Bhaskara",
                "You handled yourself well today.",
                3f
            );

            // Separate clip 2
            yield return NarratorUIManager.Instance.PlayNarration(
                "BHASKARA_EPILOGUE_02",
                "Bhaskara",
                "Here—your share. You've earned it.",
                3f
            );
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
                "With your earnings in hand, there's one thing left to do. Return to Meera and buy the book.",
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
                "You came back. I had a feeling you would.",
                4f
            );
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        if (teleportManager != null)
            teleportManager.EnableGroup("General");

        // Purchase input is now allowed.
        meeraTrigger?.EnablePurchaseInput();

        sequenceRunning = false;
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

        /*
         * No prerecorded player line.
         *
         * Pressing X already communicates the player's decision
         * to purchase the book.
         */

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                "MEERA_EPILOGUE_02",
                "Meera",
                $"Ten Varahas, as promised. The book is yours.",
                4f
            );
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
                "Take it. Perhaps now you'll understand why it found you.",
                5f
            );
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        // ── Advance to OpenBook ───────────────────────────────────────────────

        currentStage =
            EpilogueStage.OpenBook;

        objectiveUI?.SetObjective(
            "Take the book"
        );

        endingBookInteraction?.Enable();

        if (teleportManager != null)
            teleportManager.EnableGroup("General");

        sequenceRunning = false;
    }

    // ─── BOOK INTERACTED ──────────────────────────────────────────────────────

    public void NotifyBookInteracted()
    {
        if (currentStage != EpilogueStage.OpenBook)
            return;

        currentStage =
            EpilogueStage.Transitioning;

        objectiveUI?.HideObjective();

        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        if (teleportManager != null)
            teleportManager.DisableAll();

        if (fader == null)
            fader =
                FindFirstObjectByType<ScreenFader>();

        if (fader != null)
        {
            // Existing epilogue transition kept unchanged here.
            fader.fadeColor = Color.white;

            yield return fader.FadeOut();
        }

        currentStage =
            EpilogueStage.Complete;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneByName(
                presentDaySceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "[EPILOGUE] GameManager not found — " +
                "scene will not transition until it is added " +
                "to the scene list."
            );
        }
    }
}