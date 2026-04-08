using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Zero Luck")]
public class ZeroLuckPassive : PassivesDefinition
{
    public override void BeforeSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        skill.skillDetailShell.bonusEffectChance -= Globals.MAX_STAT_CHANGE;
    }

    public override void BeforeSkillExecuteReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        skill.skillDetailShell.bonusEffectChance -= Globals.MAX_STAT_CHANGE;
    }

    public override void GetStatBoosts(BattleCharacter self)
    {
        self.bonusStats.critChance -= Globals.MAX_STAT_CHANGE;
    }

    public override void OnSkillReceivedEnd(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnSkillReceivedEnd);
    }
    
    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnSkillUsedEnd);
    }
}
