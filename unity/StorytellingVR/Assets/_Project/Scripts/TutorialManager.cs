using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
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

    void Start()
    {
        

        coinsEarnedText.text = "Coins Earned: 0";
        spokenPriceText.text = "Spoken Price: --";

        respectUIManager.SetRespect(respect);

        StartCoroutine(TutorialSequence());
    }

    IEnumerator TutorialSequence()
    {
        ShowNarrator(
            "Ah... look there.\n\nYour first customer approaches.\n\nLet us see how you handle your very first deal..."
        );

        yield return new WaitForSeconds(5f);

        ShowCustomer(
            "Greetings, merchant.\n\nI am Rahim, a trader from the lands of Persia.\n\nI have travelled far across deserts and seas in search of fine spices.\n\nI would like to buy 1 kilogram of cardamom... at a fair price."
        );

        yield return new WaitForSeconds(8f);

        ShowNarrator(
            "Let us now understand the art of negotiation...\n\nThe base price for cardamom is 50 Varahas.\n\nTry beginning with 200.\n\nOffer too high... and you may lose the deal."
        );

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
        ShowCustomer(
            offer + " Varahas?!\n\nThat is outrageous, merchant...\n\nPlease offer a fair price."
        );

        yield return new WaitForSeconds(5f);

        ShowNarrator(
            "As you can see... greed may cost you the trade.\n\nNow... make a wiser decision.\n\nOffer a fair price while still securing your profit."
        );

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
        int profit = offer - 50;
        coins += offer;

        coinsEarnedText.text = "Coins Earned: " + coins;

        respect += 10;
        respectUIManager.SetRespect(respect);

        ShowCustomer(
            "Hmm... " + offer + " Varahas...\n\nThat seems more reasonable.\n\nI accept your offer."
        );

        yield return new WaitForSeconds(5f);

        ShowNarrator(
            "Balance is the key to trade...\n\nToo high... and you lose the customer.\n\nToo low... and you lose your profit.\n\nChoose wisely."
        );

        tutorialFinished = true;
    }

    IEnumerator TooHighAgainSequence(int offer)
    {
        ShowCustomer(
            offer + " Varahas?\n\nThat is still too expensive.\n\nI may take my business elsewhere."
        );

        yield return new WaitForSeconds(5f);

        ShowNarrator(
            "That price is still too high.\n\nTry offering something closer to 60 or 70 Varahas."
        );

        waitingForFairPrice = true;
    }

    IEnumerator TooLowSequence(int offer)
    {
        ShowCustomer(
            offer + " Varahas?\n\nThat is very generous.\n\nI happily accept."
        );

        yield return new WaitForSeconds(5f);

        ShowNarrator(
            "The customer is pleased...\n\nBut your profit is very low.\n\nTry to find a better balance next time."
        );

        tutorialFinished = true;
    }

    void ShowNarrator(string text)
    {
        speakerNameText.text = "Narrator";
        speakerNameText.color = Color.yellow;

        dialogueText.text = text;
        dialogueText.color = Color.yellow;
    }

    void ShowCustomer(string text)
    {
        speakerNameText.text = "Rahim";
        speakerNameText.color = Color.white;

        dialogueText.text = text;
        dialogueText.color = Color.white;
    }
}