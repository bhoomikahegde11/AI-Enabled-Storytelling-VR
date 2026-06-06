# Onboarding & Setup Guide: AI-Enabled Storytelling VR

Welcome to the **AI-Enabled Storytelling VR** project! This is an interactive historical storytelling simulation set in the Peak Era of the **Vijayanagara Empire (1500s)**. 

This guide details the complete local setup, dependency installations, hand-downloaded model configurations, and developer testing tools.

---

## 📋 Prerequisites
* **Python**: 3.10 to 3.12 (Recommend 3.11)
* **Unity**: 2022.3 LTS or higher (with XR Interaction Toolkit installed)

---

## 🛠️ Step 1: Python Backend Setup & Local Build

1. Open your terminal and navigate to the project `backend/` directory:
   ```bash
   cd backend
   ```
2. Install the backend python dependencies:
   * **Minimum Hardware Option (CPU Fallback Mode)**:
     * **Minimum**: CPU mode is supported but slower.
     * Install using:
       ```bash
       pip install -r requirements.txt
       ```
     * *Note: Works out of the box on any machine. GPU support will be disabled, and AI inference runs on the CPU.*
   * **Recommended Hardware Option (GPU Accelerated Mode - NVIDIA RTX GPU 6GB+ VRAM)**:
     * **Recommended**: NVIDIA RTX GPU 6GB+ VRAM.
     * GPU acceleration requires installing the base requirements first, uninstalling the CPU version of `llama-cpp-python`, and then installing the GPU extras to ensure the precompiled CUDA wheel is correctly installed:
       ```bash
       # 1. Install base requirements
       pip install -r requirements.txt
       
       # 2. Uninstall CPU llama-cpp-python
       pip uninstall llama-cpp-python -y
       
       # 3. Install GPU-accelerated wheels and dependencies
       pip install -r requirements-gpu.txt
       ```
     * *Note: Auto-detects and offloads maximum LLM layers automatically, and accelerates Speech-To-Text processing via CUDA. Cuts response latency down to under 1.5 seconds overall.*

You can force CPU mode at any time for testing by setting `USE_GPU=false` in your `.env` file.

### 🔍 GPU Execution Verification
To verify that GPU acceleration is active and actually running on the hardware:
1. Open a command prompt or terminal window.
2. Run the NVIDIA system monitor tool:
   ```bash
   nvidia-smi
   ```
3. During active AI inference (e.g. while asking the NPC a question or transcribing voice), you should observe:
   * A `python.exe` process appearing in the Processes section of the output.
   * VRAM usage increasing on your GPU (typically 4.8 GB - 5.5 GB allocated).

---

## 🧠 Step 2: Hand-Downloaded Model Placements (CRITICAL)

To protect repository storage limits, the large GGUF model files are **not** committed to GitHub. You must download and place them manually for full semantic capabilities:

### A. Intent Classification LLM (GGUF Model)
1. Download a lightweight Llama-3 or Llama-2 instruction-tuned GGUF model.
   * **Recommended Model**: `Llama-3-8B-Instruct-Q4_K_M.gguf` (approx. 4.8 GB)
   * **Download Source**: [Hugging Face Llama-3-8B-Instruct-GGUF (Meta-Llama-3-8B-Instruct.Q4_K_M.gguf)](https://huggingface.co/MaziyarPanahi/Meta-Llama-3-8B-Instruct-GGUF/tree/main?show_file_info=Meta-Llama-3-8B-Instruct.Q4_K_M.gguf)
2. Place the downloaded `.gguf` file inside the `backend/models/` directory.
3. **Important**: Rename the file exactly to:
   ```
   backend/models/model.gguf
   ```

*Fallback Warning:* If `model.gguf` is missing, the backend will still boot successfully without crashing, but will fall back entirely to **regex heuristics** instead of utilizing local LLM classification prompts.

### B. Text-to-Speech Engine (Piper ONNX Models)
The offline Piper voice assets are already located in the workspace under the following structure:
* **Engine DLLs & Executable**: `backend/piper/`
* **Voice Model File**: `backend/models/en_US-lessac-medium.onnx`
* **Voice Configuration Json**: `backend/models/en_US-lessac-medium.onnx.json`

If you ever wish to expand or use different voices, you can download alternative voices from the [Rhasspy Piper Voices Registry](https://huggingface.co/rhasspy/piper-voices) and replace the files inside `backend/models/`.

---

## ⚙️ Step 3: Local Configuration (`.env`)

Create a `.env` file under `backend/` to configure the system routing:

```env
# Speech Synthesizer Toggle ("piper", "openai", "elevenlabs")
# Defaulting to "piper" enables 100% offline local development without API keys!
TTS_PROVIDER=piper

# (Optional) Online cloud credentials for ElevenLabs or OpenAI
OPENAI_API_KEY=your-openai-api-key
ELEVENLABS_API_KEY=your-elevenlabs-api-key
VOICE_ID=yCxjZ3dvaYYrkVmdHAe9
```

---

## 🎯 Step 4: Running & Testing the Backend

You can test the system in two ways: via a **local terminal sandbox** (extremely fast for debugging) or by launching the **API server** to connect to Unity.

### Method A: Local Terminal Sandbox (Highly Recommended)
We have built a dedicated **terminal test bed** inside `testing/`. This tool loads the full negotiation logic and intent classifier offline, prints rich emotional debug dashboards, and lets you interact directly via CLI:
```bash
# Run from the root repository directory
python testing/terminal_test.py
```
* **Why use it?**: It runs entirely local and offline, requires zero API keys, completely bypasses slow audio generations, and prints a direct real-time readout of the buyer's internal trust, frustration, and concessions!

### Method B: Launching the API Server (Unity Connection)
To connect the backend to your Unity VR client:
1. Start the FastAPI server using Uvicorn:
   ```bash
   cd backend
   uvicorn api:app --reload
   ```
2. The server will run at: `http://127.0.0.1:8000`.
3. Check the Swagger API documentation in your browser to verify it is running:
   `http://127.0.0.1:8000/docs`

## 🧠 AI Architecture

The marketplace NPC negotiation and dialogue system is structured as a tiered, modular pipeline:

1. **Intent Classifier**: Parses the player's spoken or typed input (from the Speech-To-Text module) and classifies the input intent (e.g., `PRICE`, `QUERY_QUANTITY`, `SOCIAL`, `OUT_OF_WORLD`, `CLARIFICATION`). It uses deterministic keyword overrides for high confidence and speed, falling back to a semantic GGUF model check only when ambiguous.
2. **Negotiation Engine**: The core business logic and economic state machine that controls the transaction. It dynamically tracks buyer frustration, concessions, price offers, and reputation updates. It remains the final, deterministic authority on the game state (the LLM cannot hallucinate or accept deals on its own).
3. **Local LLM**: A GGUF-based Llama model that rephrases the deterministic templates generated by the Negotiation Engine. It injects the NPC's persistent personality (e.g., strict, friendly, impatient, curious traveler) and origin context dynamically.
4. **Validator**: A safety layer that screens the LLM's rephrased output. It validates that the traded spice is correct, preserves all numbers (price, quantity), rejects modern immersion-breaking terms (e.g., computer, cryptocurrency), and blocks invalid future purchase language.
5. **TTS (Text-To-Speech)**: Converts the validated character response into spoken audio using local offline Piper TTS.

---

## 🏛️ Repository Architecture
For a detailed developer reference on how to add **Level 2 (Craftsmanship)** or **Level 3 (Royal Court)** to the storytelling framework, please read the **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** file!
