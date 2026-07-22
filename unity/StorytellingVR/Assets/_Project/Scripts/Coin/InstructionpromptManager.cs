using UnityEngine;

public class InstructionPromptManager : MonoBehaviour
{
    private void Awake()
    {
        if (PromptManager.Instance != null)
            PromptManager.Instance.HidePrompt();
    }

    public void ShowTrigger(string message)
    {
        if (PromptManager.Instance == null)
            return;

        PromptManager.Instance.ShowPrompt(
            message,
            PromptManager.Instance.rightTriggerButton
        );
    }

    public void ShowWristRotate(string message)
    {
        if (PromptManager.Instance == null)
            return;

        PromptManager.Instance.ShowPrompt(
            message,
            PromptManager.Instance.gripButton
        );
    }

    public void Hide()
    {
        if (PromptManager.Instance == null)
            return;

        PromptManager.Instance.HidePrompt();
    }
}