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
            indexArray[i] = i;
        
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
}
