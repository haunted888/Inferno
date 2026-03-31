// HealAllySkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal Ally")]
public class HealAllySkill : Skill
{
    [Header("Heal Skill")]
    public int healAmount = 15;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (target == null || target.IsDead) return;

        target.Heal(healAmount);

        ExecuteFollowUps(user, target);
    }
}

