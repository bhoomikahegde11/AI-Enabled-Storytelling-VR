using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// JsonUtility's field-shaped data is read without loading Unity in this executable.
namespace UnityEngine
{
    public static class JsonUtility
    {
        public static T FromJson<T>(string value) =>
            new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Deserialize<T>(value);
    }
}

public static class IndianVoiceStaticChecks
{
    private static void Require(bool flag, string message)
    {
        if (!flag) throw new Exception(message);
    }

    public static void Main(string[] args)
    {
        switch (args[0])
        {
            case "reject":
                try { IndianNpcVoicePackage.Read(args[1]); }
                catch (Exception e)
                {
                    Require(e.Message.Contains(args[2]), e.ToString());
                    Console.WriteLine("Rejected: " + e.Message); return;
                }
                throw new Exception("Invalid package was accepted.");
            case "legacy":
                TestLegacy(args[1]); return;
            default:
                foreach (var path in args.Skip(1)) TestPackage(IndianNpcVoicePackage.Read(path));
                return;
        }
    }

    private static void TestPackage(IndianNpcVoicePackage package)
    {
        var bank = package.Bank;
        var planner = new NpcVoicePlanner(bank);
        var t = bank.templates.Single(x => x.id == "Offer");
        var vocabulary = package.Clips.ToDictionary(c => c.clipId, c => c.text);
        int count = 0;
        foreach (int price in Enumerable.Range(0, 1000).Concat(new[] { 1001, 1000001, int.MaxValue }))
        {
            var values = new Dictionary<string, string> { ["price"] = price.ToString(), ["quantityLabel"] = "2 Seers (~560g)", ["spiceName"] = "pepper" };
            var request = new NpcVoicePresentation(bank.characterId, t.scenario, t.sourceTemplate, "unchanged subtitle", values);
            Require(planner.TryPlan(request, out var plan, out var error), error);
            string words = string.Join(" ", NpcVoicePlanner.NumberWords(price));
            Require(plan.Text == "At " + words + (price == 1 ? " varaha" : " varahas") + " for two Seers of pepper.", plan.Text);
            Require(plan.PreserveOriginalSubtitle, "Subtitle policy changed");
            Require(plan.ClipIds.All(vocabulary.ContainsKey), "Unexpected clip");
            Require(plan.ClipIds.Contains("quantity_pair"), "Quantity phrase not selected");
            if (price == 234) Require(plan.ClipIds.Contains("whole_234"), "Whole phrase not selected");
            Require(plan.GapSeconds.Take(plan.GapSeconds.Count - 1).All(g => g == 0f), "Numeric boundary is not zero");
            Require(Math.Abs(plan.GapSeconds.Last() - .06f) < 1e-6, "Package gap not applied");
            var pcm = plan.AssembleSamples(plan.ClipIds.Select((id,index) => new[] { (float)index + 1 }).ToArray());
            Require(pcm.Where(s => s != 0f).SequenceEqual(Enumerable.Range(1, plan.ClipIds.Count).Select(n => (float)n)), "PCM order changed");
            values["quantityLabel"] = "2.5 Seers";
            Require(!planner.TryPlan(new NpcVoicePresentation(bank.characterId, t.scenario, t.sourceTemplate, "", values), out _, out _),
                "Unsupported quantity accepted");
            count++;
        }
        Require(!planner.TryPlan(new NpcVoicePresentation("father_penteado", t.scenario, t.sourceTemplate, "", new Dictionary<string,string>()), out _, out _),
            "Incorrect character routed");
        var fixedLine = bank.templates.Single(x => x.id == "Hold");
        Require(planner.TryPlan(new NpcVoicePresentation(bank.characterId, fixedLine.scenario, fixedLine.sourceTemplate, fixedLine.sourceTemplate,
            new Dictionary<string,string>()), out var fixedPlan, out var fixedError), fixedError);
        Require(fixedPlan.ClipIds.SequenceEqual(new[] { "hold" }), "Fixed dialogue is not a single clip");
        Console.WriteLine(bank.characterId + ": " + count + " numeric/PCM plans, routing, fixed clip and preflight passed.");
    }

    private static void TestLegacy(string file)
    {
        var data = UnityEngine.JsonUtility.FromJson<NpcVoiceBankData>(File.ReadAllText(file));
        var planner = new NpcVoicePlanner(data);
        var t = data.templates.First(x => x.id == "L1_FAT_042");
        for (int n=1; n<=999; n++)
        {
            var values = new Dictionary<string, string> { ["counterPrice"]=n.ToString(), ["spiceName"]="pepper" };
            Require(planner.TryPlan(new NpcVoicePresentation(data.characterId,t.scenario,t.sourceTemplate,"",values), out var plan, out var error), error);
            Require(!plan.PreserveOriginalSubtitle, "Portuguese subtitle policy changed");
            Require(plan.Text.Contains(string.Join(" ", NpcVoicePlanner.NumberWords(n))), "Portuguese price changed");
            Require(plan.Text.Contains(n == 1 ? " varaha " : " varahas "), "Portuguese currency changed");
            Require(plan.GapSeconds.All(g => g > 0), "Portuguese spacing became zero");
        }
        Console.WriteLine("Father's actual bank: 999 legacy plans passed.");
    }
}
