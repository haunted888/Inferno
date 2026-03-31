// HealAllAlliesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal All Allies")]
public class HealAllAlliesSkill : Skill
{
    [Header("Heal ALL Skill")]
    public int healAmount = 10;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (user == null) return;

        var group = user.GetAllies();

        foreach (var ally in group)
        {
            if (ally == null || ally.IsDead) continue;
            ally.Heal(healAmount);
        }

        ExecuteFollowUps(user, target);
    }

}
