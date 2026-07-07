using System;
using UnityEngine;

public enum NegotiationIntent
{
    ACCEPT,
    REJECT,
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
    LlmInterpreted,
    PromptInjection,
    OffTopicModern,
    Hostile,
    PureAcceptance,
    HardRejection,
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
    public bool needsClarification;
    public ParseConfidence parseConfidence = ParseConfidence.Low;
    public ParseReason parseReason = ParseReason.Unknown;
    public ExpectedReplyState expectedReplyState = ExpectedReplyState.None;
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
            Level1DebugForceAccept.LogParser("[LOCAL UNDERSTANDING] raw=" + playerText +
                                             " | normalized=" + result.normalizedText +
                                             " | expectedReplyState=" + CurrentExpectedReplyState +
                                             " | detectedNumber=" + detectedNumber +
                                             " | numberReason=" + numberReason +
                                             " | intent=" + result.intent +
                                             " | parseConfidence=" + result.parseConfidence +
                                             " | parseReason=" + result.parseReason +
                                             " | needsClarification=" + result.needsClarification +
                                             " | sellerPrice=" + result.sellerPrice +
                                             " | quantityGrams=" + result.quantityGrams +
                                             " | currentOffer=" + (trade != null ? trade.npcOffer : 0) +
                                             " | reason=" + reason);
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
            SetMeta(ParseReason.EmptyInput, ParseConfidence.Low, true);
            return Finish("empty player input");
        }

        string itemKey = trade != null ? (trade.spiceKey ?? string.Empty).ToLowerInvariant() : string.Empty;
        string itemDisplay = trade != null ? (trade.spiceDisplayName ?? string.Empty).ToLowerInvariant() : string.Empty;
        string text = InputNormalizer.Normalize(playerText, hasActiveOffer);
        string[] tokens = InputNormalizer.Tokenize(playerText, hasActiveOffer);
        LastNormalizedInput = text;
        result.normalizedText = text;

        bool hasTradeNumber = TryParseTradeNumber(text, tokens, out detectedNumber, out numberReason);

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

        if (IsPureAcceptance(text, tokens) && hasActiveOffer && !HasCounterPriceAttachedToAcceptance(text, tokens))
        {
            result.hasExplicitAcceptance = true;
            result.intent = NegotiationIntent.ACCEPT;
            SetMeta(ParseReason.PureAcceptance, ParseConfidence.High);
            return Finish("pure acceptance without new price");
        }

        if (IsPureRejection(text, tokens) && hasActiveOffer && !HasCounterPriceAttachedToAcceptance(text, tokens))
        {
            result.intent = NegotiationIntent.REJECT;
            result.hasHardRejection = IsHardRejectPhrase(text, tokens);
            SetMeta(result.hasHardRejection ? ParseReason.HardRejection : ParseReason.SoftRejection, result.hasHardRejection ? ParseConfidence.High : ParseConfidence.Medium);
            return Finish("pure rejection without new price");
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

        bool asksItem = ContainsAnyPhrase(text, "what item", "which spice", "what spice", "what are you buying", "what do you want");
        bool asksPrice = ContainsAnyPhrase(text, "how much will you pay", "what can you offer", "your offer", "your budget", "your price", "what will you give", "best price", "maximum you can give", "what is your best", "what price", "what is price", "what cost");

        if (asksItem && asksPrice)
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
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
            SetMeta(ParseReason.TradeVocabularyFallback, ParseConfidence.Low, true);
            return Finish("trade vocabulary without structured parse", detectedNumber);
        }

        if (text.Split(' ').Length <= 3)
        {
            result.intent = hasTradeNumber ? NegotiationIntent.CLARIFICATION : NegotiationIntent.SOCIAL;
            result.socialSubIntent = result.intent == NegotiationIntent.SOCIAL ? DetectSocialSubIntent(text) : string.Empty;
            SetMeta(hasTradeNumber ? ParseReason.ShortNumericUnclear : ParseReason.ShortSocial, ParseConfidence.Low, hasTradeNumber);
            return Finish(hasTradeNumber ? "short numeric reply without clear trade context" : "short social reply", detectedNumber);
        }

        result.intent = NegotiationIntent.UNKNOWN;
        SetMeta(ParseReason.UnknownFallback, ParseConfidence.Low, true);
        return Finish("unknown fallback", detectedNumber);
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
            if (token != "no" && token != "reject" && token != "leave")
            {
                return false;
            }
        }

        return tokens.Length > 0;
    }

    private bool IsHardRejectPhrase(string text, string[] tokens)
    {
        if (ContainsAnyPhrase(text, "walk away", "not interested", "no deal", "leave it", "leave this", "never mind"))
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

        if (IsInfoQueryIntent(result.intent))
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
        reason = debugReason;
    }

    private static bool IsInfoQueryIntent(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.ITEM_QUERY ||
               intent == NegotiationIntent.PRICE_QUERY ||
               intent == NegotiationIntent.QUANTITY_QUERY ||
               intent == NegotiationIntent.QUERY_BUYER_BUDGET;
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
               intent == NegotiationIntent.BARGAIN ||
               intent == NegotiationIntent.CLARIFICATION ||
               intent == NegotiationIntent.CONFUSED;
    }

    private static bool IsNegotiationIntent(NegotiationIntent intent)
    {
        return intent == NegotiationIntent.ACCEPT ||
               intent == NegotiationIntent.REJECT ||
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
