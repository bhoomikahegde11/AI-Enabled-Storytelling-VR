from __future__ import annotations

import hashlib
import json
import math
import os
import secrets
import time
from datetime import datetime, timezone
from pathlib import Path
from uuid import uuid4

ROOT = Path(__file__).resolve().parent
CACHE = ROOT / "cache" / "huggingface"
os.environ.setdefault("HF_HOME", str(CACHE))
os.environ.setdefault("GRADIO_ANALYTICS_ENABLED", "False")

from tts_text import normalize_tts_text, NORMALIZATION_VERSION
from sample_storage import new_sample_path, resolve_sample_path, fresh_export_path

import gradio as gr
import numpy as np
import soundfile as sf

MODEL_ID = "ai4bharat/indic-parler-tts"
SPEAKERS = [
    "Thoma", "Mary", "Swapna", "Dinesh", "Meera", "Jatin", "Aakash",
    "Sneha", "Kabir", "Tisha", "Chingkhei", "Thoiba", "Priya", "Tarun",
    "Gauri", "Nisha", "Raghav", "Kavya", "Ravi", "Vikas", "Riya",
]
MOODS = {
    "neutral": "The delivery is conversational, responsive, and varied.",
    "friendly": "The delivery is welcoming and conversational.",
    "stern": "The delivery is firm and controlled.",
    "impatient": "The delivery conveys restrained urgency.",
    "happy": "The delivery sounds pleased and warm.",
    "persuasive": "The delivery is confident and gently persuasive.",
    "authoritative": "The delivery is calm and assured.",
}
EXAMPLES = {
    "Greeting": "Welcome to my stall. What brings you to the market today?",
    "Higher price": "Your offer is fair, but I would ask a little more for this measure.",
    "Rejecting": "I cannot accept that price for two seers of cardamom.",
    "Agreement": "We have a deal at one hundred twenty five varahas.",
    "Impatience": "We have discussed the price. Shall we decide?",
    "Trade words": "One varaha, two varahas, one seer, two seers, cardamom, and pepper.",
    "Number 11": "eleven",
    "Number 20": "twenty",
    "Number 100": "one hundred",
    "Number 200": "two hundred",
}
DELIVERY_NAMES = ("Natural Conversational", "Lively Negotiation", "Brisk Conversational")
SHORTLIST = ("Lakshmi Amma", "Chinnamma Naik", "Saraswati Chetti", "Abdul Rahman")
PACE_WORDING = {
    "slow": "a slower conversational",
    "moderate": "a normal, fluent conversational",
    "brisk": "a lively, fluent conversational",
}
EXPRESSION_WORDING = {
    "restrained": "subtle",
    "moderate": "natural",
    "high": "animated but believable",
}
PRESETS_PATH = ROOT / "character_presets.json"
SAMPLES = ROOT / "samples"
EXPORTS = ROOT / "exports"
for directory in (CACHE, SAMPLES, EXPORTS):
    directory.mkdir(parents=True, exist_ok=True)

def presets() -> dict:
    return json.loads(PRESETS_PATH.read_text(encoding="utf-8"))

def describe(speaker: str, base: str, mood: str, pace: str, expression: str) -> str:
    if speaker not in SPEAKERS:
        raise ValueError("Choose one of the documented English speakers.")
    base = base.strip().rstrip(".")
    if not base:
        raise ValueError("Enter a voice description.")
    return (
        f"{speaker}'s voice is {base}. {MOODS[mood]} "
        f"The words flow at {PACE_WORDING[pace]} pace with {EXPRESSION_WORDING[expression]} expression. "
        "The speaker responds naturally in connected phrases, with short pauses between sentences. "
        "The recording is clean and free of background noise."
    )

class ModelSession:
    def __init__(self) -> None:
        self.model = None
        self.prompt_tokenizer = None
        self.description_tokenizer = None
        self.torch = None

    def load(self) -> None:
        if self.model is not None:
            return
        import torch
        if not torch.cuda.is_available():
            raise RuntimeError("CUDA is unavailable in this environment. Check the NVIDIA driver and CUDA PyTorch installation; this studio does not silently switch to CPU.")
        from parler_tts import ParlerTTSForConditionalGeneration
        from transformers import AutoTokenizer
        try:
            model = ParlerTTSForConditionalGeneration.from_pretrained(
                MODEL_ID, torch_dtype=torch.float16, low_cpu_mem_usage=True,
                cache_dir=str(CACHE),
            ).to("cuda:0").eval()
            prompt_tokenizer = AutoTokenizer.from_pretrained(MODEL_ID, cache_dir=str(CACHE))
            description_tokenizer = AutoTokenizer.from_pretrained(
                model.config.text_encoder._name_or_path, cache_dir=str(CACHE)
            )
        except Exception as exc:
            message = str(exc)
            if "gated" in message.lower() or "401" in message or "403" in message:
                raise RuntimeError(
                    "Model access is gated. Accept the conditions at "
                    "https://huggingface.co/ai4bharat/indic-parler-tts, then run "
                    "'set HF_HOME=%CD%\\cache\\huggingface' and "
                    "'.venv\\Scripts\\hf.exe auth login' from this project directory."
                ) from exc
            raise RuntimeError(f"Could not load {MODEL_ID}: {message}") from exc
        self.torch = torch
        self.model = model
        self.prompt_tokenizer = prompt_tokenizer
        self.description_tokenizer = description_tokenizer

    def generate(self, text: str, speaker: str, base: str, mood: str,
                 pace: str, expression: str, preset_name: str = "Manual",
                 seed: int | None = None, comparison_group: str | None = None, *,
                 sample_label: str | None = None, sample_stage: str = "improved",
                 dialogue_id: str | None = None) -> tuple[str, dict]:
        original_text = text
        text = text.strip()
        if not text or len(text) > 500:
            raise ValueError("Enter 1–500 characters of dialogue.")
        tts_input = normalize_tts_text(text)
        if len(tts_input) > 1500 or ("{" in tts_input and "}" in tts_input):
            raise ValueError("Resolve placeholders and keep normalized speech under 1500 characters.")
        description = describe(speaker, base, mood, pace, expression)
        self.load()
        torch = self.torch
        seed = secrets.randbits(31) if seed is None else int(seed)
        torch.manual_seed(seed)
        started = time.perf_counter()
        try:
            desc = self.description_tokenizer(description, return_tensors="pt").to("cuda:0")
            prompt = self.prompt_tokenizer(tts_input, return_tensors="pt").to("cuda:0")
            with torch.inference_mode():
                result = self.model.generate(
                    input_ids=desc.input_ids,
                    attention_mask=desc.attention_mask,
                    prompt_input_ids=prompt.input_ids,
                    prompt_attention_mask=prompt.attention_mask,
                )
            samples = result[0].detach().float().cpu().numpy().reshape(-1)
            if not np.isfinite(samples).all() or len(samples) == 0:
                raise RuntimeError("The model returned empty or invalid audio.")
            if float(np.max(np.abs(samples))) < 0.005:
                raise RuntimeError("The model generated silence. Regenerate or try a short phrase with more context.")
            rate = int(self.model.config.sampling_rate)
            path = new_sample_path(speaker, sample_stage, sample_label)
            with path.open("xb") as handle:
                sf.write(handle, samples, rate, format="WAV", subtype="PCM_24")
            with sf.SoundFile(path) as saved:
                if saved.frames == 0 or saved.samplerate != rate:
                    raise RuntimeError("The generated WAV failed validation.")
            metadata = {
                "model": MODEL_ID,
                "model_revision": getattr(self.model.config, "_commit_hash", None),
                "speaker": speaker,
                "dialogue_text": original_text,
                "original_text": original_text,
                "tts_input": tts_input,
                "text_normalization": NORMALIZATION_VERSION,
                "dialogue_id": dialogue_id,
                "generation_settings": json.loads(self.model.generation_config.to_json_string(use_diff=False)),
                "model_output_sample_rate": rate,
                "codec_sample_rate": int(self.model.config.audio_encoder.sampling_rate),
                "voice_description": description,
                "voice_description_base": base,
                "delivery_preset": preset_name,
                "comparison_group": comparison_group,
                "processing": {"method": "none", "speed_ratio": 1.0, "sample_rate": rate},
                "mood": mood,
                "pace": pace,
                "expressiveness": expression,
                "seed": seed,
                "sample_rate": rate,
                "duration_seconds": round(len(samples) / rate, 3),
                "generation_seconds": round(time.perf_counter() - started, 3),
                "recording_status": "PENDING",
                "created_utc": datetime.now(timezone.utc).isoformat(),
            }
            with path.with_suffix(".json").open("x", encoding="utf-8") as handle:
                handle.write(json.dumps(metadata, indent=2, ensure_ascii=False))
            return str(path), metadata
        except torch.cuda.OutOfMemoryError as exc:
            torch.cuda.empty_cache()
            raise RuntimeError("CUDA ran out of memory. Close other GPU applications and retry a shorter line.") from exc

SESSION = ModelSession()

def audition(text, speaker, base, mood, pace, expression, delivery_name):
    try:
        path, meta = SESSION.generate(
            text, speaker, base, mood, pace, expression, preset_name=delivery_name
        )
        return path, path, path, f"Saved {Path(path).name} at {meta['sample_rate']} Hz in {meta['generation_seconds']} s. Pending review."
    except Exception as exc:
        return None, None, None, f"Generation failed: {exc}"

def compare(text, selected, base, mood, pace, expression):
    if not selected or len(selected) > 4:
        return None, None, None, None, "Choose 1–4 speakers."
    files, messages = [], []
    for speaker in selected:
        try:
            path, meta = SESSION.generate(text, speaker, base, mood, pace, expression)
            files.append(path)
            messages.append(f"{speaker}: {Path(path).name} ({meta['generation_seconds']} s)")
        except Exception as exc:
            files.append(None)
            messages.append(f"{speaker}: {exc}")
    return *(files + [None] * (4 - len(files))), "\n".join(messages)

def load_character(name):
    if name == "Custom":
        return gr.update(interactive=True), gr.update(), gr.update(), gr.update(), gr.update(), "Manual"
    item = presets()[name]
    variant = item.get("delivery_presets", {}).get(DELIVERY_NAMES[0])
    if variant:
        return (
            gr.update(value=item["speaker"], interactive=False),
            variant["description"], variant["mood"], variant["pace"],
            variant["expression"], DELIVERY_NAMES[0],
        )
    return (
        gr.update(value=item["speaker"], interactive=True),
        item["description"], "neutral", "moderate", "moderate", "Manual",
    )

def load_delivery(name, delivery_name):
    if name == "Custom":
        return gr.update(), gr.update(), gr.update(), gr.update()
    item = presets()[name]
    variant = item.get("delivery_presets", {}).get(delivery_name)
    if variant:
        return variant["description"], variant["mood"], variant["pace"], variant["expression"]
    return item["description"], "neutral", "moderate", "moderate"

def save_preset(name, delivery_name, speaker, description, mood, pace, expression):
    if name == "Custom":
        return "Select a character preset to save."
    if speaker not in SPEAKERS or not description.strip():
        return "Choose a documented speaker and enter a description."
    data = presets()
    if name in SHORTLIST and speaker != data[name]["speaker"]:
        return "This shortlisted character's speaker is fixed; edit the description instead."
    item = data[name]
    if delivery_name in item.get("delivery_presets", {}):
        item["delivery_presets"][delivery_name] = {
            "description": description.strip(), "mood": mood,
            "pace": pace, "expression": expression,
        }
        if delivery_name == DELIVERY_NAMES[0]:
            item["description"] = description.strip()
    else:
        item["description"] = description.strip()
    item["approved"] = False
    PRESETS_PATH.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    return f"Saved {name}'s draft {delivery_name} preset. Voice approval remains manual."

def pitch_preserving_speed(source_path, speed):
    speed = round(float(speed), 2)
    if speed not in (1.0, 1.08, 1.15):
        raise ValueError("Choose 1.00×, 1.08×, or 1.15×.")
    source = resolve_sample_path(source_path)
    if not source.is_relative_to(SAMPLES.resolve()) or not source.is_file():
        raise ValueError("Choose a generated WAV master from this studio.")
    if speed == 1.0:
        return str(source)
    source_hash = hashlib.sha256(source.read_bytes()).hexdigest()
    output = EXPORTS / f"{source.stem}_speed_{speed:.2f}x.wav"
    sidecar = output.with_suffix(".json")
    if output.exists() and sidecar.exists():
        previous = json.loads(sidecar.read_text(encoding="utf-8"))
        if previous.get("source_sha256") == source_hash:
            return str(output)
    output = fresh_export_path(EXPORTS, f"{source.stem}_speed_{speed:.2f}x")
    sidecar = output.with_suffix(".json")
    import librosa
    started = time.perf_counter()
    samples, rate = sf.read(source, dtype="float32")
    if samples.ndim != 1 or samples.size == 0 or not np.isfinite(samples).all():
        raise ValueError("Speed comparison requires a valid mono WAV master.")
    adjusted = librosa.effects.time_stretch(samples, rate=speed)
    if not np.isfinite(adjusted).all() or float(np.max(np.abs(adjusted))) < 0.005:
        raise RuntimeError("The speed-adjusted comparison produced invalid or silent audio.")
    sf.write(output, adjusted, rate, subtype="PCM_24")
    metadata = json.loads(source.with_suffix(".json").read_text(encoding="utf-8"))
    metadata.update({
        "derived_from_master": source.name,
        "source_sha256": source_hash,
        "sample_rate": rate,
        "duration_seconds": round(len(adjusted) / rate, 3),
        "processing_seconds": round(time.perf_counter() - started, 3),
        "processing": {
            "method": "librosa.effects.time_stretch",
            "library_version": librosa.__version__,
            "speed_ratio": speed,
            "pitch_preserving": True,
            "internal_pause_shortening": False,
            "source_sample_rate": rate,
            "output_sample_rate": rate,
        },
        "recording_status": "PENDING",
        "created_utc": datetime.now(timezone.utc).isoformat(),
    })
    sidecar.write_text(json.dumps(metadata, indent=2, ensure_ascii=False), encoding="utf-8")
    return str(output)

def load_delivery_comparison(character):
    item = presets()[character]
    return (item["speaker"], *(item["delivery_presets"][name]["description"] for name in DELIVERY_NAMES))

def compare_deliveries(text, character, speaker, natural, lively, brisk, speed):
    item = presets()[character]
    if speaker != item["speaker"]:
        return *(None for _ in range(6)), [None, None, None], "The comparison speaker must match the shortlisted voice."
    group = uuid4().hex[:12]
    seed = secrets.randbits(31)
    masters, played, messages = [], [], []
    for name, edited in zip(DELIVERY_NAMES, (natural, lively, brisk)):
        variant = item["delivery_presets"][name]
        try:
            master, metadata = SESSION.generate(
                text, speaker, edited, variant["mood"], variant["pace"],
                variant["expression"], preset_name=name, seed=seed,
                comparison_group=group,
            )
        except Exception as exc:
            masters.append(None)
            played.append(None)
            messages.append(f"{name}: generation failed: {exc}")
            continue
        masters.append(master)
        try:
            played.append(pitch_preserving_speed(master, speed))
            messages.append(f"{name}: {Path(master).name} ({metadata['generation_seconds']} s)")
        except Exception as exc:
            played.append(None)
            messages.append(f"{name}: original saved as {Path(master).name}; speed preview failed: {exc}")
    return *played, *masters, masters, "\n".join(messages)

def replay_delivery_speed(masters, speed):
    if not masters or not any(masters):
        return None, None, None, "Generate the three delivery presets first."
    played, messages = [], []
    for name, master in zip(DELIVERY_NAMES, masters):
        if not master:
            played.append(None)
            messages.append(f"{name}: no original master available.")
            continue
        try:
            played.append(pitch_preserving_speed(master, speed))
            messages.append(f"{name}: {float(speed):.2f}× preview; original master retained.")
        except Exception as exc:
            played.append(None)
            messages.append(f"{name}: {exc}")
    return *played, "\n".join(messages)

def join_preview(hundred, twenty, five, gap_ms):
    if not all((hundred, twenty, five)):
        return None, "Upload three clips: 100, 20, and 5."
    try:
        clips = []
        rates = []
        for path in (hundred, twenty, five):
            path = resolve_sample_path(path)
            data, rate = sf.read(path, dtype="float32", always_2d=False)
            if data.ndim != 1:
                raise ValueError("Join preview expects mono WAV clips.")
            if data.size == 0 or not np.isfinite(data).all() or float(np.max(np.abs(data))) < 0.005:
                raise ValueError("A join-preview clip is empty or silent; regenerate it first.")
            clips.append(data)
            rates.append(rate)
        if len(set(rates)) != 1:
            raise ValueError("All three clips must have the same sample rate.")
        gap = np.zeros(round(rates[0] * gap_ms / 1000), dtype=np.float32)
        assembled = np.concatenate((clips[0], gap, clips[1], gap, clips[2]))
        out = new_sample_path("NumberJoin", "comparisons", f"preview_125_{uuid4().hex[:8]}")
        sf.write(out, assembled, rates[0], subtype="PCM_24")
        return str(out), f"Joined 100 + 20 + 5 at {rates[0]} Hz with {gap_ms} ms gaps."
    except Exception as exc:
        return None, f"Preview failed: {exc}"

def export_rate(source, target_rate):
    if not source:
        return None, "Generate a master first."
    try:
        from scipy.signal import resample_poly
        source = str(resolve_sample_path(source))
        data, source_rate = sf.read(source, dtype="float32")
        target_rate = int(target_rate)
        divisor = math.gcd(source_rate, target_rate)
        converted = resample_poly(data, target_rate // divisor, source_rate // divisor)
        out = fresh_export_path(EXPORTS, f"{Path(source).stem}_{target_rate}hz")
        sf.write(out, converted, target_rate, subtype="PCM_24")
        source_path = Path(source)
        source_metadata = source_path.with_suffix(".json")
        metadata = json.loads(source_metadata.read_text(encoding="utf-8")) if source_metadata.exists() else {}
        metadata.update({
            "derived_from_master": source_path.name,
            "source_sha256": hashlib.sha256(source_path.read_bytes()).hexdigest(),
            "sample_rate": target_rate,
            "duration_seconds": round(len(converted) / target_rate, 3),
            "processing": {
                "method": "scipy.signal.resample_poly",
                "speed_ratio": 1.0,
                "pitch_preserving": True,
                "internal_pause_shortening": False,
                "source_sample_rate": source_rate,
                "output_sample_rate": target_rate,
            },
            "recording_status": "PENDING",
            "created_utc": datetime.now(timezone.utc).isoformat(),
        })
        out.with_suffix(".json").write_text(json.dumps(metadata, indent=2, ensure_ascii=False), encoding="utf-8")
        return str(out), f"Separate {target_rate} Hz export saved; native-rate master retained."
    except Exception as exc:
        return None, f"Export failed: {exc}"

with gr.Blocks(title="NPC Voice Generation Studio", theme=gr.themes.Soft()) as demo:
    gr.Markdown("# NPC Voice Generation Studio\\nLocal audition only · AI4Bharat Indic Parler-TTS · No voice is auto-approved")
    with gr.Tab("Voice audition"):
        with gr.Row():
            character = gr.Dropdown(["Custom", *presets().keys()], value="Custom", label="Character draft preset")
            delivery = gr.Dropdown(["Manual", *DELIVERY_NAMES], value="Manual", label="Delivery preset")
            speaker = gr.Dropdown(SPEAKERS, value="Mary", label="Documented English speaker")
        base = gr.Textbox(value="a responsive adult speaking naturally with a person nearby", label="Editable voice description", lines=3)
        save_button = gr.Button("Save draft character preset")
        preset_status = gr.Textbox(label="Preset status", interactive=False)
        with gr.Row():
            mood = gr.Dropdown(list(MOODS), value="neutral", label="Mood")
            pace = gr.Dropdown(["slow", "moderate", "brisk"], value="moderate", label="Speaking-rate description")
            expression = gr.Dropdown(["restrained", "moderate", "high"], value="moderate", label="Expressiveness description")
        example = gr.Dropdown(list(EXAMPLES), value="Greeting", label="Example")
        text = gr.Textbox(value=EXAMPLES["Greeting"], label="Dialogue text", lines=3)
        with gr.Row():
            generate_button = gr.Button("Generate", variant="primary")
            regenerate_button = gr.Button("Regenerate")
        audio = gr.Audio(label="Original native-rate WAV audition")
        download = gr.File(label="Download original WAV")
        last_path = gr.State()
        status = gr.Textbox(label="Generation status", interactive=False)
        with gr.Row():
            export_target = gr.Dropdown([24000, 44100, 48000], value=48000, label="Separate export sample rate")
            export_button = gr.Button("Export converted WAV")
        export_file = gr.File(label="Download converted WAV")
        export_status = gr.Textbox(label="Export status", interactive=False)
        character.change(load_character, character, [speaker, base, mood, pace, expression, delivery])
        delivery.change(load_delivery, [character, delivery], [base, mood, pace, expression])
        save_button.click(save_preset, [character, delivery, speaker, base, mood, pace, expression], preset_status)
        example.change(lambda key: EXAMPLES[key], example, text)
        inputs = [text, speaker, base, mood, pace, expression, delivery]
        for button in (generate_button, regenerate_button):
            button.click(audition, inputs, [audio, download, last_path, status])
        export_button.click(export_rate, [last_path, export_target], [export_file, export_status])
    with gr.Tab("Delivery preset comparison"):
        gr.Markdown(
            "Compare the same line with the same shortlisted speaker. Edit any of the three descriptions before generating. "
            "Speed previews preserve pitch and are saved separately; original WAVs remain available below. "
            "Internal pauses are not trimmed."
        )
        with gr.Row():
            delivery_character = gr.Dropdown(list(SHORTLIST), value="Abdul Rahman", label="Character")
            delivery_speaker = gr.Textbox(value=presets()["Abdul Rahman"]["speaker"], label="Fixed shortlisted speaker", interactive=False)
            speed_choice = gr.Radio([1.0, 1.08, 1.15], value=1.0, label="Pitch-preserving preview speed")
        delivery_text = gr.Textbox(value=EXAMPLES["Higher price"], label="Same dialogue text for all three", lines=3)
        default_variants = presets()["Abdul Rahman"]["delivery_presets"]
        descriptions = [
            gr.Textbox(value=default_variants[name]["description"], label=f"{name} · editable description", lines=3)
            for name in DELIVERY_NAMES
        ]
        delivery_button = gr.Button("Generate three delivery variants", variant="primary")
        with gr.Row():
            delivery_audio = [gr.Audio(label=name) for name in DELIVERY_NAMES]
        with gr.Row():
            delivery_masters = [gr.File(label=f"{name} · original WAV") for name in DELIVERY_NAMES]
        master_paths = gr.State([None, None, None])
        delivery_status = gr.Textbox(label="Comparison status", lines=4, interactive=False)
        delivery_character.change(load_delivery_comparison, delivery_character, [delivery_speaker, *descriptions])
        delivery_button.click(
            compare_deliveries,
            [delivery_text, delivery_character, delivery_speaker, *descriptions, speed_choice],
            [*delivery_audio, *delivery_masters, master_paths, delivery_status],
        )
        speed_choice.change(replay_delivery_speed, [master_paths, speed_choice], [*delivery_audio, delivery_status])
    with gr.Tab("Speaker comparison"):
        gr.Markdown("Generate the same sentence sequentially for up to four speakers. All clips remain pending review.")
        compare_text = gr.Textbox(value=EXAMPLES["Trade words"], label="Shared sentence", lines=2)
        compare_speakers = gr.CheckboxGroup(SPEAKERS, value=["Mary", "Swapna"], label="Speakers (1–4)")
        compare_button = gr.Button("Compare speakers")
        compare_outputs = [gr.Audio(label=f"Comparison {i}") for i in range(1, 5)]
        compare_status = gr.Textbox(label="Comparison status", lines=5, interactive=False)
        compare_button.click(compare, [compare_text, compare_speakers, base, mood, pace, expression], [*compare_outputs, compare_status])
    with gr.Tab("Clip join preview"):
        gr.Markdown("Upload separately generated **100**, **20**, and **5** clips to hear “one hundred twenty five”.")
        pieces = [gr.Audio(label=word, type="filepath", sources=["upload"]) for word in ("100", "20", "5")]
        gap = gr.Slider(0, 150, value=35, step=5, label="Gap between clips (ms)")
        join_button = gr.Button("Preview joined number")
        joined = gr.Audio(label="Joined preview")
        join_status = gr.Textbox(label="Preview status", interactive=False)
        join_button.click(join_preview, [*pieces, gap], [joined, join_status])
demo.queue(max_size=8, default_concurrency_limit=1)
if __name__ == "__main__":
    demo.launch(server_name="127.0.0.1", server_port=7860, share=False, inbrowser=True, allowed_paths=[str(SAMPLES), str(EXPORTS)])
