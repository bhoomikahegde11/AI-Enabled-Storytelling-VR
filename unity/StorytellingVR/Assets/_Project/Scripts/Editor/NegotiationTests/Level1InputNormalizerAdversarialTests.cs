using NUnit.Framework;

public class Level1InputNormalizerAdversarialTests
{
    [TestCase("price for pepper")]
    [TestCase("what is this for")]
    [TestCase("I am looking for cloves")]
    [TestCase("cloves and pepper")]
    [TestCase("salt and pepper")]
    public void ForAndAnd_DoNotCreateFakeNumbers(string utterance)
    {
        string normalized = InputNormalizer.Normalize(utterance);
        StringAssert.DoesNotContain(" 0 ", " " + normalized + " ");
        StringAssert.DoesNotContain("four", normalized);
    }

    [TestCase("four varahas", "4 varahas")]
    [TestCase("forty varahas", "40 varahas")]
    [TestCase("one hundred and forty", "140")]
    [TestCase("one hundred and five", "105")]
    [TestCase("two hundred", "200")]
    [TestCase("ninety", "90")]
    [TestCase("one hundred", "100")]
    [TestCase("one hundred and ten", "110")]
    [TestCase("for 140 varahas", "for 140 varahas")]
    [TestCase("offer forty", "offer 40")]
    [TestCase("make it one hundred forty", "make it 140")]
    public void SupportedNumberPhrases_NormalizeAsExpected(string utterance, string expectedNormalized)
    {
        string normalized = InputNormalizer.Normalize(utterance, true);
        Assert.AreEqual(expectedNormalized, normalized);
    }

    [Test]
    public void FirstOffered_RemainsIntact()
    {
        StringAssert.Contains("first", InputNormalizer.Normalize("First you offered 110"));
    }

    [Test]
    public void StandardPrice_RemainsIntact()
    {
        StringAssert.Contains("standard", InputNormalizer.Normalize("standard price"));
    }

    [Test]
    public void FourSacks_NormalizesWithoutWordCorruption()
    {
        string normalized = InputNormalizer.Normalize("four sacks");
        StringAssert.Contains("4", normalized);
    }

    [Test]
    public void ICanSellItFor140_StillContainsPrice()
    {
        string normalized = InputNormalizer.Normalize("I can sell it for 140", true);
        StringAssert.Contains("140", normalized);
    }
}
