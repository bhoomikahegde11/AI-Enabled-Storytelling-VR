using System.Collections.Generic;
using UnityEngine;

public class DialogueTableResponseProvider
{
    private enum DialogueAction
    {
        ACCEPT,
        COUNTER,
        REJECT_TOO_LOW,
        ASK_QUANTITY,
        ASK_PRICE,
        SOCIAL,
        OFF_TOPIC,
        UNKNOWN
    }

    private enum RoundBucket
    {
        First,
        Middle,
        Final
    }

    private sealed class DialogueTemplate
    {
        public DialogueAction action;
        public string personality;
        public RoundBucket roundBucket;
        public string template;

        public DialogueTemplate(DialogueAction action, string personality, RoundBucket roundBucket, string template)
        {
            this.action = action;
            this.personality = personality;
            this.roundBucket = roundBucket;
            this.template = template;
        }
    }

    private static readonly List<DialogueTemplate> Templates = new List<DialogueTemplate>
    {
        new DialogueTemplate(DialogueAction.ACCEPT, "Any", RoundBucket.First, "Agreed, {buyerName}. {finalPrice} {currency} for {quantityLabel} of {spiceName} is fair."),
        new DialogueTemplate(DialogueAction.ACCEPT, "Friendly", RoundBucket.Middle, "You bargain well, merchant. I accept {finalPrice} {currency} for the {spiceName}."),
        new DialogueTemplate(DialogueAction.ACCEPT, "Strict", RoundBucket.Middle, "Very well. We will settle at {finalPrice} {currency} for {quantityLabel} of {spiceName}."),
        new DialogueTemplate(DialogueAction.ACCEPT, "Impatient", RoundBucket.Final, "Done. {finalPrice} {currency}. Let us finish this trade."),
        new DialogueTemplate(DialogueAction.ACCEPT, "Any", RoundBucket.Final, "We have an agreement. {finalPrice} {currency} for {quantityLabel} of {spiceName}."),

        new DialogueTemplate(DialogueAction.COUNTER, "Any", RoundBucket.First, "My first word is not my last. I can move to {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.COUNTER, "Friendly", RoundBucket.Middle, "You press your case well. I can raise my offer to {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.COUNTER, "Strict", RoundBucket.Middle, "I will move only a little. {counterPrice} {currency}, no more for now."),
        new DialogueTemplate(DialogueAction.COUNTER, "Impatient", RoundBucket.Middle, "Quickly then: {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.COUNTER, "Any", RoundBucket.Final, "This is my nearest step. {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.COUNTER, "Any", RoundBucket.Final, "We are close. I can make it {counterPrice} {currency}."),

        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Any", RoundBucket.First, "That is too low for {spiceName}. I cannot go below {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Friendly", RoundBucket.Middle, "I would wrong myself at that price. I still need {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Strict", RoundBucket.Middle, "No. {offeredPrice} will not do. My floor is {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Impatient", RoundBucket.Middle, "Too low. {counterPrice} {currency}, or we waste time."),
        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Any", RoundBucket.Final, "I have already yielded enough. {counterPrice} {currency} is my limit."),
        new DialogueTemplate(DialogueAction.REJECT_TOO_LOW, "Any", RoundBucket.Final, "I will not drop beneath {counterPrice} {currency} for this {spiceName}."),

        new DialogueTemplate(DialogueAction.ASK_QUANTITY, "Any", RoundBucket.First, "First tell me, merchant, how much {spiceName} are you offering?"),
        new DialogueTemplate(DialogueAction.ASK_QUANTITY, "Friendly", RoundBucket.Middle, "Name the quantity for me. How much {spiceName} do you mean?"),
        new DialogueTemplate(DialogueAction.ASK_QUANTITY, "Strict", RoundBucket.Middle, "Be exact. State the quantity before we haggle further."),
        new DialogueTemplate(DialogueAction.ASK_QUANTITY, "Impatient", RoundBucket.Middle, "Quantity first. How much?"),
        new DialogueTemplate(DialogueAction.ASK_QUANTITY, "Any", RoundBucket.Final, "We cannot settle anything until the quantity is clear."),

        new DialogueTemplate(DialogueAction.ASK_PRICE, "Any", RoundBucket.First, "For {quantityLabel} of {spiceName}, I ask {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.ASK_PRICE, "Friendly", RoundBucket.Middle, "My present offer is {counterPrice} {currency} for the {spiceName}."),
        new DialogueTemplate(DialogueAction.ASK_PRICE, "Strict", RoundBucket.Middle, "The price stands at {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.ASK_PRICE, "Impatient", RoundBucket.Middle, "{counterPrice} {currency}. That is my offer."),
        new DialogueTemplate(DialogueAction.ASK_PRICE, "Any", RoundBucket.Final, "You know my price already: {counterPrice} {currency}."),
        new DialogueTemplate(DialogueAction.ASK_PRICE, "Any", RoundBucket.Final, "I still stand at {counterPrice} {currency} for {quantityLabel}."),

        new DialogueTemplate(DialogueAction.SOCIAL, "Any", RoundBucket.First, "Well met, merchant. Let us speak of {spiceName} and price."),
        new DialogueTemplate(DialogueAction.SOCIAL, "Friendly", RoundBucket.Middle, "A pleasant word is welcome, but I am still here for {quantityLabel} of {spiceName}."),
        new DialogueTemplate(DialogueAction.SOCIAL, "Strict", RoundBucket.Middle, "Courtesy is enough. Now return to the trade."),
        new DialogueTemplate(DialogueAction.SOCIAL, "Impatient", RoundBucket.Middle, "Pleasantries later. We should finish this bargain."),
        new DialogueTemplate(DialogueAction.SOCIAL, "Any", RoundBucket.Final, "We have spoken enough. Let us settle the trade."),

        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Any", RoundBucket.First, "Let us keep to the trade, merchant."),
        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Friendly", RoundBucket.Middle, "Interesting, perhaps, but I came for {spiceName}, not chatter."),
        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Strict", RoundBucket.Middle, "Stay to the matter at hand. Speak of price or quantity."),
        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Impatient", RoundBucket.Middle, "Off topic again. Return to the bargain."),
        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Any", RoundBucket.Final, "Enough wandering talk. Trade, or let us end this."),
        new DialogueTemplate(DialogueAction.OFF_TOPIC, "Any", RoundBucket.Final, "Speak of {spiceName} and price, not of other things."),

        new DialogueTemplate(DialogueAction.UNKNOWN, "Any", RoundBucket.First, "{ruleReply}"),
        new DialogueTemplate(DialogueAction.UNKNOWN, "Friendly", RoundBucket.Middle, "Say it plainly, merchant. {ruleReply}"),
        new DialogueTemplate(DialogueAction.UNKNOWN, "Strict", RoundBucket.Middle, "Be clear. {ruleReply}"),
        new DialogueTemplate(DialogueAction.UNKNOWN, "Impatient", RoundBucket.Middle, "Speak plainly. {ruleReply}"),
        new DialogueTemplate(DialogueAction.UNKNOWN, "Any", RoundBucket.Final, "{ruleReply}")
    };

    public string GetReply(NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult, int roundCount)
    {
        if (brainResult == null)
        {
            return string.Empty;
        }

        DialogueAction action = DetermineAction(input, trade, brainResult);
        RoundBucket roundBucket = DetermineRoundBucket(roundCount);
        string personality = string.IsNullOrWhiteSpace(trade != null ? trade.buyerPersonality : string.Empty) ? "Any" : trade.buyerPersonality;

        Debug.Log("[DIALOGUE-TABLE] Action: " + action);

        string template = SelectTemplate(action, personality, roundBucket);
        Debug.Log("[DIALOGUE-TABLE] Template selected: " + template);

        string finalReply = ReplacePlaceholders(template, input, trade, brainResult);
        Debug.Log("[DIALOGUE-TABLE] Final reply: " + finalReply);

        return finalReply;
    }

    private static DialogueAction DetermineAction(NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult)
    {
        if (brainResult.isAccepted || string.Equals(brainResult.resolutionAction, "ACCEPT", System.StringComparison.OrdinalIgnoreCase))
        {
            return DialogueAction.ACCEPT;
        }

        if (brainResult.walkedAway || string.Equals(brainResult.resolutionAction, "WALK_AWAY", System.StringComparison.OrdinalIgnoreCase))
        {
            return DialogueAction.REJECT_TOO_LOW;
        }

        switch (input != null ? input.intent : NegotiationIntent.UNKNOWN)
        {
            case NegotiationIntent.QUANTITY_QUERY:
                return DialogueAction.ASK_QUANTITY;

            case NegotiationIntent.PRICE_QUERY:
            case NegotiationIntent.QUERY_BUYER_BUDGET:
                return DialogueAction.ASK_PRICE;

            case NegotiationIntent.GREETING:
            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.GENERAL_DIALOGUE:
                return DialogueAction.SOCIAL;

            case NegotiationIntent.OFF_TOPIC:
            case NegotiationIntent.HOSTILE:
                return DialogueAction.OFF_TOPIC;
        }

        if (trade != null && brainResult.updatedOffer > trade.npcOffer)
        {
            return DialogueAction.COUNTER;
        }

        if (input != null && input.hasSellerPrice && trade != null)
        {
            if (input.sellerPrice > Mathf.Max(trade.maxBuyerPrice, trade.npcOffer))
            {
                return DialogueAction.REJECT_TOO_LOW;
            }

            if (brainResult.updatedOffer <= trade.npcOffer && input.sellerPrice > brainResult.updatedOffer)
            {
                return DialogueAction.REJECT_TOO_LOW;
            }
        }

        if (input != null && (input.intent == NegotiationIntent.COUNTER || input.intent == NegotiationIntent.BARGAIN || input.intent == NegotiationIntent.PRICE || input.intent == NegotiationIntent.QUANTITY_PRICE))
        {
            return brainResult.updatedOffer > (trade != null ? trade.npcOffer : 0)
                ? DialogueAction.COUNTER
                : DialogueAction.REJECT_TOO_LOW;
        }

        return DialogueAction.UNKNOWN;
    }

    private static RoundBucket DetermineRoundBucket(int roundCount)
    {
        if (roundCount <= 1)
        {
            return RoundBucket.First;
        }

        if (roundCount >= 4)
        {
            return RoundBucket.Final;
        }

        return RoundBucket.Middle;
    }

    private static string SelectTemplate(DialogueAction action, string personality, RoundBucket roundBucket)
    {
        List<string> exactMatches = new List<string>();
        List<string> fallbackMatches = new List<string>();

        for (int i = 0; i < Templates.Count; i++)
        {
            DialogueTemplate candidate = Templates[i];
            if (candidate.action != action || candidate.roundBucket != roundBucket)
            {
                continue;
            }

            if (string.Equals(candidate.personality, personality, System.StringComparison.OrdinalIgnoreCase))
            {
                exactMatches.Add(candidate.template);
            }
            else if (candidate.personality == "Any")
            {
                fallbackMatches.Add(candidate.template);
            }
        }

        List<string> options = exactMatches.Count > 0 ? exactMatches : fallbackMatches;
        if (options.Count == 0)
        {
            return "{ruleReply}";
        }

        return options[Random.Range(0, options.Count)];
    }

    private static string ReplacePlaceholders(string template, NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult)
    {
        string buyerName = trade != null && !string.IsNullOrWhiteSpace(trade.buyerName) ? trade.buyerName : "Customer";
        string spiceName = trade != null && !string.IsNullOrWhiteSpace(trade.spiceDisplayName) ? trade.spiceDisplayName.ToLowerInvariant() : "spice";
        string quantityLabel = trade != null && !string.IsNullOrWhiteSpace(trade.quantityLabel) ? trade.quantityLabel : "this lot";
        int offeredPrice = input != null && input.hasSellerPrice ? input.sellerPrice : (trade != null ? trade.lastSellerPrice : 0);
        int counterPrice = brainResult != null && brainResult.updatedOffer > 0 ? brainResult.updatedOffer : (trade != null ? trade.npcOffer : 0);
        int finalPrice = brainResult != null && brainResult.resolvedPrice > 0 ? brainResult.resolvedPrice : counterPrice;
        string currency = "varahas";
        string ruleReply = brainResult != null && !string.IsNullOrWhiteSpace(brainResult.replyText) ? brainResult.replyText : "Speak plainly, merchant.";

        return template
            .Replace("{buyerName}", buyerName)
            .Replace("{spiceName}", spiceName)
            .Replace("{quantityLabel}", quantityLabel)
            .Replace("{offeredPrice}", offeredPrice.ToString())
            .Replace("{counterPrice}", counterPrice.ToString())
            .Replace("{finalPrice}", finalPrice.ToString())
            .Replace("{currency}", currency)
            .Replace("{ruleReply}", ruleReply);
    }
}
