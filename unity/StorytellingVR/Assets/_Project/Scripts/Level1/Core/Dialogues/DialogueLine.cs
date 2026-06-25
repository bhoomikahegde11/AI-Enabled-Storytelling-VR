using System;

[Serializable]
public class DialogueLine
{
    public DialogueScenario scenario;
    public string characterId;
    public PlayerReputationBucket? reputationBucket;
    public NpcPatienceBucket? patienceBucket;
    public NpcDesperationBucket? desperationBucket;
    public NpcAggressionBucket? aggressionBucket;
    public RoundBucket? roundBucket;
    public NpcPersonalityBucket? personalityBucket;
    public string[] templates;

    public DialogueLine(
        DialogueScenario scenario,
        string[] templates,
        string characterId = null,
        PlayerReputationBucket? reputationBucket = null,
        NpcPatienceBucket? patienceBucket = null,
        NpcDesperationBucket? desperationBucket = null,
        RoundBucket? roundBucket = null,
        NpcPersonalityBucket? personalityBucket = null,
        NpcAggressionBucket? aggressionBucket = null)
    {
        this.scenario = scenario;
        this.templates = templates;
        this.characterId = characterId;
        this.reputationBucket = reputationBucket;
        this.patienceBucket = patienceBucket;
        this.desperationBucket = desperationBucket;
        this.roundBucket = roundBucket;
        this.personalityBucket = personalityBucket;
        this.aggressionBucket = aggressionBucket;
    }
}
