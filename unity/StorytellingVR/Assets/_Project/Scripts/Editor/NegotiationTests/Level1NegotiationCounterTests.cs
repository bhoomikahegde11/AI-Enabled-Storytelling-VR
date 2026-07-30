using NUnit.Framework;

public class Level1NegotiationCounterTests : Level1NegotiationTestBase
{
    [TestCase("140")]
    [TestCase("I want 140")]
    [TestCase("I need 140")]
    [TestCase("I ask 140")]
    [TestCase("I am asking 140")]
    [TestCase("make it 140")]
    [TestCase("give me 140")]
    [TestCase("give 140")]
    [TestCase("can you do 140")]
    [TestCase("what about 140")]
    [TestCase("how about 140")]
    [TestCase("my price is 140")]
    [TestCase("my offer is 140")]
    [TestCase("I will take 140")]
    [TestCase("I can sell for 140")]
    [TestCase("I will sell for 140")]
    [TestCase("leave it at 140")]
    [TestCase("keep it at 140")]
    [TestCase("settle at 140")]
    [TestCase("final price 140")]
    [TestCase("final offer 140")]
    [TestCase("140 only")]
    [TestCase("only 140")]
    [TestCase("no less than 140")]
    [TestCase("at least 140")]
    [TestCase("I will give it for 140")]
    [TestCase("let us say 140")]
    [TestCase("call it 140")]
    [TestCase("make that 140")]
    [TestCase("put it at 140")]
    [TestCase("set it at 140")]
    [TestCase("I am firm at 140")]
    [TestCase("140 is my price")]
    [TestCase("140 is my final")]
    [TestCase("my final is 140")]
    [TestCase("I need at least 140")]
    [TestCase("I can do 140")]
    [TestCase("I would accept 140")]
    [TestCase("I will agree at 140")]
    public void DirectCounteroffers_ParseToCounter140(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125, ExpectedReplyState.ExpectAcceptOrCounter, 220);
        AssertCounter(result, 140);
    }

    [TestCase("no, not 120, I said 140", 120)]
    [TestCase("I want 140, not 120", 120)]
    [TestCase("not 120, make it 140", 120)]
    [TestCase("I meant 140, not 120", 120)]
    [TestCase("not 130, 140", 130)]
    [TestCase("120 was wrong, use 140", 120)]
    [TestCase("forget 120, I want 140", 120)]
    [TestCase("change my offer from 120 to 140", 120)]
    [TestCase("scratch 120, use 140", 120)]
    [TestCase("I said 140, not 120", 120)]
    [TestCase("replace 120 with 140", 120)]
    [TestCase("make it 140 instead of 120", 120)]
    public void Corrections_SelectFinalActionablePrice(string utterance, int replacedValue)
    {
        NegotiationInput result = Parse(utterance, 125, ExpectedReplyState.ExpectAcceptOrCounter, 220);
        AssertCounter(result, 140);
        Assert.AreEqual(replacedValue, result.rejectedPrice);
        AssertReferencedPrices(result, replacedValue);
    }

    [TestCase("I wanted 150, but I can do 140", 140, 150)]
    [TestCase("I wanted 150, but I can come down to 140", 140, 150)]
    [TestCase("150 was my price, though I will take 140", 140, 150)]
    [TestCase("I asked 150, but make it 140", 140, 150)]
    [TestCase("I can lower it from 150 to 140", 140, 150)]
    [TestCase("my first price was 150; final price is 140", 140, 150)]
    [TestCase("I will reduce it to 140", 140, 0)]
    [TestCase("alright, I can settle at 140", 140, 0)]
    [TestCase("I cannot do 130, but I can do 140", 140, 130)]
    [TestCase("150 ideally, 140 at the lowest", 140, 150)]
    [TestCase("I wanted 160, but I will accept 145", 145, 160)]
    [TestCase("I asked for 155, though I can agree to 145", 145, 155)]
    [TestCase("my original price was 150; I can lower it to 140", 140, 150)]
    [TestCase("I will come down from 150 to 140", 140, 150)]
    [TestCase("fine, I can meet you at 140", 140, 0)]
    [TestCase("I can compromise at 140", 140, 0)]
    [TestCase("I will reduce my ask to 140", 140, 0)]
    [TestCase("my lowest is 140", 140, 0)]
    [TestCase("140 is as low as I can go", 140, 0)]
    [TestCase("I started at 150, but 140 is final", 140, 150)]
    public void Concessions_ChooseFinalPrice(string utterance, int expectedPrice, int referenced)
    {
        NegotiationInput result = Parse(utterance, 125, ExpectedReplyState.ExpectAcceptOrCounter, 220);
        AssertCounter(result, expectedPrice);
        if (referenced > 0)
        {
            AssertReferencedPrices(result, referenced);
        }
    }

    [Test]
    public void ConcessionThatAcceptsCurrentOffer_BecomesAccept()
    {
        NegotiationInput result = Parse("I wanted 150, but I accept 140", 140, ExpectedReplyState.ExpectAcceptOrCounter, 220);
        AssertAccept(result, 140);
    }

    [TestCase("first you offered 110, then 125, I want 140", 140, 110, 125)]
    [TestCase("you said 100 before and 120 now; give me 140", 140, 100, 120)]
    [TestCase("I asked 160, you offered 120, let us settle at 140", 140, 160, 120)]
    [TestCase("not 110 or 120; I want 140", 140, 110, 120)]
    [TestCase("110 was too low, 125 was better, 140 is my final price", 140, 110, 125)]
    [TestCase("yesterday it was 100, earlier 120, now I need 140", 140, 100, 120)]
    [TestCase("you moved from 110 to 125; I can come down from 150 to 140", 140, 110, 125)]
    [TestCase("I said 150 first, then 145, but 140 is final", 140, 150, 145)]
    [TestCase("your 125 is low; I wanted 160, but I can do 145", 145, 125, 160)]
    [TestCase("100, 110, 120 none work; make it 140", 140, 100, 120)]
    [TestCase("first 100, then 110, then 120; I want 140", 140, 100, 120)]
    [TestCase("you offered 120, I asked 160, meet me at 140", 140, 120, 160)]
    [TestCase("my opening was 150, your last was 125, final 140", 140, 150, 125)]
    [TestCase("you said 110 before, 125 now, but my price is 140", 140, 110, 125)]
    [TestCase("I considered 135, but I need 140", 140, 135, 0)]
    [TestCase("not 120, not 130, 140", 140, 120, 130)]
    [TestCase("from 150 I came down to 145; now I can do 140", 140, 150, 145)]
    [TestCase("your 100 and 120 offers were too low; give me 140", 140, 100, 120)]
    [TestCase("we discussed 130, but settle at 140", 140, 130, 0)]
    [TestCase("ignore the old 120; my final is 140", 140, 120, 0)]
    public void MultiplePricesAndHistory_SelectFinalActionablePrice(string utterance, int expectedPrice, int firstReference, int secondReference)
    {
        NegotiationInput result = Parse(utterance, 125, ExpectedReplyState.ExpectAcceptOrCounter, 220);
        AssertCounter(result, expectedPrice);
        if (firstReference > 0)
        {
            AssertReferencedPrices(result, firstReference);
        }
        if (secondReference > 0)
        {
            AssertReferencedPrices(result, secondReference);
        }
    }
}
