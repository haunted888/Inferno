using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Caesar/Vidi")]
public class VidiPassive : PassivesDefinition
{
    
    private const float IncomingDamageMultiplierMin = 0f;
    private const float IncomingDamageMultiplierMax = 1f;
    private const float OutgoingDamageMultiplierMin = 1f;
    private const float OutgoingDamageMultiplierMax = 0f;

    public override void BeforeDamageSkillExecute(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        self.SetOutgoingDamageMultiplierLimits(OutgoingDamageMultiplierMin, OutgoingDamageMultiplierMax);
        if (skill is not DamageSkillParent) return;
        if (target == null) return;

        target.SetIncomingDamageMultiplierLimits(IncomingDamageMultiplierMin, IncomingDamageMultiplierMax);
    }
}
