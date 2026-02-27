using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorPartyEntryUI : MonoBehaviour
{
    public TMP_Text nameText;

    public Slider hpBar;
    public Slider hpBarPreview; 
    public TMP_Text hpText;
    public TMP_Text hpPreviewText;

    public Slider spBar;
    public Slider spBarPreview;
    public TMP_Text spText;
    public TMP_Text spPreviewText;

    public Image itemIcon;

    public void Setup(MapPartyMemberDefinition def)
    {
        if (def == null) return;

        // Name (display name)
        if (nameText != null)
        {
            if (!string.IsNullOrEmpty(def.displayName)) nameText.text = def.displayName;
            else if (def.characterPrefab != null) nameText.text = def.characterPrefab.name;
            else nameText.text = "Unnamed";
        }

        // Max HP / Max SP (shown full)
        int maxHp = !def.stats.IsUnityNull() ? def.stats.maxHealth : 0;
        int maxSp = !def.stats.IsUnityNull() ? def.stats.maxSp : 0;

        if (hpBar != null)
        {
            hpBar.minValue = 0;
            setMaxHP(maxHp);
            setHP(def.health);
        }

        if (spBar != null)
        {
            spBar.minValue = 0;
            setMaxSP(maxSp);
            setSP(maxSp);
        }

        // Equipped item icon
        if (itemIcon != null)
        {
            var transfer = MapCombatTransfer.Instance;
            var equipped = transfer != null ? transfer.GetEquippedItem(def) : null;
            var icon = equipped != null ? equipped.icon : null;
            
            itemIcon.sprite = icon;
            itemIcon.enabled = icon != null;
        }

        // Char icon intentionally not implemented yet
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
    
    void setMaxSP(int max)
    {
        if (spBar != null)
        {
            spBar.maxValue = max;
            spBarPreview.maxValue = max;
        }

        if (spText != null)
            spText.text = $"{spBar.value}/{max}";
            spPreviewText.text = $"{spBarPreview.value}/{max}";
    }
    void setSP(int current)
    {
        if (spBar != null)
        {
            spBar.value = current;
            spBarPreview.value = current;
        }

        if (spText != null)
            spText.text = $"{current}/{spBar.maxValue}";
            spPreviewText.text = $"{current}/{spBarPreview.maxValue}";
    }
}