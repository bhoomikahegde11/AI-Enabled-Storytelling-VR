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

    public bool IsOpen { get; private set; }
    private NPCInteraction currentNPC;
    private NPCDialogueData currentDialogue;
    private bool[] askedQuestions;
    private Coroutine answerRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetCanvasVisible(false);
    }

    public void Open(
    NPCDialogueData dialogue,
    NPCInteraction npc = null)
    {
        currentDialogue = dialogue;

        if (npc != null)
            currentNPC = npc;

        if (askedQuestions == null ||
            askedQuestions.Length != dialogue.questions.Length)
        {
            askedQuestions = new bool[dialogue.questions.Length];
        }

        for (int i = 0; i < questionButtons.Length; i++)
        {
            int index = i;

            bool shouldShow =
                i < dialogue.questions.Length &&
                !askedQuestions[i];

            questionButtons[i].gameObject.SetActive(shouldShow);

            if (!shouldShow)
                continue;

            questionButtonTexts[i].text =
                dialogue.questions[i].question;

            questionButtons[i].onClick.RemoveAllListeners();
            questionButtons[i].onClick.AddListener(
                () => AskQuestion(index)
            );
        }

        SetCanvasVisible(true);

        Debug.Log("[QUESTION UI] Opened.");
    }

    private void AskQuestion(int index)
    {
        if (currentDialogue == null ||
            index < 0 ||
            index >= currentDialogue.questions.Length)
        {
            return;
        }

        askedQuestions[index] = true;

        SetCanvasVisible(false);

        if (answerRoutine != null)
            StopCoroutine(answerRoutine);

        answerRoutine =
            StartCoroutine(AnswerThenReopen(index));
    }

    private IEnumerator AnswerThenReopen(int index)
    {
        NPCDialogueData dialogueAtStart =
            currentDialogue;

        yield return NarratorUIManager.Instance
            .PlayNarrationLineByLine(
                dialogueAtStart.npcName,
                dialogueAtStart.questions[index].response
            );

        answerRoutine = null;

        if (currentDialogue == null)
            yield break;

        if (HasRemainingQuestions())
        {
            Open(currentDialogue);
        }
        else
        {
            Debug.Log("[QUESTION UI] All questions asked.");

            SetCanvasVisible(false);

            if (currentNPC != null)
                currentNPC.OnAllQuestionsAsked();
        }
    }

    private bool HasRemainingQuestions()
    {
        if (askedQuestions == null)
            return false;

        foreach (bool asked in askedQuestions)
        {
            if (!asked)
                return true;
        }

        return false;
    }

    public void Close()
    {
        if (answerRoutine != null)
        {
            StopCoroutine(answerRoutine);
            answerRoutine = null;
        }

        SetCanvasVisible(false);

        currentDialogue = null;
        askedQuestions = null;
        currentNPC = null;
        Debug.Log("[QUESTION UI] Closed.");
    }

    private void SetCanvasVisible(bool visible)
    {
        IsOpen = visible;

        if (questionCanvas != null)
            questionCanvas.SetActive(visible);

        SetLaserPointerEnabled(visible);
    }

    private void SetLaserPointerEnabled(bool enabled)
    {
        NPCDialogueVRLaserPointer laser =
            Object.FindAnyObjectByType<NPCDialogueVRLaserPointer>(
                FindObjectsInactive.Include
            );

        if (laser != null)
            laser.enabled = enabled;
    }
}