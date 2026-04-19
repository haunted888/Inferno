using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Freezing Passive")]
public class FreezingPassiveDefinition : StatusPassiveDefinition
{

    public float cureChance = 0.1f;
    private  bool willFreeze = true;
    
    
    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if(Random.value < cureChance)
        {
            self.QueuePassiveToRemove(this, PassiveHook.OnResolvePhaseEnd);
            SetDisplayText($"{self.name} has thawed out!");
            return;
        }
        if (willFreeze)
        {
            self.IsFrozen = true;
            SetDisplayText($"{self.name} is frozen solid!");
        }
        else
        {
            self.IsFrozen = false;
        }

        willFreeze = !willFreeze;
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        self.IsFrozen = false;
        ApplyStatusBuffer(self);
    }


}
