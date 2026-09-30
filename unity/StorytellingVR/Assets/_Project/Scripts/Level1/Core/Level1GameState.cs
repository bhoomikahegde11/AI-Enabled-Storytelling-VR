using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NegotiationTactic
{
    NONE,
    PRICE_ANCHOR,
    CONSISTENCY_CHALLENGE,
    APPEAL_TO_FAIRNESS,
    QUALITY_ARGUMENT,
    URGENCY,
    RELUCTANT_CONCESSION,
    SPLIT_DIFFERENCE,
    FINAL_OFFER,
    THREAT_TO_LEAVE,
    FRIENDLY_SMALL_TALK
}

public enum ClarificationKind
{
    None,
    EmptyTranscript,
    UnrecognizedSpeech,
    MissingPrice,
    MissingQuantity,
    AmbiguousAcceptOrCounter,
    HistoricalPriceOnly,
    MultipleActionablePrices,
    FulfillmentExpected,
    UnresolvedTerms
}

public enum TradeSpeaker
{
    None,
    Player,
    NPC
}

public class TradeOfferRecord
{
    public TradeSpeaker speaker;
    public int value;
    public int turnIndex;
    public bool wasAccepted;
    public bool wasRejected;
    public bool wasCountered;
    public string sourceText;
}

// The terms of an outstanding/accepted offer, independent of subsequent mutable state.
public sealed class TradeTermsSnapshot
{
    public LocalTradeState Source { get; }
    public string SpiceKey { get; }
    public string SpiceName { get; }
    public string QuantityLabel { get; }
    public int QuantityGrams { get; }
    public int Price { get; }
    public int MarketValue { get; }
    public string BuyerName { get; }
    public string BuyerOrigin { get; }

    public TradeTermsSnapshot(LocalTradeState trade, int price, int quantityGrams, string quantityLabel, int marketValue)
    {
        Source = trade;
        SpiceKey = trade.spiceKey;
        SpiceName = trade.spiceDisplayName;
        BuyerName = trade.buyerName;
        BuyerOrigin = trade.buyerOrigin;
        Price = price;
        QuantityGrams = quantityGrams;
        QuantityLabel = quantityLabel;
        MarketValue = marketValue;
    }

    public static TradeTermsSnapshot Capture(LocalTradeState trade)
    {
        return new TradeTermsSnapshot(trade, trade.npcOffer, trade.quantityGrams, trade.quantityLabel, trade.marketValue);
    }
}

public class LocalTradeState
{
    public bool negotiationInitialized;
    public TradeTermsSnapshot AcceptedTerms { get; private set; }
    public bool SettlementClaimed { get; internal set; }

    // Backend/debug acceptance uses the same frozen contract as the local decision path.
    public bool BindAcceptedTerms(TradeTermsSnapshot terms)
    {
        if (terms == null || !ReferenceEquals(terms.Source, this) || SettlementClaimed ||
            terms.Price <= 0 || terms.QuantityGrams <= 0)
            return false;
        if (AcceptedTerms != null) return ReferenceEquals(AcceptedTerms, terms);
        AcceptedTerms = terms;
        return true;
    }

    // Only a validated brain decision reaches this commit boundary. Candidates remain on the input/result.
    public void ApplyDecision(RuleBasedNPCBrainResult result)
    {
        if (AcceptedTerms != null || result == null || result.requiresClarification || result.walkedAway)
            return;

        if (result.acceptedTerms != null)
        {
            if (!ReferenceEquals(result.acceptedTerms.Source, this))
                return;
            if (!BindAcceptedTerms(result.acceptedTerms)) return;
            spiceKey = AcceptedTerms.SpiceKey;
            spiceDisplayName = AcceptedTerms.SpiceName;
            quantityGrams = AcceptedTerms.QuantityGrams;
            quantityLabel = AcceptedTerms.QuantityLabel;
            marketValue = AcceptedTerms.MarketValue;
        }
        else if (result.commitsQuantity)
        {
            quantityGrams = result.resolvedQuantityGrams;
            quantityLabel = result.quantityLabel;
            marketValue = result.marketValue;
        }

        previousNpcOffer = npcOffer;
        npcOffer = AcceptedTerms != null ? AcceptedTerms.Price : Mathf.Max(0, result.updatedOffer);
        lastSpeaker = TradeSpeaker.NPC;
        npcOfferHistory.Add(new TradeOfferRecord
        {
            speaker = TradeSpeaker.NPC, value = npcOffer, turnIndex = turnIndex,
            wasAccepted = result.isAccepted, sourceText = result.replyText
        });
    }
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
    public int actualPriceOfferCount;
    public int softBargainCount;
    public int hardRejectCount;
    public int repeatedOverMaxCount;
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
    public int turnIndex;
    public int currentPlayerAsk;
    public int lastAcceptedCandidate;
    public int lastRejectedOffer;
    public TradeSpeaker lastSpeaker;
    public string lastNpcQuestion;
    public string unresolvedClarification;
    public List<TradeOfferRecord> npcOfferHistory = new List<TradeOfferRecord>();
    public List<TradeOfferRecord> playerOfferHistory = new List<TradeOfferRecord>();
}

public class Level1GameState : MonoBehaviour
{
    private static Level1GameState instance;
    private const float DefaultMarketDayDurationSeconds = 720f;
    private const float MinimumMarketDayDurationSeconds = 2000f;
    private const float TimerAutosaveIntervalSeconds = 5f;

    #if UNITY_EDITOR
    [Header("Editor Debug")]
    [SerializeField] private string debugForceCharacterId = "";
    #endif

    [Header("Market Day")]
    [SerializeField] private float marketDayDurationSeconds = DefaultMarketDayDurationSeconds;

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
    private Coroutine marketDayTimerCoroutine;
    private float marketDayStartedAt = -1f;
    private float marketDayRemainingSeconds = DefaultMarketDayDurationSeconds;
    private int lastLoggedRemainingWholeSecond = -1;
    private float timerAutosaveElapsed;

    public int CurrentMoney => playerState.CurrentVarahas;
    public int CurrentReputation => playerState.CurrentReputation;
    public LocalTradeState ActiveTrade => activeTrade;
    public MarketEventData ActiveEvent => activeEvent;
    public string ActiveSavePath => localSaveManager != null ? localSaveManager.SavePath : LocalSaveManager.GetActiveSavePath();
    public static Level1GameState ExistingInstance => instance;
    public bool MarketDayStarted { get; private set; }
    public bool MarketDayEnded { get; private set; }
    public float MarketDayDurationSeconds => Mathf.Max(MinimumMarketDayDurationSeconds, marketDayDurationSeconds);
    public float MarketDayRemainingSeconds => marketDayRemainingSeconds;
    public float MarketDayNormalizedProgress => Mathf.Clamp01(1f - (marketDayRemainingSeconds / MarketDayDurationSeconds));
    public event Action MarketDayStartedEvent;
    public event Action MarketDayEndedEvent;
    public event Action<float> MarketDayTickEvent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        ValidateMarketDayDuration();
        EnsureInitialized();
    }

    private void OnValidate()
    {
        ValidateMarketDayDuration();
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

    public void StartMarketDay()
    {
        EnsureInitialized();

        if (MarketDayStarted || MarketDayEnded)
        {
            return;
        }

        if (marketDayTimerCoroutine != null)
        {
            StopCoroutine(marketDayTimerCoroutine);
        }

        float restoredRemainingSeconds = profile != null && profile.hasSavedMarketDayTimer
            ? Mathf.Clamp(profile.remainingMarketDaySeconds, 0f, MarketDayDurationSeconds)
            : MarketDayDurationSeconds;

        ApplyMarketDayState(restoredRemainingSeconds, broadcastState: true);
    }

    public void EndMarketDay()
    {
        if (!MarketDayStarted || MarketDayEnded)
        {
            return;
        }

        MarketDayEnded = true;
        marketDayRemainingSeconds = 0f;
        timerAutosaveElapsed = 0f;

        if (marketDayTimerCoroutine != null)
        {
            StopCoroutine(marketDayTimerCoroutine);
            marketDayTimerCoroutine = null;
        }

        Debug.Log("[MARKET DAY] Market day ended.");
        PersistMarketDayProgress();
        MarketDayTickEvent?.Invoke(marketDayRemainingSeconds);
        MarketDayEndedEvent?.Invoke();
    }

    private IEnumerator MarketDayTimerRoutine()
    {
        while (!MarketDayEnded)
        {
            float elapsed = Mathf.Max(0f, Time.time - marketDayStartedAt);
            float remaining = Mathf.Clamp(MarketDayDurationSeconds - elapsed, 0f, MarketDayDurationSeconds);
            marketDayRemainingSeconds = remaining;
            timerAutosaveElapsed += Time.deltaTime;

            int remainingWholeSeconds = Mathf.CeilToInt(remaining);
            if (remainingWholeSeconds != lastLoggedRemainingWholeSecond)
            {
                lastLoggedRemainingWholeSecond = remainingWholeSeconds;
                MarketDayTickEvent?.Invoke(marketDayRemainingSeconds);

                if ((remainingWholeSeconds > 0 && remainingWholeSeconds % 60 == 0) || remainingWholeSeconds <= 10)
                {
                    Debug.Log($"[MARKET DAY] Remaining time: {FormatMarketDayTime(remainingWholeSeconds)}");
                }
            }

            if (timerAutosaveElapsed >= TimerAutosaveIntervalSeconds)
            {
                PersistMarketDayProgress();
            }

            if (remaining <= 0f)
            {
                break;
            }

            yield return null;
        }

        EndMarketDay();
    }

    private void ValidateMarketDayDuration()
    {
        marketDayDurationSeconds = Mathf.Max(MinimumMarketDayDurationSeconds, marketDayDurationSeconds);

        if (!MarketDayStarted || MarketDayEnded)
        {
            marketDayRemainingSeconds = MarketDayDurationSeconds;
        }
    }

    private static string FormatMarketDayTime(int totalSeconds)
    {
        int safeSeconds = Mathf.Max(0, totalSeconds);
        int minutes = safeSeconds / 60;
        int seconds = safeSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    public void PrepareForNewCustomer()
    {
        EnsureInitialized();
        activeTrade = null;
        activeEvent = null;
    }

    public void SaveProfileToDisk()
    {
        EnsureInitialized();
        RefreshProgressionFieldsFromDisk();
        SyncMarketDayTimerToProfile();
        localSaveManager.SaveProfile(profile);
        timerAutosaveElapsed = 0f;
    }

    public void ReloadProfileFromDisk()
    {
        EnsureInitialized();

        profile = localSaveManager.LoadProfile(marketManager);
        playerState.LoadFrom(profile.global_metrics.total_varahas, profile.global_metrics.reputation);
        activeTrade = null;
        activeEvent = null;
        lastDealReferencePrice = 0;
        StopMarketDayTimerRoutine();
        ApplyLoadedMarketDayStateWithoutStartingRoutine();
    }

    public void ResetProfileToDefaults()
    {
        EnsureInitialized();
        localSaveManager.DeleteProfile();
        ReloadProfileFromDisk();
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

        #if UNITY_EDITOR
        LocalGeneratedTradeSession session = localTradeSessionGenerator.Generate(marketManager, profile, activeEvent, debugForceCharacterId);
        #else
        LocalGeneratedTradeSession session = localTradeSessionGenerator.Generate(marketManager, profile, activeEvent);
        #endif
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
            negotiationInitialized = true,
            minIncrement = Mathf.Max(2, Mathf.RoundToInt(marketManager.CalculateMarketValue(spiceKey, session.quantityGrams, activeEvent) * 0.02f)),
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
            spice = activeTrade.AcceptedTerms != null ? activeTrade.AcceptedTerms.SpiceName : activeTrade.spiceDisplayName,
            quantity = activeTrade.AcceptedTerms != null ? activeTrade.AcceptedTerms.QuantityLabel : activeTrade.quantityLabel,
            npc_offer = activeTrade.AcceptedTerms != null ? activeTrade.AcceptedTerms.Price : activeTrade.npcOffer,
            market_value = activeTrade.AcceptedTerms != null ? activeTrade.AcceptedTerms.MarketValue : activeTrade.marketValue
        };
    }

    public void UpdateActiveTradeOffer(int npcOffer)
    {
        if (activeTrade == null || activeTrade.AcceptedTerms != null)
        {
            return;
        }

        activeTrade.previousNpcOffer = activeTrade.npcOffer;
        activeTrade.npcOffer = Mathf.Max(0, npcOffer);
        activeTrade.lastSpeaker = TradeSpeaker.NPC;
        if (activeTrade.npcOffer > 0)
        {
            activeTrade.npcOfferHistory.Add(new TradeOfferRecord
            {
                speaker = TradeSpeaker.NPC,
                value = activeTrade.npcOffer,
                turnIndex = activeTrade.turnIndex,
                sourceText = "npc offer updated"
            });
        }
    }

    public void UpdateActiveTradeQuantity(int quantityGrams)
    {
        if (activeTrade == null || activeTrade.AcceptedTerms != null)
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

        bool accepting = string.Equals(action, "ACCEPT", StringComparison.OrdinalIgnoreCase);
        if (activeTrade == null || activeTrade.SettlementClaimed ||
            (accepting && !CanSettleAcceptedTrade(activeTrade.AcceptedTerms)) ||
            (!accepting && activeTrade.AcceptedTerms != null))
        {
            return new LocalTradeOutcome
            {
                currentMoney = playerState.CurrentVarahas,
                currentReputation = playerState.CurrentReputation,
                isSuccess = false,
                reputationDelta = 0
            };
        }

        LocalTradeState settlingTrade = activeTrade;
        TradeTermsSnapshot accepted = settlingTrade.AcceptedTerms;
        if (accepted != null && string.Equals(action, "ACCEPT", StringComparison.OrdinalIgnoreCase))
        {
            finalPrice = accepted.Price;
            finalQuantityGrams = accepted.QuantityGrams;
        }

        // Claim before reward/save or external callbacks can re-enter settlement.
        settlingTrade.SettlementClaimed = true;
        activeTrade = null;
        LocalTradeOutcome outcome = transactionManager.ApplyTrade(
            playerState,
            profile,
            marketManager,
            accepted != null ? accepted.SpiceKey : settlingTrade.spiceKey,
            finalPrice,
            finalQuantityGrams,
            trust,
            frustration,
            outOfWorldCount,
            action,
            accepted != null ? accepted.MarketValue : settlingTrade.marketValue,
            accepted != null ? accepted.BuyerName : settlingTrade.buyerName,
            accepted != null ? accepted.BuyerOrigin : settlingTrade.buyerOrigin,
            accepted?.QuantityLabel
        );

        if (string.Equals(action, "ACCEPT", System.StringComparison.OrdinalIgnoreCase) && finalPrice > 0)
        {
            lastDealReferencePrice = finalPrice;
        }

        SaveProfileToDisk();
        return outcome;
    }

    public LocalTradeOutcome ResolveAcceptedTrade(TradeTermsSnapshot agreement, float trust, float frustration, int outOfWorldCount)
    {
        if (agreement == null || !ReferenceEquals(agreement.Source, activeTrade) ||
            !ReferenceEquals(activeTrade?.AcceptedTerms, agreement))
            return new LocalTradeOutcome { currentMoney = CurrentMoney, currentReputation = CurrentReputation };
        return ResolveTradeFromBackend("ACCEPT", agreement.Price, agreement.QuantityGrams, trust, frustration, outOfWorldCount);
    }

    public bool CanSettleAcceptedTrade(TradeTermsSnapshot agreement)
    {
        if (agreement == null || !ReferenceEquals(agreement.Source, activeTrade) ||
            !ReferenceEquals(activeTrade?.AcceptedTerms, agreement) || activeTrade.SettlementClaimed)
            return false;
        if (OrderManager.Instance != null && OrderManager.Instance.CanCompleteMarketplaceFulfillment(agreement))
            return true;
        // Preserve the existing hardware-free Editor soak. Runtime release always needs delivery.
        #if UNITY_EDITOR
        return OrderManager.Instance == null || Level1DebugForceAccept.ShouldBypassScoopFulfillment();
        #elif DEVELOPMENT_BUILD
        return Level1DebugForceAccept.ShouldBypassScoopFulfillment();
        #else
        return false;
        #endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProfileToDisk();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveProfileToDisk();
        }
    }

    private void RefreshProgressionFieldsFromDisk()
    {
        if (profile == null || localSaveManager == null)
        {
            return;
        }

        LocalProfileData diskProfile = localSaveManager.LoadProfile(marketManager);
        if (diskProfile == null)
        {
            return;
        }

        profile.current_scene = diskProfile.current_scene;
        profile.progression_index = diskProfile.progression_index;
        profile.intro_completed = diskProfile.intro_completed;
    }

    private void ApplyLoadedMarketDayStateWithoutStartingRoutine()
    {
        float restoredRemainingSeconds = profile != null && profile.hasSavedMarketDayTimer
            ? Mathf.Clamp(profile.remainingMarketDaySeconds, 0f, MarketDayDurationSeconds)
            : MarketDayDurationSeconds;

        marketDayRemainingSeconds = restoredRemainingSeconds;
        marketDayStartedAt = -1f;
        timerAutosaveElapsed = 0f;
        lastLoggedRemainingWholeSecond = Mathf.CeilToInt(restoredRemainingSeconds);
        MarketDayStarted = false;
        MarketDayEnded = profile != null && profile.hasSavedMarketDayTimer && restoredRemainingSeconds <= 0f;
    }

    private void ApplyMarketDayState(float restoredRemainingSeconds, bool broadcastState)
    {
        float clampedRemainingSeconds = Mathf.Clamp(restoredRemainingSeconds, 0f, MarketDayDurationSeconds);
        marketDayRemainingSeconds = clampedRemainingSeconds;
        timerAutosaveElapsed = 0f;
        lastLoggedRemainingWholeSecond = Mathf.CeilToInt(clampedRemainingSeconds);

        if (clampedRemainingSeconds <= 0f)
        {
            MarketDayStarted = false;
            MarketDayEnded = true;
            marketDayStartedAt = -1f;
            PersistMarketDayProgress();

            if (broadcastState)
            {
                MarketDayTickEvent?.Invoke(marketDayRemainingSeconds);
                MarketDayEndedEvent?.Invoke();
            }

            Debug.Log("[MARKET DAY] Restored market day in ended state.");
            return;
        }

        MarketDayStarted = true;
        MarketDayEnded = false;
        marketDayStartedAt = Time.time - (MarketDayDurationSeconds - clampedRemainingSeconds);

        Debug.Log($"[MARKET DAY] Market day started. Duration={FormatMarketDayTime(Mathf.CeilToInt(clampedRemainingSeconds))} ({Mathf.CeilToInt(clampedRemainingSeconds)}s)");

        if (broadcastState)
        {
            MarketDayStartedEvent?.Invoke();
            MarketDayTickEvent?.Invoke(marketDayRemainingSeconds);
        }

        marketDayTimerCoroutine = StartCoroutine(MarketDayTimerRoutine());
    }

    private void SyncMarketDayTimerToProfile()
    {
        if (profile == null)
        {
            return;
        }

        profile.remainingMarketDaySeconds = Mathf.Clamp(marketDayRemainingSeconds, 0f, MarketDayDurationSeconds);
        profile.hasSavedMarketDayTimer = true;
    }

    private void PersistMarketDayProgress()
    {
        if (!initialized || localSaveManager == null || profile == null)
        {
            return;
        }

        SaveProfileToDisk();
    }

    private void StopMarketDayTimerRoutine()
    {
        if (marketDayTimerCoroutine == null)
        {
            return;
        }

        StopCoroutine(marketDayTimerCoroutine);
        marketDayTimerCoroutine = null;
    }
}
