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

    [Header("Controller Rays")]
    public GameObject leftRay;
    public GameObject rightRay;

    private NPCDialogueData currentDialogue;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (questionCanvas != null)
            questionCanvas.SetActive(false);

        SetRays(false);
    }

    public void Open(NPCDialogueData dialogue)
    {
        currentDialogue = dialogue;

        questionCanvas.SetActive(true);
        SetRays(true);

        for (int i = 0; i < questionButtons.Length; i++)
        {
            int index = i;

            if (i < dialogue.questions.Length)
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

    public void Close()
    {
        currentDialogue = null;

        if (questionCanvas != null)
            questionCanvas.SetActive(false);

        SetRays(false);

        Debug.Log("[QUESTION UI] Closed");
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

    private void SetRays(bool active)
    {
        if (leftRay != null)
            leftRay.SetActive(active);

        if (rightRay != null)
            rightRay.SetActive(active);
    }
}