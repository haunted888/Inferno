using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Soldier/General Passive")]
public class GeneralPassive : PassivesDefinition
{

    public HeroPassiveDefinition heroPassive;

    public override void OnBattleStart(BattleCharacter self)
    {
        if (heroPassive == null) return;
        if (!self.traitTypes.Contains(CharacterTrait.Hero))
        {
            self.RemovePassive(this);
            return; // Only applies if the character has the Hero trait
        }

        HeroTraitDefinition trait = self.Traits.Find(t => t is HeroTraitDefinition) as HeroTraitDefinition;

        if (trait == null) return;

        foreach (var ally in self.GetAllies())
        {
            if (ally == self) continue; // Skip self
            heroPassive.SetStatBoosts(trait.physicalAttackBoost, trait.elementalAttackBoost, trait.physicalDefenseBoost, trait.elementalResistanceBoost);
            ally.AddPassive(heroPassive);
        }
    }
}
