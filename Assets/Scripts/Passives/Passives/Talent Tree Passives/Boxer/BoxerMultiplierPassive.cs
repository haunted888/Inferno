using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Boxer/Boxer Style Multiplier")]
public class BoxerStyleMultiplierPassive : PassivesDefinition
{
    public float multiplier = 1.0f; // Multiplier for damage, default to 1 for no change
    private bool addBoost = true;


    public override void GetStatBoosts(BattleCharacter self)
    {
        if(!addBoost)
        {
            addBoost = true;
            return;
        }

        float attackBoostPercent = 0.00f;
        float elementalPowerBoostPercent = 0.00f;
        float defenseBoostPercent = 0.00f;
        float elementalResistanceBoostPercent = 0.00f;

        foreach (var p in self.passives)
        {
            if (p is BoxerStylePassive stylePassive)
            {
                attackBoostPercent += stylePassive.statBoostsPercent.physicalAttack * 0.01f;
                elementalPowerBoostPercent += stylePassive.statBoostsPercent.elementalPower * 0.01f;
                defenseBoostPercent += stylePassive.statBoostsPercent.defense * 0.01f;
                elementalResistanceBoostPercent += stylePassive.statBoostsPercent.elementalResistance * 0.01f;
                break;
            }
        }

        float totalMultiplier = multiplier;
        foreach (var p in self.passives)
        {
            if (p is BoxerStyleMultiplierPassive stylePassive && p != this)
            {
                totalMultiplier *= stylePassive.multiplier;
                stylePassive.SetAddBoost(false);
            }
        }

        
        Debug.Log($"Applying Boxer Style Multiplier: {totalMultiplier}x total multiplier, resulting in an additional {(totalMultiplier)*100}% boost to all stats");
        totalMultiplier--; // Account for base passive boost already being applied in BoxerStylePassive

        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * Mathf.Abs(attackBoostPercent) * totalMultiplier) * (int)Mathf.Sign(attackBoostPercent);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * Mathf.Abs(elementalPowerBoostPercent) * totalMultiplier) * (int)Mathf.Sign(elementalPowerBoostPercent);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * Mathf.Abs(defenseBoostPercent) * totalMultiplier) * (int)Mathf.Sign(defenseBoostPercent);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * Mathf.Abs(elementalResistanceBoostPercent) * totalMultiplier) * (int)Mathf.Sign(elementalResistanceBoostPercent);
    }

    public void SetAddBoost(bool value)
    {
        addBoost = value;
    }
}
