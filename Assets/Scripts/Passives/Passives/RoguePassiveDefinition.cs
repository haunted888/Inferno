using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/RoguePassiveDefinition")]
public class RoguePassiveDefinition : PassivesDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnActionOrdered(QueuedAction action, List<QueuedAction> actions)
    {
        if (action.skill == null) return;
        if (action.skill.targetType == SkillTargetType.AllEnemies 
            || action.skill.targetType == SkillTargetType.AllAllies)
        {
            return;
        }
        if (action.user == action.target) return;

        foreach (var a in actions)
        {
            if (a.user == action.target)
            {
                
                actions.Remove(action);
                actions.Insert(actions.IndexOf(a), action);
                break;
            }
        }
    }
}
