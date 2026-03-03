using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLineupEntryUI : MonoBehaviour
{
    public TMP_Text letterText;

    public Button button;
    public Image colorBorder;
    public TMP_Text effectText;

    private int index;
    private Action<int> onClicked;


    public void Setup(MapEnemyDefinition def, int index, Action<int> onClicked)
    {
        this.index = index;
        this.onClicked = onClicked;

        if (def != null && letterText != null)
        {
            string name = def.displayName;
            letterText.text = string.IsNullOrEmpty(name) ? "?" : name.Substring(0, 1).ToUpperInvariant();
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.onClicked?.Invoke(this.index));
        }
    }

    public void SetInteractable(bool value)
    {
        if (button != null)
            button.interactable = value;
        if (colorBorder != null)
            colorBorder.color = value ? Color.blue : Color.red;
    }

    public void SetEffectDamage(string text)
    {
        if (effectText != null)
            if(effectText.text.Length > 0) effectText.text += "\n";
            effectText.text += text;
    }

    public void ClearEffectText()
    {
        if (effectText != null)
            effectText.text = "";
    }
}