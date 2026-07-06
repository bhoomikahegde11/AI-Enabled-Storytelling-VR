using TMPro;
using UnityEngine;

public class ObjectiveUIManager : MonoBehaviour
{
    public static ObjectiveUIManager Instance;

    [Header("UI")]
    public GameObject objectiveCanvas;
    public TMP_Text objectiveTitleText;
    public TMP_Text objectiveBodyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideObjective();
    }

    public void SetObjective(string objective)
    {
        objectiveCanvas.SetActive(true);

        if (objectiveTitleText != null)
            objectiveTitleText.text = "Current Objective";

        objectiveBodyText.text = objective;
    }

    public void CompleteObjective(string completedText)
    {
        objectiveCanvas.SetActive(true);

        if (objectiveTitleText != null)
            objectiveTitleText.text = "Objective Complete";

        objectiveBodyText.text = completedText;
    }

    public void HideObjective()
    {
        if (objectiveCanvas != null)
            objectiveCanvas.SetActive(false);
    }
}