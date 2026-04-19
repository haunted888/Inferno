using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Chemical Burn Passive")]
public class ChemicalBurnPassiveDefinition : StatusPassiveDefinition
{
    public float burnDamagePercent = .01f;
    
    public float healCutPercent = 1.0f;
    public int counter = 1;
    public float doubleCounterOdds = 0.5f;


    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        int burnDamage = Mathf.CeilToInt(self.MaxHealth * burnDamagePercent * counter);
        SetDisplayText($"{self.name} takes {burnDamage} chemical burn damage!");
        
        self.TakeDamage(burnDamage);
        
        if(Random.value < doubleCounterOdds)
        {
            counter *= 2;
            SetDisplayText($"{self.name} takes {burnDamage} chemical burn damage! and {self.name}'s chemical burn counter has doubled to {counter}!");

        }
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
