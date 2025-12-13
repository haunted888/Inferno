using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Map Consumable/Teach Skill")]
public class TeachSkillMap : ItemConsumableInMap
{
    [Header("Skill to teach")]
    public Skill skillToTeach;

    // Open Camp UI → click a member → teach if valid → consume 1 → refresh UI
    public override void BeginUseOnMap(ItemDefinition item, InventoryUI invUI)
    {
        if (skillToTeach == null) return;

        var camp = FindFirstObjectByType<CampUIManager>();
        if (camp == null) return;

        camp.BeginItemTargeting(member =>
        {
            if (member == null) return;
            if (member.health <= 0) return; // cannot teach dead

            // Ensure member.skills exists and avoid duplicates
            var current = member.skills != null ? new List<Skill>(member.skills) : new List<Skill>();
            if (current.Contains(skillToTeach)) return;

            current.Add(skillToTeach);
            member.skills = current.ToArray();

            // Consume item (keeps zero-qty entries)
            MapCombatTransfer.Instance.RemoveItem(item, 1);

            // Close targeting and refresh inventory UI
            camp.CancelItemTargeting();
            invUI.Refresh();
        });
    }
    public override void UseInCamp(ItemDefinition item, MapPartyMemberDefinition member)
    {
        if (skillToTeach == null) return;

        if (member == null) return;
        if (member.health <= 0) return; // cannot teach dead

        var current = member.skills != null ? new List<Skill>(member.skills) : new List<Skill>();
        if (current.Contains(skillToTeach)) return;

        current.Add(skillToTeach);
        member.skills = current.ToArray();

        // Consume item (keeps zero-qty entries)
        MapCombatTransfer.Instance.RemoveItem(item, 1);
    }
}
