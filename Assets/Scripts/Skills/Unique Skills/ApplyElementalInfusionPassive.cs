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
        var targets = BattleUtility.GetTargetsForEffectsCharacters(characters, user, target, this);

        var elementalSubTypes = new List<DamageSubType> { DamageSubType.Fire, DamageSubType.Storm, DamageSubType.Ice, DamageSubType.Blood, DamageSubType.Psychic, DamageSubType.Acid };

        foreach (var t in targets)
        {
            if (t == null || t.IsDead) continue;

            BeforeSkillExecute(user, t);

            Dictionary<DamageSubType, int> subTypeCounts = user.GetSubAttackStats();
            DamageSubType subType = DamageSubType.None;
            int highestCount = 0;
            foreach (var kvp in subTypeCounts)
            {
                if (kvp.Value >= highestCount && elementalSubTypes.Contains(kvp.Key))
                {
                    highestCount = kvp.Value;
                    subType = kvp.Key;
                }
            }
            passiveToApply.conversionType = subType;

            string elementName = subType.ToString();

            passiveToApply.displayName = "Elemental Infusion: " + elementName;
            passiveToApply.description = "Infuses the target's physical attacks with " + elementName + " damage.";

            t.AddPassive(passiveToApply, user);
            EndExecution();
        }

        ExecuteFollowUps(user, target);
        EndExecution();

        
    }
}
