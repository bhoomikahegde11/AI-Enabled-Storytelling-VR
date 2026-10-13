using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    public string spiceKey;
    public int grams;
}

[Serializable]
public class GlobalMetricsData
{
    public int reputation = PlayerState.DefaultReputation;
    public int total_varahas = PlayerState.DefaultVarahas;
    public List<string> completed_levels = new List<string>();
}

[Serializable]
public class ShiftStatsData
{
    public int shifts_completed;
    public int total_varahas_earned;
    public int total_deals_made;
}

[Serializable]
public class LocalProfileData
{
    public GlobalMetricsData global_metrics = new GlobalMetricsData();
    public List<InventoryEntry> inventory = new List<InventoryEntry>();
    public ShiftStatsData shift_stats = new ShiftStatsData();
    public string current_scene = LocalSaveManager.DefaultCurrentSceneName;
    public int progression_index = LocalSaveManager.DefaultProgressionIndex;
    public bool intro_completed;
    public float remainingMarketDaySeconds;
    public bool hasSavedMarketDayTimer;
    public string save_name = "";
    public string character_name = "";
    public string selected_gender = "Male";
}

public class LocalSaveManager
{
    public const string ProfileFileName = "level1_player_profile.json";
    public const int SlotCount = 3;
    public const string DefaultCurrentSceneName = "";
    public const int DefaultProgressionIndex = -1;

    #if UNITY_EDITOR
    // Test fixtures can isolate Level 1's real save writes from the developer's profile.
    private static string editorTestSaveDirectoryOverride;
    public static string EditorTestSaveDirectoryOverride
    {
        get => editorTestSaveDirectoryOverride;
        set
        {
            editorTestSaveDirectoryOverride = value;
            ActiveSlot = 0;
        }
    }
    #endif

    private readonly string savePath;
    public string SavePath => savePath;
    public static int ActiveSlot { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSlotForNewApplicationSession() => ActiveSlot = 0;

    public LocalSaveManager()
    {
        savePath = GetActiveSavePath();
    }

    public LocalProfileData LoadProfile(MarketManager marketManager)
    {
        MarketManager resolvedMarketManager = marketManager ?? new MarketManager();

        if (File.Exists(savePath))
        {
            if (TryReadProfile(savePath, out LocalProfileData data))
            {
                EnsureDefaults(data, resolvedMarketManager);
                return data;
            }

            // A damaged slot must not be replaced by an apparently empty profile.
            Debug.LogError("[LocalSaveManager] Profile is invalid; refusing to overwrite " + savePath);
            return CreateDefaultProfile(resolvedMarketManager);
        }

        if (File.Exists(savePath + ".bak") || File.Exists(savePath + ".tmp"))
        {
            Debug.LogError("[LocalSaveManager] Interrupted profile write; preserving recovery files at " + savePath);
            return CreateDefaultProfile(resolvedMarketManager);
        }

        LocalProfileData defaults = CreateDefaultProfile(resolvedMarketManager);
        SaveProfile(defaults);
        return defaults;
    }

    public LocalProfileData LoadProfile()
    {
        return LoadProfile(new MarketManager());
    }

    public void SaveProfile(LocalProfileData profile)
    {
        WriteProfile(profile, false);
    }

    public bool CreateFreshProfile(string saveName = "", string characterName = "", string gender = "Male")
    {
        LocalProfileData fresh = CreateDefaultProfile(new MarketManager());
        fresh.save_name = saveName ?? "";
        fresh.character_name = characterName ?? "";
        fresh.selected_gender = string.IsNullOrWhiteSpace(gender) ? "Male" : gender;
        return WriteProfile(fresh, true);
    }

    private bool WriteProfile(LocalProfileData profile, bool allowExplicitOverwrite)
    {
        if (profile == null) return false;
        string tempPath = savePath + ".tmp";
        string backupPath = savePath + ".bak";

        try
        {
            if (!allowExplicitOverwrite &&
                ((File.Exists(savePath) && !TryReadProfile(savePath, out _)) ||
                 (!File.Exists(savePath) && (File.Exists(backupPath) || File.Exists(tempPath)))))
            {
                Debug.LogError("[LocalSaveManager] Refusing to overwrite an invalid profile: " + savePath);
                return false;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            string json = JsonUtility.ToJson(profile, true);
            File.WriteAllText(tempPath, json);

            if (File.Exists(savePath))
            {
                // Quest keeps the previous file as a rollback copy during the rename sequence.
                if (Application.platform == RuntimePlatform.Android)
                {
                    ReplaceWithBackup(tempPath, savePath, backupPath);
                }
                else
                {
                    try { File.Replace(tempPath, savePath, backupPath); }
                    catch (NotSupportedException) { ReplaceWithBackup(tempPath, savePath, backupPath); }
                }
            }
            else
            {
                File.Move(tempPath, savePath);
            }

            Debug.Log($"[SAVE] Saved progression current_scene={profile.current_scene}, progression_index={profile.progression_index}, intro_completed={profile.intro_completed}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[LocalSaveManager] Failed to save profile: " + ex.Message);

            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch (Exception cleanupException)
            {
                Debug.LogWarning("[LocalSaveManager] Failed to clean up temp save file: " + cleanupException.Message);
            }
            return false;
        }
    }

    private static void ReplaceWithBackup(string tempPath, string targetPath, string backupPath)
    {
        if (File.Exists(backupPath)) File.Delete(backupPath);
        File.Move(targetPath, backupPath);
        try { File.Move(tempPath, targetPath); }
        catch
        {
            File.Move(backupPath, targetPath);
            throw;
        }
    }

    public bool DeleteProfile()
    {
        return DeleteProfileAtPath(savePath);
    }

    public static string GetActiveSavePath()
    {
        return ActiveSlot > 0 ? GetSlotSavePath(ActiveSlot) : Path.Combine(GetActiveSaveDirectory(), ProfileFileName);
    }

    public static bool IsValidSlot(int slot) => slot >= 1 && slot <= SlotCount;

    public static bool SelectSlot(int slot)
    {
        if (!IsValidSlot(slot)) return false;
        ActiveSlot = slot;
        return true;
    }

    public static void ClearActiveSlot() => ActiveSlot = 0;

    public static string GetSlotSavePath(int slot)
    {
        if (!IsValidSlot(slot)) throw new ArgumentOutOfRangeException(nameof(slot));
        #if UNITY_EDITOR
        string root = !string.IsNullOrEmpty(EditorTestSaveDirectoryOverride)
            ? EditorTestSaveDirectoryOverride : Application.persistentDataPath;
        #else
        string root = Application.persistentDataPath;
        #endif
        return Path.Combine(root, "JourneySlots", $"journey_slot_{slot}.json");
    }

    public static bool SlotHasAnyData(int slot)
    {
        string path = GetSlotSavePath(slot);
        return File.Exists(path) || File.Exists(path + ".bak") || File.Exists(path + ".tmp");
    }

    public static bool TryLoadSlot(int slot, out LocalProfileData profile)
    {
        return TryReadProfile(GetSlotSavePath(slot), out profile);
    }

    public bool TryLoadExistingProfile(out LocalProfileData profile) => TryReadProfile(savePath, out profile);

    public static bool DeleteSlot(int slot)
    {
        string path = GetSlotSavePath(slot);
        try
        {
            foreach (string suffix in new[] { "", ".tmp", ".bak" })
            {
                string file = path + suffix;
                if (File.Exists(file)) File.Delete(file);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[LocalSaveManager] Failed to delete slot " + slot + ": " + ex.Message);
            return false;
        }
    }

    private static bool TryReadProfile(string path, out LocalProfileData profile)
    {
        profile = null;
        try
        {
            if (!File.Exists(path)) return false;
            string json = File.ReadAllText(path);
            if (!json.Contains("\"global_metrics\"") || !json.Contains("\"inventory\"") ||
                !json.Contains("\"shift_stats\"")) return false;
            LocalProfileData parsed = JsonUtility.FromJson<LocalProfileData>(json);
            if (parsed == null || parsed.global_metrics == null || parsed.inventory == null || parsed.shift_stats == null)
                return false;
            profile = parsed;
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[LocalSaveManager] Cannot read profile at " + path + ": " + ex.Message);
            return false;
        }
    }

    public static string GetActiveSaveDirectory()
    {
        #if UNITY_EDITOR
        if (!string.IsNullOrEmpty(EditorTestSaveDirectoryOverride)) return EditorTestSaveDirectoryOverride;
        return Path.Combine(Application.dataPath, "_Project", "SaveStates", "Level1");
        #else
        return Application.persistentDataPath;
        #endif
    }

    public static string GetEditorTestStatesDirectory()
    {
        #if UNITY_EDITOR
        return Path.Combine(GetActiveSaveDirectory(), "TestStates");
        #else
        return string.Empty;
        #endif
    }

    public static void EnsureActiveSaveDirectoryExists()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(GetActiveSavePath()));
    }

    public static void EnsureEditorTestStatesDirectoryExists()
    {
        #if UNITY_EDITOR
        Directory.CreateDirectory(GetEditorTestStatesDirectory());
        #endif
    }

    public static bool DeleteActiveProfile()
    {
        return DeleteProfileAtPath(GetActiveSavePath());
    }

    public static bool ActiveProfileExists()
    {
        return File.Exists(GetActiveSavePath());
    }

    private static bool DeleteProfileAtPath(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[LocalSaveManager] Failed to delete profile: " + ex.Message);
            return false;
        }
    }

    private static LocalProfileData CreateDefaultProfile(MarketManager marketManager)
    {
        return new LocalProfileData
        {
            global_metrics = new GlobalMetricsData
            {
                reputation = PlayerState.DefaultReputation,
                total_varahas = PlayerState.DefaultVarahas,
                completed_levels = new List<string>()
            },
            inventory = marketManager.CreateDefaultInventoryEntries(),
            shift_stats = new ShiftStatsData(),
            current_scene = DefaultCurrentSceneName,
            progression_index = DefaultProgressionIndex,
            intro_completed = false,
            remainingMarketDaySeconds = 0f,
            hasSavedMarketDayTimer = false
        };
    }

    private static void EnsureDefaults(LocalProfileData profile, MarketManager marketManager)
    {
        if (profile.global_metrics == null)
        {
            profile.global_metrics = new GlobalMetricsData();
        }

        if (profile.global_metrics.completed_levels == null)
        {
            profile.global_metrics.completed_levels = new List<string>();
        }

        if (profile.shift_stats == null)
        {
            profile.shift_stats = new ShiftStatsData();
        }

        if (string.IsNullOrWhiteSpace(profile.selected_gender))
        {
            profile.selected_gender = "Male";
        }

        if (profile.inventory == null)
        {
            profile.inventory = new List<InventoryEntry>();
        }

        if (profile.progression_index < DefaultProgressionIndex)
        {
            profile.progression_index = DefaultProgressionIndex;
        }

        if (float.IsNaN(profile.remainingMarketDaySeconds) || float.IsInfinity(profile.remainingMarketDaySeconds))
        {
            profile.remainingMarketDaySeconds = 0f;
            profile.hasSavedMarketDayTimer = false;
        }

        foreach (InventoryEntry defaultEntry in marketManager.CreateDefaultInventoryEntries())
        {
            if (!profile.inventory.Exists(entry => string.Equals(entry.spiceKey, defaultEntry.spiceKey, StringComparison.OrdinalIgnoreCase)))
            {
                profile.inventory.Add(defaultEntry);
            }
        }
    }
}
