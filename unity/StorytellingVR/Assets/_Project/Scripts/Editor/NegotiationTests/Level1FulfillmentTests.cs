using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using TMPro;

public class Level1FulfillmentTests
{
    private static LocalTradeState AcceptedTrade(int grams = 560)
    {
        var trade = new LocalTradeState { spiceKey = "pepper", spiceDisplayName = "Pepper",
            quantityGrams = grams, quantityLabel = new MarketManager().FormatTraditionalQuantity(grams),
            npcOffer = 173, marketValue = 100, buyerName = "Lakshmi Amma", buyerOrigin = "Hampi" };
        Assert.IsTrue(trade.BindAcceptedTerms(TradeTermsSnapshot.Capture(trade)));
        return trade;
    }

    [Test]
    public void EveryGeneratedSpiceHasPhysicalZonesAndScoopAndBagVisuals()
    {
        string scene = File.ReadAllText(Path.Combine(Application.dataPath,
            "_Project/Scenes/CompleteFlowWithFinalScenes/Level1_MainLoopUpdated.unity"));
        string[] objects = Regex.Split(scene, @"(?m)^--- !u!");
        string zoneMeta = File.ReadAllText(Path.Combine(Application.dataPath, "_Project/Scripts/SpiceInteraction/SpiceZone.cs.meta"));
        string zoneGuid = Regex.Match(zoneMeta, @"guid: (\w+)").Groups[1].Value;
        var market = new MarketManager();
        foreach (string key in LocalTradeSessionGenerator.PhysicalTradeSpiceKeys)
        {
            Assert.IsTrue(market.TryGetSpice(key, out SpiceData spice), key);
            SpiceType physical = OrderManager.MapSpiceName(spice.displayName);
            Assert.AreNotEqual(SpiceType.None, physical, key);
            Assert.IsTrue(objects.Any(obj => obj.Contains("guid: " + zoneGuid) &&
                Regex.IsMatch(obj, @"(?m)^  spiceType: " + (int)physical + @"\r?$")), "No physical zone: " + key);
            foreach (string component in new[] { "ScooperFill", "HandBagAnimation" })
            {
                var visuals = objects.Where(obj => obj.Contains("Assembly-CSharp::" + component)).ToArray();
                Assert.IsNotEmpty(visuals, component);
                Assert.IsTrue(visuals.All(obj => Regex.IsMatch(obj,
                    @"spiceType: " + (int)physical + @"\r?\n\s+visual: \{fileID: [1-9]\d*\}")), component + ": " + key);
            }
        }
        CollectionAssert.DoesNotContain(LocalTradeSessionGenerator.PhysicalTradeSpiceKeys, "clove");
        Assert.AreEqual(SpiceType.None, OrderManager.MapSpiceName("Clove"));
    }

    [Test]
    public void GeneratedCustomersAreFulfillableAndDoNotImmediatelyRepeat()
    {
        UnityEngine.Random.State previousRandom = UnityEngine.Random.state;
        try
        {
            UnityEngine.Random.InitState(33003);
            var market = new MarketManager();
            var profile = new LocalProfileData { inventory = market.CreateDefaultInventoryEntries() };
            var generator = new LocalTradeSessionGenerator();
            string previous = null;
            for (int index = 0; index < 300; index++)
            {
                LocalGeneratedTradeSession session = generator.Generate(market, profile, null);
                Assert.AreNotEqual(SpiceType.None, OrderManager.MapSpiceName(session.spiceName), "customer " + index);
                Assert.AreNotEqual(previous, session.characterId, "customer " + index);
                Assert.Greater(session.quantityGrams, 0);
                previous = session.characterId;
            }
            Assert.AreEqual("abdul_rahman", generator.Generate(market, profile, null, "abdul_rahman").characterId);
            Assert.AreEqual("abdul_rahman", generator.Generate(market, profile, null, "abdul_rahman").characterId,
                "Explicit Editor character selection is preserved.");
        }
        finally { UnityEngine.Random.state = previousRandom; }
    }

    [Test]
    public void DeliveryUsesFrozenItemQuantityPriceUnitAndCustomer()
    {
        LocalTradeState trade = AcceptedTrade();
        TradeTermsSnapshot agreement = trade.AcceptedTerms;
        var order = new MarketplaceFulfillmentOrder(agreement);
        trade.spiceKey = "cardamom";
        trade.spiceDisplayName = "Cardamom";
        trade.quantityGrams = 1;
        trade.quantityLabel = "changed";
        trade.npcOffer = 999;
        trade.buyerName = "someone else";
        Assert.IsTrue(order.TryDeliver(trade, agreement, SpiceType.Pepper, true));
        Assert.IsTrue(order.CanComplete(trade, agreement));
        Assert.AreSame(trade, agreement.Source);
        Assert.AreEqual("pepper", agreement.SpiceKey);
        Assert.AreEqual(560, agreement.QuantityGrams);
        Assert.AreEqual("2 Seers (~560g)", agreement.QuantityLabel);
        Assert.AreEqual(173, agreement.Price);
        Assert.AreEqual("Lakshmi Amma", agreement.BuyerName);
    }

    [Test]
    public void WrongSpiceAndEmptyScoopCannotCompleteOrder()
    {
        LocalTradeState trade = AcceptedTrade();
        var order = new MarketplaceFulfillmentOrder(trade.AcceptedTerms);
        Assert.IsFalse(order.TryDeliver(trade, trade.AcceptedTerms, SpiceType.Cardamom, true));
        Assert.IsFalse(order.TryDeliver(trade, trade.AcceptedTerms, SpiceType.None, true));
        Assert.IsFalse(order.TryDeliver(trade, trade.AcceptedTerms, SpiceType.Pepper, false));
        Assert.IsFalse(order.IsComplete);
        Assert.IsFalse(order.CanComplete(trade, trade.AcceptedTerms));
    }

    [TestCase(35)]
    [TestCase(280)]
    [TestCase(350)]
    [TestCase(560)]
    [TestCase(1400)]
    [TestCase(2800)]
    public void CorrectSpiceCompletesAnyAcceptedQuantityWithOneScoop(int grams)
    {
        LocalTradeState trade = AcceptedTrade(grams);
        var order = new MarketplaceFulfillmentOrder(trade.AcceptedTerms);
        Assert.IsTrue(order.TryDeliver(trade, trade.AcceptedTerms, SpiceType.Pepper, true));
        Assert.IsTrue(order.CanComplete(trade, trade.AcceptedTerms));
        Assert.AreEqual(grams, order.Agreement.QuantityGrams);
        Assert.IsFalse(order.TryDeliver(trade, trade.AcceptedTerms, SpiceType.Pepper, true));
    }

    [Test]
    public void PreviousCustomerOrderAndUnacceptedSnapshotCannotBeDelivered()
    {
        LocalTradeState oldTrade = AcceptedTrade();
        LocalTradeState nextTrade = AcceptedTrade(); // Same name/terms still constitute a different customer session.
        var oldOrder = new MarketplaceFulfillmentOrder(oldTrade.AcceptedTerms);
        Assert.IsFalse(oldOrder.TryDeliver(nextTrade, oldTrade.AcceptedTerms, SpiceType.Pepper, true));
        Assert.IsFalse(oldOrder.TryDeliver(oldTrade, nextTrade.AcceptedTerms, SpiceType.Pepper, true));
        Assert.IsFalse(oldOrder.TryDeliver(null, oldTrade.AcceptedTerms, SpiceType.Pepper, true));
        Assert.IsFalse(oldOrder.CanComplete(nextTrade, nextTrade.AcceptedTerms));
        var nextOrder = new MarketplaceFulfillmentOrder(nextTrade.AcceptedTerms);
        Assert.IsFalse(nextOrder.IsComplete);
        var unaccepted = new LocalTradeState { spiceKey = "pepper", quantityGrams = 560, npcOffer = 173 };
        TradeTermsSnapshot candidate = TradeTermsSnapshot.Capture(unaccepted);
        var candidateOrder = new MarketplaceFulfillmentOrder(candidate);
        Assert.IsFalse(candidateOrder.TryDeliver(unaccepted, candidate, SpiceType.Pepper, true));
    }

    [Test]
    public void UnsupportedSpiceCannotCreatePhysicalOrder()
    {
        var trade = new LocalTradeState { spiceKey = "clove", quantityGrams = 280, npcOffer = 100 };
        Assert.Throws<ArgumentException>(() => new MarketplaceFulfillmentOrder(TradeTermsSnapshot.Capture(trade)));
    }

    [Test]
    public void ScooperResetClearsContentsAndSelectedSpice()
    {
        var host = new GameObject("Pass3 test scooper");
        host.SetActive(false);
        try
        {
            var scoop = host.AddComponent<ScooperFill>();
            typeof(ScooperFill).GetField("filled", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(scoop, true);
            scoop.currentSpice = SpiceType.Pepper;
            Assert.IsTrue(scoop.IsFilled());
            scoop.ResetScooper();
            Assert.IsFalse(scoop.IsFilled());
            Assert.AreEqual(SpiceType.None, scoop.currentSpice);
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [Test]
    public void PlayerTradeHudUsesBuyerNameAndHidesBuyerMaximum()
    {
        var host = new GameObject("Pass3 HUD fixture");
        host.SetActive(false);
        try
        {
            var hud = host.AddComponent<Level1HUDManager>();
            hud.currentTradePanel = new GameObject("Trade panel");
            hud.currentTradePanel.transform.SetParent(host.transform);
            var buyerText = new GameObject("Buyer", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            buyerText.transform.SetParent(host.transform);
            hud.tradeBuyerText = buyerText.GetComponent<TextMeshProUGUI>();
            var offerText = new GameObject("Offer", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            offerText.transform.SetParent(host.transform);
            typeof(Level1HUDManager).GetField("tradeNPCOfferText", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(hud, offerText.GetComponent<TextMeshProUGUI>());
            hud.ShowCurrentTrade("Pepper", "1 Seer (~280g)", "Lakshmi Amma");
            hud.UpdateCurrentTrade(new CurrentTrade { spice = "Pepper", npc_offer = 173, market_value = 100 });
            StringAssert.Contains("Lakshmi Amma", hud.tradeBuyerText.text);
            Assert.AreEqual("NPC Offer:\n173 Varahas", offerText.GetComponent<TextMeshProUGUI>().text);
            hud.ShowCurrentTrade("Pepper", "1 Seer (~280g)", "");
            StringAssert.Contains("Waiting for customer", hud.tradeBuyerText.text);
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [Test]
    public void Level1LedgerControllerButtonCannotAlsoSkipGameplayScene()
    {
        Assert.IsFalse(GameManager.ControllerSceneSkipAllowedIn(GameManager.DefaultGameplaySceneName));
        Assert.IsTrue(GameManager.ControllerSceneSkipAllowedIn(GameManager.DefaultIntroSceneName));
        Assert.IsTrue(GameManager.ControllerSceneSkipAllowedIn("SpicesInteraction"));
    }
}
