using TMPro;
using UnityEngine;

public class BattleText : MonoBehaviour
{
    public TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetText(string message)
    {
        text.text = message;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        text.text = "";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
