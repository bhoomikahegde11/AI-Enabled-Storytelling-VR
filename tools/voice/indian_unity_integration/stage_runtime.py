"""Build the staged runtime diff without changing a source locked by voice production."""
from pathlib import Path
import difflib
import sys

ROOT = Path(__file__).resolve().parents[3]
LEVEL = Path("unity/StorytellingVR/Assets/_Project/Scripts/Level1")


def replace(source, old, new):
    if source.count(old) != 1:
        raise ValueError(f"Expected exactly one occurrence: {old[:100]}")
    return source.replace(old, new)


def planner(source):
    source = replace(source, "[Serializable] public sealed class NpcVoiceBankData", """[Serializable] public sealed class NpcVoiceProcessing
{
    public const string IndianOfflineVersion = "indian-hysteresis-zero-number-v1";
    public string assemblyVersion;
    public float defaultGapMs, numberGapMs, quantityGapMs, numberPaddingMs, numberCrossfadeMs;
}
[Serializable] public sealed class NpcVoiceBankData""")
    source = replace(source, "    public string[] edgeGuardClipIds;", """    public NpcVoicePair[] numberPhrases, quantityPhrases;
    public NpcVoiceProcessing processing;
    public string[] edgeGuardClipIds;""")
    source = replace(source, '    internal string PreviousPiece = "";', """    public bool PreserveOriginalSubtitle { get; internal set; }
    internal bool PreviousNumeric;
    internal string PreviousPiece = "";""")
    source = replace(source, "private readonly Dictionary<string, string> numbers, spices, descriptors, units, aliases;",
                     "private readonly Dictionary<string, string> numbers, spices, descriptors, units, aliases, numberPhrases, quantityPhrases;\n    private readonly bool useOfflineEdges;")
    source = replace(source, "        units = Index(bank.units); aliases = Index(bank.aliases);", """        units = Index(bank.units); aliases = Index(bank.aliases);
        useOfflineEdges = bank.processing?.assemblyVersion == NpcVoiceProcessing.IndianOfflineVersion;
        numberPhrases = Index(bank.numberPhrases); quantityPhrases = Index(bank.quantityPhrases);
        if (useOfflineEdges && (bank.processing.numberGapMs != 0 ||
            bank.processing.quantityGapMs != 0 || bank.processing.numberPaddingMs != 0 ||
            bank.processing.numberCrossfadeMs != 0 || float.IsNaN(bank.processing.defaultGapMs) ||
            bank.processing.defaultGapMs < 0 || bank.processing.defaultGapMs > 100))
            throw new ArgumentException("Unsupported offline voice processing settings");""")
    source = replace(source, "InteractionId = request.InteractionId, TurnId = request.TurnId, SampleRate = bank.sampleRate };",
                     "InteractionId = request.InteractionId, TurnId = request.TurnId, SampleRate = bank.sampleRate,\n                PreserveOriginalSubtitle = useOfflineEdges };")
    source = replace(source, '            values["counterPrice"] = Number("updatedOffer", values).ToString(CultureInfo.InvariantCulture);',
                     """            if (useOfflineEdges)
                throw new ArgumentException("Counter speech would disagree with the unchanged subtitle");
            values["counterPrice"] = Number("updatedOffer", values).ToString(CultureInfo.InvariantCulture);""")
    source = replace(source, 'AddClip(plan, bank.quantityFallback, false, label);', 'AddClip(plan, bank.quantityFallback, false, label, numeric: true);')
    source = replace(source, "                AppendNumber(count, plan, text); text.Append(' '); AddClip(plan, units[unit], false, unit); text.Append(unit); continue;", """                string quantityPhrase = string.Join(" ", NumberWords(count)) + " " + unit;
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
                continue;""")
    source = replace(source, """        var words = NumberWords(n);
        for (int i = 0; i < words.Count; i++) { if (i > 0) text.Append(' '); AddClip(plan, numbers[words[i]], i > 0, words[i]); text.Append(words[i]); }""", """        var words = NumberWords(n);
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
        }""")
    source = replace(source, "private void AddClip(NpcVoicePlan plan, string id, bool numberJoin, string piece)",
                     "private void AddClip(NpcVoicePlan plan, string id, bool numberJoin, string piece, bool numeric = false)")
    source = replace(source, "            plan.GapSeconds.Add(gap);", """            if (useOfflineEdges)
            {
                gap = bank.processing.defaultGapMs / 1000f;
                if (Regex.IsMatch(plan.PreviousPiece, @"[.!?]\\s*$") || Regex.IsMatch(piece ?? "", @"^\\s*[.!?]")) gap = Math.Max(gap, 0.055f);
                else if (Regex.IsMatch(plan.PreviousPiece, @"[,;:]\\s*$") || Regex.IsMatch(piece ?? "", @"^\\s*[,;:]")) gap = Math.Max(gap, 0.025f);
                if (plan.PreviousNumeric || numeric) gap = 0f;
            }
            plan.GapSeconds.Add(gap);""")
    return replace(source, '        plan.PreviousPiece = piece ?? "";', '        plan.PreviousPiece = piece ?? "";\n        plan.PreviousNumeric = numeric;')


def chat(source):
    return replace(source, "        if (voicePlan != null)\n        {\n            replyText = voicePlan.Text;",
                   "        if (voicePlan != null && !voicePlan.PreserveOriginalSubtitle)\n        {\n            replyText = voicePlan.Text;")


def patch_file(path, before, after):
    a, b = (f"a/{path.as_posix()}" if before else "/dev/null"), f"b/{path.as_posix()}"
    prefix = f"diff --git a/{path.as_posix()} {b}\n"
    if not before:
        prefix += "new file mode 100644\n"
    return prefix + "".join(difflib.unified_diff(before.splitlines(True), after.splitlines(True), a, b))


def main():
    folder = Path(__file__).resolve().parent
    output = Path(sys.argv[1])
    staged = []
    for name, transform in (("NpcVoicePresentation.cs", planner), ("ChatManager.cs", chat)):
        path = LEVEL / name
        source = (ROOT / path).read_text(encoding="utf-8-sig")
        staged.append(patch_file(path, source, transform(source)))
    for name, destination in (
        ("IndianNpcVoicePackage.cs", LEVEL.parent / "Editor"),
        ("IndianNpcVoicePackageImporter.cs", LEVEL.parent / "Editor"),
        ("IndianNpcVoiceIntegrationTests.cs", LEVEL.parent / "Editor/NegotiationTests"),
    ):
        staged.append(patch_file(destination / name, "", (folder / name).read_text(encoding="utf-8-sig")))
    output.write_text("".join(staged), encoding="utf-8", newline="\n")


if __name__ == "__main__":
    main()
