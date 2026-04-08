using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Caesar/Betrayal")]
public class BetrayalPassive : PassivesDefinition
{
    public BattleCharacter betrayed;
    public float damagePercent = 0.1f;

    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if (self == null || skill == null) return;
        if (betrayed == null || betrayed.IsDead) return;

        if (skill.damageType != SkillDamageType.None)
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
