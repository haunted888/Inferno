using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Fighter")]
public class FighterTraitDefinition : TraitDefinition
{
    public float elementalResistanceBonus = .3f;
    public float physicalDefenseBonus = .3f;
    private int storedDefenseBonus = 0;
    private int storedResistanceBonus = 0;

    public void Awake()
    {
        traitType = CharacterTrait.Fighter;
    }

    public override void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar)
    {
        storedDefenseBonus = 0;
        storedResistanceBonus = 0;
    }

    public override void onActionResolveStart(BattleCharacter user, Skill skill, BattleCharacter target)
    {
        if(skill.damageType != SkillDamageType.None)
        {
            // Apply Fighter trait effects
            storedDefenseBonus = Mathf.CeilToInt(physicalDefenseBonus * user.baseStats.defense);
            storedResistanceBonus = Mathf.CeilToInt(elementalResistanceBonus * user.baseStats.elementalResistance);
            user.bonusStats.elementalResistance += storedResistanceBonus;
            user.bonusStats.defense += storedDefenseBonus;
        }
    }

    public override void OnActionResolveEnd(BattleCharacter user, Skill skill, BattleCharacter target)
    {
        user.bonusStats.defense -= storedDefenseBonus;
        user.bonusStats.elementalResistance -= storedResistanceBonus;
        storedDefenseBonus = 0;
        storedResistanceBonus = 0;
    }
 

}
