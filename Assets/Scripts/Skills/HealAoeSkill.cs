using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal AOE")]
public class HealAoeSkill : Skill
{
    [Header("Heal AOE Skill")]
    public int healAmount = 10;
    public bool asPercent = false;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;

        var group = GetHealingTargets(user, target);

        foreach (var character in group)
        {
            if (character == null || character.IsDead) continue;

            BeforeSkillExecute(user, character);
            BeforeHealingSkillExecute(user, character);

            _ = asPercent ? Mathf.CeilToInt(character.MaxHealth * (healAmount / 100f)) : healAmount;

            int healing = character.ApplyIncomingHealingModifiers(healAmount);

            if (user.GetAllies().AsReadOnlyList().Contains(character))
                user.AddThreat(healing);
            else if(user.GetEnemies().AsReadOnlyList().Contains(character))
                user.AddThreat(-healing);
            character.Heal(healing);
            character.ClearIncomingHealingModifiers();

            EndExecution();
        }

        ExecuteFollowUps(user, target);

        EndExecution();
    }

    private List<BattleCharacter> GetHealingTargets(BattleCharacter user, BattleCharacter target)
    {
        IEnumerable<BattleCharacter> targets;

        switch (targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                targets = user.GetEnemies();
                break;
            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
            case SkillTargetType.Self:
                targets = user.GetAllies();
                break;
            case SkillTargetType.SingleTarget:
            case SkillTargetType.SingleTargetNoSelf:
                targets = target != null ? target.GetAllies() : user.GetAllies();
                break;
            default:
                targets = user.GetAllies();
                break;
        }

        var candidates = new List<BattleCharacter>();
        foreach (var candidate in targets)
        {
            candidates.Add(candidate);
        }

        return candidates;
    }
}
