using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Vampire")]
public class VampireTraitDefinition : TraitDefinition
{
    [Header("Vampire Passive")]
    public VampirePassiveDefinition vampirePassive;

    public void Awake()
    {
        traitType = CharacterTrait.Vampire;
    }

    public override void OnBattleStart(BattleCharacter user)
    {
        user.AddPassive(vampirePassive);
    }
}
