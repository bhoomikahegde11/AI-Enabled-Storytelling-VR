using NUnit.Framework;

public class Level1NegotiationParserTests
{
    private static LocalTradeState CreateTrade(int offer = 111, int maxOffer = 140)
    {
        return new LocalTradeState
        {
            spiceKey = "pepper",
            spiceDisplayName = "Pepper",
            quantityGrams = 280,
            quantityLabel = "1 Seer (~280g)",
            npcOffer = offer,
            previousNpcOffer = offer,
            maxBuyerPrice = maxOffer,
            marketValue = 100
        };
    }

    private static LocalTradeState CreateAcceptanceTrade()
    {
        return CreateTrade(173, 200);
    }

    [TestCase("no go away")]
    [TestCase("go away")]
    [TestCase("please leave")]
    public void ExplicitDismissPhrases_ClassifyAsDismiss(string input)
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput(input, CreateTrade());

        Assert.AreEqual(NegotiationIntent.DISMISS, result.intent);
        Assert.IsTrue(result.terminalAction);
    }

    [TestCase("forget it")]
    [TestCase("no deal")]
    [TestCase("i do not want to sell")]
    [TestCase("we are done")]
    public void ExplicitHardRejectPhrases_ClassifyAsHardReject(string input)
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput(input, CreateTrade());

        Assert.AreEqual(NegotiationIntent.REJECT, result.intent);
        Assert.IsTrue(result.hasHardRejection);
        Assert.IsTrue(result.terminalAction);
    }

    [Test]
    public void LeaveItAt140_DoesNotBecomeDismiss()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("leave it at 140", CreateTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        Assert.IsFalse(result.needsClarification);
    }

    [Test]
    public void LeaveThePriceAt140_DoesNotBecomeDismiss()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I will leave the price at 140", CreateTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        Assert.IsFalse(result.needsClarification);
    }

    [Test]
    public void LeaveTheOfferAt140_DoesNotBecomeDismiss()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("leave the offer at 140", CreateTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        Assert.IsFalse(result.terminalAction);
        Assert.IsFalse(result.needsClarification);
    }

    [Test]
    public void LeaveNow_RemainsDismiss()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("leave now", CreateTrade());

        Assert.AreEqual(NegotiationIntent.DISMISS, result.intent);
    }

    [Test]
    public void LeaveIt_WithoutNumber_DoesNotBecomePriceOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("leave it", CreateTrade());

        Assert.AreNotEqual(NegotiationIntent.COUNTER, result.intent);
    }

    [Test]
    public void ThatDealSoundsFine_ClassifiesAsAccept()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.UpdateExpectedReplyStateFromNpcReply("I can offer 111 varahas.", false, false);

        NegotiationInput result = manager.ClassifyInput("that deal sounds fine", CreateTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void NotBadDeal_ClassifiesAsAccept()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.UpdateExpectedReplyStateFromNpcReply("I can offer 111 varahas.", false, false);

        NegotiationInput result = manager.ClassifyInput("not bad deal", CreateTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void No145_BecomesCounterOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("no 145", CreateTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(145, result.sellerPrice);
    }

    [Test]
    public void NoDealAt100MakeIt140_BecomesCounterOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("no deal at 100, make it 140", CreateTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        CollectionAssert.Contains(result.referencedPrices, 100);
    }

    [Test]
    public void WhatDoYouWant_IsItemQuery()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, "test");

        NegotiationInput result = manager.ClassifyInput("what do you want", CreateTrade());

        Assert.AreEqual(NegotiationIntent.ITEM_QUERY, result.intent);
    }

    [Test]
    public void WhatAreYouLookingFor_IsItemQuery()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, "test");

        NegotiationInput result = manager.ClassifyInput("what are you looking for", CreateTrade());

        Assert.AreEqual(NegotiationIntent.ITEM_QUERY, result.intent);
        Assert.AreEqual(-1, result.sellerPrice);
        StringAssert.Contains("for", result.normalizedText);
        StringAssert.DoesNotContain("four", result.normalizedText);
    }

    [Test]
    public void WhatSpiceWhatPrice_PrioritizesPriceSideQuery()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectOfferPrice, "test");

        NegotiationInput result = manager.ClassifyInput("what spice what price", CreateTrade());

        Assert.AreEqual(NegotiationIntent.PRICE_QUERY, result.intent);
    }

    [Test]
    public void Huh_IsConfused()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("huh", CreateTrade());

        Assert.AreEqual(NegotiationIntent.CONFUSED, result.intent);
    }

    [Test]
    public void UnintelligibleText_BecomesClarification()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("blorpy snargle", CreateTrade());

        Assert.AreEqual(NegotiationIntent.CLARIFICATION, result.intent);
        Assert.AreEqual(ParseReason.UnrecognizedSpeech, result.parseReason);
    }

    [Test]
    public void NpcOfferReply_SetsExpectAcceptOrCounter()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        manager.UpdateExpectedReplyStateFromNpcReply("For a fair journey's trade, I can offer 111 varahas.", false, false);

        Assert.AreEqual(ExpectedReplyState.ExpectAcceptOrCounter, manager.CurrentExpectedReplyState);
    }

    [Test]
    public void LongFormCounter_ExtractsFinalActionablePrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("First you offered 110, then 125. I want 140.", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        CollectionAssert.Contains(result.referencedPrices, 110);
        CollectionAssert.Contains(result.referencedPrices, 125);
    }

    [Test]
    public void ConcessionStatement_UsesLowerFinalPrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I wanted 150, but I can come down to 140.", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        CollectionAssert.Contains(result.referencedPrices, 150);
    }

    [Test]
    public void CorrectionStatement_PrefersCorrectedPrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(120);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("No, not 120. I said 140.", CreateTrade(120, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        Assert.AreEqual(120, result.rejectedPrice);
        Assert.IsTrue(result.correctionDetected);
    }

    [Test]
    public void HistoricalReferenceWithoutAsk_BecomesClarification()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("You said 130 earlier.", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.CLARIFICATION, result.intent);
        Assert.AreEqual(ClarificationKind.HistoricalPriceOnly, result.clarificationKind);
        CollectionAssert.Contains(result.referencedPrices, 130);
    }

    [Test]
    public void CanYouDo140_BecomesCounterOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("Can you do 140?", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
    }

    [Test]
    public void WhatAbout140_BecomesContextualCounterOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("What about 140?", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
    }

    [Test]
    public void ContrastiveAcceptance_AcceptsCurrentOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.UpdateExpectedReplyStateFromNpcReply("I can offer 125 varahas.", false, false);

        NegotiationInput result = manager.ClassifyInput("That is lower than I wanted, but fine.", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(125, result.acceptanceTarget);
    }

    [Test]
    public void FineButOnlyAt135_RemainsCounterOffer()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.UpdateExpectedReplyStateFromNpcReply("I can offer 125 varahas.", false, false);

        NegotiationInput result = manager.ClassifyInput("Fine, but only at 135.", CreateTrade(125, 150));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(135, result.sellerPrice);
    }

    [Test]
    public void AcceptEarlierOffer_TracksAcceptanceTarget()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(125);
        manager.UpdateExpectedReplyStateFromNpcReply("I can offer 130 varahas.", false, false);

        NegotiationInput result = manager.ClassifyInput("I accept the 130 you offered before.", CreateTrade(130, 150));

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(130, result.acceptanceTarget);
    }

    [Test]
    public void MultiIntentQuestion_PreservesItemAndPrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("What spice do you want and what will you pay?", CreateTrade());

        Assert.AreEqual(NegotiationIntent.PRICE_QUERY, result.intent);
        CollectionAssert.Contains(result.secondaryIntents, NegotiationIntent.ITEM_QUERY);
    }

    [Test]
    public void TradeOpeningQuery_IsNotClarification()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("How can I help you?", CreateTrade());

        Assert.AreEqual(NegotiationIntent.ITEM_QUERY, result.intent);
        Assert.IsTrue(result.tradeOpeningQuery);
    }

    [Test]
    public void CanYouExplainYourOffer_IsNotItemQuery()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);

        NegotiationInput result = manager.ClassifyInput("Can you explain your offer?", CreateTrade());

        Assert.AreNotEqual(NegotiationIntent.ITEM_QUERY, result.intent);
    }

    [Test]
    public void SoftBargainNaturalLine_StaysBargain()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(111);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I am not saying no, but improve the offer.", CreateTrade());

        Assert.AreEqual(NegotiationIntent.BARGAIN, result.intent);
    }

    [Test]
    public void TooLowGiveMe145_PicksCorrectivePrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(120);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("130 is too low, give me 145", CreateTrade(120, 160));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(145, result.sellerPrice);
        CollectionAssert.Contains(result.referencedPrices, 130);
    }

    [Test]
    public void IWant140Not120_PicksRequestedPrice()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(120);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I want 140, not 120", CreateTrade(120, 160));

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(140, result.sellerPrice);
        Assert.AreEqual(120, result.rejectedPrice);
    }

    [Test]
    public void YesOkayExactPrice_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("yes ok 173 is fine", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
        Assert.AreEqual(-1, result.sellerPrice);
        Assert.IsFalse(result.needsClarification);
    }

    [Test]
    public void ExactPriceSeemsFair_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("ok 173 seems fair", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void ExactPriceWorks_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("173 works", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
        Assert.AreEqual(-1, result.sellerPrice);
    }

    [Test]
    public void ExactPriceIsFine_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("173 is fine", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void FineExactPriceWorks_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("fine 173 works for me", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void ExplicitAcceptExactPrice_IsAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I accept your 173", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void BareYes_AcceptsCurrentOfferInExpectedState()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("yes", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void DifferentAffirmativePrice_IsCounter()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("yes okay 180 is fine", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(180, result.sellerPrice);
    }

    [Test]
    public void AffirmativeWithNewCounterCue_IsCounter()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("yes if you make it 180", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(180, result.sellerPrice);
    }

    [Test]
    public void ExactPriceTooLow_IsNotAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("173 is too low", CreateAcceptanceTrade());

        Assert.AreNotEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void NegatedExactPrice_IsNotAcceptance()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("not 173", CreateAcceptanceTrade());

        Assert.AreNotEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void BareExactCurrentOffer_Accepts()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("173", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(173, result.acceptanceTarget);
    }

    [Test]
    public void DifferentBarePrice_IsCounter()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("180", CreateAcceptanceTrade());

        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(180, result.sellerPrice);
    }

    [Test]
    public void ExactPriceQuestion_DoesNotAutoAccept()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("173?", CreateAcceptanceTrade());

        Assert.AreNotEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void NegatedExactPrice_DoesNotAccept()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(173);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("not 173", CreateAcceptanceTrade());

        Assert.AreNotEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    [Test]
    public void ISaid140Earlier_IsHistoricalReferenceNotCounter()
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(120);
        manager.SetExpectedReplyState(ExpectedReplyState.ExpectAcceptOrCounter, "test");

        NegotiationInput result = manager.ClassifyInput("I said 140 earlier", CreateTrade(120, 160));

        Assert.AreEqual(NegotiationIntent.CLARIFICATION, result.intent);
        Assert.AreEqual(ClarificationKind.HistoricalPriceOnly, result.clarificationKind);
    }
}

public class Level1InputNormalizerTests
{
    [Test]
    public void AndInTradeQuestion_DoesNotBecomeZero()
    {
        string normalized = InputNormalizer.Normalize("What spice do you want and what will you pay?");

        StringAssert.DoesNotContain(" 0 ", " " + normalized + " ");
    }

    [Test]
    public void OneHundredAndForty_StillNormalizes()
    {
        string normalized = InputNormalizer.Normalize("one hundred and forty", true);

        Assert.AreEqual("140", normalized);
    }

    [Test]
    public void OneHundredAndFive_StillNormalizes()
    {
        string normalized = InputNormalizer.Normalize("one hundred and five", true);

        Assert.AreEqual("105", normalized);
    }

    [Test]
    public void ClovesAndPepper_DoesNotGainNumber()
    {
        string normalized = InputNormalizer.Normalize("cloves and pepper");

        StringAssert.DoesNotContain("0", normalized);
    }

    [Test]
    public void FirstOffered_RemainsIntact()
    {
        string normalized = InputNormalizer.Normalize("First you offered 110");

        StringAssert.Contains("first", normalized);
    }

    [Test]
    public void StandardPrice_RemainsIntact()
    {
        string normalized = InputNormalizer.Normalize("standard price");

        StringAssert.Contains("standard", normalized);
    }

    [Test]
    public void WhatIsThisFor_DoesNotBecomeFour()
    {
        string normalized = InputNormalizer.Normalize("what is this for");

        StringAssert.Contains("for", normalized);
        StringAssert.DoesNotContain("four", normalized);
    }

    [Test]
    public void FourVarahas_StillNormalizesToFour()
    {
        string normalized = InputNormalizer.Normalize("four varahas", true);

        Assert.AreEqual("4 varahas", normalized);
    }
}
