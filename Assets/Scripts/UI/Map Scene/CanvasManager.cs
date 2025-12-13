using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> openOnRefreshUI;
    public List<GameObject> closeOnRefreshUI;
    public void RefreshUI()
    {
        foreach (var ui in openOnRefreshUI)
        {
            if (ui != null) ui.SetActive(true);
        }
        foreach (var ui in closeOnRefreshUI)
        {
            if (ui != null) ui.SetActive(false);
        }
    }
}
