using System.Collections.Generic;

public static class FatherPenteadoDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "father_penteado",
            "Father Penteado",
            NpcPersonalityBucket.Normal,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Peace be with you, merchant. Father Penteado seeks {quantityLabel} of {spiceName}.",
                    "I am Father Penteado, and I have come for {quantityLabel} of {spiceName} with modest means.",
                    "Good day. I seek {quantityLabel} of {spiceName} for my table and the road ahead.",
                    "I have come for {spiceName}, merchant, and in a careful measure.",
                    "Father Penteado greets you. I require {quantityLabel} of {spiceName} for travel and hospitality.",
                    "Peace to your stall. I am here to buy {spiceName} with care."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "We have met before. Let us speak plainly and finish the bargain well.",
                    "We meet again, merchant. Let us continue with patience.",
                    "I return to your stall in good faith. Let us settle matters calmly.",
                    "Here we are again. Perhaps this time the bargain will be easier.",
                    "We have spoken before. Let us resume with clear words.",
                    "I am glad to return. Now let us see what price can be made."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "I seek {quantityLabel} of {spiceName}, enough for my table and those who travel with me.",
                    "The {spiceName} is what brings me here today, merchant.",
                    "My need is {spiceName}, and in the measure of {quantityLabel}.",
                    "I came for {quantityLabel} of {spiceName}, if it is good and fairly priced.",
                    "It is {spiceName} I seek for travel, study, and hospitality.",
                    "I ask for {spiceName}, enough to serve a small company on the road."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "My purse allows {currentBuyerOffer} {currency}, and I must spend with care.",
                    "I can manage {currentBuyerOffer} {currency} for this purchase.",
                    "My present offer is {currentBuyerOffer} {currency}.",
                    "I can set aside {currentBuyerOffer} {currency} for this need.",
                    "For this quantity, {currentBuyerOffer} {currency} is within my means.",
                    "I am prepared to pay {currentBuyerOffer} {currency}, but not lightly."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "I seek {quantityLabel} of {spiceName}, if you please.",
                    "My quantity is {quantityLabel} of {spiceName}.",
                    "For my table and travels, {quantityLabel} of {spiceName} will do.",
                    "I ask for {quantityLabel} of {spiceName}.",
                    "The measure I need is {quantityLabel} of {spiceName}.",
                    "I would take {quantityLabel} of {spiceName}, merchant.",
                    "My order is {quantityLabel} of {spiceName}."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "{offeredPrice} {currency} is beyond what I can honestly pay.",
                    "That sum is too heavy for my small purse.",
                    "I am afraid that price is too high for me.",
                    "No, merchant, that figure is more than I can manage.",
                    "That price asks too much of a careful buyer.",
                    "I cannot give {offeredPrice} {currency}. It is too steep for me.",
                    "That amount is beyond my means today."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high for my means, merchant.",
                    "{offeredPrice} {currency} is beyond what I can sensibly give.",
                    "I am afraid that figure is still too heavy for me."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high, and I cannot remain overlong.",
                    "{offeredPrice} {currency} is more than I can pay. Please be brief now.",
                    "You ask too much, merchant, and I may soon have to take my leave."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You ask somewhat above my reach. I could offer {counterPrice} {currency}.",
                    "If you would ease the price to {counterPrice} {currency}, we may yet agree.",
                    "You are near my limit, but still above it. I can offer {counterPrice} {currency}.",
                    "Lower it a little, and {counterPrice} {currency} would be possible.",
                    "That is close to fair, though {counterPrice} {currency} suits me better.",
                    "If you come down gently, {counterPrice} {currency} may settle it.",
                    "A modest step lower would help. I can manage {counterPrice} {currency}."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "You are close, merchant. I could manage {counterPrice} {currency}.",
                    "{counterPrice} {currency} would be the calmer middle path for me.",
                    "A small step lower to {counterPrice} {currency} may settle us well."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "We are close, but I must be brief. {counterPrice} {currency}.",
                    "A quicker end would be {counterPrice} {currency}, merchant.",
                    "I can do {counterPrice} {currency}, though I cannot linger much longer."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "Very well. That is a fair and manageable price.",
                    "I can accept that with a clear conscience.",
                    "Yes, that price is reasonable.",
                    "That figure sits well enough with me.",
                    "I find that amount fair.",
                    "Very good. We may proceed on that price.",
                    "That will do, merchant."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "You speak more generously than I expected, merchant.",
                    "That is a kinder price than I had prepared to hear.",
                    "You surprise me. That is lower than I expected.",
                    "That is a gentler figure than I had feared.",
                    "I had prepared for more. This is welcome news.",
                    "That price is more modest than I expected to hear.",
                    "You have named a kinder sum than I anticipated."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than I expected, and welcome.",
                    "You speak more moderately than I had prepared for.",
                    "That figure rests more gently on my purse."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "That is lower than expected. Good. Let us conclude it soon.",
                    "A kinder figure than I expected, merchant. Let us not delay it.",
                    "You speak more reasonably now. We may finish this quickly."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "Then let my first counter be {counterPrice} {currency}.",
                    "I can begin at {counterPrice} {currency}, if you will consider it.",
                    "My first answer is {counterPrice} {currency}.",
                    "Let us begin at {counterPrice} {currency} and see whether we can agree.",
                    "I place {counterPrice} {currency} before you as my opening counter.",
                    "To begin, I can offer {counterPrice} {currency}.",
                    "My starting figure is {counterPrice} {currency}."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can move to {counterPrice} {currency} in the hope of agreement.",
                    "Let us try {counterPrice} {currency}; that is nearer my limit.",
                    "I can come a little further, to {counterPrice} {currency}.",
                    "That is closer. My next offer is {counterPrice} {currency}.",
                    "In the hope of peace between us, I offer {counterPrice} {currency}.",
                    "I will step to {counterPrice} {currency}, though carefully.",
                    "Perhaps {counterPrice} {currency} will bring us together."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can move to {counterPrice} {currency} in the hope of agreement.",
                    "{counterPrice} {currency} is my next careful step.",
                    "Let us try {counterPrice} {currency}; it is nearer my limit."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can come to {counterPrice} {currency}, but I must not stay long.",
                    "{counterPrice} {currency} is my next step, merchant. Please answer plainly.",
                    "Let us be quick now. I can offer {counterPrice} {currency}."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Middle, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is the furthest I can go today.",
                    "I can promise no more than {counterPrice} {currency}.",
                    "That is my final figure, {counterPrice} {currency}.",
                    "I can go no higher than {counterPrice} {currency}.",
                    "My last word on price is {counterPrice} {currency}.",
                    "No more can come from my purse today. {counterPrice} {currency}.",
                    "Take {counterPrice} {currency} as my final offer."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my final figure.",
                    "I can go no further than {counterPrice} {currency}, merchant.",
                    "This is my last offer, friend: {counterPrice} {currency}."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "{counterPrice} {currency} is my last step, and I must soon be on my way.",
                    "I end at {counterPrice} {currency}, merchant. I cannot remain much longer.",
                    "This is my final offer: {counterPrice} {currency}. Please decide soon."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must remain at {currentBuyerOffer} {currency}, merchant.",
                    "My offer stays at {currentBuyerOffer} {currency}; I cannot stretch it further.",
                    "No, merchant, I must stay at {currentBuyerOffer} {currency}.",
                    "That is where my purse must rest: {currentBuyerOffer} {currency}.",
                    "I cannot move beyond {currentBuyerOffer} {currency}.",
                    "My position remains {currentBuyerOffer} {currency}.",
                    "I must hold to {currentBuyerOffer} {currency} and no more."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I must hold at {currentBuyerOffer} {currency}, merchant.",
                    "{currentBuyerOffer} {currency} remains the limit of my purse.",
                    "No further, I am afraid. {currentBuyerOffer} {currency} is where I stand."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "I hold at {currentBuyerOffer} {currency}, and I must soon depart.",
                    "{currentBuyerOffer} {currency} is my limit, merchant. Let us not delay further.",
                    "No more from me, I am afraid. {currentBuyerOffer} {currency}, and we must finish."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Then we are agreed at {finalPrice} {currency}. My thanks.",
                    "Good. Let the trade be settled at {finalPrice} {currency}.",
                    "Very well. We close at {finalPrice} {currency}.",
                    "Then the bargain stands at {finalPrice} {currency}.",
                    "I thank you. Let us settle it at {finalPrice} {currency}.",
                    "Agreed. {finalPrice} {currency} will conclude the matter.",
                    "That is well. We finish at {finalPrice} {currency}."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "Then I must accept your refusal with grace.",
                    "So we cannot reach an agreement today.",
                    "Very well. Then my offer does not suit you.",
                    "I see. Then the bargain will not be made today.",
                    "That is unfortunate, though I understand.",
                    "Then we remain apart on price.",
                    "So be it. We do not yet agree."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "There are many roads and many kingdoms to speak of, but first let us finish this trade.",
                    "We may speak of distant lands later. For now, let us return to the bargain.",
                    "There is much to observe in this city, yet the trade before us comes first.",
                    "I would gladly speak of travels and learning later. For now, let us settle the spice.",
                    "Many lands have their stories, merchant, but this bargain is present.",
                    "Another time, perhaps, we may speak of distant roads, ports, and customs.",
                    "Let us complete the trade first, and then speak of wider things."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Your courtesy is appreciated, merchant. Shall we return to the matter of price?",
                    "Peace to you as well. Now let us conclude our business.",
                    "You are gracious, merchant. Let us now speak of terms.",
                    "A kind greeting is welcome. A fair bargain is welcome too.",
                    "Your good manners honour the market. Now let us return to trade.",
                    "Thank you for your courtesy. Shall we continue?",
                    "Peace and good order are welcome. Let us proceed with the bargain."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us stay with the purchase before us.",
                    "I would rather keep to the matter of {spiceName} and its price.",
                    "Let us not stray, merchant. I am here for this purchase.",
                    "I would prefer to return to the spice and the price.",
                    "Let us keep our attention on the trade itself.",
                    "The matter before us is {spiceName}, not another subject.",
                    "Please, let us stay with the bargain."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "I did not understand you clearly. {ruleReply}",
                    "The market din obscured your words. {ruleReply}",
                    "I could not make that out properly. {ruleReply}",
                    "Please say that again more clearly. {ruleReply}",
                    "Your words were lost in the noise. {ruleReply}",
                    "I missed your meaning just then. {ruleReply}",
                    "Would you repeat that plainly? {ruleReply}"
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Thank you. This trade has been concluded fairly.",
                    "A good bargain, merchant. You have my thanks.",
                    "This was a fair purchase. I am grateful.",
                    "Well done. The matter is settled honourably.",
                    "A kindly conclusion to our trade.",
                    "I thank you. This purchase will serve well on the road and at table.",
                    "The bargain is complete, and fairly so."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without a sale, and I wish you well.",
                    "No agreement today, it seems. May another buyer suit your price.",
                    "Then there will be no purchase from me today.",
                    "So be it. We part without a bargain.",
                    "No accord, then. I wish you a peaceful day.",
                    "It seems we cannot agree this time.",
                    "Then I must take my leave without buying."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then we part without agreement today.",
                    "No purchase this time, merchant.",
                    "So be it. The bargain does not conclude."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Medium, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "Then I must take my leave now, merchant.",
                    "No bargain today, and I have little time left to remain.",
                    "So be it. The trade fails, and I must be on my way."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "I cannot remain long. Please give me your answer.",
                    "Time grows short for me, merchant. Let us finish this soon.",
                    "I must not linger too long here. Please decide.",
                    "The hour presses on me. Let us conclude if we can.",
                    "I have little time left for bargaining.",
                    "Please, merchant, let us finish this without delay.",
                    "I must be on my way soon. Give me your answer."
                }, "father_penteado"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "Merchant, my time is nearly spent. Please decide now.",
                    "I must be on my way shortly. Let us finish without delay.",
                    "I cannot stay much longer, friend. Speak your answer at once."
                }, "father_penteado", PlayerReputationBucket.Neutral, NpcPatienceBucket.Low, NpcDesperationBucket.Medium, RoundBucket.Final, NpcPersonalityBucket.Normal),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "{ruleReply}",
                    "I would ask for clearer words. {ruleReply}",
                    "Your meaning is not yet plain to me. {ruleReply}",
                    "Please speak more directly. {ruleReply}",
                    "I am not certain what you intend. {ruleReply}",
                    "Let us be clear with each other. {ruleReply}"
                }, "father_penteado")
            });
    }
}
