using System.Collections.Generic;

public static class FranciscoDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "francisco_de_almeida",
            "Francisco de Almeida",
            NpcPersonalityBucket.Strict,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Francisco de Almeida stands at your stall for {quantityLabel} of {spiceName}. Let us be direct.",
                    "I am Francisco. I came from the ports for {spiceName}, not for delay.",
                    "Good day. I seek {quantityLabel} of {spiceName} for the next voyage.",
                    "Francisco de Almeida, at your service. Show me the {spiceName} fit for shipment.",
                    "I have business in {spiceName} today. Let us speak as merchants under contract.",
                    "I came to inspect {quantityLabel} of {spiceName}. Let us begin without wasting the tide."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We meet again, merchant. Let us return to price without delay.",
                    "I have come back to finish this bargain properly.",
                    "We have spoken before. Let us conclude matters now.",
                    "Again we meet. I trust we can be more efficient this time.",
                    "I return for the same trade, merchant. Speak clearly.",
                    "Let us resume where we left off and waste no time."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I seek {quantityLabel} of {spiceName} for trade beyond this market and out through the ports.",
                    "The {spiceName} is what I came for, and I require {quantityLabel}.",
                    "My interest is in {spiceName}, measured at {quantityLabel} and fit for voyage.",
                    "I want {quantityLabel} of {spiceName}, if the quality is sound enough for shipment.",
                    "It is {spiceName} I seek, and in proper quantity for the holds.",
                    "I came for {spiceName}. The measure I need is {quantityLabel}."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "My present offer is {currentBuyerOffer} {currency}.",
                    "I can commit {currentBuyerOffer} {currency} for this lot.",
                    "My purse opens to {currentBuyerOffer} {currency} for this purchase.",
                    "I am prepared to pay {currentBuyerOffer} {currency}.",
                    "For this quantity, {currentBuyerOffer} {currency} is my offer.",
                    "You may consider {currentBuyerOffer} {currency} my working figure."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I require {quantityLabel} of {spiceName} for shipment.",
                    "My quantity is {quantityLabel} of {spiceName}.",
                    "Set my order at {quantityLabel} of {spiceName}.",
                    "I seek {quantityLabel} of {spiceName} for the voyage ahead.",
                    "The measure I want is {quantityLabel} of {spiceName}.",
                    "My cargo requires {quantityLabel} of {spiceName}."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is far too steep for serious trade.",
                    "That price is excessive, merchant. I will not pay it.",
                    "Your demand is too high for any disciplined account.",
                    "No, that figure is far beyond reason.",
                    "That price would ruin the advantage of the trade.",
                    "I cannot entertain {offeredPrice} {currency}. It is too much."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price delays my voyage. Lower it.",
                    "{offeredPrice} {currency} is too high, and my time is short.",
                    "No. I will not lose the tide over such a price."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask somewhat above reason. I can come to {counterPrice} {currency}.",
                    "Lower it a little, and {counterPrice} {currency} becomes possible.",
                    "You stand a little high. {counterPrice} {currency} is nearer the mark.",
                    "That is close, but still too rich for me. I can offer {counterPrice} {currency}.",
                    "Come down modestly, and we may settle at {counterPrice} {currency}.",
                    "Your figure is not impossible, only too high. {counterPrice} {currency} is better."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We do not have time for tiny delays. Make it {counterPrice} {currency}.",
                    "Come down at once to {counterPrice} {currency}.",
                    "You are close, merchant. Be swift and take {counterPrice} {currency}."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "Very well. That price is acceptable.",
                    "That figure serves the trade. I accept.",
                    "Yes, that is within reason.",
                    "That price suits the transaction.",
                    "We may proceed on that figure.",
                    "I find that amount acceptable."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "You speak lower than I expected. That is unusual, but welcome.",
                    "That is beneath the figure I prepared for, merchant.",
                    "You surprise me. That price is lower than expected.",
                    "That is a favorable figure, if the goods are worthy of it.",
                    "You ask less than I had anticipated.",
                    "That number comes below my expectation."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "Good. A quicker bargain suits me.",
                    "That is lower than expected. We may finish this swiftly.",
                    "A sensible surprise, merchant. Do not lose the moment."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "My first counter is {counterPrice} {currency}.",
                    "Let us begin at {counterPrice} {currency} and proceed sensibly.",
                    "I open with {counterPrice} {currency}.",
                    "My answer is {counterPrice} {currency} to begin with.",
                    "We start at {counterPrice} {currency}, merchant.",
                    "Set my first counter at {counterPrice} {currency}."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "We are closer now. I can offer {counterPrice} {currency}.",
                    "I will move to {counterPrice} {currency}, though not beyond reason.",
                    "I can advance to {counterPrice} {currency}.",
                    "That is nearer. My next figure is {counterPrice} {currency}.",
                    "I will come to {counterPrice} {currency}, but no lightly earned coin goes further.",
                    "We approach agreement. {counterPrice} {currency} is my next offer."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I move to {counterPrice} {currency}. Let us end this soon.",
                    "{counterPrice} {currency}. That is my quicker answer.",
                    "Take {counterPrice} {currency} seriously. My patience shortens."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "This is my final counter: {counterPrice} {currency}.",
                    "I go no further than {counterPrice} {currency}. Decide on that.",
                    "My last figure is {counterPrice} {currency}.",
                    "This is the end of my movement: {counterPrice} {currency}.",
                    "You have my final word on price: {counterPrice} {currency}.",
                    "No further concession. {counterPrice} {currency} is final."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency}. Final, and quickly.",
                    "That is my last figure. The ship will not wait.",
                    "I stop at {counterPrice} {currency}. Decide now."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "My offer remains {currentBuyerOffer} {currency}.",
                    "I hold at {currentBuyerOffer} {currency}; that is my limit.",
                    "I do not move beyond {currentBuyerOffer} {currency}.",
                    "My position stays at {currentBuyerOffer} {currency}.",
                    "No, merchant. {currentBuyerOffer} {currency} remains my offer.",
                    "I stand firm at {currentBuyerOffer} {currency}."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}. Do not delay me further.",
                    "My figure stands. I have no patience for circling.",
                    "{currentBuyerOffer} {currency} remains my offer. Be brief now."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Good. Then we are agreed at {finalPrice} {currency}.",
                    "Excellent. Seal the trade at {finalPrice} {currency}.",
                    "Very good. We settle at {finalPrice} {currency}.",
                    "Then the bargain is struck at {finalPrice} {currency}.",
                    "Agreed. Let the trade stand at {finalPrice} {currency}.",
                    "Good. We close at {finalPrice} {currency}."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then you refuse a workable offer.",
                    "So be it. We do not have an agreement.",
                    "Then you decline my terms.",
                    "Very well. The bargain fails there.",
                    "You reject the offer, then.",
                    "So the trade stops without accord."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "The sea routes carry many stories, but trade comes first.",
                    "There is time for tales of ports later. Now we bargain.",
                    "Ask of ships and harbors later. First we settle the spice.",
                    "Voyages are long, merchant, but this contract stands before us now.",
                    "I know many harbors, but I am here to trade, not lecture.",
                    "History may wait. Price cannot."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Courtesy is fine, merchant, but profit keeps the ships moving.",
                    "Save the pleasantries. Let us return to the bargain.",
                    "You are courteous. Now let us speak of terms.",
                    "A civil greeting is welcome. Let us continue the trade.",
                    "Well met. Now to business.",
                    "Good manners are useful, but I still require a sound price."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Be precise, merchant. {ruleReply}",
                    "State it clearly. {ruleReply}",
                    "Your meaning is not yet clear. {ruleReply}",
                    "Speak directly, if you please. {ruleReply}",
                    "I require a clearer statement. {ruleReply}",
                    "Be exact with me, merchant. {ruleReply}"
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "I crossed sea routes for trade, not idle talk.",
                    "Keep to the bargain, merchant. My business is in spice, not wandering talk.",
                    "Leave aside stray matters. We are here to trade.",
                    "Let us stay with the spice and the price.",
                    "I have no use for distractions. Return to the bargain.",
                    "Speak of trade, merchant, not of wandering subjects."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I did not catch that clearly. {ruleReply}",
                    "The market noise obscures your meaning. {ruleReply}",
                    "I could not hear that properly. {ruleReply}",
                    "Your words were lost in the bazaar. {ruleReply}",
                    "Say that again more clearly. {ruleReply}",
                    "The meaning escaped me. {ruleReply}"
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "A profitable bargain. We both leave satisfied.",
                    "Good. The trade is concluded cleanly, as any proper contract should be.",
                    "This transaction serves us both well.",
                    "A sound bargain. I am satisfied, and the voyage may continue.",
                    "Well done. The trade is complete.",
                    "That concludes our business profitably. The ports will hear well of such goods."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then this trade ends without agreement.",
                    "No sale today, merchant. I will seek better terms elsewhere.",
                    "Then we part without a bargain.",
                    "So the trade fails here.",
                    "Very well. I will take my coin elsewhere.",
                    "No accord today. We are finished."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we are done. My route cannot wait longer.",
                    "No agreement. I must move on at once.",
                    "This ends here. Time has been spent enough."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "My time is limited. Decide now.",
                    "Do not delay me further, merchant. Give me your answer.",
                    "I cannot linger here much longer.",
                    "Make your decision quickly.",
                    "My schedule does not allow more delay.",
                    "Come, merchant. Time is passing."
                }, "francisco_de_almeida")
                ,new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Decide now. The tide does not pause for us.",
                    "My ship will not wait on this stall forever.",
                    "You have little time left to answer me."
                }, "francisco_de_almeida", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Strict)
            });
    }
}
