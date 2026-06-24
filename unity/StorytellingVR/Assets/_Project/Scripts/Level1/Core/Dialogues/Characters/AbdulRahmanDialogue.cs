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
                    "Peace upon you, merchant. I am Abdul Rahman, seeking {quantityLabel} of {spiceName} for the caravan road.",
                    "Greetings, merchant. Abdul Rahman has come by the long trade route for {quantityLabel} of {spiceName}.",
                    "Peace to your stall. I seek {quantityLabel} of {spiceName} before the caravan moves on.",
                    "Well met, merchant. My caravan requires {spiceName} in honest measure.",
                    "I have come far with the dust of the road on me for {spiceName}. Let us trade fairly.",
                    "Abdul Rahman stands before you for {quantityLabel} of {spiceName}, hoping for a worthy bargain."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We know each other already, friend. Let us return to the spice and the price.",
                    "We meet again, merchant. May this bargain be as fair as the last.",
                    "It is good to return to a familiar stall. Let us speak of trade.",
                    "Again our roads cross, friend. Let us settle the matter well.",
                    "We have bargained before. Let us resume with clear minds.",
                    "I return in trust, merchant. Now let us speak of {spiceName} and price."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "It is the {spiceName} I seek, enough for {quantityLabel} on the onward road.",
                    "My need is {spiceName}, measured at {quantityLabel} for the next caravan stop.",
                    "I came for {quantityLabel} of {spiceName}, if the quality is as honest as your scales.",
                    "It is {spiceName} I seek, friend, and in proper measure for long miles ahead.",
                    "The road calls for {spiceName}; I require {quantityLabel}.",
                    "I seek {quantityLabel} of {spiceName} for trade along the next route east."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "For a fair journey's trade, I can offer {currentBuyerOffer} {currency}.",
                    "My purse opens to {currentBuyerOffer} {currency} for this lot.",
                    "I can lay down {currentBuyerOffer} {currency} in good faith.",
                    "My present offer stands at {currentBuyerOffer} {currency}.",
                    "For this quantity, {currentBuyerOffer} {currency} is a fair purse.",
                    "You may take {currentBuyerOffer} {currency} as my honest offer."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for the caravan road.",
                    "My measure is {quantityLabel} of {spiceName}.",
                    "For this journey, I seek {quantityLabel} of {spiceName}.",
                    "Let it be {quantityLabel} of {spiceName}, friend.",
                    "My order is {quantityLabel} of {spiceName}.",
                    "I ask for {quantityLabel} of {spiceName} before I ride on."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is too steep even for the long caravan road.",
                    "That price rides too high for my accounts, merchant.",
                    "Friend, that figure is too high for fair dealing.",
                    "No, merchant, that price burdens the journey too heavily.",
                    "That demand is too rich for a road trader's purse.",
                    "I cannot follow you to {offeredPrice} {currency}. The price is too high.",
                    "Such a price leaves no wisdom in the bargain."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, friend, though I would still trade fairly.",
                    "You ask too much, yet I would rather mend the bargain than lose it.",
                    "{offeredPrice} {currency} is too high for me, but we may still find a road together."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for my purse, merchant.",
                    "{offeredPrice} {currency} is beyond what I can call fair.",
                    "Friend, you ask more than I can reasonably give."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, and the caravan will not wait forever.",
                    "{offeredPrice} {currency} asks too much of me, friend. Speak more quickly now.",
                    "You stand too high, merchant, and my time on the road grows short."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask a little above my comfort. I can come nearer to {counterPrice} {currency}.",
                    "Let us meet more gently. My best step is {counterPrice} {currency}.",
                    "You stand a little high, friend. I can offer {counterPrice} {currency}.",
                    "Come a little closer to me, and {counterPrice} {currency} may settle it.",
                    "That is near enough, but still above my reach. {counterPrice} {currency} is better.",
                    "A small step down would help. I can move to {counterPrice} {currency}.",
                    "We are not far apart. {counterPrice} {currency} would honour both of us."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, friend. I can come to {counterPrice} {currency} in goodwill.",
                    "Your price is near mine. Let us settle at {counterPrice} {currency}.",
                    "A gentle step toward {counterPrice} {currency} would keep this trade warm."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You are close enough, merchant. I can offer {counterPrice} {currency}.",
                    "{counterPrice} {currency} is the fairer meeting point.",
                    "Come a little lower, friend, and {counterPrice} {currency} will do."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, but let us not linger. {counterPrice} {currency}.",
                    "A quicker agreement would be {counterPrice} {currency}, friend.",
                    "I can come to {counterPrice} {currency}, but the caravan is already stirring."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is fair by the road and by the scales. I accept.",
                    "Your price can be honoured. Let us conclude it well.",
                    "Yes, friend, that is a fair bargain.",
                    "That price sits well with me.",
                    "I can accept that with an easy mind.",
                    "The scales and the purse agree. Let us proceed.",
                    "Very well. That figure respects the trade."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "You speak lower than I expected, merchant. Let us weigh it carefully.",
                    "That is a gentler figure than I had prepared for.",
                    "You surprise me, friend. The price is lower than expected.",
                    "That number comes kindly from your lips.",
                    "You ask less than I feared for this {spiceName}.",
                    "That is below my expectation, and welcome besides."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is kindly spoken, friend. It gives this bargain a good beginning.",
                    "You ask less than expected. I will remember such fairness.",
                    "That price falls gently on the ear, merchant."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than I expected, and welcome.",
                    "A fair surprise, friend. That price is easier to meet.",
                    "You speak more reasonably than I expected."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Good. Let us finish this while we may.",
                    "A welcome price, friend. It saves us both more delay.",
                    "You speak better than I feared, merchant. Let us settle quickly."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "My first counter is {counterPrice} {currency}, offered in good faith.",
                    "Let us begin at {counterPrice} {currency} and see if we can walk the same road.",
                    "I open with {counterPrice} {currency}, friend.",
                    "My first answer is {counterPrice} {currency} for this lot.",
                    "Let the bargaining start at {counterPrice} {currency}.",
                    "I place {counterPrice} {currency} before you in fairness.",
                    "We begin at {counterPrice} {currency}, and perhaps meet on the road between."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We draw closer now. I can offer {counterPrice} {currency}.",
                    "The road between us shortens. {counterPrice} {currency} is my next step.",
                    "I can move to {counterPrice} {currency}, and no insult is meant.",
                    "That is nearer, friend. My next offer is {counterPrice} {currency}.",
                    "Let me come forward to {counterPrice} {currency}.",
                    "We approach agreement. I offer {counterPrice} {currency}.",
                    "I will step a little more, to {counterPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "In goodwill, I can offer {counterPrice} {currency}.",
                    "Let us keep the road between us short. {counterPrice} {currency}.",
                    "I will come to {counterPrice} {currency}, friend, so the trade may live."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are drawing near. I can offer {counterPrice} {currency}.",
                    "{counterPrice} {currency} is my next honest step.",
                    "I move to {counterPrice} {currency}, merchant."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can offer {counterPrice} {currency}, but we should not delay much longer.",
                    "{counterPrice} {currency} is my next step, friend. The road is calling.",
                    "Let us be quick now. I can come to {counterPrice} {currency}."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "Friend, this is the last pace I can take: {counterPrice} {currency}.",
                    "My purse reaches almost no further. {counterPrice} {currency} is my final road.",
                    "This is my final figure, {counterPrice} {currency}.",
                    "I can go no further than {counterPrice} {currency}, merchant.",
                    "My last word on price is {counterPrice} {currency}.",
                    "No more steps remain in me. {counterPrice} {currency} is final.",
                    "Take this as my last offer: {counterPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "This is my final step, offered in goodwill: {counterPrice} {currency}.",
                    "{counterPrice} {currency} is as far as I can go and still trade with honour.",
                    "Friend, I end at {counterPrice} {currency}. Let us keep good feeling between us."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final figure.",
                    "I stop at {counterPrice} {currency}, merchant.",
                    "This is my last offer, friend: {counterPrice} {currency}."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final road, and I must soon take it.",
                    "I end at {counterPrice} {currency}, friend. The caravan cannot wait longer.",
                    "This is my last offer: {counterPrice} {currency}. Decide quickly now."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}; beyond that, the journey stops making sense.",
                    "My offer stays where it is, merchant: {currentBuyerOffer} {currency}.",
                    "I remain at {currentBuyerOffer} {currency}, friend.",
                    "My purse will not pass {currentBuyerOffer} {currency}.",
                    "No, merchant. {currentBuyerOffer} {currency} is where I must stand.",
                    "I hold firm at {currentBuyerOffer} {currency}.",
                    "Trust me, friend, {currentBuyerOffer} {currency} is my true limit."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, yet I do so with respect, friend.",
                    "{currentBuyerOffer} {currency} is my limit, though I would still part well with you.",
                    "My offer remains {currentBuyerOffer} {currency}. I hope you see the fairness in it."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}, merchant.",
                    "{currentBuyerOffer} {currency} remains my honest limit.",
                    "No further, friend. {currentBuyerOffer} {currency} is where I stand."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, and the caravan is already moving in my mind.",
                    "{currentBuyerOffer} {currency} is my limit, friend. Let us not tarry longer.",
                    "No more from me, merchant. {currentBuyerOffer} {currency}, and we must finish."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Excellent. We are agreed at {finalPrice} {currency}.",
                    "Then let the bargain be sealed at {finalPrice} {currency}.",
                    "Good. Let us settle at {finalPrice} {currency} in honour.",
                    "Then the trade is made at {finalPrice} {currency}.",
                    "We are of one mind now: {finalPrice} {currency}.",
                    "Agreed, friend. {finalPrice} {currency} shall close it.",
                    "A fair ending. We conclude at {finalPrice} {currency}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "A pity. Then my offer does not satisfy you.",
                    "So the road between our prices remains too wide.",
                    "Then we do not yet meet, friend.",
                    "I see my offer does not reach your heart.",
                    "So be it. Our prices still stand apart.",
                    "Then the bargain remains unfinished.",
                    "That is unfortunate. We are not yet of one price."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Trade routes carry many stories, but first we must settle this spice.",
                    "The caravan road is full of tales, yet the bargain comes first.",
                    "I have seen ports, deserts, and many markets, friend, but first let us finish this trade.",
                    "There is time for talk of far roads later. Now we weigh the spice.",
                    "Stories travel with the caravan, but so do debts and promises.",
                    "Ask of distant roads another time. First, let us settle the price."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Your courtesy honours the market. Now let us return to {spiceName}.",
                    "Well spoken, merchant. Shall we finish the bargain now?",
                    "Kind words travel well, friend. So does fair dealing.",
                    "You speak with grace. Let us also bargain with grace.",
                    "Your manners please me, merchant. Now let us come to terms.",
                    "A courteous word is welcome. A fair price is better.",
                    "Well met, friend. Let us return to the business of {spiceName}."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us stay with the trade, friend. The caravan waits for no wandering talk.",
                    "My mind is on {spiceName}, not on stray subjects.",
                    "Let us keep our words upon the bargain, merchant.",
                    "The road is long, friend, and I must stay with the trade.",
                    "Speak of the spice and the price, not of other matters.",
                    "My thoughts are on business, not wandering talk.",
                    "Let us return to {spiceName}; the caravan has no time for distractions."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "The market noise hides your meaning. {ruleReply}",
                    "I did not hear you cleanly above the bazaar. {ruleReply}",
                    "Your words were lost in the crowd, friend. {ruleReply}",
                    "Say it again more clearly, merchant. {ruleReply}",
                    "The bazaar swallowed your words. {ruleReply}",
                    "I missed your meaning just then. {ruleReply}",
                    "Speak plainly once more, friend. {ruleReply}"
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "A good trade for both of us. May the road favour your stall.",
                    "This bargain honours us both, friend.",
                    "Well done. May trust bring us together when the caravan returns.",
                    "A fair trade. I will remember your stall on the next journey.",
                    "Good business, merchant. May your scales stay honest and your name travel well.",
                    "We part well satisfied. May fortune follow your market days and my road alike."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement, though I bear no ill will.",
                    "No sale today, it seems. Perhaps another market day.",
                    "Then we go our separate ways without anger.",
                    "So be it, friend. Today gives us no bargain.",
                    "No accord today, though I wish you well.",
                    "We part without trade, but not without respect.",
                    "Then let us leave it for another day and another road."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without trade, friend, though I still wish you well.",
                    "No bargain today, yet I would leave as friends.",
                    "So be it. Another day may bring us together again."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement today.",
                    "No trade this time, merchant.",
                    "So be it, friend. The bargain does not close."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we must part now, friend. The caravan cannot linger.",
                    "No bargain today, and I must return to the road.",
                    "So be it, merchant. Time has thinned, and we still have no trade."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "My caravan cannot wait much longer. Decide soon, merchant.",
                    "Time presses on the road. We must conclude this quickly.",
                    "Friend, the caravan stirs. I need your answer soon.",
                    "The road calls to me. Let us finish this now.",
                    "I cannot linger much longer, merchant. Speak your decision.",
                    "My companions will not wait all day. Let us conclude.",
                    "Time grows short for a traveller. Decide, friend."
                }, "abdul_rahman"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Friend, the caravan is already growing restless. Decide now.",
                    "My time is nearly spent, merchant. Speak your answer at once.",
                    "The road will not wait for us much longer. Let us finish quickly."
                }, "abdul_rahman", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "{ruleReply}",
                    "Speak plainly, friend. {ruleReply}",
                    "Your meaning is still not clear to me. {ruleReply}",
                    "Let us be direct, merchant. {ruleReply}",
                    "I would hear your intent more clearly. {ruleReply}",
                    "Say it in simpler words, friend. {ruleReply}"
                }, "abdul_rahman")
            });
    }
}
