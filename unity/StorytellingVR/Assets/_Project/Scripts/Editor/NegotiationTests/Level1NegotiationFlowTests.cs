using NUnit.Framework;

// Exercises the same classify -> brain -> commit boundary used by ChatManager.
// No scene, singleton creation, save IO, audio, or backend is needed.
public class Level1NegotiationFlowTests : Level1NegotiationTestBase
{
    private static LocalTradeState Trade(int patience = 8)
    {
        return new LocalTradeState
        {
            spiceKey = "pepper", spiceDisplayName = "Pepper", quantityGrams = 1000,
            quantityLabel = "1 kg", npcOffer = 100, previousNpcOffer = 90,
            marketValue = 96, maxBuyerPrice = 180, buyerName = "Buyer", buyerOrigin = "Hampi",
            buyerPatience = patience, buyerTrust = 0.5f, buyerFrustration = 0.1f,
            minIncrement = 2, negotiationInitialized = true
        };
    }

    private RuleBasedNPCBrainResult Turn(string text, NegotiationStateManager manager, LocalTradeState trade)
    {
        return manager.ProcessLocalTurn(manager.ClassifyInput(text, trade), trade, new RuleBasedNPCBrain());
    }

    [TestCase("okay so 100 works")]
    [TestCase("yes so 100 is fine")]
    [TestCase("I accept the 100 you offered before")]
    [TestCase("I accept the 100 you offered earlier")]
    [TestCase("I accept the 100 you previously offered")]
    public void ExactCurrentOffer_WithDiscourseOrHistoryStillSettlesThatOffer(string text)
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        RuleBasedNPCBrainResult result = Turn(text, manager, trade);
        Assert.IsTrue(result.isAccepted);
        Assert.AreEqual(100, result.resolvedPrice);
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
    }

    [TestCase("okay so 100 works if you include delivery")]
    [TestCase("I don't accept the 100 you offered before")]
    [TestCase("I accept the 100 you offered before Monday")]
    [TestCase("okay so 100 works on Monday")]
    [TestCase("I accept the 100 you offered before, but make it 120")]
    [TestCase("yes if you make it 180")]
    public void DiscourseAndHistory_DoNotBypassAcceptanceSafety(string text)
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        NegotiationInput input = manager.ClassifyInput(text, trade);
        Assert.AreNotEqual(NegotiationIntent.ACCEPT, input.intent);
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isAccepted);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [TestCase(0)]
    [TestCase(900)]
    [TestCase(10)]
    public void AcceptOutstandingOffer_FreezesExactPriceAndQuantity(int referencePrice)
    {
        LocalTradeState trade = Trade();
        trade.referencePrice = referencePrice;
        trade.negotiationInitialized = false; // Also exercise lazy initialization.
        NegotiationStateManager manager = CreateManager(100);
        RuleBasedNPCBrainResult result = Turn("I accept 100", manager, trade);

        Assert.IsTrue(result.isAccepted);
        Assert.AreEqual(100, result.resolvedPrice);
        Assert.AreEqual(100, trade.AcceptedTerms.Price);
        Assert.AreEqual(1000, trade.AcceptedTerms.QuantityGrams);
        Assert.AreEqual(ExpectedReplyState.ExpectFulfillment, manager.CurrentExpectedReplyState);
        Assert.IsTrue(manager.IsNegotiationFinished);
    }

    [Test]
    public void AcceptanceUsesInterpretationSnapshot_EvenIfMutableOfferChangesBeforeDecision()
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        NegotiationInput input = manager.ClassifyInput("I accept 100", trade);
        trade.npcOffer = 170;
        trade.quantityGrams = 2000;
        trade.spiceKey = "clove";

        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.isAccepted);
        Assert.AreEqual(100, result.resolvedPrice);
        Assert.AreEqual(1000, result.resolvedQuantityGrams);
        Assert.AreEqual("pepper", trade.AcceptedTerms.SpiceKey);
        Assert.AreEqual(100, trade.npcOffer);
    }

    [Test]
    public void AcceptedTermsSurviveLaterInitializationAndQuantityDecision()
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        Turn("deal", manager, trade);
        TradeTermsSnapshot accepted = trade.AcceptedTerms;
        trade.referencePrice = 900;
        trade.negotiationInitialized = false;
        trade.npcOffer = 999; // Simulate an unrelated legacy write.
        trade.quantityGrams = 1;
        RuleBasedNPCBrainResult later = Turn("two seers for 150", manager, trade);
        trade.ApplyDecision(new RuleBasedNPCBrainResult { updatedOffer = 400, commitsQuantity = true, resolvedQuantityGrams = 9000 });

        Assert.AreSame(accepted, trade.AcceptedTerms);
        Assert.AreSame(accepted, later.acceptedTerms);
        Assert.AreEqual(100, later.resolvedPrice);
        Assert.AreEqual(1000, later.resolvedQuantityGrams);
        Assert.AreEqual(100, accepted.Price);
        Assert.AreEqual(1000, accepted.QuantityGrams);
        Assert.AreEqual(100, manager.LastOffer);
    }

    [TestCase("maybe two seers")]
    [TestCase("I accept if you include two seers")]
    public void AmbiguousTranscript_DoesNotChangeCommittedTerms(string text)
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        RuleBasedNPCBrainResult result = Turn(text, manager, trade);
        Assert.IsFalse(result.isAccepted);
        Assert.IsTrue(result.requiresClarification);
        Assert.AreEqual(1000, trade.quantityGrams);
        Assert.AreEqual("1 kg", trade.quantityLabel);
        Assert.AreEqual(96, trade.marketValue);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(0, trade.playerOfferHistory.Count);
    }

    [Test]
    public void ExtractedQuantityAndPrice_OnClarificationAreCandidatesOnly()
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        var input = new NegotiationInput
        {
            intent = NegotiationIntent.CLARIFICATION, needsClarification = true,
            hasQuantity = true, quantityGrams = 560, hasSellerPrice = true, sellerPrice = 150,
            expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter
        };
        manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.AreEqual(1000, trade.quantityGrams);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(0, trade.lastSellerPrice);
        Assert.AreEqual(0, trade.playerOfferHistory.Count);
        Assert.AreEqual(8, trade.buyerPatience);
        Assert.IsFalse(manager.IsNegotiationFinished);
    }

    [Test]
    public void BrainClarification_DoesNotCommitLowConfidenceProposalOrConsumeRound()
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.PRICE, hasSellerPrice = true,
            sellerPrice = 100, hasQuantity = true, quantityGrams = 560,
            parseConfidence = ParseConfidence.Low, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsTrue(result.requiresClarification);
        Assert.IsFalse(result.isAccepted);
        Assert.AreEqual(1000, trade.quantityGrams);
        Assert.AreEqual(100, trade.npcOffer);
        Assert.AreEqual(0, trade.lastSellerPrice);
        Assert.AreEqual(0, manager.CurrentRound);
        Assert.AreEqual(8, trade.buyerPatience);
    }

    [TestCase("I don't agree.")]
    [TestCase("I don't accept.")]
    [TestCase("I don't accept 100.")]
    [TestCase("I don't agree to 100.")]
    [TestCase("I do not really agree.")]
    [TestCase("I won't agree.")]
    [TestCase("I am not agreeing.")]
    [TestCase("I refuse to accept.")]
    [TestCase("I accept if you include delivery.")]
    [TestCase("I agree provided you deliver tomorrow.")]
    [TestCase("Unless you include delivery, I accept.")]
    [TestCase("Deal, but include delivery.")]
    [TestCase("I accept on credit.")]
    [TestCase("I accept on Monday.")]
    [TestCase("yes, 100 on Monday.")]
    [TestCase("deal for 100 plus transport.")]
    [TestCase("I accept with a guarantee.")]
    [TestCase("I accept and you pay tomorrow.")]
    [TestCase("I accept, actually wait.")]
    [TestCase("Maybe I accept.")]
    [TestCase("I accept 100 for cinnamon.")]
    [TestCase("I accept 100 for two seers.")]
    [TestCase("I accept if the caravan arrives.")]
    [TestCase("I offer 100 if the caravan arrives.")]
    public void UnsafeAcceptance_DoesNotCreateAgreement(string text)
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        NegotiationInput input = manager.ClassifyInput(text, trade);
        Assert.AreNotEqual(NegotiationIntent.ACCEPT, input.intent);
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isAccepted);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.AreNotEqual(ExpectedReplyState.ExpectFulfillment, manager.CurrentExpectedReplyState);
    }

    [TestCase(0, 1f)]
    [TestCase(8, 1f)]
    [TestCase(0, 0f)]
    public void ValidOutstandingAcceptance_WinsOverExhaustedPatienceAndFrustration(int patience, float frustration)
    {
        LocalTradeState trade = Trade(patience);
        trade.buyerFrustration = frustration;
        NegotiationStateManager manager = CreateManager(100);
        // Exhaust the old manager counter to verify it is not authoritative locally.
        for (int i = 0; i < 6; i++) manager.ProcessNegotiationTurn(NegotiationIntent.PRICE);
        Assert.IsTrue(manager.IsNegotiationFinished);
        RuleBasedNPCBrainResult result = Turn("yes", manager, trade);
        Assert.IsTrue(result.isAccepted);
        Assert.IsFalse(result.walkedAway);
        Assert.AreEqual(100, result.resolvedPrice);
    }

    [Test]
    public void QueryDoesNotResetPersonalityPatienceOrZeroEmotionOrRepriceOffer()
    {
        LocalTradeState trade = Trade(12);
        trade.buyerTrust = 0;
        trade.buyerFrustration = 0;
        trade.referencePrice = 900;
        NegotiationStateManager manager = CreateManager(100); // Default five must not overwrite twelve.
        Turn("what is your offer?", manager, trade);
        Turn("what is your offer?", manager, trade);
        Assert.AreEqual(12, trade.buyerPatience);
        Assert.AreEqual(12, manager.BuyerPatience);
        Assert.AreEqual(0, trade.buyerTrust);
        Assert.AreEqual(0, trade.buyerFrustration);
        Assert.AreEqual(100, trade.npcOffer);
    }

    [TestCase("I seek pepper. Name your price.")]
    [TestCase("Agreed. Bring the goods.")]
    [TestCase("We shall never trade again.")]
    public void RenderedWordingCannotChangeSemanticExpectedState(string replacement)
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        NegotiationInput input = manager.ClassifyInput("what is your offer?", trade);
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        var provider = new DialogueTableResponseProvider();
        result.replyText = provider.GetReply(input, trade, result, manager.CurrentRound);
        result.replyText = replacement; // Simulate any replacement authored template.
        manager.ApplyLocalDecision(result, trade);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
        Assert.IsFalse(manager.IsNegotiationFinished);
        Assert.IsNull(trade.AcceptedTerms);
    }

    [TestCase(100)]
    [TestCase(105)]
    [TestCase(150)]
    [TestCase(250)]
    public void EquivalentExplicitPriceAndCounter_HaveIdenticalEconomicDecisions(int price)
    {
        LocalTradeState first = Trade();
        LocalTradeState second = Trade();
        NegotiationStateManager firstManager = CreateManager(100, ExpectedReplyState.ExpectOfferPrice);
        NegotiationStateManager secondManager = CreateManager(100);
        var priceInput = new NegotiationInput { intent = NegotiationIntent.PRICE, hasSellerPrice = true,
            sellerPrice = price, parseConfidence = ParseConfidence.High, expectedReplyState = ExpectedReplyState.ExpectOfferPrice };
        var counterInput = new NegotiationInput { intent = NegotiationIntent.COUNTER, hasSellerPrice = true,
            sellerPrice = price, parseConfidence = ParseConfidence.High, expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        RuleBasedNPCBrainResult a = firstManager.ProcessLocalTurn(priceInput, first, new RuleBasedNPCBrain());
        RuleBasedNPCBrainResult b = secondManager.ProcessLocalTurn(counterInput, second, new RuleBasedNPCBrain());
        Assert.AreEqual(a.isAccepted, b.isAccepted);
        Assert.AreEqual(a.walkedAway, b.walkedAway);
        Assert.AreEqual(a.updatedOffer, b.updatedOffer);
        Assert.AreEqual(a.resolvedPrice, b.resolvedPrice);
        Assert.AreEqual(first.buyerPatience, second.buyerPatience);
        Assert.AreEqual(a.frustration, b.frustration);
        Assert.AreEqual(a.trust, b.trust);
        if (price == 100) Assert.IsTrue(a.isAccepted);
    }

    [Test]
    public void ValidQuantityChange_CommitsOnlyAfterDecision()
    {
        LocalTradeState trade = Trade();
        NegotiationStateManager manager = CreateManager(100);
        var input = new NegotiationInput { intent = NegotiationIntent.QUANTITY_CHANGE,
            hasQuantity = true, quantityGrams = 560, parseConfidence = ParseConfidence.High,
            expectedReplyState = ExpectedReplyState.ExpectAcceptOrCounter };
        Assert.AreEqual(1000, trade.quantityGrams);
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, trade, new RuleBasedNPCBrain());
        Assert.IsFalse(result.requiresClarification);
        Assert.AreEqual(560, trade.quantityGrams);
        Assert.IsNull(trade.AcceptedTerms);
        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void AcceptanceSnapshotFromDifferentSessionIsRejected()
    {
        LocalTradeState first = Trade();
        LocalTradeState second = Trade();
        NegotiationStateManager manager = CreateManager(100);
        NegotiationInput input = manager.ClassifyInput("deal", first);
        RuleBasedNPCBrainResult result = manager.ProcessLocalTurn(input, second, new RuleBasedNPCBrain());
        Assert.IsFalse(result.isAccepted);
        Assert.IsTrue(result.requiresClarification);
        Assert.IsNull(second.AcceptedTerms);
    }

    [Test]
    public void NegotiationPressureCostsOnePatience_ThenOutstandingOfferCanStillBeAccepted()
    {
        LocalTradeState trade = Trade(1);
        NegotiationStateManager manager = CreateManager(100);
        RuleBasedNPCBrainResult counter = Turn("give me 150", manager, trade);
        Assert.IsFalse(counter.isFinished);
        Assert.AreEqual(0, trade.buyerPatience);
        Assert.AreEqual(0, manager.BuyerPatience);
        int offeredPrice = trade.npcOffer;
        RuleBasedNPCBrainResult accepted = Turn("deal", manager, trade);
        Assert.IsTrue(accepted.isAccepted);
        Assert.AreEqual(offeredPrice, accepted.resolvedPrice);
    }

    [Test]
    public void ExhaustedPatience_StillEndsFurtherBargaining()
    {
        LocalTradeState trade = Trade(0);
        NegotiationStateManager manager = CreateManager(100);
        RuleBasedNPCBrainResult result = Turn("give me 150", manager, trade);
        Assert.IsTrue(result.walkedAway);
        Assert.IsTrue(manager.IsNegotiationFinished);
        Assert.AreEqual(ExpectedReplyState.None, manager.CurrentExpectedReplyState);
        Assert.IsNull(trade.AcceptedTerms);
    }
}
