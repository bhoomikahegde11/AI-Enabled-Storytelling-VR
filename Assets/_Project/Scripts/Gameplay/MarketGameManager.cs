using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;

public class MarketGameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerPriceText;
    public Slider priceSlider;

    [Header("AI Systems")]
    public RAGRetriever rag;
    public OllamaClient ollama;

    [Header("Session / UI Helpers")]
    public PlayerSessionData sessionData;
    public CostBoardUI costBoard;

    CustomerState currentCustomer;

    int negotiationRounds = 0;
    float customerStartTime = 0f;

    void Start()
    {
        UpdatePriceText();

        // Populate cost board if available
        if (rag != null && costBoard != null)
        {
            costBoard.Populate(rag.goods);
        }

        StartCustomer();
    }

    // ----------------------------------
    // CUSTOMER ARRIVES
    // ----------------------------------

    void StartCustomer()
    {
        negotiationRounds = 0;

        // Select good and personality
        var good = rag.GetRandomGood();
        var personality = rag.GetRandomPersonality();

        if (good == null || personality == null)
        {
            dialogueText.text = "No goods or customers available.";
            return;
        }

        currentCustomer = new CustomerState();
        currentCustomer.good = good;
        currentCustomer.personality = personality;
        currentCustomer.name = personality.display_name;
        currentCustomer.item = good.name;
        currentCustomer.quantity = Random.Range(2, 9); // 2..8

        // Pricing calculations
        currentCustomer.fairTotalPrice = good.cost_price_per_unit * currentCustomer.quantity * 1.3f;
        currentCustomer.customerMinAccept = currentCustomer.fairTotalPrice * (1f - personality.desperation * 0.3f);
        currentCustomer.currentCustomerOffer = currentCustomer.fairTotalPrice * personality.opening_offer_ratio;

        // Effective patience considers player respect
        float respect = sessionData != null ? sessionData.respectScore : 50f;
        currentCustomer.patience = personality.patience;
        currentCustomer.effectivePatience = personality.patience + Mathf.FloorToInt(respect / 25f);

        currentCustomer.roundCount = 0;

        negotiationRounds = 0;

        // Highlight cost on board
        if (costBoard != null)
            costBoard.Highlight(good.id);

        // Set slider defaults around fair price
        if (priceSlider != null)
        {
            priceSlider.minValue = Mathf.Max(1f, currentCustomer.fairTotalPrice * 0.5f);
            priceSlider.maxValue = Mathf.Max(priceSlider.minValue + 1f, currentCustomer.fairTotalPrice * 2f);
            priceSlider.value = currentCustomer.fairTotalPrice;
            UpdatePriceText();
        }

        // Record arrival time
        customerStartTime = Time.time;

        // Customer speaks opening line (AI-generated)
        string prompt = PromptBuilder.BuildOpeningPrompt(
            rag.GetKnowledge(),
            personality.tone_prompt_tag,
            currentCustomer.quantity,
            good.unit,
            good.name
        );

        ollama.Generate(prompt, (raw) =>
        {
            try
            {
                var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw);
                dialogueText.text = outer.response;
            }
            catch
            {
                dialogueText.text = "The customer asks after your price.";
            }
        });
    }

    // ----------------------------------
    // SLIDER TEXT UPDATE
    // ----------------------------------

    public void UpdatePriceText()
    {
        int price = (int)priceSlider.value;

        playerPriceText.text = "Your Offer: " + price + " varahas";
    }

    // ----------------------------------
    // PLAYER SUBMITS OFFER
    // ----------------------------------

    public void SubmitOffer()
    {
        if (currentCustomer == null) return;

        float playerPrice = priceSlider.value;
        negotiationRounds++;
        currentCustomer.roundCount++;

        float fair = currentCustomer.fairTotalPrice;
        var p = currentCustomer.personality;

        float timeTaken = Time.time - customerStartTime;

        // Immediate acceptance if below customer's minimum accept
        if (playerPrice <= currentCustomer.customerMinAccept)
        {
            // Customer accepts
            string prompt = PromptBuilder.BuildAcceptPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name);
            ollama.Generate(prompt, (raw) =>
            {
                try
                {
                    var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw);
                    dialogueText.text = outer.response;
                }
                catch
                {
                    dialogueText.text = "Very well. We have a deal.";
                }

                // Update session
                if (sessionData != null)
                {
                    bool isDesperate = p.desperation > 0.7f;
                    sessionData.RecordDeal(playerPrice, fair, currentCustomer.quantity, currentCustomer.good.cost_price_per_unit, timeTaken, isDesperate);
                }

                Invoke("StartCustomer", 3f);
            });

            return;
        }

        // If within 10% above fair, may accept probabilistically
        if (playerPrice <= fair * 1.1f)
        {
            float chanceToAccept = p.desperation * p.price_knowledge;
            if (Random.value < chanceToAccept)
            {
                string prompt = PromptBuilder.BuildAcceptPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); dialogueText.text = outer.response; }
                    catch { dialogueText.text = "Agreed."; }

                    if (sessionData != null)
                    {
                        bool isDesperate = p.desperation > 0.7f;
                        sessionData.RecordDeal(playerPrice, fair, currentCustomer.quantity, currentCustomer.good.cost_price_per_unit, timeTaken, isDesperate);
                    }

                    Invoke("StartCustomer", 3f);
                });

                return;
            }
            // else: fall through to counter
        }

        // If player's price is very high compared to fair, reduce patience
        if (playerPrice > fair * 1.6f)
        {
            currentCustomer.patience--;
        }

        // If patience exhausted
        if (currentCustomer.roundCount >= currentCustomer.effectivePatience)
        {
            if (p.walkaway_aggression > 0.7f)
            {
                // Customer leaves immediately
                string prompt = PromptBuilder.BuildWalkawayPrompt(rag.GetKnowledge(), p.tone_prompt_tag, currentCustomer.roundCount, currentCustomer.good.name, p.walkaway_aggression);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); dialogueText.text = outer.response; }
                    catch { dialogueText.text = "The customer storms off."; }

                    if (sessionData != null) sessionData.RecordWalkaway(false);
                    Invoke("StartCustomer", 3f);
                });

                return;
            }
            else
            {
                // Make one final offer equal to current offer and leave if not accepted
                float finalOffer = currentCustomer.currentCustomerOffer;

                string prompt = PromptBuilder.BuildCounterPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name, fair, finalOffer, currentCustomer.roundCount, currentCustomer.effectivePatience);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); dialogueText.text = outer.response; }
                    catch { dialogueText.text = "This is my final offer."; }

                    // Check acceptance
                    if (playerPrice <= finalOffer)
                    {
                        if (sessionData != null) sessionData.RecordDeal(playerPrice, fair, currentCustomer.quantity, currentCustomer.good.cost_price_per_unit, timeTaken, p.desperation > 0.7f);
                    }
                    else
                    {
                        if (sessionData != null) sessionData.RecordWalkaway(currentCustomer.roundCount <= 1);
                    }

                    Invoke("StartCustomer", 3f);
                });

                return;
            }
        }

        // Normal counter-offer flow
        // If the player's first quote is extremely high, react then counter
        if (currentCustomer.roundCount == 1 && priceSlider != null && playerPrice > fair * 1.6f)
        {
            string react = PromptBuilder.BuildOverpricedReactionPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name);
            ollama.Generate(react, (rawReact) =>
            {
                try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(rawReact); dialogueText.text = outer.response; }
                catch { dialogueText.text = "The customer bristles at your price."; }

                // after short pause, continue with counter
                StartCoroutine(DelayedCounter(playerPrice));
            });

            return;
        }

        // Otherwise compute new customer offer and present it
        StartCoroutine(PerformCounter(playerPrice));
    }

    IEnumerator DelayedCounter(float playerPrice)
    {
        yield return new WaitForSeconds(1.4f);
        yield return PerformCounter(playerPrice);
    }

    IEnumerator PerformCounter(float playerPrice)
    {
        var p = currentCustomer.personality;
        float fair = currentCustomer.fairTotalPrice;

        float increment = fair * (p.concession_step_percent / 100f);
        float newOffer = currentCustomer.currentCustomerOffer + increment;
        currentCustomer.currentCustomerOffer = newOffer;

        // If counter reaches or exceeds player's price, accept
        if (newOffer >= playerPrice)
        {
            string prompt = PromptBuilder.BuildAcceptPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name);
            bool accepted = true;
            ollama.Generate(prompt, (raw) =>
            {
                try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); dialogueText.text = outer.response; }
                catch { dialogueText.text = "Very well, we have a deal."; }

                if (sessionData != null) sessionData.RecordDeal(playerPrice, fair, currentCustomer.quantity, currentCustomer.good.cost_price_per_unit, Time.time - customerStartTime, p.desperation > 0.7f);
                Invoke("StartCustomer", 3f);
            });

            yield break;
        }

        // Otherwise present counter offer dialogue
        string counterPrompt = PromptBuilder.BuildCounterPrompt(rag.GetKnowledge(), p.tone_prompt_tag, playerPrice, currentCustomer.quantity, currentCustomer.good.unit, currentCustomer.good.name, fair, currentCustomer.currentCustomerOffer, currentCustomer.roundCount, currentCustomer.effectivePatience);

        ollama.Generate(counterPrompt, (raw) =>
        {
            try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); dialogueText.text = outer.response; }
            catch { dialogueText.text = "The customer counters with a new price."; }
        });

        yield return null;
    }
}