using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Passive Steal")]
public class PassiveStealSkill : Skill
{

    public PassivesTypes stealType = PassivesTypes.StatModifier;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        foreach (var passive in target.passives)
        {
            if (passive.type == stealType)
            {
                user.AddPassive(passive);
                target.RemovePassive(passive);
            }
        }

        
        ExecuteFollowUps(user, target);
    }
}
