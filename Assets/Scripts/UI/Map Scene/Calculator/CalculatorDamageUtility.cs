using UnityEngine;

public static class CalculatorDamageUtility
{
    public static int EstimateDamage(DamageSkill skill, CombatStats userStats, CombatStats targetStats)
    {
        if (skill == null) return 0;

        int baseOff = (skill.damageType == SkillDamageType.Physical) ? userStats.physicalAttack : userStats.elementalPower;
        int baseDef = (skill.damageType == SkillDamageType.Physical) ? targetStats.defense : targetStats.elementalResistance;

        int subOff = GetSubAttack(userStats, skill.subType);
        int subDef = GetSubDefense(targetStats, skill.subType);

        int casterOffense = baseOff + subOff;
        int targetDef = baseDef + subDef;

        float baseDamage = skill.power * casterOffense * 0.01f;

        float defMitigation = (targetDef > 0) ? (targetDef / (100f + targetDef)) : 0f;

        int totalCritChance = Mathf.Max(0, userStats.critChance + skill.skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skill.skillCritDamage);

        // Match your Skill.EstimateExpectedDamageInternal behavior: only apply crit if guaranteed
        float critMultiplier = 1f;
        if (totalCritChance >= 100)
        {
            critMultiplier = totalCritDamage * 0.01f;
            defMitigation *= 0.5f;
        }

        float afterDef = baseDamage * (1f - defMitigation);
        float expected = afterDef * critMultiplier;

        return Mathf.Max(0, Mathf.RoundToInt(expected));
    }

    private static int GetSubAttack(CombatStats s, DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return s.bludgeoningAttack;
            case DamageSubType.Slashing:    return s.slashingAttack;
            case DamageSubType.Piercing:    return s.piercingAttack;
            case DamageSubType.Fire:        return s.fireAttack;
            case DamageSubType.Ice:         return s.iceAttack;
            case DamageSubType.Storm:       return s.stormAttack;
            case DamageSubType.Acid:        return s.acidAttack;
            case DamageSubType.Psychic:     return s.psychicAttack;
            case DamageSubType.Blood:       return s.bloodAttack;
            default:                        return 0;
        }
    }

    private static int GetSubDefense(CombatStats s, DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return s.bludgeoningDefense;
            case DamageSubType.Slashing:    return s.slashingDefense;
            case DamageSubType.Piercing:    return s.piercingDefense;
            case DamageSubType.Fire:        return s.fireDefense;
            case DamageSubType.Ice:         return s.iceDefense;
            case DamageSubType.Storm:       return s.stormDefense;
            case DamageSubType.Acid:        return s.acidDefense;
            case DamageSubType.Psychic:     return s.psychicDefense;
            case DamageSubType.Blood:       return s.bloodDefense;
            default:                        return 0;
        }
    }
}