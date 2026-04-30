using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Sovereign Trait")]
public class SovereignTraitDefinition : TraitDefinition
{
    [Header("Sovereign Passive")]
    public SovereignPassiveDefinition sovereignPassive;


    void Awake()
    {
        traitType = CharacterTrait.Sovereign;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        battleChar.AddPassive(sovereignPassive);
    }
}
