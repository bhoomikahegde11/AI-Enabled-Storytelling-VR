using UnityEngine;

public class OrderManager : MonoBehaviour
{
    private static readonly Vector3 MarketplaceHandTargetLocalOffset = new Vector3(0.227f, 0.791f, 0f);

    public static OrderManager Instance;

    [Header("Tutorial")]
    public bool tutorialMode = true;

    public SpiceType tutorialSpice = SpiceType.Cardamom;

    [Header("Gameplay")]
    public SpiceType requestedSpice;

    private HandBagAnimation handBagAnimation;
    private HandBagAnimation handoffTemplate;
    private MarketplaceFulfillmentOrder marketplaceOrder;

    public bool IsMarketplaceFulfillmentActive => marketplaceOrder != null;
    public MarketplaceFulfillmentOrder MarketplaceOrder => marketplaceOrder;
    public SpiceType ExpectedSpice => marketplaceOrder?.Spice ?? requestedSpice;

    void Awake()
    {
        Instance = this;
        handoffTemplate = FindTemplateHandBagAnimation(null);
        handBagAnimation = handoffTemplate;
    }

    void Start()
    {
        if (tutorialMode)
        {
            requestedSpice = tutorialSpice;
        }
    }

    public void SetRequestedSpice(SpiceType spice)
    {
        if (marketplaceOrder == null) requestedSpice = spice;
    }

    public bool BeginMarketplaceFulfillment(TradeTermsSnapshot agreement)
    {
        SpiceType mappedSpice = MapSpiceName(agreement?.SpiceKey);
        if (mappedSpice == SpiceType.None || agreement.Price <= 0 || agreement.QuantityGrams <= 0 ||
            !ReferenceEquals(agreement.Source.AcceptedTerms, agreement) || agreement.Source.SettlementClaimed ||
            !ReferenceEquals(agreement.Source, Level1GameState.ExistingInstance?.ActiveTrade))
        {
            Debug.LogWarning("[OrderManager] Cannot fulfill invalid or unsupported accepted terms.");
            return false;
        }
        if (ReferenceEquals(marketplaceOrder?.Agreement, agreement)) return true;
        tutorialMode = false;
        CancelMarketplaceFulfillment();

        requestedSpice = mappedSpice;
        marketplaceOrder = new MarketplaceFulfillmentOrder(agreement);

        handBagAnimation = PrepareMarketplaceCustomerHandoff();

        if (handBagAnimation == null)
        {
            handBagAnimation = FindFirstObjectByType<HandBagAnimation>();
        }

        if (handBagAnimation != null)
        {
            handBagAnimation.StartOrder();
            handBagAnimation.BindMarketplaceAgreement(agreement);
            handBagAnimation.bagReceiver?.BindMarketplaceOrder(agreement);
        }
        else
        {
            Debug.LogWarning("[OrderManager] Marketplace fulfillment started, but HandBagAnimation was not found.");
        }

        return true;
    }

    public void CompleteMarketplaceFulfillment()
    {
        marketplaceOrder = null;
        requestedSpice = SpiceType.None;
        Debug.Log("[OrderManager] Marketplace fulfillment completed. Awaiting next customer reset.");
    }

    public void CancelMarketplaceFulfillment()
    {
        marketplaceOrder = null;
        requestedSpice = tutorialMode ? tutorialSpice : SpiceType.None;
        ResetReusableFulfillmentState();
        Debug.Log("[OrderManager] Marketplace fulfillment state reset.");
    }

    public bool TryDeliverMarketplaceScoop(TradeTermsSnapshot agreement, SpiceType spice, bool scoopFilled)
    {
        return marketplaceOrder != null && marketplaceOrder.TryDeliver(
            Level1GameState.ExistingInstance?.ActiveTrade, agreement, spice, scoopFilled);
    }

    public bool CanCompleteMarketplaceFulfillment(TradeTermsSnapshot agreement)
    {
        return marketplaceOrder != null && marketplaceOrder.CanComplete(
            Level1GameState.ExistingInstance?.ActiveTrade, agreement);
    }

    public static SpiceType MapSpiceName(string spiceName)
    {
        if (string.IsNullOrWhiteSpace(spiceName))
        {
            return SpiceType.None;
        }

        string normalized = spiceName.Trim().ToLowerInvariant();
        return normalized switch
        {
            "cardamom" => SpiceType.Cardamom,
            "pepper" => SpiceType.Pepper,
            "cinnamon" => SpiceType.Cinnamon,
            "turmeric" => SpiceType.Turmeric,
            _ => SpiceType.None
        };
    }

    private HandBagAnimation PrepareMarketplaceCustomerHandoff()
    {
        MarketplaceManager marketplaceManager = FindFirstObjectByType<MarketplaceManager>();
        GameObject activeCustomer = marketplaceManager != null ? marketplaceManager.buyerNPC : null;
        if (activeCustomer == null)
        {
            return null;
        }

        if (handoffTemplate == null) handoffTemplate = FindTemplateHandBagAnimation(activeCustomer);
        HandBagAnimation template = handoffTemplate;
        if (template == null)
        {
            Debug.LogWarning("[OrderManager] Could not find tutorial handoff template for marketplace customer setup.");
            return null;
        }

        HandBagAnimation activeCustomerHandoff = activeCustomer.GetComponent<HandBagAnimation>();
        if (activeCustomerHandoff == null)
        {
            activeCustomerHandoff = activeCustomer.AddComponent<HandBagAnimation>();
        }

        activeCustomerHandoff.ConfigureMarketplaceCustomerHandoff(
            template.bagReceiver,
            template.subtitleCanvas,
            template.handBag,
            template.bagFillPosition,
            template.spiceVisuals,
            MarketplaceHandTargetLocalOffset);

        if (template.bagReceiver != null)
        {
            template.bagReceiver.customer = activeCustomerHandoff;
            template.bagReceiver.ResetBag();
        }

        template.SetActorVisualsVisible(false);
        return activeCustomerHandoff;
    }

    private static HandBagAnimation FindTemplateHandBagAnimation(GameObject activeCustomer)
    {
        HandBagAnimation[] allHandoffs = FindObjectsByType<HandBagAnimation>(FindObjectsSortMode.None);
        foreach (HandBagAnimation handoff in allHandoffs)
        {
            if (handoff != null && !handoff.UsesMarketplaceCustomerVisuals && handoff.gameObject != activeCustomer &&
                handoff.handBag != null && handoff.bagReceiver != null && handoff.bagFillPosition != null)
            {
                return handoff;
            }
        }

        return null;
    }

    private void ResetReusableFulfillmentState()
    {
        // Stop this order's actors before reassigning their shared bag, without touching unrelated handoffs.
        if (handBagAnimation != null) handBagAnimation.ResetHandoffState();
        if (handoffTemplate != null && handoffTemplate != handBagAnimation) handoffTemplate.ResetHandoffState();

        BagReceiver bagReceiver = handBagAnimation != null ? handBagAnimation.bagReceiver : handoffTemplate?.bagReceiver;
        if (bagReceiver != null)
        {
            bagReceiver.ResetBag();
        }

        if (ScooperFill.Instance != null)
        {
            ScooperFill.Instance.ResetScooper();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
