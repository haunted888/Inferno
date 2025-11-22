using UnityEngine;

public class MapCombatTrigger : MonoBehaviour
{
    private MapNode mapNode;                     // assign in inspector
    public MapEnemyDefinition[] enemies;        // assign in inspectorpublic MapEnemyDefinition[] enemies;
    public MapRewardGroup[] rewardGroups;


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

    private void HandleArrivedAtNode(PathNode arrivedNode)
    {

        if (triggered) return;
        if (mapNode.location != arrivedNode) return;


        if (mapNode == null)
        {
            Debug.LogError($"MapCombatTrigger on {name} has no MapNode component.");
            return;
        }

        if (mapNode.location != arrivedNode)
            return;

        triggered = true;

        var transfer = MapCombatTransfer.Instance;
        if (transfer == null)
        {
            Debug.LogError("MapCombatTransfer.Instance is null in MapCombatTrigger.");
            return;
        }

        if (!HasLivingParty(transfer))
        {
            Debug.Log("Combat not started: party is empty or all members are dead.");
            return;
        }

        transfer.SetupEnemies(enemies);

        transfer.SetPendingRewardsFromGroups(rewardGroups);



        string safeNodeName = "";
        var pf = PathfindingManager.Instance;
        if (pf != null && pf.LastNodeBeforeMove != null)
        {
            safeNodeName = pf.LastNodeBeforeMove.name;
        }
        else
        {
            Debug.LogWarning("LastNodeBeforeMove is null when starting combat; safe node name will be empty.");
        }

        transfer.RegisterBattleContext(safeNodeName, gameObject.name);

        var loader = MapSceneLoader.Instance;
        if (loader != null)
        {
            loader.LoadBattleScene();
        }
        else
        {
            Debug.LogError("MapSceneLoader.Instance is null; cannot load battle scene.");
        }
    }
    public bool IsOnNode(PathNode node)
    {
        return mapNode != null && mapNode.location == node;
    }

    private bool HasLivingParty(MapCombatTransfer transfer)
    {
        if (transfer.party == null) return false;
        foreach (var def in transfer.party)
        {
            if (def != null && def.health > 0)
                return true;
        }
        return false;
    }

}
