using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/Passive AOE Steal")]
public class PassiveStealAOESkill : Skill
{

    public PassivesTypes stealType = PassivesTypes.StatModifier;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {   
        if (user == null || target == null) return;

        foreach (var character in target.GetAllies())
        {
            if (character == null || character.IsDead) continue;

            BeforeSkillExecute(user, character);

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
                user.AddPassive(passive, character);
                character.RemovePassive(passive);
            }
            EndExecution();
        }

        
        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
