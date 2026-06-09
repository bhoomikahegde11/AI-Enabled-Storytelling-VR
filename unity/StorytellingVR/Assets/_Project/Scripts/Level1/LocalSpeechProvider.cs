using System.Threading.Tasks;
using UnityEngine;
using Whisper;

public class LocalSpeechProvider : ISpeechToTextProvider
{
    private static readonly object InitLock = new object();
    private static WhisperWrapper wrapper;
    private static WhisperParams whisperParams;
    private static bool initAttempted;

    public async Task<string> Transcribe(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[LocalSpeechProvider] AudioClip is null.");
            return string.Empty;
        }

        try
        {
            WhisperWrapper loadedWrapper = await GetWrapperAsync();
            if (loadedWrapper == null || whisperParams == null)
            {
                Debug.LogWarning("[LocalSpeechProvider] Whisper model is not available.");
                return string.Empty;
            }

            WhisperResult result = await loadedWrapper.GetTextAsync(clip, whisperParams);
            return result != null && !string.IsNullOrWhiteSpace(result.Result)
                ? result.Result.Trim()
                : string.Empty;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[LocalSpeechProvider] Local transcription failed: " + ex.Message);
            return string.Empty;
        }
    }

    private static async Task<WhisperWrapper> GetWrapperAsync()
    {
        if (wrapper != null)
        {
            return wrapper;
        }

        string modelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Whisper/ggml-small.en.bin");

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

        if (!System.IO.File.Exists(modelPath))
        {
            Debug.LogWarning("[LocalSpeechProvider] Whisper model not found at: " + modelPath);
            return null;
        }

        WhisperWrapper loadedWrapper = await WhisperWrapper.InitFromFileAsync(modelPath);
        if (loadedWrapper == null)
        {
            Debug.LogWarning("[LocalSpeechProvider] Failed to load Whisper model from: " + modelPath);
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
}
