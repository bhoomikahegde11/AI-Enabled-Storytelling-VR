using System.Collections;
using UnityEngine;

public class GateIntroFlow : MonoBehaviour
{
    [System.Serializable]
    public struct NarrationLine
    {
        public string lineId;
        public string text;
    }

    [Header("Timing (Arrival)")]
    [SerializeField] private float startupSilentDelay = 2.5f;
    [SerializeField] private float postLookPause = 1.2f;
    [SerializeField] private float betweenNarrationLines = 0.5f;
    [SerializeField] private float postNarrationPause = 2f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Look Around Tutorial")]
    [SerializeField] private string lookAroundTitle = "Look Around";
    [SerializeField] private string lookAroundBody = "Use the LEFT JOYSTICK to look around.";
    
    [Tooltip("Assign the XR camera or CenterEyeAnchor here to track head rotation.")]
    [SerializeField] private Transform playerHeadCamera;
    
    [Tooltip("Total horizontal rotation (yaw) required to proceed.")]
    [SerializeField] private float lookAroundThresholdDegrees = 35f;

    [Header("Objective")]
    [SerializeField] private string objectiveText = "Look around";

    [Header("Narration")]
    [SerializeField] private string narratorName = "Narrator (V.O.)";
    [SerializeField] private NarrationLine[] narratorLines = new NarrationLine[]
    {
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_01", text = "You seem to be lost, traveler." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_02", text = "That is understandable." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_03", text = "A moment ago, this place was unfamiliar to you." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_04", text = "Now, you stand before Hampi \u2014 the roaring heart of the Vijayanagara Empire." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_05", text = "You may be wondering why you are here." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_06", text = "For now, do not search for every answer at once." },
        new NarrationLine { lineId = "GATE_INTRO_NARRATOR_07", text = "Let me be your guide." }
    };

    [Header("Transition")]
    [SerializeField] private ScreenFader screenFader;

    private IEnumerator Start()
    {
        // 1. Scene begins. Player gets a short silent moment.
        yield return new WaitForSecondsRealtime(startupSilentDelay);

        // Optional objective
        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(objectiveText);
        }

        // 3. Show tutorial prompt
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowLeftJoystickPrompt(
                lookAroundTitle,
                lookAroundBody,
                this
            );
        }

        // 5. Wait until the player has genuinely looked around.
        yield return WaitForPlayerLookAround();

        // 6. Hide the look-around tutorial.
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.HidePrompt(this);
        }

        // 7. Small pause.
        yield return new WaitForSecondsRealtime(postLookPause);

        // 8. Play a sequence of narrator lines, one after another.
        if (NarratorUIManager.Instance != null)
        {
            foreach (var line in narratorLines)
            {
                yield return NarratorUIManager.Instance.PlayNarration(
                    line.lineId,
                    narratorName,
                    line.text
                );
                
                yield return new WaitForSecondsRealtime(betweenNarrationLines);
            }
        }
        else
        {
            Debug.LogWarning("[GATE INTRO] NarratorUIManager.Instance is missing.");
        }

        // 11. After narration completes, pause briefly.
        yield return new WaitForSecondsRealtime(postNarrationPause);

        // 12. Fade the screen to WHITE.
        if (screenFader != null)
        {
            screenFader.fadeColor = Color.white;
            screenFader.speed = 1f / fadeDuration;
            yield return screenFader.FadeOut();
        }

        // 13. Let the canonical progression system choose the next playable scene.
        if (GameManager.Instance != null)
        {
            Debug.Log("[GATE INTRO] Intro completed. Requesting next scene from GameManager.");
            GameManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("[GATE INTRO] GameManager.Instance is missing. Cannot advance scene flow.");
        }
    }

    private IEnumerator WaitForPlayerLookAround()
    {
        if (playerHeadCamera == null)
        {
            Debug.LogWarning("[GATE INTRO] playerHeadCamera is not assigned. Waiting a default time instead.");
            yield return new WaitForSecondsRealtime(3f);
            yield break;
        }

        float initialYaw = playerHeadCamera.eulerAngles.y;
        float maxDeviation = 0f;

        while (maxDeviation < lookAroundThresholdDegrees)
        {
            float currentYaw = playerHeadCamera.eulerAngles.y;
            float delta = Mathf.Abs(Mathf.DeltaAngle(initialYaw, currentYaw));
            if (delta > maxDeviation)
            {
                maxDeviation = delta;
            }
            yield return null;
        }
    }
}
