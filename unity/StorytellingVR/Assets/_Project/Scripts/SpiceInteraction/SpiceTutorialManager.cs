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
    public AudioSource narratorAudioSource;
    public AudioSource customerAudioSource;

    [Header("Dialogue Audio")]
    public AudioClip customerRequestClip;
    public AudioClip narratorIntroClip;
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
    public string customerName = "Customer";
    public SpiceType requestedSpice = SpiceType.Cardamom;

    private Coroutine dialogueCoroutine;
    private bool tutorialStarted = false;
    private bool scooperAppeared = false;
    private bool scooperFilled = false;
    private bool bagFilled = false;

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
            customerName + ":",
            Color.white,
            customerAudioSource,
            customerRequestClip,
            "Could you fill one bag of " + GetRequestedSpiceName() + " for me?"
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorIntroClip,
            "Let us learn how spices are packed in the markets of Vijayanagara."
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
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorScooperAppearedClip,
            "Good. Move the scooper into the highlighted sack."
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
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorScoopedClip,
            "Excellent. Now carry the spice to the customer's bag."
        ));
    }

    public void NotifyWrongSpiceCollected()
    {
        if (!IsTutorialActive)
            return;

        SetPrompt("Collect " + GetRequestedSpiceName() + ".");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorWrongSpiceClip,
            "That is not the spice the customer requested."
        ));
    }

    public void NotifyWrongSpiceBroughtToBag()
    {
        if (!IsTutorialActive)
            return;

        scooperFilled = false;
        SetPrompt("Collect " + GetRequestedSpiceName() + ".");

        StartDialogueSequence(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorWrongBagClip,
            "Collect the correct spice before filling the customer's bag."
        ));
    }

    public void NotifyCorrectBagFilled()
    {
        if (!IsTutorialActive || bagFilled)
            return;

        bagFilled = true;
        HidePrompt();

        StartDialogueSequence(CompletionSequence());
    }

    IEnumerator CompletionSequence()
    {
        yield return StartCoroutine(ShowDialogueSequence(
            customerName + ":",
            Color.white,
            customerAudioSource,
            customerThanksClip,
            "Thank you, Merchant."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorCompletedClip,
            "Well done. You have successfully completed your first spice order."
        ));
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

        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(speaker, lines.Length > 0 ? lines[0] : "");
        }

        float clipLength = audioClip != null ? audioClip.length : 5f;
        float timePerLine = lines.Length > 0 ? clipLength / lines.Length : clipLength;

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

        if (audioClip != null && audioSource != null)
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
            if (audioSource != null)
            {
                yield return new WaitUntil(() => audioSource.time >= startTimes[i]);
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
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

    void StartDialogueSequence(IEnumerator sequence)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!enabled)
        {
            enabled = true;
        }

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(RunDialogueSequence(sequence));
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
