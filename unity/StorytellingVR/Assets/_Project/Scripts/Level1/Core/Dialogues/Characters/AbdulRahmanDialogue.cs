using System.Collections.Generic;

public static class AbdulRahmanDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "abdul_rahman",
            "Abdul Rahman",
            NpcPersonalityBucket.Friendly,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Peace upon you, merchant. I am Abdul Rahman, seeking {quantityLabel} of {spiceName} for my caravan.",
                    "Greetings, merchant. Abdul Rahman has come along the trade road for {quantityLabel} of {spiceName}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We know each other already, friend. Let us return to the spice and the price."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "It is the {spiceName} I seek, enough for {quantityLabel} on the onward road."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "For a fair journey's trade, I can offer {currentBuyerOffer} {currency}.",
                    "My purse opens to {currentBuyerOffer} {currency} for this lot."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "Tell me the measure, friend. How much {spiceName} stands before us?"
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is too steep even for the long caravan road.",
                    "That price rides too high for my accounts, merchant."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask a little above my comfort. I can come nearer to {counterPrice} {currency}.",
                    "Let us meet more gently. My best step is {counterPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair by the road and by the scales. I accept.",
                    "Your price can be honoured. Let us conclude it well."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "You speak lower than I expected, merchant. Let us weigh it carefully."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "My first counter is {counterPrice} {currency}, offered in good faith.",
                    "Let us begin at {counterPrice} {currency} and see if we can walk the same road."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We draw closer now. I can offer {counterPrice} {currency}.",
                    "The road between us shortens. {counterPrice} {currency} is my next step."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "Friend, this is the last pace I can take: {counterPrice} {currency}.",
                    "My purse reaches almost no further. {counterPrice} {currency} is my final road."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}; beyond that, the journey stops making sense.",
                    "My offer stays where it is, merchant: {currentBuyerOffer} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Excellent. We are agreed at {finalPrice} {currency}.",
                    "Then let the bargain be sealed at {finalPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "A pity. Then my offer does not satisfy you.",
                    "So the road between our prices remains too wide."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Trade routes carry many stories, but first we must settle this spice."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Your courtesy honours the market. Now let us return to {spiceName}.",
                    "Well spoken, merchant. Shall we finish the bargain now?"
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us stay with the trade, friend. The caravan waits for no wandering talk.",
                    "My mind is on {spiceName}, not on stray subjects."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "The market noise hides your meaning. {ruleReply}",
                    "I did not hear you cleanly above the bazaar. {ruleReply}"
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "A good trade for both of us. May the road favour your stall."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement, though I bear no ill will.",
                    "No sale today, it seems. Perhaps another market day."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "My caravan cannot wait much longer. Decide soon, merchant.",
                    "Time presses on the road. We must conclude this quickly."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "{ruleReply}"
                }, "abdul_rahman")
            });
    }
}
