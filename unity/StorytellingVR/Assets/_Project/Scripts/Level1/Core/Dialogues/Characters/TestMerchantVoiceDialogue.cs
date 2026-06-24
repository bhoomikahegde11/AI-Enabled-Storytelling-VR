using System.Collections.Generic;

public static class TestMerchantVoiceDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "test_merchant_voice",
            "Test Merchant Voice",
            NpcPersonalityBucket.Normal,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Good day, merchant. I am here to buy {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
                {
                    "Good day, merchant. I am here to buy {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
                {
                    "Good day, merchant. I am here to buy {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
                {
                    "I can offer {currentBuyerOffer} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.AskQuantity, new[]
                {
                    "How much {spiceName} can you sell me?"
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
                {
                    "That price is too high. Lower it a little."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
                {
                    "That price is too high. I can increase my offer to {counterPrice} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
                {
                    "Agreed. We have a deal."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
                {
                    "I can increase my offer to {counterPrice} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
                {
                    "I can offer {counterPrice} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
                {
                    "I can increase my offer to {counterPrice} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
                {
                    "My final offer is {counterPrice} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
                {
                    "My final offer is {currentBuyerOffer} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
                {
                    "Agreed. We have a deal."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
                {
                    "No deal. I will go elsewhere."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.HistoryQuestion, new[]
                {
                    "Please say that again, merchant."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Good day, merchant. I am here to buy {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us speak clearly and continue our trade."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.UnclearSpeech, new[]
                {
                    "Please say that again, merchant."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.TransactionSuccess, new[]
                {
                    "Agreed. We have a deal."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.TransactionFailure, new[]
                {
                    "No deal. I will go elsewhere."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.TimePressure, new[]
                {
                    "My final offer is {currentBuyerOffer} varahas for {spiceName}."
                }, "test_merchant_voice"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Let us speak clearly and continue our trade."
                }, "test_merchant_voice")
            });
    }
}
