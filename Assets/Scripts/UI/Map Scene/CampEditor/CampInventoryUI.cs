using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class CampInventoryUI : MonoBehaviour
{
    public CampEditorScreenController campEditor;
    public Transform contentParent;     // ScrollView/Viewport/Content
    public GameObject itemEntryPrefab;  // InventoryItemEntry prefab
    public TMP_Text emptyText;          // optional “No items”
    private ItemType inventoryTab = ItemType.Equippable;      // Which tab this inventory shows

    [Header("Tabs")]
    public Button equippableTabButton;
    public Button consumableTabButton;
    public Button battleTabButton;
    public Button scrollTabButton;

    private MapPartyMemberDefinition member;


    void Awake()
    {
        if (equippableTabButton != null)
            equippableTabButton.onClick.AddListener(() => setInventoryType(ItemType.Equippable));
        if (consumableTabButton != null)
            consumableTabButton.onClick.AddListener(() => setInventoryType(ItemType.Consumable));        
        if (battleTabButton != null)
            battleTabButton.onClick.AddListener(() => setInventoryType(ItemType.Battle));
        if (scrollTabButton != null)    
            scrollTabButton.onClick.AddListener(() => setInventoryType(ItemType.Scroll));
    }

    public void SetCharacter(MapPartyMemberDefinition m)
    {
        member = m;
        inventoryTab = ItemType.Equippable;
        Refresh();
    }

    public void setInventoryType(ItemType type)
    {
        inventoryTab = type;
        Refresh();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Refresh()
    {
        // Clear previous
        if (contentParent != null)
        {
            for (int i = contentParent.childCount - 1; i >= 0; i--)
                Destroy(contentParent.GetChild(i).gameObject);
        }

        var transfer = MapCombatTransfer.Instance;
        IReadOnlyList<ItemStack> items = transfer != null ? transfer.GetInventory() : null;
        bool hasItems = items != null && items.Count > 0;

        if (emptyText != null) emptyText.gameObject.SetActive(!hasItems);

        if (!hasItems || itemEntryPrefab == null || contentParent == null) return;

        foreach (var stack in items)
        {
            if (!stack.item.itemTypes.Contains(inventoryTab))
                continue;
            if (stack == null || stack.item == null) continue;
            if(stack.quantity <= 0) continue;
            var row = Instantiate(itemEntryPrefab, contentParent);
            var entry = row.GetComponent<InventoryItemEntry>();
            if (entry != null) entry.Set(stack.item, stack.quantity, OnItemClicked);
        }
        
        if (campEditor != null)
            campEditor.RefreshCharacterPortrait();
    }
    private void OnItemClicked(ItemDefinition item)
    {
        if (item == null) return;


        if (item.mapConsumable != null)
            Debug.Log($"Using {item.displayName} on {member.displayName} in camp.");
            item.mapConsumable.UseInCamp(item, member);
            Refresh();
        if (item.heldEquippable != null)
        {
            
            var transfer = MapCombatTransfer.Instance;
            if (transfer != null && item.heldEquippable != null &&
                item.heldEquippable.CanEquip(member))
            {
                var currentOnMember = transfer.GetEquippedItem(member);

                if (currentOnMember == item)
                {
                    transfer.UnequipHeldItemFromMember(member);
                }
                else
                {
                    transfer.EquipHeldItem(member, item);
                }
            }
            Refresh();
        }
    }
}
