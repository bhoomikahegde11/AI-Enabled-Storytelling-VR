using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PreRecordedNpcTtsProvider : MonoBehaviour, INpcTtsProvider, ICharacterNpcTtsProvider, INpcTtsPlaybackAware
{
    [Serializable]
    public class IntentClipSet
    {
        public string intent;
        public AudioClip[] clips;
    }

    [Serializable]
    public class VariableClip
    {
        public string key;
        public AudioClip clip;
    }

    [Serializable]
    public class CharacterVoiceProfile
    {
        public string characterId;
        public IntentClipSet[] fullClips;
        public IntentClipSet[] prefixClips;
        public IntentClipSet[] suffixClips;
        public VariableClip[] variableClips;
    }

    [Header("Pre-Recorded NPC TTS")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float clipGapSeconds = 0.03f;
    [SerializeField] private string defaultCharacterId = "test_merchant_voice";
    [SerializeField] private bool debugLogs = true;

    [Header("Profiles")]
    [SerializeField] private CharacterVoiceProfile genericProfile;
    [SerializeField] private CharacterVoiceProfile[] characterProfiles;

    [Header("Debug")]
    [TextArea(2, 5)]
    [SerializeField] private string testText = "I can offer 37 varahas for pepper.";
    [SerializeField] private string testCharacterId = "test_merchant_voice";

    public event Action PlaybackStarted;
    public event Action<string> PlaybackFailed;

    private Coroutine activePlaybackCoroutine;

    private static readonly string IntentGreeting = "greeting";
    private static readonly string IntentAskSpice = "ask_spice";
    private static readonly string IntentAskQuantity = "ask_quantity";
    private static readonly string IntentInitialOffer = "initial_offer";
    private static readonly string IntentCounterOffer = "counter_offer";
    private static readonly string IntentPriceTooHigh = "price_too_high";
    private static readonly string IntentHoldFirm = "hold_firm";
    private static readonly string IntentAcceptDeal = "accept_deal";
    private static readonly string IntentWalkAway = "walk_away";
    private static readonly string IntentClarification = "clarification";
    private static readonly string IntentFallback = "fallback";

    private static readonly Regex DigitsRegex = new Regex(@"\d+", RegexOptions.Compiled);

    private void Awake()
    {
        EnsureAudioSource();
    }

    [ContextMenu("Speak Test Text")]
    public void SpeakTestText()
    {
        Speak(testText, testCharacterId);
    }

    public void Speak(string text)
    {
        Speak(text, defaultCharacterId);
    }

    public void Speak(string text, string characterId)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            NotifyPlaybackFailed("empty NPC reply");
            return;
        }

        EnsureAudioSource();
        if (audioSource == null)
        {
            NotifyPlaybackFailed("audio source unavailable");
            return;
        }

        string resolvedCharacterId = !string.IsNullOrWhiteSpace(characterId)
            ? characterId.Trim().ToLowerInvariant()
            : (defaultCharacterId ?? string.Empty).Trim().ToLowerInvariant();
        string intent = DetectIntent(text);

        CharacterVoiceProfile profile = ResolveProfile(resolvedCharacterId);
        CharacterVoiceProfile fallbackProfile = genericProfile;

        if (profile == null && fallbackProfile == null)
        {
            NotifyPlaybackFailed("no voice profile configured");
            return;
        }

        AudioClip fullClip = GetIntentClip(profile, intent, ClipKind.Full);
        if (fullClip == null)
        {
            fullClip = GetIntentClip(fallbackProfile, intent, ClipKind.Full);
        }

        if (fullClip != null)
        {
            StartPlayback(new List<AudioClip> { fullClip }, resolvedCharacterId, intent, text);
            return;
        }

        List<AudioClip> sequence = BuildClipSequence(profile, fallbackProfile, intent, text);
        if (sequence.Count > 0)
        {
            StartPlayback(sequence, resolvedCharacterId, intent, text);
            return;
        }

        AudioClip fallbackClip = GetIntentClip(profile, IntentFallback, ClipKind.Full);
        if (fallbackClip == null)
        {
            fallbackClip = GetIntentClip(fallbackProfile, IntentFallback, ClipKind.Full);
        }

        if (fallbackClip != null)
        {
            StartPlayback(new List<AudioClip> { fallbackClip }, resolvedCharacterId, IntentFallback, text);
            return;
        }

        NotifyPlaybackFailed("no playable clip sequence found");
    }

    public List<string> BuildNumberClipKeys(int number)
    {
        List<string> keys = new List<string>();
        if (number <= 0)
        {
            return keys;
        }

        if (number <= 19)
        {
            keys.Add("number_" + number);
            return keys;
        }

        if (number == 100)
        {
            keys.Add("number_100");
            return keys;
        }

        if (number > 100)
        {
            int hundreds = number / 100;
            int remainder = number % 100;
            for (int i = 0; i < hundreds; i++)
            {
                keys.Add("number_100");
            }

            if (remainder > 0)
            {
                keys.AddRange(BuildNumberClipKeys(remainder));
            }

            return keys;
        }

        int tens = (number / 10) * 10;
        int ones = number % 10;

        if (tens > 0)
        {
            keys.Add("number_" + tens);
        }

        if (ones > 0)
        {
            keys.Add("number_" + ones);
        }

        return keys;
    }

    private enum ClipKind
    {
        Full,
        Prefix,
        Suffix
    }

    private void StartPlayback(List<AudioClip> clips, string characterId, string intent, string sourceText)
    {
        if (clips == null || clips.Count == 0)
        {
            NotifyPlaybackFailed("playback requested with empty clip list");
            return;
        }

        if (activePlaybackCoroutine != null)
        {
            StopCoroutine(activePlaybackCoroutine);
            activePlaybackCoroutine = null;
        }

        audioSource.Stop();
        activePlaybackCoroutine = StartCoroutine(PlaySequenceCoroutine(clips, characterId, intent, sourceText));
    }

    private IEnumerator PlaySequenceCoroutine(List<AudioClip> clips, string characterId, string intent, string sourceText)
    {
        if (debugLogs)
        {
            Debug.Log("[PreRecordedNpcTtsProvider] Playing " + clips.Count + " clip(s) | character=" + characterId + " | intent=" + intent + " | text=" + sourceText);
        }

        bool started = false;

        for (int i = 0; i < clips.Count; i++)
        {
            AudioClip clip = clips[i];
            if (clip == null)
            {
                continue;
            }

            audioSource.clip = clip;
            audioSource.Play();

            if (!started)
            {
                started = true;
                PlaybackStarted?.Invoke();
            }

            yield return null;

            while (audioSource != null && audioSource.isPlaying)
            {
                yield return null;
            }

            if (clipGapSeconds > 0f && i < clips.Count - 1)
            {
                yield return new WaitForSeconds(clipGapSeconds);
            }
        }

        activePlaybackCoroutine = null;
    }

    private List<AudioClip> BuildClipSequence(CharacterVoiceProfile profile, CharacterVoiceProfile fallbackProfile, string intent, string text)
    {
        List<AudioClip> clips = new List<AudioClip>();

        AudioClip prefix = GetIntentClip(profile, intent, ClipKind.Prefix);
        if (prefix == null)
        {
            prefix = GetIntentClip(fallbackProfile, intent, ClipKind.Prefix);
        }

        AudioClip suffix = GetIntentClip(profile, intent, ClipKind.Suffix);
        if (suffix == null)
        {
            suffix = GetIntentClip(fallbackProfile, intent, ClipKind.Suffix);
        }

        List<string> variableKeys = BuildVariableKeys(text);
        List<AudioClip> variableClips = ResolveVariableClips(profile, fallbackProfile, variableKeys);

        bool hasUsefulSequence = prefix != null && (variableClips.Count > 0 || suffix != null);
        if (!hasUsefulSequence)
        {
            return clips;
        }

        clips.Add(prefix);
        clips.AddRange(variableClips);
        if (suffix != null)
        {
            clips.Add(suffix);
        }

        return clips;
    }

    private List<string> BuildVariableKeys(string text)
    {
        List<string> keys = new List<string>();
        string normalized = Normalize(text);

        AddPriceNumberKeys(normalized, keys);

        if (ContainsAny(normalized, "varaha", "varahas"))
        {
            AddUnique(keys, "currency_varahas");
        }

        if (normalized.Contains("pepper"))
        {
            AddUnique(keys, "spice_pepper");
        }
        if (normalized.Contains("cardamom"))
        {
            AddUnique(keys, "spice_cardamom");
        }
        if (normalized.Contains("cinnamon"))
        {
            AddUnique(keys, "spice_cinnamon");
        }
        if (normalized.Contains("clove") || normalized.Contains("cloves"))
        {
            AddUnique(keys, "spice_clove");
        }

        if (ContainsQuantity(normalized, "1 seer", "one seer"))
        {
            AddUnique(keys, "quantity_1_seer");
        }
        if (ContainsQuantity(normalized, "2 seers", "two seers", "2 seer", "two seer"))
        {
            AddUnique(keys, "quantity_2_seers");
        }
        if (ContainsQuantity(normalized, "1 veesai", "one veesai", "1 viss", "one viss"))
        {
            AddUnique(keys, "quantity_1_veesai");
        }
        if (ContainsQuantity(normalized, "2 veesai", "two veesai", "2 viss", "two viss"))
        {
            AddUnique(keys, "quantity_2_veesai");
        }

        return keys;
    }

    private void AddPriceNumberKeys(string normalizedText, List<string> keys)
    {
        if (!TryExtractPriceNumber(normalizedText, out int price))
        {
            return;
        }

        AddRangeUnique(keys, BuildNumberClipKeys(price));
    }

    private bool TryExtractPriceNumber(string normalizedText, out int price)
    {
        price = 0;

        Match varahaMatch = Regex.Match(normalizedText, @"(\d+)\s*varahas?");
        if (varahaMatch.Success && int.TryParse(varahaMatch.Groups[1].Value, out int explicitPrice) && explicitPrice > 0)
        {
            price = explicitPrice;
            return true;
        }

        if (GetNumericIntentPriority(normalizedText) == 0)
        {
            return false;
        }

        MatchCollection matches = DigitsRegex.Matches(normalizedText);
        for (int i = 0; i < matches.Count; i++)
        {
            if (!int.TryParse(matches[i].Value, out int value) || value <= 0)
            {
                continue;
            }

            int unitWordStart = matches[i].Index + matches[i].Length;
            string trailing = normalizedText.Substring(Mathf.Min(unitWordStart, normalizedText.Length));
            if (Regex.IsMatch(trailing, @"^\s*(seer|seers|veesai|viss|palam|palams|gram|grams|kg|kgs)\b"))
            {
                continue;
            }

            price = value;
            return true;
        }

        return false;
    }

    private static int GetNumericIntentPriority(string normalizedText)
    {
        if (ContainsAny(normalizedText, "offer", "price", "varaha", "varahas", "counter", "agreed", "deal", "final"))
        {
            return 1;
        }

        return 0;
    }

    private List<AudioClip> ResolveVariableClips(CharacterVoiceProfile profile, CharacterVoiceProfile fallbackProfile, List<string> keys)
    {
        List<AudioClip> clips = new List<AudioClip>();
        for (int i = 0; i < keys.Count; i++)
        {
            AudioClip clip = GetVariableClip(profile, keys[i]);
            if (clip == null)
            {
                clip = GetVariableClip(fallbackProfile, keys[i]);
            }

            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        return clips;
    }

    private string DetectIntent(string text)
    {
        string normalized = Normalize(text);

        if (ContainsAnyPhrase(normalized, "did not understand", "say again", "repeat", "unclear", "could you repeat"))
        {
            return IntentClarification;
        }

        if (ContainsAnyPhrase(normalized, "no deal", "cannot trade", "go elsewhere", "take my leave", "we are finished"))
        {
            return IntentWalkAway;
        }

        if (ContainsAnyPhrase(normalized, "agreed", "we have a deal", "deal", "accepted", "fair bargain"))
        {
            return IntentAcceptDeal;
        }

        if (ContainsAnyPhrase(normalized, "final", "last offer", "i hold", "hold at", "cannot go higher", "cannot move beyond"))
        {
            return IntentHoldFirm;
        }

        if (ContainsAnyPhrase(normalized, "too high", "costly", "expensive", "reduce", "lower"))
        {
            return IntentPriceTooHigh;
        }

        if (ContainsAnyPhrase(normalized, "raise", "move to", "counter", "better price", "closer", "increase my offer"))
        {
            return IntentCounterOffer;
        }

        if (ContainsAnyPhrase(normalized, "i can offer", "my offer", "i can pay", "offer stands"))
        {
            return IntentInitialOffer;
        }

        if (ContainsAnyPhrase(normalized, "how much", "how many", "quantity", "seer", "veesai"))
        {
            return IntentAskQuantity;
        }

        if (ContainsAnyPhrase(normalized, "what are you selling", "spice", "pepper", "cardamom", "cinnamon", "clove"))
        {
            return IntentAskSpice;
        }

        if (ContainsAnyPhrase(normalized, "hello", "welcome", "greetings", "good day"))
        {
            return IntentGreeting;
        }

        return IntentFallback;
    }

    private CharacterVoiceProfile ResolveProfile(string characterId)
    {
        if (characterProfiles == null || string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        for (int i = 0; i < characterProfiles.Length; i++)
        {
            CharacterVoiceProfile candidate = characterProfiles[i];
            if (candidate == null || string.IsNullOrWhiteSpace(candidate.characterId))
            {
                continue;
            }

            if (string.Equals(candidate.characterId.Trim(), characterId, StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        return null;
    }

    private AudioClip GetIntentClip(CharacterVoiceProfile profile, string intent, ClipKind kind)
    {
        if (profile == null || string.IsNullOrWhiteSpace(intent))
        {
            return null;
        }

        IntentClipSet[] sets = null;
        switch (kind)
        {
            case ClipKind.Full:
                sets = profile.fullClips;
                break;
            case ClipKind.Prefix:
                sets = profile.prefixClips;
                break;
            case ClipKind.Suffix:
                sets = profile.suffixClips;
                break;
        }

        if (sets == null)
        {
            return null;
        }

        for (int i = 0; i < sets.Length; i++)
        {
            IntentClipSet set = sets[i];
            if (set == null || string.IsNullOrWhiteSpace(set.intent) || set.clips == null || set.clips.Length == 0)
            {
                continue;
            }

            if (!string.Equals(set.intent.Trim(), intent, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            List<AudioClip> valid = new List<AudioClip>();
            for (int clipIndex = 0; clipIndex < set.clips.Length; clipIndex++)
            {
                if (set.clips[clipIndex] != null)
                {
                    valid.Add(set.clips[clipIndex]);
                }
            }

            if (valid.Count == 0)
            {
                return null;
            }

            return valid[UnityEngine.Random.Range(0, valid.Count)];
        }

        return null;
    }

    private AudioClip GetVariableClip(CharacterVoiceProfile profile, string key)
    {
        if (profile == null || string.IsNullOrWhiteSpace(key) || profile.variableClips == null)
        {
            return null;
        }

        for (int i = 0; i < profile.variableClips.Length; i++)
        {
            VariableClip variableClip = profile.variableClips[i];
            if (variableClip == null || string.IsNullOrWhiteSpace(variableClip.key) || variableClip.clip == null)
            {
                continue;
            }

            if (string.Equals(variableClip.key.Trim(), key, StringComparison.OrdinalIgnoreCase))
            {
                return variableClip.clip;
            }
        }

        return null;
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void NotifyPlaybackFailed(string reason)
    {
        if (debugLogs)
        {
            Debug.LogWarning("[PreRecordedNpcTtsProvider] PlaybackFailed: " + reason);
        }

        PlaybackFailed?.Invoke(reason);
    }

    private static string Normalize(string text)
    {
        return (text ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static void AddUnique(List<string> keys, string key)
    {
        if (!string.IsNullOrWhiteSpace(key) && !keys.Contains(key))
        {
            keys.Add(key);
        }
    }

    private static void AddRangeUnique(List<string> keys, List<string> range)
    {
        for (int i = 0; i < range.Count; i++)
        {
            AddUnique(keys, range[i]);
        }
    }

    private static bool ContainsAny(string text, params string[] phrases)
    {
        for (int i = 0; i < phrases.Length; i++)
        {
            if (text.Contains(phrases[i]))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsAnyPhrase(string text, params string[] phrases)
    {
        for (int i = 0; i < phrases.Length; i++)
        {
            if (ContainsPhrase(text, phrases[i]))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsPhrase(string text, string phrase)
    {
        return (" " + text + " ").Contains(" " + phrase + " ");
    }

    private static bool ContainsQuantity(string text, params string[] phrases)
    {
        return ContainsAnyPhrase(text, phrases);
    }
}
