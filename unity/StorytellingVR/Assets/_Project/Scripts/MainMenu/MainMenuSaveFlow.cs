using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSaveFlow : MonoBehaviour
{
    public const string SaveNameKey = "MainMenu.SelectedSaveName";
    public const string CharacterNameKey = "MainMenu.SelectedCharacterName";
    public const string GenderKey = "MainMenu.SelectedGender";

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

    private void Awake()
    {
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(newSavePanel, false);
    }

    public void OpenSaveSelect()
    {
        PlayMenuClick();
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
        CloseSaveSelect();
        GameManager.Instance.ContinueJourney();
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
        CloseSaveSelect();
        GameManager.Instance.StartNewJourney();
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
