using NUnit.Framework;

public class Level1NegotiationQuestionTests : Level1NegotiationTestBase
{
    [TestCase("not 173")]
    [TestCase("no, not 173")]
    [TestCase("173 is not fine")]
    [TestCase("173 does not work")]
    [TestCase("I cannot accept 173")]
    [TestCase("I will not take 173")]
    [TestCase("anything but 173")]
    [TestCase("definitely not 173")]
    [TestCase("do not settle at 173")]
    [TestCase("I reject 173")]
    [TestCase("I refuse 173")]
    [TestCase("173 is unacceptable")]
    [TestCase("173 is impossible")]
    [TestCase("173 will not do")]
    [TestCase("no deal at 173")]
    [TestCase("173 is too low")]
    [TestCase("173 is too high")]
    [TestCase("173 is unfair")]
    [TestCase("173 is terrible")]
    [TestCase("173 is not enough")]
    [TestCase("173 is too much")]
    [TestCase("173 is a bad price")]
    [TestCase("173 needs improvement")]
    public void NegationAndCriticism_DoNotAutoAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertNotAccept(result);
    }

    [TestCase("173?")]
    [TestCase("173, right?")]
    [TestCase("was it 173?")]
    [TestCase("did you say 173?")]
    [TestCase("you mean 173?")]
    [TestCase("are you offering 173?")]
    [TestCase("is 173 your final offer?")]
    [TestCase("173 or something else?")]
    [TestCase("was your offer 173?")]
    [TestCase("are we at 173?")]
    public void ExactPriceConfirmationQuestions_AreNotCommittingCounters(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertNotAccept(result);
        Assert.AreNotEqual(NegotiationIntent.COUNTER, result.intent);
    }

    [TestCase("what about 173")]
    [TestCase("can you do 173")]
    public void SamePriceCounterStyleQuestions_AreNotAcceptance(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertNotAccept(result);
    }

    [TestCase("you said 173 earlier")]
    [TestCase("your previous offer was 173")]
    [TestCase("first you offered 173")]
    [TestCase("I remember 173")]
    [TestCase("before, you said 173")]
    [TestCase("the last offer was 173")]
    [TestCase("we discussed 173")]
    [TestCase("you mentioned 173")]
    public void HistoricalCurrentOfferReferences_DoNotAutoAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertNotAccept(result);
    }

    [TestCase("how much")]
    [TestCase("what price")]
    [TestCase("what is your offer")]
    [TestCase("what will you pay")]
    [TestCase("how many varahas")]
    [TestCase("your price?")]
    [TestCase("what are you offering")]
    [TestCase("what is the current offer")]
    [TestCase("repeat your offer")]
    [TestCase("how much did you say")]
    [TestCase("what was your last offer")]
    [TestCase("is that your final offer")]
    [TestCase("what is the best you can do")]
    [TestCase("what is your highest offer")]
    [TestCase("what did you offer before")]
    [TestCase("what are we at now")]
    [TestCase("what amount are you proposing")]
    [TestCase("how much for the spices")]
    [TestCase("what is your buying price")]
    [TestCase("tell me your offer again")]
    public void PriceQuestions_ReturnQueryLikeIntent(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.Contains(result.intent, new[]
        {
            NegotiationIntent.PRICE_QUERY,
            NegotiationIntent.QUERY_BUYER_BUDGET,
            NegotiationIntent.BARGAIN,
            NegotiationIntent.CLARIFICATION
        });
    }

    [TestCase("can you improve your offer")]
    [TestCase("why only 125")]
    [TestCase("why is your offer so low")]
    [TestCase("how did you decide that price")]
    [TestCase("are you firm on that")]
    [TestCase("is that all")]
    [TestCase("can you offer more")]
    public void PricePressureQuestions_AreNotAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        AssertNotAccept(result);
    }

    [TestCase("what do you want")]
    [TestCase("what spice do you want")]
    [TestCase("which item")]
    [TestCase("which spice")]
    [TestCase("cloves or pepper")]
    [TestCase("what are you buying")]
    [TestCase("what are you looking for")]
    [TestCase("which goods")]
    [TestCase("what do you need")]
    [TestCase("what item are you after")]
    [TestCase("do you want cloves")]
    [TestCase("do you want pepper")]
    [TestCase("are you buying spices")]
    [TestCase("which product")]
    [TestCase("what should I prepare")]
    [TestCase("tell me the item")]
    public void ItemQuestions_ReturnItemIntent(string utterance)
    {
        NegotiationInput result = Parse(utterance, 173, ExpectedReplyState.ExpectOfferPrice);
        Assert.AreEqual(NegotiationIntent.ITEM_QUERY, result.intent);
    }

    [TestCase("what spice do you want and what will you pay")]
    [TestCase("which item and what price")]
    [TestCase("what quantity and what offer")]
    [TestCase("cloves or pepper, and how much")]
    [TestCase("what are you buying and what is your price")]
    [TestCase("how many sacks and how many varahas")]
    [TestCase("what spice, what quantity, and what offer")]
    [TestCase("what do you want, and can you pay 140")]
    [TestCase("which goods, and what will you offer")]
    [TestCase("what item are you buying, and why so little")]
    public void MultiIntentQuestions_PreserveTradeQueryMeaning(string utterance)
    {
        NegotiationInput result = Parse(utterance, 173, ExpectedReplyState.ExpectOfferPrice);
        Assert.Contains(result.intent, new[] { NegotiationIntent.PRICE_QUERY, NegotiationIntent.QUERY_BUYER_BUDGET, NegotiationIntent.ITEM_QUERY });
    }

    [Test]
    public void StateDependent_NumberBehavesDifferentlyByState()
    {
        AssertAccept(Parse("173", 173, ExpectedReplyState.ExpectAcceptOrCounter), 173);
        AssertCounter(Parse("173", 160, ExpectedReplyState.ExpectAcceptOrCounter), 173);
        Assert.AreEqual(NegotiationIntent.COUNTER, Parse("173", 160, ExpectedReplyState.ExpectOfferPrice).intent);
        Assert.AreNotEqual(NegotiationIntent.UNKNOWN, Parse("173", 160, ExpectedReplyState.None).intent);
    }

    [Test]
    public void StateDependent_YesBehavesDifferentlyByState()
    {
        AssertAccept(Parse("yes", 173, ExpectedReplyState.ExpectAcceptOrCounter), 173);
        Assert.AreNotEqual(NegotiationIntent.ACCEPT, Parse("yes", 173, ExpectedReplyState.ExpectOfferPrice).intent);
        Assert.AreNotEqual(NegotiationIntent.UNKNOWN, Parse("yes", 173, ExpectedReplyState.None).intent);
    }

    [Test]
    public void StateDependent_ExactPriceWorksBehavesDifferentlyByState()
    {
        AssertAccept(Parse("173 works", 173, ExpectedReplyState.ExpectAcceptOrCounter), 173);
        AssertCounter(Parse("173 works", 160, ExpectedReplyState.ExpectAcceptOrCounter), 173);
        Assert.AreEqual(NegotiationIntent.COUNTER, Parse("173 works", 160, ExpectedReplyState.ExpectOfferPrice).intent);
        Assert.AreNotEqual(NegotiationIntent.UNKNOWN, Parse("173 works", 160, ExpectedReplyState.None).intent);
    }
}
