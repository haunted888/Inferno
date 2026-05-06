using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Damage Taken Counter")]
public class DamageTakenCounter : PassivesDefinition
{
    public int multiplier = 1;
    public SkillDamageType damageTypeToCount = SkillDamageType.None;
    public DamageSubType subDamageTypeToCount = DamageSubType.None;

    public override void OnAfterTakeDamage(BattleCharacter self, int amount, SkillDamageType damageType, DamageSubType subDamageType)
    {
        switch (damageTypeToCount)
        {
            case SkillDamageType.Physical:
                if (damageType != SkillDamageType.Physical) return;
                counter += subDamageType == subDamageTypeToCount || subDamageTypeToCount == DamageSubType.None ? amount * multiplier : 0;
                break;
            case SkillDamageType.Elemental:
                if (damageType != SkillDamageType.Elemental) return;
                counter += subDamageType == subDamageTypeToCount || subDamageTypeToCount == DamageSubType.None ? amount * multiplier : 0;
                break;
            case SkillDamageType.Adaptive:
                if (damageType != SkillDamageType.Adaptive) return;
                counter += subDamageType == subDamageTypeToCount || subDamageTypeToCount == DamageSubType.None ? amount * multiplier : 0;
                break;
            case SkillDamageType.None:
                counter += subDamageType == subDamageTypeToCount || subDamageTypeToCount == DamageSubType.None ? amount * multiplier : 0;
                break;
        }
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        self.QueuePassiveToRemove(this, PassiveHook.OnResolvePhaseEnd);
    }
}
