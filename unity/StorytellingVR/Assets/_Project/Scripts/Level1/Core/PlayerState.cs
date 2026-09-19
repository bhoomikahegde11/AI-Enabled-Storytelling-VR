using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public const int DefaultReputation = 50;
    public const int DefaultVarahas = 100;
    public const int MinReputation = 0;
    public const int MaxReputation = 100;

    [SerializeField] private int currentVarahas = DefaultVarahas;
    [SerializeField] private int currentReputation = DefaultReputation;

    public int CurrentVarahas => currentVarahas;
    public int CurrentReputation => currentReputation;

    public void LoadFrom(int varahas, int reputation)
    {
        currentVarahas = Mathf.Max(0, varahas);
        currentReputation = Mathf.Clamp(reputation, MinReputation, MaxReputation);
    }

    public void AddMoney(int amount)
    {
        currentVarahas = Mathf.Max(0, currentVarahas + amount);
    }

    public void RemoveMoney(int amount)
    {
        currentVarahas = Mathf.Max(0, currentVarahas - Mathf.Abs(amount));
    }

    public void UpdateReputation(int delta)
    {
        currentReputation = Mathf.Clamp(currentReputation + delta, MinReputation, MaxReputation);
    }

    public static string GetRankName(int reputation)
    {
        if (reputation <= 20) return "Unknown Trader";
        if (reputation <= 40) return "Small Merchant";
        if (reputation <= 60) return "Trusted Merchant";
        if (reputation <= 80) return "Royal Supplier";
        return "Legendary Merchant";
    }
}
