using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives a simple branching conversation: Intro -> Question List -> Answer -> back
/// to Question List (minus the asked question) -> ... -> Leave.
/// Put this on the ConversationCanvas GameObject (the World Space Canvas).
/// </summary>
public class TraderConversationManager : MonoBehaviour
{
    [Serializable]
    public class QuestionData
    {
        [TextArea(1, 2)] public string question;
        [TextArea(2, 5)] public string answer;
        [HideInInspector] public bool asked;
    }

    [Header("Canvas Root")]
    [Tooltip("The root GameObject that gets enabled/disabled to show/hide the whole conversation UI. Usually this same GameObject.")]
    [SerializeField] private GameObject canvasRoot;

    [Header("Intro Panel")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private TMP_Text introText;
    [SerializeField] private Button continueButton;
    [TextArea(2, 5)][SerializeField] private string introMessage = "Namaskara, traveler. I have come far to trade in this market. Ask me what you wish to know.";

    [Header("Questions Panel")]
    [SerializeField] private GameObject questionsPanel;
    [Tooltip("Exactly 4 buttons, each with a TMP_Text child for the question label.")]
    [SerializeField] private Button[] questionButtons = new Button[4];
    [SerializeField] private GameObject noMoreQuestionsLabel; // optional: small text shown when list is empty

    [Header("Answer Panel")]
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TMP_Text answerText;
    [SerializeField] private Button backButton;

    [Header("Leave")]
    [Tooltip("Always-visible button the player can press at any point to end the conversation.")]
    [SerializeField] private Button leaveButton;

    [Header("Conversation Content (placeholders — replace with real content)")]
    [SerializeField]
    private List<QuestionData> questions = new List<QuestionData>
    {
        new QuestionData
        {
            question = "Where do you come from, traveler?",
            answer = "I sailed from Hormuz, many months across open water, to reach the port of Goa, then travelled inland to Hampi to trade my wares."
        },
        new QuestionData
        {
            question = "What goods do you bring to trade?",
            answer = "Fine Arabian horses, rosewater, and pearls beyond compare. In return I seek your pepper, cardamom, and cotton cloth."
        },
        new QuestionData
        {
            question = "How is your gold exchanged for our currency here?",
            answer = "Your moneychangers weigh my gold against the varaha, the pagoda coin struck here in Vijayanagara. A fair weight buys a fair price."
        },
        new QuestionData
        {
            question = "Is the journey to this market safe?",
            answer = "The roads are watched by the Empire's soldiers, and the markets of Hampi are well guarded. Still, a wise trader keeps company on the road."
        },
    };

    // runtime list of indices into `questions` that have not been asked yet
    private List<int> unaskedIndices = new List<int>();
    private bool conversationActive;

    [Tooltip("If true, questions the player already asked will be available again next time they start a new conversation. If false, once asked they stay asked for the whole play session.")]
    [SerializeField] private bool resetQuestionsEachConversation = true;

    private void Awake()
    {
        if (canvasRoot == null) canvasRoot = gameObject;

        continueButton.onClick.AddListener(ShowQuestionList);
        backButton.onClick.AddListener(ShowQuestionList);
        leaveButton.onClick.AddListener(EndConversation);

        for (int i = 0; i < questionButtons.Length; i++)
        {
            int capturedIndex = i; // avoid closure bug
            questionButtons[i].onClick.AddListener(() => OnQuestionButtonClicked(capturedIndex));
        }

        canvasRoot.SetActive(false);
    }

    /// <summary>
    /// Called by ForeignTraderInteractable when the player points+triggers the trader.
    /// </summary>
    public void StartConversation()
    {
        if (conversationActive) return; // already mid-conversation, ignore re-trigger

        conversationActive = true;

        if (resetQuestionsEachConversation)
        {
            foreach (var q in questions) q.asked = false;
        }

        RebuildUnaskedIndices();

        canvasRoot.SetActive(true);
        ShowIntro();
    }

    private void ShowIntro()
    {
        introText.text = introMessage;
        SetPanel(introPanel);
    }

    private void ShowQuestionList()
    {
        RebuildUnaskedIndices();

        SetPanel(questionsPanel);

        bool anyLeft = unaskedIndices.Count > 0;
        if (noMoreQuestionsLabel != null) noMoreQuestionsLabel.SetActive(!anyLeft);

        for (int i = 0; i < questionButtons.Length; i++)
        {
            if (i < unaskedIndices.Count)
            {
                int questionIndex = unaskedIndices[i];
                questionButtons[i].gameObject.SetActive(true);
                var label = questionButtons[i].GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = questions[questionIndex].question;
            }
            else
            {
                questionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnQuestionButtonClicked(int slotIndex)
    {
        if (slotIndex >= unaskedIndices.Count) return; // safety check, shouldn't happen

        int questionIndex = unaskedIndices[slotIndex];
        questions[questionIndex].asked = true;

        answerText.text = questions[questionIndex].answer;
        SetPanel(answerPanel);
    }

    private void RebuildUnaskedIndices()
    {
        unaskedIndices.Clear();
        for (int i = 0; i < questions.Count; i++)
        {
            if (!questions[i].asked) unaskedIndices.Add(i);
        }
    }

    /// <summary>
    /// Ends the conversation. Wired to the always-visible Leave button,
    /// so the player can back out at any point (intro, question list, or answer).
    /// </summary>
    public void EndConversation()
    {
        conversationActive = false;
        canvasRoot.SetActive(false);
    }

    private void SetPanel(GameObject panelToShow)
    {
        introPanel.SetActive(panelToShow == introPanel);
        questionsPanel.SetActive(panelToShow == questionsPanel);
        answerPanel.SetActive(panelToShow == answerPanel);
    }
}