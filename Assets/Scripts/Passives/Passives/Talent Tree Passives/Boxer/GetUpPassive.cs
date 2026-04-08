using UnityEngine;


[CreateAssetMenu(menuName = "Passives/Boxer/Get Up Passive")]
public class GetUpPassive : PassivesDefinition
{
    public override void OnAfterTakeDamage(BattleCharacter self, int amount)
    {
        Debug.Log($"Boxer is dead: {self.IsDead}");
        if (self.IsDead)
        {
            Debug.Log("Boxer gets up!");
            self.SetCurrentHealth(1); 
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnAfterTakeDamage);
        }
    }
}
