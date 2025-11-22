// CampButton.cs
using UnityEngine;
using UnityEngine.UI;

public class CampButton : MonoBehaviour
{
    public CampUIManager campUI;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            campUI.Open();
        });
    }
}
