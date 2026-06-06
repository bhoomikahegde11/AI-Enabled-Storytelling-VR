import os
import sys
import re
import wave
import numpy as np
from dotenv import load_dotenv

# Compute the correct absolute paths relative to backend directory
BACKEND_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
if BACKEND_DIR not in sys.path:
    sys.path.append(BACKEND_DIR)

load_dotenv(os.path.join(BACKEND_DIR, ".env"))

from npc_engine.utils import hardware
from faster_whisper import WhisperModel

# Expose local alias for backwards compatibility
CUDA_AVAILABLE = hardware.CUDA_AVAILABLE

# Configure and load Whisper model
whisper_model_env = os.getenv("WHISPER_MODEL", "small.en")

model = None
device = "cpu"
compute_type = "int8"
loaded_model_name = "base.en"

try:
    if hardware.CUDA_AVAILABLE:
        print(f"[STT] Loading faster-whisper model: {whisper_model_env} on CUDA...")
        model = WhisperModel(whisper_model_env, device="cuda", compute_type="float16")
        hardware.WHISPER_DEVICE = "CUDA"
        device = "CUDA"
        compute_type = "float16"
        loaded_model_name = whisper_model_env
    else:
        raise ValueError("CUDA not available/enabled in hardware config")
except Exception as e:
    print(f"[WARNING STT] Loading Whisper on CUDA failed ({e}). Falling back to CPU...")
    model = WhisperModel("base.en", device="cpu", compute_type="int8")
    hardware.WHISPER_DEVICE = "CPU"
    device = "CPU"
    compute_type = "int8"
    loaded_model_name = "base.en"

# Print STT CONFIG log block as requested
print("\n" + "="*30)
print("[STT CONFIG]")
print(f"Model: {loaded_model_name}")
print(f"Device: {device}")
print(f"Compute: {compute_type}")
print("Beam: 5")
print("="*30 + "\n")

print("[STT] Whisper model loaded successfully.")

# Print device status report AFTER both LLM and Whisper load
print("\n" + "="*50)
print("[INFO DEVICE]")
print(f"CUDA Support: {hardware.CUDA_AVAILABLE}")
print(f"LLM: {hardware.LLM_DEVICE}")
print(f"Whisper: {hardware.WHISPER_DEVICE}")

# Expose backend status for demo debugging
llm_backend = "CUDA" if "GPU" in hardware.LLM_DEVICE else "CPU"
whisper_backend = "CUDA" if hardware.WHISPER_DEVICE == "CUDA" else "CPU"

print("\n[PERF DEVICE]")
print(f"LLM Backend: {llm_backend}")
print(f"Whisper Backend: {whisper_backend}")
print("="*50 + "\n")

# Broad context prompt to preserve player freedom (bargaining + history + random queries)
initial_prompt = """
The speaker is having a natural conversation
with a character inside a historical
Vijayanagara Empire marketplace simulation.

The conversation may include:
- bargaining
- prices
- quantities
- spices
- travel
- weather
- history
- kings and rulers
- personal questions
- random questions

Common terms:
Vijayanagara
Hampi
merchant
trade
varahas
pepper
cardamom
clove
cinnamon
spices
gold
king
empire
journey

Transcribe exactly what the speaker says.
Do not replace unrelated speech with marketplace terms.
"""

def get_audio_diagnostics(file_path: str):
    """
    Computes duration, peak amplitude, and RMS value of a WAV file.
    """
    try:
        with wave.open(file_path, "rb") as wf:
            n_channels = wf.getnchannels()
            sampwidth = wf.getsampwidth()
            framerate = wf.getframerate()
            n_frames = wf.getnframes()
            
            if framerate == 0 or n_frames == 0:
                return 0.0, 0.0, 0.0
                
            duration = n_frames / float(framerate)
            raw_data = wf.readframes(n_frames)
            
            if sampwidth == 1:
                dtype = np.uint8
            elif sampwidth == 2:
                dtype = np.int16
            elif sampwidth == 4:
                dtype = np.int32
            else:
                return duration, 0.0, 0.0
                
            audio_data = np.frombuffer(raw_data, dtype=dtype)
            if len(audio_data) == 0:
                return duration, 0.0, 0.0
                
            # If multi-channel, convert to mono/mean
            if n_channels > 1:
                audio_data = audio_data.reshape(-1, n_channels).mean(axis=1)
                
            # Normalize to [-1.0, 1.0] for analysis
            if dtype == np.uint8:
                normalized = (audio_data.astype(np.float32) - 128.0) / 128.0
            elif dtype == np.int16:
                normalized = audio_data.astype(np.float32) / 32768.0
            elif dtype == np.int32:
                normalized = audio_data.astype(np.float32) / 2147483648.0
            else:
                normalized = audio_data.astype(np.float32)
                
            peak = float(np.max(np.abs(normalized)))
            rms = float(np.sqrt(np.mean(normalized ** 2)))
            
            return duration, peak, rms
    except Exception as e:
        print(f"[WARNING STT] Could not compute audio diagnostics: {e}")
        return 0.0, 0.0, 0.0

def transcribe_audio_file(file_path: str) -> str:
    """
    Transcribes a WAV file using the pre-loaded faster-whisper model.
    """
    if not os.path.exists(file_path):
        print(f"[STT] Audio file not found: {file_path}")
        return ""
    
    # Audio diagnostics & logging
    duration, peak, rms = get_audio_diagnostics(file_path)
    print("\n[VOICE INPUT]")
    print(f"Duration: {duration:.2f}s")
    print(f"Peak: {peak:.4f}")
    print(f"RMS: {rms:.4f}")
    
    if duration < 0.5:
        print(f"[WARNING STT] Audio duration is very short ({duration:.2f}s). Transcription accuracy may be reduced.")
    if rms < 0.005:
        print(f"[WARNING STT] Audio is extremely quiet (RMS: {rms:.4f}). Transcription accuracy may be reduced.")
    
    # Transcribe audio with language, task, beam search, VAD and custom prompt options
    segments, info = model.transcribe(
        file_path,
        language="en",
        task="transcribe",
        beam_size=5,
        best_of=5,
        temperature=0.0,
        initial_prompt=initial_prompt,
        vad_filter=True,
        vad_parameters={
            "min_silence_duration_ms": 500,
            "speech_pad_ms": 300
        }
    )
    
    # Reconstruct transcript with confidence segment check and blacklist filtering
    blacklist = [
        "thanks for watching",
        "thank you for being here",
        "subscribe",
        "welcome welcome",
        "subtitles"
    ]
    
    valid_texts = []
    for segment in segments:
        text_clean = segment.text.strip()
        if not text_clean:
            continue
            
        # Confidence filtering: Reject segment only if no_speech_prob > 0.65 AND avg_logprob < -1.0
        if segment.no_speech_prob > 0.65 and segment.avg_logprob < -1.0:
            print(f"[STT] Rejecting segment due to low confidence: '{text_clean}' (no_speech_prob={segment.no_speech_prob:.3f}, avg_logprob={segment.avg_logprob:.3f})")
            continue
            
        # Hallucination blacklist filtering (case-insensitive) under low-confidence conditions
        text_lower = text_clean.lower()
        is_hallucination = False
        for phrase in blacklist:
            if phrase in text_lower:
                # low confidence condition: no_speech_prob > 0.35 or avg_logprob < -0.5
                if segment.no_speech_prob > 0.35 or segment.avg_logprob < -0.5:
                    is_hallucination = True
                    print(f"[STT] Rejecting blacklisted hallucination: '{text_clean}' (matched '{phrase}', no_speech_prob={segment.no_speech_prob:.3f}, avg_logprob={segment.avg_logprob:.3f})")
                    break
        
        if is_hallucination:
            continue
            
        valid_texts.append(text_clean)
        
    text = " ".join(valid_texts).strip()
    return clean_transcript(text)

def clean_transcript(text: str) -> str:
    """
    Collapses repeated sentences and removes common silent Whisper hallucinations.
    """
    if not text:
        return ""
    
    # Split text into sentences using basic sentence boundaries, preserving trailing punctuations
    sentences = re.split(r'(?<=[.!?])\s+', text.strip())
    
    cleaned_sentences = []
    for s in sentences:
        s_stripped = s.strip()
        if not s_stripped:
            continue
        
        # Check for duplication (case-insensitive, ignoring punctuation)
        s_norm = re.sub(r'[^\w\s]', '', s_stripped.lower()).strip()
        if not s_norm:
            continue
            
        if cleaned_sentences:
            last_norm = re.sub(r'[^\w\s]', '', cleaned_sentences[-1].lower()).strip()
            if s_norm == last_norm:
                # Skip duplicate adjacent sentence
                continue
                
        cleaned_sentences.append(s_stripped)
        
    # Remove Whisper hallucination endings from final sentence
    hallucination_endings = [
        "thanks for watching",
        "thank you for being here",
        "subscribe",
        "welcome welcome",
        "subtitles"
    ]
    
    if len(cleaned_sentences) > 1:
        last_s_lower = cleaned_sentences[-1].lower()
        for ending in hallucination_endings:
            # If the ending is a substring and the sentence is short (e.g. contains almost only the hallucination)
            if ending in last_s_lower and len(last_s_lower) < len(ending) + 10:
                cleaned_sentences.pop()
                break
                
    return " ".join(cleaned_sentences).strip()


