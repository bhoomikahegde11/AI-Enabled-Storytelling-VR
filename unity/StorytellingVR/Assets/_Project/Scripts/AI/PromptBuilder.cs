using UnityEngine;
using Data;

public static class PromptBuilder
{
    const string styleGuide =
        "IMPORTANT: You are speaking in 16th century Vijayanagar. " +
        "Use period-appropriate vocabulary. Favour words like 'mayhaps', 'pray tell', 'good merchant', 'verily', 'nay', 'aye'. " +
        "Weave in occasional Telugu or Sanskrit words naturally — greet with 'Namaskara', refer to money as 'varaha' or 'fanam', " +
        "call the seller 'vyapari' (merchant) or 'anna' (elder brother) based on tone. " +
        "Never use modern slang, contractions like 'I'm' or 'it's', or contemporary phrasing. " +
        "Keep sentences short and direct as was common in market speech of the era.\n\n";

    public static string BuildIntroPrompt(string knowledge, CustomerPersonality personality)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "You are a " + personality.profession + " visiting a spice market stall in Vijayanagar. " +
            "Your character: " + personality.tone_prompt_tag + "\n\n" +
            "You have just arrived at the stall. Give a single sentence introduction of yourself — " +
            "who you are and what brings you to this market today. " +
            "Do not mention any specific goods or prices yet. Stay completely in character. " +
            "Reveal your personality through your tone, not by stating it directly.\n";
    }

    public static string BuildOpeningPrompt(string knowledge, string tonePromptTag, string customerName, int quantity, string unit, string goodName)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "You are a customer at a Vijayanagar spice market stall. Your character: " + tonePromptTag + "\n\n" +
            "You want to buy exactly " + quantity + " " + unit + " of " + goodName + ". You have just approached the seller's stall.\n" +
            "Ask the seller what their price is. You MUST mention the quantity (" + quantity + " " + unit + ") and the good (" + goodName + ") in your line. " +
            "Stay completely in character. One or two sentences only.\n" +
            "Do not mention any numbers. Do not mention your budget. Just ask.\n" +
            "Begin your line with your name: " + customerName + ":\n";
    }

    public static string BuildCounterPrompt(string knowledge, string tonePromptTag, string customerName, int playerPrice, int quantity, string unit, string goodName, float fairTotal, int customerOffer, int round, int patience, string currency)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has quoted " + playerPrice + " " + currency + " for " + quantity + " " + unit + " of " + goodName + ".\n" +
            "You think a fair price is around " + Mathf.RoundToInt(fairTotal) + " " + currency + " (but do not say this number directly).\n" +
            "You are countering with " + customerOffer + " " + currency + ". This is round " + round + " of " + patience + " rounds you are willing to negotiate.\n" +
            "Respond in character. Express your reaction to their price and make your counter offer naturally within your dialogue. One to three sentences. " +
            "Do not break character. Let your personality come through in how you speak, not by stating who you are.\n" +
            "Begin your line with your name: " + customerName + ":\n";
    }

    public static string BuildAcceptPrompt(string knowledge, string tonePromptTag, string customerName, int playerPrice, int quantity, string unit, string goodName, string currency)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has offered " + playerPrice + " " + currency + " for " + quantity + " " + unit + " of " + goodName + ". You are accepting this deal.\n" +
            "Respond in one sentence. Stay in character. Your acceptance should feel like your character accepting, not a generic agreement.\n" +
            "Begin your line with your name: " + customerName + ":\n";
    }

    public static string BuildWalkawayPrompt(string knowledge, string tonePromptTag, string customerName, int round, string goodName, float walkawayAggression)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "Character: " + tonePromptTag + "\n\n" +
            "After " + round + " rounds of negotiation for " + goodName + ", you are walking away. The seller would not meet your price.\n" +
            "Respond in one to two sentences. Walk away in character.\n" +
            "If walkaway_aggression is above 0.7, your exit should sting. Below 0.7, it should feel like quiet disappointment.\n" +
            "Your walkaway_aggression is " + walkawayAggression.ToString("F1") + " — let this shape your tone without stating it.\n" +
            "Begin your line with your name: " + customerName + ":\n";
    }

    public static string BuildOverpricedReactionPrompt(string knowledge, string tonePromptTag, string customerName, int playerPrice, int quantity, string unit, string goodName, string currency)
    {
        return
            knowledge + "\n\n" +
            styleGuide +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has quoted " + playerPrice + " " + currency + " for " + quantity + " " + unit + " of " + goodName + ".\n" +
            "This is significantly above what you consider fair. React to this price in character before making your counter. " +
            "One to two sentences. Your reaction should reveal your personality through your word choice and tone, not through explanation.\n" +
            "Begin your line with your name: " + customerName + ":\n";
    }
}