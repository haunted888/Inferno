using UnityEngine;
using UnityEngine.UI;

public class CampEditorOpenButton : MonoBehaviour
{
    public CampEditorScreenController screen;
    public CampUIManager campUIManager;
    public InventoryUI inventoryUI;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (screen != null) screen.Open();
            if (campUIManager != null) {
                campUIManager.Open();
                campUIManager.SaveAndCloseForEditorOpen();
            }
            if (inventoryUI != null) inventoryUI.Close();
        });
    }
}
