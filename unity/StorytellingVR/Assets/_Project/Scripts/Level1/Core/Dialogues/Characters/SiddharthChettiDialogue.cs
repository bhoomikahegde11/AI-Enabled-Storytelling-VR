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
                    "Greetings, merchant. Saraswati Chetti seeks {quantityLabel} of {spiceName} for the bazaar trade.",
                    "I am Saraswati Chetti, and I have come for {quantityLabel} of {spiceName} to fill my shop jars.",
                    "Well met. I need {quantityLabel} of {spiceName} for the day's selling.",
                    "My shelves would welcome some good {spiceName} before the bazaar grows crowded.",
                    "I came for {spiceName}, merchant, and in enough measure to keep customers smiling.",
                    "Saraswati Chetti is here for {quantityLabel} of {spiceName}. Let us see what your stall offers."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We meet again, friend. Let us see whether today brings a kinder bargain.",
                    "Back to your stall again, am I? Good. Let us talk business.",
                    "We know each other now. That should make this easier.",
                    "Here I am again, merchant. Let us see if the price smiles on us today.",
                    "Another day, another bargain, friend.",
                    "I return for more stock. Let us settle it briskly."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I want {quantityLabel} of {spiceName} for my customers in the bazaar lanes.",
                    "The {spiceName} will sell well if the price leaves me a margin worth smiling over.",
                    "I seek {quantityLabel} of {spiceName} for the front of my shop.",
                    "It is {spiceName} I need, and enough of it to keep the jars full from dawn to dusk.",
                    "My customers ask for good {spiceName}, so that is what I seek.",
                    "I came for {spiceName}, merchant, and in proper measure for resale."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can safely spend {currentBuyerOffer} {currency} and still keep my little shop running.",
                    "My purse reaches {currentBuyerOffer} {currency} for this purchase.",
                    "My working figure is {currentBuyerOffer} {currency}.",
                    "I can lay down {currentBuyerOffer} {currency} and still keep a margin.",
                    "For this lot, {currentBuyerOffer} {currency} is a sensible price.",
                    "I can offer {currentBuyerOffer} {currency} without hurting the books."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I need {quantityLabel} of {spiceName} for my shop.",
                    "My quantity is {quantityLabel} of {spiceName}.",
                    "Set me down for {quantityLabel} of {spiceName}.",
                    "For resale, I want {quantityLabel} of {spiceName}.",
                    "My shop jars need {quantityLabel} of {spiceName}.",
                    "The amount I seek is {quantityLabel} of {spiceName}.",
                    "I will take {quantityLabel} of {spiceName} for the bazaar."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is too hard on a shopkeeper's margin.",
                    "That price leaves too little room for me to resell fairly.",
                    "No, friend, that cuts too deep into my margin.",
                    "That figure is too heavy for a man who must sell again tomorrow.",
                    "I cannot keep customers and pay that price as well.",
                    "I will not take {offeredPrice} {currency}. There is no room left in it.",
                    "That price squeezes the life out of the bargain."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is steep, friend, though I would still rather trade with you.",
                    "{offeredPrice} {currency} is too much for my little shop, but we can keep talking.",
                    "You ask too high, merchant, though I am willing to work toward a fairer number."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for a practical reseller.",
                    "{offeredPrice} {currency} leaves me too little room to sell well.",
                    "No, friend, that figure is still too heavy for my books."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, and my customers will not wait forever.",
                    "{offeredPrice} {currency} is too much. Speak faster if we are to finish.",
                    "You ask too high, merchant, and I must get back to my stall."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask a touch too much, friend. Could we meet at {counterPrice} {currency}?",
                    "Come a little lower, and {counterPrice} {currency} will do.",
                    "You are close, merchant, but not close enough. I can offer {counterPrice} {currency}.",
                    "Trim it a little, and {counterPrice} {currency} will keep us both smiling.",
                    "That price is nearly there. {counterPrice} {currency} would be fairer.",
                    "A small step down, friend, and {counterPrice} {currency} will settle it.",
                    "My side of the scale says {counterPrice} {currency}."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, friend. I could settle gladly at {counterPrice} {currency}.",
                    "Your price is near enough. Let us meet at {counterPrice} {currency} and keep goodwill.",
                    "A small step lower to {counterPrice} {currency} would keep both our stalls happy."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You are close, merchant. I can do {counterPrice} {currency}.",
                    "{counterPrice} {currency} is the practical figure for my resale.",
                    "Come a shade lower, friend, and {counterPrice} {currency} will settle it."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, but quickly now. {counterPrice} {currency}.",
                    "A faster end would be {counterPrice} {currency}, merchant.",
                    "I can do {counterPrice} {currency}, but I cannot linger from my stall."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That is workable for me. Let us close the bargain.",
                    "A fair enough price. I can accept that.",
                    "Yes, that will serve my trade well enough.",
                    "Good. That price leaves room for both of us.",
                    "That will do nicely, merchant.",
                    "I can work with that figure.",
                    "Very well. Let us close on that."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is friendlier than I expected, merchant.",
                    "You speak a lower number than I feared. That gladdens a buyer.",
                    "Ah, that is kinder than I had prepared for.",
                    "You surprise me, friend. That price is lighter than expected.",
                    "That is better than I feared I would hear.",
                    "A pleasant figure, merchant. Lower than I expected.",
                    "That price sits easier than I thought it would."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is kindly said, friend. I will remember such fairness.",
                    "You ask less than I expected, and that does a shopkeeper's heart good.",
                    "That is a welcome surprise, merchant."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than I expected, and useful.",
                    "A fair surprise, friend. That sits better with my books.",
                    "You speak more reasonably than I expected."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Good. Let us settle it quickly.",
                    "A welcome price, friend. It saves me time and worry.",
                    "You speak better than I feared, merchant. Let us finish now."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "Let me begin at {counterPrice} {currency}, if that seems fair.",
                    "My first counter is {counterPrice} {currency} for the lot.",
                    "I will start at {counterPrice} {currency}.",
                    "My opening answer is {counterPrice} {currency}, friend.",
                    "Let us begin the bargaining at {counterPrice} {currency}.",
                    "I place {counterPrice} {currency} before you to start.",
                    "First offer from me: {counterPrice} {currency}."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are getting warmer. I can offer {counterPrice} {currency}.",
                    "I will stretch to {counterPrice} {currency} to keep this trade alive.",
                    "We are coming together now. {counterPrice} {currency} is my next step.",
                    "I can move to {counterPrice} {currency}, and that is a fair stretch.",
                    "That is closer. I will go to {counterPrice} {currency}.",
                    "Let me come up a little more, to {counterPrice} {currency}.",
                    "I still want this lot, so I offer {counterPrice} {currency}."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "In goodwill, I can offer {counterPrice} {currency}.",
                    "Let us keep this cheerful and fair. {counterPrice} {currency}.",
                    "I will come to {counterPrice} {currency}, friend, so we may trade well together."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are drawing near. I can offer {counterPrice} {currency}.",
                    "{counterPrice} {currency} is my next sensible step.",
                    "I move to {counterPrice} {currency}, merchant."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can offer {counterPrice} {currency}, but let us not drag this out.",
                    "{counterPrice} {currency} is my next step, friend. My stall still needs me.",
                    "Be quick now. I can come to {counterPrice} {currency}."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "Friend, {counterPrice} {currency} is the furthest I can sensibly go.",
                    "This must be my last step: {counterPrice} {currency}.",
                    "That is my last figure, {counterPrice} {currency}.",
                    "I can go no further and still keep a margin. {counterPrice} {currency}.",
                    "My last word is {counterPrice} {currency}.",
                    "No more steps from me. {counterPrice} {currency} is final.",
                    "That is the end of my road on price: {counterPrice} {currency}."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "This is my last step, given in friendship: {counterPrice} {currency}.",
                    "{counterPrice} {currency} is as far as I can go and still smile at the trade.",
                    "Friend, I end at {counterPrice} {currency}. Let us close well if we can."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final figure.",
                    "I stop at {counterPrice} {currency}, merchant.",
                    "This is my last offer, friend: {counterPrice} {currency}."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my last step, and my stall cannot wait longer.",
                    "I end at {counterPrice} {currency}, friend. Customers may already be looking for me.",
                    "This is my last offer: {counterPrice} {currency}. Decide quickly now."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must stay at {currentBuyerOffer} {currency}, or my books will curse me later.",
                    "My offer holds at {currentBuyerOffer} {currency}.",
                    "No, friend, I stay at {currentBuyerOffer} {currency}.",
                    "My books stop at {currentBuyerOffer} {currency}.",
                    "That is where I must stand: {currentBuyerOffer} {currency}.",
                    "I hold firm at {currentBuyerOffer} {currency}.",
                    "I cannot push beyond {currentBuyerOffer} {currency} and still trade well."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, though I would rather keep this friendly.",
                    "{currentBuyerOffer} {currency} is my limit, but I hope you see the fairness in it.",
                    "My offer remains {currentBuyerOffer} {currency}. I would still rather part well."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}, merchant.",
                    "{currentBuyerOffer} {currency} remains my sensible limit.",
                    "No further, friend. {currentBuyerOffer} {currency} is where I stand."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, and my stall still calls me back.",
                    "{currentBuyerOffer} {currency} is my limit, friend. Let us not linger longer.",
                    "No more from me, merchant. {currentBuyerOffer} {currency}, and we must finish."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Excellent. We are agreed at {finalPrice} {currency}.",
                    "Then let us finish it at {finalPrice} {currency} and part content.",
                    "Good. We settle at {finalPrice} {currency}.",
                    "A fine end to it. {finalPrice} {currency} it is.",
                    "Agreed, merchant. Let the bargain stand at {finalPrice} {currency}.",
                    "That will do nicely. We close at {finalPrice} {currency}.",
                    "Very good. {finalPrice} {currency} finishes the matter."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "A pity. I had hoped we would find common ground.",
                    "Then my offer does not please you today.",
                    "Ah well, then we do not meet on price.",
                    "So be it. My offer falls short for you.",
                    "That is a shame. I thought we were close.",
                    "Then the bargain slips away from us today.",
                    "You do not care for my terms, it seems."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "The bazaar carries many tales, but my concern today is this trade.",
                    "Stories can wait a moment. First let us settle the {spiceName}.",
                    "There is always a story in the bazaar, but I still need stock for my shop.",
                    "Talk of old days can wait. Let us finish the buying first.",
                    "I know many market tales, friend, but today I need good {spiceName} on my shelves.",
                    "The city is full of stories. My shop is full only when I buy well.",
                    "First the spice, then the stories."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Well met, merchant. Now let us see if kindness can lead to a fair price.",
                    "You speak warmly, and I thank you. Shall we return to the bargain?",
                    "A cheerful word is always welcome. So is a fair price.",
                    "You are kind, friend. Now let us see what the bargain says.",
                    "Good to hear a warm voice in the market. Let us talk terms.",
                    "Well met indeed. Now, what price do we make of this?",
                    "Pleasant words help the day along. A good bargain helps even more."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us keep our minds on trade, friend.",
                    "The shop must be stocked, so let us speak of {spiceName} and price.",
                    "I would rather keep to the bargain, merchant.",
                    "My customers wait on good stock, not wandering talk.",
                    "Let us return to {spiceName} and the coin that follows it.",
                    "The spice jars do not fill themselves, friend.",
                    "Speak of trade, not straying matters."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I could not follow that over the market noise. {ruleReply}",
                    "Your words blurred together for me. {ruleReply}",
                    "Say that again, friend. {ruleReply}",
                    "The market swallowed your meaning. {ruleReply}",
                    "I missed that in the noise. {ruleReply}",
                    "Speak a little clearer for me. {ruleReply}",
                    "Your words ran together, merchant. {ruleReply}"
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "A pleasing bargain. My customers will be glad of this purchase.",
                    "Good trade, merchant. May your stall stay busy and my shop stay fuller.",
                    "That will keep my shelves happy for a while.",
                    "A fine purchase. My customers will thank me for it.",
                    "Good business for both of us, friend.",
                    "Well done. This stock will move well in my shop.",
                    "A tidy bargain. I leave satisfied."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without a sale, though I bear no grudge.",
                    "No bargain today. Perhaps another market hour will be kinder.",
                    "Then I leave without stock today.",
                    "So be it. I will seek another stall.",
                    "No sale, then. The market is wide enough.",
                    "Ah well, perhaps the next bargain will be kinder.",
                    "Then we part without business this time."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without trade, friend, though I still wish you good custom.",
                    "No bargain today, yet I would rather leave on friendly terms.",
                    "So be it. Another market hour may treat us better."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.High, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement today.",
                    "No purchase this time, merchant.",
                    "So be it, friend. The bargain does not close."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I must part now, friend. My stall still needs me.",
                    "No bargain today, and I must return to my customers.",
                    "So be it, merchant. Time has thinned, and we still have no trade."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I cannot linger too long away from my shop. Decide soon.",
                    "Time presses on me, friend. Let us finish this quickly.",
                    "My stall needs me back soon, merchant.",
                    "Come now, friend, I cannot leave the shop unattended all day.",
                    "Let us finish this before my customers wander off.",
                    "Time is moving, and so is the market. Decide soon.",
                    "I must get back to my stall. Give me your answer."
                }, "saraswati_chetti"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Friend, my customers will drift away if I linger longer. Decide now.",
                    "My stall cannot wait on me much longer. Let us finish quickly.",
                    "My time is nearly spent, merchant. Speak your answer at once."
                }, "saraswati_chetti", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Friendly),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "{ruleReply}",
                    "Speak a little clearer, friend. {ruleReply}",
                    "I am not sure what you mean there. {ruleReply}",
                    "Say it plainly for me. {ruleReply}",
                    "Your meaning slips past me. {ruleReply}",
                    "Let us be clear with each other. {ruleReply}"
                }, "saraswati_chetti")
            });
    }
}
