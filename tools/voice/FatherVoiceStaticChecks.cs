using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Runs only the production pure planner. Does not load Unity or invoke game code.
public static class FatherVoiceStaticChecks
{
    public sealed class Case
    {
        public string id, scenario, source, original, ruleReply, expected;
        public Dictionary<string,string> values;
    }
    public static int Main(string[] args)
    {
        var json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 256 };
        var data = json.Deserialize<NpcVoiceBankData>(File.ReadAllText(args[0]));
        var planner = new NpcVoicePlanner(data);
        var cases = json.Deserialize<List<Case>>(File.ReadAllText(args[1]));
        int count = 0;
        foreach (var c in cases)
        {
            var request = new NpcVoicePresentation(data.characterId,c.scenario,c.source,c.original,c.values,c.ruleReply);
            if (!planner.TryPlan(request,out var plan,out var error)) throw new Exception(c.id+": "+error+" ORIGINAL="+c.original);
            if (plan.Text != c.expected) throw new Exception(c.id+" EXPECTED="+c.expected+" ACTUAL="+plan.Text);
            if (plan.GapSeconds.Count!=plan.ClipIds.Count-1 || plan.GapSeconds.Any(g=>g<0||g>.2f)) throw new Exception("Invalid boundary plan "+c.id);
            if (plan.Text.Contains("Portuguese Cleric") || plan.Text.Contains("kilograms")) throw new Exception("Role/metric speech leak");
            count++;
        }
        var example=cases.First(c=>c.id=="L1_FAT_042");
        var copy=new Dictionary<string,string>(example.values);
        var frozen=new NpcVoicePresentation(data.characterId,example.scenario,example.source,example.original,copy);
        copy["counterPrice"]="999";
        if (!planner.TryPlan(frozen,out var snapshot,out _) || snapshot.Text!=example.expected) throw new Exception("Mutable context leak");
        var pieces=snapshot.ClipIds.Select((id,index)=>new[]{.1f+index*.01f,-.2f,.3f}).ToArray();
        var pcm=snapshot.AssembleSamples(pieces);int offset=0;
        for(int i=0;i<pieces.Length;i++)
        {
            foreach(var sample in pieces[i])if(pcm[offset++]!=sample)throw new Exception("PCM altered/reordered");
            if(i<snapshot.GapSeconds.Count)for(int n=0;n<(int)Math.Round(snapshot.SampleRate*(double)snapshot.GapSeconds[i]);n++)if(pcm[offset++]!=0)throw new Exception("PCM gap corrupted");
        }
        if(offset!=pcm.Length)throw new Exception("PCM assembly length mismatch");
        var foreign=new NpcVoicePresentation("abdul_rahman",example.scenario,example.source,example.original,example.values);
        if (planner.TryPlan(foreign,out _,out _)) throw new Exception("Foreign voice selected");
        var unknown=new NpcVoicePresentation(data.characterId,example.scenario,"unknown","unknown",example.values);
        if (planner.TryPlan(unknown,out _,out _)) throw new Exception("Unknown dialogue guessed");
        foreach (var quantity in new[]{"1.5 Seers","1 Seers","3 kilograms",""})
        {
            var v=new Dictionary<string,string>(example.values);v["quantityLabel"]=quantity;
            var t=data.templates.First(x=>x.id=="L1_FAT_009");
            if(planner.TryPlan(new NpcVoicePresentation(data.characterId,t.scenario,t.sourceTemplate,"",v),out _,out _))throw new Exception("Invalid quantity accepted");
        }
        var agreement=data.templates.First(x=>x.id=="L1_FAT_058");
        if(planner.TryPlan(new NpcVoicePresentation(data.characterId,agreement.scenario,agreement.sourceTemplate,"",example.values),out _,out _))throw new Exception("Agreement without frozen terms");
        Console.WriteLine("PASS: "+count+" production-planner/reference text comparisons; immutable snapshot, character isolation and fail-closed checks.");
        return 0;
    }
}
