"""Audition layout and old-path compatibility, separate from future production output."""
import json
from pathlib import Path
import re
from datetime import datetime, timezone
from uuid import uuid4

ROOT = Path(__file__).resolve().parent
SAMPLES = ROOT / "samples"
CHARACTERS = {"Swapna":"Lakshmamma_Swapna", "Priya":"Chinnamma_Priya", "Sneha":"Saraswati_Sneha",
              "Kabir":"Abdul_Kabir", "Thoma":"Francisco_Thoma", "Raghav":"FatherPenteado_Raghav"}


def resolve_sample_path(value):
    path = Path(value).resolve()
    if path.is_file(): return path
    manifest = SAMPLES / "relocation_manifest.json"
    if manifest.exists():
        records = json.loads(manifest.read_text(encoding="utf-8"))["files"]
        for item in records:
            if str(path).casefold() == item["original_path"].casefold():
                return Path(item["current_path"])
    return path


def new_sample_path(speaker, stage="improved", label=None):
    if stage not in ("improved", "comparisons"):
        raise ValueError("Generation writes auditions only; bulk production is disabled.")
    if stage == "improved" and speaker in CHARACTERS:
        folder = SAMPLES / "02_Improved_Audition" / CHARACTERS[speaker]
    else:
        folder = SAMPLES / "03_Comparisons" / re.sub(r"[^A-Za-z0-9_-]", "_", speaker)
    folder.mkdir(parents=True, exist_ok=True)
    if label is None:
        label = f"AUDITION_{speaker}_{datetime.now(timezone.utc):%Y%m%dT%H%M%SZ}_{uuid4().hex[:8]}"
    if not re.fullmatch(r"[A-Za-z0-9_-]+", label): raise ValueError("Unsafe sample label.")
    candidate = folder / (label + ".wav")
    take = 2
    while candidate.exists() or candidate.with_suffix(".json").exists():
        candidate = folder / f"{label}_take_{take:02d}.wav"
        take += 1
    return candidate


def fresh_export_path(folder, stem):
    path = folder / (stem + ".wav")
    if path.exists() or path.with_suffix(".json").exists():
        path = folder / (stem + "_" + uuid4().hex[:8] + ".wav")
    return path
