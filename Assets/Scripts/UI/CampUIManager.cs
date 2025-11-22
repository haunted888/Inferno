// CampUIManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampUIManager : MonoBehaviour
{
    [Header("Data")]
    public MapPartyMemberDefinition[] campMembers;   // full roster shown in camp
    public PartyDefinition partyDefinition;  // assign in inspector


    [Header("UI")]
    public Transform partySlotsParent;           // 4 children with PartySlotUI
    public Transform campGridParent;            // grid container for camp entries
    public GameObject campEntryPrefab;          // prefab with CampCharacterUI + Text
    public Button confirmButton;
    public Button campButton;   // assign your Camp button here


    MapPartyMemberDefinition[] partyDefs = new MapPartyMemberDefinition[4];
    PartySlotUI[] partySlots;
    

    private List<MapPartyMemberDefinition> runtimeCampList = new List<MapPartyMemberDefinition>();


    bool initialized = false;

    void Awake()
    {
        InitOnce();
        PullCampMembersFromTransfer();
        gameObject.SetActive(false);            // camp screen hidden at start
    }

    void InitOnce()
    {
        if (initialized) return;
        initialized = true;

        int count = partySlotsParent.childCount;
        partySlots = new PartySlotUI[count];
        for (int i = 0; i < count; i++)
        {
            var slot = partySlotsParent.GetChild(i).GetComponent<PartySlotUI>();
            slot.Init(this, i);
            partySlots[i] = slot;
        }

        confirmButton.onClick.AddListener(OnConfirm);
    }

    void BuildCampGrid()
    {
        // Set cell size to 1/6 of grid width, square
        var grid = campGridParent.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            var rt = (RectTransform)campGridParent;
            float width = rt.rect.width;
            float cell = width / 6f;
            grid.cellSize = new Vector2(cell, cell);
        }

        foreach (Transform child in campGridParent)
            Destroy(child.gameObject);

        foreach (var def in runtimeCampList)
        {
            var go = Instantiate(campEntryPrefab, campGridParent);
            var ui = go.GetComponent<CampCharacterUI>();
            ui.Init(this, def);
        }
    }



    // Called by the camp button in the map
    public void Open()
    {
        InitOnce();

        // get current party from MapCombatTransfer
        var currentParty = MapCombatTransfer.Instance.GetParty(); // List<MapPartyMemberDefinition>

        // 1) Build a working list from existing campMembers
        var campList = new List<MapPartyMemberDefinition>(campMembers);

        // 2) Ensure all party members are permanently in the camp list
        foreach (var p in currentParty)
        {
            if (p == null) continue;
            if (!campList.Contains(p))
            {
                // insert new ones at the top so they appear first
                campList.Insert(0, p);
            }
        }

        // 3) Write back to campMembers so they are now permanent camp entries
        campMembers = campList.ToArray();
        SyncCampMembersToTransfer();

        // 4) Build runtimeCampList from updated campMembers
        runtimeCampList.Clear();
        runtimeCampList.AddRange(campMembers);

        // 5) Sync party slots from current party
        for (int i = 0; i < partyDefs.Length; i++)
            partyDefs[i] = i < currentParty.Count ? currentParty[i] : null;

        BuildCampGrid();
        RefreshPartySlots();
        RefreshConfirmInteractable();
        
        campButton.interactable = false;
        gameObject.SetActive(true);
    }



    void Close()
    {
        gameObject.SetActive(false);
        campButton.interactable = true;

    }

    // click in camp grid
    public void OnCampCharacterClicked(MapPartyMemberDefinition def)
    {
        if (def == null || def.health <= 0) return;

        int existing = FindSlotIndex(def);

        if (existing >= 0)
        {
            // remove from party
            partyDefs[existing] = null;
        }
        else
        {
            // add to leftmost empty slot
            int empty = System.Array.FindIndex(partyDefs, d => d == null);
            if (empty >= 0)
                partyDefs[empty] = def;
        }

        RefreshPartySlots();
        RefreshConfirmInteractable();
    }

    // drag onto party slot
    public void AssignToSlot(int slotIndex, MapPartyMemberDefinition def)
    {
        if (slotIndex < 0 || slotIndex >= partyDefs.Length) return;
        if (def == null || def.health <= 0) return;

        int existing = FindSlotIndex(def);
        if (existing >= 0 && existing != slotIndex)
            partyDefs[existing] = null;

        partyDefs[slotIndex] = def;

        RefreshPartySlots();
        RefreshConfirmInteractable();
    }

    // click party slot to clear
    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= partyDefs.Length) return;
        partyDefs[slotIndex] = null;

        RefreshPartySlots();
        RefreshConfirmInteractable();
    }

    int FindSlotIndex(MapPartyMemberDefinition def)
    {
        for (int i = 0; i < partyDefs.Length; i++)
            if (partyDefs[i] == def) return i;
        return -1;
    }

    void RefreshPartySlots()
    {
        for (int i = 0; i < partySlots.Length; i++)
        {
            var def = i < partyDefs.Length ? partyDefs[i] : null;
            partySlots[i].SetCharacter(def);
        }
    }

    void RefreshConfirmInteractable()
    {
        bool hasAny = partyDefs.Any(d => d != null);
        confirmButton.interactable = hasAny;   // cannot confirm if party empty
    }

    void OnConfirm()
    {
        var finalList = partyDefs
            .Where(d => d != null)
            .ToArray();

        if (finalList.Length == 0)
            return; // cannot confirm an empty party

        // Replace the PartyDefinition.members with the new selected party
        partyDefinition.members = finalList;

        // Also update MapCombatTransfer to stay in sync for battle loading
        var transfer = MapCombatTransfer.Instance;
        if (transfer != null)
        {
            transfer.SetupParty(finalList);
            SyncCampMembersToTransfer();
        }

        Close();
    }

    void PullCampMembersFromTransfer()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        if (transfer.camp != null && transfer.camp.Count > 0)
        {
            campMembers = transfer.camp.ToArray();
        }
        else
        {
            transfer.SyncCampMembers(campMembers);
        }
    }

    void SyncCampMembersToTransfer()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        transfer.SyncCampMembers(campMembers);
    }
    

}
