using UnityEngine;

[System.Serializable]
public struct CombatStats
{
    public int maxHealth;
    public int maxSp;
    public int speed;
    public int physicalAttack;
    public int elementalPower;
    public int defense;
    public int elementalResistance;
    public int critChance;
    public int critDamage;

    // Physical sub-attack
    public int bludgeoningAttack;
    public int slashingAttack;
    public int piercingAttack;

    // Physical sub-defense
    public int bludgeoningDefense;
    public int slashingDefense;
    public int piercingDefense;

    // Elemental sub-attack
    public int fireAttack;
    public int iceAttack;
    public int stormAttack;
    public int acidAttack;
    public int psychicAttack;
    public int bloodAttack;

    // Elemental sub-defense
    public int fireDefense;
    public int iceDefense;
    public int stormDefense;
    public int acidDefense;
    public int psychicDefense;
    public int bloodDefense;
}
