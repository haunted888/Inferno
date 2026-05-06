using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Skills/Unique/Apply Elemental Infusion Passive")]
public class ApplyElementalInfusionPassive : Skill
{
    public ElementalInfusionPassive passiveToApply;
    public AffectsCharacters characters;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        var targets = BattleUtility.GetTargetsForEffectsCharacters(characters, user, target);



        foreach (var t in targets)
        {
            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            DamageSubType subType = DamageSubType.None;
            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value > highestCount)
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                }
            }
            passiveToApply.conversionType = subType;

            String elementName = subType.ToString();

            passiveToApply.displayName = "Elemental Infusion: " + elementName;
            passiveToApply.description = "Infuses the target's physical attacks with " + elementName + " damage.";

            t.AddPassive(passiveToApply, user);
        }

        ExecuteFollowUps(user, target);
        EndExecution();

        
    }
}
