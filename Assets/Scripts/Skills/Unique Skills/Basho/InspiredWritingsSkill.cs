using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Skills/Basho/Inspired Writings")]
public class InspiredWritings : Skill
{
    
    public List<Skill> uncopiableSkills = new List<Skill>();

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null || target == null)
        {
            EndExecution();
            return;
        }

        BeforeSkillExecute(user, target);

        if (target.lastUsedSkill == null || uncopiableSkills.Contains(target.lastUsedSkill) || target.lastUsedSkill == this)
        {
            combatText = "But it failed!";
            EndExecution();
            return;
        }

        var skillToCopy = target.lastUsedSkill;

        skillToCopy.InstantiateDetailShells();

        skillToCopy.skillDetailShell.ClearCosts();

        var action = new QueuedAction
        {
            user = user,
            target = BattleUtility.GetRandomSelectableTarget(skillToCopy, user),
            skill = skillToCopy,
        };

        user.GetCurrentActionOrderMutable().Insert(0, action);

        combatText = $"Copied {target.name}'s {skillToCopy.skillName}!";

        Debug.Log($"{user.name} copied {target.name}'s {skillToCopy.skillName} using Inspired Writings! Costing: {skillToCopy.skillDetailShell.spCost}");

        ExecuteFollowUps(user, target);

        EndExecution();
    }
}
