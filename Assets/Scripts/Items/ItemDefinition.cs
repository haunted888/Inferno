using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string displayName = "Item";
    [TextArea] public string description;
    public Sprite icon;

    public ItemConsumableInMap    mapConsumable;     // already added earlier (was Map… renamed)
    public ItemConsumableInBattle battleConsumable;  // already added earlier
    public ItemHeldEquippable     heldEquippable;    // NEW: mark as equippable (held)
}
