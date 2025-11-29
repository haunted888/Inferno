using UnityEngine;

public abstract class ItemHeldEquippable : ScriptableObject
{
    // Hook for future restrictions; default allow all
    public virtual bool CanEquip(MapPartyMemberDefinition member) => member != null && member.health > 0;
}
