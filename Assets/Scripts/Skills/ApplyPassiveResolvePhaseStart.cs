using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Apply Passives Resolve Phase Start")]
public class ApplyPassiveResolvePhaseStart : Skill
{
    [Header("Passives To Apply")]
    public List<PassivesDefinition> passivesToApply = new List<PassivesDefinition>();

    public override void OnResolvePhaseStart(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null || target.IsDead) return;
        if (passivesToApply == null || passivesToApply.Count == 0) return;
        if (Random.value > skillDetailShell.bonusEffectChance) return;

        


        for (int i = 0; i < passivesToApply.Count; i++)
        {
            var passive = passivesToApply[i];
            if (passive == null) continue;
            target.AddPassive(passive, user);

        }


        
        
    }

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}