using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Skills/Damage Skill")]
public class DamageSkill : Skill
{
    public int power = 10;

    public override int EstimateDamage(BattleCharacter user, BattleCharacter target)
    {
        return power;
    }

    public override void Execute(BattleCharacter user, BattleCharacter target)
    {
        if (target == null || target.IsDead) return;

        int dealt = target.TakeDamage(power);
        BattleTurnManager.Instance?.RegisterDamage(user, target, dealt);

        Debug.Log($"{user.name} used {skillName} on {target.name} for {dealt} damage.");
    }
}
