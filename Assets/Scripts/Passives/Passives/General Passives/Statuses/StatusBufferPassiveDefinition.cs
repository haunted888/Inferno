using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Status Buffer Passive")]
public class StatusBufferPassiveDefinition : StatusPassiveDefinition
{
    

    public override void OnActionEnd(BattleCharacter self, BattleCharacter target)
    {
        self.QueuePassiveToRemove(this, PassiveHook.OnActionEnd);
    }

}
