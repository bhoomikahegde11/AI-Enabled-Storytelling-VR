# AI-Enabled Storytelling VR: Backend Server Setup Guide

This guide is for teammates testing the Unity VR project on a local machine or Meta Quest VR headset. It outlines the steps to configure and launch the FastAPI backend server from a fresh repository clone.

---

## 1. Prerequisites

Before setting up the server, ensure you have the following installed:
* **Python**: Version `3.12.5` (or any `3.12.x` release) is required.
* **pip**: Python Package Installer (usually bundled with Python).
* **Virtual Environment Support**: Recommended to isolate project dependencies.

---

## 2. Initial Setup

Open a terminal or command prompt, navigate to the project repository root, and run the following commands:

### Step 1: Navigate to the backend folder
```bash
cd backend
```

### Step 2: Create a Virtual Environment (`venv`)

#### Windows (Command Prompt or PowerShell):
```powershell
python -m venv venv
venv\Scripts\activate
```

#### macOS / Linux:
```bash
python3 -m venv venv
source venv/bin/activate
```

*(You will know the virtual environment is active when you see `(venv)` prepended to your terminal prompt.)*

---

## 3. Install Dependencies

To install the required Python packages, choose the appropriate command based on your computer's hardware:

### Standard Installation (CPU-only / Development / macOS / Linux)
```bash
pip install -r requirements.txt
```

### GPU-Accelerated Installation (Windows / Linux with NVIDIA GPU and CUDA Support)
```bash
pip install -r requirements-gpu.txt
```

---

## 4. Environment & API Keys Configuration

The backend reads configuration settings from a local `.env` file. 

1. Duplicate `.env.example` in the `backend/` directory and rename the copy to `.env`:
   ```bash
   cp .env.example .env
   ```
2. Open `.env` in a text editor and adjust the settings:
   * **USE_GPU**: Set to `true` to utilize CUDA acceleration (highly recommended for STT/LLM response speeds), or `false` to force CPU fallback.
   * **USE_LLM_PERSONALITY**: Set to `true` to enable GGUF LLM dialogue rephrasing, or `false` to use fast template-based replies (useful for quick testing without loading large models).
   * **TTS_PROVIDER**: Set to `piper` (offline local TTS, default), `openai`, or `elevenlabs`.
   * **OPENAI_API_KEY** / **ELEVENLABS_API_KEY**: Provide your API keys here if using cloud-based TTS providers.
   
> [!WARNING]
> Never commit your `.env` file containing actual keys back to the repository. The `.gitignore` file is configured to ignore `.env` by default.

---

## 5. Running the Server

To start the FastAPI backend server used by the Unity game, make sure your virtual environment is active and run:

```bash
python -m uvicorn api:app --reload --host 0.0.0.0 --port 8000
```

* **`--host 0.0.0.0`**: This instructs the server to listen on all available network interfaces. This is **required** so that external devices (like a standalone Meta Quest headset) on your local Wi-Fi can communicate with your computer.
* **`--port 8000`**: The default port.

---

## 6. Unity Connection & Headset Testing

For the VR game to load characters, speak dialog, or process voice requests, **the backend server must be running before you enter play mode or load the marketplace scene.**

### standalone Quest Headset Testing:
* स्टैंडअलोन testing (Quest link, APK, or side-loaded builds) uses the local Wi-Fi network. **`localhost` or `127.0.0.1` will NOT work** because the headset runs on its own internal Android OS and looks for the server locally.
* You must find your host computer's Local Area Network (LAN) IP address (e.g., `192.168.1.45`).
  * **Windows**: Open cmd and type `ipconfig`. Find the `IPv4 Address` under your active Wi-Fi adapter.
  * **macOS / Linux**: Open terminal and type `ifconfig` or `ip a`.

### Network Configurations in Unity:
1. **APIManager Base URL**:
   * Open `APIManager.cs` (located in `Assets/_Project/Scripts/Level1/APIManager.cs`).
   * Update line 8 to point to your computer's LAN IP address:
     ```csharp
     private string baseURL = "http://192.168.x.x:8000"; // Replace 192.168.x.x with your PC's IP
     ```
2. **Speech-to-Text Server URL**:
   * Select the **`[BuildingBlock] Speech To Text`** GameObject in the scene hierarchy.
   * In the Inspector panel, update the public `Server Url` field on the `Level1VoiceInputManager` component:
     ```text
     http://192.168.x.x:8000/stt
     ```

---

## 7. Testing Your Setup

1. **Verify Server is Alive**: Open a browser on any device (e.g. your phone or PC) and navigate to `http://localhost:8000/docs` (or `http://<your-pc-ip>:8000/docs`). You should see the interactive FastAPI Swagger UI documentation page.
2. **Start the Game**: Load and play from the **`Bootstrap`** scene in the Unity Editor.
3. **Verify Transition**: Play through the Spice Introduction and Trader Intro until you reach the bargaining tutorial.
4. **Test Bargaining**: Hold down the mic input key (e.g. `V`), speak an offer (e.g., *"How about twenty Varahas?"*), and verify that the customer responds and your Varaha amounts or reputation values update on the HUD.

---

## 8. Troubleshooting

### `ModuleNotFoundError: No module named '...'`
Ensure you have activated the virtual environment and installed the dependencies. If you added new libraries, make sure they are in `requirements.txt`.

### `[Errno 10048] error while attempting to bind on address...`
This means port `8000` is already in use by another process.
* **Fix**: Find and kill the process using port `8000`, or launch uvicorn on another port using `--port 8080` and update the Unity script variables accordingly.

### Unity Cannot Connect (`ConnectToHostFailed`)
* Make sure uvicorn is running.
* Double-check that your computer and the Quest headset are connected to the **exact same Wi-Fi router**.
* Verify that you did not use `localhost` in Unity when testing on the headset.

### Windows Defender Firewall Blocking
Windows might block incoming connections to Python/uvicorn.
* **Fix**: Go to *Control Panel > System and Security > Windows Defender Firewall > Allowed apps*, click *Allow another app*, select your Python executable in the virtual environment (`venv/Scripts/python.exe`), and allow it for **Private** networks.
