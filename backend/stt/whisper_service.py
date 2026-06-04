import os
from faster_whisper import WhisperModel

# Load the model once on module import
model_size = "base.en"
print(f"[STT] Loading faster-whisper model: {model_size} on CPU...")
model = WhisperModel(model_size, device="cpu", compute_type="int8")
print("[STT] Whisper model loaded successfully.")

def transcribe_audio_file(file_path: str) -> str:
    """
    Transcribes a WAV file using the pre-loaded faster-whisper model.
    """
    if not os.path.exists(file_path):
        print(f"[STT] Audio file not found: {file_path}")
        return ""
    
    # Transcribe the audio
    segments, info = model.transcribe(file_path, beam_size=5)
    
    # Reconstruct transcript from segments
    text = " ".join([segment.text for segment in segments]).strip()
    return text
