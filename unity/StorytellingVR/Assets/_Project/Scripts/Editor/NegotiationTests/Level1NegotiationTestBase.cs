using System.Linq;
using NUnit.Framework;

public abstract class Level1NegotiationTestBase
{
    protected NegotiationStateManager CreateManager(int startingOffer = 173, ExpectedReplyState state = ExpectedReplyState.ExpectAcceptOrCounter)
    {
        NegotiationStateManager manager = new NegotiationStateManager();
        manager.ResetState(startingOffer);
        manager.SetExpectedReplyState(state, "test");
        return manager;
    }

    protected LocalTradeState CreateTrade(int npcOffer = 173, int maxOffer = 220)
    {
        return new LocalTradeState
        {
            spiceKey = "pepper",
            spiceDisplayName = "Pepper",
            quantityGrams = 280,
            quantityLabel = "1 Seer (~280g)",
            npcOffer = npcOffer,
            previousNpcOffer = npcOffer,
            maxBuyerPrice = maxOffer,
            marketValue = 100
        };
    }

    protected NegotiationInput Parse(string utterance, int npcOffer = 173, ExpectedReplyState state = ExpectedReplyState.ExpectAcceptOrCounter, int maxOffer = 220)
    {
        NegotiationStateManager manager = CreateManager(npcOffer, state);
        return manager.ClassifyInput(utterance, CreateTrade(npcOffer, maxOffer));
    }

    protected NegotiationInput ParseWithManager(string utterance, NegotiationStateManager manager, int npcOffer = 173, int maxOffer = 220)
    {
        return manager.ClassifyInput(utterance, CreateTrade(npcOffer, maxOffer));
    }

    protected static void AssertAccept(NegotiationInput result, int expectedAcceptanceTarget)
    {
        Assert.AreEqual(NegotiationIntent.ACCEPT, result.intent);
        Assert.AreEqual(expectedAcceptanceTarget, result.acceptanceTarget);
        Assert.AreEqual(-1, result.sellerPrice);
        Assert.IsFalse(result.needsClarification);
        Assert.IsFalse(result.terminalAction);
    }

    protected static void AssertCounter(NegotiationInput result, int expectedPrice)
    {
        Assert.AreEqual(NegotiationIntent.COUNTER, result.intent);
        Assert.AreEqual(expectedPrice, result.sellerPrice);
        Assert.LessOrEqual(result.acceptanceTarget, 0);
        Assert.IsFalse(result.needsClarification);
        Assert.IsFalse(result.terminalAction);
    }

    protected static void AssertClarification(NegotiationInput result)
    {
        Assert.AreEqual(NegotiationIntent.CLARIFICATION, result.intent);
        Assert.IsTrue(result.needsClarification);
        Assert.IsFalse(result.terminalAction);
    }

    protected static void AssertNotAccept(NegotiationInput result)
    {
        Assert.AreNotEqual(NegotiationIntent.ACCEPT, result.intent);
    }

    protected static void AssertIntent(NegotiationInput result, NegotiationIntent expectedIntent)
    {
        Assert.AreEqual(expectedIntent, result.intent);
    }

    protected static void AssertReferencedPrices(NegotiationInput result, params int[] expected)
    {
        foreach (int value in expected)
        {
            CollectionAssert.Contains(result.referencedPrices, value);
        }
    }

    protected static void AssertNoReferencedPrice(NegotiationInput result, int value)
    {
        CollectionAssert.DoesNotContain(result.referencedPrices, value);
    }

    protected static void AssertNoInventedPrice(NegotiationInput result)
    {
        Assert.LessOrEqual(result.sellerPrice, 0);
    }

    protected static void AssertNonTerminal(NegotiationInput result)
    {
        Assert.IsFalse(result.terminalAction);
    }

    protected static void AssertTerminal(NegotiationInput result)
    {
        Assert.IsTrue(result.terminalAction);
    }
}
