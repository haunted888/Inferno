using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Damage Skill")]
public class DamageSkill : Skill
{
    [Header("Damage Skill")]
    public int power = 10;
    public DamageSubType subType = DamageSubType.None;
    public int skillCritChance = 0;
    public int skillCritDamage = 0;

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return 0;
        return EstimateDamage(user.GetEffectiveStats(), target.GetEffectiveStats());
    }

    public int EstimateDamage(CombatStats userStats, CombatStats targetStats)
    {
        return EstimateExpectedDamageInternal(
            userStats,
            targetStats,
            power,
            damageType,
            skillCritChance,
            skillCritDamage,
            subType);
    }

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null || target.IsDead) return;

        int damage = ComputeActualDamage(
            user.GetEffectiveStats(), target.GetEffectiveStats(),
            power,
            damageType,
            skillCritChance,
            skillCritDamage,
            subType);

        damage = user.ApplyTraitDamageModifiers(this, target, damage);
        int dealt = target.TakeDamage(damage);
        BattleTurnManager.Instance?.RegisterDamage(user, target, dealt);

        ExecuteFollowUps(user, target);
    }


    // ===== Damage helpers =====

    protected int ComputeActualDamage(
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
        int targetDef     = baseDef + subDef;
        // Base damage
        float baseDamage = skillPower * casterOffense * 0.01f;

        // Defense mitigation: def / (100 + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;

        int totalCritChance = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        bool isCrit = Random.Range(0f, 100f) < totalCritChance;

        if (isCrit)
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


    protected int EstimateExpectedDamageInternal(
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
        int targetDef     = baseDef + subDef;

        float baseDamage = skillPower * casterOffense * 0.01f;

        // Defense mitigation term: def / (100 + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;



        int totalCritChance = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        float critMultiplier;

        if (totalCritChance >= 100)
        {
            // Guaranteed crit: use full crit multiplier
            critMultiplier = totalCritDamage * 0.01f;
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

    protected int GetSubTypeAttack(CombatStats stats, DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return stats.bludgeoningAttack;
            case DamageSubType.Slashing:    return stats.slashingAttack;
            case DamageSubType.Piercing:    return stats.piercingAttack;
            case DamageSubType.Fire:        return stats.fireAttack;
            case DamageSubType.Ice:         return stats.iceAttack;
            case DamageSubType.Storm:       return stats.stormAttack;
            case DamageSubType.Acid:        return stats.acidAttack;
            case DamageSubType.Psychic:     return stats.psychicAttack;
            case DamageSubType.Blood:       return stats.bloodAttack;
            default:                        return 0;
        }
    }

    protected int GetSubTypeDefense(CombatStats stats, DamageSubType subType)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning: return stats.bludgeoningDefense;
            case DamageSubType.Slashing:    return stats.slashingDefense;
            case DamageSubType.Piercing:    return stats.piercingDefense;
            case DamageSubType.Fire:        return stats.fireDefense;
            case DamageSubType.Ice:         return stats.iceDefense;
            case DamageSubType.Storm:       return stats.stormDefense;
            case DamageSubType.Acid:        return stats.acidDefense;
            case DamageSubType.Psychic:     return stats.psychicDefense;
            case DamageSubType.Blood:       return stats.bloodDefense;
            default:                        return 0;
        }
    }

}
