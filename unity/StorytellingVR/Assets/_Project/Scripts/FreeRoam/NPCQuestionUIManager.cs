using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionUIManager : MonoBehaviour
{
    public static NPCQuestionUIManager Instance;

    public bool IsOpen { get; private set; }

    public bool IsAnswerPlaying
    {
        get { return answerRoutine != null; }
    }

    private NPCInteraction currentNPC;
    private NPCDialogueData currentDialogue;
    private NPCQuestionCanvasView currentCanvasView;

    private bool[] askedQuestions;
    private Coroutine answerRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "[QUESTION UI] Duplicate NPCQuestionUIManager found. " +
                "Destroying the duplicate component."
            );

            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void Open(
        NPCDialogueData dialogue,
        NPCInteraction npc,
        NPCQuestionCanvasView canvasView)
    {
        if (dialogue == null)
        {
            Debug.LogError(
                "[QUESTION UI] Cannot open because dialogue is null."
            );

            return;
        }

        if (canvasView == null)
        {
            Debug.LogError(
                "[QUESTION UI] Cannot open because the NPC has no " +
                "NPCQuestionCanvasView assigned."
            );

            return;
        }

        if (!canvasView.IsConfigured())
        {
            Debug.LogError(
                "[QUESTION UI] The assigned canvas view is not configured. " +
                "Check its buttons and text references."
            );

            return;
        }

        // Hide the previous NPC canvas when switching NPCs.
        if (currentCanvasView != null &&
            currentCanvasView != canvasView)
        {
            currentCanvasView.SetVisible(false);
        }

        currentDialogue = dialogue;
        currentNPC = npc;
        currentCanvasView = canvasView;

        if (askedQuestions == null ||
            askedQuestions.Length != dialogue.questions.Length)
        {
            askedQuestions =
                new bool[dialogue.questions.Length];
        }

        RefreshButtons();
        SetCanvasVisible(true);

        Debug.Log(
            $"[QUESTION UI] Opened canvas for {dialogue.npcName}."
        );
    }

    private void RefreshButtons()
    {
        if (currentCanvasView == null ||
            currentDialogue == null)
        {
            return;
        }

        Button[] buttons =
            currentCanvasView.QuestionButtons;

        TMP_Text[] buttonTexts =
            currentCanvasView.QuestionButtonTexts;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            bool questionExists =
                i < currentDialogue.questions.Length;

            bool alreadyAsked =
                questionExists &&
                askedQuestions != null &&
                i < askedQuestions.Length &&
                askedQuestions[i];

            bool shouldShow =
                questionExists && !alreadyAsked;

            if (buttons[i] == null)
                continue;

            buttons[i].gameObject.SetActive(shouldShow);

            buttons[i].onClick.RemoveAllListeners();

            if (!shouldShow)
                continue;

            if (i < buttonTexts.Length &&
                buttonTexts[i] != null)
            {
                buttonTexts[i].text =
                    currentDialogue.questions[i].question;
            }

            buttons[i].onClick.AddListener(
                () => AskQuestion(index)
            );
        }
    }

    private void AskQuestion(int index)
    {
        if (IsAnswerPlaying)
            return;

        if (currentDialogue == null ||
            index < 0 ||
            index >= currentDialogue.questions.Length)
        {
            return;
        }

        if (askedQuestions == null ||
            index >= askedQuestions.Length)
        {
            return;
        }

        askedQuestions[index] = true;

        // Hide only the current NPC's canvas.
        SetCanvasVisible(false);

        answerRoutine =
            StartCoroutine(AnswerThenReopen(index));
    }

    private IEnumerator AnswerThenReopen(int index)
    {
        NPCDialogueData dialogueAtStart =
            currentDialogue;

        NPCInteraction npcAtStart =
            currentNPC;

        NPCQuestionCanvasView canvasAtStart =
            currentCanvasView;

        yield return NarratorUIManager.Instance
            .PlayNarrationLineByLine(
                dialogueAtStart.npcName,
                dialogueAtStart.questions[index].response
            );

        answerRoutine = null;

        // Conversation may have been closed externally.
        if (currentDialogue == null ||
            currentNPC == null ||
            currentCanvasView == null)
        {
            yield break;
        }

        // Ensure this is still the same NPC conversation.
        if (currentDialogue != dialogueAtStart ||
            currentNPC != npcAtStart ||
            currentCanvasView != canvasAtStart)
        {
            yield break;
        }

        if (HasRemainingQuestions())
        {
            RefreshButtons();
            SetCanvasVisible(true);

            Debug.Log(
                "[QUESTION UI] Answer completed; questions reopened."
            );
        }
        else
        {
            Debug.Log(
                "[QUESTION UI] All questions asked."
            );

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
        currentNPC = null;
        currentCanvasView = null;
        askedQuestions = null;

        Debug.Log("[QUESTION UI] Closed.");
    }

    private void SetCanvasVisible(bool visible)
    {
        IsOpen = visible;

        if (currentCanvasView != null)
            currentCanvasView.SetVisible(visible);

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