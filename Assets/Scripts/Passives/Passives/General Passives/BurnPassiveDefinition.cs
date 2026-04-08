using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Burn")]
public class BurnPassiveDefinition : PassivesDefinition
{

    private const float burnDamagePercent = .0625f;
    public int counter = 1;

    public override void OnCreated(BattleCharacter self)
    {
        bool removeThisBurn = false;
        foreach(var passive in self.passives)
        {
            if(passive is BurnPassiveDefinition && passive != this)
            {
                ((BurnPassiveDefinition)passive).counter++;
                removeThisBurn = true;
            }
        }
        if (removeThisBurn)
        {
            self.RemovePassive(this);
            Debug.Log($"{self.name} already has burn, increasing counter to {counter} and removing duplicate.");
        }
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        int burnDamage = Mathf.CeilToInt(self.MaxHealth * burnDamagePercent);
        
        self.TakeDamage(burnDamage);
        SetDisplayText($"{self.name} is hurt by their burn!");

        counter--;
        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
        }
    }
    
}
