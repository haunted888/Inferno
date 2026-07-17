using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held/Apply Passive")]
public class ApplyPassiveScript : ItemHeldEquippable
{
    public PassivesDefinition passiveToApply;

    public override bool CanEquip(MapPartyMemberDefinition member)
    {
        // Default check + alive condition
        return base.CanEquip(member);
    }

    // Called when equipped (we’ll add proper hook integration later)
    public override void OnEquip(MapPartyMemberDefinition member)
    {
        if (member == null || passiveToApply == null) return;

        // Apply the passive to the member
        member.AddPassive(passiveToApply);
    }

    // Called when unequipped
    public override void OnUnequip(MapPartyMemberDefinition member)
    {
        if (member == null || passiveToApply == null) return;

        // Remove the passive from the member
        member.RemovePassive(passiveToApply);
    }
}
