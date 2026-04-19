using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Steamed Passive")]
public class SteamedPassiveDefinition : StatusPassiveDefinition
{

    public float steamedMultiplier = 0.8f;
    public float healingIncreasePercent = .5f;

    public override void OnCreated(BattleCharacter self)
    {
        self.IsSteamed = true;
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        self.IsSteamed = false;
        ApplyStatusBuffer(self);
    }

    public override void BeforeReceivingHealing(BattleCharacter self, int healingAmount)
    {
        self.ModifyIncomingHealingMultiplier(1+healingIncreasePercent);
    }


    public float GetSteamedMultiplier()
    {
        // Implement the logic to calculate and return the steamed multiplier
        return steamedMultiplier; // Placeholder value
    }
}
