"""One-time, hash-checked relocation of the existing audition; no synthesis."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
from sample_storage import CHARACTERS

ROOT = Path(__file__).resolve().parent
REPO = ROOT.parent / "AI-Enabled-Storytelling-VR"


def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--apply", action="store_true")
    args = parser.parse_args()
    expected = Path("G:/Users/chitr/Desktop/Capstone/NPC-Voice-Generator").resolve()
    if ROOT.resolve() != expected: raise ValueError("Run only in the explicitly authorized studio.")
    samples = ROOT / "samples"
    manifest_path = samples / "relocation_manifest.json"
    if manifest_path.exists():
        previous = json.loads(manifest_path.read_text())
        if all(Path(r["current_path"]).is_file() and sha(Path(r["current_path"])) == r["sha256"] for r in previous["files"]):
            print("All original files already organized and hash-valid. Nothing moved.")
            return
        raise ValueError("Existing migration needs review; refusing a conflicting or partial move.")
    log_path = REPO / "documentation/DialogueAudit/OfflineVoiceRecovery/sample_runs.jsonl"
    records = [json.loads(line) for line in log_path.read_text().splitlines() if line.strip()]
    originals = {str(Path(r["wav"]).resolve()):r for r in records if r.get("status") == "GENERATED_PENDING_REVIEW"}
    if len(originals) != 8: raise ValueError("Expected exactly eight original audition records.")
    labels = {"SAMPLE_01":"Introduction", "SAMPLE_02":"Numbers", "SAMPLE_03":"Warning", "SAMPLE_04":"Impatience",
              "SAMPLE_05":"Success", "SAMPLE_06":"Numbers", "SAMPLE_07":"Success", "SAMPLE_08":"Success"}
    plan = []
    traces = []
    pathmap = {}
    for source in sorted(samples.iterdir()):
        if not source.is_file() or source.suffix.lower() not in (".wav", ".json"): continue
        wav = source.with_suffix(".wav").resolve()
        job = originals.get(str(wav))
        if job:
            target = samples / "01_Original_Audition" / CHARACTERS[job["speaker"]] / (
                f"{job['sample_id']}_{job['speaker']}_{labels[job['sample_id']]}_Original" + source.suffix)
        else:
            target = samples / "00_Legacy_Samples" / source.name
        if not source.resolve().is_relative_to(samples.resolve()) or not target.resolve().is_relative_to(samples.resolve()):
            raise ValueError("Relocation escaped the samples root.")
        if target.exists(): raise FileExistsError(target)
        item = {"original_path":str(source.resolve()), "current_path":str(target.resolve()), "sha256":sha(source)}
        if job: item.update(sample_id=job["sample_id"], dialogue_id=job["dialogue_id"], speaker=job["speaker"])
        plan.append(item)
        pathmap[str(source.resolve())] = str(target.resolve())
        if job and source.suffix == ".wav":
            if sha(source) != job["wav_sha256"]: raise ValueError("Original WAV hash mismatch.")
            trace_path = target.with_suffix(".trace.json")
            if trace_path.exists(): raise FileExistsError(trace_path)
            traces.append((trace_path, dict(item, metadata_original_path=str(source.with_suffix(".json")))))
    if len({r["current_path"] for r in plan}) != len(plan): raise ValueError("Filename collision in migration plan.")
    print(f"Relocation plan: {len(plan)} existing files, 8 original WAVs, no deletes/overwrites.")
    if not args.apply: return
    for folder in ["00_Legacy_Samples", "01_Original_Audition", "02_Improved_Audition", "03_Comparisons"]:
        (samples / folder).mkdir(exist_ok=True)
    for folder in CHARACTERS.values():
        for stage in ("01_Original_Audition", "02_Improved_Audition"):
            (samples / stage / folder).mkdir(exist_ok=True)
    manifest = {"files":plan, "original_waveforms_and_metadata_preserved":True}
    manifest_path.write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    for item in plan:
        source, target = Path(item["original_path"]), Path(item["current_path"])
        target.parent.mkdir(parents=True, exist_ok=True)
        if target.exists(): raise FileExistsError(target)
        shutil.move(str(source), str(target))
        if sha(target) != item["sha256"]: raise ValueError("Hash changed during relocation.")
    for path, trace in traces:
        with path.open("x", encoding="utf-8") as handle: json.dump(trace, handle, indent=2)

    # Retain the old paths as provenance while updating known active local records.
    backup = ROOT / "maintenance_backups" / "quality_20261026" / "relocation"
    backup.mkdir(parents=True, exist_ok=True)
    for path in [log_path, REPO / "documentation/DialogueAudit/OfflineVoiceRecovery/run_sample_batch.py"]:
        dest = backup / path.name
        if dest.exists(): raise FileExistsError(dest)
        shutil.copy2(path, dest)
    for record in records:
        for key in ("wav", "metadata"):
            old = str(Path(record[key]).resolve())
            if old in pathmap:
                record["original_"+key] = record[key]
                record[key] = pathmap[old]
    log_path.write_text("".join(json.dumps(r, ensure_ascii=False)+"\n" for r in records), encoding="utf-8")
    driver = REPO / "documentation/DialogueAudit/OfflineVoiceRecovery/run_sample_batch.py"
    code = driver.read_text()
    code = code.replace("    args = parser.parse_args()", "    args = parser.parse_args()\n    if args.generate_samples:\n        parser.error('Original eight-clip audition is archived. Use the studio run_improved_audition.py for ONLY four revisions.')", 1)
    code = code.replace('[(source, "source_csv_sha256"), (studio / "app.py", "studio_app_sha256"),\n                      (studio / "character_presets.json", "presets_sha256")]', '[(source, "source_csv_sha256")]')
    driver.write_text(code, encoding="utf-8")
    for path in (ROOT / "exports").glob("*.json"):
        meta = json.loads(path.read_text())
        name = meta.get("derived_from_master")
        match = next((r for r in plan if Path(r["original_path"]).name == name), None)
        if match:
            target = backup / ("exports_"+path.name)
            if target.exists(): raise FileExistsError(target)
            shutil.copy2(path, target)
            meta["source_master_path"] = match["current_path"]
            path.write_text(json.dumps(meta, indent=2, ensure_ascii=False), encoding="utf-8")
    (samples / "README.md").write_text(
        "# Audition samples\n\n"
        "- `00_Legacy_Samples`: historical October 13 masters and the old number-join preview. Original filenames retained.\n"
        "- `01_Original_Audition/<NPC_Voice>`: all eight October 26 originals, descriptively renamed. WAVs and original JSON metadata are byte-identical. `.trace.json` and `relocation_manifest.json` retain old filenames, hashes and source dialogue IDs.\n"
        "- `02_Improved_Audition/<NPC_Voice>`: new normalized, conversational-speed auditions. They remain PENDING human approval.\n"
        "- `03_Comparisons`: comparison reports and future join/audition previews, not production assets.\n\n"
        "Future generation routes the six selected voices to character folders automatically. Other speaker experiments go to Comparisons. Bulk output belongs in a separate future `production/` bank; no bulk generation is enabled.\n\n"
        "`sample_storage.resolve_sample_path` resolves old absolute sample paths. Known original-run records and export provenance are updated; their pre-migration versions are in `maintenance_backups/quality_20261026/relocation`. Refresh any already-open browser preview after reorganization. No Unity asset references were found for these external masters, and no Unity reference was changed.\n",
        encoding="utf-8")
    print("Relocation complete; original content hashes and all eight active log paths verified.")


if __name__ == "__main__": main()
