using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Level1TransactionIntegrityTests
{
    private Level1GameState state;
    private GameObject host;
    private string saveDirectory;
    private string previousSaveOverride;
    private OrderManager ownedOrder;
    private static readonly FieldInfo InstanceField = typeof(Level1GameState).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
    private LocalProfileData Profile => (LocalProfileData)typeof(Level1GameState)
        .GetField("profile", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(state);

    [SetUp]
    public void SetUp()
    {
        Assert.IsFalse(Level1GameState.ExistingInstance != null, "Run outside an active game session; this fixture must not touch a live player's state.");
        previousSaveOverride = LocalSaveManager.EditorTestSaveDirectoryOverride;
        saveDirectory = Path.Combine(Path.GetTempPath(), "Level1Pass3_" + Guid.NewGuid().ToString("N"));
        LocalSaveManager.EditorTestSaveDirectoryOverride = saveDirectory;
        host = new GameObject("Pass3 transaction fixture");
        host.SetActive(false); // No Start, timers, scene loading or customer coroutines.
        state = host.AddComponent<Level1GameState>();
        InstanceField.SetValue(null, state);
        state.EnsureInitialized();
    }

    [TearDown]
    public void TearDown()
    {
        if (host != null)
        {
            UnityEngine.Object.DestroyImmediate(host);
            if (ownedOrder != null || ReferenceEquals(OrderManager.Instance, ownedOrder)) OrderManager.Instance = null;
            InstanceField.SetValue(null, null);
            LocalSaveManager.EditorTestSaveDirectoryOverride = previousSaveOverride;
        }
        if (saveDirectory != null && Directory.Exists(saveDirectory)) Directory.Delete(saveDirectory, true);
    }

    private TradeTermsSnapshot AcceptCurrent(int grams = 560)
    {
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "2 Seers (~560g)", grams, 173, null);
        LocalTradeState trade = state.ActiveTrade;
        var manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "fulfillment test");
        RuleBasedNPCBrainResult decision = manager.ProcessLocalTurn(manager.ClassifyInput("I accept 173 varahas", trade), trade, new RuleBasedNPCBrain());
        Assert.IsTrue(decision.isAccepted);
        Assert.IsNotNull(trade.AcceptedTerms);
        return trade.AcceptedTerms;
    }

    private OrderManager PhysicalOrder(TradeTermsSnapshot agreement)
    {
        Assert.IsNull(OrderManager.Instance, "Do not replace a live scene's order manager.");
        Assert.IsFalse(Level1DebugForceAccept.ShouldBypassScoopFulfillment(), "Disable fulfillment debug bypass for integrity tests.");
        var orderHost = new GameObject("Pass3 physical order");
        orderHost.transform.SetParent(host.transform);
        var order = orderHost.AddComponent<OrderManager>();
        OrderManager.Instance = order;
        ownedOrder = order;
        order.tutorialMode = false;
        typeof(OrderManager).GetField("marketplaceOrder", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(order, new MarketplaceFulfillmentOrder(agreement));
        return order;
    }

    private ChatManager PendingChat(TradeTermsSnapshot agreement)
    {
        var chatHost = new GameObject("Pass3 fulfillment chat");
        chatHost.transform.SetParent(host.transform);
        var chat = chatHost.AddComponent<ChatManager>();
        typeof(ChatManager).GetField("pendingFulfillment", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(chat, new ChatManager.PendingFulfillmentData { sourceTrade = agreement.Source,
                acceptedTerms = agreement, agreedPrice = agreement.Price, quantityGrams = agreement.QuantityGrams,
                buyerTrust = 0.7f, buyerFrustration = 0.1f });
        return chat;
    }

    [Test]
    public void ProductionCompletionRejectsEmptyWrongStaleAndRepeatedDelivery()
    {
        TradeTermsSnapshot agreement = AcceptCurrent();
        OrderManager order = PhysicalOrder(agreement);
        ChatManager chat = PendingChat(agreement);
        int money = state.CurrentMoney;
        order.requestedSpice = SpiceType.Cardamom; // A stale/debug field cannot replace accepted spice identity.
        order.SetRequestedSpice(SpiceType.Cinnamon);
        Assert.AreEqual(SpiceType.Pepper, order.ExpectedSpice);
        Assert.IsFalse(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Cardamom, true));
        Assert.IsFalse(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Pepper, false));
        Assert.IsFalse(state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0).isSuccess);
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.IsTrue(chat.HasPendingFulfillment);
        Assert.AreEqual(money, state.CurrentMoney);
        Assert.IsTrue(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Pepper, true));
        Assert.IsTrue(order.BeginMarketplaceFulfillment(agreement), "Repeat start notification is idempotent.");
        Assert.IsTrue(order.CanCompleteMarketplaceFulfillment(agreement));
        Assert.AreEqual(560, order.MarketplaceOrder.Agreement.QuantityGrams);
        Assert.IsFalse(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Pepper, true));
        chat.CompleteAcceptedFulfillment(agreement);
        int respect = state.CurrentReputation;
        Assert.AreEqual(money + 173, state.CurrentMoney);
        Assert.IsFalse(chat.HasPendingFulfillment);
        Assert.IsFalse(order.IsMarketplaceFulfillmentActive);
        Assert.IsNull(state.ActiveTrade);
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.AreEqual(money + 173, state.CurrentMoney);
        Assert.AreEqual(respect, state.CurrentReputation);
        Assert.AreEqual(1, Profile.shift_stats.total_deals_made);

        TradeTermsSnapshot next = AcceptCurrent();
        typeof(OrderManager).GetField("marketplaceOrder", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(order, new MarketplaceFulfillmentOrder(next));
        typeof(ChatManager).GetField("pendingFulfillment", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(chat, new ChatManager.PendingFulfillmentData { sourceTrade = next.Source, acceptedTerms = next });
        Assert.IsFalse(order.BeginMarketplaceFulfillment(agreement));
        Assert.IsFalse(order.TryDeliverMarketplaceScoop(agreement, SpiceType.Pepper, true));
        chat.CompleteAcceptedFulfillment(agreement);
        Assert.AreSame(next, order.MarketplaceOrder.Agreement);
        Assert.IsFalse(order.MarketplaceOrder.IsComplete);
        Assert.AreSame(next, chat.CurrentPendingFulfillment.acceptedTerms);
        Assert.AreEqual(money + 173, state.CurrentMoney);
        // The same production cleanup called by UI reset and component disable, without running voice/UI hooks.
        typeof(ChatManager).GetMethod("ResetFulfillment", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(chat, null);
        Assert.IsFalse(chat.HasPendingFulfillment);
        Assert.IsFalse(order.IsMarketplaceFulfillmentActive);
        Assert.AreEqual(SpiceType.None, order.requestedSpice);
    }

    [Test]
    public void PreviousCustomerAnimationEventCannotReopenSharedBag()
    {
        TradeTermsSnapshot old = AcceptCurrent();
        OrderManager order = PhysicalOrder(old);
        var actorHost = new GameObject("Pass3 old handoff actor");
        actorHost.transform.SetParent(host.transform);
        var handoff = actorHost.AddComponent<HandBagAnimation>();
        var bag = new GameObject("Pass3 shared bag");
        bag.transform.SetParent(host.transform);
        handoff.handBag = bag;
        handoff.BindMarketplaceAgreement(old);
        state.PrepareForNewCustomer();
        TradeTermsSnapshot next = AcceptCurrent();
        typeof(OrderManager).GetField("marketplaceOrder", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(order, new MarketplaceFulfillmentOrder(next));
        bag.SetActive(false);
        handoff.FreezeHand(); // The real animation event previously activated the global bag unconditionally.
        handoff.ReceiveBag();
        Assert.IsFalse(bag.activeSelf);
        Assert.AreSame(next, order.MarketplaceOrder.Agreement);
        Assert.IsFalse(order.MarketplaceOrder.IsComplete);
    }

    [Test]
    public void FrozenAgreementSettlesOnceAndSavesMoneyRespectStatsAndInventoryTogether()
    {
        TradeTermsSnapshot agreement = AcceptCurrent();
        var order = new MarketplaceFulfillmentOrder(agreement);
        Assert.IsTrue(order.TryDeliver(state.ActiveTrade, agreement, SpiceType.Pepper, true));
        Assert.IsTrue(order.CanComplete(state.ActiveTrade, agreement));
        int money = state.CurrentMoney, respect = state.CurrentReputation;
        int stock = Profile.inventory.Find(entry => entry.spiceKey == "pepper").grams;
        state.ActiveTrade.npcOffer = 999;
        state.ActiveTrade.quantityGrams = 1;
        state.ActiveTrade.spiceKey = "cardamom";
        state.ActiveTrade.buyerName = "changed";
        int saves = 0;
        Application.LogCallback countSaves = (message, stack, type) => { if (message.StartsWith("[SAVE]")) saves++; };
        Application.logMessageReceived += countSaves;
        LocalTradeOutcome result;
        try
        {
            result = state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0);
            state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0);
            state.ResolveTradeFromBackend("WALK_AWAY", 0, 0, 0, 1, 0);
        }
        finally { Application.logMessageReceived -= countSaves; }
        Assert.IsTrue(result.isSuccess);
        Assert.IsTrue(agreement.Source.SettlementClaimed);
        Assert.IsNull(state.ActiveTrade);
        Assert.AreEqual(money + 173, state.CurrentMoney);
        Assert.AreEqual(respect + result.reputationDelta, state.CurrentReputation);
        Assert.AreEqual("Pepper", result.transaction.item);
        Assert.AreEqual("Lakshmi Amma", result.transaction.buyer_name);
        Assert.AreEqual("2 Seers (~560g)", result.transaction.quantity);
        Assert.AreEqual(1, saves, "A duplicate must not initiate another settlement save.");
        LocalProfileData disk = new LocalSaveManager().LoadProfile(new MarketManager());
        Assert.AreEqual(state.CurrentMoney, disk.global_metrics.total_varahas);
        Assert.AreEqual(state.CurrentReputation, disk.global_metrics.reputation);
        Assert.AreEqual(1, disk.shift_stats.total_deals_made);
        Assert.AreEqual(173, disk.shift_stats.total_varahas_earned);
        Assert.AreEqual(stock - 560, disk.inventory.Find(entry => entry.spiceKey == "pepper").grams);
    }

    [Test]
    public void AcceptedSessionCannotAlsoResolveAsNoDeal()
    {
        TradeTermsSnapshot agreement = AcceptCurrent();
        int money = state.CurrentMoney, respect = state.CurrentReputation;
        LocalTradeOutcome noDeal = state.ResolveTradeFromBackend("WALK_AWAY", 0, 0, 0, 1, 0);
        Assert.IsFalse(noDeal.isSuccess);
        Assert.AreSame(agreement.Source, state.ActiveTrade);
        Assert.AreEqual(money, state.CurrentMoney);
        Assert.AreEqual(respect, state.CurrentReputation);
        Assert.IsTrue(state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0).isSuccess);
        Assert.AreEqual(1, Profile.shift_stats.total_deals_made);
    }

    [Test]
    public void PreviousCustomerCompletionCannotSettleNextCustomer()
    {
        TradeTermsSnapshot old = AcceptCurrent();
        state.PrepareForNewCustomer();
        Assert.IsNull(state.ActiveTrade);
        TradeTermsSnapshot next = AcceptCurrent();
        int money = state.CurrentMoney, respect = state.CurrentReputation;
        Assert.IsFalse(state.ResolveAcceptedTrade(old, 0.7f, 0.1f, 0).isSuccess);
        Assert.AreSame(next.Source, state.ActiveTrade);
        Assert.AreEqual(money, state.CurrentMoney);
        Assert.AreEqual(respect, state.CurrentReputation);
        Assert.IsTrue(state.ResolveAcceptedTrade(next, 0.7f, 0.1f, 0).isSuccess);
        Assert.AreEqual(1, Profile.shift_stats.total_deals_made);
    }

    [Test]
    public void NoAcceptedAgreementCannotAwardTradeAndNoDealPreservesBuyerName()
    {
        state.SyncTradeFromBackend("Lakshmi Amma", "Hampi", "Pepper", "1 Seer (~280g)", 280, 173, null);
        int money = state.CurrentMoney;
        Assert.IsFalse(state.ResolveTradeFromBackend("ACCEPT", 173, 280, 0.7f, 0.1f, 0).isSuccess);
        Assert.AreEqual(money, state.CurrentMoney);
        Assert.IsNotNull(state.ActiveTrade);
        LocalTradeOutcome result = state.ResolveTradeFromBackend("WALK_AWAY", 0, 0, 0, 1, 0);
        Assert.AreEqual("Lakshmi Amma", result.transaction.buyer_name);
        int respect = state.CurrentReputation;
        state.ResolveTradeFromBackend("WALK_AWAY", 0, 0, 0, 1, 0);
        Assert.AreEqual(respect, state.CurrentReputation);
        Assert.AreEqual(0, Profile.shift_stats.total_deals_made);
    }

    [Test]
    public void FailedSaveDoesNotReopenSettlementOrDuplicateRewards()
    {
        TradeTermsSnapshot agreement = AcceptCurrent();
        Directory.CreateDirectory(state.ActiveSavePath + ".tmp"); // File.WriteAllText must fail; no save API substitution.
        LogAssert.Expect(LogType.Error, new Regex(@"\[LocalSaveManager\] Failed to save profile:"));
        int money = state.CurrentMoney;
        Assert.IsTrue(state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0).isSuccess);
        int respect = state.CurrentReputation;
        Assert.IsFalse(state.ResolveAcceptedTrade(agreement, 0.7f, 0.1f, 0).isSuccess);
        Assert.IsNull(state.ActiveTrade);
        Assert.AreEqual(money + 173, state.CurrentMoney);
        Assert.AreEqual(respect, state.CurrentReputation);
        Assert.AreEqual(1, Profile.shift_stats.total_deals_made);
        Assert.AreEqual(173, Profile.shift_stats.total_varahas_earned);
    }
}
