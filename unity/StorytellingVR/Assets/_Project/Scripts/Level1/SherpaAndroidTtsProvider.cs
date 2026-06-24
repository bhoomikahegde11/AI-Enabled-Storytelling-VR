using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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

    [Header("Android Test Override")]
    public bool forceSingleVoiceForAndroidTest = true;
    public string forcedAndroidTestVoiceFolderName = "official_hfc_female";

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

    private struct CopyResult
    {
        public bool success;
        public string failureReason;
    }
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
            LogRuntimeVoiceValidation(runtimeVoiceFolderPath);
            yield break;
        }

        if (!EnsureAndroidContext())
        {
            LogFailure("EnsureVoicePrepared returned false: Unity Android context unavailable", null);
            NotifyPlaybackFailed("Unity Android context unavailable");
            yield break;
        }

        Directory.CreateDirectory(runtimeVoiceFolderPath);

        bool copySucceeded = false;
        string copyFailureReason = string.Empty;
        string sourceVoiceRoot = CombineAndroidAssetPath(voiceRootRelativePath, voiceFolderName);

        yield return CopyVoiceAssets(sourceVoiceRoot, runtimeVoiceFolderPath, result =>
        {
            copySucceeded = result.success;
            copyFailureReason = result.failureReason;
        });

        if (!copySucceeded)
        {
            LogFailure("voice asset copy failed: " + copyFailureReason, null);
            NotifyPlaybackFailed("voice asset copy failed");
            yield break;
        }

        runtimeVoiceFolderCache[voiceFolderName] = runtimeVoiceFolderPath;
        LogRuntimeVoiceValidation(runtimeVoiceFolderPath);
    }

    private bool EnsureOfflineTtsLoaded(string voiceFolderName, string runtimeVoiceFolderPath, string tokensPath, string dataDirPath)
    {
        if (offlineTts != null && loadedVoiceFolderName == voiceFolderName)
        {
            return true;
        }

        ReleaseOfflineTts();

        string modelPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");

        if (!LogVoicePreflight(voiceFolderName, modelPath, tokensPath, dataDirPath))
        {
            NotifyPlaybackFailed("voice preflight failed");
            return false;
        }

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
            vitsConfig.Call("setDictDir", string.Empty);
            vitsConfig.Call("setNoiseScale", 0.667f);
            vitsConfig.Call("setNoiseScaleW", 0.8f);
            vitsConfig.Call("setLengthScale", 1.0f);

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
            LogInfo("Using file-path based Sherpa init (newFromFile equivalent) with persistentDataPath assets only");
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

    private IEnumerator CopyVoiceAssets(string sourceVoiceRoot, string runtimeVoiceFolderPath, Action<CopyResult> onComplete)
    {
        string modelRelativePath = CombineAndroidAssetPath(sourceVoiceRoot, "model.onnx");
        string tokensRelativePath = CombineAndroidAssetPath(sourceVoiceRoot, "tokens.txt");
        string espeakRelativePath = CombineAndroidAssetPath(sourceVoiceRoot, "espeak-ng-data");

        string modelDestinationPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");
        string tokensDestinationPath = Path.Combine(runtimeVoiceFolderPath, "tokens.txt");
        string espeakDestinationPath = Path.Combine(runtimeVoiceFolderPath, "espeak-ng-data");

        bool stepSuccess = false;
        string stepFailure = string.Empty;

        yield return CopyAssetFileUnityWebRequest(modelRelativePath, modelDestinationPath, result =>
        {
            stepSuccess = result.success;
            stepFailure = result.failureReason;
        });
        if (!stepSuccess)
        {
            onComplete?.Invoke(new CopyResult { success = false, failureReason = stepFailure });
            yield break;
        }

        yield return CopyAssetFileUnityWebRequest(tokensRelativePath, tokensDestinationPath, result =>
        {
            stepSuccess = result.success;
            stepFailure = result.failureReason;
        });
        if (!stepSuccess)
        {
            onComplete?.Invoke(new CopyResult { success = false, failureReason = stepFailure });
            yield break;
        }

        yield return CopyAssetDirectoryRecursiveUnityWebRequest(espeakRelativePath, espeakDestinationPath, result =>
        {
            stepSuccess = result.success;
            stepFailure = result.failureReason;
        });
        if (!stepSuccess)
        {
            onComplete?.Invoke(new CopyResult { success = false, failureReason = stepFailure });
            yield break;
        }

        onComplete?.Invoke(new CopyResult { success = true, failureReason = string.Empty });
    }

    private IEnumerator CopyAssetDirectoryRecursiveUnityWebRequest(string assetRelativePath, string destinationPath, Action<CopyResult> onComplete)
    {
        string[] entries = null;
        try
        {
            entries = assetManager.Call<string[]>("list", assetRelativePath);
        }
        catch (Exception ex)
        {
            onComplete?.Invoke(new CopyResult
            {
                success = false,
                failureReason = "assetManager.list failed for " + assetRelativePath + ": " + ex.Message
            });
            yield break;
        }

        if (entries == null || entries.Length == 0)
        {
            yield return CopyAssetFileUnityWebRequest(assetRelativePath, destinationPath, onComplete);
            yield break;
        }

        Directory.CreateDirectory(destinationPath);

        for (int i = 0; i < entries.Length; i++)
        {
            string entryName = entries[i];
            string childAssetPath = CombineAndroidAssetPath(assetRelativePath, entryName);
            string childDestinationPath = Path.Combine(destinationPath, entryName);

            string[] childEntries = null;
            try
            {
                childEntries = assetManager.Call<string[]>("list", childAssetPath);
            }
            catch (Exception ex)
            {
                onComplete?.Invoke(new CopyResult
                {
                    success = false,
                    failureReason = "assetManager.list failed for " + childAssetPath + ": " + ex.Message
                });
                yield break;
            }

            bool childSuccess = false;
            string childFailure = string.Empty;

            if (childEntries == null || childEntries.Length == 0)
            {
                yield return CopyAssetFileUnityWebRequest(childAssetPath, childDestinationPath, result =>
                {
                    childSuccess = result.success;
                    childFailure = result.failureReason;
                });
            }
            else
            {
                yield return CopyAssetDirectoryRecursiveUnityWebRequest(childAssetPath, childDestinationPath, result =>
                {
                    childSuccess = result.success;
                    childFailure = result.failureReason;
                });
            }

            if (!childSuccess)
            {
                onComplete?.Invoke(new CopyResult { success = false, failureReason = childFailure });
                yield break;
            }
        }

        onComplete?.Invoke(new CopyResult { success = true, failureReason = string.Empty });
    }

    private IEnumerator CopyAssetFileUnityWebRequest(string assetRelativePath, string destinationPath, Action<CopyResult> onComplete)
    {
        string destinationDirectory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        string requestUrl = BuildStreamingAssetUrl(assetRelativePath);
        using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(new CopyResult
                {
                    success = false,
                    failureReason = "UnityWebRequest failed for " + requestUrl + ": " + request.error
                });
                yield break;
            }

            try
            {
                File.WriteAllBytes(destinationPath, request.downloadHandler.data);
            }
            catch (Exception ex)
            {
                onComplete?.Invoke(new CopyResult
                {
                    success = false,
                    failureReason = "Failed to write file " + destinationPath + ": " + ex.Message
                });
                yield break;
            }
        }

        onComplete?.Invoke(new CopyResult { success = true, failureReason = string.Empty });
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

    private void LogRuntimeVoiceValidation(string runtimeVoiceFolderPath)
    {
        string modelPath = Path.Combine(runtimeVoiceFolderPath, "model.onnx");
        string tokensPath = Path.Combine(runtimeVoiceFolderPath, "tokens.txt");
        string espeakPath = Path.Combine(runtimeVoiceFolderPath, "espeak-ng-data");

        LogInfo("Runtime voice path: " + runtimeVoiceFolderPath);
        LogInfo("model exists " + File.Exists(modelPath));
        LogInfo("tokens exists " + File.Exists(tokensPath));
        LogInfo("espeak exists " + Directory.Exists(espeakPath));
    }

    private bool LogVoicePreflight(
        string voiceFolderName,
        string persistentModelPath,
        string persistentTokensPath,
        string persistentDataDir)
    {
        try
        {
            LogInfo("Preflight character voice folder: " + voiceFolderName);
            LogInfo("Preflight mode: file-path only");
            LogInfo("Preflight persistent model path: " + persistentModelPath);
            LogInfo("Preflight persistent tokens path: " + persistentTokensPath);
            LogInfo("Preflight persistent data dir: " + persistentDataDir);

            bool modelExists = File.Exists(persistentModelPath);
            bool tokensExists = File.Exists(persistentTokensPath);
            bool dataDirExists = Directory.Exists(persistentDataDir);

            LogInfo("Preflight model exists: " + modelExists);
            LogInfo("Preflight tokens exists: " + tokensExists);
            LogInfo("Preflight data dir exists: " + dataDirExists);

            if (!modelExists)
            {
                LogFailure("Preflight failed: persistent model missing at " + persistentModelPath, null);
                return false;
            }

            if (!tokensExists)
            {
                LogFailure("Preflight failed: persistent tokens missing at " + persistentTokensPath, null);
                return false;
            }

            if (!dataDirExists)
            {
                LogFailure("Preflight failed: persistent data dir missing at " + persistentDataDir, null);
                return false;
            }

            long modelSize = new FileInfo(persistentModelPath).Length;
            long tokensSize = new FileInfo(persistentTokensPath).Length;
            LogInfo("Preflight model file size bytes: " + modelSize);
            LogInfo("Preflight tokens file size bytes: " + tokensSize);

            string[] tokenPreviewLines = File.ReadAllLines(persistentTokensPath);
            int tokenPreviewCount = Mathf.Min(8, tokenPreviewLines.Length);
            for (int i = 0; i < tokenPreviewCount; i++)
            {
                LogInfo("Preflight tokens[" + i + "]: " + tokenPreviewLines[i]);
            }

            string[] requiredEspeakFiles =
            {
                "phontab",
                "phonindex",
                "phondata",
                "intonations"
            };

            for (int i = 0; i < requiredEspeakFiles.Length; i++)
            {
                string requiredPath = Path.Combine(persistentDataDir, requiredEspeakFiles[i]);
                bool exists = File.Exists(requiredPath);
                LogInfo("Preflight espeak file exists? " + exists + " | " + requiredPath);

                if (!exists)
                {
                    LogFailure("Preflight failed: required espeak file missing at " + requiredPath, null);
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            LogFailure("Preflight failed with exception", ex);
            return false;
        }
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

    private string BuildStreamingAssetUrl(string assetRelativePath)
    {
        string root = Application.streamingAssetsPath.TrimEnd('/', '\\');
        string relative = assetRelativePath.TrimStart('/', '\\').Replace("\\", "/");
        return root + "/" + relative;
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
#if UNITY_ANDROID && !UNITY_EDITOR
        if (forceSingleVoiceForAndroidTest && !string.IsNullOrWhiteSpace(forcedAndroidTestVoiceFolderName))
        {
            LogInfo("Android test override active. Using forced voice folder: " + forcedAndroidTestVoiceFolderName);
            return forcedAndroidTestVoiceFolderName;
        }
#endif

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
