using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Sovereign Passive")]
public class SovereignPassiveDefinition : PassivesDefinition
{
    public StatBoostFlatPassiveDefinition sovereignStatBoost;
    public float statBoostPercentage = 0.2f;

    public override void OnCommandPhaseStart(BattleCharacter self)
    {
        foreach (BattleCharacter ally in self.GetAllies())
        {
            if (ally != self && ally.HasTrait(CharacterTrait.Sovereign) && !ally.IsDead)
            {
                return;
            }
        }

        DamageSubType highestAttackType = DamageSubType.Slashing;
        DamageSubType highestDefenseType = DamageSubType.Slashing;

        var subAttackStats = self.GetSubAttackStats();
        var subDefenseStats = self.GetSubDefenseStats();

        foreach (var kvp in subAttackStats)
        {
            if(kvp.Value > subAttackStats[highestAttackType])
            {
                highestAttackType = kvp.Key;
            }
        }

        foreach (var kvp in subDefenseStats)
        {
            if(kvp.Value > subDefenseStats[highestDefenseType])
            {
                highestDefenseType = kvp.Key;
            }
        }

        CombatStats statBoosts = new CombatStats();
        statBoosts.SetSubAttackBoost(highestAttackType, (int)(subAttackStats[highestAttackType] * statBoostPercentage));
        Debug.Log($"Sovereign Passive: Highest Attack Type: {highestAttackType} with value {subAttackStats[highestAttackType]}. Applying boost of {(int)(subAttackStats[highestAttackType] * statBoostPercentage)}.");
        statBoosts.SetSubDefenseBoost(highestDefenseType, (int)(subDefenseStats[highestDefenseType] * statBoostPercentage));
        Debug.Log($"Sovereign Passive: Highest Defense Type: {highestDefenseType} with value {subDefenseStats[highestDefenseType]}. Applying boost of {(int)(subDefenseStats[highestDefenseType] * statBoostPercentage)}.");

        foreach (BattleCharacter ally in self.GetAllies())
        {
            if (!ally.IsDead && ally != self)
            {
                sovereignStatBoost.SetStatBoosts(statBoosts);
                ally.AddPassive(sovereignStatBoost);
            }
        }
    }
}
