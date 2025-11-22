using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    
    public static PathfindingManager Instance { get; private set; }

    [Tooltip("Node to treat as the starting point for pathfinding.")]
    public PathNode startNode;

    [Tooltip("Player object that should move along the spline path.")]
    public Transform player;

    [Tooltip("Units per second along the spline.")]
    public float moveSpeed = 3f;

    public PathNode LastNodeBeforeMove { get; private set; }

    private SplinePathSegment[] SplinePathSegments;

    private HashSet<PathNode> visitedNodes = new HashSet<PathNode>();

    public static System.Action<PathNode> OnArrivedAtNode;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cache all spline edges in the scene
        SplinePathSegments = FindObjectsByType<SplinePathSegment>(FindObjectsSortMode.None);
        visitedNodes.Clear();
        if (startNode != null){
            visitedNodes.Add(startNode);
            LastNodeBeforeMove = startNode;
        }
        RestorePathStateFromTransfer();
        PersistPathStateToTransfer();

        ClickManager.OnNodeClicked.AddListener(HandleNodeClicked);
    }


    void OnDestroy()
    {
        // Clean up subscription
        if (ClickManager.OnNodeClicked != null)
            ClickManager.OnNodeClicked.RemoveListener(HandleNodeClicked);
    }

    void HandleNodeClicked(PathNode clickedNode)
    {
        if (startNode == null || clickedNode == null)
            return;
        
        if (IsCombatNode(clickedNode) && !HasLivingParty())
        {
            Debug.Log("Cannot move to combat node: party is empty or all members are dead.");
            return;
        }


        if (!IsNodeUnlocked(clickedNode))
        {
            Debug.Log("Node " + clickedNode.name + " is locked (no visited neighbors).");
            return;
        }

        List<PathNode> path = FindShortestPath(startNode, clickedNode);
        if (path == null)
        {
            Debug.Log("No path found from " + startNode.name + " to " + clickedNode.name);
            return;
        }

        PrintPath(path);

        LastNodeBeforeMove = startNode;

        // Move player along path of splines
        StopAllCoroutines();
        StartCoroutine(MovePlayerAlongPath(path));

        // Optionally, treat the clicked node as new start
        startNode = clickedNode;
    }

    List<PathNode> FindShortestPath(PathNode start, PathNode goal)
    {
        if (start == goal)
            return new List<PathNode> { start };

        var queue = new Queue<PathNode>();
        var cameFrom = new Dictionary<PathNode, PathNode>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            PathNode current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (PathNode neighbor in current.neighbors)
            {
                if (neighbor == null) continue;
                if (cameFrom.ContainsKey(neighbor)) continue; // visited

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null; // unreachable

        var path = new List<PathNode>();
        PathNode node = goal;
        while (node != null)
        {
            path.Add(node);
            node = cameFrom[node];
        }

        path.Reverse();
        return path;
    }

    void PrintPath(List<PathNode> path)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < path.Count; i++)
        {
            sb.Append(path[i].name);
            if (i < path.Count - 1)
                sb.Append(" -> ");
        }

        int steps = path.Count - 1;
        Debug.Log($"Path: {sb} | Steps: {steps}");
    }

    IEnumerator MovePlayerAlongPath(List<PathNode> path)
    {
        if (player == null) yield break;
        if (path.Count < 2) yield break;

        for (int i = 0; i < path.Count - 1; i++)
        {
            PathNode from = path[i];
            PathNode to   = path[i + 1];

            SplinePathSegment edge = FindEdge(from, to);
            if (edge == null)
            {
                Debug.LogWarning($"No SplinePathSegment found between {from.name} and {to.name}");
                yield break;
            }

            yield return MoveAlongSplinePathSegment(edge, from, to);
        }
        foreach (var node in path)
        {
            if (node != null)
                visitedNodes.Add(node);
        }
        
        startNode = path[path.Count - 1];
        OnArrivedAtNode?.Invoke(startNode);
        PersistPathStateToTransfer();
    }

    SplinePathSegment FindEdge(PathNode from, PathNode to)
    {
        foreach (var edge in SplinePathSegments)
        {
            if (edge != null && edge.Connects(from, to))
                return edge;
        }
        return null;
    }

    IEnumerator MoveAlongSplinePathSegment(SplinePathSegment edge, PathNode from, PathNode to)
    {
        float length = edge.GetLength();
        if (length <= 0f) yield break;

        float duration = length / moveSpeed;
        float t = 0f;

        // Decide direction along the spline
        bool forward = (from == edge.nodeA && to == edge.nodeB);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float param = Mathf.Clamp01(t);
            if (!forward)
                param = 1f - param;

            Vector3 pos = edge.EvaluatePosition01(param);
            player.position = pos;

            yield return null;
        }
    }
    bool IsNodeUnlocked(PathNode target)
    {
        if (target == null) return false;

        // already visited or is the starting node
        if (visitedNodes.Contains(target) || target == startNode)
            return true;

        // unlocked if any neighbor has been visited
        foreach (var neighbor in target.neighbors)
        {
            if (neighbor != null && visitedNodes.Contains(neighbor))
                return true;
        }

        return false;
    }

   public void ForceSetPlayerNode(PathNode node)
    {
        if (node == null) return;

        // Update lastNodeBeforeMove EVERY time we explicitly set the player node
        LastNodeBeforeMove = node;

        startNode = node;
        player.position = node.transform.position;
        PersistPathStateToTransfer();
    }

    public void MarkNodeUnvisited(PathNode node)
    {
        if (node == null) return;
        visitedNodes.Remove(node);
        PersistPathStateToTransfer();
    }
    private bool HasLivingParty()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null || transfer.party == null) return false;

        foreach (var def in transfer.party)
        {
            if (def != null && def.health > 0)
                return true;
        }
        return false;
    }

    private bool IsCombatNode(PathNode node)
    {
        if (node == null) return false;

        var triggers = FindObjectsByType<MapCombatTrigger>(FindObjectsSortMode.None);
        foreach (var trig in triggers)
        {
            if (trig != null && trig.IsOnNode(node))
                return true;
        }
        return false;
    }

    void RestorePathStateFromTransfer()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null || !transfer.HasPathState)
            return;

        var allNodes = FindObjectsByType<PathNode>(FindObjectsSortMode.None);
        var lookup = new Dictionary<string, PathNode>();
        foreach (var node in allNodes)
        {
            if (node == null) continue;
            if (!lookup.ContainsKey(node.name))
                lookup.Add(node.name, node);
        }

        visitedNodes.Clear();
        var visitedNames = transfer.GetVisitedNodeNames();
        if (visitedNames != null)
        {
            foreach (var name in visitedNames)
            {
                if (string.IsNullOrEmpty(name)) continue;
                if (lookup.TryGetValue(name, out var node))
                    visitedNodes.Add(node);
            }
        }

        PathNode current = null;
        if (!string.IsNullOrEmpty(transfer.currentMapNodeName))
            lookup.TryGetValue(transfer.currentMapNodeName, out current);

        if (current == null && startNode != null)
            current = startNode;

        if (current != null)
        {
            startNode = current;
            LastNodeBeforeMove = current;
            visitedNodes.Add(current);
            if (player != null)
                player.position = current.transform.position;
        }
    }

    void PersistPathStateToTransfer()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;
        transfer.SavePathState(visitedNodes, startNode);
    }

}
