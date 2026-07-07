using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TravellerDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text titleText;
    public TMP_Text answerText;

    [Header("Buttons")]
    public Button question1;
    public Button question2;
    public Button question3;
    public Button question4;

    private bool[] asked = new bool[4];

    private string[] questions =
    {
        "Where are you from?",
        "Why did you visit Vijayanagara?",
        "What impressed you the most?",
        "Why are foreign visitors important?"
    };

    private string[] answers =
    {
        "I come from a distant kingdom across the sea. Travellers and merchants journey here because Vijayanagara is known throughout the world for its wealth and magnificent city.",

        "I travelled here after hearing stories about its bustling markets, beautiful temples, and powerful rulers. I wanted to see whether these stories were true.",

        "The markets impressed me the most. People from many different lands traded spices, silk, precious stones, horses, and many other valuable goods. The city was always full of life.",

        "Travellers share what they see with people in their own countries. Their stories help others learn about the culture, trade, architecture, and daily life of Vijayanagara."
    };

    private void Start()
    {
        dialoguePanel.SetActive(false);

        question1.onClick.AddListener(() => AskQuestion(0));
        question2.onClick.AddListener(() => AskQuestion(1));
        question3.onClick.AddListener(() => AskQuestion(2));
        question4.onClick.AddListener(() => AskQuestion(3));
    }

    public void OpenDialogue()
    {
        dialoguePanel.SetActive(true);

        titleText.text =
            "Greetings, traveller!\n\n" +
            "I have travelled from a distant land to witness the famous city of Vijayanagara.\n\n" +
            "Ask me anything.";

        answerText.text = "";
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    void AskQuestion(int index)
    {
        answerText.text =
            "<b>" + questions[index] + "</b>\n\n" +
            answers[index];

        asked[index] = true;

        switch (index)
        {
            case 0:
                question1.gameObject.SetActive(false);
                break;

            case 1:
                question2.gameObject.SetActive(false);
                break;

            case 2:
                question3.gameObject.SetActive(false);
                break;

            case 3:
                question4.gameObject.SetActive(false);
                break;
        }

        CheckFinished();
    }

    void CheckFinished()
    {
        foreach (bool b in asked)
        {
            if (!b)
                return;
        }

        titleText.text =
            "You have asked all the questions.\n\nThank you for learning about the foreign visitors of Vijayanagara!";
    }

    public void ResetDialogue()
    {
        dialoguePanel.SetActive(false);

        answerText.text = "";

        for (int i = 0; i < asked.Length; i++)
            asked[i] = false;

        question1.gameObject.SetActive(true);
        question2.gameObject.SetActive(true);
        question3.gameObject.SetActive(true);
        question4.gameObject.SetActive(true);
    }
}