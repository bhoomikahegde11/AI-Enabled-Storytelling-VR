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
    private bool marketplaceFulfillmentActive;

    public bool IsMarketplaceFulfillmentActive => marketplaceFulfillmentActive;

    void Awake()
    {
        Instance = this;
        handBagAnimation = FindFirstObjectByType<HandBagAnimation>();
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
        requestedSpice = spice;
    }

    public bool BeginMarketplaceFulfillment(string spiceName)
    {
        tutorialMode = false;

        SpiceType mappedSpice = MapSpiceName(spiceName);
        if (mappedSpice == SpiceType.None)
        {
            Debug.LogWarning("[OrderManager] Could not map negotiated spice to a scoopable spice: " + spiceName);
            return false;
        }

        requestedSpice = mappedSpice;
        marketplaceFulfillmentActive = true;

        handBagAnimation = PrepareMarketplaceCustomerHandoff();

        if (handBagAnimation == null)
        {
            handBagAnimation = FindFirstObjectByType<HandBagAnimation>();
        }

        if (handBagAnimation != null)
        {
            handBagAnimation.StartOrder();
        }
        else
        {
            Debug.LogWarning("[OrderManager] Marketplace fulfillment started, but HandBagAnimation was not found.");
        }

        return true;
    }

    public void CompleteMarketplaceFulfillment()
    {
        marketplaceFulfillmentActive = false;
    }

    public void CancelMarketplaceFulfillment()
    {
        marketplaceFulfillmentActive = false;
    }

    private static SpiceType MapSpiceName(string spiceName)
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

        HandBagAnimation template = FindTemplateHandBagAnimation(activeCustomer);
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
        }

        template.SetActorVisualsVisible(false);
        return activeCustomerHandoff;
    }

    private static HandBagAnimation FindTemplateHandBagAnimation(GameObject activeCustomer)
    {
        HandBagAnimation[] allHandoffs = FindObjectsByType<HandBagAnimation>(FindObjectsSortMode.None);
        foreach (HandBagAnimation handoff in allHandoffs)
        {
            if (handoff != null && handoff.gameObject != activeCustomer)
            {
                return handoff;
            }
        }

        return null;
    }
}
