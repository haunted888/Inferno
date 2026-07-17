using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Blades/Serrated Blade")]
public class SerratedBladePassive : DamageConversionClass
{

    public BleedingPassiveDefinition bleedingPassive;

    private bool applyStatus = false;

    public override bool BeforeDamageSkillExecuteOncePerSkill => true;

    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(skill is not DamageSkillParent) return;
        if(ShouldConvert(self, skill))
        {
            applyStatus = true;
            var damageSkill = skill as DamageSkillParent;
            damageSkill.ConvertDamageType(conversionType);
        }
    }

    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(skill is not DamageSkillParent) return;
        applyStatus = false;
    }

    public override void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount, SkillDamageType damageType, DamageSubType subDamageType)
    {
        
        if(applyStatus && amount > 0 && target != null && !target.IsDead && bleedingPassive != null)
        {
            target.AddPassive(bleedingPassive, self);
        }
    }
}
