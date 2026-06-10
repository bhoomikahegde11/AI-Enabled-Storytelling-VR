using System;
using UnityEngine;

public class LocalTradeState
{
    public string spiceKey;
    public string spiceDisplayName;
    public int quantityGrams;
    public string quantityLabel;
    public string buyerName;
    public string buyerOrigin;
    public string buyerPersonality;
    public int startingNpcOffer;
    public int npcOffer;
    public int marketValue;
    public int previousNpcOffer;
    public int repeatedRejectedPrice;
    public int repeatedPriceQueries;
    public int repeatedItemQueries;
    public int repeatedQuantityQueries;
    public int repeatedBargains;
    public int offTopicCount;
    public string lastNormalizedPlayerInput;
    public string lastIntentName;
    public int lastPlayerPrice;
    public int lastSellerPrice;
    public int maxBuyerPrice;
    public int minIncrement;
    public int buyerPatience;
    public int lowPriceCount;
    public int tooExpensiveCount;
    public int rejectionCount;
    public int counterCount;
    public int hostileCount;
    public int outOfWorldCount;
    public int noCount;
    public int sellerMinPrice;
    public int referencePrice;
    public float buyerTrust;
    public float buyerFrustration;
    public float buyerDesperation;
    public bool priceIntroduced;
    public bool budgetRevealed;
}

public class Level1GameState : MonoBehaviour
{
    private static Level1GameState instance;

    public static Level1GameState Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject host = new GameObject("Level1GameState");
                instance = host.AddComponent<Level1GameState>();
            }

            return instance;
        }
    }

    private readonly PlayerState playerState = new PlayerState();
    private MarketManager marketManager;
    private LocalSaveManager localSaveManager;
    private TransactionManager transactionManager;
    private LocalTradeSessionGenerator localTradeSessionGenerator;
    private LocalProfileData profile;
    private MarketEventData activeEvent;
    private LocalTradeState activeTrade;
    private int lastDealReferencePrice;
    private bool initialized;

    public int CurrentMoney => playerState.CurrentVarahas;
    public int CurrentReputation => playerState.CurrentReputation;
    public LocalTradeState ActiveTrade => activeTrade;
    public MarketEventData ActiveEvent => activeEvent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureInitialized();
    }

    public void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        marketManager = new MarketManager();
        localSaveManager = new LocalSaveManager();
        transactionManager = new TransactionManager();
        localTradeSessionGenerator = new LocalTradeSessionGenerator();
        profile = localSaveManager.LoadProfile(marketManager);
        playerState.LoadFrom(profile.global_metrics.total_varahas, profile.global_metrics.reputation);
        initialized = true;
    }

    public void PrepareForNewCustomer()
    {
        EnsureInitialized();
        activeTrade = null;
        activeEvent = null;
    }

    public void SyncTradeFromBackend(string buyerName, string buyerOrigin, string spiceName, string quantityLabel, int quantityGrams, int npcOffer, MarketEventData backendEvent)
    {
        EnsureInitialized();

        if (backendEvent != null)
        {
            activeEvent = backendEvent;
        }
        else if (activeEvent == null)
        {
            activeEvent = marketManager.RollRandomMarketEvent();
        }

        string spiceKey = marketManager.NormalizeSpiceKey(spiceName);
        if (string.IsNullOrEmpty(spiceKey))
        {
            return;
        }

        if (!marketManager.TryGetSpice(spiceKey, out SpiceData spiceData))
        {
            return;
        }

        int safeQuantity = Mathf.Max(0, quantityGrams);
        string resolvedQuantityLabel = !string.IsNullOrEmpty(quantityLabel)
            ? quantityLabel
            : marketManager.FormatTraditionalQuantity(safeQuantity);

        activeTrade = new LocalTradeState
        {
            spiceKey = spiceData.key,
            spiceDisplayName = spiceData.displayName,
            quantityGrams = safeQuantity,
            quantityLabel = resolvedQuantityLabel,
            buyerName = buyerName,
            buyerOrigin = buyerOrigin,
            buyerPersonality = string.Empty,
            startingNpcOffer = npcOffer,
            npcOffer = npcOffer,
            previousNpcOffer = npcOffer,
            marketValue = marketManager.CalculateMarketValue(spiceData.key, safeQuantity, activeEvent),
            referencePrice = lastDealReferencePrice
        };
    }

    public LocalGeneratedTradeSession GenerateLocalSession()
    {
        EnsureInitialized();

        if (activeEvent == null)
        {
            activeEvent = marketManager.RollRandomMarketEvent();
        }

        LocalGeneratedTradeSession session = localTradeSessionGenerator.Generate(marketManager, profile, activeEvent);
        string spiceKey = marketManager.NormalizeSpiceKey(session.spiceName);
        marketManager.TryGetSpice(spiceKey, out SpiceData spiceData);

        activeTrade = new LocalTradeState
        {
            spiceKey = spiceKey,
            spiceDisplayName = spiceData != null ? spiceData.displayName : session.spiceName,
            quantityGrams = session.quantityGrams,
            quantityLabel = session.quantityLabel,
            buyerName = session.buyerName,
            buyerOrigin = session.buyerOrigin,
            buyerPersonality = session.buyerPersonality,
            startingNpcOffer = session.startingOffer,
            npcOffer = session.startingOffer,
            previousNpcOffer = session.startingOffer,
            marketValue = marketManager.CalculateMarketValue(spiceKey, session.quantityGrams, activeEvent),
            maxBuyerPrice = session.maxAcceptablePrice,
            buyerPatience = session.buyerPatience,
            buyerTrust = session.buyerTrust,
            buyerFrustration = session.buyerFrustration,
            buyerDesperation = session.buyerDesperation,
            referencePrice = lastDealReferencePrice
        };

        return session;
    }

    public CurrentTrade BuildCurrentTradeForHud()
    {
        if (activeTrade == null)
        {
            return null;
        }

        return new CurrentTrade
        {
            spice = activeTrade.spiceDisplayName,
            quantity = activeTrade.quantityLabel,
            npc_offer = activeTrade.npcOffer,
            market_value = activeTrade.marketValue
        };
    }

    public void UpdateActiveTradeOffer(int npcOffer)
    {
        if (activeTrade == null)
        {
            return;
        }

        activeTrade.previousNpcOffer = activeTrade.npcOffer;
        activeTrade.npcOffer = Mathf.Max(0, npcOffer);
    }

    public void UpdateActiveTradeQuantity(int quantityGrams)
    {
        if (activeTrade == null)
        {
            return;
        }

        int safeQuantity = Mathf.Max(1, quantityGrams);
        activeTrade.quantityGrams = safeQuantity;
        activeTrade.quantityLabel = marketManager.FormatTraditionalQuantity(safeQuantity);
        activeTrade.marketValue = marketManager.CalculateMarketValue(activeTrade.spiceKey, safeQuantity, activeEvent);
    }

    public LocalTradeOutcome ResolveTradeFromBackend(string action, int finalPrice, int finalQuantityGrams, float trust, float frustration, int outOfWorldCount)
    {
        EnsureInitialized();

        if (activeTrade == null)
        {
            return new LocalTradeOutcome
            {
                currentMoney = playerState.CurrentVarahas,
                currentReputation = playerState.CurrentReputation,
                isSuccess = false,
                reputationDelta = 0
            };
        }

        LocalTradeOutcome outcome = transactionManager.ApplyTrade(
            playerState,
            profile,
            marketManager,
            activeTrade.spiceKey,
            finalPrice,
            finalQuantityGrams,
            trust,
            frustration,
            outOfWorldCount,
            action,
            activeTrade.marketValue,
            activeTrade.buyerName,
            activeTrade.buyerOrigin
        );

        if (string.Equals(action, "ACCEPT", System.StringComparison.OrdinalIgnoreCase) && finalPrice > 0)
        {
            lastDealReferencePrice = finalPrice;
        }

        localSaveManager.SaveProfile(profile);
        activeTrade = null;
        return outcome;
    }
}
