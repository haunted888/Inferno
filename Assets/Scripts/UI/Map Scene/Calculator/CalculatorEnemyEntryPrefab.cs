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
    
    [NonSerialized]
    public CalculatorSkillList skillList;
    [NonSerialized]
    public GameObject skillListContainer;

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


        def.EnsureInitializedFromAsset();

        // Max HP only (enemies do not have SP)
        int maxHp = !def.stats.IsUnityNull() ? def.stats.maxHealth : 0;

        if (hpBar != null)
        {
            hpBar.minValue = 0;
            setMaxHP(maxHp);
            setHP(maxHp);
        }

        if(skillListContainer != null)
        {
            skillButton.onClick.AddListener(OpenSkillList);
        }
    }
    
    public void SetSkillList(CalculatorSkillList skillList, GameObject skillListContainer)
    {
        this.skillList = skillList;
        this.skillListContainer = skillListContainer;
    }

    void OpenSkillList()
    {
        if (skillList != null)
        {
            skillList.SetActiveCharacter(storedDef);
            skillList.UpdateSkillList(storedDef.skills, false);
            skillListContainer.SetActive(true);
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
    public void SetTurnOrder(int order)
    {
        if (turnOrderText != null)
            turnOrderText.text = $"{order}";
    }
}