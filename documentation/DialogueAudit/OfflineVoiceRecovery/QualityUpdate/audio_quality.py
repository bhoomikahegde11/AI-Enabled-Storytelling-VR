"""Energy-based timing diagnostics, not speech alignment or a listening verdict."""
import re
import numpy as np
import soundfile as sf


def measure_wav(path, intended_text=""):
    data, rate = sf.read(str(path), dtype="float32")
    info = sf.info(str(path))
    if data.ndim != 1 or not data.size or not np.isfinite(data).all():
        raise ValueError("Expected nonempty, finite mono audio.")
    duration = data.size / rate
    hop = round(rate * .02)
    rms = np.sqrt(np.mean(np.pad(data, (0, (-len(data)) % hop)).reshape(-1, hop) ** 2, axis=1))
    quiet = rms < 10 ** (-45 / 20)
    edges = np.diff(np.r_[False, quiet, False].astype(int))
    runs = [(a * .02, min(b * .02, duration)) for a, b in zip(np.flatnonzero(edges == 1), np.flatnonzero(edges == -1))]
    leading = next((end for start, end in runs if start == 0), 0)
    trailing = next((duration - start for start, end in runs if end == duration), 0)
    internal = [{"start":round(start, 3), "duration":round(end-start, 3)} for start, end in runs
                if start > 0 and end < duration and end-start >= .35]
    return {"duration_seconds":round(duration, 3), "sample_rate":rate, "channels":info.channels,
            "encoding":info.subtype, "frames":info.frames, "peak":float(np.max(np.abs(data))),
            "silence_threshold_dbfs":-45, "analysis_frame_ms":20,
            "leading_low_energy_seconds":round(leading, 3), "trailing_low_energy_seconds":round(trailing, 3),
            "total_low_energy_seconds":round(sum(end-start for start,end in runs), 3),
            "internal_low_energy_gaps_at_least_350ms":internal,
            "intended_words_per_minute":round(len(re.findall(r"\b[\w'-]+\b", intended_text)) / duration * 60, 1)}
