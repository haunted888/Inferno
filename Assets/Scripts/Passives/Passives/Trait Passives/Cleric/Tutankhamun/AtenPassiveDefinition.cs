using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Cleric/Tutankhamun/Aten")]
public class AtenPassiveDefinition : PassivesDefinition
{

    public BurnPassiveDefinition burnPassive;
    public int numberOfBurnsToApply = 10;
    
    public override void OnBattleStart(BattleCharacter self)
    {
        var cleric = self.Traits.Find(t => t is ClericTraitDefinition) as ClericTraitDefinition;
        if (cleric == null) return;

        var subTypeValues = cleric.GetSubTypeValues(self);

        
        List<DamageSubType>[] highestSubTypes = { new(), new() };
        int[] highestValues = {int.MinValue, int.MinValue};

        foreach(var kvp in subTypeValues)
        {
            if(kvp.Key == DamageSubType.Psychic) continue;
            if(kvp.Value > highestValues[0])
            {
                highestValues[1] = highestValues[0];
                highestValues[0] = kvp.Value;
                highestSubTypes[1] = new List<DamageSubType>(highestSubTypes[0]);
                highestSubTypes[0] = new List<DamageSubType>() { kvp.Key };
            }
            else if(kvp.Value == highestValues[0])
            {
                highestSubTypes[0].Add(kvp.Key);
            }
            else if(kvp.Value > highestValues[1])
            {
                highestValues[1] = kvp.Value;
                highestSubTypes[1] = new List<DamageSubType>() { kvp.Key };
            }
            else if(kvp.Value == highestValues[1])
            {
                highestSubTypes[1].Add(kvp.Key);
            }
        }

        DamageSubType chosenSubTypeOne;
        DamageSubType chosenSubTypeTwo;

        if(highestSubTypes[0].Count > 1)
        {
            int index = Random.Range(0, highestSubTypes[0].Count);
            chosenSubTypeOne = highestSubTypes[0][index];
            chosenSubTypeTwo = highestSubTypes[0][(index + 1) % highestSubTypes[0].Count];
        } else 
        {
            chosenSubTypeOne = highestSubTypes[0][0];
            chosenSubTypeTwo = highestSubTypes[1][Random.Range(0, highestSubTypes[1].Count)];
            
        }

        var godOnePassive = cleric.GetPassiveForSubType(chosenSubTypeOne);
        var godTwoPassive = cleric.GetPassiveForSubType(chosenSubTypeTwo);

        self.QueuePassiveToAdd(godOnePassive, PassiveHook.OnBattleStart);
        self.QueuePassiveToAdd(godTwoPassive, PassiveHook.OnBattleStart);
        
        var burnPassiveInstance = Instantiate(burnPassive);
        burnPassiveInstance.counter = numberOfBurnsToApply;


        self.QueuePassiveToAdd(burnPassiveInstance, PassiveHook.OnBattleStart);
    }
}
