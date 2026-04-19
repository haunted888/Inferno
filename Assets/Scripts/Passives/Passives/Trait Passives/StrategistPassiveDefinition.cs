using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Strategist Passive Definition")]
public class StrategistPassiveDefinition : PassivesDefinition
{
    public PassivesDefinition zeroLuckPassive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        attacker.AddPassive(zeroLuckPassive, self);
    }
    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        self.QueuePassiveToAdd(zeroLuckPassive, PassivesDefinition.PassiveHook.OnSkillUsed, self);
    }
}
