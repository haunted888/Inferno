using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtility
{
    public static int[] splitInt(int value, int numSplit, int min = 0, int max = int.MaxValue)
    {
        if(min > value/numSplit) min = Mathf.FloorToInt(value/numSplit);
        if (max < value/numSplit) max = Mathf.CeilToInt(value/numSplit);

        int[] returnArray = new int[numSplit];
        List<int> indexArray = new List<int>();
        for(int i = 0; i < numSplit; i++)
        {
            //setup index array
            indexArray.Add(i);
        
            //put in min values in return array
            returnArray[i] = min;
            value -= min;
        }
        


        for(int i = 0; i < value; i++)
        {
            int randomIndex = Mathf.FloorToInt(Random.Range(0, indexArray.Count));
            returnArray[indexArray[randomIndex]] += 1;
            if(returnArray[indexArray[randomIndex]] + 1 > max) indexArray.RemoveAt(randomIndex);
        }
        return returnArray;
    }

    public static CombatStats CombatStatsSum(CombatStats a, CombatStats b)
    {
        return new CombatStats
        {
            maxHealth = a.maxHealth + b.maxHealth,
            maxSp = a.maxSp + b.maxSp,
            spGeneration = a.spGeneration + b.spGeneration,
            speed = a.speed + b.speed,
            physicalAttack = a.physicalAttack + b.physicalAttack,
            elementalPower = a.elementalPower + b.elementalPower,
            defense = a.defense + b.defense,
            elementalResistance = a.elementalResistance + b.elementalResistance,
            critChance = a.critChance + b.critChance,
            critDamage = a.critDamage + b.critDamage,
            accuracy = a.accuracy + b.accuracy,
            evasion = a.evasion + b.evasion,

            bludgeoningAttack = a.bludgeoningAttack + b.bludgeoningAttack,
            slashingAttack = a.slashingAttack + b.slashingAttack,
            piercingAttack = a.piercingAttack + b.piercingAttack,

            bludgeoningDefense = a.bludgeoningDefense + b.bludgeoningDefense,
            slashingDefense = a.slashingDefense + b.slashingDefense,
            piercingDefense = a.piercingDefense + b.piercingDefense,

            fireAttack = a.fireAttack + b.fireAttack,
            iceAttack = a.iceAttack + b.iceAttack,
            stormAttack = a.stormAttack + b.stormAttack,
            acidAttack = a.acidAttack + b.acidAttack,
            psychicAttack = a.psychicAttack + b.psychicAttack,
            bloodAttack = a.bloodAttack + b.bloodAttack,

            fireDefense = a.fireDefense + b.fireDefense,
            iceDefense = a.iceDefense + b.iceDefense,
            stormDefense = a.stormDefense + b.stormDefense,
            acidDefense = a.acidDefense + b.acidDefense,
            psychicDefense = a.psychicDefense + b.psychicDefense,
            bloodDefense = a.bloodDefense + b.bloodDefense,

        };
    }
}
