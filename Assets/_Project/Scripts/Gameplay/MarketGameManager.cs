using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketGameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerPriceText;
    public Slider priceSlider;

    [Header("AI Systems")]
    public RAGRetriever rag;
    public OllamaClient ollama;

    CustomerState currentCustomer;

    string currentItem = "pepper";
    int quantity = 5;

    int negotiationRounds = 0;

    void Start()
    {
        UpdatePriceText();
        StartCustomer();
    }

    // ----------------------------------
    // CUSTOMER ARRIVES
    // ----------------------------------

    void StartCustomer()
    {
        currentCustomer = new CustomerState();

        currentCustomer.name = "Raghav Shetty";
        currentCustomer.item = currentItem;
        currentCustomer.quantity = quantity;

        currentCustomer.truePrice = Random.Range(22, 28);
        currentCustomer.lastOffer = (int)currentCustomer.truePrice - 4;

        currentCustomer.patience = 4;

        negotiationRounds = 0;

        dialogueText.text =
        "Greetings merchant. I am " + currentCustomer.name +
        ". I seek " + quantity + " kg of " + currentItem +
        ". What price do you ask?";
    }

    // ----------------------------------
    // SLIDER TEXT UPDATE
    // ----------------------------------

    public void UpdatePriceText()
    {
        int price = (int)priceSlider.value;

        playerPriceText.text =
        "Your Offer: " + price + " gold varahas";
    }

    // ----------------------------------
    // PLAYER SUBMITS OFFER
    // ----------------------------------

    public void SubmitOffer()
    {
        int playerPrice = (int)priceSlider.value;

        negotiationRounds++;

        string decision = DecideOutcome(playerPrice);

        if (decision == "accept")
        {
            dialogueText.text =
            "Very well merchant. The price is fair. We have a deal.";

            Invoke("StartCustomer", 4f);
            return;
        }

        if (decision == "leave")
        {
            dialogueText.text =
            "You drive a hard bargain merchant. I will seek another stall.";

            Invoke("StartCustomer", 4f);
            return;
        }

        int counter = GenerateCounterOffer(playerPrice);

        if (counter >= playerPrice)
        {
            dialogueText.text =
            "Very well merchant. Your price is acceptable.";

            Invoke("StartCustomer", 4f);
            return;
        }

        string context = rag.RetrieveContext(currentItem);

        string prompt =
        PromptBuilder.BuildDialoguePrompt(
            context,
            currentCustomer,
            playerPrice,
            counter
        );

        ollama.Generate(prompt, HandleDialogueResponse);
    }

    // ----------------------------------
    // DECISION LOGIC (GAME CONTROLLED)
    // ----------------------------------

    string DecideOutcome(int playerPrice)
    {
        float truePrice = currentCustomer.truePrice;

        if (playerPrice <= truePrice * 1.1f)
        {
            return "accept";
        }

        if (playerPrice > truePrice * 1.6f)
        {
            currentCustomer.patience--;
        }

        if (currentCustomer.patience <= 0)
        {
            return "leave";
        }

        if (negotiationRounds > 6)
        {
            return "leave";
        }

        return "counter";
    }

    // ----------------------------------
    // COUNTER OFFER GENERATION
    // ----------------------------------

    int GenerateCounterOffer(int playerPrice)
    {
        int truePrice = (int)currentCustomer.truePrice;

        int counter = (playerPrice + truePrice) / 2;

        if (counter <= currentCustomer.lastOffer)
        {
            counter = currentCustomer.lastOffer + 1;
        }

        currentCustomer.lastOffer = counter;

        return counter;
    }

    // ----------------------------------
    // HANDLE LLM DIALOGUE
    // ----------------------------------

    void HandleDialogueResponse(string rawJson)
    {
        try
        {
            OllamaOuterResponse outer =
                JsonUtility.FromJson<OllamaOuterResponse>(rawJson);

            dialogueText.text = outer.response;
        }
        catch
        {
            dialogueText.text =
            "The trader strokes his beard and considers the offer.";
        }
    }
}