using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Zero Luck")]
public class ZeroLuckPassive : PassivesDefinition
{
    public override void GetStatBoosts(BattleCharacter self)
    {
        self.bonusStats.critChance -= Globals.MAX_STAT_CHANGE;
    }

    public override void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        self.QueuePassiveToRemove(this);
    }
    
    public override void OnSkillUsedEnd(BattleCharacter self, Skill skill)
    {
        self.QueuePassiveToRemove(this);
    }
}
