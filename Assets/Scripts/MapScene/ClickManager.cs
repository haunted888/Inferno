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
    public static UnityEvent<MapEnemyDefinition[]> OnNodeRightClicked = new UnityEvent<MapEnemyDefinition[]>();
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
                if (hitInfo.collider.TryGetComponent(out MapNode clickedNode))
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
        //Handle right-clicks
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            
            GameObject uiObject = GetUIObjectUnderPointer();
            if (uiObject != null)
            {
                OnUIObjectRightClicked?.Invoke(uiObject);
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hitInfo) && GetUIObjectUnderPointer() == null)
                {
                    //Broadcast pathnode click event
                    if (hitInfo.collider.TryGetComponent(out MapCombatTrigger clickedNode))
                    {
                        MapEnemyDefinition[] enemies = clickedNode.enemies;
                        if(enemies != null){
                            Debug.Log("Right-clicked on non-UI element");
                            OnNodeRightClicked?.Invoke(enemies);
                        } else {
                            Debug.Log("Enemies array is null");
                        }
                    }
                }
            }

            return;
            
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
