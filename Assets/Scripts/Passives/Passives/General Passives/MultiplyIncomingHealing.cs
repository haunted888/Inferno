using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Multiply Incoming Healing")]
public class MultiplyIncomingHealing : PassivesDefinition
{
    public float healingMultiplier = 2f;

    public override void BeforeReceivingHealing(BattleCharacter self, int healingAmount)
    {
        self.ModifyIncomingHealingMultiplier(healingMultiplier);
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        base.OnResolvePhaseEnd(self);
    }

    public override string GetDescription(BattleCharacter character)
    {
        return $"Increases incoming healing by {healingMultiplier * 100}%.";
    }
}
