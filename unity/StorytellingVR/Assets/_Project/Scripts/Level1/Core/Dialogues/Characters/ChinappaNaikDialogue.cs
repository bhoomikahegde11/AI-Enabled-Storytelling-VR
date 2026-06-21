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
                    "Chinnamma Naik has come for {quantityLabel} of {spiceName}. Let us weigh words as carefully as goods.",
                    "I am Chinnamma Naik, buying for larger accounts. Show me a sensible path to {quantityLabel} of {spiceName}.",
                    "Good day. I require {quantityLabel} of {spiceName} for warehouse trade in the royal city.",
                    "Chinnamma Naik stands here for bulk purchase. Speak clearly on {spiceName}.",
                    "I have come for {spiceName} in proper quantity. Let us proceed without fuss.",
                    "My business today is {quantityLabel} of {spiceName}. Keep your terms fit for a city account."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We know each other already, merchant. Let us return to the matter of {spiceName}.",
                    "We have dealt before. Let us waste no time now.",
                    "Again we meet. Speak plainly and keep to the terms.",
                    "I return to your stall for business, not delay.",
                    "We know the matter already. Let us settle it efficiently.",
                    "I have come back to conclude this trade properly."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I am here for {quantityLabel} of {spiceName}, enough for a worthwhile city order.",
                    "The {spiceName} is my business today, and I seek {quantityLabel} of it.",
                    "I require {quantityLabel} of {spiceName} for bulk handling and warehouse storage.",
                    "My interest is in {spiceName}, and in full measure.",
                    "It is {spiceName} I want, enough for a proper account in the royal city.",
                    "I came for {spiceName}. The quantity I need is {quantityLabel}.",
                    "I seek {quantityLabel} of {spiceName} for larger dealings under my seal."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "For this quantity, I can justify {currentBuyerOffer} {currency}.",
                    "My books permit {currentBuyerOffer} {currency}, no more without reason.",
                    "My present figure is {currentBuyerOffer} {currency}.",
                    "I can place {currentBuyerOffer} {currency} on this lot.",
                    "For this purchase, {currentBuyerOffer} {currency} is a defensible price.",
                    "My account allows {currentBuyerOffer} {currency} and no careless excess.",
                    "You may take {currentBuyerOffer} {currency} as my offer."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I require {quantityLabel} of {spiceName} for this account.",
                    "My order stands at {quantityLabel} of {spiceName}.",
                    "Set the measure at {quantityLabel} of {spiceName}.",
                    "For this city trade, I want {quantityLabel} of {spiceName}.",
                    "The proper quantity is {quantityLabel} of {spiceName}.",
                    "I seek {quantityLabel} of {spiceName} in full measure.",
                    "My warehouse order is {quantityLabel} of {spiceName}."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is too swollen a figure for honest trade.",
                    "That price would empty margin from the bargain before it begins.",
                    "Your demand is too high for disciplined trade.",
                    "No, merchant. That figure is beyond reason.",
                    "That price would damage the account from the start.",
                    "I will not pay {offeredPrice} {currency}. It is excessive.",
                    "Such a price does not suit a serious market."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for a disciplined account.",
                    "{offeredPrice} {currency} exceeds what sound trade permits.",
                    "No, merchant. That figure remains above reason."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, and I will not debate it much longer.",
                    "{offeredPrice} {currency} is excessive. Lower it or end this quickly.",
                    "You ask too much, merchant, and my time is already shortened."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask somewhat above reason. I can step to {counterPrice} {currency}.",
                    "Trim your demand a little, and {counterPrice} {currency} becomes possible.",
                    "You stand a little high. {counterPrice} {currency} is more sensible.",
                    "That figure is near, but still above my mark. I can offer {counterPrice} {currency}.",
                    "Lower it modestly, and {counterPrice} {currency} will settle it.",
                    "I can move to {counterPrice} {currency}, but not to your full demand.",
                    "Your price is close to fair, though {counterPrice} {currency} is fairer."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You stand somewhat high. I can do {counterPrice} {currency}.",
                    "{counterPrice} {currency} is the proper adjustment from your price.",
                    "Come down modestly, merchant, and {counterPrice} {currency} will settle it."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, but quickly now. {counterPrice} {currency}.",
                    "A faster end would be {counterPrice} {currency}, merchant.",
                    "Lower it at once to {counterPrice} {currency}, or I move on."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "That number is sound. We may conclude this trade.",
                    "Very well. That price respects both scales and profit.",
                    "Yes, that figure is acceptable.",
                    "That amount sits properly with the account.",
                    "A sound price. We may proceed.",
                    "That number will do.",
                    "Very well. The trade may stand on that price."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "You have named less than I expected, merchant. Let us settle it before fortune changes.",
                    "That is lower than I prepared for. I will not complain if the quality matches.",
                    "You speak below my expectation. That is notable.",
                    "That figure comes lower than I had reckoned.",
                    "A lower price than expected is no insult to me.",
                    "You surprise me. The number is better than anticipated.",
                    "That is beneath the price I had prepared to hear."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Sensible.",
                    "You speak more reasonably than I had anticipated.",
                    "That figure sits better with a disciplined account."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Good. Let us close this quickly.",
                    "A better figure than I expected, merchant. Do not waste it.",
                    "You speak more sensibly now. Finish the matter."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "My opening counter is {counterPrice} {currency}. Consider it seriously.",
                    "I begin at {counterPrice} {currency} for this lot.",
                    "We start at {counterPrice} {currency}.",
                    "My first answer is {counterPrice} {currency}.",
                    "Set my opening figure at {counterPrice} {currency}.",
                    "I place {counterPrice} {currency} before you as my first counter.",
                    "Let the bargaining begin at {counterPrice} {currency}."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are closing the distance. I can offer {counterPrice} {currency}.",
                    "I will advance to {counterPrice} {currency}, though not gladly.",
                    "I can move to {counterPrice} {currency}.",
                    "That is nearer the mark. My next figure is {counterPrice} {currency}.",
                    "I advance to {counterPrice} {currency}, and no further for the moment.",
                    "We approach agreement. I offer {counterPrice} {currency}.",
                    "I will step forward to {counterPrice} {currency}."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can advance to {counterPrice} {currency}.",
                    "{counterPrice} {currency} is my next serious figure.",
                    "We move closer. I offer {counterPrice} {currency}."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can move to {counterPrice} {currency}, but not for long.",
                    "{counterPrice} {currency} is my next step. Answer promptly.",
                    "Be quick, merchant. I offer {counterPrice} {currency}."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "This is my final working figure: {counterPrice} {currency}.",
                    "I move no further than {counterPrice} {currency}. Decide on that.",
                    "My last figure is {counterPrice} {currency}.",
                    "This is the end of my movement: {counterPrice} {currency}.",
                    "You have my final price, {counterPrice} {currency}.",
                    "No further concession. {counterPrice} {currency} is final.",
                    "That is my closing counter: {counterPrice} {currency}."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final figure.",
                    "I stop at {counterPrice} {currency}, merchant.",
                    "This is my closing offer: {counterPrice} {currency}."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my last step. Decide now.",
                    "I end at {counterPrice} {currency}, merchant. I will not repeat it.",
                    "This is my final offer: {counterPrice} {currency}. Answer at once."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "My position remains {currentBuyerOffer} {currency}. The accounts do not bend further.",
                    "I hold at {currentBuyerOffer} {currency}, merchant.",
                    "I do not move beyond {currentBuyerOffer} {currency}.",
                    "My figure stays at {currentBuyerOffer} {currency}.",
                    "No, merchant. {currentBuyerOffer} {currency} remains my position.",
                    "I stand firm at {currentBuyerOffer} {currency}.",
                    "The account ends at {currentBuyerOffer} {currency} and no higher."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I remain at {currentBuyerOffer} {currency}. That is the proper limit.",
                    "{currentBuyerOffer} {currency} is my standing figure.",
                    "No further, merchant. {currentBuyerOffer} {currency} is where I stand."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, and I will not repeat myself often.",
                    "{currentBuyerOffer} {currency} is my limit. Conclude this now.",
                    "No more from me, merchant. {currentBuyerOffer} {currency}, final."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. Then we are agreed at {finalPrice} {currency}.",
                    "A proper conclusion. Let the bargain stand at {finalPrice} {currency}.",
                    "Very well. We settle at {finalPrice} {currency}.",
                    "Then the agreement stands at {finalPrice} {currency}.",
                    "Good. Let the account close at {finalPrice} {currency}.",
                    "That is acceptable. We conclude at {finalPrice} {currency}.",
                    "Agreed. {finalPrice} {currency} finishes the matter."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then you decline a workable offer.",
                    "So the bargain ends because your price will not meet the market.",
                    "Then you refuse terms I consider sound.",
                    "Very well. You reject the offer.",
                    "So the matter remains unsettled.",
                    "Then the trade halts here.",
                    "Your refusal leaves the account unfinished."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Hampi's markets hold many stories, but this ledger comes first.",
                    "Ask of history later. For now, let us finish the trade.",
                    "The market has memory enough, but the account before us matters first.",
                    "There are many stories in this royal city. I am here for business.",
                    "History may wait. Warehouse accounts cannot.",
                    "Ask of the market later. First we settle the goods and coin.",
                    "The city is old, merchant, but this bargain is the matter at hand."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Your courtesy is noted. Now let us return to terms and quantity.",
                    "Good manners help a bargain, but numbers finish it.",
                    "You are polite, merchant. Be equally precise.",
                    "Courtesy is proper. So is a clear account.",
                    "Well enough. Now let us speak in figures.",
                    "Civil words are welcome. Accurate terms matter more.",
                    "Good. Now return to measure and price."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Keep to the bargain, merchant. My purpose here is trade.",
                    "Stray talk will not move the price of {spiceName}.",
                    "Leave aside distractions. Speak of the trade.",
                    "I have no need of wandering talk. Return to business.",
                    "Keep your words on quantity and price.",
                    "Off-topic talk serves no account, merchant.",
                    "Speak of {spiceName}, measure, and coin. Nothing else."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "The bazaar swallowed your meaning. {ruleReply}",
                    "Speak plainly, merchant. {ruleReply}",
                    "Your meaning is not yet clear. {ruleReply}",
                    "I did not catch that properly. {ruleReply}",
                    "State it again with precision. {ruleReply}",
                    "Your words were lost in the market noise. {ruleReply}",
                    "Be clearer, merchant. {ruleReply}"
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "This trade was concluded with good order.",
                    "A sound bargain. May both our stores and warehouses benefit.",
                    "Good. The account is settled properly.",
                    "A disciplined trade. That is how business should be done in the royal city.",
                    "This bargain serves both reputation and profit.",
                    "Well concluded. The market will remember proper dealing.",
                    "The trade stands complete and orderly."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we close the ledger without a sale today.",
                    "No agreement, then. I will carry my coin elsewhere.",
                    "Then this account ends without trade.",
                    "So be it. The bargain fails today.",
                    "No sale. I will look to another stall.",
                    "The ledger closes empty for now.",
                    "Then we part without agreement."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we close without agreement today.",
                    "No purchase this time, merchant.",
                    "So be it. The account does not conclude."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I end this matter now and take my business elsewhere.",
                    "No bargain today, merchant. You have delayed it enough.",
                    "So be it. The trade fails, and I move on."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I have other stalls to inspect. Decide without delay.",
                    "My time thins, merchant. Give me your answer now.",
                    "I cannot spend the day on one bargain.",
                    "Make your decision quickly. I have other accounts to attend.",
                    "Do not delay me further, merchant.",
                    "My time is not idle. Speak your answer now.",
                    "Come to a decision. I must move on."
                }, "chinnamma_naik"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Merchant, my patience is ending. Decide now.",
                    "I will not be delayed further. Speak your answer at once.",
                    "My time is spent here. Conclude this immediately."
                }, "chinnamma_naik", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "{ruleReply}",
                    "Be precise, merchant. {ruleReply}",
                    "Your answer lacks clarity. {ruleReply}",
                    "State it plainly and correctly. {ruleReply}",
                    "I require a clearer statement. {ruleReply}",
                    "Speak in direct terms. {ruleReply}"
                }, "chinnamma_naik")
            });
    }
}
