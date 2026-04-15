using System;
using UnityEngine;
using System.Collections.Generic;


public class BattleSlots : MonoBehaviour
{
    public Transform playerSlotsParent;
    public Transform enemySlotsParent;
    public Transform playerSummonerSlotsParent;
    public Transform enemySummonerSlotsParent;

    private Transform[] playerSlots;
    private Transform[] enemySlots;
    private Transform[] playerSummonerSlots;
    private Transform[] enemySummonerSlots;

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

        int childCountPS = playerSummonerSlotsParent.childCount;
        playerSummonerSlots = new Transform[childCountPS];
        for (int i = 0; i < childCountPS; i++)
            playerSummonerSlots[i] = playerSummonerSlotsParent.GetChild(i);

        int childCountES = enemySummonerSlotsParent.childCount;
        enemySummonerSlots = new Transform[childCountES];
        for (int i = 0; i < childCountES; i++)
            enemySummonerSlots[i] = enemySummonerSlotsParent.GetChild(i);
    }

    public Transform[] GetSlots(bool isPlayerSide, BattleCharacter[] characters, int minGap = 1)
    {
        if (characters == null || characters.Length == 0)
            return new Transform[0];

        Transform[] normalSlots = isPlayerSide ? playerSlots : enemySlots;
        Transform[] summonerSlots = isPlayerSide ? playerSummonerSlots : enemySummonerSlots;

        int[] slotIndices = GetSlotIndicesForCharacters(isPlayerSide, characters, minGap);

        Transform[] result = new Transform[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            var chr = characters[i];
            if (chr == null) continue;

            int slotIndex = slotIndices[i];
            if (slotIndex < 0) continue;

            bool goesToSummonerRow = chr.HasLivingSummon() && !chr.hideWhileSummonIsAlive;

            Transform[] sourceSlots = goesToSummonerRow ? summonerSlots : normalSlots;
            if (sourceSlots == null || slotIndex >= sourceSlots.Length) continue;

            result[i] = sourceSlots[slotIndex];
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

        int[] result = new int[n];
        for (int i = 0; i < n; i++)
            result[i] = -1;

        List<BattleCharacter> frontlineCharacters = new List<BattleCharacter>(n);
        List<int> frontlineOriginalIndices = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var chr = characters[i];
            if (chr == null) continue;

            // Visible summoners do not occupy the frontline row.
            if (chr.HasLivingSummon() && !chr.hideWhileSummonIsAlive)
                continue;

            frontlineCharacters.Add(chr);
            frontlineOriginalIndices.Add(i);
        }

        if (frontlineCharacters.Count == 0)
            return result;

        int frontlineCount = frontlineCharacters.Count;

        double[] centers = new double[frontlineCount];
        int[] minDists = new int[frontlineCount];

        centers[0] = 0.0;
        minDists[0] = 0;

        for (int i = 1; i < frontlineCount; i++)
        {
            int prevSize = Mathf.Max(1, frontlineCharacters[i - 1].slotSize);
            int curSize  = Mathf.Max(1, frontlineCharacters[i].slotSize);

            int minDist = Mathf.CeilToInt((prevSize + curSize) / 2f) + minGap;
            centers[i] = centers[i - 1] + minDist;
            minDists[i] = minDist;
        }

        float firstHalf = (Mathf.Max(1, frontlineCharacters[0].slotSize) - 1) / 2f;
        float lastHalf  = (Mathf.Max(1, frontlineCharacters[frontlineCount - 1].slotSize) - 1) / 2f;

        double estMin   = centers[0] - firstHalf;
        double estMax   = centers[frontlineCount - 1] + lastHalf;
        double estWidth = estMax - estMin + 1.0;

        if (estWidth > totalSlots)
        {
            Debug.LogError("Not enough slots for requested formation.");
            return result;
        }

        double currentCenter = (centers[0] + centers[frontlineCount - 1]) / 2.0;
        double targetCenter  = (totalSlots - 1) / 2.0;
        double offset        = targetCenter - currentCenter;

        int[] frontlineSlotIndices = new int[frontlineCount];

        double firstShifted = centers[0] + offset;
        int idx0 = (int)System.Math.Round(firstShifted, System.MidpointRounding.ToEven);
        frontlineSlotIndices[0] = Mathf.Clamp(idx0, 0, totalSlots - 1);

        for (int i = 1; i < frontlineCount; i++)
        {
            double desired = centers[i] + offset;
            int baseIdx = (int)System.Math.Round(desired, System.MidpointRounding.ToEven);

            int minIdx = frontlineSlotIndices[i - 1] + minDists[i];
            int idx = Mathf.Max(baseIdx, minIdx);

            frontlineSlotIndices[i] = Mathf.Clamp(idx, 0, totalSlots - 1);
        }

        for (int i = 0; i < frontlineCount; i++)
        {
            int originalIndex = frontlineOriginalIndices[i];
            result[originalIndex] = frontlineSlotIndices[i];
        }

        // Visible summoners take the same slot index as their summon, but on the summoner row.
        for (int i = 0; i < n; i++)
        {
            var chr = characters[i];
            if (chr == null) continue;

            if (!(chr.HasLivingSummon() && !chr.hideWhileSummonIsAlive))
                continue;

            var summon = chr.activeSummon;
            if (summon == null) continue;

            for (int j = 0; j < n; j++)
            {
                if (characters[j] == summon)
                {
                    result[i] = result[j];
                    break;
                }
            }
        }

        return result;
    }

    public Transform[] GetRawSlots(bool isPlayerSide)
    {
        return isPlayerSide ? playerSlots : enemySlots;
    }

    public Transform[] GetRawSummonerSlots(bool isPlayerSide)
    {
        return isPlayerSide ? playerSummonerSlots : enemySummonerSlots;
    }

    public int GetClosestSlotIndex(bool isPlayerSide, Vector3 position)
    {
        Transform[] slots = GetRawSlots(isPlayerSide);
        if (slots == null || slots.Length == 0)
            return -1;

        int bestIndex = 0;
        float bestDistance = float.PositiveInfinity;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            float dist = (slots[i].position - position).sqrMagnitude;
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

}
