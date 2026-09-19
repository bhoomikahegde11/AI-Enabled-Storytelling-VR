using UnityEngine;

public class LocalTradeOutcome
{
    public int reputationDelta;
    public int currentMoney;
    public int currentReputation;
    public bool isSuccess;
    public TransactionSummary transaction;
}

public class TransactionManager
{
    public LocalTradeOutcome ApplyTrade(
        PlayerState playerState,
        LocalProfileData profile,
        MarketManager marketManager,
        string spiceKey,
        int finalPrice,
        int finalQuantityGrams,
        float trust,
        float frustration,
        int outOfWorldCount,
        string outcome,
        int marketPrice,
        string buyerName,
        string buyerOrigin)
    {
        LocalTradeOutcome result = new LocalTradeOutcome();

        int varahaChange = 0;
        int reputationChange = 0;
        bool accepted = string.Equals(outcome, "ACCEPT", System.StringComparison.OrdinalIgnoreCase);

        if (accepted)
        {
            varahaChange = Mathf.Max(0, finalPrice);
            reputationChange = 2;

            if (finalPrice > marketPrice)
            {
                reputationChange += 2;
            }

            if (trust >= 0.7f && frustration <= 0.3f)
            {
                reputationChange += 1;
            }

            if (frustration >= 0.6f)
            {
                reputationChange = -5;
            }
        }
        else
        {
            reputationChange = (frustration >= 0.6f || string.Equals(outcome, "WALK_AWAY", System.StringComparison.OrdinalIgnoreCase)) ? -5 : -3;
        }

        if (outOfWorldCount > 0)
        {
            reputationChange -= 10 * outOfWorldCount;
        }

        if (varahaChange > 0)
        {
            playerState.AddMoney(varahaChange);
        }

        playerState.UpdateReputation(reputationChange);

        profile.global_metrics.total_varahas = playerState.CurrentVarahas;
        profile.global_metrics.reputation = playerState.CurrentReputation;

        if (accepted && finalQuantityGrams > 0)
        {
            marketManager.DeductInventory(profile.inventory, spiceKey, finalQuantityGrams);
        }

        if (accepted)
        {
            int baseValue = marketManager.CalculateBaseValue(spiceKey, finalQuantityGrams);
            result.transaction = new TransactionSummary
            {
                item = Capitalize(spiceKey),
                quantity = marketManager.FormatTraditionalQuantity(finalQuantityGrams),
                earned = Mathf.Max(0, finalPrice),
                profit = Mathf.Max(0, finalPrice - baseValue),
                respect_change = reputationChange,
                buyer_name = buyerName,
                buyer_origin = buyerOrigin
            };
        }

        result.reputationDelta = reputationChange;
        result.currentMoney = playerState.CurrentVarahas;
        result.currentReputation = playerState.CurrentReputation;
        result.isSuccess = accepted && varahaChange > 0;
        return result;
    }

    private static string Capitalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Unknown";
        }

        string normalized = value.Trim().ToLowerInvariant();
        return char.ToUpperInvariant(normalized[0]) + normalized.Substring(1);
    }
}
