using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class Level1PrerecordedVoiceTests
{
    private const string Root = "Assets/_Project/Audio/NPCVoices/Level1/FatherPenteadoV2/";
    private NpcVoiceBankData bank;
    private NpcVoicePlanner planner;

    [SetUp] public void SetUp()
    {
        var json = AssetDatabase.LoadAssetAtPath<TextAsset>(Root + "FatherVoiceBank.json");
        Assert.IsNotNull(json, "Import Father v2 assets before running these tests.");
        bank = JsonUtility.FromJson<NpcVoiceBankData>(json.text);
        planner = new NpcVoicePlanner(bank);
    }
    private static Dictionary<string, string> Values()
    {
        return new Dictionary<string, string> {
            { "quantityLabel", "1 Seer (~280g)" }, { "brainQuantityLabel", "1 Seer (~280g)" },
            { "spiceName", "pepper" }, { "spiceDescriptor", "black pepper" },
            { "counterPrice", "234" }, { "currentBuyerOffer", "234" }, { "previousOffer", "173" },
            { "updatedOffer", "234" }, { "offeredPrice", "300" }, { "sellerPrice", "300" }, { "finalPrice", "234" },
            { "isAccepted", "false" }, { "hasAcceptedTerms", "false" }, { "walkedAway", "false" }, { "settlementClaimed", "false" }
        };
    }
    private NpcVoicePlan Plan(string id, Dictionary<string, string> values, string ruleReply = "")
    {
        var t = bank.templates.Single(x => x.id == id);
        var request = new NpcVoicePresentation(bank.characterId, t.scenario, t.sourceTemplate, "unused authored prose", values, ruleReply);
        Assert.IsTrue(planner.TryPlan(request, out var plan, out var error), error);
        return plan;
    }
    [Test] public void AllPrices1Through999UseExactCompositionalGrammar()
    {
        for (int n = 1; n <= 999; n++)
        {
            var values = Values(); values["counterPrice"] = n.ToString();
            var plan = Plan("L1_FAT_042", values);
            StringAssert.Contains(string.Join(" ", NpcVoicePlanner.NumberWords(n)), plan.Text);
            Assert.AreEqual(n == 1, plan.Text.Contains(" varaha for"));
            Assert.AreEqual(plan.ClipIds.Count - 1, plan.GapSeconds.Count);
        }
        CollectionAssert.AreEqual(new[] { "two", "hundred", "and", "thirty", "four" }, NpcVoicePlanner.NumberWords(234));
    }
    [Test] public void EveryBankReferenceHasAnImportedClip()
    {
        var db = AssetDatabase.LoadAssetAtPath<DialogueVoiceDatabase>(Root + "FatherVoiceDatabase.asset");
        Assert.IsNotNull(db);
        foreach (var t in bank.templates) foreach (var op in t.operations)
        {
            if (!string.IsNullOrEmpty(op.id)) Assert.IsNotNull(db.GetAudioClip(op.id), op.id);
            if (!string.IsNullOrEmpty(op.singularId)) Assert.IsNotNull(db.GetAudioClip(op.singularId), op.singularId);
            if (op.choices != null) foreach (var choice in op.choices) Assert.IsNotNull(db.GetAudioClip(choice.id), choice.id);
        }
        foreach (var vocabulary in new[] { bank.numbers, bank.spices, bank.descriptors, bank.units })
            foreach (var entry in vocabulary) Assert.IsNotNull(db.GetAudioClip(entry.id), entry.id);
    }
    [Test] public void AcceptedSpeechUsesCopiedFrozenTermsAndNeverAsksForMoneyDelivery()
    {
        var v = Values(); v["isAccepted"] = "true"; v["hasAcceptedTerms"] = "true";
        v["acceptedPrice"] = "234"; v["acceptedQuantityLabel"] = "2 Seers (~560g)"; v["acceptedSpiceName"] = "pepper";
        v["finalPrice"] = "999"; v["quantityLabel"] = "1 Veesai (~1.4 kg)"; v["spiceName"] = "cinnamon";
        var plan = Plan("L1_FAT_065", v);
        Assert.AreEqual("I accept that bargain. Bring two Seers of pepper for two hundred and thirty four varahas, please.", plan.Text);
        StringAssert.DoesNotContain("nine hundred", plan.Text);
    }
    [Test] public void ExactTemplateSelectionCannotRandomizeOrUseAnotherCharacter()
    {
        var t = bank.templates.Single(x => x.id == "L1_FAT_042");
        var wrong = new NpcVoicePresentation("abdul_rahman", t.scenario, t.sourceTemplate, "", Values());
        Assert.IsFalse(planner.TryPlan(wrong, out _, out _));
        var unknown = new NpcVoicePresentation(bank.characterId, t.scenario, "Unknown complete sentence", "Unknown complete sentence", Values());
        Assert.IsFalse(planner.TryPlan(unknown, out _, out _));
        Assert.AreEqual("L1_FAT_042", Plan("L1_FAT_042", Values()).DialogueId);
    }
    [Test] public void ClarificationPreservesExactRuleReplyAndCurrentDescriptor()
    {
        var v = Values(); v["previousOffer"] = "234";
        var plan = Plan("L1_FAT_106", v, "The market is busy and your words were unclear. Say it again plainly.");
        Assert.AreEqual("Perdão, I would rather ask than mistake your meaning. The market is busy and your words were unclear. Say it again plainly.", plan.Text);
    }
    [Test] public void ContinuousPcmPreservesEverySampleAndConfiguredBoundary()
    {
        var plan=Plan("L1_FAT_042",Values());
        var pieces=plan.ClipIds.Select((id,index)=>new[]{.1f+index*.01f,-.2f,.3f}).ToArray();
        var pcm=plan.AssembleSamples(pieces);int offset=0;
        for(int i=0;i<pieces.Length;i++)
        {
            foreach(float sample in pieces[i])Assert.AreEqual(sample,pcm[offset++]);
            if(i<plan.GapSeconds.Count)for(int n=0;n<(int)System.Math.Round(plan.SampleRate*(double)plan.GapSeconds[i]);n++)Assert.AreEqual(0,pcm[offset++]);
        }
        Assert.AreEqual(pcm.Length,offset);
    }
    [Test] public void ReceiptRequiresSettlementAndUnknownTermsFailClosed()
    {
        var t = bank.templates.Single(x => x.id == "L1_FAT_114");
        Assert.IsFalse(planner.TryPlan(new NpcVoicePresentation(bank.characterId,t.scenario,t.sourceTemplate,"",Values()),out _,out _));
        var v = Values(); v["spiceName"] = "clove";
        t = bank.templates.Single(x => x.id == "L1_FAT_009");
        Assert.IsFalse(planner.TryPlan(new NpcVoicePresentation(bank.characterId,t.scenario,t.sourceTemplate,"",v),out _,out _));
        v = Values(); v["quantityLabel"] = "1.5 Seers";
        Assert.IsFalse(planner.TryPlan(new NpcVoicePresentation(bank.characterId,t.scenario,t.sourceTemplate,"",v),out _,out _));
    }
    [Test] public void DialogueProviderRetainsItsStringApiAndExactSelectedTemplateSnapshot()
    {
        var trade = new LocalTradeState { buyerName="Father Penteado", buyerOrigin="Portuguese Cleric", buyerPersonality="Normal",
            spiceKey="pepper",spiceDisplayName="Pepper",quantityLabel="1 Seer (~280g)",quantityGrams=280,npcOffer=173,buyerPatience=5 };
        var provider = new DialogueTableResponseProvider();
        string text = provider.GetGreeting(trade,50,false);
        Assert.AreEqual(text,provider.LastPresentation.OriginalText);
        Assert.IsTrue(bank.templates.Any(t=>t.sourceTemplate==provider.LastPresentation.SourceTemplate && t.scenario=="CustomerGreeting"));
        trade.quantityLabel="2 Veesai (~2.8 kg)";
        Assert.IsTrue(planner.TryPlan(provider.LastPresentation,out var plan,out var error),error);
        StringAssert.Contains("one Seer",plan.Text);
        provider.GetIdleReminder(null,"unmapped",1,false);
        Assert.IsNull(provider.LastPresentation,"Old customer presentation must not leak through an unbound fallback.");
    }
    [Test] public void DepartingCustomerTerminalSelectionSurvivesActiveTradeCleanup()
    {
        var departing = new LocalTradeState { buyerName="Father Penteado", buyerOrigin="Portuguese Cleric",
            buyerPersonality="Normal",spiceKey="pepper",spiceDisplayName="Pepper",
            quantityLabel="1 Seer (~280g)",quantityGrams=280,npcOffer=173,buyerPatience=0 };
        var provider = new DialogueTableResponseProvider();
        string line = provider.GetIdleReminder(departing,"My patience is spent. I will take my leave.",2,true);
        var selected = provider.LastPresentation;
        Assert.IsNotNull(selected);
        Assert.AreEqual(line,selected.OriginalText);
        Assert.AreEqual("TransactionFailure",selected.Scenario);
        Assert.IsTrue(planner.TryPlan(selected,out var plan,out var error),error);
        Assert.AreEqual("father_penteado",plan.CharacterId);
        provider.GetIdleReminder(null,"fallback",2,true);
        Assert.IsNull(provider.LastPresentation);
        Assert.IsTrue(planner.TryPlan(selected,out _,out error),error);
    }
    [TestCase(false)]
    [TestCase(true)]
    public void VoiceAudibilityOverrideIsOptInAndStopRestoresTheSharedSource(bool enabled)
    {
        var host = new GameObject("Prerecorded voice audibility test");
        try
        {
            var source = host.AddComponent<AudioSource>();
            var provider = host.AddComponent<PreRecordedNpcTtsProvider>();
            provider.audioSource = source;
            source.volume = 0.6f; source.minDistance = 1f; source.maxDistance = 500f;
            source.spatialBlend = 1f; source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.pitch = 1f;
            var settings = new PreRecordedNpcTtsProvider.CharacterVoiceBank {
                characterId="father_penteado",overrideAudibility=enabled,voiceVolume=1f,minimumVoiceDistance=4f };
            typeof(PreRecordedNpcTtsProvider).GetMethod("ApplyVoiceAudibility",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(provider,new object[]{settings});
            Assert.AreEqual(enabled ? 1f : 0.6f,source.volume);
            Assert.AreEqual(enabled ? 4f : 1f,source.minDistance);
            Assert.AreEqual(500f,source.maxDistance);
            Assert.AreEqual(1f,source.spatialBlend);
            Assert.AreEqual(1f,source.pitch);
            Assert.AreEqual(AudioRolloffMode.Logarithmic,source.rolloffMode);
            provider.StopSpeaking();
            Assert.AreEqual(0.6f,source.volume,"The next NPC must retain the previous source volume.");
            Assert.AreEqual(1f,source.minDistance,"Cancellation must restore the previous distance rolloff.");
        }
        finally { Object.DestroyImmediate(host); }
    }
}
