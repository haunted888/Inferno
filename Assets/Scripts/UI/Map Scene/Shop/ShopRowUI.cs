using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRowUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;
    public TMP_Text quantityText;   // hook to your new column

    private ItemDefinition item;
    private Action<ItemDefinition> onBuy;
    private int quantity;

    public void Set(ItemDefinition item, int quantity, Action<ItemDefinition> onBuy)
    {
        this.item = item;
        this.onBuy = onBuy;
        this.quantity = quantity;

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => this.onBuy?.Invoke(this.item));
        }

        Refresh(canAfford: true);
    }

    public ItemDefinition GetItem() => item;

    public void SetQuantity(int newQuantity)
    {
        quantity = newQuantity;
    }

    public int GetQuantity() => quantity;

    public void Refresh(bool canAfford)
    {
        if (item == null) return;

        if (nameText != null) nameText.text = item.displayName;
        if (priceText != null) priceText.text = item.price.ToString();
        if (quantityText != null) quantityText.text = quantity.ToString();

        if (buyButton != null)
            buyButton.interactable = quantity > 0 && canAfford;
    }

}
