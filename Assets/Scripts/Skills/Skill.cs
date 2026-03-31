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

    [Header("Additional Effects")]
    public Skill[] followUpSkills;   // skills to trigger after this one

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

    
    //NOTE: When you add animations, add them directly to the skill and have them execute in this function.
    public abstract void Execute(BattleCharacter user, BattleCharacter target);

    protected void ExecuteFollowUps(BattleCharacter user, BattleCharacter target)
    {
        if (followUpSkills == null) return;

        for (int i = 0; i < followUpSkills.Length; i++)
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
}
