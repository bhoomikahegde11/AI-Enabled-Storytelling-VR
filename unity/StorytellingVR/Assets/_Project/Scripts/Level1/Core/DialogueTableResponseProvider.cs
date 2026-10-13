using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    private LocalTradeState recentCharacterTrade;
    private readonly Queue<string> recentCharacterTemplates = new Queue<string>();

    public static bool IsLakshmi(LocalTradeState trade)
    {
        return trade != null && DialogueCharacterRegistry.NormalizeCharacterId(trade.buyerName) == "lakshmi_amma";
    }

    public static bool UsesAuthoredPresentation(LocalTradeState trade)
    {
        return trade != null && UsesAuthoredPresentation(DialogueCharacterRegistry.NormalizeCharacterId(trade.buyerName));
    }

    public static bool UsesAuthoredPresentation(string characterId)
    {
        return characterId == "lakshmi_amma" || characterId == "chinnamma_naik" ||
            characterId == "saraswati_chetti" || characterId == "abdul_rahman" ||
            characterId == "francisco_de_almeida" || characterId == "father_penteado";
    }

    // These hooks render existing events; they do not start or advance negotiation/idle stages.
    public string GetIdleReminder(LocalTradeState trade, string fallback, int stage, bool terminal)
    {
        if (!UsesAuthoredPresentation(trade)) return fallback;
        return GetPresentation(trade, terminal ? DialogueScenario.TransactionFailure :
            stage == 2 ? DialogueScenario.IdleReminderSecond : DialogueScenario.IdleReminderFirst, fallback);
    }

    public string GetTransactionSuccess(LocalTradeState trade)
    {
        if (!UsesAuthoredPresentation(trade) || trade.AcceptedTerms == null || !trade.SettlementClaimed) return string.Empty;
        return GetPresentation(trade, DialogueScenario.TransactionSuccess, "Thank you, merchant.");
    }

    private string GetPresentation(LocalTradeState trade, DialogueScenario scenario, string fallback)
    {
        try
        {
            PrepareCharacterHistory(trade);
            DialogueContext context = BuildContext(null, trade, new RuleBasedNPCBrainResult { replyText = fallback }, 1);
            context.scenario = scenario;
            string level;
            string template = FindTemplate(context, registry.FindCharacter(context.characterId), out level);
            return string.IsNullOrWhiteSpace(template) ? fallback : ReplacePlaceholders(template, context);
        }
        catch (Exception ex)
        {
            if (Level1DebugForceAccept.VerboseLogsEnabled())
                Debug.LogWarning("[DIALOGUE-TABLE] Presentation fallback: " + ex.Message);
            return fallback;
        }
    }

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

            PrepareCharacterHistory(trade);
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
        if (brainResult == null || (brainResult.requiresClarification && !UsesAuthoredPresentation(trade)))
        {
            return fallbackReply;
        }

        try
        {
            DialogueContext context = BuildContext(input, trade, brainResult, roundCount);
            PrepareCharacterHistory(trade);
            if (UsesAuthoredPresentation(trade))
                context.scenario = MapAuthoredScenario(input, brainResult, context.scenario, context.characterId);
            CharacterDialogueSet characterSet = registry.FindCharacter(context.characterId);
            string matchLevel;
            string template = FindTemplate(context, characterSet, out matchLevel);

            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Character: " + (characterSet != null ? characterSet.displayName : "Generic"));
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Scenario: " + context.scenario);
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Reputation: " + context.reputationBucket);
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Patience: " + context.patienceBucket);
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Desperation: " + context.desperationBucket);
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] RoundBucket: " + context.roundBucket);
            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Match level: " + matchLevel);

            if (string.IsNullOrWhiteSpace(template))
            {
                return fallbackReply;
            }

            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Template selected: " + template);

            string finalReply = ReplacePlaceholders(template, context);
            if (string.IsNullOrWhiteSpace(finalReply))
            {
                return fallbackReply;
            }

            Level1DebugForceAccept.LogVerbose("[DIALOGUE-TABLE] Final reply: " + finalReply);
            return finalReply;
        }
        catch (Exception ex)
        {
            if (Level1DebugForceAccept.VerboseLogsEnabled())
            {
                Debug.LogWarning("[DIALOGUE-TABLE] Provider failed, using rule reply. Reason: " + ex.Message);
            }
            return fallbackReply;
        }
    }

    private string FindTemplate(DialogueContext context, CharacterDialogueSet characterSet, out string matchLevel)
    {
        if (UsesAuthoredPresentation(context.characterId) && characterSet != null)
        {
            matchLevel = "Character matching restrictions + recent variation";
            return FindRestrictedTemplate(characterSet.lines, context);
        }

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

    private void PrepareCharacterHistory(LocalTradeState trade)
    {
        if (ReferenceEquals(recentCharacterTrade, trade)) return;
        recentCharacterTrade = trade;
        recentCharacterTemplates.Clear();
    }

    private string FindRestrictedTemplate(List<DialogueLine> lines, DialogueContext context)
    {
        var highMatches = new List<string>();
        var moodMatches = new List<string>();
        var genericMatches = new List<string>();

        foreach (DialogueLine line in lines)
        {
            if (line == null || line.scenario != context.scenario ||
                (!string.IsNullOrEmpty(line.characterId) && line.characterId != context.characterId) ||
                !MatchesBucket(line.reputationBucket, context.reputationBucket) ||
                !MatchesBucket(line.patienceBucket, context.patienceBucket) ||
                !MatchesBucket(line.desperationBucket, context.desperationBucket) ||
                !MatchesBucket(line.roundBucket, context.roundBucket) ||
                !MatchesBucket(line.personalityBucket, context.personalityBucket) || line.templates == null)
                continue;

            // Low patience takes precedence over a general personality variant, never the reverse.
            int specificity = (line.patienceBucket.HasValue ? 2 : 0) +
                (line.personalityBucket.HasValue ? 1 : 0) + (line.roundBucket.HasValue ? 1 : 0) +
                (line.reputationBucket.HasValue ? 1 : 0) + (line.desperationBucket.HasValue ? 1 : 0);

            if (specificity >= 2)
            {
                foreach (string template in line.templates)
                    if (!string.IsNullOrWhiteSpace(template) && !highMatches.Contains(template)) highMatches.Add(template);
            }
            else if (specificity == 1)
            {
                foreach (string template in line.templates)
                    if (!string.IsNullOrWhiteSpace(template) && !moodMatches.Contains(template)) moodMatches.Add(template);
            }
            else
            {
                foreach (string template in line.templates)
                    if (!string.IsNullOrWhiteSpace(template) && !genericMatches.Contains(template)) genericMatches.Add(template);
            }
        }

        // When low patience or high-specificity constraint matches, strictly use those matches.
        if (highMatches.Count > 0)
        {
            return SelectFromBucket(highMatches);
        }

        // Only HOLD blends mood and generic wording. Other scenarios keep their mood-specific priority.
        if (context.scenario == DialogueScenario.BuyerHoldsFirm && moodMatches.Count > 0 && genericMatches.Count > 0)
        {
            List<string> unusedMood = moodMatches.FindAll(t => !recentCharacterTemplates.Contains(t));
            List<string> unusedGeneric = genericMatches.FindAll(t => !recentCharacterTemplates.Contains(t));

            string selectedTemplate = null;
            if (unusedMood.Count > 0 && unusedGeneric.Count > 0)
            {
                selectedTemplate = UnityEngine.Random.value < 0.70f
                    ? unusedMood[UnityEngine.Random.Range(0, unusedMood.Count)]
                    : unusedGeneric[UnityEngine.Random.Range(0, unusedGeneric.Count)];
            }
            else if (unusedMood.Count > 0)
            {
                selectedTemplate = unusedMood[UnityEngine.Random.Range(0, unusedMood.Count)];
            }
            else if (unusedGeneric.Count > 0)
            {
                selectedTemplate = unusedGeneric[UnityEngine.Random.Range(0, unusedGeneric.Count)];
            }

            if (selectedTemplate != null)
            {
                RecordRecentTemplate(selectedTemplate);
                return selectedTemplate;
            }

            var combined = new List<string>(moodMatches);
            foreach (var g in genericMatches)
            {
                if (!combined.Contains(g)) combined.Add(g);
            }
            return SelectFromBucket(combined);
        }

        if (moodMatches.Count > 0)
        {
            return SelectFromBucket(moodMatches);
        }

        if (genericMatches.Count > 0)
        {
            return SelectFromBucket(genericMatches);
        }

        return null; // Preserve precise rule reply if no safe character line exists.
    }

    private string SelectFromBucket(List<string> candidates)
    {
        List<string> unused;
        do
        {
            unused = candidates.FindAll(template => !recentCharacterTemplates.Contains(template));
            if (unused.Count == 0) recentCharacterTemplates.Dequeue();
        } while (unused.Count == 0);

        string selected = unused[UnityEngine.Random.Range(0, unused.Count)];
        RecordRecentTemplate(selected);
        return selected;
    }

    private void RecordRecentTemplate(string selected)
    {
        recentCharacterTemplates.Enqueue(selected);
        while (recentCharacterTemplates.Count > 4) recentCharacterTemplates.Dequeue();
    }

    private static DialogueScenario MapAuthoredScenario(NegotiationInput input, RuleBasedNPCBrainResult result, DialogueScenario scenario, string characterId)
    {
        if (result.requiresClarification) return DialogueScenario.UnclearSpeech;
        if (result.isFinished || result.isAccepted || result.walkedAway || input == null) return scenario;
        switch (input.intent)
        {
            case NegotiationIntent.GREETING:
                return DialogueScenario.SocialGreeting; // A greeting mid-visit is not a repeat customer visit.
            case NegotiationIntent.ITEM_QUERY:
                if (result.nextExpectedReplyState == ExpectedReplyState.ExpectOfferPrice) return DialogueScenario.AskSellerPrice;
                return input.asksCurrentOffer ? DialogueScenario.AskBuyerBudget : DialogueScenario.AskWhatBuyerWants;
            case NegotiationIntent.PRICE_QUERY:
                return input.asksReason && !input.asksItem ? DialogueScenario.OfferReason : DialogueScenario.AskBuyerBudget;
            case NegotiationIntent.QUERY_BUYER_BUDGET:
                // Price-pressure questions already carry asksReason; preserve Lakshmi's verified mapping.
                return (characterId == "chinnamma_naik" || characterId == "saraswati_chetti" ||
                    characterId == "abdul_rahman" || characterId == "francisco_de_almeida" ||
                    characterId == "father_penteado") && input.asksReason
                    ? DialogueScenario.OfferReason : scenario;
            case NegotiationIntent.CONTINUE:
                return DialogueScenario.AskBuyerBudget;
            case NegotiationIntent.QUANTITY_CHANGE:
                return result.commitsQuantity ? DialogueScenario.QuantityChanged : DialogueScenario.Unknown;
            case NegotiationIntent.REJECT:
                return result.updatedOffer > result.previousOffer ? scenario : DialogueScenario.PlayerRejectedBuyerOffer;
            case NegotiationIntent.HOSTILE:
                return DialogueScenario.HostileWarning;
            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.GENERAL_DIALOGUE:
                string text = input.normalizedText ?? string.Empty;
                if (HasWord(text, "weather|rain|sun|sunshine")) return DialogueScenario.WeatherQuestion;
                if (HasWord(text, "name") || Regex.IsMatch(text, @"^who\s+are(?:\s+you)?$", RegexOptions.IgnoreCase)) return DialogueScenario.AskName;
                if (HasWord(text, "king|krishnadevaraya")) return DialogueScenario.HistoryKing;
                if (HasWord(text, "temple|virupaksha")) return DialogueScenario.HistoryTemple;
                if (HasWord(text, "where|origin|from")) return DialogueScenario.AskOrigin;
                if (HasWord(text, "history|city|hampi|vijayanagara|market|bazaar")) return DialogueScenario.HistoryQuestion;
                return DialogueScenario.SocialGreeting;
        }
        return scenario;
    }

    private static bool HasWord(string text, string words)
    {
        return Regex.IsMatch(text, @"\b(" + words + @")\b", RegexOptions.IgnoreCase);
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
        int reputation = Level1GameState.ExistingInstance != null ? Level1GameState.ExistingInstance.CurrentReputation : PlayerState.DefaultReputation;
        int patience = trade != null ? trade.buyerPatience : 5;
        float desperation = trade != null ? trade.buyerDesperation : 0.5f;
        int offeredPrice = input != null && input.hasSellerPrice ? input.sellerPrice : (trade != null ? trade.lastSellerPrice : 0);
        int currentBuyerOffer = trade != null ? trade.npcOffer : brainResult.updatedOffer;
        int counterPrice = brainResult.updatedOffer > 0 ? brainResult.updatedOffer : currentBuyerOffer;
        int minimumPrice = trade != null ? trade.maxBuyerPrice : counterPrice;
        int finalPrice = brainResult.resolvedPrice > 0 ? brainResult.resolvedPrice : counterPrice;

        TradeTermsSnapshot accepted = UsesAuthoredPresentation(trade) ? brainResult.acceptedTerms ?? trade.AcceptedTerms : null;
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
            spiceName = accepted != null ? Safe(accepted.SpiceName, "spice").ToLowerInvariant() :
                !string.IsNullOrWhiteSpace(trade != null ? trade.spiceDisplayName : string.Empty) ? trade.spiceDisplayName.ToLowerInvariant() : "spice",
            quantityLabel = accepted != null ? accepted.QuantityLabel :
                !string.IsNullOrWhiteSpace(trade != null ? trade.quantityLabel : string.Empty) ? trade.quantityLabel : "this lot",
            offeredPrice = offeredPrice,
            counterPrice = counterPrice,
            finalPrice = accepted != null ? accepted.Price : finalPrice,
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
        bool offerMovedUp = brainResult.updatedOffer > brainResult.previousOffer;
        bool buyerHeldFirm = brainResult.updatedOffer <= brainResult.previousOffer;

        if (brainResult.isAccepted || string.Equals(brainResult.resolutionAction, "ACCEPT", StringComparison.OrdinalIgnoreCase))
        {
            return intent == NegotiationIntent.ACCEPT ? DialogueScenario.PlayerAcceptedDeal : DialogueScenario.SellerPriceAccepted;
        }

        if (brainResult.walkedAway || string.Equals(brainResult.resolutionAction, "WALK_AWAY", StringComparison.OrdinalIgnoreCase))
        {
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
                if (offerMovedUp)
                {
                    return ToCounterScenario(roundCount);
                }

                return offeredPrice - trade.maxBuyerPrice <= Mathf.Max(6, trade.minIncrement * 3)
                    ? DialogueScenario.SellerPriceSlightlyHigh
                    : DialogueScenario.SellerPriceTooHigh;
            }

            if (offeredPrice > brainResult.previousOffer)
            {
                if (offerMovedUp)
                {
                    return ToCounterScenario(roundCount);
                }

                if (buyerHeldFirm)
                {
                    return offeredPrice - brainResult.previousOffer <= Mathf.Max(5, trade.minIncrement * 2)
                        ? DialogueScenario.SellerPriceSlightlyHigh
                        : DialogueScenario.BuyerHoldsFirm;
                }
            }

            if (offeredPrice < brainResult.previousOffer)
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

        if (roundCount >= 5)
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

        if (roundCount >= 5)
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
