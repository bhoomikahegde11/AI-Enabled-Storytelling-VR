using UnityEngine;
using Data;

public static class PromptBuilder
{
    public static string BuildOpeningPrompt(string knowledge, string tonePromptTag, int quantity, string unit, string goodName)
    {
        return
            knowledge + "\n\n" +
            "You are a customer at a Vijayanagar spice market stall. Your character: " + tonePromptTag + "\n\n" +
            "You want to buy " + quantity + " " + unit + " of " + goodName + ". You have just approached the seller's stall.\n" +
            "Ask the seller what their price is for this quantity. Stay completely in character. One or two sentences only.\n" +
            "Do not mention any numbers. Do not mention your budget. Just ask.\n";
    }

    public static string BuildCounterPrompt(string knowledge, string tonePromptTag, float playerPrice, int quantity, string unit, string goodName, float fairTotal, float customerOffer, int round, int patience)
    {
        return
            knowledge + "\n\n" +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has quoted " + playerPrice + " Varahas for " + quantity + " " + unit + " of " + goodName + ".\n" +
            "You think a fair price is around " + fairTotal + " Varahas (but do not say this number directly).\n" +
            "You are countering with " + customerOffer + " Varahas. This is round " + round + " of " + patience + " rounds you are willing to negotiate.\n" +
            "Respond in character. Express your reaction to their price and make your counter offer naturally within your dialogue. One to three sentences. Do not break character. Let your personality come through in how you speak, not by stating who you are.\n";
    }

    public static string BuildAcceptPrompt(string knowledge, string tonePromptTag, float playerPrice, int quantity, string unit, string goodName)
    {
        return
            knowledge + "\n\n" +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has offered " + playerPrice + " Varahas for " + quantity + " " + unit + " of " + goodName + ". You are accepting this deal.\n" +
            "Respond in one sentence. Stay in character. Your acceptance should feel like your character accepting, not a generic agreement.\n";
    }

    public static string BuildWalkawayPrompt(string knowledge, string tonePromptTag, int round, string goodName, float walkawayAggression)
    {
        return
            knowledge + "\n\n" +
            "Character: " + tonePromptTag + "\n\n" +
            "After " + round + " rounds of negotiation for " + goodName + ", you are walking away. The seller would not meet your price.\n" +
            "Respond in one to two sentences. Walk away in character. If you are high aggression, your exit should sting. If you are low aggression, it should feel like quiet disappointment.\n" +
            "Your walkaway_aggression is " + walkawayAggression + " out of 1.0 — let this shape your tone without stating it.\n";
    }

    public static string BuildOverpricedReactionPrompt(string knowledge, string tonePromptTag, float playerPrice, int quantity, string unit, string goodName)
    {
        return
            knowledge + "\n\n" +
            "Character: " + tonePromptTag + "\n\n" +
            "The seller has quoted " + playerPrice + " Varahas for " + quantity + " " + unit + " of " + goodName + ".\n" +
            "This is significantly above what you consider fair. React to this price in character before making your counter. One to two sentences. Your reaction should reveal your personality through your word choice and tone, not through explanation.\n";
    }
}