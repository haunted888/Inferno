using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEditor.UI;

[CreateAssetMenu(menuName = "Skills/Redirect")]
public class RedirectSkill : Skill
{
    public AffectsCharacters characters;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        BeforeSkillExecute(user, target);

        var targets = BattleUtility.GetTargetsForEffectsCharacters(characters, user, target, this);

        

        var currentActionOrder = user.GetCurrentActionOrder();

        foreach (var a in currentActionOrder)
        {

            if (targets.Contains(a.user))
            {
                a.target = user;
            }
        }

        ExecuteFollowUps(user, target);
        EndExecution();
    }
}
