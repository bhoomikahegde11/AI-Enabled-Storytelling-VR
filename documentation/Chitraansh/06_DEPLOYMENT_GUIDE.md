# Deployment and Setup Guide

This document details the configuration steps, dependency requirements, and platform setup instructions needed to build and run the AI NPC Marketplace backend on local development machines or staging servers.

---

## 🛠️ Requirements & Prerequisites

- **Python**: 3.10 to 3.12 (Recommend 3.11)
- **CUDA Support**: NVIDIA GPU with 6GB+ VRAM, CUDA Toolkit 12.x, and cuBLAS DLL dependencies.
- **Dependencies**: All packages listed in `requirements.txt` and `requirements-gpu.txt`.

---

## 🚀 Installation Stages

### 1. Repository Setup & Virtual Environment
Open a terminal and clone the repository:
```powershell
# Navigate to the backend directory
cd backend

# Create a virtual environment
python -m venv venv
venv\Scripts\activate
```

### 2. Dependency Setup Modes

#### Option A: CPU Fallback Mode (Local Testing)
Install base packages without GPU requirements:
```powershell
pip install -r requirements.txt
```

#### Option B: GPU-Accelerated Mode (NVIDIA CUDA Toolkit 12.x)
Ensure your CUDA Toolkit is active (`nvcc --version`). Install base packages, remove default CPU llama wheels, and load precompiled CUDA packages:
```powershell
# Install base requirements
pip install -r requirements.txt

# Uninstall CPU version of llama-cpp-python
pip uninstall llama-cpp-python -y

# Install GPU wheels and CUDA packages
pip install -r requirements-gpu.txt
```

---

## 🧠 Large Model Files Placement (CRITICAL)

Because models exceed GitHub file size limits, you must retrieve and place them manually:

1. **Llama-3 LLM (GGUF)**:
   - Download the model file `Llama-3-8B-Instruct-Q4_K_M.gguf`.
   - Save it inside the folder: `backend/models/`.
   - Rename the file exactly to: `model.gguf`.

2. **Piper TTS Voice Files (ONNX)**:
   - Verify that `backend/models/en_US-lessac-medium.onnx` and `backend/models/en_US-lessac-medium.onnx.json` are present.

---

## ⚙️ Local Configuration (`.env`)

Configure local pipeline routes by creating `backend/.env`:
```env
# Speech Synthesizer Toggle ("piper", "openai", "elevenlabs")
TTS_PROVIDER=piper

# (Optional) Cloud Credentials
OPENAI_API_KEY=your-openai-api-key
ELEVENLABS_API_KEY=your-elevenlabs-api-key
VOICE_ID=yCxjZ3dvaYYrkVmdHAe9
```

---

## 🧪 Testing Verification Commands

Verify the complete stack installation and latency metrics using:

```powershell
# 1. Run local integration & safety regression tests
python testing/test_negotiation_safety.py

# 2. Run large-scale conversation benchmark in fast mode
python testing/evaluation/conversation_runner.py --fast

# 3. Run full GPU/GGUF/Whisper conversation benchmark
python testing/evaluation/conversation_runner.py --full
```
