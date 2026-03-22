using UnityEngine;

public class PlayerSessionData : MonoBehaviour
{
    [Header("Session Metrics")]
    public float respectScore = 50f; // 0 to 100
    public float totalRevenue = 0f;
    public float totalCost = 0f;
    public float totalProfit = 0f;
    public int customersServed = 0;
    public int customersWalkedAway = 0;

    [Header("Spawn Rate")]
    public float baseSpawnRate = 1f;
    public float customerSpawnRate = 1f;

    void Awake()
    {
        respectScore = Mathf.Clamp(respectScore, 0f, 100f);
        RecalculateSpawnRate();
    }

    public void RecalculateSpawnRate()
    {
        customerSpawnRate = baseSpawnRate * (0.5f + respectScore / 100f);
    }

    public void RecordDeal(float playerPrice, float fairTotalPrice, int quantity, float costPerUnit, float timeTakenSeconds, bool isDesperate)
    {
        customersServed++;
        totalRevenue += playerPrice;
        totalCost += costPerUnit * quantity;

        // Respect adjustments based on how fair the deal was
        float overPercent = (playerPrice - fairTotalPrice) / fairTotalPrice;

        if (playerPrice <= fairTotalPrice * 1.20f)
        {
            respectScore += 8f;
        }
        else if (playerPrice <= fairTotalPrice * 1.40f)
        {
            respectScore += 3f;
        }
        else
        {
            respectScore += 1f;
        }

        if (isDesperate && overPercent <= 0.0f)
        {
            respectScore += 5f;
        }

        // Time penalties
        if (timeTakenSeconds > 90f)
        {
            float extra = timeTakenSeconds - 90f;
            int steps = Mathf.FloorToInt(extra / 30f);
            respectScore -= 2f * steps;
        }

        ClampRespectAndRecalc();
    }

    public void RecordWalkaway(bool roundOne)
    {
        customersWalkedAway++;
        if (roundOne)
            respectScore -= 12f;
        else
            respectScore -= 8f;

        ClampRespectAndRecalc();
    }

    void ClampRespectAndRecalc()
    {
        respectScore = Mathf.Clamp(respectScore, 0f, 100f);
        RecalculateSpawnRate();
    }

    public void EndOfDaySave()
    {
        totalProfit = totalRevenue - totalCost;

        PlayerPrefs.SetFloat("Day_TotalRevenue", totalRevenue);
        PlayerPrefs.SetFloat("Day_TotalCost", totalCost);
        PlayerPrefs.SetFloat("Day_TotalProfit", totalProfit);
        PlayerPrefs.SetFloat("Day_RespectScore", respectScore);
        PlayerPrefs.SetInt("Day_CustomersServed", customersServed);
        PlayerPrefs.SetInt("Day_CustomersWalkedAway", customersWalkedAway);
        PlayerPrefs.Save();
    }

    public string GetRespectLabel()
    {
        if (respectScore >= 80f) return "Beloved";
        if (respectScore >= 60f) return "Respected";
        if (respectScore >= 40f) return "Neutral";
        if (respectScore >= 20f) return "Distrusted";
        return "Infamous";
    }
}
