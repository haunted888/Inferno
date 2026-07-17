using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BattleUtility
{

    private static readonly List<Skill> skillsLooped = new List<Skill>(); 

    public static List<BattleCharacter> GetTargetsForSkill(Skill skill, BattleCharacter user, BattleCharacter target)
    {
        skillsLooped.Clear();
        return GetTargets(skill, user, target);

    }

    private static List<BattleCharacter> GetTargets(Skill skill, BattleCharacter user, BattleCharacter target)
    {
        if (skill == null || user == null) return new List<BattleCharacter>();

        IEnumerable<BattleCharacter> targetsEnum;
        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.SingleTarget:
            case SkillTargetType.SingleAlly:
            case SkillTargetType.SingleTargetNoSelf:
                targetsEnum = new List<BattleCharacter> { target };
                break;
            case SkillTargetType.AllEnemies:
                targetsEnum = user.GetEnemies();
                break;
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
            if (skillsLooped.Contains(followUpSkill)) continue;
            skillsLooped.Add(followUpSkill);
            var followUpTargets = GetTargets(followUpSkill, user, target);
            targetsEnum = targetsEnum.Union(followUpTargets).ToList();
        }

        var candidates = new List<BattleCharacter>();
        foreach (var c in targetsEnum)
        {
            if (c == null || c.IsDead) continue;
            if (c.HasLivingSummon()) continue;
            if (c.IsProtectedFromSkills()) continue;
            candidates.Add(c);
        }
        return candidates;
    }


    public static List<BattleCharacter> GetTargetsForEffectsCharacters(
        Skill.AffectsCharacters characters,
        BattleCharacter user,
        BattleCharacter target,
        Skill skill)
    {
        if (user == null) return new List<BattleCharacter>();

        IEnumerable<BattleCharacter> targetsEnum;
        switch (characters)
        {
            case Skill.AffectsCharacters.Target:
                targetsEnum = new List<BattleCharacter> { target };
                break;
            case Skill.AffectsCharacters.TargetTeam:
                targetsEnum = target != null ? target.GetAllies() : new List<BattleCharacter>();
                break;
            case Skill.AffectsCharacters.Self:
                targetsEnum = new List<BattleCharacter> { user };
                break;
            case Skill.AffectsCharacters.Allies:
                targetsEnum = user.GetAllies();
                break;
            case Skill.AffectsCharacters.Enemies:
                targetsEnum = user.GetEnemies();
                break;
            case Skill.AffectsCharacters.AllOtherAllies:
                targetsEnum = user.GetAllies().Where(c => c != user);
                break;
            default:
                targetsEnum = new List<BattleCharacter>();
                break;
        }

        var candidates = new List<BattleCharacter>();
        foreach (var c in targetsEnum)
        {
            if (c == null || c.IsDead) continue;
            if (c.HasLivingSummon()) continue;
            if (c.IsProtectedFromSkills()) continue;
            if (c.IsDodging() && !skill.BypassAccuracy) continue; 
            candidates.Add(c);
        }

        return candidates;
    }


    public static bool IsTargetValidForSkill(
        Skill skill,
        BattleCharacter user,
        BattleCharacter target)
    {
        if (skill == null || user == null || target == null) return false;
        if (!CanReceiveSkill(target)) return false;

        bool targetIsAlly = user.GetAllies().Contains(target);
        bool targetIsEnemy = user.GetEnemies().Contains(target);
        bool targetIsSelf = user == target;

        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                return targetIsEnemy;

            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
                return targetIsAlly;

            case SkillTargetType.Self:
                return targetIsSelf;
            case SkillTargetType.SingleTarget:
                return targetIsAlly || targetIsEnemy || targetIsSelf;
            case SkillTargetType.SingleTargetNoSelf:
                return (targetIsAlly || targetIsEnemy) && !targetIsSelf;
            default:
                return false;
        }
    }

    public static BattleCharacter GetRandomSelectableTarget(
        Skill skill,
        BattleCharacter user,
        bool prioritizeEnemies = true)
    {
        if (skill == null || user == null) return null;

        IEnumerable<BattleCharacter> candidatesEnum;

        switch (skill.targetType)
        {
            case SkillTargetType.SingleEnemy:
            case SkillTargetType.AllEnemies:
                candidatesEnum = user.GetEnemies();
                break;

            case SkillTargetType.SingleAlly:
            case SkillTargetType.AllAllies:
                candidatesEnum = user.GetAllies();
                break;

            case SkillTargetType.Self:
                candidatesEnum = new List<BattleCharacter> { user };
                break;

            default:
                return null;
        }

        var candidates = new List<BattleCharacter>();

        foreach (var candidate in candidatesEnum)
        {
            if (IsTargetValidForSkill(skill, user, candidate))
                candidates.Add(candidate);
        }

        if (candidates.Count == 0)
            return null;
        
        if (prioritizeEnemies)
        {
            var enemyCandidates = candidates.Where(c => user.GetEnemies().Contains(c)).ToList();
            if (enemyCandidates.Count > 0)
                candidates = enemyCandidates;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public static bool CanReceiveSkill(BattleCharacter target, bool ignoreDeath = false, bool ignoreSummons = false, bool ignoreProtection = false)
    {
        if (target == null) return false;
        if (!ignoreDeath && target.IsDead) return false;
        if (!ignoreSummons && target.HasLivingSummon()) return false;
        if (!ignoreProtection && target.IsProtectedFromSkills()) return false;

        return true;
    }
}
