using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

// Exercises Francisco's classify -> decide -> render path without scenes, audio or save IO.
public class Level1FranciscoDialogueTests : Level1NegotiationTestBase
{
    private UnityEngine.Random.State randomState;
    [SetUp] public void SetUp() { randomState = UnityEngine.Random.state; UnityEngine.Random.InitState(1522); }
    [TearDown] public void TearDown() { UnityEngine.Random.state = randomState; }

    private static LocalTradeState Trade(int patience = 8, string personality = "Strict")
    {
        return new LocalTradeState {
            buyerName = "Francisco de Almeida", buyerOrigin = "Portuguese Trade Agent", buyerPersonality = personality,
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

    private static string Fill(string template, LocalTradeState trade, int counter = 100, int seller = 100,
        string ruleReply = "Exact domain reply")
    {
        return template.Replace("{quantityLabel}", trade.AcceptedTerms != null ? trade.AcceptedTerms.QuantityLabel : trade.quantityLabel)
            .Replace("{spiceName}", trade.AcceptedTerms != null ? trade.AcceptedTerms.SpiceName.ToLowerInvariant() : trade.spiceDisplayName.ToLowerInvariant())
            .Replace("{currency}", "varahas").Replace("{currentBuyerOffer}", trade.npcOffer.ToString())
            .Replace("{counterPrice}", counter.ToString()).Replace("{offeredPrice}", seller.ToString())
            .Replace("{finalPrice}", trade.AcceptedTerms != null ? trade.AcceptedTerms.Price.ToString() : counter.ToString())
            .Replace("{ruleReply}", ruleReply);
    }

    private static void AssertCategory(string reply, DialogueScenario scenario, LocalTradeState trade, int counter = 100, int seller = 100,
        string ruleReply = "Exact domain reply")
    {
        var candidates = FranciscoDialogue.Create().lines.Where(line => line.scenario == scenario)
            .SelectMany(line => line.templates).Select(template => Fill(template, trade, counter, seller, ruleReply)).ToArray();
        CollectionAssert.Contains(candidates, reply, scenario.ToString());
        StringAssert.DoesNotContain("{", reply);
    }

    private static bool HoldClaimsConcessionAgreementOrDeparture(string reply)
    {
        return Regex.IsMatch(reply,
            @"\b(?:I|we)\s+(?:(?:have|had|am|are|can|could|will|did)\s+)?(?:rais(?:e|ed|ing)|increas(?:e|ed|ing)|mov(?:e|ed|ing))\b" +
            @"|\bmy\s+(?:revised|increased)\s+offer\b" +
            @"|\b(?:we\s+(?:agree|agreed|are\s+agreed|have\s+agreed)|I\s+accept(?:ed)?|the\s+(?:price|terms)\s+(?:is|are)\s+settled)\b" +
            @"|\b(?:I\s+(?:(?:am|will|must)\s+)?(?:leav(?:e|ing)|depart(?:ing)?)|we\s+part|I\s+(?:will|must)\s+go\s+elsewhere)\b",
            RegexOptions.IgnoreCase);
    }

    [Test]
    public void ActiveIdentity_AllScenarios_SupportedPlaceholders_AndDistinctVoice()
    {
        var table = FranciscoDialogue.Create();
        Assert.AreEqual("francisco_de_almeida", table.characterId);
        Assert.AreEqual("Francisco de Almeida", table.displayName);
        Assert.AreEqual("Francisco de Almeida", new DialogueCharacterRegistry().FindCharacter("francisco_de_almeida").displayName);
        Assert.AreEqual("francisco_de_almeida", DialogueCharacterRegistry.NormalizeCharacterId("Francisco de Almeida"));
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(Trade()));
        var otherTemplates = new[] { LakshmiAmmaDialogue.Create(), ChinappaNaikDialogue.Create(),
            SiddharthChettiDialogue.Create(), AbdulRahmanDialogue.Create(), FatherPenteadoDialogue.Create() }
            .SelectMany(set => set.lines).SelectMany(line => line.templates).ToArray();
        string[] supported = { "buyerName", "characterName", "spiceName", "quantityLabel", "offeredPrice", "counterPrice",
            "finalPrice", "currentBuyerOffer", "minimumPrice", "currency", "round", "reputation", "patience", "desperation", "ruleReply" };
        foreach (DialogueScenario scenario in Enum.GetValues(typeof(DialogueScenario)))
            Assert.IsTrue(table.lines.Any(line => line.scenario == scenario && !line.patienceBucket.HasValue &&
                !line.personalityBucket.HasValue && !line.roundBucket.HasValue && !line.reputationBucket.HasValue &&
                !line.desperationBucket.HasValue), scenario.ToString());
        foreach (var line in table.lines)
        foreach (string template in line.templates)
        {
            Assert.AreEqual("francisco_de_almeida", line.characterId);
            Assert.IsFalse(string.IsNullOrWhiteSpace(template));
            foreach (Match match in Regex.Matches(template, @"\{([^}]+)\}"))
                CollectionAssert.Contains(supported, match.Groups[1].Value, template);
            if (template != "{ruleReply}") CollectionAssert.DoesNotContain(otherTemplates, template);
        }
    }

    [Test]
    public void PortugueseExpressionsStayInAppropriateContexts()
    {
        var lines = FranciscoDialogue.Create().lines;
        string all = string.Join(" ", lines.SelectMany(line => line.templates));
        foreach (string expression in new[] { "Olá", "Ora", "Pois bem", "Por favor", "Muito bem", "Não", "Obrigado" })
            StringAssert.Contains(expression, all);
        Assert.IsFalse(Regex.IsMatch(all, @"\b(Ayyo|Swalpa|Sari|Howdu|Beda|Saaku|Namaskaram|Avunu|Konchem|Marhaban|Shukran)\b"));
        foreach (var line in lines)
        foreach (string template in line.templates)
        {
            if (template.Contains("Olá")) Assert.IsTrue(line.scenario == DialogueScenario.CustomerGreeting ||
                line.scenario == DialogueScenario.RepeatCustomerGreeting || line.scenario == DialogueScenario.SocialGreeting);
            if (template.Contains("Ora")) Assert.IsTrue(line.scenario == DialogueScenario.SellerPriceTooHigh ||
                line.scenario == DialogueScenario.SellerPriceBelowExpected);
            if (template.Contains("Muito bem")) Assert.IsTrue(line.scenario == DialogueScenario.SellerPriceAccepted ||
                line.scenario == DialogueScenario.PlayerAcceptedDeal);
            if (template.Contains("Não")) Assert.AreEqual(DialogueScenario.SellerPriceTooHigh, line.scenario);
            if (template.Contains("Obrigado")) Assert.AreEqual(DialogueScenario.TransactionSuccess, line.scenario);
            if (template.Contains("Por favor")) Assert.IsTrue(line.scenario == DialogueScenario.AskSellerPrice ||
                line.scenario == DialogueScenario.UnclearSpeech);
            if (template.Contains("Pois bem")) Assert.AreEqual(DialogueScenario.BuyerCounterFirst, line.scenario);
        }
    }

    [TestCase(8, "Friendly")]
    [TestCase(8, "Normal")]
    [TestCase(8, "Strict")]
    [TestCase(8, "Impatient")]
    [TestCase(2, "Strict")]
    [TestCase(1, "Friendly")]
    public void HoldRespectsMoodPatience_AndAvoidsRecentFour(int patience, string personality)
    {
        var trade = Trade(patience, personality);
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.BARGAIN };
        var low = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.patienceBucket == NpcPatienceBucket.Low).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var bucket = personality == "Friendly" ? NpcPersonalityBucket.Friendly : personality == "Strict" ?
            NpcPersonalityBucket.Strict : personality == "Impatient" ? NpcPersonalityBucket.Impatient : NpcPersonalityBucket.Normal;
        var mood = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            line.personalityBucket == bucket).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var generic = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var allowed = mood.Concat(generic).ToArray();
        var recent = new Queue<string>();
        for (int i = 0; i < 18; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            if (patience <= 2) CollectionAssert.Contains(low, reply);
            else { CollectionAssert.DoesNotContain(low, reply); CollectionAssert.Contains(allowed, reply); }
            CollectionAssert.DoesNotContain(recent, reply);
            recent.Enqueue(reply);
            if (recent.Count > 4) recent.Dequeue();
            Assert.IsFalse(HoldClaimsConcessionAgreementOrDeparture(reply), reply);
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
        var generic = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
            !line.personalityBucket.HasValue && !line.patienceBucket.HasValue).SelectMany(line => line.templates).Select(t => Fill(t, trade)).ToArray();
        var mood = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.BuyerHoldsFirm &&
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
    public void FranciscoDialogue_HasNoUnimplementedInspectionPhrases()
    {
        var lines = FranciscoDialogue.Create().lines;
        string all = string.Join(" ", lines.SelectMany(line => line.templates));
        Assert.IsFalse(Regex.IsMatch(all, @"\b(?:inspect|examine|assess sample|see the delivery|show me the delivery)\b", RegexOptions.IgnoreCase), all);
    }

    [TestCase("I have not raised my offer. It remains 100 varahas.", false)]
    [TestCase("I have not increased my offer. It remains 100 varahas.", false)]
    [TestCase("I raised my offer to 110 varahas.", true)]
    [TestCase("We are agreed at 100 varahas.", true)]
    [TestCase("I will leave this stall.", true)]
    public void HoldLanguageCheckDistinguishesNegationFromFalseOutcomes(string reply, bool misleading)
    {
        Assert.AreEqual(misleading, HoldClaimsConcessionAgreementOrDeparture(reply), reply);
    }

    [TestCase(8, "Strict")]
    [TestCase(8, "Impatient")]
    [TestCase(1, "Friendly")]
    public void SocialGreetingUsesOnlyItsEligibleMoodOrPatienceLines(int patience, string personality)
    {
        var trade = Trade(patience, personality);
        var lines = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.SocialGreeting);
        var eligible = lines.Single(line => patience <= 2 ? line.patienceBucket == NpcPatienceBucket.Low :
            line.personalityBucket == (personality == "Strict" ? NpcPersonalityBucket.Strict : NpcPersonalityBucket.Impatient))
            .templates.Select(template => Fill(template, trade)).ToArray();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.GREETING }, trade, Offer(), 3);
            CollectionAssert.Contains(eligible, reply);
        }
        Assert.AreEqual(patience, trade.buyerPatience);
    }

    [TestCase(1, DialogueScenario.BuyerCounterFirst)]
    [TestCase(3, DialogueScenario.BuyerCounterMiddle)]
    [TestCase(6, DialogueScenario.BuyerCounterFinal)]
    public void IncreasedOfferUsesCounterScenarioWithoutInventingAnAbsoluteLimit(int round, DialogueScenario scenario)
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.BARGAIN }, trade, Offer(110), round);
            AssertCategory(reply, scenario, trade, 110);
            StringAssert.Contains("110", reply);
            Assert.IsFalse(Regex.IsMatch(reply, @"\b(?:final|last|never)\b|no further|no higher", RegexOptions.IgnoreCase), reply);
        }
    }

    [TestCase(200, DialogueScenario.SellerPriceTooHigh)]
    [TestCase(183, DialogueScenario.SellerPriceSlightlyHigh)]
    public void RejectedHighPriceDoesNotSoundAccepted(int sellerPrice, DialogueScenario scenario)
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(new NegotiationInput { intent = NegotiationIntent.PRICE,
                hasSellerPrice = true, sellerPrice = sellerPrice }, trade, Offer(), 3);
            AssertCategory(reply, scenario, trade, 100, sellerPrice);
            Assert.IsFalse(Regex.IsMatch(reply, @"\b(?:agreed|accepted|received|delivered)\b", RegexOptions.IgnoreCase), reply);
        }
    }

    [Test]
    public void LowPatienceHighPriceUsesUrgencyOnlyWhenPatienceIsLow()
    {
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE, hasSellerPrice = true, sellerPrice = 200 };
        var low = Trade(1);
        var lowLines = FranciscoDialogue.Create().lines.Where(line => line.scenario == DialogueScenario.SellerPriceTooHigh &&
            line.patienceBucket == NpcPatienceBucket.Low).SelectMany(line => line.templates)
            .Select(template => Fill(template, low, 100, 200)).ToArray();
        for (int i = 0; i < 6; i++) CollectionAssert.Contains(lowLines, provider.GetReply(input, low, Offer(), 3));
        var calm = Trade(8);
        for (int i = 0; i < 6; i++) CollectionAssert.DoesNotContain(lowLines, provider.GetReply(input, calm, Offer(), 3));
    }

    [Test]
    public void SuspiciousLowPriceIsNotAnAgreement()
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
            AssertCategory(reply, DialogueScenario.SellerPriceBelowExpected, trade, result.updatedOffer, 1);
            Assert.IsFalse(Regex.IsMatch(reply, @"\b(?:agreed|accepted|received|delivered|thank)\b", RegexOptions.IgnoreCase), reply);
        }
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.IsEmpty(provider.GetTransactionSuccess(trade));
    }

    [Test]
    public void NonterminalRejectionDoesNotInventDeparture()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        var input = new NegotiationInput { intent = NegotiationIntent.REJECT };
        for (int i = 0; i < 8; i++)
        {
            string reply = provider.GetReply(input, trade, Offer(), 3);
            AssertCategory(reply, DialogueScenario.PlayerRejectedBuyerOffer, trade);
            Assert.IsFalse(Regex.IsMatch(reply, @"\b(?:leave|leaving|depart|part|received|delivered)\b|\bwe are agreed\b",
                RegexOptions.IgnoreCase), reply);
        }
        Assert.IsNull(trade.AcceptedTerms);
    }

    [TestCase(1)]
    [TestCase(3)]
    public void AgreementUsesFrozenTerms_CompletionRequiresClaimedSettlement(int round)
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
            string reply = provider.GetReply(input, trade, result, round);
            StringAssert.Contains("100", reply);
            StringAssert.Contains("1 kg", reply);
            StringAssert.Contains("pepper", reply);
            Assert.IsFalse(Regex.IsMatch(reply, @"999|wrong quantity|received|delivered|complete|thank", RegexOptions.IgnoreCase), reply);
        }
        typeof(LocalTradeState).GetProperty(nameof(LocalTradeState.SettlementClaimed)).SetValue(trade, true);
        AssertCategory(provider.GetTransactionSuccess(trade), DialogueScenario.TransactionSuccess, trade);
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
        Assert.AreEqual(1000, trade.AcceptedTerms.QuantityGrams);
        Assert.AreEqual(ExpectedReplyState.ExpectFulfillment, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void FirstAndRepeatGreetingsAreDistinctWithoutInventingACompletedTrade()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
        {
            AssertCategory(provider.GetGreeting(trade, 50, false), DialogueScenario.CustomerGreeting, trade);
            string repeat = provider.GetGreeting(trade, 50, true);
            AssertCategory(repeat, DialogueScenario.RepeatCustomerGreeting, trade);
            Assert.IsFalse(Regex.IsMatch(repeat, @"\b(?:sold|delivered|paid|bought|completed)\b", RegexOptions.IgnoreCase), repeat);
        }
        trade.spiceDisplayName = "Cinnamon";
        StringAssert.Contains("cinnamon", provider.GetGreeting(trade, 50, false));
    }

    [TestCase("what do you want", DialogueScenario.AskSellerPrice)]
    [TestCase("what spice what price", DialogueScenario.AskBuyerBudget)]
    [TestCase("what quantity do you want", DialogueScenario.AskQuantity)]
    [TestCase("why is your offer so low", DialogueScenario.OfferReason)]
    public void TradeQuestionsUseTheirOwnPresentationWithoutChangingTheOffer(string text, DialogueScenario scenario)
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

    [TestCase("what is your name", "Francisco")]
    [TestCase("where are you from", "Portugal|Portuguese")]
    [TestCase("who is king", "Krishnadevaraya")]
    [TestCase("tell me about the temple", "Virupaksha")]
    [TestCase("where is the temple", "Virupaksha")]
    public void SocialAndHistoryQuestionsGetAnswersWithoutNegotiationPressure(string text, string expected)
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput(text, trade);
        Assert.AreEqual(NegotiationIntent.GENERAL_DIALOGUE, input.intent);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        var provider = new DialogueTableResponseProvider();
        for (int i = 0; i < 8; i++)
            Assert.IsTrue(Regex.IsMatch(provider.GetReply(input, trade, result, manager.CurrentRound), expected), text);
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void WeatherAndOffTopicUseTheirOwnNonterminalPresentation()
    {
        var trade = Trade();
        var provider = new DialogueTableResponseProvider();
        var weather = new NegotiationInput { intent = NegotiationIntent.GENERAL_DIALOGUE, normalizedText = "how is the weather" };
        AssertCategory(provider.GetReply(weather, trade, Offer(), 3), DialogueScenario.WeatherQuestion, trade);
        var offTopic = new NegotiationInput { intent = NegotiationIntent.OFF_TOPIC };
        string reply = provider.GetReply(offTopic, trade, Offer(), 3);
        AssertCategory(reply, DialogueScenario.OffTopic, trade);
        Assert.IsFalse(Regex.IsMatch(reply, @"\b(?:leaving|departing|we part|trade is over)\b", RegexOptions.IgnoreCase), reply);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [Test]
    public void ClarificationKeepsTheBrainQuestion_QuantityChangeGetsItsOwnReply()
    {
        var trade = Trade();
        var manager = CreateManager(100);
        var input = manager.ClassifyInput("I accept if you include delivery", trade);
        var result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.requiresClarification);
        var provider = new DialogueTableResponseProvider();
        StringAssert.Contains(result.replyText, provider.GetReply(input, trade, result, manager.CurrentRound));
        Assert.IsNull(trade.AcceptedTerms);
        input = new NegotiationInput { intent = NegotiationIntent.QUANTITY_CHANGE, hasQuantity = true,
            quantityGrams = 560, parseConfidence = ParseConfidence.High, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        Assert.AreEqual(560, trade.quantityGrams);
        AssertCategory(provider.GetReply(input, trade, result, manager.CurrentRound), DialogueScenario.QuantityChanged, trade);
    }

    [Test]
    public void RemindersWarningsAndActualDepartureUseExistingEvents()
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
    public void EarlierCharactersRemainAuthored_FatherPenteadoUsesAuthoredResponses()
    {
        var provider = new DialogueTableResponseProvider();
        provider.GetGreeting(Trade(), 50, false);
        foreach (string name in new[] { "Lakshmi Amma", "Chinnamma Naik", "Saraswati Chetti", "Abdul Rahman" })
        {
            var trade = Trade(); trade.buyerName = name;
            Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(trade));
            UnityEngine.Random.InitState(42);
            string afterFrancisco = provider.GetGreeting(trade, 50, false);
            UnityEngine.Random.InitState(42);
            Assert.AreEqual(new DialogueTableResponseProvider().GetGreeting(trade, 50, false), afterFrancisco);
        }
        var father = Trade(); father.buyerName = "Father Penteado";
        Assert.IsTrue(DialogueTableResponseProvider.UsesAuthoredPresentation(father));
        var result = Offer(); result.requiresClarification = true;
        StringAssert.Contains(result.replyText, provider.GetReply(null, father, result, 1));
        CollectionAssert.Contains(FatherPenteadoDialogue.Create().lines.Single(
            line => line.scenario == DialogueScenario.IdleReminderSecond).templates,
            provider.GetIdleReminder(father, "unchanged reminder", 2, false));
        Assert.IsEmpty(provider.GetTransactionSuccess(father));
    }
}
