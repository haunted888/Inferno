using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ClickManagerBattle : MonoBehaviour
{

    public delegate void CharacterClicked(BattleCharacter character);
    public static UnityEvent<BattleCharacter> OnCharacterClicked = new UnityEvent<BattleCharacter>();

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
                if (hitInfo.collider.TryGetComponent<BattleCharacter>(out BattleCharacter clickedCharacter))
                {
                    if(clickedCharacter != null){
                        OnCharacterClicked?.Invoke(clickedCharacter);
                    } else {
                        Debug.Log("Clicked Character is null");
                    }
                }
            }
        }
    }

}
