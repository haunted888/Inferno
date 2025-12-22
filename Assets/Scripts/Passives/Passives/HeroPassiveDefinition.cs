using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Hero Passive Definition")]
public class HeroPassiveDefinition : PassivesDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int physicalAttackBoost = 0;
    int elementalAttackBoost = 0;
    int defenseBoost = 0;
    int resistanceBoost = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setStatBoosts(int physicalAttackBoost, int elementalAttackBoost, int defenseBoost, int resistanceBoost)
    {
        this.physicalAttackBoost = physicalAttackBoost;
        this.elementalAttackBoost = elementalAttackBoost;
        this.defenseBoost = defenseBoost;
        this.resistanceBoost = resistanceBoost;
    }

    public override void getStatBoosts(BattleCharacter self)
    {
        int h = 0;
        var allies = BattleTurnManager.Instance.GetAlliesOf(self);
        foreach (var a in allies)
        {
            if (a == self) continue;
            if (a.traitTypes.Contains(CharacterTrait.Hero)) continue;
            if (a.traitTypes.Contains(CharacterTrait.Villain)) continue;
            h++;
        }
        self.bonusStats.physicalAttack += physicalAttackBoost * h;
        self.bonusStats.elementalPower += elementalAttackBoost * h;
        self.bonusStats.defense += defenseBoost * h;
        self.bonusStats.elementalResistance += resistanceBoost * h;
    }
}
