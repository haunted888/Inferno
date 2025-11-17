using UnityEngine;

public class MapCombatTrigger : MonoBehaviour
{
    private MapNode mapNode;                     // assign in inspector
    public MapEnemyDefinition[] enemies;        // assign in inspector

    private bool triggered = false;

    void Awake()
    {
        mapNode = GetComponent<MapNode>();
    }
    
    void OnEnable()
    {
        PathfindingManager.OnArrivedAtNode += HandleArrivedAtNode;
    }

    void OnDisable()
    {
        PathfindingManager.OnArrivedAtNode -= HandleArrivedAtNode;
    }

    void HandleArrivedAtNode(PathNode arrivedNode)
    {
        if (triggered) return;
        if (mapNode == null || mapNode.location != arrivedNode) return;

        triggered = true;

        MapCombatTransfer.Instance.SetupEnemies(enemies);

        // keep this object, but you probably want to disable its collider
        // so it won't retrigger if you ever come back:
        // var col = GetComponent<Collider>();
        // if (col != null) col.enabled = false;

        MapSceneLoader.Instance.LoadBattleScene();
    }
}
