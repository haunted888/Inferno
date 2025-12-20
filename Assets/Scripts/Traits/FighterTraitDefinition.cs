using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Fighter")]
public class FighterTraitDefinition : TraitDefinition
{
    [Header("Fighter Defense Passive")]
    public FighterPassiveDefinition defensePassive;

    public float defenseBonusPercent = 0.30f;

    public float elementalResistanceBonusPercent = 0.30f;

    void Awake()
    {
        traitType = CharacterTrait.Fighter;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        defensePassive.setDefenseBoostAmount(defenseBonusPercent);
        defensePassive.setResistanceBoostAmount(elementalResistanceBonusPercent);
        battleChar.AddPassive(defensePassive);
    }

}
