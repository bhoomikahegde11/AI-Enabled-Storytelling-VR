using System.Text;
using UnityEngine;

public static class InputNormalizer
{
    private static readonly string[] Empty = new string[0];
    private static readonly string[] ContextWords =
    {
        "varaha", "varahas", "price", "prices", "offer", "offers", "pay", "paying", "sell", "selling", "buy", "buying",
        "cost", "costs", "palam", "palams", "seer", "seers", "veesai", "viss", "manangu", "maund", "maunds", "bahar", "bahars",
        "candy", "candies", "kg", "kgs", "kilogram", "kilograms", "g", "gm", "gram", "grams", "quantity", "amount", "weight",
        "pepper", "cardamom", "cinnamon", "clove", "turmeric", "deal", "agree", "accept", "take", "give", "want", "budget"
    };
    private static readonly string[] NumberWords =
    {
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
        "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
        "twenty", "thirty", "forty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety", "hundred", "and"
    };
    private static readonly string[] FillerWords =
    {
        "a", "an", "the", "please", "just", "uh", "um", "ah", "hmm", "well", "maybe",
        "merchant", "sir", "kindly", "can", "could", "would", "will", "you", "me", "actually",
        "basically", "like", "brother", "friend"
    };

    public static string Normalize(string input)
    {
        return Normalize(input, false);
    }

    public static string Normalize(string input, bool awaitingPrice)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string text = input.Trim().ToLowerInvariant();
        text = text
            .Replace("?", " ")
            .Replace("best brace", "best price")
            .Replace("base price", "best price")
            .Replace("best prize", "best price")
            .Replace("what is the prize", "what is the price")
            .Replace("what's the price", "what price")
            .Replace("whats the price", "what price")
            .Replace("for palam", "four palam")
            .Replace("for palams", "four palams")
            .Replace("for seer", "four seer")
            .Replace("for seers", "four seers")
            .Replace("for veesai", "four veesai")
            .Replace("for viss", "four viss")
            .Replace("for manangu", "four manangu")
            .Replace("for maund", "four maund")
            .Replace("for maunds", "four maunds")
            .Replace("for bahar", "four bahar")
            .Replace("for bahars", "four bahars")
            .Replace("for candy", "four candy")
            .Replace("for candies", "four candies")
            .Replace("for varaha", "four varaha")
            .Replace("for varahas", "four varahas")
            .Replace("what is your offer", "your offer")
            .Replace("how much will you pay", "your offer")
            .Replace("what will you pay", "your offer")
            .Replace("how much will you give", "your offer")
            .Replace("what can you offer", "your offer")
            .Replace("name your price", "your offer")
            .Replace("what is your best", "best price")
            .Replace("your final", "final price")
            .Replace("take it or leave it", "take it or leave it")
            .Replace("less price", "lower price")
            .Replace("law price", "lower price")
            .Replace("low price", "lower price")
            .Replace("lower the price", "lower price")
            .Replace("reduce the price", "reduce price")
            .Replace("make it cheaper", "cheaper")
            .Replace("little lower", "lower")
            .Replace("to expensive", "too expensive")
            .Replace("two expensive", "too expensive")
            .Replace("little chipper", "little cheaper")
            .Replace("a little cheaper", "little cheaper")
            .Replace("tree hundred", "three hundred")
            .Replace("one for tea", "one forty")
            .Replace("one fivety", "one fifty")
            .Replace("fivety", "fifty")
            .Replace("what are you offering", "your offer")
            .Replace("what do you offer", "your offer")
            .Replace("can you reduce it", "reduce")
            .Replace("can you reduce price", "reduce price")
            .Replace("could you lower price", "lower price")
            .Replace("can you lower price", "lower price")
            .Replace("little cheaper please", "cheaper")
            .Replace("cheap price", "cheaper")
            .Replace("how many do you want", "how many")
            .Replace("what quantity", "quantity")
            .Replace("how much quantity", "quantity")
            .Replace("what spice do you want", "what item")
            .Replace("what are you buying", "what item")
            .Replace("paper", "pepper")
            .Replace("peper", "pepper")
            .Replace("cardamon", "cardamom")
            .Replace("cardamum", "cardamom")
            .Replace("cinamon", "cinnamon")
            .Replace("tumeric", "turmeric")
            .Replace("waraha", "varaha")
            .Replace("warahas", "varahas")
            .Replace("vara ha", "varaha")
            .Replace("baraha", "varaha")
            .Replace("barahas", "varahas")
            .Replace("dollars", "varahas")
            .Replace("dollar", "varaha")
            .Replace("rupees", "varahas")
            .Replace("rupee", "varaha")
            .Replace("coins", "varahas")
            .Replace("coin", "varaha")
            .Replace("gold", "varaha")
            .Replace("$", " varaha ")
            .Replace("rs.", " varaha ")
            .Replace("rs", " varaha ")
            .Replace("my friend", " ")
            .Replace("okay then", "okay")
            .Replace("all right", "okay");

        if (awaitingPrice)
        {
            text = text
                .Replace(" seventeen ", " 70 ")
                .Replace(" seven ", " 70 ")
                .Replace(" eighteen ", " 80 ")
                .Replace(" eight ", " 80 ")
                .Replace(" nineteen ", " 90 ")
                .Replace(" nine ", " 90 ");
        }

        StringBuilder cleaned = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            cleaned.Append(char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) ? c : ' ');
        }

        string[] words = cleaned.ToString().Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
        StringBuilder normalized = new StringBuilder(cleaned.Length);
        for (int i = 0; i < words.Length; i++)
        {
            string rawWord = words[i];
            string word = NormalizeWord(rawWord);
            if (TryNormalizeDigitBridge(words, ref i, awaitingPrice, out string bridgedNumber))
            {
                AppendToken(normalized, bridgedNumber);
                continue;
            }

            if (Contains(FillerWords, word))
            {
                continue;
            }

            if (TryNormalizeNumber(words, ref i, awaitingPrice, out string numberToken))
            {
                AppendToken(normalized, numberToken);
                continue;
            }

            AppendToken(normalized, word);
        }

        return normalized.ToString();
    }

    public static string[] Tokenize(string input)
    {
        return Tokenize(input, false);
    }

    public static string[] Tokenize(string input, bool awaitingPrice)
    {
        string normalized = Normalize(input, awaitingPrice);
        return string.IsNullOrEmpty(normalized)
            ? Empty
            : normalized.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
    }

    private static string NormalizeWord(string word)
    {
        switch (word)
        {
            case "prize":
            case "prise":
                return "price";
            case "costly":
                return "expensive";
            case "decrease":
                return "reduce";
            case "discounted":
                return "discount";
            case "cheep":
            case "chipper":
                return "cheaper";
            case "ok":
            case "okey":
            case "oky":
                return "okay";
            case "qty":
                return "quantity";
            case "tree":
                return "three";
            default:
                return word;
        }
    }

    private static bool TryNormalizeNumber(string[] words, ref int index, bool awaitingPrice, out string normalizedNumber)
    {
        normalizedNumber = null;
        string current = NormalizeWord(words[index]);
        if (!Contains(NumberWords, current))
        {
            return false;
        }

        int maxLookAhead = Mathf.Min(words.Length - 1, index + 3);
        int bestConsumed = 0;
        string bestNumber = null;
        for (int end = maxLookAhead; end >= index; end--)
        {
            if (!IsNumberSequence(words, index, end))
            {
                continue;
            }

            if (!awaitingPrice && !IsNearContext(words, index, end) && words.Length > (end - index + 1))
            {
                continue;
            }

            bestConsumed = end - index + 1;
            bestNumber = ParseNumberSequence(words, index, end);
            break;
        }

        if (bestConsumed <= 0 || string.IsNullOrEmpty(bestNumber))
        {
            return false;
        }

        normalizedNumber = bestNumber;
        index += bestConsumed - 1;
        return true;
    }

    private static bool IsNumberSequence(string[] words, int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            string word = NormalizeWord(words[i]);
            if (word == "and")
            {
                continue;
            }

            if (!Contains(NumberWords, word))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsNearContext(string[] words, int start, int end)
    {
        for (int i = Mathf.Max(0, start - 3); i < start; i++)
        {
            if (Contains(ContextWords, NormalizeWord(words[i])))
            {
                return true;
            }
        }

        for (int i = end + 1; i <= Mathf.Min(words.Length - 1, end + 3); i++)
        {
            if (Contains(ContextWords, NormalizeWord(words[i])))
            {
                return true;
            }
        }

        return false;
    }

    private static string ParseNumberSequence(string[] words, int start, int end)
    {
        bool allDigits = end > start;
        for (int i = start; i <= end; i++)
        {
            string word = NormalizeWord(words[i]);
            if (ParseDigitWord(word) < 0)
            {
                allDigits = false;
                break;
            }
        }

        if (allDigits)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = start; i <= end; i++)
            {
                int digit = ParseDigitWord(NormalizeWord(words[i]));
                if (digit >= 0)
                {
                    builder.Append(digit);
                }
            }
            return builder.ToString();
        }

        int total = 0;
        int current = 0;
        for (int i = start; i <= end; i++)
        {
            string word = NormalizeWord(words[i]);
            if (word == "and")
            {
                continue;
            }

            int unit = ParseNumberToken(word);
            if (word == "hundred")
            {
                current = Mathf.Max(1, current) * 100;
                continue;
            }

            if (unit >= 20 && unit % 10 == 0)
            {
                current += unit;
                continue;
            }

            current += unit;
        }

        total += current;
        if (start + 1 == end)
        {
            string first = NormalizeWord(words[start]);
            string second = NormalizeWord(words[end]);
            if (first == "one" && second == "forty")
            {
                return "140";
            }
            if (ParseDigitWord(first) > 0 && ParseNumberToken(second) >= 10)
            {
                return ((ParseDigitWord(first) * 100) + ParseNumberToken(second)).ToString();
            }
        }

        if (start + 2 == end)
        {
            string first = NormalizeWord(words[start]);
            string second = NormalizeWord(words[start + 1]);
            string third = NormalizeWord(words[end]);
            if (second == "hundred")
            {
                return ((Mathf.Max(1, ParseNumberToken(first)) * 100) + ParseNumberToken(third)).ToString();
            }
            if (ParseDigitWord(first) > 0 && ParseNumberToken(second) >= 20 && ParseDigitWord(third) >= 0)
            {
                return ((ParseDigitWord(first) * 100) + ParseNumberToken(second) + Mathf.Max(0, ParseDigitWord(third))).ToString();
            }
        }

        return total.ToString();
    }

    private static bool TryNormalizeDigitBridge(string[] words, ref int index, bool awaitingPrice, out string normalizedNumber)
    {
        normalizedNumber = null;
        if (!int.TryParse(words[index], out int first))
        {
            return false;
        }

        if (index + 2 < words.Length &&
            NormalizeWord(words[index + 1]) == "and" &&
            int.TryParse(words[index + 2], out int third) &&
            (awaitingPrice || IsNearContext(words, index, index + 2) || words.Length == 3) &&
            first >= 100 && third > 0 && third < 100)
        {
            normalizedNumber = (first + third).ToString();
            index += 2;
            return true;
        }

        return false;
    }

    private static int ParseDigitWord(string word)
    {
        switch (word)
        {
            case "zero": return 0;
            case "one": return 1;
            case "two": return 2;
            case "three": return 3;
            case "four": return 4;
            case "five": return 5;
            case "six": return 6;
            case "seven": return 7;
            case "eight": return 8;
            case "nine": return 9;
            default: return -1;
        }
    }

    private static int ParseNumberToken(string word)
    {
        switch (word)
        {
            case "zero": return 0;
            case "one": return 1;
            case "two": return 2;
            case "three": return 3;
            case "four": return 4;
            case "five": return 5;
            case "six": return 6;
            case "seven": return 7;
            case "eight": return 8;
            case "nine": return 9;
            case "ten": return 10;
            case "eleven": return 11;
            case "twelve": return 12;
            case "thirteen": return 13;
            case "fourteen": return 14;
            case "fifteen": return 15;
            case "sixteen": return 16;
            case "seventeen": return 17;
            case "eighteen": return 18;
            case "nineteen": return 19;
            case "twenty": return 20;
            case "thirty": return 30;
            case "forty":
            case "fourty":
                return 40;
            case "fifty": return 50;
            case "sixty": return 60;
            case "seventy": return 70;
            case "eighty": return 80;
            case "ninety": return 90;
            case "hundred": return 100;
            default: return 0;
        }
    }

    private static void AppendToken(StringBuilder builder, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append(' ');
        }

        builder.Append(token);
    }

    private static bool Contains(string[] values, string candidate)
    {
        foreach (string value in values)
        {
            if (value == candidate)
            {
                return true;
            }
        }

        return false;
    }
}
