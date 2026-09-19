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
                    "Lakshmi Amma has come for {quantityLabel} of {spiceName}, merchant. Let us keep this practical.",
                    "Good day. I need {quantityLabel} of {spiceName} for the house jars and kitchen shelf.",
                    "I have come for {quantityLabel} of {spiceName}. Let us speak sensibly, as market folk do.",
                    "Merchant, I need {spiceName} for the house and the hearth.",
                    "I seek {quantityLabel} of {spiceName} today. No grand show, only fair trade.",
                    "Good day to you. I have come to buy {spiceName} in proper measure for the family."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We have met before, merchant. Let us settle this neatly today.",
                    "Back again, am I? Then let us get to the price.",
                    "We know each other now. Let us not circle around the matter.",
                    "I have returned, merchant. Let us finish this cleanly.",
                    "Here I am again. This time, let us settle it faster.",
                    "We have bargained before. Let us be practical today."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I need {quantityLabel} of {spiceName} for cooking and the household stores.",
                    "The {spiceName} is what I came for, enough for the family and feast days.",
                    "I want {quantityLabel} of {spiceName} for the jars at home.",
                    "It is {spiceName} I seek, for daily cooking and the better meals.",
                    "I came for {spiceName}, enough for the house and perhaps a temple feast or two.",
                    "My need is simple: {quantityLabel} of {spiceName}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can offer {currentBuyerOffer} {currency} and still keep my accounts balanced.",
                    "My purse allows {currentBuyerOffer} {currency} for this purchase.",
                    "I can spare {currentBuyerOffer} {currency} for this lot.",
                    "My working amount is {currentBuyerOffer} {currency}.",
                    "For this purchase, I can pay {currentBuyerOffer} {currency}.",
                    "{currentBuyerOffer} {currency} is the most sensible figure for me."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for the house.",
                    "For my kitchen, {quantityLabel} of {spiceName} will do.",
                    "My measure is {quantityLabel} of {spiceName}, merchant.",
                    "I want {quantityLabel} of {spiceName} for the family stores.",
                    "Set aside {quantityLabel} of {spiceName} for me.",
                    "My household needs {quantityLabel} of {spiceName}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is too much for a careful household buyer.",
                    "That price is too high for my kitchen accounts.",
                    "No, merchant, that is more than I can sensibly pay.",
                    "That figure is too heavy for household stores.",
                    "You ask too much for a buyer who counts every coin.",
                    "I cannot give {offeredPrice} {currency}. That is too steep."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, though I would rather settle kindly.",
                    "You ask too much, merchant, but we may still find a fair path.",
                    "{offeredPrice} {currency} is too high for me, friend, though I am willing to keep talking."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for my household accounts.",
                    "{offeredPrice} {currency} is beyond what I can reasonably pay.",
                    "No, friend, that figure is too much for me."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, merchant, and I cannot stand here all day.",
                    "{offeredPrice} {currency} is too much. Speak more briskly now.",
                    "You ask too high a price, and my errands still wait on me."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask a little too much. I could manage {counterPrice} {currency}.",
                    "Come down a touch, and {counterPrice} {currency} would be fair.",
                    "That is near enough, but still high. I can do {counterPrice} {currency}.",
                    "Lower it a little, and we may settle at {counterPrice} {currency}.",
                    "You are close, merchant. {counterPrice} {currency} is more sensible.",
                    "A small step down would help. I can offer {counterPrice} {currency}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, friend. I could settle at {counterPrice} {currency}.",
                    "Your price is near enough. Let us meet at {counterPrice} {currency}.",
                    "A gentle step lower to {counterPrice} {currency} would suit us both."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You are close, merchant. I can do {counterPrice} {currency}.",
                    "{counterPrice} {currency} is the more practical figure for me.",
                    "Come a little lower, friend, and {counterPrice} {currency} will do."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, but let us be quick. {counterPrice} {currency}.",
                    "A quicker ending would be {counterPrice} {currency}, merchant.",
                    "I can do {counterPrice} {currency}, but I must not linger longer."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is sensible. I can agree to it.",
                    "Very well, that price suits me.",
                    "Yes, that seems fair enough.",
                    "That figure will do.",
                    "I can accept that without complaint.",
                    "Good. That is a workable price."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is kinder than I expected, merchant.",
                    "You have named a lower figure than I feared. Good.",
                    "That is less than I expected to hear.",
                    "You surprise me. That price is quite reasonable.",
                    "That figure sits lower than I had prepared for.",
                    "Ah, that is better than I expected."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is kindly said, friend. I will remember it well.",
                    "You ask less than expected. That gladdens a careful buyer.",
                    "That is a warm surprise, merchant."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than I expected, and welcome.",
                    "A fair surprise, friend. That is easier to manage.",
                    "You speak more reasonably than I expected."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Good. Let us settle it quickly.",
                    "A welcome price, friend. It saves us both more delay.",
                    "You speak better than I feared, merchant. Let us finish this now."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "Let me begin at {counterPrice} {currency}.",
                    "My first offer back to you is {counterPrice} {currency}.",
                    "I will start at {counterPrice} {currency}.",
                    "My opening counter is {counterPrice} {currency}.",
                    "Let us begin sensibly at {counterPrice} {currency}.",
                    "{counterPrice} {currency} is my first answer to your price."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are getting closer. I can offer {counterPrice} {currency}.",
                    "I will stretch to {counterPrice} {currency}, but carefully.",
                    "We are nearer now. {counterPrice} {currency} is my next step.",
                    "I can come to {counterPrice} {currency} if we finish this well.",
                    "That is closer to fair. I offer {counterPrice} {currency}.",
                    "I will move a little more, to {counterPrice} {currency}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "In goodwill, I can offer {counterPrice} {currency}.",
                    "Let us keep this warm and fair. {counterPrice} {currency}.",
                    "I will come to {counterPrice} {currency}, friend, so we may finish kindly."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are drawing near. I can offer {counterPrice} {currency}.",
                    "{counterPrice} {currency} is my next sensible step.",
                    "I move to {counterPrice} {currency}, merchant."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can offer {counterPrice} {currency}, but we should not drag this out.",
                    "{counterPrice} {currency} is my next step, friend. My work is waiting.",
                    "Let us be quick now. I can come to {counterPrice} {currency}."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is the furthest I can go today.",
                    "This is my last step, merchant: {counterPrice} {currency}.",
                    "I can go no higher than {counterPrice} {currency}.",
                    "That is my final figure, {counterPrice} {currency}.",
                    "No more from me. {counterPrice} {currency} must be enough.",
                    "My last word on this is {counterPrice} {currency}."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "This is my last step, given in goodwill: {counterPrice} {currency}.",
                    "{counterPrice} {currency} is as far as I can go and still feel content.",
                    "Friend, I end at {counterPrice} {currency}. Let us part well if we can."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final figure.",
                    "I stop at {counterPrice} {currency}, merchant.",
                    "This is my last offer, friend: {counterPrice} {currency}."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my last step, and I must soon be on my way.",
                    "I end at {counterPrice} {currency}, friend. The house still waits on me.",
                    "This is my last offer: {counterPrice} {currency}. Decide quickly now."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}.",
                    "My offer holds at {currentBuyerOffer} {currency}; I cannot spare more.",
                    "No, merchant. I stay at {currentBuyerOffer} {currency}.",
                    "That is where my offer rests: {currentBuyerOffer} {currency}.",
                    "I will not move past {currentBuyerOffer} {currency}.",
                    "{currentBuyerOffer} {currency} is my limit today."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, though I speak to you kindly, friend.",
                    "{currentBuyerOffer} {currency} is my limit, but I hope you see the fairness in it.",
                    "My offer remains {currentBuyerOffer} {currency}. I would still rather part well."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}, merchant.",
                    "{currentBuyerOffer} {currency} remains my sensible limit.",
                    "No further, friend. {currentBuyerOffer} {currency} is where I stand."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, and my errands still call me away.",
                    "{currentBuyerOffer} {currency} is my limit, friend. Let us not linger longer.",
                    "No more from me, merchant. {currentBuyerOffer} {currency}, and we must finish."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. Then we are agreed at {finalPrice} {currency}.",
                    "Excellent. Let us finish the purchase at {finalPrice} {currency}.",
                    "Very good. We settle at {finalPrice} {currency}.",
                    "Then the bargain is done at {finalPrice} {currency}.",
                    "Agreed. Let us close it at {finalPrice} {currency}.",
                    "Good. That price, {finalPrice} {currency}, will do nicely."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "A pity. Then my offer does not suit you.",
                    "So we cannot settle it today.",
                    "Then you do not care for my terms.",
                    "Ah well, then the bargain does not hold.",
                    "So be it. My offer is not enough for you.",
                    "Then we part without agreement."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "The old city has many stories, but I still must buy my spices.",
                    "We can speak of temple feasts and market days later. First, the bargain.",
                    "There are many stories in Hampi, but my kitchen still needs filling.",
                    "Talk of the city can wait. First let us settle the spice.",
                    "History is fine, merchant, but the household must be fed before stories are told.",
                    "We may speak of feasts later. For now, let us bargain."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Warm words are good, but I still need a sensible price.",
                    "You are kind, merchant. Now tell me what we can settle.",
                    "A friendly word is welcome. A fair price is better.",
                    "You speak well. Now let us come to the matter.",
                    "Kindness is good, but I still have buying to do.",
                    "Well met, merchant. Now speak plainly on price."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "I need clearer words than that. {ruleReply}",
                    "Say it more plainly, merchant. {ruleReply}",
                    "I am not sure what you mean. {ruleReply}",
                    "Your meaning is not clear to me. {ruleReply}",
                    "Speak more directly, if you please. {ruleReply}",
                    "I need a clearer answer. {ruleReply}"
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us speak of the household purchase, not of other matters.",
                    "My mind is on spices and provisions, merchant.",
                    "Let us stay with the buying, not wander elsewhere.",
                    "I came for provisions, not stray talk.",
                    "Speak of the spice, merchant, not of other things.",
                    "My thoughts are on the kitchen and the stores, nothing else."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I could not make that out clearly. {ruleReply}",
                    "Speak a little more plainly, merchant. {ruleReply}",
                    "I did not catch that properly. {ruleReply}",
                    "The market noise swallowed your words. {ruleReply}",
                    "Please say that again more clearly. {ruleReply}",
                    "I missed your meaning just then. {ruleReply}"
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Good trade. The household will make fine use of this.",
                    "Thank you. This purchase is settled well.",
                    "Good. This will serve the family and the kitchen nicely.",
                    "A fair purchase. I am satisfied.",
                    "Well done, merchant. This trade suits a careful home.",
                    "That is settled well. My thanks."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I will leave without buying today.",
                    "No agreement, then. Perhaps another market day.",
                    "Then there will be no purchase from me today.",
                    "So we part without a bargain.",
                    "Very well. I will look elsewhere.",
                    "No sale today, it seems."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without trade, friend, though I still wish you well.",
                    "No bargain today, yet I would rather leave on good terms.",
                    "So be it. Another day may treat us more kindly."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement today.",
                    "No purchase this time, merchant.",
                    "So be it, friend. The bargain does not close."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I must part now, friend. The house still waits on me.",
                    "No bargain today, and I must return to my errands.",
                    "So be it, merchant. Time has thinned, and we still have no trade."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I cannot stand here all day, merchant. Decide soon.",
                    "The kitchen waits on me. Let us finish this quickly.",
                    "I have other errands to tend. Speak your answer.",
                    "Come now, merchant. I cannot linger much longer.",
                    "My work is waiting. Let us settle this.",
                    "Time is passing, and I still have a household to run."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Merchant, my errands are piling up. Decide now.",
                    "The kitchen and the house both wait on me. Let us finish quickly.",
                    "My time is nearly spent, friend. Speak your answer at once."
                }, "lakshmi_amma", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly)
            });
    }
}
