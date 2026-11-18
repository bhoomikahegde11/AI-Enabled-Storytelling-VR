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

    private void OnTriggerEnter(Collider other) => TryReceive(other);
    private void OnTriggerStay(Collider other) => TryReceive(other);

    private void TryReceive(Collider other)
    {
        if (completed || PauseMenuController.GameplayInputBlocked)
            return;

        // The collider on the scoop itself is the bowl sensor. The controller and fill visuals cannot deliver.
        if (!other.TryGetComponent(out ScooperFill scooper) || scooper != ScooperFill.Instance || !scooper.IsFilled())
            return;

        OrderManager orderManager = OrderManager.Instance;
        if (orderManager == null) return;
        bool tutorialModeActive = orderManager.tutorialMode;
        bool pendingMarketplaceFulfillment = chatManager != null && chatManager.HasPendingFulfillment &&
            ReferenceEquals(chatManager.CurrentPendingFulfillment.acceptedTerms, marketplaceAgreement) &&
            orderManager.MarketplaceOrder != null && orderManager.MarketplaceOrder.BelongsTo(
                Level1GameState.ExistingInstance?.ActiveTrade, marketplaceAgreement);

        if (!tutorialModeActive && !pendingMarketplaceFulfillment)
            return;

        SpiceType expectedSpice = tutorialModeActive ? orderManager.requestedSpice : orderManager.MarketplaceOrder.Spice;
        if (scooper.currentSpice != expectedSpice)
        {
            Debug.Log("Wrong Spice!");

            if (SpiceTutorialManager.Instance != null)
                SpiceTutorialManager.Instance.NotifyWrongSpiceBroughtToBag();

            OVRInput.SetControllerVibration(0.3f, 0.3f, OVRInput.Controller.RTouch);
            scooper.EmptyScooper();
            StartCoroutine(StopHaptics());
            return;
        }

        SpiceType deliveredSpice = scooper.currentSpice;
        if (!tutorialModeActive &&
            !orderManager.TryDeliverMarketplaceScoop(marketplaceAgreement, deliveredSpice, scooper.IsFilled()))
            return;

        completed = true;
        scooper.EmptyScooper(); // Consume before completion callbacks.
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch);
        if (customer != null) customer.FillBag(deliveredSpice);

        if (tutorialModeActive)
        {
            if (SpiceTutorialManager.Instance != null)
                SpiceTutorialManager.Instance.NotifyCorrectBagFilled();
        }
        else
            chatManager.CompleteAcceptedFulfillment(marketplaceAgreement);

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
