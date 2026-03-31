using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Dummy")]
public class DummyTraitDefinition : TraitDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created[Header("Fighter Defense Passive")]
    public CaesarPassive TestPassive;


    void Awake()
    {
        traitType = CharacterTrait.Rogue;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        battleChar.AddPassive(TestPassive);
    }
}
