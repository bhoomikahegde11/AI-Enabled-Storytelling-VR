using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class VoiceRecognitionManager : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public TMP_Text spokenPriceText;

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SubmitPrice(200);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SubmitPrice(70);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SubmitPrice(60);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SubmitPrice(40);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SubmitPrice(500);
        }
    }

    public void SubmitPrice(int price)
    {
        spokenPriceText.text = "Spoken Price: " + price + " Varahas";
        tutorialManager.HandlePlayerOffer(price);
    }
}