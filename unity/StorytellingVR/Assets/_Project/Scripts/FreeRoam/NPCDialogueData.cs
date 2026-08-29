using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    public string npcName;

    [TextArea(3, 8)]
    public string openingDialogue;
    public string openingDialogueLineId;

    [TextArea(3, 8)]
    public string closingDialogue;
    public string closingDialogueLineId;

    public DialogueQuestion[] questions;
}

[System.Serializable]
public class DialogueQuestion
{
    public string question;

    [TextArea(3, 8)]
    public string response;

    public string answerVoiceLineId;
}