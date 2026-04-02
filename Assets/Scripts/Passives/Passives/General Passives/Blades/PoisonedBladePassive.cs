using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Blades/Poisoned Blade")]
public class PoisonedBladePassive : DamageConversionClass
{

    public PoisonPassiveDefinition poisonPassive;

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
            target.AddPassive(poisonPassive);
        }
        applyStatus = false;
    }
}
