// DamageAllEnemiesSkill.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Damage All Enemies")]
public class DamageAllEnemiesSkill : Skill
{
    [Header("Damage AOE SKill")]
    public affectsCharacters characters;
    public int power = 10;
    public DamageSubType subType = DamageSubType.None;

    public int skillCritChance = 0;
    public int skillCritDamage = 0;


    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return 0;

        SkillDamageType damageType = this.damageType;

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


    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;

        List<BattleCharacter> group;

        switch (characters)
        {
            case affectsCharacters.Target:
                group = new List<BattleCharacter> { target };
                break;
            case affectsCharacters.TargetTeam:
                group = new List<BattleCharacter>(target.GetAllies());
                break;
            case affectsCharacters.Self:
                group = new List<BattleCharacter> { user };
                break;
            case affectsCharacters.Allies:
                group = new List<BattleCharacter>(user.GetAllies());
                break;
            case affectsCharacters.Enemies:
                group = new List<BattleCharacter>(user.GetEnemies());
                break;
            case affectsCharacters.AllOtherAllies:
                group = new List<BattleCharacter>(user.GetAllies());
                group.Remove(user);
                break;
            default:
                group = new List<BattleCharacter>(target.GetAllies());
                break;
        }

        SkillDamageType damageType = this.damageType;

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
        
        foreach (var member in group)
        {
            if (member == null || member.IsDead) continue;
            
            


            int damage = ComputeActualDamage(
                user.GetEffectiveStats(),
                member.GetEffectiveStats(),
                power,
                damageType,
                skillCritChance,
                skillCritDamage,
                subType);

            damage = user.ApplyTraitDamageModifiers(this, member, damage);
            damage = member.ApplyIncomingDamageModifiers(damage);
            damage = user.ApplyOutgoingDamageModifiers(damage);

            int dealt = member.TakeDamage(damage);
            member.ClearIncomingDamageModifiers();
            user.ClearOutgoingDamageModifiers();

            BattleTurnManager.Instance?.RegisterDamage(user, member, dealt);
        }

        ExecuteFollowUps(user, target);
    }


    // Slight refactor: better to compute once per target:
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

        float baseDamage = skillPower * casterOffense * 0.01f;
        
        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;

        int totalCritChance  = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage  = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        bool isCrit = Random.Range(0f, 100f) < totalCritChance;

        if (isCrit)
            //crit ignores 50% of defense mitigation
            defMitigation *= 0.5f;

        float afterDef = baseDamage * (1f - defMitigation);

        float critMultiplier = 1f;
        if (isCrit)
            critMultiplier = totalCritDamage * 0.01f;

        float final = afterDef * critMultiplier;
        int actual   = Mathf.Max(0, Mathf.RoundToInt(final));

        return actual;
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

        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;


        int totalCritChance = Mathf.Max(0, userStats.critChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, userStats.critDamage + skillCritDamage);

        float critMultiplier;

        if (totalCritChance >= 100)
        {
            critMultiplier = totalCritDamage * 0.01f;
            defMitigation *= 0.5f;
        }
        else
        {
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

