using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SilkTraderDialogueManager : MonoBehaviour
{
    public TMP_Text responseText;
    public GameObject dialoguePanel;
    public GameObject interactionHint;
    public Button questionButtonTemplate;

    [TextArea(2, 4)]
    public string welcomeText = "Welcome traveller.\nWhat would you like to know?";

    private readonly string[] questions =
    {
        "What type of silk do you sell?",
        "How many varahas does this silk cost?",
        "Tell me about import and export of silk."
    };

    private readonly string[] answers =
    {
        "I sell fine plain silk, richly dyed silk, embroidered silk, and brocaded silk with shining patterns. The finest pieces are chosen by nobles, temple patrons, and wealthy traders.",
        "The price depends on the weave, dye, and decoration. A simpler silk cloth may cost a few varahas, while richly brocaded silk can cost many more because it takes skilled work and costly materials.",
        "Silk reaches Vijayanagara through inland and coastal trade routes, while finished cloth leaves these markets with merchants travelling to ports and distant kingdoms. Hampi's bazaars connect local weavers, foreign traders, and royal buyers."
    };

    private bool buttonsBuilt;
    private Transform hitboxRoot;
    private int hoveredQuestionIndex = -1;
    private int selectedAnswerIndex = -1;
    private bool showingAnswer;

    private void Awake()
    {
        ResolveReferences();
        PrepareDialogue();
    }

    private void OnEnable()
    {
        ResetResponse();
    }

    public void PrepareDialogue()
    {
        ResolveReferences();
        BuildQuestionButtons();
        BuildQuestionHitboxes();
        ShowQuestionButtons(true);
        ResetResponse();
    }

    public void WhatAreYouSelling()
    {
        ShowAnswer(0);
    }

    public void SilkTypes()
    {
        ShowAnswer(0);
    }

    public void SilkCost()
    {
        ShowAnswer(1);
    }

    public void WhoBuysSilk()
    {
        ShowAnswer(2);
    }

    public void SilkOrigin()
    {
        ShowAnswer(2);
    }

    public void TradeInfo()
    {
        ShowAnswer(2);
    }

    public void ResetResponse()
    {
        if (responseText != null)
        {
            showingAnswer = false;
            selectedAnswerIndex = -1;
            hoveredQuestionIndex = -1;
            responseText.enableAutoSizing = true;
            responseText.richText = true;
            responseText.fontSizeMin = 14f;
            responseText.text = BuildQuestionPrompt();
            RebuildQuestionHitboxes();
        }
    }

    public void CloseDialogue()
    {
        showingAnswer = false;
        selectedAnswerIndex = -1;
        hoveredQuestionIndex = -1;

        if (dialoguePanel == null && responseText != null)
        {
            dialoguePanel = responseText.transform.root.gameObject;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (interactionHint != null)
        {
            interactionHint.SetActive(true);
        }
    }

    public void ShowAnswer(int index)
    {
        if (responseText == null || index < 0 || index >= answers.Length)
        {
            return;
        }

        responseText.enableAutoSizing = true;
        responseText.richText = true;
        responseText.fontSizeMin = 14f;
        showingAnswer = true;
        selectedAnswerIndex = index;
        hoveredQuestionIndex = -1;
        responseText.text = BuildAnswerText(index);
        RebuildQuestionHitboxes();
    }

    public void SelectQuestion(int index)
    {
        ShowAnswer(index);
    }

    public void SetHoveredQuestion(int index)
    {
        if (showingAnswer || hoveredQuestionIndex == index)
        {
            return;
        }

        hoveredQuestionIndex = index;

        if (responseText != null)
        {
            responseText.text = BuildQuestionPrompt();
        }
    }

    private void BuildQuestionButtons()
    {
        ResolveReferences();

        Transform parent = GetQuestionRoot();

        if (buttonsBuilt)
        {
            LayoutQuestionButtons(parent);
            return;
        }

        buttonsBuilt = true;

        for (int i = 0; i < questions.Length; i++)
        {
            Button button = CreateButton($"Question{i + 1}", parent);
            button.name = $"Question{i + 1}";

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = questions[i];
                label.color = Color.white;
                label.enableAutoSizing = true;
                label.fontSizeMin = 24f;
                label.fontSizeMax = 36f;
            }

            int answerIndex = i;
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => ShowAnswer(answerIndex));
        }

        Button closeButton = CreateButton("CloseButton", parent);
        closeButton.name = "CloseButton";

        TMP_Text closeLabel = closeButton.GetComponentInChildren<TMP_Text>();
        if (closeLabel != null)
        {
            closeLabel.text = "Close";
            closeLabel.color = Color.white;
        }

        closeButton.onClick = new Button.ButtonClickedEvent();
        closeButton.onClick.AddListener(CloseDialogue);

        LayoutQuestionButtons(parent);
    }

    private string BuildQuestionPrompt()
    {
        StringBuilder builder = new StringBuilder(welcomeText);
        builder.AppendLine();

        for (int i = 0; i < questions.Length; i++)
        {
            builder.AppendLine();
            if (i == hoveredQuestionIndex)
            {
                builder.Append("<mark=#66440088><color=#FFD45A><size=155%>");
            }
            else
            {
                builder.Append("<color=#FFFFFF><size=135%>");
            }

            builder.Append(i + 1);
            builder.Append(". ");
            builder.Append(questions[i]);
            builder.AppendLine("</size></color>");

            if (i == hoveredQuestionIndex)
            {
                builder.Append("</mark>");
            }
        }

        builder.AppendLine();
        builder.AppendLine();
        builder.Append("<color=#FFFFFF><size=125%>Close</size></color>");
        return builder.ToString();
    }

    private string BuildAnswerText(int index)
    {
        return "<color=#FFFFFF><size=125%>"
            + answers[index]
            + "</size></color>\n\n"
            + "<color=#FFFFFF><size=125%>Close</size></color>";
    }

    private void BuildQuestionHitboxes()
    {
        ResolveReferences();
        if (responseText == null)
        {
            return;
        }

        RebuildQuestionHitboxes();
    }

    private void RebuildQuestionHitboxes()
    {
        if (responseText == null)
        {
            return;
        }

        RectTransform responseRect = responseText.rectTransform;
        responseText.ForceMeshUpdate();

        if (hitboxRoot == null)
        {
            Transform existingRoot = responseRect.Find("SilkTraderQuestionHitboxes");
            hitboxRoot = existingRoot != null ? existingRoot : new GameObject("SilkTraderQuestionHitboxes").transform;
            hitboxRoot.SetParent(responseRect, false);
            hitboxRoot.gameObject.layer = responseText.gameObject.layer;
        }

        for (int i = hitboxRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(hitboxRoot.GetChild(i).gameObject);
        }

        TMP_TextInfo textInfo = responseText.textInfo;
        int activeQuestionIndex = -1;
        bool activeAnswer = false;
        float minX = 0f;
        float maxX = 0f;
        float minY = 0f;
        float maxY = 0f;

        for (int lineIndex = 0; lineIndex < textInfo.lineCount; lineIndex++)
        {
            TMP_LineInfo line = textInfo.lineInfo[lineIndex];
            string trimmedLine = GetRenderedLineText(line).TrimStart();
            int detectedQuestionIndex = GetQuestionIndexFromLine(trimmedLine);
            bool detectedClose = trimmedLine.Equals("Close");

            if (detectedQuestionIndex >= 0)
            {
                if (activeQuestionIndex >= 0)
                {
                    CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Question, activeQuestionIndex, minX, maxX, minY, maxY);
                }

                if (activeAnswer)
                {
                    CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Answer, selectedAnswerIndex, minX, maxX, minY, maxY);
                    activeAnswer = false;
                }

                activeQuestionIndex = detectedQuestionIndex;
                minX = line.lineExtents.min.x;
                maxX = line.lineExtents.max.x;
                minY = line.descender;
                maxY = line.ascender;
                continue;
            }

            if (detectedClose)
            {
                if (activeQuestionIndex >= 0)
                {
                    CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Question, activeQuestionIndex, minX, maxX, minY, maxY);
                    activeQuestionIndex = -1;
                }

                if (activeAnswer)
                {
                    CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Answer, selectedAnswerIndex, minX, maxX, minY, maxY);
                    activeAnswer = false;
                }

                CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Close, -1, line.lineExtents.min.x, line.lineExtents.max.x, line.descender, line.ascender);
                continue;
            }

            bool answerLine = showingAnswer && !string.IsNullOrWhiteSpace(trimmedLine);
            if (answerLine)
            {
                if (!activeAnswer)
                {
                    activeAnswer = true;
                    minX = line.lineExtents.min.x;
                    maxX = line.lineExtents.max.x;
                    minY = line.descender;
                    maxY = line.ascender;
                }
                else
                {
                    minX = Mathf.Min(minX, line.lineExtents.min.x);
                    maxX = Mathf.Max(maxX, line.lineExtents.max.x);
                    minY = Mathf.Min(minY, line.descender);
                    maxY = Mathf.Max(maxY, line.ascender);
                }

                continue;
            }

            if (activeQuestionIndex >= 0 && !string.IsNullOrWhiteSpace(trimmedLine) && !trimmedLine.StartsWith("NPC:"))
            {
                minX = Mathf.Min(minX, line.lineExtents.min.x);
                maxX = Mathf.Max(maxX, line.lineExtents.max.x);
                minY = Mathf.Min(minY, line.descender);
                maxY = Mathf.Max(maxY, line.ascender);
            }
            else if (activeQuestionIndex >= 0)
            {
                CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Question, activeQuestionIndex, minX, maxX, minY, maxY);
                activeQuestionIndex = -1;
            }
        }

        if (activeQuestionIndex >= 0)
        {
            CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Question, activeQuestionIndex, minX, maxX, minY, maxY);
        }

        if (activeAnswer)
        {
            CreateHitbox(SilkTraderQuestionHitbox.HitboxAction.Answer, selectedAnswerIndex, minX, maxX, minY, maxY);
        }
    }

    private int GetQuestionIndexFromLine(string line)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            if (line.StartsWith($"{i + 1}."))
            {
                return i;
            }
        }

        return -1;
    }

    private void CreateHitbox(SilkTraderQuestionHitbox.HitboxAction action, int questionIndex, float minX, float maxX, float minY, float maxY)
    {
        float textWidth = Mathf.Max(220f, maxX - minX);
        float textHeight = Mathf.Max(34f, maxY - minY);
        float centerX = (minX + maxX) * 0.5f;
        float centerY = (minY + maxY) * 0.5f;

        string hitboxName = action == SilkTraderQuestionHitbox.HitboxAction.Question
            ? $"Question{questionIndex + 1}Hitbox"
            : $"{action}Hitbox";

        GameObject hitboxObject = new GameObject(hitboxName);
        hitboxObject.transform.SetParent(hitboxRoot, false);
        hitboxObject.layer = responseText.gameObject.layer;
        hitboxObject.transform.localPosition = new Vector3(centerX, centerY, -0.02f);

        BoxCollider collider = hitboxObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        float widthPadding = action == SilkTraderQuestionHitbox.HitboxAction.Question ? 90f : 36f;
        float heightPadding = action == SilkTraderQuestionHitbox.HitboxAction.Question ? 26f : 14f;
        collider.size = new Vector3(textWidth + widthPadding, textHeight + heightPadding, 0.12f);

        SilkTraderQuestionHitbox hitbox = hitboxObject.AddComponent<SilkTraderQuestionHitbox>();
        hitbox.dialogueManager = this;
        hitbox.action = action;
        hitbox.questionIndex = questionIndex;
    }

    private string GetRenderedLineText(TMP_LineInfo line)
    {
        if (responseText == null || line.characterCount <= 0)
        {
            return string.Empty;
        }

        TMP_TextInfo textInfo = responseText.textInfo;
        StringBuilder builder = new StringBuilder(line.characterCount);

        int lastCharacterIndex = line.firstCharacterIndex + line.characterCount;
        for (int i = line.firstCharacterIndex; i < lastCharacterIndex && i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (characterInfo.isVisible || !char.IsControl(characterInfo.character))
            {
                builder.Append(characterInfo.character);
            }
        }

        return builder.ToString();
    }

    private void ResolveReferences()
    {
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("DialoguePanel");
        }

        Transform searchRoot = dialoguePanel != null ? dialoguePanel.transform : transform;

        if (responseText == null)
        {
            responseText = FindChildComponent<TMP_Text>(searchRoot, "ResponseText");
        }

        if (interactionHint == null)
        {
            interactionHint = GameObject.Find("InteractionHint");
        }

        if (questionButtonTemplate == null)
        {
            questionButtonTemplate = FindChildComponent<Button>(searchRoot, "Question1");
        }
    }

    private void LayoutQuestionButtons(Transform parent)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            Button button = FindChildComponent<Button>(parent, $"Question{i + 1}");
            LayoutButton(button, new Vector2(0f, 70f - (i * 52f)), new Vector2(620f, 48f));
        }

        Button closeButton = FindChildComponent<Button>(parent, "CloseButton");
        LayoutButton(closeButton, new Vector2(0f, -255f), new Vector2(220f, 48f));
    }

    private static void LayoutButton(Button button, Vector2 anchoredPosition, Vector2 size)
    {
        if (button == null)
        {
            return;
        }

        button.gameObject.SetActive(true);

        RectTransform rectTransform = (RectTransform)button.transform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;
        rectTransform.localScale = Vector3.one;

        if (button.TryGetComponent(out Image image))
        {
            image.color = new Color(1f, 1f, 1f, 0.01f);
            image.raycastTarget = true;
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.color = Color.white;
            label.enableAutoSizing = true;
            label.fontSizeMin = 24f;
            label.fontSizeMax = 36f;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;

            RectTransform labelRectTransform = label.rectTransform;
            labelRectTransform.anchorMin = Vector2.zero;
            labelRectTransform.anchorMax = Vector2.one;
            labelRectTransform.offsetMin = new Vector2(8f, 2f);
            labelRectTransform.offsetMax = new Vector2(-8f, -2f);
        }
    }

    private void ShowQuestionButtons(bool visible)
    {
        if (dialoguePanel == null)
        {
            return;
        }

        Transform questionRoot = dialoguePanel.transform.Find("SilkTraderQuestionGroup");
        if (questionRoot == null)
        {
            return;
        }

        for (int i = 0; i < questions.Length; i++)
        {
            Button button = FindChildComponent<Button>(questionRoot, $"Question{i + 1}");
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }

        Button closeButton = FindChildComponent<Button>(questionRoot, "CloseButton");
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(visible);
        }
    }

    private Transform GetQuestionRoot()
    {
        Transform parent = dialoguePanel != null ? dialoguePanel.transform : transform;
        Transform questionRoot = parent.Find("SilkTraderQuestionGroup");
        if (questionRoot != null)
        {
            return questionRoot;
        }

        GameObject rootObject = new GameObject("SilkTraderQuestionGroup", typeof(RectTransform));
        rootObject.transform.SetParent(parent, false);
        rootObject.layer = parent.gameObject.layer;

        RectTransform rectTransform = rootObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, -45f);
        rectTransform.sizeDelta = new Vector2(650f, 350f);

        if (questionButtonTemplate != null)
        {
            questionButtonTemplate.gameObject.SetActive(false);
        }

        return rootObject.transform;
    }

    private static Button CreateButton(string buttonName, Transform parent)
    {
        GameObject buttonObject = new GameObject(buttonName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        buttonObject.layer = parent.gameObject.layer;

        GameObject labelObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        labelObject.layer = parent.gameObject.layer;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = buttonName;

        return buttonObject.GetComponent<Button>();
    }

    private static T FindChildComponent<T>(Transform root, string childName) where T : Component
    {
        if (root == null)
        {
            return null;
        }

        if (root.name == childName && root.TryGetComponent(out T rootComponent))
        {
            return rootComponent;
        }

        foreach (Transform child in root)
        {
            T component = FindChildComponent<T>(child, childName);
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }
}
