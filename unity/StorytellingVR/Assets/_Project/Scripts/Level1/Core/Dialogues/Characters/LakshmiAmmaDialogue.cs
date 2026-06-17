using System.Collections.Generic;

public static class LakshmiAmmaDialogue
{
    public static CharacterDialogueSet Create()
    {
        return new CharacterDialogueSet(
            "lakshmi_amma",
            "Lakshmi Amma",
            NpcPersonalityBucket.Friendly,
            new List<DialogueLine>
            {
                new DialogueLine(DialogueScenario.CustomerGreeting, new[]
                {
                    "Lakshmi Amma has come for {quantityLabel} of {spiceName}, merchant. Let us keep this practical.",
                    "Good day. I need {quantityLabel} of {spiceName} for the household stores."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.SocialGreeting, new[]
                {
                    "Warm words are good, but I still need a sensible price.",
                    "You are kind, merchant. Now tell me what we can settle."
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.Unknown, new[]
                {
                    "I need clearer words than that. {ruleReply}"
                }, "lakshmi_amma"),
                new DialogueLine(DialogueScenario.OffTopic, new[]
                {
                    "Let us speak of the household purchase, not of other matters."
                }, "lakshmi_amma")
            });
    }
}
