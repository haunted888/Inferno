using UnityEngine;

public abstract class StatusPassiveDefinition : PassivesDefinition
{
    public StatusBufferPassiveDefinition statusBufferPrefab;

    //Status effect methods
    public bool CharHasStatus(BattleCharacter self)
    {
        
        foreach(var passive in self.passives)
        {
            if(passive.type == PassivesTypes.StatusEffect && passive != this)
            {
                if(passive.GetType() == GetType()) continue;
                
                Debug.Log("Removing existing steamed passive");
                self.RemovePassive(this);
                return true;
            }
        }

        return false;
    }

    public void ApplyStatusBuffer(BattleCharacter self)
    {
        Debug.Log($"{statusBufferPrefab.displayName} has been applied to {self.name}.");
        self.AddPassive(statusBufferPrefab);

    }

}
