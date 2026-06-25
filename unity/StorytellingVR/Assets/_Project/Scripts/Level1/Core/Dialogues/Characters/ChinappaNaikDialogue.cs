using System.Collections.Generic;

public static class ChinappaNaikDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "chinnamma_naik",
            "Chinnamma Naik",
            NpcPersonalityBucket.Strict,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Namaskara, merchant. I came for {quantityLabel} of {spiceName}. Let us talk straight."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We meet again. Good. Then no wasting time on market tricks."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I want {quantityLabel} of {spiceName} for trade."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can offer {currentBuyerOffer} {currency}. That is a sound rate."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for trade accounts."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is a little high. Reduce it and let us finish."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high. I know the market better than that."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "No, that is too much. Do not try to fool me on spice rates."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "A little high. Make it {counterPrice} {currency}."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair. We can close the account."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "Hmm. That price is better than I expected."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "I can offer {counterPrice} {currency} for {spiceName}. That is a trader's rate."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency} for {spiceName}. No higher without reason."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase a little, to {counterPrice} {currency}. I am being reasonable."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency}. That is proper market dealing."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase to {counterPrice} {currency}. Do not mistake firmness for weakness."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. That is my last market word."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I have already come enough."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. Now decide like a trader."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I will not raise it again."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That is the proper rate."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That is already a fair rate."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. I know the value of this spice."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That is final. No more games."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. We settle at {finalPrice} {currency}. Let the account stand clean."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then there is no deal. I do not chase unsteady bargains."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Hampi's markets are old and wise. Still, let us finish this trade first."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Good. Now let us come to the rate without extra sweetness."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Stay with the trade, merchant. I did not come for loose talk."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "Speak clearly. I do not bargain with half-heard words. {ruleReply}"
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Good trade. That is how proper business is done in this bazaar."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will take my business to a sharper stall."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave it for now. Another stall may speak more wisely."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will go. We are not agreeing on a true market price today."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "No deal. I have no time left for crooked pricing."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Do not delay. Market time and money both are moving."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide soon, merchant. I still have accounts waiting."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide now. The market is moving and I will move with it."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Enough delay. Answer now or I will close this bargain myself."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Say it plainly. {ruleReply}"
                }, "chinnamma_naik")
            });
    }
}
