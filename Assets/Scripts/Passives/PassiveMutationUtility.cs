using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PassiveMutationUtility
{
    public sealed class PassiveMutationContext
    {
        public readonly Dictionary<PassivesDefinition, PassivesDefinition.PassiveHook> passivesToRemove = new Dictionary<PassivesDefinition, PassivesDefinition.PassiveHook>();
        public readonly Dictionary<PassivesDefinition, PassivesDefinition.PassiveHook> passivesToAdd = new Dictionary<PassivesDefinition, PassivesDefinition.PassiveHook>();

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
        PassivesDefinition.PassiveHook hook,
        PassiveMutationContext context)
    {
        

        if (owner == null || invokeHook == null || context == null) return;

        var passivesList = getCurrentPassives?.Invoke();
        if (passivesList == null) return;

        do
        {

            foreach (var p in passivesList)
            {
                if (p == null) continue;
                if (context.passivesToRemove.ContainsKey(p)) continue;

                invokeHook(p);
            }

            passivesList = new List<PassivesDefinition>();
            foreach (var kvp in context.passivesToAdd)
            {
                if (kvp.Key == null || kvp.Value != hook) continue;
                passivesList.Add(kvp.Key);
            }
            foreach (var p in passivesList)
            {
                if (p == null) continue;
                owner.AddPassiveNoInstance(p);
                context.passivesToAdd.Remove(p);
            }
        }
        while (passivesList.Count > 0);

        var passivesToRemoveList = new List<PassivesDefinition>();
        foreach (var kvp in context.passivesToRemove)
        {
            if (kvp.Key == null || kvp.Value != hook) continue;
            owner.RemovePassive(kvp.Key);
            passivesToRemoveList.Add(kvp.Key);
        }

        while(passivesToRemoveList.Count > 0)
        {
            context.passivesToRemove.Remove(passivesToRemoveList[0]);
            passivesToRemoveList.RemoveAt(0);
        }
    }

    public static IEnumerator InvokePassivesWithMutationCoroutine(
        BattleCharacter owner,
        Func<List<PassivesDefinition>> getCurrentPassives,
        Action<PassivesDefinition> invokeHook,
        PassivesDefinition.PassiveHook hook,
        PassiveMutationContext context)
    {
        if (owner == null || invokeHook == null || context == null) yield break;

        var passivesList = getCurrentPassives?.Invoke();
        if (passivesList == null) yield break;

        do
        {

            foreach (var p in passivesList)
            {
                if (p == null) continue;
                if (context.passivesToRemove.ContainsKey(p)) continue;

                invokeHook(p);
                if(!string.IsNullOrEmpty(p.GetDisplayText()))
                {
                    if(BattleTurnManager.Instance != null)
                    {
                        BattleTurnManager.Instance.SetBattleText(p.GetDisplayText());
                        Debug.Log("Waiting");
                        yield return new WaitForSeconds(1f);
                        Debug.Log("Done waiting");
                    }
                    
                    p.SetDisplayText("");
                }
            }

            passivesList = new List<PassivesDefinition>();
            foreach (var kvp in context.passivesToAdd)
            {
                if (kvp.Key == null || kvp.Value != hook) continue;
                passivesList.Add(kvp.Key);
            }
            foreach (var p in passivesList)
            {
                if (p == null) continue;
                owner.AddPassiveNoInstance(p);
                context.passivesToAdd.Remove(p);
            }

        }
        while (passivesList.Count > 0);

        var passivesToRemoveList = new List<PassivesDefinition>();
        foreach (var kvp in context.passivesToRemove)
        {
            Debug.Log($"Checking passive {kvp.Value == hook}, value: {kvp.Value}, invokeHook: {hook}");
            if (kvp.Key == null || kvp.Value != hook) continue;
            owner.RemovePassive(kvp.Key);
            passivesToRemoveList.Add(kvp.Key);
        }

        while(passivesToRemoveList.Count > 0)
        {
            context.passivesToRemove.Remove(passivesToRemoveList[0]);
            passivesToRemoveList.RemoveAt(0);
        }
        
    }
}
