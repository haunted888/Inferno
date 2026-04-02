using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Passive AOE Steal")]
public class PassiveStealAOESkill : Skill
{

    public PassivesTypes stealType = PassivesTypes.StatModifier;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {   
        foreach (var character in target.GetAllies())
        {
            foreach (var passive in character.passives)
            {
                if (passive.type == stealType)
                {
                    user.AddPassive(passive);
                    character.RemovePassive(passive);
                }
            }
        }

        
        ExecuteFollowUps(user, target);
    }
}
