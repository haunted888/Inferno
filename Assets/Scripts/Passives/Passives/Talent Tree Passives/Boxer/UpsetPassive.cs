using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Boxer/Upset Passive")]
public class UpsetPassive : PassivesDefinition
{
    public float multiplier = 1; // Multiplier for SP recovery, default to 1 for no change

    public override void OnAfterTakeDamage(BattleCharacter self, int damageAmount, SkillDamageType damageType, DamageSubType subDamageType)
    {
        if (damageAmount <= 0) return; 
        if(self.IsDead) return; 

        self.RecoverSp((int)(damageAmount * multiplier)); 

    }
}
