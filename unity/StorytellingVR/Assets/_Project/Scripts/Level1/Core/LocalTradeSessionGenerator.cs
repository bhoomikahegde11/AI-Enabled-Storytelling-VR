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
    private static readonly string[] DemoSpiceKeys = { "pepper", "cardamom" };
    private static readonly int[] DemoOfferValues = { 20, 25, 30, 35, 40, 45, 49 };
    private static readonly int[] DemoStartingOffers = { 25, 30 };
    private readonly DialogueCharacterRegistry dialogueCharacterRegistry = new DialogueCharacterRegistry();

    public LocalGeneratedTradeSession Generate(
        MarketManager marketManager,
        LocalProfileData profile,
        MarketEventData activeEvent,
        string forcedCharacterId = "",
        bool enablePrerecordedVoiceDemoMode = false)
    {
        string spiceKey = enablePrerecordedVoiceDemoMode
            ? PickDemoSpice(profile)
            : PickAvailableSpice(profile);
        marketManager.TryGetSpice(spiceKey, out SpiceData spiceData);

        int stock = GetInventory(profile, spiceKey);
        int quantity = PickQuantity(stock);
        int marketValue = marketManager.CalculateMarketValue(spiceKey, quantity, activeEvent);

        DialogueCharacterProfile selectedCharacter = !string.IsNullOrWhiteSpace(forcedCharacterId)
            ? dialogueCharacterRegistry.GetRegisteredCharacterOrRandom(forcedCharacterId)
            : dialogueCharacterRegistry.GetRandomRegisteredCharacter();
        if (selectedCharacter == null || !dialogueCharacterRegistry.IsRegisteredCharacterId(selectedCharacter.characterId))
        {
            selectedCharacter = dialogueCharacterRegistry.GetRegisteredCharacterOrRandom(string.Empty);
        }

        string buyerName = selectedCharacter != null ? selectedCharacter.displayName : "Abdul Rahman";
        string buyerOrigin = selectedCharacter != null ? selectedCharacter.buyerOrigin : "Arab Caravan Trader";
        string buyerPersonality = selectedCharacter != null ? selectedCharacter.buyerPersonality : "Friendly";
        int startingOffer;
        int maxAcceptablePrice;

        if (enablePrerecordedVoiceDemoMode)
        {
            startingOffer = PickDemoOpeningOffer();
            maxAcceptablePrice = PickDemoMaxAcceptablePrice(startingOffer);
        }
        else
        {
            string wealthType = WealthTypes[Random.Range(0, WealthTypes.Length)];
            float startMultiplier = GetStartMultiplier(buyerPersonality, wealthType);
            float maxMultiplier = GetMaxMultiplier(buyerPersonality, wealthType);
            startingOffer = Mathf.Max(1, Mathf.RoundToInt(marketValue * startMultiplier));
            maxAcceptablePrice = Mathf.Max(startingOffer, Mathf.RoundToInt(marketValue * maxMultiplier));
        }

        Debug.Log("[CUSTOMER] Selected dialogue character: " + (selectedCharacter != null ? selectedCharacter.characterId : "abdul_rahman"));
        Debug.Log("[CUSTOMER] Display name: " + buyerName);
        Debug.Log("[CUSTOMER] Personality: " + buyerPersonality);
        Debug.Log("[CUSTOMER] Prerecorded voice demo mode: " + enablePrerecordedVoiceDemoMode);

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
            buyerPatience = enablePrerecordedVoiceDemoMode ? 6 : GetBuyerPatience(buyerPersonality),
            buyerTrust = GetStartingTrust(buyerPersonality),
            buyerFrustration = GetStartingFrustration(buyerPersonality),
            buyerDesperation = GetDesperation(buyerPersonality),
            greetingText = enablePrerecordedVoiceDemoMode
                ? BuildDemoGreeting(spiceData != null ? spiceData.displayName : "pepper")
                : BuildGreeting(
                buyerName,
                buyerPersonality,
                marketManager.FormatTraditionalQuantity(quantity),
                spiceData != null ? spiceData.displayName : "spice")
        };
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

    private static string PickDemoSpice(LocalProfileData profile)
    {
        List<string> available = new List<string>();
        for (int i = 0; i < DemoSpiceKeys.Length; i++)
        {
            if (GetInventory(profile, DemoSpiceKeys[i]) > 0)
            {
                available.Add(DemoSpiceKeys[i]);
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

    private static string BuildDemoGreeting(string spiceName)
    {
        string spice = (spiceName ?? "pepper").Trim().ToLowerInvariant();
        return $"Good day, merchant. I am here to buy {spice}.";
    }

    private static int PickDemoOpeningOffer()
    {
        return DemoStartingOffers[Random.Range(0, DemoStartingOffers.Length)];
    }

    private static int PickDemoMaxAcceptablePrice(int startingOffer)
    {
        if (startingOffer <= 25)
        {
            return 40;
        }

        return 35;
    }

    private static float GetStartMultiplier(string buyerPersonality, string wealthType)
    {
        float wealthBias = 1f;
        switch (wealthType)
        {
            case "Low":
                wealthBias = 0.92f;
                break;
            case "Medium":
                wealthBias = 1f;
                break;
            case "High":
                wealthBias = 1.08f;
                break;
            case "Very High":
                wealthBias = 1.15f;
                break;
        }

        switch (buyerPersonality)
        {
            case "Friendly":
                return 0.86f * wealthBias;
            case "Strict":
                return 0.72f * wealthBias;
            case "Impatient":
                return 0.78f * wealthBias;
            default:
                return 0.8f * wealthBias;
        }
    }

    private static float GetMaxMultiplier(string buyerPersonality, string wealthType)
    {
        float wealthBias = 1f;
        switch (wealthType)
        {
            case "Low":
                wealthBias = 0.95f;
                break;
            case "Medium":
                wealthBias = 1.08f;
                break;
            case "High":
                wealthBias = 1.18f;
                break;
            case "Very High":
                wealthBias = 1.28f;
                break;
        }

        switch (buyerPersonality)
        {
            case "Friendly":
                return 1.12f * wealthBias;
            case "Strict":
                return 1.0f * wealthBias;
            case "Impatient":
                return 1.04f * wealthBias;
            default:
                return 1.08f * wealthBias;
        }
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
