using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Utility/Decision Value At Health Percent")]
public class DecisionValueAtHealthPercent : Skill
{
    [Range(1, 100)]public int healthPercentThreshold = 50; // The health percentage threshold to check against
    public int valueIfBelowThreshold = 100; 
    private bool hasExecuted = false;
   
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

        if (hasExecuted || healthPercent > healthPercentThreshold)
        {
            return -100;         
        }
        else
        {
            hasExecuted = true;
            return valueIfBelowThreshold; // Return the specified value if the health percentage is below the threshold
        }

        
    }
}
