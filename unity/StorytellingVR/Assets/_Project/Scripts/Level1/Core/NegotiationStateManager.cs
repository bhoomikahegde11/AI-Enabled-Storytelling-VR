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
        result.expectedReplyState = CurrentExpectedReplyState;

        void SetMeta(ParseReason parseReason, ParseConfidence confidence, bool needsClarification = false)
        {
            result.parseReason = parseReason;
            result.parseConfidence = confidence;
            result.needsClarification = needsClarification;
        }

        NegotiationInput Finish(string reason, int detectedNumber = -1)
        {
            ApplyStateSafetyGates(result, hasActiveOffer, ref reason);
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
        string text = InputNormalizer.Normalize(playerText, hasActiveOffer);
        string[] tokens = InputNormalizer.Tokenize(playerText, hasActiveOffer);
        LastNormalizedInput = text;
        result.normalizedText = text;
        PopulateSemanticFlags(result, text, tokens);

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

        if (IsPureAcceptance(text, tokens) && hasActiveOffer && !HasCounterPriceAttachedToAcceptance(text, tokens))
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

        if (TryResolveContextualAcceptance(text, tokens, trade, hasTradeNumber, detectedNumber, result, out string acceptanceReason))
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

    private static void PopulateSemanticFlags(NegotiationInput result, string text, string[] tokens)
    {
        if (result == null)
        {
            return;
        }

        result.asksItem = ContainsAnyPhrase(text,
            "what item",
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
            "tell me what you need",
            "tell me what need",
            "are you here to buy something",
            "you want");
        result.asksCurrentOffer = ContainsAnyPhrase(text,
            "how much will you pay",
            "what can you offer",
            "your offer",
            "your budget",
            "your price",
            "what will you give",
            "what price",
            "what cost",
            "what are you offering");
        result.asksQuantity = ContainsAnyPhrase(text, "how many", "what quantity", "what amount", "how much quantity");
        result.asksReason = ContainsAnyPhrase(text, "why so low", "why low", "why is your offer low", "explain why", "why that price");
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

        if (TryParseHistoricalReferenceOnly(text, result, out reason))
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

        if (TryParseStructuredBargain(text, result, out reason))
        {
            return true;
        }

        return false;
    }

    private bool TryParseHistoricalReferenceOnly(string text, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        MatchCollection matches = Regex.Matches(text, @"\b\d+\b");
        if (matches.Count == 0)
        {
            return false;
        }

        bool referencesHistory = ContainsAnyPhrase(text, "you said", "you offered", "earlier", "before", "first you offered", "then you said");
        bool hasActionCue = ContainsAnyPhrase(text, "i want", "give me", "make it", "accept", "deal", "fine at", "can you do", "what about", "come down to", "leave it at", "keep it at", "hold it at", "settle it at");
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

    private bool TryResolveContextualAcceptance(string text, string[] tokens, LocalTradeState trade, bool hasTradeNumber, int detectedNumber, NegotiationInput result, out string reason)
    {
        reason = "contextual acceptance not resolved";
        int currentNpcOffer = trade != null ? trade.npcOffer : -1;
        int tradeNumberCount = CountTradeNumbers(tokens);
        bool affirmative =
            ContainsAnyToken(tokens, "yes", "yeah", "okay", "ok", "fine", "deal", "agreed", "accepted", "accept") ||
            ContainsAnyPhrase(text, "sounds good", "seems fair", "is fine", "is okay", "is good", "will do", "is acceptable", "works for me", "that works", "i will take it") ||
            MatchesAnyPattern(text,
                @"^\d+\s+works$",
                @"^\d+\s+is\s+okay$",
                @"^\d+\s+is\s+fine$",
                @"^\d+\s+is\s+good$",
                @"^\d+\s+sounds\s+good$",
                @"^\d+\s+seems\s+fair$",
                @"^\d+\s+will\s+do$",
                @"^\d+\s+is\s+acceptable$");
        bool rejectionCue =
            ContainsAnyPhrase(text, "too low", "too high", "not interested", "no deal", "reject", "not enough") ||
            Regex.IsMatch(text, @"\bnot\s+" + currentNpcOffer + @"\b");
        bool historicalCue =
            ContainsAnyPhrase(text, "you said", "you offered", "earlier", "before", "previously", "first", "then");
        bool contradictoryCounterCue =
            ContainsAnyPhrase(text,
                "make it",
                "give me",
                "i want",
                "want",
                "only at",
                "what about",
                "can you do",
                "leave it at",
                "leave price at",
                "leave offer at",
                "keep it at",
                "hold it at",
                "settle at",
                "come down to") ||
            Regex.IsMatch(text, @"\byes\b.*\bif\b");
        bool questionCue = ContainsQuestionSignal(text);
        bool priceMatches = hasTradeNumber && detectedNumber > 0 && currentNpcOffer > 0 && detectedNumber == currentNpcOffer;
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
                        !contradictoryCounterCue &&
                        !questionCue &&
                        ((affirmative && (priceMatches || !hasTradeNumber)) || bareExactCurrentOffer);

        Level1DebugForceAccept.LogParser("[ACCEPTANCE RESOLUTION] affirmative=" + affirmative +
                                         " | referencedPrice=" + (hasTradeNumber ? detectedNumber : -1) +
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
        result.evidence = bareExactCurrentOffer ? "bare exact current npc offer accepted" : "affirmative acceptance of current npc offer";
        reason = bareExactCurrentOffer ? "contextual bare exact-price acceptance of current npc offer" : "contextual acceptance of current npc offer";
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

        Match acceptAtPriceMatch = Regex.Match(text, @"\b(?:i accept|accept|accepted)\b.*?\b(?:at|for)\s+(\d+)\b");
        if (acceptAtPriceMatch.Success && int.TryParse(acceptAtPriceMatch.Groups[1].Value, out int acceptedPrice))
        {
            result.intent = NegotiationIntent.ACCEPT;
            result.hasExplicitAcceptance = true;
            result.hasSellerPrice = true;
            result.sellerPrice = acceptedPrice;
            result.acceptanceTarget = acceptedPrice;
            result.evidence = "explicit acceptance tied to price";
            reason = "structured acceptance at specific price";
            return true;
        }

        if ((ContainsAnyPhrase(text, "i accept", "accept your offer", "i accept your offer") || Regex.IsMatch(text, @"\baccept\b.*\b\d+\b")) &&
            TryParseTradeNumber(text, text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries), out int referencedOffer, out _))
        {
            result.intent = NegotiationIntent.ACCEPT;
            result.hasExplicitAcceptance = true;
            result.acceptanceTarget = referencedOffer;
            result.referencedPrices.Add(referencedOffer);
            result.evidence = "acceptance references earlier npc offer";
            reason = "structured acceptance referencing earlier offer";
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

        if (IsQuantityContext(text, tokens, trade))
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

        Match correctionMatch = Regex.Match(text, @"\bnot\s+(\d+)\b.*?\b(?:i said|instead|but|make it|give me)\s+(\d+)\b");
        if (correctionMatch.Success)
        {
            result.correctionDetected = true;
            if (int.TryParse(correctionMatch.Groups[1].Value, out int rejectedPrice))
            {
                result.rejectedPrice = rejectedPrice;
                result.rejectsCurrentOffer = trade != null && trade.npcOffer == rejectedPrice;
                if (!result.referencedPrices.Contains(rejectedPrice))
                {
                    result.referencedPrices.Add(rejectedPrice);
                }
            }
            if (int.TryParse(correctionMatch.Groups[2].Value, out int correctedPrice))
            {
                chosenPrice = correctedPrice;
                cueReason = "correction pattern not X -> Y";
            }
        }

        if (actionablePrices.Count > 1)
        {
            int distinctCount = 0;
            int lastValue = int.MinValue;
            for (int i = 0; i < actionablePrices.Count; i++)
            {
                StructuredPriceCandidate candidate = actionablePrices[i];
                if (candidate.value != lastValue)
                {
                    distinctCount++;
                    lastValue = candidate.value;
                }
            }

            if (distinctCount > 1 && bestScore < 120)
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
            "final price",
            "take it or leave it",
            "let us stop arguing",
            "and take it",
            "that is my final price");
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
        public bool rejectionCue;
        public bool historicalCue;
        public bool correctionCue;
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

            string beforeWindow = JoinTokenWindow(tokens, Mathf.Max(0, i - 4), i - 1);
            string afterWindow = JoinTokenWindow(tokens, i + 1, Mathf.Min(tokens.Length - 1, i + 3));
            int clauseIndex = CountClausesBeforeToken(tokens, i);

            bool hasActionCue =
                WindowEndsWith(beforeWindow, "make it") ||
                WindowEndsWith(beforeWindow, "make") ||
                WindowEndsWith(beforeWindow, "give me") ||
                WindowEndsWith(beforeWindow, "give") ||
                WindowEndsWith(beforeWindow, "i want") ||
                WindowEndsWith(beforeWindow, "want") ||
                WindowEndsWith(beforeWindow, "my price is") ||
                WindowEndsWith(beforeWindow, "price is") ||
                WindowEndsWith(beforeWindow, "settle at") ||
                WindowEndsWith(beforeWindow, "settle it at") ||
                WindowEndsWith(beforeWindow, "i can do") ||
                WindowEndsWith(beforeWindow, "can do") ||
                WindowEndsWith(beforeWindow, "i can accept at") ||
                WindowEndsWith(beforeWindow, "accept at") ||
                WindowEndsWith(beforeWindow, "come down to") ||
                WindowEndsWith(beforeWindow, "raise it to") ||
                WindowEndsWith(beforeWindow, "offer me") ||
                WindowEndsWith(beforeWindow, "i said") ||
                WindowEndsWith(beforeWindow, "said") ||
                WindowEndsWith(beforeWindow, "leave it at") ||
                WindowEndsWith(beforeWindow, "leave price at") ||
                WindowEndsWith(beforeWindow, "leave offer at") ||
                WindowEndsWith(beforeWindow, "leave my price at") ||
                WindowEndsWith(beforeWindow, "keep it at") ||
                WindowEndsWith(beforeWindow, "keep price at") ||
                WindowEndsWith(beforeWindow, "hold it at") ||
                WindowEndsWith(beforeWindow, "hold price at");

            bool hasRejectionCue =
                WindowEndsWith(beforeWindow, "no deal at") ||
                WindowEndsWith(beforeWindow, "not") ||
                WindowEndsWith(beforeWindow, "too low") ||
                WindowEndsWith(beforeWindow, "too high") ||
                WindowEndsWith(beforeWindow, "cannot do") ||
                WindowEndsWith(beforeWindow, "can t do") ||
                WindowEndsWith(beforeWindow, "cant do") ||
                WindowContains(afterWindow, "too low") ||
                WindowContains(afterWindow, "too high");

            bool hasHistoricalCue =
                WindowContains(beforeWindow, "you offered") ||
                WindowContains(beforeWindow, "you said") ||
                WindowContains(beforeWindow, "earlier") ||
                WindowContains(beforeWindow, "previously") ||
                WindowContains(beforeWindow, "before") ||
                WindowContains(beforeWindow, "first") ||
                WindowContains(beforeWindow, "then said") ||
                WindowContains(afterWindow, "earlier");

            bool hasCorrectionCue =
                WindowEndsWith(beforeWindow, "make it") ||
                WindowEndsWith(beforeWindow, "give me") ||
                WindowEndsWith(beforeWindow, "give") ||
                WindowEndsWith(beforeWindow, "i said") ||
                WindowEndsWith(beforeWindow, "said") ||
                WindowEndsWith(beforeWindow, "not");

            int score = 0;
            if (hasActionCue)
            {
                score += 140;
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
            score += clauseIndex * 8;

            StructuredPriceCandidate candidate = new StructuredPriceCandidate
            {
                value = value,
                clauseIndex = clauseIndex,
                tokenPosition = i,
                actionCue = hasActionCue,
                rejectionCue = hasRejectionCue,
                historicalCue = hasHistoricalCue,
                correctionCue = hasCorrectionCue,
                score = score,
                cue = "candidate score=" + score + " | actionCue=" + hasActionCue + " | rejectionCue=" + hasRejectionCue + " | historicalCue=" + hasHistoricalCue + " | correctionCue=" + hasCorrectionCue
            };
            candidates.Add(candidate);
            Level1DebugForceAccept.LogParser("[PRICE CANDIDATE] value=" + candidate.value +
                                             " | position=" + candidate.tokenPosition +
                                             " | clauseIndex=" + candidate.clauseIndex +
                                             " | actionCue=" + candidate.actionCue +
                                             " | rejectionCue=" + candidate.rejectionCue +
                                             " | historicalCue=" + candidate.historicalCue +
                                             " | correctionCue=" + candidate.correctionCue +
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

    private bool TryParseStructuredBargain(string text, NegotiationInput result, out string reason)
    {
        reason = string.Empty;
        if (ContainsAnyPhrase(text,
            "i am not saying no",
            "im not saying no",
            "i cannot accept that",
            "that will not do",
            "perhaps we can meet somewhere between",
            "you must improve the price",
            "improve the offer",
            "raise it",
            "raise your offer"))
        {
            result.intent = NegotiationIntent.BARGAIN;
            result.rejectsCurrentOffer = true;
            result.evidence = "soft bargaining language without final numeric ask";
            reason = "structured bargain language";
            return true;
        }

        return false;
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
                "get lost"))
        {
            return true;
        }

        if (tokens.Length == 1 && (tokens[0] == "leave" || tokens[0] == "stop"))
        {
            return true;
        }

        if (ContainsAnyToken(tokens, "leave") && !ContainsAnyPhrase(text, "leave it at", "leave the price at", "leave it", "leave price"))
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
                "no deal",
                "forget it",
                "cancel the trade",
                "cancel trade",
                "i do not want to sell",
                "i don't want to sell",
                "i dont want to sell",
                "i am not selling",
                "no sale",
                "we are done",
                "end this",
                "i changed my mind",
                "not interested",
                "i do not want to sell this anymore",
                "i don't want to sell this anymore",
                "i dont want to sell this anymore"))
        {
            return true;
        }

        return tokens.Length >= 2 &&
               ContainsAnyToken(tokens, "done") &&
               ContainsAnyToken(tokens, "stop", "okay", "ok", "we", "are");
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

    private bool IsPureAcceptance(string text, string[] tokens)
    {
        if (HasCounterPriceAttachedToAcceptance(text, tokens) || ContainsQuestionSignal(text))
        {
            return false;
        }

        if (ContainsAnyPhrase(text, "i accept", "sounds good", "that works"))
        {
            return true;
        }

        if (ContainsAnyPhrase(text, "sounds fine", "that deal sounds fine", "not bad deal"))
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

        if (ContainsAnyPhrase(text, "i want", "give me", "need", "take") &&
            !ContainsAnyToken(tokens, "varaha", "varahas", "price", "offer", "pay"))
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

    private static bool HasAcceptBlockers(string text)
    {
        return ContainsQuestionSignal(text) ||
               ContainsAny(text, HostileWords) ||
               text.Contains("?");
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
