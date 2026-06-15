using System.Collections.Generic;

public static class FranciscoDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "francisco_de_almeida",
            "Francisco de Almeida",
            NpcPersonalityBucket.Strict,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Francisco de Almeida stands at your stall for {quantityLabel} of {spiceName}. Let us be direct.",
                    "I am Francisco. I came by sea route for {spiceName}, not for delay."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Courtesy is fine, merchant, but profit keeps the ships moving.",
                    "Save the pleasantries. Let us return to the bargain."
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "Be precise, merchant. {ruleReply}"
                }, "francisco_de_almeida"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "I crossed sea routes for trade, not idle talk."
                }, "francisco_de_almeida")
            });
    }
}
