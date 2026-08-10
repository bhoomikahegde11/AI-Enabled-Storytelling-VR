using System.Collections;
using UnityEngine;

public class FreeRoamIntroFlow : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Disable all teleport hotspots immediately on startup
        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance.SetAllTeleportEnabled(false);
        }

        yield return new WaitForSeconds(1f);

        ObjectiveUIManager.Instance.SetObjective("Listen");

        // Yield on the first narration coroutine
        yield return StartCoroutine(NarratorUIManager.Instance.PlayNarration(
            "Narrator",
            "Welcome to Hampi Bazaar. Take a moment to observe the people around you.",
            5f
        ));

        yield return new WaitForSeconds(0.5f);

        // Yield on the second narration coroutine
        yield return StartCoroutine(NarratorUIManager.Instance.PlayNarration(
            "Narrator",
            "This marketplace is alive with merchants, travelers, craftsmen, and pilgrims. For many here, this is simply another ordinary morning.",
            7f
        ));

        yield return new WaitForSeconds(0.5f);

        // Show the movement tutorial prompt
        TutorialPromptUIManager.Instance.ShowPrompt(
            "Teleport",
            "Use the RIGHT JOYSTICK to aim at a hotspot, then release it to teleport."
        );

        ObjectiveUIManager.Instance.SetObjective("Learn to move using teleport");

        // Enable only the tutorial hotspots when the prompt appears
        if (TeleportLockController.Instance != null)
        {
            TeleportLockController.Instance.SetTutorialHotspotsEnabled(true);
        }
    }
}