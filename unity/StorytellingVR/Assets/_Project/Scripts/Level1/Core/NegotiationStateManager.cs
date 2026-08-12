using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum NegotiationIntent
{
    ACCEPT,
    REJECT,
    DISMISS,
    BARGAIN,
    PRICE_QUERY,
    ITEM_QUERY,
    QUANTITY_QUERY,
    GREETING,
    OFF_TOPIC,
    UNKNOWN,
    PRICE,
    COUNTER,
    QUERY_BUYER_BUDGET,
    QUANTITY_PRICE,
    QUANTITY_CHANGE,
    ULTIMATUM,
    SOCIAL,
    HOSTILE,
    GENERAL_DIALOGUE,
    CLARIFICATION,
    CONFUSED,
    CONTINUE
}

public enum ParseConfidence
{
    Low,
    Medium,
    High
}

public enum ParseReason
{
    Unknown,
    EmptyInput,
    EmptyTranscript,
    LlmInterpreted,
    PromptInjection,
    OffTopicModern,
    Hostile,
    PureAcceptance,
    HardRejection,
    DismissTrade,
    SoftRejection,
    PriceOfferParsed,
    QuantityAnswerParsed,
    BuyerBudgetQuery,
    ItemQuery,
    QuantityQuery,
    PriceQuery,
    Greeting,
    GeneralDialogue,
    CounterLanguage,
    BargainLanguage,
    FallbackAcceptance,
    TradeVocabularyFallback,
    ShortNumericUnclear,
    ShortSocial,
    ConfusedPlayer,
    StateBlockedAccept,
    StateBlockedReject,
    FulfillmentExpected,
    AmbiguousAcceptOrCounter,
    MissingPrice,
    MissingQuantity,
    UnrecognizedSpeech,
    UnknownFallback
}

public enum ExpectedReplyState
{
    None,
    ExpectOfferPrice,
    ExpectQuantity,
    ExpectAcceptOrCounter,
    ExpectFulfillment
}

public class NegotiationInput
{
    public NegotiationIntent intent = NegotiationIntent.UNKNOWN;
    public string normalizedText = string.Empty;
    public int sellerPrice = -1;
    public int quantityGrams = -1;
    public string socialSubIntent = string.Empty;
    public bool hasSellerPrice;
    public bool hasQuantity;
    public bool hasExplicitAcceptance;
    public bool hasExplicitUltimatum;
    public bool hasHardRejection;
    public bool terminalAction;
    public bool needsClarification;
    public ParseConfidence parseConfidence = ParseConfidence.Low;
    public ParseReason parseReason = ParseReason.Unknown;
    public ExpectedReplyState expectedReplyState = ExpectedReplyState.None;
    public readonly List<NegotiationIntent> secondaryIntents = new List<NegotiationIntent>();
    public readonly List<int> referencedPrices = new List<int>();
    public readonly List<int> referencedQuantities = new List<int>();
    public int acceptanceTarget = -1;
    public int rejectedPrice = -1;
    public bool rejectsCurrentOffer;
    public bool asksItem;
    public bool asksQuantity;
    public bool asksCurrentOffer;
    public bool asksReason;
    public bool tradeOpeningQuery;
    public bool correctionDetected;
    public NegotiationTactic negotiationTactic = NegotiationTactic.NONE;
    public ClarificationKind clarificationKind = ClarificationKind.None;
    public string evidence = string.Empty;
}

public class NegotiationStateManager
{
    private static readonly string[] TradeWords =
    {
        "price", "offer", "pay", "sell", "buy", "cost", "varaha", "varahas", "deal", "trade",
        "reduce", "lower", "discount", "cheap", "cheaper", "more", "less", "higher", "quantity"
    };

    private static readonly string[] ModernWords =
    {
        "phone", "mobile", "laptop", "computer", "internet", "google", "youtube", "instagram",
        "xbox", "playstation", "wifi", "camera", "battery", "dollar", "rupee", "bitcoin",
        "online", "website", "email", "selfie", "uber", "amazon", "netflix", "app", "software",
        "robot", "electricity", "plastic", "nasa", "crypto", "credit"
    };

    private static readonly string[] HostileWords =
    {
        "idiot", "stupid", "dumb", "shut up", "get out", "leave", "scam", "thief", "liar", "greedy", "fuck",
        "bitch", "cheat", "worst", "terrible", "fool", "donkey"
    };

    public int MaxRounds { get; private set; }
    public int CurrentRound { get; private set; }
    public int BuyerPatience { get; private set; }
    public int RepeatedIntentCount { get; private set; }
    public int ConsecutiveBargains { get; private set; }
    public int ConsecutiveQueries { get; private set; }
    public int OffTopicCount { get; private set; }
    public int UnknownCount { get; private set; }
    public int LastOffer { get; private set; }
    public bool IsNegotiationFinished { get; private set; }
    public NegotiationIntent LastIntent { get; private set; } = NegotiationIntent.UNKNOWN;
    public string LastNormalizedInput { get; private set; } = string.Empty;
    public ExpectedReplyState CurrentExpectedReplyState { get; private set; } = ExpectedReplyState.None;
    public bool LastTurnCountedAsNegotiation { get; private set; }
    private ExpectedReplyState lastUsefulNegotiationReplyState = ExpectedReplyState.None;
    private bool lastNpcReplyPresentedOfferAmount;

    public void ResetState(int startingOffer, int buyerPatience = 5)
    {
        CurrentRound = 0;
        BuyerPatience = Mathf.Max(1, buyerPatience);
        MaxRounds = Mathf.Max(3, 3 + (BuyerPatience * 2));
        RepeatedIntentCount = 0;
        ConsecutiveBargains = 0;
        ConsecutiveQueries = 0;
        OffTopicCount = 0;
        UnknownCount = 0;
        LastOffer = Mathf.Max(0, startingOffer);
        IsNegotiationFinished = false;
        LastIntent = NegotiationIntent.UNKNOWN;
        LastNormalizedInput = string.Empty;
        CurrentExpectedReplyState = ExpectedReplyState.None;
        LastTurnCountedAsNegotiation = false;
        lastUsefulNegotiationReplyState = ExpectedReplyState.None;
        lastNpcReplyPresentedOfferAmount = false;
    }

    public void SetLastOffer(int offer)
    {
        LastOffer = Mathf.Max(0, offer);
    }

    public void SetExpectedReplyState(ExpectedReplyState state, string reason = "")
    {
        CurrentExpectedReplyState = state;
        if (state == ExpectedReplyState.ExpectOfferPrice ||
            state == ExpectedReplyState.ExpectQuantity ||
            state == ExpectedReplyState.ExpectAcceptOrCounter)
        {
            lastUsefulNegotiationReplyState = state;
        }
        Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] expectedReplyState=" + state + (string.IsNullOrWhiteSpace(reason) ? string.Empty : " | reason=" + reason));
    }

    public void UpdateExpectedReplyStateFromNpcReply(string npcReply, bool negotiationFinished, bool acceptedTradePending)
    {
        ExpectedReplyState previousState = CurrentExpectedReplyState;

        if (acceptedTradePending)
        {
            SetExpectedReplyState(ExpectedReplyState.ExpectFulfillment, "accepted trade pending fulfillment");
            return;
        }

        if (negotiationFinished)
        {
            SetExpectedReplyState(ExpectedReplyState.None, "negotiation finished");
            return;
        }

        string text = InputNormalizer.Normalize(npcReply ?? string.Empty, false);
        string[] tokens = string.IsNullOrEmpty(text)
            ? Array.Empty<string>()
            : text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        string patternReason;

        if (ContainsAnyPhrase(text, "how many", "what quantity", "what amount", "how much quantity"))
        {
            patternReason = "npc quantity-request pattern";
            Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] npcReplyPattern=" + patternReason + " | npcReply=" + text);
            SetExpectedReplyState(ExpectedReplyState.ExpectQuantity, patternReason);
            return;
        }

        if (ContainsAnyPhrase(text, "do we have a deal", "shall we finish", "shall we settle", "will you take", "can we settle"))
        {
            patternReason = "npc explicit accept-or-counter prompt";
            Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] npcReplyPattern=" + patternReason + " | npcReply=" + text);
            SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, patternReason);
            return;
        }

        bool presentsNpcOffer =
            ContainsAnyPhrase(text,
                "i offer",
                "i can offer",
                "can offer",
                "i pay",
                "i can pay",
                "i can do",
                "my offer is",
                "i will pay",
                "i can raise to",
                "i can raise my offer to",
                "i can make it",
                "my offer stands at",
                "i can move to",
                "my last offer",
                "my final figure",
                "my last word") ||
            MatchesAnyPattern(text,
                @"\bi offer\s+\d+",
                @"\bi can offer\s+\d+",
                @"\bcan offer\s+\d+",
                @"\bi pay\s+\d+",
                @"\bi can pay\s+\d+",
                @"\bi can do\s+\d+",
                @"\bmy offer is\s+\d+",
                @"\bi will pay\s+\d+",
                @"\bi can raise(?: my offer)? to\s+\d+",
                @"\bi can make it\s+\d+",
                @"\bmy offer stands at\s+\d+",
                @"\bi can move to\s+\d+",
                @"\bcurrent offer is\s+\d+",
                @"\b\d+\s+varahas\b");

        lastNpcReplyPresentedOfferAmount = presentsNpcOffer;

        bool requestsPlayerPrice =
            ContainsAnyPhrase(text,
                "what is your offer",
                "state your price",
                "say your price",
                "say it plainly",
                "what do you ask",
                "name your price") ||
            MatchesAnyPattern(text,
                @"\bwhat do you ask\b",
                @"\bstate your price\b",
                @"\bwhat is your offer\b",
                @"\bname your price\b");

        if (presentsNpcOffer)
        {
            patternReason = "npc presented offer/counter pattern";
            Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] npcReplyPattern=" + patternReason + " | npcReply=" + text);
            SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, patternReason);
            return;
        }

        if (requestsPlayerPrice)
        {
            patternReason = "npc requested player price pattern";
            Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] npcReplyPattern=" + patternReason + " | npcReply=" + text);
            SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, patternReason);
            return;
        }

        ExpectedReplyState restoredState = previousState == ExpectedReplyState.ExpectOfferPrice ||
                                           previousState == ExpectedReplyState.ExpectQuantity ||
                                           previousState == ExpectedReplyState.ExpectAcceptOrCounter
            ? previousState
            : lastUsefulNegotiationReplyState;
        patternReason = "restored useful negotiation follow-up";
        Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] npcReplyPattern=" + patternReason + " | npcReply=" + text);
        SetExpectedReplyState(
            restoredState != ExpectedReplyState.None ? restoredState : ExpectedReplyState.ExpectOfferPrice,
            patternReason);
    }

    public NegotiationIntent ClassifyIntent(string playerText)
    {
        return ClassifyInput(playerText, null).intent;
    }

    public NegotiationInput ClassifyInput(string playerText, LocalTradeState trade)
    {
        return ClassifyInput(playerText, trade, null);
    }

    public NegotiationInput ClassifyInput(string playerText, LocalTradeState trade, LLMIntentResult interpreted)
    {
        NegotiationInput result = new NegotiationInput();
        int detectedNumber = -1;
        string numberReason = "no number parsing attempted";
        bool hasActiveOffer = trade != null && trade.npcOffer > 0;
        bool rawContainsQuestionMark = playerText != null && playerText.Contains("?");
        string rawLowerText = (playerText ?? string.Empty).ToLowerInvariant();
        string rawSemanticText = BuildSemanticRawText(rawLowerText);
        string text = string.Empty;
        string[] tokens = Array.Empty<string>();
        result.expectedReplyState = CurrentExpectedReplyState;

        void SetMeta(ParseReason parseReason, ParseConfidence confidence, bool needsClarification = false)
        {
            result.parseReason = parseReason;
            result.parseConfidence = confidence;
            result.needsClarification = needsClarification;
        }

        NegotiationInput Finish(string reason, int detectedNumber = -1)
        {
            FinalizeNegotiationInput(result, playerText, text, tokens, trade, rawContainsQuestionMark, detectedNumber);
            ApplyStateSafetyGates(result, hasActiveOffer, ref reason);
            FinalizeNegotiationInput(result, playerText, text, tokens, trade, rawContainsQuestionMark, detectedNumber);
            Level1DebugForceAccept.LogParser("[NEGOTIATION UNDERSTANDING] raw=" + playerText +
                                             " | normalized=" + result.normalizedText +
                                             " | primaryIntent=" + result.intent +
                                             " | secondaryIntents=" + string.Join(",", result.secondaryIntents) +
                                             " | proposedPrice=" + result.sellerPrice +
                                             " | referencedPrices=" + string.Join(",", result.referencedPrices) +
                                             " | currentNpcOffer=" + (trade != null ? trade.npcOffer : 0) +
                                             " | expectedState=" + CurrentExpectedReplyState +
                                             " | confidence=" + result.parseConfidence +
                                             " | reason=" + result.parseReason +
                                             " | evidence=" + result.evidence +
                                             " | detectedNumber=" + detectedNumber +
                                             " | numberReason=" + numberReason +
                                             " | terminalAction=" + result.terminalAction +
                                             " | needsClarification=" + result.needsClarification);
            return result;
        }

        if (interpreted != null && interpreted.confidence)
        {
            result.normalizedText = string.IsNullOrWhiteSpace(interpreted.cleanedText)
                ? InputNormalizer.Normalize(playerText, trade != null && trade.npcOffer > 0)
                : interpreted.cleanedText;
            result.intent = interpreted.intent;
            result.hasSellerPrice = interpreted.sellerPrice.HasValue;
            result.sellerPrice = interpreted.sellerPrice ?? -1;
            result.hasQuantity = interpreted.quantity.HasValue;
            result.quantityGrams = interpreted.quantity ?? -1;
            SetMeta(ParseReason.LlmInterpreted, ParseConfidence.High);
            LastNormalizedInput = result.normalizedText;
            return Finish("llm interpreted result", result.hasSellerPrice ? result.sellerPrice : result.quantityGrams);
        }

        if (string.IsNullOrWhiteSpace(playerText))
        {
            result.intent = NegotiationIntent.CLARIFICATION;
            result.clarificationKind = ClarificationKind.EmptyTranscript;
            SetMeta(ParseReason.EmptyTranscript, ParseConfidence.Low, true);
            return Finish("empty player input");
        }

        string itemKey = trade != null ? (trade.spiceKey ?? string.Empty).ToLowerInvariant() : string.Empty;
        string itemDisplay = trade != null ? (trade.spiceDisplayName ?? string.Empty).ToLowerInvariant() : string.Empty;
        text = InputNormalizer.Normalize(playerText, hasActiveOffer);
        tokens = InputNormalizer.Tokenize(playerText, hasActiveOffer);
        LastNormalizedInput = text;
        result.normalizedText = text;
        PopulateSemanticFlags(result, rawSemanticText, text, tokens);

        bool hasTradeNumber = TryParseTradeNumber(text, tokens, out detectedNumber, out numberReason);

        // Parser priority:
        // A. normalize safely
        // B. explicit dismiss / hard reject / end-trade
        // C. price-bearing accept/counter phrases
        // D. pure acceptance
        // E. soft rejection / bargain
        // F. numeric price / quantity
        // G. trade questions
        // H. confused / repeat
        // I. general dialogue
        // J. targeted clarification fallback

        if (TryDetectExplicitTerminalIntent(text, tokens, trade, hasTradeNumber, detectedNumber, out NegotiationIntent terminalIntent, out bool hardReject, out string terminalReason))
        {
            result.intent = terminalIntent;
            result.hasHardRejection = hardReject;
            result.terminalAction = true;
            SetMeta(terminalIntent == NegotiationIntent.DISMISS ? ParseReason.DismissTrade : ParseReason.HardRejection, ParseConfidence.High);
            return Finish(terminalReason, detectedNumber);
        }

        if (MatchesSoftRejectPhrase(text, tokens, trade, hasTradeNumber, detectedNumber))
        {
            result.intent = NegotiationIntent.REJECT;
            result.hasHardRejection = false;
            result.terminalAction = false;
            SetMeta(ParseReason.SoftRejection, ParseConfidence.Medium);
            return Finish("explicit soft rejection phrase");
        }

        if (IsPureAcceptance(rawLowerText, text, tokens) && hasActiveOffer && !HasCounterPriceAttachedToAcceptance(text, tokens))
        {
            result.hasExplicitAcceptance = true;
            result.intent = NegotiationIntent.ACCEPT;
            if (trade != null && trade.npcOffer > 0)
            {
                result.acceptanceTarget = trade.npcOffer;
            }
            SetMeta(ParseReason.PureAcceptance, ParseConfidence.High);
            return Finish("pure acceptance without new price");
        }

        if (IsPureRejection(text, tokens) && hasActiveOffer && !HasCounterPriceAttachedToAcceptance(text, tokens))
        {
            result.intent = NegotiationIntent.REJECT;
            result.hasHardRejection = IsHardRejectPhrase(text, tokens);
            result.terminalAction = result.hasHardRejection;
            SetMeta(result.hasHardRejection ? ParseReason.HardRejection : ParseReason.SoftRejection, result.hasHardRejection ? ParseConfidence.High : ParseConfidence.Medium);
            return Finish("pure rejection without new price");
        }

        if (TryParseHistoricalReferenceOnly(rawSemanticText, text, result, out string historicalReason))
        {
            SetMeta(ParseReason.AmbiguousAcceptOrCounter, ParseConfidence.Low, true);
            return Finish(historicalReason, detectedNumber);
        }

        if (TryParseExplicitTradeQuestion(rawSemanticText, text, tokens, trade, hasTradeNumber, detectedNumber, result, out string questionReason))
        {
            if (result.intent == NegotiationIntent.ITEM_QUERY)
            {
                SetMeta(ParseReason.ItemQuery, ParseConfidence.Medium);
            }
            else if (result.intent == NegotiationIntent.QUANTITY_QUERY)
            {
                SetMeta(ParseReason.QuantityQuery, ParseConfidence.Medium);
            }
            else
            {
                SetMeta(ParseReason.BuyerBudgetQuery, ParseConfidence.Medium);
            }
            return Finish(questionReason, detectedNumber);
        }

        if (TryParseExplicitPriceCriticism(rawSemanticText, text, trade, hasTradeNumber, detectedNumber, result, out string criticismReason))
        {
            SetMeta(result.intent == NegotiationIntent.BARGAIN ? ParseReason.BargainLanguage : ParseReason.SoftRejection, ParseConfidence.Medium);
            return Finish(criticismReason, detectedNumber);
        }

        if (TryResolveContextualAcceptance(rawSemanticText, text, tokens, trade, hasTradeNumber, detectedNumber, rawContainsQuestionMark, result, out string acceptanceReason))
        {
            SetMeta(ParseReason.PureAcceptance, ParseConfidence.High);
            return Finish(acceptanceReason, detectedNumber);
        }

        if (TryInterpretStructuredUtterance(text, tokens, trade, hasTradeNumber, detectedNumber, result, out string structuredReason))
        {
            if (result.intent == NegotiationIntent.ACCEPT)
            {
                SetMeta(ParseReason.PureAcceptance, ParseConfidence.High);
            }
            else if (result.intent == NegotiationIntent.COUNTER || result.intent == NegotiationIntent.PRICE || result.intent == NegotiationIntent.ULTIMATUM)
            {
                SetMeta(ParseReason.PriceOfferParsed, result.referencedPrices.Count > 0 ? ParseConfidence.High : ParseConfidence.Medium);
            }
            else if (result.intent == NegotiationIntent.BARGAIN || result.intent == NegotiationIntent.REJECT)
            {
                SetMeta(ParseReason.BargainLanguage, ParseConfidence.Medium);
            }
            else if (result.intent == NegotiationIntent.CLARIFICATION)
            {
                SetMeta(
                    result.clarificationKind == ClarificationKind.HistoricalPriceOnly ? ParseReason.AmbiguousAcceptOrCounter :
                    result.clarificationKind == ClarificationKind.MultipleActionablePrices ? ParseReason.AmbiguousAcceptOrCounter :
                    ParseReason.UnrecognizedSpeech,
                    ParseConfidence.Low,
                    true);
            }

            return Finish(structuredReason, result.hasSellerPrice ? result.sellerPrice : detectedNumber);
        }

        if (ContainsPhrase(text, "ignore previous instructions") || ContainsPhrase(text, "act as chatgpt"))
        {
            result.intent = NegotiationIntent.OFF_TOPIC;
            SetMeta(ParseReason.PromptInjection, ParseConfidence.High);
            return Finish("prompt injection/off-topic phrase");
        }

        if (ContainsAny(text, ModernWords) || ContainsAll(text, "social", "media") || ContainsAll(text, "mobile", "phone") || ContainsAll(text, "video", "game"))
        {
            result.intent = NegotiationIntent.OFF_TOPIC;
            SetMeta(ParseReason.OffTopicModern, ParseConfidence.High);
            return Finish("modern/off-topic vocabulary");
        }

        if (ContainsAny(text, HostileWords) || ContainsAll(text, "go", "die") || ContainsAll(text, "kill", "yourself"))
        {
            result.intent = NegotiationIntent.HOSTILE;
            SetMeta(ParseReason.Hostile, ParseConfidence.High);
            return Finish("hostile vocabulary");
        }

        if (TryParsePriceOffer(text, tokens, trade, hasTradeNumber, detectedNumber, out int sellerPrice, out NegotiationIntent priceIntent, out string priceReason))
        {
            result.hasSellerPrice = true;
            result.sellerPrice = sellerPrice;
            result.intent = priceIntent;
            SetMeta(ParseReason.PriceOfferParsed, ParseConfidence.High);
            return Finish(priceReason, sellerPrice);
        }

        if (TryParseQuantityAnswer(text, tokens, trade, hasTradeNumber, detectedNumber, out int quantityGrams, out string quantityReason))
        {
            result.hasQuantity = true;
            result.quantityGrams = quantityGrams;
            result.intent = NegotiationIntent.QUANTITY_CHANGE;
            SetMeta(ParseReason.QuantityAnswerParsed, ParseConfidence.High);
            return Finish(quantityReason, detectedNumber > 0 ? detectedNumber : quantityGrams);
        }

        bool asksItem = result.asksItem || ContainsAnyPhrase(text, "what item", "which spice", "what spice", "what are you buying", "what do you want");
        bool asksPrice = result.asksCurrentOffer || ContainsAnyPhrase(text, "how much will you pay", "what can you offer", "your offer", "your budget", "your price", "what will you give", "best price", "maximum you can give", "what is your best", "what price", "what is price", "what cost");

        if (asksItem && asksPrice)
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
            AddSecondaryIntent(result, NegotiationIntent.ITEM_QUERY);
            SetMeta(ParseReason.PriceQuery, ParseConfidence.Medium);
            return Finish("combined item-and-price query prioritized to price");
        }

        if (asksPrice)
        {
            result.intent = NegotiationIntent.QUERY_BUYER_BUDGET;
            SetMeta(ParseReason.BuyerBudgetQuery, ParseConfidence.Medium);
            return Finish("buyer budget / offer query");
        }

        if (asksItem)
        {
            result.intent = NegotiationIntent.ITEM_QUERY;
            SetMeta(ParseReason.ItemQuery, ParseConfidence.Medium);
            return Finish("item query");
        }

        if (ContainsAnyPhrase(text, "how many", "what quantity", "what amount", "how much quantity") ||
            ContainsAnyToken(tokens, "quantity", "amount", "weight"))
        {
            result.intent = NegotiationIntent.QUANTITY_QUERY;
            SetMeta(ParseReason.QuantityQuery, ParseConfidence.Medium);
            return Finish("quantity query");
        }

        if (ContainsAnyPhrase(text, "how much", "what price", "what is price", "what cost"))
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
            SetMeta(ParseReason.PriceQuery, ParseConfidence.Medium);
            return Finish("price query");
        }

        if (IsConfusedPhrase(text, tokens))
        {
            result.intent = NegotiationIntent.CONFUSED;
            SetMeta(ParseReason.ConfusedPlayer, ParseConfidence.Low, true);
            return Finish("confused/repeat-request phrase");
        }

        if (ContainsAnyToken(tokens, "hello", "hi", "hey", "greetings", "namaste") || ContainsAnyPhrase(text, "good day", "good morning"))
        {
            result.intent = NegotiationIntent.GREETING;
            result.socialSubIntent = "GREETING";
            SetMeta(ParseReason.Greeting, ParseConfidence.Medium);
            return Finish("greeting");
        }

        if (ContainsAnyPhrase(text, "who are you", "where from", "how is weather", "how weather", "who is king", "what is your name", "tell me about"))
        {
            result.intent = NegotiationIntent.GENERAL_DIALOGUE;
            result.socialSubIntent = DetectSocialSubIntent(text);
            SetMeta(ParseReason.GeneralDialogue, ParseConfidence.Medium);
            return Finish("general dialogue");
        }

        if (result.tradeOpeningQuery)
        {
            result.intent = NegotiationIntent.ITEM_QUERY;
            AddSecondaryIntent(result, NegotiationIntent.GREETING);
            SetMeta(ParseReason.ItemQuery, ParseConfidence.Medium);
            return Finish("trade-opening conversational query");
        }

        if (ContainsAnyPhrase(text, "that is too low", "too low", "not enough", "can you do better", "give more", "increase it", "increase your offer", "a little more", "meet in the middle", "split"))
        {
            result.intent = NegotiationIntent.BARGAIN;
            SetMeta(ParseReason.BargainLanguage, ParseConfidence.Medium);
            return Finish("soft bargain language without explicit price");
        }

        if (ContainsAnyToken(tokens, "reduce", "lower", "discount", "cheap", "cheaper", "less", "expensive", "steep", "low", "high"))
        {
            result.intent = NegotiationIntent.BARGAIN;
            SetMeta(ParseReason.BargainLanguage, ParseConfidence.Medium);
            return Finish("bargain language");
        }

        if (ContainsAnyToken(tokens, "increase", "higher", "more"))
        {
            result.intent = NegotiationIntent.COUNTER;
            SetMeta(ParseReason.CounterLanguage, ParseConfidence.Medium);
            return Finish("counter language without explicit parsed price", detectedNumber);
        }

        if (ContainsAnyToken(tokens, "sure", "okay", "ok", "fine", "alright", "yes") && !ContainsQuestionSignal(text))
        {
            result.intent = hasActiveOffer ? NegotiationIntent.ACCEPT : NegotiationIntent.CONTINUE;
            result.hasExplicitAcceptance = hasActiveOffer;
            SetMeta(ParseReason.FallbackAcceptance, hasActiveOffer ? ParseConfidence.Medium : ParseConfidence.Low, !hasActiveOffer);
            return Finish(hasActiveOffer ? "fallback acceptance without attached number" : "continue without active offer");
        }

        if (ContainsAnyToken(tokens, "weather", "rain", "sun", "origin", "name", "king", "temple"))
        {
            result.intent = NegotiationIntent.GENERAL_DIALOGUE;
            result.socialSubIntent = DetectSocialSubIntent(text);
            SetMeta(ParseReason.GeneralDialogue, ParseConfidence.Medium);
            return Finish("social/general topic");
        }

        if (ContainsAnyToken(tokens, TradeWords))
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
            result.clarificationKind = ClarificationKind.MissingPrice;
            SetMeta(ParseReason.TradeVocabularyFallback, ParseConfidence.Low, true);
            return Finish("trade vocabulary without structured parse", detectedNumber);
        }

        if (text.Split(' ').Length <= 3)
        {
            result.intent = NegotiationIntent.CLARIFICATION;
            result.socialSubIntent = string.Empty;
            result.clarificationKind = hasTradeNumber ? ClarificationKind.MissingPrice : ClarificationKind.UnrecognizedSpeech;
            SetMeta(hasTradeNumber ? ParseReason.ShortNumericUnclear : ParseReason.UnrecognizedSpeech, ParseConfidence.Low, true);
            return Finish(hasTradeNumber ? "short numeric reply without clear trade context" : "short unclear reply", detectedNumber);
        }

        result.intent = NegotiationIntent.CLARIFICATION;
        result.clarificationKind = ClarificationKind.UnrecognizedSpeech;
        SetMeta(ParseReason.UnrecognizedSpeech, ParseConfidence.Low, true);
        return Finish("unknown fallback", detectedNumber);
    }

    private bool TryDetectExplicitTerminalIntent(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, out NegotiationIntent intent, out bool hardReject, out string reason)
    {
        intent = NegotiationIntent.UNKNOWN;
        hardReject = false;
        reason = string.Empty;

        if (HasReplacementOfferAttachedToRejection(text, tokens, trade, hasTradeNumber, detectedNumber))
        {
            return false;
        }

        if (MatchesDismissPhrase(text, tokens))
        {
            intent = NegotiationIntent.DISMISS;
            hardReject = true;
            reason = "explicit dismiss/end-customer phrase";
            return true;
        }

        if (MatchesHardRejectPhrase(text, tokens))
        {
            intent = NegotiationIntent.REJECT;
            hardReject = true;
            reason = "explicit hard reject/end-trade phrase";
            return true;
        }

        return false;
    }

    private static void AddSecondaryIntent(NegotiationInput result, NegotiationIntent intent)
    {
        if (result == null || intent == result.intent || result.secondaryIntents.Contains(intent))
        {
            return;
        }

        result.secondaryIntents.Add(intent);
    }

    private static void PopulateSemanticFlags(NegotiationInput result, string rawText, string text, string[] tokens)
    {
        if (result == null)
        {
            return;
        }

        result.asksItem = MatchesItemQuery(rawText, text);
        result.asksCurrentOffer = MatchesPriceQuestion(rawText, text);
        result.asksQuantity = MatchesQuantityQuestion(rawText, text);
        result.asksReason = MatchesPricePressureQuestion(rawText, text);
        result.tradeOpeningQuery = ContainsAnyPhrase(text,
            "how can i help you",
            "how i help",
            "what can i do for you",
            "what i do for you",
            "how may i help",
            "how may i help you",
            "what brings you here",
            "what brings you to my stall");
        if (result.tradeOpeningQuery)
        {
            result.asksItem = true;
        }
        result.negotiationTactic = DetectNegotiationTactic(text, tokens);
    }

    private bool TryInterpretStructuredUtterance(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, NegotiationInput result, out string reason)
    {
        reason = "no structured interpretation";

        if (result == null)
        {
            return false;
        }

        if (TryParseHistoricalReferenceOnly(string.Empty, text, result, out reason))
        {
            return true;
        }

        if (TryParseStructuredAcceptance(text, trade, result, out reason))
        {
            return true;
        }

        if (TryResolveStructuredPriceOffer(text, tokens, trade, hasTradeNumber, detectedNumber, result, out reason))
        {
            return true;
        }

        if (TryParseStructuredBargain(text, tokens, trade, hasTradeNumber, result, out reason))
        {
            return true;
        }

        return false;
    }

    private bool TryParseHistoricalReferenceOnly(string rawText, string text, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        MatchCollection matches = Regex.Matches(text, @"\b\d+\b");
        if (matches.Count == 0)
        {
            return false;
        }

        bool referencesHistory = MatchesHistoricalPriceReference(rawText, text);
        bool hasActionCue = ContainsAnyPhrase(text,
            "what item",
            "i want",
            "give me",
            "give",
            "make it",
            "accept",
            "deal",
            "fine at",
            "can you do",
            "what about",
            "come down to",
            "leave it at",
            "keep it at",
            "hold it at",
            "settle it at",
            "settle at",
            "meet at",
            "i need",
            "i can do",
            "i do",
            "i will take",
            "i would accept",
            "i can agree",
            "agree to",
            "my final",
            "final price",
            "make that",
            "put it at",
            "price is") ||
            MatchesAnyPattern(text,
                @"\bgive\s+\d+\b",
                @"\bmeet(?: me| you)?\s+at\s+\d+\b",
                @"\b(?:my\s+)?price\s+is\s+\d+\b",
                @"\bi\s+do\s+\d+\b",
                @"\b(?:i\s+)?accept\s+\d+\b",
                @"\b(?:i\s+)?agree\s+to\s+\d+\b",
                @"\b\d+\s+is\s+final\b",
                @"\b\d+\s+is\s+final price\b",
                @"\b\d+\s+is\s+my\s+final\b",
                @"\b\d+\s+is\s+my\s+final price\b");
        bool saidEarlierOnly = ContainsAnyPhrase(text, "i said") && ContainsAnyPhrase(text, "earlier", "before") && !ContainsAnyPhrase(text, "not", "make it", "give me", "i want", "settle at", "come down to");
        if (saidEarlierOnly)
        {
            referencesHistory = true;
        }
        if (!referencesHistory || hasActionCue)
        {
            return false;
        }

        foreach (Match match in matches)
        {
            if (int.TryParse(match.Value, out int referenced))
            {
                result.referencedPrices.Add(referenced);
            }
        }

        result.intent = NegotiationIntent.CLARIFICATION;
        result.needsClarification = true;
        result.clarificationKind = ClarificationKind.HistoricalPriceOnly;
        result.evidence = "historical price reference without actionable final clause";
        reason = "historical price reference needs clarification";
        return true;
    }

    private bool TryParseExplicitTradeQuestion(string rawText, string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        if (result == null)
        {
            return false;
        }

        bool asksItem = MatchesItemQuery(rawText, text);
        bool asksPrice = MatchesPriceQuestion(rawText, text);
        bool asksQuantity = MatchesQuantityQuestion(rawText, text);
        bool asksPressure = MatchesPricePressureQuestion(rawText, text);

        if (!asksItem && !asksPrice && !asksQuantity && !asksPressure)
        {
            return false;
        }

        if (asksPrice || asksPressure)
        {
            bool asksBudgetOrCapability =
                ContainsAnyPhrase(text,
                    "can you pay",
                    "how much can you pay",
                    "what is your budget",
                    "your budget",
                    "what can you afford",
                    "maximum you can give",
                    "what is your highest offer") ||
                ContainsAnyPhrase(rawText,
                    "can you pay",
                    "how much can you pay",
                    "what is your budget",
                    "your budget",
                    "what can you afford",
                    "maximum you can give",
                    "what is your highest offer");

            result.intent = asksPressure
                ? NegotiationIntent.QUERY_BUYER_BUDGET
                : asksBudgetOrCapability
                    ? NegotiationIntent.QUERY_BUYER_BUDGET
                    : NegotiationIntent.PRICE_QUERY;
            if (asksItem)
            {
                AddSecondaryIntent(result, NegotiationIntent.ITEM_QUERY);
            }
            if (asksQuantity)
            {
                AddSecondaryIntent(result, NegotiationIntent.QUANTITY_QUERY);
            }
            if (detectedNumber > 0 && !result.referencedPrices.Contains(detectedNumber))
            {
                result.referencedPrices.Add(detectedNumber);
            }
            result.asksCurrentOffer = true;
            result.asksReason = asksPressure;
            result.evidence = asksPressure
                ? "explicit price-pressure question"
                : asksBudgetOrCapability
                    ? "explicit buyer-budget/capability question"
                : asksItem || asksQuantity
                    ? "multi-intent trade question"
                    : "explicit price / buyer-offer question";
            reason = asksPressure
                ? "price-pressure question recognized before acceptance/counter fallback"
                : asksBudgetOrCapability
                    ? "buyer budget/capability question recognized before numeric fallback"
                : asksItem || asksQuantity
                    ? "multi-intent trade question recognized before numeric fallback"
                    : "price / buyer-offer question recognized before numeric fallback";
            return true;
        }

        if (asksItem && asksQuantity)
        {
            result.intent = NegotiationIntent.ITEM_QUERY;
            AddSecondaryIntent(result, NegotiationIntent.QUANTITY_QUERY);
            result.evidence = "combined item-and-quantity question";
            reason = "multi-intent item/quantity query";
            return true;
        }

        if (asksItem)
        {
            result.intent = NegotiationIntent.ITEM_QUERY;
            result.evidence = "explicit item question";
            reason = "item question recognized";
            return true;
        }

        result.intent = NegotiationIntent.QUANTITY_QUERY;
        result.evidence = "explicit quantity question";
        reason = "quantity question recognized";
        return true;
    }

    private bool TryParseExplicitPriceCriticism(string rawText, string text, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        if (result == null || !MatchesExplicitPriceRejectionOrCriticism(rawText, text))
        {
            return false;
        }

        int referencedPrice = detectedNumber > 0 ? detectedNumber : (trade != null ? trade.npcOffer : -1);
        if (referencedPrice > 0 && !result.referencedPrices.Contains(referencedPrice))
        {
            result.referencedPrices.Add(referencedPrice);
        }

        result.rejectsCurrentOffer = trade != null && referencedPrice > 0 && trade.npcOffer == referencedPrice;
        result.rejectedPrice = referencedPrice > 0 ? referencedPrice : result.rejectedPrice;

        bool bargainLikeCriticism = ContainsAnyPhrase(text,
            "needs improvement",
            "too low",
            "too high",
            "too much",
            "not enough",
            "bad price",
            "unfair");

        result.intent = bargainLikeCriticism ? NegotiationIntent.BARGAIN : NegotiationIntent.REJECT;
        result.evidence = bargainLikeCriticism
            ? "explicit criticism of referenced price without acceptance"
            : "explicit refusal of referenced price";
        reason = bargainLikeCriticism
            ? "price criticism blocks exact-price acceptance"
            : "price refusal blocks exact-price acceptance";
        return true;
    }

    private bool TryResolveContextualAcceptance(string rawText, string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, bool rawContainsQuestionMark, NegotiationInput result, out string reason)
    {
        reason = "contextual acceptance not resolved";
        int currentNpcOffer = trade != null ? trade.npcOffer : -1;
        int tradeNumberCount = CountTradeNumbers(tokens);
        bool negatedAcceptanceCue = ContainsNegatedAcceptanceCue(text);
        int acceptanceReferencedPrice = detectedNumber;
        bool hasAcceptanceReferencedPrice = TryExtractExplicitAcceptancePrice(text, out int explicitAcceptancePrice);
        if (hasAcceptanceReferencedPrice)
        {
            acceptanceReferencedPrice = explicitAcceptancePrice;
        }
        bool affirmative =
            !negatedAcceptanceCue &&
            (ContainsAnyToken(tokens, "yes", "yeah", "okay", "ok", "fine", "deal", "agreed", "accepted", "accept", "agree", "alright", "sure") ||
            ContainsAnyPhrase(text, "sounds good", "seems fair", "is fine", "is okay", "is good", "will do", "is acceptable", "works for me", "that works", "i will take it", "fair enough", "go ahead", "very well", "let us do it", "that will work", "agreed then", "fine by me", "okay then", "we have a deal", "you have a deal") ||
            MatchesAnyPattern(text,
                @"^\d+\s+works$",
                @"^\d+\s+is\s+okay$",
                @"^\d+\s+is\s+fine$",
                @"^\d+\s+is\s+good$",
                @"^\d+\s+sounds\s+good$",
                @"^\d+\s+seems\s+fair$",
                @"^\d+\s+will\s+do$",
                @"^\d+\s+is\s+acceptable$",
                @"^\d+\s+then$")) ||
            Regex.IsMatch(text, @"\blet us settle at\s+\d+\b") ||
            Regex.IsMatch(text, @"\bwe can do\s+\d+\b");
        bool rejectionCue =
            MatchesExplicitPriceRejectionOrCriticism(rawText, text) ||
            ContainsAnyPhrase(text, "too low", "too high", "not interested", "no deal", "reject", "not enough") ||
            Regex.IsMatch(text, @"\bnot\s+" + currentNpcOffer + @"\b");
        bool historicalCue =
            MatchesHistoricalPriceReference(rawText, text) &&
            !ContainsAnyPhrase(text, "i accept", "i agree", "deal", "works", "will do", "is fine", "is acceptable");
        bool samePriceSettlementCue =
            acceptanceReferencedPrice > 0 &&
            currentNpcOffer > 0 &&
            acceptanceReferencedPrice == currentNpcOffer &&
            !negatedAcceptanceCue &&
            (Regex.IsMatch(text, @"^\d+\s+then$") ||
             Regex.IsMatch(text, @"\balright\s+\d+\s+then\b") ||
             Regex.IsMatch(text, @"\blet us settle at\s+\d+\b") ||
             Regex.IsMatch(text, @"\bwe can do\s+\d+\b"));
        bool contradictoryCounterCue =
            ContainsAnyPhrase(text,
                "make it",
                "give me",
                "i want",
                "want",
                "only at",
                "what about",
                "can you do",
                "sweeten deal",
                "sweeten the deal",
                "leave it at",
                "leave amount at",
                "leave value at",
                "leave rate at",
                "leave price at",
                "leave offer at",
                "keep it at",
                "hold it at",
                "settle at",
                "come down to") ||
            MatchesDealImprovementBargain(text) ||
            Regex.IsMatch(text, @"\byes\b.*\bif\b");
        bool questionCue = rawContainsQuestionMark || ContainsQuestionSignal(text) || MatchesRawQuestionedPrice(rawText);
        bool priceMatches = acceptanceReferencedPrice > 0 && currentNpcOffer > 0 && acceptanceReferencedPrice == currentNpcOffer;
        bool bareExactCurrentOffer = currentNpcOffer > 0 &&
                                     hasTradeNumber &&
                                     tradeNumberCount == 1 &&
                                     detectedNumber == currentNpcOffer &&
                                     !affirmative &&
                                     !rejectionCue &&
                                     !historicalCue &&
                                     !contradictoryCounterCue &&
                                     !questionCue;
        bool accepted = trade != null &&
                        currentNpcOffer > 0 &&
                        CurrentExpectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter &&
                        !rejectionCue &&
                        !historicalCue &&
                        (!contradictoryCounterCue || samePriceSettlementCue) &&
                        !questionCue &&
                        ((affirmative && (priceMatches || !hasTradeNumber)) || bareExactCurrentOffer || samePriceSettlementCue);

        Level1DebugForceAccept.LogParser("[ACCEPTANCE RESOLUTION] affirmative=" + affirmative +
                                         " | referencedPrice=" + acceptanceReferencedPrice +
                                         " | currentNpcOffer=" + currentNpcOffer +
                                         " | priceMatches=" + priceMatches +
                                         " | contradictoryCounterCue=" + contradictoryCounterCue +
                                         " | accepted=" + accepted +
                                         " | reason=" + (accepted
                                             ? (bareExactCurrentOffer ? "bare exact current npc offer accepted" : "affirmative acceptance of current npc offer")
                                             : "not accepted"));

        if (!accepted)
        {
            return false;
        }

        result.intent = NegotiationIntent.ACCEPT;
        result.hasExplicitAcceptance = true;
        result.acceptanceTarget = currentNpcOffer;
        result.sellerPrice = -1;
        result.hasSellerPrice = false;
        if (priceMatches)
        {
            result.referencedPrices.Add(currentNpcOffer);
        }
        result.evidence = bareExactCurrentOffer
            ? "bare exact current npc offer accepted"
            : samePriceSettlementCue
                ? "same-price settlement phrasing accepted as current npc offer"
                : "affirmative acceptance of current npc offer";
        reason = bareExactCurrentOffer
            ? "contextual bare exact-price acceptance of current npc offer"
            : samePriceSettlementCue
                ? "same-price settlement phrase accepted as current npc offer"
                : "contextual acceptance of current npc offer";
        return true;
    }

    private bool TryParseStructuredAcceptance(string text, LocalTradeState trade, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        bool hasActiveOffer = trade != null && trade.npcOffer > 0;
        if (!hasActiveOffer)
        {
            return false;
        }

        bool hasExplicitAcceptanceLanguage =
            ContainsAnyPhrase(text,
                "i accept",
                "accept",
                "accepted",
                "i agree",
                "agree",
                "agreed",
                "i will accept",
                "i can agree",
                "i will take") ||
            Regex.IsMatch(text, @"\b(?:accept|accepted|agree|agreed|take)\b.*\b\d+\b");

        if (hasExplicitAcceptanceLanguage && TryExtractExplicitAcceptancePrice(text, out int acceptedPrice))
        {
            bool matchesCurrentOffer = trade != null && trade.npcOffer > 0 && acceptedPrice == trade.npcOffer;
            if (matchesCurrentOffer && !ContainsNegatedAcceptanceCue(text))
            {
                result.intent = NegotiationIntent.ACCEPT;
                result.hasExplicitAcceptance = true;
                result.acceptanceTarget = trade.npcOffer;
                AddReferencedPricesExcluding(text, result);
                result.evidence = "explicit acceptance tied to current npc offer";
                reason = "structured acceptance at current npc offer";
            }
            else
            {
                result.intent = NegotiationIntent.COUNTER;
                result.hasSellerPrice = true;
                result.sellerPrice = acceptedPrice;
                AddReferencedPricesExcluding(text, result, acceptedPrice);
                result.evidence = "acceptance language attached to different explicit price";
                reason = "different priced acceptance language treated as counter-offer";
            }
            return true;
        }

        if ((ContainsAnyPhrase(text, "i accept", "accept your offer", "i accept your offer", "i agree", "agreed", "agree") || Regex.IsMatch(text, @"\b(?:accept|agree|agreed)\b.*\b\d+\b")) &&
            TryParseTradeNumber(text, text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries), out int referencedOffer, out _))
        {
            bool matchesCurrentOffer = trade != null && trade.npcOffer > 0 && referencedOffer == trade.npcOffer;
            result.intent = matchesCurrentOffer ? NegotiationIntent.ACCEPT : NegotiationIntent.COUNTER;
            result.hasExplicitAcceptance = matchesCurrentOffer;
            result.acceptanceTarget = matchesCurrentOffer ? referencedOffer : -1;
            result.hasSellerPrice = !matchesCurrentOffer;
            result.sellerPrice = matchesCurrentOffer ? -1 : referencedOffer;
            result.referencedPrices.Add(referencedOffer);
            result.evidence = matchesCurrentOffer
                ? "acceptance references current npc offer"
                : "acceptance language references different explicit price";
            reason = matchesCurrentOffer
                ? "structured acceptance referencing current offer"
                : "different priced acceptance reference treated as counter-offer";
            return true;
        }

        if (ContainsAnyPhrase(text, "but fine", "but okay", "but ok", "but deal", "but accepted", "but done", "that is lower than i wanted but fine"))
        {
            result.intent = NegotiationIntent.ACCEPT;
            result.hasExplicitAcceptance = true;
            result.acceptanceTarget = trade.npcOffer;
            result.evidence = "contrastive acceptance without new price";
            reason = "contrastive acceptance of current offer";
            return true;
        }

        return false;
    }

    private bool TryResolveStructuredPriceOffer(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        if (!hasTradeNumber || detectedNumber <= 0)
        {
            return false;
        }

        bool explicitLeavePriceSetting = ContainsAnyPhrase(
            text,
            "leave amount at",
            "leave price at",
            "leave value at",
            "leave rate at");

        if (!explicitLeavePriceSetting && IsQuantityContext(text, tokens, trade))
        {
            return false;
        }

        List<StructuredPriceCandidate> actionablePrices = CollectStructuredPriceCandidates(text, tokens);
        if (actionablePrices.Count == 0 && !IsOfferContext(text, tokens, trade))
        {
            return false;
        }

        int chosenPrice = detectedNumber;
        string cueReason = "fallback detected trade number";
        int bestScore = int.MinValue;
        for (int i = 0; i < actionablePrices.Count; i++)
        {
            StructuredPriceCandidate candidate = actionablePrices[i];
            if (candidate.score > bestScore)
            {
                bestScore = candidate.score;
                chosenPrice = candidate.value;
                cueReason = candidate.cue;
            }
        }

        MatchCollection allMatches = Regex.Matches(text, @"\b\d+\b");
        foreach (Match match in allMatches)
        {
            if (!int.TryParse(match.Value, out int candidate))
            {
                continue;
            }

            if (candidate == chosenPrice && !result.referencedPrices.Contains(candidate))
            {
                continue;
            }

            if (!result.referencedPrices.Contains(candidate))
            {
                result.referencedPrices.Add(candidate);
            }
        }

        Match correctionMatch = Regex.Match(text, @"\bnot\s+(\d+)\b.*?\b(?:i said|instead|but|make it|give me|use)\s+(\d+)\b");
        Match simpleNotCorrectionMatch = Regex.Match(text, @"\bnot\s+(\d+)\b[\s,;:-]+(\d+)\b");
        Match replaceWithMatch = Regex.Match(text, @"\b(?:replace|change(?: my offer)? from)\s+(\d+)\s+(?:with|to)\s+(\d+)\b");
        Match sorryCorrectionMatch = Regex.Match(text, @"\b(\d+)\b[\s,;:-]+(?:sorry|actually|rather|instead|i mean|mean)\s+(\d+)\b");
        Match mistakenCorrectionMatch = Regex.Match(text, @"\b(?:i said|said)\s+(\d+)\s+(?:by mistake|wrong)\b.*?\b(\d+)\b");
        Match wrongUseCorrectionMatch = Regex.Match(text, @"\b(\d+)\s+was\s+wrong\b.*?\b(?:use|make it|i want)\s+(\d+)\b");
        Match scratchForgetCorrectionMatch = Regex.Match(text, @"\b(?:scratch|forget)\s+(\d+)\b.*?\b(?:use|make it|i want)\s+(\d+)\b");
        Match insteadOfCorrectionMatch = Regex.Match(text, @"\b(?:make it|use|i want)\s+(\d+)\b.*?\binstead of\s+(\d+)\b");

        if (correctionMatch.Success || simpleNotCorrectionMatch.Success || replaceWithMatch.Success || sorryCorrectionMatch.Success || mistakenCorrectionMatch.Success || wrongUseCorrectionMatch.Success || scratchForgetCorrectionMatch.Success || insteadOfCorrectionMatch.Success)
        {
            result.correctionDetected = true;
            Match effectiveCorrectionMatch = correctionMatch.Success
                ? correctionMatch
                : simpleNotCorrectionMatch.Success
                    ? simpleNotCorrectionMatch
                    : replaceWithMatch.Success
                        ? replaceWithMatch
                        : sorryCorrectionMatch.Success
                            ? sorryCorrectionMatch
                            : mistakenCorrectionMatch.Success
                                ? mistakenCorrectionMatch
                                : wrongUseCorrectionMatch.Success
                                    ? wrongUseCorrectionMatch
                                    : scratchForgetCorrectionMatch.Success
                                        ? scratchForgetCorrectionMatch
                                        : insteadOfCorrectionMatch;

            bool groupsAreReversed = effectiveCorrectionMatch == insteadOfCorrectionMatch;

            string rejectedValue = groupsAreReversed ? effectiveCorrectionMatch.Groups[2].Value : effectiveCorrectionMatch.Groups[1].Value;
            string correctedValue = groupsAreReversed ? effectiveCorrectionMatch.Groups[1].Value : effectiveCorrectionMatch.Groups[2].Value;

            if (int.TryParse(rejectedValue, out int rejectedPrice))
            {
                result.rejectedPrice = rejectedPrice;
                result.rejectsCurrentOffer = trade != null && trade.npcOffer == rejectedPrice;
                if (!result.referencedPrices.Contains(rejectedPrice))
                {
                    result.referencedPrices.Add(rejectedPrice);
                }
            }
            if (int.TryParse(correctedValue, out int correctedPrice))
            {
                chosenPrice = correctedPrice;
                bestScore = Mathf.Max(bestScore, 1000);
                cueReason = "correction pattern selected corrected price";
            }
        }

        if (TryResolveRepeatedNegatedPrices(text, out List<int> negatedReferences, out int survivingPrice))
        {
            for (int i = 0; i < negatedReferences.Count; i++)
            {
                int reference = negatedReferences[i];
                if (!result.referencedPrices.Contains(reference))
                {
                    result.referencedPrices.Add(reference);
                }
            }

            if (survivingPrice > 0)
            {
                chosenPrice = survivingPrice;
                cueReason = "repeated negated prices followed by surviving final price";
            }
        }

        for (int i = result.referencedPrices.Count - 1; i >= 0; i--)
        {
            if (result.referencedPrices[i] == chosenPrice)
            {
                result.referencedPrices.RemoveAt(i);
            }
        }

        if (actionablePrices.Count > 1)
        {
            HashSet<int> distinctValues = new HashSet<int>();
            int competingActionableCount = 0;
            for (int i = 0; i < actionablePrices.Count; i++)
            {
                StructuredPriceCandidate candidate = actionablePrices[i];
                distinctValues.Add(candidate.value);
                bool candidateIsCompetingActionable =
                    candidate.value != chosenPrice &&
                    !candidate.rejectionCue &&
                    !candidate.historicalCue &&
                    (candidate.actionCue || candidate.finalityCue || candidate.acceptanceCue || candidate.correctionCue) &&
                    candidate.score >= bestScore - 25;
                if (candidateIsCompetingActionable)
                {
                    competingActionableCount++;
                }
            }

            if (distinctValues.Count > 1 && competingActionableCount > 0 && bestScore < 150)
            {
                result.intent = NegotiationIntent.CLARIFICATION;
                result.needsClarification = true;
                result.clarificationKind = ClarificationKind.MultipleActionablePrices;
                result.evidence = "multiple actionable price cues in one line";
                reason = "multiple actionable prices require clarification";
                return true;
            }
        }

        result.hasSellerPrice = true;
        result.sellerPrice = chosenPrice;
        result.evidence = cueReason;

        StructuredPriceCandidate selectedCandidate = null;
        for (int i = 0; i < actionablePrices.Count; i++)
        {
            if (actionablePrices[i].value == chosenPrice && actionablePrices[i].score == bestScore)
            {
                selectedCandidate = actionablePrices[i];
                break;
            }
        }

        if (selectedCandidate != null && selectedCandidate.actionCue)
        {
            Level1DebugForceAccept.LogParser("[PRICE SELECTION] selected=" + chosenPrice +
                                             " | reason=" + cueReason +
                                             " | structuredResultReturned=true | legacyFallbackUsed=false");
        }
        else
        {
            Level1DebugForceAccept.LogParser("[PRICE SELECTION] selected=" + chosenPrice +
                                             " | reason=" + cueReason +
                                             " | structuredResultReturned=false | legacyFallbackUsed=true");
        }

        int bestRejectedScore = int.MinValue;
        int bestRejectedPrice = -1;
        for (int i = 0; i < actionablePrices.Count; i++)
        {
            StructuredPriceCandidate candidate = actionablePrices[i];
            if (candidate.value == chosenPrice)
            {
                continue;
            }

            if ((candidate.rejectionCue || candidate.historicalCue) && candidate.score > bestRejectedScore)
            {
                bestRejectedScore = candidate.score;
                bestRejectedPrice = candidate.value;
            }
        }
        if (bestRejectedPrice > 0)
        {
            result.rejectedPrice = bestRejectedPrice;
        }

        bool isUltimatum = ContainsAnyPhrase(text,
            "take it or leave it",
            "let us stop arguing",
            "and take it",
            "that is my final price",
            "this is my final price");
        bool isAcceptanceWithPrice = ContainsAnyPhrase(text, "deal at", "fine at", "accept at", "accepted at") ||
                                     Regex.IsMatch(text, @"\b(?:deal|fine|okay|ok|accepted)\b.*\b\d+\b");

        if (isAcceptanceWithPrice)
        {
            result.intent = NegotiationIntent.COUNTER;
            result.acceptanceTarget = chosenPrice;
            reason = "price attached to acceptance language treated as counter/final price";
            return true;
        }

        result.intent = isUltimatum ? NegotiationIntent.ULTIMATUM :
            ((trade != null && trade.npcOffer > 0) || CurrentExpectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter ? NegotiationIntent.COUNTER : NegotiationIntent.PRICE);

        if (ContainsAnyPhrase(text, "you said", "you offered", "earlier", "before"))
        {
            result.negotiationTactic = NegotiationTactic.CONSISTENCY_CHALLENGE;
        }
        else if (ContainsAnyPhrase(text, "i can come down to", "i was asking", "but fine", "fine i can", "i can do"))
        {
            result.negotiationTactic = NegotiationTactic.RELUCTANT_CONCESSION;
        }
        else if (ContainsAnyPhrase(text, "meet somewhere between", "meet in the middle", "split difference"))
        {
            result.negotiationTactic = NegotiationTactic.SPLIT_DIFFERENCE;
        }

        if (result.correctionDetected)
        {
            result.evidence = result.evidence + " | correction detected";
        }

        if (result.asksItem)
        {
            AddSecondaryIntent(result, NegotiationIntent.ITEM_QUERY);
        }
        if (result.asksCurrentOffer)
        {
            AddSecondaryIntent(result, NegotiationIntent.PRICE_QUERY);
        }

        reason = "structured actionable price resolved";
        return true;
    }

    private class StructuredPriceCandidate
    {
        public int value;
        public int clauseIndex;
        public int tokenPosition;
        public bool actionCue;
        public bool finalityCue;
        public bool acceptanceCue;
        public bool rejectionCue;
        public bool historicalCue;
        public bool correctionCue;
        public bool directionalSourceCue;
        public bool directionalDestinationCue;
        public int score;
        public string cue;
    }

    private static List<StructuredPriceCandidate> CollectStructuredPriceCandidates(string text, string[] tokens)
    {
        List<StructuredPriceCandidate> candidates = new List<StructuredPriceCandidate>();

        for (int i = 0; i < tokens.Length; i++)
        {
            if (!TryParseNumberToken(tokens[i], out int value) || value <= 0)
            {
                continue;
            }

            string beforeWindow = JoinTokenWindow(tokens, Mathf.Max(0, i - 5), i - 1);
            string afterWindow = JoinTokenWindow(tokens, i + 1, Mathf.Min(tokens.Length - 1, i + 3));
            int clauseIndex = CountClausesBeforeToken(tokens, i);
            bool hasBeforeNumberFinalityCue =
                WindowEndsWith(beforeWindow, "final") ||
                WindowEndsWith(beforeWindow, "final price") ||
                WindowEndsWith(beforeWindow, "final price is") ||
                WindowEndsWith(beforeWindow, "final offer") ||
                WindowEndsWith(beforeWindow, "my final is") ||
                WindowEndsWith(beforeWindow, "my final price") ||
                WindowEndsWith(beforeWindow, "my final price is") ||
                WindowEndsWith(beforeWindow, "bottom line") ||
                WindowEndsWith(beforeWindow, "bottom line is");
            bool hasAfterNumberFinalityCue =
                WindowStartsWith(afterWindow, "is final") ||
                WindowStartsWith(afterWindow, "is final price") ||
                WindowStartsWith(afterWindow, "is my final") ||
                WindowStartsWith(afterWindow, "is my final price");

            bool hasActionCue =
                WindowEndsWith(beforeWindow, "make it") ||
                WindowEndsWith(beforeWindow, "make") ||
                WindowEndsWith(beforeWindow, "give me") ||
                WindowEndsWith(beforeWindow, "give") ||
                WindowEndsWith(beforeWindow, "i want") ||
                WindowEndsWith(beforeWindow, "want") ||
                WindowEndsWith(beforeWindow, "my price is") ||
                WindowEndsWith(beforeWindow, "price is") ||
                WindowEndsWith(beforeWindow, "i accept") ||
                WindowEndsWith(beforeWindow, "i agree to") ||
                WindowEndsWith(beforeWindow, "agree to") ||
                hasBeforeNumberFinalityCue ||
                hasAfterNumberFinalityCue ||
                WindowEndsWith(beforeWindow, "final offer") ||
                WindowEndsWith(beforeWindow, "my final is") ||
                WindowEndsWith(beforeWindow, "bottom line") ||
                WindowEndsWith(beforeWindow, "settle at") ||
                WindowEndsWith(beforeWindow, "settle it at") ||
                WindowEndsWith(beforeWindow, "i can do") ||
                WindowEndsWith(beforeWindow, "can do") ||
                WindowEndsWith(beforeWindow, "i do") ||
                WindowEndsWith(beforeWindow, "i can accept at") ||
                WindowEndsWith(beforeWindow, "accept at") ||
                WindowEndsWith(beforeWindow, "i would accept") ||
                WindowEndsWith(beforeWindow, "would accept") ||
                WindowEndsWith(beforeWindow, "i can agree at") ||
                WindowEndsWith(beforeWindow, "can agree at") ||
                WindowEndsWith(beforeWindow, "i will agree at") ||
                WindowEndsWith(beforeWindow, "will agree at") ||
                WindowEndsWith(beforeWindow, "i will take") ||
                WindowEndsWith(beforeWindow, "take") ||
                WindowEndsWith(beforeWindow, "i need") ||
                WindowEndsWith(beforeWindow, "need") ||
                WindowEndsWith(beforeWindow, "come down to") ||
                WindowEndsWith(beforeWindow, "lower it to") ||
                WindowEndsWith(beforeWindow, "reduce it to") ||
                WindowEndsWith(beforeWindow, "reduce my ask to") ||
                WindowEndsWith(beforeWindow, "meet you at") ||
                WindowEndsWith(beforeWindow, "meet at") ||
                WindowEndsWith(beforeWindow, "compromise at") ||
                WindowEndsWith(beforeWindow, "lowest is") ||
                WindowEndsWith(beforeWindow, "raise it to") ||
                WindowEndsWith(beforeWindow, "offer me") ||
                WindowEndsWith(beforeWindow, "i said") ||
                WindowEndsWith(beforeWindow, "said") ||
                WindowEndsWith(beforeWindow, "leave it at") ||
                WindowEndsWith(beforeWindow, "leave amount at") ||
                WindowEndsWith(beforeWindow, "leave value at") ||
                WindowEndsWith(beforeWindow, "leave rate at") ||
                WindowEndsWith(beforeWindow, "leave price at") ||
                WindowEndsWith(beforeWindow, "leave offer at") ||
                WindowEndsWith(beforeWindow, "leave my price at") ||
                WindowEndsWith(beforeWindow, "keep it at") ||
                WindowEndsWith(beforeWindow, "keep price at") ||
                WindowEndsWith(beforeWindow, "hold it at") ||
                WindowEndsWith(beforeWindow, "hold price at") ||
                WindowEndsWith(beforeWindow, "make that") ||
                WindowEndsWith(beforeWindow, "put it at") ||
                WindowEndsWith(beforeWindow, "set it at") ||
                WindowEndsWith(beforeWindow, "go with");

            bool hasFinalityCue =
                hasBeforeNumberFinalityCue ||
                hasAfterNumberFinalityCue ||
                WindowEndsWith(beforeWindow, "lowest is") ||
                WindowContains(afterWindow, "final") ||
                WindowContains(afterWindow, "lowest") ||
                WindowContains(afterWindow, "only");

            bool hasAcceptanceCue =
                WindowEndsWith(beforeWindow, "i accept") ||
                WindowEndsWith(beforeWindow, "i agree to") ||
                WindowEndsWith(beforeWindow, "agree to") ||
                WindowEndsWith(beforeWindow, "i would accept") ||
                WindowEndsWith(beforeWindow, "would accept") ||
                WindowEndsWith(beforeWindow, "i will agree at") ||
                WindowEndsWith(beforeWindow, "will agree at") ||
                WindowEndsWith(beforeWindow, "i can agree at") ||
                WindowEndsWith(beforeWindow, "can agree at") ||
                WindowEndsWith(beforeWindow, "i will take") ||
                WindowEndsWith(beforeWindow, "take") ||
                WindowContains(afterWindow, "works") ||
                WindowContains(afterWindow, "fine") ||
                WindowContains(afterWindow, "acceptable");

            bool hasRejectionCue =
                WindowEndsWith(beforeWindow, "no deal at") ||
                WindowEndsWith(beforeWindow, "not") ||
                WindowEndsWith(beforeWindow, "too low") ||
                WindowEndsWith(beforeWindow, "too high") ||
                WindowEndsWith(beforeWindow, "cannot do") ||
                WindowEndsWith(beforeWindow, "can t do") ||
                WindowEndsWith(beforeWindow, "cant do") ||
                WindowContains(afterWindow, "too low") ||
                WindowContains(afterWindow, "too high") ||
                WindowStartsWith(afterWindow, "is low") ||
                WindowStartsWith(afterWindow, "is too low");

            bool hasHistoricalCue =
                WindowEndsWith(beforeWindow, "your") ||
                WindowContains(beforeWindow, "you offered") ||
                WindowContains(beforeWindow, "you said") ||
                WindowContains(beforeWindow, "offered") ||
                WindowContains(beforeWindow, "said") ||
                WindowContains(beforeWindow, "your last offer was") ||
                WindowContains(beforeWindow, "you started at") ||
                WindowContains(beforeWindow, "moved from") ||
                WindowContains(beforeWindow, "we discussed") ||
                WindowContains(beforeWindow, "earlier") ||
                WindowContains(beforeWindow, "previously") ||
                WindowContains(beforeWindow, "before") ||
                WindowContains(beforeWindow, "first") ||
                WindowContains(beforeWindow, "then said") ||
                WindowContains(beforeWindow, "wanted") ||
                WindowContains(beforeWindow, "asked") ||
                WindowContains(beforeWindow, "preferred") ||
                WindowContains(beforeWindow, "hoping for") ||
                WindowContains(beforeWindow, "considered") ||
                WindowContains(beforeWindow, "original price") ||
                WindowContains(beforeWindow, "started at") ||
                WindowContains(beforeWindow, "was my price") ||
                WindowContains(beforeWindow, "opening was") ||
                WindowContains(beforeWindow, "your last was") ||
                WindowContains(afterWindow, "earlier");

            bool hasCorrectionCue =
                WindowEndsWith(beforeWindow, "make it") ||
                WindowEndsWith(beforeWindow, "give me") ||
                WindowEndsWith(beforeWindow, "give") ||
                WindowEndsWith(beforeWindow, "i said") ||
                WindowEndsWith(beforeWindow, "said") ||
                WindowEndsWith(beforeWindow, "not") ||
                WindowEndsWith(beforeWindow, "sorry") ||
                WindowEndsWith(beforeWindow, "i mean") ||
                WindowEndsWith(beforeWindow, "mean") ||
                WindowEndsWith(beforeWindow, "rather") ||
                WindowEndsWith(beforeWindow, "actually") ||
                WindowEndsWith(beforeWindow, "instead") ||
                WindowEndsWith(beforeWindow, "wrong") ||
                WindowEndsWith(beforeWindow, "by mistake") ||
                WindowEndsWith(beforeWindow, "replace") ||
                WindowEndsWith(beforeWindow, "from");

            bool directionalSourceCue =
                WindowEndsWith(beforeWindow, "from") ||
                WindowEndsWith(beforeWindow, "started at") ||
                WindowEndsWith(beforeWindow, "opening was") ||
                WindowEndsWith(beforeWindow, "first price was");

            bool directionalDestinationCue =
                (i > 0 && tokens[i - 1] == "to" && WindowContains(beforeWindow, "from")) ||
                WindowEndsWith(beforeWindow, "reduce it to") ||
                WindowEndsWith(beforeWindow, "reduce my ask to") ||
                WindowEndsWith(beforeWindow, "come down to") ||
                WindowEndsWith(beforeWindow, "lower it to") ||
                WindowEndsWith(beforeWindow, "move to") ||
                WindowEndsWith(beforeWindow, "change to");

            int score = 0;
            if (hasActionCue)
            {
                score += 140;
            }
            if (hasFinalityCue)
            {
                score += 95;
            }
            if (hasAcceptanceCue)
            {
                score += 40;
            }
            if (WindowEndsWith(beforeWindow, "at") || WindowEndsWith(beforeWindow, "for"))
            {
                score += 20;
            }
            if (WindowContains(afterWindow, "varaha") || WindowContains(afterWindow, "varahas"))
            {
                score += 10;
            }
            if (hasCorrectionCue)
            {
                score += 35;
            }
            if (directionalDestinationCue)
            {
                score += 85;
            }
            if (directionalSourceCue)
            {
                score -= 55;
            }
            if (hasRejectionCue && !hasActionCue)
            {
                score -= 95;
            }
            else if (hasRejectionCue)
            {
                score -= 20;
            }
            if (hasHistoricalCue && !hasActionCue)
            {
                score -= 70;
            }
            if (hasHistoricalCue && (hasActionCue || hasFinalityCue || hasAcceptanceCue))
            {
                score -= 15;
            }
            if (clauseIndex > 0 && (hasActionCue || hasFinalityCue || hasAcceptanceCue || hasCorrectionCue))
            {
                score += clauseIndex * 24;
            }
            else
            {
                score += clauseIndex * 8;
            }
            if (i > 0 && (tokens[i - 1] == "but" || tokens[i - 1] == "however" || tokens[i - 1] == "instead" || tokens[i - 1] == "actually" || tokens[i - 1] == "rather" || tokens[i - 1] == "now"))
            {
                score += 45;
            }
            if (i > 1 && (tokens[i - 2] == "but" || tokens[i - 2] == "however" || tokens[i - 2] == "instead"))
            {
                score += 30;
            }
            if (WindowContains(beforeWindow, "now") && (hasActionCue || hasFinalityCue || hasAcceptanceCue))
            {
                score += 55;
            }

            StructuredPriceCandidate candidate = new StructuredPriceCandidate
            {
                value = value,
                clauseIndex = clauseIndex,
                tokenPosition = i,
                actionCue = hasActionCue,
                finalityCue = hasFinalityCue,
                acceptanceCue = hasAcceptanceCue,
                rejectionCue = hasRejectionCue,
                historicalCue = hasHistoricalCue,
                correctionCue = hasCorrectionCue,
                directionalSourceCue = directionalSourceCue,
                directionalDestinationCue = directionalDestinationCue,
                score = score,
                cue = "candidate score=" + score + " | actionCue=" + hasActionCue + " | finalityCue=" + hasFinalityCue + " | acceptanceCue=" + hasAcceptanceCue + " | rejectionCue=" + hasRejectionCue + " | historicalCue=" + hasHistoricalCue + " | correctionCue=" + hasCorrectionCue + " | directionalSourceCue=" + directionalSourceCue + " | directionalDestinationCue=" + directionalDestinationCue
            };
            candidates.Add(candidate);
            Level1DebugForceAccept.LogParser("[PRICE CANDIDATE] value=" + candidate.value +
                                             " | position=" + candidate.tokenPosition +
                                             " | clauseIndex=" + candidate.clauseIndex +
                                             " | actionCue=" + candidate.actionCue +
                                             " | finalityCue=" + candidate.finalityCue +
                                             " | acceptanceCue=" + candidate.acceptanceCue +
                                             " | rejectionCue=" + candidate.rejectionCue +
                                             " | historicalCue=" + candidate.historicalCue +
                                             " | correctionCue=" + candidate.correctionCue +
                                             " | directionalSourceCue=" + candidate.directionalSourceCue +
                                             " | directionalDestinationCue=" + candidate.directionalDestinationCue +
                                             " | score=" + candidate.score);
        }

        return candidates;
    }

    private static int CountClausesBeforeToken(string[] tokens, int tokenIndex)
    {
        if (tokens == null || tokenIndex <= 0)
        {
            return 0;
        }

        int clauses = 0;
        for (int i = 0; i < tokenIndex; i++)
        {
            if (tokens[i] == "but" || tokens[i] == "however" || tokens[i] == "instead" || tokens[i] == "then" || tokens[i] == "so")
            {
                clauses++;
            }
        }

        return clauses;
    }

    private static string JoinTokenWindow(string[] tokens, int start, int end)
    {
        if (tokens == null || start > end || start < 0 || end < 0 || start >= tokens.Length)
        {
            return string.Empty;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        int clampedEnd = Mathf.Min(end, tokens.Length - 1);
        for (int i = start; i <= clampedEnd; i++)
        {
            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(tokens[i]);
        }

        return builder.ToString();
    }

    private static bool WindowContains(string window, string phrase)
    {
        return !string.IsNullOrEmpty(window) && window.Contains(phrase);
    }

    private static bool WindowEndsWith(string window, string phrase)
    {
        return !string.IsNullOrEmpty(window) && window.EndsWith(phrase, StringComparison.Ordinal);
    }

    private static bool WindowStartsWith(string window, string phrase)
    {
        return !string.IsNullOrEmpty(window) && window.StartsWith(phrase, StringComparison.Ordinal);
    }

    private bool TryParseStructuredBargain(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        if (result == null)
        {
            return false;
        }

        if (hasTradeNumber || HasDigits(text))
        {
            return false;
        }

        if (TryDetectExplicitTerminalIntent(text, tokens ?? Array.Empty<string>(), trade, false, -1, out _, out _, out _))
        {
            return false;
        }

        if (IsPureAcceptance(text, text, tokens ?? Array.Empty<string>()) || IsPureRejection(text, tokens ?? Array.Empty<string>()))
        {
            return false;
        }

        bool isSoftBargain =
            MatchesDealImprovementBargain(text) ||
            ContainsAnyPhrase(text,
                "i am not saying no",
                "im not saying no",
                "i cannot accept that",
                "that will not do",
                "perhaps we can meet somewhere between",
                "you must improve the price",
                "improve the offer",
                "raise it",
                "raise your offer",
                "can you do better",
                "offer me more",
                "that is too little",
                "come up a bit",
                "give me a fairer price",
                "you can do better than that",
                "a little more",
                "meet me halfway",
                "make it worth my while",
                "close the gap",
                "try again",
                "that offer needs work",
                "give me something better",
                "move higher",
                "we are too far apart",
                "come closer to my price",
                "sweeten the deal",
                "offer a fair amount",
                "give fairer price",
                "sweeten deal",
                "offer fair amount",
                "you need to improve",
                "offer more",
                "little more",
                "do better",
                "do better than that",
                "come up bit",
                "meet halfway",
                "close gap",
                "give something better",
                "need to improve") ||
            MatchesAnyPattern(text,
                @"\b(?:improve|improve the)\s+offer\b",
                @"\bcan you do better\b",
                @"\boffer me more\b",
                @"\bthat is too little\b",
                @"\bcome up a bit\b",
                @"\bgive me (?:a )?fair(?:er)? price\b",
                @"\byou can do better than that\b",
                @"\ba little more\b",
                @"\bmeet me halfway\b",
                @"\bmake it worth my while\b",
                @"\bclose the gap\b",
                @"\btry again\b",
                @"\bthat offer needs work\b",
                @"\bgive me something better\b",
                @"\bmove higher\b",
                @"\bwe are too far apart\b",
                @"\bcome closer to my price\b",
                @"\bsweeten the deal\b",
                @"\boffer a fair amount\b",
                @"\bgive fairer price\b",
                @"\bsweeten deal\b",
                @"\boffer fair amount\b",
                @"\byou need to improve\b",
                @"\boffer more\b",
                @"\blittle more\b",
                @"\bdo better\b",
                @"\bdo better than that\b",
                @"\bcome up bit\b",
                @"\bmeet halfway\b",
                @"\bclose gap\b",
                @"\bgive something better\b",
                @"\bneed to improve\b");

        if (isSoftBargain)
        {
            result.intent = NegotiationIntent.BARGAIN;
            result.rejectsCurrentOffer = true;
            result.evidence = "soft bargaining language without final numeric ask";
            reason = "structured bargain language";
            return true;
        }

        return false;
    }

    private static bool MatchesDealImprovementBargain(string text)
    {
        return ContainsAnyPhrase(text,
                   "make deal better",
                   "improve deal",
                   "give better deal",
                   "make this deal better",
                   "make deal fairer",
                   "give fairer deal",
                   "can improve deal",
                   "need to improve deal") ||
               MatchesAnyPattern(text,
                   @"\bmake(?: this)? deal better\b",
                   @"\bimprove deal\b",
                   @"\bgive (?:me )?better deal\b",
                   @"\bmake deal fairer\b",
                   @"\bgive (?:me )?fairer deal\b",
                   @"\bcan(?: you)? improve deal\b",
                   @"\bneed to improve deal\b");
    }

    private static NegotiationTactic DetectNegotiationTactic(string text, string[] tokens)
    {
        if (ContainsAnyPhrase(text, "you said", "you offered", "earlier", "before"))
        {
            return NegotiationTactic.CONSISTENCY_CHALLENGE;
        }
        if (ContainsAnyPhrase(text, "fair", "too little", "so little", "fair price"))
        {
            return NegotiationTactic.APPEAL_TO_FAIRNESS;
        }
        if (ContainsAnyPhrase(text, "come down to", "i was asking", "but fine"))
        {
            return NegotiationTactic.RELUCTANT_CONCESSION;
        }
        if (ContainsAnyPhrase(text, "meet somewhere between", "meet in the middle", "split difference"))
        {
            return NegotiationTactic.SPLIT_DIFFERENCE;
        }
        if (ContainsAnyPhrase(text, "final price", "take it or leave it", "we are done"))
        {
            return NegotiationTactic.FINAL_OFFER;
        }
        if (ContainsAnyPhrase(text, "how can i help you", "how i help", "what brings you here"))
        {
            return NegotiationTactic.FRIENDLY_SMALL_TALK;
        }

        return NegotiationTactic.NONE;
    }

    private bool HasReplacementOfferAttachedToRejection(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber)
    {
        if (!hasTradeNumber || detectedNumber <= 0)
        {
            return false;
        }

        if (!TryParsePriceOffer(text, tokens, trade, hasTradeNumber, detectedNumber, out _, out NegotiationIntent offerIntent, out _))
        {
            return false;
        }

        return offerIntent == NegotiationIntent.COUNTER ||
               offerIntent == NegotiationIntent.PRICE ||
               offerIntent == NegotiationIntent.ULTIMATUM;
    }

    private static bool MatchesDismissPhrase(string text, string[] tokens)
    {
        if (IsLeavePriceConstruction(text))
        {
            return false;
        }

        if (ContainsAnyPhrase(text,
                "go away",
                "no go away",
                "please leave",
                "leave me",
                "get lost",
                "get out",
                "move along",
                "i do not want to talk",
                "leave my stall",
                "be gone",
                "walk away",
                "get away from here",
                "go bother someone else",
                "out of my stall"))
        {
            return true;
        }

        if (ContainsAnyPhrase(text, "enough go", "leave alone"))
        {
            return true;
        }

        if (tokens.Length == 1 && (tokens[0] == "leave" || tokens[0] == "stop"))
        {
            return true;
        }

        if (ContainsAnyToken(tokens, "leave") && !ContainsAnyPhrase(text, "leave it at", "leave the price at", "leave it", "leave price", "leave amount at", "leave value at", "leave rate at"))
        {
            return ContainsAnyToken(tokens, "please", "just", "now", "away", "me") || text == "leave";
        }

        return false;
    }

    private static bool IsLeavePriceConstruction(string text)
    {
        return MatchesAnyPattern(text,
            @"\bleave\s+(?:it|price|offer|amount)\s+at\s+\d+\b",
            @"\bleave\s+the\s+(?:price|offer|amount)\s+at\s+\d+\b",
            @"\blet us leave\s+it\s+at\s+\d+\b",
            @"\bwe can leave\s+it\s+at\s+\d+\b",
            @"\bi(?:\s+\w+){0,2}\s+leave\s+(?:my\s+)?(?:price|offer|amount)\s+at\s+\d+\b",
            @"\bkeep\s+it\s+at\s+\d+\b",
            @"\bhold\s+it\s+at\s+\d+\b",
            @"\bsettle\s+it\s+at\s+\d+\b");
    }

    private static bool MatchesHardRejectPhrase(string text, string[] tokens)
    {
        if (ContainsAnyPhrase(text,
                "forget it",
                "cancel the trade",
                "cancel trade",
                "cancel deal",
                "i do not want to sell",
                "i don't want to sell",
                "i dont want to sell",
                "i am not selling",
                "no sale",
                "we are done",
                "end this",
                "end negotiation",
                "negotiation is over",
                "stop bargaining",
                "forget whole thing",
                "nothing more to discuss",
                "i am finished",
                "i finished",
                "i am walking away",
                "there will be no trade",
                "there be no trade",
                "i changed my mind",
                "i do not want to sell this anymore",
                "i don't want to sell this anymore",
                "i dont want to sell this anymore"))
        {
            return true;
        }

        if (ContainsPhrase(text, "no deal") && !ContainsAnyPhrase(text, "no deal at that price", "no deal at that amount"))
        {
            return true;
        }

        if (ContainsPhrase(text, "not interested") && !ContainsAnyPhrase(text, "not interested at that amount", "not interested at that price"))
        {
            return true;
        }

        return tokens.Length >= 2 &&
               ContainsAnyToken(tokens, "done") &&
               ContainsAnyToken(tokens, "stop", "okay", "ok", "we", "are");
    }

    private bool MatchesSoftRejectPhrase(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber)
    {
        if (HasReplacementOfferAttachedToRejection(text, tokens, trade, hasTradeNumber, detectedNumber))
        {
            return false;
        }

        if (ContainsAnyPhrase(text,
                "no thanks",
                "not good enough",
                "too low",
                "too high",
                "cannot take that",
                "can t take that",
                "cant take that",
                "that will not work",
                "that not work",
                "unacceptable",
                "reject that offer",
                "no deal at that price",
                "not interested at that amount",
                "need better",
                "cannot agree",
                "can t agree",
                "cant agree",
                "price is impossible",
                "pass on that offer"))
        {
            return true;
        }

        return false;
    }

    private bool TryParseTradeNumber(string text, string[] tokens, out int detectedNumber, out string reason)
    {
        detectedNumber = -1;
        reason = "no trade number found";

        for (int i = 0; i < tokens.Length; i++)
        {
            if (TryParseNumberToken(tokens[i], out detectedNumber))
            {
                reason = "numeric token '" + tokens[i] + "'";
                return true;
            }

            if (i + 1 < tokens.Length && TryParseCompoundNumber(tokens[i], tokens[i + 1], out detectedNumber))
            {
                reason = "compound number tokens '" + tokens[i] + " " + tokens[i + 1] + "'";
                return true;
            }
        }

        return false;
    }

    private int CountTradeNumbers(string[] tokens)
    {
        if (tokens == null || tokens.Length == 0)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < tokens.Length; i++)
        {
            if (TryParseNumberToken(tokens[i], out int parsed) && parsed > 0)
            {
                count++;
                continue;
            }

            if (i + 1 < tokens.Length && TryParseCompoundNumber(tokens[i], tokens[i + 1], out parsed) && parsed > 0)
            {
                count++;
                i++;
            }
        }

        return count;
    }

    private bool TrySelectPlausiblePriceToken(string text, string[] tokens, LocalTradeState trade, out int selectedPrice, out string reason)
    {
        selectedPrice = -1;
        reason = "no plausible price token";

        int marketValue = trade != null ? trade.marketValue : 0;
        int buyerMax = trade != null ? trade.maxBuyerPrice : 0;
        int bestScore = int.MinValue;

        for (int i = 0; i < tokens.Length; i++)
        {
            if (!TryParseNumberToken(tokens[i], out int candidate))
            {
                continue;
            }

            int score = 0;
            string previous = i > 0 ? tokens[i - 1] : string.Empty;
            string previousTwo = i > 1 ? tokens[i - 2] : string.Empty;

            if (previous == "for" || previous == "at")
            {
                score += 60;
            }

            if (previous == "give" || previous == "offer" || previous == "pay" || previous == "make" ||
                previousTwo == "give" || previousTwo == "offer" || previousTwo == "pay" || previousTwo == "make")
            {
                score += 80;
            }

            if (candidate == 4 && i + 1 < tokens.Length && TryParseNumberToken(tokens[i + 1], out int nextCandidate) && nextCandidate >= 10)
            {
                score -= 120;
            }

            if (marketValue > 0)
            {
                int distanceToMarket = Mathf.Abs(candidate - marketValue);
                score += Mathf.Max(0, 45 - distanceToMarket);
            }

            if (buyerMax > 0)
            {
                int distanceToBuyerMax = Mathf.Abs(candidate - buyerMax);
                score += Mathf.Max(0, 30 - distanceToBuyerMax);
            }

            if (candidate >= 10)
            {
                score += 10;
            }

            if (candidate > selectedPrice)
            {
                score += 4;
            }

            if (score > bestScore)
            {
                bestScore = score;
                selectedPrice = candidate;
                reason = "plausible price token '" + candidate + "' at index " + i + " score=" + score;
            }
        }

        return selectedPrice > 0;
    }

    private bool TryParsePriceOffer(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, out int sellerPrice, out NegotiationIntent intent, out string reason)
    {
        sellerPrice = -1;
        intent = NegotiationIntent.UNKNOWN;
        reason = "no price offer parsed";

        if (!hasTradeNumber || detectedNumber <= 0)
        {
            return false;
        }

        if (IsQuantityContext(text, tokens, trade))
        {
            reason = "quantity context wins over price parsing";
            return false;
        }

        bool offerContext = IsOfferContext(text, tokens, trade);
        if (!offerContext)
        {
            reason = "number present without offer context";
            return false;
        }

        int selectedPrice = detectedNumber;
        if (TrySelectPlausiblePriceToken(text, tokens, trade, out int plausiblePrice, out string plausibleReason))
        {
            selectedPrice = plausiblePrice;
            reason = plausibleReason;
        }

        sellerPrice = selectedPrice;
        bool hasAcceptanceLeadIn = ContainsAnyToken(tokens, "yes", "okay", "ok", "fine", "deal", "accept", "accepted", "agreed", "no");
        bool hasUltimatumLeadIn = ContainsAnyPhrase(text, "take it or leave it", "final price", "this is my final price", "last price", "last offer") ||
            MatchesAnyPattern(text, @"not going lower than\s+\d+", @"nothing less than\s+\d+", @"not less than\s+\d+", @"minimum(?: price)?\s*(?:is)?\s*\d+");
        bool hasCounterLeadIn = ContainsAnyPhrase(text, "make it", "deal at", "okay at", "fine at", "accepted at", "i will pay", "i ll pay", "i'll pay", "i can pay", "my offer is", "my offer", "i offer", "i can offer", "i give", "no ") ||
            ContainsAnyToken(tokens, "pay", "offer", "make", "counter", "price");

        if (hasUltimatumLeadIn)
        {
            intent = NegotiationIntent.ULTIMATUM;
            reason = reason + " | price offer parsed as ultimatum";
            return true;
        }

        if (CurrentExpectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter || hasAcceptanceLeadIn || hasCounterLeadIn)
        {
            intent = NegotiationIntent.COUNTER;
            reason = reason + " | price offer parsed as counter-offer";
            return true;
        }

        intent = NegotiationIntent.PRICE;
        reason = reason + " | price offer parsed from offer context";
        return true;
    }

    private bool TryParseQuantityAnswer(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, out int quantityGrams, out string reason)
    {
        quantityGrams = -1;
        reason = "no quantity parsed";

        if (TryExtractQuantity(text, trade, out quantityGrams))
        {
            reason = "explicit quantity unit parsed";
            return true;
        }

        if (!hasTradeNumber || detectedNumber <= 0)
        {
            return false;
        }

        bool quantityContext = IsQuantityContext(text, tokens, trade);
        if (!quantityContext)
        {
            return false;
        }

        int referenceQuantity = trade != null && trade.quantityGrams > 0 ? trade.quantityGrams : 280;
        quantityGrams = Mathf.Max(1, detectedNumber) * Mathf.Max(1, referenceQuantity);
        reason = "state-aware quantity answer parsed from short numeric reply";
        return true;
    }

    private bool HasCounterPriceAttachedToAcceptance(string text, string[] tokens)
    {
        if (!ContainsAnyToken(tokens, "yes", "okay", "ok", "fine", "deal", "accept", "accepted", "agreed", "no"))
        {
            return false;
        }

        return HasDigits(text) || ContainsAnyToken(tokens,
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
            "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
            "twenty", "thirty", "forty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety");
    }

    private bool IsPureAcceptance(string rawText, string text, string[] tokens)
    {
        if (HasCounterPriceAttachedToAcceptance(text, tokens) ||
            HasDigits(rawText) ||
            ContainsQuestionSignal(text) ||
            (!string.IsNullOrWhiteSpace(rawText) && rawText.Contains("?")) ||
            ContainsNegatedAcceptanceCue(rawText) ||
            ContainsNegatedAcceptanceCue(text))
        {
            return false;
        }

        if (ContainsAnyPhrase(rawText,
                "works for me",
                "works for us",
                "alright then",
                "very well",
                "i will take it",
                "that will work",
                "agreed then",
                "fair enough",
                "go ahead",
                "let us do it",
                "we have a deal",
                "you have a deal") ||
            ContainsAnyPhrase(text, "i accept", "sounds good", "that works", "i take it", "that work"))
        {
            return true;
        }

        if (ContainsAnyPhrase(rawText, "sounds fine", "sounds reasonable", "fine by me", "okay then") ||
            ContainsAnyPhrase(text, "sounds fine", "that deal sounds fine", "not bad deal"))
        {
            return true;
        }

        foreach (string token in tokens)
        {
            if (token != "yes" && token != "okay" && token != "ok" && token != "fine" && token != "deal" && token != "accept" && token != "agreed" && token != "sure" && token != "done")
            {
                return false;
            }
        }

        return tokens.Length > 0;
    }

    private bool IsPureRejection(string text, string[] tokens)
    {
        if (HasCounterPriceAttachedToAcceptance(text, tokens) || ContainsQuestionSignal(text))
        {
            return false;
        }

        if (ContainsAnyPhrase(text, "walk away", "not interested", "no deal", "never mind"))
        {
            return true;
        }

        foreach (string token in tokens)
        {
            if (token != "no" && token != "reject")
            {
                return false;
            }
        }

        return tokens.Length > 0;
    }

    private bool IsHardRejectPhrase(string text, string[] tokens)
    {
        if (ContainsAnyPhrase(text, "walk away", "not interested", "no deal", "never mind", "forget it", "no sale", "we are done", "end this"))
        {
            return true;
        }

        return ContainsAnyToken(tokens, "reject");
    }

    private bool IsConfusedPhrase(string text, string[] tokens)
    {
        if (ContainsAnyPhrase(text, "i do not know", "i don't know", "dont know", "not sure", "do not understand", "don't understand", "say again", "come again", "repeat that", "repeat please", "what do you mean"))
        {
            return true;
        }

        if (tokens.Length == 1 && ContainsAnyToken(tokens, "what", "huh", "pardon", "repeat"))
        {
            return true;
        }

        return false;
    }

    private bool IsOfferContext(string text, string[] tokens, LocalTradeState trade)
    {
        if (CurrentExpectedReplyState == ExpectedReplyState.ExpectOfferPrice || CurrentExpectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter)
        {
            return true;
        }

        if (ContainsAnyPhrase(text, "i ll pay", "i'll pay", "i will pay", "i can pay", "my offer is", "my offer", "i offer", "i can offer", "make it", "deal at", "pay", "offer"))
        {
            return true;
        }

        if (ContainsAnyToken(tokens, "varaha", "varahas", "price", "offer", "pay", "sell", "final", "last", "make"))
        {
            return true;
        }

        return trade != null && trade.npcOffer > 0 && text.Split(' ').Length <= 2;
    }

    private bool IsQuantityContext(string text, string[] tokens, LocalTradeState trade)
    {
        if (CurrentExpectedReplyState == ExpectedReplyState.ExpectQuantity)
        {
            return true;
        }

        if (ContainsAnyToken(tokens, "bag", "bags", "quantity", "amount", "weight", "palam", "palams", "seer", "seers", "veesai", "viss", "kg", "kgs", "gram", "grams"))
        {
            return true;
        }

        if (ContainsAnyPhrase(text, "i want", "want", "give me", "give", "need", "take") &&
            !ContainsAnyToken(tokens, "varaha", "varahas", "price", "offer", "pay") &&
            CurrentExpectedReplyState != ExpectedReplyState.ExpectOfferPrice &&
            CurrentExpectedReplyState != ExpectedReplyState.ExpectAcceptOrCounter)
        {
            return trade != null;
        }

        return false;
    }

    public void ProcessNegotiationTurn(NegotiationInput input)
    {
        ProcessNegotiationTurn(input != null ? input.intent : NegotiationIntent.UNKNOWN);
    }

    public void ProcessNegotiationTurn(NegotiationIntent intent)
    {
        if (IsNegotiationFinished)
        {
            return;
        }

        bool countsAsNegotiation = CountsAsNegotiationPressure(intent);
        LastTurnCountedAsNegotiation = countsAsNegotiation;
        if (countsAsNegotiation)
        {
            CurrentRound++;
        }
        RepeatedIntentCount = intent == LastIntent ? RepeatedIntentCount + 1 : 1;
        LastIntent = intent;

        switch (intent)
        {
            case NegotiationIntent.ACCEPT:
                IsNegotiationFinished = true;
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                break;
            case NegotiationIntent.REJECT:
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                BuyerPatience = Mathf.Max(0, BuyerPatience - 1);
                break;
            case NegotiationIntent.BARGAIN:
            case NegotiationIntent.PRICE:
            case NegotiationIntent.COUNTER:
            case NegotiationIntent.ULTIMATUM:
            case NegotiationIntent.QUANTITY_PRICE:
                ConsecutiveBargains++;
                ConsecutiveQueries = 0;
                BuyerPatience = Mathf.Max(0, BuyerPatience - 1);
                break;
            case NegotiationIntent.PRICE_QUERY:
            case NegotiationIntent.ITEM_QUERY:
            case NegotiationIntent.QUANTITY_QUERY:
            case NegotiationIntent.QUERY_BUYER_BUDGET:
                ConsecutiveQueries++;
                ConsecutiveBargains = 0;
                break;
            case NegotiationIntent.GREETING:
            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.CONTINUE:
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                break;
            case NegotiationIntent.GENERAL_DIALOGUE:
            case NegotiationIntent.OFF_TOPIC:
            case NegotiationIntent.HOSTILE:
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                OffTopicCount++;
                BuyerPatience = Mathf.Max(0, BuyerPatience - (intent == NegotiationIntent.HOSTILE ? 2 : 1));
                break;
            case NegotiationIntent.CLARIFICATION:
            case NegotiationIntent.CONFUSED:
            case NegotiationIntent.UNKNOWN:
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                UnknownCount++;
                break;
        }

        if (BuyerPatience <= 0 || CurrentRound >= MaxRounds)
        {
            IsNegotiationFinished = true;
        }

        Level1DebugForceAccept.LogTrade("[NEGOTIATION TURN] intent=" + intent +
                                        " | countedAsNegotiation=" + countsAsNegotiation +
                                        " | round=" + CurrentRound +
                                        " | patience=" + BuyerPatience);
    }

    private static bool CountsAsNegotiationPressure(NegotiationIntent intent)
    {
        switch (intent)
        {
            case NegotiationIntent.REJECT:
            case NegotiationIntent.BARGAIN:
            case NegotiationIntent.PRICE:
            case NegotiationIntent.COUNTER:
            case NegotiationIntent.ULTIMATUM:
            case NegotiationIntent.QUANTITY_PRICE:
                return true;
            default:
                return false;
        }
    }

    private void ApplyStateSafetyGates(NegotiationInput result, bool hasActiveOffer, ref string reason)
    {
        if (result == null)
        {
            return;
        }

        if (IsInfoQueryIntent(result.intent) || IsTerminalIntent(result.intent, result.hasHardRejection))
        {
            return;
        }

        switch (CurrentExpectedReplyState)
        {
            case ExpectedReplyState.ExpectQuantity:
                if (result.intent == NegotiationIntent.ACCEPT)
                {
                    BlockToClarification(result, ParseReason.StateBlockedAccept, "quantity expected; acceptance blocked", ref reason);
                }
                else if (!IsAllowedWhenExpectingQuantity(result.intent))
                {
                    BlockToClarification(result, result.hasQuantity ? ParseReason.MissingQuantity : ParseReason.MissingQuantity, "quantity expected; redirected to clarification", ref reason);
                }
                break;

            case ExpectedReplyState.ExpectOfferPrice:
                if (result.intent == NegotiationIntent.ACCEPT)
                {
                    BlockToClarification(result, ParseReason.StateBlockedAccept, "offer price expected; acceptance blocked", ref reason);
                }
                else if (!IsAllowedWhenExpectingOffer(result.intent))
                {
                    BlockToClarification(result, ParseReason.MissingPrice, "offer price expected; redirected to clarification", ref reason);
                }
                break;

            case ExpectedReplyState.ExpectAcceptOrCounter:
                if (!IsAllowedWhenExpectingAcceptOrCounter(result.intent))
                {
                    BlockToClarification(result, ParseReason.AmbiguousAcceptOrCounter, "accept-or-counter expected; redirected to clarification", ref reason);
                }
                break;

            case ExpectedReplyState.ExpectFulfillment:
                if (IsNegotiationIntent(result.intent))
                {
                    BlockToClarification(result, ParseReason.FulfillmentExpected, "fulfillment expected; redirected to fulfillment help", ref reason);
                }
                break;
        }

        if (result.intent == NegotiationIntent.ACCEPT)
        {
            bool allowAccept = result.parseConfidence == ParseConfidence.High &&
                (CurrentExpectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter ||
                 (hasActiveOffer && CurrentExpectedReplyState != ExpectedReplyState.ExpectQuantity && CurrentExpectedReplyState != ExpectedReplyState.ExpectOfferPrice));

            if (!allowAccept &&
                hasActiveOffer &&
                lastNpcReplyPresentedOfferAmount &&
                CurrentExpectedReplyState != ExpectedReplyState.ExpectQuantity &&
                CurrentExpectedReplyState != ExpectedReplyState.ExpectFulfillment)
            {
                allowAccept = true;
                Level1DebugForceAccept.LogParser("[NEGOTIATION STATE] acceptFallback=lastNpcReplyPresentedOfferAmount | currentState=" + CurrentExpectedReplyState);
            }

            if (!allowAccept)
            {
                BlockToClarification(result, ParseReason.StateBlockedAccept, "accept safety gate blocked low-confidence or wrong-state accept", ref reason);
            }
        }

        if (result.intent == NegotiationIntent.REJECT && !result.hasHardRejection)
        {
            result.parseConfidence = ParseConfidence.Low;
        }
    }

    private void FinalizeNegotiationInput(NegotiationInput result, string rawInput, string normalizedText, string[] tokens, LocalTradeState trade, bool rawContainsQuestionMark, int detectedNumber)
    {
        if (result == null)
        {
            return;
        }

        normalizedText = normalizedText ?? result.normalizedText ?? string.Empty;
        tokens = tokens ?? Array.Empty<string>();

        if (ShouldDowngradeToPriceConfirmationQuery(result, rawInput, normalizedText, tokens, trade, rawContainsQuestionMark, detectedNumber))
        {
            int referencedPrice = result.hasSellerPrice && result.sellerPrice > 0
                ? result.sellerPrice
                : detectedNumber;

            result.intent = NegotiationIntent.PRICE_QUERY;
            result.hasExplicitAcceptance = false;
            result.hasSellerPrice = false;
            result.sellerPrice = -1;
            result.acceptanceTarget = -1;
            result.needsClarification = false;
            result.terminalAction = false;
            result.clarificationKind = ClarificationKind.None;
            if (referencedPrice > 0 && !result.referencedPrices.Contains(referencedPrice))
            {
                result.referencedPrices.Add(referencedPrice);
            }
            if (result.parseReason == ParseReason.PriceOfferParsed || result.parseReason == ParseReason.PureAcceptance)
            {
                result.parseReason = ParseReason.PriceQuery;
            }
            if (result.parseConfidence == ParseConfidence.Low)
            {
                result.parseConfidence = ParseConfidence.Medium;
            }
            if (string.IsNullOrWhiteSpace(result.evidence))
            {
                result.evidence = "price confirmation question downgraded to non-committing query";
            }
        }

        switch (result.intent)
        {
            case NegotiationIntent.ACCEPT:
                if (result.acceptanceTarget <= 0 && trade != null && trade.npcOffer > 0)
                {
                    result.acceptanceTarget = trade.npcOffer;
                }
                result.hasSellerPrice = false;
                result.sellerPrice = -1;
                result.needsClarification = false;
                result.terminalAction = false;
                break;

            case NegotiationIntent.COUNTER:
                result.acceptanceTarget = -1;
                result.hasExplicitAcceptance = false;
                result.needsClarification = false;
                result.terminalAction = false;
                if (result.sellerPrice > 0)
                {
                    result.hasSellerPrice = true;
                }
                else
                {
                    result.hasSellerPrice = false;
                    result.sellerPrice = -1;
                }
                break;

            case NegotiationIntent.CLARIFICATION:
                result.hasExplicitAcceptance = false;
                result.hasSellerPrice = false;
                result.sellerPrice = -1;
                result.acceptanceTarget = -1;
                result.needsClarification = true;
                result.terminalAction = false;
                break;

            case NegotiationIntent.PRICE_QUERY:
            case NegotiationIntent.ITEM_QUERY:
            case NegotiationIntent.QUANTITY_QUERY:
            case NegotiationIntent.QUERY_BUYER_BUDGET:
            case NegotiationIntent.CONFUSED:
            case NegotiationIntent.GREETING:
            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.GENERAL_DIALOGUE:
            case NegotiationIntent.CONTINUE:
                result.hasExplicitAcceptance = false;
                result.hasSellerPrice = false;
                result.sellerPrice = -1;
                result.acceptanceTarget = -1;
                result.terminalAction = false;
                break;
        }
    }

    private static void BlockToClarification(NegotiationInput result, ParseReason parseReason, string debugReason, ref string reason)
    {
        result.intent = parseReason == ParseReason.FulfillmentExpected ? NegotiationIntent.CLARIFICATION : NegotiationIntent.CLARIFICATION;
        result.hasExplicitAcceptance = false;
        result.hasHardRejection = false;
        result.needsClarification = true;
        result.parseConfidence = ParseConfidence.Low;
        result.parseReason = parseReason;
        result.clarificationKind =
            parseReason == ParseReason.MissingQuantity ? ClarificationKind.MissingQuantity :
            parseReason == ParseReason.MissingPrice || parseReason == ParseReason.StateBlockedAccept ? ClarificationKind.MissingPrice :
            parseReason == ParseReason.AmbiguousAcceptOrCounter ? ClarificationKind.AmbiguousAcceptOrCounter :
            parseReason == ParseReason.FulfillmentExpected ? ClarificationKind.FulfillmentExpected :
            ClarificationKind.UnrecognizedSpeech;
        reason = debugReason;
    }

    private static bool IsInfoQueryIntent(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.ITEM_QUERY ||
               intent == NegotiationIntent.PRICE_QUERY ||
               intent == NegotiationIntent.QUANTITY_QUERY ||
               intent == NegotiationIntent.QUERY_BUYER_BUDGET;
    }

    private static bool IsTerminalIntent(NegotiationIntent intent, bool hasHardRejection)
    {
        return intent == NegotiationIntent.DISMISS ||
               (intent == NegotiationIntent.REJECT && hasHardRejection);
    }

    private static bool IsAllowedWhenExpectingQuantity(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.QUANTITY_CHANGE ||
               intent == NegotiationIntent.QUANTITY_QUERY ||
               intent == NegotiationIntent.CLARIFICATION ||
               intent == NegotiationIntent.CONFUSED;
    }

    private static bool IsAllowedWhenExpectingOffer(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.PRICE ||
               intent == NegotiationIntent.COUNTER ||
               intent == NegotiationIntent.BARGAIN ||
               intent == NegotiationIntent.PRICE_QUERY ||
               intent == NegotiationIntent.QUERY_BUYER_BUDGET ||
               intent == NegotiationIntent.ITEM_QUERY ||
               intent == NegotiationIntent.QUANTITY_QUERY ||
               intent == NegotiationIntent.CLARIFICATION ||
               intent == NegotiationIntent.CONFUSED;
    }

    private static bool IsAllowedWhenExpectingAcceptOrCounter(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.ACCEPT ||
               intent == NegotiationIntent.COUNTER ||
               intent == NegotiationIntent.REJECT ||
               intent == NegotiationIntent.DISMISS ||
               intent == NegotiationIntent.BARGAIN ||
               intent == NegotiationIntent.CLARIFICATION ||
               intent == NegotiationIntent.CONFUSED;
    }

    private static bool ShouldDowngradeToPriceConfirmationQuery(NegotiationInput result, string rawInput, string text, string[] tokens, LocalTradeState trade, bool rawContainsQuestionMark, int detectedNumber)
    {
        if (result == null)
        {
            return false;
        }

        if (result.intent != NegotiationIntent.COUNTER && result.intent != NegotiationIntent.ACCEPT)
        {
            return false;
        }

        if (!rawContainsQuestionMark && !ContainsAnyPhrase(text,
                "was it",
                "did you say",
                "you mean",
                "are you offering",
                "is your offer",
                "was your offer",
                "are we at"))
        {
            return false;
        }

        if (IsCounterProposalQuestion(text))
        {
            return false;
        }

        int referencedPrice = result.hasSellerPrice && result.sellerPrice > 0
            ? result.sellerPrice
            : detectedNumber;
        if (referencedPrice <= 0)
        {
            return false;
        }

        bool currentOfferMention = trade != null && trade.npcOffer > 0 && referencedPrice == trade.npcOffer;
        bool exactQuestionRepeat = Regex.IsMatch(text, @"^\d+$") || Regex.IsMatch(text, @"^\d+\s+right$");
        bool confirmationPhrase =
            ContainsAnyPhrase(text,
                "was it",
                "did you say",
                "you mean",
                "are you offering",
                "is your offer",
                "was your offer",
                "are we at") ||
            Regex.IsMatch(text, @"^\d+\s+right$");

        return currentOfferMention || exactQuestionRepeat || confirmationPhrase;
    }

    private static bool IsNegotiationIntent(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.ACCEPT ||
               intent == NegotiationIntent.REJECT ||
               intent == NegotiationIntent.DISMISS ||
               intent == NegotiationIntent.BARGAIN ||
               intent == NegotiationIntent.PRICE ||
               intent == NegotiationIntent.COUNTER ||
               intent == NegotiationIntent.QUERY_BUYER_BUDGET ||
               intent == NegotiationIntent.QUANTITY_PRICE ||
               intent == NegotiationIntent.QUANTITY_CHANGE ||
               intent == NegotiationIntent.ULTIMATUM ||
               intent == NegotiationIntent.CONTINUE;
    }

    private static bool TryExtractPrice(string text, bool quantityAlreadyMatched, out int price)
    {
        price = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string sanitized = StripCurrencyTokens(text);
        string[] words = sanitized.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (!int.TryParse(words[i], out int candidate))
            {
                continue;
            }

            string next = i + 1 < words.Length ? words[i + 1] : string.Empty;
            if (IsQuantityUnit(next))
            {
                continue;
            }

            price = candidate;
        }

        if (price > 0)
        {
            return true;
        }

        return quantityAlreadyMatched && IsPureNumber(sanitized) && words.Length > 0 && int.TryParse(words[0], out price);
    }

    private static bool TryExtractQuantity(string text, LocalTradeState trade, out int quantityGrams)
    {
        quantityGrams = 0;
        string[] words = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length - 1; i++)
        {
            if (!int.TryParse(words[i], out int amount))
            {
                continue;
            }

            string unit = words[i + 1];
            if (!IsQuantityUnit(unit))
            {
                continue;
            }

            quantityGrams = ConvertToGrams(amount, unit, trade);
            return quantityGrams > 0;
        }

        return false;
    }

    private static int ConvertToGrams(int amount, string unit, LocalTradeState trade)
    {
        switch (unit)
        {
            case "bag":
            case "bags":
                return Mathf.Max(1, amount) * Mathf.Max(1, trade != null && trade.quantityGrams > 0 ? trade.quantityGrams : 280);
            case "g":
            case "gm":
            case "gram":
            case "grams":
                return amount;
            case "kg":
            case "kgs":
            case "kilogram":
            case "kilograms":
                return amount * 1000;
            case "palam":
            case "palams":
                return amount * 35;
            case "seer":
            case "seers":
                return amount * 280;
            case "veesai":
            case "viss":
                return amount * 1400;
            case "manangu":
            case "manangus":
            case "maund":
            case "maunds":
                return amount * 11200;
            case "bahar":
            case "bahars":
            case "candy":
            case "candies":
                return amount * 448000;
            default:
                return 0;
        }
    }

    private static bool IsQuantityUnit(string word)
    {
        switch (word)
        {
            case "g":
            case "gm":
            case "gram":
            case "grams":
            case "kg":
            case "kgs":
            case "kilogram":
            case "kilograms":
            case "palam":
            case "palams":
            case "seer":
            case "seers":
            case "veesai":
            case "viss":
            case "manangu":
            case "manangus":
            case "maund":
            case "maunds":
            case "bahar":
            case "bahars":
            case "candy":
            case "candies":
            case "bag":
            case "bags":
                return true;
            default:
                return false;
        }
    }

    private static bool IsPureNumber(string text)
    {
        string value = StripCurrencyTokens(text);
        foreach (char character in value)
        {
            if (!char.IsDigit(character) && !char.IsWhiteSpace(character))
            {
                return false;
            }
        }

        return HasDigits(value);
    }

    private static bool IsAcceptance(string text, bool hasActiveOffer)
    {
        if (!hasActiveOffer)
        {
            return false;
        }

        if (HasAcceptBlockers(text))
        {
            return false;
        }

        return ContainsAnyPhrase(text, "i accept", "that works", "sounds good", "take it", "it is a deal") ||
               ContainsAnyToken(text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries), "deal", "done", "agreed", "accept", "confirmed", "yes", "sure", "okay", "fine");
    }

    private static bool IsRejection(string text, bool hasActiveOffer)
    {
        return ContainsAnyPhrase(text, "walk away", "not interested", "no deal", "never mind") ||
               (hasActiveOffer && ContainsAnyToken(text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries), "reject", "leave", "no"));
    }

    private static bool TryParseNumberToken(string token, out int number)
    {
        if (int.TryParse(token, out number))
        {
            return true;
        }

        switch (token)
        {
            case "zero": number = 0; return true;
            case "one": number = 1; return true;
            case "two": number = 2; return true;
            case "three": number = 3; return true;
            case "four": number = 4; return true;
            case "five": number = 5; return true;
            case "six": number = 6; return true;
            case "seven": number = 7; return true;
            case "eight": number = 8; return true;
            case "nine": number = 9; return true;
            case "ten": number = 10; return true;
            case "eleven": number = 11; return true;
            case "twelve": number = 12; return true;
            case "thirteen": number = 13; return true;
            case "fourteen": number = 14; return true;
            case "fifteen": number = 15; return true;
            case "sixteen": number = 16; return true;
            case "seventeen": number = 17; return true;
            case "eighteen": number = 18; return true;
            case "nineteen": number = 19; return true;
            case "twenty": number = 20; return true;
            case "thirty": number = 30; return true;
            case "forty":
            case "fourty":
                number = 40;
                return true;
            case "fifty": number = 50; return true;
            case "sixty": number = 60; return true;
            case "seventy": number = 70; return true;
            case "eighty": number = 80; return true;
            case "ninety": number = 90; return true;
            default:
                number = -1;
                return false;
        }
    }

    private static bool TryParseCompoundNumber(string firstToken, string secondToken, out int number)
    {
        number = -1;
        if (!TryParseNumberToken(firstToken, out int first) || !TryParseNumberToken(secondToken, out int second))
        {
            return false;
        }

        if (first >= 20 && first % 10 == 0 && second > 0 && second < 10)
        {
            number = first + second;
            return true;
        }

        return false;
    }

    private static bool TryExtractExplicitAcceptancePrice(string text, out int acceptedPrice)
    {
        acceptedPrice = -1;
        MatchCollection matches = Regex.Matches(
            text,
            @"(?:\b(?:i accept|accept|accepted|i agree|agree|agreed)\b(?:\s+your)?(?:\s+offer)?(?:\s+(?:at|to|for))?\s+(\d+)\b)|(?:\b(\d+)\s+(?:then|it is|agreed|works|is\s+(?:fine|okay|ok|good|acceptable)|sounds\s+(?:good|fine|reasonable)|seems\s+fair|will\s+do)\b)");

        for (int i = matches.Count - 1; i >= 0; i--)
        {
            Match match = matches[i];
            for (int groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++)
            {
                if (int.TryParse(match.Groups[groupIndex].Value, out acceptedPrice) && acceptedPrice > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string BuildSemanticRawText(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return string.Empty;
        }

        string cleaned = Regex.Replace(rawText.ToLowerInvariant(), @"[^\w\s]", " ");
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
        return cleaned;
    }

    private static bool MatchesRawQuestionedPrice(string rawText)
    {
        return ContainsAnyPhrase(rawText,
            "can you do",
            "could you do",
            "would you do",
            "can we make it",
            "could we settle at",
            "can you pay",
            "are you offering",
            "are we at",
            "did you say",
            "is it",
            "was it",
            "you mean",
            "why only");
    }

    private static bool MatchesExplicitPriceRejectionOrCriticism(string rawText, string text)
    {
        return ContainsAnyPhrase(text,
                   "does not work",
                   "is a bad price",
                   "is bad price",
                   "is impossible",
                   "is not fine",
                   "is terrible",
                   "is too much",
                   "is unacceptable",
                   "is unfair",
                   "needs improvement",
                   "will not do",
                   "i cannot accept",
                   "i refuse",
                   "i will not take",
                   "i not take",
                   "anything but",
                   "definitely not",
                   "do not settle at") ||
               ContainsAnyPhrase(rawText,
                   "does not work",
                   "is a bad price",
                   "is impossible",
                   "is not fine",
                   "is terrible",
                   "is too much",
                   "is unacceptable",
                   "is unfair",
                   "needs improvement",
                   "will not do",
                   "i cannot accept",
                   "i refuse",
                   "i will not take",
                   "anything but",
                   "definitely not",
                   "do not settle at");
    }

    private static bool MatchesHistoricalPriceReference(string rawText, string text)
    {
        return ContainsAnyPhrase(text,
                   "you said",
                   "you offered",
                   "earlier",
                   "before",
                   "first you offered",
                   "first offered",
                   "then you said",
                   "previously offered",
                   "last offer was",
                   "started at",
                   "mentioned earlier",
                   "i remember",
                   "we discussed",
                   "mentioned",
                   "previous offer was",
                   "your previous offer was") ||
               ContainsAnyPhrase(rawText,
                   "you said",
                   "you offered",
                   "first you offered",
                   "first offered",
                   "last offer was",
                   "i remember",
                   "we discussed",
                   "you mentioned",
                   "previous offer was",
                   "your previous offer was");
    }

    private static bool MatchesItemQuery(string rawText, string text)
    {
        return ContainsAnyPhrase(text,
                   "what item",
                   "which item",
                   "which spice",
                   "what spice",
                   "what are you buying",
                   "what do you want",
                   "what do want",
                   "what are you looking for",
                   "what are looking for",
                   "what would you like",
                   "what would like",
                   "what do you need",
                   "what do need",
                   "what item are you after",
                   "do want cloves",
                   "do want pepper",
                   "are buying spices",
                   "which goods",
                   "which product",
                   "what should i prepare",
                   "tell item",
                   "cloves or pepper") ||
               ContainsAnyPhrase(rawText,
                   "which item",
                   "which spice",
                   "what spice do you want",
                   "what do you want",
                   "what are you buying",
                   "what are you looking for",
                   "what do you need",
                   "what item are you after",
                   "do you want cloves",
                   "do you want pepper",
                   "are you buying spices",
                   "which goods",
                   "which product",
                   "what should i prepare",
                   "tell me the item",
                   "cloves or pepper");
    }

    private static bool MatchesPriceQuestion(string rawText, string text)
    {
        return ContainsAnyPhrase(text,
                   "how much",
                   "what price",
                   "what offer",
                   "what is your offer",
                   "what will you offer",
                   "what will you pay",
                   "how many varahas",
                   "your price",
                   "what are you offering",
                   "what is the current offer",
                   "repeat your offer",
                   "how much did you say",
                   "what was your last offer",
                   "is that your final offer",
                   "what is the best you can do",
                   "what is your highest offer",
                   "what did you offer before",
                   "what are we at now",
                   "what amount are you proposing",
                   "how much for the spices",
                   "what is your buying price",
                   "tell me your offer again",
                   "can you pay") ||
               ContainsAnyPhrase(rawText,
                   "how much",
                   "what price",
                   "what offer",
                   "what is your offer",
                   "what will you offer",
                   "what will you pay",
                   "how many varahas",
                   "your price",
                   "what are you offering",
                   "what is the current offer",
                   "repeat your offer",
                   "how much did you say",
                   "what was your last offer",
                   "is that your final offer",
                   "what is the best you can do",
                   "what is your highest offer",
                   "what did you offer before",
                   "what are we at now",
                   "what amount are you proposing",
                   "how much for the spices",
                   "what is your buying price",
                   "tell me your offer again",
                   "can you pay");
    }

    private static bool MatchesQuantityQuestion(string rawText, string text)
    {
        bool priceLikeAmount = ContainsAnyPhrase(text, "how many varahas", "what amount are you proposing") ||
                               ContainsAnyPhrase(rawText, "how many varahas", "what amount are you proposing");
        if (priceLikeAmount)
        {
            return false;
        }

        return ContainsAnyPhrase(text, "how many", "what quantity", "how much quantity", "what amount of", "how many sacks", "how many units") ||
               ContainsAnyPhrase(rawText, "how many", "what quantity", "how much quantity", "what amount of", "how many sacks", "how many units");
    }

    private static bool MatchesPricePressureQuestion(string rawText, string text)
    {
        return ContainsAnyPhrase(text,
                   "can you improve your offer",
                   "why only",
                   "why is your offer so low",
                   "how did you decide that price",
                   "are you firm on that",
                   "is that all",
                   "can you offer more") ||
               ContainsAnyPhrase(rawText,
                   "can you improve your offer",
                   "why only",
                   "why is your offer so low",
                   "how did you decide that price",
                   "are you firm on that",
                   "is that all",
                   "can you offer more");
    }

    private static void AddReferencedPricesExcluding(string text, NegotiationInput result, params int[] excludedValues)
    {
        if (result == null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        HashSet<int> excluded = new HashSet<int>();
        if (excludedValues != null)
        {
            for (int i = 0; i < excludedValues.Length; i++)
            {
                if (excludedValues[i] > 0)
                {
                    excluded.Add(excludedValues[i]);
                }
            }
        }

        MatchCollection matches = Regex.Matches(text, @"\b\d+\b");
        for (int i = 0; i < matches.Count; i++)
        {
            if (!int.TryParse(matches[i].Value, out int referencedPrice) ||
                referencedPrice <= 0 ||
                excluded.Contains(referencedPrice) ||
                result.referencedPrices.Contains(referencedPrice))
            {
                continue;
            }

            result.referencedPrices.Add(referencedPrice);
        }
    }

    private static bool TryResolveRepeatedNegatedPrices(string text, out List<int> negatedReferences, out int survivingPrice)
    {
        negatedReferences = new List<int>();
        survivingPrice = -1;

        MatchCollection negatedMatches = Regex.Matches(text, @"\b(?:not|nor)\s+(\d+)\b");
        if (negatedMatches.Count < 2)
        {
            return false;
        }

        for (int i = 0; i < negatedMatches.Count; i++)
        {
            if (int.TryParse(negatedMatches[i].Groups[1].Value, out int negatedValue) && !negatedReferences.Contains(negatedValue))
            {
                negatedReferences.Add(negatedValue);
            }
        }

        Match finalBareMatch = Regex.Match(text, @"\b(?:not\s+\d+\b(?:\s*(?:,|or|and))?\s*){2,}(\d+)\b$");
        if (!finalBareMatch.Success)
        {
            finalBareMatch = Regex.Match(text, @"\bneither\s+\d+\s+nor\s+\d+\b.*?\b(\d+)\b$");
        }

        if (!finalBareMatch.Success || !int.TryParse(finalBareMatch.Groups[1].Value, out survivingPrice))
        {
            survivingPrice = -1;
            return false;
        }

        return survivingPrice > 0 && !negatedReferences.Contains(survivingPrice);
    }

    private static bool HasAcceptBlockers(string text)
    {
        return ContainsQuestionSignal(text) ||
               ContainsAny(text, HostileWords) ||
               text.Contains("?");
    }

    private static bool ContainsNegatedAcceptanceCue(string text)
    {
        return ContainsAnyPhrase(text,
                   "do not agree",
                   "do not accept",
                   "cannot agree",
                   "can t agree",
                   "cant agree",
                   "not fair enough",
                   "will not work",
                   "will not take it",
                   "cannot accept",
                   "does not work",
                   "not fine",
                   "not okay",
                   "not good",
                   "not acceptable") ||
               Regex.IsMatch(text, @"\bnot\s+\d+\b");
    }

    private static bool IsCounterProposalQuestion(string text)
    {
        return ContainsAnyPhrase(text,
            "what about",
            "how about",
            "can you do",
            "would you do",
            "could we make it",
            "why not");
    }

    private static bool ContainsQuestionSignal(string text)
    {
        return ContainsAnyPhrase(text,
            "what do you want",
            "how much",
            "what price",
            "what quantity",
            "what spice",
            "what are you buying",
            "which spice",
            "what item",
            "what will you give",
            "what can you offer",
            "how much will you pay",
            "how much will you give");
    }

    private static string DetectSocialSubIntent(string text)
    {
        if (ContainsAnyPhrase(text, "hello", "hi", "hey", "greetings", "good morning", "good day"))
        {
            return "GREETING";
        }
        if (ContainsAnyPhrase(text, "weather", "rain", "sun", "wind"))
        {
            return "WEATHER";
        }
        if (ContainsAnyPhrase(text, "what", "huh", "what do you mean", "do not understand"))
        {
            return "CONFUSION";
        }
        if (ContainsAnyPhrase(text, "where", "from", "name", "king", "temple"))
        {
            return "GENERAL";
        }
        return "GENERAL";
    }

    private static bool ContainsAny(string text, string[] values)
    {
        foreach (string value in values)
        {
            if (ContainsPhrase(text, value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsAnyPhrase(string text, params string[] phrases)
    {
        foreach (string phrase in phrases)
        {
            if (ContainsPhrase(text, phrase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsAnyToken(string[] tokens, params string[] terms)
    {
        foreach (string token in tokens)
        {
            foreach (string term in terms)
            {
                if (token == term)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool ContainsAll(string text, string first, string second)
    {
        return ContainsPhrase(text, first) && ContainsPhrase(text, second);
    }

    private static bool ContainsPhrase(string text, string phrase)
    {
        string paddedText = $" {text} ";
        string paddedPhrase = $" {phrase} ";
        return paddedText.Contains(paddedPhrase);
    }

    private static bool MatchesAnyPattern(string text, params string[] patterns)
    {
        foreach (string pattern in patterns)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(text, pattern))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasDigits(string text)
    {
        foreach (char character in text)
        {
            if (char.IsDigit(character))
            {
                return true;
            }
        }

        return false;
    }

    private static string StripCurrencyTokens(string text)
    {
        return text
            .Replace("$", " ")
            .Replace("varahas", " ")
            .Replace("varaha", " ")
            .Replace("rupees", " ")
            .Replace("rupee", " ")
            .Replace("rs", " ")
            .Replace("coins", " ")
            .Replace("coin", " ")
            .Replace("gold", " ")
            .Replace("dollars", " ")
            .Replace("dollar", " ");
    }
}
