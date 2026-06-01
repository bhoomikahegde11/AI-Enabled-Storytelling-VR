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
   ```bash
   pip install -r requirements.txt
   ```

*Note on llama-cpp-python:* If you are using a GPU (NVIDIA CUDA or Apple Metal) for accelerated local LLM processing, please install `llama-cpp-python` with the appropriate compiler flags:
* **NVIDIA CUDA**:
  ```bash
  $env:CMAKE_ARGS="-GGuid Visual Studio 17 2022 -A x64 -DLLAMA_CUDA=on"
  pip install llama-cpp-python --force-reinstall --upgrade --no-cache-dir
  ```
* **Apple Silicon**:
  ```bash
  CMAKE_ARGS="-DLLAMA_METAL=on" pip install llama-cpp-python --force-reinstall --upgrade --no-cache-dir
  ```

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

---

## 🏛️ Repository Architecture
For a detailed developer reference on how to add **Level 2 (Craftsmanship)** or **Level 3 (Royal Court)** to the storytelling framework, please read the **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** file!
