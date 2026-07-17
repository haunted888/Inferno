using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Sleep")]
public class SleepPassiveDefinition : StatusPassiveDefinition
{
    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;

        bool removeThisSleep = false;
        foreach(var passive in self.passives)
        {
            if(passive is SleepPassiveDefinition && passive != this)
            {
                removeThisSleep = true;
            }
        }
        if (removeThisSleep)
        {
            self.RemovePassive(this);
        }
        self.IsAsleep = true;
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        if (counter <= 1)
        {
            counter = 0;
            SetDisplayText($"{self.name} wakes up!");
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
            return;
        }

        base.OnResolvePhaseEnd(self);
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        
        self.IsAsleep = false;
        ApplyStatusBuffer(self);
    }

    
}
