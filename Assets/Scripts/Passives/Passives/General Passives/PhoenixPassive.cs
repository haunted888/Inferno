using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Phoenix Passive")]
public class PhoenixPassiveDefinition : PassivesDefinition
{
    public int healthToRestore = 1;
    public float healthPercentToRestore = .5f;
    public override void OnAfterTakeDamage(BattleCharacter self, int amount, SkillDamageType damageType, DamageSubType subDamageType)
    {
        if (self.IsDead)
        {
            int health = healthToRestore > 0 ? healthToRestore : Mathf.CeilToInt(self.MaxHealth * healthPercentToRestore);
            self.SetCurrentHealth(health); 
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnAfterTakeDamage);
        }
    }
}
