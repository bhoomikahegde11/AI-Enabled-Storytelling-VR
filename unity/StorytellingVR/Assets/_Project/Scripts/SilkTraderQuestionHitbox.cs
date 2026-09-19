using UnityEngine;

public class SilkTraderQuestionHitbox : MonoBehaviour
{
    public enum HitboxAction
    {
        Question,
        Answer,
        Close
    }

    public SilkTraderDialogueManager dialogueManager;
    public HitboxAction action;
    public int questionIndex;

    public void SelectQuestion()
    {
        if (dialogueManager == null)
        {
            return;
        }

        switch (action)
        {
            case HitboxAction.Question:
                dialogueManager.ShowAnswer(questionIndex);
                break;
            case HitboxAction.Answer:
                dialogueManager.ResetResponse();
                break;
            case HitboxAction.Close:
                dialogueManager.CloseDialogue();
                break;
        }
    }

    public void SetHovered(bool hovered)
    {
        if (dialogueManager != null && action == HitboxAction.Question)
        {
            dialogueManager.SetHoveredQuestion(hovered ? questionIndex : -1);
        }
    }
}
