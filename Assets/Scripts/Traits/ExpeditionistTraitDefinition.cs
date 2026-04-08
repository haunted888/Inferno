using System;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Expeditionist")]
public class ExpeditionistTraitDefinition : TraitDefinition
{
    //NOTE: Expedition rewards applied at the beginning of combat, may want to change to end later for game design purposes. If done, also change Vici passive.
    [Header("Expedition Rewards")]
    public List<MapRewardDefinition> expeditionRewards;

    void Awake()
    {
        traitType = CharacterTrait.Expeditionist;
    }

    public override void OnBattleStart(BattleCharacter user)
    {
        addReward();
    }

    public void addReward()
    {
        if(BattleTurnManager.Instance == null) return;
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
