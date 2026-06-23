using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(AudioSource))]
public class SherpaEditorTtsProvider : MonoBehaviour, INpcTtsProvider, ICharacterNpcTtsProvider, INpcTtsPlaybackAware
{
    [System.Serializable]
    public class SherpaVoiceProfile
    {
        public string characterId;
        public string voiceFolderName;
        public string displayName;
    }

    [Header("Sherpa Editor TTS")]
    [Tooltip("Optional Sherpa Exe Override")]
    public string sherpaExePath = "";
    public string voiceRootRelativePath = "Sherpa/voices";
    public string fallbackVoiceFolderName = "en_IN_female";
    [TextArea(2, 5)]
    public string testText = "Namaste traveler. Welcome to the Vijayanagara market. I have the finest spices for trade.";
    public bool debugSpeakNow = false;
    public AudioSource audioSource;

    public event System.Action PlaybackStarted;
    public event System.Action<string> PlaybackFailed;

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

    private Coroutine activeSpeakRoutine;

    private void Awake()
    {
        EnsureAudioSource();
        LogProviderDiagnostics();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (debugSpeakNow)
        {
            debugSpeakNow = false;
            Speak(testText);
        }
#endif
    }

    [ContextMenu("Speak Test Text")]
    public void SpeakTestText()
    {
        Speak(testText);
    }

    public void Speak(string text)
    {
        Speak(text, string.Empty);
    }

    public void Speak(string text, string characterId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.Log("[TTS] Skipped: empty NPC reply");
            NotifyPlaybackFailed("empty NPC reply");
            return;
        }

        SherpaVoiceProfile selectedProfile = GetVoiceProfile(characterId);
        string selectedVoiceFolderName = selectedProfile != null && !string.IsNullOrWhiteSpace(selectedProfile.voiceFolderName)
            ? selectedProfile.voiceFolderName
            : fallbackVoiceFolderName;
        string selectedVoice = selectedProfile != null && !string.IsNullOrWhiteSpace(selectedProfile.displayName)
            ? selectedProfile.displayName
            : "Fallback Voice";
        string selectedVoiceFolderPath = GetResolvedVoiceFolderPath(selectedVoiceFolderName);
        string selectedModelPath = GetResolvedModelPath(selectedVoiceFolderName);
        string selectedTokensPath = GetResolvedTokensPath(selectedVoiceFolderName);
        string selectedDataDirPath = GetResolvedDataDirPath(selectedVoiceFolderName);

        LogVoiceDiagnostics(characterId, selectedVoice, selectedVoiceFolderPath, selectedModelPath, selectedTokensPath, selectedDataDirPath);

#if UNITY_EDITOR
        EnsureAudioSource();

        if (activeSpeakRoutine != null)
        {
            StopCoroutine(activeSpeakRoutine);
            activeSpeakRoutine = null;
        }

        activeSpeakRoutine = StartCoroutine(SpeakRoutine(text, characterId, selectedVoice, selectedModelPath, selectedTokensPath, selectedDataDirPath));
#else
        Debug.LogWarning("[TTS] Quest TTS unavailable: SherpaEditorTtsProvider uses desktop executable only.");
        Debug.LogWarning("[TTS] Android runtime provider missing. Voice assets are present but no Android Sherpa native/plugin provider is implemented.");
        NotifyPlaybackFailed("Quest TTS unavailable: SherpaEditorTtsProvider uses desktop executable only.");
#endif
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private IEnumerator SpeakRoutine(string text, string characterId, string selectedVoice, string selectedModelPath, string selectedTokensPath, string selectedDataDirPath)
    {
        string resolvedSherpaExePath = ResolveSherpaExePath();
        if (!ValidatePaths(resolvedSherpaExePath, selectedModelPath, selectedTokensPath, selectedDataDirPath))
        {
            yield break;
        }

        string outputDirectory = Path.Combine(Application.temporaryCachePath, "SherpaTts");
        Directory.CreateDirectory(outputDirectory);

        string outputFileName = "sherpa_tts_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".wav";
        string outputWavPath = Path.Combine(outputDirectory, outputFileName);

        string arguments =
            "--vits-model=" + QuoteArgument(selectedModelPath) + " " +
            "--vits-tokens=" + QuoteArgument(selectedTokensPath) + " " +
            "--vits-data-dir=" + QuoteArgument(selectedDataDirPath) + " " +
            "--output-filename=" + QuoteArgument(outputWavPath) + " " +
            QuoteArgument(text);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = resolvedSherpaExePath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(resolvedSherpaExePath)
        };

        Debug.Log("[TTS] Sherpa command started: " + startInfo.FileName + " " + startInfo.Arguments);

        Process process = null;
        try
        {
            process = new Process { StartInfo = startInfo };
            process.Start();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TTS] Failed reason: unable to start Sherpa process. " + ex.Message);
            NotifyPlaybackFailed("unable to start Sherpa process");
            yield break;
        }

        while (!process.HasExited)
        {
            yield return null;
        }

        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();
        int exitCode = process.ExitCode;
        process.Dispose();

        Debug.Log("[TTS] Sherpa exit code: " + exitCode);
        if (!string.IsNullOrWhiteSpace(stdout))
        {
            Debug.Log("[TTS] Sherpa stdout:\n" + stdout);
        }
        if (!string.IsNullOrWhiteSpace(stderr))
        {
            Debug.Log("[TTS] Sherpa stderr:\n" + stderr);
        }

        Debug.Log("[TTS] Sherpa wav path: " + outputWavPath);

        if (exitCode != 0 && !File.Exists(outputWavPath))
        {
            Debug.LogWarning("[TTS] Failed reason: Sherpa exited with code " + exitCode + " and no WAV was produced");
            NotifyPlaybackFailed("Sherpa exited with code " + exitCode + " and no WAV was produced");
            yield break;
        }

        if (!File.Exists(outputWavPath))
        {
            Debug.LogWarning("[TTS] Failed reason: output WAV missing");
            NotifyPlaybackFailed("output WAV missing");
            yield break;
        }

        string wavUrl = "file:///" + outputWavPath.Replace("\\", "/");
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(wavUrl, AudioType.WAV))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[TTS] Failed reason: could not load generated WAV. " + request.error);
                NotifyPlaybackFailed("could not load generated WAV");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogWarning("[TTS] Failed reason: generated WAV loaded as null clip");
                NotifyPlaybackFailed("generated WAV loaded as null clip");
                yield break;
            }

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("[TTS] Audio playback started: " + outputWavPath);
            PlaybackStarted?.Invoke();
        }

        activeSpeakRoutine = null;
    }

    private bool ValidatePaths(string resolvedSherpaExePath, string selectedModelPath, string selectedTokensPath, string selectedDataDirPath)
    {
        if (string.IsNullOrWhiteSpace(resolvedSherpaExePath) || !File.Exists(resolvedSherpaExePath))
        {
            Debug.LogWarning("[TTS] Failed reason: sherpa exe not found");
            NotifyPlaybackFailed("sherpa exe not found");
            return false;
        }

        if (!File.Exists(selectedModelPath))
        {
            Debug.LogWarning("[TTS] Failed reason: model not found at " + selectedModelPath);
            NotifyPlaybackFailed("model not found");
            return false;
        }

        if (!File.Exists(selectedTokensPath))
        {
            Debug.LogWarning("[TTS] Failed reason: tokens file not found at " + selectedTokensPath);
            NotifyPlaybackFailed("tokens file not found");
            return false;
        }

        if (!Directory.Exists(selectedDataDirPath))
        {
            Debug.LogWarning("[TTS] Failed reason: data dir not found at " + selectedDataDirPath);
            NotifyPlaybackFailed("data dir not found");
            return false;
        }

        return true;
    }

    private string ResolveSherpaExePath()
    {
        if (!string.IsNullOrWhiteSpace(sherpaExePath))
        {
            string manualPath = Path.GetFullPath(sherpaExePath);
            Debug.Log("[TTS] Using manual override executable: " + manualPath);
            return manualPath;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string[] relativeSearchPaths =
        {
            "../../sherpa-onnx-win/sherpa-onnx-v1.13.0-win-x64-shared-MT-Release/bin/sherpa-onnx-offline-tts.exe",
            "../../../sherpa-onnx-win/sherpa-onnx-v1.13.0-win-x64-shared-MT-Release/bin/sherpa-onnx-offline-tts.exe",
            "tools/sherpa-onnx/bin/sherpa-onnx-offline-tts.exe",
            "../tools/sherpa-onnx/bin/sherpa-onnx-offline-tts.exe"
        };

        List<string> searchedPaths = new List<string>();
        for (int i = 0; i < relativeSearchPaths.Length; i++)
        {
            string candidatePath = Path.GetFullPath(Path.Combine(projectRoot, relativeSearchPaths[i]));
            searchedPaths.Add(candidatePath);
            if (File.Exists(candidatePath))
            {
                Debug.Log("[TTS] Auto-discovered executable: " + candidatePath);
                return candidatePath;
            }
        }

        Debug.LogWarning("[TTS] Executable not found. Searched paths:\n" + string.Join("\n", searchedPaths.ToArray()));
        return string.Empty;
    }

    private static string QuoteArgument(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "\"\"";
        }

        return "\"" + value.Replace("\"", "\\\"") + "\"";
    }
#endif

    private SherpaVoiceProfile GetVoiceProfile(string characterId)
    {
        if (voiceProfiles == null || voiceProfiles.Length == 0 || string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        for (int i = 0; i < voiceProfiles.Length; i++)
        {
            if (voiceProfiles[i] != null &&
                string.Equals(voiceProfiles[i].characterId, characterId, System.StringComparison.OrdinalIgnoreCase))
            {
                return voiceProfiles[i];
            }
        }

        return null;
    }

    private string GetResolvedModelPath(string voiceFolderName)
    {
        return Path.Combine(GetResolvedVoiceFolderPath(voiceFolderName), "model.onnx");
    }

    private string GetResolvedTokensPath(string voiceFolderName)
    {
        return Path.Combine(GetResolvedVoiceFolderPath(voiceFolderName), "tokens.txt");
    }

    private string GetResolvedDataDirPath(string voiceFolderName)
    {
        return Path.Combine(GetResolvedVoiceFolderPath(voiceFolderName), "espeak-ng-data");
    }

    private string GetResolvedVoiceFolderPath(string voiceFolderName)
    {
        return Path.Combine(Application.streamingAssetsPath, voiceRootRelativePath, voiceFolderName);
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
        PlaybackFailed?.Invoke(reason);
    }

    private void LogProviderDiagnostics()
    {
        Debug.Log("[TTS] Active provider: " + GetType().Name);
        Debug.Log("[TTS] Voice root path: " + Path.Combine(Application.streamingAssetsPath, voiceRootRelativePath));
#if UNITY_EDITOR
        Debug.Log("[TTS] Platform mode: Editor/Desktop Sherpa executable");
#else
        Debug.LogWarning("[TTS] Platform mode: Android/Quest. SherpaEditorTtsProvider cannot synthesize speech here because it depends on a desktop executable.");
#endif
    }

    private void LogVoiceDiagnostics(string characterId, string selectedVoice, string selectedVoiceFolderPath, string selectedModelPath, string selectedTokensPath, string selectedDataDirPath)
    {
        Debug.Log("[TTS] Character: " + characterId);
        Debug.Log("[TTS] Selected voice: " + selectedVoice);
        Debug.Log("[TTS] Voice folder path: " + selectedVoiceFolderPath);
        Debug.Log("[TTS] Model path: " + selectedModelPath);
        Debug.Log("[TTS] Tokens path: " + selectedTokensPath);
        Debug.Log("[TTS] Espeak data path: " + selectedDataDirPath);
        Debug.Log("[TTS] Model exists: " + File.Exists(selectedModelPath));
        Debug.Log("[TTS] Tokens exist: " + File.Exists(selectedTokensPath));
        Debug.Log("[TTS] Espeak data exists: " + Directory.Exists(selectedDataDirPath));
    }
}
