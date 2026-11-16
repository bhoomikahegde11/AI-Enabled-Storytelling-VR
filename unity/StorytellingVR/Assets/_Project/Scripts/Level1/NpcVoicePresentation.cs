using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

// A copied presentation context, never an owner of negotiation state.
public sealed class NpcVoicePresentation
{
    public string CharacterId { get; }
    public string Scenario { get; }
    public string SourceTemplate { get; }
    public string OriginalText { get; }
    public string RuleReply { get; }
    public int InteractionId { get; }
    public int TurnId { get; }
    private readonly Dictionary<string, string> values;

    public NpcVoicePresentation(string characterId, string scenario, string sourceTemplate, string originalText,
        IDictionary<string, string> values, string ruleReply = "", int interactionId = 0, int turnId = 0)
    {
        CharacterId = characterId; Scenario = scenario; SourceTemplate = sourceTemplate;
        OriginalText = originalText; RuleReply = ruleReply; InteractionId = interactionId; TurnId = turnId;
        this.values = new Dictionary<string, string>(values, StringComparer.Ordinal);
    }
    public bool TryGet(string key, out string value) => values.TryGetValue(key, out value);
    public Dictionary<string, string> CopyValues() => new Dictionary<string, string>(values, StringComparer.Ordinal);
    public NpcVoicePresentation ForTurn(int interaction, int turn) =>
        new NpcVoicePresentation(CharacterId, Scenario, SourceTemplate, OriginalText, values, RuleReply, interaction, turn);
}

[Serializable] public sealed class NpcVoicePair { public string key; public string id; public string text; }
[Serializable] public sealed class NpcVoiceOperation
{
    public string kind, id, singularId, priceSlot, name, sourcePiece, text;
    public NpcVoicePair[] choices;
}
[Serializable] public sealed class NpcVoiceTemplate
{
    public string id, scenario, sourceTemplate, spokenTemplate, category;
    public bool requiresAgreement, requiresSettlement, conditionalCounter;
    public NpcVoiceOperation[] operations;
}
[Serializable] public sealed class NpcVoiceProcessing
{
    public const string IndianOfflineVersion = "indian-hysteresis-zero-number-v1";
    public string assemblyVersion;
    public float defaultGapMs, numberGapMs, quantityGapMs, numberPaddingMs, numberCrossfadeMs;
}
[Serializable] public sealed class NpcVoiceBankData
{
    public string characterId;
    public int sampleRate;
    public NpcVoiceTemplate[] templates;
    public NpcVoicePair[] numbers, spices, descriptors, units, aliases;
    public NpcVoicePair[] numberPhrases, quantityPhrases;
    public NpcVoiceProcessing processing;
    public string[] edgeGuardClipIds;
    public string quantityFallback;
    public string holdTemplateId;
}
public sealed class NpcVoicePlan
{
    public string CharacterId { get; internal set; }
    public string DialogueId { get; internal set; }
    public string Text { get; internal set; }
    public int InteractionId { get; internal set; }
    public int TurnId { get; internal set; }
    public int SampleRate { get; internal set; }
    public readonly List<string> ClipIds = new List<string>();
    public readonly List<float> GapSeconds = new List<float>();
    public bool PreserveOriginalSubtitle { get; internal set; }
    internal bool PreviousNumeric;
    internal string PreviousPiece = "";

    public float[] AssembleSamples(IList<float[]> pieces)
    {
        if (SampleRate <= 0 || pieces == null || pieces.Count != ClipIds.Count || GapSeconds.Count != pieces.Count - 1)
            throw new ArgumentException("Incomplete utterance PCM");
        long length = 0;
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] == null || pieces[i].Length == 0) throw new ArgumentException("Empty fragment PCM");
            length += pieces[i].Length;
            if (i < GapSeconds.Count) length += (int)Math.Round(SampleRate * (double)GapSeconds[i]);
        }
        if (length <= 0 || length > int.MaxValue) throw new ArgumentException("Invalid utterance PCM length");
        var pcm = new float[(int)length]; int offset = 0;
        for (int i = 0; i < pieces.Count; i++)
        {
            Array.Copy(pieces[i], 0, pcm, offset, pieces[i].Length); offset += pieces[i].Length;
            if (i < GapSeconds.Count) offset += (int)Math.Round(SampleRate * (double)GapSeconds[i]);
        }
        return pcm;
    }
}

public interface IPrerecordedNpcVoiceProvider
{
    bool HandlesCharacter(string characterId);
    bool IsBusy { get; }
    bool TryPrepare(NpcVoicePresentation presentation, out NpcVoicePlan plan, out string error);
    void Speak(NpcVoicePlan plan);
}

// Pure planning policy, also usable in EditMode/standalone checks without audio hardware.
public sealed class NpcVoicePlanner
{
    private readonly NpcVoiceBankData bank;
    private readonly Dictionary<string, NpcVoiceTemplate> templates = new Dictionary<string, NpcVoiceTemplate>();
    private readonly Dictionary<string, string> numbers, spices, descriptors, units, aliases, numberPhrases, quantityPhrases;
    private readonly bool useOfflineEdges;
    private readonly Dictionary<string, string> descriptorText = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> edgeGuards;
    private static readonly Regex Fields = new Regex(@"\{([^{}]+)\}");
    private static readonly string[] Small = "zero one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen".Split(' ');
    private static readonly string[] Tens = "zero ten twenty thirty forty fifty sixty seventy eighty ninety".Split(' ');

    public NpcVoicePlanner(NpcVoiceBankData bank)
    {
        this.bank = bank ?? throw new ArgumentNullException(nameof(bank));
        if (bank.sampleRate <= 0 || bank.templates == null) throw new ArgumentException("Invalid voice bank");
        foreach (var template in bank.templates) templates.Add(template.id, template);
        numbers = Index(bank.numbers); spices = Index(bank.spices); descriptors = Index(bank.descriptors);
        units = Index(bank.units); aliases = Index(bank.aliases);
        useOfflineEdges = bank.processing?.assemblyVersion == NpcVoiceProcessing.IndianOfflineVersion;
        numberPhrases = Index(bank.numberPhrases); quantityPhrases = Index(bank.quantityPhrases);
        if (useOfflineEdges && (bank.processing.numberGapMs != 0 ||
            bank.processing.quantityGapMs != 0 || bank.processing.numberPaddingMs != 0 ||
            bank.processing.numberCrossfadeMs != 0 || float.IsNaN(bank.processing.defaultGapMs) ||
            bank.processing.defaultGapMs < 0 || bank.processing.defaultGapMs > 100))
            throw new ArgumentException("Unsupported offline voice processing settings");
        foreach (var entry in bank.descriptors) descriptorText.Add(entry.key, entry.text);
        edgeGuards = new HashSet<string>(bank.edgeGuardClipIds ?? new string[0]);
    }
    private static Dictionary<string, string> Index(NpcVoicePair[] entries)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (entries != null) foreach (var entry in entries) result.Add(entry.key, entry.id);
        return result;
    }
    public static List<string> NumberWords(int value)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        var words = new List<string>();
        AddNumber(value, words); return words;
    }
    private static void AddNumber(int n, List<string> words)
    {
        if (n < 20) { words.Add(Small[n]); return; }
        if (n < 100) { words.Add(Tens[n / 10]); if (n % 10 != 0) words.Add(Small[n % 10]); return; }
        if (n < 1000) { words.Add(Small[n / 100]); words.Add("hundred"); if (n % 100 != 0) { words.Add("and"); AddNumber(n % 100, words); } return; }
        int divisor = n >= 1000000000 ? 1000000000 : n >= 1000000 ? 1000000 : 1000;
        AddNumber(n / divisor, words); words.Add(divisor == 1000 ? "thousand" : divisor == 1000000 ? "million" : "billion");
        if (n % divisor != 0) { if (n % divisor < 100) words.Add("and"); AddNumber(n % divisor, words); }
    }
    public bool TryPlan(NpcVoicePresentation request, out NpcVoicePlan plan, out string error)
    {
        plan = null; error = null;
        try
        {
            if (request == null || request.CharacterId != bank.characterId) throw new ArgumentException("Wrong character voice bank");
            var values = request.CopyValues();
            NpcVoiceTemplate selected = null;
            // Authored selection was already made by the dialogue provider. Never select another variant here.
            foreach (var t in bank.templates)
                if (t.category == "AUTHORED" && t.scenario == request.Scenario && t.sourceTemplate == request.SourceTemplate)
                { selected = t; break; }
            if (selected == null) selected = FindExactFallback(request.OriginalText, values);
            if (selected == null) throw new ArgumentException("No exact selected dialogue mapping");
            var result = new NpcVoicePlan { CharacterId = bank.characterId, DialogueId = selected.id,
                InteractionId = request.InteractionId, TurnId = request.TurnId, SampleRate = bank.sampleRate,
                PreserveOriginalSubtitle = useOfflineEdges };
            var text = new StringBuilder();
            Append(selected, request, values, result, text, 0);
            result.Text = text.ToString().Trim();
            if (result.ClipIds.Count == 0 || string.IsNullOrWhiteSpace(result.Text)) throw new ArgumentException("Empty dialogue plan");
            plan = result; return true;
        }
        catch (Exception ex) { error = ex.Message; return false; }
    }
    private NpcVoiceTemplate FindExactFallback(string text, Dictionary<string, string> values)
    {
        NpcVoiceTemplate found = null;
        foreach (var t in bank.templates)
        {
            if (t.category == "AUTHORED") continue;
            string rendered;
            if (!TryRenderSource(t.sourceTemplate, values, out rendered) || rendered != text) continue;
            if (found != null && found.sourceTemplate != t.sourceTemplate) throw new ArgumentException("Ambiguous exact fallback mapping");
            found = t;
        }
        return found;
    }
    private static bool TryRenderSource(string template, Dictionary<string, string> values, out string rendered)
    {
        bool complete = true;
        rendered = Fields.Replace(template ?? "", match => { string v; if (values.TryGetValue(match.Groups[1].Value, out v)) return v; complete = false; return match.Value; });
        return complete;
    }
    private string Canonical(string name) { string canonical; return aliases.TryGetValue(name, out canonical) ? canonical : name; }
    private int Number(string name, Dictionary<string, string> values)
    {
        string value; int result;
        if (!values.TryGetValue(Canonical(name), out value) || !int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result) || result < 0)
            throw new ArgumentException("Missing/unsupported numeric field " + name);
        return result;
    }
    private static string Required(Dictionary<string, string> values, string key)
    {
        string value; if (!values.TryGetValue(key, out value) || string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Missing field " + key); return value;
    }
    private void Append(NpcVoiceTemplate t, NpcVoicePresentation request, Dictionary<string, string> original,
        NpcVoicePlan plan, StringBuilder text, int depth)
    {
        if (depth > 2) throw new ArgumentException("Recursive rule reply");
        var values = new Dictionary<string, string>(original);
        if (t.scenario == "TransactionFailure" &&
            (Required(values, "walkedAway") != "true" || Required(values, "hasAcceptedTerms") == "true"))
            throw new ArgumentException("Failure speech requires a resolved non-agreement");
        if (t.requiresAgreement || t.requiresSettlement)
        {
            if (Required(values, "isAccepted") != "true" || Required(values, "hasAcceptedTerms") != "true") throw new ArgumentException("Agreement requires frozen accepted terms");
            string walkedAway;
            if (values.TryGetValue("walkedAway", out walkedAway) && walkedAway == "true") throw new ArgumentException("Agreement cannot also be NO DEAL");
            if (t.requiresSettlement && Required(values, "settlementClaimed") != "true") throw new ArgumentException("Receipt before settlement");
            var price = Required(values, "acceptedPrice"); int acceptedPrice;
            if (!int.TryParse(price, out acceptedPrice) || acceptedPrice <= 0) throw new ArgumentException("Invalid accepted price");
            foreach (var key in new[] { "finalPrice", "sellerPrice", "offeredPrice", "result.acceptedTerms.Price" }) values[key] = price;
            foreach (var key in new[] { "quantityLabel", "brainQuantityLabel", "acceptedQuantityLabel", "quantity", "result.acceptedTerms.QuantityLabel" }) values[key] = Required(values, "acceptedQuantityLabel");
            foreach (var key in new[] { "spiceName", "acceptedSpiceName", "result.acceptedTerms.SpiceName" }) values[key] = Required(values, "acceptedSpiceName");
        }
        if (t.conditionalCounter && Number("updatedOffer", values) <= Number("previousOffer", values))
        {
            if (useOfflineEdges)
                throw new ArgumentException("Counter speech would disagree with the unchanged subtitle");
            values["counterPrice"] = Number("updatedOffer", values).ToString(CultureInfo.InvariantCulture);
            Append(templates[bank.holdTemplateId], request, values, plan, text, depth + 1); return;
        }
        foreach (var op in t.operations)
        {
            if (op.kind == "PUNCTUATION") { text.Append(op.text); plan.PreviousPiece += op.text; continue; }
            if (op.kind == "CLIP" || op.kind == "CURRENCY_CLIP")
            {
                bool one = op.kind == "CURRENCY_CLIP" && Number(op.priceSlot, values) == 1;
                AddClip(plan, one ? op.singularId : op.id, false, op.sourcePiece);
                text.Append(one ? Regex.Replace(op.sourcePiece, @"\bvarahas\b", "varaha") : op.sourcePiece); continue;
            }
            if (op.kind == "ITEM_BOUND_FULL")
            {
                string spice = Required(values, "spiceName").ToLowerInvariant(); NpcVoicePair choice = null;
                foreach (var candidate in op.choices) if (candidate.key == spice) choice = candidate;
                if (choice == null) throw new ArgumentException("Unsupported item");
                string full = t.spokenTemplate.Replace("{spiceName}", spice);
                AddClip(plan, choice.id, false, full); text.Append(full); continue;
            }
            if (op.kind != "VALUE") throw new ArgumentException("Unknown voice operation " + op.kind);
            if (op.name == "ruleReply")
            {
                var nested = FindExactFallback(request.RuleReply, values);
                if (nested == null || nested.category != "FALLBACK") throw new ArgumentException("Unknown precise rule reply");
                Append(nested, request, values, plan, text, depth + 1); continue;
            }
            string name = Canonical(op.name);
            if (name == "quantityLabel" || name == "brainQuantityLabel" || name == "acceptedQuantityLabel")
            {
                string label = Required(values, name);
                if (label == "this lot") { AddClip(plan, bank.quantityFallback, false, label, numeric: true); text.Append(label); continue; }
                var match = Regex.Match(label, @"^\s*(\d+)\s+(Palams?|Seers?|Veesai|Manangus?|Bahars?)(?:\s*\(~\s*\d+(?:\.\d+)?\s*(?:g|kg)\))?\s*$", RegexOptions.IgnoreCase);
                int count;
                if (!match.Success || !int.TryParse(match.Groups[1].Value, out count) || count <= 0) throw new ArgumentException("Unsupported traditional quantity " + label);
                string unit = match.Groups[2].Value;
                foreach (var singular in new[] { "Palam", "Seer", "Manangu", "Bahar" })
                    if ((unit.Equals(singular, StringComparison.OrdinalIgnoreCase) && count != 1) || (unit.Equals(singular + "s", StringComparison.OrdinalIgnoreCase) && count == 1)) throw new ArgumentException("Noncanonical quantity unit");
                string quantityPhrase = string.Join(" ", NumberWords(count)) + " " + unit;
                if (useOfflineEdges && quantityPhrases.TryGetValue(quantityPhrase, out string quantityClip))
                {
                    AddClip(plan, quantityClip, false, quantityPhrase, numeric: true);
                    text.Append(quantityPhrase);
                }
                else
                {
                    AppendNumber(count, plan, text);
                    text.Append(' ');
                    AddClip(plan, units[unit], false, unit, numeric: true);
                    text.Append(unit);
                }
                continue;
            }
            if (name == "spiceName" || name == "acceptedSpiceName" || op.name == "spiceDescriptor")
            {
                string spice = Required(values, name == "acceptedSpiceName" ? name : "spiceName").ToLowerInvariant();
                string spoken = op.name == "spiceDescriptor" ? descriptorText[spice] : spice;
                AddClip(plan, (op.name == "spiceDescriptor" ? descriptors : spices)[spice], false, spoken);
                text.Append(spoken); continue;
            }
            AppendNumber(Number(op.name, values), plan, text);
        }
    }
    private void AppendNumber(int n, NpcVoicePlan plan, StringBuilder text)
    {
        var words = NumberWords(n);
        if (useOfflineEdges && numberPhrases.TryGetValue(n.ToString(CultureInfo.InvariantCulture), out string phraseClip))
        {
            string phrase = string.Join(" ", words);
            AddClip(plan, phraseClip, false, phrase, numeric: true);
            text.Append(phrase);
            return;
        }
        for (int i = 0; i < words.Count; i++)
        {
            if (i > 0) text.Append(' ');
            AddClip(plan, numbers[words[i]], i > 0, words[i], numeric: true);
            text.Append(words[i]);
        }
    }
    private void AddClip(NpcVoicePlan plan, string id, bool numberJoin, string piece, bool numeric = false)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentException("Missing clip ID");
        if (plan.ClipIds.Count > 0)
        {
            float gap = numberJoin ? 0.001f : 0.004f;
            if (Regex.IsMatch(plan.PreviousPiece, @"[.!?]\s*$") || Regex.IsMatch(piece ?? "", @"^\s*[.!?]")) gap = 0.055f;
            else if (Regex.IsMatch(plan.PreviousPiece, @"[,;:]\s*$") || Regex.IsMatch(piece ?? "", @"^\s*[,;:]")) gap = 0.025f;
            if (edgeGuards.Contains(id) || edgeGuards.Contains(plan.ClipIds[plan.ClipIds.Count - 1])) gap = Math.Max(gap, 0.015f);
            if (useOfflineEdges)
            {
                gap = bank.processing.defaultGapMs / 1000f;
                if (Regex.IsMatch(plan.PreviousPiece, @"[.!?]\s*$") || Regex.IsMatch(piece ?? "", @"^\s*[.!?]")) gap = Math.Max(gap, 0.055f);
                else if (Regex.IsMatch(plan.PreviousPiece, @"[,;:]\s*$") || Regex.IsMatch(piece ?? "", @"^\s*[,;:]")) gap = Math.Max(gap, 0.025f);
                if (plan.PreviousNumeric || numeric) gap = 0f;
            }
            plan.GapSeconds.Add(gap);
        }
        plan.ClipIds.Add(id);
        plan.PreviousPiece = piece ?? "";
        plan.PreviousNumeric = numeric;
    }
}
