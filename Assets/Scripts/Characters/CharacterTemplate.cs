using System.Collections.Generic;
using UnityEngine;

public enum CombatMainStat
{
    MaxHealth,
    MaxSp,
    Speed,
    PhysicalAttack,
    ElementalPower,
    Defense,
    ElementalResistance,
    CritChance,
    CritDamage
}

public enum CombatSubStat
{
    // Physical sub-attack
    BludgeoningAttack,
    SlashingAttack,
    PiercingAttack,

    // Physical sub-defense
    BludgeoningDefense,
    SlashingDefense,
    PiercingDefense,

    // Elemental sub-attack
    FireAttack,
    IceAttack,
    StormAttack,
    AcidAttack,
    PsychicAttack,
    BloodAttack,

    // Elemental sub-defense
    FireDefense,
    IceDefense,
    StormDefense,
    AcidDefense,
    PsychicDefense,
    BloodDefense
}


[CreateAssetMenu(menuName = "Characters/Character Template")]
public class CharacterTemplate : ScriptableObject
{
    public string displayName = "Unnamed";

    [Header("Traits")]
    public List<TraitDefinition> traits = new List<TraitDefinition>();

    [Header("Stats")]
    public CombatStats baseStats = new CombatStats
    {
        maxHealth          = 100,
        maxSp              = 10,
        speed              = 10,
        physicalAttack     = 10,
        elementalPower     = 10,
        defense            = 0,
        elementalResistance= 0,
        critChance         = 0,
        critDamage         = 150,

        // Physical sub-attack
        bludgeoningAttack  = 0,
        slashingAttack     = 0,
        piercingAttack     = 0,

        // Physical sub-defense
        bludgeoningDefense = 0,
        slashingDefense    = 0,
        piercingDefense    = 0,

        // Elemental sub-attack
        fireAttack         = 0,
        iceAttack          = 0,
        stormAttack       = 0,
        acidAttack        = 0,
        psychicAttack     = 0,
        bloodAttack       = 0,

        // Elemental sub-defense
        fireDefense        = 0,
        iceDefense         = 0,
        stormDefense      = 0,
        acidDefense       = 0,
        psychicDefense    = 0,
        bloodDefense      = 0
    };

    [Header("Skills")]
    public List<Skill> skills;

    [Header("Substat Seed (exactly 6)")]
    public List<CombatSubStat> predeterminedSubStats = new List<CombatSubStat>(6);


}
