using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionUIManager : MonoBehaviour
{
    public static NPCQuestionUIManager Instance;

    [Header("UI")]
    public GameObject canvas;

    [Header("Question Buttons")]
    public Button[] buttons;
    public TMP_Text[] buttonTexts;

    private NPCDialogueData currentDialogue;
    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (canvas != null)
            canvas.SetActive(false);
    }

    public void Open(NPCDialogueData data)
    {
        currentDialogue = data;
        isOpen = true;

        canvas.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            if (i < data.questions.Length)
            {
                buttons[i].gameObject.SetActive(true);
                buttonTexts[i].text = data.questions[i].question;

                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    AskQuestion(index);
                });
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    private void AskQuestion(int index)
    {
        if (currentDialogue == null)
            return;

        NarratorUIManager.Instance.ShowNarration(
            currentDialogue.npcName,
            currentDialogue.questions[index].response,
            6f
        );
    }

    public void Close()
    {
        isOpen = false;
        currentDialogue = null;

        if (canvas != null)
            canvas.SetActive(false);
    }
}