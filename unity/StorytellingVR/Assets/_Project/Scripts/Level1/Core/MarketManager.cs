using System;
using System.Collections.Generic;
using UnityEngine;

public class MarketManager
{
    private const float EventChance = 0.35f;
    public const int GramsPerVeesai = 1400;
    public const int MinimumSaleableGrams = 35;

    private readonly Dictionary<string, SpiceData> spiceCatalog = new Dictionary<string, SpiceData>(StringComparer.OrdinalIgnoreCase)
    {
        { "pepper", new SpiceData("pepper", "Pepper", 12, 1.2f, 15000) },
        { "clove", new SpiceData("clove", "Clove", 98, 1.3f, 8000) },
        { "cinnamon", new SpiceData("cinnamon", "Cinnamon", 20, 1.3f, 12000) },
        { "cardamom", new SpiceData("cardamom", "Cardamom", 18, 1.5f, 4000) }
    };

    private readonly List<MarketEventData> events = new List<MarketEventData>
    {
        new MarketEventData
        {
            name = "Portuguese Caravan Arrival",
            description = "A grand Portuguese merchant caravan has arrived at Hampi from Goa. Demand for Pepper has skyrocketed!",
            affected_spice = "pepper",
            price_multiplier = 1.35f,
            quantity_multiplier = 1.5f,
            dialogue_trigger = "portuguese_caravan"
        },
        new MarketEventData
        {
            name = "Temple Chariot Festival",
            description = "The annual Virupaksha Temple festival has begun. Religious offerings demand cardamom in massive amounts!",
            affected_spice = "cardamom",
            price_multiplier = 1.25f,
            quantity_multiplier = 1.3f,
            dialogue_trigger = "temple_festival"
        },
        new MarketEventData
        {
            name = "Krishna Bazaar Wholesale Demand",
            description = "Wholesale merchants are buying up cardamom stocks for bulk shipments. Cardamom demand increases!",
            affected_spice = "cardamom",
            price_multiplier = 1.2f,
            quantity_multiplier = 1.4f,
            dialogue_trigger = "wholesale_demand"
        },
        new MarketEventData
        {
            name = "Malabar Monsoon Deluge",
            description = "Heavy monsoon rains have flooded the southern spice roads. Cinnamon supply is severely restricted!",
            affected_spice = "cinnamon",
            price_multiplier = 1.4f,
            quantity_multiplier = 0.5f,
            dialogue_trigger = "monsoon_flood"
        }
    };

    public List<InventoryEntry> CreateDefaultInventoryEntries()
    {
        return new List<InventoryEntry>
        {
            new InventoryEntry { spiceKey = "pepper", grams = spiceCatalog["pepper"].startingInventoryGrams },
            new InventoryEntry { spiceKey = "clove", grams = spiceCatalog["clove"].startingInventoryGrams },
            new InventoryEntry { spiceKey = "cinnamon", grams = spiceCatalog["cinnamon"].startingInventoryGrams },
            new InventoryEntry { spiceKey = "cardamom", grams = spiceCatalog["cardamom"].startingInventoryGrams }
        };
    }

    public bool TryGetSpice(string spiceKey, out SpiceData spiceData)
    {
        return spiceCatalog.TryGetValue(NormalizeSpiceKey(spiceKey), out spiceData);
    }

    public int CalculateMarketValue(string spiceKey, int quantityGrams, MarketEventData activeEvent) =>
        (int)Math.Round(CalculateMarketTotal(spiceKey, quantityGrams, activeEvent));

    public float CalculateMarketValueExact(string spiceKey, int quantityGrams, MarketEventData activeEvent) =>
        (float)CalculateMarketTotal(spiceKey, quantityGrams, activeEvent);

    private decimal CalculateMarketTotal(string spiceKey, int quantityGrams, MarketEventData activeEvent)
    {
        if (!TryGetSpice(spiceKey, out SpiceData spiceData))
        {
            return 0;
        }

        decimal marketTotal = BaseTotal(spiceData, quantityGrams) * (decimal)spiceData.marketMultiplier;

        if (activeEvent != null && string.Equals(NormalizeSpiceKey(activeEvent.affected_spice), spiceData.key, StringComparison.OrdinalIgnoreCase))
        {
            marketTotal *= (decimal)activeEvent.price_multiplier;
        }

        return marketTotal;
    }

    public int CalculateBaseValue(string spiceKey, int quantityGrams)
    {
        if (!TryGetSpice(spiceKey, out SpiceData spiceData))
        {
            return 0;
        }

        return (int)Math.Round(BaseTotal(spiceData, quantityGrams));
    }

    private static decimal BaseTotal(SpiceData spice, int grams) =>
        (decimal)spice.basePricePerVeesai * Math.Max(0, grams) / GramsPerVeesai;

    private InventoryEntry FindInventory(List<InventoryEntry> inventory, string spiceKey)
    {
        string key = NormalizeSpiceKey(spiceKey);
        return inventory?.Find(item => item != null && string.Equals(item.spiceKey, key, StringComparison.OrdinalIgnoreCase));
    }

    public int AvailableStock(List<InventoryEntry> inventory, string spiceKey) =>
        Mathf.Max(0, FindInventory(inventory, spiceKey)?.grams ?? 0);

    public bool TryDeductInventory(List<InventoryEntry> inventory, string spiceKey, int grams)
    {
        InventoryEntry entry = FindInventory(inventory, spiceKey);
        if (entry == null || grams <= 0 || entry.grams < grams)
            return false;
        entry.grams -= grams;
        return true;
    }

    public MarketEventData RollRandomMarketEvent()
    {
        if (UnityEngine.Random.value >= EventChance)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, events.Count);
        MarketEventData template = events[index];
        return new MarketEventData
        {
            name = template.name,
            description = template.description,
            affected_spice = template.affected_spice,
            price_multiplier = template.price_multiplier,
            quantity_multiplier = template.quantity_multiplier,
            dialogue_trigger = template.dialogue_trigger
        };
    }

    public string NormalizeSpiceKey(string spiceKey)
    {
        if (string.IsNullOrWhiteSpace(spiceKey))
        {
            return string.Empty;
        }

        return spiceKey.Trim().ToLowerInvariant();
    }

    public string FormatTraditionalQuantity(int grams)
    {
        float value = grams;
        if (value <= 0f)
        {
            return "0 Palams (~0g)";
        }

        if (value >= 448000f * 0.9f)
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(value / 448000f));
            float snapped = amount * 448000f;
            string unit = amount == 1 ? "Bahar" : "Bahars";
            return $"{amount} {unit} (~{Math.Round(snapped / 1000f, 1)} kg)";
        }

        if (value >= 11200f * 0.9f)
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(value / 11200f));
            float snapped = amount * 11200f;
            string unit = amount == 1 ? "Manangu" : "Manangus";
            return $"{amount} {unit} (~{Math.Round(snapped / 1000f, 1)} kg)";
        }

        if (value >= 1400f * 0.9f)
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(value / 1400f));
            float snapped = amount * 1400f;
            return $"{amount} Veesai (~{Math.Round(snapped / 1000f, 1)} kg)";
        }

        if (value >= 280f * 0.8f)
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(value / 280f));
            float snapped = amount * 280f;
            string unit = amount == 1 ? "Seer" : "Seers";
            string modernLabel = snapped < 1000f ? $"{(int)snapped}g" : $"{Math.Round(snapped / 1000f, 1)}kg";
            return $"{amount} {unit} (~{modernLabel})";
        }

        int palams = Mathf.Max(1, Mathf.RoundToInt(value / 35f));
        int snappedPalams = palams * 35;
        string palamUnit = palams == 1 ? "Palam" : "Palams";
        return $"{palams} {palamUnit} (~{snappedPalams}g)";
    }
}
