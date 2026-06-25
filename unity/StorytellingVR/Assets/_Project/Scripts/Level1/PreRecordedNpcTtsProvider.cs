using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class PreRecordedNpcTtsProvider : MonoBehaviour, INpcTtsProvider, ICharacterNpcTtsProvider, IScenarioNpcTtsProvider, INpcTtsPlaybackAware
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
    private static readonly string IntentRepeatGreeting = "repeat_greeting";
    private static readonly string IntentAskSpice = "ask_spice";
    private static readonly string IntentAskQuantity = "ask_quantity";
    private static readonly string IntentInitialOffer = "initial_offer";
    private static readonly string IntentCounterOffer = "counter_offer";
    private static readonly string IntentPriceTooHigh = "price_too_high";
    private static readonly string IntentHoldFirm = "hold_firm";
    private static readonly string IntentAcceptDeal = "accept_deal";
    private static readonly string IntentAcceptPrice = "accept_price";
    private static readonly string IntentWalkAway = "walk_away";
    private static readonly string IntentClarification = "clarification";
    private static readonly string IntentFallback = "fallback";
    private static readonly string IntentFinalCounterOffer = "final_counter_offer";
    private static readonly string IntentHistoryQuestion = "history_question";
    private static readonly string IntentSocialGreeting = "social_greeting";
    private static readonly string IntentOffTopic = "off_topic";
    private static readonly string IntentTimePressure = "time_pressure";

    private static readonly Regex DigitsRegex = new Regex(@"\d+", RegexOptions.Compiled);
    private static readonly Regex VariantSuffixRegex = new Regex(@"_\d+$", RegexOptions.Compiled);

    private void Awake()
    {
        EnsureAudioSource();
    }

    [ContextMenu("Test PreRecorded Voice")]
    private void TestPreRecordedVoice()
    {
        Speak(testText, testCharacterId);
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Fill Test Merchant Voice Clips")]
    private void AutoFillTestMerchantVoiceClips()
    {
        string root = "Assets/_Project/Audio/NPCVoices/Level1/test_merchant_voice";
        string fullPath = root + "/full";
        string prefixPath = root + "/prefix";
        string variablePath = root + "/variable";
        string suffixPath = root + "/suffix";

        Undo.RecordObject(this, "Auto Fill Test Merchant Voice Clips");
        CharacterVoiceProfile profile = GetOrCreateProfile("test_merchant_voice");
        profile.fullClips = BuildIntentClipSetsFromFolder(fullPath, stripTrailingVariantNumber: true);
        profile.prefixClips = BuildIntentClipSetsFromFolder(prefixPath, stripTrailingVariantNumber: true);
        profile.variableClips = BuildVariableClipsFromFolder(variablePath);
        profile.suffixClips = BuildSuffixClipSetsFromFolder(suffixPath);

        EditorUtility.SetDirty(this);

        LogDebug("Auto-filled test_merchant_voice profile");
        LogDebug("Full clips: " + SafeLength(profile.fullClips) +
                 " | Prefix clips: " + SafeLength(profile.prefixClips) +
                 " | Variable clips: " + SafeLength(profile.variableClips) +
                 " | Suffix clips: " + SafeLength(profile.suffixClips));
    }
#endif

    public void Speak(string text)
    {
        Speak(text, defaultCharacterId);
    }

    public void Speak(string text, string characterId)
    {
        SpeakInternal(text, characterId, null);
    }

    public void Speak(string text, string characterId, DialogueScenario scenario)
    {
        SpeakInternal(text, characterId, scenario);
    }

    private void SpeakInternal(string text, string characterId, DialogueScenario? scenario)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            LogDebug("Ignoring empty startup TTS request");
            return;
        }

        if (IsIgnorableStartupText(text))
        {
            LogDebug("Ignoring empty startup TTS request");
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
        string intent = scenario.HasValue && scenario.Value != DialogueScenario.Unknown
            ? MapScenarioToIntent(scenario.Value)
            : DetectIntent(text);
        bool usedScenarioIntent = scenario.HasValue && scenario.Value != DialogueScenario.Unknown;
        LogDebug("Detected intent: " + intent + " | character=" + resolvedCharacterId + " | text=" + text);

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

        if (fullClip == null && usedScenarioIntent && string.Equals(intent, IntentRepeatGreeting, StringComparison.OrdinalIgnoreCase))
        {
            fullClip = GetIntentClip(profile, IntentGreeting, ClipKind.Full) ??
                       GetIntentClip(fallbackProfile, IntentGreeting, ClipKind.Full);
        }

        if (fullClip != null)
        {
            LogDebug("Selected full clip for intent " + intent + ": " + fullClip.name);
            StartPlayback(new List<AudioClip> { fullClip }, resolvedCharacterId, intent, text);
            return;
        }

        List<AudioClip> sequence = BuildClipSequence(profile, fallbackProfile, intent, text);
        if (sequence.Count > 0)
        {
            LogDebug("Selected sequence for intent " + intent + ": " + string.Join(" -> ", sequence.Select(clip => clip != null ? clip.name : "<null>").ToArray()));
            StartPlayback(sequence, resolvedCharacterId, intent, text);
            return;
        }

        if (usedScenarioIntent)
        {
            string fallbackIntent = DetectIntent(text);
            if (!string.Equals(fallbackIntent, intent, StringComparison.OrdinalIgnoreCase))
            {
                LogDebug("Scenario intent fallback to text intent: " + fallbackIntent + " | scenario=" + scenario.Value);
                intent = fallbackIntent;

                fullClip = GetIntentClip(profile, intent, ClipKind.Full);
                if (fullClip == null)
                {
                    fullClip = GetIntentClip(fallbackProfile, intent, ClipKind.Full);
                }

                if (fullClip != null)
                {
                    LogDebug("Selected fallback full clip for intent " + intent + ": " + fullClip.name);
                    StartPlayback(new List<AudioClip> { fullClip }, resolvedCharacterId, intent, text);
                    return;
                }

                sequence = BuildClipSequence(profile, fallbackProfile, intent, text);
                if (sequence.Count > 0)
                {
                    LogDebug("Selected fallback sequence for intent " + intent + ": " + string.Join(" -> ", sequence.Select(clip => clip != null ? clip.name : "<null>").ToArray()));
                    StartPlayback(sequence, resolvedCharacterId, intent, text);
                    return;
                }
            }
        }

        AudioClip fallbackClip = GetIntentClip(profile, IntentFallback, ClipKind.Full);
        if (fallbackClip == null)
        {
            fallbackClip = GetIntentClip(fallbackProfile, IntentFallback, ClipKind.Full);
        }

        if (fallbackClip != null)
        {
            LogDebug("Falling back to full fallback clip: " + fallbackClip.name + " | original intent=" + intent);
            StartPlayback(new List<AudioClip> { fallbackClip }, resolvedCharacterId, IntentFallback, text);
            return;
        }

        LogDebug("Fallback failed. No full clip, no sequence, no fallback clip for intent " + intent);
        NotifyPlaybackFailed("no playable clip sequence found");
    }

    private string MapScenarioToIntent(DialogueScenario scenario)
    {
        switch (scenario)
        {
            case DialogueScenario.CustomerGreeting:
                return IntentGreeting;
            case DialogueScenario.RepeatCustomerGreeting:
                return IntentRepeatGreeting;
            case DialogueScenario.AskWhatBuyerWants:
                return IntentAskSpice;
            case DialogueScenario.AskQuantity:
                return IntentAskQuantity;
            case DialogueScenario.AskBuyerBudget:
                return IntentInitialOffer;
            case DialogueScenario.SellerPriceTooHigh:
                return IntentPriceTooHigh;
            case DialogueScenario.SellerPriceSlightlyHigh:
            case DialogueScenario.SellerPriceBelowExpected:
            case DialogueScenario.BuyerCounterMiddle:
                return IntentCounterOffer;
            case DialogueScenario.BuyerCounterFirst:
                return IntentInitialOffer;
            case DialogueScenario.BuyerCounterFinal:
                return IntentFinalCounterOffer;
            case DialogueScenario.BuyerHoldsFirm:
                return IntentHoldFirm;
            case DialogueScenario.SellerPriceAccepted:
            case DialogueScenario.TransactionSuccess:
                return IntentAcceptDeal;
            case DialogueScenario.PlayerAcceptedDeal:
                return IntentAcceptPrice;
            case DialogueScenario.PlayerRejectedBuyerOffer:
            case DialogueScenario.TransactionFailure:
                return IntentWalkAway;
            case DialogueScenario.HistoryQuestion:
                return IntentHistoryQuestion;
            case DialogueScenario.SocialGreeting:
                return IntentSocialGreeting;
            case DialogueScenario.OffTopic:
                return IntentOffTopic;
            case DialogueScenario.UnclearSpeech:
                return IntentClarification;
            case DialogueScenario.TimePressure:
                return IntentTimePressure;
            case DialogueScenario.Unknown:
            default:
                return IntentFallback;
        }
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
        if (IsOfferIntent(intent))
        {
            return BuildOfferSequence(profile, fallbackProfile, intent, text);
        }

        if (string.Equals(intent, IntentAcceptPrice, StringComparison.OrdinalIgnoreCase))
        {
            return BuildAcceptPriceSequence(profile, fallbackProfile, intent, text);
        }

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
        if (!TryResolveVariableClips(profile, fallbackProfile, variableKeys, requireAll: false, out List<AudioClip> variableClips, out List<string> missingKeys))
        {
            LogDebug("Missing required keys for intent " + intent + ": " + string.Join(", ", missingKeys.ToArray()));
            return clips;
        }

        bool hasUsefulSequence = prefix != null && (variableClips.Count > 0 || suffix != null);
        if (!hasUsefulSequence)
        {
            if (prefix == null)
            {
                LogDebug("Missing prefix for intent " + intent);
            }
            if (suffix == null)
            {
                LogDebug("Missing suffix for intent " + intent);
            }
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

    private List<AudioClip> BuildOfferSequence(CharacterVoiceProfile profile, CharacterVoiceProfile fallbackProfile, string intent, string text)
    {
        List<AudioClip> clips = new List<AudioClip>();
        AudioClip prefix = GetIntentClip(profile, intent, ClipKind.Prefix);
        if (prefix == null)
        {
            prefix = GetIntentClip(fallbackProfile, intent, ClipKind.Prefix);
        }

        if (prefix == null)
        {
            LogDebug("Missing prefix for intent " + intent);
            return clips;
        }

        if (!TryExtractPriceNumber(Normalize(text), out int price))
        {
            LogDebug("Fallback reason: could not extract offer price for intent " + intent);
            return clips;
        }

        if (!TryDetectSpiceKey(text, out string spiceKey))
        {
            LogDebug("Fallback reason: could not detect spice key for intent " + intent);
            return clips;
        }

        List<string> sequenceKeys = new List<string>();
        sequenceKeys.AddRange(BuildNumberClipKeys(price));
        sequenceKeys.Add("currency_varahas");
        sequenceKeys.Add(spiceKey);

        AudioClip forSuffix = GetIntentClip(profile, "for", ClipKind.Suffix);
        if (forSuffix == null)
        {
            forSuffix = GetIntentClip(fallbackProfile, "for", ClipKind.Suffix);
        }

        if (forSuffix == null)
        {
            LogDebug("Missing suffix key: for");
            LogDebug("Fallback reason: incomplete stitched offer sequence");
            return clips;
        }

        if (!TryResolveVariableClips(profile, fallbackProfile, sequenceKeys, requireAll: true, out List<AudioClip> resolvedClips, out List<string> missingKeys))
        {
            LogDebug("Missing required keys for intent " + intent + ": " + string.Join(", ", missingKeys.ToArray()));
            LogDebug("Fallback reason: incomplete stitched offer sequence");
            return clips;
        }

        LogDebug("Selected sequence clip keys for intent " + intent + ": prefix:" + intent + " -> " +
                 string.Join(" -> ", BuildOfferDebugKeys(sequenceKeys).ToArray()));

        clips.Add(prefix);
        clips.AddRange(resolvedClips.GetRange(0, resolvedClips.Count - 1));
        clips.Add(forSuffix);
        clips.Add(resolvedClips[resolvedClips.Count - 1]);
        return clips;
    }

    private List<AudioClip> BuildAcceptPriceSequence(CharacterVoiceProfile profile, CharacterVoiceProfile fallbackProfile, string intent, string text)
    {
        List<AudioClip> clips = new List<AudioClip>();
        AudioClip prefix = GetIntentClip(profile, intent, ClipKind.Prefix);
        if (prefix == null)
        {
            prefix = GetIntentClip(fallbackProfile, intent, ClipKind.Prefix);
        }

        if (prefix == null)
        {
            LogDebug("Missing prefix for intent " + intent);
            return clips;
        }

        if (!TryExtractPriceNumber(Normalize(text), out int price))
        {
            LogDebug("Fallback reason: could not extract accepted price");
            return clips;
        }

        List<string> sequenceKeys = new List<string>();
        sequenceKeys.AddRange(BuildNumberClipKeys(price));
        sequenceKeys.Add("currency_varahas");

        if (!TryResolveVariableClips(profile, fallbackProfile, sequenceKeys, requireAll: true, out List<AudioClip> resolvedClips, out List<string> missingKeys))
        {
            LogDebug("Missing required keys for intent " + intent + ": " + string.Join(", ", missingKeys.ToArray()));
            LogDebug("Fallback reason: incomplete stitched accepted-price sequence");
            return clips;
        }

        LogDebug("Selected sequence clip keys for intent " + intent + ": prefix:" + intent + " -> " + string.Join(" -> ", sequenceKeys.ToArray()));

        clips.Add(prefix);
        clips.AddRange(resolvedClips);
        return clips;
    }

    private static bool IsOfferIntent(string intent)
    {
        return
            string.Equals(intent, IntentInitialOffer, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(intent, IntentCounterOffer, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(intent, IntentFinalCounterOffer, StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> BuildOfferDebugKeys(List<string> variableKeys)
    {
        List<string> debugKeys = new List<string>();
        if (variableKeys == null || variableKeys.Count == 0)
        {
            return debugKeys;
        }

        int spiceIndex = variableKeys.Count - 1;
        for (int i = 0; i < variableKeys.Count; i++)
        {
            if (i == spiceIndex)
            {
                debugKeys.Add("suffix:for");
            }

            debugKeys.Add(variableKeys[i]);
        }

        return debugKeys;
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

    private bool TryResolveVariableClips(
        CharacterVoiceProfile profile,
        CharacterVoiceProfile fallbackProfile,
        List<string> keys,
        bool requireAll,
        out List<AudioClip> clips,
        out List<string> missingKeys)
    {
        clips = new List<AudioClip>();
        missingKeys = new List<string>();

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
            else
            {
                LogDebug("Missing variable key: " + keys[i]);
                missingKeys.Add(keys[i]);
                if (requireAll)
                {
                    clips.Clear();
                    return false;
                }
            }
        }

        return missingKeys.Count == 0 || !requireAll;
    }

    private string DetectIntent(string text)
    {
        string normalized = Normalize(text);

        if (ContainsAny(normalized, "please say that again", "say that again", "did not understand", "could you repeat", "repeat that", "repeat", "unclear"))
        {
            return IntentClarification;
        }

        if (ContainsAny(normalized, "no deal", "i will go elsewhere", "go elsewhere", "take my leave", "we are finished", "cannot trade"))
        {
            return IntentWalkAway;
        }

        if (ContainsAny(normalized, "agreed at") && ContainsAny(normalized, "varaha", "varahas"))
        {
            return IntentAcceptPrice;
        }

        if (ContainsAny(normalized, "we have a deal", "agreed", "accepted", "fair bargain"))
        {
            return IntentAcceptDeal;
        }

        if (ContainsAny(normalized, "too high", "costly", "expensive", "reduce", "lower"))
        {
            return IntentPriceTooHigh;
        }

        if (StartsWithAnyPhrase(normalized, "good day", "hello", "greetings", "welcome"))
        {
            return IntentGreeting;
        }

        if (ContainsAny(normalized, "my final offer is", "final offer", "last offer"))
        {
            return IntentFinalCounterOffer;
        }

        if (ContainsAny(normalized, "i hold", "hold at", "cannot go higher", "cannot move beyond"))
        {
            return IntentHoldFirm;
        }

        if (ContainsAny(normalized, "increase my offer to", "raise my offer to", "counter offer", "counter at", "move to", "better price", "closer"))
        {
            return IntentCounterOffer;
        }

        if (ContainsAny(normalized, "i can offer", "my offer is", "my offer", "i can pay", "offer stands"))
        {
            return IntentInitialOffer;
        }

        if (ContainsAnyPhrase(normalized, "how much", "how many", "quantity", "seer", "veesai"))
        {
            return IntentAskQuantity;
        }

        if (ContainsAny(normalized, "what are you selling", "spice", "pepper", "cardamom", "cinnamon", "clove"))
        {
            return IntentAskSpice;
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

    private static bool IsIgnorableStartupText(string text)
    {
        string normalized = Normalize(text);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return true;
        }

        return
            string.Equals(normalized, "customer approaching...", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "customer approaching", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "approaching...", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "thinking...", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "listening...", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "processing...", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(normalized, "loading...", StringComparison.OrdinalIgnoreCase);
    }

    private void LogDebug(string message)
    {
        if (debugLogs)
        {
            Debug.Log("[PreRecordedNpcTtsProvider] " + message);
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

    private static bool StartsWithAnyPhrase(string text, params string[] phrases)
    {
        for (int i = 0; i < phrases.Length; i++)
        {
            string phrase = phrases[i];
            if (!text.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (text.Length == phrase.Length)
            {
                return true;
            }

            char next = text[phrase.Length];
            if (!char.IsLetterOrDigit(next))
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

    private static int SafeLength(Array array)
    {
        return array != null ? array.Length : 0;
    }

    private bool TryDetectSpiceKey(string text, out string spiceKey)
    {
        string normalized = Normalize(text);
        if (normalized.Contains("pepper"))
        {
            spiceKey = "spice_pepper";
            return true;
        }

        if (normalized.Contains("cardamom"))
        {
            spiceKey = "spice_cardamom";
            return true;
        }

        if (normalized.Contains("cinnamon"))
        {
            spiceKey = "spice_cinnamon";
            return true;
        }

        if (normalized.Contains("clove") || normalized.Contains("cloves"))
        {
            spiceKey = "spice_clove";
            return true;
        }

        spiceKey = string.Empty;
        return false;
    }

#if UNITY_EDITOR
    private CharacterVoiceProfile GetOrCreateProfile(string characterId)
    {
        if (characterProfiles == null)
        {
            characterProfiles = Array.Empty<CharacterVoiceProfile>();
        }

        for (int i = 0; i < characterProfiles.Length; i++)
        {
            if (characterProfiles[i] != null &&
                string.Equals(characterProfiles[i].characterId, characterId, StringComparison.OrdinalIgnoreCase))
            {
                return characterProfiles[i];
            }
        }

        CharacterVoiceProfile profile = new CharacterVoiceProfile
        {
            characterId = characterId,
            fullClips = Array.Empty<IntentClipSet>(),
            prefixClips = Array.Empty<IntentClipSet>(),
            suffixClips = Array.Empty<IntentClipSet>(),
            variableClips = Array.Empty<VariableClip>()
        };

        Array.Resize(ref characterProfiles, characterProfiles.Length + 1);
        characterProfiles[characterProfiles.Length - 1] = profile;
        return profile;
    }

    private IntentClipSet[] BuildIntentClipSetsFromFolder(string assetFolder, bool stripTrailingVariantNumber)
    {
        Dictionary<string, List<AudioClip>> grouped = new Dictionary<string, List<AudioClip>>(StringComparer.OrdinalIgnoreCase);
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { assetFolder });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
            {
                continue;
            }

            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            string key = stripTrailingVariantNumber ? StripTrailingVariantNumber(name) : name;
            if (!grouped.TryGetValue(key, out List<AudioClip> list))
            {
                list = new List<AudioClip>();
                grouped[key] = list;
            }
            list.Add(clip);
        }

        return grouped
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new IntentClipSet
            {
                intent = pair.Key,
                clips = pair.Value.Where(clip => clip != null).ToArray()
            })
            .ToArray();
    }

    private VariableClip[] BuildVariableClipsFromFolder(string assetFolder)
    {
        List<VariableClip> clips = new List<VariableClip>();
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { assetFolder });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
            {
                continue;
            }

            string key = System.IO.Path.GetFileNameWithoutExtension(path);
            clips.Add(new VariableClip
            {
                key = key,
                clip = clip
            });
        }

        return clips
            .OrderBy(item => item.key, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private IntentClipSet[] BuildSuffixClipSetsFromFolder(string assetFolder)
    {
        Dictionary<string, List<AudioClip>> grouped = new Dictionary<string, List<AudioClip>>(StringComparer.OrdinalIgnoreCase);
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { assetFolder });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
            {
                continue;
            }

            string key = StripTrailingVariantNumber(System.IO.Path.GetFileNameWithoutExtension(path));
            if (!grouped.TryGetValue(key, out List<AudioClip> list))
            {
                list = new List<AudioClip>();
                grouped[key] = list;
            }
            list.Add(clip);
        }

        return grouped
            .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
            .Select(pair => new IntentClipSet
            {
                intent = pair.Key,
                clips = pair.Value.Where(clip => clip != null).Distinct().ToArray()
            })
            .ToArray();
    }

    private static string StripTrailingVariantNumber(string name)
    {
        return VariantSuffixRegex.Replace(name ?? string.Empty, string.Empty);
    }
#endif
}
