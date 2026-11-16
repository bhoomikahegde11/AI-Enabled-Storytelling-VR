using System;
using System.Collections;
using UnityEngine;

// Selectively reused from feature/prerecorded-npc-voices: character banks,
// AudioSource playback and start/failure events. No intent guessing or random clips.
[RequireComponent(typeof(AudioSource))]
public sealed class PreRecordedNpcTtsProvider : MonoBehaviour, INpcTtsProvider, ICharacterNpcTtsProvider,
    INpcTtsPlaybackAware, INpcTtsCancellable, IPrerecordedNpcVoiceProvider
{
    [Serializable] public sealed class CharacterVoiceBank
    {
        public string characterId;
        public TextAsset mapping;
        public DialogueVoiceDatabase clips;
        [Tooltip("Temporarily adjust the shared AudioSource only while this character speaks. Original settings are restored afterwards.")]
        public bool overrideAudibility;
        [Range(0f, 1f)] public float voiceVolume = 1f;
        [Min(0.01f), Tooltip("Full-volume radius in metres for the existing 3D distance rolloff. Does not move the source or change spatial blend.")]
        public float minimumVoiceDistance = 4f;
        [NonSerialized] internal NpcVoicePlanner planner;
    }
    public AudioSource audioSource;
    public CharacterVoiceBank[] characterBanks;
    public event Action PlaybackStarted;
    public event Action<string> PlaybackFailed;
    public bool IsBusy { get; private set; }
    private Coroutine activePlaybackCoroutine;
    private AudioClip assembledClip;
    private int generation;
    private AudioSource adjustedSource;
    private float originalVolume;
    private float originalMinDistance;

    public bool HandlesCharacter(string characterId) => FindBank(characterId) != null;
    private CharacterVoiceBank FindBank(string characterId)
    {
        if (characterBanks != null) foreach (var bank in characterBanks)
            if (bank != null && string.Equals(bank.characterId, characterId, StringComparison.Ordinal)) return bank;
        return null;
    }
    public bool TryPrepare(NpcVoicePresentation presentation, out NpcVoicePlan plan, out string error)
    {
        plan = null; error = null;
        try
        {
            var bank = FindBank(presentation.CharacterId);
            if (bank == null || bank.mapping == null || bank.clips == null) throw new ArgumentException("Voice bank is not configured");
            if (bank.planner == null) bank.planner = new NpcVoicePlanner(JsonUtility.FromJson<NpcVoiceBankData>(bank.mapping.text));
            if (!bank.planner.TryPlan(presentation, out plan, out error)) return false;
            // All-or-nothing: never speak a partial price or unrelated recovery recording.
            foreach (var id in plan.ClipIds) if (bank.clips.GetAudioClip(id) == null) throw new ArgumentException("Missing voice clip " + id);
            return true;
        }
        catch (Exception ex) { error = ex.Message; plan = null; return false; }
    }
    public void Speak(string text) => RejectUnboundText();
    public void Speak(string text, string characterId) => RejectUnboundText();
    private void RejectUnboundText() { StopSpeaking(); PlaybackFailed?.Invoke("Prerecorded dialogue requires the exact selected template and values"); }

    public void Speak(NpcVoicePlan plan)
    {
        StopSpeaking();
        if (plan == null || !HandlesCharacter(plan.CharacterId)) { PlaybackFailed?.Invoke("Invalid prerecorded presentation"); return; }
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { PlaybackFailed?.Invoke("No AudioSource"); return; }
        IsBusy = true;
        activePlaybackCoroutine = StartCoroutine(PauseMenuController.RunGameplay(PlaySequenceCoroutine(plan, generation)));
    }
    private IEnumerator PlaySequenceCoroutine(NpcVoicePlan plan, int ticket)
    {
        var bank = FindBank(plan.CharacterId);
        var clips = new AudioClip[plan.ClipIds.Count];
        long totalSamples = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            var clip = bank.clips.GetAudioClip(plan.ClipIds[i]); clips[i] = clip;
            if (clip == null) { Fail("Missing recording " + plan.ClipIds[i], ticket); yield break; }
            if (clip.loadState == AudioDataLoadState.Unloaded && !clip.LoadAudioData()) { Fail("Cannot load recording " + clip.name, ticket); yield break; }
            while (clip.loadState == AudioDataLoadState.Loading) yield return null;
            if (ticket != generation) yield break;
            if (clip.loadState != AudioDataLoadState.Loaded || clip.channels != 1 || clip.frequency != plan.SampleRate)
            { Fail("Expected loaded mono recording at the bank's native sample rate: " + clip.name, ticket); yield break; }
            totalSamples += clip.samples;
            if (i < plan.GapSeconds.Count) totalSamples += Mathf.RoundToInt(plan.SampleRate * plan.GapSeconds[i]);
        }
        if (totalSamples <= 0 || totalSamples > int.MaxValue) { Fail("Invalid assembled audio length", ticket); yield break; }
        if (ticket != generation) yield break;
        if (clips.Length == 1) audioSource.clip = clips[0];
        else
        {
            var pieces = new float[clips.Length][];
            for (int i = 0; i < clips.Length; i++)
            {
                var samples = new float[clips[i].samples];
                if (!clips[i].GetData(samples, 0)) { Fail("Recording must be imported as Decompress On Load: " + clips[i].name, ticket); yield break; }
                pieces[i] = samples;
            }
            var pcm = plan.AssembleSamples(pieces);
            assembledClip = AudioClip.Create("NPC_" + plan.DialogueId, pcm.Length, 1, plan.SampleRate, false);
            if (!assembledClip.SetData(pcm, 0)) { Fail("Cannot create assembled NPC audio", ticket); yield break; }
            audioSource.clip = assembledClip;
            pieces = null; pcm = null;
        }
        ApplyVoiceAudibility(bank);
        audioSource.Play();
        Debug.Log($"[NPC VOICE] character={plan.CharacterId} dialogue={plan.DialogueId} session={plan.InteractionId} turn={plan.TurnId} clips={clips.Length}");
        PlaybackStarted?.Invoke();
        yield return null;
        while (ticket == generation && audioSource != null && audioSource.isPlaying) yield return null;
        if (ticket == generation) { IsBusy = false; activePlaybackCoroutine = null; ReleaseAssembly(); RestoreVoiceAudibility(); }
    }
    private void ApplyVoiceAudibility(CharacterVoiceBank bank)
    {
        if (!bank.overrideAudibility || audioSource == null) return;
        // This is a shared source. Never leave Father's distance/volume on the next NPC.
        RestoreVoiceAudibility();
        adjustedSource = audioSource;
        originalVolume = audioSource.volume;
        originalMinDistance = audioSource.minDistance;
        audioSource.volume = Mathf.Clamp01(bank.voiceVolume);
        audioSource.minDistance = Mathf.Clamp(bank.minimumVoiceDistance, 0.01f, Mathf.Max(0.01f, audioSource.maxDistance));
        Debug.Log($"[NPC VOICE LEVEL] character={bank.characterId} volume={originalVolume:0.00}->{audioSource.volume:0.00} minDistance={originalMinDistance:0.00}->{audioSource.minDistance:0.00}m spatialBlend={audioSource.spatialBlend:0.00} rolloff={audioSource.rolloffMode}");
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        var listener = FindFirstObjectByType<AudioListener>();
        if (listener != null)
            Debug.Log($"[NPC VOICE DISTANCE] character={bank.characterId} listenerDistance={Vector3.Distance(audioSource.transform.position, listener.transform.position):0.00}m");
        #endif
    }
    private void RestoreVoiceAudibility()
    {
        if (adjustedSource != null)
        {
            adjustedSource.volume = originalVolume;
            adjustedSource.minDistance = originalMinDistance;
        }
        adjustedSource = null;
    }
    private void Fail(string reason, int ticket)
    {
        if (ticket != generation) return;
        IsBusy = false; activePlaybackCoroutine = null; ReleaseAssembly(); RestoreVoiceAudibility();
        Debug.LogWarning("[NPC VOICE] " + reason); PlaybackFailed?.Invoke(reason);
    }
    public void StopSpeaking()
    {
        generation++;
        if (activePlaybackCoroutine != null) StopCoroutine(activePlaybackCoroutine);
        activePlaybackCoroutine = null;
        if (IsBusy && audioSource != null) audioSource.Stop();
        IsBusy = false; ReleaseAssembly(); RestoreVoiceAudibility();
    }
    private void ReleaseAssembly()
    {
        if (assembledClip == null) return;
        if (audioSource != null && audioSource.clip == assembledClip) audioSource.clip = null;
        Destroy(assembledClip); assembledClip = null;
    }
    private void OnDisable() => StopSpeaking();
    private void OnDestroy() => StopSpeaking();
}
