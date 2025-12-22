using UnityEngine;

[CreateAssetMenu(menuName = "Passives/StrategistPassiveDefinition")]
public class StrategistPassiveDefinition : PassivesDefinition
{
    public PassivesDefinition zeroLuckPassive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        attacker.AddPassive(zeroLuckPassive);
    }
    public override void OnSkillUsed(BattleCharacter self, Skill skill)
    {
        self.AddPassive(zeroLuckPassive);
    }
}
