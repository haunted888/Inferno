using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Damage Skill")]
public class DamageSkill : DamageSkillParent
{

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
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
        user.Heal(Mathf.RoundToInt(dealt * damageSkillDetailShell.lifeStealPercent));
        user.AddThreat(dealt);
        target.ClearIncomingDamageModifiers();
        user.ClearOutgoingDamageModifiers();

        if(BattleTurnManager.Instance != null)
            BattleTurnManager.Instance.RegisterDamage(user, target, dealt, skillDetailShell.damageType, subType);


        
        AfterExecute(user, target);
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }



}
