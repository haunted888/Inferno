using System.Collections.Generic;
using UnityEngine;

public class PartyStatusPanel : MonoBehaviour
{
    public Transform entriesParent;   // where rows will be spawned
    public GameObject entryPrefab;    // prefab with PartyStatusEntry

    void Start()
    {
        var btm = BattleTurnManager.Instance;
        if (btm == null || entryPrefab == null || entriesParent == null) return;

        IReadOnlyList<BattleCharacter> party = btm.PlayerParty;
        if (party == null) return;

        foreach (var chr in party)
        {
            if (chr == null) continue;

            GameObject obj = Instantiate(entryPrefab, entriesParent);
            var entry = obj.GetComponent<PartyStatusEntry>();
            if (entry != null)
                entry.Initialize(chr);
        }
    }
}
