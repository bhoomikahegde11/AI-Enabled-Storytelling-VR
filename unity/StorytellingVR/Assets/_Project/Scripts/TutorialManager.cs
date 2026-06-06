using System.Collections;
using TMPro;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
   
    [Header("Dialogue UI")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public VoiceRecognitionManager voiceRecognitionManager;

    [Header("Audio Sources")]
    public AudioSource narratorAudioSource;
    public AudioSource customerAudioSource;

    [Header("Dialogue Audio")]
    public AudioClip narratorIntroClip;
    public AudioClip customerIntroClip;
    public AudioClip customerAngryClip;
    public AudioClip narratorGreedClip;
    public AudioClip customerAcceptClip;
    public AudioClip narratorEndingClip;
    public AudioClip customerTooHighClip;
    public AudioClip narratorTryAgainClip;
    public AudioClip customerTooLowClip;
    public AudioClip narratorLowProfitClip;
    [Header("UI")]
    public TMP_Text coinsEarnedText;
    public TMP_Text spokenPriceText;

    [Header("Respect")]
    public RespectUIManager respectUIManager;

    private int respect = 100;
    private int coins = 0;

    private bool waitingForHighPrice = false;
    private bool waitingForFairPrice = false;
    private bool tutorialFinished = false;

    void Start()
    {
        if (coinsEarnedText != null)
            coinsEarnedText.text = "0";
        if (spokenPriceText != null)
            spokenPriceText.text = "Spoken Price: --";

        if (respectUIManager != null)
            respectUIManager.SetRespect(respect);

        StartCoroutine(TutorialSequence());
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
            speakerNameText.color = color;
        }

        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        float clipLength = audioClip != null ? audioClip.length : 5f;
        float timePerLine = clipLength / lines.Length;

        foreach (string line in lines)
        {
            if (dialogueText != null)
            {
                dialogueText.text = line;
                dialogueText.color = color;
            }

            yield return new WaitForSeconds(timePerLine);
        }

        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
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
            speakerNameText.color = color;
        }

        if (dialogueText != null)
            dialogueText.color = color;

        if (audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (audioSource != null)
            {
                yield return new WaitUntil(() =>
                    audioSource.time >= startTimes[i]);
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
            }

            if (dialogueText != null)
                dialogueText.text = lines[i];
        }

        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
        }
    }
    IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(
            ShowDialogueSequenceWithTimings(
                "Rahim:",
                 Color.white,
                 customerAudioSource,
                 customerIntroClip,

                new string[]
                {
                    "Greetings Merchant!",
                    "My name is Rahim.",
                    "I have journeyed here from the Deccan Sultanate to trade in the markets of Vijayanagara.",
                    "I am looking to purchase one veesai of cardamom today, if the price is fair."
                },

                new float[]
                {
                    0.0f,
                    1.8f,
                    3.6f,
                    8.8f
                }
    )
);

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorIntroClip,
            "Now, let us learn the art of negotiation.",
            "The base price of one veesai of cardamom is 18 Varahas.",
            "To your right, you will see the number of Varahas you earn from each successful trade.",
            "Next to it, you will also find your Reputation in the market.",
            "As a trader, you must maintain a good reputation.",
            "Merchants who earn the trust and respect of their customers attract more business and greater opportunities.",
            "Start by offering 70 varahas.",
            "Be careful... a price that is too high may cost you the deal entirely."
        ));

        voiceRecognitionManager.voicePromptText.text = "Say 70";

        voiceRecognitionManager.ListenForPrice();

        waitingForHighPrice = true;
    }

    public void HandlePlayerOffer(int offer)
    {
        if (tutorialFinished)
            return;

        if (spokenPriceText != null)
            spokenPriceText.text = "Spoken Price: " + offer + " Varahas";

        if (waitingForHighPrice)
        {
            HandleHighPriceStage(offer);
            return;
        }

        if (waitingForFairPrice)
        {
            HandleFairPriceStage(offer);
            return;
        }
    }

    void HandleHighPriceStage(int offer)
    {
        if (offer >= 60)
        {
            waitingForHighPrice = false;

            respect -= 40;
            if (respectUIManager != null)
                respectUIManager.SetRespect(respect);

            StartCoroutine(HighPriceReactionSequence(offer));
        }
        else
        {
            //
            ShowNarrator(
                "Try offering a very high price like 70 Varahas so you can see the customer's reaction."
            );
            voiceRecognitionManager.ListenForPrice();
            waitingForHighPrice = true;
        }
    }
    IEnumerator HighPriceReactionSequence(int offer)
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            customerAudioSource,
            customerAngryClip,
            "That price is outrageous, merchant.",
            "Surely you can offer something more reasonable."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorGreedClip,
            "As you can see, greed can quickly drive customers away.",
            "Also, notice how your Reputation has fallen.",
            "Word travels quickly through the bustling markets of Vijayanagara.",
            "If traders believe you are unfair, fewer customers may choose to do business with you.",
            "Now, make a wiser decision.",
            "Try offering a fair price that earns a profit while keeping the customer satisfied."
        ));

        voiceRecognitionManager.voicePromptText.text =
    "Offer a fair price";

        voiceRecognitionManager.ListenForPrice();

        waitingForFairPrice = true;
    }

    void HandleFairPriceStage(int offer)
    {
        waitingForFairPrice = false;

        if (offer >= 22 && offer <= 30)
        {
            StartCoroutine(FairPriceSequence(offer));
        }
        else if (offer > 30)
        {
            respect -= 20;
            if (respectUIManager != null)
                respectUIManager.SetRespect(respect);

            StartCoroutine(TooHighAgainSequence(offer));
        }
        else if (offer < 18)
        {
            coins += offer;
            if (coinsEarnedText != null)
                coinsEarnedText.text = coins.ToString();

            StartCoroutine(TooLowSequence(offer));
        }
        else
        {
            StartCoroutine(FairPriceSequence(offer));
        }
    }

    IEnumerator FairPriceSequence(int offer)
    {
        coins += offer;
        if (coinsEarnedText != null)
            coinsEarnedText.text = coins.ToString();

        respect += 20;
        if (respectUIManager != null)
            respectUIManager.SetRespect(respect);

        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            customerAudioSource,
            customerAcceptClip,
           "Hmm... that seems much more reasonable.",
            "Very well. I accept your offer."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorEndingClip,
            "Balance is the foundation of successful trade.",
            "Your Reputation has improved, and your earned Varahas have increased.",
            "Ask too much, and you risk losing the customer.",
            "Ask too little, and you sacrifice your profit.",
            "The most successful merchants of Vijayanagara knew how to build both wealth and trust.",
            "A skilled merchant learns to find the right balance."
        ));

        FinishTutorial();
    }

    IEnumerator TooHighAgainSequence(int offer)
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            customerAudioSource,
            customerTooHighClip,
            "That price is still too expensive.",
            "At those rates, I may take my business elsewhere."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorTryAgainClip,
            "That offer is still too expensive for the customer.",
            "Notice how your Reputation continues to decline.",
            "Even the wealthiest traders of Vijayanagara could not prosper without the trust of their customers.",
            "Try proposing a price closer to 25 Varahas."
        ));

        voiceRecognitionManager.ListenForPrice();
        waitingForFairPrice = true;
    }

    IEnumerator TooLowSequence(int offer)
    {
        coins += offer;
        if (coinsEarnedText != null)
            coinsEarnedText.text = coins.ToString();
        respect += 30;
        if (respectUIManager != null)
            respectUIManager.SetRespect(respect);


        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            customerAudioSource,
            customerTooLowClip,
            "That is a very generous offer.",
            "I happily accept your price."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorLowProfitClip,
                "The customer is certainly pleased.",
                "However, your earnings from this trade are quite low.",
                "A merchant who consistently sells below value may gain customers, but will struggle to build wealth.",
                "To thrive in the markets of Vijayanagara, you must balance customer satisfaction with sustainable profit."
        ));

        FinishTutorial();
    }
    void FinishTutorial()
    {
        tutorialFinished = true;

        Debug.Log(
            "TRANSACTION TUTORIAL FINISHED"
        );

        // Load the Level 1 Marketplace scene directly by name as per safety requirements
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene1");
    }
    public void ShowNarrator(string text)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = "Narrator:";
            speakerNameText.color = Color.yellow;
        }

        if (dialogueText != null)
        {
            dialogueText.text = text;
            dialogueText.color = Color.yellow;
        }
    }

    void ShowCustomer(string text)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = "Rahim:";
            speakerNameText.color = Color.white;
        }

        if (dialogueText != null)
        {
            dialogueText.text = text;
            dialogueText.color = Color.white;
        }
    }
}