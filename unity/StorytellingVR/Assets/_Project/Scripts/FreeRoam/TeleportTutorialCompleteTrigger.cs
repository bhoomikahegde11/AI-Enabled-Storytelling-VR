using System.Collections;
using UnityEngine;

public class TeleportTutorialCompleteTrigger : MonoBehaviour
{
    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[TELEPORT TUTORIAL] Trigger entered by: " + other.name);

        if (completed)
        {
            Debug.Log("[TELEPORT TUTORIAL] Ignored because tutorial is already complete.");
            return;
        }

        if (!IsPlayer(other))
        {
            Debug.LogWarning(
                "[TELEPORT TUTORIAL] Ignored collider because it was not recognised as the player: "
                + other.name
            );
            return;
        }

        Debug.Log("[TELEPORT TUTORIAL] Player accepted.");

        completed = true;

        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance.SetAllTeleportEnabled(false);
            Debug.Log("[TELEPORT TUTORIAL] Teleport hotspots disabled.");
        }
        else
        {
            Debug.LogWarning("[TELEPORT TUTORIAL] TeleportLockController.Instance is null.");
        }

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.HidePrompt();
            Debug.Log("[TELEPORT TUTORIAL] Tutorial prompt hidden.");
        }
        else
        {
            Debug.LogError("[TELEPORT TUTORIAL] TutorialPromptUIManager.Instance is null.");
        }

        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.CompleteObjective(
                "Learn to move using teleport"
            );
        }
        else
        {
            Debug.LogWarning("[TELEPORT TUTORIAL] ObjectiveUIManager.Instance is null.");
        }

        StartCoroutine(ShowNextObjectiveRoutine());
    }

    private bool IsPlayer(Collider other)
    {
        // Preferred component-based check.
        if (other.GetComponentInParent<CharacterController>() != null)
            return true;

        // Fallback for the Meta VR player collider used in this scene.
        if (other.name.Contains("PlayerController"))
            return true;

        if (other.transform.root.name.Contains("PlayerController"))
            return true;

        return false;
    }

    private IEnumerator ShowNextObjectiveRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(
                "Talk to the local resident"
            );
        }

        if (NarratorUIManager.Instance != null)
        {
            yield return StartCoroutine(
                NarratorUIManager.Instance.PlayNarration(
                    "Narrator",
                    "Good. Now that you can move through the bazaar, speak with someone nearby. A local resident may help you understand this place.",
                    6f
                )
            );
        }
        else
        {
            Debug.LogError("[TELEPORT TUTORIAL] NarratorUIManager.Instance is null.");
        }

        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance.SetGeneralHotspotsEnabled(true);
            Debug.Log("[TELEPORT TUTORIAL] General hotspots enabled.");
        }

        gameObject.SetActive(false);
    }
}