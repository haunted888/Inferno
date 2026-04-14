using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Villain Passive Boost")]
public class VillainPassiveDefinition : PassivesDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int physicalAttackBoost = 10;
    int elementalAttackBoost = 10;
    int defenseBoost = 10;
    int resistanceBoost = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setStatBoosts(int physicalAttackBoost, int elementalAttackBoost, int defenseBoost, int resistanceBoost)
    {
        this.physicalAttackBoost = physicalAttackBoost;
        this.elementalAttackBoost = elementalAttackBoost;
        this.defenseBoost = defenseBoost;
        this.resistanceBoost = resistanceBoost;
    }

    public override void GetStatBoosts(BattleCharacter self)
    {
        int h = 1;
        h += self.traitTypes.Contains(CharacterTrait.Hero) ? 1 : 0;
        h -= self.traitTypes.Contains(CharacterTrait.Villain) ? 1 : 0;
        self.bonusStats.physicalAttack += physicalAttackBoost * h * self.level;
        self.bonusStats.elementalPower += elementalAttackBoost * h * self.level;
        self.bonusStats.defense += defenseBoost * h * self.level;
        self.bonusStats.elementalResistance += resistanceBoost * h * self.level;
    }
}
