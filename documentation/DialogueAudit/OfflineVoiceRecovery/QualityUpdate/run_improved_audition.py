"""Only the four authorized comparisons; default is measurement/plan, not synthesis."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import sys
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parent
REPO = ROOT.parent / "AI-Enabled-Storytelling-VR"
for key, value in {"HF_HOME":str(ROOT / "cache/huggingface"), "HF_HUB_OFFLINE":"1",
                   "TRANSFORMERS_OFFLINE":"1", "GRADIO_ANALYTICS_ENABLED":"False"}.items(): os.environ[key] = value
sys.dont_write_bytecode = True
from audio_quality import measure_wav
from tts_text import normalize_tts_text, NORMALIZATION_VERSION

JOBS = [
    ("SAMPLE_01", "SAMPLE_02", "Swapna", "Lakshmi Amma", "Numbers", "stern", "moderate", "moderate",
     "Ayyo, two hundred and fifty varahas? That is too much. My offer remains one hundred and seventy-three varahas."),
    ("SAMPLE_02", "SAMPLE_06", "Thoma", "Francisco", "Numbers", "persuasive", "moderate", "high",
     "Ora! Three hundred varahas is beyond what this lot can bear. I remain at one hundred and eighty varahas."),
    ("SAMPLE_03", "SAMPLE_01", "Swapna", "Lakshmi Amma", "Pacing", "friendly", "moderate", "moderate",
     "I am Lakshmamma. And you, merchant?"),
    ("SAMPLE_04", "SAMPLE_05", "Kabir", "Abdul Rahman", "Pacing", "happy", "moderate", "moderate",
     "Shukran, merchant. The goods are here, and you have kept your word.")]


def sha(path): return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--generate", action="store_true")
    args = parser.parse_args()
    original_log = REPO / "documentation/DialogueAudit/OfflineVoiceRecovery/sample_runs.jsonl"
    originals = {r["sample_id"]:r for r in map(json.loads, original_log.read_text().splitlines())}
    presets = json.loads((ROOT / "character_presets.json").read_text())
    out = ROOT / "samples/03_Comparisons"
    out.mkdir(parents=True, exist_ok=True)
    log_path = out / "improved_runs.jsonl"
    previous = []
    if log_path.exists():
        for line in log_path.read_text().splitlines():
            try: previous.append(json.loads(line))
            except ValueError: print("Incomplete previous log row ignored, not counted as a success.")
    source_hash = sha(ROOT / "app.py") + sha(ROOT / "character_presets.json") + sha(ROOT / "tts_text.py")
    engine = None
    comparisons = []
    for sample_id, original_id, speaker, key, label, mood, pace, expression, expected in JOBS:
        original = originals[original_id]
        if original["speaker"] != speaker or presets[key]["speaker"] != speaker:
            raise ValueError("Selected speaker assignment changed.")
        if sha(original["wav"]) != original["wav_sha256"]: raise ValueError("Original WAV hash changed.")
        original_meta = json.loads(Path(original["metadata"]).read_text())
        original_text = original_meta["dialogue_text"]
        if normalize_tts_text(original_text) != expected: raise ValueError("Revised text differs from the authorized script.")
        fingerprint = hashlib.sha256((source_hash + json.dumps([sample_id, original["dialogue_id"], expected, speaker, mood, pace, expression, original_meta["seed"]])).encode()).hexdigest()
        item = {"sample_id":sample_id, "source_original_sample_id":original_id, "dialogue_id":original["dialogue_id"],
                "speaker":speaker, "original_path":original["wav"], "original_metadata_path":original["metadata"],
                "original_input":original_meta.get("tts_input", original_text), "revised_input":expected,
                "original_metrics":measure_wav(original["wav"], normalize_tts_text(original_text)),
                "original_settings":original_meta, "fingerprint":fingerprint, "status":"NOT_GENERATED"}
        for old in reversed(previous):
            if old.get("fingerprint") != fingerprint or old.get("status") != "PENDING_HUMAN_REVIEW": continue
            try:
                metrics = measure_wav(old["revised_path"], expected)
                if sha(old["revised_path"]) != old["wav_sha256"]: continue
                if metrics["sample_rate"] != 44100 or metrics["encoding"] != "PCM_24": continue
                item.update(old)
                break
            except Exception: continue
        if args.generate and item["status"] == "NOT_GENERATED":
            try:
                if engine is None:
                    import app as engine
                wav, metadata = engine.SESSION.generate(original_text, speaker, presets[key]["description"],
                    mood, pace, expression, preset_name="Natural-speed quality comparison",
                    seed=original_meta["seed"], comparison_group="quality-20261026",
                    sample_label=f"{sample_id}_{speaker}_{label}_Improved", dialogue_id=original["dialogue_id"])
                if metadata["model_revision"] != original_meta["model_revision"]:
                    raise ValueError("Unexpected model revision; output is not approved.")
                metrics = measure_wav(wav, metadata["tts_input"])
                if metrics["sample_rate"] != 44100 or metrics["encoding"] != "PCM_24" or metrics["peak"] < .005:
                    raise ValueError("Invalid native WAV output.")
                item.update(status="PENDING_HUMAN_REVIEW", revised_path=wav, revised_metadata_path=str(Path(wav).with_suffix(".json")),
                            revised_settings=metadata, revised_metrics=metrics, wav_sha256=sha(wav))
            except Exception as exc:
                item.update(status="FAILED", error=str(exc))
            with log_path.open("a", encoding="utf-8") as handle:
                handle.write(json.dumps(item, ensure_ascii=False) + "\n")
        print(sample_id, item["status"], item.get("revised_path", item.get("error", expected)), flush=True)
        comparisons.append(item)
    all_originals = []
    for record in originals.values():
        meta = json.loads(Path(record["metadata"]).read_text())
        all_originals.append({"sample_id":record["sample_id"], "path":record["wav"],
                              "metrics":measure_wav(record["wav"], normalize_tts_text(meta["dialogue_text"]))})
    result = {"created_utc":datetime.now(timezone.utc).isoformat(), "normalization":NORMALIZATION_VERSION,
              "post_processing":"none; 1.00x native masters; no trimming, resampling or pause deletion",
              "full_batch_enabled":False, "comparisons":comparisons, "all_eight_originals":all_originals}
    (out / "comparison_metrics.json").write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    report = ["# Indic Parler-TTS audio comparison — 2026-10-26", "",
        "**All revised recordings require human listening approval. Shorter duration is not proof of better speech.**", "",
        "## Diagnosis", "",
        "All eight original WAV headers and sidecar rates are 44,100 Hz / mono / PCM_24. The same cached model's DAC codec is 44,100 Hz. No sample-rate mismatch or metadata-only slowdown was found. Generation used the cached model's sampling configuration, not a speed-halving transform. A playback-device issue cannot be excluded without listening/device validation.", "",
        "The measured recordings differ substantially: some have long edge/internal low-energy intervals, others do not. Uniform 0.5x speech is not established by these measurements. Low-energy timing and older pause-heavy descriptions support investigating model phrasing/pauses. Raw numeric strings reached the old tokenizer without normalization; the revised path expands them deterministically before tokenization. Intelligibility still requires listening.", "",
        "RMS measurements use 20 ms frames, threshold -45 dBFS. These are energy gaps, not word alignment: quiet consonants can be counted. Internal gaps >=350 ms are listed, not automatically removed or declared excessive. Intended-word WPM uses the normalized script, not recognized/verified speech. No new masters are sped up or trimmed. Existing optional 1.08x / 1.15x pitch-preserving previews remain available separately and were not applied here.", "",
        "## All original clips", "", "| Original | Seconds | Leading quiet | Trailing quiet | Total quiet | Internal gaps >=350ms |", "|---|---:|---:|---:|---:|---|"]
    for record in all_originals:
        m = record["metrics"]
        report.append(f"| {record['sample_id']} | {m['duration_seconds']} | {m['leading_low_energy_seconds']} | {m['trailing_low_energy_seconds']} | {m['total_low_energy_seconds']} | {m['internal_low_energy_gaps_at_least_350ms']} |")
    if all(item.get("revised_metrics") for item in comparisons):
        report += ["", "## Measured comparison outcome", "",
                   "**The new descriptions did not consistently accelerate delivery. This is not an approved universal pacing fix.** Expanded price text and changed descriptions must be judged by listening, not duration alone."]
        for item in comparisons:
            report.append(f"- {item['sample_id']} / {item['speaker']}: {item['original_metrics']['duration_seconds']}s → {item['revised_metrics']['duration_seconds']}s; low-energy total {item['original_metrics']['total_low_energy_seconds']}s → {item['revised_metrics']['total_low_energy_seconds']}s.")
    for item in comparisons:
        report += ["", f"## {item['sample_id']} — {item['speaker']} / {item['dialogue_id']}", "",
                   f"Status: **{item['status']}**. Original audition identity: {item['source_original_sample_id']}.",
                   f"\nOriginal WAV: `{item['original_path']}`", f"\nRevised WAV: `{item.get('revised_path','not generated')}`",
                   f"\nOriginal TTS input:\n```text\n{item['original_input']}\n```",
                   f"\nRevised TTS input:\n```text\n{item['revised_input']}\n```"]
        if item.get("error"): report.append("Generation error: " + item["error"])
        report += ["", "| Measurement | Original | Revised |", "|---|---|---|"]
        for field in ["duration_seconds", "sample_rate", "encoding", "leading_low_energy_seconds", "trailing_low_energy_seconds", "total_low_energy_seconds", "internal_low_energy_gaps_at_least_350ms", "intended_words_per_minute"]:
            report.append(f"| {field} | {item['original_metrics'][field]} | {item.get('revised_metrics',{}).get(field,'n/a')} |")
        report += ["", "Original generation metadata:", "```json", json.dumps(item["original_settings"],indent=2,ensure_ascii=False), "```",
                   "", "Revised generation metadata:", "```json", json.dumps(item.get("revised_settings",{}),indent=2,ensure_ascii=False), "```"]
    report += ["", "## Review and scope", "", "Same model and original per-pair seed; speaker identities unchanged. Number normalization and delivery descriptions changed together, so this is not a causal isolation experiment. No automatic approval, phonetic rewrites, automatic retries, pitch shift or full-library generation. All originals and their original sidecars are hash-preserved under the relocation manifest. Old active driver log paths were updated; old source filenames remain in provenance. The unknown Record Now/Record Later selection is not reconstructed or invented. Future bulk output must remain separate from auditions."]
    (ROOT / "AUDIO_COMPARISON_REPORT.md").write_text("\n".join(report)+"\n",encoding="utf-8")
    if args.generate and any(r["status"] == "FAILED" for r in comparisons): raise SystemExit(1)


if __name__ == "__main__": main()
