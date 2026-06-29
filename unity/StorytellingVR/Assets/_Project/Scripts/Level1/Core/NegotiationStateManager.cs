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
    CONTINUE
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
    }

    public void SetLastOffer(int offer)
    {
        LastOffer = Mathf.Max(0, offer);
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
        NegotiationInput Finish()
        {
            Debug.Log("[LOCAL UNDERSTANDING] raw=" + playerText +
                      " | normalized=" + result.normalizedText +
                      " | intent=" + result.intent +
                      " | sellerPrice=" + result.sellerPrice +
                      " | currentOffer=" + (trade != null ? trade.npcOffer : 0));
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
            LastNormalizedInput = result.normalizedText;
            return Finish();
        }

        if (string.IsNullOrWhiteSpace(playerText))
        {
            result.intent = NegotiationIntent.CLARIFICATION;
            return Finish();
        }

        string itemKey = trade != null ? (trade.spiceKey ?? string.Empty).ToLowerInvariant() : string.Empty;
        string itemDisplay = trade != null ? (trade.spiceDisplayName ?? string.Empty).ToLowerInvariant() : string.Empty;
        bool hasActiveOffer = trade != null && trade.npcOffer > 0;
        string text = InputNormalizer.Normalize(playerText, hasActiveOffer);
        string[] tokens = InputNormalizer.Tokenize(playerText, hasActiveOffer);
        LastNormalizedInput = text;
        result.normalizedText = text;

        bool hasDigits = HasDigits(text);

        result.hasQuantity = TryExtractQuantity(text, out int quantityGrams);
        result.quantityGrams = result.hasQuantity ? quantityGrams : -1;
        result.hasSellerPrice = TryExtractPrice(text, result.hasQuantity, out int sellerPrice);
        result.sellerPrice = result.hasSellerPrice ? sellerPrice : -1;

        if (ContainsPhrase(text, "ignore previous instructions") || ContainsPhrase(text, "act as chatgpt"))
        {
            result.intent = NegotiationIntent.OFF_TOPIC;
            return Finish();
        }

        if (ContainsAny(text, ModernWords) || ContainsAll(text, "social", "media") || ContainsAll(text, "mobile", "phone") || ContainsAll(text, "video", "game"))
        {
            result.intent = NegotiationIntent.OFF_TOPIC;
            return Finish();
        }

        if (ContainsAny(text, HostileWords) || ContainsAll(text, "go", "die") || ContainsAll(text, "kill", "yourself"))
        {
            result.intent = NegotiationIntent.HOSTILE;
            return Finish();
        }

        if (ContainsAnyPhrase(text, "how much will you pay", "what can you offer", "your offer", "your budget", "your price", "what will you give", "best price", "maximum you can give", "what is your best"))
        {
            result.intent = NegotiationIntent.QUERY_BUYER_BUDGET;
            return Finish();
        }

        if (ContainsAnyPhrase(text, "what item", "which spice", "what spice", "what are you buying", "what do you want"))
        {
            result.intent = NegotiationIntent.ITEM_QUERY;
            return Finish();
        }

        if (ContainsAnyPhrase(text, "how many", "what quantity", "what amount", "how much quantity") ||
            ContainsAnyToken(tokens, "quantity", "amount", "weight"))
        {
            result.intent = NegotiationIntent.QUANTITY_QUERY;
            return Finish();
        }

        if (ContainsAnyPhrase(text, "how much", "what price", "what is price", "what cost"))
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
            return Finish();
        }

        if (ContainsAnyToken(tokens, "hello", "hi", "hey", "greetings", "namaste") || ContainsAnyPhrase(text, "good day", "good morning"))
        {
            result.intent = NegotiationIntent.GREETING;
            result.socialSubIntent = "GREETING";
            return Finish();
        }

        if (ContainsAnyPhrase(text, "who are you", "where from", "how is weather", "how weather", "who is king", "what is your name"))
        {
            result.intent = NegotiationIntent.GENERAL_DIALOGUE;
            result.socialSubIntent = DetectSocialSubIntent(text);
            return Finish();
        }

        result.hasExplicitUltimatum = ContainsAnyPhrase(text, "take it or leave it", "final price", "this is my final price", "last price", "last offer") ||
            MatchesAnyPattern(text, @"not going lower than\s+\d+", @"nothing less than\s+\d+", @"not less than\s+\d+", @"minimum(?: price)?\s*(?:is)?\s*\d+");

        if (result.hasExplicitUltimatum ||
            MatchesAnyPattern(text, @"not going lower than\s+\d+", @"nothing less than\s+\d+", @"not less than\s+\d+", @"minimum(?: price)?\s*(?:is)?\s*\d+"))
        {
            result.intent = result.hasSellerPrice ? NegotiationIntent.ULTIMATUM : NegotiationIntent.CLARIFICATION;
            return Finish();
        }

        if (IsRejection(text, hasActiveOffer))
        {
            result.intent = NegotiationIntent.REJECT;
            return Finish();
        }

        if (result.hasQuantity && result.hasSellerPrice)
        {
            result.intent = NegotiationIntent.QUANTITY_PRICE;
            return Finish();
        }

        if (result.hasQuantity)
        {
            if (string.IsNullOrEmpty(itemKey) || text.Contains(itemKey) || text.Contains(itemDisplay) || ContainsAnyToken(tokens, "for", "only", "left", "remaining", "take", "give", "want", "need"))
            {
                result.intent = NegotiationIntent.QUANTITY_CHANGE;
                return Finish();
            }
        }

        if (result.hasSellerPrice)
        {
            if (ContainsAnyPhrase(text, "i will give", "i can give", "i give", "i can offer", "i offer", "i will sell for", "i sell", "i can do", "i do", "let us do", "lets do"))
            {
                result.intent = NegotiationIntent.PRICE;
                return Finish();
            }

            if (ContainsAnyToken(tokens, "okay", "ok", "fine", "yes", "sure", "then"))
            {
                result.intent = result.hasExplicitUltimatum ? NegotiationIntent.ULTIMATUM : NegotiationIntent.COUNTER;
                return Finish();
            }

            if (ContainsAnyPhrase(text, "too low", "not enough", "little more", "meet in middle", "meet in the middle", "split") || ContainsAnyToken(tokens, "higher", "increase", "more"))
            {
                result.intent = NegotiationIntent.COUNTER;
                return Finish();
            }

            if (ContainsAnyPhrase(text, "okay deal", "deal at", "okay at", "fine at", "accepted at"))
            {
                result.intent = result.hasExplicitUltimatum ? NegotiationIntent.ULTIMATUM : NegotiationIntent.COUNTER;
                result.hasExplicitAcceptance = true;
                return Finish();
            }

            if (ContainsAnyPhrase(text, "then", "make it", "i want", "give me", "i will sell", "take", "last price", "last offer") ||
                IsPureNumber(text) || ContainsAnyToken(tokens, "price", "offer", "give", "want", "take", "sell", "final", "last", "make"))
            {
                result.intent = NegotiationIntent.PRICE;
                return Finish();
            }
        }

        result.hasExplicitAcceptance = !result.hasSellerPrice && IsAcceptance(text, hasActiveOffer);
        if (result.hasExplicitAcceptance)
        {
            result.intent = NegotiationIntent.ACCEPT;
            return Finish();
        }

        if (ContainsAnyPhrase(text, "too low", "not enough", "meet in the middle", "split") || ContainsAnyToken(tokens, "increase", "higher", "more"))
        {
            result.intent = NegotiationIntent.COUNTER;
            return Finish();
        }

        if (ContainsAnyToken(tokens, "reduce", "lower", "discount", "cheap", "cheaper", "less", "expensive", "steep", "low", "high"))
        {
            result.intent = NegotiationIntent.BARGAIN;
            return Finish();
        }

        if (ContainsAnyToken(tokens, "sure", "okay", "ok", "fine", "alright", "yes") && !ContainsQuestionSignal(text))
        {
            result.intent = hasActiveOffer ? NegotiationIntent.ACCEPT : NegotiationIntent.CONTINUE;
            result.hasExplicitAcceptance = hasActiveOffer;
            return Finish();
        }

        if (ContainsAnyToken(tokens, "weather", "rain", "sun", "origin", "name", "king", "temple"))
        {
            result.intent = NegotiationIntent.GENERAL_DIALOGUE;
            result.socialSubIntent = DetectSocialSubIntent(text);
            return Finish();
        }

        if (ContainsAnyToken(tokens, TradeWords))
        {
            result.intent = NegotiationIntent.PRICE_QUERY;
            return Finish();
        }

        if (hasDigits)
        {
            result.intent = NegotiationIntent.CLARIFICATION;
            return Finish();
        }

        if (text.Split(' ').Length <= 3)
        {
            result.intent = NegotiationIntent.SOCIAL;
            result.socialSubIntent = DetectSocialSubIntent(text);
            return Finish();
        }

        result.intent = NegotiationIntent.UNKNOWN;
        return Finish();
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

        CurrentRound++;
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
            case NegotiationIntent.UNKNOWN:
                ConsecutiveBargains = 0;
                ConsecutiveQueries = 0;
                UnknownCount++;
                BuyerPatience = Mathf.Max(0, BuyerPatience - 1);
                break;
        }

        if (BuyerPatience <= 0 || CurrentRound >= MaxRounds)
        {
            IsNegotiationFinished = true;
        }
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

    private static bool TryExtractQuantity(string text, out int quantityGrams)
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

            quantityGrams = ConvertToGrams(amount, unit);
            return quantityGrams > 0;
        }

        return false;
    }

    private static int ConvertToGrams(int amount, string unit)
    {
        switch (unit)
        {
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
