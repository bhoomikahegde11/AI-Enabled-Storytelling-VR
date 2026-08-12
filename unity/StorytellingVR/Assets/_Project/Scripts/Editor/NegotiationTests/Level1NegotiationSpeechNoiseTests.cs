using NUnit.Framework;

public class Level1NegotiationSpeechNoiseTests : Level1NegotiationTestBase
{
    [TestCase("yes 173 is fine")]
    [TestCase("173 works for me")]
    [TestCase("yes yes 173 works")]
    [TestCase("well okay 173 is fine")]
    [TestCase("so yeah 173 works")]
    [TestCase("okay so 173 works")]
    [TestCase("173 okay")]
    [TestCase("okay 173 good")]
    public void SpeechNoise_AcceptanceVariantsStillAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertAccept(result, 173);
    }

    [TestCase("no not 120 i want 140", 140)]
    [TestCase("first you said 110 then 125 i want 140", 140)]
    [TestCase("okay make it 180", 180)]
    [TestCase("i want 140 not 120", 140)]
    [TestCase("you offered 125 i need 140", 140)]
    [TestCase("I want I want 140", 140)]
    [TestCase("make it make it 140", 140)]
    [TestCase("uh I want 140", 140)]
    [TestCase("hmm make it 140", 140)]
    [TestCase("like can you do 140", 140)]
    [TestCase("actually um I want 140", 140)]
    [TestCase("give 140", 140)]
    [TestCase("140 final", 140)]
    [TestCase("price 140", 140)]
    [TestCase("no 120 want 140", 140)]
    [TestCase("you 125 me 140", 140)]
    [TestCase("offer low give 140", 140)]
    [TestCase("140 my price", 140)]
    [TestCase("not 120 140", 140)]
    [TestCase("you give 125 I want 140", 140)]
    [TestCase("140 only", 140)]
    public void SpeechNoise_CounterVariantsStillFindPrice(string utterance, int expectedPrice)
    {
        NegotiationInput result = Parse(utterance, 125);
        AssertCounter(result, expectedPrice);
    }

    [TestCase("what price what spice")]
    [TestCase("what what price")]
    [TestCase("um what was that")]
    [TestCase("want more")]
    [TestCase("price too low")]
    [TestCase("more money")]
    [TestCase("give better price")]
    public void SpeechNoise_AmbiguousLinesStayNonTerminal(string utterance)
    {
        NegotiationInput result = Parse(utterance, 125);
        AssertNonTerminal(result);
        Assert.AreNotEqual(NegotiationIntent.DISMISS, result.intent);
    }
}
