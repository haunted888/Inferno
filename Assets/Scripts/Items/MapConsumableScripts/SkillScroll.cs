using System.Collections.Generic;
using System.Linq;
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

            if(member.LearnSkill(skillToTeach)) return;

            
            member.ApplyMapItemUseTraitEffects(item);

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

        if(!member.LearnSkill(skillToTeach))
        {
            Debug.LogWarning($"Failed to teach skill {skillToTeach.skillName} to {member.GetDisplayName()} - check trait requirements and existing skills.");
            return;
        }

        member.ApplyMapItemUseTraitEffects(item);
        
        // Consume item (keeps zero-qty entries)
        MapCombatTransfer.Instance.RemoveItem(item, 1);
    }
}
