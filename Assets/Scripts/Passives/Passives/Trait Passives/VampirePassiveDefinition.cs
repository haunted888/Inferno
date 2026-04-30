using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Vampire")]
public class VampirePassiveDefinition : PassivesDefinition
{

    public float lifestealPercentage = 0.5f;
    private bool isVampiricDamage = false;

    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if(skill.traitTags.Contains(CharacterTrait.Vampire))
            isVampiricDamage = true;
    }

    public override void OnAfterDealDamage(BattleCharacter self, BattleCharacter target, int amount)
    {
        if (!isVampiricDamage) return;
        
        int healAmount = Mathf.RoundToInt(amount * lifestealPercentage);
        self.Heal(healAmount);
    }

    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        isVampiricDamage = false;
    }

 

}
