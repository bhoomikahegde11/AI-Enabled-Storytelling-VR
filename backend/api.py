from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from npc_engine.interface import NPCSession
from dotenv import load_dotenv
from openai import OpenAI
import requests
import uuid
import os

load_dotenv()

app = FastAPI()

# Create audio folder inside the backend directory
os.makedirs("audio", exist_ok=True)

# Mount the audio directory so Unity can access the files directly
app.mount("/audio", StaticFiles(directory="audio"), name="audio")


# ----------------- TTS CONFIGURATION -----------------

def get_tts_provider():
    return os.getenv("TTS_PROVIDER", "openai").lower()

def generate_elevenlabs_audio(text: str) -> str:
    api_key = os.getenv("ELEVENLABS_API_KEY")
    VOICE_ID = os.getenv("VOICE_ID", "yCxjZ3dvaYYrkVmdHAe9")

    if not api_key:
        print("[ERROR ElevenLabs] ELEVENLABS_API_KEY not set.")
        return ""

    filename = f"{uuid.uuid4()}.mp3"
    filepath = os.path.join("audio", filename)

    url = f"https://api.elevenlabs.io/v1/text-to-speech/{VOICE_ID}"
    headers = {
        "xi-api-key": api_key,
        "Content-Type": "application/json"
    }
    data = {
        "text": text,
        "model_id": "eleven_turbo_v2",
        "voice_settings": {
            "stability": 0.35,
            "similarity_boost": 0.85,
            "style": 0.6,
            "use_speaker_boost": True
        }
    }

    try:
        response = requests.post(url, json=data, headers=headers)
        if response.status_code == 200:
            with open(filepath, "wb") as f:
                f.write(response.content)
            return f"http://127.0.0.1:8000/audio/{filename}"
        else:
            print(f"[ERROR ElevenLabs] API error: {response.text}")
    except Exception as e:
        print(f"[ERROR ElevenLabs] request failed: {e}")
    
    return ""

def generate_openai_audio(text: str) -> str:
    api_key = os.getenv("OPENAI_API_KEY")
    if not api_key:
        print("[ERROR OpenAI] API key missing")
        return ""
    
    try:
        client = OpenAI(api_key=api_key)
    except Exception as e:
        print("[ERROR OpenAI] Client init failed:", e)
        return ""
        
    filename = f"{uuid.uuid4()}.mp3"
    filepath = os.path.join("audio", filename)
    
    try:
        response = client.audio.speech.create(
            model="gpt-4o-mini-tts",  # Note: Standard OpenAI TTS relies on models 'tts-1' or 'tts-1-hd'. But we use exact request here.
            voice="alloy",
            input=text
        )
        
        # In modern versions of the OpenAI python SDK
        response.stream_to_file(filepath)
        return f"http://127.0.0.1:8000/audio/{filename}"
    except Exception as e:
        # Fallback for SDK version differences
        try:
            if hasattr(response, 'content'):
                with open(filepath, "wb") as f:
                    f.write(response.content)
                return f"http://127.0.0.1:8000/audio/{filename}"
        except Exception as inner_e:
            pass
        
        print(f"[ERROR OpenAI] request failed: {e}")
        return ""

def generate_audio_url(text: str) -> str:
    if not text:
        return ""

    provider = get_tts_provider()
    print(f"[INFO] Using provider: {provider}")

    if provider == "elevenlabs":
        url = generate_elevenlabs_audio(text)
        if url: return url
        print("[WARNING] ElevenLabs failed, falling back to OpenAI...")
        return generate_openai_audio(text)

    elif provider == "openai":
        url = generate_openai_audio(text)
        if url: return url
        print("[WARNING] OpenAI failed, falling back to ElevenLabs...")
        return generate_elevenlabs_audio(text)

    return ""


# 🔥 Session storage (in-memory for now)
sessions = {}


# 📦 Request models
class StartRequest(BaseModel):
    pass


class StepRequest(BaseModel):
    session_id: str
    player_input: str


# 🚀 START NEW SESSION
@app.post("/start")
def start_session():
    session_id = str(uuid.uuid4())

    session = NPCSession()
    sessions[session_id] = session

    response = session.start()

    return {
        "session_id": session_id,
        "npc_text": response.get("npc_text", ""),
        "action": response.get("action", ""),
        "price": response.get("price"),
        "quantity": response.get("quantity"),
        "done": response.get("done", False),
        "audio_url": generate_audio_url(response.get("npc_text", "")),
        "response": response
    }


# 🔁 CONTINUE SESSION
@app.post("/step")
def step_session(req: StepRequest):
    session_id = req.session_id

    if session_id not in sessions:
        return {"error": "Invalid session_id"}

    session = sessions[session_id]

    response = session.step(req.player_input)

    return {
        "session_id": session_id,
        "npc_text": response.get("npc_text", ""),
        "action": response.get("action", ""),
        "price": response.get("price"),
        "quantity": response.get("quantity"),
        "done": response.get("done", False),
        "audio_url": generate_audio_url(response.get("npc_text", "")),
        "response": response
    }


# ❤️ HEALTH CHECK (optional but useful)
@app.get("/")
def health():
    return {"status": "NPC Engine API running"}
