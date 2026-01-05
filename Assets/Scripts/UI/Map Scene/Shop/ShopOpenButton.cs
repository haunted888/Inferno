using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopOpenButton : MonoBehaviour
{
    [Header("UI")]
    public ShopUIManager shopUI;

    [Header("Stock (set by node)")]
    [SerializeField] private List<ItemDefinition> stock;

    public Button button;

    void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OpenShop);
        }

        gameObject.SetActive(false);
    }

    public void SetStock(List<ItemDefinition> newStock)
    {
        stock = newStock;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OpenShop()
    {
        if (shopUI == null || stock == null) return;
        shopUI.Open(stock);
    }
}
