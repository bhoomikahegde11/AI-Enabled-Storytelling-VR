using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI captionText; // Drag your BottomCaption here

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
        // Display the first line immediately
        UpdateUI();
    }

    void Update()
    {
        // Detect Mouse Click (Temporary for Demo)
        if (Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue();
        }
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
            captionText.text = ""; // Clear text when tutorial ends
            Debug.Log("Tutorial Finished. Starting 5-minute timer.");
        }
    }

    void UpdateUI()
    {
        captionText.text = dialogues[currentIndex];
    }
}