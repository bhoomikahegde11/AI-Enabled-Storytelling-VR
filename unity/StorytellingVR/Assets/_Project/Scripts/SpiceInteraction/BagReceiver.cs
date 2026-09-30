using UnityEngine;
using System.Collections;
public class BagReceiver : MonoBehaviour
{
    public HandBagAnimation customer;
    public ChatManager chatManager;

    private bool completed = false;
    private TradeTermsSnapshot marketplaceAgreement;

    public void BindMarketplaceOrder(TradeTermsSnapshot agreement)
    {
        completed = false;
        marketplaceAgreement = agreement;
    }

    private void Awake()
    {
        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
            return;

        Debug.Log(
            "BAG RECEIVED: " +
            other.name
        );

        ScooperFill scooper = ScooperFill.Instance;

        // A hand/controller/other collider cannot deliver whatever happens to be in the global scooper.
        if (scooper == null ||
            (other.GetComponentInParent<ScooperFill>() != scooper && other.GetComponentInChildren<ScooperFill>() != scooper))
        {
            Debug.Log("Scooper doesn't exist!");
            return;
        }

        Debug.Log("Scooper Instance Found");
        Debug.Log("Scooper Filled: " + scooper.IsFilled());
        Debug.Log("Current Spice: " + scooper.currentSpice);

        OrderManager orderManager = OrderManager.Instance;
        if (orderManager == null) return;
        bool tutorialModeActive = orderManager.tutorialMode;
        bool pendingMarketplaceFulfillment = chatManager != null && chatManager.HasPendingFulfillment &&
            ReferenceEquals(chatManager.CurrentPendingFulfillment.acceptedTerms, marketplaceAgreement) &&
            orderManager.MarketplaceOrder != null && orderManager.MarketplaceOrder.BelongsTo(
                Level1GameState.ExistingInstance?.ActiveTrade, marketplaceAgreement);

        if (!tutorialModeActive && !pendingMarketplaceFulfillment)
        {
            Debug.LogWarning("[BagReceiver] Ignoring bag delivery because there is no active tutorial order or accepted marketplace fulfillment.");
            return;
        }

        if (!scooper.IsFilled())
        {
            Debug.Log("Scooper Empty");
            return;
        }
        SpiceType expectedSpice = tutorialModeActive ? orderManager.requestedSpice : orderManager.MarketplaceOrder.Spice;
        if (scooper.currentSpice != expectedSpice)
        {
            Debug.Log("Wrong Spice!");

            if (SpiceTutorialManager.Instance != null)
                SpiceTutorialManager.Instance.NotifyWrongSpiceBroughtToBag();

            OVRInput.SetControllerVibration(
        0.3f,
        0.3f,
        OVRInput.Controller.RTouch
    );

            scooper.EmptyScooper();

            StartCoroutine(StopHaptics());

            return;
        }

        SpiceType deliveredSpice = scooper.currentSpice;
        if (!tutorialModeActive)
        {
            if (!orderManager.TryDeliverMarketplaceScoop(marketplaceAgreement, deliveredSpice, scooper.IsFilled()))
                return;
            scooper.EmptyScooper(); // Consume the scoop before completion callbacks can deliver it twice.
        }

        completed = true;

        Debug.Log("Correct Spice");
        OVRInput.SetControllerVibration(
        1f,
        1f,
        OVRInput.Controller.RTouch
    );

        if (customer != null) customer.FillBag(deliveredSpice);
        scooper.EmptyScooper();

        if (tutorialModeActive && SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyCorrectBagFilled();

        if (!tutorialModeActive && chatManager != null)
        {
            if (chatManager.HasPendingFulfillment)
            {
                chatManager.CompleteAcceptedFulfillment(marketplaceAgreement);
            }
            else
            {
                Debug.LogWarning("[BagReceiver] ChatManager assigned, but there is no pending accepted fulfillment to complete.");
            }
        }



        StartCoroutine(StopHaptics());

        
    }
    IEnumerator StopHaptics()
    {
        yield return new WaitForSeconds(0.15f);

        OVRInput.SetControllerVibration(
            0,
            0,
            OVRInput.Controller.RTouch
        );
    }
    public void ResetBag()
    {
        completed = false;
        marketplaceAgreement = null;
    }
}
