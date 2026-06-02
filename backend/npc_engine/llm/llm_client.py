import os
import sys

# Compute the correct absolute paths relative to backend directory
BACKEND_DIR = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
MODEL_PATH = os.path.join(BACKEND_DIR, "models", "model.gguf")

_llm = None
llm_loaded = False

try:
    from llama_cpp import Llama
    
    if os.path.exists(MODEL_PATH):
        print(f"[INFO LLM] Loading local Llama model from: {MODEL_PATH}")
        gpu_layers = int(os.getenv("LLM_GPU_LAYERS", 0))
        _llm = Llama(
            model_path=MODEL_PATH,
            n_ctx=1024,       # Optimized context size for faster memory allocation
            n_threads=6,
            n_gpu_layers=gpu_layers, # Support GPU offloading if configured (e.g. LLM_GPU_LAYERS=16 or 33)
            use_mmap=False,   # Critical fix for mapping errors
            use_mlock=False,  # Safety flag
            verbose=False
        )
        llm_loaded = True
        print(f"[INFO LLM] Llama model loaded successfully (GPU Layers: {gpu_layers}).")
    else:
        print("\n" + "="*80)
        print("[WARNING LLM] Local LLM model.gguf not found!")
        print(f"Expected Path: {MODEL_PATH}")
        print("Heuristics and Rule-Based classifications will be used as fallback.")
        print("To enable full LLM capabilities:")
        print("1. Download a GGUF model (e.g., Llama-3-8B-Instruct-Q4_K_M.gguf)")
        print(f"2. Save it under: {MODEL_PATH}")
        print("="*80 + "\n")
except Exception as e:
    print("\n" + "="*80)
    print(f"[ERROR LLM] Failed to initialize Llama library: {e}")
    print("Ensure llama-cpp-python is correctly installed for your system.")
    print("Proceeding with Rule-Based classifiers and templates as fallback.")
    print("="*80 + "\n")


def run_llm(prompt: str, max_tokens: int = 3, stop: list = None) -> str:
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
            temperature=0.3  # Lower temperature for faster, more focused generations
        )
        return response["choices"][0]["text"].strip()
    except Exception as e:
        print(f"[ERROR LLM] Inference failed: {e}")
        return ""
