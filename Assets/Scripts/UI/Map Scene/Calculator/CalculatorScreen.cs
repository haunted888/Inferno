using UnityEngine;
using UnityEngine.InputSystem;

public class CalculatorScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false); // Start hidden
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;
        if(InputSystem.actions.FindAction("exit").WasPressedThisFrame())        {
            gameObject.SetActive(false);
        }
    }
}
