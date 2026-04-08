using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Apply Passive Future")]
public class ApplyPassiveFuture : PassivesDefinition
{
    public PassivesDefinition passiveToApply;
    public int turnsUntilApply = 1;

    public override void OnCommandPhaseStart(BattleCharacter self)
    {
        turnsUntilApply--;
        if (turnsUntilApply <= 0)
        {
            self.QueuePassiveToAdd(passiveToApply, PassivesDefinition.PassiveHook.OnCommandPhaseStart);
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnCommandPhaseStart);
        }
    }
}
