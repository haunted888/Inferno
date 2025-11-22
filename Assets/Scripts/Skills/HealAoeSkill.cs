// HealAllAlliesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Heal All Allies")]
public class HealAllAlliesSkill : Skill
{
    public int healAmount = 10;

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (BattleTurnManager.Instance == null || user == null) return;

        var center = target != null ? target : user;
        var group = BattleTurnManager.Instance.GetAlliesOf(center);

        foreach (var ally in group)
        {
            if (ally == null || ally.IsDead) continue;
            ally.Heal(healAmount);
        }

        ExecuteFollowUps(user, target);
    }

}
