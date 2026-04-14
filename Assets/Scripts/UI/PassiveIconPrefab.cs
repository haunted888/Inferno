using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection;

public class PassiveIconPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public UnityEngine.UI.Image iconImage;
    public GameObject descriptionPanel;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public void Awake()
    {
        descriptionPanel.SetActive(false);
    }

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }
    
    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetData(PassivesDefinition passive)
    {
        SetIcon(passive.icon);
        SetDescription(passive.GetDescription(null));
        SetName(passive.displayName);

    }

    public void SetData(PassivesDefinition passive, BattleCharacter character)
    {
        SetIcon(passive.icon);
        SetDescription(passive.GetDescription(character));
        SetName(passive.displayName);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetActive(true);
        Debug.Log("Description: " + descriptionText.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
        Debug.Log("Hide description");
    }

}
