using System;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Expeditionist")]
public class ExpeditionistTraitDefinition : TraitDefinition
{

    [Header("Expedition Rewards")]
    public List<MapRewardDefinition> expeditionRewards;

    void Awake()
    {
        traitType = CharacterTrait.Expeditionist;
    }

    public override void OnBattleStart(BattleCharacter user)
    {
        MapRewardDefinition chosen = null;
        int total = 0;

        foreach (var reward in expeditionRewards)
        {
            total += reward.expeditionistQuantity;
        }

        int roll = UnityEngine.Random.Range(0, total);
        int cumulative = 0;
        foreach (var reward in expeditionRewards)
        {
            cumulative += reward.expeditionistQuantity;
            if (roll < cumulative)
            {
                chosen = reward;
                break;
            }
        }

        if (chosen != null)
        MapCombatTransfer.Instance.AddToPendingRewards(chosen);
    }
}
