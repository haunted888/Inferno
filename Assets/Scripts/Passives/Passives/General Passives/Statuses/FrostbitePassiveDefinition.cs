using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Frostbite")]
public class FrostbitePassiveDefinition : StatusPassiveDefinition
{

    public override void OnCreated(BattleCharacter self)
    {
        if(CharHasStatus(self)) return;

        bool removeThisFrostbite = false;
        foreach(var passive in self.passives)
        {
            if(passive is FrostbitePassiveDefinition && passive != this)
            {
                removeThisFrostbite = true;
            }
        }
        if (removeThisFrostbite)
        {
            self.RemovePassive(this);
        }
        self.IsFrostbitten = true;
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        self.IsFrostbitten = false;
        ApplyStatusBuffer(self);
    }



    
}
