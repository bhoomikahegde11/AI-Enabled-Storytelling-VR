using System.Collections;
using TMPro;
using UnityEngine;

public class SpiceTutorialManager : MonoBehaviour
{
    public static SpiceTutorialManager Instance;

    [Header("Dialogue UI")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public Level1HUDManager hudManager;

    [Header("Audio Sources")]
    public AudioSource customerAudioSource;
    public AudioSource merchantAudioSource;

    [Header("Dialogue Audio")]
    public AudioClip customerRequestClip;
    public AudioClip customerThanksClip;
    public AudioClip merchantExplainOrderClip;
    public AudioClip merchantMoveToSackClip;
    public AudioClip merchantCarryBagClip;
    public AudioClip merchantWrongSpiceClip;
    public AudioClip merchantWrongBagClip;
    public AudioClip merchantCompletedClip;

    [Header("Tutorial")]
    public string customerName = "Customer";
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
            PromptManager.Instance.HidePrompt();
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

        StartDialogueSequence(OpeningSequence());
    }

    IEnumerator OpeningSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                customerName + ":",
                Color.white,
                customerAudioSource,
                customerRequestClip,
                "Could you fill one bag of " + GetRequestedSpiceName() + " for me?"
            )
        );

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantExplainOrderClip,
                "The customer wants a bag of cardamom.",
                "Make sure you collect the spice they asked for."
            )
        );

        PromptManager.Instance.ShowPrompt(
            "Hold the right trigger to pick up the scooper.",
            PromptManager.Instance.rightTriggerButton
        );
    }

    public void NotifyCustomerHandedBag()
    {
        if (!IsTutorialActive)
            return;


        PromptManager.Instance.ShowPrompt(
            "Hold the right trigger to pick up the scooper.",
            PromptManager.Instance.rightTriggerButton
        );
    }

    public void NotifyScooperAppeared()
    {
        if (!IsTutorialActive || scooperAppeared)
            return;

        scooperAppeared = true;

        PromptManager.Instance.HidePrompt();

        StartDialogueSequence(
            ScooperAppearedSequence()
        );
    }
    IEnumerator ScooperAppearedSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantMoveToSackClip,
                "Good. Take the scooper to the cardamom sack."
            )
        );

    }

    public void NotifyScooperEnteredSack(SpiceType spice)
    {
        if (!IsTutorialActive || scooperFilled)
            return;

        if (spice == GetRequestedSpice())
        {
            PromptManager.Instance.ShowPrompt(
                "Keep holding the trigger to scoop.",
                PromptManager.Instance.rightTriggerButton
            );
        }
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

        PromptManager.Instance.HidePrompt();

        StartDialogueSequence(
            ScooperFilledSequence()
        );
    }
    IEnumerator ScooperFilledSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantCarryBagClip,
                "Excellent. Now carry the cardamom to the customer's bag."
            )
        );

        PromptManager.Instance.ShowPrompt(
            "Bring the filled scooper to the customer's bag.",
            PromptManager.Instance.rightTriggerButton
        );
    }

    public void NotifyWrongSpiceCollected()
    {
        if (!IsTutorialActive)
            return;

        if (Time.time < nextWrongSpiceReminderTime)
            return;

        nextWrongSpiceReminderTime = Time.time + wrongActionReminderCooldown;

        PromptManager.Instance.HidePrompt();

        StartDialogueSequence(
            WrongSpiceSequence()
        );
    }
    IEnumerator WrongSpiceSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantWrongSpiceClip,
                "That's the wrong spice.",
                "The customer asked for cardamom. Try again."
            )
        );

    }
    public void NotifyWrongSpiceBroughtToBag()
    {
        if (!IsTutorialActive)
            return;

        if (Time.time < nextWrongBagReminderTime)
            return;

        nextWrongBagReminderTime = Time.time + wrongActionReminderCooldown;

        scooperFilled = false;

        PromptManager.Instance.HidePrompt();

        StartDialogueSequence(
            WrongBagSequence()
        );
    }
    IEnumerator WrongBagSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantWrongBagClip,
                "You need the correct spice before filling the customer's bag.",
                "Collect the cardamom and try again."
            )
        );

    }

    public void NotifyCorrectBagFilled()
    {
        if (!IsTutorialActive || bagFilled)
            return;

        bagFilled = true;
        EnsureCanRunCoroutines();
        //StartCoroutine(HidePromptAfterDelay(completionPromptHoldSeconds));
        StartDialogueSequence(CompletionSequence());
    }

    IEnumerator CompletionSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequence(
                customerName + ":",
                Color.white,
                customerAudioSource,
                customerThanksClip,
                "Thank you, merchant."
            )
        );

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Bhaskara:",
                Color.yellow,
                merchantAudioSource,
                merchantCompletedClip,
                "Well done.",
                "You've successfully completed your first spice order."
            )
        );

        PromptManager.Instance.HidePrompt();
    }

    IEnumerator ShowDialogueSequence(
        string speaker,
        Color color,
        AudioSource audioSource,
        AudioClip audioClip,
        params string[] lines)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = speaker;
        }

        bool usingAudio = audioClip != null && audioSource != null;

        if (usingAudio)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(speaker, lines.Length > 0 ? lines[0] : "");
        }

        float clipLength = usingAudio ? audioClip.length : subtitleSecondsPerLine;
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

        while (audioSource != null && audioSource.isPlaying)
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
        if (speakerNameText != null)
        {
            speakerNameText.text = speaker;
        }

        bool usingAudio = audioClip != null && audioSource != null;

        if (usingAudio)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(speaker, lines.Length > 0 ? lines[0] : "");
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (usingAudio)
            {
                yield return new WaitUntil(() => audioSource.time >= startTimes[i]);
            }
            else
            {
                yield return new WaitForSeconds(timedFallbackLineSeconds);
            }

            if (dialogueText != null)
                dialogueText.text = lines[i];

            if (hudManager != null)
                hudManager.ShowSubtitle(speaker, lines[i]);
        }

        while (audioSource != null && audioSource.isPlaying)
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

    public void ShowNarrator(string text)
    {
        if (!IsTutorialActive)
            return;

        if (speakerNameText != null)
        {
            speakerNameText.text = "Narrator:";
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
            speakerNameText.text = customerName + ":";
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
}
