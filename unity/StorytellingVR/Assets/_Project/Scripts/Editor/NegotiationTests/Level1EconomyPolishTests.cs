using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Level1EconomyPolishTests
{
    private readonly MarketManager market = new MarketManager();

    [TestCase("pepper", 2, 3, 5, 6, 12, 14, 24, 29)]
    [TestCase("cardamom", 4, 5, 7, 11, 18, 27, 36, 54)]
    [TestCase("cinnamon", 4, 5, 8, 10, 20, 26, 40, 52)]
    public void CostsAndReferencesRoundOnlyAtTheOrderTotal(string spice, int b1, int m1, int b2, int m2,
        int b3, int m3, int b4, int m4)
    {
        int[] grams = { 280, 560, 1400, 2800 };
        int[] costs = { b1, b2, b3, b4 };
        int[] references = { m1, m2, m3, m4 };
        for (int i = 0; i < grams.Length; i++)
        {
            Assert.AreEqual(costs[i], market.CalculateBaseValue(spice, grams[i]));
            Assert.AreEqual(references[i], market.CalculateMarketValue(spice, grams[i], null));
        }
    }

    [TestCase("pepper", 1.35f, 19)]
    [TestCase("cinnamon", 1.4f, 36)]
    [TestCase("cardamom", 1.25f, 34)]
    [TestCase("cardamom", 1.2f, 32)]
    public void EventAppliesToUnroundedReferenceForItsOwnSpice(string spice, float modifier, int expected)
    {
        var marketEvent = new MarketEventData { affected_spice = spice, price_multiplier = modifier };
        Assert.AreEqual(expected, market.CalculateMarketValue(spice, 1400, marketEvent));
        const string other = "clove";
        Assert.AreEqual(market.CalculateMarketValue(other, 1400, null), market.CalculateMarketValue(other, 1400, marketEvent));
    }

    [TestCase("pepper", 175, 2)]
    [TestCase("cinnamon", 105, 2)]
    [TestCase("cinnamon", 175, 2)]
    public void TinyLotsRoundExactHalfVarahasWithoutPerKiloRounding(string spice, int grams, int cost)
    {
        Assert.AreEqual(cost, market.CalculateBaseValue(spice, grams));
    }

    [TestCase(0, 0)]
    [TestCase(34, 0)]
    [TestCase(35, 35)]
    [TestCase(78, 70)]
    [TestCase(279, 210)]
    [TestCase(280, 280)]
    public void DepletedStockNeverProducesAnOversizedOrInactiveOrder(int onHand, int expected)
    {
        var profile = new LocalProfileData
        {
            inventory = new List<InventoryEntry> {
                new InventoryEntry { spiceKey = "pepper", grams = onHand },
                new InventoryEntry { spiceKey = "turmeric", grams = 8000 },
                new InventoryEntry { spiceKey = "clove", grams = 8000 }
            }
        };
        var session = new LocalTradeSessionGenerator().Generate(market, profile, null, "francisco_de_almeida");
        Assert.AreEqual(expected, session?.quantityGrams ?? 0);
        if (expected > 0) Assert.AreEqual("Pepper", session.spiceName);
    }

    [Test]
    public void AOneVarahaPalamCanSellAndASmallCounterCanAdvance()
    {
        var brain = new RuleBasedNPCBrain();
        var tiny = Trade(35, 1, 1, 210);
        var sale = brain.GenerateReply("", new NegotiationInput
        {
            intent = NegotiationIntent.PRICE, hasSellerPrice = true, sellerPrice = 1, parseConfidence = ParseConfidence.High
        }, tiny, 1, 5);
        Assert.IsTrue(sale.isAccepted, sale.replyText);

        var small = Trade(560, 5, 8, 830);
        var bargain = brain.GenerateReply("", new NegotiationInput { intent = NegotiationIntent.BARGAIN }, small, 1, 5);
        Assert.That(bargain.updatedOffer, Is.GreaterThan(5).And.LessThanOrEqualTo(8));
    }

    [Test]
    public void QuantityBeyondStockLeavesTheOpenTermsUntouched()
    {
        var trade = Trade(280, 3, 4, 500);
        var result = new RuleBasedNPCBrain().GenerateReply("", new NegotiationInput
        {
            intent = NegotiationIntent.QUANTITY_CHANGE, hasQuantity = true, quantityGrams = 560,
            parseConfidence = ParseConfidence.High
        }, trade, 1, 5);
        trade.ApplyDecision(result);
        Assert.IsTrue(result.requiresClarification);
        Assert.That(trade.quantityGrams, Is.EqualTo(280));
        Assert.That(trade.npcOffer, Is.EqualTo(3));
    }

    [Test]
    public void QuantityChangeRequotesUsingTheOriginalBuyerMultipliers()
    {
        var trade = Trade(35, 1, 1, 1400);
        trade.startingNpcOffer = 1;
        trade.openingOfferMultiplier = .9f;
        trade.buyerCeilingMultiplier = 1.3f;
        var brain = new RuleBasedNPCBrain();
        var change = new NegotiationInput
        {
            intent = NegotiationIntent.QUANTITY_CHANGE, hasQuantity = true, quantityGrams = 1400,
            parseConfidence = ParseConfidence.High
        };
        var quote = brain.GenerateReply("", change, trade, 1, 5);
        Assert.AreEqual(1, trade.npcOffer);
        Assert.AreEqual(1, trade.maxBuyerPrice);
        trade.ApplyDecision(quote);
        Assert.AreEqual(13, trade.npcOffer);
        Assert.AreEqual(19, trade.maxBuyerPrice);
        Assert.AreEqual(14, trade.marketValue);

        change.intent = NegotiationIntent.QUANTITY_PRICE;
        change.quantityGrams = 35;
        change.hasSellerPrice = true;
        change.sellerPrice = 13;
        var overscale = brain.GenerateReply("", change, trade, 1, 5);
        Assert.IsFalse(overscale.isAccepted);
        Assert.LessOrEqual(overscale.updatedOffer, 1);
        Assert.AreEqual(13, trade.npcOffer);
    }

    [TestCase(17, -1)]
    [TestCase(18, 0)]
    [TestCase(25, 7)]
    public void SettlementCreditsGrossAndKeepsSignedMarginSeparate(int price, int margin)
    {
        var profile = new LocalProfileData { inventory = market.CreateDefaultInventoryEntries() };
        var player = new PlayerState();
        int before = player.CurrentVarahas;
        var settlement = new TransactionManager();
        var sale = settlement.ApplyTrade(player, profile, market, "cardamom", price, 1400, .5f, 0, 0, "ACCEPT", 27, "Rahim", "");
        Assert.IsTrue(sale.isSuccess);
        Assert.AreEqual(before + price, player.CurrentVarahas);
        Assert.AreEqual(margin, sale.transaction.signed_margin);
        Assert.AreEqual(Math.Max(0, margin), sale.transaction.profit);
        Assert.AreEqual(2600, market.AvailableStock(profile.inventory, "cardamom"));
        var oversale = settlement.ApplyTrade(player, profile, market, "cardamom", price, 2800, .5f, 0, 0, "ACCEPT", 27, "Rahim", "");
        Assert.IsFalse(oversale.isSuccess);
        Assert.AreEqual(before + price, player.CurrentVarahas);
        Assert.AreEqual(2600, market.AvailableStock(profile.inventory, "cardamom"));
    }

    [TestCase(17, 65)]
    [TestCase(18, 70)]
    [TestCase(25, 80)]
    public void PracticeCreditsOnceAndRewardsProfitableFairBargainingMost(int price, int respect)
    {
        var host = new GameObject("Practice economy test");
        host.SetActive(false);
        try
        {
            var tutorial = host.AddComponent<TutorialManager>();
            var type = typeof(TutorialManager);
            type.GetField("respect", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(tutorial, 60);
            var pay = type.GetMethod("CompletePracticeSale", BindingFlags.Instance | BindingFlags.NonPublic);
            pay.Invoke(tutorial, new object[] { price });
            pay.Invoke(tutorial, new object[] { price });
            Assert.AreEqual(price, type.GetField("coins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tutorial));
            Assert.AreEqual(respect, type.GetField("respect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tutorial));
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [TestCase("NewSpiceScene")]
    [TestCase("NewTransactionTutorial")]
    [TestCase("Level1_MainLoopUpdated")]
    public void ScenesHaveCostGuideAndTradeFieldsWhereRequired(string sceneName)
    {
        var scene = EditorSceneManager.OpenPreviewScene(
            $"Assets/_Project/Scenes/CompleteFlowWithFinalScenes/{sceneName}.unity");
        try
        {
            var roots = scene.GetRootGameObjects();
            SpiceCostGuideController guide = null;
            Level1HUDManager hud = null;
            foreach (var root in roots)
            {
                guide = guide ?? root.GetComponentInChildren<SpiceCostGuideController>(true);
                foreach (var candidate in root.GetComponentsInChildren<Level1HUDManager>(true))
                    if (candidate.currentTradePanel != null) hud = candidate;
            }
            Assert.IsNotNull(guide);
            Assert.IsTrue(guide.gameObject.activeInHierarchy);
            var serialized = new SerializedObject(guide);
            var view = (CanvasGroup)serialized.FindProperty("guideCanvas").objectReferenceValue;
            Assert.IsNotNull(view);
            foreach (string price in new[] { "pepperPrice", "cardamomPrice", "cinnamonPrice" })
                Assert.IsInstanceOf<TMP_Text>(serialized.FindProperty(price).objectReferenceValue);
            var toggle = typeof(SpiceCostGuideController).GetMethod("HandleToggleInput", BindingFlags.NonPublic | BindingFlags.Instance);
            guide.UnlockGuide();
            toggle.Invoke(guide, new object[] { true, true });
            toggle.Invoke(guide, new object[] { true, false });
            Assert.IsFalse(guide.IsOpen, "Holding X through a pause must not open the guide.");
            toggle.Invoke(guide, new object[] { false, false });
            toggle.Invoke(guide, new object[] { true, false });
            toggle.Invoke(guide, new object[] { true, false });
            Assert.IsTrue(guide.IsOpen);
            toggle.Invoke(guide, new object[] { false, false });
            toggle.Invoke(guide, new object[] { true, false });
            Assert.IsFalse(guide.IsOpen);
            Assert.AreEqual(0f, view.alpha);
            if (sceneName != "NewSpiceScene")
            {
                Assert.IsNotNull(hud);
                Assert.IsNotNull(hud.tradeBuyerText);
                Assert.IsNotNull(hud.tradeQuantityText);
                hud.SetTutorialTrade("Rahim", "Cardamom", "1 Veesai", 18, 27, 17, true, false);
                Assert.IsTrue(hud.currentTradePanel.activeSelf);
                Assert.AreEqual("Rahim", hud.tradeBuyerText.text);
                Assert.AreEqual("1 Veesai", hud.tradeQuantityText.text);
                var details = (TMP_Text)new SerializedObject(hud).FindProperty("tradeMarketValueText").objectReferenceValue;
                StringAssert.Contains("Margin: -1 Varahas", details.text);
                hud.SetTutorialTrade("Rahim", "Cardamom", "1 Veesai", 18, 27, 18, false, false);
                Assert.IsFalse(hud.currentTradePanel.activeSelf);
                StringAssert.Contains("Margin: 0 Varahas", details.text);
            }
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }

    private LocalTradeState Trade(int grams, int offer, int ceiling, int stock) => new LocalTradeState
    {
        spiceKey = "pepper", spiceDisplayName = "Pepper", quantityGrams = grams,
        quantityLabel = market.FormatTraditionalQuantity(grams),
        marketValue = market.CalculateMarketValue("pepper", grams, null),
        preciseMarketValue = market.CalculateMarketValueExact("pepper", grams, null),
        availableStockGrams = stock, npcOffer = offer, maxBuyerPrice = ceiling, minIncrement = 1,
        buyerPatience = 5, buyerPersonality = "Neutral", negotiationInitialized = true
    };
}
