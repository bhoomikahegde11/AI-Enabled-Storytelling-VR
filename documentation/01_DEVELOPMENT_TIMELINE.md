# Development Timeline: AI-Enabled Storytelling VR

This document logs the structured project milestones and development progression from inception to production deployment (April 2026 - June 2026), mapped to git commits and architectural milestones.

## 📅 Roadmap Overview

```mermaid
gantt
    title Development Roadmap (H1 2026)
    dateFormat  YYYY-MM-DD
    section Phase 1: Core Mechanics
    Blend Shape Animations & Avatar Rigging :active, 2026-04-01, 2026-04-12
    ElevenLabs API & Audio Pipeline        :active, 2026-04-12, 2026-04-20
    section Phase 2: Offline Integration
    Modular Offline GGUF Engine            :active, 2026-05-15, 2026-06-01
    Dynamic Gaze & Leaving Delay           :active, 2026-06-01, 2026-06-02
    Thinking State Coroutines              :active, 2026-06-02, 2026-06-03
    section Phase 3: Hardening
    Robustness Preprocessor & Fuzzy Matching:active, 2026-06-03, 2026-06-04
    Large Scale Benchmark & Documentation  :active, 2026-06-04, 2026-06-05
```

---

## 🚀 Development Phases

### Phase 1: Interactive Avatars & Speech Pipeline (April 2026)

- **April 11, 2026: Blend Shape Animations & Character Rigging**
  * *Focus*: Rigging marketplace NPC models in Unity and implementing blend shape facial expression animation loops (mouth, eyes) synchronized with raw volume inputs.
  * *Git Commit Ref*: `feat(unity/avatars): add basic blend shape mapping for speech mouth movements`
  * *Status*: COMPLETED.

- **April 18, 2026: ElevenLabs Cloud TTS & Speech Pipeline Integration**
  * *Focus*: Setting up the cloud speech pipeline, sending generated dialogue responses from Python API to ElevenLabs for natural voice synthesis, and fetching streaming audio bytes into Unity.
  * *Git Commit Ref*: `feat(tts/elevenlabs): integrate cloud audio streaming & caching`
  * *Status*: COMPLETED.

---

### Phase 2: Local AI Engines & VR Experience Polish (May - June 2026)

- **June 1, 2026: Offline GGUF LLM Engine & Piper TTS Deployment**
  * *Focus*: Replaced cloud API dependencies (ElevenLabs, OpenAI) with 100% offline, local, private, low-latency engines running on CPU/GPU CUDA. Integrated `llama-cpp-python` with `model.gguf` (Llama-3-8B) and Piper TTS ONNX models to achieve latency reduction under 1.5 seconds.
  * *Git Commit Ref*: `feat(backend/gguf): deploy offline Llama-3 and Piper TTS engines`
  * *Status*: COMPLETED.

- **June 2, 2026: Dynamic Gaze, Walking Away Delay & State Actions**
  * *Focus*: Implementing VR spatial immersion mechanics in Unity. NPCs track player head gaze, react to prolonged silence, and state-aware walkaway delays occur if the player is hostile or refuses to propose offers.
  * *Git Commit Ref*: `feat(unity/xr): integrate head gaze tracking and walkaway coroutines`
  * *Status*: COMPLETED.

- **June 3, 2026: Unity Coroutine Thinking State Animations**
  * *Focus*: Adding visual cues to indicate AI processing states. When the NPC is thinking, they display custom idle thinking animations (stroking chin, folding arms) managed by async coroutines to mask engine loading latencies.
  * *Git Commit Ref*: `feat(unity/states): add coroutine-driven thinking animation triggers`
  * *Status*: COMPLETED.

---

### Phase 3: Conversational Robustness & Hardening (June 2026)

- **June 4, 2026: Production Conversational Robustness Layer**
  * *Focus*: Created `conversation_understanding.py` preprocessing layer using fuzzy semantic matching (`rapidfuzz`) and state-aware corrections. Cleaned transcriptions of Whisper hallucinations (e.g. removing "Thanks for watching.").
  * *Git Commit Ref*: `feat(robustness): implement RapidFuzz preprocessor and Whisper post-processing`
  * *Status*: COMPLETED.

- **June 5, 2026: Large-Scale Benchmark Suite & Capstone Archive**
  * *Focus*: Programmatically generated 1,750+ inputs, simulated 100 multi-turn negotiations, scored LLM rephrasing uniqueness, verified performance constraints, and compiled the documentation archive.
  * *Git Commit Ref*: `feat(testing/benchmark): implement 1,750+ input benchmark and metrics compiler`
  * *Status*: COMPLETED.
