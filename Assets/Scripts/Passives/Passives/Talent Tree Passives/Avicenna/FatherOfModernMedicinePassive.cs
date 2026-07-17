using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Ibn Sina/Father of Modern Medicine")]
public class FatherOfModernMedicinePassive : PassivesDefinition
{

    public MultiplyIncomingHealing healingBoostPassive;

    public override void OnBattleStart(BattleCharacter self)
    {
        if (healingBoostPassive == null) return;
        
        self.QueuePassiveToAdd(healingBoostPassive, PassiveHook.OnBattleStart);

        foreach (var ally in self.GetAllies())
        {
            if (ally == null || ally == self) continue;
            ally.AddPassive(healingBoostPassive, self);
        }
    }
}
