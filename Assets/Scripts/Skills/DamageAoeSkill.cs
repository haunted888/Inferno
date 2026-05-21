// DamageAllEnemiesSkill.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Damage All Enemies")]
public class DamageAllEnemiesSkill : DamageSkillParent
{
    [Header("AOE Skill")]
    public AffectsCharacters characters;


    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return 0;


        DamageSkillParent damageSkillDetailShell = skillDetailShell as DamageSkillParent;

        SkillDamageType damageType = skillDetailShell.damageType;
        DamageSubType subType = damageSkillDetailShell.subType;

        if(subType == DamageSubType.Adaptive && damageType != SkillDamageType.Adaptive)
        {
            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            subType = DamageSubType.None;
            List<DamageSubType> physicalSubTypes = new List<DamageSubType> { DamageSubType.Slashing, DamageSubType.Piercing, DamageSubType.Bludgeoning };
            List<DamageSubType> elementalSubTypes = new List<DamageSubType> { DamageSubType.Fire, DamageSubType.Ice, DamageSubType.Storm, DamageSubType.Acid, DamageSubType.Psychic, DamageSubType.Blood };

            
            List<DamageSubType> subTypesToCheck = (damageType == SkillDamageType.Physical) ? physicalSubTypes : elementalSubTypes;

            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value > highestCount && subTypesToCheck.Contains(kvp.Key))
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                }
            }
        }

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

        
        BeforeSkillExecute(user, target);
        
        BeforeDamageSkillExecute(user, target);

        
        var damageSkillDetailShell = skillDetailShell as DamageSkillParent;
        var power = damageSkillDetailShell.power;
        var damageVariance = damageSkillDetailShell.damageVariance;
        var skillCritChance = damageSkillDetailShell.skillCritChance;
        var skillCritDamage = damageSkillDetailShell.skillCritDamage;

        List<BattleCharacter> group;

        group = BattleUtility.GetTargetsForEffectsCharacters(characters, user, target, this);

        SkillDamageType damageType = skillDetailShell.damageType;
        DamageSubType subType = damageSkillDetailShell.subType;

        if(subType == DamageSubType.Adaptive && damageType != SkillDamageType.Adaptive)
        {
            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            subType = DamageSubType.None;
            List<DamageSubType> physicalSubTypes = new List<DamageSubType> { DamageSubType.Slashing, DamageSubType.Piercing, DamageSubType.Bludgeoning };
            List<DamageSubType> elementalSubTypes = new List<DamageSubType> { DamageSubType.Fire, DamageSubType.Ice, DamageSubType.Storm, DamageSubType.Acid, DamageSubType.Psychic, DamageSubType.Blood };

            
            List<DamageSubType> subTypesToCheck = (damageType == SkillDamageType.Physical) ? physicalSubTypes : elementalSubTypes;

            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value > highestCount && subTypesToCheck.Contains(kvp.Key))
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                }
            }
        }

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
            
            
            int powerRange = Random.Range(power - damageVariance, power + damageVariance);

            int damage = ComputeActualDamage(
                user.GetEffectiveStats(),
                member.GetEffectiveStats(),
                powerRange,
                damageType,
                skillCritChance,
                skillCritDamage,
                subType);

            damage = user.ApplyTraitDamageModifiers(this, member, damage);
            damage = member.ApplyIncomingDamageModifiers(damage);
            damage = user.ApplyOutgoingDamageModifiers(damage);

            int dealt = member.TakeDamage(damage, skillDetailShell.damageType, subType);
            user.Heal(Mathf.RoundToInt(dealt * damageSkillDetailShell.lifeStealPercent));
            user.AddThreat(dealt);
            member.ClearIncomingDamageModifiers();
            user.ClearOutgoingDamageModifiers();

            if(BattleTurnManager.Instance != null)
                BattleTurnManager.Instance.RegisterDamage(user, member, dealt, skillDetailShell.damageType, subType);
        }

        AfterExecute(user, target);
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }



}

