using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Template")]
public class CharacterTemplate : ScriptableObject
{
    public string displayName = "Unnamed";

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
    public Skill[] skills;
}
