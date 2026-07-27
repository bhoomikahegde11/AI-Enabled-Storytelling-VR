using UnityEngine;

public class NPCSubtitlePositionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SubtitleFollower subtitleFollower;

    [SerializeField]
    private Transform subtitleAnchor;

    private void Awake()
    {
        if (subtitleFollower == null)
        {
            subtitleFollower =
                FindFirstObjectByType<SubtitleFollower>();
        }
    }

    public void BeginNPCDialogue()
    {
        if (subtitleFollower == null)
        {
            Debug.LogError(
                "[NPC SUBTITLE] SubtitleFollower was not found."
            );

            return;
        }

        if (subtitleAnchor == null)
        {
            Debug.LogError(
                $"[NPC SUBTITLE] Subtitle anchor is missing on {name}."
            );

            return;
        }

        subtitleFollower.UseFixedModeAt(
            subtitleAnchor
        );
    }

    public void EndNPCDialogue()
    {
        if (subtitleFollower != null)
        {
            subtitleFollower.UseFollowMode();
        }
    }
}