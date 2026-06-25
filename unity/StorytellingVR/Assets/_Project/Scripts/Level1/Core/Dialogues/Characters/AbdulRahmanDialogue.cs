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
                    "Peace be upon you, merchant. I came for Vijayanagara spices from the western ports."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "Peace be upon you again. Let us trade fairly, as men of the road should."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I need {quantityLabel} of {spiceName} for the next caravan and port journey."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can offer {currentBuyerOffer} {currency}. My journey has already cost many coins."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for the next road from port to caravan."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for me."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "Friend, the spice is good, but my journey has already cost many coins."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "I have traded in Calicut and Goa. This price is higher than expected."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "Do not mistake me for a new trader. I know the value of these spices."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "A little high. Make it {counterPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair. I accept it as a good trader would."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That price is better than I expected. Good."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "I can offer {counterPrice} {currency} for {spiceName}. That is fair from a road trader."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency} for {spiceName}. Sea and road have taught me this value."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase a little, to {counterPrice} {currency}. Let us keep this honourable."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency}. I have traded in many ports and know a fair step."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase to {counterPrice} {currency}. Do not test a seasoned trader too much."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. That is my last port-side figure."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I have already walked far to meet you."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. This is a fair port-side bargain."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I will not go higher than this."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. I know what this spice brings across the routes."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That still leaves honour in the trade."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. I know what these spices fetch across the sea."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. My word on this is finished."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. We settle at {finalPrice} {currency}. This trade can travel with honour."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then we have no deal today. Many markets wait beyond this one."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "I have seen Calicut, Goa, and many roads besides. Still, first let us finish this trade."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "You speak kindly, merchant. Let us make a fair bargain worthy of travellers and traders."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us stay with the trade, friend. The caravan road gives no time for wandering words."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I did not hear you clearly over the bazaar. {ruleReply}"
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Good trade. I will remember this stall when I next return by sea and road."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave without buying today and carry my coins to another stall."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave it for now. Perhaps another merchant will meet me better."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will go. We are not agreeing like men of fair trade today."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "No deal. I have crossed too many markets to accept this."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "My caravan cannot wait much longer. The port road is calling me."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Please decide soon. My companions and cargo are waiting."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide now, merchant. The road and the port do not wait for us."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Enough waiting. Decide now or I will take my trade elsewhere."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Please speak plainly. {ruleReply}"
                }, "abdul_rahman")
            });
    }
}
