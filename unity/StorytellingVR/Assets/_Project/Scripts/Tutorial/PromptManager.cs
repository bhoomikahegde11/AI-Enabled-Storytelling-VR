using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text promptText;
    public Image buttonIcon;

    [Header("Icons")]
    public Sprite aButton;
    public Sprite bButton;
    public Sprite xButton;
    public Sprite yButton;
    public Sprite leftTriggerButton;
    public Sprite rightTriggerButton;
    public Sprite gripButton;
    public Sprite thumbstickButton;

    [Header("Prompt Audio")]
    public AudioSource audioSource;
    public AudioClip promptAppearSound;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowPrompt(string message, Sprite icon)
    {
        promptText.text = message;
        buttonIcon.sprite = icon;

        panel.SetActive(true);

        if (promptAppearSound != null)
            audioSource.PlayOneShot(promptAppearSound);
    }

    public void HidePrompt()
    {
        panel.SetActive(false);
    }
}