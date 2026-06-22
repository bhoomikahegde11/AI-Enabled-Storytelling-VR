using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPromptManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    public Image iconImage;
    public TMP_Text instructionText;


    [Header("Icons")]
    public Sprite triggerIcon;
    public Sprite wristRotateIcon;


    private void Awake()
    {
        Hide();
    }


    public void ShowTrigger(string message)
    {
        ShowPrompt(
            triggerIcon,
            message
        );
    }


    public void ShowWristRotate(string message)
    {
        ShowPrompt(
            wristRotateIcon,
            message
        );
    }


    private void ShowPrompt(
        Sprite icon,
        string message
    )
    {
        if (panel != null)
            panel.SetActive(true);


        if (iconImage != null)
            iconImage.sprite = icon;


        if (instructionText != null)
            instructionText.text = message;
    }


    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}