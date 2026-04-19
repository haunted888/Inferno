using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Boxer/Stance Swap")]
public class PassiveSwap : Skill
{
    public PassivesDefinition offensivePassive;
    public PassivesDefinition defensivePassive;

    public PassivesDefinition offensiveFollowUp;
    public PassivesDefinition defensiveFollowUp;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;
        
        BeforeSkillExecute(user, target);
        // Swap passives
        if (offensivePassive != null && defensivePassive != null)
        {
            bool hasOffensivePassive = user.passives.Contains(offensivePassive);
            bool hasDefensivePassive = user.passives.Contains(defensivePassive);

            if (hasOffensivePassive && !hasDefensivePassive)
            {
                user.RemovePassive(offensivePassive);
                user.AddPassive(defensivePassive, user);
                user.AddPassive(defensiveFollowUp, user);
            }
            else if (!hasOffensivePassive && hasDefensivePassive)
            {
                user.RemovePassive(defensivePassive);
                user.AddPassive(offensivePassive, user);
                user.AddPassive(offensiveFollowUp, user);
            }
        }
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
