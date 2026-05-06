using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Change Damage To Passive Counter")]
public class ChangeDamageToPassiveCounter : Skill
{
    public PassivesDefinition passiveCounterToUse;

    public DamageSkillParent damageSkillToUse;

    public AffectsCharacters characters;


    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null || target.IsDead) return;
        if (damageSkillToUse == null || passiveCounterToUse == null) return;

        var targets = BattleUtility.GetTargetsForEffectsCharacters(characters, user, target);


        int damage = 0;

        foreach (var t in targets)
        {
            if (t == null || t.IsDead) continue;

            foreach (var passive in t.passives)
            {
                if (passive.displayName == passiveCounterToUse.displayName)
                {
                    damage += passive.counter;
                    break;
                }
            }
        }

        var skillTargetDetailShell = damageSkillToUse.skillDetailShell as DamageSkillParent;

        skillTargetDetailShell.power = damage;

        ExecuteFollowUps(user, target);

        EndExecution();
    }
}
