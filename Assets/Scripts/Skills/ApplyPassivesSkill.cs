using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Apply Passives")]
public class ApplyPassivesSkill : Skill
{
    [Header("Passives To Apply")]
    public List<PassivesDefinition> passivesToApply = new List<PassivesDefinition>();

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null || target.IsDead) return;
        if (passivesToApply == null || passivesToApply.Count == 0) return;
        if (Random.value > skillDetailShell.bonusEffectChance) return;

        
        BeforeSkillExecute(user, target);


        for (int i = 0; i < passivesToApply.Count; i++)
        {
            var passive = passivesToApply[i];
            if (passive == null) continue;
            target.AddPassive(passive);

        }


        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}