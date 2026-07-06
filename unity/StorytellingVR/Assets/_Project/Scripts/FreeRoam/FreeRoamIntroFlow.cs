using System.Collections;
using UnityEngine;

public class FreeRoamIntroFlow : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        ObjectiveUIManager.Instance.SetObjective("Listen");

        NarratorUIManager.Instance.ShowNarration(
            "Narrator",
            "Welcome to Hampi Bazaar. Take a moment to observe the people around you.",
            5f
        );

        yield return new WaitForSeconds(5.5f);

        NarratorUIManager.Instance.ShowNarration(
            "Narrator",
            "This marketplace is alive with merchants, travelers, craftsmen, and pilgrims. For many here, this is simply another ordinary morning.",
            7f
        );

        yield return new WaitForSeconds(7.5f);

        TutorialPromptUIManager.Instance.ShowPrompt(
            "Teleport Movement",
            "Use the teleport arc to move around the market. Aim at a clear spot on the street and release to teleport."
        );

        ObjectiveUIManager.Instance.SetObjective("Learn to move using teleport");
    }
}