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

    [Header("Icons")]
    [SerializeField] private Image promptIcon;
    [SerializeField] private Sprite leftJoystickSprite;
    [SerializeField] private Sprite rightTriggerSprite;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePrompt();
    }

    public void ShowPrompt(string title, string body)
    {
        promptCanvas.SetActive(true);

        promptTitleText.text = title;
        promptBodyText.text = body;
    }

    public void ShowLeftJoystickPrompt(string title, string body)
    {
        promptCanvas.SetActive(true);

        promptTitleText.text = title;
        promptBodyText.text = body;

        if (promptIcon != null && leftJoystickSprite != null)
        {
            promptIcon.sprite = leftJoystickSprite;
            promptIcon.gameObject.SetActive(true);
        }
    }

    public void ShowRightTriggerPrompt(string title, string body)
    {
        promptCanvas.SetActive(true);

        promptTitleText.text = title;
        promptBodyText.text = body;

        if (promptIcon != null && rightTriggerSprite != null)
        {
            promptIcon.sprite = rightTriggerSprite;
            promptIcon.gameObject.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}