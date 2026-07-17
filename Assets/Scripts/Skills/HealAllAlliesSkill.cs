// HealAllAlliesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal All Allies")]
public class HealAllAlliesSkill : Skill
{
    [Header("Heal ALL Skill")]
    public int healAmount = 10;
    public bool asPercent = false;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;

        var group = user.GetAllies();

        foreach (var ally in group)
        {
            if (ally == null || ally.IsDead) continue;

            BeforeSkillExecute(user, ally);
            BeforeHealingSkillExecute(user, ally);
            
            
            _ = asPercent ? Mathf.CeilToInt(target.MaxHealth * (healAmount / 100f)) : healAmount;

            int healing = ally.ApplyIncomingHealingModifiers(healAmount);

            user.AddThreat(healing);

            ally.Heal(healing);

            ally.ClearIncomingHealingModifiers();
            EndExecution();
        }

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }

}
