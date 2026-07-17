using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Characters/Thief/No Honor")]
public class NoHonorPassive : PassivesDefinition
{
    public int spRestore = 100;


    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        int maxSp = self.MaxSp < spRestore ? self.MaxSp : spRestore;

        if(self.CurrentSp < maxSp)
        {
            int spToRestore = maxSp - self.CurrentSp;
            BattleCharacter rightAlly = GetRightAlly(self);
            if (rightAlly == null) return;
                         
            if(rightAlly.CurrentSp >= spToRestore)
            {
                self.RecoverSp(spToRestore);
                rightAlly.TrySpendSp(spToRestore);
            }
            else
            {
                self.RecoverSp(rightAlly.CurrentSp);
                rightAlly.TrySpendSp(rightAlly.CurrentSp);
            }
        }
    }

    private BattleCharacter GetRightAlly(BattleCharacter self)
    {
        BattleCharacter rightAlly = null;
        bool passedSelf = false;
        foreach(var ally in self.GetAllies())
        {   
            if (ally == self) 
            {
                passedSelf = true;
                continue;
            }
            if (!passedSelf) continue;

            if (ally == null || ally.IsDead) continue;
            rightAlly = ally;
            return rightAlly;
        }
        return rightAlly;
    }
}
