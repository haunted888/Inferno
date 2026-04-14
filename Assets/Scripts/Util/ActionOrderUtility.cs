using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ActionOrderUtility
{
    public static List<QueuedAction> GetOrderedActions(List<QueuedAction> actions)
    {
        if (actions == null)
            return new List<QueuedAction>();

        var orderedActions = actions
            .Where(a => a != null && a.user != null && !a.user.IsDead)
            .OrderByDescending(a => a.user.GetSpeed())
            .ThenBy(_ => Random.value)
            .ToList();

        var actionsArray = orderedActions.ToArray();

        foreach (var action in actionsArray)
        {
            if (action == null || action.user == null || action.user.IsDead)
                continue;

            PassiveMutationUtility.InvokePassivesWithMutation(
                action.user,
                () => action.user.passives,
                p => p.OnActionOrdered(action, orderedActions),
                PassivesDefinition.PassiveHook.OnActionOrdered,
                action.user.passiveMutationContext
            );
        }

        foreach (var action in orderedActions)
        {
            if (action?.user == null) continue;
            action.user.currentActionOrder = orderedActions;
        }

        return orderedActions;
    }

    public static List<BattleCharacter> GetOrderedCharacters(List<QueuedAction> actions)
    {
        return GetOrderedActions(actions)
            .Where(a => a != null && a.user != null && !a.user.IsDead)
            .Select(a => a.user)
            .ToList();
    }
}
