using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Level1HUDManager : MonoBehaviour
{
    [Header("Subtitle References")]
    public GameObject subtitlePanel;
    public TMP_Text speakerNameText;
    public TMP_Text npcSubtitleText;

    [Header("Player References")]
    public TMP_InputField playerInput;
    public TMP_Text voiceStatusText;

    [Header("Economy References")]
    public TMP_Text varahaText;
    public Slider reputationSlider;
    public TMP_Text reputationText;

    [Header("NPC Intro References")]
    public GameObject npcIntroPanel;
    public TMP_Text introNameText;
    public TMP_Text introOriginText;

    [Header("Transaction References")]
    public GameObject tradeCompletePanel;
    public TMP_Text tradeSummaryText;

    [Header("Current Trade References")]
    public GameObject currentTradePanel;
    public TMP_Text tradeSpiceText;
    public TMP_Text tradeQuantityText;
    public TMP_Text tradeBuyerText;

    private Coroutine introFadeCoroutine;
    private Coroutine tradeCompleteCoroutine;

    private void Start()
    {
        // Set initial UI panel states
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (npcIntroPanel != null) npcIntroPanel.SetActive(false);
        if (tradeCompletePanel != null) tradeCompletePanel.SetActive(false);
        if (currentTradePanel != null) currentTradePanel.SetActive(false);
    }

    public void ShowSubtitle(string speaker, string text)
    {
        if (subtitlePanel == null) return;
        subtitlePanel.SetActive(true);

        if (speakerNameText != null) speakerNameText.text = speaker;
        if (npcSubtitleText != null) npcSubtitleText.text = text;
    }

    public void HideSubtitle()
    {
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
    }

    public void ShowNPCIntro(string name, string origin)
    {
        if (npcIntroPanel == null) return;
        npcIntroPanel.SetActive(true);

        if (introNameText != null) introNameText.text = name;
        if (introOriginText != null) introOriginText.text = origin;

        if (introFadeCoroutine != null)
        {
            StopCoroutine(introFadeCoroutine);
        }
        introFadeCoroutine = StartCoroutine(NPCIntroFadeRoutine(3f));
    }

    private IEnumerator NPCIntroFadeRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (npcIntroPanel != null)
        {
            npcIntroPanel.SetActive(false);
        }
    }

    public void UpdateMoney(int varaha)
    {
        if (varahaText != null)
        {
            varahaText.text = $"🪙 {varaha}";
        }
    }

    public void UpdateRespect(int respect)
    {
        if (reputationSlider != null)
        {
            reputationSlider.value = respect;
        }

        if (reputationText != null)
        {
            reputationText.text = $"Respect: {respect}";
        }
    }

    public void ShowTradeComplete(TransactionSummary transaction)
    {
        if (tradeCompletePanel == null) return;
        tradeCompletePanel.SetActive(true);

        if (tradeSummaryText != null)
        {
            tradeSummaryText.text = $"TRADE SEALED\n\nBuyer:\n{transaction.buyer_name}\n\nSold:\n{transaction.quantity} {transaction.item}\n\nEarned:\n+{transaction.earned} Varaha\n\nRespect:\n+{transaction.respect_change}";
        }

        if (tradeCompleteCoroutine != null)
        {
            StopCoroutine(tradeCompleteCoroutine);
        }
        tradeCompleteCoroutine = StartCoroutine(HideTradeCompleteRoutine(5f));
    }

    private IEnumerator HideTradeCompleteRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tradeCompletePanel != null)
        {
            tradeCompletePanel.SetActive(false);
        }
    }

    public void ShowCurrentTrade(string spice, string quantity, string buyerName)
    {
        if (currentTradePanel == null) return;
        currentTradePanel.SetActive(true);

        if (tradeSpiceText != null) tradeSpiceText.text = $"Spice: {spice}";
        if (tradeQuantityText != null) tradeQuantityText.text = $"Quantity: {quantity}";
        if (tradeBuyerText != null) tradeBuyerText.text = $"Buyer: {buyerName}";
    }

    public void HideCurrentTrade()
    {
        if (currentTradePanel != null)
        {
            currentTradePanel.SetActive(false);
        }
    }
}
