using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Integrator between the tutorial system and your teammate's dialogue script.
/// Replace the placeholder methods with actual calls to your dialogue system.
/// </summary>
public class TutorialDialogueIntegrator : MonoBehaviour
{
    [Header("External Dialogue System")]
    [Tooltip("Reference to your teammate's dialogue script")]
    public MonoBehaviour externalDialogueScript;
    
    [Header("Dialogue Events")]
    public UnityEvent<string> OnNarratorDialogueTriggered;
    public UnityEvent<string> OnCustomerDialogueTriggered;
    
    [Header("Debug Settings")]
    public bool useDebugMode = true;
    public bool logDialogueTriggers = true;
    
    // Dictionary to store dialogue mappings (key -> actual dialogue text)
    // Your teammate's script should populate this or provide the text directly
    private Dictionary<string, string> narratorDialogueMap = new Dictionary<string, string>();
    private Dictionary<string, string> customerDialogueMap = new Dictionary<string, string>();

    void Awake()
    {
        InitializeDebugDialogues();
    }

    /// <summary>
    /// Triggers narrator dialogue by key.
    /// Replace this implementation with actual call to your teammate's system.
    /// </summary>
    public void TriggerNarratorDialogue(string dialogueKey)
    {
        if (logDialogueTriggers)
        {
            Debug.Log($"[Narrator] Triggered: {dialogueKey}");
        }
        
        OnNarratorDialogueTriggered?.Invoke(dialogueKey);
        
        // REPLACE THIS SECTION with actual integration
        if (useDebugMode)
        {
            string dialogueText = GetNarratorDialogue(dialogueKey);
            Debug.Log($"[Narrator Says]: {dialogueText}");
            
            // Example: Call your teammate's script
            // externalDialogueScript.SendMessage("ShowNarratorDialogue", dialogueText);
            // OR
            // ((YourDialogueScript)externalDialogueScript).ShowNarratorDialogue(dialogueText);
        }
    }

    /// <summary>
    /// Triggers customer dialogue by key.
    /// Replace this implementation with actual call to your teammate's system.
    /// </summary>
    public void TriggerCustomerDialogue(string dialogueKey)
    {
        if (logDialogueTriggers)
        {
            Debug.Log($"[Customer] Triggered: {dialogueKey}");
        }
        
        OnCustomerDialogueTriggered?.Invoke(dialogueKey);
        
        // REPLACE THIS SECTION with actual integration
        if (useDebugMode)
        {
            string dialogueText = GetCustomerDialogue(dialogueKey);
            Debug.Log($"[Customer Says]: {dialogueText}");
            
            // Example: Call your teammate's script
            // externalDialogueScript.SendMessage("ShowCustomerDialogue", dialogueText);
            // OR
            // ((YourDialogueScript)externalDialogueScript).ShowCustomerDialogue(dialogueText);
        }
    }

    /// <summary>
    /// Get narrator dialogue text by key.
    /// This is a placeholder - your teammate's script should provide the actual text.
    /// </summary>
    string GetNarratorDialogue(string key)
    {
        if (narratorDialogueMap.ContainsKey(key))
        {
            return narratorDialogueMap[key];
        }
        
        return $"[NARRATOR DIALOGUE: {key}]";
    }

    /// <summary>
    /// Get customer dialogue text by key.
    /// This is a placeholder - your teammate's script should provide the actual text.
    /// </summary>
    string GetCustomerDialogue(string key)
    {
        if (customerDialogueMap.ContainsKey(key))
        {
            return customerDialogueMap[key];
        }
        
        return $"[CUSTOMER DIALOGUE: {key}]";
    }

    /// <summary>
    /// Debug placeholder dialogues for testing.
    /// Remove this when integrating with real dialogue system.
    /// </summary>
    void InitializeDebugDialogues()
    {
        // Narrator dialogues
        narratorDialogueMap["intro_welcome"] = "Welcome to the Vijayanagar Market tutorial!";
        narratorDialogueMap["spice_intro_start"] = "Let me show you the spices you'll be trading...";
        narratorDialogueMap["spice_highlight_cardamom"] = "This is Cardamom. Notice the cost price carefully.";
        narratorDialogueMap["customer_intro_start"] = "Now, let's learn about your customers...";
        narratorDialogueMap["narrator_remind_cost"] = "Remember, the cost is 15 pagodas per unit. You're buying 10 units.";
        narratorDialogueMap["narrator_ask_absurd_price"] = "Try quoting an absurdly high price. Let's see how the customer reacts!";
        narratorDialogueMap["narrator_ask_better_price"] = "Now try offering a fairer price.";
        
        narratorDialogueMap["narrator_explain_below_cost"] = "That's below your cost! You'd lose money.";
        narratorDialogueMap["narrator_explain_too_generous"] = "That's very generous, but you're barely making profit.";
        narratorDialogueMap["narrator_explain_fair_price"] = "A fair price! Good for building reputation.";
        narratorDialogueMap["narrator_explain_slightly_high"] = "A bit high, but might work with a desperate customer.";
        narratorDialogueMap["narrator_explain_very_high"] = "That's quite expensive! Most customers will negotiate hard.";
        narratorDialogueMap["narrator_explain_absurd"] = "That's way too high! See the customer's reaction?";
        
        narratorDialogueMap["narrator_final_fair_deal"] = "Excellent! A fair deal builds reputation.";
        narratorDialogueMap["narrator_final_good_profit"] = "Good profit, and the customer still accepted!";
        narratorDialogueMap["narrator_final_lucky_accept"] = "Lucky! The customer was desperate enough to accept.";
        narratorDialogueMap["narrator_final_walkaway_lesson"] = "The customer walked away. Price it better next time!";
        narratorDialogueMap["narrator_explain_walkaway"] = "Too expensive! The customer left, hurting your reputation.";
        narratorDialogueMap["narrator_tutorial_complete"] = "Tutorial complete! You're ready for the real market.";
        
        // Customer dialogues
        customerDialogueMap["customer_greeting"] = "Namaskara, merchant! I've heard good things about your stall.";
        customerDialogueMap["customer_request_spice"] = "I need 10 measures of cardamom for a wedding feast.";
        customerDialogueMap["customer_confused_too_low"] = "That price... are you sure? That seems too low.";
        customerDialogueMap["customer_suspicious_generous"] = "Your generosity is unusual, merchant. Is something wrong with the spice?";
        customerDialogueMap["customer_pleased_fair"] = "A fair price! Let's make this deal.";
        customerDialogueMap["customer_hesitant_high"] = "Hmm... that's higher than I expected. Let me think...";
        customerDialogueMap["customer_angry_very_high"] = "What?! That's highway robbery! You must be joking.";
        customerDialogueMap["customer_insulted_absurd"] = "THAT PRICE?! You insult me and my family! Do I look like a fool?";
        customerDialogueMap["customer_accept"] = "Alright, merchant. You have a deal.";
        customerDialogueMap["customer_walkaway"] = "I'll take my business elsewhere. Good day!";
    }

    // ============================================
    // INTEGRATION METHODS FOR YOUR TEAMMATE
    // ============================================
    
    /// <summary>
    /// Call this from your teammate's script to register narrator dialogues.
    /// Example: dialogueIntegrator.RegisterNarratorDialogue("intro_welcome", "Welcome to the market!");
    /// </summary>
    public void RegisterNarratorDialogue(string key, string dialogueText)
    {
        if (narratorDialogueMap.ContainsKey(key))
        {
            narratorDialogueMap[key] = dialogueText;
        }
        else
        {
            narratorDialogueMap.Add(key, dialogueText);
        }
    }

    /// <summary>
    /// Call this from your teammate's script to register customer dialogues.
    /// </summary>
    public void RegisterCustomerDialogue(string key, string dialogueText)
    {
        if (customerDialogueMap.ContainsKey(key))
        {
            customerDialogueMap[key] = dialogueText;
        }
        else
        {
            customerDialogueMap.Add(key, dialogueText);
        }
    }

    /// <summary>
    /// Batch register all narrator dialogues at once.
    /// </summary>
    public void RegisterNarratorDialogues(Dictionary<string, string> dialogues)
    {
        foreach (var kvp in dialogues)
        {
            RegisterNarratorDialogue(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Batch register all customer dialogues at once.
    /// </summary>
    public void RegisterCustomerDialogues(Dictionary<string, string> dialogues)
    {
        foreach (var kvp in dialogues)
        {
            RegisterCustomerDialogue(kvp.Key, kvp.Value);
        }
    }
}