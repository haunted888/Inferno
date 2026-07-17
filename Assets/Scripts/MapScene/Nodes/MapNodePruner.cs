using System.Collections.Generic;
using UnityEngine;

public class MapNodeBranchPruner : MonoBehaviour
{
    [Header("Map Nodes Removed When This Node Is Selected")]
    public List<MapNode> mapNodesToRemove = new List<MapNode>();

    [Header("Paths Removed When Connected To Removed Nodes")]
    public bool removeAttachedPaths = true;

    public void Prune()
    {
        MapCombatTransfer transfer = MapCombatTransfer.Instance;

        HashSet<PathNode> removedPathNodes = new HashSet<PathNode>();

        foreach (MapNode mapNode in mapNodesToRemove)
        {
            if (mapNode == null) continue;

            if (transfer != null)
                transfer.RegisterRemovedMapNode(mapNode.gameObject.name);

            if (mapNode.location != null)
                removedPathNodes.Add(mapNode.location);

            mapNode.gameObject.SetActive(false);
        }

        if (removeAttachedPaths)
            DisableAttachedPaths(removedPathNodes);
    }

    private void DisableAttachedPaths(HashSet<PathNode> removedPathNodes)
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

            if (removedPathNodes.Contains(path.nodeA) || removedPathNodes.Contains(path.nodeB))
                path.gameObject.SetActive(false);
        }
    }
}