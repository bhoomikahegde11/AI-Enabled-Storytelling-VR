using TMPro;
using UnityEngine;

public class TutorialPromptUIManager : MonoBehaviour
{
    public static TutorialPromptUIManager Instance;

    [Header("UI")]
    public GameObject promptCanvas;
    public TMP_Text promptTitleText;
    public TMP_Text promptBodyText;

    private object currentOwner = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePrompt();
    }

    public void ShowPrompt(string title, string body, object owner = null)
    {
        currentOwner = owner;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);

        if (promptTitleText != null)
            promptTitleText.text = title;

        if (promptBodyText != null)
            promptBodyText.text = body;
    }

    public void HidePrompt(object owner = null)
    {
        if (owner != null && currentOwner != null && currentOwner != owner)
            return;

        currentOwner = null;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    public bool IsCurrentOwner(object owner)
    {
        return currentOwner != null && currentOwner == owner;
    }
}