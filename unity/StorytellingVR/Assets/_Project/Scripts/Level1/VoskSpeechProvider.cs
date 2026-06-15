using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Vosk;

// Vosk is intended for standalone Quest offline STT.
public class VoskSpeechProvider : MonoBehaviour, ISpeechToTextProvider
{
    public string modelFolderName = "vosk-model-small-en-us-0.15";
    public bool debugLogs = true;

    private const string ModelRootFolderName = "Vosk";
    private const string ModelManifestFileName = "filelist.txt";

    private readonly object initLock = new object();
    private Task<Model> modelInitTask;
    private Model cachedModel;

    public async Task<string> Transcribe(AudioClip clip)
    {
        if (clip == null)
        {
            LogError("AudioClip is null.");
            return string.Empty;
        }

        try
        {
            Model model = await GetOrCreateModelAsync();
            if (model == null)
            {
                return string.Empty;
            }

            Log("Audio received: samples=" + clip.samples + ", channels=" + clip.channels + ", frequency=" + clip.frequency);
            short[] pcm = ExtractPcm16(clip);
            if (pcm == null || pcm.Length == 0)
            {
                LogError("Unable to extract PCM data from AudioClip.");
                return string.Empty;
            }

            using (VoskRecognizer recognizer = new VoskRecognizer(model, clip.frequency))
            {
                recognizer.SetMaxAlternatives(0);
                recognizer.SetWords(false);
                recognizer.AcceptWaveform(pcm, pcm.Length);

                string rawResult = recognizer.FinalResult();
                if (string.IsNullOrWhiteSpace(rawResult))
                {
                    rawResult = recognizer.Result();
                }
                Log("Raw result: " + rawResult);

                string finalText = ExtractText(rawResult).Trim();
                Log("Final transcription: " + finalText);
                return finalText;
            }
        }
        catch (DllNotFoundException ex)
        {
            Log("Native library loaded: false");
            LogError(ex.Message);
            return string.Empty;
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return string.Empty;
        }
    }

    private async Task<Model> GetOrCreateModelAsync()
    {
        if (cachedModel != null)
        {
            return cachedModel;
        }

        lock (initLock)
        {
            if (modelInitTask == null)
            {
                modelInitTask = InitializeModelAsync();
            }
        }

        cachedModel = await modelInitTask;
        return cachedModel;
    }

    private async Task<Model> InitializeModelAsync()
    {
        Log("Initializing");

        try
        {
            Vosk.Vosk.SetLogLevel(debugLogs ? 0 : -1);
            Log("Native library loaded: true");
        }
        catch (DllNotFoundException ex)
        {
            Log("Native library loaded: false");
            LogError(ex.Message);
            return null;
        }

        string modelPath = await ResolveModelPathAsync();
        bool modelExists = HasRequiredModelFiles(modelPath);
        Log("Model path: " + modelPath);
        Log("Model exists: " + modelExists);
        if (!modelExists)
        {
            LogError("Model folder is missing required files.");
            return null;
        }

        try
        {
            Model model = new Model(modelPath);
            using (VoskRecognizer recognizer = new VoskRecognizer(model, 16000.0f))
            {
                recognizer.SetMaxAlternatives(0);
            }

            Log("Provider initialized");
            return model;
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
            return null;
        }
    }

    private async Task<string> ResolveModelPathAsync()
    {
        string sourcePath = CombineStreamingPath(ModelRootFolderName, modelFolderName);
        string runtimePath = Path.Combine(Application.persistentDataPath, ModelRootFolderName, modelFolderName);

        Log("Model source path: " + sourcePath);
        Log("Model runtime path: " + runtimePath);

        if (Application.platform != RuntimePlatform.Android)
        {
            return Path.Combine(Application.streamingAssetsPath, ModelRootFolderName, modelFolderName);
        }

        if (HasRequiredModelFiles(runtimePath))
        {
            return runtimePath;
        }

        if (sourcePath.StartsWith("jar:", StringComparison.OrdinalIgnoreCase) || sourcePath.Contains("://"))
        {
            await CopyModelFromStreamingAssetsAsync(sourcePath, runtimePath);
            return runtimePath;
        }

        if (!Directory.Exists(runtimePath))
        {
            CopyDirectory(Path.Combine(Application.streamingAssetsPath, ModelRootFolderName, modelFolderName), runtimePath);
        }

        return runtimePath;
    }

    private async Task CopyModelFromStreamingAssetsAsync(string sourceRoot, string runtimeRoot)
    {
        string manifestPath = CombineStreamingPath(ModelRootFolderName, modelFolderName, ModelManifestFileName);
        string manifestContent = await ReadTextAsync(manifestPath);
        if (string.IsNullOrWhiteSpace(manifestContent))
        {
            LogError("Model manifest is missing or empty at: " + manifestPath);
            return;
        }

        string[] relativeFiles = manifestContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string relativeFile in relativeFiles)
        {
            string sourceFilePath = AppendStreamingPath(sourceRoot, relativeFile);
            string destinationFilePath = Path.Combine(runtimeRoot, relativeFile.Replace('/', Path.DirectorySeparatorChar));
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            byte[] fileBytes = await ReadBytesAsync(sourceFilePath);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                LogError("Failed to copy model file: " + relativeFile);
                return;
            }

            File.WriteAllBytes(destinationFilePath, fileBytes);
        }
    }

    private static short[] ExtractPcm16(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        if (!clip.GetData(samples, 0))
        {
            return null;
        }

        short[] pcm = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            pcm[i] = (short)Mathf.Clamp(samples[i] * short.MaxValue, short.MinValue, short.MaxValue);
        }

        return pcm;
    }

    private static string ExtractText(string rawResult)
    {
        if (string.IsNullOrWhiteSpace(rawResult))
        {
            return string.Empty;
        }

        VoskRecognitionResult parsed = JsonUtility.FromJson<VoskRecognitionResult>(rawResult);
        if (parsed != null && !string.IsNullOrWhiteSpace(parsed.text))
        {
            return parsed.text;
        }

        return string.Empty;
    }

    private static bool HasRequiredModelFiles(string modelPath)
    {
        return Directory.Exists(modelPath)
            && File.Exists(Path.Combine(modelPath, "conf", "model.conf"))
            && File.Exists(Path.Combine(modelPath, "am", "final.mdl"));
    }

    private static void CopyDirectory(string sourcePath, string destinationPath)
    {
        Directory.CreateDirectory(destinationPath);
        foreach (string directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(directory.Replace(sourcePath, destinationPath));
        }

        foreach (string file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (string.Equals(Path.GetExtension(file), ".meta", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string destinationFile = file.Replace(sourcePath, destinationPath);
            string destinationDirectory = Path.GetDirectoryName(destinationFile);
            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Copy(file, destinationFile, true);
        }
    }

    private static string CombineStreamingPath(params string[] parts)
    {
        string path = Application.streamingAssetsPath.TrimEnd('/', '\\');
        foreach (string part in parts)
        {
            path += "/" + part.Trim('/', '\\');
        }
        return path;
    }

    private static string AppendStreamingPath(string root, string relativeFile)
    {
        return root.TrimEnd('/', '\\') + "/" + relativeFile.TrimStart('/', '\\');
    }

    private static async Task<byte[]> ReadBytesAsync(string path)
    {
        if (path.Contains("://") || path.StartsWith("jar:", StringComparison.OrdinalIgnoreCase))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                operation.completed += _ => taskCompletionSource.TrySetResult(true);
                await taskCompletionSource.Task;

#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isHttpError || request.isNetworkError)
#endif
                {
                    return null;
                }

                return request.downloadHandler.data;
            }
        }

        if (!File.Exists(path))
        {
            return null;
        }

        return await Task.Run(() => File.ReadAllBytes(path));
    }

    private static async Task<string> ReadTextAsync(string path)
    {
        byte[] bytes = await ReadBytesAsync(path);
        return bytes == null ? string.Empty : System.Text.Encoding.UTF8.GetString(bytes);
    }

    private void Log(string message)
    {
        if (debugLogs)
        {
            Debug.Log("[VOSK-STT] " + message);
        }
    }

    private void LogError(string message)
    {
        Debug.LogError("[VOSK-STT] Error: " + message);
    }

    private void OnDestroy()
    {
        if (cachedModel != null)
        {
            cachedModel.Dispose();
            cachedModel = null;
        }
    }

    [Serializable]
    private class VoskRecognitionResult
    {
        public string text;
    }
}
