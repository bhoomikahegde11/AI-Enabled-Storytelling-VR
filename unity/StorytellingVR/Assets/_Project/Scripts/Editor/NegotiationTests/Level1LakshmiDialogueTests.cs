using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

// Pure production classify/decision/render path. No scene, singleton creation, audio or save IO.
public class Level1LakshmiDialogueTests : Level1NegotiationTestBase
{
    private UnityEngine.Random.State randomState;

    [SetUp]
    public void SetUp()
    {
        randomState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(1522);
    }

    [TearDown]
    public void TearDown() { UnityEngine.Random.state = randomState; }

    private static LocalTradeState Trade(int patience = 8, string personality = "Friendly")
    {
        return new LocalTradeState {
            buyerName = "Lakshmi Amma", buyerOrigin = "Local Household Buyer", buyerPersonality = personality,
            spiceKey = "pepper", spiceDisplayName = "Pepper", quantityGrams = 1000, quantityLabel = "1 kg",
            npcOffer = 100, previousNpcOffer = 100, marketValue = 96, maxBuyerPrice = 180,
            buyerPatience = patience, minIncrement = 2, negotiationInitialized = true,
            buyerTrust = 0.5f, buyerFrustration = 0.1f, buyerDesperation = 0.5f
        };
    }

    private static RuleBasedNPCBrainResult HoldingResult()
    {
        return new RuleBasedNPCBrainResult {
            previousOffer = 100, updatedOffer = 100, resolvedPrice = 100,
            nextExpectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter, replyText = "The offer remains 100."
        };
    }

    [Test]
    public void EveryAuthoredScenarioHasAnUnrestrictedFallback_AndOnlySupportedPlaceholders()
    {
        var lines = LakshmiAmmaDialogue.Create().lines;
        foreach (DialogueScenario scenario in Enum.GetValues(typeof(DialogueScenario)))
            Assert.IsTrue(lines.Any(line => line.scenario == scenario && !line.patienceBucket.HasValue &&
                !line.personalityBucket.HasValue && !line.reputationBucket.HasValue &&
                !line.roundBucket.HasValue && !line.desperationBucket.HasValue), scenario.ToString());

        string[] supported = { "buyerName", "characterName", "spiceName", "quantityLabel", "offeredPrice",
            "counterPrice", "finalPrice", "currentBuyerOffer", "minimumPrice", "currency", "round",
            "reputation", "patience", "desperation", "ruleReply" };
        foreach (var line in lines)
        foreach (string template in line.templates)
        foreach (Match match in Regex.Matches(template, @"\{([^}]+)\}"))
            CollectionAssert.Contains(supported, match.Groups[1].Value, template);
    }

    [TestCase(1, "Friendly")]
    [TestCase(2, "Strict")]
    [TestCase(8, "Friendly")]
    [TestCase(8, "Normal")]
    [TestCase(8, "Strict")]
    [TestCase(8, "Impatient")]
    public void PatienceRestrictionsNeverLeakThroughFallback(int patience, string personality)
    {
        var trade = Trade(patience, personality);
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        var lines = LakshmiAmmaDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm);
        var lowLines = lines.Where(line => line.patienceBucket == NpcPatienceBucket.Low)
            .SelectMany(line => line.templates).Select(t => t.Replace("{counterPrice}", "100").Replace("{currency}", "varahas")).ToArray();
        for (int i = 0; i < 24; i++)
        {
            string reply = provider.GetReply(input, trade, HoldingResult(), 3);
            if (patience <= 2) CollectionAssert.Contains(lowLines, reply);
            else CollectionAssert.DoesNotContain(lowLines, reply);
        }
    }

    [TestCase(8, "Friendly")]
    [TestCase(8, "Strict")]
    [TestCase(8, "Impatient")]
    [TestCase(2, "Strict")]
    public void RepeatedObjectionsAvoidTheLastFourTemplates(int patience, string personality)
    {
        var provider = new DialogueTableResponseProvider();
        var trade = Trade(patience, personality);
        var recent = new System.Collections.Generic.Queue<string>();
        for (int i = 0; i < 30; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.BARGAIN }, trade, HoldingResult(), 3);
            CollectionAssert.DoesNotContain(recent, reply);
            recent.Enqueue(reply);
            if (recent.Count > 4) recent.Dequeue();
        }
    }

    [Test]
    public void BuyerHoldsFirm_GenericLinesAreReachableAlongsideMoodVariants()
    {
        var trade = Trade(8, "Strict");
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        var generic = LakshmiAmmaDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates)
            .Select(t => t.Replace("{counterPrice}", "100").Replace("{currency}", "varahas")).ToArray();
        var mood = LakshmiAmmaDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.personalityBucket == NpcPersonalityBucket.Strict).SelectMany(line => line.templates)
            .Select(t => t.Replace("{counterPrice}", "100").Replace("{currency}", "varahas")).ToArray();

        Assert.IsNotEmpty(generic);
        Assert.IsNotEmpty(mood);
        var eligible = generic.Concat(mood).ToArray();

        bool sawGeneric = false;
        bool sawMood = false;
        for (int i = 0; i < 40; i++)
        {
            string reply = provider.GetReply(input, trade, HoldingResult(), 3);
            CollectionAssert.Contains(eligible, reply);
            if (generic.Contains(reply)) sawGeneric = true;
            if (mood.Contains(reply)) sawMood = true;
        }
        Assert.IsTrue(sawMood, "Mood-specific variants should be selected.");
        Assert.IsTrue(sawGeneric, "Generic variants must be reachable and not shadowed.");
    }

    [TestCase("Friendly")]
    [TestCase("Normal")]
    public void BuyerHoldsFirm_MoodsWithoutAuthoredVariantsUseGenericLines(string personality)
    {
        var trade = Trade(8, personality);
        var provider = new DialogueTableResponseProvider();
        var generic = LakshmiAmmaDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates)
            .Select(t => t.Replace("{counterPrice}", "100").Replace("{currency}", "varahas")).ToArray();

        for (int i = 0; i < 8; i++)
            CollectionAssert.Contains(generic, provider.GetReply(
                new NegotiationInput { intent = NegotiationIntent.BARGAIN }, trade, HoldingResult(), 3));
    }

    [Test]
    public void LakshmiDialogue_HasNoUnimplementedInspectionPhrases()
    {
        var lines = LakshmiAmmaDialogue.Create().lines;
        string all = string.Join(" ", lines.SelectMany(line => line.templates));
        Assert.IsFalse(Regex.IsMatch(all, @"\b(?:inspect|examine|assess sample|see the delivery)\b", RegexOptions.IgnoreCase), all);
    }

    [Test]
    public void GreetingVariantsDoNotRepeatImmediatelyDuringTheSameVisit()
    {
        var provider = new DialogueTableResponseProvider();
        var trade = Trade();
        var recent = new System.Collections.Generic.Queue<string>();
        for (int i = 0; i < 18; i++)
        {
            string reply = provider.GetGreeting(trade, 50, false);
            CollectionAssert.DoesNotContain(recent, reply);
            recent.Enqueue(reply);
            if (recent.Count > 4) recent.Dequeue();
        }
    }

    [Test]
    public void NewTradeResetsVariationHistory()
    {
        var provider = new DialogueTableResponseProvider();
        UnityEngine.Random.InitState(42);
        string first = provider.GetGreeting(Trade(), 50, false);
        UnityEngine.Random.InitState(42);
        string nextCustomer = provider.GetGreeting(Trade(), 50, false);
        Assert.AreEqual(first, nextCustomer);
        StringAssert.Contains("1 kg", first);
        StringAssert.Contains("pepper", first);
        StringAssert.DoesNotContain("{", first);
    }

    [Test]
    public void GreetingDuringNegotiationDoesNotPretendThisIsAnotherVisit()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.GREETING }, trade, HoldingResult(), 6);
        string[] social = LakshmiAmmaDialogue.Create().lines.Single(line => line.scenario == DialogueScenario.SocialGreeting && !line.patienceBucket.HasValue).templates;
        CollectionAssert.Contains(social, reply);
        string repeat = provider.GetGreeting(trade, 50, true);
        StringAssert.Contains("1 kg", repeat);
        StringAssert.Contains("pepper", repeat);
        Assert.AreNotEqual(reply, repeat);
    }

    [TestCase("what is your name", "Lakshmi Amma")]
    [TestCase("what's your name?", "Lakshmi Amma")]
    [TestCase("who are you", "Lakshmi Amma")]
    [TestCase("tell me your name", "Lakshmi Amma")]
    [TestCase("Could you tell me your name, please?", "Lakshmi Amma")]
    [TestCase("where from", "local")]
    [TestCase("where are you from", "local")]
    [TestCase("are you from here", "local")]
    [TestCase("where do you come from?", "local")]
    [TestCase("who is king", "Krishnadevaraya")]
    [TestCase("who is the king", "Krishnadevaraya")]
    [TestCase("tell me about the temple", "Virupaksha")]
    [TestCase("Please tell me about this temple", "Virupaksha")]
    [TestCase("where is the temple", "Virupaksha")]
    [TestCase("what temple is this", "Virupaksha")]
    public void SupportedQuestionsGetTheirOwnAnswers(string text, string expected)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput(text, trade);
        Assert.AreEqual(NegotiationIntent.GENERAL_DIALOGUE, input.intent);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, result, manager.CurrentRound);
            if (expected == "local") Assert.IsTrue(reply.Contains("local") || reply.Contains("Vijayanagara"), reply);
            else StringAssert.Contains(expected, reply);
        }
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.IsFalse(result.isFinished);
    }

    [Test]
    public void WeatherIsNotReplacedByHistory_AndDoesNotInventCurrentConditions()
    {
        var reply = new DialogueTableResponseProvider().GetReply(new NegotiationInput {
            intent = NegotiationIntent.GENERAL_DIALOGUE, normalizedText = "how is the weather", socialSubIntent = "WEATHER"
        }, Trade(), HoldingResult(), 1);
        var weather = LakshmiAmmaDialogue.Create().lines.Single(line => line.scenario == DialogueScenario.WeatherQuestion).templates;
        CollectionAssert.Contains(weather, reply);
    }

    [Test]
    public void PriceReasonIsHouseholdBuying_NotResaleProfit()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE_QUERY, asksReason = true,
            expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter, parseConfidence = ParseConfidence.High };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        string reply = new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound);
        StringAssert.Contains("100", reply);
        StringAssert.DoesNotContain("profit", reply);
        StringAssert.DoesNotContain("sell", reply);
        Assert.IsFalse(result.isAccepted);
    }

    [Test]
    public void CombinedItemAndReasonQuestionPreservesTheBrainsItemAnswerPriority()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE_QUERY,
            asksItem = true, asksReason = true, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        string reply = new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound);
        StringAssert.Contains("1 kg", reply);
        StringAssert.Contains("pepper", reply);
        StringAssert.Contains("100", reply);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void ContinuationRestatesTheOutstandingTermsWithoutAskingForClearerSpeech()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.CONTINUE,
            expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        string reply = new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound);
        StringAssert.Contains("1 kg", reply);
        StringAssert.Contains("pepper", reply);
        StringAssert.Contains("100", reply);
        StringAssert.DoesNotContain("plainly", reply);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void HostileWarningKeepsHerVoiceWithoutInventingATerminalOutcome()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.HOSTILE,
            normalizedText = "you fool", expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isFinished);
        var warnings = LakshmiAmmaDialogue.Create().lines.Single(
            line => line.scenario == DialogueScenario.HostileWarning).templates;
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 12; i++)
            CollectionAssert.Contains(warnings, provider.GetReply(input, trade, result, manager.CurrentRound));
        Assert.AreEqual(1, trade.hostileCount);
        Assert.AreEqual(6, trade.buyerPatience);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void OpeningItemQuestionAsksSellerPrice_ButOfferQuestionGivesCurrentOffer()
    {
        var trade = Trade();
        var manager = CreateManager(100, ExpectedReplyState.ExpectOfferPrice);
        var provider = new DialogueTableResponseProvider();
        var opening = new NegotiationInput { intent = NegotiationIntent.ITEM_QUERY, tradeOpeningQuery = true,
            expectedReplyState = ExpectedReplyState.ExpectOfferPrice, parseConfidence = ParseConfidence.High };
        var result = manager.ProcessLocalTurn(opening, trade, new RuleBasedNPCBrain());
        string reply = provider.GetReply(opening, trade, result, manager.CurrentRound);
        StringAssert.Contains("1 kg", reply);
        var openingLines = LakshmiAmmaDialogue.Create().lines.Single(line => line.scenario == DialogueScenario.AskSellerPrice)
            .templates.Select(template => template.Replace("{quantityLabel}", "1 kg").Replace("{spiceName}", "pepper")).ToArray();
        CollectionAssert.Contains(openingLines, reply);
        Assert.AreEqual(ExpectedReplyState.ExpectOfferPrice, manager.CurrentExpectedReplyState);

        var offer = new NegotiationInput { intent = NegotiationIntent.ITEM_QUERY, asksCurrentOffer = true,
            expectedReplyState = ExpectedReplyState.ExpectOfferPrice, parseConfidence = ParseConfidence.High };
        result = manager.ProcessLocalTurn(offer, trade, new RuleBasedNPCBrain());
        reply = provider.GetReply(offer, trade, result, manager.CurrentRound);
        StringAssert.Contains("100", reply);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [TestCase(NegotiationIntent.PRICE)]
    [TestCase(NegotiationIntent.DISMISS)]
    public void RefusalAndDepartureDoNotSoundLikeAgreementOrDelivery(NegotiationIntent intent)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = intent, hasSellerPrice = intent == NegotiationIntent.PRICE,
            sellerPrice = 400, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter,
            parseConfidence = ParseConfidence.High };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isAccepted);
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 12; i++)
        {
            string reply = provider.GetReply(input, trade, result, manager.CurrentRound).ToLowerInvariant();
            StringAssert.DoesNotContain("agreed at", reply);
            StringAssert.DoesNotContain("bring", reply);
            StringAssert.DoesNotContain("thank you", reply);
        }
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void ValidQuantityChangeIsAcknowledged_NotCalledUnclear()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.QUANTITY_CHANGE,
            hasQuantity = true, quantityGrams = 560, parseConfidence = ParseConfidence.High,
            expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        string reply = new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound);
        StringAssert.Contains(trade.quantityLabel, reply);
        StringAssert.Contains("pepper", reply);
        StringAssert.Contains("100", reply);
        StringAssert.DoesNotContain("clearer", reply);
        Assert.AreEqual(560, trade.quantityGrams);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void ClarificationRetainsTheExactDomainQuestion_AndDoesNotMutateTerms()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept if you include delivery", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.requiresClarification);
        string original = result.replyText;
        string reply = new DialogueTableResponseProvider().GetReply(input, trade, result, manager.CurrentRound);
        StringAssert.Contains(original, reply);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(1000, trade.quantityGrams);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.IsFalse(manager.IsNegotiationFinished);
    }

    [Test]
    public void SuspiciousLowPriceRetainsRefusal_RealBrainPath()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE, hasSellerPrice = true,
            sellerPrice = 1, parseConfidence = ParseConfidence.High, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isAccepted);
        Assert.IsFalse(result.walkedAway);
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 12; i++)
        {
            string reply = provider.GetReply(input, trade, result, manager.CurrentRound);
            StringAssert.Contains("100", reply);
            StringAssert.DoesNotContain("agreed", reply.ToLowerInvariant());
            StringAssert.DoesNotContain("thank you", reply.ToLowerInvariant());
        }
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void AcceptedWordingUsesFrozenTerms_AndDoesNotClaimDelivery()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept 100", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.isAccepted);
        trade.npcOffer = 999;
        trade.quantityLabel = "wrong quantity";
        trade.spiceDisplayName = "Cinnamon";
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 12; i++)
        {
            string reply = provider.GetReply(input, trade, result, manager.CurrentRound);
            StringAssert.Contains("100", reply);
            StringAssert.Contains("1 kg", reply);
            StringAssert.Contains("pepper", reply);
            StringAssert.DoesNotContain("999", reply);
            StringAssert.DoesNotContain("{", reply);
            StringAssert.DoesNotContain("purchase is complete", reply.ToLowerInvariant());
            StringAssert.DoesNotContain("thank you", reply.ToLowerInvariant());
        }
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
        Assert.AreEqual(1000, trade.AcceptedTerms.QuantityGrams);
        Assert.AreEqual(ExpectedReplyState.ExpectFulfillment, manager.CurrentExpectedReplyState);
        Assert.IsEmpty(provider.GetTransactionSuccess(trade), "Acceptance alone is not delivery.");
    }

    [Test]
    public void HeldOfferDoesNotClaimIncrease_AndLateCounterDoesNotPromisePermanentLimit()
    {
        var provider = new DialogueTableResponseProvider();
        var trade = Trade();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        for (int i = 0; i < 12; i++)
        {
            string held = provider.GetReply(input, trade, HoldingResult(), 6);
            StringAssert.Contains("100", held);
            StringAssert.DoesNotContain("raised", held);
            var moved = HoldingResult();
            moved.updatedOffer = 110;
            string counter = provider.GetReply(input, trade, moved, 6);
            StringAssert.Contains("110", counter);
            StringAssert.DoesNotContain("last offer", counter);
            StringAssert.DoesNotContain("no higher", counter);
            Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, moved.nextExpectedReplyState);
        }
    }

    [Test]
    public void ReminderStagesAndTerminalDepartureHaveDistinctPresentation()
    {
        var provider = new DialogueTableResponseProvider();
        var trade = Trade();
        var lines = LakshmiAmmaDialogue.Create().lines;
        foreach (int stage in new[] { 1, 2 })
        {
            string reply = provider.GetIdleReminder(trade, "generic reminder", stage, false);
            var scenario = stage == 1 ? DialogueScenario.IdleReminderFirst : DialogueScenario.IdleReminderSecond;
            CollectionAssert.Contains(lines.Single(line => line.scenario == scenario).templates, reply);
        }
        string terminal = provider.GetIdleReminder(trade, "generic final", 2, true);
        CollectionAssert.Contains(lines.Single(line => line.scenario == DialogueScenario.TransactionFailure && !line.patienceBucket.HasValue).templates, terminal);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void CompletionThanksAreAvailableOnlyAfterAcceptedSettlement()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept 100", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.isAccepted);
        var provider = new DialogueTableResponseProvider();
        Assert.IsEmpty(provider.GetTransactionSuccess(trade));
        // Set up the completed event for this renderer test without invoking reward/save IO.
        typeof(LocalTradeState).GetProperty(nameof(LocalTradeState.SettlementClaimed)).SetValue(trade, true);
        string reply = provider.GetTransactionSuccess(trade);
        CollectionAssert.Contains(LakshmiAmmaDialogue.Create().lines.Single(
            line => line.scenario == DialogueScenario.TransactionSuccess).templates, reply);
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
        Assert.AreEqual(ExpectedReplyState.ExpectFulfillment, result.nextExpectedReplyState);
    }

    [Test]
    public void FatherPenteadoKeepsClarificationAndUsesAuthoredReminder()
    {
        var provider = new DialogueTableResponseProvider();
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation("francisco_de_almeida"));
        foreach (string name in new[] { "Father Penteado" })
        {
            var trade = Trade();
            trade.buyerName = name;
            var result = HoldingResult();
            result.requiresClarification = true;
            result.replyText = "Exact domain clarification";
            Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(trade));
            StringAssert.Contains(result.replyText, provider.GetReply(null, trade, result, 3));
            CollectionAssert.Contains(FatherPenteadoDialogue.Create().lines.Single(
                line => line.scenario == DialogueScenario.IdleReminderSecond).templates,
                provider.GetIdleReminder(trade, "Original reminder", 2, false));
            Assert.IsEmpty(provider.GetTransactionSuccess(trade));
        }
    }
}
