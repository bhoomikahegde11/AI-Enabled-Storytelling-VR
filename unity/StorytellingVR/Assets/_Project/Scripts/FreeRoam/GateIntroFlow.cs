using System.Collections;
using UnityEngine;

public class GateIntroFlow : MonoBehaviour
{
    [SerializeField] private float startupDelay = 1f;

    [Header("Tutorial Prompt")]
    [SerializeField] private string promptTitle = "Prompt";
    [SerializeField]
    private string promptBody =
        "Use the Left Joystick to look at your surroundings.";

    [Header("Objective")]
    [SerializeField] private string objectiveText = "Look around";

    [Header("Narration")]
    [SerializeField] private string narratorName = "Narrator (V.O.)";
    [SerializeField]
    private string narratorLine =
        "You seem to be lost, traveler. Let me be your guide. Welcome to Hampi, the roaring heart of the Vijayanagara Empire.";

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(startupDelay);

        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(objectiveText);
        }

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowPrompt(
                promptTitle,
                promptBody
            );
        }

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                narratorName,
                narratorLine
            );
        }
    }
}

