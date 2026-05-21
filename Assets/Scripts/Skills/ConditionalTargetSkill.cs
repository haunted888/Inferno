using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Check Target Requirements")]
public class ConditionalTargetSkill : Skill
{
    public List<SkillTargetType> targetConditions;
    
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null) return;


        bool shouldExecute = false;

        foreach (var condition in targetConditions)
        {
            switch (condition)
            {
                case SkillTargetType.AllAllies:
                case SkillTargetType.SingleAlly:
                    List<BattleCharacter> allies = new List<BattleCharacter>(user.GetAllies());
                    if (allies.Contains(target))
                    {
                        shouldExecute = true;
                    }
                    break;
                case SkillTargetType.AllEnemies:
                case SkillTargetType.SingleEnemy:
                    List<BattleCharacter> enemies = new List<BattleCharacter>(user.GetEnemies());
                    if (enemies.Contains(target))
                    {
                        shouldExecute = true;
                    }
                    break;
                case SkillTargetType.Self:
                    if (user == target)
                    {
                        shouldExecute = true;
                    }
                    break;
                case SkillTargetType.SingleTargetNoSelf:
                    if (user != target)
                    {
                        shouldExecute = true;
                    }
                    break;
                default:
                    shouldExecute = true; // If no specific conditions, allow execution
                    break;
            }

        }

        if (!shouldExecute)
        {
            EndExecution();
            return; // Not enough resources, so the skill fails
        }
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }

}
