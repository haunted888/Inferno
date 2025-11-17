// HealAllAlliesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Heal All Allies")]
public class HealAllAlliesSkill : Skill
{
    public int healAmount = 10;

    public override void Execute(BattleCharacter user, BattleCharacter _)
    {
        if (BattleTurnManager.Instance == null) return;

        foreach (var ally in BattleTurnManager.Instance.GetAlliesOf(user))
        {
            if (ally == null || ally.IsDead) continue;
            ally.Heal(healAmount);
        }

        Debug.Log($"{user.name} used {skillName} to heal all allies by {healAmount}.");
    }
}
