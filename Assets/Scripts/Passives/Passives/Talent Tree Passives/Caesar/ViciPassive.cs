using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Caesar/Vici")]
public class ViciPassive : PassivesDefinition
{
    public override void OnBattleEnd(BattleCharacter self, bool playerWon)
    {
        if(self == null || self.IsDead) return;
        foreach (var ally in self.GetAllies())
        {
            if (ally == null || ally.IsDead) continue;
            foreach (var t in ally.Traits)
            {
                if (t == null) continue;
                if (t is ExpeditionistTraitDefinition)
                {
                    var expeditionistTrait = t as ExpeditionistTraitDefinition;
                    expeditionistTrait.AddReward();
                    Debug.Log($"ViciPassive triggered for {ally.name}.");
                }
            }
        }
    }
}
