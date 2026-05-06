using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Damage Skill")]
public class DamageSkill : DamageSkillParent
{

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        SkillDamageType damageType = this.skillDetailShell.damageType;

        if (damageType == SkillDamageType.Adaptive)
        {
            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            subType = DamageSubType.None;
            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value > highestCount)
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                    damageType = subTypeToDamageType[subType];
                }
            }
        }


        return EstimateExpectedDamageInternal(
            user.GetEffectiveStats(),
            target.GetEffectiveStats(),
            power,
            damageType,
            skillCritChance,
            skillCritDamage,
            subType);

        
    
    }

    //NOTE: When you add animations, add them directly to the skill and have them execute in this function.
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null || target.IsDead) return;

        BeforeSkillExecute(user, target);

        BeforeDamageSkillExecute(user, target);

        
        var damageSkillDetailShell = skillDetailShell as DamageSkillParent;
        var power = damageSkillDetailShell.power;
        var damageVariance = damageSkillDetailShell.damageVariance;
        var skillCritChance = damageSkillDetailShell.skillCritChance;
        var skillCritDamage = damageSkillDetailShell.skillCritDamage;

        SkillDamageType damageType = this.skillDetailShell.damageType;

        if (damageType == SkillDamageType.Adaptive)
        {
            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            subType = DamageSubType.None;
            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value > highestCount)
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                    damageType = subTypeToDamageType[subType];
                }
            }
            Debug.Log($"{this.skillDetailShell.damageType} damage type determined: {damageType} based on sub-type {subType}");
        }

        int powerRange = Random.Range(power - damageVariance, power + damageVariance);

        int damage = ComputeActualDamage(
            user.GetEffectiveStats(), target.GetEffectiveStats(),
            power,
            damageType,
            skillCritChance,
            skillCritDamage,
            subType);

        damage = user.ApplyTraitDamageModifiers(this, target, damage);
        damage = target.ApplyIncomingDamageModifiers(damage);
        damage = user.ApplyOutgoingDamageModifiers(damage);

        int dealt = target.TakeDamage(damage, skillDetailShell.damageType, subType);
        target.ClearIncomingDamageModifiers();
        user.ClearOutgoingDamageModifiers();

        if(BattleTurnManager.Instance != null)
            BattleTurnManager.Instance.RegisterDamage(user, target, dealt, skillDetailShell.damageType, subType);


        
        AfterExecute(user, target);
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
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

        // Defense mitigation: def / (defenseScale + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (defenseScale + targetDef))
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
        int targetDef     = Mathf.CeilToInt(baseDef * mainDefenseCalculated + subDef * subDefenseCalculated); // Sub-defense is partially calculated for flat damage skills

        float baseDamage = skillPower * casterOffense * 0.01f;

        // Defense mitigation term: def / (100 + def)
        float defMitigation = (targetDef > 0)
            ? (targetDef / (defenseScale + targetDef))
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
