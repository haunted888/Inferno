using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Entertainer")]
public class EntertainerTraitDefinition : TraitDefinition
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EntertainerPassiveDefinition entertainerPassive;

    [Header("SP heal percent")]
    public float spHeal = 0.5f;

    void Awake()
    {
        traitType = CharacterTrait.Entertainer;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnBattleStart(BattleCharacter self)
    {
        entertainerPassive.setSpHeal(spHeal);
        self.QueuePassiveToAdd(entertainerPassive, PassivesDefinition.PassiveHook.OnBattleStart);
    }
}
