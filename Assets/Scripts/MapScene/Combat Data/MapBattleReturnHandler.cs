using UnityEngine;

public class MapBattleReturnHandler : MonoBehaviour
{
    void Start()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        transfer.ApplyDestroyedNodesInScene();

        if (string.IsNullOrEmpty(transfer.combatNodeObjectName) &&
            string.IsNullOrEmpty(transfer.lastSafeNodeName))
            return; // no battle context

        if (transfer.lastBattlePlayerWon)
        {
            // Player WON: stay on the node they moved to.
            // Record + destroy the combat node object (remains destroyed on future loads).
            transfer.RegisterDestroyedNode(transfer.combatNodeObjectName);
            transfer.ApplyDestroyedNodesInScene();
        }
        else
        {
            // Player LOST: move back to last safe node
            var safeNodeGO = GameObject.Find(transfer.lastSafeNodeName);
            if (safeNodeGO != null)
            {
                var safeNode = safeNodeGO.GetComponent<PathNode>();
                if (safeNode != null)
                {
                    PathfindingManager.Instance.ForceSetPlayerNode(safeNode);
                }
            }

            // Ensure the combat node is not considered visited => no new unlocks
            var combatObj = GameObject.Find(transfer.combatNodeObjectName);
            if (combatObj != null)
            {
                var combatMapNode = combatObj.GetComponent<MapNode>();
                if (combatMapNode != null && combatMapNode.location != null)
                {
                    PathfindingManager.Instance.MarkNodeUnvisited(combatMapNode.location);
                }
            }
        }

        // Clear context
        transfer.combatNodeObjectName = "";
        transfer.lastSafeNodeName = "";
    }

}
