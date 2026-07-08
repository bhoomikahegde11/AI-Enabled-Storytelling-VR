using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionUIManager : MonoBehaviour
{
    public static NPCQuestionUIManager Instance;

    [Header("Question Canvas")]
    public GameObject questionCanvas;

    [Header("Question Buttons")]
    public Button[] questionButtons;
    public TMP_Text[] questionButtonTexts;

    private NPCDialogueData currentDialogue;
    private bool[] askedQuestions;
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (questionCanvas != null)
            questionCanvas.SetActive(false);
    }
    
    public void Open(NPCDialogueData dialogue)
    {
        currentDialogue = dialogue;

        if (askedQuestions == null || askedQuestions.Length != dialogue.questions.Length)
            askedQuestions = new bool[dialogue.questions.Length];

        questionCanvas.SetActive(true);

        for (int i = 0; i < questionButtons.Length; i++)
        {
            int index = i;

            if (i < dialogue.questions.Length && !askedQuestions[i])
            {
                questionButtons[i].gameObject.SetActive(true);
                questionButtonTexts[i].text = dialogue.questions[i].question;

                questionButtons[i].onClick.RemoveAllListeners();
                questionButtons[i].onClick.AddListener(() => AskQuestion(index));
            }
            else
            {
                questionButtons[i].gameObject.SetActive(false);
            }
        }
        

        Debug.Log("[QUESTION UI] Opened");
    }

    private void AskQuestion(int index)
    {
        if (currentDialogue == null)
            return;

        askedQuestions[index] = true;

        if (questionCanvas != null)
            questionCanvas.SetActive(false);

        StartCoroutine(AnswerThenReopen(index));
    }

    private IEnumerator AnswerThenReopen(int index)
    {
        NarratorUIManager.Instance.ShowNarration(
            currentDialogue.npcName,
            currentDialogue.questions[index].response,
            6f
        );

        yield return new WaitForSeconds(6.2f);

        if (HasRemainingQuestions())
        {
            Open(currentDialogue);
        }
        else
        {
            Debug.Log("[QUESTION UI] All questions asked");
        }
    }

    private bool HasRemainingQuestions()
    {
        for (int i = 0; i < askedQuestions.Length; i++)
        {
            if (!askedQuestions[i])
                return true;
        }

        return false;
    }

    public void Close()
    {
        currentDialogue = null;
        askedQuestions = null;

        if (questionCanvas != null)
            questionCanvas.SetActive(false);

        Debug.Log("[QUESTION UI] Closed");
    }
}