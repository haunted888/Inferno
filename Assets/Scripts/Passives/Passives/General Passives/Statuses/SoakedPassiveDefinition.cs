using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Soaked")]
public class SoakedPassiveDefinition : StatusPassiveDefinition
{
    public float soakMultiplier = .75f;

    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;

        bool removeThisSoaked = false;
        foreach(var passive in self.passives)
        {
            if(passive is SoakedPassiveDefinition && passive != this)
            {
                removeThisSoaked = true;
            }
        }
        if (removeThisSoaked)
        {
            self.RemovePassive(this);
        }
        self.IsSoaked = true;
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        self.IsSoaked = false;
        ApplyStatusBuffer(self);
    }


    public float GetSoakMultiplier()
    {
        return soakMultiplier;
    }

    
}
