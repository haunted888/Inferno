using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Poison")]
public class PoisonPassiveDefinition : StatusPassiveDefinition
{

    private const int poisonDamageAmount = 1;
    public int counter = 1;
    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;

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
        int poisonDamage = poisonDamageAmount * counter;

        self.TakeDamage(poisonDamage);
        SetDisplayText($"{self.name} is hurt by poison!");

        if (counter <= 0)
        {
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnResolvePhaseEnd);
        }
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }

    
}
