using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Cleric")]
public class ClericTraitDefinition : TraitDefinition
{

    public PassivesDefinition fireBlessingPassive;
    public PassivesDefinition stormBlessingPassive;
    public PassivesDefinition iceBlessingPassive;
    public PassivesDefinition bloodBlessingPassive;
    public PassivesDefinition acidBlessingPassive;
    public PassivesDefinition psychicBlessingPassive;

    void Awake()
    {
        traitType = CharacterTrait.Cleric;
    }

    public override void OnBattleStart(BattleCharacter user)
    {
        if(user == null || user.IsDead) return;

        var attackStats = user.GetSubAttackStats();
        var defenseStats = user.GetSubDefenseStats();

        Dictionary<DamageSubType, int> subTypeValues = new Dictionary<DamageSubType, int>();
        subTypeValues[DamageSubType.Fire] = attackStats[DamageSubType.Fire] + defenseStats[DamageSubType.Fire];
        subTypeValues[DamageSubType.Ice] = attackStats[DamageSubType.Ice] + defenseStats[DamageSubType.Ice];
        subTypeValues[DamageSubType.Storm] = attackStats[DamageSubType.Storm] + defenseStats[DamageSubType.Storm];
        subTypeValues[DamageSubType.Blood] = attackStats[DamageSubType.Blood] + defenseStats[DamageSubType.Blood];
        subTypeValues[DamageSubType.Acid] = attackStats[DamageSubType.Acid] + defenseStats[DamageSubType.Acid];
        subTypeValues[DamageSubType.Psychic] = attackStats[DamageSubType.Psychic] + defenseStats[DamageSubType.Psychic];

        List<DamageSubType> highestSubTypes = new List<DamageSubType>();
        int highestValue = int.MinValue;

        foreach(var kvp in subTypeValues)
        {
            if(kvp.Value > highestValue)
            {
                highestSubTypes.Clear();
                highestSubTypes.Add(kvp.Key);
                highestValue = kvp.Value;
            }
            else if(kvp.Value == highestValue)
            {
                highestSubTypes.Add(kvp.Key);
            }
        }

        var highestSubType = highestSubTypes[Random.Range(0, highestSubTypes.Count)];

        // Apply the corresponding passive based on the highest sub type
        switch (highestSubType)
        {
            case DamageSubType.Fire:
                user.AddPassive(fireBlessingPassive);
                break;
            case DamageSubType.Storm:
                user.AddPassive(stormBlessingPassive);
                break;
            case DamageSubType.Ice:
                user.AddPassive(iceBlessingPassive);
                break;
            case DamageSubType.Blood:
                user.AddPassive(bloodBlessingPassive);
                break;
            case DamageSubType.Acid:
                user.AddPassive(acidBlessingPassive);
                break;
            case DamageSubType.Psychic:
                user.AddPassive(psychicBlessingPassive);
                break;
        }
    }

    public Dictionary<DamageSubType, int> GetSubTypeValues(BattleCharacter character)
    {
        var attackStats = character.GetSubAttackStats();
        var defenseStats = character.GetSubDefenseStats();

        Dictionary<DamageSubType, int> subTypeValues = new Dictionary<DamageSubType, int>();
        subTypeValues[DamageSubType.Fire] = attackStats[DamageSubType.Fire] + defenseStats[DamageSubType.Fire];
        subTypeValues[DamageSubType.Ice] = attackStats[DamageSubType.Ice] + defenseStats[DamageSubType.Ice];
        subTypeValues[DamageSubType.Storm] = attackStats[DamageSubType.Storm] + defenseStats[DamageSubType.Storm];
        subTypeValues[DamageSubType.Blood] = attackStats[DamageSubType.Blood] + defenseStats[DamageSubType.Blood];
        subTypeValues[DamageSubType.Acid] = attackStats[DamageSubType.Acid] + defenseStats[DamageSubType.Acid];
        subTypeValues[DamageSubType.Psychic] = attackStats[DamageSubType.Psychic] + defenseStats[DamageSubType.Psychic];

        return subTypeValues;
    }

    public PassivesDefinition GetPassiveForSubType(DamageSubType subType)
    {
        return subType switch
        {
            DamageSubType.Fire => fireBlessingPassive,
            DamageSubType.Storm => stormBlessingPassive,
            DamageSubType.Ice => iceBlessingPassive,
            DamageSubType.Blood => bloodBlessingPassive,
            DamageSubType.Acid => acidBlessingPassive,
            DamageSubType.Psychic => psychicBlessingPassive,
            _ => null
        };
    }
}
