using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    public delegate void NodeClicked(PathNode node);
    public static UnityEvent<PathNode> OnNodeClicked = new UnityEvent<PathNode>();

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                //Broadcast pathnode click event
                if (hitInfo.collider.TryGetComponent<MapNode>(out MapNode clickedNode))
                {
                    PathNode nodeLocation = clickedNode.location;
                    if(nodeLocation != null){
                        OnNodeClicked?.Invoke(nodeLocation);
                    } else {
                        Debug.Log("Node Location is null");
                    }
                }
            }
        }
    }

}
