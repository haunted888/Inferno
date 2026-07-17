// SplinePathSegment.cs
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;

[ExecuteAlways]
[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(LineRenderer))]
public class SplinePathSegment : MonoBehaviour
{
    
    public PathNode nodeA;
    public PathNode nodeB;
    [Range(8, 100)]
    public int lineResolution = 40;

    SplineContainer container;
    LineRenderer line;

    private Vector3 lastNodeAPosition;
    private Vector3 lastNodeBPosition;

    void Awake()
    {
        if (container == null)
            container = GetComponent<SplineContainer>();
    }


    void OnEnable()
    {
        Cache();
        ForceRefresh();
    }


    void Update()
    {
        if (Application.isPlaying)
            return;

        Cache();

        if (nodeA == null || nodeB == null)
            return;

        if (nodeA.transform.position == lastNodeAPosition &&
            nodeB.transform.position == lastNodeBPosition)
            return;

        ForceRefresh();
    }

    private void ForceRefresh()
    {
        if (nodeA == null || nodeB == null)
            return;

        lastNodeAPosition = nodeA.transform.position;
        lastNodeBPosition = nodeB.transform.position;

        UpdateKnotLocations();
        UpdateVisual();
    }

    public bool Connects(PathNode from, PathNode to)
    {
        return (from == nodeA && to == nodeB) || (from == nodeB && to == nodeA);
    }

    public float GetLength()
    {
        if (container == null)
            container = GetComponent<SplineContainer>();

        return container.CalculateLength();
    }

    public Vector3 EvaluatePosition01(float t)
    {
        if (container == null)
            container = GetComponent<SplineContainer>();

        t = Mathf.Clamp01(t);
        float3 p = container.EvaluatePosition(t); // world-space
        return new Vector3(p.x, p.y, p.z);
    }
    void Cache()
    {
        if (container == null) container = GetComponent<SplineContainer>();
        if (line == null) line = GetComponent<LineRenderer>();
    }


    public void UpdateKnotLocations()
    {


        var knots = container.Spline;
        var knotA = knots[0];
        var knotB = knots[^1];

        knotA.Position = container.transform.InverseTransformPoint(nodeA.transform.position);
        knotB.Position = container.transform.InverseTransformPoint(nodeB.transform.position);

        knots[0] = knotA;
        knots[^1] = knotB;

    }

    public void UpdateVisual()
    {
        Cache();
        if (container == null || line == null) return;

        if (lineResolution < 2) lineResolution = 2;

        line.positionCount = lineResolution;

        for (int i = 0; i < lineResolution; i++)
        {
            float t = (lineResolution == 1) ? 0f : i / (float)(lineResolution - 1);
            float3 p = container.EvaluatePosition(t);
            line.SetPosition(i, new Vector3(p.x, p.y, p.z));
        }
    }
}