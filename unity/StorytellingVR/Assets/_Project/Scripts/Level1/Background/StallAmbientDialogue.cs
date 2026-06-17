using System.Collections;
using UnityEngine;

public class StallAmbientDialogue : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceLines;

    [Header("Timing")]
    [SerializeField] private float initialDelayMin = 2f;
    [SerializeField] private float initialDelayMax = 12f;
    [SerializeField] private float minDelay = 10f;
    [SerializeField] private float maxDelay = 25f;

    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;

    private Coroutine dialogueCoroutine;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (playOnStart)
            dialogueCoroutine = StartCoroutine(DialogueLoop());
    }

    private IEnumerator DialogueLoop()
    {
        yield return new WaitForSeconds(Random.Range(initialDelayMin, initialDelayMax));

        while (true)
        {
            PlayRandomLine();

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

    private void PlayRandomLine()
    {
        if (audioSource == null)
        {
            Debug.LogWarning($"{name}: No AudioSource assigned.");
            return;
        }

        if (voiceLines == null || voiceLines.Length == 0)
        {
            Debug.LogWarning($"{name}: No voice lines assigned.");
            return;
        }

        if (audioSource.isPlaying)
            return;

        AudioClip clip = voiceLines[Random.Range(0, voiceLines.Length)];

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        if (audioSource != null)
            audioSource.Stop();
    }

    public void StartDialogue()
    {
        if (dialogueCoroutine == null)
            dialogueCoroutine = StartCoroutine(DialogueLoop());
    }
}