using UnityEngine;


public enum SkillDamageType
{
    Physical,
    Elemental
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

public enum SkillTargetType
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies
}

public abstract class Skill : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;

    public SkillTargetType targetType;

    [Header("Cost")]
    public int spCost = 0;

    [Header("Additional Effects")]
    public Skill[] followUpSkills;   // skills to trigger after this one

    public virtual int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        // Default: non-damaging skills
        return 0;
    }

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
}
