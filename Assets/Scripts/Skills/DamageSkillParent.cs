using UnityEngine;

public abstract class DamageSkillParent : Skill
{
    
    
    [Header("Damage Skill")]
    public int power = 10;
    public DamageSubType subType = DamageSubType.None;
    public int skillCritChance = 0;
    public int skillCritDamage = 0;
    public int damageVariance = 0;

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
