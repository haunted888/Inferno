using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleItemSelectionUI : MonoBehaviour
{
    public GameObject buttonPrefab;     // same style as skill button
    public Transform  buttonParent;     // vertical layout

    private Action<int> onItemChosen;   // returns inventory index within filtered list
    private List<int>   filteredIndices = new List<int>(); // map visible rows -> inventory indices

    public void ShowForCharacter(BattleCharacter character, Action<int> callback)
    {
        gameObject.SetActive(true);
        onItemChosen = callback;

        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        filteredIndices.Clear();

        var inv = MapCombatTransfer.Instance?.GetInventory();
        if (inv == null) return;

        for (int i = 0; i < inv.Count; i++)
        {
            var stack = inv[i];
            if (stack == null || stack.item == null) continue;
            if (stack.quantity <= 0) continue;

            // Only items with a battle consumable show up
            if (stack.item.battleConsumable == null) continue;

            var row  = Instantiate(buttonPrefab, buttonParent);
            var btn  = row.GetComponent<Button>();
            var text = row.GetComponentInChildren<TMP_Text>();

            if (text != null)
                text.text = stack.item.displayName + $" x{stack.quantity}";

            int visibleIndex = filteredIndices.Count;
            filteredIndices.Add(i);

            btn.onClick.AddListener(() => OnButtonClicked(visibleIndex));
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onItemChosen = null;
    }

    private void OnButtonClicked(int visibleIndex)
    {
        if (visibleIndex < 0 || visibleIndex >= filteredIndices.Count)
            return;

        int inventoryIndex = filteredIndices[visibleIndex];
        onItemChosen?.Invoke(inventoryIndex);
    }
}
