using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemEntry : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text qtyText;
    public Button button;

    public void Set(ItemDefinition item, int quantity, System.Action<ItemDefinition> onClick = null)
    {
        if (icon)     icon.sprite = item ? item.icon : null;
        if (nameText) nameText.text = item ? item.displayName : "Unknown";
        if (qtyText)  qtyText.text = $"x{quantity}";

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            if (onClick != null)
                button.onClick.AddListener(() => onClick(item));
        }
    }
}
