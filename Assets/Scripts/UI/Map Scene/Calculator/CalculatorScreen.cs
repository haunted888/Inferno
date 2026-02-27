using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CalculatorScreen : MonoBehaviour
{
    [Header("Party UI")]
    public Transform partyListContainer;
    public CalculatorPartyEntryUI partyEntryPrefab;

    [Header("Enemy UI")]
    public Transform enemyListContainer;
    public CalculatorEnemyEntryUI enemyEntryPrefab;


    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if(InputSystem.actions.FindAction("exit").WasPressedThisFrame())        {
            gameObject.SetActive(false);
        }
    }

    public void Open(List<MapPartyMemberDefinition> party, MapEnemyDefinition[] enemies)
    {
        Debug.Log("Opening Calculator");
        gameObject.SetActive(true);

        PopulateParty(party);
        PopulateEnemies(enemies);
    }

    private void PopulateParty(List<MapPartyMemberDefinition> party)
    {
        if (partyListContainer == null || partyEntryPrefab == null) return;

        for (int i = partyListContainer.childCount - 1; i >= 0; i--)
            Destroy(partyListContainer.GetChild(i).gameObject);

        if (party == null) return;

        foreach (var def in party)
        {
            if (def == null) continue;

            var row = Instantiate(partyEntryPrefab, partyListContainer);
            row.Setup(def);
        }
    }

    private void PopulateEnemies(MapEnemyDefinition[] enemies)
    {
        if (enemyListContainer == null || enemyEntryPrefab == null) return;

        for (int i = enemyListContainer.childCount - 1; i >= 0; i--)
            Destroy(enemyListContainer.GetChild(i).gameObject);

        if (enemies == null) return;

        foreach (var enemyDef in enemies)
        {
            if (enemyDef == null) continue;

            var row = Instantiate(enemyEntryPrefab, enemyListContainer);
            row.Setup(enemyDef);
        }
    }
}
