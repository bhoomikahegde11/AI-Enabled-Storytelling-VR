using System;
using System.IO;
using UnityEngine;

[DisallowMultipleComponent]
public class Level1SaveDebugTool : MonoBehaviour
{
    [SerializeField] private string selectedTestStateName = string.Empty;
    [SerializeField] private string newTestStateName = "level1_test_state";

    public string CurrentSavePath => LocalSaveManager.GetActiveSavePath();
    public string TestStatesDirectory => LocalSaveManager.GetEditorTestStatesDirectory();
    public string SelectedTestStateName
    {
        get => selectedTestStateName;
        set => selectedTestStateName = value ?? string.Empty;
    }

    public string NewTestStateName
    {
        get => newTestStateName;
        set => newTestStateName = value ?? string.Empty;
    }

    public bool ActiveSaveExists => File.Exists(CurrentSavePath);

    public void EnsureDirectoriesExist()
    {
        LocalSaveManager.EnsureActiveSaveDirectoryExists();
        LocalSaveManager.EnsureEditorTestStatesDirectoryExists();
    }

    public string[] GetAvailableTestStateNames()
    {
        #if UNITY_EDITOR
        EnsureDirectoriesExist();

        string directory = TestStatesDirectory;
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        string[] files = Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly);
        Array.Sort(files, StringComparer.OrdinalIgnoreCase);

        string[] names = new string[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            names[i] = Path.GetFileName(files[i]);
        }

        return names;
        #else
        return Array.Empty<string>();
        #endif
    }

    public string GetSelectedTestStatePath()
    {
        if (string.IsNullOrWhiteSpace(selectedTestStateName))
        {
            return string.Empty;
        }

        return Path.Combine(TestStatesDirectory, selectedTestStateName);
    }

    public bool DeleteActiveSave()
    {
        EnsureDirectoriesExist();

        bool deleted = LocalSaveManager.DeleteActiveProfile();
        Debug.Log(deleted
            ? "[Level1SaveDebugTool] Deleted active Level 1 save."
            : "[Level1SaveDebugTool] No active Level 1 save file was present to delete.");

        RefreshLiveStateAfterDiskChange();
        return deleted;
    }

    public void ReloadActiveSave()
    {
        EnsureDirectoriesExist();
        EnsureDefaultSaveExists();
        RefreshLiveStateAfterDiskChange();
        Debug.Log("[Level1SaveDebugTool] Reloaded active Level 1 save from disk.");
    }

    public void StartFresh()
    {
        EnsureDirectoriesExist();
        LocalSaveManager.DeleteActiveProfile();
        EnsureDefaultSaveExists();
        RefreshLiveStateAfterDiskChange();
        Debug.Log("[Level1SaveDebugTool] Started fresh Level 1 save state.");
    }

    public bool CopySelectedTestStateToActiveSave()
    {
        #if UNITY_EDITOR
        EnsureDirectoriesExist();

        string sourcePath = GetSelectedTestStatePath();
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            Debug.LogWarning("[Level1SaveDebugTool] Select a valid test state before copying.");
            return false;
        }

        File.Copy(sourcePath, CurrentSavePath, true);
        RefreshLiveStateAfterDiskChange();
        Debug.Log("[Level1SaveDebugTool] Copied test state to active save: " + Path.GetFileName(sourcePath));
        return true;
        #else
        Debug.LogWarning("[Level1SaveDebugTool] Test-state swapping is editor-only.");
        return false;
        #endif
    }

    public bool SaveCurrentActiveSaveAsNewTestState()
    {
        #if UNITY_EDITOR
        EnsureDirectoriesExist();
        SaveLiveStateBeforeSnapshot();

        if (!File.Exists(CurrentSavePath))
        {
            Debug.LogWarning("[Level1SaveDebugTool] No active Level 1 save file exists to snapshot.");
            return false;
        }

        string sanitizedName = SanitizeFileName(newTestStateName);
        if (string.IsNullOrWhiteSpace(sanitizedName))
        {
            sanitizedName = "level1_test_state";
        }

        string destinationPath = Path.Combine(TestStatesDirectory, sanitizedName + ".json");
        File.Copy(CurrentSavePath, destinationPath, true);
        selectedTestStateName = Path.GetFileName(destinationPath);
        Debug.Log("[Level1SaveDebugTool] Saved active Level 1 save as test state: " + selectedTestStateName);
        return true;
        #else
        Debug.LogWarning("[Level1SaveDebugTool] Saving test states is editor-only.");
        return false;
        #endif
    }

    private void RefreshLiveStateAfterDiskChange()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Level1GameState.ExistingInstance != null)
        {
            Level1GameState.ExistingInstance.ReloadProfileFromDisk();
        }
    }

    private void SaveLiveStateBeforeSnapshot()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Level1GameState.ExistingInstance != null)
        {
            Level1GameState.ExistingInstance.SaveProfileToDisk();
        }
    }

    private void EnsureDefaultSaveExists()
    {
        if (File.Exists(CurrentSavePath))
        {
            return;
        }

        LocalSaveManager saveManager = new LocalSaveManager();
        saveManager.LoadProfile(new MarketManager());
    }

    private static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        string sanitized = value.Trim();
        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
        {
            sanitized = sanitized.Replace(invalidCharacter.ToString(), string.Empty);
        }

        return sanitized;
    }
}
