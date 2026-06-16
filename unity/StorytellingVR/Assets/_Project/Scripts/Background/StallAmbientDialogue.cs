using System.Collections;
using UnityEngine;

public class StallAmbientDialogue : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] voiceLines;

    [Header("Timing")]
    public float minDelay = 8f;
    public float maxDelay = 20f;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(DialogueLoop());
    }

    IEnumerator DialogueLoop()
    {
        yield return new WaitForSeconds(
            Random.Range(2f, 8f)
        );

        while (true)
        {
            PlayRandomLine();

            yield return new WaitForSeconds(
                Random.Range(minDelay, maxDelay)
            );
        }
    }

    void PlayRandomLine()
    {
        if (voiceLines.Length == 0)
            return;

        if (audioSource.isPlaying)
            return;

        AudioClip clip = voiceLines[
            Random.Range(0, voiceLines.Length)
        ];

        audioSource.PlayOneShot(clip);
    }
}