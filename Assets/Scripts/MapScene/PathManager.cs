using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    [Tooltip("Node to treat as the starting point for pathfinding.")]
    public PathNode startNode;

    [Tooltip("Player object that should move along the spline path.")]
    public Transform player;

    [Tooltip("Units per second along the spline.")]
    public float moveSpeed = 3f;

    private SplinePathSegment[] SplinePathSegments;

    private HashSet<PathNode> visitedNodes = new HashSet<PathNode>();

    public static System.Action<PathNode> OnArrivedAtNode;

    void Awake()
    {
        // Cache all spline edges in the scene
        SplinePathSegments = FindObjectsByType<SplinePathSegment>(FindObjectsSortMode.None);
        visitedNodes.Clear();
        if (startNode != null)
            visitedNodes.Add(startNode);

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

   
}