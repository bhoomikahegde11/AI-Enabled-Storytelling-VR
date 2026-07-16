using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance;

    public GameObject panel;
    public TMP_Text promptText;
    public Image buttonIcon;

    public Sprite aButton;
    public Sprite bButton;
    public Sprite xButton;
    public Sprite yButton;
    public Sprite leftTriggerButton;
    public Sprite rightTriggerButton;
    public Sprite leftGripButton;
    public Sprite rightGripButton;
    public Sprite leftJoyStick;
    public Sprite rightJoyStick;
    public AudioSource audioSource;
    public AudioClip promptSound;
    
    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowPrompt(string message, Sprite icon)
    {
        promptText.text = message;
        buttonIcon.sprite = icon;

        panel.SetActive(true);

        if (promptSound != null)
            audioSource.PlayOneShot(promptSound);
    }

    public void HidePrompt()
    {
        panel.SetActive(false);
    }
}