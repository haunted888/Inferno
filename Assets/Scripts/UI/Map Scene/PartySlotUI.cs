// PartySlotUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartySlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public TMP_Text nameText;

    CampUIManager manager;
    int slotIndex;
    MapPartyMemberDefinition currentDef;

    public void Init(CampUIManager mgr, int index)
    {
        manager = mgr;
        slotIndex = index;
        SetCharacter(null);
    }

    public void SetCharacter(MapPartyMemberDefinition def)
    {
        currentDef = def;
        if (nameText != null)
            nameText.text = def != null && def.displayName != null
                ? def.displayName
                : "";
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (CampCharacterUI.currentDragged == null) return;
        var def = CampCharacterUI.currentDragged.definition;
        manager.AssignToSlot(slotIndex, def);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentDef != null)
            manager.ClearSlot(slotIndex);
    }
}
