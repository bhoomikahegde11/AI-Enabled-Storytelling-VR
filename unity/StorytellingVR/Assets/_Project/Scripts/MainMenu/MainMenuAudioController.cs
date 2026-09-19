using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    public static MainMenuAudioController Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private bool playBgmOnAwake = true;

    [Header("UI SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] [Range(0f, 1f)] private float hoverVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float clickVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{nameof(MainMenuAudioController)}] Duplicate controller on '{gameObject.name}'. Destroying duplicate.");
            Destroy(this);
            return;
        }

        Instance = this;

        if (bgmSource == null)
        {
            bgmSource = GetComponent<AudioSource>();
        }

        EnsureSfxSource();
        ConfigureBgmSource();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayHover()
    {
        PlaySfx(hoverClip, hoverVolume, false);
    }

    public void PlayClick()
    {
        PlaySfx(clickClip, clickVolume, true);
    }

    private void ConfigureBgmSource()
    {
        if (bgmSource == null)
        {
            return;
        }

        if (backgroundMusicClip != null)
        {
            bgmSource.clip = backgroundMusicClip;
        }

        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;
        bgmSource.loop = true;

        if (playBgmOnAwake && bgmSource.clip != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    private void EnsureSfxSource()
    {
        if (sfxSource != null && sfxSource != bgmSource)
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;
            return;
        }

        GameObject sfxObject = new GameObject("MainMenu_SFX");
        sfxObject.transform.SetParent(transform, false);

        sfxSource = sfxObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
    }

    private void PlaySfx(AudioClip clip, float volumeScale, bool surviveSceneLoad)
    {
        if (clip == null)
        {
            return;
        }

        if (surviveSceneLoad)
        {
            PlayTransientClip(clip, volumeScale);
            return;
        }

        if (sfxSource == null)
        {
            EnsureSfxSource();
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    private static void PlayTransientClip(AudioClip clip, float volumeScale)
    {
        GameObject tempAudioObject = new GameObject("MainMenu_ClickSfx");
        DontDestroyOnLoad(tempAudioObject);

        AudioSource tempSource = tempAudioObject.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.loop = false;
        tempSource.spatialBlend = 0f;
        tempSource.clip = clip;
        tempSource.volume = Mathf.Clamp01(volumeScale);
        tempSource.Play();

        Destroy(tempAudioObject, clip.length + 0.1f);
    }
}
