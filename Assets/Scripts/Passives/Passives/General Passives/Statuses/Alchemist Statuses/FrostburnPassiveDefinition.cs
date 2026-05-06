using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Frostburn Passive")]
public class FrostburnPassiveDefinition : StatusPassiveDefinition
{
    public float frostburnDamagePercentMax = 0.05f;
    public float frostburnDamagePercentMin = 0.01f;

    public float healingCutAmount = 0.5f;

    

    public override void OnActionEnd(BattleCharacter self, BattleCharacter target)
    {
        int damage = Mathf.RoundToInt(self.MaxHealth * Random.Range(frostburnDamagePercentMin, frostburnDamagePercentMax));
        self.TakeDamage(damage, SkillDamageType.Elemental, DamageSubType.Ice);
    }

    public override void BeforeReceivingHealing(BattleCharacter self, int healingAmount)
    {
        self.ModifyIncomingHealingMultiplier(1 - healingCutAmount);
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }
}
