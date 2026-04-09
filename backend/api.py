from fastapi import FastAPI
from pydantic import BaseModel
from npc_engine.interface import NPCSession
import uuid

app = FastAPI()

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
        "npc_text": response["npc_text"],
        "action": response["action"],
        "price": response["price"],
        "quantity": response["quantity"],
        "done": response["done"],
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
        "npc_text": response["npc_text"],
        "action": response["action"],
        "price": response["price"],
        "quantity": response["quantity"],
        "done": response["done"],
        "response": response
    }


# ❤️ HEALTH CHECK (optional but useful)
@app.get("/")
def health():
    return {"status": "NPC Engine API running"}
