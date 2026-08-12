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

        if (localNpcTtsProvider == null)
        {
            localNpcTtsProvider = GetComponent<MonoBehaviour>();
            if (!(localNpcTtsProvider is INpcTtsProvider))
            {
                MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    if (behaviours[i] is INpcTtsProvider)
                    {
                        localNpcTtsProvider = behaviours[i];
                        break;
                    }
                }
            }

            if (localNpcTtsProvider == null || !(localNpcTtsProvider is INpcTtsProvider))
            {
                MonoBehaviour[] sceneBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
                for (int i = 0; i < sceneBehaviours.Length; i++)
                {
                    if (sceneBehaviours[i] is INpcTtsProvider)
                    {
                        localNpcTtsProvider = sceneBehaviours[i];
                        break;
                    }
                }
            }
        }
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
        if (MarketplaceManager.CanDriveAnimator(animator))
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
        if (MarketplaceManager.CanDriveAnimator(animator))
        {
            animator.SetBool("isTalking", false);
            Debug.Log("[ANIM] Talking OFF");
        }
    }

    private Animator GetNPCAnimator()
    {
        MarketplaceManager mm = FindFirstObjectByType<MarketplaceManager>();
        if (mm != null)
        {
            return mm.GetActiveNpcAnimator();
        }
        return null;
    }
}
