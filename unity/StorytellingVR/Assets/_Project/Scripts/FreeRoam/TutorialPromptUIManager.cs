using TMPro;
using UnityEngine;

public class TutorialPromptUIManager : MonoBehaviour
{
    public static TutorialPromptUIManager Instance;

    [Header("UI")]
    public GameObject promptCanvas;
    public TMP_Text promptTitleText;
    public TMP_Text promptBodyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePrompt();
    }

    public void ShowPrompt(string title, string body)
    {
        promptCanvas.SetActive(true);

        promptTitleText.text = title;
        promptBodyText.text = body;
    }

    public void HidePrompt()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}