using System.Collections.Generic;
using UnityEngine;

public class MapShopTrigger : MonoBehaviour
{
    private MapNode mapNode;

    [Header("Shop Stock (per node)")]
    public List<ItemDefinition> stock = new List<ItemDefinition>();

    public ShopOpenButton shopButton;

    void Awake()
    {
        mapNode = GetComponent<MapNode>();
    }

    void OnEnable()
    {
        PathfindingManager.OnArrivedAtNode += HandleArrived;
        PathfindingManager.OnExitedNode += HandleExited;
    }

    void OnDisable()
    {
        PathfindingManager.OnArrivedAtNode -= HandleArrived;
        PathfindingManager.OnExitedNode -= HandleExited;
    }

    private void HandleArrived(PathNode node)
    {
        if (mapNode == null || mapNode.location != node) return;
        if (shopButton == null) return;

        shopButton.SetStock(stock);
        shopButton.Show();
    }

    private void HandleExited(PathNode node)
    {
        if (mapNode == null || mapNode.location != node) return;
        if (shopButton == null) return;

        shopButton.Hide();
    }
}
