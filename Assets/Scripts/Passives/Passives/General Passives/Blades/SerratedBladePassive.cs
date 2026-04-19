using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Blades/Serrated Blade")]
public class SerratedBladePassive : DamageConversionClass
{

    public BleedingPassiveDefinition bleedingPassive;

    private bool applyStatus = false;

    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(!(skill is DamageSkillParent)) return;
        if(skill.damageType == convertingDamageType) applyStatus = true;
        if(ShouldConvert(self, skill))
        {
            var damageSkill = skill as DamageSkillParent;
            damageSkill.ConvertDamageType(conversionType);
        }
    }

    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(!(skill is DamageSkillParent)) return;
        if(applyStatus)
        {
            //Use AddPasive instead of QueuePassiveToAdd to ensure the bleeding 
            target.AddPassive(bleedingPassive, self);
        }
        applyStatus = false;
    }
}
