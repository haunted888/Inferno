using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Cleric/Tutankhamun/Set")]
public class SetPassiveDefinition : PassivesDefinition
{
    public StatBoostFlatPassiveDefinition passiveToApply;
    public List<CombatSubStat> possibleStatsToBoost = 
        new() {CombatSubStat.FireDefense, CombatSubStat.IceDefense, CombatSubStat.StormDefense, CombatSubStat.BloodDefense, CombatSubStat.AcidDefense, CombatSubStat.PsychicDefense};

    public override void OnCommandPhaseStart(BattleCharacter self)
    {
        List<BattleCharacter> allies = new();
        foreach(var ally in self.GetAllies())
        {
            if(ally == null || ally.IsDead) continue;
            allies.Add(ally);
        }
        if(allies.Count == 0) return;

        BattleCharacter target = allies[Random.Range(0, allies.Count)];


        CombatStats stats = new();
        int boostAmount = self.GetSubDefenseStats()[DamageSubType.Storm];
        var statToBoost = possibleStatsToBoost[Random.Range(0, possibleStatsToBoost.Count)];
        

        switch (statToBoost)
        {
            case CombatSubStat.FireDefense:
                stats.fireDefense = boostAmount;
                break;
            case CombatSubStat.IceDefense:
                stats.iceDefense = boostAmount;
                break;
            case CombatSubStat.StormDefense:
                stats.stormDefense = boostAmount;
                break;
            case CombatSubStat.BloodDefense:
                stats.bloodDefense = boostAmount;
                break;
            case CombatSubStat.AcidDefense:
                stats.acidDefense = boostAmount;
                break;
            case CombatSubStat.PsychicDefense:
                stats.psychicDefense = boostAmount;
                break;
        }

        passiveToApply.SetStatBoosts(stats);

        target.AddPassive(passiveToApply, self);
    }
}
