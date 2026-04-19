using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Passive Steal")]
public class PassiveStealSkill : Skill
{

    public PassivesTypes stealType = PassivesTypes.StatModifier;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {

        BeforeSkillExecute(user, target);

        List<PassivesDefinition> passivesToSteal = new List<PassivesDefinition>();
        foreach (var passive in target.passives)
        {
            if (passive.type == stealType)
            {
                passivesToSteal.Add(passive);
            }
        }

        foreach (var passive in passivesToSteal)
        {
            user.AddPassive(passive, target);
            target.RemovePassive(passive);
        }

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
