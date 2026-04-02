using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DamageConversionClass : PassivesDefinition
{
    public SkillDamageType convertingDamageType = SkillDamageType.None;
    public DamageSubType conversionType = DamageSubType.None;

    private int counter = 0;

    public bool ShouldConvert(BattleCharacter user, Skill skill)
    {
        if(convertingDamageType != skill.damageType)
            return false;

        counter++;
        int conversionCount = 0;
        List<DamageConversionClass> damageConversionPassives = new List<DamageConversionClass>();
        foreach(var passive in user.passives)
        {
            if (passive is DamageConversionClass)
            {
                if (((DamageConversionClass)passive).convertingDamageType == skill.damageType)
                {
                    conversionCount++;
                    damageConversionPassives.Add((DamageConversionClass)passive);
                }
            }
        }
        counter = counter % conversionCount;

        if (this == damageConversionPassives[counter])
        {
            return true;
        }

        return false;
    }
}
