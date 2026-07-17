using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorEnemyEntryUI : MonoBehaviour
{
    public TMP_Text nameText;

    [Header("HP/SP")]
    public Slider hpBar;
    public Slider hpBarPreview;
    public TMP_Text hpText;
    public TMP_Text hpPreviewText;

    public Slider spBar;
    public Slider spBarPreview;
    public TMP_Text spText;
    public TMP_Text spPreviewText;

    [Header("Skills")]
    public Button skillButton;

    [Header("Stats")]
    public Button statButton;

    [NonSerialized] public CalculatorScreen calculatorScreen;

    [Header("Turn Order")]
    public TMP_Text turnOrderText;

    private MapEnemyDefinition storedDef;

    public void Setup(MapEnemyDefinition def)
    {
        if (def == null) return;
        storedDef = def;
        // Name
        if (nameText != null)
        {
            if (!string.IsNullOrEmpty(def.displayName))
                nameText.text = def.displayName;
            else
                nameText.text = def.enemyPrefab != null ? def.enemyPrefab.name : "Unnamed";
        }


        storedDef.EnsureInitializedFromAsset();

        // Max HP / Max SP (shown full)
        int maxHp = !storedDef.stats.IsUnityNull() ? storedDef.stats.maxHealth : 0;
        int maxSp = !storedDef.stats.IsUnityNull() ? storedDef.stats.maxSp : 0;

        if (hpBar != null)
        {
            hpBar.minValue = 0;
            setMaxHP(maxHp);
            setHP(maxHp);
        }

        if (spBar != null)
        {
            spBar.minValue = 0;
            setMaxSP(maxSp);
            setSP(maxSp);
        }

        if (skillButton != null)
        {
            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(OpenSkillList);
        }

        if (statButton != null)
        {
            statButton.onClick.RemoveAllListeners();
            statButton.onClick.AddListener(OpenStatScreen);
        }
    }
    

    public void SetCalculatorScreen(CalculatorScreen screen)
    {
        calculatorScreen = screen;
    }

    void OpenSkillList()
    {
        if (calculatorScreen != null)
            calculatorScreen.OpenEnemySkillScreen(storedDef);
    }

    void setMaxHP(int max)
    {
        if (hpBar != null)
        {
            hpBar.maxValue = max;
            if (hpBarPreview != null)
                hpBarPreview.maxValue = max;
        }

        if (hpText != null && hpBar != null)
            hpText.text = $"{hpBar.value}/{max}";

        if (hpPreviewText != null && hpBarPreview != null)
            hpPreviewText.text = $"{hpBarPreview.value}/{max}";
    }
    void setHP(int current)
    {
        if (hpBar != null)
        {
            hpBar.value = current;
            if (hpBarPreview != null)
                hpBarPreview.value = current;
        }

        if (hpText != null && hpBar != null)
            hpText.text = $"{current}/{hpBar.maxValue}";

        if (hpPreviewText != null && hpBarPreview != null)
            hpPreviewText.text = $"{current}/{hpBarPreview.maxValue}";
    }

    void setMaxSP(int max)
    {
        if (spBar != null)
        {
            spBar.maxValue = max;
            if (spBarPreview != null)
                spBarPreview.maxValue = max;
        }

        if (spText != null && spBar != null)
            spText.text = $"{spBar.value}/{max}";

        if (spPreviewText != null && spBarPreview != null)
            spPreviewText.text = $"{spBarPreview.value}/{max}";
    }

    void setSP(int current)
    {
        if (spBar != null)
        {
            spBar.value = current;
            if (spBarPreview != null)
                spBarPreview.value = current;
        }

        if (spText != null && spBar != null)
            spText.text = $"{current}/{spBar.maxValue}";

        if (spPreviewText != null && spBarPreview != null)
            spPreviewText.text = $"{current}/{spBarPreview.maxValue}";
    }

    public void SetTurnOrder(int order)
    {
        if (turnOrderText != null)
            turnOrderText.text = $"{order}";
    }

    void OpenStatScreen()
    {
        if (calculatorScreen != null)
            calculatorScreen.OpenEnemyStatScreen(storedDef);
    }
}
