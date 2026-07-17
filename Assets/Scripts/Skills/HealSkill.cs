// HealAllySkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal Ally")]
public class HealAllySkill : Skill
{
    [Header("Heal Skill")]
    public int healAmount = 15;
    public bool asPercent = false;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (target == null || target.IsDead) return;

        
        BeforeSkillExecute(user, target);

        BeforeHealingSkillExecute(user, target);
        
        _ = asPercent ? Mathf.CeilToInt(target.MaxHealth * (healAmount / 100f)) : healAmount;

        int healing = target.ApplyIncomingHealingModifiers(healAmount);

        target.Heal(healing);

        user.AddThreat(healing);

        target.ClearIncomingHealingModifiers();

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}

