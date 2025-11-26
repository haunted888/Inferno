using UnityEngine;

[CreateAssetMenu(menuName = "Items/Battle Consumable/Health Potion (Full Heal)")]
public class HealthPotionBattle : ItemConsumableInBattle
{
    public override bool RequiresTarget => false; // self-use, no targeting step

    public override void Execute(BattleCharacter user, BattleCharacter target, ItemDefinition item)
    {
        var t = user;                           // always self
        if (t == null || t.IsDead) return;

        int missing = t.MaxHealth - t.CurrentHealth;
        if (missing <= 0) return;

        t.Heal(missing);
        MapCombatTransfer.Instance?.RemoveItem(item, 1);
    }
}
