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
}

public class LocalSaveManager
{
    public const string ProfileFileName = "level1_player_profile.json";

    private readonly string savePath;
    public string SavePath => savePath;

    public LocalSaveManager()
    {
        savePath = GetActiveSavePath();
    }

    public LocalProfileData LoadProfile(MarketManager marketManager)
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                LocalProfileData data = JsonUtility.FromJson<LocalProfileData>(json);
                if (data != null)
                {
                    EnsureDefaults(data, marketManager);
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[LocalSaveManager] Failed to load profile. Using defaults. " + ex.Message);
            }
        }

        LocalProfileData defaults = CreateDefaultProfile(marketManager);
        SaveProfile(defaults);
        return defaults;
    }

    public void SaveProfile(LocalProfileData profile)
    {
        try
        {
            Directory.CreateDirectory(GetActiveSaveDirectory());
            string json = JsonUtility.ToJson(profile, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError("[LocalSaveManager] Failed to save profile: " + ex.Message);
        }
    }

    public bool DeleteProfile()
    {
        return DeleteProfileAtPath(savePath);
    }

    public static string GetActiveSavePath()
    {
        return Path.Combine(GetActiveSaveDirectory(), ProfileFileName);
    }

    public static string GetActiveSaveDirectory()
    {
        #if UNITY_EDITOR
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
        Directory.CreateDirectory(GetActiveSaveDirectory());
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
            shift_stats = new ShiftStatsData()
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

        if (profile.inventory == null)
        {
            profile.inventory = new List<InventoryEntry>();
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
