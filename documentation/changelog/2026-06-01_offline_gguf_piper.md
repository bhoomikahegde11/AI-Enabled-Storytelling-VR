# Changelog: June 1, 2026

## [2026-06-01] - Offline GGUF LLM & Piper TTS Pipeline

### Added
- Replaced ElevenLabs cloud TTS with Piper local TTS for fully offline, zero-latency speech synthesis.
- Integrated Llama 3.1 8B Instruct (Q4_K_M GGUF) via llama-cpp-python with full CUDA GPU offload on RTX 4060.
- Implemented `llm_client.py` with `run_llm()` and `run_llm_timeout()` for local GGUF inference with configurable token limits, stop sequences, and temperature.
- Added `piper_tts.py` for local neural text-to-speech using Piper ONNX voice models with configurable speaker, rate, and output format.
- Configured GPU-first loading strategy: full model offloaded to CUDA with `n_gpu_layers=-1`.

### Changed
- Removed dependency on ElevenLabs API keys and cloud connectivity for speech generation.
- Updated `dialogue_generator.py` to route all NPC personality rewrites through the local GGUF model instead of cloud LLM.
- Achieved sub-200ms total NPC response latency (intent + LLM rewrite + TTS) on local hardware.

### Technical Details
- Model path: `backend/models/model.gguf` (Llama 3.1 8B Instruct Q4_K_M, ~4.6 GB)
- VRAM usage: ~4.7 GB for model + ~256 MB KV cache on RTX 4060 (8 GB)
- Piper voice model: English medium-quality ONNX (~20 MB)
