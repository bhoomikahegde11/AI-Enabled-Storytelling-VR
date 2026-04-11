using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main tutorial controller that manages the tutorial flow and coordinates with dialogue system
/// Handles tutorial stages: Spice Introduction -> Customer Introduction -> Transaction Demo
/// </summary>
public class TutorialManager1 : MonoBehaviour
{
    [Header("Tutorial Stages")]
    public enum TutorialStage
    {
        Intro,
        SpiceIntroduction,
        CustomerIntroduction,
        TransactionSetup,
        FirstPriceEntry,      // Player enters absurd price
        CustomerReaction,      // Show customer reaction to absurd price
        SecondPriceEntry,      // Player tries again
        FinalReaction,         // Final outcome
        TutorialComplete
    }

    [Header("References")]
    public TutorialUIManager uiManager;
    public TutorialDialogueIntegrator dialogueIntegrator;
    public TutorialSpiceDisplay spiceDisplay;
    public TutorialCustomerDisplay customerDisplay;
    public TutorialTransactionManager transactionManager;
    
    [Header("Tutorial Configuration")]
    public TutorialStage currentStage = TutorialStage.Intro;
    public bool autoAdvanceEnabled = false;
    public float autoAdvanceDelay = 2f;

    [Header("Hardcoded Tutorial Data")]
    public string tutorialSpiceName = "Cardamom";
    public float tutorialSpiceCost = 15f;
    public int tutorialQuantity = 10;
    public float tutorialFairPrice = 195f; // cost * quantity * 1.3
    
    [Header("Customer Stats for Tutorial")]
    public string customerName = "Ravi";
    public float customerPatience = 3f;
    public float customerDesperation = 0.6f;
    public float customerPriceKnowledge = 0.7f;

    [Header("Tutorial State")]
    public float playerFirstPrice = 0f;
    public float playerSecondPrice = 0f;
    public int currentRespect = 50;
    public float currentProfit = 0f;
    
    private bool waitingForPlayerInput = false;
    private bool stageCompleted = false;

    void Start()
    {
        InitializeTutorial();
    }

    void InitializeTutorial()
    {
        Debug.Log("Tutorial Initialized");
        currentRespect = 50;
        currentProfit = 0f;
        
        // Initialize UI
        if (uiManager != null)
        {
            uiManager.UpdateRespectScore(currentRespect, 0);
            uiManager.UpdateProfitDisplay(0f, tutorialSpiceCost * tutorialQuantity, 0f);
        }
        
        StartCoroutine(RunTutorialSequence());
    }

    IEnumerator RunTutorialSequence()
    {
        yield return StartCoroutine(RunIntro());
        yield return StartCoroutine(RunSpiceIntroduction());
        yield return StartCoroutine(RunCustomerIntroduction());
        yield return StartCoroutine(RunTransactionDemo());
        
        currentStage = TutorialStage.TutorialComplete;
        OnTutorialComplete();
    }

    IEnumerator RunIntro()
    {
        currentStage = TutorialStage.Intro;
        Debug.Log("Stage: Intro");
        
        // Trigger narrator dialogue through integrator
        dialogueIntegrator.TriggerNarratorDialogue("intro_welcome");
        
        yield return new WaitForSeconds(3f);
    }

    IEnumerator RunSpiceIntroduction()
    {
        currentStage = TutorialStage.SpiceIntroduction;
        Debug.Log("Stage: Spice Introduction");
        
        // Show all spices
        dialogueIntegrator.TriggerNarratorDialogue("spice_intro_start");
        yield return new WaitForSeconds(2f);
        
        if (spiceDisplay != null)
        {
            yield return StartCoroutine(spiceDisplay.ShowAllSpices());
        }
        
        // Highlight the tutorial spice
        yield return new WaitForSeconds(1f);
        dialogueIntegrator.TriggerNarratorDialogue("spice_highlight_cardamom");
        
        if (spiceDisplay != null)
        {
            spiceDisplay.HighlightSpice(tutorialSpiceName, tutorialSpiceCost);
        }
        
        yield return new WaitForSeconds(3f);
    }

    IEnumerator RunCustomerIntroduction()
    {
        currentStage = TutorialStage.CustomerIntroduction;
        Debug.Log("Stage: Customer Introduction");
        
        dialogueIntegrator.TriggerNarratorDialogue("customer_intro_start");
        yield return new WaitForSeconds(2f);
        
        if (customerDisplay != null)
        {
            yield return StartCoroutine(customerDisplay.ShowCustomerStats(
                customerName, 
                customerPatience, 
                customerDesperation, 
                customerPriceKnowledge
            ));
        }
        
        yield return new WaitForSeconds(3f);
    }

    IEnumerator RunTransactionDemo()
    {
        // Setup transaction
        currentStage = TutorialStage.TransactionSetup;
        Debug.Log("Stage: Transaction Setup");
        
        transactionManager.SetupTransaction(tutorialSpiceName, tutorialQuantity, tutorialSpiceCost);
        
        // Customer greeting
        dialogueIntegrator.TriggerCustomerDialogue("customer_greeting");
        yield return new WaitForSeconds(2f);
        
        // Customer asks for spice
        dialogueIntegrator.TriggerCustomerDialogue("customer_request_spice");
        yield return new WaitForSeconds(2f);
        
        // Narrator reminds cost
        dialogueIntegrator.TriggerNarratorDialogue("narrator_remind_cost");
        transactionManager.HighlightCostPrice(tutorialSpiceCost);
        yield return new WaitForSeconds(2f);
        
        // First price entry - Ask for absurd price
        currentStage = TutorialStage.FirstPriceEntry;
        dialogueIntegrator.TriggerNarratorDialogue("narrator_ask_absurd_price");
        transactionManager.EnablePriceInput(true);
        
        waitingForPlayerInput = true;
        yield return new WaitUntil(() => !waitingForPlayerInput);
        
        // Process first price
        yield return StartCoroutine(ProcessFirstPrice());
        
        // Second price entry - Ask for better price
        currentStage = TutorialStage.SecondPriceEntry;
        dialogueIntegrator.TriggerNarratorDialogue("narrator_ask_better_price");
        transactionManager.EnablePriceInput(true);
        
        waitingForPlayerInput = true;
        yield return new WaitUntil(() => !waitingForPlayerInput);
        
        // Process second price
        yield return StartCoroutine(ProcessSecondPrice());
    }

    IEnumerator ProcessFirstPrice()
    {
        currentStage = TutorialStage.CustomerReaction;
        transactionManager.EnablePriceInput(false);
        
        Debug.Log($"Player first price: {playerFirstPrice}");
        
        // Determine customer reaction based on price
        string reactionKey = DetermineCustomerReaction(playerFirstPrice, true);
        dialogueIntegrator.TriggerCustomerDialogue(reactionKey);
        
        // Update UI based on if deal would happen
        float tempRespect = CalculateRespectChange(playerFirstPrice);
        float tempProfit = playerFirstPrice - (tutorialSpiceCost * tutorialQuantity);
        
        // Show temporary UI feedback
        uiManager.ShowTemporaryRespectChange(tempRespect);
        transactionManager.ShowProfitPreview(tempProfit);
        
        yield return new WaitForSeconds(3f);
        
        // Narrator explains what went wrong
        string narratorReactionKey = DetermineNarratorReaction(playerFirstPrice, true);
        dialogueIntegrator.TriggerNarratorDialogue(narratorReactionKey);
        
        yield return new WaitForSeconds(3f);
    }

    IEnumerator ProcessSecondPrice()
    {
        currentStage = TutorialStage.FinalReaction;
        transactionManager.EnablePriceInput(false);
        
        Debug.Log($"Player second price: {playerSecondPrice}");
        
        // Determine final outcome
        bool dealAccepted = EvaluateDeal(playerSecondPrice);
        
        if (dealAccepted)
        {
            // Customer accepts
            dialogueIntegrator.TriggerCustomerDialogue("customer_accept");
            yield return new WaitForSeconds(2f);
            
            // Calculate actual profit and respect
            float profit = playerSecondPrice - (tutorialSpiceCost * tutorialQuantity);
            int respectChange = (int)CalculateRespectChange(playerSecondPrice);
            
            currentProfit = profit;
            currentRespect += respectChange;
            currentRespect = Mathf.Clamp(currentRespect, 0, 100);
            
            // Update UI
            uiManager.UpdateRespectScore(currentRespect, respectChange);
            uiManager.UpdateProfitDisplay(playerSecondPrice, tutorialSpiceCost * tutorialQuantity, profit);
            transactionManager.ShowDealSuccess();
            
            // Narrator congratulates
            string narratorKey = DetermineNarratorFinalReaction(playerSecondPrice, true);
            dialogueIntegrator.TriggerNarratorDialogue(narratorKey);
        }
        else
        {
            // Customer walks away
            dialogueIntegrator.TriggerCustomerDialogue("customer_walkaway");
            yield return new WaitForSeconds(2f);
            
            // Respect penalty
            int respectChange = -8;
            currentRespect += respectChange;
            currentRespect = Mathf.Clamp(currentRespect, 0, 100);
            
            uiManager.UpdateRespectScore(currentRespect, respectChange);
            transactionManager.ShowDealFailure();
            
            // Narrator explains
            dialogueIntegrator.TriggerNarratorDialogue("narrator_explain_walkaway");
        }
        
        yield return new WaitForSeconds(3f);
    }

    // Called by TutorialTransactionManager when player submits price
    public void OnPlayerSubmitPrice(float price)
    {
        if (currentStage == TutorialStage.FirstPriceEntry)
        {
            playerFirstPrice = price;
            waitingForPlayerInput = false;
        }
        else if (currentStage == TutorialStage.SecondPriceEntry)
        {
            playerSecondPrice = price;
            waitingForPlayerInput = false;
        }
    }

    string DetermineCustomerReaction(float price, bool isFirstAttempt)
    {
        float fairPrice = tutorialFairPrice;
        float percentAboveFair = ((price - fairPrice) / fairPrice) * 100f;
        
        if (price < tutorialSpiceCost * tutorialQuantity)
        {
            return "customer_confused_too_low"; // Below cost
        }
        else if (percentAboveFair < -10f)
        {
            return "customer_suspicious_generous"; // Suspiciously low
        }
        else if (percentAboveFair <= 20f)
        {
            return "customer_pleased_fair"; // Fair deal
        }
        else if (percentAboveFair <= 40f)
        {
            return "customer_hesitant_high"; // Slightly high
        }
        else if (percentAboveFair <= 80f)
        {
            return "customer_angry_very_high"; // Very high
        }
        else
        {
            return "customer_insulted_absurd"; // Absurdly high
        }
    }

    string DetermineNarratorReaction(float price, bool isFirstAttempt)
    {
        float fairPrice = tutorialFairPrice;
        float percentAboveFair = ((price - fairPrice) / fairPrice) * 100f;
        
        if (price < tutorialSpiceCost * tutorialQuantity)
        {
            return "narrator_explain_below_cost";
        }
        else if (percentAboveFair < -10f)
        {
            return "narrator_explain_too_generous";
        }
        else if (percentAboveFair <= 20f)
        {
            return "narrator_explain_fair_price";
        }
        else if (percentAboveFair <= 40f)
        {
            return "narrator_explain_slightly_high";
        }
        else if (percentAboveFair <= 80f)
        {
            return "narrator_explain_very_high";
        }
        else
        {
            return "narrator_explain_absurd";
        }
    }

    string DetermineNarratorFinalReaction(float price, bool dealAccepted)
    {
        float fairPrice = tutorialFairPrice;
        float percentAboveFair = ((price - fairPrice) / fairPrice) * 100f;
        
        if (!dealAccepted)
        {
            return "narrator_final_walkaway_lesson";
        }
        else if (percentAboveFair <= 20f)
        {
            return "narrator_final_fair_deal";
        }
        else if (percentAboveFair <= 40f)
        {
            return "narrator_final_good_profit";
        }
        else
        {
            return "narrator_final_lucky_accept";
        }
    }

    float CalculateRespectChange(float price)
    {
        float fairPrice = tutorialFairPrice;
        float percentAboveFair = ((price - fairPrice) / fairPrice) * 100f;
        
        if (percentAboveFair <= 20f)
        {
            return 8f; // Fair deal
        }
        else if (percentAboveFair <= 40f)
        {
            return 3f; // Slightly overpriced
        }
        else
        {
            return 1f; // Heavily overpriced
        }
    }

    bool EvaluateDeal(float price)
    {
        float fairPrice = tutorialFairPrice;
        float customerMaxAccept = fairPrice * (1f + customerDesperation * 0.4f);
        
        // Tutorial: simplified acceptance logic
        // In real game, this would include probabilistic elements
        return price <= customerMaxAccept;
    }

    void OnTutorialComplete()
    {
        Debug.Log("Tutorial Complete!");
        dialogueIntegrator.TriggerNarratorDialogue("narrator_tutorial_complete");
        
        if (uiManager != null)
        {
            uiManager.ShowTutorialCompleteScreen(currentProfit, currentRespect);
        }
    }

    // Public method to skip to specific stage (for testing)
    public void SkipToStage(TutorialStage stage)
    {
        StopAllCoroutines();
        currentStage = stage;
        
        switch (stage)
        {
            case TutorialStage.SpiceIntroduction:
                StartCoroutine(RunSpiceIntroduction());
                break;
            case TutorialStage.CustomerIntroduction:
                StartCoroutine(RunCustomerIntroduction());
                break;
            case TutorialStage.TransactionSetup:
                StartCoroutine(RunTransactionDemo());
                break;
        }
    }
}