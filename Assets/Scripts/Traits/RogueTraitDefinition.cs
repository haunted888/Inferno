using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Rogue")]
public class RogueTraitDefinition : TraitDefinition
{
    [Header("Rogue Passive")]
    public RoguePassiveDefinition roguePassive;


    void Awake()
    {
        traitType = CharacterTrait.Rogue;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        battleChar.AddPassive(roguePassive);
    }

}
