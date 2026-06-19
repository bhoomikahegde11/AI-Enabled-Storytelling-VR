using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCharacterProfile
{
    public string characterId;
    public string displayName;
    public string buyerOrigin;
    public string buyerPersonality;

    public DialogueCharacterProfile(string characterId, string displayName, string buyerOrigin, string buyerPersonality)
    {
        this.characterId = characterId;
        this.displayName = displayName;
        this.buyerOrigin = buyerOrigin;
        this.buyerPersonality = buyerPersonality;
    }
}

public class DialogueCharacterRegistry
{
    private readonly Dictionary<string, CharacterDialogueSet> characterSets;
    private readonly List<DialogueCharacterProfile> supportedProfiles;

    public CharacterDialogueSet GenericSet { get; private set; }

    public DialogueCharacterRegistry()
    {
        CharacterDialogueSet abdulRahman = AbdulRahmanDialogue.Create();
        CharacterDialogueSet francisco = FranciscoDialogue.Create();
        CharacterDialogueSet lakshmiAmma = LakshmiAmmaDialogue.Create();
        CharacterDialogueSet chinappaNaik = ChinappaNaikDialogue.Create();
        CharacterDialogueSet siddharthChetti = SiddharthChettiDialogue.Create();
        CharacterDialogueSet fatherPenteado = FatherPenteadoDialogue.Create();

        supportedProfiles = new List<DialogueCharacterProfile>
        {
            new DialogueCharacterProfile("abdul_rahman", "Abdul Rahman", "Arab Caravan Trader", "Friendly"),
            new DialogueCharacterProfile("francisco_de_almeida", "Francisco de Almeida", "Portuguese Trade Agent", "Strict"),
            new DialogueCharacterProfile("lakshmi_amma", "Lakshmi Amma", "Local Household Buyer", "Friendly"),
            new DialogueCharacterProfile("chinappa_naik", "Chinappa Naik", "Vijayanagara Wholesale Buyer", "Strict"),
            new DialogueCharacterProfile("siddharth_chetti", "Siddharth Chetti", "Local Retail Shopkeeper", "Friendly"),
            new DialogueCharacterProfile("father_penteado", "Father Penteado", "Jesuit Missionary", "Normal")
        };

        GenericSet = new CharacterDialogueSet(
            "generic",
            "Generic Bazaar Buyer",
            NpcPersonalityBucket.Normal,
            CreateGenericLines());

        characterSets = new Dictionary<string, CharacterDialogueSet>(StringComparer.OrdinalIgnoreCase)
        {
            { abdulRahman.characterId, abdulRahman },
            { francisco.characterId, francisco },
            { lakshmiAmma.characterId, lakshmiAmma },
            { chinappaNaik.characterId, chinappaNaik },
            { siddharthChetti.characterId, siddharthChetti },
            { fatherPenteado.characterId, fatherPenteado }
        };
    }

    public CharacterDialogueSet FindCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        CharacterDialogueSet result;
        return characterSets.TryGetValue(characterId, out result) ? result : null;
    }

    public bool IsRegisteredCharacterId(string characterId)
    {
        return !string.IsNullOrWhiteSpace(characterId) && characterSets.ContainsKey(characterId);
    }

    public DialogueCharacterProfile GetRandomRegisteredCharacter()
    {
        if (supportedProfiles.Count == 0)
        {
            return null;
        }

        return supportedProfiles[UnityEngine.Random.Range(0, supportedProfiles.Count)];
    }

    public DialogueCharacterProfile GetRegisteredCharacterOrRandom(string characterId)
    {
        for (int i = 0; i < supportedProfiles.Count; i++)
        {
            if (string.Equals(supportedProfiles[i].characterId, characterId, StringComparison.OrdinalIgnoreCase))
            {
                return supportedProfiles[i];
            }
        }

        return GetRandomRegisteredCharacter();
    }

    // Add new customer characters by creating a CharacterDialogueSet and registering it in DialogueCharacterRegistry.
    public IReadOnlyList<DialogueCharacterProfile> GetSupportedCharacterProfiles()
    {
        return supportedProfiles;
    }

    public static string NormalizeCharacterId(string buyerName)
    {
        string normalized = (buyerName ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.Contains("abdul rahman"))
        {
            return "abdul_rahman";
        }

        if (normalized.Contains("francisco"))
        {
            return "francisco_de_almeida";
        }

        if (normalized.Contains("lakshmi"))
        {
            return "lakshmi_amma";
        }

        if (normalized.Contains("chinappa"))
        {
            return "chinappa_naik";
        }

        if (normalized.Contains("siddharth"))
        {
            return "siddharth_chetti";
        }

        if (normalized.Contains("penteado") || normalized.Contains("father"))
        {
            return "father_penteado";
        }

        return string.Empty;
    }

    private static List<DialogueLine> CreateGenericLines()
    {
        return new List<DialogueLine>
        {
            new DialogueLine(DialogueScenario.CustomerGreeting, new[]
            {
                "Greetings, merchant. I seek {quantityLabel} of {spiceName}.",
                "Good day. I have come for {quantityLabel} of {spiceName}."
            }),
            new DialogueLine(DialogueScenario.RepeatCustomerGreeting, new[]
            {
                "We have already met, merchant. Let us return to the bargain."
            }),
            new DialogueLine(DialogueScenario.AskWhatBuyerWants, new[]
            {
                "I am here to buy {quantityLabel} of {spiceName}.",
                "The {spiceName} is what brings me to your stall."
            }),
            new DialogueLine(DialogueScenario.AskBuyerBudget, new[]
            {
                "My offer stands at {currentBuyerOffer} {currency} for {quantityLabel}.",
                "I can pay {currentBuyerOffer} {currency} at present."
            }),
            new DialogueLine(DialogueScenario.AskQuantity, new[]
            {
                "State the quantity clearly before we settle the price.",
                "How much {spiceName} do you mean to sell?"
            }),
            new DialogueLine(DialogueScenario.SellerPriceTooHigh, new[]
            {
                "{offeredPrice} {currency} is far above what I can justify for {spiceName}.",
                "That price is too high for me. I cannot follow you there."
            }),
            new DialogueLine(DialogueScenario.SellerPriceSlightlyHigh, new[]
            {
                "You ask a little too much, merchant. I can only move toward {counterPrice} {currency}.",
                "That is above my reach. Let us come nearer to {counterPrice} {currency}."
            }),
            new DialogueLine(DialogueScenario.SellerPriceAccepted, new[]
            {
                "That price is acceptable. We can conclude this bargain.",
                "Very well. That sits within reason for me."
            }),
            new DialogueLine(DialogueScenario.SellerPriceBelowExpected, new[]
            {
                "That is below what I expected to hear from you.",
                "Your number is lower than I prepared for, merchant."
            }),
            new DialogueLine(DialogueScenario.BuyerCounterFirst, new[]
            {
                "My first counter is {counterPrice} {currency}. Consider it carefully.",
                "I begin at {counterPrice} {currency} for this {spiceName}."
            }),
            new DialogueLine(DialogueScenario.BuyerCounterMiddle, new[]
            {
                "We are narrowing the gap. I can offer {counterPrice} {currency}.",
                "I will come to {counterPrice} {currency}, but not freely."
            }),
            new DialogueLine(DialogueScenario.BuyerCounterFinal, new[]
            {
                "This is my last meaningful step: {counterPrice} {currency}.",
                "I am nearly at my edge. {counterPrice} {currency} is my final counter."
            }),
            new DialogueLine(DialogueScenario.BuyerHoldsFirm, new[]
            {
                "I hold firm at {currentBuyerOffer} {currency}.",
                "My offer does not move beyond {currentBuyerOffer} {currency}."
            }),
            new DialogueLine(DialogueScenario.PlayerAcceptedDeal, new[]
            {
                "Then we are agreed at {finalPrice} {currency}.",
                "Good. We will settle at {finalPrice} {currency}."
            }),
            new DialogueLine(DialogueScenario.PlayerRejectedBuyerOffer, new[]
            {
                "Then you refuse my offer.",
                "So be it. You reject the bargain I put forward."
            }),
            new DialogueLine(DialogueScenario.HistoryQuestion, new[]
            {
                "There is time for stories later. First, let us finish this trade.",
                "History can wait, merchant. Speak of {spiceName} and price."
            }),
            new DialogueLine(DialogueScenario.SocialGreeting, new[]
            {
                "A courteous word is welcome, but trade is why I am here.",
                "Greetings to you as well. Now let us return to the bargain."
            }),
            new DialogueLine(DialogueScenario.OffTopic, new[]
            {
                "Let us keep to the trade, merchant.",
                "Speak of price or quantity, not of other matters."
            }),
            new DialogueLine(DialogueScenario.UnclearSpeech, new[]
            {
                "I did not catch that clearly. {ruleReply}",
                "Your meaning is not clear to me. {ruleReply}"
            }),
            new DialogueLine(DialogueScenario.TransactionSuccess, new[]
            {
                "The trade is complete and honourably settled."
            }),
            new DialogueLine(DialogueScenario.TransactionFailure, new[]
            {
                "Then this bargain ends without a sale.",
                "We part without agreement today."
            }),
            new DialogueLine(DialogueScenario.TimePressure, new[]
            {
                "My patience is spent. Decide now or we are done.",
                "Time runs thin, merchant. I cannot linger longer."
            }),
            new DialogueLine(DialogueScenario.Unknown, new[]
            {
                "{ruleReply}"
            })
        };
    }
}
