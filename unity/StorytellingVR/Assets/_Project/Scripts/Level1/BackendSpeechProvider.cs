using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BackendSpeechProvider : ISpeechToTextProvider
{
    private readonly string serverUrl;

    public BackendSpeechProvider(string serverUrl)
    {
        this.serverUrl = serverUrl;
    }

    public Task<string> Transcribe(AudioClip clip)
    {
        var taskCompletionSource = new TaskCompletionSource<string>();

        if (clip == null)
        {
            taskCompletionSource.SetResult(string.Empty);
            return taskCompletionSource.Task;
        }

        byte[] wavBytes = EncodeWav(clip);
        if (wavBytes == null || wavBytes.Length == 0)
        {
            taskCompletionSource.SetResult(string.Empty);
            return taskCompletionSource.Task;
        }

        Debug.Log("[STT] Sending audio");
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("file", wavBytes, "voice.wav", "audio/wav"));

        UnityWebRequest request = UnityWebRequest.Post(serverUrl, formData);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        operation.completed += _ =>
        {
            stopwatch.Stop();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                STTResponse response = JsonUtility.FromJson<STTResponse>(jsonResponse);
                string transcript = (response != null && !string.IsNullOrEmpty(response.text)) ? response.text.Trim() : "";

                Debug.Log($"[STT] Transcript: {transcript}");
                Debug.Log($"[PERF STT] {stopwatch.ElapsedMilliseconds} ms");
                taskCompletionSource.TrySetResult(transcript);
            }
            else
            {
                Debug.LogError($"[BACKEND] Request failed.\nURL Attempted: {serverUrl}\nError: {request.error}");
                taskCompletionSource.TrySetResult(string.Empty);
            }

            request.Dispose();
        };

        return taskCompletionSource.Task;
    }

    private static byte[] EncodeWav(AudioClip clip)
    {
        int channels = clip.channels;
        int frequency = clip.frequency;
        int totalSamples = clip.samples * channels;
        if (totalSamples <= 0) return null;

        float[] samples = new float[totalSamples];
        clip.GetData(samples, 0);

        byte[] wavData = new byte[44 + totalSamples * 2];
        System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(wavData, 0);
        System.BitConverter.GetBytes(36 + totalSamples * 2).CopyTo(wavData, 4);
        System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(wavData, 8);
        System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(wavData, 12);
        System.BitConverter.GetBytes(16).CopyTo(wavData, 16);
        System.BitConverter.GetBytes((short)1).CopyTo(wavData, 20);
        System.BitConverter.GetBytes((short)channels).CopyTo(wavData, 22);
        System.BitConverter.GetBytes(frequency).CopyTo(wavData, 24);
        System.BitConverter.GetBytes(frequency * channels * 2).CopyTo(wavData, 28);
        System.BitConverter.GetBytes((short)(channels * 2)).CopyTo(wavData, 32);
        System.BitConverter.GetBytes((short)16).CopyTo(wavData, 34);
        System.Text.Encoding.ASCII.GetBytes("data").CopyTo(wavData, 36);
        System.BitConverter.GetBytes(totalSamples * 2).CopyTo(wavData, 40);

        int offset = 44;
        for (int i = 0; i < totalSamples; i++)
        {
            short value = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
            System.BitConverter.GetBytes(value).CopyTo(wavData, offset);
            offset += 2;
        }

        return wavData;
    }

    [System.Serializable]
    private class STTResponse
    {
        public string text;
    }
}
