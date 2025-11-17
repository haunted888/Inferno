// DamageAllEnemiesSkill.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Damage All Enemies")]
public class DamageAllEnemiesSkill : Skill
{
    public int power = 10;

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        return power;
    }

    public override void Execute(BattleCharacter user, BattleCharacter _)
    {
        if (BattleTurnManager.Instance == null) return;

        foreach (var enemy in BattleTurnManager.Instance.GetEnemiesOf(user))
        {
            if (enemy == null || enemy.IsDead) continue;

            int dealt = enemy.TakeDamage(power);
            BattleTurnManager.Instance.RegisterDamage(user, enemy, dealt);
        }

        Debug.Log($"{user.name} used {skillName} on all enemies for {power} damage each.");
    }
}
