using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Caesar/Betrayal")]
public class BetrayalPassive : PassivesDefinition
{
    public BattleCharacter betrayed;
    public float damagePercent = 0.1f;

    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if (self == null || skill == null) return;
        if (betrayed == null || betrayed.IsDead) return;

        if (skill.GetAllEffectTypes().Contains(SkillEffectType.Damage))
        {   
            betrayed.TakeDamage(Mathf.RoundToInt(betrayed.MaxHealth * damagePercent));
            Debug.Log($"BetrayalPassive triggered for {betrayed.name}.");
        }
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
    }
}
