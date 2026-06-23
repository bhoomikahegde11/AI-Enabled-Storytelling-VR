using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SherpaAndroidTtsProvider : MonoBehaviour, INpcTtsProvider, ICharacterNpcTtsProvider, INpcTtsPlaybackAware
{
    [Serializable]
    public class SherpaVoiceProfile
    {
        public string characterId;
        public string voiceFolderName;
        public string displayName;
    }

    [Header("Sherpa Android TTS")]
    public bool enableTts = true;
    public string voiceRootRelativePath = "Sherpa/voices";
    public string fallbackVoiceFolderName = "en_IN_female";
    public float speechSpeed = 1.0f;
    public int speakerId = 0;
    public int numThreads = 2;
    public bool debugLogs = true;
    public AudioSource audioSource;

    [Header("Character Voice Profiles")]
    public SherpaVoiceProfile[] voiceProfiles = new SherpaVoiceProfile[]
    {
        new SherpaVoiceProfile
        {
            characterId = "lakshmi_amma",
            voiceFolderName = "en_IN_female",
            displayName = "Local Indian Female"
        },
        new SherpaVoiceProfile
        {
            characterId = "chinnamma_naik",
            voiceFolderName = "en_IN_female",
            displayName = "Local Indian Female"
        },
        new SherpaVoiceProfile
        {
            characterId = "saraswati_chetti",
            voiceFolderName = "en_IN_female",
            displayName = "Local Indian Female"
        },
        new SherpaVoiceProfile
        {
            characterId = "francisco_de_almeida",
            voiceFolderName = "en_GB_male",
            displayName = "Foreign Male"
        },
        new SherpaVoiceProfile
        {
            characterId = "father_penteado",
            voiceFolderName = "en_GB_male",
            displayName = "Foreign Male"
        },
        new SherpaVoiceProfile
        {
            characterId = "abdul_rahman",
            voiceFolderName = "kusal_male",
            displayName = "Abdul / Fallback Male"
        }
    };

    public event Action PlaybackStarted;
    public event Action<string> PlaybackFailed;

    private Coroutine activeSpeakRoutine;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject assetManager;
    private AndroidJavaObject offlineTts;
    private string loadedVoiceFolderName = string.Empty;
    private readonly Dictionary<string, string> runtimeVoiceFolderCache = new Dictionary<string, string>();
#endif

    private void Awake()
    {
        EnsureAudioSource();
        LogProviderActive();
    }

    public void Speak(string text)
    {
        Speak(text, string.Empty);
    }

    public void Speak(string text, string characterId)
    {
        if (!enableTts)
        {
            LogFailure("Speak skipped: Sherpa Android TTS disabled", null);
            NotifyPlaybackFailed("Sherpa Android TTS disabled");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            LogFailure("Speak skipped: empty NPC reply", null);
            NotifyPlaybackFailed("empty NPC reply");
            return;
        }

        EnsureAudioSource();

        if (activeSpeakRoutine != null)
        {
            StopCoroutine(activeSpeakRoutine);
            activeSpeakRoutine = null;
        }

        activeSpeakRoutine = StartCoroutine(SpeakRoutine(text, characterId));
    }

    private IEnumerator SpeakRoutine(string text, string characterId)
    {
        string selectedVoiceFolderName = ResolveVoiceFolderName(characterId);
        string selectedVoiceDisplayName = ResolveVoiceDisplayName(characterId, selectedVoiceFolderName);

        LogInfo("Android provider active");
        LogInfo("Character: " + characterId);
        LogInfo("Selected voice: " + selectedVoiceDisplayName);
        LogInfo("Selected voice folder: " + selectedVoiceFolderName);

#if UNITY_ANDROID && !UNITY_EDITOR
        yield return EnsureVoicePrepared(selectedVoiceFolderName);
        if (!runtimeVoiceFolderCache.TryGetValue(selectedVoiceFolderName, out string runtimeVoiceFolderPath) ||
            string.IsNullOrWhiteSpace(runtimeVoiceFolderPath))
        {
            LogFailure("Speak returned false: runtime voice folder unavailable for " + selectedVoiceFolderName, null);
            NotifyPlaybackFailed("runtime voice folder unavailable");
            activeSpeakRoutine = null;
            yield break;
        }

        string modelPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");
        string tokensPath = Path.Combine(runtimeVoiceFolderPath, "tokens.txt");
        string espeakPath = Path.Combine(runtimeVoiceFolderPath, "espeak-ng-data");

        LogInfo("Persistent voice path: " + runtimeVoiceFolderPath);
        LogInfo("Model path exists? " + File.Exists(modelPath) + " | " + modelPath);
        LogInfo("Tokens path exists? " + File.Exists(tokensPath) + " | " + tokensPath);
        LogInfo("Espeak dir exists? " + Directory.Exists(espeakPath) + " | " + espeakPath);

        if (!File.Exists(modelPath))
        {
            LogFailure("Speak returned false: model missing at " + modelPath, null);
            NotifyPlaybackFailed("model missing");
            activeSpeakRoutine = null;
            yield break;
        }

        if (!File.Exists(tokensPath))
        {
            LogFailure("Speak returned false: tokens missing at " + tokensPath, null);
            NotifyPlaybackFailed("tokens missing");
            activeSpeakRoutine = null;
            yield break;
        }

        if (!Directory.Exists(espeakPath))
        {
            LogFailure("Speak returned false: espeak data missing at " + espeakPath, null);
            NotifyPlaybackFailed("espeak data missing");
            activeSpeakRoutine = null;
            yield break;
        }

        if (!EnsureOfflineTtsLoaded(selectedVoiceFolderName, runtimeVoiceFolderPath, tokensPath, espeakPath))
        {
            LogFailure("Speak returned false: EnsureOfflineTtsLoaded failed for " + selectedVoiceFolderName, null);
            activeSpeakRoutine = null;
            yield break;
        }

        LogInfo("Synthesis started");

        AndroidJavaObject generatedAudio = null;
        try
        {
            generatedAudio = offlineTts.Call<AndroidJavaObject>("generate", text, speakerId, speechSpeed);
        }
        catch (Exception ex)
        {
            LogFailure("Sherpa Android generate failed", ex);
            NotifyPlaybackFailed("Sherpa Android generate failed");
            activeSpeakRoutine = null;
            yield break;
        }

        if (generatedAudio == null)
        {
            LogFailure("Speak returned false: Sherpa Android returned null audio", null);
            NotifyPlaybackFailed("Sherpa Android returned null audio");
            activeSpeakRoutine = null;
            yield break;
        }

        float[] samples;
        int sampleRate;
        try
        {
            samples = generatedAudio.Call<float[]>("getSamples");
            sampleRate = generatedAudio.Call<int>("getSampleRate");
        }
        catch (Exception ex)
        {
            LogFailure("unable to read generated audio", ex);
            NotifyPlaybackFailed("unable to read generated audio");
            generatedAudio.Dispose();
            activeSpeakRoutine = null;
            yield break;
        }
        finally
        {
            generatedAudio.Dispose();
        }

        if (samples == null || samples.Length == 0 || sampleRate <= 0)
        {
            LogFailure("Speak returned false: generated audio empty or invalid. sampleCount=" +
                (samples == null ? 0 : samples.Length) + " sampleRate=" + sampleRate, null);
            NotifyPlaybackFailed("generated audio empty");
            activeSpeakRoutine = null;
            yield break;
        }

        AudioClip clip = AudioClip.Create(
            "SherpaAndroidTts_" + selectedVoiceFolderName,
            samples.Length,
            1,
            sampleRate,
            false);
        clip.SetData(samples, 0);

        float clipLengthSeconds = samples.Length / (float)sampleRate;
        LogInfo("Synthesis finished");
        LogInfo("AudioClip length: " + clipLengthSeconds.ToString("0.00") + "s");

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        PlaybackStarted?.Invoke();
#else
        LogFailure("Quest TTS unavailable: SherpaAndroidTtsProvider runs only on Android device builds.", null);
        NotifyPlaybackFailed("Sherpa Android TTS unavailable outside Android device builds");
        activeSpeakRoutine = null;
        yield break;
#endif

        activeSpeakRoutine = null;
        yield break;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator EnsureVoicePrepared(string voiceFolderName)
    {
        string runtimeVoiceFolderPath = Path.Combine(Application.persistentDataPath, "Sherpa", "voices", voiceFolderName);

        if (ValidateRuntimeVoiceFolder(runtimeVoiceFolderPath))
        {
            runtimeVoiceFolderCache[voiceFolderName] = runtimeVoiceFolderPath;
            yield break;
        }

        if (!EnsureAndroidContext())
        {
            NotifyPlaybackFailed("Unity Android context unavailable");
            yield break;
        }

        Directory.CreateDirectory(runtimeVoiceFolderPath);

        string sourceRoot = GetAndroidStreamingAssetsRelativeRoot();
        if (string.IsNullOrWhiteSpace(sourceRoot))
        {
            NotifyPlaybackFailed("unable to resolve Android StreamingAssets root");
            yield break;
        }

        string sourceVoiceRoot = CombineAndroidAssetPath(sourceRoot, voiceRootRelativePath);
        sourceVoiceRoot = CombineAndroidAssetPath(sourceVoiceRoot, voiceFolderName);

        bool copySucceeded = false;
        string copyFailureReason = string.Empty;

        try
        {
            CopyAssetDirectoryRecursive(sourceVoiceRoot, runtimeVoiceFolderPath);
            copySucceeded = true;
        }
        catch (Exception ex)
        {
            copyFailureReason = ex.Message;
        }

        yield return null;

        if (!copySucceeded)
        {
            LogFailure("voice asset copy failed: " + copyFailureReason, null);
            NotifyPlaybackFailed("voice asset copy failed");
            yield break;
        }

        runtimeVoiceFolderCache[voiceFolderName] = runtimeVoiceFolderPath;
    }

    private bool EnsureOfflineTtsLoaded(string voiceFolderName, string runtimeVoiceFolderPath, string tokensPath, string dataDirPath)
    {
        if (offlineTts != null && loadedVoiceFolderName == voiceFolderName)
        {
            return true;
        }

        ReleaseOfflineTts();

        string modelPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");

        try
        {
            const string vitsConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsVitsModelConfig";
            const string modelConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsModelConfig";
            const string configClass = "com.k2fsa.sherpa.onnx.OfflineTtsConfig";
            const string offlineTtsClass = "com.k2fsa.sherpa.onnx.OfflineTts";

            LogInfo("Loading AndroidJavaObject class: " + vitsConfigClass);
            AndroidJavaObject vitsConfig = new AndroidJavaObject(vitsConfigClass);
            vitsConfig.Call("setModel", modelPath);
            vitsConfig.Call("setTokens", tokensPath);
            vitsConfig.Call("setDataDir", dataDirPath);
            vitsConfig.Call("setLexicon", string.Empty);

            LogInfo("Loading AndroidJavaObject class: " + modelConfigClass);
            AndroidJavaObject modelConfig = new AndroidJavaObject(modelConfigClass);
            modelConfig.Call("setVits", vitsConfig);
            modelConfig.Call("setNumThreads", numThreads);
            modelConfig.Call("setDebug", debugLogs);
            modelConfig.Call("setProvider", "cpu");

            LogInfo("Loading AndroidJavaObject class: " + configClass);
            AndroidJavaObject config = new AndroidJavaObject(configClass);
            config.Call("setModel", modelConfig);
            config.Call("setMaxNumSentences", 1);
            config.Call("setSilenceScale", 0.2f);

            LogInfo("Loading AndroidJavaObject class: " + offlineTtsClass);
            offlineTts = new AndroidJavaObject(offlineTtsClass, null, config);
            loadedVoiceFolderName = voiceFolderName;
            return true;
        }
        catch (Exception ex)
        {
            LogFailure("Sherpa Android init failed", ex);
            NotifyPlaybackFailed("Sherpa Android init failed");
            ReleaseOfflineTts();
            return false;
        }
    }

    private void CopyAssetDirectoryRecursive(string assetRelativePath, string destinationPath)
    {
        string[] entries = assetManager.Call<string[]>("list", assetRelativePath);
        if (entries == null || entries.Length == 0)
        {
            CopyAssetFile(assetRelativePath, destinationPath);
            return;
        }

        Directory.CreateDirectory(destinationPath);

        for (int i = 0; i < entries.Length; i++)
        {
            string entryName = entries[i];
            string childAssetPath = CombineAndroidAssetPath(assetRelativePath, entryName);
            string childDestinationPath = Path.Combine(destinationPath, entryName);
            string[] childEntries = assetManager.Call<string[]>("list", childAssetPath);

            if (childEntries == null || childEntries.Length == 0)
            {
                CopyAssetFile(childAssetPath, childDestinationPath);
            }
            else
            {
                CopyAssetDirectoryRecursive(childAssetPath, childDestinationPath);
            }
        }
    }

    private void CopyAssetFile(string assetRelativePath, string destinationPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

        using (AndroidJavaObject inputStream = assetManager.Call<AndroidJavaObject>("open", assetRelativePath))
        using (FileStream outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8192];
            while (true)
            {
                int bytesRead = inputStream.Call<int>("read", buffer);
                if (bytesRead <= 0)
                {
                    break;
                }

                outputStream.Write(buffer, 0, bytesRead);
            }

            inputStream.Call("close");
        }
    }

    private bool EnsureAndroidContext()
    {
        if (unityActivity != null && assetManager != null)
        {
            return true;
        }

        try
        {
            LogInfo("Loading AndroidJavaObject class: com.unity3d.player.UnityPlayer");
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            if (unityActivity == null)
            {
                LogFailure("EnsureAndroidContext returned false: UnityPlayer.currentActivity was null", null);
                return false;
            }

            assetManager = unityActivity.Call<AndroidJavaObject>("getAssets");
            if (assetManager == null)
            {
                LogFailure("EnsureAndroidContext returned false: activity.getAssets() returned null", null);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            LogFailure("unable to obtain Android asset manager", ex);
            return false;
        }
    }

    private bool ValidateRuntimeVoiceFolder(string runtimeVoiceFolderPath)
    {
        if (string.IsNullOrWhiteSpace(runtimeVoiceFolderPath))
        {
            return false;
        }

        string modelPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");
        string tokensPath = Path.Combine(runtimeVoiceFolderPath, "tokens.txt");
        string espeakPath = Path.Combine(runtimeVoiceFolderPath, "espeak-ng-data");
        return File.Exists(modelPath) && File.Exists(tokensPath) && Directory.Exists(espeakPath);
    }

    private string GetAndroidStreamingAssetsRelativeRoot()
    {
        string streamingAssetsPath = Application.streamingAssetsPath.Replace("\\", "/");
        const string assetsMarker = "!/assets/";
        int assetsMarkerIndex = streamingAssetsPath.IndexOf(assetsMarker, StringComparison.OrdinalIgnoreCase);
        if (assetsMarkerIndex >= 0)
        {
            return streamingAssetsPath.Substring(assetsMarkerIndex + assetsMarker.Length).Trim('/');
        }

        const string plainAssetsMarker = "/assets/";
        int plainIndex = streamingAssetsPath.IndexOf(plainAssetsMarker, StringComparison.OrdinalIgnoreCase);
        if (plainIndex >= 0)
        {
            return streamingAssetsPath.Substring(plainIndex + plainAssetsMarker.Length).Trim('/');
        }

        return string.Empty;
    }

    private static string CombineAndroidAssetPath(string left, string right)
    {
        if (string.IsNullOrEmpty(left))
        {
            return right.Trim('/');
        }

        if (string.IsNullOrEmpty(right))
        {
            return left.Trim('/');
        }

        return left.TrimEnd('/') + "/" + right.TrimStart('/');
    }

    private void ReleaseOfflineTts()
    {
        if (offlineTts == null)
        {
            loadedVoiceFolderName = string.Empty;
            return;
        }

        try
        {
            offlineTts.Call("release");
        }
        catch (Exception ex)
        {
            LogFailure("Sherpa Android release failed", ex);
        }
        finally
        {
            offlineTts.Dispose();
            offlineTts = null;
            loadedVoiceFolderName = string.Empty;
        }
    }
#endif

    private string ResolveVoiceFolderName(string characterId)
    {
        if (!string.IsNullOrWhiteSpace(characterId) && voiceProfiles != null)
        {
            for (int i = 0; i < voiceProfiles.Length; i++)
            {
                SherpaVoiceProfile profile = voiceProfiles[i];
                if (profile != null &&
                    string.Equals(profile.characterId, characterId, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(profile.voiceFolderName))
                {
                    return profile.voiceFolderName;
                }
            }
        }

        return fallbackVoiceFolderName;
    }

    private string ResolveVoiceDisplayName(string characterId, string fallbackFolderName)
    {
        if (!string.IsNullOrWhiteSpace(characterId) && voiceProfiles != null)
        {
            for (int i = 0; i < voiceProfiles.Length; i++)
            {
                SherpaVoiceProfile profile = voiceProfiles[i];
                if (profile != null &&
                    string.Equals(profile.characterId, characterId, StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(profile.displayName) ? profile.voiceFolderName : profile.displayName;
                }
            }
        }

        return fallbackFolderName;
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void NotifyPlaybackFailed(string reason)
    {
        LogWarning("PlaybackFailed: " + reason);
        PlaybackFailed?.Invoke(reason);
    }

    private void LogFailure(string reason, Exception ex)
    {
        if (ex != null)
        {
            Debug.LogError("[TTS_ANDROID] " + reason + "\n" + ex);
            return;
        }

        Debug.LogWarning("[TTS_ANDROID] " + reason);
    }

    private void LogInfo(string message)
    {
        Debug.Log("[TTS_ANDROID] " + message);
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning("[TTS_ANDROID] " + message);
    }

    private void LogProviderActive()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        LogInfo("Android provider active");
#else
        LogInfo("SherpaAndroidTtsProvider loaded outside Android device build");
#endif
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ReleaseOfflineTts();
#endif
    }
}
