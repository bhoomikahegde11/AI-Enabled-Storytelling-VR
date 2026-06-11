using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Whisper;

public class LocalSpeechProvider : ISpeechToTextProvider
{
    private static readonly object InitLock = new object();
    private static WhisperWrapper wrapper;
    private static WhisperParams whisperParams;
    private static bool initAttempted;
    private static string lastFailureReason = string.Empty;
    private static string lastRawTranscription = string.Empty;
    private static string resolvedModelPath = string.Empty;
    private const string WhisperModelFileName = "ggml-small.en.bin";
    private const long MinimumExpectedModelBytes = 400L * 1024L * 1024L;

    public static string WhisperModelPath => string.IsNullOrWhiteSpace(resolvedModelPath)
        ? GetPreferredModelPath()
        : resolvedModelPath;
    public static bool WhisperModelExists => File.Exists(WhisperModelPath) || File.Exists(GetPersistentModelPath());
    public static string LastFailureReason => lastFailureReason;
    public static string LastRawTranscription => lastRawTranscription;

    public async Task<string> Transcribe(AudioClip clip)
    {
        lastFailureReason = string.Empty;
        lastRawTranscription = string.Empty;

        if (clip == null)
        {
            Debug.LogWarning("[LocalSpeechProvider] AudioClip is null.");
            lastFailureReason = "AudioClip is null.";
            return string.Empty;
        }

        try
        {
            WhisperWrapper loadedWrapper = await GetWrapperAsync();
            if (loadedWrapper == null || whisperParams == null)
            {
                Debug.LogWarning("[LocalSpeechProvider] Whisper model is not available.");
                if (string.IsNullOrWhiteSpace(lastFailureReason))
                {
                    lastFailureReason = "Whisper model is not available.";
                }
                return string.Empty;
            }

            WhisperResult result = await loadedWrapper.GetTextAsync(clip, whisperParams);
            lastRawTranscription = result != null ? result.Result : string.Empty;
            if (string.IsNullOrWhiteSpace(lastRawTranscription))
            {
                lastFailureReason = "Whisper returned empty transcription.";
                return string.Empty;
            }

            return lastRawTranscription.Trim();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[LocalSpeechProvider] Local transcription failed: " + ex.Message);
            lastFailureReason = ex.Message;
            return string.Empty;
        }
    }

    private static async Task<WhisperWrapper> GetWrapperAsync()
    {
        if (wrapper != null)
        {
            return wrapper;
        }

        lock (InitLock)
        {
            if (wrapper != null)
            {
                return wrapper;
            }

            if (initAttempted)
            {
                return null;
            }

            initAttempted = true;
        }

        string modelPath = await ResolveModelPathAsync();
        Debug.Log("[STT-QUEST] Whisper model path: " + modelPath);
        Debug.Log("[STT-QUEST] Usable model path: " + modelPath);
        Debug.Log("[STT-QUEST] Whisper expected filename: " + WhisperModelFileName);
        Debug.Log("[STT-QUEST] Native plugin/library available: " + GetNativePluginAvailabilityHint());

        if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
        {
            Debug.LogWarning("[LocalSpeechProvider] Whisper model not found at: " + modelPath);
            lastFailureReason = "Whisper model not found at: " + modelPath;
            return null;
        }

        long modelFileSize = GetFileSizeSafe(modelPath);
        Debug.Log("[STT-QUEST] Model file size bytes: " + modelFileSize);
        if (modelFileSize > 0 && modelFileSize < MinimumExpectedModelBytes)
        {
            Debug.LogWarning("[STT-QUEST] Copied model incomplete.");
        }

        WhisperWrapper loadedWrapper = null;
        try
        {
            loadedWrapper = await WhisperWrapper.InitFromFileAsync(modelPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[STT-QUEST] Whisper load exception: " + ex);
            Debug.LogWarning("[STT-QUEST] Whisper load result: exception");
            lastFailureReason = "Failed to load Whisper model from: " + modelPath + " | " + ex.Message;
            return null;
        }

        Debug.Log("[STT-QUEST] Whisper load result: " + (loadedWrapper != null ? "success" : "null"));
        if (loadedWrapper == null)
        {
            Debug.LogWarning("[LocalSpeechProvider] Failed to load Whisper model from: " + modelPath);
            lastFailureReason = "Failed to load Whisper model from: " + modelPath;
            return null;
        }

        WhisperParams parameters = WhisperParams.GetDefaultParams();
        parameters.Language = "en";
        parameters.Translate = false;
        parameters.NoContext = true;
        parameters.SingleSegment = false;
        parameters.EnableTokens = false;

        lock (InitLock)
        {
            wrapper = loadedWrapper;
            whisperParams = parameters;
        }

        return wrapper;
    }

    private static string GetPreferredModelPath()
    {
        string streamingModelPath = Path.Combine(Application.streamingAssetsPath, "Whisper", WhisperModelFileName);
        if (Application.platform == RuntimePlatform.Android)
        {
            string persistentModelPath = GetPersistentModelPath();
            return File.Exists(persistentModelPath) ? persistentModelPath : streamingModelPath;
        }

        return streamingModelPath;
    }

    private static string GetPersistentModelPath()
    {
        return Path.Combine(Application.persistentDataPath, WhisperModelFileName);
    }

    private static async Task<string> ResolveModelPathAsync()
    {
        string streamingModelPath = Path.Combine(Application.streamingAssetsPath, "Whisper", WhisperModelFileName);
        Debug.Log("[STT-QUEST] Whisper source path: " + streamingModelPath);
        Debug.Log("[STT-QUEST] Whisper expected filename: " + WhisperModelFileName);
        Debug.Log("[STT-QUEST] Model copy source size bytes: " + GetFileSizeSafe(streamingModelPath));

        if (Application.platform != RuntimePlatform.Android)
        {
            resolvedModelPath = streamingModelPath;
            return resolvedModelPath;
        }

        string persistentModelPath = GetPersistentModelPath();
        if (File.Exists(persistentModelPath))
        {
            Debug.Log("[STT-QUEST] Model copy destination size bytes: " + GetFileSizeSafe(persistentModelPath));
            if (GetFileSizeSafe(persistentModelPath) > 0 && GetFileSizeSafe(persistentModelPath) < MinimumExpectedModelBytes)
            {
                Debug.LogWarning("[STT-QUEST] Copied model incomplete.");
            }
            resolvedModelPath = persistentModelPath;
            return resolvedModelPath;
        }

        if (!streamingModelPath.StartsWith("jar:", System.StringComparison.OrdinalIgnoreCase))
        {
            resolvedModelPath = streamingModelPath;
            return resolvedModelPath;
        }

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(streamingModelPath))
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                operation.completed += _ => taskCompletionSource.TrySetResult(true);
                await taskCompletionSource.Task;

#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    lastFailureReason = "Failed to copy Whisper model from StreamingAssets: " + request.error;
                    Debug.LogWarning("[LocalSpeechProvider] " + lastFailureReason);
                    return string.Empty;
                }

                byte[] modelBytes = request.downloadHandler.data;
                if (modelBytes == null || modelBytes.Length == 0)
                {
                    lastFailureReason = "StreamingAssets Whisper model download returned no data.";
                    Debug.LogWarning("[LocalSpeechProvider] " + lastFailureReason);
                    return string.Empty;
                }

                string directory = Path.GetDirectoryName(persistentModelPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(persistentModelPath, modelBytes);
                Debug.Log("[STT-QUEST] Whisper model copied to: " + persistentModelPath);
                Debug.Log("[STT-QUEST] Model copy destination size bytes: " + GetFileSizeSafe(persistentModelPath));
                if (GetFileSizeSafe(persistentModelPath) > 0 && GetFileSizeSafe(persistentModelPath) < MinimumExpectedModelBytes)
                {
                    Debug.LogWarning("[STT-QUEST] Copied model incomplete.");
                }
                resolvedModelPath = persistentModelPath;
                return resolvedModelPath;
            }
        }
        catch (System.Exception ex)
        {
            lastFailureReason = "Failed to prepare Whisper model path: " + ex.Message;
            Debug.LogWarning("[LocalSpeechProvider] " + lastFailureReason);
            return string.Empty;
        }
    }

    private static long GetFileSizeSafe(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return -1;
            }

            return new FileInfo(path).Length;
        }
        catch
        {
            return -1;
        }
    }

    private static string GetNativePluginAvailabilityHint()
    {
        try
        {
            return typeof(WhisperWrapper).Assembly != null ? "managed wrapper present (native library not yet validated)" : "false";
        }
        catch (System.Exception ex)
        {
            return "false (" + ex.Message + ")";
        }
    }
}
