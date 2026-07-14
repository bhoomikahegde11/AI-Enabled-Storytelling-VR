using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    public string npcName;

    [TextArea(3, 8)]
    public string openingDialogue;

    [TextArea(3, 8)]
    public string closingDialogue;

    public DialogueQuestion[] questions;
}

[System.Serializable]
public class DialogueQuestion
{
    public string question;

    [TextArea(3, 8)]
    public string response;
}