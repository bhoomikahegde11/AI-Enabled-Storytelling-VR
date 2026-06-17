using System.Threading.Tasks;
using UnityEngine;

public interface ISpeechToTextProvider
{
    Task<string> Transcribe(AudioClip clip);
}
