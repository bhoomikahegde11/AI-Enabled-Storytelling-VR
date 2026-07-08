using System.Collections.Generic;
using UnityEngine;

public class LocalGeneratedTradeSession
{
    public string greetingText;
    public string buyerName;
    public string buyerOrigin;
    public string buyerPersonality;
    public string spiceName;
    public string quantityLabel;
    public int quantityGrams;
    public int startingOffer;
    public int maxAcceptablePrice;
    public int buyerPatience;
    public float buyerTrust;
    public float buyerFrustration;
    public float buyerDesperation;
}

public class LocalTradeSessionGenerator
{
    private static readonly string[] WealthTypes = { "Low", "Medium", "High", "Very High" };
    private static readonly int[] QuantityOptions = { 280, 560, 1400, 2800 };
    private static readonly string[] SpiceKeys = { "pepper", "clove", "cinnamon", "cardamom" };
    private readonly DialogueCharacterRegistry dialogueCharacterRegistry = new DialogueCharacterRegistry();

    public LocalGeneratedTradeSession Generate(MarketManager marketManager, LocalProfileData profile, MarketEventData activeEvent, string forcedCharacterId = "")
    {
        string spiceKey = PickAvailableSpice(profile);
        marketManager.TryGetSpice(spiceKey, out SpiceData spiceData);
        int reputation = Level1GameState.ExistingInstance != null ? Level1GameState.Instance.CurrentReputation : 50;

        int stock = GetInventory(profile, spiceKey);
        int quantity = PickQuantity(stock);
        int marketValue = marketManager.CalculateMarketValue(spiceKey, quantity, activeEvent);

        DialogueCharacterProfile selectedCharacter = !string.IsNullOrWhiteSpace(forcedCharacterId)
            ? dialogueCharacterRegistry.GetRegisteredCharacterOrRandom(forcedCharacterId)
            : GetWeightedCharacterForReputation(reputation);
        if (selectedCharacter == null || !dialogueCharacterRegistry.IsRegisteredCharacterId(selectedCharacter.characterId))
        {
            selectedCharacter = dialogueCharacterRegistry.GetRegisteredCharacterOrRandom(string.Empty);
        }

        string buyerName = selectedCharacter != null ? selectedCharacter.displayName : "Abdul Rahman";
        string buyerOrigin = selectedCharacter != null ? selectedCharacter.buyerOrigin : "Arab Caravan Trader";
        string buyerPersonality = GetEffectivePersonalityForReputation(
            selectedCharacter != null ? selectedCharacter.buyerPersonality : "Friendly",
            reputation);
        string wealthType = GetWealthTypeForReputation(reputation);
        float startMultiplier = GetStartMultiplier(buyerPersonality, wealthType);
        float maxMultiplier = GetMaxMultiplier(buyerPersonality, wealthType);
        int startingOffer = Mathf.Max(1, Mathf.RoundToInt(marketValue * startMultiplier));
        int maxAcceptablePrice = Mathf.Max(startingOffer, Mathf.RoundToInt(marketValue * maxMultiplier));
        float trust = GetStartingTrust(buyerPersonality) + GetTrustModifierForReputation(reputation);
        float frustration = GetStartingFrustration(buyerPersonality) + GetFrustrationModifierForReputation(reputation);
        float desperation = GetDesperation(buyerPersonality) + GetDesperationModifierForReputation(reputation);

        Level1DebugForceAccept.LogTrade("[CUSTOMER] character=" + (selectedCharacter != null ? selectedCharacter.characterId : "abdul_rahman") +
                                        " | buyer=" + buyerName +
                                        " | personality=" + buyerPersonality +
                                        " | wealth=" + wealthType +
                                        " | marketValue=" + marketValue +
                                        " | startingOffer=" + startingOffer +
                                        " | buyerMaxOffer=" + maxAcceptablePrice);

        return new LocalGeneratedTradeSession
        {
            buyerName = buyerName,
            buyerOrigin = buyerOrigin,
            buyerPersonality = buyerPersonality,
            spiceName = spiceData != null ? spiceData.displayName : "Spice",
            quantityLabel = marketManager.FormatTraditionalQuantity(quantity),
            quantityGrams = quantity,
            startingOffer = startingOffer,
            maxAcceptablePrice = maxAcceptablePrice,
            buyerPatience = GetBuyerPatience(buyerPersonality),
            buyerTrust = Mathf.Clamp01(trust),
            buyerFrustration = Mathf.Clamp01(frustration),
            buyerDesperation = Mathf.Clamp01(desperation),
            greetingText = BuildGreeting(
                buyerName,
                buyerPersonality,
                marketManager.FormatTraditionalQuantity(quantity),
                spiceData != null ? spiceData.displayName : "spice")
        };
    }

    private DialogueCharacterProfile GetWeightedCharacterForReputation(int reputation)
    {
        IReadOnlyList<DialogueCharacterProfile> supportedProfiles = dialogueCharacterRegistry.GetSupportedCharacterProfiles();
        if (supportedProfiles == null || supportedProfiles.Count == 0)
        {
            return dialogueCharacterRegistry.GetRandomRegisteredCharacter();
        }

        float totalWeight = 0f;
        for (int i = 0; i < supportedProfiles.Count; i++)
        {
            totalWeight += GetCharacterWeightForReputation(supportedProfiles[i], reputation);
        }

        if (totalWeight <= 0f)
        {
            return dialogueCharacterRegistry.GetRandomRegisteredCharacter();
        }

        float roll = Random.value * totalWeight;
        for (int i = 0; i < supportedProfiles.Count; i++)
        {
            DialogueCharacterProfile candidate = supportedProfiles[i];
            roll -= GetCharacterWeightForReputation(candidate, reputation);
            if (roll <= 0f)
            {
                return candidate;
            }
        }

        return supportedProfiles[supportedProfiles.Count - 1];
    }

    private static float GetCharacterWeightForReputation(DialogueCharacterProfile profile, int reputation)
    {
        string personality = profile != null ? profile.buyerPersonality : "Normal";

        if (reputation < 35)
        {
            switch (personality)
            {
                case "Strict":
                    return 2.4f;
                case "Normal":
                    return 1.4f;
                case "Friendly":
                    return 0.8f;
            }
        }
        else if (reputation < 70)
        {
            switch (personality)
            {
                case "Strict":
                    return 1.2f;
                case "Normal":
                    return 1.1f;
                case "Friendly":
                    return 1.2f;
            }
        }
        else
        {
            switch (personality)
            {
                case "Strict":
                    return 0.85f;
                case "Normal":
                    return 1.15f;
                case "Friendly":
                    return 2.1f;
            }
        }

        return 1f;
    }

    private static string GetEffectivePersonalityForReputation(string basePersonality, int reputation)
    {
        if (reputation < 35)
        {
            float roll = Random.value;
            if (roll < 0.45f)
            {
                return "Impatient";
            }

            if (roll < 0.8f)
            {
                return "Strict";
            }

            return basePersonality == "Friendly" ? "Normal" : basePersonality;
        }

        if (reputation < 70)
        {
            float roll = Random.value;
            if (roll < 0.15f)
            {
                return "Impatient";
            }

            if (roll < 0.55f)
            {
                return "Strict";
            }

            return basePersonality == "Normal" ? "Friendly" : basePersonality;
        }

        float highRepRoll = Random.value;
        if (highRepRoll < 0.15f)
        {
            return "Strict";
        }

        if (highRepRoll < 0.55f)
        {
            return "Friendly";
        }

        return "Normal";
    }

    private static string GetWealthTypeForReputation(int reputation)
    {
        float roll = Random.value;

        if (reputation < 35)
        {
            if (roll < 0.45f) return "Low";
            if (roll < 0.8f) return "Medium";
            if (roll < 0.95f) return "High";
            return "Very High";
        }

        if (reputation < 70)
        {
            if (roll < 0.2f) return "Low";
            if (roll < 0.6f) return "Medium";
            if (roll < 0.9f) return "High";
            return "Very High";
        }

        if (roll < 0.1f) return "Low";
        if (roll < 0.35f) return "Medium";
        if (roll < 0.75f) return "High";
        return "Very High";
    }

    private static float GetTrustModifierForReputation(int reputation)
    {
        if (reputation < 35)
        {
            return -0.08f;
        }

        if (reputation < 70)
        {
            return 0f;
        }

        return 0.08f;
    }

    private static float GetFrustrationModifierForReputation(int reputation)
    {
        if (reputation < 35)
        {
            return 0.08f;
        }

        if (reputation < 70)
        {
            return 0f;
        }

        return -0.04f;
    }

    private static float GetDesperationModifierForReputation(int reputation)
    {
        if (reputation < 35)
        {
            return 0.12f;
        }

        if (reputation < 70)
        {
            return 0f;
        }

        return -0.08f;
    }

    private static string PickAvailableSpice(LocalProfileData profile)
    {
        List<string> available = new List<string>();
        foreach (string spiceKey in SpiceKeys)
        {
            if (GetInventory(profile, spiceKey) > 0)
            {
                available.Add(spiceKey);
            }
        }

        return available.Count > 0 ? available[Random.Range(0, available.Count)] : "pepper";
    }

    private static int GetInventory(LocalProfileData profile, string spiceKey)
    {
        InventoryEntry entry = profile.inventory.Find(item => item.spiceKey == spiceKey);
        return entry != null ? entry.grams : 0;
    }

    private static int PickQuantity(int stock)
    {
        int fallback = Mathf.Max(35, stock);
        List<int> allowed = new List<int>();
        foreach (int quantity in QuantityOptions)
        {
            if (quantity <= stock)
            {
                allowed.Add(quantity);
            }
        }

        return allowed.Count > 0 ? allowed[Random.Range(0, allowed.Count)] : fallback;
    }

    private static string BuildGreeting(string buyerName, string buyerPersonality, string quantityLabel, string spiceName)
    {
        string spice = spiceName.ToLowerInvariant();
        switch (buyerPersonality)
        {
            case "Friendly":
                return $"Greetings, merchant. I am {buyerName}, and I seek {quantityLabel} of {spice}.";
            case "Strict":
                return $"Good day. I am {buyerName}. I am here for {quantityLabel} of {spice}.";
            case "Impatient":
                return $"Merchant, I am {buyerName}. I need {quantityLabel} of {spice} quickly.";
            default:
                return $"Greetings, merchant. I seek {quantityLabel} of {spice}.";
        }
    }

    private static float GetStartMultiplier(string buyerPersonality, string wealthType)
    {
        Vector2 range = wealthType switch
        {
            "Low" => new Vector2(0.85f, 0.93f),
            "Medium" => new Vector2(0.92f, 1.0f),
            "High" => new Vector2(0.98f, 1.06f),
            "Very High" => new Vector2(1.02f, 1.1f),
            _ => new Vector2(0.92f, 1.0f)
        };

        switch (buyerPersonality)
        {
            case "Friendly":
                range += new Vector2(0.03f, 0.05f);
                break;
            case "Strict":
                range += new Vector2(-0.02f, -0.01f);
                break;
            case "Impatient":
                range += new Vector2(0f, 0.02f);
                break;
        }

        return Random.Range(range.x, range.y);
    }

    private static float GetMaxMultiplier(string buyerPersonality, string wealthType)
    {
        Vector2 range = wealthType switch
        {
            "Low" => new Vector2(1.1f, 1.2f),
            "Medium" => new Vector2(1.2f, 1.3f),
            "High" => new Vector2(1.3f, 1.42f),
            "Very High" => new Vector2(1.4f, 1.55f),
            _ => new Vector2(1.2f, 1.3f)
        };

        switch (buyerPersonality)
        {
            case "Friendly":
                range += new Vector2(0.04f, 0.08f);
                break;
            case "Strict":
                range += new Vector2(-0.03f, -0.01f);
                break;
            case "Impatient":
                range += new Vector2(0f, 0.03f);
                break;
        }

        return Random.Range(range.x, range.y);
    }

    private static int GetBuyerPatience(string buyerPersonality)
    {
        switch (buyerPersonality)
        {
            case "Friendly":
                return 7;
            case "Strict":
                return 4;
            case "Impatient":
                return 3;
            default:
                return 5;
        }
    }

    private static float GetStartingTrust(string buyerPersonality)
    {
        switch (buyerPersonality)
        {
            case "Friendly":
                return 0.65f;
            case "Strict":
                return 0.35f;
            case "Impatient":
                return 0.42f;
            default:
                return 0.5f;
        }
    }

    private static float GetStartingFrustration(string buyerPersonality)
    {
        switch (buyerPersonality)
        {
            case "Friendly":
                return 0.05f;
            case "Strict":
                return 0.12f;
            case "Impatient":
                return 0.16f;
            default:
                return 0.08f;
        }
    }

    private static float GetDesperation(string buyerPersonality)
    {
        switch (buyerPersonality)
        {
            case "Friendly":
                return 0.55f;
            case "Strict":
                return 0.4f;
            case "Impatient":
                return 0.72f;
            default:
                return 0.5f;
        }
    }
}
