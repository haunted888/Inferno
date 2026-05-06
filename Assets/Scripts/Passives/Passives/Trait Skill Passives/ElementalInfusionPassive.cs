using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Elemental Infusion")]
public class ElementalInfusionPassive : DamageConversionClass
{
    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(skill is not DamageSkillParent) return;
        if(ShouldConvert(self, skill))
        {
            var damageSkill = skill as DamageSkillParent;
            damageSkill.ConvertDamageType(conversionType);
        }
    }
}
