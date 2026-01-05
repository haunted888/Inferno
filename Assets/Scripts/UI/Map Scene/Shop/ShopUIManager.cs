using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopUIManager : MonoBehaviour
{
    public GameObject root;
    public Transform contentParent;
    public GameObject rowPrefab; // prefab with ShopRowUI
    public TMP_Text moneyText;

    private List<ItemDefinition> currentStock;
    private readonly Dictionary<ItemDefinition, ShopRowUI> rowByItem = new();

    public void Start()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void Update()
    {
        // Optional: live update affordability while open
        if(InputSystem.actions.FindAction("exit")?.WasPressedThisFrame() == true)
        {
            if (root != null && root.activeSelf)
            {
                Close();
            }
        }
        
    }

    public void Open(List<ItemDefinition> stock)
    {
        currentStock = stock;

        ClearRows(); // NEW

        if (root != null) root.SetActive(true);

        RebuildOrUpdateRows();
        RefreshMoneyAndAffordability();
    }

    public void Close()
    {
        if (root != null) root.SetActive(false);

        ClearRows(); // NEW (prevents stale refs + guarantees no duplicates next open)
    }


    private void TryBuy(ItemDefinition item)
    {
        if (item == null) return;

        var t = MapCombatTransfer.Instance;
        int price = Mathf.Max(0, item.price);

        // Find a stock instance to remove (represents 1 quantity)
        if (currentStock == null) return;
        int idx = currentStock.IndexOf(item);
        if (idx < 0) return; // out of stock

        if (!t.TrySpendMoney(price))
        {
            RefreshMoneyAndAffordability();
            return;
        }

        t.AddItem(item, 1);
        currentStock.RemoveAt(idx);

        // Decrement row quantity (do not remove row)
        if (rowByItem.TryGetValue(item, out var row) && row != null)
        {
            int newQty = Mathf.Max(0, row.GetQuantity() - 1);
            row.SetQuantity(newQty);
        }

        RefreshMoneyAndAffordability();
    }


    private void RebuildOrUpdateRows()
    {
        if (contentParent == null || rowPrefab == null) return;

        // Count quantities (duplicates)
        var counts = new Dictionary<ItemDefinition, int>();
        if (currentStock != null)
        {
            foreach (var item in currentStock)
            {
                if (item == null) continue;
                counts[item] = counts.TryGetValue(item, out var c) ? c + 1 : 1;
            }
        }

        // Create rows for any new items
        foreach (var kvp in counts)
        {
            var item = kvp.Key;
            var qty  = kvp.Value;

            if (!rowByItem.TryGetValue(item, out var row) || row == null)
            {
                var go = Instantiate(rowPrefab, contentParent);
                row = go.GetComponent<ShopRowUI>();
                rowByItem[item] = row;
            }

            row.Set(item, qty, TryBuy);
        }

        // For items that are no longer in stock, keep row but set qty to 0
        foreach (var kvp in rowByItem)
        {
            var item = kvp.Key;
            var row  = kvp.Value;
            if (row == null) continue;

            if (!counts.TryGetValue(item, out var qty))
            {
                row.SetQuantity(0);
                row.Refresh(canAfford: false);
            }
        }
    }
    private void RefreshMoneyAndAffordability()
    {
        int money = MapCombatTransfer.Instance.GetMoney();

        if (moneyText != null)
            moneyText.text = $"Money: {money}";

        foreach (var kvp in rowByItem)
        {
            var item = kvp.Key;
            var row  = kvp.Value;
            if (item == null || row == null) continue;

            bool canAfford = money >= Mathf.Max(0, item.price);
            row.Refresh(canAfford);
        }
    }

    private void ClearRows()
    {
        rowByItem.Clear();

        if (contentParent == null) return;

        var shopRows = contentParent.GetComponentsInChildren<ShopRowUI>();

        foreach (var row in shopRows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }
    }


}
