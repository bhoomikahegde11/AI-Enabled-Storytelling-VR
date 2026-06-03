using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    IEnumerator LoadNextScene() 
    { yield return new WaitForSeconds(2f); SceneManager.LoadScene(1); }
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
    public AudioClip narratorNegotiationClip;
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
    private bool waitingForNextLine = false;

    void Start()
    {
        

        coinsEarnedText.text = "Coins Earned: 0";
        spokenPriceText.text = "Spoken Price: --";

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
        speakerNameText.text = speaker;
        speakerNameText.color = color;

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        float clipLength = audioClip != null ? audioClip.length : 5f;
        float timePerLine = clipLength / lines.Length;

        foreach (string line in lines)
        {
            dialogueText.text = line;
            dialogueText.color = color;

            yield return new WaitForSeconds(timePerLine);
        }

        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
        }
    }
    IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            customerAudioSource,
            customerIntroClip,
            "Greetings, merchant.",
            "My name is Rahim.",
            "I have journeyed here from the Deccan Sultanate to trade in the markets of Vijayanagara.",
            "I am looking to purchase some cardamom today, if the price is fair."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            narratorAudioSource,
            narratorNegotiationClip,
            "Now, let us learn the art of negotiation.",
            "The base price of one kilogram of cardamom is 50 Varahas.",
            "To your right, you will see the number of Varahas you earn from each successful trade.",
            "Above it, you will also find your Reputation.",
            "As a trader, you must maintain a good reputation.",
            "Merchants who earn the trust and respect of their customers attract more business and greater opportunities.",
            "Start by offering 200 varahas.",
            "Be careful... a price that is too high may cost you the deal entirely."
        ));

        voiceRecognitionManager.ListenForPrice();
        waitingForHighPrice = true;
    }

    public void HandlePlayerOffer(int offer)
    {
        if (tutorialFinished)
            return;

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
        if (offer >= 120)
        {
            waitingForHighPrice = false;

            respect -= 40;
            respectUIManager.SetRespect(respect);

            StartCoroutine(HighPriceReactionSequence(offer));
        }
        else
        {
            ShowNarrator(
                "Try offering a very high price like 200 Varahas so you can see the customer's reaction."
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

        voiceRecognitionManager.ListenForPrice();
        waitingForFairPrice = true;
    }

    void HandleFairPriceStage(int offer)
    {
        waitingForFairPrice = false;

        if (offer >= 60 && offer <= 80)
        {
            StartCoroutine(FairPriceSequence(offer));
        }
        else if (offer > 80)
        {
            respect -= 20;
            respectUIManager.SetRespect(respect);

            StartCoroutine(TooHighAgainSequence(offer));
        }
        else if (offer < 50)
        {
            coins += offer;
            coinsEarnedText.text = "Coins Earned: " + coins;

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
        coinsEarnedText.text = "Coins Earned: " + coins;

        respect += 10;
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

        tutorialFinished = true;
        StartCoroutine(LoadNextScene());
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
            "Try proposing a price closer to 60 or 70 Varahas."
        ));

        voiceRecognitionManager.ListenForPrice();
        waitingForFairPrice = true;
    }

    IEnumerator TooLowSequence(int offer)
    {
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

        tutorialFinished = true;
        StartCoroutine(LoadNextScene());
    }

    public void ShowNarrator(string text)
    {
        speakerNameText.text = "Narrator:";
        speakerNameText.color = Color.yellow;

        dialogueText.text = text;
        dialogueText.color = Color.yellow;
    }

    void ShowCustomer(string text)
    {
        speakerNameText.text = "Rahim:";
        speakerNameText.color = Color.white;

        dialogueText.text = text;
        dialogueText.color = Color.white;
    }
}