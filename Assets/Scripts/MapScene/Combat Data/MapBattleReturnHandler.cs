using System.Collections.Generic;
using UnityEngine;

public class MapBattleReturnHandler : MonoBehaviour
{
    void Start()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        transfer.ApplyDestroyedNodesInScene();
        ApplyRemovedMapNodes();

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

    private void ApplyRemovedMapNodes()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        MapNode[] mapNodes = FindObjectsByType<MapNode>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        HashSet<PathNode> removedPathNodes = new HashSet<PathNode>();

        foreach (MapNode mapNode in mapNodes)
        {
            if (mapNode == null) continue;

            if (!transfer.IsMapNodeRemoved(mapNode.gameObject.name))
                continue;

            if (mapNode.location != null)
                removedPathNodes.Add(mapNode.location);

            mapNode.gameObject.SetActive(false);
        }

        DisablePathsConnectedToRemovedNodes(removedPathNodes);
    }

    private void DisablePathsConnectedToRemovedNodes(HashSet<PathNode> removedPathNodes)
    {
        if (removedPathNodes == null || removedPathNodes.Count == 0)
            return;

        SplinePathSegment[] paths = FindObjectsByType<SplinePathSegment>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (SplinePathSegment path in paths)
        {
            if (path == null) continue;

            if (removedPathNodes.Contains(path.nodeA) ||
                removedPathNodes.Contains(path.nodeB))
            {
                path.gameObject.SetActive(false);
            }
        }
    }
}