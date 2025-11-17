using System;
using UnityEngine;

public class BattleSlots : MonoBehaviour
{
    public Transform playerSlotsParent;
    public Transform enemySlotsParent;

    private Transform[] playerSlots;
    private Transform[] enemySlots;
    // Returns symmetric slot indices with one empty slot between characters.
    // Example: total=7, count=3 -> [1,3,5]
    void Awake()
    {
        // Get all slot children in order
        int childCountP = playerSlotsParent.childCount;
        playerSlots = new Transform[childCountP];
        for (int i = 0; i < childCountP; i++)
            playerSlots[i] = playerSlotsParent.GetChild(i);

        int childCountE = enemySlotsParent.childCount;
        enemySlots = new Transform[childCountE];
        for (int i = 0; i < childCountE; i++)
            enemySlots[i] = enemySlotsParent.GetChild(i);
    }

    public Transform[] GetSlots(bool isPlayerSide, BattleCharacter[] characters, int minGap = 1)
    {
        int[] indices = GetSlotIndicesForCharacters(isPlayerSide, characters, minGap);
        Transform[] slots = isPlayerSide ? playerSlots : enemySlots;
        Transform[] result = new Transform[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            result[i] = slots[indices[i]];
        }
        return result;
    }

    //If you want to change the code is: “Let’s center using outer-edges instead of centers,”
    public int[] GetSlotIndicesForCharacters(bool isPlayerSide, BattleCharacter[] characters, int minGap = 1)
    {
        if (characters == null || characters.Length == 0)
            return new int[0];

        Transform[] slots = isPlayerSide ? playerSlots : enemySlots;
        if (slots == null || slots.Length == 0)
            return new int[0];

        int totalSlots = slots.Length;
        int n = characters.Length;

        // Logical (floating) centers, spaced by size + minGap
        double[] centers = new double[n];
        // Minimum required center-to-center distance per pair
        int[] minDists = new int[n];

        centers[0] = 0.0;
        minDists[0] = 0;

        for (int i = 1; i < n; i++)
        {
            int prevSize = Mathf.Max(1, characters[i - 1].slotSize);
            int curSize  = Mathf.Max(1, characters[i].slotSize);

            // Minimum center distance so their occupied slot ranges are separated
            // by at least minGap free slots.
            int minDist = Mathf.CeilToInt((prevSize + curSize) / 2f) + minGap;
            centers[i] = centers[i - 1] + minDist;
            minDists[i] = minDist;
        }

        // Estimate total width in slots the formation would occupy
        float firstHalf = (Mathf.Max(1, characters[0].slotSize)     - 1) / 2f;
        float lastHalf  = (Mathf.Max(1, characters[n - 1].slotSize) - 1) / 2f;

        double estMin   = centers[0]      - firstHalf;
        double estMax   = centers[n - 1]  + lastHalf;
        double estWidth = estMax - estMin + 1.0;

        if (estWidth > totalSlots)
        {
            Debug.LogError("Not enough slots for requested formation.");
            return new int[0];
        }

        // Shift the whole set of centers so that the group is centered in the slot line
        double currentCenter = (centers[0] + centers[n - 1]) / 2.0;
        double targetCenter  = (totalSlots - 1) / 2.0;
        double offset        = targetCenter - currentCenter;

        int[] indices = new int[n];

        // First index: centered and rounded
        double firstShifted = centers[0] + offset;
        int idx0 = (int)System.Math.Round(firstShifted, System.MidpointRounding.ToEven);
        indices[0] = Mathf.Clamp(idx0, 0, totalSlots - 1);

        // Subsequent indices: respect both desired center and min distances
        for (int i = 1; i < n; i++)
        {
            double desired = centers[i] + offset;
            int baseIdx = (int)System.Math.Round(desired, System.MidpointRounding.ToEven);

            int minIdx = indices[i - 1] + minDists[i];
            int idx = Mathf.Max(baseIdx, minIdx);

            indices[i] = Mathf.Clamp(idx, 0, totalSlots - 1);
        }
        Debug.Log("Slot indices: " + string.Join(",", indices));
        return indices;
    }

}
