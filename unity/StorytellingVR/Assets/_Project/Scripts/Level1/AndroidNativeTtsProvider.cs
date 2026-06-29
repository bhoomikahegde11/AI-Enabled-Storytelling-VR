using UnityEngine;

public class AndroidNativeTtsProvider : MonoBehaviour, INpcTtsProvider
{
    public bool enableTts = true;
    public float speechRate = 0.95f;
    public float pitch = 1.0f;
    public bool flushPrevious = true;

    private AndroidJavaObject textToSpeech;
    private AndroidJavaObject unityActivity;
    private TtsInitListener initListener;
    private bool isInitializing;
    private bool isReady;
    private string pendingText;

    public void Speak(string text)
    {
        if (!enableTts)
        {
            Debug.Log("[TTS] Skipped: Android native TTS disabled");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.Log("[TTS] Skipped: empty NPC reply");
            return;
        }

#if UNITY_EDITOR
        Debug.Log("[TTS] Android native TTS unavailable in editor");
        return;
#elif UNITY_ANDROID
        try
        {
            if (!EnsureInitialized())
            {
                pendingText = text;
                return;
            }

            SpeakInternal(text);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TTS] Android TTS failed reason: " + ex.Message);
        }
#else
        Debug.Log("[TTS] Skipped: Android native TTS only available on Android");
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private bool EnsureInitialized()
    {
        if (isReady && textToSpeech != null)
        {
            return true;
        }

        if (isInitializing)
        {
            return false;
        }

        Debug.Log("[TTS] Android TTS initializing");
        isInitializing = true;

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        if (unityActivity == null)
        {
            isInitializing = false;
            Debug.LogWarning("[TTS] Android TTS failed reason: Unity activity unavailable");
            return false;
        }

        initListener = new TtsInitListener(OnTtsInitialized);
        textToSpeech = new AndroidJavaObject("android.speech.tts.TextToSpeech", unityActivity, initListener);
        return false;
    }

    private void OnTtsInitialized(int status)
    {
        isInitializing = false;

        if (textToSpeech == null)
        {
            Debug.LogWarning("[TTS] Android TTS failed reason: TextToSpeech instance missing");
            return;
        }

        if (status != 0)
        {
            Debug.LogWarning("[TTS] Android TTS failed reason: init status " + status);
            return;
        }

        try
        {
            using (AndroidJavaClass localeClass = new AndroidJavaClass("java.util.Locale"))
            {
                AndroidJavaObject englishLocale = localeClass.GetStatic<AndroidJavaObject>("ENGLISH");
                textToSpeech.Call<int>("setLanguage", englishLocale);
            }

            textToSpeech.Call<int>("setSpeechRate", speechRate);
            textToSpeech.Call<int>("setPitch", pitch);
            isReady = true;
            Debug.Log("[TTS] Android TTS ready");

            if (!string.IsNullOrWhiteSpace(pendingText))
            {
                string text = pendingText;
                pendingText = null;
                SpeakInternal(text);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TTS] Android TTS failed reason: " + ex.Message);
        }
    }

    private void SpeakInternal(string text)
    {
        if (textToSpeech == null || !isReady)
        {
            pendingText = text;
            return;
        }

        int queueMode = flushPrevious ? 0 : 1;
        Debug.Log("[TTS] Android TTS speaking: " + text);
        textToSpeech.Call<int>("speak", text, queueMode, null, "level1_npc_reply");
    }

    private void OnDestroy()
    {
        try
        {
            if (textToSpeech != null)
            {
                textToSpeech.Call("stop");
                textToSpeech.Call("shutdown");
                textToSpeech.Dispose();
                textToSpeech = null;
                Debug.Log("[TTS] Android TTS shutdown");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[TTS] Android TTS failed reason: " + ex.Message);
        }
        finally
        {
            isReady = false;
            isInitializing = false;
            pendingText = null;
        }
    }

#endif

    private class TtsInitListener : AndroidJavaProxy
    {
        private readonly System.Action<int> callback;

        public TtsInitListener(System.Action<int> callback)
            : base("android.speech.tts.TextToSpeech$OnInitListener")
        {
            this.callback = callback;
        }

        public void onInit(int status)
        {
            callback?.Invoke(status);
        }
    }
}
