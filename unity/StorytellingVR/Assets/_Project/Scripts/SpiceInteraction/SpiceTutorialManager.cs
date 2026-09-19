using System.Collections;
using TMPro;
using UnityEngine;

public class SpiceTutorialManager : MonoBehaviour
{
    private const string CustomerRequestLineId = "CUSTOMER_SPICE_INTERACTION_REQUEST_01";
    private const string NarratorScooperAppearedLineId = "NARRATOR_SPICE_INTERACTION_SCOOPER_01";
    private const string NarratorScoopedLineId = "NARRATOR_SPICE_INTERACTION_CARRY_01";
    private const string NarratorWrongSpiceLineId = "NARRATOR_SPICE_INTERACTION_WRONG_SPICE_01";
    private const string NarratorWrongBagLineId = "NARRATOR_SPICE_INTERACTION_WRONG_BAG_01";
    private const string CustomerThanksLineId = "CUSTOMER_SPICE_INTERACTION_THANKS_01";
    private const string NarratorCompletedLineId = "NARRATOR_SPICE_INTERACTION_COMPLETE_01";

    public static SpiceTutorialManager Instance;

    [Header("Dialogue UI")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Level1HUDManager hudManager;

    [Header("Audio Sources")]
    public AudioSource narratorAudioSource;
    public AudioSource customerAudioSource;

    [Header("Dialogue Audio")]
    [SerializeField] private DialogueVoiceDatabase voiceDatabase;

    public AudioClip customerRequestClip;
    public AudioClip narratorScooperAppearedClip;
    public AudioClip narratorScoopedClip;
    public AudioClip narratorWrongSpiceClip;
    public AudioClip narratorWrongBagClip;
    public AudioClip customerThanksClip;
    public AudioClip narratorCompletedClip;

    [Header("UI Prompt")]
    public GameObject promptPanel;
    public TMP_Text promptText;
    public InstructionPromptManager instructionPromptManager;

    [Header("Tutorial")]
    public string customerName = "Rahim";
    public SpiceType requestedSpice = SpiceType.Cardamom;

    [Header("Timing")]
    public float subtitleSecondsPerLine = 3.25f;
    public float subtitlePostHoldSeconds = 0.35f;
    public float timedFallbackLineSeconds = 2.0f;
    public float completionPromptHoldSeconds = 1.25f;
    public float wrongActionReminderCooldown = 1.5f;

    private Coroutine dialogueCoroutine;
    private bool tutorialStarted = false;
    private bool scooperAppeared = false;
    private bool scooperFilled = false;
    private bool bagFilled = false;
    private float nextWrongSpiceReminderTime;
    private float nextWrongBagReminderTime;
    private AudioSource fallbackAudioSource;

    private bool IsTutorialActive
    {
        get
        {
            return OrderManager.Instance != null &&
                OrderManager.Instance.tutorialMode;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!IsTutorialActive)
        {
            HidePrompt();
            return;
        }

        requestedSpice = OrderManager.Instance.requestedSpice;
        StartTutorial();
    }

    public void StartTutorial()
    {
        if (!IsTutorialActive || tutorialStarted)
            return;

        tutorialStarted = true;
        SetPrompt("Hold Right Trigger to pick up the scooper.");

        StartDialogueSequence(OpeningSequence());
    }

    IEnumerator OpeningSequence()
    {
        yield return StartCoroutine(ShowDialogueSequence(
            customerName,
            Color.white,
            customerAudioSource,
            customerRequestClip,
            CustomerRequestLineId,
            "Could you fill one bag of " + GetRequestedSpiceName() + " for me?"
        ));

        
    }

    public void NotifyCustomerHandedBag()
    {
        if (!IsTutorialActive)
            return;

        SetPrompt("Hold Right Trigger to pick up the scooper.");
    }

    public void NotifyScooperAppeared()
    {
        if (!IsTutorialActive || scooperAppeared)
            return;

        scooperAppeared = true;
        SetPrompt("Move the scooper into the highlighted sack.");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator",
            Color.yellow,
            narratorAudioSource,
            narratorScooperAppearedClip,
            NarratorScooperAppearedLineId,
            "Good. Move the scooper into the sack of the requested spice."
        ));
    }

    public void NotifyScooperEnteredSack(SpiceType spice)
    {
        if (!IsTutorialActive || scooperFilled)
            return;

        if (spice == GetRequestedSpice())
            SetPrompt("Keep holding the trigger to scoop.");
        else
            SetPrompt("Collect " + GetRequestedSpiceName() + ".");
    }

    public void NotifyScooperFilled(SpiceType spice)
    {
        if (!IsTutorialActive)
            return;

        if (spice != GetRequestedSpice())
        {
            NotifyWrongSpiceCollected();
            return;
        }

        if (scooperFilled)
            return;

        scooperFilled = true;
        SetPrompt("Bring the filled scooper to the customer's bag.");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator",
            Color.yellow,
            narratorAudioSource,
            narratorScoopedClip,
            NarratorScoopedLineId,
            "Excellent. Now carry the spice to the customer's bag."
        ));
    }

    public void NotifyWrongSpiceCollected()
    {
        if (!IsTutorialActive)
            return;

        if (Time.time < nextWrongSpiceReminderTime)
            return;

        nextWrongSpiceReminderTime = Time.time + wrongActionReminderCooldown;
        SetPrompt("Collect " + GetRequestedSpiceName() + ".");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator",
            Color.yellow,
            narratorAudioSource,
            narratorWrongSpiceClip,
            NarratorWrongSpiceLineId,
            "That is not the spice the customer requested. Try Again."
        ));
    }

    public void NotifyWrongSpiceBroughtToBag()
    {
        if (!IsTutorialActive)
            return;

        if (Time.time < nextWrongBagReminderTime)
            return;

        nextWrongBagReminderTime = Time.time + wrongActionReminderCooldown;
        scooperFilled = false;
        SetPrompt("Collect " + GetRequestedSpiceName() + ".");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator",
            Color.yellow,
            narratorAudioSource,
            narratorWrongBagClip,
            NarratorWrongBagLineId,
            "Collect the correct spice before filling the customer's bag."
        ));
    }

    public void NotifyCorrectBagFilled()
    {
        if (!IsTutorialActive || bagFilled)
            return;

        bagFilled = true;
        EnsureCanRunCoroutines();
        StartCoroutine(HidePromptAfterDelay(completionPromptHoldSeconds));
        StartDialogueSequence(CompletionSequence());
    }

    IEnumerator CompletionSequence()
    {
        yield return StartCoroutine(ShowDialogueSequence(
            customerName,
            Color.white,
            customerAudioSource,
            customerThanksClip,
            CustomerThanksLineId,
            "Thank you, Merchant."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator",
            Color.yellow,
            narratorAudioSource,
            narratorCompletedClip,
            NarratorCompletedLineId,
            "Well done. You have successfully completed your first spice order."

        ));
    }

    IEnumerator ShowDialogueSequence(
        string speaker,
        Color color,
        AudioSource audioSource,
        AudioClip audioClip,
        string lineId,
        params string[] lines)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = speaker;
        }

        AudioClip resolvedClip = ResolveVoiceClip(lineId, audioClip);
        AudioSource playbackSource = GetPlaybackSource(audioSource, resolvedClip);
        bool usingAudio = resolvedClip != null && playbackSource != null;

        if (usingAudio)
        {
            playbackSource.clip = resolvedClip;
            playbackSource.Play();
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(speaker, lines.Length > 0 ? lines[0] : "");
        }

        float clipLength = usingAudio ? resolvedClip.length : subtitleSecondsPerLine;
        float timePerLine = lines.Length > 0 ? clipLength / lines.Length : clipLength;
        if (!usingAudio)
            timePerLine = Mathf.Max(timePerLine, subtitleSecondsPerLine);

        foreach (string line in lines)
        {
            if (dialogueText != null)
            {
                dialogueText.text = line;
            }

            if (hudManager != null)
            {
                hudManager.ShowSubtitle(speaker, line);
            }

            yield return new WaitForSeconds(timePerLine);
        }

        while (playbackSource != null && playbackSource.isPlaying)
        {
            yield return null;
        }

        if (!usingAudio && subtitlePostHoldSeconds > 0f)
        {
            yield return new WaitForSeconds(subtitlePostHoldSeconds);
        }

        if (hudManager != null)
        {
            hudManager.HideSubtitle();
        }
    }

        IEnumerator ShowDialogueSequenceWithTimings(
        string speaker,
        Color color,
        AudioSource audioSource,
        AudioClip audioClip,
        string[] lines,
        float[] startTimes)
    {
        yield return ShowDialogueSequence(speaker, color, audioSource, audioClip, null, lines);
    }

    public void ShowNarrator(string text)
    {
        if (!IsTutorialActive)
            return;

        if (speakerNameText != null)
        {
            speakerNameText.text = "Narrator";
        }

        if (dialogueText != null)
        {
            dialogueText.text = text;
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle("Narrator", text);
        }
    }

    void ShowCustomer(string text)
    {
        if (!IsTutorialActive)
            return;

        if (speakerNameText != null)
        {
            speakerNameText.text = customerName;
        }

        if (dialogueText != null)
        {
            dialogueText.text = text;
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(customerName, text);
        }
    }

    void SetPrompt(string message)
    {
        if (instructionPromptManager != null)
        {
            instructionPromptManager.ShowTrigger(message);
            return;
        }

        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = message;
    }

    void HidePrompt()
    {
        if (instructionPromptManager != null)
        {
            instructionPromptManager.Hide();
            return;
        }

        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (promptText != null)
            promptText.text = "";
    }

    IEnumerator HidePromptAfterDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        HidePrompt();
    }

    void StartDialogueSequence(IEnumerator sequence)
    {
        EnsureCanRunCoroutines();

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(RunDialogueSequence(sequence));
    }

    void EnsureCanRunCoroutines()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!enabled)
        {
            enabled = true;
        }
    }

    IEnumerator RunDialogueSequence(IEnumerator sequence)
    {
        yield return StartCoroutine(sequence);
        dialogueCoroutine = null;
    }

    SpiceType GetRequestedSpice()
    {
        if (OrderManager.Instance != null)
            return OrderManager.Instance.requestedSpice;

        return requestedSpice;
    }

    string GetRequestedSpiceName()
    {
        return GetRequestedSpice().ToString();
    }

    private AudioClip ResolveVoiceClip(string lineId, AudioClip fallbackClip)
    {
        if (voiceDatabase == null)
            return fallbackClip;

        AudioClip databaseClip = voiceDatabase.GetAudioClip(lineId);
        return databaseClip != null ? databaseClip : fallbackClip;
    }

    private AudioSource GetPlaybackSource(AudioSource preferredSource, AudioClip clip)
    {
        if (preferredSource != null || clip == null)
            return preferredSource;

        if (fallbackAudioSource == null)
        {
            fallbackAudioSource = gameObject.AddComponent<AudioSource>();
            fallbackAudioSource.playOnAwake = false;
        }

        return fallbackAudioSource;
    }
}
