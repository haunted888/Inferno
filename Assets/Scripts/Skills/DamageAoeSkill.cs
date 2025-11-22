// DamageAllEnemiesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Damage All Enemies")]
public class DamageAllEnemiesSkill : Skill
{
    public int power = 10;
    public SkillDamageType damageType = SkillDamageType.Physical;
    public DamageSubType subType = DamageSubType.None;

    public int skillCritChance = 0;
    public int skillCritDamage = 0;

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return 0;
        return EstimateExpectedDamageInternal(user, target, power, damageType, skillCritChance, skillCritDamage, subType);
    }

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (BattleTurnManager.Instance == null || user == null) return;

        // Use the target's team (fallback to user if target is null)
        var center = target != null ? target : user;
        var group = BattleTurnManager.Instance.GetAlliesOf(center);
        foreach (var member in group)
        {
            if (member == null || member.IsDead) continue;

            int damage = ComputeActualDamage(user, member, power, damageType, skillCritChance, skillCritDamage, subType);
            int dealt  = member.TakeDamage(damage);
            BattleTurnManager.Instance.RegisterDamage(user, member, dealt);
        }

        ExecuteFollowUps(user, target);
    }


    // Slight refactor: better to compute once per target:
    protected int ComputeActualDamage(
    BattleCharacter user,
    BattleCharacter target,
    int skillPower,
    SkillDamageType type,
    int skillCritChance,
    int skillCritDamage,
    DamageSubType subType)
    {
        // Base offense/defense (physical or elemental)
        int baseOff = (type == SkillDamageType.Physical)
            ? user.PhysicalAttack
            : user.ElementalPower;

        int baseDef = (type == SkillDamageType.Physical)
            ? target.Defense
            : target.ElementalResistance;

        // Sub-type bonuses
        int subOff = user.GetSubAttack(subType);
        int subDef = target.GetSubDefense(subType);

        int casterOffense = baseOff + subOff;
        int targetDef     = baseDef + subDef;

        float baseDamage = skillPower * casterOffense * 0.01f;
        
        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;

        int totalCritChance  = Mathf.Max(0, user.CritChance + skillCritChance);
        int totalCritDamage  = Mathf.Max(0, user.CritDamage + skillCritDamage);

        bool isCrit = Random.Range(0f, 100f) < totalCritChance;

        if (isCrit)
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
    BattleCharacter user,
    BattleCharacter target,
    int skillPower,
    SkillDamageType type,
    int skillCritChance,
    int skillCritDamage,
    DamageSubType subType)
    {
        // Base offense/defense (physical or elemental)
        int baseOff = (type == SkillDamageType.Physical)
            ? user.PhysicalAttack
            : user.ElementalPower;

        int baseDef = (type == SkillDamageType.Physical)
            ? target.Defense
            : target.ElementalResistance;

        // Sub-type bonuses
        int subOff = user.GetSubAttack(subType);
        int subDef = target.GetSubDefense(subType);

        int casterOffense = baseOff + subOff;
        int targetDef     = baseDef + subDef;
        float baseDamage = skillPower * casterOffense * 0.01f;

        float defMitigation = (targetDef > 0)
            ? (targetDef / (100f + targetDef))
            : 0f;

        float afterDef = baseDamage * (1f - defMitigation);

        int totalCritChance = Mathf.Max(0, user.CritChance + skillCritChance);
        int totalCritDamage = Mathf.Max(0, user.CritDamage + skillCritDamage);

        float critMultiplier;

        if (totalCritChance >= 100)
        {
            critMultiplier = totalCritDamage * 0.01f;
        }
        else
        {
            critMultiplier = 1f;
        }

        float expected = afterDef * critMultiplier;
        return Mathf.Max(0, Mathf.RoundToInt(expected));
    }

}

