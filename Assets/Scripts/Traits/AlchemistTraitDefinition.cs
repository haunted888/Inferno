using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Alchemist Trait")]
public class AlchemistTraitDefinition : TraitDefinition
{
    public PassivesDefinition soakedFrostbitePassive;
    public PassivesDefinition soakedPoisonPassive;
    public PassivesDefinition soakedBurnPassive;
    public PassivesDefinition frostbiteBurnPassive;
    public PassivesDefinition frostbitePoisonPassive;
    public PassivesDefinition burnPoisonPassive;

    void Awake()
    {
        traitType = CharacterTrait.Alchemist;
    }

    public override void OnPassiveApplied(BattleCharacter user, PassivesDefinition passive, BattleCharacter target)
    {   
        PassivesDefinition targetStatus = target.passives.Find(p => p is StatusPassiveDefinition);
        if(targetStatus == null) return;
        if(passive is SoakedPassiveDefinition)
        {
            if(targetStatus is FrostbitePassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(soakedFrostbitePassive);
                return;
            }
            if(targetStatus is PoisonPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(soakedPoisonPassive);
                return;
            }
            if(targetStatus is BurnPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                Debug.Log("Applying soaked burn passive");
                target.AddPassive(soakedBurnPassive);
                return;
            }
        }
        if(passive is FrostbitePassiveDefinition)
        {
            if(targetStatus is BurnPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(frostbiteBurnPassive);
                return;
            }
            if(targetStatus is PoisonPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(frostbitePoisonPassive);
                return;
            }
            if(targetStatus is SoakedPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(soakedFrostbitePassive);
                return;
            }
        }
        if(passive is BurnPassiveDefinition)
        {
            if(targetStatus is PoisonPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(burnPoisonPassive);
                return;
            }
            if(targetStatus is FrostbitePassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(frostbiteBurnPassive);
                return;
            }
            if(targetStatus is SoakedPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(soakedBurnPassive);
                return;
            }
        }
        if(passive is PoisonPassiveDefinition)
        {
            if(targetStatus is FrostbitePassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(frostbitePoisonPassive);
                return;
            }
            if(targetStatus is BurnPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(burnPoisonPassive);
                return;
            }
            if(targetStatus is SoakedPassiveDefinition)
            {
                target.RemovePassive(targetStatus);
                target.AddPassive(soakedPoisonPassive);
                return;
            }
        }
    }
}
