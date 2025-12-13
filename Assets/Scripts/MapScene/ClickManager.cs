using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ClickManager : MonoBehaviour
{
    public delegate void NodeClicked(PathNode node);
    public static UnityEvent<PathNode> OnNodeClicked = new UnityEvent<PathNode>();
    public static UnityEvent<GameObject> OnUIObjectRightClicked = new UnityEvent<GameObject>();
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
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && GetUIObjectUnderPointer() == null)
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
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                GameObject uiObject = GetUIObjectUnderPointer();
                if (uiObject != null)
                {
                    OnUIObjectRightClicked?.Invoke(uiObject);
                }

                return;
            }
        }
    }

    private GameObject GetUIObjectUnderPointer()
    {
        if (EventSystem.current == null || Mouse.current == null)
        {
            return null;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            return raycastResults[0].gameObject;
        }

        return null;
    }
}
