using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Healer")]
public class HealerTraitDefinition : TraitDefinition
{
    [Header("Healer Passive")]
    public HealerPassiveDefinition healerPassive;


    void Awake()
    {
        traitType = CharacterTrait.Healer;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        battleChar.AddPassive(healerPassive);
    }

}
