using UnityEngine;

[CreateAssetMenu(menuName = "Items/Held/All Stats +1")]
public class ItemAllStatsUp : ItemHeldEquippable
{
    public override bool CanEquip(MapPartyMemberDefinition member)
    {
        // Default check + alive condition
        return base.CanEquip(member);
    }

    // Called when equipped (we’ll add proper hook integration later)
    public override void OnEquip(MapPartyMemberDefinition member)
    {
        if (member == null) return;

        member.stats.maxHealth           += 1;
        member.stats.physicalAttack       += 1000000;
        member.stats.elementalPower       += 1;
        member.stats.defense              += 1;
        member.stats.elementalResistance  += 1;
        member.stats.critDamage           += 1;
        member.stats.critChance           += 1;
        member.stats.speed                += 1;
    }

    // Called when unequipped
    public override void OnUnequip(MapPartyMemberDefinition member)
    {
        if (member == null) return;

        member.stats.maxHealth           -= 1;
        member.stats.physicalAttack       -= 1000000;
        member.stats.elementalPower       -= 1;
        member.stats.defense              -= 1;
        member.stats.elementalResistance  -= 1;
        member.stats.critDamage           -= 1;
        member.stats.critChance           -= 1;
        member.stats.speed                -= 1;
    }
}
