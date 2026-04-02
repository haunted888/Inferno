using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleUtility
{
    public static List<BattleCharacter> GetTargetsForSkill(Skill skill, BattleCharacter user, BattleCharacter target)
    {
        if (skill == null || user == null) return new List<BattleCharacter>();

        IEnumerable<BattleCharacter> targetsEnum;
        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                targetsEnum = user.GetEnemies();
                break;
            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
                targetsEnum = user.GetAllies();
                break;
            case SkillTargetType.Self:
                targetsEnum = new List<BattleCharacter> { user };
                break;
            default:
                targetsEnum = new List<BattleCharacter> { target };
                break;
        }

        foreach (var followUpSkill in skill.followUpSkills)
        {
            if (followUpSkill == null) continue;
            var followUpTargets = GetTargetsForSkill(followUpSkill, user, target);
            targetsEnum = targetsEnum.Union(followUpTargets).ToList();
        }

        var candidates = new List<BattleCharacter>();
        foreach (var c in targetsEnum)
            if (c != null && !c.IsDead) candidates.Add(c);
        return candidates;
    }
}
