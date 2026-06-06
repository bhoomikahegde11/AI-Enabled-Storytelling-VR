# Performance Optimization: GPU Acceleration & CPU Fallbacks

This document outlines the performance optimizations, CUDA DLL preloading mechanisms, memory offloading configurations, and CPU fallback paths that enable real-time execution of the AI engines.

## 🚀 Performance Statistics

| Platform / Mode | STT (Whisper small.en) | LLM (Llama-3-8B) | TTS (Piper ONNX) | Total Response Latency |
|---|---|---|---|---|
| **CPU Only (Fallback)** | ~3.5s - 5.0s | ~12.0s - 18.0s | ~800ms | **~16.0s - 23.0s** |
| **GPU Accelerated (CUDA)** | **~250ms - 400ms** | **~400ms - 800ms** | **~100ms** | **~750ms - 1.5s** |

*Note: GPU acceleration delivers a **~15x reduction** in overall response times, making speech interactions in VR feel interactive and immediate.*

---

## ⚙️ GPU Acceleration Architecture (`hardware.py`)

The backend dynamically detects and manages execution environments to prioritize GPU hardware:
1. **NVIDIA CUDA Auto-detection**: Checks for NVIDIA driver availability using helper scripts and preloads required CUDA DLLs (e.g. `cublas64_12.dll`, `cudnn_ops_train64_9.dll`) to prevent Windows environment variable path lookup failures.
2. **Layer Offloading (n_gpu_layers)**: 
   - Llama GGUF models offload repeating neural network layers to the GPU:
     * **Tier 1 (Full Offload)**: Sets `n_gpu_layers = -1` (offloading all 32 model layers directly to GPU VRAM).
     * **Tier 2 (Partial Offload)**: Falls back to `n_gpu_layers = 25` if GPU memory allocation limits are reached.
     * **Tier 3 (CPU Fallback)**: Sets `n_gpu_layers = 0` if CUDA runtime libraries are completely unavailable.
3. **STT Float16 Quantization**: Whisper's execution is accelerated by setting compute type to `float16` and setting device to `cuda` (instead of `cpu`).

---

## 🛠️ Dynamic preloading and DLL fixes
On Windows systems, Python packages like `llama-cpp-python` and `faster-whisper` frequently experience load-time runtime failures due to missing or unmapped CUDA/cuBLAS DLL dependency paths. 

To overcome this, `hardware.py` implements proactive path injections:
- Detects the CUDA Toolkit installation path (via checking environmental variables or common registry paths).
- Calls `os.add_dll_directory()` programmatically on the CUDA `bin` path at application startup.
- Verifies GPU offloading output parameters (e.g. tracking active processes via `nvidia-smi` logging).
