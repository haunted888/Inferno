using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Villain")]
public class VillainTraitDefinition : TraitDefinition
{
    public VillainPassiveDefinition allyPassiveBoost;

    [Header("Stat boosts")]
    public int physicalAttackBoost;
    public int elementalAttackBoost;
    public int physicalDefenseBoost;
    public int elementalResistanceBoost;

    void Awake()
    {
        traitType = CharacterTrait.Villain;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnBattleStart(BattleCharacter user)
    {
        var allies = BattleTurnManager.Instance.GetAlliesOf(user);
        allyPassiveBoost.setStatBoosts(physicalAttackBoost, elementalAttackBoost, physicalDefenseBoost, elementalResistanceBoost);
        foreach (var ally in allies)
        {
            if(ally == user) continue;
            ally.AddPassive(allyPassiveBoost);
        }
    }

    public override void OnDeath(BattleCharacter user)
    {
        var allies = BattleTurnManager.Instance.GetAlliesOf(user);
        foreach (var ally in allies)
        {
            if(ally == user) continue;
            ally.RemovePassive(allyPassiveBoost);
        }
        base.OnBattleStart(user);
    }
}
