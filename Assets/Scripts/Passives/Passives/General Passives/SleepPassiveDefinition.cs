using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Sleep")]
public class SleepPassiveDefinition : PassivesDefinition
{
    public int counter = 1;

    public override void OnCreated(BattleCharacter self)
    {
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
        counter--;
        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this);
        }
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        self.IsAsleep = false;
    }

    
}
