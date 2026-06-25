using System.Collections.Generic;

public static class SiddharthChettiDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "saraswati_chetti",
            "Saraswati Chetti",
            NpcPersonalityBucket.Friendly,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Namaskara, merchant. I am looking for good {spiceName} for my shop."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "Namaskara again. If the spice is good, I will buy from you happily."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I need {quantityLabel} of {spiceName} for trade and temple customers."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can offer {currentBuyerOffer} {currency}. My family keeps careful trade accounts."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for my shop shelves."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for me."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "The price is a little high. Can you bring it down with care?"
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high. Good quality also needs a fair rate."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "No, that is too much. I know what fine spice should cost in this bazaar."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "The spice is good, but make it {counterPrice} {currency}."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair. I accept with a clear mind."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That price is better than I expected. Good."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "I can offer {counterPrice} {currency} for {spiceName}. I must protect my family trade."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency} for {spiceName}. Good quality deserves fairness."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase a little, to {counterPrice} {currency}. I am trying to meet you well."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency}. Good trade should keep its dignity."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase to {counterPrice} {currency}. Please do not test my patience further."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. That is as far as I can go with honour."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I have come as far as I can with respect."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. Now let us finish this properly."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I cannot go higher than that."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. Quality must still leave room for fairness."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That still keeps honour in the trade."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. Quality and price must balance."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. I will not move from my word."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. We settle at {finalPrice} {currency}. My family name is safe on this."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then we have no deal today. I do not trade without balance."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Hampi's temples and markets teach many things, but first let us finish this trade."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "You speak well. Now let us see if the spice and the price both have honour."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us stay with the spice, merchant. Trade also has its discipline."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I did not understand properly. Please speak clearly. {ruleReply}"
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Good trade. My family name will be safe on this purchase."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will look at another stall with better balance of price and quality."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave it for now. Perhaps another merchant will match the quality."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will go. We are not agreeing on a worthy bargain today."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "No deal. Reputation also has a price, and this is not it."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I cannot wait too long. Shop work and temple orders are waiting."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Please decide soon. I still have shop work and temple orders waiting."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide now, merchant. The market is moving and I must return."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Enough waiting. Decide now or I will take my business elsewhere."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Please say it clearly. {ruleReply}"
                }, "saraswati_chetti")
            });
    }
}
