using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Burn")]
public class BurnPassiveDefinition : StatusPassiveDefinition
{

    private const float burnDamagePercent = .02f;
    
    public float healCutPercent = .5f;

    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;

        bool removeThisBurn = false;
        foreach(var passive in self.passives)
        {
            if(passive is BurnPassiveDefinition definition && passive != this)
            {
                definition.counter += counter;
                removeThisBurn = true;
            }
        }
        if (removeThisBurn)
        {
            self.RemovePassive(this);
            Debug.Log($"{self.name} already has burn, increasing counter by {counter} and removing duplicate.");
        }
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        int burnDamage = Mathf.CeilToInt(self.MaxHealth * burnDamagePercent);
        
        self.TakeDamage(burnDamage, SkillDamageType.Elemental, DamageSubType.Fire);
        SetDisplayText($"{self.name} is hurt by their burn!");

        base.OnResolvePhaseEnd(self);
    }

    
    public override void BeforeReceivingHealing(BattleCharacter self, int healingAmount)
    {
        self.ModifyIncomingHealingMultiplier(1-healCutPercent);
    }
    
    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }

}
