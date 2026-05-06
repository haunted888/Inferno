using UnityEngine;

public abstract class DamageSkillParent : Skill
{
    
    
    [Header("Damage Skill")]
    public int power = 10;
    public DamageSubType subType = DamageSubType.None;
    public int skillCritChance = 0;
    public int skillCritDamage = 0;
    public int damageVariance = 0;
    public readonly float defenseScale = 1000f; // Higher means defense is less impactful. x means 50% mitigation at x def and 200% damage at -x def.

    [Header("Ignore Defenses")]
    public float mainDefenseCalculated = 1f;
    public float subDefenseCalculated = 1f;

    private DamageSubType permanentType;

    public void Awake()
    {
        permanentType = subType;
    }


    public void AfterExecute(BattleCharacter user, BattleCharacter target)
    {
        subType = permanentType;
    }

    public void ConvertDamageType(DamageSubType newType)
    {
        subType = newType;
    }

    

}
