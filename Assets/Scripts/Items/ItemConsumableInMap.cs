using System;
using UnityEngine;

public abstract class ItemConsumableInMap : ScriptableObject
{
    // Called when player clicks the item in Inventory
    // You should: open camp (if closed) and request a member selection.
    public abstract void BeginUseOnMap(ItemDefinition item, InventoryUI invUI);
}
