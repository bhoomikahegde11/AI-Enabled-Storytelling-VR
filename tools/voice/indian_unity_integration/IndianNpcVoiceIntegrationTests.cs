using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class IndianNpcVoiceIntegrationTests
{
    private static NpcVoicePair Pair(string key, string id) => new NpcVoicePair { key = key, id = id };
    private static NpcVoiceBankData Bank(string character, bool offline = true) => new NpcVoiceBankData
    {
        characterId = character, sampleRate = 44100,
        processing = offline ? new NpcVoiceProcessing { assemblyVersion = NpcVoiceProcessing.IndianOfflineVersion, defaultGapMs = 60f } : null,
        numbers = new[] { Pair("one", "one"), Pair("two", "two"), Pair("thousand", "thousand"),
            Pair("hundred", "hundred"), Pair("and", "and"), Pair("thirty", "thirty"), Pair("four", "four") },
        numberPhrases = new[] { Pair("234", "price234") },
        quantityPhrases = new[] { Pair("two Seers", "twoSeers") },
        units = new[] { Pair("Seers", "seers") },
        spices = new[] { Pair("pepper", "pepper") },
        descriptors = new NpcVoicePair[0],
        templates = new[] {
            new NpcVoiceTemplate { id = "same", scenario = "BuyerCounterFirst", category = "AUTHORED",
                sourceTemplate = "At {price} varahas for {quantityLabel} of {spiceName}.",
                operations = new[] {
                    new NpcVoiceOperation { kind = "CLIP", id = "start", sourcePiece = "At " },
                    new NpcVoiceOperation { kind = "VALUE", name = "price" },
                    new NpcVoiceOperation { kind = "CURRENCY_CLIP", id = "plural", singularId = "singular", priceSlot = "price", sourcePiece = " varahas for " },
                    new NpcVoiceOperation { kind = "VALUE", name = "quantityLabel" },
                    new NpcVoiceOperation { kind = "CLIP", id = "of", sourcePiece = " of " },
                    new NpcVoiceOperation { kind = "VALUE", name = "spiceName" },
                    new NpcVoiceOperation { kind = "PUNCTUATION", text = "." }
                }
            },
            new NpcVoiceTemplate { id = "fixed", scenario = "AskName", category = "AUTHORED", sourceTemplate = "Welcome.",
                operations = new[] { new NpcVoiceOperation { kind = "CLIP", id = "shared", sourcePiece = "Welcome." } } }
        }
    };

    private static NpcVoicePresentation Request(NpcVoiceBankData bank, string price = "234", string quantity = "2 Seers (~560g)") =>
        new NpcVoicePresentation(bank.characterId, bank.templates[0].scenario, bank.templates[0].sourceTemplate,
            "Original subtitle", new Dictionary<string, string> { ["price"] = price, ["quantityLabel"] = quantity, ["spiceName"] = "pepper" });

    [Test] public void WholeNumberAndQuantityPhrasesTakePriorityAndKeepZeroBoundaryGaps()
    {
        var bank = Bank("lakshmi_amma");
        Assert.IsTrue(new NpcVoicePlanner(bank).TryPlan(Request(bank), out var plan, out var error), error);
        CollectionAssert.AreEqual(new[] { "start", "price234", "plural", "twoSeers", "of", "pepper" }, plan.ClipIds);
        CollectionAssert.AreEqual(new[] { 0f, 0f, 0f, 0f, .060f }, plan.GapSeconds);
        Assert.IsTrue(plan.PreserveOriginalSubtitle);
        Assert.AreEqual("At two hundred and thirty four varahas for two Seers of pepper.", plan.Text);
        var pieces = plan.ClipIds.Select((clip, index) => new[] { .25f, index / 10f }).ToList();
        var pcm = plan.AssembleSamples(pieces);
        CollectionAssert.AreEqual(pieces.Take(5).SelectMany(p => p), pcm.Take(10));
        CollectionAssert.AreEqual(pieces[5], pcm.Skip(pcm.Length - 2));
    }

    [Test] public void AbsentPhrasesUseExactGrammarAndHistoricalUnit()
    {
        var bank = Bank("saraswati_chetti");
        bank.quantityPhrases = new NpcVoicePair[0];
        var planner = new NpcVoicePlanner(bank);
        Assert.IsTrue(planner.TryPlan(Request(bank, "1001"), out var big, out var error), error);
        CollectionAssert.AreEqual(new[] { "start", "one", "thousand", "and", "one", "plural", "two", "seers", "of", "pepper" }, big.ClipIds);
        Assert.AreEqual("At one thousand and one varahas for two Seers of pepper.", big.Text);
        Assert.IsTrue(planner.TryPlan(Request(bank, "1"), out var small, out error), error);
        Assert.AreEqual("singular", small.ClipIds[2]);
        Assert.IsFalse(planner.TryPlan(Request(bank, "3000"), out _, out _), "No 'three' recording: fail closed.");
        Assert.IsFalse(planner.TryPlan(Request(bank, "234", "1.5 Seers"), out _, out _));
    }

    [TestCase("father_penteado")]
    [TestCase("francisco_de_almeida")]
    public void LegacyBanksKeepNumberCompositionAndLegacySpacing(string character)
    {
        var bank = Bank(character, offline: false);
        Assert.IsTrue(new NpcVoicePlanner(bank).TryPlan(Request(bank), out var plan, out var error), error);
        CollectionAssert.AreEqual(new[] { "two", "hundred", "and", "thirty", "four" }, plan.ClipIds.Skip(1).Take(5));
        Assert.AreEqual(.004f, plan.GapSeconds[0]);
        Assert.AreEqual(.001f, plan.GapSeconds[1]);
        Assert.IsFalse(plan.PreserveOriginalSubtitle);
    }

    [Test] public void SameClipIdIsResolvedInTheActiveBankAndMissingClipFailsClosed()
    {
        var host = new GameObject("Indian voice fixture");
        var lak = ScriptableObject.CreateInstance<DialogueVoiceDatabase>();
        var sar = ScriptableObject.CreateInstance<DialogueVoiceDatabase>();
        var clip = AudioClip.Create("Lakshmi fixed", 16, 1, 44100, false);
        var lakJson = new TextAsset(JsonUtility.ToJson(Bank("lakshmi_amma")));
        var sarJson = new TextAsset(JsonUtility.ToJson(Bank("saraswati_chetti")));
        // Recreate the database after serialization to exercise its actual lookup initialization.
        var property = new SerializedObject(lak);
        var entries = property.FindProperty("entries");
        entries.arraySize = 1;
        var entry = entries.GetArrayElementAtIndex(0);
        entry.FindPropertyRelative("lineId").stringValue = "shared";
        entry.FindPropertyRelative("audioClip").objectReferenceValue = clip;
        property.ApplyModifiedPropertiesWithoutUndo();
        typeof(DialogueVoiceDatabase).GetMethod("InitializeDictionary", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(lak, null);
        try
        {
            var provider = host.AddComponent<PreRecordedNpcTtsProvider>();
            provider.characterBanks = new[] {
                new PreRecordedNpcTtsProvider.CharacterVoiceBank { characterId = "lakshmi_amma", mapping = lakJson, clips = lak },
                new PreRecordedNpcTtsProvider.CharacterVoiceBank { characterId = "saraswati_chetti", mapping = sarJson, clips = sar }
            };
            NpcVoicePresentation Fixed(string id) => new NpcVoicePresentation(id, "AskName", "Welcome.", "Welcome.", new Dictionary<string, string>());
            Assert.IsTrue(provider.TryPrepare(Fixed("lakshmi_amma"), out var plan, out var error), error);
            CollectionAssert.AreEqual(new[] { "shared" }, plan.ClipIds);
            Assert.AreEqual("lakshmi_amma", plan.CharacterId);
            Assert.IsFalse(provider.TryPrepare(Fixed("saraswati_chetti"), out var missing, out _));
            Assert.IsNull(missing, "The other actor's clip must not be used.");
            int callbacks = 0;
            provider.PlaybackFailed += _ => callbacks++;
            provider.StopSpeaking();
            typeof(PreRecordedNpcTtsProvider).GetMethod("Fail", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(provider, new object[] { "old cancelled load", 0 });
            Assert.AreEqual(0, callbacks);
            Assert.IsFalse(provider.IsBusy);
        }
        finally
        {
            foreach (var obj in new Object[] { host, lak, sar, clip, lakJson, sarJson }) Object.DestroyImmediate(obj);
        }
    }
}
