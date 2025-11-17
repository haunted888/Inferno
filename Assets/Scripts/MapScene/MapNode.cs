using UnityEngine;
using UnityEngine.UIElements;

public class MapNode : MonoBehaviour
{
    public PathNode location;

    private void Awake()
    {
        if (location != null)
        {
            transform.position = location.transform.position;
        }
    }
}
