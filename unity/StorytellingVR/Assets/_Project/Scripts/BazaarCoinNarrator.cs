using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class BazaarCoinNarrator : MonoBehaviour
{
    public TextMeshProUGUI captionText;

    private int index = 0;

    private string[] lines = new string[]
    {
        "That is how transactions are carried out in this market.",
        "After every successful deal, you receive coins known as Varahas.",
        "These coins were widely used during the Vijayanagara Empire for trade and commerce.",
        "Different denominations represented different values, depending on the material and minting.",
        "The more you earn, the more successful your trade becomes.",
        "Managing your coins wisely will help you grow as a merchant.",
        "Now get ready... more customers are coming.",
        "Stay sharp, negotiate well, and maximize your profit.",
        "Your next phase begins now."
    };

    void Start()
    {
        StartCoroutine(PlayNarration());
    }

    IEnumerator PlayNarration()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            captionText.text = lines[i];

            yield return new WaitForSeconds(3.5f); // adjust timing
        }

        EndNarration();
    }

    void ShowLine()
    {
        captionText.text = lines[index];
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            ShowLine();
        }
        else
        {
            EndNarration();
        }
    }

    void EndNarration()
    {
        Debug.Log("Narration finished → Starting 5-minute game loop");

        
        // FindObjectOfType<GameManager>().StartGameLoop();

        // OR trigger NPC spawning / timer etc.
    }
}