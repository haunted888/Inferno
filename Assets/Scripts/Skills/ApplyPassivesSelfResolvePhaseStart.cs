using System.Collections.Generic;
using UnityEngine;

//Used for skills that need to keep track of things before execution (like counter)
[CreateAssetMenu(menuName = "Skills/Apply Passives Self Resolve Phase Start")]
public class ApplyPassiveSelfResolvePhaseStart : Skill
{
    [Header("Passives To Apply")]
    public List<PassivesDefinition> passivesToApply = new List<PassivesDefinition>();

    public override void OnResolvePhaseStart(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;
        if (passivesToApply == null || passivesToApply.Count == 0) return;
        if (Random.value > skillDetailShell.bonusEffectChance && bonusEffectChance < 1) return;

        


        for (int i = 0; i < passivesToApply.Count; i++)
        {
            var passive = passivesToApply[i];
            if (passive == null) continue;
            user.AddPassive(passive, user);

        }


        
        
    }

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}