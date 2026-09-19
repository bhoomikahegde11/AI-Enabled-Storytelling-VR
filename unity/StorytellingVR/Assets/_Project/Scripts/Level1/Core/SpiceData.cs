using System;

[Serializable]
public class SpiceData
{
    public string key;
    public string displayName;
    public int basePricePerKg;
    public float marketMultiplier;
    public int startingInventoryGrams;

    public SpiceData(string key, string displayName, int basePricePerKg, float marketMultiplier, int startingInventoryGrams)
    {
        this.key = key;
        this.displayName = displayName;
        this.basePricePerKg = basePricePerKg;
        this.marketMultiplier = marketMultiplier;
        this.startingInventoryGrams = startingInventoryGrams;
    }
}
