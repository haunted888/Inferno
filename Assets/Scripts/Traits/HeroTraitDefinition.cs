using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Hero")]
public class HeroTraitDefinition : TraitDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public HeroPassiveDefinition heroPassiveBoost;

    [Header("Stat boosts")]
    public int physicalAttackBoost = 10;
    public int elementalAttackBoost = 10;
    public int physicalDefenseBoost = 10;
    public int elementalResistanceBoost = 10;

    void Awake()
    {
        traitType = CharacterTrait.Hero;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnBattleStart(BattleCharacter user)
    {
        heroPassiveBoost.SetStatBoosts(physicalAttackBoost, elementalAttackBoost, physicalDefenseBoost, elementalResistanceBoost);
        user.AddPassive(heroPassiveBoost);
    }

}
