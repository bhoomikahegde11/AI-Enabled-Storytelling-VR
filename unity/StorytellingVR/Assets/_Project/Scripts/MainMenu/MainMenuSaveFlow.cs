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
    private enum ConfirmationKind { None, Delete, Overwrite }
    private SlotMode mode;
    private ConfirmationKind confirmationKind;
    private readonly Button[] slotButtons = new Button[LocalSaveManager.SlotCount];
    private readonly Button[] deleteButtons = new Button[LocalSaveManager.SlotCount];
    private GameObject deleteConfirmation;
    private TMP_Text deleteTitle;
    private TMP_Text deleteHint;
    private Button deleteConfirmButton;
    private int selectedSlot;
    private bool journeyStarting;
    private float originalStartFontSize;
    private float originalSlotHintFontSize;
    private Vector2 originalSlotHintSize;
    private float journeyRequestTime;
    private float nextActionTime;

    private void Awake()
    {
        if (startJourneyLabel != null) originalStartFontSize = startJourneyLabel.fontSize;
        if (slotHint != null)
        {
            originalSlotHintFontSize = slotHint.fontSize;
            originalSlotHintSize = slotHint.rectTransform.sizeDelta;
        }
        if (newSaveHint != null)
            newSaveHint.rectTransform.sizeDelta = new Vector2(newSaveHint.rectTransform.sizeDelta.x, 100f);
        BuildSlotControls();
        PreparePanel(mainMenuRoot, true);
        PreparePanel(saveSelectPanel, false, slotTitle);
        PreparePanel(newSavePanel, false, newSaveTitle);
        PreparePanel(deleteConfirmation, false, deleteTitle);
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
    }

    // Kept for the scene-authored New Journey button; the form is now the first screen.
    public void OpenSaveSelect()
    {
        if (journeyStarting || newSavePanel == null) return;
        PlayMenuClick();
        mode = SlotMode.NewJourney;
        selectedSlot = 0;
        ResetNewSaveForm();
        ShowNewSaveForm();
    }

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
        if (slotTitle != null) slotTitle.text = "CONTINUE JOURNEY";
        if (slotHint != null) slotHint.text = "Choose a saved journey to continue.";
        RestoreSlotHintLayout();
        RefreshSlots();
        SetPanelActive(mainMenuRoot, false);
        SetPanelActive(saveSelectPanel, true);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
        confirmationKind = ConfirmationKind.None;
        nextActionTime = Time.unscaledTime + 0.25f;
    }

    public void CloseSaveSelect()
    {
        SetPanelActive(mainMenuRoot, true);
        SetPanelActive(newSavePanel, false);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(deleteConfirmation, false);
        selectedSlot = 0;
        confirmationKind = ConfirmationKind.None;
    }

    public void Back()
    {
        if (journeyStarting) return;
        PlayMenuClick();
        if (deleteConfirmation != null && IsPanelVisible(deleteConfirmation))
        {
            ShowSlotList();
            return;
        }
        if (newSavePanel != null && IsPanelVisible(newSavePanel))
        {
            CloseSaveSelect();
            return;
        }
        if (saveSelectPanel != null && IsPanelVisible(saveSelectPanel) && mode == SlotMode.NewJourney)
        {
            ShowNewSaveForm();
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

        // The New Journey slot list is shown only when every slot is occupied.
        if (!LocalSaveManager.SlotHasAnyData(slot) || !LocalSaveManager.TryLoadSlot(slot, out _))
        {
            RefreshSlots();
            return;
        }
        if (!TryReadNewJourneyForm(out string saveName, out _, out _))
        {
            if (slotHint != null) slotHint.text = "Journey details are incomplete. Go Back to the form.";
            return;
        }
        int nameConflict = LocalSaveManager.FindJourneyNameConflict(saveName, slot);
        if (nameConflict != 0)
        {
            if (slotHint != null) slotHint.text = $"That name belongs to Slot {nameConflict}. Choose it or go Back and change the name.";
            return;
        }
        selectedSlot = slot;
        PlayMenuClick();
        ShowOverwriteConfirmation(slot);
    }

    public void StartNewSave()
    {
        if (journeyStarting || Time.unscaledTime < nextActionTime ||
            !TryReadNewJourneyForm(out string saveName, out string characterName, out string gender)) return;

        int emptySlot = LocalSaveManager.FirstEmptySlot();
        int nameConflict = LocalSaveManager.FindJourneyNameConflict(saveName);
        if (nameConflict != 0 && emptySlot != 0)
        {
            if (newSaveHint != null) newSaveHint.text = "A journey with this name already exists. Please choose a different name.";
            return;
        }

        if (emptySlot == 0)
        {
            ShowOverwriteSlots();
            return;
        }

        if (GameManager.Instance.TryStartNewJourneyInSlot(emptySlot, saveName, characterName, gender, false))
        {
            MarkJourneyStarting();
            return;
        }

        // Another action may have filled the candidate slot before GameManager's final check.
        if (LocalSaveManager.FirstEmptySlot() == 0) ShowOverwriteSlots();
        else if (newSaveHint != null) newSaveHint.text = LocalSaveManager.FindJourneyNameConflict(saveName) != 0
            ? "A journey with this name already exists. Please choose a different name."
            : "Could not start. Your saved journeys are unchanged; please try again.";
    }

    private void RequestDelete(int slot)
    {
        if (journeyStarting || mode != SlotMode.ContinueJourney || Time.unscaledTime < nextActionTime ||
            !LocalSaveManager.SlotHasAnyData(slot)) return;
        selectedSlot = slot;
        confirmationKind = ConfirmationKind.Delete;
        if (deleteTitle != null) deleteTitle.text = $"DELETE SLOT {slot}?";
        if (deleteHint != null) deleteHint.text = "This removes only this slot's journey. The other slots and app settings stay unchanged.";
        SetConfirmationButtonLabel("Delete Slot");
        PlayMenuClick();
        SetPanelActive(saveSelectPanel, false);
        OpenConfirmation();
    }

    private void Update()
    {
        if (journeyStarting && Time.unscaledTime > journeyRequestTime + 1f &&
            SceneManager.GetActiveScene().name == GameManager.DefaultMainMenuSceneName &&
            !GameManager.Instance.IsLoading)
        {
            journeyStarting = false;
            if (mode == SlotMode.NewJourney)
            {
                ShowNewSaveForm();
                if (newSaveHint != null) newSaveHint.text = "The opening scene could not be loaded. Your journey was saved; use Continue to try again.";
            }
            else
            {
                if (slotHint != null) slotHint.text = "The saved scene could not be loaded. Please try again.";
                ShowSlotList();
            }
        }
        if (deleteConfirmButton != null && deleteConfirmation != null && IsPanelVisible(deleteConfirmation) &&
            !deleteConfirmButton.interactable && Time.unscaledTime >= nextActionTime)
        {
            deleteConfirmButton.interactable = true;
            MenuButtonHoverEffect hover = deleteConfirmButton.GetComponent<MenuButtonHoverEffect>();
            if (hover != null) hover.enabled = true;
        }
    }

    private void ConfirmSlotAction()
    {
        if (Time.unscaledTime < nextActionTime || !LocalSaveManager.IsValidSlot(selectedSlot)) return;
        if (confirmationKind == ConfirmationKind.Overwrite)
        {
            if (!TryReadNewJourneyForm(out string saveName, out string characterName, out string gender) ||
                !LocalSaveManager.TryLoadSlot(selectedSlot, out _) ||
                LocalSaveManager.FindJourneyNameConflict(saveName, selectedSlot) != 0)
            {
                if (deleteHint != null) deleteHint.text = "The slot or journey name changed. Go Back and review your choice; no save was replaced.";
                return;
            }
            if (GameManager.Instance.TryStartNewJourneyInSlot(selectedSlot, saveName, characterName, gender, true))
            {
                MarkJourneyStarting();
                return;
            }
            if (deleteHint != null) deleteHint.text = "Could not begin. The selected journey was not replaced; go Back and try again.";
            return;
        }
        if (confirmationKind != ConfirmationKind.Delete) return;
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
        confirmationKind = ConfirmationKind.None;
        RefreshSlots();
        SetPanelActive(newSavePanel, false);
        SetPanelActive(deleteConfirmation, false);
        SetPanelActive(saveSelectPanel, true);
        nextActionTime = Time.unscaledTime + 0.25f;
    }

    private void ShowNewSaveForm()
    {
        confirmationKind = ConfirmationKind.None;
        if (newSaveTitle != null) newSaveTitle.text = "NEW JOURNEY";
        if (newSaveHint != null) newSaveHint.text = "Name your journey and character to begin your story.";
        if (startJourneyLabel != null)
        {
            startJourneyLabel.text = "Start Journey";
            startJourneyLabel.fontSize = originalStartFontSize;
        }
        SetPanelActive(mainMenuRoot, false);
        SetPanelActive(saveSelectPanel, false);
        SetPanelActive(deleteConfirmation, false);
        SetPanelActive(newSavePanel, true);
        nextActionTime = Time.unscaledTime + 0.25f;
    }

    private void ShowOverwriteSlots()
    {
        mode = SlotMode.NewJourney;
        selectedSlot = 0;
        if (slotTitle != null) slotTitle.text = "CHOOSE A SLOT";
        if (slotHint != null)
        {
            slotHint.text = "All three journey slots are full. Choose a journey to overwrite to begin a new one.";
            slotHint.fontSize = Mathf.Min(originalSlotHintFontSize, 45f);
            slotHint.rectTransform.sizeDelta = new Vector2(originalSlotHintSize.x, 105f);
        }
        RefreshSlots();
        SetPanelActive(newSavePanel, false);
        SetPanelActive(saveSelectPanel, true);
        nextActionTime = Time.unscaledTime + 0.45f;
    }

    private void RestoreSlotHintLayout()
    {
        if (slotHint == null) return;
        slotHint.fontSize = originalSlotHintFontSize;
        slotHint.rectTransform.sizeDelta = originalSlotHintSize;
    }

    private void ShowOverwriteConfirmation(int slot)
    {
        confirmationKind = ConfirmationKind.Overwrite;
        LocalSaveManager.TryLoadSlot(slot, out LocalProfileData existing);
        string name = string.IsNullOrWhiteSpace(existing?.save_name)
            ? $"Slot {slot}" : existing.save_name.Trim();
        // Bound only the display copy so an exceptionally long saved name cannot hide the warning.
        if (name.Length > 80) name = name.Substring(0, char.IsHighSurrogate(name[79]) ? 79 : 80) + "...";
        if (deleteTitle != null) deleteTitle.text = $"OVERWRITE SLOT {slot}?";
        if (deleteHint != null) deleteHint.text =
            $"Permanently replace {name} and its progress? Only Slot {slot} will be overwritten. This cannot be undone.";
        SetConfirmationButtonLabel("Overwrite & Begin");
        SetPanelActive(saveSelectPanel, false);
        OpenConfirmation();
    }

    private void OpenConfirmation()
    {
        SetPanelActive(deleteConfirmation, true);
        if (deleteConfirmButton != null)
        {
            deleteConfirmButton.interactable = false;
            MenuButtonHoverEffect hover = deleteConfirmButton.GetComponent<MenuButtonHoverEffect>();
            if (hover != null) hover.enabled = false;
        }
        nextActionTime = Time.unscaledTime + 0.45f;
    }

    private void SetConfirmationButtonLabel(string label)
    {
        if (deleteConfirmButton == null) return;
        TMP_Text text = deleteConfirmButton.GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = label;
    }

    private bool TryReadNewJourneyForm(out string saveName, out string characterName, out string gender)
    {
        saveName = (saveNameInput != null ? saveNameInput.text ?? string.Empty : string.Empty).Trim();
        characterName = (characterNameInput != null ? characterNameInput.text ?? string.Empty : string.Empty).Trim();
        gender = GetSelectedGender();
        if (saveName.Length > 0 && characterName.Length > 0 &&
            ((maleToggle != null && maleToggle.isOn) || (femaleToggle != null && femaleToggle.isOn) || genderDropdown != null))
            return true;
        if (newSaveHint != null) newSaveHint.text = "Enter a journey name, your name and a gender before beginning.";
        return false;
    }

    private void MarkJourneyStarting()
    {
        journeyStarting = true;
        journeyRequestTime = Time.unscaledTime;
        PlayMenuClick();
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
                choose.interactable = mode == SlotMode.NewJourney ? hasData && valid : resumable;
                MenuButtonHoverEffect hover = choose.GetComponent<MenuButtonHoverEffect>();
                if (hover != null) hover.enabled = choose.interactable;
                if (!choose.interactable) choose.transform.localScale = Vector3.one;
                MainMenuSlotView view = choose.GetComponent<MainMenuSlotView>();
                if (view != null)
                    view.SetContent(slot, valid ? profile.save_name : null, ChapterLabel(chapter),
                        valid ? LastSavedLabel(slot) : string.Empty, hasData, valid, resumable, choose.interactable);
            }
            if (delete != null) delete.gameObject.SetActive(hasData && mode == SlotMode.ContinueJourney);
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
            float y = 160f - i * 225f;
            // Match the panel-centered title, hint and Back; Delete is an independent action to the right.
            slotButtons[i] = CloneButton(slotButtonTemplate, saveSelectPanel.transform,
                $"Slot {slot}", new Vector2(0f, y), new Vector2(940f, 210f), () => SelectSlot(slot));
            deleteButtons[i] = CloneButton(secondaryButtonTemplate, saveSelectPanel.transform,
                "Delete", new Vector2(590f, y - 30f), new Vector2(200f, 100f), () => RequestDelete(slot));
            TMP_Text label = slotButtons[i].GetComponentInChildren<TMP_Text>(true);
            if (label != null)
                slotButtons[i].gameObject.AddComponent<MainMenuSlotView>().Initialize(label, slotHint);
            TMP_Text deleteLabel = deleteButtons[i].GetComponentInChildren<TMP_Text>(true);
            if (deleteLabel != null) deleteLabel.fontSize = 34f;
        }
        if (slotBackButton != null)
        {
            RectTransform rect = slotBackButton.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0f, -490f);
            rect.sizeDelta = new Vector2(520f, 110f);
        }
        BuildDeleteConfirmation();
    }

    private void BuildDeleteConfirmation()
    {
        GameObject panel = new GameObject("Save Slot Confirmation", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
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
        deleteTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 300f);
        deleteHint = Instantiate(slotHint, panel.transform);
        deleteHint.name = "Delete warning";
        deleteHint.raycastTarget = false;
        RectTransform hintRect = deleteHint.GetComponent<RectTransform>();
        hintRect.anchoredPosition = new Vector2(0f, 70f);
        hintRect.sizeDelta = new Vector2(1160f, 250f);
        deleteHint.textWrappingMode = TextWrappingModes.Normal;
        deleteHint.richText = false;
        deleteHint.fontSize = 38f;
        deleteHint.enableAutoSizing = true;
        deleteHint.fontSizeMin = 32f;
        deleteHint.fontSizeMax = 38f;

        deleteConfirmButton = CloneButton(slotButtonTemplate, panel.transform, "Delete Slot",
            new Vector2(-290f, -310f), new Vector2(520f, 160f), ConfirmSlotAction);
        CloneButton(secondaryButtonTemplate, panel.transform, "Cancel",
            new Vector2(290f, -310f), new Vector2(520f, 160f), Back);
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
    private static void PreparePanel(GameObject panel, bool visible, TMP_Text title = null)
    {
        if (panel == null) return;
        MainMenuPanelTransition transition = panel.GetComponent<MainMenuPanelTransition>();
        if (transition == null) transition = panel.AddComponent<MainMenuPanelTransition>();
        transition.Initialize(visible, title != null ? title.rectTransform : null);
    }

    private static bool IsPanelVisible(GameObject panel)
    {
        if (panel == null) return false;
        MainMenuPanelTransition transition = panel.GetComponent<MainMenuPanelTransition>();
        return transition != null ? transition.IsVisible : panel.activeSelf;
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel == null) return;
        MainMenuPanelTransition transition = panel.GetComponent<MainMenuPanelTransition>();
        if (transition != null) transition.SetVisible(active, Application.isPlaying);
        else panel.SetActive(active);
    }
    private static void PlayMenuClick() => MainMenuAudioController.Instance?.PlayClick();
}
