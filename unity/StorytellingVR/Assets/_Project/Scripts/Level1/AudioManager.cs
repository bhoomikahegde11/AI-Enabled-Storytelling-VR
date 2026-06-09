using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

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
