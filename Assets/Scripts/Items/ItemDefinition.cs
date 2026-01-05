using UnityEngine;

public enum ItemType
{
    Equippable,
    Consumable,
    Battle,
    Scroll,
    KeyItem
}

[CreateAssetMenu(menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string displayName = "Item";
    public ItemType[] itemTypes;
    [TextArea] public string description;
    public Sprite icon;

    public ItemConsumableInMap    mapConsumable;     // already added earlier (was Map… renamed)
    public ItemConsumableInBattle battleConsumable;  // already added earlier
    public ItemHeldEquippable     heldEquippable;    // NEW: mark as equippable (held)

    [Header("Shop")]
    [Min(0)] public int price = 0;

}
