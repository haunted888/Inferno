// SplinePathSegment.cs
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

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


    void Awake()
    {
        if (container == null)
            container = GetComponent<SplineContainer>();
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

    void OnEnable()   { Cache(); UpdateVisual(); }
    void OnValidate() { Cache(); UpdateVisual(); }
    void Reset()      { Cache(); UpdateVisual(); }

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