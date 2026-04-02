using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Caesar/Vici")]
public class ViciPassive : PassivesDefinition
{
    public override void OnBattleStart(BattleCharacter self)
    {
        foreach (var ally in self.GetAllies())
        {
            if (ally == null || ally.IsDead) continue;
            foreach (var t in ally.Traits)
            {
                if (t == null) continue;
                if (t is ExpeditionistTraitDefinition)
                {
                    var expeditionistTrait = t as ExpeditionistTraitDefinition;
                    expeditionistTrait.addReward();
                    Debug.Log($"ViciPassive triggered for {ally.name}.");
                }
            }
        }
    }
}
