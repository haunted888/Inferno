using UnityEngine;

public class StrategistTraitDefinition : TraitDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public StrategistPassiveDefinition strategistPassiveDefinition;


    void Awake()
    {
        traitType = CharacterTrait.Strategist;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnBattleStart(BattleCharacter user)
    {
        user.AddPassive(strategistPassiveDefinition);
    }
}
