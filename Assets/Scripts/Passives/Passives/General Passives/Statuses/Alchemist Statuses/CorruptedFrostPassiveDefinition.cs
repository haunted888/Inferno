using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Statuses/Alchemist/Corrupted Frost Passive")]
public class CorruptedFrostPassiveDefinition : StatusPassiveDefinition
{
    public int counter = 0;
  

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        if (self == null) return;

        bool counterIncrement = UnityEngine.Random.value >= 0.5f;

        if(counterIncrement)
            counter++;
        else
            counter = Math.Max(0, counter-1);

        
        SetDisplayText($"{self.name}'s corrupted frost counter has {(counterIncrement ? "increased" : "decreased")} to {counter}.");

        if(counter >= 3)
        {
            self.Die();
            SetDisplayText($"{self.name} has died from their corrupted frost.");
        }
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        ApplyStatusBuffer(self);
    }
}
