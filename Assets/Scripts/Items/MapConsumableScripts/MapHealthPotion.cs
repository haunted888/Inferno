using UnityEngine;

[CreateAssetMenu(menuName = "Items/Map Consumable/Health Potion (Full Heal)")]
public class HealthPotionMap : ItemConsumableInMap
{
    public override void BeginUseOnMap(ItemDefinition item, InventoryUI invUI)
    {
        var camp = FindFirstObjectByType<CampUIManager>();
        if (camp == null) return;

        // Enter one-click item targeting mode
        camp.BeginItemTargeting(member =>
        {
            if (member == null) return;
            
            if (member.health <= 0) return; // can't heal dead
            if (member.health >= member.GetMaxHealth()) return; // already full
            
            // Full heal
            member.health = member.GetMaxHealth();

            // Consume 1 from inventory
            MapCombatTransfer.Instance.RemoveItem(item, 1);

            // Exit targeting + refresh inventory UI
            camp.CancelItemTargeting();
            invUI.Refresh();
        });
    }

}
