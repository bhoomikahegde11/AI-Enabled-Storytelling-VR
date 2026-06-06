import os
import sys
import ctypes

# Initialize dotenv
try:
    from dotenv import load_dotenv
    # Load dotenv from current working directory or backend directory if running from there
    load_dotenv()
except ImportError:
    pass

USE_GPU = os.getenv("USE_GPU", "true").lower() in ["true", "1", "yes"]
USE_LLM_PERSONALITY = os.getenv("USE_LLM_PERSONALITY", "true").lower() in ["true", "1", "yes"]
CUDA_AVAILABLE = False

def setup_cuda_dlls():
    """
    Search and preload pip-packaged nvidia CUDA/CuDNN DLLs on Windows.
    This registers the folders and preloads DLLs into process memory so that
    llama.dll and CTranslate2 can resolve dependencies properly.
    """
    nvidia_dirs = []
    if sys.platform == "win32":
        import site
        site_packages_paths = site.getsitepackages()
        try:
            user_site = site.getusersitepackages()
            if user_site not in site_packages_paths:
                site_packages_paths.append(user_site)
        except Exception:
            pass
            
        for site_path in site_packages_paths:
            nvidia_root = os.path.join(site_path, "nvidia")
            if os.path.exists(nvidia_root):
                for root, dirs, files in os.walk(nvidia_root):
                    for d in ["bin", "lib"]:
                        if d in dirs:
                            folder = os.path.join(root, d)
                            if any(f.endswith(".dll") for f in os.listdir(folder)):
                                nvidia_dirs.append(folder)
                                try:
                                    os.add_dll_directory(folder)
                                except Exception:
                                    pass
                                    
        # Preload the core CUDA DLLs using ctypes
        for folder in nvidia_dirs:
            for file in os.listdir(folder):
                if file.endswith(".dll") and any(x in file.lower() for x in ["cudart", "cublas", "nvrtc", "cudnn"]):
                    path = os.path.join(folder, file)
                    try:
                        ctypes.CDLL(path)
                    except Exception:
                        pass

# 1. Setup CUDA DLL paths (always try on Windows to prevent DLL import errors for CUDA-compiled wheels)
try:
    setup_cuda_dlls()
except Exception as e:
    pass

# 2. Check for CUDA support in llama-cpp
if USE_GPU:
    try:
        from llama_cpp import llama_cpp
        if llama_cpp.llama_supports_gpu_offload():
            CUDA_AVAILABLE = True
    except Exception:
        # Gracefully handle if llama-cpp is missing, compiled without CUDA, or fails to link DLLs
        CUDA_AVAILABLE = False
else:
    CUDA_AVAILABLE = False

DEVICE_MODE = "cuda" if CUDA_AVAILABLE else "cpu"
LLM_DEVICE = "CPU Fallback"
WHISPER_DEVICE = "CPU"
