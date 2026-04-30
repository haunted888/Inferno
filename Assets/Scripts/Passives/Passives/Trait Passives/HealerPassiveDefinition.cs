using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Healer")]
public class HealerPassiveDefinition : PassivesDefinition
{
    public float healIncreaseMultiplier = 0.5f;

    public override void BeforeHealingSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        float healIncreaseAmount = 1.0f + (healIncreaseMultiplier * (1.0f - target.CurrentHealth / target.MaxHealth));

        target.ModifyIncomingHealingMultiplier(healIncreaseAmount);
    }
}
