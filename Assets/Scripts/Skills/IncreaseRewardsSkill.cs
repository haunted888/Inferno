using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Increase Rewards")]
public class IncreaseRewardsSkill : Skill
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    MapCombatTransfer transfer = MapCombatTransfer.Instance;

    public MapRewardDefinition[] rewardsToAdd;
    public int maxRewardsToAdd = int.MaxValue;
    
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if(maxRewardsToAdd <= 0) return;
        foreach (var reward in rewardsToAdd)
        {
            transfer.AddToPendingRewards(reward);
        }
        maxRewardsToAdd--;

        
        ExecuteFollowUps(user, target);
    }
}
