# AI NPC Negotiation System – Project Instructions & Agent Context

Welcome, Agent! This file contains the complete, up-to-date system architecture, design rules, and integration protocols for the **1500s Vijayanagara Empire Marketplace VR Simulation**. 

---

## 🧠 Project Overview

The project is an AI-powered NPC negotiation engine set in the Hampi bazaars of the 1500s. The player acts as a **spice seller** and the NPC acts as a **cautious or aggressive buyer**. 

This is **NOT a chatbot**; it is a highly structured, deterministic bargaining game where LLMs are used *exclusively* for dialogue rephrasing and semantic intent safety nets. All pricing logic, emotional frustration thresholds, and concession math are fully deterministic.

---

## ⚙️ Core Architecture

The system utilizes a clean, modular split between pricing logic, natural language classification, and speech synthesis:

```
[ Player (VR Hand / Voice) ]
          │
          ▼
[ Unity WebSockets Client ] ◄──(Dynamic 3D Audio & Blendshapes)
          │
      (ws://)
          ▼
[ FastAPI / uvicorn Server ]
          │
          ├───► [ Hybrid Intent Classifier ] ──► (PRICE, ACCEPT, QUANTITY, OOW)
          │
          ├───► [ Deterministic Negotiation Engine ] ──► (Bargaining State)
          │
          ├───► [ LLM Dialogue Rephraser ] ──► (Period-Appropriate Subtitles)
          │
          └───► [ Asynchronous Piper TTS Generator ] ──► (Non-blocking WAV compilation)
```

### 1. Persistent WebSocket Channel (`api.py`)
* The core entry point for Unity VR clients is the persistent, full-duplex WebSocket route at `/ws/negotiate/{session_id}`.
* **Low-Latency QoS Delivery**: The server processes steps and returns text dialogues, tones, and expressions *instantly* (within milliseconds) so subtitles render without lag.
* **Asynchronous Voice Compilation**: Speech synthesis runs in a non-blocking background thread pool (`asyncio.to_thread`). Once the lossless `.wav` clip is compiled in `backend/audio/`, the server pushes an `{"type": "audio_ready", "audio_url": "..."}` signal over the socket to stream audio to the VR headset.

### 2. Deterministic Negotiation Engine (`negotiation_engine.py`)
* Manages the state machine (stages: `OPENING` -> `BARGAINING` -> `FINALIZATION`).
* Calculates concession increments based on the buyer's personality (Aggressive Trader, Cautious Buyer, Polite Merchant), patience, and desperation levels.
* **Overshoot Safeguard**: Concession concessions must *always* clamp at or below the seller's active asking price:
  `self.current_offer = min(self.current_offer, self.last_seller_price)`
  preventing logic desyncs where the buyer offers more than the seller asked.

### 3. Natural Language Interpreter & Safety Nets (`intent_classifier.py` & `input_interpreter.py`)
* Extracts player counter-offers and quantity statements.
* **Traditional Weight Safeguard**: Explicitly ignores numbers specified with traditional units (`veesai`, `seer`, `palam`, `manangu`, `bahar`, etc.) inside the price parser, preventing quantity declarations (e.g. `"I have 1 Veesai"`) from being falsely interpreted as a pricing offer of 1 varaha.
* Standardizes all weights internally to **grams**, but displays them dynamically as dual-unit traditional labels (e.g. `1 Veesai (~1.4 kg)`).

---

## 📁 File Structure

Keep files organized inside these precise paths:

```
/backend/
  ├── api.py                    # FastAPI server exposing REST routes & /ws/negotiate WebSocket
  ├── requirements.txt          # Backend package dependencies (fastapi, uvicorn, piper-tts, etc.)
  │
  ├── npc_engine/
  │     ├── interface.py        # NPCSession orchestrator; manages persistence & level loaders
  │     ├── AGENTS.md           # This instructions & context file
  │     │
  │     ├── core/
  │     │     ├── controller.py    # Level-agnostic flow runner & early exit filters
  │     │     ├── measurements.py  # Traditional Vijayanagara measurement snapping & conversions
  │     │     ├── persistence.py   # Disk persistence loader & deal ledger records
  │     │     └── rag.py           # Historical RAG retriever
  │     │
  │     ├── levels/level1_market/
  │     │     ├── negotiation_engine.py  # Core 1200+ line deterministic state machine
  │     │     ├── intent_classifier.py   # Regex & LLM fallback classifier
  │     │     ├── input_interpreter.py   # Semantic price & signal extractor
  │     │     ├── dialogue_generator.py  # LLM dialogue rephraser & template validation
  │     │     ├── buyer_model.py         # NPC personality specs
  │     │     └── item_model.py          # Spice price multipliers
  │     │
  │     ├── piper/              # Precompiled Piper TTS executable & DLLs (Local Offline Windows)
  │     └── models/             # GGUF LLM and Piper ONNX voice models (git-ignored)
  │
/testing/
  ├── system_integration_test.py  # Integration test asserting persistence, RAG, and capping
  ├── websocket_client_test.py    # Simulated automated WebSocket Unity client (Event synchronized)
  └── terminal_test.py            # Local developer CLI sandbox playground
```

---

## 🔒 Strict Engineering Rules (DO NOT BREAK)

1. **Deterministic Logic Only**: The LLM must **never** decide prices, acceptance thresholds, or session terminations. The GGUF model only handles intent classification safety-nets and rephrasing templates.
2. **Monotonic Concessions**: Buyer offers must only increase during the bargaining stage. Cautious concessions must clamp at or below the seller's active asking price.
3. **No Immersion-Breaking Suffixes**: Always output spice weights using traditional dual-unit labels (e.g. `1 Seer (~280g)`) and currency in `varahas`. Never allow modern terms (like `dollars`, `rupees`, `kg`, `decimals` in final deals) to leak into dialogue templates.
4. **Non-Blocking WS Event Loop**: Never execute long-running subprocesses (like `piper.exe`) synchronously within an async WebSocket connection thread. Always delegate CPU-bound tasks to `asyncio.to_thread`.
5. **No Model Files in Git**: Always keep `backend/models/*.gguf` and `backend/models/*.onnx` securely under `.gitignore`.

---

## 🔥 Current Systems Implemented & Verified

* **Duplex WebSocket Endpoint**: Verified with [websocket_client_test.py](file:///g:/Users/chitr/Desktop/Capstone/AI-Enabled-Storytelling-VR/testing/websocket_client_test.py).
* **Asynchronous Offline Piper TTS**: Compiles lossless speech in background threads without blocking WebSocket latency.
* **Deterministic Concession Capping**: Clamps buyer concession calculations.
* **Traditional Snapping & Exclusions**: Quantity parser excludes traditional weights from price extraction.
* **Disk Session Persistence**: Serializes global respect, varahas, and levels dynamically.
* **Historical Fact Sheet RAG**: Context-aware injection from 1500s chronicles.

---

## 🚀 Future Development Scope

If you are picking up work, consider these upcoming modules:
* **Lip-Sync & Visemes Pipeline**: Exposing standard mouth blendshape maps (visemes) inside the WebSocket JSON alongside dialogue text to drive real-time facial mesh lip-sync in Unity.
* **Player Bazaar Reputation Progression**: Expanding persistence so player reputation spreads across customers in subsequent bargaining turns.
* **Caravan & Port Scarcity Events**: Fluctuating base market multipliers based on dynamic bazaar news notifications.