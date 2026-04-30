using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullStatScreenUI : MonoBehaviour
{
    public TMP_Text hpStatText;
    public TMP_Text spStatText;
    public TMP_Text attackStatText;
    public TMP_Text subAttackStatText;
    public TMP_Text elementalStatText;
    public TMP_Text subElementalStatText;
    public TMP_Text defenseStatText;
    public TMP_Text subDefenseStatText;
    public TMP_Text resistanceStatText;
    public TMP_Text subResistanceStatText;
    public TMP_Text speedStatText;
    public TMP_Text critChanceStatText;
    public TMP_Text critDamageStatText;

    public void UpdateStats(CombatStats stats)
    {
        if (hpStatText != null)
            hpStatText.text = $"HP: {stats.maxHealth}";
        if (spStatText != null)
            spStatText.text = $"SP: {stats.maxSp}";
        if (attackStatText != null)
            attackStatText.text = $"Attack: {stats.physicalAttack}";
        if (subAttackStatText != null)
            subAttackStatText.text = $"<color=#DBB543>Piercing: {stats.piercingAttack} <br>" +
                $"<color=#79A49B>Bludgeoning: {stats.bludgeoningAttack} <br>" +
                $"<color=#9774AA>Slashing: {stats.slashingAttack}";
        if (elementalStatText != null)
            elementalStatText.text = $"Elemental Power: {stats.elementalPower}";
        if (subElementalStatText != null)
            subElementalStatText.text = $"<color=#F16413>Fire: {stats.fireAttack} <br>" +
                $"<color=#A0E0F3>Ice: {stats.iceAttack} <br>" +
                $"<color=#15225B>Storm: {stats.stormAttack} <br>" +
                $"<color=#60F11D>Acid: {stats.acidAttack} <br>" +
                $"<color=#F292E2>Psychic: {stats.psychicAttack} <br>" +
                $"<color=#931011>Blood: {stats.bloodAttack}";
        if (defenseStatText != null)
            defenseStatText.text = $"Defense: {stats.defense}";
        if (subDefenseStatText != null)
            subDefenseStatText.text = $"<color=#DBB543>Piercing: {stats.piercingDefense} <br>" +
                $"<color=#79A49B>Bludgeoning: {stats.bludgeoningDefense} <br>" +
                $"<color=#9774AA>Slashing: {stats.slashingDefense}";
        if (resistanceStatText != null)
            resistanceStatText.text = $"Elemental Resistance: {stats.elementalResistance}";
        if (subResistanceStatText != null)
            subResistanceStatText.text = $"<color=#F16413>Fire: {stats.fireDefense} <br>" +
                $"<color=#A0E0F3>Ice: {stats.iceDefense} <br>" +
                $"<color=#15225B>Storm: {stats.stormDefense} <br>" +
                $"<color=#60F11D>Acid: {stats.acidDefense} <br>" +
                $"<color=#F292E2>Psychic: {stats.psychicDefense} <br>" +
                $"<color=#931011>Blood: {stats.bloodDefense}";
        if (speedStatText != null)
            speedStatText.text = $"Speed: {stats.speed}";
        if (critChanceStatText != null)
            critChanceStatText.text = $"Crit Chance: {stats.critChance}%";
        if (critDamageStatText != null)
            critDamageStatText.text = $"Crit Damage: {stats.critDamage}%";
    }
}
