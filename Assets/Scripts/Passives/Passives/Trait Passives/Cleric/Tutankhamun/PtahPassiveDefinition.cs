using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Cleric/Tutankhamun/Ptah")]
public class PtahPassiveDefinition : PassivesDefinition
{
    public float activationHealthPercent = .5f;
    public StasisPassiveDefinition passiveToApply;

    public override void BeforeSkillExecuteReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        base.BeforeSkillExecuteReceived(self, attacker, skill);
    }

    public override void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount, SkillDamageType damageType, DamageSubType subType)
    {
        if (target == null || target.IsDead) return;
        if (target.CurrentHealth < target.MaxHealth * activationHealthPercent && target.CurrentHealth + amount >= target.MaxHealth * activationHealthPercent)
        {
            target.AddPassive(passiveToApply, self);
        }
    }
}
