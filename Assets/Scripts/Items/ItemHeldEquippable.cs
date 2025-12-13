using UnityEngine;

public abstract class ItemHeldEquippable : ScriptableObject
{
    // Hook for future restrictions; default allow all
    public virtual bool CanEquip(MapPartyMemberDefinition member) => member != null && member.health > 0;

     // Called when the item is equipped on a member
    public virtual void OnEquip(MapPartyMemberDefinition member) { }

    // Called when the item is unequipped from a member
    public virtual void OnUnequip(MapPartyMemberDefinition member) { }
}
