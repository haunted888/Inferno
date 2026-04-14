using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Flat Damage Skill")]
public class FlatDamageSkill : DamageSkillParent
{

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {

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

        int dealt = target.TakeDamage(powerRange);
        target.ClearIncomingDamageModifiers();
        user.ClearOutgoingDamageModifiers();

        if(BattleTurnManager.Instance != null)
            BattleTurnManager.Instance.RegisterDamage(user, target, dealt);


        
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
        return skillPower; // Flat damage ignores all stats and modifiers except for trait-based damage modifiers
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
       return skillPower; // Flat damage ignores all stats and modifiers except for trait-based damage modifiers, which we can't reliably estimate, so we just return the base power as the estimate
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
