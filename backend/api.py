from fastapi import FastAPI, WebSocket, WebSocketDisconnect, BackgroundTasks, UploadFile, File
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from npc_engine.interface import NPCSession
from stt.whisper_service import transcribe_audio_file
from npc_engine.utils.text_normalizer import normalize_text, normalize_trade_numbers
from dotenv import load_dotenv
from openai import OpenAI
import requests
import uuid
import os
import asyncio

load_dotenv()

DEBUG_PERFORMANCE = True

app = FastAPI()

# Create audio folder inside the backend directory
os.makedirs("audio", exist_ok=True)

# Mount the audio directory so Unity can access the files directly
app.mount("/audio", StaticFiles(directory="audio"), name="audio")

# Connection Manager for Unity VR WebSocket clients
class ConnectionManager:
    def __init__(self):
        self.active_connections = {}

    async def connect(self, session_id: str, websocket: WebSocket):
        await websocket.accept()
        self.active_connections[session_id] = websocket

    def disconnect(self, session_id: str):
        if session_id in self.active_connections:
            del self.active_connections[session_id]

    async def send_personal_message(self, message: dict, session_id: str):
        websocket = self.active_connections.get(session_id)
        if websocket:
            await websocket.send_json(message)

manager = ConnectionManager()


# ----------------- TTS CONFIGURATION -----------------

def get_tts_provider():
    return os.getenv("TTS_PROVIDER", "piper").lower()

def generate_piper_audio(text: str) -> str:
    base_dir = os.path.dirname(os.path.abspath(__file__))
    piper_exe = os.path.join(base_dir, "piper", "piper.exe")
    model_path = os.path.join(base_dir, "models", "en_US-lessac-medium.onnx")

    if not os.path.exists(piper_exe):
        print(f"[ERROR Piper] Executable not found at: {piper_exe}")
        return ""
    if not os.path.exists(model_path):
        print(f"[ERROR Piper] Model file not found at: {model_path}")
        return ""

    filename = f"{uuid.uuid4()}.wav"
    filepath = os.path.join(base_dir, "audio", filename)

    command = [
        piper_exe,
        "--model", model_path,
        "--output_file", filepath
    ]

    try:
        import subprocess
        print(f"[INFO Piper] Generating audio offline for: \"{text}\"")
        process = subprocess.Popen(
            command,
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            encoding="utf-8"
        )
        stdout, stderr = process.communicate(input=text)
        
        if process.returncode == 0 and os.path.exists(filepath):
            return f"http://127.0.0.1:8000/audio/{filename}"
        else:
            print(f"[ERROR Piper] Process exited with code {process.returncode}. Stderr: {stderr}")
    except Exception as e:
        print(f"[ERROR Piper] Execution failed: {e}")

    return ""

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

def clean_text_for_speech(text: str) -> str:
    import re
    # 1. Remove parenthesized measurements e.g., " (~1.4 kg)" or "(~28g)" or "(~113.2 g)"
    text = re.sub(r'\s*\([^)]*(?:kg|g|gram|grams|veesai|palam)[^)]*\)', '', text, flags=re.IGNORECASE)
    
    # 2. Remove ~ symbols and other technical symbols
    text = text.replace('~', '')
    
    # 3. Specifically convert "1 Veesai" (case-insensitive) to "one veesai"
    text = re.sub(r'\b1\s+veesai\b', 'one veesai', text, flags=re.IGNORECASE)
    text = re.sub(r'\b1\s+veesais\b', 'one veesai', text, flags=re.IGNORECASE)
    
    # Clean up double/multiple spaces
    text = re.sub(r'\s+', ' ', text).strip()
    return text


def generate_audio_url(text: str) -> str:
    if not text:
        return ""

    cleaned_text = clean_text_for_speech(text)
    provider = get_tts_provider()
    print(f"[INFO] Using provider: {provider}")

    if provider == "piper":
        url = generate_piper_audio(cleaned_text)
        if url: return url
        print("[WARNING] Piper failed, falling back to OpenAI...")
        return generate_openai_audio(cleaned_text)

    elif provider == "elevenlabs":
        url = generate_elevenlabs_audio(cleaned_text)
        if url: return url
        print("[WARNING] ElevenLabs failed, falling back to OpenAI...")
        return generate_openai_audio(cleaned_text)

    elif provider == "openai":
        url = generate_openai_audio(cleaned_text)
        if url: return url
        print("[WARNING] OpenAI failed, falling back to ElevenLabs...")
        return generate_elevenlabs_audio(cleaned_text)

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

    from npc_engine.core.market_events import get_random_market_event
    active_event = get_random_market_event()
    session = NPCSession(session_id=session_id, active_event=active_event)
    sessions[session_id] = session

    response = session.start()

    from npc_engine.core.persistence import load_session, DEFAULT_REPUTATION, DEFAULT_VARAHAS
    state = load_session(session_id)
    reputation = state.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION)
    total_varahas = state.get("global_metrics", {}).get("total_varahas", DEFAULT_VARAHAS)

    print(f"[REP BACKEND] {reputation}")

    from npc_engine.core.measurements import grams_to_traditional_label
    spice_name = session.item.name
    spice_qty = grams_to_traditional_label(session.item.quantity * 1000.0)

    return {
        "session_id": session_id,
        "npc_text": response.get("npc_text", ""),
        "action": response.get("action", ""),
        "price": response.get("price"),
        "quantity": response.get("quantity"),
        "done": response.get("done", False),
        "audio_url": generate_audio_url(response.get("npc_text", "")),
        "active_event": active_event,
        "reputation": reputation,
        "total_varahas": total_varahas,
        "reputation_delta": 0,
        "buyer_trust": round(session.engine.trust, 4),
        "buyer_frustration": round(session.engine.frustration, 4),
        "out_of_world_count": session.engine.out_of_world_count,
        
        # New HUD and identity keys
        "player_reputation": reputation,
        "player_money": total_varahas,
        "buyer_name": getattr(session.buyer, "name", "Abdul Rahman"),
        "buyer_origin": getattr(session.buyer, "origin", "Persian Trader"),
        "spice_name": spice_name.capitalize(),
        "spice_quantity": spice_qty,
        "current_trade": {
            "spice": spice_name.capitalize(),
            "quantity": spice_qty,
            "npc_offer": int(session.engine.current_offer),
            "market_value": int(round(session.engine.market_price))
        },
        
        "response": response
    }


# 🔁 CONTINUE SESSION
@app.post("/step")
def step_session(req: StepRequest):
    import time
    start_step = time.time()
    session_id = req.session_id

    if session_id not in sessions:
        return {"error": "Invalid session_id"}

    session = sessions[session_id]

    from npc_engine.core.persistence import load_session, DEFAULT_REPUTATION, DEFAULT_VARAHAS
    old_state = load_session(session_id)
    rep_before = old_state.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION)

    response = session.step(req.player_input)

    state = load_session(session_id)
    reputation = state.get("global_metrics", {}).get("reputation", DEFAULT_REPUTATION)
    total_varahas = state.get("global_metrics", {}).get("total_varahas", DEFAULT_VARAHAS)
    reputation_delta = reputation - rep_before

    print(f"[REP BACKEND] {reputation} (delta: {reputation_delta})")

    start_tts = time.time()
    audio_url = generate_audio_url(response.get("npc_text", ""))
    tts_duration_ms = int((time.time() - start_tts) * 1000)

    from npc_engine.core.measurements import grams_to_traditional_label
    spice_name = session.item.name
    spice_qty = grams_to_traditional_label(session.item.quantity * 1000.0)

    total_duration_ms = int((time.time() - start_step) * 1000)
    perf_intent = response.get("perf_intent", 0)
    perf_llm = response.get("perf_llm", 0)

    if DEBUG_PERFORMANCE:
        print(f"\n[PERF]")
        print(f"Intent: {perf_intent} ms")
        print(f"LLM: {perf_llm} ms")
        print(f"TTS: {tts_duration_ms} ms")
        print(f"Total: {total_duration_ms} ms\n")

    return {
        "session_id": session_id,
        "npc_text": response.get("npc_text", ""),
        "action": response.get("action", ""),
        "price": response.get("price"),
        "quantity": response.get("quantity"),
        "done": response.get("done", False),
        "audio_url": audio_url,
        "reputation": reputation,
        "total_varahas": total_varahas,
        "transaction": response.get("transaction"),
        "reputation_delta": reputation_delta,
        "buyer_trust": round(session.engine.trust, 4),
        "buyer_frustration": round(session.engine.frustration, 4),
        "out_of_world_count": session.engine.out_of_world_count,
        "active_event": session.active_event,
        
        # New HUD and identity keys
        "player_reputation": reputation,
        "player_money": total_varahas,
        "buyer_name": getattr(session.buyer, "name", "Abdul Rahman"),
        "buyer_origin": getattr(session.buyer, "origin", "Persian Trader"),
        "spice_name": spice_name.capitalize(),
        "spice_quantity": spice_qty,
        "current_trade": {
            "spice": spice_name.capitalize(),
            "quantity": spice_qty,
            "npc_offer": int(session.engine.current_offer),
            "market_value": int(round(session.engine.market_price))
        },
        
        "response": response
    }


# Asynchronous Background Audio Compiler & Dispatcher
async def generate_and_send_audio(session_id: str, npc_text: str, perf_intent: int = 0, perf_llm: int = 0, start_step: float = 0):
    if not npc_text:
        return
    try:
        import time
        start_tts = time.time()
        # Run Piper TTS generation in a non-blocking background thread
        audio_url = await asyncio.to_thread(generate_audio_url, npc_text)
        tts_duration_ms = int((time.time() - start_tts) * 1000)

        if DEBUG_PERFORMANCE and start_step > 0:
            total_duration_ms = int((time.time() - start_step) * 1000)
            print(f"\n[PERF]")
            print(f"Intent: {perf_intent} ms")
            print(f"LLM: {perf_llm} ms")
            print(f"TTS: {tts_duration_ms} ms")
            print(f"Total: {total_duration_ms} ms\n")

        if audio_url:
            await manager.send_personal_message({
                "type": "audio_ready",
                "session_id": session_id,
                "audio_url": audio_url
            }, session_id)
    except Exception as e:
        print(f"[ERROR WebSocket] Background audio generation failed for session {session_id}: {e}")


# 🔌 HIGH-QOS WEB-SOCKET ENDPOINT FOR UNITY VR CLIENTS
@app.websocket("/ws/negotiate/{session_id}")
async def websocket_negotiation(websocket: WebSocket, session_id: str):
    await manager.connect(session_id, websocket)
    print(f"[INFO WebSocket] VR Client connected: session_id={session_id}")

    try:
        # Load or start session
        if session_id not in sessions:
            from npc_engine.core.market_events import get_random_market_event
            active_event = get_random_market_event()
            sessions[session_id] = NPCSession(session_id=session_id, active_event=active_event)
        session = sessions[session_id]

        # Trigger welcome step if engine has not started
        if not session.engine.started:
            import time
            start_step = time.time()
            response = session.start()
            npc_text = response.get("npc_text", "")
            
            from npc_engine.core.persistence import load_session
            state = load_session(session_id)
            reputation = state.get("global_metrics", {}).get("reputation", 50)
            print(f"[REP BACKEND] {reputation}")

            # Send immediate subtitle/text response
            await websocket.send_json({
                "type": "welcome",
                "session_id": session_id,
                "npc_text": npc_text,
                "action": response.get("action", ""),
                "price": response.get("price"),
                "quantity": response.get("quantity"),
                "done": response.get("done", False),
                "tone": response.get("tone", "neutral"),
                "emotion": response.get("emotion", "idle"),
                "active_event": session.active_event
            })

            # Synthesize voice asynchronously in background thread
            if npc_text:
                asyncio.create_task(generate_and_send_audio(
                    session_id, 
                    npc_text,
                    perf_intent=response.get("perf_intent", 0),
                    perf_llm=response.get("perf_llm", 0),
                    start_step=start_step
                ))

        # Main interactive duplex communication loop
        while True:
            data = await websocket.receive_json()
            player_input = data.get("player_input", "").strip()

            import time
            start_step = time.time()
            response = session.step(player_input)
            npc_text = response.get("npc_text", "")

            from npc_engine.core.persistence import load_session
            state = load_session(session_id)
            reputation = state.get("global_metrics", {}).get("reputation", 50)
            print(f"[REP BACKEND] {reputation}")

            # Send immediate subtitle response (extremely low latency)
            await websocket.send_json({
                "type": "text_response",
                "session_id": session_id,
                "npc_text": npc_text,
                "action": response.get("action", ""),
                "price": response.get("price"),
                "quantity": response.get("quantity"),
                "done": response.get("done", False),
                "tone": response.get("tone", "neutral"),
                "emotion": response.get("emotion", "idle")
            })

            # Synthesize voice asynchronously in background thread
            if npc_text:
                asyncio.create_task(generate_and_send_audio(
                    session_id, 
                    npc_text,
                    perf_intent=response.get("perf_intent", 0),
                    perf_llm=response.get("perf_llm", 0),
                    start_step=start_step
                ))

            if response.get("done", False):
                break

    except WebSocketDisconnect:
        print(f"[INFO WebSocket] VR Client disconnected: session_id={session_id}")
    except Exception as e:
        print(f"[ERROR WebSocket] Connection error in session {session_id}: {e}")
        try:
            await websocket.send_json({"type": "error", "message": str(e)})
        except:
            pass
    finally:
        manager.disconnect(session_id)


# 🎙️ OFFLINE WHISPER SPEECH TO TEXT ENDPOINT
@app.post("/stt")
async def speech_to_text(file: UploadFile = File(...), session_id: str = None):
    import time
    import shutil

    temp_filename = f"temp_{uuid.uuid4()}.wav"
    temp_filepath = os.path.join("audio", temp_filename)
    
    start_time = time.time()
    try:
        with open(temp_filepath, "wb") as buffer:
            shutil.copyfileobj(file.file, buffer)
            
        print(f"[STT] Transcribing received file: {file.filename}")
        transcript = transcribe_audio_file(temp_filepath)
        
        # Get active session's engine to enable context-aware number normalization
        active_engine = None
        if session_id and session_id in sessions:
            active_engine = sessions[session_id].engine
        elif sessions:
            active_engine = list(sessions.values())[0].engine

        normalized_transcript = normalize_text(transcript)
        from npc_engine.utils.text_normalizer import normalize_currency_tokens
        normalized_transcript = normalize_currency_tokens(normalized_transcript)
        normalized_transcript = normalize_trade_numbers(normalized_transcript, active_engine)
        
        elapsed_ms = int((time.time() - start_time) * 1000)
        print(f"[STT RAW]: {transcript}")
        print(f"[STT NORMALIZED]: {normalized_transcript}")
        print(f"[PERF STT] Inference: {elapsed_ms} ms")
        
        return {"text": normalized_transcript}
    except Exception as e:
        print(f"[STT] Transcription error: {e}")
        return {"text": "", "error": str(e)}
    finally:
        if os.path.exists(temp_filepath):
            os.remove(temp_filepath)


# ❤️ HEALTH CHECK
@app.get("/")
def health():
    return {"status": "NPC Engine API running"}
