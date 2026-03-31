using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Damage Bonus Passive")]
public class DamageBonusPassiveDefinition : PassivesDefinition
{
    public float damageBonusPercent = 0.20f;

    public int turnsToLast = 1;

    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if (self == null || skill == null) return;
        if (skill.damageType == SkillDamageType.None) return;

        self.AddOutgoingDamageMultiplier(damageBonusPercent);
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        turnsToLast--;
        if (turnsToLast <= 0)
        {
            self.QueuePassiveToRemove(this);
        }
    }
}
