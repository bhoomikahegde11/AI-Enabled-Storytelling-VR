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
    IEnumerator ShowDialogueSequence(string speaker, Color color, params string[] lines)
    {
        speakerNameText.text = speaker;
        speakerNameText.color = color;

        foreach (string line in lines)
        {
            dialogueText.text = line;
            dialogueText.color = color;

            waitingForNextLine = true;

            while (waitingForNextLine == true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitingForNextLine = false;
                }

                yield return null;
            }

            yield return null;
        }
    }
    IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "Let us see how you handle your very first deal..."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            "Greetings, merchant.",
            "I am Rahim, a trader from the lands of Persia.",
            "I have travelled far across deserts and seas in search of fine spices.",
            "I would like to buy 1 kilogram of cardamom... at a fair price."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "Let us now understand the art of negotiation...",
            "The base price for cardamom is 50 Varahas.",
            "Try beginning with 200.",
            "Offer too high... and you may lose the deal."
        ));

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
        }
    }

    IEnumerator HighPriceReactionSequence(int offer)
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            offer + " Varahas?!",
            "That is outrageous, merchant...",
            "Please offer a fair price."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "As you can see... greed may cost you the trade.",
            "Now... make a wiser decision.",
            "Offer a fair price while still securing your profit."
        ));

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
            "Hmm... " + offer + " Varahas...",
            "That seems more reasonable.",
            "I accept your offer."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "Balance is the key to trade...",
            "Too high... and you lose the customer.",
            "Too low... and you lose your profit.",
            "Choose wisely."
        ));

        tutorialFinished = true;
        StartCoroutine(LoadNextScene());
    }

    IEnumerator TooHighAgainSequence(int offer)
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            offer + " Varahas?",
            "That is still too expensive.",
            "I may take my business elsewhere."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "That price is still too high.",
            "Try offering something closer to 60 or 70 Varahas."
        ));

        waitingForFairPrice = true;
    }

    IEnumerator TooLowSequence(int offer)
    {
        yield return StartCoroutine(ShowDialogueSequence(
            "Rahim:",
            Color.white,
            offer + " Varahas?",
            "That is very generous.",
            "I happily accept."
        ));

        yield return StartCoroutine(ShowDialogueSequence(
            "Narrator:",
            Color.yellow,
            "The customer is pleased...",
            "But your profit is very low.",
            "Try to find a better balance next time."
        ));

        tutorialFinished = true;
        StartCoroutine(LoadNextScene());
    }

    void ShowNarrator(string text)
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