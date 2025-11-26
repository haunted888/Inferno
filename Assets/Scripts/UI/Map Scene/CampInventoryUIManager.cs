using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CampInventoryUIManager : MonoBehaviour
{
    public Toggle inventoryButton;
    public HorizontalLayoutGroup campInventoryPanel;
    void Awake()
    {
        inventoryButton.onValueChanged.AddListener(OnInventoryButtonClicked);
    }
    
    void OnInventoryButtonClicked(bool isOn)
    {
        campInventoryPanel.childScaleWidth = isOn;
    }
}
