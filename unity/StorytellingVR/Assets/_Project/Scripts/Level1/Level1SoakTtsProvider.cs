#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;

// Editor-only delayed playback callbacks exercise real ChatManager presentation cleanup.
public sealed class Level1SoakTtsProvider : MonoBehaviour, INpcTtsProvider, INpcTtsPlaybackAware, INpcTtsCancellable
{
    public event Action PlaybackStarted;
    public event Action<string> PlaybackFailed;
    public int NextDelayFrames { get; set; } = 1;
    public int SpeakCount { get; private set; }
    public int StopCount { get; private set; }
    public bool HasPendingPlayback => pending != null;
    private Coroutine pending;

    public void Speak(string text)
    {
        SpeakCount++;
        if (pending != null) StopCoroutine(pending);
        pending = StartCoroutine(StartAfterFrames(Mathf.Max(1, NextDelayFrames)));
    }

    private IEnumerator StartAfterFrames(int frames)
    {
        for (int i = 0; i < frames; i++) yield return null;
        pending = null;
        PlaybackStarted?.Invoke();
    }

    public void StopSpeaking()
    {
        StopCount++;
        if (pending != null) StopCoroutine(pending);
        pending = null;
    }

    public void EmitLatePlaybackStarted() => PlaybackStarted?.Invoke();
    public void EmitLatePlaybackFailure() => PlaybackFailed?.Invoke("stale simulated playback");
}
#endif
