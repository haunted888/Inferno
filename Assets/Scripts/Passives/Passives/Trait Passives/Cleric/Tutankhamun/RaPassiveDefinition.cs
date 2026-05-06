using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Cleric/Tutankhamun/Ra")]
public class RaPassiveDefinition : PassivesDefinition
{
    public float damagePercent = .1f;

    public override void OnResolvePhaseEnd(BattleCharacter self)
    {
        List<BattleCharacter> enemies = new();
        foreach(var enemy in self.GetEnemies())
        {
            if(enemy == null || enemy.IsDead) continue;
            enemies.Add(enemy);
        }
        if(enemies.Count == 0) return;

        BattleCharacter target = enemies[Random.Range(0, enemies.Count)];
        target.TakeDamage(Mathf.CeilToInt(self.GetSubAttackStats()[DamageSubType.Fire] * damagePercent), SkillDamageType.Elemental, DamageSubType.Fire);

    }
}
