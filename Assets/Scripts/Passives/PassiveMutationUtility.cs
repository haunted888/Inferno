using System;
using System.Collections.Generic;
using UnityEngine;

public static class PassiveMutationUtility
{
    public sealed class PassiveMutationContext
    {
        public readonly List<PassivesDefinition> passivesToRemove = new List<PassivesDefinition>();
        public readonly List<PassivesDefinition> passivesToAdd = new List<PassivesDefinition>();

        public void Clear()
        {
            passivesToRemove.Clear();
            passivesToAdd.Clear();
        }
    }

    public static void InvokePassivesWithMutation(
        BattleCharacter owner,
        Func<List<PassivesDefinition>> getCurrentPassives,
        Action<PassivesDefinition> invokeHook,
        PassiveMutationContext context)
    {
        if (owner == null || invokeHook == null || context == null) return;

        var passivesList = getCurrentPassives?.Invoke();
        if (passivesList == null) return;

        do
        {
            context.passivesToAdd.Clear();

            foreach (var p in passivesList)
            {
                if (p == null) continue;
                if (context.passivesToRemove.Contains(p)) continue;

                invokeHook(p);
            }

            passivesList = new List<PassivesDefinition>(context.passivesToAdd);
            foreach (var p in passivesList)
            {
                if (p == null) continue;
                owner.AddPassive(p);
            }
        }
        while (passivesList.Count > 0);

        while (context.passivesToRemove.Count > 0)
        {
            owner.RemovePassive(context.passivesToRemove[0]);
            context.passivesToRemove.RemoveAt(0);
        }
    }
}