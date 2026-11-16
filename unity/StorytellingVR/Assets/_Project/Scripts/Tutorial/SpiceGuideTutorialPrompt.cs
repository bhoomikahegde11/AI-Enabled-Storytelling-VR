using System.Collections;
using TMPro;
using UnityEngine;

public class SpiceGuideTutorialPrompt : MonoBehaviour
{
    [SerializeField] private SpiceCostGuideController guideController;
    [SerializeField] private Animator[] animatorsToPause;
    [SerializeField] private MonoBehaviour[] scriptsToPause;
    [SerializeField] private AudioSource[] audioSourcesToLower;
    [SerializeField] private float loweredVolume = 0.2f;

    private void Awake() => guideController?.LockGuide();

    public IEnumerator RunTutorial(TMP_Text instructions)
    {
        if (guideController == null) yield break;

        var speeds = new float[animatorsToPause.Length];
        var enabled = new bool[scriptsToPause.Length];
        var volumes = new float[audioSourcesToLower.Length];
        for (int i = 0; i < speeds.Length; i++)
        {
            if (animatorsToPause[i] == null) continue;
            speeds[i] = animatorsToPause[i].speed;
            animatorsToPause[i].speed = 0f;
        }
        for (int i = 0; i < enabled.Length; i++)
        {
            if (scriptsToPause[i] == null) continue;
            enabled[i] = scriptsToPause[i].enabled;
            scriptsToPause[i].enabled = false;
        }
        for (int i = 0; i < volumes.Length; i++)
        {
            if (audioSourcesToLower[i] == null) continue;
            volumes[i] = audioSourcesToLower[i].volume;
            audioSourcesToLower[i].volume *= loweredVolume;
        }

        try
        {
            guideController.UnlockGuide();
            if (instructions != null) instructions.text = "Press X to open the Spice Cost Guide.";
            yield return new WaitUntil(() => guideController.IsOpen);

            if (instructions != null) instructions.text = "Take your time. Press X again when you are ready to close the guide and continue.";
            yield return new WaitUntil(() => !guideController.IsOpen);
        }
        finally
        {
            for (int i = 0; i < speeds.Length; i++)
                if (animatorsToPause[i] != null) animatorsToPause[i].speed = speeds[i];
            for (int i = 0; i < enabled.Length; i++)
                if (scriptsToPause[i] != null) scriptsToPause[i].enabled = enabled[i];
            for (int i = 0; i < volumes.Length; i++)
                if (audioSourcesToLower[i] != null) audioSourcesToLower[i].volume = volumes[i];
        }
    }
}
