using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueVoiceEntry
{
    public string lineId;
    public AudioClip audioClip;
    public string description;
}

[CreateAssetMenu(menuName = "Dialogue/Voice Database")]
public class DialogueVoiceDatabase : ScriptableObject
{
    [SerializeField]
    private List<DialogueVoiceEntry> entries = new List<DialogueVoiceEntry>();

    private Dictionary<string, DialogueVoiceEntry> entryDictionary;

    private void OnEnable()
    {
        InitializeDictionary();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        InitializeDictionary();
    }
#endif

    private void InitializeDictionary()
    {
        entryDictionary = new Dictionary<string, DialogueVoiceEntry>();

        if (entries == null)
            return;

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.lineId))
                continue;

            // Trim whitespace to avoid accidental mismatches
            string cleanId = entry.lineId.Trim();

            if (entryDictionary.ContainsKey(cleanId))
            {
                Debug.LogWarning($"[DialogueVoiceDatabase] Duplicate line ID found: {cleanId}");
            }
            else
            {
                entryDictionary.Add(cleanId, entry);
            }
        }
    }

    public AudioClip GetAudioClip(string lineId)
    {
        if (string.IsNullOrEmpty(lineId))
            return null;

        string cleanId = lineId.Trim();

        if (entryDictionary == null)
            InitializeDictionary();

        Debug.Log($"[VOICE DB] Serialized entries={(entries != null ? entries.Count : 0)}");
        Debug.Log($"[VOICE DB] Lookup entries={(entryDictionary != null ? entryDictionary.Count : 0)}");
        Debug.Log($"[VOICE DB] Looking for '{cleanId}'");

        if (!entryDictionary.ContainsKey(cleanId) && entries != null && entries.Count != entryDictionary.Count)
        {
            // Fallback: If not found, and counts are mismatched, rebuild in case the cache is stale.
            InitializeDictionary();
        }

        bool found = entryDictionary.TryGetValue(cleanId, out var entry);
        Debug.Log($"[VOICE DB] Found={found}");
        
        AudioClip clip = found ? entry.audioClip : null;
        Debug.Log($"[VOICE DB] Clip={(clip != null ? clip.name : "null")}");

        return clip;
    }
}
