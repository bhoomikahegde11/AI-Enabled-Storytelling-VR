using System.Collections.Generic;

public static class LakshmiAmmaDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "lakshmi_amma",
            "Lakshmi Amma",
            NpcPersonalityBucket.Friendly,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Namaskara, merchant. I need some {spiceName} for the house today."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "Ayyo, we meet again. Come, let us settle this like market people."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I need {quantityLabel} of {spiceName} for cooking and festival dishes."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "For my household money, I can offer {currentBuyerOffer} {currency}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for my kitchen jars."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "Ayyo, that price is too high for me."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "The price is a little high, child. Reduce it a bit."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high. Give me a better market rate."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "No, no. That is too much. I run a house, not a treasury."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "Hmm, a little costly. Make it {counterPrice} {currency}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair. I will take it for the house."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "Ah, that price is kinder than I expected."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "I can offer {counterPrice} {currency} for {spiceName} from my household money."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency} for {spiceName} for my house use."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase a little, to {counterPrice} {currency}. Let us keep it easy."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} {currency}. That is a fair household price."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase to {counterPrice} {currency}. Do not stretch me more, merchant."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. That is my kitchen limit."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I am giving my clean best."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. Now let us decide properly."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} {currency}. I cannot go one coin higher."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That is my household limit."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. That is what my household can manage."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. I know today's market well."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}. Ayyo, do not press me again."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. We settle at {finalPrice} {currency}. My kitchen will bless it."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then we have no deal now. I cannot carry empty kitchen talk home."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Hampi has many stories, but first let me finish buying for the house."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "You speak nicely, merchant. Now give me a fair bazaar price."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Come, come, let us talk about the spice and the price."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "Ayyo, I did not catch that clearly. {ruleReply}"
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Good trade. This will go straight to my kitchen jars."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave it for today and try another bazaar stall."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave it for now. Maybe another market day."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will go. We are not agreeing like sensible people today."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "No deal. I have wasted enough market time here."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I cannot wait too long, merchant. The cooking and errands are waiting."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Please decide soon. I still have cooking and errands waiting."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Low),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide now, merchant. The market day will not stand still."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.Medium),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Enough waiting. Decide now or I will walk to the next stall."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly, NpcAggressionBucket.High),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Please say it clearly. {ruleReply}"
                }, "lakshmi_amma")
            });
    }
}
