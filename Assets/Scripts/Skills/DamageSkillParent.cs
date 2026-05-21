using UnityEngine;

public abstract class DamageSkillParent : Skill
{
    
    
    [Header("Damage Skill")]
    public int power = 10;
    public DamageSubType subType = DamageSubType.None;
    public int skillCritChance = 0;
    public int skillCritDamage = 0;
    public int damageVariance = 0;
    public readonly float defenseScale = 1000f; // Higher means negative defense is less impactful. x means 200% damage at -x def.
    public float lifeStealPercent = 0f; 

    [Header("Ignore Defenses")]
    public float mainDefenseCalculated = 1f;
    public float subDefenseCalculated = 1f;

    private DamageSubType permanentType;

    public void Awake()
    {
        permanentType = subType;
    }


    public void AfterExecute(BattleCharacter user, BattleCharacter target)
    {
        subType = permanentType;
    }

    public void ConvertDamageType(DamageSubType newType)
    {
        subType = newType;
    }

    public int ComputeActualDamage(
    CombatStats userStats,
    CombatStats targetStats,
    int skillPower,
    SkillDamageType type,
    int skillCritChance,
    int skillCritDamage,
    DamageSubType subType)
    {
        // Base offense/defense (physical or elemental)
        int baseOff = (type == SkillDamageType.Physical)
            ? userStats.physicalAttack
            : userStats.elementalPower;

        int baseDef = (type == SkillDamageType.Physical)
            ? targetStats.defense
            : targetStats.elementalResistance;

        // Sub-type bonuses
        int subOff = GetSubTypeAttack(userStats, subType);
        int subDef = GetSubTypeDefense(targetStats, subType);

        int casterOffense = baseOff + subOff;
        int targetDef     = Mathf.CeilToInt(baseDef * mainDefenseCalculated + subDef * subDefenseCalculated);
        // Base damage
        float baseDamage = skillPower * casterOffense * 0.01f;

        // Defense mitigation: def / (defenseScale + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (baseDamage + targetDef))
            : targetDef / defenseScale;

        int totalCritChance = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        bool isCrit = Random.Range(0f, 100f) < totalCritChance;

        if (isCrit && defMitigation > 0)
            // Crits ignore 50% of defense mitigation
            defMitigation *= 0.5f;

        float afterDef = baseDamage * (1f - defMitigation);

        float critMultiplier = 1f;
        if (isCrit)
            critMultiplier = totalCritDamage * 0.01f;
        //
        
        float final = afterDef * critMultiplier;
        return Mathf.Max(0, Mathf.RoundToInt(final));
    }


    public int EstimateExpectedDamageInternal(
        CombatStats userStats,
        CombatStats targetStats,
        int skillPower,
        SkillDamageType type,
        int skillCritChance,
        int skillCritDamage,
        DamageSubType subType)
    {
        // Base offense/defense (physical or elemental)
        int baseOff = (type == SkillDamageType.Physical)
            ? userStats.physicalAttack
            : userStats.elementalPower;

        int baseDef = (type == SkillDamageType.Physical)
            ? targetStats.defense
            : targetStats.elementalResistance;

        // Sub-type bonuses
        int subOff = GetSubTypeAttack(userStats, subType);
        int subDef = GetSubTypeDefense(targetStats, subType);

        int casterOffense = baseOff + subOff;
        int targetDef     = Mathf.CeilToInt(baseDef * mainDefenseCalculated + subDef * subDefenseCalculated); // Sub-defense is partially calculated for flat damage skills

        float baseDamage = skillPower * casterOffense * 0.01f;

        // Defense mitigation term: def / (baseDamage + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (baseDamage + targetDef))
            : targetDef / defenseScale;



        int totalCritChance = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        float critMultiplier;

        if (totalCritChance >= 100)
        {
            // Guaranteed crit: use full crit multiplier
            critMultiplier = totalCritDamage * 0.01f;
            if(defMitigation > 0)
                defMitigation *= 0.5f;
        }
        else
        {
            // Crit is not guaranteed: ignore crit for expectation
            critMultiplier = 1f;
        }

        float afterDef = baseDamage * (1f - defMitigation);

        

        float expected = afterDef * critMultiplier;
        return Mathf.Max(0, Mathf.RoundToInt(expected));
    }


    
    public int GetSubTypeAttack(CombatStats stats, DamageSubType subType)
    {
        return subType switch
        {
            DamageSubType.Bludgeoning => stats.bludgeoningAttack,
            DamageSubType.Slashing => stats.slashingAttack,
            DamageSubType.Piercing => stats.piercingAttack,
            DamageSubType.Fire => stats.fireAttack,
            DamageSubType.Ice => stats.iceAttack,
            DamageSubType.Storm => stats.stormAttack,
            DamageSubType.Acid => stats.acidAttack,
            DamageSubType.Psychic => stats.psychicAttack,
            DamageSubType.Blood => stats.bloodAttack,
            _ => 0,
        };
    }

    public int GetSubTypeDefense(CombatStats stats, DamageSubType subType)
    {
        return subType switch
        {
            DamageSubType.Bludgeoning => stats.bludgeoningDefense,
            DamageSubType.Slashing => stats.slashingDefense,
            DamageSubType.Piercing => stats.piercingDefense,
            DamageSubType.Fire => stats.fireDefense,
            DamageSubType.Ice => stats.iceDefense,
            DamageSubType.Storm => stats.stormDefense,
            DamageSubType.Acid => stats.acidDefense,
            DamageSubType.Psychic => stats.psychicDefense,
            DamageSubType.Blood => stats.bloodDefense,
            _ => 0,
        };
    }
    

}
