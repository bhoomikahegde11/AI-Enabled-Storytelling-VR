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
        if (teleportManager == null && TeleportManager.Instance != null)
        {
            teleportManager = TeleportManager.Instance;
        }

        if (teleportManager != null)
            teleportManager.DisableAll();

        StartCoroutine(BhaskaraSequence());
    }

    private IEnumerator BhaskaraSequence()
    {
        sequenceRunning = true;
        yield return new WaitForSeconds(1f);

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration("BHASKARA_EPILOGUE_01", "Bhaskara", "You did well today. Here is your share of the earnings.", 5f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        LocalSaveManager saveManager = new LocalSaveManager();
        LocalProfileData profile = saveManager.LoadProfile();
        int balanceBefore = profile.global_metrics.total_varahas;
        
        int paycheck = defaultPaycheck;
        if (balanceBefore + paycheck < bookPrice)
        {
            paycheck = bookPrice - balanceBefore;
        }

        // Authoritative state update
        profile.global_metrics.total_varahas += paycheck;
        saveManager.SaveProfile(profile);

        if (Level1GameState.Instance != null)
        {
            Level1GameState.Instance.ReloadProfileFromDisk();
        }

        if (walletHUD != null)
        {
            walletHUD.SetBalance(profile.global_metrics.total_varahas);
            walletHUD.ShowGain(paycheck);
        }

        yield return new WaitForSeconds(1f);

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration("NARRATOR_EPILOGUE_01", "Narrator", "With enough Varahas now in hand, perhaps it is finally time to return to Meera.", 6f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        currentStage = EpilogueStage.ReturnToMeera;

        if (teleportManager != null)
        {
            teleportManager.EnableGroup("General");
        }
        
        sequenceRunning = false;
    }

    public void NotifyMeeraApproached()
    {
        if (currentStage != EpilogueStage.ReturnToMeera) return;
        currentStage = EpilogueStage.PurchaseBook;
        StartCoroutine(MeeraApproachSequence());
    }

    private IEnumerator MeeraApproachSequence()
    {
        sequenceRunning = true;
        if (teleportManager != null)
            teleportManager.DisableAll();

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration("MEERA_EPILOGUE_01", "Meera", "You came back.", 3f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        if (teleportManager != null)
            teleportManager.EnableGroup("General");
            
        sequenceRunning = false;
    }

    public void NotifyMeeraInteracted()
    {
        if (currentStage != EpilogueStage.PurchaseBook || sequenceRunning) return;
        StartCoroutine(MeeraPurchaseSequence());
    }

    private IEnumerator MeeraPurchaseSequence()
    {
        sequenceRunning = true;
        if (teleportManager != null)
            teleportManager.DisableAll();

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration("PLAYER_EPILOGUE_01", "You", "I'd like to buy that book.", 3f);
            yield return NarratorUIManager.Instance.PlayNarration("MEERA_EPILOGUE_02", "Meera", $"It is yours for {bookPrice} Varahas.", 4f);
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }

        LocalSaveManager saveManager = new LocalSaveManager();
        LocalProfileData profile = saveManager.LoadProfile();

        // Authoritative deduction
        profile.global_metrics.total_varahas -= bookPrice;
        if (profile.global_metrics.total_varahas < 0) profile.global_metrics.total_varahas = 0;
        saveManager.SaveProfile(profile);

        if (Level1GameState.Instance != null)
        {
            Level1GameState.Instance.ReloadProfileFromDisk();
        }

        if (walletHUD != null)
        {
            walletHUD.ShowSpend(bookPrice);
            walletHUD.SetBalance(profile.global_metrics.total_varahas);
        }

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration("MEERA_EPILOGUE_03", "Meera", "Take it. Perhaps it will answer your questions.", 4f);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        currentStage = EpilogueStage.OpenBook;
        
        if (teleportManager != null)
            teleportManager.EnableGroup("General");

        sequenceRunning = false;
    }

    public void NotifyBookInteracted()
    {
        if (currentStage != EpilogueStage.OpenBook) return;
        currentStage = EpilogueStage.Transitioning;
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        if (teleportManager != null)
            teleportManager.DisableAll();

        if (fader == null)
            fader = FindFirstObjectByType<ScreenFader>();

        if (fader != null)
            yield return fader.FadeOut();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneByName(presentDaySceneName);
        }
        else
        {
            SceneManager.LoadScene(presentDaySceneName);
        }

        currentStage = EpilogueStage.Complete;
    }
}
