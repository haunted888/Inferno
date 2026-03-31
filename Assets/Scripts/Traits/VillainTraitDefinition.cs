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
    public override void OnBattleStart(BattleCharacter self)
    {
        var allies = self.GetAllies();
        allyPassiveBoost.setStatBoosts(physicalAttackBoost, elementalAttackBoost, physicalDefenseBoost, elementalResistanceBoost);
        foreach (var ally in allies)
        {
            if(ally == self) continue;
            ally.AddPassive(allyPassiveBoost);
        }
        base.OnBattleStart(self);
    }

    public override void OnDeath(BattleCharacter self)
    {
        var allies = self.GetAllies();
        foreach (var ally in allies)
        {
            if(ally == self) continue;
            ally.RemovePassive(allyPassiveBoost);
        }
        base.OnDeath(self);
    }
}
