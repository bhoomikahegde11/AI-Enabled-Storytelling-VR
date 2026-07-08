using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSaveFlow : MonoBehaviour
{
    public const string SaveNameKey = "MainMenu.SelectedSaveName";
    public const string CharacterNameKey = "MainMenu.SelectedCharacterName";
    public const string GenderKey = "MainMenu.SelectedGender";

    private const string DefaultGameplaySceneName = "Level1_MainLoopUpdated";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuRoot;
    [SerializeField] private GameObject saveSelectPanel;
    [SerializeField] private GameObject newSavePanel;

    [Header("New Save Fields")]
    [SerializeField] private TMP_InputField saveNameInput;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private Toggle maleToggle;
    [SerializeField] private Toggle femaleToggle;
    [SerializeField] private TMP_Dropdown genderDropdown;

    [Header("Fallback")]
    [SerializeField] private string fallbackGameplaySceneName = DefaultGameplaySceneName;

    private void Awake()
    {
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(newSavePanel, false);
    }

    public void OpenSaveSelect()
    {
        PopulateFieldsFromPrefs();
        SetPanelActive(mainMenuRoot, false);
        SetPanelActive(saveSelectPanel, true);
        SetPanelActive(newSavePanel, false);
    }

    public void CloseSaveSelect()
    {
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(saveSelectPanel, false);
    }

    public void ContinueExistingSave()
    {
        PlayMenuClick();
        SaveCurrentFieldValuesToPrefs();
        StartGameplay();
    }

    public void OpenNewSave()
    {
        PlayMenuClick();
        PopulateFieldsFromPrefs();
        SetPanelActive(saveSelectPanel, true);
        SetPanelActive(newSavePanel, true);
    }

    public void Back()
    {
        PlayMenuClick();

        if (newSavePanel != null && newSavePanel.activeSelf)
        {
            SetPanelActive(newSavePanel, false);
            return;
        }

        CloseSaveSelect();
    }

    public void StartNewSave()
    {
        PlayMenuClick();
        SaveCurrentFieldValuesToPrefs();
        ClearExistingLevel1SaveForNewJourney();
        StartGameplay();
    }

    private void SaveCurrentFieldValuesToPrefs()
    {
        string saveName = SanitizeValue(saveNameInput != null ? saveNameInput.text : string.Empty, "Save 1");
        string characterName = SanitizeValue(characterNameInput != null ? characterNameInput.text : string.Empty, "Merchant");
        string gender = GetSelectedGender();

        PlayerPrefs.SetString(SaveNameKey, saveName);
        PlayerPrefs.SetString(CharacterNameKey, characterName);
        PlayerPrefs.SetString(GenderKey, gender);
        PlayerPrefs.Save();

        Debug.Log($"[MainMenuSaveFlow] Stored temporary save selection. Save='{saveName}', Character='{characterName}', Gender='{gender}'.");
    }

    private void PopulateFieldsFromPrefs()
    {
        if (saveNameInput != null)
        {
            saveNameInput.text = PlayerPrefs.GetString(SaveNameKey, saveNameInput.text);
        }

        if (characterNameInput != null)
        {
            characterNameInput.text = PlayerPrefs.GetString(CharacterNameKey, characterNameInput.text);
        }

        string savedGender = PlayerPrefs.GetString(GenderKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(savedGender))
        {
            ApplySavedGender(savedGender);
        }
    }

    private void StartGameplay()
    {
        CloseSaveSelect();

        if (GameManager.Instance != null)
        {
            Debug.Log("[MainMenuSaveFlow] Starting gameplay through GameManager.LoadNextScene().");
            GameManager.Instance.LoadNextScene();
            return;
        }

        if (string.IsNullOrWhiteSpace(fallbackGameplaySceneName))
        {
            Debug.LogError("[MainMenuSaveFlow] Cannot start gameplay because GameManager is missing and fallbackGameplaySceneName is empty.");
            return;
        }

        Debug.LogWarning($"[MainMenuSaveFlow] GameManager.Instance is missing. Falling back to direct scene load '{fallbackGameplaySceneName}'.");
        SceneManager.LoadScene(fallbackGameplaySceneName);
    }

    private string GetSelectedGender()
    {
        if (maleToggle != null && maleToggle.isOn)
        {
            return "Male";
        }

        if (femaleToggle != null && femaleToggle.isOn)
        {
            return "Female";
        }

        if (genderDropdown != null && genderDropdown.options != null && genderDropdown.options.Count > 0)
        {
            int safeIndex = Mathf.Clamp(genderDropdown.value, 0, genderDropdown.options.Count - 1);
            string optionText = genderDropdown.options[safeIndex].text;
            return SanitizeValue(optionText, "Male");
        }

        return "Male";
    }

    private void ApplySavedGender(string savedGender)
    {
        bool isMale = string.Equals(savedGender, "Male", System.StringComparison.OrdinalIgnoreCase);
        bool isFemale = string.Equals(savedGender, "Female", System.StringComparison.OrdinalIgnoreCase);

        if (maleToggle != null)
        {
            maleToggle.isOn = isMale;
        }

        if (femaleToggle != null)
        {
            femaleToggle.isOn = isFemale;
        }

        if (genderDropdown != null)
        {
            int optionIndex = FindGenderOptionIndex(savedGender);
            if (optionIndex >= 0)
            {
                genderDropdown.value = optionIndex;
            }
        }
    }

    private int FindGenderOptionIndex(string savedGender)
    {
        if (genderDropdown == null || genderDropdown.options == null)
        {
            return -1;
        }

        for (int i = 0; i < genderDropdown.options.Count; i++)
        {
            if (string.Equals(genderDropdown.options[i].text, savedGender, System.StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    private static string SanitizeValue(string rawValue, string fallback)
    {
        return string.IsNullOrWhiteSpace(rawValue) ? fallback : rawValue.Trim();
    }

    private void ClearExistingLevel1SaveForNewJourney()
    {
        bool deletedAny = LocalSaveManager.DeleteActiveProfile();

        string legacyPersistentPath = Path.Combine(Application.persistentDataPath, LocalSaveManager.ProfileFileName);
        deletedAny |= DeleteSaveFileIfPresent(legacyPersistentPath);

        #if UNITY_EDITOR
        string editorSavePath = Path.Combine(Application.dataPath, "_Project", "SaveStates", "Level1", LocalSaveManager.ProfileFileName);
        if (!string.Equals(editorSavePath, legacyPersistentPath, System.StringComparison.OrdinalIgnoreCase))
        {
            deletedAny |= DeleteSaveFileIfPresent(editorSavePath);
        }
        #endif

        if (deletedAny)
        {
            Debug.Log("[MainMenuSaveFlow] Cleared existing Level 1 profile before starting New Journey.");
        }
        else
        {
            Debug.Log("[MainMenuSaveFlow] No existing Level 1 profile found to clear before starting New Journey.");
        }
    }

    private static bool DeleteSaveFileIfPresent(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[MainMenuSaveFlow] Failed to delete save file '{path}': {ex.Message}");
            return false;
        }
    }

    private static void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    private static void PlayMenuClick()
    {
        MainMenuAudioController.Instance?.PlayClick();
    }
}
