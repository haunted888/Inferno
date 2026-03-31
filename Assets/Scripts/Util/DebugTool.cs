using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugUnlockHotkey : MonoBehaviour
{
    [Header("Hotkey")]
    public Key requiredKey1 = Key.LeftCtrl;
    public Key requiredKey2 = Key.LeftShift;
    public Key requiredKey3 = Key.U;

    [Header("Rewards")]
    public List<MapPartyMemberDefinition> charactersToAdd = new List<MapPartyMemberDefinition>();
    public List<ItemGrantEntry> itemsToAdd = new List<ItemGrantEntry>();

    private bool hasTriggered = false;

    [System.Serializable]
    public class ItemGrantEntry
    {
        public ItemDefinition item;
        public int quantity = 1;
    }

    void Update()
    {
        if (hasTriggered) return;
        if (Keyboard.current == null) return;

        bool key1Held = Keyboard.current[requiredKey1].isPressed;
        bool key2Held = Keyboard.current[requiredKey2].isPressed;
        bool key3PressedThisFrame = Keyboard.current[requiredKey3].wasPressedThisFrame;

        if (!key1Held || !key2Held || !key3PressedThisFrame)
            return;

        GrantRewards();
        hasTriggered = true;
    }

    private void GrantRewards()
    {
        var transfer = MapCombatTransfer.Instance;
        if (transfer == null) return;

        for (int i = 0; i < charactersToAdd.Count; i++)
        {
            var def = charactersToAdd[i];
            if (def == null) continue;

            transfer.AddPartyMember(def, addToPartyIfSpace: true);
        }

        for (int i = 0; i < itemsToAdd.Count; i++)
        {
            var entry = itemsToAdd[i];
            if (entry == null || entry.item == null) continue;

            int qty = Mathf.Max(1, entry.quantity);
            transfer.AddItem(entry.item, qty);
        }
    }
}