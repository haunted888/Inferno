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

    public void SetSubAttackBoost(DamageSubType subType, int boostAmount)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning:
                bludgeoningAttack += boostAmount;
                break;
            case DamageSubType.Slashing:
                slashingAttack += boostAmount;
                break;
            case DamageSubType.Piercing:
                piercingAttack += boostAmount;
                break;
            case DamageSubType.Fire:
                fireAttack += boostAmount;
                break;
            case DamageSubType.Ice:
                iceAttack += boostAmount;
                break;
            case DamageSubType.Storm:
                stormAttack += boostAmount;
                break;
            case DamageSubType.Acid:
                acidAttack += boostAmount;
                break;
            case DamageSubType.Psychic:
                psychicAttack += boostAmount;
                break;
            case DamageSubType.Blood:
                bloodAttack += boostAmount;
                break;
        }
    }

    public void SetSubDefenseBoost(DamageSubType subType, int boostAmount)
    {
        switch (subType)
        {
            case DamageSubType.Bludgeoning:
                bludgeoningDefense += boostAmount;
                break;
            case DamageSubType.Slashing:
                slashingDefense += boostAmount;
                break;
            case DamageSubType.Piercing:
                piercingDefense += boostAmount;
                break;
            case DamageSubType.Fire:
                fireDefense += boostAmount;
                break;
            case DamageSubType.Ice:
                iceDefense += boostAmount;
                break;
            case DamageSubType.Storm:
                stormDefense += boostAmount;
                break;
            case DamageSubType.Acid:
                acidDefense += boostAmount;
                break;
            case DamageSubType.Psychic:
                psychicDefense += boostAmount;
                break;
            case DamageSubType.Blood:
                bloodDefense += boostAmount;
                break;
        }
    }

}
