using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Decision Value Below Health Percent")]
public class DecisionValueBelowHealthPercent : Skill
{
    [Range(1, 100)]public int healthPercentThreshold = 50; // The health percentage threshold to check against
    public int valueIfBelowThreshold = 0; 
    public int valueIfAboveThreshold = -1;
   
   // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {

        ExecuteFollowUps(user, target);
        
        EndExecution();

    }

    public override int GetSelectionValue(BattleCharacter user, BattleCharacter target)
    {
        if (target == null || target.IsDead) return 0;

        float healthPercent = (float)user.CurrentHealth / target.MaxHealth * 100f;

        if (healthPercent > healthPercentThreshold)
        {
            return valueIfAboveThreshold; // Return the specified value if the health percentage is above the threshold
        }
        else
        {
            return valueIfBelowThreshold; // Return the specified value if the health percentage is below the threshold
        }

        
    }
}
