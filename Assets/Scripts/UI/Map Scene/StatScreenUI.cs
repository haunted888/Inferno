using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatScreenUI : MonoBehaviour
{
    public TMP_Text hpStatText;
    public TMP_Text attackStatText;
    public TMP_Text elementalStatText;
    public TMP_Text defenseStatText;
    public TMP_Text resistanceStatText;
    public TMP_Text speedStatText;
    public GameObject statPanel;

    public void Start()
    {
        statPanel.gameObject.SetActive(false);
    }
    public void UpdateStats(CombatStats stats)
    {
        if (hpStatText != null)
            hpStatText.text = $"HP: {stats.maxHealth}";
        if (attackStatText != null)
            attackStatText.text = $"Attack: {stats.physicalAttack}";
        if (elementalStatText != null)
            elementalStatText.text = $"Elemental Power: {stats.elementalPower}";
        if (defenseStatText != null)
            defenseStatText.text = $"Defense: {stats.defense}";
        if (resistanceStatText != null)
            resistanceStatText.text = $"Elemental Resistance: {stats.elementalResistance}";
        if (speedStatText != null)
            speedStatText.text = $"Speed: {stats.speed}";
    }
}
