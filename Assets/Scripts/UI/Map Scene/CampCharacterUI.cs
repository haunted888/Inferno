// CampCharacterUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampCharacterUI : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IEndDragHandler
{
    public TMP_Text nameText;
    public Slider healthBar;

    [HideInInspector] public MapPartyMemberDefinition definition;
    CampUIManager manager;

    public static CampCharacterUI currentDragged;

    public void Init(CampUIManager mgr, MapPartyMemberDefinition def)
    {
        manager = mgr;
        definition = def;

        if (nameText != null)
        {
            if (definition == null)
            {
                nameText.text = "Unnamed";
            }
            else if (!string.IsNullOrEmpty(definition.displayName))
            {
                nameText.text = definition.displayName;
            }
            else if (definition.characterPrefab != null)
            {
                nameText.text = definition.characterPrefab.name;
            }
            else
            {
                nameText.text = "Unnamed";
            }
        }
        if (healthBar != null && definition != null)
        {
            UpdateHealthBar(definition.health, definition.GetMaxHealth());
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnCampCharacterClicked(definition);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentDragged = this;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentDragged = null;
    }
    public void UpdateHealthBar(int health, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = maxHealth > 0 ? (float)health / (float)maxHealth : 0f;
        }
    }
}
