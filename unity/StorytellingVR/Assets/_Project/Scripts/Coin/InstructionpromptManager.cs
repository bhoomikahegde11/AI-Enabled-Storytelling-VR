using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPromptManager : MonoBehaviour
{
    public GameObject panel;

    public Image iconImage;
    public TMP_Text instructionText;


    public Sprite aButtonIcon;
    public Sprite joystickIcon;


    void Awake()
    {
        Hide();
    }


    public void ShowAButton(string message)
    {
        panel.SetActive(true);

        iconImage.sprite =
            aButtonIcon;

        instructionText.text =
            message;
    }


    public void ShowJoystick(string message)
    {
        panel.SetActive(true);

        iconImage.sprite =
            joystickIcon;

        instructionText.text =
            message;
    }


    public void Hide()
    {
        panel.SetActive(false);
    }
}