using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionUIManager : MonoBehaviour
{
    public static NPCQuestionUIManager Instance { get; private set; }

    public bool IsOpen { get; private set; }

    public bool IsAnswerPlaying =>
        answerRoutine != null;

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
                "Destroying duplicate component."
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

        if (npc == null)
        {
            Debug.LogError(
                "[QUESTION UI] Cannot open because NPCInteraction is null."
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

        // Hide a previous NPC canvas when changing conversations.
        if (currentCanvasView != null &&
            currentCanvasView != canvasView)
        {
            currentCanvasView.SetVisible(false);
        }

        currentDialogue = dialogue;
        currentNPC = npc;
        currentCanvasView = canvasView;

        // A new conversation should always start with fresh questions.
        askedQuestions =
            new bool[dialogue.questions != null
                ? dialogue.questions.Length
                : 0];

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

        DialogueQuestion[] questions =
            currentDialogue.questions;

        int questionCount =
            questions != null ? questions.Length : 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            if (button == null)
                continue;

            int index = i;

            bool questionExists =
                index < questionCount;

            bool alreadyAsked =
                questionExists &&
                askedQuestions != null &&
                index < askedQuestions.Length &&
                askedQuestions[index];

            bool shouldShow =
                questionExists && !alreadyAsked;

            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(shouldShow);

            if (!shouldShow)
                continue;

            if (index < buttonTexts.Length &&
                buttonTexts[index] != null)
            {
                buttonTexts[index].text =
                    questions[index].question;
            }

            button.onClick.AddListener(
                () => AskQuestion(index)
            );
        }
    }

    private void AskQuestion(int index)
    {
        if (IsAnswerPlaying)
            return;

        if (currentDialogue == null ||
            currentDialogue.questions == null ||
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

        SetCanvasVisible(false);

        answerRoutine =
            StartCoroutine(AnswerThenContinue(index));
    }

    private IEnumerator AnswerThenContinue(int index)
    {
        NPCDialogueData dialogueAtStart =
            currentDialogue;

        NPCInteraction npcAtStart =
            currentNPC;

        NPCQuestionCanvasView canvasAtStart =
            currentCanvasView;

        DialogueQuestion question =
            dialogueAtStart.questions[index];

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance
                .PlayNarrationLineByLine(
                    dialogueAtStart.npcName,
                    question.response
                );
        }
        else
        {
            Debug.LogError(
                "[QUESTION UI] NarratorUIManager.Instance is missing."
            );
        }

        // This answer coroutine has now genuinely finished.
        answerRoutine = null;

        // The conversation might have been closed externally.
        if (currentDialogue == null ||
            currentNPC == null ||
            currentCanvasView == null)
        {
            yield break;
        }

        // Ensure another NPC conversation did not replace this one.
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
                "[QUESTION UI] Answer complete; remaining questions reopened."
            );

            yield break;
        }

        Debug.Log(
            "[QUESTION UI] All questions asked. " +
            "Preparing closing dialogue."
        );

        /*
         * Important:
         * Save the NPC before clearing this manager's state.
         * Then clear the UI locally without stopping this coroutine.
         */
        NPCInteraction npcToComplete =
            currentNPC;

        ClearConversationState();

        /*
         * Wait one frame before asking NPCInteraction to start the
         * closing narration. This prevents the final answer and the
         * closing sequence from fighting over the narrator/UI state.
         */
        yield return null;

        if (npcToComplete != null)
        {
            Debug.Log(
                "[QUESTION UI] Calling OnAllQuestionsAsked."
            );

            npcToComplete.OnAllQuestionsAsked();
        }
        else
        {
            Debug.LogError(
                "[QUESTION UI] Cannot start closing dialogue because " +
                "the NPC reference was lost."
            );
        }
    }

    private bool HasRemainingQuestions()
    {
        if (askedQuestions == null ||
            askedQuestions.Length == 0)
        {
            return false;
        }

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

        ClearConversationState();

        Debug.Log("[QUESTION UI] Closed.");
    }

    private void ClearConversationState()
    {
        SetCanvasVisible(false);

        currentDialogue = null;
        currentNPC = null;
        currentCanvasView = null;
        askedQuestions = null;
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