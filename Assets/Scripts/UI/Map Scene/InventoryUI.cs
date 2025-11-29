using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Wiring")]
    public GameObject panelRoot;        // set inactive by default
    public Toggle openToggle;           // toggle inventory open/closed
    public Button closeButton;          // still supported
    public Transform contentParent;     // ScrollView/Viewport/Content
    public GameObject itemEntryPrefab;  // InventoryItemEntry prefab
    public TMP_Text emptyText;          // optional “No items”
    public CampUIManager campUIManager; // for opening camp if needed

    void Start()
    {
        // Ensure starting OFF (closed) when entering the scene (e.g., Map Scene)
        if (openToggle != null)
        {
            openToggle.isOn = false;
            openToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        SetPanelActive(false);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn) Open();
        else      SetPanelActive(false);
    }

    public void Open()
    {
        SetPanelActive(true);
        Refresh();
    }

    public void Close()
    {
        // Close via button should ALSO toggle the toggle OFF
        if (openToggle != null) openToggle.isOn = false;
        SetPanelActive(false);
    }

    private void SetPanelActive(bool active)
    {
        if (panelRoot != null) panelRoot.SetActive(active);
    }

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
            if (stack == null || stack.item == null || stack.quantity <= 0) continue;
            var row = Instantiate(itemEntryPrefab, contentParent);
            var entry = row.GetComponent<InventoryItemEntry>();
            if (entry != null) entry.Set(stack.item, stack.quantity, OnItemClicked);
        }
    }
    private void OnItemClicked(ItemDefinition item)
    {
        if (item == null) return;

        if(campUIManager != null)
            campUIManager.Open();
        else
        {
            return;
        }

        // Only map consumables act here (battle use comes later)
        if (item.mapConsumable != null)
            item.mapConsumable.BeginUseOnMap(item, this);
        if (item.heldEquippable != null)
    {
        
        campUIManager.BeginEquipTargeting(item, onFinished: () =>
        {
            // After equip/unequip/move, refresh inventory counts
            Refresh();
        });
        return;
    }
    }

}
