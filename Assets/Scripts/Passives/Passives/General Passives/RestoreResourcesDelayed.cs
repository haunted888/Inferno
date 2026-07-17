using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Restore Resources Delayed")]
public class RestoreResourcesDelayed : PassivesDefinition
{
    public int resourceAmount = 10;
    public bool isPercentage = false;
    public ResourceType resourceType;

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        if (counter > 1)
        {
            base.OnResolvePhaseEnd(self);
            return;
        }

        counter = 0;
        int amountToRestore = isPercentage ? Mathf.RoundToInt(GetResourceAmount(self) * (resourceAmount * 0.01f)) : resourceAmount;
        RestoreResource(self, amountToRestore);
        self.QueuePassiveToRemove(this, PassiveHook.OnResolvePhaseEnd);
    }

    private int GetResourceAmount(BattleCharacter character)
    {
        switch (resourceType)
        {
            case ResourceType.HP:
                return character.CurrentHealth;
            case ResourceType.SP:
                return character.CurrentSp;
            case ResourceType.Ammo:
                return character.CurrentAmmo;
            default:
                return 0;
        }
    }

    private void RestoreResource(BattleCharacter character, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.HP:
                character.Heal(amount);
                break;
            case ResourceType.SP:
                character.RecoverSp(amount);
                break;
            case ResourceType.Ammo:
                character.AddAmmo(amount);
                break;
        }
    }

    public override string GetDescription(BattleCharacter character)
    {
        string resourceName = resourceType.ToString();
        string amountText = isPercentage ? $"{resourceAmount}%" : resourceAmount.ToString();
        return $"Restores {amountText} {resourceName} after {counter} turns.";
    }

}
