using TMPro;
using UnityEngine;

public class LevelUpStatsUIPrefab : MonoBehaviour
{
    private CombatStats statBonus;
    public TMP_Text statText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setStatBonus(CombatStats statBonus)
    {
        this.statBonus = statBonus;
    }

    public CombatStats getStatBonus()
    {
        return statBonus;
    }

    public void setStatText(CombatStats stats = new CombatStats())
    {
        statText.text = "Receive:\n\n";
        if (stats.maxHealth > 0) statText.text += $"+{stats.maxHealth} Max Health\n";
        if (stats.maxSp > 0) statText.text += $"+{stats.maxSp} Max SP\n";
        if (stats.physicalAttack > 0) statText.text += $"+{stats.physicalAttack} Attack\n";
        if (stats.defense > 0) statText.text += $"+{stats.defense} Defense\n";
        if (stats.elementalPower > 0) statText.text += $"+{stats.elementalPower} Magic\n";
        if (stats.elementalResistance > 0) statText.text += $"+{stats.elementalResistance} Elemental Resistance\n";
        if (stats.speed > 0) statText.text += $"+{stats.speed} Speed\n";
        if (stats.critChance > 0) statText.text += $"+{stats.critChance} Crit Chance\n";
        if (stats.critDamage > 0) statText.text += $"+{stats.critDamage} Crit Damage\n";
        if (stats.bludgeoningAttack > 0) statText.text += $"+{  stats.bludgeoningAttack} Bludgeoning Attack\n";
        if (stats.slashingAttack > 0) statText.text += $"+{stats.slashingAttack} Slashing Attack\n";
        if (stats.piercingAttack > 0) statText.text += $"+{stats.piercingAttack} Piercing Attack\n";
        if (stats.fireAttack > 0) statText.text += $"+{stats.fireAttack} Fire Attack\n";
        if (stats.iceAttack > 0) statText.text += $"+{stats.iceAttack} Ice Attack\n";
        if (stats.stormAttack > 0) statText.text += $"+{stats.stormAttack} Storm Attack\n";
        if (stats.acidAttack > 0) statText.text += $"+{stats.acidAttack} Acid Attack\n";
        if (stats.psychicAttack > 0) statText.text += $"+{stats.psychicAttack} Psychic Attack\n";
        if (stats.bloodAttack > 0) statText.text += $"+{stats.bloodAttack} Blood Attack\n";
        if (stats.bludgeoningDefense > 0) statText.text += $"+{stats.bludgeoningDefense} Bludgeoning Defense\n";
        if (stats.slashingDefense > 0) statText.text += $"+{stats.slashingDefense} Slashing Defense\n";
        if (stats.piercingDefense > 0) statText.text += $"+{stats.piercingDefense} Piercing Defense\n";
        if (stats.fireDefense > 0) statText.text += $"+{stats.fireDefense} Fire Defense\n";
        if (stats.iceDefense > 0) statText.text += $"+{stats.iceDefense} Ice Defense\n";
        if (stats.stormDefense > 0) statText.text += $"+{stats.stormDefense} Storm Defense\n";
        if (stats.acidDefense > 0) statText.text += $"+{stats.acidDefense} Acid Defense\n";
        if (stats.psychicDefense > 0) statText.text += $"+{stats.psychicDefense} Psychic Defense\n";
        if (stats.bloodDefense > 0) statText.text += $"+{stats.bloodDefense} Blood Defense\n";

    }

}
