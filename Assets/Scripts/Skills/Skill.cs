using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public enum SkillDamageType
{
    Physical,
    Elemental,
    None,
    Adaptive
}

public enum DamageSubType
{
    None = 0,

    // Physical
    Bludgeoning,
    Slashing,
    Piercing,

    // Elemental
    Fire,
    Ice,
    Storm,
    Acid,
    Psychic,
    Blood
}

// Used externally
public enum SkillTargetType
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Self
}

public enum SkillEffectType
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Utility,
    Misc
}

public abstract class Skill : ScriptableObject
{
    // Used internally for skill logic
    public enum affectsCharacters
    {
        Target,
        TargetTeam,
        Self,
        Allies,
        Enemies,
        AllOtherAllies
    }

    [NonSerialized]
    public Dictionary<DamageSubType, SkillDamageType> subTypeToDamageType = new Dictionary<DamageSubType, SkillDamageType>()
    {
        { DamageSubType.Bludgeoning, SkillDamageType.Physical },
        { DamageSubType.Slashing, SkillDamageType.Physical },
        { DamageSubType.Piercing, SkillDamageType.Physical },
        { DamageSubType.Fire, SkillDamageType.Elemental },
        { DamageSubType.Ice, SkillDamageType.Elemental },
        { DamageSubType.Storm, SkillDamageType.Elemental },
        { DamageSubType.Acid, SkillDamageType.Elemental },
        { DamageSubType.Psychic, SkillDamageType.Elemental },
        { DamageSubType.Blood, SkillDamageType.Elemental }
    };

    public string skillName;
    [TextArea] public string description;
    public SkillTargetType targetType;
    public SkillDamageType damageType = SkillDamageType.Physical;

    [Header("Cost")]
    public int spCost = 0;
    [Range(0f, 1f)] public float hpCost = 0f;
    public int delay = 0; // Turns until skill executes after being chosen

    [Header("Additional Effects")]
    public List<Skill> followUpSkills;   // skills to trigger after this one

    [Header("Trait Requirements")]
    public List<CharacterTrait> traitTags = new List<CharacterTrait>();
    // If empty, skill is learnable by anyone.

    [Header("Trait Specifics")]
    [Header("Marksman")]
    [Range(0f, 1f)]
    public float ammoCost = 0f;

    public virtual int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        // Default: non-damaging skills
        return 0;
    }

    public void BeforeDamageSkillExecute(BattleCharacter user, BattleCharacter target)
    {

        for (int i = 0; i < user.passives.Count; i++)
        {
            var p = user.passives[i];
            if (p == null) continue;
            p.BeforeDamageSkillExecute(user, target, this);
        }
    }

    public void BeforeHealingSkillExecute(BattleCharacter user, BattleCharacter target)
    {

        for (int i = 0; i < user.passives.Count; i++)
        {
            var p = user.passives[i];
            if (p == null) continue;
            p.BeforeHealingSkillExecute(user, target, this);
        }
    }
    
    //NOTE: When you add animations, add them directly to the skill and have them execute in this function.
    public abstract void Execute(BattleCharacter user, BattleCharacter target);

    protected void ExecuteFollowUps(BattleCharacter user, BattleCharacter target)
    {
        if (followUpSkills == null) return;

        for (int i = 0; i < followUpSkills.Count; i++)
        {
            var s = followUpSkills[i];
            if (s == null) continue;
            s.Execute(user, target);
        }
    }

    public bool CanBeLearnedBy(MapPartyMemberDefinition member)
    {
        if (member == null) return false;

        // No trait tags → universally learnable
        if (traitTags == null || traitTags.Count == 0)
            return true;

        if (member.traits == null || member.traits.Count == 0)
            return false;

        // At least one overlapping trait
        for (int i = 0; i < traitTags.Count; i++)
        {
            if (member.traitTypes.Contains(traitTags[i]))
                return true;
        }
        return false;
    }

    public void UseNewSkill(BattleCharacter user, BattleCharacter target, Skill skillToUse)
    {

        if (user.IsAsleep || user.IsDead || target.IsDead) return;
        if (user.passives != null)
        {
            PassiveMutationUtility.InvokePassivesWithMutation(
                user,
                () => user.passives,
                p => p.OnSkillUsed(user, target, skillToUse),
                user.passiveMutationContext
            );
        }

        List<BattleCharacter> targets = BattleUtility.GetTargetsForSkill(skillToUse, user, target);
        foreach (var t in targets)
        {
            PassiveMutationUtility.InvokePassivesWithMutation(
                t,
                () => t.passives,
                p => p.OnSkillReceived(t, user, skillToUse),
                t.passiveMutationContext
            );

        }

        skillToUse.Execute(user, target);

        // Apply passive effects
        if (user.passives != null)
        {
            PassiveMutationUtility.InvokePassivesWithMutation(
                user,
                () => user.passives,
                p => p.OnSkillUsedEnd(user, target, skillToUse),
                user.passiveMutationContext
            );
        }

        foreach (var t in targets)
        {
            PassiveMutationUtility.InvokePassivesWithMutation(
                t,
                () => t.passives,
                p => p.OnSkillReceivedEnd(t, user, skillToUse),
                t.passiveMutationContext
            );
        }
    }

    public List<SkillEffectType> GetAllEffectTypes()
    {
        List<SkillEffectType> effectTypes = new List<SkillEffectType>();
        List<Skill> allSkills = new List<Skill> { this };
        while (allSkills.Count > 0) // Arbitrary limit to prevent infinite loops
        {
            allSkills[1].followUpSkills?.ForEach(s => allSkills.Add(s));
            var current = allSkills[0];
            if(current is DamageSkillParent) effectTypes.Add(SkillEffectType.Damage);
            else effectTypes.Add(SkillEffectType.Misc); // Placeholder for non-damage skills until we have more types
            allSkills.RemoveAt(0);
        }
        return effectTypes;
    }
}
