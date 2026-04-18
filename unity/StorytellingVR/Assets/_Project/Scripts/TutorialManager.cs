using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // 1. Add this namespace

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI captionText;

    private string[] dialogues = new string[]
    {
        "Welcome to Hampi, the crown jewel of the Vijayanagara Empire.",
        "As a Spice Trader, your goal is to maximize profit in this 5-minute market session.",
        "Look ahead. A Persian Merchant approaches. Our markets attract traders from across the globe.",
        "He wants your pepper. Use your hands to select 'Bargain' to negotiate a better price.",
        "Transaction complete! You've earned your first Gold Varaha.",
        "In 16th-century Hampi, 1 Varaha is worth 10 Gadyanas. Remember this for your taxes.",
        "Now, the market is open. Good luck, Merchant!"
    };

    private int currentIndex = 0;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        // 2. Use the New Input System's Mouse detection
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }

        // 3. (Optional) Also allow the Space bar for easier testing
        //if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        //{
        //    AdvanceDialogue();
        //}
    }

    void AdvanceDialogue()
    {
        currentIndex++;
        if (currentIndex < dialogues.Length)
        {
            UpdateUI();
        }
        else
        {
            captionText.text = "";
            Debug.Log("Tutorial Finished. Transitioning to 5-minute game loop.");
        }
    }

    void UpdateUI()
    {
        captionText.text = dialogues[currentIndex];
    }
}