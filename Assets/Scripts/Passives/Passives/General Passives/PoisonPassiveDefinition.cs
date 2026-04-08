using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Poison")]
public class PoisonPassiveDefinition : PassivesDefinition
{

    private const float poisonDamagePercent = .01f;
    public int counter = 1;
    public override void OnCreated(BattleCharacter self)
    {
        bool removeThisPoison = false;
        foreach(var passive in self.passives)
        {
            if(passive is PoisonPassiveDefinition && passive != this)
            {
                ((PoisonPassiveDefinition)passive).counter++;
                removeThisPoison = true;
            }
        }
        if (removeThisPoison)
        {
            self.RemovePassive(this);
            Debug.Log($"{self.name} already has poison, increasing counter to {counter} and removing duplicate.");
        }
    }

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;
        int poisonDamage = Mathf.CeilToInt(self.MaxHealth * poisonDamagePercent * counter);

        self.TakeDamage(poisonDamage);
        SetDisplayText($"{self.name} is hurt by poison!");

        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
        }
    }
    
}
