using System;
using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Level1/Sherpa Android TTS Smoke Test")]
public class SherpaAndroidTtsSmokeTest : MonoBehaviour
{
    private const string DemoModelDir = "Sherpa/android_demo/vits-piper-en_US-hfc_female-medium";
    private const string DemoModelFileName = "en_US-hfc_female-medium.onnx";
    private const string TestText = "hello";
    private const int SpeakerId = 0;
    private const float SpeechSpeed = 1.0f;
    private const int NumThreads = 2;
    private const float NoiseScale = 0.667f;
    private const float NoiseScaleW = 0.8f;
    private const float LengthScale = 1.0f;
    private const float SilenceScale = 0.2f;
    private const string LogPrefix = "[TTS_SMOKE] ";

    private AudioSource audioSource;

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject assetManager;
    private AndroidJavaObject offlineTts;
#endif

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();
    }

    private void Start()
    {
        StartCoroutine(RunSmokeTest());
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ReleaseOfflineTts();
#endif
    }

    private void ConfigureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    private IEnumerator RunSmokeTest()
    {
        Debug.Log(LogPrefix + "Smoke test starting");

#if UNITY_ANDROID && !UNITY_EDITOR
        if (!EnsureAndroidContext())
        {
            Debug.LogError(LogPrefix + "Android context unavailable");
            yield break;
        }

        if (!ValidateAssetModelLayout())
        {
            Debug.LogError(LogPrefix + "Smoke test aborted because required asset paths are missing");
            yield break;
        }

        if (!CreateOfflineTtsFromAssets())
        {
            yield break;
        }

        AndroidJavaObject generatedAudio = null;
        try
        {
            Debug.Log(LogPrefix + "Synthesizing text: " + TestText);
            generatedAudio = offlineTts.Call<AndroidJavaObject>("generate", TestText, SpeakerId, SpeechSpeed);
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Generate failed\n" + ex);
            yield break;
        }

        if (generatedAudio == null)
        {
            Debug.LogError(LogPrefix + "Generate returned null audio");
            yield break;
        }

        float[] samples = null;
        int sampleRate = 0;
        try
        {
            samples = generatedAudio.Call<float[]>("getSamples");
            sampleRate = generatedAudio.Call<int>("getSampleRate");
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Failed to read generated audio\n" + ex);
            generatedAudio.Dispose();
            yield break;
        }
        finally
        {
            generatedAudio.Dispose();
        }

        if (samples == null || samples.Length == 0 || sampleRate <= 0)
        {
            Debug.LogError(LogPrefix + "Generated audio invalid. Samples=" +
                (samples == null ? 0 : samples.Length) + " sampleRate=" + sampleRate);
            yield break;
        }

        AudioClip clip = AudioClip.Create("SherpaSmokeHello", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();

        Debug.Log(LogPrefix + "Playback started. SampleRate=" + sampleRate +
            " clipLength=" + (samples.Length / (float)sampleRate).ToString("0.00") + "s");
#else
        Debug.LogWarning(LogPrefix + "Smoke test is Android-only. Build and run this scene on Quest.");
#endif

        yield break;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private bool CreateOfflineTtsFromAssets()
    {
        ReleaseOfflineTts();

        try
        {
            const string vitsConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsVitsModelConfig";
            const string matchaConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsMatchaModelConfig";
            const string kokoroConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsKokoroModelConfig";
            const string zipVoiceConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsZipVoiceModelConfig";
            const string kittenConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsKittenModelConfig";
            const string pocketConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsPocketModelConfig";
            const string supertonicConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsSupertonicModelConfig";
            const string modelConfigClass = "com.k2fsa.sherpa.onnx.OfflineTtsModelConfig";
            const string configClass = "com.k2fsa.sherpa.onnx.OfflineTtsConfig";
            const string offlineTtsClass = "com.k2fsa.sherpa.onnx.OfflineTts";

            string modelAssetPath = CombineAssetPath(DemoModelDir, DemoModelFileName);
            string tokensAssetPath = CombineAssetPath(DemoModelDir, "tokens.txt");
            string dataDirAssetPath = CombineAssetPath(DemoModelDir, "espeak-ng-data");
            string dataDirRuntimePath = GetExternalFilesPath(dataDirAssetPath);

            Debug.Log(LogPrefix + "Creating exact official Android sample VITS config");
            Debug.Log(LogPrefix + "Model asset path: " + modelAssetPath);
            Debug.Log(LogPrefix + "Tokens asset path: " + tokensAssetPath);
            Debug.Log(LogPrefix + "Data dir asset path: " + dataDirAssetPath);
            Debug.Log(LogPrefix + "Data dir runtime path: " + dataDirRuntimePath);

            if (!CopyAssetTreeToExternalFiles(dataDirAssetPath))
            {
                Debug.LogError(LogPrefix + "Failed to copy espeak-ng-data to external files");
                return false;
            }

            Debug.Log(LogPrefix + "Runtime data dir exists: " + Directory.Exists(dataDirRuntimePath));

            AndroidJavaObject vitsConfig = new AndroidJavaObject(
                vitsConfigClass,
                modelAssetPath,
                string.Empty,
                tokensAssetPath,
                dataDirRuntimePath,
                string.Empty,
                NoiseScale,
                NoiseScaleW,
                LengthScale);

            AndroidJavaObject matchaConfig = new AndroidJavaObject(matchaConfigClass);
            AndroidJavaObject kokoroConfig = new AndroidJavaObject(kokoroConfigClass);
            AndroidJavaObject zipVoiceConfig = new AndroidJavaObject(zipVoiceConfigClass);
            AndroidJavaObject kittenConfig = new AndroidJavaObject(kittenConfigClass);
            AndroidJavaObject pocketConfig = new AndroidJavaObject(pocketConfigClass);
            AndroidJavaObject supertonicConfig = new AndroidJavaObject(supertonicConfigClass);

            AndroidJavaObject modelConfig = new AndroidJavaObject(
                modelConfigClass,
                vitsConfig,
                matchaConfig,
                kokoroConfig,
                zipVoiceConfig,
                kittenConfig,
                pocketConfig,
                supertonicConfig,
                NumThreads,
                false,
                "cpu");

            AndroidJavaObject config = new AndroidJavaObject(
                configClass,
                modelConfig,
                string.Empty,
                string.Empty,
                1,
                SilenceScale);

            offlineTts = new AndroidJavaObject(offlineTtsClass, assetManager, config);
            Debug.Log(LogPrefix + "OfflineTts initialized successfully using assetManager mode");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "OfflineTts init failed\n" + ex);
            ReleaseOfflineTts();
            return false;
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
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            if (unityActivity == null)
            {
                return false;
            }

            assetManager = unityActivity.Call<AndroidJavaObject>("getAssets");
            return assetManager != null;
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Failed to get Android context\n" + ex);
            return false;
        }
    }

    private bool ValidateAssetModelLayout()
    {
        string[] topLevelEntries;
        try
        {
            topLevelEntries = assetManager.Call<string[]>("list", DemoModelDir);
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "assetManager.list failed for " + DemoModelDir + "\n" + ex);
            return false;
        }

        if (topLevelEntries == null || topLevelEntries.Length == 0)
        {
            Debug.LogError(LogPrefix + "No assets found under " + DemoModelDir);
            return false;
        }

        bool hasModel = false;
        bool hasTokens = false;
        bool hasEspeak = false;

        for (int i = 0; i < topLevelEntries.Length; i++)
        {
            string entry = topLevelEntries[i];
            if (entry == DemoModelFileName)
            {
                hasModel = true;
            }
            else if (entry == "tokens.txt")
            {
                hasTokens = true;
            }
            else if (entry == "espeak-ng-data")
            {
                hasEspeak = true;
            }
        }

        Debug.Log(LogPrefix + "Asset model dir: " + DemoModelDir);
        Debug.Log(LogPrefix + "Asset model exists: " + hasModel);
        Debug.Log(LogPrefix + "Asset tokens exists: " + hasTokens);
        Debug.Log(LogPrefix + "Asset espeak dir exists: " + hasEspeak);

        if (!hasEspeak)
        {
            try
            {
                string[] espeakEntries = assetManager.Call<string[]>("list", CombineAssetPath(DemoModelDir, "espeak-ng-data"));
                hasEspeak = espeakEntries != null && espeakEntries.Length > 0;
                Debug.Log(LogPrefix + "Asset espeak nested entries: " + (espeakEntries == null ? 0 : espeakEntries.Length));
            }
            catch (Exception ex)
            {
                Debug.LogError(LogPrefix + "assetManager.list failed for espeak path\n" + ex);
            }
        }

        return hasModel && hasTokens && hasEspeak;
    }

    private string CombineAssetPath(string left, string right)
    {
        string normalizedLeft = (left ?? string.Empty).Replace("\\", "/").TrimEnd('/');
        string normalizedRight = (right ?? string.Empty).Replace("\\", "/").TrimStart('/');

        if (string.IsNullOrEmpty(normalizedLeft))
        {
            return normalizedRight;
        }

        if (string.IsNullOrEmpty(normalizedRight))
        {
            return normalizedLeft;
        }

        return normalizedLeft + "/" + normalizedRight;
    }

    private bool CopyAssetTreeToExternalFiles(string assetRelativePath)
    {
        try
        {
            string[] entries = assetManager.Call<string[]>("list", assetRelativePath);
            if (entries == null)
            {
                Debug.LogError(LogPrefix + "assetManager.list returned null for " + assetRelativePath);
                return false;
            }

            if (entries.Length == 0)
            {
                return CopyAssetFileToExternalFiles(assetRelativePath);
            }

            Directory.CreateDirectory(GetExternalFilesPath(assetRelativePath));

            for (int i = 0; i < entries.Length; i++)
            {
                string childAssetPath = CombineAssetPath(assetRelativePath, entries[i]);
                if (!CopyAssetTreeToExternalFiles(childAssetPath))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Failed to copy asset tree for " + assetRelativePath + "\n" + ex);
            return false;
        }
    }

    private bool CopyAssetFileToExternalFiles(string assetRelativePath)
    {
        AndroidJavaObject inputStream = null;
        FileStream outputStream = null;

        try
        {
            inputStream = assetManager.Call<AndroidJavaObject>("open", assetRelativePath);
            if (inputStream == null)
            {
                Debug.LogError(LogPrefix + "Asset open returned null for " + assetRelativePath);
                return false;
            }

            string targetPath = GetExternalFilesPath(assetRelativePath);
            string targetDirectory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            outputStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);

            while (true)
            {
                sbyte[] chunk = inputStream.Call<sbyte[]>("readNBytes", 4096);
                byte[] buffer = AndroidJavaBufferToManagedArray(chunk);
                if (buffer.Length == 0)
                {
                    break;
                }

                outputStream.Write(buffer, 0, buffer.Length);
            }

            Debug.Log(LogPrefix + "Copied asset file: " + assetRelativePath + " -> " + targetPath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(LogPrefix + "Failed to copy asset file " + assetRelativePath + "\n" + ex);
            return false;
        }
        finally
        {
            if (outputStream != null)
            {
                outputStream.Dispose();
            }

            if (inputStream != null)
            {
                try
                {
                    inputStream.Call("close");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(LogPrefix + "InputStream close warning for " + assetRelativePath + "\n" + ex);
                }

                inputStream.Dispose();
            }
        }
    }

    private string GetExternalFilesPath(string assetRelativePath)
    {
        string externalFilesRoot = unityActivity.Call<AndroidJavaObject>("getExternalFilesDir", (object)null)
            .Call<string>("getAbsolutePath");
        return CombineFileSystemPath(externalFilesRoot, assetRelativePath);
    }

    private static byte[] AndroidJavaBufferToManagedArray(sbyte[] source)
    {
        if (source == null || source.Length == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] result = new byte[source.Length];
        Buffer.BlockCopy(source, 0, result, 0, source.Length);
        return result;
    }

    private string CombineFileSystemPath(string left, string right)
    {
        string normalizedRight = (right ?? string.Empty).Replace("/", Path.DirectorySeparatorChar.ToString());
        return Path.Combine(left ?? string.Empty, normalizedRight);
    }

    private void ReleaseOfflineTts()
    {
        if (offlineTts == null)
        {
            return;
        }

        try
        {
            offlineTts.Call("release");
        }
        catch (Exception ex)
        {
            Debug.LogWarning(LogPrefix + "Release warning\n" + ex);
        }

        offlineTts.Dispose();
        offlineTts = null;
    }
#endif
}
