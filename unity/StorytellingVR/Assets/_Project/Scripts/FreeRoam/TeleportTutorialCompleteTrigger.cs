using UnityEngine;

public class TeleportTutorialCompleteTrigger : MonoBehaviour
{
    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
            return;

        Debug.Log("[TELEPORT TUTORIAL] Something entered: " + other.name);

        completed = true;

        TutorialPromptUIManager.Instance.HidePrompt();

        ObjectiveUIManager.Instance.CompleteObjective("Learn to move using teleport");

        Invoke(nameof(ShowNextObjective), 2f);

        gameObject.SetActive(false);
    }

    private void ShowNextObjective()
    {
        ObjectiveUIManager.Instance.SetObjective("Talk to the local resident");

        NarratorUIManager.Instance.ShowNarration(
            "Narrator",
            "Good. Now that you can move through the bazaar, speak with someone nearby. A local resident may help you understand this place.",
            6f
        );
    }
}