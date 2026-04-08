using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;

public class MarketGameManager : MonoBehaviour
{
    [Header("UI — Dialogue")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI playerPriceText;
    public Slider priceSlider;
    public int sliderMin = 1;
    public int sliderMax = 500;

    [Header("UI — Cost Board (separate box)")]
    public TextMeshProUGUI costBoardText;   // assign a separate TMP text in the Inspector

    [Header("UI — Respect Score")]
    public TextMeshProUGUI respectScoreText; // assign a TMP text at the top of the screen
    [Header("UI — Earnings")]
    public TextMeshProUGUI earningsText;

    [Header("AI Systems")]
    public RAGRetriever rag;
    public OllamaClient ollama;

    [Header("Session")]
    public PlayerSessionData sessionData;

    CustomerState currentCustomer;
    int negotiationRounds = 0;
    float customerStartTime = 0f;

    void Start()
    {
        if (priceSlider != null)
        {
            priceSlider.wholeNumbers = true;
            float prev = priceSlider.value;
            priceSlider.minValue = sliderMin;
            priceSlider.maxValue = sliderMax;
            priceSlider.value = Mathf.Clamp(prev, sliderMin, sliderMax);
        }

        UpdatePriceText();
        UpdateRespectUI();
        UpdateEarningsUI();
        StartCustomer();
    }

    // ----------------------------------
    // CUSTOMER ARRIVES
    // ----------------------------------

    void StartCustomer()
    {
        negotiationRounds = 0;

        var good = rag.GetRandomGood();
        var personality = rag.GetRandomPersonality();

        if (good == null || personality == null)
        {
            SetDialogue("No goods or customers available.");
            return;
        }

        currentCustomer = new CustomerState();
        currentCustomer.good = good;
        currentCustomer.personality = personality;
        currentCustomer.name = personality.display_name;
        currentCustomer.item = good.name;
        currentCustomer.quantity = Random.Range(2, 9);

        currentCustomer.fairTotalPrice = good.cost_price_per_unit * currentCustomer.quantity * 1.3f;
        currentCustomer.customerMaxAccept = currentCustomer.fairTotalPrice * (1f + personality.desperation * 0.4f);
        currentCustomer.currentCustomerOffer = Mathf.Round(currentCustomer.fairTotalPrice * personality.opening_offer_ratio);

        float respect = sessionData != null ? sessionData.respectScore : 50f;
        currentCustomer.patience = personality.patience;
        currentCustomer.effectivePatience = personality.patience + Mathf.FloorToInt(respect / 25f);
        currentCustomer.roundCount = 0;
        negotiationRounds = 0;
        customerStartTime = Time.time;

        // --- Issue 7: Console log all customer details ---
        Debug.Log(
            $"[Customer Spawned]\n" +
            $"  Name:               {personality.display_name}\n" +
            $"  Profession:         {personality.profession}\n" +
            $"  Personality ID:     {personality.id}\n" +
            $"  Patience:           {personality.patience} (effective: {currentCustomer.effectivePatience})\n" +
            $"  Desperation:        {personality.desperation:F2}\n" +
            $"  Price Knowledge:    {personality.price_knowledge:F2}\n" +
            $"  Opening Offer Ratio:{personality.opening_offer_ratio:F2}\n" +
            $"  Concession Step:    {personality.concession_step_percent:F1}%\n" +
            $"  Walkaway Aggression:{personality.walkaway_aggression:F2}\n" +
            $"  Good:               {good.name} ({good.id})\n" +
            $"  Quantity:           {currentCustomer.quantity} {good.unit}\n" +
            $"  Cost per unit:      {good.cost_price_per_unit} {good.GetCurrency()}\n" +
            $"  Fair total price:   {currentCustomer.fairTotalPrice:F1} {good.GetCurrency()}\n" +
            $"  Customer max accept:{currentCustomer.customerMaxAccept:F1}\n" +
            $"  Opening offer:      {currentCustomer.currentCustomerOffer:F1}\n" +
            $"  Currency:           {good.GetCurrency()}"
        );

        // --- Issue 4: Update cost board separately ---
        UpdateCostBoard();

        // --- Issue 3: Show intro line first, then opening ask ---
        string introQuery = rag.BuildQuery(personality.id, good.id, good.category, "intro");
        string introKnowledge = rag.RetrieveContext(introQuery);
        string introPrompt = PromptBuilder.BuildIntroPrompt(introKnowledge, personality);

        ollama.Generate(introPrompt, (rawIntro) =>
        {
            string introLine = "";
            try
            {
                var outer = JsonUtility.FromJson<OllamaOuterResponse>(rawIntro);
                introLine = personality.display_name + ": " + LLMUtils.SanitizeDialogue(outer.response);
            }
            catch
            {
                introLine = personality.display_name + ": Namaskara.";
            }

            SetDialogue(introLine);

            // After intro, fire the opening ask after a short pause
            StartCoroutine(DelayedOpeningAsk(1.8f));
        });
    }

    IEnumerator DelayedOpeningAsk(float delay)
    {
        yield return new WaitForSeconds(delay);

        var good = currentCustomer.good;
        var personality = currentCustomer.personality;

        string query = rag.BuildQuery(personality.id, good.id, good.category, "opening");
        string knowledge = rag.RetrieveContext(query);

        string openPrompt = PromptBuilder.BuildOpeningPrompt(
            knowledge,
            personality.tone_prompt_tag,
            personality.display_name,
            currentCustomer.quantity,
            good.unit,
            good.name
        );

        ollama.Generate(openPrompt, (raw) =>
        {
            try
            {
                var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw);
                SetDialogue(LLMUtils.SanitizeDialogue(outer.response));
            }
            catch
            {
                SetDialogue(personality.display_name + ": Pray tell, what is your price for "
                    + currentCustomer.quantity + " " + good.unit + " of " + good.name + "?");
            }
        });
    }

    // --- Issue 4: Cost board is separate from dialogue ---
    void UpdateCostBoard()
    {
        if (costBoardText == null || currentCustomer?.good == null) return;
        var g = currentCustomer.good;
        costBoardText.text =
            "Cost: " + Mathf.RoundToInt(g.cost_price_per_unit) + " " + g.GetCurrency() + " per " + g.unit;
    }

    // --- Issue 6: Respect UI ---
    void UpdateRespectUI()
    {
        if (respectScoreText == null || sessionData == null) return;
        int score = Mathf.RoundToInt(sessionData.respectScore);
        string label = sessionData.GetRespectLabel();
        respectScoreText.text = "Respect: " + score + " — " + label;
    }
    void UpdateEarningsUI()
    {
        if (earningsText == null || sessionData == null) return;
        float profit = sessionData.totalRevenue - sessionData.totalCost;
        earningsText.text =
            "Earned: " + Mathf.RoundToInt(sessionData.totalRevenue) + " varaha" +
            "  |  Cost: " + Mathf.RoundToInt(sessionData.totalCost) + " varaha" +
            "  |  Profit: " + Mathf.RoundToInt(profit) + " varaha";
    }

    void SetDialogue(string content)
    {
        if (dialogueText != null)
            dialogueText.text = content;
    }

    public void UpdatePriceText()
    {
        int price = (int)priceSlider.value;
        string currency = currentCustomer?.good?.GetCurrency() ?? "varaha";
        playerPriceText.text = "Your Offer: " + price + " " + currency;
    }

    // ----------------------------------
    // PLAYER SUBMITS OFFER
    // ----------------------------------

    public void SubmitOffer()
    {
        if (currentCustomer == null) return;

        float playerPrice = priceSlider.value;
        int playerPriceInt = Mathf.RoundToInt(playerPrice);
        negotiationRounds++;
        currentCustomer.roundCount++;

        float fair = currentCustomer.fairTotalPrice;
        var p = currentCustomer.personality;
        var good = currentCustomer.good;
        string currency = good.GetCurrency();
        float timeTaken = Time.time - customerStartTime;

        string query = rag.BuildQuery(p.id, good.id, good.category, "counter");
        string knowledge = rag.RetrieveContext(query);

        // Immediate acceptance if below customer minimum
        if (playerPriceInt <= Mathf.RoundToInt(currentCustomer.customerMaxAccept))
        {
            string prompt = PromptBuilder.BuildAcceptPrompt(knowledge, p.tone_prompt_tag, p.display_name, playerPriceInt, currentCustomer.quantity, good.unit, good.name, currency);
            ollama.Generate(prompt, (raw) =>
            {
                try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                catch { SetDialogue(p.display_name + ": Very well. We have a deal."); }

                if (sessionData != null)
                    sessionData.RecordDeal(playerPriceInt, fair, currentCustomer.quantity, good.cost_price_per_unit, timeTaken, p.desperation > 0.7f);

                UpdateRespectUI();
                UpdateEarningsUI();
                Invoke("StartCustomer", 3f);
            });
            return;
        }

        // Probabilistic acceptance if within 10% of fair
        if (playerPrice <= fair * 1.1f)
        {
            float chanceToAccept = p.desperation * 0.6 + p.price_knowledge * 0.4;
            if (Random.value < chanceToAccept)
            {
                string prompt = PromptBuilder.BuildAcceptPrompt(knowledge, p.tone_prompt_tag, p.display_name, playerPriceInt, currentCustomer.quantity, good.unit, good.name, currency);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                    catch { SetDialogue(p.display_name + ": Agreed."); }

                    if (sessionData != null)
                        sessionData.RecordDeal(playerPriceInt, fair, currentCustomer.quantity, good.cost_price_per_unit, timeTaken, p.desperation > 0.7f);

                    UpdateRespectUI();
                    UpdateEarningsUI();
                    Invoke("StartCustomer", 3f);
                });
                return;
            }
        }

        // Reduce patience for very high prices
        if (playerPrice > fair * 1.6f)
            currentCustomer.patience--;

        // Patience exhausted
        if (currentCustomer.roundCount >= currentCustomer.effectivePatience)
        {
            string walkQuery = rag.BuildQuery(p.id, good.id, good.category, "walkaway");
            string walkKnowledge = rag.RetrieveContext(walkQuery);

            if (p.walkaway_aggression > 0.7f)
            {
                string prompt = PromptBuilder.BuildWalkawayPrompt(walkKnowledge, p.tone_prompt_tag, p.display_name, currentCustomer.roundCount, good.name, p.walkaway_aggression);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                    catch { SetDialogue(p.display_name + ": I have no time for this. Good day."); }

                    if (sessionData != null) sessionData.RecordWalkaway(false);
                    UpdateRespectUI();
                    UpdateEarningsUI();
                    Invoke("StartCustomer", 3f);
                });
                return;
            }
            else
            {
                int finalOfferInt = Mathf.RoundToInt(currentCustomer.currentCustomerOffer);
                string prompt = PromptBuilder.BuildCounterPrompt(walkKnowledge, p.tone_prompt_tag, p.display_name, playerPriceInt, currentCustomer.quantity, good.unit, good.name, fair, finalOfferInt, currentCustomer.roundCount, currentCustomer.effectivePatience, currency);
                ollama.Generate(prompt, (raw) =>
                {
                    try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                    catch { SetDialogue(p.display_name + ": This is my final offer."); }

                    if (playerPriceInt <= finalOfferInt)
                    {
                        if (sessionData != null) sessionData.RecordDeal(playerPriceInt, fair, currentCustomer.quantity, good.cost_price_per_unit, timeTaken, p.desperation > 0.7f);
                    }
                    else
                    {
                        if (sessionData != null) sessionData.RecordWalkaway(currentCustomer.roundCount <= 1);
                    }

                    UpdateRespectUI();
                    UpdateEarningsUI();
                    Invoke("StartCustomer", 3f);
                });
                return;
            }
        }

        // Overpriced reaction on round 1
        if (currentCustomer.roundCount == 1 && playerPrice > fair * 1.6f)
        {
            string reactQuery = rag.BuildQuery(p.id, good.id, good.category, "overpriced");
            string reactKnowledge = rag.RetrieveContext(reactQuery);
            string react = PromptBuilder.BuildOverpricedReactionPrompt(reactKnowledge, p.tone_prompt_tag, p.display_name, playerPriceInt, currentCustomer.quantity, good.unit, good.name, currency);
            ollama.Generate(react, (rawReact) =>
            {
                try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(rawReact); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                catch { SetDialogue(p.display_name + ": That price is an insult."); }
                StartCoroutine(DelayedCounter(playerPrice));
            });
            return;
        }

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
        var good = currentCustomer.good;
        string currency = good.GetCurrency();
        float fair = currentCustomer.fairTotalPrice;

        float increment = fair * (p.concession_step_percent / 100f);
        float rawNew = currentCustomer.currentCustomerOffer + increment;
        int newOfferInt = Mathf.RoundToInt(rawNew);
        currentCustomer.currentCustomerOffer = newOfferInt;
        int playerPriceIntLocal = Mathf.RoundToInt(playerPrice);

        string query = rag.BuildQuery(p.id, good.id, good.category, "counter");
        string knowledge = rag.RetrieveContext(query);

        if (newOfferInt >= playerPriceIntLocal)
        {
            string prompt = PromptBuilder.BuildAcceptPrompt(knowledge, p.tone_prompt_tag, p.display_name, playerPriceIntLocal, currentCustomer.quantity, good.unit, good.name, currency);
            ollama.Generate(prompt, (raw) =>
            {
                try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
                catch { SetDialogue(p.display_name + ": Very well, we have a deal."); }

                if (sessionData != null) sessionData.RecordDeal(playerPriceIntLocal, fair, currentCustomer.quantity, good.cost_price_per_unit, Time.time - customerStartTime, p.desperation > 0.7f);
                UpdateRespectUI();
                UpdateEarningsUI();
                Invoke("StartCustomer", 3f);
            });
            yield break;
        }

        int currentOfferInt = Mathf.RoundToInt(currentCustomer.currentCustomerOffer);
        string counterPrompt = PromptBuilder.BuildCounterPrompt(knowledge, p.tone_prompt_tag, p.display_name, playerPriceIntLocal, currentCustomer.quantity, good.unit, good.name, fair, currentOfferInt, currentCustomer.roundCount, currentCustomer.effectivePatience, currency);

        ollama.Generate(counterPrompt, (raw) =>
        {
            try { var outer = JsonUtility.FromJson<OllamaOuterResponse>(raw); SetDialogue(LLMUtils.SanitizeDialogue(outer.response)); }
            catch { SetDialogue(p.display_name + ": I offer " + currentOfferInt + " " + currency + "."); }
        });

        yield return null;
    }
    

}