using System;
using System.Collections.Generic;
using UnityEngine;

// Standalone VR dialogue system uses deterministic rule engine + expandable dialogue tables.
// Characters can be expanded by adding new CharacterDialogueSet files.
// AI backend can later replace or augment this text layer, but is not part of this task.
public class DialogueTableResponseProvider
{
    private sealed class DialogueContext
    {
        public string characterId;
        public string characterName;
        public DialogueScenario scenario;
        public PlayerReputationBucket reputationBucket;
        public NpcPatienceBucket patienceBucket;
        public NpcDesperationBucket desperationBucket;
        public RoundBucket roundBucket;
        public NpcPersonalityBucket personalityBucket;
        public string buyerName;
        public string spiceName;
        public string quantityLabel;
        public int offeredPrice;
        public int counterPrice;
        public int finalPrice;
        public int currentBuyerOffer;
        public int minimumPrice;
        public string currency;
        public int round;
        public int reputation;
        public int patience;
        public float desperation;
        public string ruleReply;
    }

    private readonly DialogueCharacterRegistry registry = new DialogueCharacterRegistry();

    public string GetGreeting(LocalTradeState trade, int reputation, bool isRepeatCustomer)
    {
        if (trade == null)
        {
            return string.Empty;
        }

        try
        {
            DialogueContext context = new DialogueContext
            {
                characterId = DialogueCharacterRegistry.NormalizeCharacterId(trade.buyerName),
                characterName = !string.IsNullOrWhiteSpace(trade.buyerName) ? trade.buyerName : "Customer",
                scenario = isRepeatCustomer ? DialogueScenario.RepeatCustomerGreeting : DialogueScenario.CustomerGreeting,
                reputationBucket = ToReputationBucket(reputation),
                patienceBucket = ToPatienceBucket(trade.buyerPatience),
                desperationBucket = ToDesperationBucket(trade.buyerDesperation),
                roundBucket = RoundBucket.First,
                personalityBucket = ToPersonalityBucket(trade.buyerPersonality),
                buyerName = !string.IsNullOrWhiteSpace(trade.buyerName) ? trade.buyerName : "Customer",
                spiceName = !string.IsNullOrWhiteSpace(trade.spiceDisplayName) ? trade.spiceDisplayName.ToLowerInvariant() : "spice",
                quantityLabel = !string.IsNullOrWhiteSpace(trade.quantityLabel) ? trade.quantityLabel : "this lot",
                offeredPrice = trade.lastSellerPrice,
                counterPrice = trade.npcOffer,
                finalPrice = trade.npcOffer,
                currentBuyerOffer = trade.npcOffer,
                minimumPrice = trade.maxBuyerPrice,
                currency = "varahas",
                round = 1,
                reputation = reputation,
                patience = trade.buyerPatience,
                desperation = trade.buyerDesperation,
                ruleReply = "Greetings, merchant."
            };

            CharacterDialogueSet characterSet = registry.FindCharacter(context.characterId);
            string matchLevel;
            string template = FindTemplate(context, characterSet, out matchLevel);
            if (string.IsNullOrWhiteSpace(template))
            {
                return string.Empty;
            }

            return ReplacePlaceholders(template, context);
        }
        catch
        {
            return string.Empty;
        }
    }

    public string GetReply(NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult, int roundCount)
    {
        string fallbackReply = brainResult != null ? brainResult.replyText : string.Empty;
        if (brainResult == null)
        {
            return fallbackReply;
        }

        try
        {
            DialogueContext context = BuildContext(input, trade, brainResult, roundCount);
            CharacterDialogueSet characterSet = registry.FindCharacter(context.characterId);
            string matchLevel;
            string template = FindTemplate(context, characterSet, out matchLevel);

            Debug.Log("[DIALOGUE-TABLE] Character: " + (characterSet != null ? characterSet.displayName : "Generic"));
            Debug.Log("[DIALOGUE-TABLE] Scenario: " + context.scenario);
            Debug.Log("[DIALOGUE-TABLE] Reputation: " + context.reputationBucket);
            Debug.Log("[DIALOGUE-TABLE] Patience: " + context.patienceBucket);
            Debug.Log("[DIALOGUE-TABLE] Desperation: " + context.desperationBucket);
            Debug.Log("[DIALOGUE-TABLE] RoundBucket: " + context.roundBucket);
            Debug.Log("[DIALOGUE-TABLE] Match level: " + matchLevel);

            if (string.IsNullOrWhiteSpace(template))
            {
                return fallbackReply;
            }

            Debug.Log("[DIALOGUE-TABLE] Template selected: " + template);

            string finalReply = ReplacePlaceholders(template, context);
            if (string.IsNullOrWhiteSpace(finalReply))
            {
                return fallbackReply;
            }

            Debug.Log("[DIALOGUE-TABLE] Final reply: " + finalReply);
            return finalReply;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[DIALOGUE-TABLE] Provider failed, using rule reply. Reason: " + ex.Message);
            return fallbackReply;
        }
    }

    private string FindTemplate(DialogueContext context, CharacterDialogueSet characterSet, out string matchLevel)
    {
        if (characterSet != null)
        {
            string exactTemplate = TryFindTemplate(characterSet.lines, context, requireAllBuckets: true);
            if (!string.IsNullOrWhiteSpace(exactTemplate))
            {
                matchLevel = "Exact character + scenario + buckets";
                return exactTemplate;
            }

            string roundPersonalityTemplate = TryFindTemplate(characterSet.lines, context, requireAllBuckets: false, onlyRoundAndPersonality: true);
            if (!string.IsNullOrWhiteSpace(roundPersonalityTemplate))
            {
                matchLevel = "Character + scenario + round/personality";
                return roundPersonalityTemplate;
            }

            string scenarioTemplate = TryFindTemplate(characterSet.lines, context, requireAllBuckets: false);
            if (!string.IsNullOrWhiteSpace(scenarioTemplate))
            {
                matchLevel = "Character + scenario";
                return scenarioTemplate;
            }
        }

        string genericTemplate = TryFindTemplate(registry.GenericSet.lines, context, requireAllBuckets: false);
        if (!string.IsNullOrWhiteSpace(genericTemplate))
        {
            matchLevel = "Generic scenario";
            return genericTemplate;
        }

        matchLevel = "Rule reply fallback";
        return null;
    }

    private static string TryFindTemplate(List<DialogueLine> lines, DialogueContext context, bool requireAllBuckets, bool onlyRoundAndPersonality = false)
    {
        if (lines == null)
        {
            return null;
        }

        List<string> matches = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            DialogueLine line = lines[i];
            if (line == null || line.scenario != context.scenario)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(line.characterId) &&
                !string.Equals(line.characterId, context.characterId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (onlyRoundAndPersonality)
            {
                if (!MatchesBucket(line.roundBucket, context.roundBucket) ||
                    !MatchesBucket(line.personalityBucket, context.personalityBucket))
                {
                    continue;
                }
            }
            else if (requireAllBuckets)
            {
                if (!line.reputationBucket.HasValue ||
                    !line.patienceBucket.HasValue ||
                    !line.desperationBucket.HasValue ||
                    !line.roundBucket.HasValue ||
                    !line.personalityBucket.HasValue)
                {
                    continue;
                }

                if (!MatchesBucket(line.reputationBucket, context.reputationBucket) ||
                    !MatchesBucket(line.patienceBucket, context.patienceBucket) ||
                    !MatchesBucket(line.desperationBucket, context.desperationBucket) ||
                    !MatchesBucket(line.roundBucket, context.roundBucket) ||
                    !MatchesBucket(line.personalityBucket, context.personalityBucket))
                {
                    continue;
                }
            }

            if (line.templates == null || line.templates.Length == 0)
            {
                continue;
            }

            for (int templateIndex = 0; templateIndex < line.templates.Length; templateIndex++)
            {
                if (!string.IsNullOrWhiteSpace(line.templates[templateIndex]))
                {
                    matches.Add(line.templates[templateIndex]);
                }
            }
        }

        if (matches.Count == 0)
        {
            return null;
        }

        return matches[UnityEngine.Random.Range(0, matches.Count)];
    }

    private static bool MatchesBucket<T>(T? lineValue, T contextValue) where T : struct
    {
        return !lineValue.HasValue || EqualityComparer<T>.Default.Equals(lineValue.Value, contextValue);
    }

    private static DialogueContext BuildContext(NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult, int roundCount)
    {
        int reputation = Level1GameState.Instance != null ? Level1GameState.Instance.CurrentReputation : PlayerState.DefaultReputation;
        int patience = trade != null ? trade.buyerPatience : 5;
        float desperation = trade != null ? trade.buyerDesperation : 0.5f;
        int offeredPrice = input != null && input.hasSellerPrice ? input.sellerPrice : (trade != null ? trade.lastSellerPrice : 0);
        int currentBuyerOffer = trade != null ? trade.npcOffer : brainResult.updatedOffer;
        int counterPrice = brainResult.updatedOffer > 0 ? brainResult.updatedOffer : currentBuyerOffer;
        int minimumPrice = trade != null ? trade.maxBuyerPrice : counterPrice;
        int finalPrice = brainResult.resolvedPrice > 0 ? brainResult.resolvedPrice : counterPrice;

        return new DialogueContext
        {
            characterId = DialogueCharacterRegistry.NormalizeCharacterId(trade != null ? trade.buyerName : string.Empty),
            characterName = !string.IsNullOrWhiteSpace(trade != null ? trade.buyerName : string.Empty) ? trade.buyerName : "Customer",
            scenario = MapScenario(input, trade, brainResult, roundCount, offeredPrice),
            reputationBucket = ToReputationBucket(reputation),
            patienceBucket = ToPatienceBucket(patience),
            desperationBucket = ToDesperationBucket(desperation),
            roundBucket = ToRoundBucket(roundCount),
            personalityBucket = ToPersonalityBucket(trade != null ? trade.buyerPersonality : string.Empty),
            buyerName = !string.IsNullOrWhiteSpace(trade != null ? trade.buyerName : string.Empty) ? trade.buyerName : "Customer",
            spiceName = !string.IsNullOrWhiteSpace(trade != null ? trade.spiceDisplayName : string.Empty) ? trade.spiceDisplayName.ToLowerInvariant() : "spice",
            quantityLabel = !string.IsNullOrWhiteSpace(trade != null ? trade.quantityLabel : string.Empty) ? trade.quantityLabel : "this lot",
            offeredPrice = offeredPrice,
            counterPrice = counterPrice,
            finalPrice = finalPrice,
            currentBuyerOffer = currentBuyerOffer,
            minimumPrice = minimumPrice,
            currency = "varahas",
            round = Mathf.Max(1, roundCount),
            reputation = reputation,
            patience = patience,
            desperation = desperation,
            ruleReply = !string.IsNullOrWhiteSpace(brainResult.replyText) ? brainResult.replyText : "Speak plainly, merchant."
        };
    }

    private static DialogueScenario MapScenario(NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult, int roundCount, int offeredPrice)
    {
        NegotiationIntent intent = input != null ? input.intent : NegotiationIntent.UNKNOWN;
        bool hasOfferedPrice = input != null && input.hasSellerPrice;
        bool offerMovedUp = trade != null && brainResult.updatedOffer > trade.npcOffer;
        bool buyerHeldFirm = trade != null && brainResult.updatedOffer <= trade.npcOffer;
        bool isDemoMode = Level1GameState.Instance != null && Level1GameState.Instance.IsPrerecordedVoiceDemoModeEnabled;

        if (brainResult.isAccepted || string.Equals(brainResult.resolutionAction, "ACCEPT", StringComparison.OrdinalIgnoreCase))
        {
            return roundCount <= 1 ? DialogueScenario.SellerPriceAccepted : DialogueScenario.PlayerAcceptedDeal;
        }

        if (brainResult.walkedAway || string.Equals(brainResult.resolutionAction, "WALK_AWAY", StringComparison.OrdinalIgnoreCase))
        {
            if (trade != null && trade.buyerPatience <= 1)
            {
                return DialogueScenario.TimePressure;
            }

            return DialogueScenario.TransactionFailure;
        }

        switch (intent)
        {
            case NegotiationIntent.GREETING:
                return roundCount > 1 ? DialogueScenario.RepeatCustomerGreeting : DialogueScenario.CustomerGreeting;

            case NegotiationIntent.ITEM_QUERY:
                return DialogueScenario.AskWhatBuyerWants;

            case NegotiationIntent.QUERY_BUYER_BUDGET:
            case NegotiationIntent.PRICE_QUERY:
                if (isDemoMode && trade != null)
                {
                    if (offerMovedUp)
                    {
                        return trade.repeatedPriceQueries >= 3
                            ? DialogueScenario.BuyerCounterFinal
                            : ToCounterScenario(Mathf.Max(2, roundCount));
                    }

                    if (trade.repeatedPriceQueries >= 3 || trade.npcOffer >= trade.maxBuyerPrice)
                    {
                        return DialogueScenario.BuyerCounterFinal;
                    }
                }
                return DialogueScenario.AskBuyerBudget;

            case NegotiationIntent.QUANTITY_QUERY:
                return DialogueScenario.AskQuantity;

            case NegotiationIntent.GENERAL_DIALOGUE:
                if (input != null && !string.IsNullOrWhiteSpace(input.socialSubIntent) &&
                    (input.socialSubIntent == "GENERAL" || input.socialSubIntent == "WEATHER"))
                {
                    return DialogueScenario.HistoryQuestion;
                }
                return DialogueScenario.SocialGreeting;

            case NegotiationIntent.SOCIAL:
                return DialogueScenario.SocialGreeting;

            case NegotiationIntent.OFF_TOPIC:
            case NegotiationIntent.HOSTILE:
                return DialogueScenario.OffTopic;

            case NegotiationIntent.CLARIFICATION:
            case NegotiationIntent.UNKNOWN:
                return DialogueScenario.UnclearSpeech;
        }

        if (hasOfferedPrice && trade != null)
        {
            if (offeredPrice > trade.maxBuyerPrice)
            {
                return DialogueScenario.SellerPriceTooHigh;
            }

            if (offeredPrice > trade.npcOffer)
            {
                if (offerMovedUp)
                {
                    return ToCounterScenario(roundCount);
                }

                if (buyerHeldFirm)
                {
                    return offeredPrice - trade.npcOffer <= Mathf.Max(5, trade.minIncrement * 2)
                        ? DialogueScenario.SellerPriceSlightlyHigh
                        : DialogueScenario.BuyerHoldsFirm;
                }
            }

            if (offeredPrice < trade.npcOffer)
            {
                if (offerMovedUp)
                {
                    return ToCounterScenario(roundCount);
                }

                return DialogueScenario.SellerPriceBelowExpected;
            }
        }

        if (offerMovedUp)
        {
            return ToCounterScenario(roundCount);
        }

        if (buyerHeldFirm &&
            (intent == NegotiationIntent.PRICE || intent == NegotiationIntent.COUNTER || intent == NegotiationIntent.BARGAIN || intent == NegotiationIntent.QUANTITY_PRICE || intent == NegotiationIntent.ULTIMATUM))
        {
            return DialogueScenario.BuyerHoldsFirm;
        }

        return DialogueScenario.Unknown;
    }

    private static DialogueScenario ToCounterScenario(int roundCount)
    {
        if (roundCount <= 1)
        {
            return DialogueScenario.BuyerCounterFirst;
        }

        if (roundCount >= 4)
        {
            return DialogueScenario.BuyerCounterFinal;
        }

        return DialogueScenario.BuyerCounterMiddle;
    }

    private static PlayerReputationBucket ToReputationBucket(int reputation)
    {
        if (reputation <= 35)
        {
            return PlayerReputationBucket.Low;
        }

        if (reputation >= 70)
        {
            return PlayerReputationBucket.High;
        }

        return PlayerReputationBucket.Neutral;
    }

    private static NpcPatienceBucket ToPatienceBucket(int patience)
    {
        if (patience <= 2)
        {
            return NpcPatienceBucket.Low;
        }

        if (patience >= 6)
        {
            return NpcPatienceBucket.High;
        }

        return NpcPatienceBucket.Medium;
    }

    private static NpcDesperationBucket ToDesperationBucket(float desperation)
    {
        if (desperation <= 0.35f)
        {
            return NpcDesperationBucket.Low;
        }

        if (desperation >= 0.67f)
        {
            return NpcDesperationBucket.High;
        }

        return NpcDesperationBucket.Medium;
    }

    private static RoundBucket ToRoundBucket(int roundCount)
    {
        if (roundCount <= 1)
        {
            return RoundBucket.First;
        }

        if (roundCount >= 4)
        {
            return RoundBucket.Final;
        }

        return RoundBucket.Middle;
    }

    private static NpcPersonalityBucket ToPersonalityBucket(string personality)
    {
        switch ((personality ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "friendly":
                return NpcPersonalityBucket.Friendly;
            case "strict":
                return NpcPersonalityBucket.Strict;
            case "impatient":
                return NpcPersonalityBucket.Impatient;
            default:
                return NpcPersonalityBucket.Normal;
        }
    }

    private static string ReplacePlaceholders(string template, DialogueContext context)
    {
        if (string.IsNullOrWhiteSpace(template) || context == null)
        {
            return string.Empty;
        }

        return template
            .Replace("{buyerName}", Safe(context.buyerName, "Customer"))
            .Replace("{characterName}", Safe(context.characterName, "Customer"))
            .Replace("{spiceName}", Safe(context.spiceName, "spice"))
            .Replace("{quantityLabel}", Safe(context.quantityLabel, "this lot"))
            .Replace("{offeredPrice}", SafeNumber(context.offeredPrice, context.currentBuyerOffer))
            .Replace("{counterPrice}", SafeNumber(context.counterPrice, context.currentBuyerOffer))
            .Replace("{finalPrice}", SafeNumber(context.finalPrice, context.counterPrice))
            .Replace("{currentBuyerOffer}", SafeNumber(context.currentBuyerOffer, context.counterPrice))
            .Replace("{minimumPrice}", SafeNumber(context.minimumPrice, context.counterPrice))
            .Replace("{currency}", Safe(context.currency, "varahas"))
            .Replace("{round}", context.round.ToString())
            .Replace("{reputation}", context.reputation.ToString())
            .Replace("{patience}", context.patience.ToString())
            .Replace("{desperation}", context.desperation.ToString("0.00"))
            .Replace("{ruleReply}", Safe(context.ruleReply, "Speak plainly, merchant."));
    }

    private static string Safe(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string SafeNumber(int value, int fallback)
    {
        return Mathf.Max(value > 0 ? value : fallback, 0).ToString();
    }
}
