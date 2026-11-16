"""Eight-line audition driver for the RECOVERED studio, not a new TTS/batch engine.

Default: validate and print the plan. --generate-samples calls its ModelSession.
No all-lines mode, model download, preset edit, approval or Unity asset write.
"""
import argparse
import csv
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import sys


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--generate-samples", action="store_true")
    args = parser.parse_args()
    if args.generate_samples:
        parser.error('Original eight-clip audition is archived. Use the studio run_improved_audition.py for ONLY four revisions.')
    folder = Path(__file__).resolve().parent
    repo = folder.parents[2]
    manifest_path = folder / "sample_batch.json"
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    studio = Path(manifest["studio_path"])
    source = repo / manifest["source_csv"]
    jobs = manifest["jobs"]
    if not manifest["sample_only"] or manifest["full_batch_enabled"] or len(jobs) != 8:
        raise ValueError("Only the reviewed eight-job SAMPLE plan is allowed; full batch is disabled.")
    for path, key in [(source, "source_csv_sha256")]:
        if sha(path) != manifest[key]:
            raise ValueError(f"{path.name} changed; review/update the audition manifest before generating.")
    with source.open(encoding="utf-8-sig", newline="") as handle:
        inventory = {row["Dialogue_ID"]: row for row in csv.DictReader(handle)}
    presets = json.loads((studio / "character_presets.json").read_text(encoding="utf-8"))
    if len({job["sample_id"] for job in jobs}) != len(jobs):
        raise ValueError("Duplicate sample IDs.")
    for job in jobs:
        if inventory[job["dialogue_id"]]["Dialogue_Text"] != job["source_template"]:
            raise ValueError(f"Source text changed: {job['dialogue_id']}")
        text = job["source_template"]
        for key, value in job["substitutions"].items():
            text = text.replace("{" + key + "}", value)
        if text != job["text"] or "{" in text or "}" in text:
            raise ValueError("Unresolved/changed sample text.")
        if presets[job["preset_key"]]["speaker"] != job["speaker"]:
            raise ValueError("The recovered character voice assignment changed.")
        print(f"{job['sample_id']} | {job['dialogue_id']} | {job['speaker']} | {job['text']}")
    if not args.generate_samples:
        print("DRY RUN. No engine loaded or audio generated. Full batch disabled.")
        return

    # Force existing cached assets. Do not install, authenticate or download anything.
    os.environ["HF_HOME"] = str(studio / "cache" / "huggingface")
    os.environ["HF_HUB_OFFLINE"] = "1"
    os.environ["TRANSFORMERS_OFFLINE"] = "1"
    os.environ["GRADIO_ANALYTICS_ENABLED"] = "False"
    sys.dont_write_bytecode = True
    spec = importlib.util.spec_from_file_location("recovered_voice_studio", studio / "app.py")
    engine = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = engine
    spec.loader.exec_module(engine)  # Existing __main__ guard prevents launching Gradio/browser.
    if engine.MODEL_ID != manifest["model_id"]:
        raise ValueError("Unexpected engine/model; refusing substitution.")
    engine.SESSION.load()
    if getattr(engine.SESSION.model.config, "_commit_hash", None) != manifest["model_revision"]:
        raise ValueError("Unexpected cached model revision; no samples generated.")

    log = folder / "sample_runs.jsonl"
    previous = []
    if log.exists():
        for line in log.read_text(encoding="utf-8").splitlines():
            try:
                previous.append(json.loads(line))
            except ValueError:
                print("Ignoring an incomplete log row; it is not a completed sample.")

    def valid_wav(path):
        info = engine.sf.info(str(path))
        data, rate = engine.sf.read(str(path), dtype="float32")
        return (info.format == "WAV" and info.subtype == "PCM_24" and
                data.ndim == 1 and rate == 44100 and data.size > 0 and
                engine.np.isfinite(data).all() and float(engine.np.max(engine.np.abs(data))) >= 0.005)

    failures = 0
    for job in jobs:
        fingerprint = hashlib.sha256((sha(manifest_path) + json.dumps(job, sort_keys=True)).encode()).hexdigest()
        completed = False
        for old in reversed(previous):
            if old.get("fingerprint") != fingerprint or old.get("status") != "GENERATED_PENDING_REVIEW":
                continue
            path = Path(old["wav"])
            try:
                completed = path.is_file() and sha(path) == old["wav_sha256"] and valid_wav(path)
            except Exception:
                completed = False
            if completed:
                print(f"SKIP valid existing sample: {job['sample_id']} -> {path}")
                break
        if completed:
            continue
        event = {"sample_id": job["sample_id"], "dialogue_id": job["dialogue_id"],
                 "speaker": job["speaker"], "text": job["text"], "fingerprint": fingerprint}
        try:
            path, metadata = engine.SESSION.generate(
                job["text"], job["speaker"], presets[job["preset_key"]]["description"],
                job["mood"], job["pace"], job["expression"], preset_name="Recovered eight-line audition",
                seed=job["seed"], comparison_group="recovered-20261026")
            path = Path(path)  # Studio creates fresh timestamp/UUID masters; never canonical Unity WAVs.
            if not valid_wav(path):
                raise ValueError("WAV is empty, silent, non-finite or has an unexpected format.")
            event.update(status="GENERATED_PENDING_REVIEW", wav=str(path), wav_sha256=sha(path),
                         metadata=str(path.with_suffix(".json")), seed=metadata["seed"])
            print(f"PENDING REVIEW: {job['sample_id']} -> {path}")
        except Exception as exc:
            failures += 1
            event.update(status="FAILED", error=str(exc))
            print(f"FAILED {job['sample_id']}: {exc}")
        with log.open("a", encoding="utf-8") as handle:
            handle.write(json.dumps(event, ensure_ascii=False) + "\n")
    print(f"Sample failures: {failures}. Full batch remains disabled; no sample is approved.")
    if failures:
        raise SystemExit(1)


if __name__ == "__main__":
    main()
