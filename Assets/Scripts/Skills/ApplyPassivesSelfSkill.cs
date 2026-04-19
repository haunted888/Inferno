using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Apply Passives Self")]
public class ApplyPassivesSelfSkill : Skill
{
    [Header("Passives To Apply")]
    public List<PassivesDefinition> passivesToApply = new List<PassivesDefinition>();

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;
        if (passivesToApply == null || passivesToApply.Count == 0) return;
        if (Random.value > skillDetailShell.bonusEffectChance) return;

        
        BeforeSkillExecute(user, user);


        for (int i = 0; i < passivesToApply.Count; i++)
        {
            var passive = passivesToApply[i];
            if (passive == null) continue;
            user.AddPassive(passive, user);

        }


        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}