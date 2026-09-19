using System.Collections.Generic;

public class CharacterDialogueSet
{
    public string characterId;
    public string displayName;
    public NpcPersonalityBucket personality;
    public List<DialogueLine> lines;

    public CharacterDialogueSet(string characterId, string displayName, NpcPersonalityBucket personality, List<DialogueLine> lines)
    {
        this.characterId = characterId;
        this.displayName = displayName;
        this.personality = personality;
        this.lines = lines ?? new List<DialogueLine>();
    }
}
