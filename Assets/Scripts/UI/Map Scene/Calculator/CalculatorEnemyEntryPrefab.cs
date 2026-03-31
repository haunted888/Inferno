using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorEnemyEntryUI : MonoBehaviour
{
    public TMP_Text nameText;

    [Header("HP")]
    public Slider hpBar;
    public Slider hpBarPreview;
    public TMP_Text hpText;
    public TMP_Text hpPreviewText;

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

        // Max HP only (enemies do not have SP)
        int maxHp = !storedDef.stats.IsUnityNull() ? storedDef.stats.maxHealth : 0;

        if (hpBar != null)
        {
            hpBar.minValue = 0;
            setMaxHP(maxHp);
            setHP(maxHp);
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