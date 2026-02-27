using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorEnemyEntryUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider hpBar;
    public Slider hpBarPreview;
    public TMP_Text hpText;
    public TMP_Text hpPreviewText;

    public void Setup(MapEnemyDefinition def)
    {
        if (def == null) return;

        // Name
        if (nameText != null)
        {
            if (!string.IsNullOrEmpty(def.displayName))
                nameText.text = def.displayName;
            else
                nameText.text = def.enemyPrefab != null ? def.enemyPrefab.name : "Unnamed";
        }


        def.EnsureInitializedFromAsset();
        
        // Max HP only (enemies do not have SP)
        int maxHp = !def.stats.IsUnityNull() ? def.stats.maxHealth : 0;

        if (hpBar != null)
        {
            hpBar.minValue = 0;
            setMaxHP(maxHp);
            setHP(maxHp);
        }
    }

    void setMaxHP(int max)
    {
        if (hpBar != null)
        {
            hpBar.maxValue = max;
            hpBarPreview.maxValue = max;
        }

        if (hpText != null)
            hpText.text = $"{hpBar.value}/{max}";
            hpPreviewText.text = $"{hpBarPreview.value}/{max}";
    }
    void setHP(int current)
    {
        if (hpBar != null)
        {
            hpBar.value = current;
            hpBarPreview.value = current;
        }

        if (hpText != null)
            hpText.text = $"{current}/{hpBar.maxValue}";
            hpPreviewText.text = $"{current}/{hpBarPreview.maxValue}";
    }
}