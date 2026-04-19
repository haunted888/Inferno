using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Status Buffer Passive")]
public class StatusBufferPassiveDefinition : StatusPassiveDefinition
{
    

    public override void OnSkillUsedEnd(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        self.QueuePassiveToRemove(this, PassiveHook.OnSkillUsedEnd);
    }

}
