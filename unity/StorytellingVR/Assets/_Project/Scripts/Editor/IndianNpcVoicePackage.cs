using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Read-only preflight. The Editor importer performs asset writes only after this returns.
public sealed class IndianNpcVoicePackage
{
    [Serializable] public sealed class Clip
    {
        public string clipId, relativeWavPath, sha256, text;
    }
    [Serializable] private sealed class ClipIndex { public Clip[] clips; }
    [Serializable] private sealed class Coverage
    {
        public string character_id, speaker;
        public bool ready_for_import_review;
        public int required, wav_count, failures, pending, mapping_count, required_mapping_count;
        public string[] missing_vocabulary;
    }
    [Serializable] private sealed class Validation
    {
        public string character, speaker;
        public string character_id;
        public bool production_complete;
        public bool integration_preflight_passed;
        public int tested_signals, mapping_count;
        public int required, validated, mappings;
        public string[] missing_assets, declared_missing_vocabulary;
        public string[] missing_ids, source_dialogue_drift, compatibility_warnings;
    }
    [Serializable] private sealed class Manifest
    {
        public int version;
        public string character_id;
        public Voice voice;
        public Recording[] recordings;
        public Template[] templates;
        public string[] missing_template_ids;
    }
    [Serializable] private sealed class Voice { public string character_id, speaker, model, reference_sha256; }
    [Serializable] private sealed class Recording
    {
        public string id, speaker, spoken_text, wav_path, wav_sha256, status, validation_status;
        public int sample_rate, channels;
    }
    [Serializable] private sealed class Template
    {
        public string id, category, scenario, source_expression_template, spoken_template;
        public bool requiresAgreement, requiresSettlement, conditionalCounter;
        public Operation[] operations;
    }
    [Serializable] private sealed class Operation
    {
        public string kind, id, singular_id, price_slot, name, source_piece, text;
    }

    private static readonly Dictionary<string, string> Speakers = new Dictionary<string, string>(StringComparer.Ordinal) {
        { "lakshmi_amma", "Swapna" }, { "chinnamma_naik", "Priya" },
        { "abdul_rahman", "Kabir" }, { "saraswati_chetti", "Mary" }
    };
    private static readonly Regex SafeId = new Regex(@"\A[A-Za-z0-9_-]+\z");
    private static readonly Regex Sha256 = new Regex(@"\A[0-9a-fA-F]{64}\z");
    public string DirectoryPath { get; private set; }
    public string BankJson { get; private set; }
    public NpcVoiceBankData Bank { get; private set; }
    public Clip[] Clips { get; private set; }
    public string SourcePath(Clip clip) => Path.Combine(DirectoryPath, clip.relativeWavPath);

    public static IndianNpcVoicePackage Read(string directory)
    {
        var root = Path.GetFullPath(directory);
        var bankJson = File.ReadAllText(Path.Combine(root, "UnityVoiceBank.json"));
        var bank = JsonUtility.FromJson<NpcVoiceBankData>(bankJson);
        if (bank != null && bank.characterId == "francisco_de_almeida")
            return ReadFrancisco(root, bankJson, bank);
        Check(bank != null && Speakers.TryGetValue(bank.characterId ?? "", out _), "Unexpected Indian character ID");
        string speaker = Speakers[bank.characterId];
        var coverage = ReadJson<Coverage>(root, "coverage_report.json");
        var validation = ReadJson<Validation>(root, "production_validation.json");
        Check(coverage.character_id == bank.characterId && validation.character == bank.characterId &&
            coverage.speaker == speaker && validation.speaker == speaker, "Report identity does not match bank");
        Check(coverage.ready_for_import_review && coverage.pending == 0 && coverage.failures == 0 &&
            Empty(coverage.missing_vocabulary) && validation.production_complete &&
            Empty(validation.missing_assets) && Empty(validation.declared_missing_vocabulary), "Package is incomplete or failed validation");
        var manifest = ReadJson<Manifest>(root, "mapping_manifest.json");
        Check(manifest.version == 1 && manifest.character_id == bank.characterId && manifest.voice != null &&
            manifest.voice.character_id == bank.characterId && manifest.voice.speaker == speaker &&
            Empty(manifest.missing_template_ids), "Manifest identity or coverage differs from the bank");
        var catalog = File.ReadAllText(Path.Combine(root, "UnityVoiceClips.json")).Trim();
        Check(catalog.StartsWith("[", StringComparison.Ordinal) && catalog.EndsWith("]", StringComparison.Ordinal), "Expected a clip index array");
        var clips = JsonUtility.FromJson<ClipIndex>("{\"clips\":" + catalog + "}").clips;
        Check(clips != null && clips.Length > 0 && coverage.required == clips.Length && coverage.wav_count == clips.Length &&
            validation.tested_signals == clips.Length && manifest.recordings != null && manifest.recordings.Length == clips.Length,
            "Clip counts do not agree with completed reports");
        Check(bank.templates != null && bank.templates.Length > 0 && manifest.templates != null &&
            manifest.templates.Length == bank.templates.Length && coverage.mapping_count == bank.templates.Length &&
            coverage.required_mapping_count == bank.templates.Length && validation.mapping_count == bank.templates.Length,
            "Template counts do not agree with completed reports");
        Check(bank.sampleRate == 44100 && bank.processing?.assemblyVersion == NpcVoiceProcessing.IndianOfflineVersion,
            "Expected a native 44.1 kHz Indian bank with offline edge processing");

        var indexed = new Dictionary<string, Clip>(StringComparer.OrdinalIgnoreCase);
        var records = manifest.recordings.ToDictionary(x => x.id, StringComparer.Ordinal);
        foreach (var clip in clips)
        {
            Check(clip != null && SafeId.IsMatch(clip.clipId ?? "") && Sha256.IsMatch(clip.sha256 ?? "") &&
                clip.relativeWavPath == "audio/" + clip.clipId + ".wav" && !string.IsNullOrWhiteSpace(clip.text), "Malformed clip index row");
            Check(!indexed.ContainsKey(clip.clipId), "Duplicate clip ID " + clip.clipId);
            indexed.Add(clip.clipId, clip);
            Check(records.TryGetValue(clip.clipId, out var record) && record.speaker == speaker &&
                record.spoken_text == clip.text && record.wav_path == clip.relativeWavPath && record.wav_sha256 == clip.sha256 &&
                record.status == "VALIDATED" && record.sample_rate == bank.sampleRate && record.channels == 1,
                "Clip differs from validated manifest: " + clip.clipId);
        }

        ValidateMappings(bank, manifest.templates, indexed);
        foreach (var clip in clips)
        {
            string path = Path.Combine(root, clip.relativeWavPath);
            Check(File.Exists(path), "Missing processed WAV: " + clip.relativeWavPath);
            Check(string.Equals(Hash(path), clip.sha256, StringComparison.OrdinalIgnoreCase), "WAV checksum mismatch: " + clip.relativeWavPath);
            VerifyWav(path, bank.sampleRate);
        }
        return new IndianNpcVoicePackage { DirectoryPath = root, BankJson = bankJson, Bank = bank, Clips = clips };
    }

    private static IndianNpcVoicePackage ReadFrancisco(string root, string bankJson, NpcVoiceBankData bank)
    {
        var validation = ReadJson<Validation>(root, "production_validation.json");
        var coverage = ReadJson<Validation>(root, "coverage_report.json");
        foreach (var report in new[] { validation, coverage })
            Check(report.character_id == bank.characterId && report.production_complete && report.integration_preflight_passed &&
                Empty(report.missing_ids) && Empty(report.source_dialogue_drift) && Empty(report.compatibility_warnings),
                "Portuguese package failed completion or integration preflight");
        var manifest = ReadJson<Manifest>(root, "mapping_manifest.json");
        Check(manifest.character_id == bank.characterId && manifest.voice != null &&
            manifest.voice.character_id == bank.characterId && manifest.voice.speaker == "Francisco de Almeida" &&
            manifest.voice.model == "Qwen/Qwen3-TTS-12Hz-1.7B-Base" &&
            manifest.voice.reference_sha256 == "f4d7c6026970569262b2cc6e1dec1ab5aa1e091ba676ae8c84cf981cea88bb4a",
            "Portuguese manifest voice identity differs from the approved reference");
        // JsonUtility can materialize an empty object for a missing nested field.
        Check(bank.sampleRate == 24000 && string.IsNullOrEmpty(bank.processing?.assemblyVersion),
            "Expected native Portuguese bank processing");
        var catalog = File.ReadAllText(Path.Combine(root, "UnityVoiceClips.json")).Trim();
        Check(catalog.StartsWith("[", StringComparison.Ordinal) && catalog.EndsWith("]", StringComparison.Ordinal),
            "Expected a clip index array");
        var clips = JsonUtility.FromJson<ClipIndex>("{\"clips\":" + catalog + "}").clips;
        Check(clips != null && clips.Length > 0 && manifest.recordings != null &&
            manifest.recordings.Length == clips.Length && bank.templates != null && bank.templates.Length > 0 &&
            manifest.templates != null && manifest.templates.Length == bank.templates.Length, "Portuguese package counts differ");
        foreach (var report in new[] { validation, coverage })
            Check(report.required == clips.Length && report.validated == clips.Length && report.mappings == bank.templates.Length,
                "Portuguese reports disagree with package counts");
        var indexed = new Dictionary<string, Clip>(StringComparer.OrdinalIgnoreCase);
        var records = manifest.recordings.ToDictionary(r => r.id, StringComparer.Ordinal);
        foreach (var clip in clips)
        {
            Check(clip != null && SafeId.IsMatch(clip.clipId ?? "") && Sha256.IsMatch(clip.sha256 ?? "") &&
                clip.relativeWavPath == "Audio/" + clip.clipId + ".wav" && !string.IsNullOrWhiteSpace(clip.text),
                "Malformed Portuguese clip index row");
            Check(!indexed.ContainsKey(clip.clipId), "Duplicate clip ID " + clip.clipId);
            indexed.Add(clip.clipId, clip);
            Check(records.TryGetValue(clip.clipId, out var record) && record.validation_status == "VALIDATED" &&
                record.spoken_text == clip.text && record.wav_path == clip.relativeWavPath &&
                record.wav_sha256 == clip.sha256 && record.sample_rate == bank.sampleRate,
                "Portuguese clip differs from validated manifest: " + clip.clipId);
            string path = Path.Combine(root, clip.relativeWavPath);
            Check(File.Exists(path) && Hash(path).Equals(clip.sha256, StringComparison.OrdinalIgnoreCase),
                "Missing or changed Portuguese WAV: " + clip.relativeWavPath);
            VerifyWav(path, bank.sampleRate);
        }
        ValidateMappings(bank, manifest.templates, indexed);
        return new IndianNpcVoicePackage { DirectoryPath = root, BankJson = bankJson, Bank = bank, Clips = clips };
    }

    private static void ValidateMappings(NpcVoiceBankData bank, Template[] manifest, Dictionary<string, Clip> indexed)
    {
        new NpcVoicePlanner(bank); // Also checks the runtime processing contract and duplicate keys.
        void HasClip(string id)
        {
            Check(!string.IsNullOrEmpty(id) && indexed.TryGetValue(id, out var clip) && clip.clipId == id, "Unknown clip reference: " + id);
        }
        void Vocabulary(NpcVoicePair[] pairs, IEnumerable<string> required)
        {
            Check(pairs != null, "Missing vocabulary group");
            foreach (var entry in pairs) HasClip(entry.id);
            Check(required.All(key => pairs.Any(pair => pair.key == key)), "Required vocabulary is incomplete");
        }

        var scales = new[] { 100, 101, 1000, 1000000, 1000000000 };
        Vocabulary(bank.numbers, Enumerable.Range(0, 100).Concat(scales).SelectMany(NpcVoicePlanner.NumberWords).Distinct());
        bool offline = bank.processing?.assemblyVersion == NpcVoiceProcessing.IndianOfflineVersion;
        if (offline) Vocabulary(bank.numberPhrases, new[] { "1", "234", "999" });
        foreach (var phrase in bank.numberPhrases ?? Array.Empty<NpcVoicePair>())
        {
            Check(int.TryParse(phrase.key, System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out var n), "Invalid number phrase key: " + phrase.key);
            Check(SpokenKey(indexed[phrase.id].text) == SpokenKey(string.Join(" ", NpcVoicePlanner.NumberWords(n))),
                "Number phrase text does not match the key: " + phrase.key);
        }
        Vocabulary(bank.units, "Palam Palams Seer Seers Veesai Manangu Manangus Bahar Bahars".Split(' '));
        if (offline) Vocabulary(bank.quantityPhrases, new[] { "one Seer", "two Seers", "one Veesai", "two Veesai" });
        foreach (var phrase in bank.quantityPhrases ?? Array.Empty<NpcVoicePair>())
            Check(SpokenKey(phrase.key) == SpokenKey(indexed[phrase.id].text), "Quantity phrase text does not match the key: " + phrase.key);
        Vocabulary(bank.spices, new[] { "pepper", "cinnamon", "cardamom" });
        Vocabulary(bank.descriptors, new[] { "pepper", "cinnamon", "cardamom" });
        foreach (var pair in bank.numbers.Concat(bank.units).Concat(bank.spices))
            Check(SpokenKey(pair.key) == SpokenKey(indexed[pair.id].text), "Vocabulary text differs from key: " + pair.key);
        foreach (var descriptor in bank.descriptors)
            Check(descriptor.text == indexed[descriptor.id].text, "Spice descriptor differs from clip text: " + descriptor.key);
        HasClip(bank.quantityFallback);
        foreach (string id in bank.edgeGuardClipIds ?? Array.Empty<string>()) HasClip(id);
        Check(SpokenKey(indexed[bank.quantityFallback].text) == "this lot", "Wrong quantity fallback");
        var byId = manifest.ToDictionary(x => x.id, StringComparer.Ordinal);
        var authored = new HashSet<(string, string)>();
        foreach (var line in bank.templates)
        {
            Check(!string.IsNullOrWhiteSpace(line.id) && !string.IsNullOrWhiteSpace(line.scenario) &&
                !string.IsNullOrWhiteSpace(line.sourceTemplate) && (line.category == "AUTHORED" || line.category == "FALLBACK"),
                "Invalid dialogue row");
            Check(byId.TryGetValue(line.id, out var m) && m.category == line.category && m.scenario == line.scenario &&
                m.source_expression_template == line.sourceTemplate && m.spoken_template == line.spokenTemplate &&
                m.requiresAgreement == line.requiresAgreement && m.requiresSettlement == line.requiresSettlement &&
                m.conditionalCounter == line.conditionalCounter && line.operations != null && line.operations.Length > 0 &&
                m.operations != null && line.operations.Length == m.operations.Length, "Dialogue differs from manifest: " + line.id);
            if (line.category == "AUTHORED")
                Check(authored.Add((line.scenario, line.sourceTemplate)), "Duplicate authored selection key: " + line.id);
            for (int i = 0; i < line.operations.Length; i++)
            {
                var op = line.operations[i];
                var expected = m.operations[i];
                Check(op != null && expected != null && op.kind == expected.kind && Same(op.id, expected.id) &&
                    Same(op.singularId, expected.singular_id) && Same(op.priceSlot, expected.price_slot) &&
                    Same(op.name, expected.name) && Same(op.sourcePiece, expected.source_piece) && Same(op.text, expected.text),
                    "Segment differs from manifest: " + line.id);
                switch (op.kind)
                {
                    case "VALUE": Check(!string.IsNullOrEmpty(op.name), "Empty value slot: " + line.id); break;
                    case "PUNCTUATION": Check(!string.IsNullOrEmpty(op.text), "Empty punctuation: " + line.id); break;
                    case "CLIP": HasClip(op.id); break;
                    case "CURRENCY_CLIP":
                        HasClip(op.id); HasClip(op.singularId);
                        Check(!string.IsNullOrEmpty(op.priceSlot), "Unbound currency: " + line.id);
                        break;
                    default: throw new InvalidDataException("Unknown Indian segment kind " + op.kind);
                }
            }
        }
        Check(bank.templates.Any(t => t.id == bank.holdTemplateId && t.scenario == "BuyerHoldsFirm"), "Missing HOLD mapping");
    }

    public static string Hash(string path)
    {
        using (var stream = File.OpenRead(path))
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }

    public static void VerifyWav(string path, int rate)
    {
        using (var stream = File.OpenRead(path))
        using (var reader = new BinaryReader(stream, Encoding.ASCII))
        {
            string Tag() => Encoding.ASCII.GetString(reader.ReadBytes(4));
            Check(Tag() == "RIFF" && reader.ReadUInt32() + 8L == stream.Length && Tag() == "WAVE", "Invalid WAV header: " + path);
            bool format = false, data = false;
            while (stream.Position + 8 <= stream.Length)
            {
                string tag = Tag();
                uint size = reader.ReadUInt32();
                long end = stream.Position + size + (size & 1);
                Check(end <= stream.Length, "Truncated WAV chunk: " + path);
                if (tag == "fmt ")
                {
                    Check(!format && size >= 16 && reader.ReadUInt16() == 1 && reader.ReadUInt16() == 1 &&
                        reader.ReadUInt32() == rate && reader.ReadUInt32() == rate * 3 &&
                        reader.ReadUInt16() == 3 && reader.ReadUInt16() == 24, "Expected native mono PCM-24: " + path);
                    format = true;
                }
                if (tag == "data")
                {
                    Check(!data && size > 0 && size % 3 == 0, "Invalid or empty WAV samples: " + path);
                    data = true;
                }
                stream.Position = end;
            }
            Check(format && data && stream.Position == stream.Length, "Incomplete WAV: " + path);
        }
    }

    private static T ReadJson<T>(string root, string file) where T : class =>
        JsonUtility.FromJson<T>(File.ReadAllText(Path.Combine(root, file))) ?? throw new InvalidDataException("Invalid JSON: " + file);
    private static bool Empty(string[] values) => values != null && values.Length == 0;
    private static bool Same(string a, string b) => (a ?? "") == (b ?? "");
    private static string SpokenKey(string value) => Regex.Replace(value.ToLowerInvariant().Replace('-', ' '), @"\s+", " ").Trim();
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
