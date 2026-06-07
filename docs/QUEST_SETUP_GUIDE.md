# Meta Quest Demo Setup Guide

This guide describes how to configure, deploy, and run the AI-Enabled Storytelling VR demo on a Meta Quest headset, connecting it to the Python backend running on your laptop.

---

## 1. Required Hardware

*   **Meta Quest Headset**: Meta Quest 2, 3, or Pro.
*   **USB-C Link/Data Cable**: For installing the APK via SideQuest or Meta Quest Developer Hub (if installing via cable).
*   **Laptop running the backend**: A machine capable of running the FastAPI server and processing speech-to-text/negotiation logic.
*   **Same WiFi Network**: The laptop and the Meta Quest headset **MUST** be connected to the exact same WiFi network. (Quest cannot resolve `localhost` or `127.0.0.1` back to the laptop; it must communicate via local IP).

---

## 2. Required Software

*   **Unity Editor**: Use the correct Unity version configured for the project (refer to `ProjectVersion.txt`).
*   **Meta Quest Developer Hub (MQDH) or SideQuest**: Used to sideload/install the built `.apk` onto the headset.
*   **Python 3.10+**: Environment to run the backend API server.
*   **Git**: Cloned repository on your machine.

---

## 3. Backend Setup

Follow these steps to start the backend so that devices on the local network can connect to it:

1.  Open your terminal and navigate to the backend folder:
    ```bash
    cd backend
    ```
2.  Install the required package dependencies if you haven't already:
    ```bash
    pip install -r requirements.txt
    ```
3.  Launch the backend server using this exact command:
    ```bash
    python -m uvicorn api:app --host 0.0.0.0 --port 8000
    ```

> [!IMPORTANT]
> *   **DO NOT** use `127.0.0.1` or `localhost` when launching the server.
> *   Binding to `--host 0.0.0.0` instructs the backend to listen on all network interfaces, allowing the Quest headset on your local WiFi network to discover and connect to the backend running on the laptop.

---

## 4. Finding Laptop WiFi IP

To allow the headset to find your computer, you need to find your laptop's current local IP address:

1.  Open Command Prompt or PowerShell on your laptop.
2.  Run the command:
    ```cmd
    ipconfig
    ```
3.  Locate the section labeled **Wireless LAN adapter Wi-Fi**.
4.  Find the **IPv4 Address**.
    *   *Example*: `192.168.18.22`

---

## 5. Verify Backend Connection

Before building or launching the Unity app, confirm that the backend is reachable via the local network:

1.  Open a browser on your laptop (or on a phone connected to the same WiFi).
2.  Navigate to:
    ```
    http://<LAPTOP_IP>:8000
    ```
    *(e.g., `http://192.168.18.22:8000`)*
3.  If it returns a response (or standard API JSON), the connection is active.
4.  **If it does not load**:
    *   Verify the backend terminal is running and did not crash.
    *   Confirm your laptop and device are on the exact same WiFi.
    *   Check your OS Firewall (Windows Firewall might block incoming connections to Python/Uvicorn). Set Python/Uvicorn permissions to allow on Private/Public networks.

---

## 6. Unity Backend URL Setup Before Building APK

In Unity, the URLs must point to your laptop's WiFi IP address instead of `localhost` before you compile the APK. 

Configure these values in the Unity Inspector:

1.  **Main Chat/Negotiation URL**
    *   Find the **APIManager** component in your active scene hierarchy (usually attached to a Managers or ChatManager object).
    *   Set **Backend Url** to:
        ```
        http://<LAPTOP_IP>:8000
        ```
        *(e.g., `http://192.168.18.22:8000`)*

2.  **Speech-To-Text (STT) URL**
    *   Find the **Level1VoiceInputManager** component in the active scene hierarchy.
    *   Set **Server Url** to:
        ```
        http://<LAPTOP_IP>:8000/stt
        ```
        *(e.g., `http://192.168.18.22:8000/stt`)*

> [!WARNING]
> Do not commit your personal IP address to the git repository. Keep the defaults as `localhost` in the codebase, and only change them in your local Unity Inspector before building your APK.

---

## 7. Quest Microphone Permission

Since this demo uses voice-input negotiation, Quest requires microphone recording permission:

*   **Unity System Integration**: The project utilizes the `UnityEngine.Microphone` API. Unity automatically injects the record permission into the temporary manifest during build time:
    ```xml
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
    ```
*   **First Run Prompt**: Upon launching the app for the first time on the Meta Quest, a prompt will ask for microphone permission. Click **Allow**.
*   **If Permssion was Denied**:
    1. Open Quest **Settings**.
    2. Go to **Apps** -> **Installed Apps**.
    3. Select this project app.
    4. Click **Permissions** and toggle **Microphone** to enabled.

---

## 8. Unity Build Settings Checklist

Before building the APK, verify that the Scene transition sequence is exactly as follows in **File** -> **Build Settings**:

1.  `0` **Bootstrap**
2.  `1` **SpicesIntro**
3.  `2` **TraderIntroScene**
4.  `3` **Transcation_Tutorial**
5.  `4` **CoinScene**
6.  `5` **MainScene1** *(or current active marketplace scene)*

> [!IMPORTANT]
> Always launch/play the game starting from the **Bootstrap** scene (Index 0). The bootstrap scene configures essential cross-scene controllers and manager systems.

---

## 9. Controller Controls

The VR project maps controls to the Quest controllers:

*   **Right Controller Trigger**: **Hold** to record voice/bargain speech (release to send).
*   **A Button (Right Controller)**: Same as pressing `Enter` on keyboard — **Confirm/Send** the transcribed text to start bargaining.
*   **B Button (Right Controller)**: Same as pressing `R` on keyboard — **Reset/Retry** bargaining, clearing the text input field.

---

## 10. Running Full Demo

Ensure you execute this flow order:

1.  Connect the laptop and the Quest headset to the same WiFi network.
2.  Start the backend server on the laptop using `--host 0.0.0.0`.
3.  Test your laptop's IP URL in a browser to make sure it loads.
4.  Build and sideload the APK onto the Quest.
5.  Launch the app on the Quest headset.
6.  **Allow microphone permission** when prompted.
7.  Verify you start from the **Bootstrap** scene.
8.  Complete the full sequential flow:
    *   **Spice Intro** -> **Trader Intro** -> **Transaction Tutorial** -> **Coin Inspection** -> **Marketplace Bargaining**.

---

## 11. Common Issues

### Problem: Quest app says connection failed / cannot reach the backend
*   **Fix**: Verify your laptop's IP has not changed (routers assign new IPs periodically). Ensure the server is started with `--host 0.0.0.0`. Check if Windows Firewall is blocking incoming Python connections.

### Problem: Voice recording/transcription not working on Quest
*   **Fix**: Go to Quest settings and verify that microphone permissions are enabled for the app. Check that your **Server Url** in `Level1VoiceInputManager` is pointing to the correct laptop IP and includes the `/stt` suffix. Check the backend console output for connection errors.

### Problem: Works perfectly in Unity Editor, but connection times out on Quest APK
*   **Fix**: Confirm that you didn't leave the URLs in `APIManager` or `Level1VoiceInputManager` pointing to `localhost` or `127.0.0.1`. Sideloaded APKs run directly on the Android OS of the headset, so `localhost` refers to the headset itself instead of the laptop.

---

## Demo Day Quick Checklist

- [ ] Python backend is running on the laptop with `--host 0.0.0.0`.
- [ ] Quest headset is on the exact same WiFi network as the laptop.
- [ ] Correct WiFi IP address is configured in the Unity Inspector for `APIManager` and `Level1VoiceInputManager`.
- [ ] Microphone permission has been allowed on the Quest headset.
- [ ] Right-hand controller buttons (Trigger, A, B) are fully working.
- [ ] Demo has been verified by playing the complete flow beginning at **Bootstrap**.
