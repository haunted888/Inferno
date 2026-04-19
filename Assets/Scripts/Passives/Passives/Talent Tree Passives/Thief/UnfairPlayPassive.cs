using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Thief/Unfair Play")]
public class UnfairPlayPassive : PassivesDefinition
{
    public TempStatBoostPercent passiveToApply;

    public override void OnSkillUsed(BattleCharacter self, BattleCharacter target, Skill skill)
    {
        if (skill == null) return;
        if (skill.targetType == SkillTargetType.AllEnemies 
            || skill.targetType == SkillTargetType.AllAllies)
        {
            return;
        }
        if (self == target) return;

        target.AddPassive(passiveToApply, self);
    }
}
