using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Blank Skill")]
public class BlankSkill : Skill
{
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
