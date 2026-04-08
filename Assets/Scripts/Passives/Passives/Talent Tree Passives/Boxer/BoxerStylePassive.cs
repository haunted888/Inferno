using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Boxer/Boxer Style")]
public class BoxerStylePassive : StatBoostPercentPassiveDefinition
{




    public override void GetStatBoosts(BattleCharacter self)
    {

        Debug.Log($"Applying stat boosts: {attackBoostPercent*100}% attack, {elementalPowerBoostPercent*100}% elemental power, {defenseBoostPercent*100}% defense, {elementalResistanceBoostPercent*100}% elemental resistance");
        if (self == null) return;
        
        self.bonusStats.physicalAttack += Mathf.CeilToInt(self.baseStats.physicalAttack * Mathf.Abs(attackBoostPercent)) * (int)Mathf.Sign(attackBoostPercent);
        self.bonusStats.elementalPower += Mathf.CeilToInt(self.baseStats.elementalPower * Mathf.Abs(elementalPowerBoostPercent) ) * (int)Mathf.Sign(elementalPowerBoostPercent);
        self.bonusStats.defense += Mathf.CeilToInt(self.baseStats.defense * Mathf.Abs(defenseBoostPercent)) * (int)Mathf.Sign(defenseBoostPercent);
        self.bonusStats.elementalResistance += Mathf.CeilToInt(self.baseStats.elementalResistance * Mathf.Abs(elementalResistanceBoostPercent)) * (int)Mathf.Sign(elementalResistanceBoostPercent);


        
    }

}