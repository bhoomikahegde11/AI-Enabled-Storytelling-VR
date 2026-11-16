using System;

[Serializable]
public class SpiceData
{
    public string key;
    public string displayName;
    public int basePricePerVeesai;
    public float marketMultiplier;
    public int startingInventoryGrams;

    public SpiceData(string key, string displayName, int basePricePerVeesai, float marketMultiplier, int startingInventoryGrams)
    {
        this.key = key;
        this.displayName = displayName;
        this.basePricePerVeesai = basePricePerVeesai;
        this.marketMultiplier = marketMultiplier;
        this.startingInventoryGrams = startingInventoryGrams;
    }
}
