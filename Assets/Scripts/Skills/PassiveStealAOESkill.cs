using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Passive AOE Steal")]
public class PassiveStealAOESkill : Skill
{

    public PassivesTypes stealType = PassivesTypes.StatModifier;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {   

        BeforeSkillExecute(user, target);

        foreach (var character in target.GetAllies())
        {
            List<PassivesDefinition> passivesToSteal = new List<PassivesDefinition>();
            foreach (var passive in character.passives)
            {
                if (passive.type == stealType)
                {
                    passivesToSteal.Add(passive);
                }
            }

            foreach (var passive in passivesToSteal)
            {
                user.AddPassive(passive);
                character.RemovePassive(passive);
            }
        }

        
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
