using NUnit.Framework;

public class Level1NegotiationSafetyTests : Level1NegotiationTestBase
{
    [TestCase("improve the offer")]
    [TestCase("can you do better")]
    [TestCase("offer me more")]
    [TestCase("that is too little")]
    [TestCase("raise your offer")]
    [TestCase("come up a bit")]
    [TestCase("give me a fairer price")]
    [TestCase("you can do better than that")]
    [TestCase("not enough")]
    [TestCase("a little more")]
    [TestCase("meet me halfway")]
    [TestCase("make it worth my while")]
    [TestCase("I am not saying no, but improve it")]
    [TestCase("close the gap")]
    [TestCase("increase it slightly")]
    [TestCase("try again")]
    [TestCase("that offer needs work")]
    [TestCase("give me something better")]
    [TestCase("move higher")]
    [TestCase("raise it a little")]
    [TestCase("we are too far apart")]
    [TestCase("come closer to my price")]
    [TestCase("sweeten the deal")]
    [TestCase("offer a fair amount")]
    [TestCase("you need to improve")]
    public void SoftBargaining_PhrasesStayBargain(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.AreEqual(NegotiationIntent.BARGAIN, result.intent);
        AssertNoInventedPrice(result);
        AssertNonTerminal(result);
    }

    [TestCase("make the deal better")]
    [TestCase("improve the deal")]
    [TestCase("give me a better deal")]
    [TestCase("make this deal better")]
    [TestCase("make the deal fairer")]
    [TestCase("give me a fairer deal")]
    [TestCase("can you improve the deal")]
    [TestCase("you need to improve the deal")]
    public void DealImprovement_PhrasesStayBargain(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.AreEqual(NegotiationIntent.BARGAIN, result.intent);
        AssertNoInventedPrice(result);
        AssertNonTerminal(result);
    }

    [TestCase("no")]
    [TestCase("no thanks")]
    [TestCase("not good enough")]
    [TestCase("too low")]
    [TestCase("too high")]
    [TestCase("I cannot take that")]
    [TestCase("that will not work")]
    [TestCase("unacceptable")]
    [TestCase("I reject that offer")]
    [TestCase("no deal at that price")]
    [TestCase("not interested at that amount")]
    [TestCase("I need better")]
    [TestCase("I cannot agree")]
    [TestCase("that price is impossible")]
    [TestCase("I will pass on that offer")]
    public void SoftRejection_PhrasesAreNonTerminal(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.AreEqual(NegotiationIntent.REJECT, result.intent);
        AssertNonTerminal(result);
    }

    [TestCase("forget it")]
    [TestCase("no deal")]
    [TestCase("we are done")]
    [TestCase("I do not want to sell")]
    [TestCase("end this trade")]
    [TestCase("cancel the deal")]
    [TestCase("I am finished")]
    [TestCase("no sale")]
    [TestCase("stop bargaining")]
    [TestCase("this negotiation is over")]
    [TestCase("there will be no trade")]
    [TestCase("I am walking away")]
    [TestCase("forget the whole thing")]
    [TestCase("end the negotiation")]
    [TestCase("we have nothing more to discuss")]
    public void HardRejection_PhrasesAreTerminal(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.AreEqual(NegotiationIntent.REJECT, result.intent);
        AssertTerminal(result);
    }

    [TestCase("go away")]
    [TestCase("please leave")]
    [TestCase("leave now")]
    [TestCase("get out")]
    [TestCase("move along")]
    [TestCase("I do not want to talk")]
    [TestCase("leave my stall")]
    [TestCase("be gone")]
    [TestCase("enough, go")]
    [TestCase("no, go away")]
    [TestCase("walk away")]
    [TestCase("get away from here")]
    [TestCase("leave me alone")]
    [TestCase("go bother someone else")]
    [TestCase("out of my stall")]
    public void Dismissal_PhrasesAreTerminal(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.AreEqual(NegotiationIntent.DISMISS, result.intent);
        AssertTerminal(result);
    }

    [TestCase("leave it at 140")]
    [TestCase("leave the price at 140")]
    [TestCase("leave the offer at 140")]
    [TestCase("I will leave it at 140")]
    [TestCase("leave my price at 140")]
    [TestCase("leave the amount at 140")]
    [TestCase("let us leave it at 140")]
    public void LeavePriceVariants_AreCountersNotDismissals(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        AssertCounter(result, 140);
    }

    [TestCase("huh")]
    [TestCase("what")]
    [TestCase("what?")]
    [TestCase("sorry?")]
    [TestCase("I do not understand")]
    [TestCase("say that again")]
    [TestCase("repeat that")]
    [TestCase("unclear")]
    [TestCase("blorpy snargle")]
    [TestCase("uh what was that")]
    [TestCase("I missed that")]
    [TestCase("come again")]
    [TestCase("what are you talking about")]
    [TestCase("no idea")]
    [TestCase("hmm?")]
    [TestCase("sorry, I did not hear")]
    [TestCase("could you repeat")]
    [TestCase("I am confused")]
    [TestCase("that made no sense")]
    [TestCase("what do you mean")]
    public void ConfusionAndGibberish_RequestClarification(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        Assert.Contains(result.intent, new[] { NegotiationIntent.CONFUSED, NegotiationIntent.CLARIFICATION });
        Assert.IsTrue(result.needsClarification);
        AssertNonTerminal(result);
    }
}
