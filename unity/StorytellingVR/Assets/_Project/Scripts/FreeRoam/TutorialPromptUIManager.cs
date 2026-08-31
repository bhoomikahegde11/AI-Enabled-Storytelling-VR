using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPromptUIManager : MonoBehaviour
{
    public static TutorialPromptUIManager Instance;

    [Header("UI")]
    public GameObject promptCanvas;
    public TMP_Text promptTitleText;
    public TMP_Text promptBodyText;

    [Header("Prompt Icon")]
    [SerializeField] private Image promptIcon;
    [SerializeField] private Sprite leftJoystickSprite;
    [SerializeField] private Sprite rightTriggerSprite;

    // Keeps track of which object currently owns the prompt.
    // This preserves the existing owner-based system in your project.
    private MonoBehaviour currentOwner;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePrompt();
    }

    // --------------------------------------------------
    // EXISTING NO-OWNER VERSION
    // --------------------------------------------------

    public void ShowPrompt(string title, string body)
    {
        ShowPromptInternal(title, body, null);
    }

    public void HidePrompt()
    {
        HidePromptInternal(null, false);
    }

    // --------------------------------------------------
    // EXISTING OWNER VERSION
    // --------------------------------------------------

    public void ShowPrompt(
        string title,
        string body,
        MonoBehaviour owner)
    {
        ShowPromptInternal(title, body, owner);
    }

    public void HidePrompt(MonoBehaviour owner)
    {
        HidePromptInternal(owner, true);
    }

    // --------------------------------------------------
    // NEW LEFT JOYSTICK PROMPT
    // --------------------------------------------------

    public void ShowLeftJoystickPrompt(
        string title,
        string body,
        MonoBehaviour owner = null)
    {
        ShowPromptInternal(title, body, owner);

        if (promptIcon != null && leftJoystickSprite != null)
        {
            promptIcon.sprite = leftJoystickSprite;
            promptIcon.gameObject.SetActive(true);
        }
    }

    // --------------------------------------------------
    // NEW RIGHT TRIGGER PROMPT
    // --------------------------------------------------

    public void ShowRightTriggerPrompt(
        string title,
        string body,
        MonoBehaviour owner = null)
    {
        ShowPromptInternal(title, body, owner);

        if (promptIcon != null && rightTriggerSprite != null)
        {
            promptIcon.sprite = rightTriggerSprite;
            promptIcon.gameObject.SetActive(true);
        }
    }

    // --------------------------------------------------
    // INTERNAL PROMPT CONTROL
    // --------------------------------------------------

    private void ShowPromptInternal(
        string title,
        string body,
        MonoBehaviour owner)
    {
        // If another owner currently controls the prompt,
        // don't overwrite it unless this is an ownerless prompt.
        if (currentOwner != null &&
            owner != null &&
            currentOwner != owner)
        {
            return;
        }

        currentOwner = owner;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);

        if (promptTitleText != null)
            promptTitleText.text = title;

        if (promptBodyText != null)
            promptBodyText.text = body;
    }

    private void HidePromptInternal(
        MonoBehaviour owner,
        bool checkOwner)
    {
        if (checkOwner &&
            currentOwner != null &&
            currentOwner != owner)
        {
            return;
        }

        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        currentOwner = null;
    }
}