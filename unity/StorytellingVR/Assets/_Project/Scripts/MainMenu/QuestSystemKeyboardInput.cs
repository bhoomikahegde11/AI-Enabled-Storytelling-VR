using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestSystemKeyboardInput : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
    [SerializeField] private bool openOnSelect = true;
    [SerializeField] private bool openOnPointerClick = true;

    private TouchScreenKeyboard activeKeyboard;
    private string lastKeyboardText;

    private void Awake()
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }
    }

    private void Update()
    {
        if (!ShouldUseSystemKeyboard() || activeKeyboard == null || inputField == null)
        {
            return;
        }

        string keyboardText = activeKeyboard.text ?? string.Empty;
        if (!string.Equals(lastKeyboardText, keyboardText, System.StringComparison.Ordinal))
        {
            lastKeyboardText = keyboardText;
            inputField.SetTextWithoutNotify(keyboardText);
            inputField.caretPosition = keyboardText.Length;
            inputField.selectionAnchorPosition = keyboardText.Length;
            inputField.selectionFocusPosition = keyboardText.Length;
            inputField.ForceLabelUpdate();
        }

        switch (activeKeyboard.status)
        {
            case TouchScreenKeyboard.Status.Done:
                CompleteKeyboardEdit(true);
                break;

            case TouchScreenKeyboard.Status.Canceled:
                CompleteKeyboardEdit(false);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (openOnPointerClick)
        {
            OpenKeyboardIfNeeded();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (openOnSelect)
        {
            OpenKeyboardIfNeeded();
        }
    }

    private void OpenKeyboardIfNeeded()
    {
        if (!ShouldUseSystemKeyboard() || inputField == null)
        {
            return;
        }

        if (activeKeyboard != null &&
            activeKeyboard.status != TouchScreenKeyboard.Status.Canceled &&
            activeKeyboard.status != TouchScreenKeyboard.Status.Done)
        {
            return;
        }

        string currentText = inputField.text ?? string.Empty;
        string placeholder = inputField.placeholder is TMP_Text placeholderText
            ? placeholderText.text
            : string.Empty;

        activeKeyboard = TouchScreenKeyboard.Open(
            currentText,
            keyboardType,
            false,
            false,
            false,
            false,
            placeholder);

        lastKeyboardText = currentText;
    }

    private void CompleteKeyboardEdit(bool submit)
    {
        if (inputField == null || activeKeyboard == null)
        {
            activeKeyboard = null;
            return;
        }

        string finalText = activeKeyboard.text ?? string.Empty;
        inputField.SetTextWithoutNotify(finalText);
        inputField.ForceLabelUpdate();

        if (submit)
        {
            inputField.onValueChanged?.Invoke(finalText);
            inputField.onSubmit?.Invoke(finalText);
            inputField.onEndEdit?.Invoke(finalText);
        }

        inputField.DeactivateInputField();
        activeKeyboard = null;
    }

    private static bool ShouldUseSystemKeyboard()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}
