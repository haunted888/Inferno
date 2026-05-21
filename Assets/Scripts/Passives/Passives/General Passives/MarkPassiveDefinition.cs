using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Mark")]
public class MarkPassiveDefinition : PassivesDefinition
{

    [Range(0f, 1f)]
    public float damageIncreasePercent = 0.10f;

    public override void OnSkillReceived(BattleCharacter self, BattleCharacter attacker, Skill skill)
    {
        if (self == null || skill == null) return;
        if (!skill.GetAllEffectTypes().Contains(SkillEffectType.Damage)) return;

        self.AddIncomingDamageMultiplier(damageIncreasePercent);
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);

    }
}
