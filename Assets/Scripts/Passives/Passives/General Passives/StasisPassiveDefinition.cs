using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Stasis")]
public class StasisPassiveDefinition : PassivesDefinition
{
    [Header("'User is ____ and cannot act!'")]
    [TextArea] public string turnSkipDisplayText = "in stasis";

    public override void OnDestroyed(BattleCharacter self)
    {
        counter--;
        if(counter <= 0)
        {
            self.QueuePassiveToRemove(this, PassivesDefinition.PassiveHook.OnDestroyed);
        }
    }
}
