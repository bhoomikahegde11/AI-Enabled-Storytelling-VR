using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public UnityEngine.MonoBehaviour localNpcTtsProvider;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("[AudioManager] No AudioSource assigned! Falling back to self.");
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        localNpcTtsProvider = ResolveBestTtsProvider(localNpcTtsProvider);

        if (localNpcTtsProvider == null)
        {
            localNpcTtsProvider = ResolveBestTtsProvider(GetComponent<MonoBehaviour>());
        }

        if (localNpcTtsProvider == null)
        {
            Debug.LogWarning("[TTS] Active provider: none");
        }
        else
        {
            Debug.Log("[TTS] Active provider: " + localNpcTtsProvider.GetType().Name);
        }
    }

    private MonoBehaviour ResolveBestTtsProvider(MonoBehaviour currentProvider)
    {
        if (IsValidNpcTtsProvider(currentProvider) && !ShouldReplaceProviderForPlatform(currentProvider))
        {
            return currentProvider;
        }

        MonoBehaviour bestProvider = FindBestProvider(GetComponents<MonoBehaviour>());
        if (bestProvider != null)
        {
            return bestProvider;
        }

        MonoBehaviour[] sceneBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        return FindBestProvider(sceneBehaviours);
    }

    private static MonoBehaviour FindBestProvider(MonoBehaviour[] behaviours)
    {
        MonoBehaviour bestProvider = null;
        int bestScore = int.MinValue;

        for (int i = 0; i < behaviours.Length; i++)
        {
            MonoBehaviour candidate = behaviours[i];
            if (!IsValidNpcTtsProvider(candidate))
            {
                continue;
            }

            int score = ScoreProviderForPlatform(candidate);
            if (score > bestScore)
            {
                bestScore = score;
                bestProvider = candidate;
            }
        }

        return bestProvider;
    }

    private static bool IsValidNpcTtsProvider(MonoBehaviour behaviour)
    {
        return behaviour != null && behaviour is INpcTtsProvider;
    }

    private static bool ShouldReplaceProviderForPlatform(MonoBehaviour provider)
    {
        string typeName = provider.GetType().Name;
#if UNITY_ANDROID && !UNITY_EDITOR
        return typeName == "SherpaEditorTtsProvider";
#else
        return typeName == "SherpaAndroidTtsProvider";
#endif
    }

    private static int ScoreProviderForPlatform(MonoBehaviour provider)
    {
        string typeName = provider.GetType().Name;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (typeName == "SherpaAndroidTtsProvider")
        {
            return 100;
        }

        if (typeName == "AndroidNativeTtsProvider")
        {
            return 90;
        }

        if (typeName == "SherpaEditorTtsProvider")
        {
            return 10;
        }
#else
        if (typeName == "SherpaEditorTtsProvider")
        {
            return 100;
        }

        if (typeName == "SherpaAndroidTtsProvider")
        {
            return 10;
        }
#endif

        return 50;
    }

    /// <summary>
    /// Downloads and plays an audio file from the provided URL.
    /// </summary>
    /// <param name="url">The URL of the audio to download and play (e.g., mp3)</param>
    public void PlayAudioFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("[AudioManager] Provided URL is null or empty.");
            return;
        }

        // Clean up any previous active audio downloads or monitoring loops
        ResetTalkingParameter();
        StopAllCoroutines();

        StartCoroutine(DownloadAndPlayAudioRoutine(url));
    }

    public bool TrySpeakText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("[TTS] Skipped: empty NPC reply");
            return false;
        }

        if (localNpcTtsProvider == null)
        {
            Debug.LogWarning("[TTS] No local TTS provider assigned");
            return false;
        }

        INpcTtsProvider provider = localNpcTtsProvider as INpcTtsProvider;
        if (provider == null)
        {
            Debug.LogWarning("[TTS] Failed reason: assigned provider does not implement INpcTtsProvider");
            return false;
        }

        try
        {
            Debug.Log("[TTS] Attempting provider speak via " + localNpcTtsProvider.GetType().Name);
            provider.Speak(text);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TTS] Failed reason: " + ex.Message);
            return false;
        }
    }

    public bool TrySpeakText(string text, string characterId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("[TTS] Skipped: empty NPC reply");
            return false;
        }

        if (localNpcTtsProvider == null)
        {
            Debug.LogWarning("[TTS] No local TTS provider assigned");
            return false;
        }

        ICharacterNpcTtsProvider characterProvider = localNpcTtsProvider as ICharacterNpcTtsProvider;
        if (characterProvider != null)
        {
            try
            {
                Debug.Log("[TTS] Attempting character-aware provider speak via " + localNpcTtsProvider.GetType().Name);
                characterProvider.Speak(text, characterId);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[TTS] Failed reason: " + ex.Message);
                return false;
            }
        }

        return TrySpeakText(text);
    }

    private IEnumerator DownloadAndPlayAudioRoutine(string url)
    {
        AudioType audioType = AudioType.MPEG;
        if (url.ToLower().Contains(".wav"))
        {
            audioType = AudioType.WAV;
            Debug.Log("[AudioManager] Detected WAV format, using AudioType.WAV");
        }
        else
        {
            Debug.Log("[AudioManager] Detected MPEG format, using AudioType.MPEG");
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[AudioManager] Error downloading audio: {www.error} | URL: {url}");
                ResetTalkingParameter(); // Clean reset on request failures
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    StartCoroutine(MonitorAudioPlayback(clip.length));
                }
                else
                {
                    Debug.LogError("[AudioManager] Audio downloaded successfully, but the clip is null.");
                    ResetTalkingParameter();
                }
            }
        }
    }

    private IEnumerator MonitorAudioPlayback(float duration)
    {
        Animator animator = GetNPCAnimator();
        if (animator != null)
        {
            animator.SetBool("isTalking", true);
            Debug.Log("[ANIM] Talking ON");
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // If the audio source is stopped or paused early, exit out
            if (audioSource != null && !audioSource.isPlaying && elapsed > 0.5f)
            {
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        ResetTalkingParameter();
    }

    private void ResetTalkingParameter()
    {
        Animator animator = GetNPCAnimator();
        if (animator != null)
        {
            animator.SetBool("isTalking", false);
            Debug.Log("[ANIM] Talking OFF");
        }
    }

    private Animator GetNPCAnimator()
    {
        MarketplaceManager mm = FindFirstObjectByType<MarketplaceManager>();
        if (mm != null && mm.buyerNPC != null)
        {
            return mm.buyerNPC.GetComponent<Animator>() ?? mm.buyerNPC.GetComponentInChildren<Animator>();
        }
        return null;
    }
}
