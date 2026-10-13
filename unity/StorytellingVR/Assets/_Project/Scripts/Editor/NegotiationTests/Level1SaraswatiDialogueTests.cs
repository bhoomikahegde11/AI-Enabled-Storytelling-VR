using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

// Exercises the existing classify -> decide -> render path without scenes, audio or save IO.
public class Level1SaraswatiDialogueTests : Level1NegotiationTestBase
{
    private UnityEngine.Random.State randomState;
    [SetUp] public void SetUp() { randomState = UnityEngine.Random.state; UnityEngine.Random.InitState(1522); }
    [TearDown] public void TearDown() { UnityEngine.Random.state = randomState; }

    private static LocalTradeState Trade(int patience = 8, string personality = "Friendly")
    {
        return new LocalTradeState {
            buyerName = "Saraswati Chetti", buyerOrigin = "Local Vijayanagara Retail Shop Owner", buyerPersonality = personality,
            spiceKey = "pepper", spiceDisplayName = "Pepper", quantityGrams = 1000, quantityLabel = "1 kg",
            npcOffer = 100, previousNpcOffer = 100, marketValue = 96, maxBuyerPrice = 180,
            buyerPatience = patience, minIncrement = 2, negotiationInitialized = true,
            buyerTrust = 0.5f, buyerFrustration = 0.1f, buyerDesperation = 0.5f
        };
    }

    private static RuleBasedNPCBrainResult Offer(int updatedOffer = 100)
    {
        return new RuleBasedNPCBrainResult { previousOffer = 100, updatedOffer = updatedOffer, resolvedPrice = 100,
            nextExpectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter, replyText = "Exact domain reply" };
    }

    private static string Fill(string template, LocalTradeState trade, int counter = 100, int seller = 100)
    {
        return template.Replace("{quantityLabel}", trade.AcceptedTerms != null ? trade.AcceptedTerms.QuantityLabel : trade.quantityLabel)
            .Replace("{spiceName}", trade.AcceptedTerms != null ? trade.AcceptedTerms.SpiceName.ToLowerInvariant() : trade.spiceDisplayName.ToLowerInvariant())
            .Replace("{currency}", "varahas").Replace("{currentBuyerOffer}", trade.npcOffer.ToString())
            .Replace("{counterPrice}", counter.ToString()).Replace("{offeredPrice}", seller.ToString())
            .Replace("{finalPrice}", trade.AcceptedTerms != null ? trade.AcceptedTerms.Price.ToString() : counter.ToString());
    }

    private static void AssertCategory(string reply, DialogueScenario scenario, LocalTradeState trade, int counter = 100, int seller = 100)
    {
        var candidates = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == scenario)
            .SelectMany(line => line.templates).Select(template => Fill(template, trade, counter, seller)).ToArray();
        CollectionAssert.Contains(candidates, reply, scenario.ToString());
        StringAssert.DoesNotContain("{", reply);
    }

    [Test]
    public void TableKeepsIdentity_AllScenarios_SupportedPlaceholders_AndDistinctVoice()
    {
        var table = SiddharthChettiDialogue.Create();
        Assert.AreEqual("saraswati_chetti", table.characterId);
        Assert.AreEqual("Saraswati Chetti", table.displayName);
        Assert.AreEqual("Saraswati Chetti", new DialogueCharacterRegistry().FindCharacter("saraswati_chetti").displayName);
        Assert.AreEqual("saraswati_chetti", DialogueCharacterRegistry.NormalizeCharacterId("Saraswati Chetti"));
        var lakshmi = LakshmiAmmaDialogue.Create().lines.SelectMany(line => line.templates).ToArray();
        var chinnamma = ChinappaNaikDialogue.Create().lines.SelectMany(line => line.templates).ToArray();
        string[] supported = { "buyerName", "characterName", "spiceName", "quantityLabel", "offeredPrice", "counterPrice",
            "finalPrice", "currentBuyerOffer", "minimumPrice", "currency", "round", "reputation", "patience", "desperation", "ruleReply" };
        foreach (DialogueScenario scenario in Enum.GetValues(typeof(DialogueScenario)))
            Assert.IsTrue(table.lines.Any(line => line.scenario == scenario && !line.patienceBucket.HasValue &&
                !line.personalityBucket.HasValue && !line.roundBucket.HasValue && !line.reputationBucket.HasValue &&
                !line.desperationBucket.HasValue), scenario.ToString());
        foreach (var line in table.lines)
        foreach (string template in line.templates)
        {
            Assert.AreEqual("saraswati_chetti", line.characterId);
            Assert.IsFalse(string.IsNullOrWhiteSpace(template));
            foreach (Match match in Regex.Matches(template, @"\{([^}]+)\}"))
                CollectionAssert.Contains(supported, match.Groups[1].Value, template);
            if (template != "{ruleReply}") CollectionAssert.DoesNotContain(lakshmi, template);
            if (template != "{ruleReply}") CollectionAssert.DoesNotContain(chinnamma, template);
        }
    }

    [Test]
    public void RegionalExpressionsHaveRestrictedMeaningfulContexts()
    {
        var lines = SiddharthChettiDialogue.Create().lines;
        string all = string.Join(" ", lines.SelectMany(line => line.templates));
        foreach (string expression in new[] { "Namaskaram", "Avunu", "Konchem" }) StringAssert.Contains(expression, all);
        Assert.IsFalse(Regex.IsMatch(all, @"\b(Ayyo|Swalpa|Sari|Howdu|Beda|Saaku)\b"));
        foreach (var line in lines)
        foreach (string template in line.templates)
        {
            if (template.Contains("Namaskaram")) Assert.IsTrue(line.scenario == DialogueScenario.CustomerGreeting ||
                line.scenario == DialogueScenario.SocialGreeting);
            if (template.Contains("Avunu")) Assert.AreEqual(DialogueScenario.SellerPriceAccepted, line.scenario);
            if (template.Contains("Konchem")) Assert.AreEqual(DialogueScenario.UnclearSpeech, line.scenario);
        }
    }

    [TestCase(8, "Friendly")]
    [TestCase(8, "Normal")]
    [TestCase(8, "Strict")]
    [TestCase(8, "Impatient")]
    [TestCase(2, "Strict")]
    [TestCase(1, "Friendly")]
    public void HoldLinesRespectMoodPatience_AndAvoidRecentFour(int patience, string personality)
    {
        var trade = Trade(patience, personality);
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        var low = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.patienceBucket == NpcPatienceBucket.Low).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var bucket = personality == "Friendly" ? NpcPersonalityBucket.Friendly : personality == "Strict" ?
            NpcPersonalityBucket.Strict : personality == "Impatient" ? NpcPersonalityBucket.Impatient : NpcPersonalityBucket.Normal;
        var mood = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.personalityBucket == bucket).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var generic = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var allowed = mood.Concat(generic).ToArray();
        var recent = new Queue<string>();
        for (int i = 0; i < 18; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            if (patience <= 2) CollectionAssert.Contains(low, reply);
            else { CollectionAssert.DoesNotContain(low, reply); if (allowed.Length > 0) CollectionAssert.Contains(allowed, reply); }
            CollectionAssert.DoesNotContain(recent, reply);
            recent.Enqueue(reply);
            if (recent.Count > 4) recent.Dequeue();
            StringAssert.DoesNotContain("raise", reply);
        }
        Assert.AreEqual(patience, trade.buyerPatience);
        Assert.AreEqual(100, trade.npcOffer);
    }

    [Test]
    public void BuyerHoldsFirm_GenericLinesAreReachableAlongsideMoodVariants()
    {
        var trade = Trade(8, "Friendly");
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        var generic = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var mood = SiddharthChettiDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.personalityBucket == NpcPersonalityBucket.Friendly).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();

        bool sawGeneric = false;
        bool sawMood = false;
        for (int i = 0; i < 40; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            if (generic.Contains(reply)) sawGeneric = true;
            if (mood.Contains(reply)) sawMood = true;
        }
        Assert.IsTrue(sawMood, "Mood-specific variants should be selected.");
        Assert.IsTrue(sawGeneric, "Generic variants must be reachable and not shadowed.");
    }

    [Test]
    public void SaraswatiDialogue_HasNoUnimplementedInspectionPhrases()
    {
        var lines = SiddharthChettiDialogue.Create().lines;
        string all = string.Join(" ", lines.SelectMany(line => line.templates));
        Assert.IsFalse(Regex.IsMatch(all, @"\b(?:inspect|examine|assess sample|see the delivery|see your measure|Show me the measure)\b", RegexOptions.IgnoreCase), all);
    }

    [TestCase(1, DialogueScenario.BuyerCounterFirst)]
    [TestCase(3, DialogueScenario.BuyerCounterMiddle)]
    [TestCase(6, DialogueScenario.BuyerCounterFinal)]
    public void IncreasingOfferUsesCounterCategory_WithoutClaimingAnAbsoluteLimit(int round, DialogueScenario scenario)
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.BARGAIN }, trade, Offer(110), round);
            AssertCategory(reply, scenario, trade, 110);
            StringAssert.Contains("110", reply);
            Assert.IsFalse(Regex.IsMatch(reply, @"last offer|final offer|no higher|no further|never"));
        }
    }

    [TestCase(200, DialogueScenario.SellerPriceTooHigh)]
    [TestCase(183, DialogueScenario.SellerPriceSlightlyHigh)]
    public void RejectedHighPriceDoesNotImplyConcessionOrAgreement(int price, DialogueScenario scenario)
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.PRICE,
                hasSellerPrice = true, sellerPrice = price }, trade, Offer(), 3);
            AssertCategory(reply, scenario, trade, 100, price);
            Assert.IsFalse(Regex.IsMatch(reply, @"agreed|raise|bring|received"));
        }
    }

    [TestCase(8)]
    [TestCase(2)]
    public void HighPriceImpatienceAppearsOnlyAtLowPatience(int patience)
    {
        var trade = Trade(patience);
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE, hasSellerPrice = true, sellerPrice = 200 };
        var low = SiddharthChettiDialogue.Create().lines.Where(line =>
            line.scenario == DialogueScenario.SellerPriceTooHigh && line.patienceBucket == NpcPatienceBucket.Low)
            .SelectMany(line => line.templates).Select(template => Fill(template, trade, 100, 200)).ToArray();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            if (patience <= 2) CollectionAssert.Contains(low, reply);
            else CollectionAssert.DoesNotContain(low, reply);
        }
    }

    [Test]
    public void FirstAndRepeatGreetingsAreDifferent_AndDoNotInventPreviousCompletedTrade()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        string first = provider.GetGreeting(trade, 50, false);
        string repeat = provider.GetGreeting(trade, 50, true);
        AssertCategory(first, DialogueScenario.CustomerGreeting, trade);
        AssertCategory(repeat, DialogueScenario.RepeatCustomerGreeting, trade);
        Assert.AreNotEqual(first, repeat);
        foreach (string template in SiddharthChettiDialogue.Create().lines.Single(line =>
            line.scenario == DialogueScenario.RepeatCustomerGreeting).templates)
            Assert.IsFalse(Regex.IsMatch(template, @"dealt before|bought before|last purchase|last bargain"));
        UnityEngine.Random.InitState(42);
        first = provider.GetGreeting(Trade(), 50, false);
        UnityEngine.Random.InitState(42);
        Assert.AreEqual(first, provider.GetGreeting(Trade(), 50, false), "New trade resets recent-template history.");
    }

    [TestCase("who are you", "Saraswati Chetti")]
    [TestCase("what's your name", "Saraswati Chetti")]
    [TestCase("where are you from", "Vijayanagara|region")]
    [TestCase("who is the king", "Krishnadevaraya")]
    [TestCase("tell me about the temple", "Virupaksha")]
    [TestCase("where is the temple", "Virupaksha")]
    public void SocialQuestionsReachTheCharacterTable_WithoutNegotiationPressure(string text, string expected)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput(text, trade);
        Assert.AreEqual(NegotiationIntent.GENERAL_DIALOGUE, input.intent);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++) Assert.IsTrue(Regex.IsMatch(provider.GetReply(input, trade, result, manager.CurrentRound), expected));
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void ClarificationPreservesTheBrainsSpecificQuestion_AndQuantityChangeIsNotUnclear()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept if you include delivery", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.requiresClarification);
        var provider = new DialogueTableResponseProvider();
        StringAssert.Contains(result.replyText, provider.GetReply(input, trade, result, manager.CurrentRound));
        Assert.IsNull(trade.AcceptedTerms);
        Assert.AreEqual(1000, trade.quantityGrams);
        input = new NegotiationInput { intent = NegotiationIntent.QUANTITY_CHANGE, hasQuantity = true,
            quantityGrams = 560, parseConfidence = ParseConfidence.High, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        Assert.AreEqual(560, trade.quantityGrams);
        AssertCategory(provider.GetReply(input, trade, result, manager.CurrentRound), DialogueScenario.QuantityChanged, trade);
    }

    [TestCase("what do you want", DialogueScenario.AskSellerPrice)]
    [TestCase("what spice what price", DialogueScenario.AskBuyerBudget)]
    [TestCase("what quantity do you want", DialogueScenario.AskQuantity)]
    [TestCase("why is your offer so low", DialogueScenario.OfferReason)]
    public void InformationalTradeQuestionsSelectTheirPresentationWithoutChangingTheOffer(string text, DialogueScenario scenario)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput(text, trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        AssertCategory(new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound), scenario, trade);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void SuspiciousLowPriceRemainsARefusal_RealDecisionPath()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("1 varaha", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        Assert.IsFalse(result.isAccepted);
        Assert.IsFalse(result.walkedAway);
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, result, manager.CurrentRound);
            AssertCategory(reply, DialogueScenario.SellerPriceBelowExpected, trade);
            Assert.IsFalse(Regex.IsMatch(reply, @"agreed|thank|bring|received"));
        }
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.IsEmpty(provider.GetTransactionSuccess(trade));
    }

    [TestCase(1)]
    [TestCase(3)]
    public void AgreementUsesFrozenTerms_AndCompletionRequiresSettlement(int presentationRound)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept 100", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.isAccepted);
        var provider = new DialogueTableResponseProvider();
        Assert.IsEmpty(provider.GetTransactionSuccess(trade));
        trade.npcOffer = 999; trade.quantityLabel = "wrong quantity"; trade.spiceDisplayName = "Cinnamon";
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, result, presentationRound);
            StringAssert.Contains("100", reply); StringAssert.Contains("1 kg", reply); StringAssert.Contains("pepper", reply);
            Assert.IsFalse(Regex.IsMatch(reply, @"999|wrong quantity|received|thank|complete"));
        }
        // Renderer boundary only; do not invoke reward, inventory or save IO in this test.
        typeof(LocalTradeState).GetProperty(nameof(LocalTradeState.SettlementClaimed)).SetValue(trade, true);
        AssertCategory(provider.GetTransactionSuccess(trade), DialogueScenario.TransactionSuccess, trade);
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
        Assert.AreEqual(1000, trade.AcceptedTerms.QuantityGrams);
        Assert.AreEqual(ExpectedReplyState.ExpectFulfillment, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void SoftRejectionWithNoConcessionDoesNotInventDeparture()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.REJECT };
        // A nonterminal refusal still invites another offer.
        const string misleadingOutcome = @"(?<!not )\bagreed\b|\b(?:leaving|part|received)\b";
        Assert.IsTrue(Regex.IsMatch("Agreed at 100 varahas.", misleadingOutcome, RegexOptions.IgnoreCase));
        Assert.IsTrue(Regex.IsMatch("I am leaving.", misleadingOutcome, RegexOptions.IgnoreCase));
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            AssertCategory(reply, DialogueScenario.PlayerRejectedBuyerOffer, trade);
            Assert.IsFalse(Regex.IsMatch(reply, misleadingOutcome, RegexOptions.IgnoreCase), reply);
        }
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void RemindersWarningAndDepartureUseExistingEventsWithoutAdvancingThem()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        AssertCategory(provider.GetIdleReminder(trade, "fallback", 1, false), DialogueScenario.IdleReminderFirst, trade);
        AssertCategory(provider.GetIdleReminder(trade, "fallback", 2, false), DialogueScenario.IdleReminderSecond, trade);
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.HOSTILE,
            normalizedText = "insult", expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isFinished);
        AssertCategory(provider.GetReply(input, trade, result, manager.CurrentRound), DialogueScenario.HostileWarning, trade);
        Assert.AreEqual(1, trade.hostileCount);
        Assert.AreEqual(6, trade.buyerPatience);
        input = manager.ClassifyInput("go away", trade);
        result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.walkedAway);
        AssertCategory(provider.GetReply(input, trade, result, manager.CurrentRound), DialogueScenario.TransactionFailure, trade);
        AssertCategory(provider.GetIdleReminder(trade, "fallback", 2, true), DialogueScenario.TransactionFailure, trade);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.IsEmpty(provider.GetTransactionSuccess(trade));
    }

    [Test]
    public void EarlierCharactersKeepTheirSelection_AndFatherPenteadoUsesAuthoredResponses()
    {
        var provider = new DialogueTableResponseProvider();
        provider.GetGreeting(Trade(), 50, false);
        var lakshmi = Trade(); lakshmi.buyerName = "Lakshmi Amma";
        UnityEngine.Random.InitState(42);
        string afterSaraswati = provider.GetGreeting(lakshmi, 50, false);
        UnityEngine.Random.InitState(42);
        Assert.AreEqual(new DialogueTableResponseProvider().GetGreeting(lakshmi, 50, false), afterSaraswati);
        var chinnamma = Trade(); chinnamma.buyerName = "Chinnamma Naik";
        UnityEngine.Random.InitState(73);
        string afterSaraswatiForChinnamma = provider.GetGreeting(chinnamma, 50, false);
        UnityEngine.Random.InitState(73);
        Assert.AreEqual(new DialogueTableResponseProvider().GetGreeting(chinnamma, 50, false), afterSaraswatiForChinnamma);
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(lakshmi));
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(chinnamma));
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(Trade()));
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation("francisco_de_almeida"));
        foreach (string name in new[] { "Father Penteado" })
        {
            var trade = Trade(); trade.buyerName = name;
            Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(trade));
            var result = Offer(); result.requiresClarification = true;
            StringAssert.Contains(result.replyText, provider.GetReply(null, trade, result, 1));
            CollectionAssert.Contains(FatherPenteadoDialogue.Create().lines.Single(
                line => line.scenario == DialogueScenario.IdleReminderSecond).templates,
                provider.GetIdleReminder(trade, "unchanged reminder", 2, false));
            Assert.IsEmpty(provider.GetTransactionSuccess(trade));
        }
    }
}
