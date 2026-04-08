using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Dazed")]
public class DazedPassiveDefinition : PassivesDefinition
{
    public override void OnCreated(BattleCharacter self)
    {
        bool removeThisDazed = false;
        foreach(var passive in self.passives)
        {
            if(passive is DazedPassiveDefinition && passive != this)
            {
                removeThisDazed = true;
            }
        }
        if (removeThisDazed)
        {
            self.RemovePassive(this);
        }
        self.IsDazed = true;
    }


    public override void OnDestroyed(BattleCharacter self)
    {
        
        self.IsDazed = false;
    }

    
}

