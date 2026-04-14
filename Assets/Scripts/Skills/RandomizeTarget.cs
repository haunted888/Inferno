using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Randomize Target")]
public class RandomizeTarget : Skill
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        List<BattleCharacter> validTargets = new List<BattleCharacter>();
        foreach (var t in target.GetAllies()) // Assuming you want to target allies of the original target; change to GetEnemies() if you want to target enemies instead
        {
            if (t != null && !t.IsDead)
                validTargets.Add(t);
        }

        if (validTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, validTargets.Count);
            BattleCharacter randomTarget = validTargets[randomIndex];
            target = randomTarget;
        }

        ExecuteFollowUps(user, target);
        
        EndExecution();

    }
}
