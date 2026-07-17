using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Soldier/Guerilla Warfare Passive")]
public class GuerillaWarfarePassive : PassivesDefinition
{
    public StatBoostPercentPassiveDefinition passiveToApply;

    public override void OnCommandPhaseStart(BattleCharacter self)
    {
        if(self.GetAllies().Count() < self.GetEnemies().Count())
        {
            foreach (var ally in self.GetAllies())
            {
                if(ally.passives.Contains(passiveToApply)) continue; // Skip if the ally already has the passive
                if(ally == self)
                {
                    self.QueuePassiveToAdd(passiveToApply, PassivesDefinition.PassiveHook.OnCommandPhaseStart, self);
                    continue;
                }
                ally.AddPassive(passiveToApply, self);
                
            }
        }
        else
        {
            foreach (var ally in self.GetAllies())
            {
                if(!ally.passives.Contains(passiveToApply)) continue; // Skip if the ally doesn't have the passive

                if(ally == self)
                {
                    self.QueuePassiveToRemove(passiveToApply, PassivesDefinition.PassiveHook.OnCommandPhaseStart);
                    continue;
                }
                ally.RemovePassive(passiveToApply);
            }
        }
    }

}
