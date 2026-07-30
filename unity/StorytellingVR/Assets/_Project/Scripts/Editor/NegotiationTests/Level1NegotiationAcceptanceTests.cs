using NUnit.Framework;

public class Level1NegotiationAcceptanceTests : Level1NegotiationTestBase
{
    [TestCase("yes")]
    [TestCase("yes please")]
    [TestCase("okay")]
    [TestCase("ok")]
    [TestCase("fine")]
    [TestCase("deal")]
    [TestCase("agreed")]
    [TestCase("accepted")]
    [TestCase("I accept")]
    [TestCase("I agree")]
    [TestCase("sounds good")]
    [TestCase("that sounds good")]
    [TestCase("works for me")]
    [TestCase("that works")]
    [TestCase("that is fine")]
    [TestCase("fair enough")]
    [TestCase("done")]
    [TestCase("we have a deal")]
    [TestCase("not bad, deal")]
    [TestCase("alright then")]
    [TestCase("sure")]
    [TestCase("go ahead")]
    [TestCase("very well")]
    [TestCase("I will take it")]
    [TestCase("I accept the offer")]
    [TestCase("you have a deal")]
    [TestCase("let us do it")]
    [TestCase("that will work")]
    [TestCase("fine by me")]
    [TestCase("okay then")]
    [TestCase("agreed then")]
    public void PureAcceptance_PhrasesAcceptCurrentOffer(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertAccept(result, 173);
    }

    [TestCase("173")]
    [TestCase("173.")]
    [TestCase("173 works")]
    [TestCase("173 works for me")]
    [TestCase("173 is fine")]
    [TestCase("173 is okay")]
    [TestCase("173 is ok")]
    [TestCase("173 is good")]
    [TestCase("173 sounds good")]
    [TestCase("173 seems fair")]
    [TestCase("173 will do")]
    [TestCase("173 is acceptable")]
    [TestCase("173 agreed")]
    [TestCase("yes 173")]
    [TestCase("okay 173")]
    [TestCase("fine 173")]
    [TestCase("deal at 173")]
    [TestCase("I accept 173")]
    [TestCase("I accept your 173")]
    [TestCase("I agree to 173")]
    [TestCase("fine, 173 it is")]
    [TestCase("alright, 173 then")]
    [TestCase("173 then")]
    [TestCase("let us settle at 173")]
    [TestCase("we can do 173")]
    [TestCase("I can live with 173")]
    [TestCase("173 is fair enough")]
    [TestCase("173 is reasonable")]
    [TestCase("sure, 173")]
    [TestCase("okay, 173 works")]
    [TestCase("yes okay 173 is fine")]
    public void ExactCurrentOfferAcceptance_PhrasesAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertAccept(result, 173);
    }

    [TestCase("that is lower than I wanted, but fine")]
    [TestCase("I do not love it, but deal")]
    [TestCase("I wanted more, but 173 works")]
    [TestCase("it is not ideal, but okay")]
    [TestCase("fine, I will accept")]
    [TestCase("alright, you have a deal")]
    [TestCase("I suppose 173 will do")]
    [TestCase("very well, 173")]
    [TestCase("I can live with 173")]
    [TestCase("I wanted 180, but I accept 173")]
    [TestCase("I asked for more, but okay")]
    [TestCase("not what I hoped for, but deal")]
    [TestCase("you drive a hard bargain, agreed")]
    [TestCase("I would prefer more, but fine")]
    [TestCase("alright, let us finish at 173")]
    public void ReluctantAcceptance_PhrasesStillAccept(string utterance)
    {
        NegotiationInput result = Parse(utterance);
        AssertAccept(result, 173);
    }

    [TestCase("180", 180)]
    [TestCase("180.", 180)]
    [TestCase("180 works", 180)]
    [TestCase("180 works for me", 180)]
    [TestCase("180 is fine", 180)]
    [TestCase("180 sounds good", 180)]
    [TestCase("180 seems fair", 180)]
    [TestCase("180 will do", 180)]
    [TestCase("yes 180", 180)]
    [TestCase("okay 180", 180)]
    [TestCase("fine 180", 180)]
    [TestCase("deal at 180", 180)]
    [TestCase("I accept 180", 180)]
    [TestCase("let us settle at 180", 180)]
    [TestCase("we can do 180", 180)]
    [TestCase("180 is reasonable", 180)]
    [TestCase("sure, 180", 180)]
    public void DifferentPriceAcceptanceLanguage_RemainsCounter(string utterance, int expectedPrice)
    {
        NegotiationInput result = Parse(utterance);
        AssertCounter(result, expectedPrice);
    }
}
