#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Editor-only controlled provider for the hardware-free runtime soak fixture.
public sealed class Level1SoakSpeechProvider : MonoBehaviour, ISpeechToTextProvider
{
    private readonly Queue<TaskCompletionSource<string>> pending = new Queue<TaskCompletionSource<string>>();
    public int PendingCount => pending.Count;

    public Task<string> Transcribe(AudioClip clip)
    {
        var completion = new TaskCompletionSource<string>();
        pending.Enqueue(completion);
        return completion.Task;
    }

    public void CompleteNext(string transcript)
    {
        if (pending.Count == 0) throw new InvalidOperationException("No simulated recognition is pending.");
        pending.Dequeue().SetResult(transcript);
    }

    public void FailNext()
    {
        if (pending.Count == 0) throw new InvalidOperationException("No simulated recognition is pending.");
        pending.Dequeue().SetCanceled();
    }
}
#endif
