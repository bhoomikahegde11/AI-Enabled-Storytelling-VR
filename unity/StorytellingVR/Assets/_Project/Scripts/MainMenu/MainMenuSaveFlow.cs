using System;
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

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuRoot;
    [SerializeField] private GameObject saveSelectPanel;
    [SerializeField] private GameObject newSavePanel;

    [Header("Existing Menu Styles")]
    [SerializeField] private Button slotButtonTemplate;
    [SerializeField] private Button secondaryButtonTemplate;
    [SerializeField] private Button slotBackButton;
    [SerializeField] private TMP_Text slotTitle;
    [SerializeField] private TMP_Text slotHint;
    [SerializeField] private TMP_Text newSaveTitle;
    [SerializeField] private TMP_Text newSaveHint;
    [SerializeField] private TMP_Text startJourneyLabel;

    [Header("New Save Fields")]
    [SerializeField] private TMP_InputField saveNameInput;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private Toggle maleToggle;
    [SerializeField] private Toggle femaleToggle;
    [SerializeField] private TMP_Dropdown genderDropdown;

    private enum SlotMode { NewJourney, ContinueJourney }
    private SlotMode mode;
    private readonly Button[] slotButtons = new Button[LocalSaveManager.SlotCount];
    private readonly Button[] deleteButtons = new Button[LocalSaveManager.SlotCount];
    private GameObject deleteConfirmation;
    private TMP_Text deleteTitle;
    private TMP_Text deleteHint;
    private Button deleteConfirmButton;
    private int selectedSlot;
    private bool journeyStarting;
    private bool formOpenedForOccupiedSlot;
    private float originalStartFontSize;
    private float journeyRequestTime;
    private float nextActionTime;

    private void Awake()
    {
        if (startJourneyLabel != null) originalStartFontSize = startJourneyLabel.fontSize;
        BuildSlotControls();
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
    }

    // Kept for the scene-authored New Journey button.
    public void OpenSaveSelect() => OpenSlots(SlotMode.NewJourney);

    public void OpenContinueSlots() => OpenSlots(SlotMode.ContinueJourney);

    public void ExitGame()
    {
        PlayMenuClick();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void OpenSlots(SlotMode requestedMode)
    {
        if (journeyStarting || saveSelectPanel == null || slotButtons[0] == null) return;
        PlayMenuClick();
        mode = requestedMode;
        selectedSlot = 0;
        if (slotTitle != null) slotTitle.text = requestedMode == SlotMode.NewJourney ? "NEW JOURNEY" : "CONTINUE JOURNEY";
        if (slotHint != null) slotHint.text = requestedMode == SlotMode.NewJourney
            ? "Choose a slot. Occupied slots require confirmation."
            : "Choose a saved journey. Empty slots cannot be resumed.";
        RefreshSlots();
        SetPanelActive(mainMenuRoot, false);
        SetPanelActive(saveSelectPanel, true);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
        nextActionTime = Time.unscaledTime + 0.25f;
    }

    public void CloseSaveSelect()
    {
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(deleteConfirmation, false);
        selectedSlot = 0;
    }

    public void Back()
    {
        if (journeyStarting) return;
        PlayMenuClick();
        if (deleteConfirmation != null && deleteConfirmation.activeSelf)
        {
            ShowSlotList();
            return;
        }
        if (newSavePanel != null && newSavePanel.activeSelf)
        {
            ShowSlotList();
            return;
        }
        CloseSaveSelect();
    }

    // Former buttons in SaveSelectPanel are hidden at runtime; preserve these public hooks for older scene copies.
    public void ContinueExistingSave() => OpenContinueSlots();
    public void OpenNewSave() => OpenSaveSelect();

    private void SelectSlot(int slot)
    {
        if (journeyStarting || Time.unscaledTime < nextActionTime || !LocalSaveManager.IsValidSlot(slot)) return;
        if (mode == SlotMode.ContinueJourney)
        {
            if (!LocalSaveManager.TryLoadSlot(slot, out LocalProfileData profile) ||
                !GameManager.Instance.TryGetContinueScene(profile, out _)) return;
            if (GameManager.Instance.TryContinueJourneyInSlot(slot))
            {
                journeyStarting = true;
                journeyRequestTime = Time.unscaledTime;
                PlayMenuClick();
            }
            else RefreshSlots();
            return;
        }

        selectedSlot = slot;
        ResetNewSaveForm();
        bool overwriting = LocalSaveManager.SlotHasAnyData(slot);
        formOpenedForOccupiedSlot = overwriting;
        if (newSaveTitle != null) newSaveTitle.text = overwriting ? $"OVERWRITE SLOT {slot}?" : $"NEW JOURNEY: SLOT {slot}";
        if (newSaveHint != null) newSaveHint.text = overwriting
            ? "This replaces this slot only. Confirm below to begin again."
            : "Choose a name and begin at the start of the story.";
        if (startJourneyLabel != null)
        {
            startJourneyLabel.text = overwriting ? "Overwrite & Begin" : "Start Journey";
            startJourneyLabel.fontSize = overwriting ? Mathf.Min(originalStartFontSize, 43f) : originalStartFontSize;
        }
        PlayMenuClick();
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(newSavePanel, true);
        nextActionTime = Time.unscaledTime + 0.45f;
    }

    public void StartNewSave()
    {
        if (journeyStarting || Time.unscaledTime < nextActionTime || !LocalSaveManager.IsValidSlot(selectedSlot)) return;
        if (!formOpenedForOccupiedSlot && LocalSaveManager.SlotHasAnyData(selectedSlot))
        {
            formOpenedForOccupiedSlot = true;
            if (newSaveTitle != null) newSaveTitle.text = $"OVERWRITE SLOT {selectedSlot}?";
            if (newSaveHint != null) newSaveHint.text = "This slot is now occupied. Confirm overwrite below.";
            if (startJourneyLabel != null)
            {
                startJourneyLabel.text = "Overwrite & Begin";
                startJourneyLabel.fontSize = Mathf.Min(originalStartFontSize, 43f);
            }
            nextActionTime = Time.unscaledTime + 0.45f;
            return;
        }
        string saveName = SanitizeValue(saveNameInput != null ? saveNameInput.text : null, $"Slot {selectedSlot}");
        string characterName = SanitizeValue(characterNameInput != null ? characterNameInput.text : null, "Merchant");
        if (GameManager.Instance.TryStartNewJourneyInSlot(selectedSlot, saveName, characterName,
                GetSelectedGender(), formOpenedForOccupiedSlot))
        {
            journeyStarting = true;
            journeyRequestTime = Time.unscaledTime;
            PlayMenuClick();
        }
        else if (newSaveHint != null)
        {
            newSaveHint.text = "Could not start. The existing save is unchanged.";
        }
    }

    private void RequestDelete(int slot)
    {
        if (journeyStarting || Time.unscaledTime < nextActionTime || !LocalSaveManager.SlotHasAnyData(slot)) return;
        selectedSlot = slot;
        if (deleteTitle != null) deleteTitle.text = $"DELETE SLOT {slot}?";
        if (deleteHint != null) deleteHint.text = "This removes only this slot's journey. The other slots and app settings stay unchanged.";
        PlayMenuClick();
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(deleteConfirmation, true);
        if (deleteConfirmButton != null)
        {
            deleteConfirmButton.interactable = false;
            MenuButtonHoverEffect hover = deleteConfirmButton.GetComponent<MenuButtonHoverEffect>();
            if (hover != null) hover.enabled = false;
        }
        nextActionTime = Time.unscaledTime + 0.45f;
    }

    private void Update()
    {
        if (journeyStarting && Time.unscaledTime > journeyRequestTime + 1f &&
            SceneManager.GetActiveScene().name == GameManager.DefaultMainMenuSceneName &&
            !GameManager.Instance.IsLoading)
        {
            journeyStarting = false;
            if (slotHint != null) slotHint.text = "The scene could not be loaded. Your slot is still available.";
            ShowSlotList();
        }
        if (deleteConfirmButton != null && deleteConfirmation != null && deleteConfirmation.activeSelf &&
            !deleteConfirmButton.interactable && Time.unscaledTime >= nextActionTime)
        {
            deleteConfirmButton.interactable = true;
            MenuButtonHoverEffect hover = deleteConfirmButton.GetComponent<MenuButtonHoverEffect>();
            if (hover != null) hover.enabled = true;
        }
    }

    private void ConfirmDelete()
    {
        if (Time.unscaledTime < nextActionTime || !LocalSaveManager.IsValidSlot(selectedSlot)) return;
        int slot = selectedSlot;
        if (!LocalSaveManager.DeleteSlot(slot))
        {
            if (deleteHint != null) deleteHint.text = "Deletion failed. The save may still be present; please try again.";
            return;
        }
        if (LocalSaveManager.ActiveSlot == slot) LocalSaveManager.ClearActiveSlot();
        PlayMenuClick();
        ShowSlotList();
    }

    private void ShowSlotList()
    {
        RefreshSlots();
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
        SetPanelActive(saveSelectPanel, true);
        nextActionTime = Time.unscaledTime + 0.25f;
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < LocalSaveManager.SlotCount; i++)
        {
            int slot = i + 1;
            bool hasData = LocalSaveManager.SlotHasAnyData(slot);
            bool valid = LocalSaveManager.TryLoadSlot(slot, out LocalProfileData profile);
            string chapter = string.Empty;
            bool resumable = valid && GameManager.Instance.TryGetContinueScene(profile, out chapter);
            Button choose = slotButtons[i];
            Button delete = deleteButtons[i];
            if (choose != null)
            {
                choose.interactable = mode == SlotMode.NewJourney || resumable;
                MenuButtonHoverEffect hover = choose.GetComponent<MenuButtonHoverEffect>();
                if (hover != null) hover.enabled = choose.interactable;
                if (!choose.interactable) choose.transform.localScale = Vector3.one;
                TMP_Text label = choose.GetComponentInChildren<TMP_Text>(true);
                if (label != null)
                {
                    label.text = !hasData ? $"Slot {slot} - Empty Slot" :
                        !valid ? $"Slot {slot} - Save needs repair" :
                        !resumable ? $"Slot {slot} - Checkpoint unavailable" :
                        $"Slot {slot} - {ChapterLabel(chapter)}\nSaved {LastSavedLabel(slot)}";
                    Color color = label.color;
                    color.a = choose.interactable ? 1f : 0.55f;
                    label.color = color;
                }
            }
            if (delete != null) delete.gameObject.SetActive(hasData);
        }
    }

    private static string ChapterLabel(string sceneName)
    {
        switch (sceneName)
        {
            case "ModernStudyRoom": return "The Study";
            case "GateIntro": return "The Gateway";
            case "FreeRoam_WithTerrain": return "Hampi Free Roam";
            case "NewSpiceScene": return "Spice Tutorial";
            case "NewTransactionTutorial": return "Trade Tutorial";
            case "NewCoinScene": return "Coin Tutorial";
            case "SpicesInteraction": return "Spice Interaction";
            case "Level1_MainLoopUpdated": return "The Marketplace";
            default: return sceneName;
        }
    }

    private static string LastSavedLabel(int slot)
    {
        try
        {
            string path = LocalSaveManager.GetSlotSavePath(slot);
            return File.Exists(path) ? File.GetLastWriteTime(path).ToString("dd MMM yyyy, HH:mm") : "date unavailable";
        }
        catch { return "date unavailable"; }
    }

    private void BuildSlotControls()
    {
        if (saveSelectPanel == null || slotButtonTemplate == null || secondaryButtonTemplate == null ||
            slotTitle == null || slotHint == null)
        {
            Debug.LogError("[MainMenuSaveFlow] Slot UI references are incomplete; leaving the main menu available.");
            return;
        }

        // Reuse the scene's artwork, font, Image and hover components. No extra Canvas or XR input module.
        slotButtonTemplate.gameObject.SetActive(false);
        secondaryButtonTemplate.gameObject.SetActive(false);
        for (int i = 0; i < LocalSaveManager.SlotCount; i++)
        {
            int slot = i + 1;
            float y = 117f - i * 180f;
            // Match the panel-centered title, hint and Back; Delete is an independent action to the right.
            slotButtons[i] = CloneButton(slotButtonTemplate, saveSelectPanel.transform,
                $"Slot {slot}", new Vector2(0f, y), new Vector2(770f, 145f), () => SelectSlot(slot));
            deleteButtons[i] = CloneButton(secondaryButtonTemplate, saveSelectPanel.transform,
                "Delete", new Vector2(625f, y), new Vector2(280f, 125f), () => RequestDelete(slot));
            TMP_Text label = slotButtons[i].GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.fontSize = 43f;
            TMP_Text deleteLabel = deleteButtons[i].GetComponentInChildren<TMP_Text>(true);
            if (deleteLabel != null) deleteLabel.fontSize = 47f;
        }
        if (slotBackButton != null)
        {
            RectTransform rect = slotBackButton.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0f, -398f);
            rect.sizeDelta = new Vector2(650f, 110f);
        }
        BuildDeleteConfirmation();
    }

    private void BuildDeleteConfirmation()
    {
        GameObject panel = new GameObject("Delete Slot Confirmation", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(saveSelectPanel.transform.parent, false);
        RectTransform sourceRect = saveSelectPanel.GetComponent<RectTransform>();
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = sourceRect.anchorMin;
        rect.anchorMax = sourceRect.anchorMax;
        rect.pivot = sourceRect.pivot;
        rect.anchoredPosition = sourceRect.anchoredPosition;
        rect.sizeDelta = sourceRect.sizeDelta;
        Image sourceImage = saveSelectPanel.GetComponent<Image>();
        Image image = panel.GetComponent<Image>();
        image.sprite = sourceImage.sprite;
        image.material = sourceImage.material;
        image.type = sourceImage.type;
        image.preserveAspect = sourceImage.preserveAspect;
        image.color = sourceImage.color;
        image.raycastTarget = true;
        deleteConfirmation = panel;

        deleteTitle = Instantiate(slotTitle, panel.transform);
        deleteTitle.name = "Delete title";
        deleteTitle.raycastTarget = false;
        deleteTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 330f);
        deleteHint = Instantiate(slotHint, panel.transform);
        deleteHint.name = "Delete warning";
        deleteHint.raycastTarget = false;
        RectTransform hintRect = deleteHint.GetComponent<RectTransform>();
        hintRect.anchoredPosition = new Vector2(0f, 90f);
        hintRect.sizeDelta = new Vector2(1550f, 260f);
        deleteHint.enableWordWrapping = true;

        deleteConfirmButton = CloneButton(slotButtonTemplate, panel.transform, "Delete Slot",
            new Vector2(-390f, -255f), new Vector2(600f, 150f), ConfirmDelete);
        CloneButton(secondaryButtonTemplate, panel.transform, "Cancel",
            new Vector2(390f, -255f), new Vector2(600f, 150f), Back);
        panel.SetActive(false);
    }

    private static Button CloneButton(Button template, Transform parent, string label,
        Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        Button button = Instantiate(template, parent);
        button.gameObject.name = label;
        button.onClick = new Button.ButtonClickedEvent(); // Discard the old scene-authored persistent action.
        button.onClick.AddListener(action);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = label;
            text.raycastTarget = false;
        }
        button.gameObject.SetActive(true);
        return button;
    }

    private string GetSelectedGender()
    {
        if (maleToggle != null && maleToggle.isOn) return "Male";
        if (femaleToggle != null && femaleToggle.isOn) return "Female";
        if (genderDropdown != null && genderDropdown.options != null && genderDropdown.options.Count > 0)
        {
            int index = Mathf.Clamp(genderDropdown.value, 0, genderDropdown.options.Count - 1);
            return SanitizeValue(genderDropdown.options[index].text, "Male");
        }
        return "Male";
    }

    private int FindGenderOptionIndex(string gender)
    {
        if (genderDropdown == null || genderDropdown.options == null) return -1;
        for (int i = 0; i < genderDropdown.options.Count; i++)
            if (string.Equals(genderDropdown.options[i].text, gender, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    private void ResetNewSaveForm()
    {
        if (saveNameInput != null) saveNameInput.SetTextWithoutNotify(string.Empty);
        if (characterNameInput != null) characterNameInput.SetTextWithoutNotify(string.Empty);
        if (maleToggle != null) maleToggle.SetIsOnWithoutNotify(true);
        if (femaleToggle != null) femaleToggle.SetIsOnWithoutNotify(false);
        if (genderDropdown != null && genderDropdown.options != null && genderDropdown.options.Count > 0)
        {
            int male = FindGenderOptionIndex("Male");
            genderDropdown.SetValueWithoutNotify(male >= 0 ? male : 0);
        }
    }

    private static string SanitizeValue(string value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    private static void SetPanelActive(GameObject panel, bool active) { if (panel != null) panel.SetActive(active); }
    private static void PlayMenuClick() => MainMenuAudioController.Instance?.PlayClick();
}
