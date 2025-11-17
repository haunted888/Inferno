using UnityEngine;

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


    public SkillTargetType targetType; // AI + targeting metadata

    public virtual int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        // Default: non-damaging skills
        return 0;
    }

    // Basic skill execution
    public abstract void Execute(BattleCharacter user, BattleCharacter target);
}