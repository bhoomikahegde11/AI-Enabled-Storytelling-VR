using UnityEngine;
using TMPro;
using System.Collections;

public class EpilogueWalletHUD : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI changeText;

    private int currentBalance;

    public void SetBalance(int balance)
    {
        currentBalance = balance;
        if (balanceText != null)
            balanceText.text = $"{balance} Varahas";
    }

    public int GetCurrentBalance()
    {
        return currentBalance;
    }

    public void ShowGain(int amount)
    {
        if (changeText != null)
        {
            changeText.text = $"+{amount} Varahas";
            changeText.color = Color.green;
            StartCoroutine(ClearChangeText());
        }
    }

    public void ShowSpend(int amount)
    {
        if (changeText != null)
        {
            changeText.text = $"-{amount} Varahas";
            changeText.color = Color.red;
            StartCoroutine(ClearChangeText());
        }
    }

    private IEnumerator ClearChangeText()
    {
        yield return new WaitForSeconds(3f);
        if (changeText != null)
            changeText.text = "";
    }
}
