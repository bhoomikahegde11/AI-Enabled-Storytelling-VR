import os
import sys
from npc_engine.utils import hardware

# Compute the correct absolute paths relative to backend directory
BACKEND_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
MODEL_PATH = os.path.join(BACKEND_DIR, "models", "model.gguf")

_llm = None
llm_loaded = False

# Local variables mirrored/imported from hardware manager
CUDA_AVAILABLE = hardware.CUDA_AVAILABLE
USE_GPU = hardware.USE_GPU

offload_status = "None"

try:
    from llama_cpp import Llama
    
    if os.path.exists(MODEL_PATH):
        print(f"[INFO LLM] Loading local Llama model from: {MODEL_PATH}")
        if CUDA_AVAILABLE:
            try:
                # Tier 1: Try full GPU offload
                _llm = Llama(
                    model_path=MODEL_PATH,
                    n_ctx=2048,
                    n_threads=6,
                    n_gpu_layers=-1,
                    use_mmap=False,
                    use_mlock=False,
                    verbose=True
                )
                llm_loaded = True
                hardware.LLM_DEVICE = "GPU Full Offload"
                print("[INFO LLM] Llama model loaded successfully on GPU (Full offload).")
            except Exception as e:
                print(f"[WARNING LLM] GPU loading with n_gpu_layers=-1 failed ({e}). Retrying with fallback n_gpu_layers=25...")
                try:
                    # Tier 2: Try partial GPU offload
                    _llm = Llama(
                        model_path=MODEL_PATH,
                        n_ctx=2048,
                        n_threads=6,
                        n_gpu_layers=25,
                        use_mmap=False,
                        use_mlock=False,
                        verbose=True
                    )
                    llm_loaded = True
                    hardware.LLM_DEVICE = "GPU Partial (25 layers)"
                    print("[INFO LLM] Llama model loaded successfully on GPU (fallback 25 layers).")
                except Exception as e2:
                    print(f"[ERROR LLM] GPU loading failed on fallback ({e2}). Falling back to CPU loading...")
                    # Update hardware module state so subsequent components (like STT) know GPU is unusable
                    hardware.CUDA_AVAILABLE = False
                    CUDA_AVAILABLE = False
                    hardware.DEVICE_MODE = "cpu"
                    hardware.LLM_DEVICE = "CPU Fallback"
                    
                    # Tier 3: CPU fallback
                    _llm = Llama(
                        model_path=MODEL_PATH,
                        n_ctx=2048,
                        n_threads=6,
                        n_gpu_layers=0,
                        use_mmap=False,
                        use_mlock=False,
                        verbose=True
                    )
                    llm_loaded = True
                    print("[INFO LLM] Llama model loaded successfully on CPU.")
        else:
            # Load directly on CPU
            _llm = Llama(
                model_path=MODEL_PATH,
                n_ctx=2048,
                n_threads=6,
                n_gpu_layers=0,
                use_mmap=False,
                use_mlock=False,
                verbose=True
            )
            llm_loaded = True
            hardware.LLM_DEVICE = "CPU Fallback"
            print("[INFO LLM] Llama model loaded successfully on CPU.")
    else:
        hardware.LLM_DEVICE = "CPU Fallback"
        print("\n" + "="*80)
        print("[WARNING LLM] Local LLM model.gguf not found!")
        print(f"Expected Path: {MODEL_PATH}")
        print("Heuristics and Rule-Based classifications will be used as fallback.")
        print("To enable full LLM capabilities:")
        print("1. Download a GGUF model (e.g., Llama-3-8B-Instruct-Q4_K_M.gguf)")
        print(f"2. Save it under: {MODEL_PATH}")
        print("="*80 + "\n")
except Exception as e:
    hardware.CUDA_AVAILABLE = False
    CUDA_AVAILABLE = False
    hardware.DEVICE_MODE = "cpu"
    hardware.LLM_DEVICE = "CPU Fallback"
    print("\n" + "="*80)
    print(f"[ERROR LLM] Failed to initialize Llama library: {e}")
    print("Ensure llama-cpp-python is correctly installed for your system.")
    print("Proceeding with Rule-Based classifiers and templates as fallback.")
    print("="*80 + "\n")




def run_llm(prompt: str, max_tokens: int = 3, stop: list = None, temperature: float = 0.3) -> str:
    """
    Executes a prompt against the local LLM if loaded, returning an empty string on fallback.
    """
    if not llm_loaded or _llm is None:
        return ""
    
    try:
        # Default stop tokens to cut off GGUF rambling instantly
        if stop is None:
            stop = ["<|eot_id|>", "<|end_of_text|>", "\n\n", "User:", "Original dialogue"]
            
        response = _llm(
            prompt, 
            max_tokens=max_tokens,
            stop=stop,
            temperature=temperature
        )
        return response["choices"][0]["text"].strip()
    except Exception as e:
        print(f"[ERROR LLM] Inference failed: {e}")
        return ""
