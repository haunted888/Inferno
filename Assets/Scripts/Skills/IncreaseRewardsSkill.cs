using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Increase Rewards")]
public class IncreaseRewardsSkill : Skill
{

    public MapRewardDefinition[] rewardsToAdd;
    public int maxRewardsToAdd = int.MaxValue;
    private int rewardsAdded = 0;

    public override void OnCreated(BattleCharacter self)
    {
        rewardsAdded = 0; // Reset counter when skill is created
        base.OnCreated(self);
    }
    
    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if(rewardsAdded >= maxRewardsToAdd) return;
        if(BattleTurnManager.Instance == null) return;

        BeforeSkillExecute(user, target);

        foreach (var reward in rewardsToAdd)
        {
            MapCombatTransfer.Instance.AddToPendingRewards(reward);
        }
        rewardsAdded++;

        
        
        ExecuteFollowUps(user, target);
        
        EndExecution();
    }
}
